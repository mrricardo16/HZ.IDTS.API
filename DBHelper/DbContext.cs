
using System.Diagnostics;
using System.Linq;
using SqlSugar;
using System;
using HZ.CommonUtil.Model;
using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.ExceptionExtend;
using System.Collections.Generic;
using System.Collections.Concurrent;
using HZ.CommonUtil;

namespace HZ.DbHelper
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class DbContext : IDisposable
    {
        public SqlSugarClient Db;   // 用来处理事务、多表查询和复杂操作

        private SessionInfo CurrSession { get; set; }

        private bool _disposed;
        private const int DbConnectionConfigCacheSeconds = 30;

        private static readonly object DbConnectionConfigCacheLock = new object();

        private static readonly ConcurrentDictionary<string, DbConnectionConfigCacheItemV2> DbConnectionConfigCacheV2 = new ConcurrentDictionary<string, DbConnectionConfigCacheItemV2>();

        /// <summary>
        /// 数据库连接配置缓存项V2。
        /// 只缓存配置结果，不缓存SqlSugarClient实例，避免多线程共享同一个Db连接上下文。
        /// </summary>
        private class DbConnectionConfigCacheItemV2
        {
            public string DataType { get; set; }

            public string ConnectionString { get; set; }

            public bool EnableSqlLog { get; set; }

            public DateTime ExpireTime { get; set; }
        }

        public SessionInfo GetCurrSession()
        {
            return CurrSession;
        }

        private static void SafeRollback(SqlSugarClient db)
        {
            try
            {
                db?.RollbackTran();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void SafeClose(SqlSugarClient db)
        {
            try
            {
                db?.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static string ApplyConnectionPoolDefaults(string dataType, string connectionString)
        {
            if (!string.Equals(dataType, "PGSQL", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            int maxPoolSize = AppSettings.GetValue<int>("DbConnection:MaxPoolSize");
            int minPoolSize = AppSettings.GetValue<int>("DbConnection:MinPoolSize");
            int idleLifetime = AppSettings.GetValue<int>("DbConnection:ConnectionIdleLifetime");
            int pruningInterval = AppSettings.GetValue<int>("DbConnection:ConnectionPruningInterval");
            int timeout = AppSettings.GetValue<int>("DbConnection:Timeout");
            int commandTimeout = AppSettings.GetValue<int>("DbConnection:CommandTimeout");

            if (maxPoolSize <= 0) maxPoolSize = 80;
            if (minPoolSize < 0) minPoolSize = 0;
            if (idleLifetime <= 0) idleLifetime = 30;
            if (pruningInterval <= 0) pruningInterval = 10;
            if (timeout <= 0) timeout = 15;
            if (commandTimeout <= 0) commandTimeout = 30;

            connectionString = AppendConnectionOption(connectionString, "Pooling", "true");
            connectionString = AppendConnectionOption(connectionString, "Maximum Pool Size", maxPoolSize.ToString(), "MaxPoolSize");
            connectionString = AppendConnectionOption(connectionString, "Minimum Pool Size", minPoolSize.ToString(), "MinPoolSize");
            connectionString = AppendConnectionOption(connectionString, "Connection Idle Lifetime", idleLifetime.ToString());
            connectionString = AppendConnectionOption(connectionString, "Connection Pruning Interval", pruningInterval.ToString());
            connectionString = AppendConnectionOption(connectionString, "Timeout", timeout.ToString());
            connectionString = AppendConnectionOption(connectionString, "Command Timeout", commandTimeout.ToString(), "CommandTimeout");

            return connectionString;
        }

        private static string AppendConnectionOption(string connectionString, string key, string value, params string[] aliases)
        {
            if (HasConnectionOption(connectionString, key, aliases))
            {
                return connectionString;
            }

            if (!connectionString.EndsWith(";"))
            {
                connectionString += ";";
            }

            return connectionString + key + "=" + value + ";";
        }

        private static bool HasConnectionOption(string connectionString, string key, params string[] aliases)
        {
            var keys = new List<string>() { key };
            if (aliases != null && aliases.Length > 0)
            {
                keys.AddRange(aliases);
            }

            return connectionString
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Split(new[] { '=' }, 2)[0].Trim())
                .Any(optionKey => keys.Any(k => string.Equals(optionKey, k, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 获取数据库连接配置V2。
        /// 时间：2026-05-29
        /// 优化内容：缓存DataType、最终连接字符串、SQL日志开关，减少高并发下反复读取AppSettings和重复拼接连接池参数。
        /// 缓存策略：按splitDbCode缓存30秒，兼顾性能和reloadOnChange配置变更生效。
        /// </summary>
        /// <param name="splitDbCode">分库标识</param>
        /// <returns>数据库连接配置缓存项</returns>
        private static DbConnectionConfigCacheItemV2 GetDbConnectionConfigV2(string splitDbCode)
        {
            string cacheKeyV2 = string.IsNullOrEmpty(splitDbCode) ? string.Empty : splitDbCode;
            DateTime nowV2 = DateTime.Now;
            DbConnectionConfigCacheItemV2 cacheItemV2 = null;

            // 高并发下绝大多数请求都是缓存命中，先无锁读取，避免所有请求都排队进入lock。
            if (DbConnectionConfigCacheV2.TryGetValue(cacheKeyV2, out cacheItemV2) && cacheItemV2.ExpireTime > nowV2)
            {
                return cacheItemV2;
            }

            lock (DbConnectionConfigCacheLock)
            {
                nowV2 = DateTime.Now;
                if (DbConnectionConfigCacheV2.TryGetValue(cacheKeyV2, out cacheItemV2) && cacheItemV2.ExpireTime > nowV2)
                {
                    return cacheItemV2;
                }

                string dataTypeV2 = AppSettings.GetValue<string>("DbConnection:DataType");
                string connectionStringKeyV2 = string.IsNullOrEmpty(splitDbCode) ? "DbConnection:ConnectionString_" : "DbConnection:ConnectionString_" + splitDbCode;
                string connectionStringV2 = AppSettings.GetValue<string>(connectionStringKeyV2);

                cacheItemV2 = new DbConnectionConfigCacheItemV2()
                {
                    DataType = dataTypeV2,
                    ConnectionString = ApplyConnectionPoolDefaults(dataTypeV2, connectionStringV2),
                    EnableSqlLog = AppSettings.GetValue<bool>("AppSettings:EnableSqlLog"),
                    ExpireTime = nowV2.AddSeconds(DbConnectionConfigCacheSeconds)
                };
                DbConnectionConfigCacheV2[cacheKeyV2] = cacheItemV2;
                return cacheItemV2;
            }
        }

        /// <summary>
        /// 根据缓存配置创建SqlSugarClientV2。
        /// </summary>
        /// <param name="dbConfigV2">数据库连接配置缓存项</param>
        /// <returns>SqlSugarClient实例</returns>
        private static SqlSugarClient CreateSqlSugarClientV2(DbConnectionConfigCacheItemV2 dbConfigV2)
        {
            var newDb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = dbConfigV2.ConnectionString,
                DbType = GetDbTypeV2(dbConfigV2.DataType),
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = SqlFuncExt.GetSqlFuncExternal()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoRemoveDataCache = true
                },
            });

            BindSqlLogV2(newDb, dbConfigV2.EnableSqlLog);
            return newDb;
        }

        /// <summary>
        /// 获取SqlSugar数据库类型V2。
        /// </summary>
        /// <param name="dataType">配置文件数据库类型</param>
        /// <returns>SqlSugar数据库类型</returns>
        private static DbType GetDbTypeV2(string dataType)
        {
            return dataType == "MSSQL" ? DbType.SqlServer : (dataType == "MYSQL" ? DbType.MySql : (dataType == "PGSQL" ? DbType.PostgreSQL : DbType.Oracle));
        }

        /// <summary>
        /// 绑定SQL日志V2。
        /// </summary>
        /// <param name="db">SqlSugarClient实例</param>
        /// <param name="enableSqlLog">是否启用SQL日志</param>
        private static void BindSqlLogV2(SqlSugarClient db, bool enableSqlLog)
        {
            if (enableSqlLog)
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine(sql);
                    Debug.WriteLine(sql);
                };
            }
        }
        public SqlSugarClient NewDbInstance()
        {
            DbConnectionConfigCacheItemV2 dbConfigV2 = GetDbConnectionConfigV2(CurrSession?.splitDbCode);
            return CreateSqlSugarClientV2(dbConfigV2);
        }
        public DbContext(SessionInfo session)
        {
            //if (string.IsNullOrEmpty(session.splitDbCode))
            //    throw new Exception("未找到分库标识！");
            CurrSession = session;
            //this.token = token;
            //this.splitDbCode = org;
            DbConnectionConfigCacheItemV2 dbConfigV2 = GetDbConnectionConfigV2(session?.splitDbCode);
            Db = CreateSqlSugarClientV2(dbConfigV2);
        }

        public DbSet<T> DbTable<T>() where T : class, new()
        {
            return new DbSet<T>(Db);
        }


        public ApiResult UseTransaction(Action<SqlSugarClient> action)
        {
            try
            {
                //db.BeginTran(); // 开启事务
                // 特别说明：在事务中，默认情况下是使用锁的，也就是说在当前事务没有结束前，其他任何查询都需要等待。
                // ReadCommitted：读取数据时保持共享锁，避免脏读；但事务结束前数据仍可被更改，可能导致不可重复读或幻读。
                //Db.BeginTran(System.Data.IsolationLevel.ReadCommitted); // 重载指定事务级别
                Db.BeginTran();
                //Db.Ado.BeginTran(System.Data.IsolationLevel.ReadCommitted);
                // 特别说明：在事务操作中，对于自增列的表，插入成功后又回滚，仍会占用一次自增值。
                action(this.Db);
                //db.BeginTran(System.Data.IsolationLevel.ReadCommitted); // 重载指定事务级别
                //Db.Ado.CommitTran();
                Db.CommitTran();
                //throw new Exception("事务执行异常");
                //Console.WriteLine(id2);
                //// 提交事务
                //db.CommitTran();
                return ApiResult.Success();
            }
            catch (ErrCodeException ex)
            {
                SafeRollback(Db); // 回滚
                return ApiResult.Error(ex.Message,ex.ErrCode);
            }
            catch (Exception ex)
            {
                SafeRollback(Db); // 回滚
                return ApiResult.Error(ex.Message);
            }
            finally
            {
                SafeClose(Db);
            }
        }

        /// <summary>
        /// 安全事务V2。
        /// 时间：2026-05-29
        /// 优化内容：使用独立SqlSugarClient执行事务，不复用当前DbContext.Db，避免事务结束关闭连接后调用方继续使用当前Db实例产生副作用。
        /// 适用场景：高并发接口、批量写入、批量更新等需要明确释放事务连接的业务。
        /// </summary>
        /// <param name="action">事务内执行的数据库操作</param>
        /// <returns>执行结果</returns>
        public ApiResult UseTransactionV2(Action<SqlSugarClient> action)
        {
            SqlSugarClient dbV2 = null;
            try
            {
                dbV2 = NewDbInstance();
                dbV2.BeginTran();
                action(dbV2);
                dbV2.CommitTran();
                return ApiResult.Success();
            }
            catch (ErrCodeException ex)
            {
                SafeRollback(dbV2);
                return ApiResult.Error(ex.Message, ex.ErrCode);
            }
            catch (Exception ex)
            {
                SafeRollback(dbV2);
                return ApiResult.Error(ex.Message);
            }
            finally
            {
                SafeClose(dbV2);
            }
        }
        public ApiResult UseTransaction(Action<SqlSugarClient> action,SqlSugarClient dbInstance)
        {
            // 事务处理
            //using (SqlSugarClient db = SugarContext.GetInstance()) // 开启数据连接
            //{
            //    db.CommandTimeOut = 30000; // 设置超时时间
            try
            {
                //db.BeginTran(); // 开启事务
                // 特别说明：在事务中，默认情况下是使用锁的，也就是说在当前事务没有结束前，其他任何查询都需要等待。
                // ReadCommitted：读取数据时保持共享锁，避免脏读；但事务结束前数据仍可被更改，可能导致不可重复读或幻读。
                //Db.BeginTran(System.Data.IsolationLevel.ReadCommitted); // 重载指定事务级别
                dbInstance.BeginTran();
                //Db.Ado.BeginTran(System.Data.IsolationLevel.ReadCommitted);
                // 特别说明：在事务操作中，对于自增列的表，插入成功后又回滚，仍会占用一次自增值。
                action(dbInstance);
                //db.BeginTran(System.Data.IsolationLevel.ReadCommitted); // 重载指定事务级别
                //Db.Ado.CommitTran();
                dbInstance.CommitTran();
                //throw new Exception("事务执行异常");
                //Console.WriteLine(id2);
                //// 提交事务
                //db.CommitTran();
                return ApiResult.Success();
            }
            catch (ErrCodeException ex)
            {
                SafeRollback(dbInstance); // 回滚
                return ApiResult.Error(ex.Message, ex.ErrCode);
            }
            catch (Exception ex)
            {
                SafeRollback(dbInstance); // 回滚
                return ApiResult.Error(ex.Message);
                //throw ex;
            }
            finally
            {
                SafeClose(dbInstance);
            }
        }

        public delegate void TransactionCommittedEventHandler();
        public ApiResult UseTransactionCallback(Action<SqlSugarClient,string> action, TransactionCommittedEventHandler transactionCommitted = null)
        {
            string tranend = "true";
            bool committed = false;
            try
            {
                Db.BeginTran();
                tranend = "false";
                action(this.Db, tranend);
                Db.CommitTran();
                committed = true;
                tranend = "true"; 
                
                transactionCommitted?.Invoke(); // 触发事件或回调函数
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                if (!committed)
                {
                    SafeRollback(Db); // 回滚
                }
                tranend = "true";
                return ApiResult.Error(ex.Message);
            }
            finally
            {
                SafeClose(Db);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                SafeClose(Db);
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// 扩展ORM
    /// </summary>
    public class DbSet<T> : SimpleClient<T> where T : class, new()
    {
        public DbSet(SqlSugarClient context) : base(context)
        {

        }
    }

    public class SessionInfo
    {
        public string token { get; set; }
        public string splitDbCode { get; set; }
    }
}
