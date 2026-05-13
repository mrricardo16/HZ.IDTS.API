/// <summary>
/// 功能描述    ：RedisUpdateDetailLogHostedService  
/// 创 建 者    ：Administrator
/// 创建日期    ：2021/2/23 16:35:16 
/// 最后修改者  ：Administrator
/// 最后修改日期：2021/2/23 16:35:16 
/// </summary>
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    //public class RedisUpdateDetailLogHostedService : IHostedService
    //{
    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        Thread t1 = new Thread(Start);
    //        t1.IsBackground = true;
    //        t1.Start();
    //        return Task.CompletedTask;
    //    }

    //    public void Start()
    //    {
    //        while (true)
    //        {
    //            RedisUpdateDetailLog.GetLog();
    //            Thread.Sleep(2000);
    //        }
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        //throw new NotImplementedException();
    //        return Task.CompletedTask;
    //    }

    //}
}
