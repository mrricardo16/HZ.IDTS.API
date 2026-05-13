using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class PlanViewModel
    {
        public PlanCompleModel planComple = new PlanCompleModel();
    }

    public class PlanCompleModel
    {
        /// <summary>
        /// 计划总数
        /// </summary>
        public string planNum { get; set; }

        /// <summary>
        /// 计划总完成数
        /// </summary>
        public string planFinishedNum { get; set; }

        /// <summary>
        /// 当日计划数
        /// </summary>
        public string currDaysPlanNum { get; set; }

        /// <summary>
        /// 当日总完成数
        /// </summary>
        public string currDaysPlanFinishedNum { get; set; }
    }
}
