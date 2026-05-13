using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// 工位呼叫任务Model
    /// </summary>
    public class StationCallMonitorViewModel
    {
        public List<CallViewModel> Call = new List<CallViewModel>();
    }

    public class CallViewModel
    {
        /// <summary>
        /// 工位名称
        /// </summary>
        public string stationName { get; set; }

        /// <summary>
        /// 站点编码
        /// </summary>
        public string bitCode { get; set; }

        /// <summary>
        /// 呼叫类型(原料上料、成品下线、空托返回、呼叫空托等自定义名称)
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 呼叫时间
        /// </summary>
        public string callTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string ext1 { get; set; }

        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string ext2 { get; set; }
    }
}
