
using HZ.IDTSCore.Common;
using Microsoft.AspNetCore.Builder;
using NLog;
using System;
using System.Diagnostics;

namespace HZ.IDTSCore.Api.Middleware
{
    public static class PerformanceLogExtension
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        public static IApplicationBuilder UsePerformanceLog(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Use(async (context, next) =>
            {
                var profiler = new Stopwatch();
                profiler.Start();
                await next();
                profiler.Stop();

                //var logger = context.RequestServices.GetService<ILoggerFactory>()
                //    .CreateLogger("PerformanceLog");
                //logger.LogInformation("TraceId:{TraceId}, RequestMethod:{RequestMethod}, RequestPath:{RequestPath}, ElapsedMilliseconds:{ElapsedMilliseconds}, Response StatusCode: {StatusCode}",
                //                        context.TraceIdentifier, context.Request.Method, context.Request.Path, profiler.ElapsedMilliseconds, context.Response.StatusCode);

                //LogHelper.Info($"TraceId:{ context.TraceIdentifier}, RequestMethod: { context.Request.Method}, RequestPath: { context.Request.Path}," +
                //    $" ElapsedMilliseconds: { profiler.ElapsedMilliseconds}, Response StatusCode: { context.Response.StatusCode}");
                //if(context.Request.Method=="GET")
                //    LogHelper.Info(context.Request.Path+ context.Request.QueryString);
            });
            return applicationBuilder;
        }

        //class StopwatchProfiler : IProfiler
        //{
        //    private readonly Stopwatch _stopwatch;

        //    public StopwatchProfiler()
        //    {
        //        _stopwatch = new Stopwatch();
        //    }

        //    public void Start()
        //    {
        //        _stopwatch.Start();
        //    }

        //    public void Stop()
        //    {
        //        _stopwatch.Stop();
        //    }

        //    public void Reset()
        //    {
        //        _stopwatch.Reset();
        //    }

        //    public void Restart()
        //    {
        //        _stopwatch.Restart();
        //    }

        //    public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
        //}
    }
}
