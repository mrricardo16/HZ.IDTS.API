using HZ.iWCS.Common.Core;
using System;
using System.IO;
using Topshelf;

namespace HZ.IDTS.SimulateService
{
    class Program
    {
        static void Main(string[] args)
        {
            var processModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }
            //日志初始化
            LogHelper.Configure();
            LogHelper.Info("Start.");

            bool isService = Convert.ToBoolean(AppConfigurtaionServices.Configuration["AppSettings:Debugger"]);
            LogHelper.Info("isService=" + isService);
            if (isService)
            {
                Bootstrap _bootstrap = new Bootstrap();
                _bootstrap.Initialize();
                _bootstrap.Start();
                Console.ReadKey();
            }
            else
            {
                var ServiceDescription = "HZ.IDTS.SimulateService";
                var ServiceDisplayName = "HZ.IDTS.SimulateService";
                var ServiceName = "HZ.IDTS.SimulateService";
                HostFactory.Run(x =>
                {
                    x.Service<Bootstrap>(s =>
                    {
                        s.ConstructUsing(b => new Bootstrap());
                        s.WhenStarted(b => b.Start());
                        s.WhenStopped(b => b.Shutdown());
                    });

                    x.RunAsLocalService();
                    x.StartAutomatically();
                    x.SetDescription(ServiceDescription);
                    x.SetDisplayName(ServiceDisplayName);
                    x.SetServiceName(ServiceName);
                });
            }
        }
    }
}
