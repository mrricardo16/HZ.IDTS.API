using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Helpers
{
   public class IdWorkerSingleton
    {
        private static readonly IdWorker instance = new IdWorker(0, 0);

        public static IdWorker Instance
        {
            get { return instance; }
        }

        public static string NewSnowId()
        {
            return instance.NextId().ToString();
        }
    }
}
