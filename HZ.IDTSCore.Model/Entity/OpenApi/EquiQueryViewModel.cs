using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class EquiQueryViewModel
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string equiCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string equiName { get; set; }
    }

    public class EquiQueryResult
    {
        public List<EquiQueryViewModel> equiQueryList = new List<EquiQueryViewModel>();
    }
}
