using HZ.IDTSCore.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class ObjectDriver : BaseController
    {
        private static readonly ObjectDriver instance = new ObjectDriver();

        private ObjectDriver()
        {

        }

        public static ObjectDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 最新复制对象唯一标识
        /// </summary>
        public string latestCopyObjectGuid { get; set; }
    }
}
