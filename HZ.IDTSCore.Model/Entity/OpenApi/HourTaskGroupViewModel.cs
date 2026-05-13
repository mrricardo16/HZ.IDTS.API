using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class HourTaskGroupViewModel
    {
        public List<HourTaskGroupModel> hourTaskInfo = new List<HourTaskGroupModel>();
    }

    public class HourTaskGroupModel
    {
        /// <summary>
        /// 小时
        /// </summary>
        public int hour { get; set; }

        /// <summary>
        /// 任务数
        /// </summary>
        public int taskNum { get; set; }
    }
}
