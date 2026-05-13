/// <summary>
/// 功能描述    ：RedisUpdateDetailLogHostedService  
/// 创 建 者    ：Administrator
/// 创建日期    ：2021/2/23 16:35:16 
/// 最后修改者  ：Administrator
/// 最后修改日期：2021/2/23 16:35:16 
/// </summary>
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Interfaces.Service
{
    /// <summary>
    /// 同步服务
    /// </summary>
    public class SyncTaskService : IHostedService
    {
        //ITransferService _ITransferService;

        public SyncTaskService()
        {
            //_ITransferService = ServiceLocator.GetService<ITransferService>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //同步发货区的物料到线边库
            Thread syncDeliveryToSide = new Thread(SyncDeliveryAreaToSide);
            syncDeliveryToSide.IsBackground = true;
            syncDeliveryToSide.Start();
    
            return Task.CompletedTask;
        }

        private void SyncDeliveryAreaToSide()
        {
            while (true)
            {
                //同步方法
                Thread.Sleep(2000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
