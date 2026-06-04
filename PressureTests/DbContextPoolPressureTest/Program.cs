using HZ.DbHelper;
using HZ.CommonUtil.ExceptionExtend;
using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbContextPoolPressureTest
{
    internal class Program
    {
        private const string AppName = "DbContextPoolPressureTest";
        private static readonly ConcurrentBag<SqlSugarClient> ExternalTransactionClients = new ConcurrentBag<SqlSugarClient>();

        private static async Task<int> Main(string[] args)
        {
            int total = GetArg(args, "--total", 2000);
            int concurrency = GetArg(args, "--concurrency", 100);
            int holdMs = GetArg(args, "--holdMs", 0);
            int coolDownSeconds = GetArg(args, "--coolDownSeconds", 15);
            bool transaction = HasArg(args, "--transaction");
            bool rollback = HasArg(args, "--rollback");
            bool externalErrCodeRollback = HasArg(args, "--externalErrCodeRollback");

            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
            ThreadPool.SetMinThreads(Math.Max(minWorkerThreads, concurrency + 20), minCompletionPortThreads);

            Console.WriteLine($"DbContext pool pressure test started.");
            Console.WriteLine($"total={total}, concurrency={concurrency}, holdMs={holdMs}, transaction={transaction}, rollback={rollback}, externalErrCodeRollback={externalErrCodeRollback}");

            var monitor = new DbContext(new SessionInfo { token = string.Empty, splitDbCode = string.Empty });

            try
            {
                await PrintPoolSnapshotAsync(monitor, "before");

                var errors = new ConcurrentBag<string>();
                var latencies = new ConcurrentBag<long>();
                var maxActiveConnections = 0;
                var stopMonitor = new CancellationTokenSource();

                var monitorTask = Task.Run(async () =>
                {
                    while (!stopMonitor.IsCancellationRequested)
                    {
                        try
                        {
                            int active = GetTestConnectionCount(monitor);
                            UpdateMax(ref maxActiveConnections, active);
                        }
                        catch
                        {
                            // Monitoring failures do not affect the main pressure test result.
                        }

                        await Task.Delay(100);
                    }
                });

                var stopwatch = Stopwatch.StartNew();
                using (var semaphore = new SemaphoreSlim(concurrency))
                {
                    var tasks = Enumerable.Range(0, total).Select(async index =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            var requestWatch = Stopwatch.StartNew();
                            await Task.Run(() => ExecuteOnce(index, holdMs, transaction, rollback, externalErrCodeRollback));
                            requestWatch.Stop();
                            latencies.Add(requestWatch.ElapsedMilliseconds);
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex.GetType().Name + ": " + ex.Message);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }).ToArray();

                    await Task.WhenAll(tasks);
                }

                stopwatch.Stop();
                stopMonitor.Cancel();
                await monitorTask;

                await PrintPoolSnapshotAsync(monitor, "after-finished");

                Console.WriteLine("waiting for idle connections to be released...");
                for (int i = 1; i <= coolDownSeconds; i++)
                {
                    await Task.Delay(1000);
                    await PrintPoolSnapshotAsync(monitor, $"cooldown-{i}s");
                }

                PrintSummary(total, concurrency, stopwatch.ElapsedMilliseconds, latencies, errors, maxActiveConnections);
                CloseExternalTransactionClients();

                return errors.IsEmpty ? 0 : 1;
            }
            finally
            {
                monitor.Db.Close();
            }
        }

        private static void ExecuteOnce(int index, int holdMs, bool transaction, bool rollback, bool externalErrCodeRollback)
        {
            var context = new DbContext(new SessionInfo { token = string.Empty, splitDbCode = string.Empty });
            try
            {
                if (externalErrCodeRollback)
                {
                    ExecuteExternalErrCodeRollback(context, holdMs);
                    return;
                }

                if (!transaction)
                {
                    context.Db.Ado.GetInt("select 1");
                    return;
                }

                var result = context.UseTransaction(db =>
                {
                    db.Ado.GetInt("select 1");

                    if (holdMs > 0)
                    {
                        Thread.Sleep(holdMs);
                    }

                    if (rollback)
                    {
                        throw new Exception("pressure test rollback");
                    }
                });

                if (!result.IsSuccess && !rollback)
                {
                    throw new Exception(result.Message);
                }
            }
            finally
            {
                // DbContext uses IsAutoCloseConnection=true, so normal CRUD returns connections automatically.
                // Close here only cleans up the pressure-test object itself.
                context.Db.Close();
            }
        }

        private static void ExecuteExternalErrCodeRollback(DbContext context, int holdMs)
        {
            var dbInstance = context.NewDbInstance();

            // This mode targets DbContext.UseTransaction(action, dbInstance) with ErrCodeException.
            // The current DbContext.cs calls Db.RollbackTran() here instead of dbInstance.RollbackTran().
            // Keep dbInstance open until snapshots finish, so pg_stat_activity can show any residue.
            ExternalTransactionClients.Add(dbInstance);

            context.UseTransaction(db =>
            {
                db.Ado.GetInt("select 1");

                if (holdMs > 0)
                {
                    Thread.Sleep(holdMs);
                }

                throw new ErrCodeException("pressure test errcode rollback", 999999);
            }, dbInstance);
        }

        private static void CloseExternalTransactionClients()
        {
            while (ExternalTransactionClients.TryTake(out SqlSugarClient client))
            {
                try
                {
                    client.RollbackTran();
                    client.Close();
                }
                catch
                {
                    // Cleanup failures should not hide the pressure test result.
                }
            }
        }

        private static int GetTestConnectionCount(DbContext monitor)
        {
            return monitor.Db.Ado.GetInt($@"
select count(1)
from pg_stat_activity
where datname = current_database()
  and application_name = '{AppName}'");
        }

        private static async Task PrintPoolSnapshotAsync(DbContext monitor, string stage)
        {
            await Task.Run(() =>
            {
                var rows = monitor.Db.Ado.SqlQuery<ConnectionStateRow>($@"
select state, count(1) as count
from pg_stat_activity
where datname = current_database()
  and application_name = '{AppName}'
group by state
order by state");

                string states = rows.Count == 0
                    ? "none"
                    : string.Join(", ", rows.Select(x => $"{(string.IsNullOrEmpty(x.state) ? "unknown" : x.state)}={x.count}"));

                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{stage}] connections: {states}");
            });
        }

        private static void PrintSummary(
            int total,
            int concurrency,
            long elapsedMs,
            ConcurrentBag<long> latencies,
            ConcurrentBag<string> errors,
            int maxActiveConnections)
        {
            var ordered = latencies.OrderBy(x => x).ToArray();
            long avg = ordered.Length == 0 ? 0 : Convert.ToInt64(ordered.Average());

            Console.WriteLine("========== summary ==========");
            Console.WriteLine($"total={total}, concurrency={concurrency}, elapsedMs={elapsedMs}");
            Console.WriteLine($"success={latencies.Count}, errors={errors.Count}");
            Console.WriteLine($"maxActiveConnections={maxActiveConnections}");
            Console.WriteLine($"avgMs={avg}, p50Ms={Percentile(ordered, 50)}, p90Ms={Percentile(ordered, 90)}, p99Ms={Percentile(ordered, 99)}, maxMs={(ordered.Length == 0 ? 0 : ordered.Last())}");

            if (!errors.IsEmpty)
            {
                Console.WriteLine("top errors:");
                foreach (var error in errors.GroupBy(x => x).OrderByDescending(x => x.Count()).Take(5))
                {
                    Console.WriteLine($"{error.Count()} x {error.Key}");
                }
            }
        }

        private static long Percentile(long[] values, int percentile)
        {
            if (values.Length == 0)
            {
                return 0;
            }

            int index = Convert.ToInt32(Math.Ceiling(values.Length * percentile / 100.0)) - 1;
            index = Math.Max(0, Math.Min(values.Length - 1, index));
            return values[index];
        }

        private static int GetArg(string[] args, string name, int defaultValue)
        {
            int index = Array.IndexOf(args, name);
            if (index >= 0 && index + 1 < args.Length && int.TryParse(args[index + 1], out int value))
            {
                return value;
            }

            return defaultValue;
        }

        private static bool HasArg(string[] args, string name)
        {
            return args.Any(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
        }

        private static void UpdateMax(ref int target, int value)
        {
            int initialValue;
            do
            {
                initialValue = target;
                if (value <= initialValue)
                {
                    return;
                }
            }
            while (Interlocked.CompareExchange(ref target, value, initialValue) != initialValue);
        }

        public class ConnectionStateRow
        {
            public string state { get; set; }

            public int count { get; set; }
        }
    }
}
