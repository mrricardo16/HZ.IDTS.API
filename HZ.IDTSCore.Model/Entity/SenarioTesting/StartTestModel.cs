using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
   
    public class StartTestModel
    {
        /// <summary>
        /// 用户选择的场景编码
        /// </summary>
        public string Sceneno { get; set; }
        /// <summary>
        /// 是否启用货位同步
        /// </summary>
        public bool IsSynchronizeStock { get; set; }
        /// <summary>
        /// 同步货位的货位设备编码（不启用货位同步时，传空字符串）
        /// </summary>
        public string Goodsequipmentno { get; set; }
        /// <summary>
        /// 用户选择的客户端IP
        /// </summary>
        public string ClientIP { get; set; }
    }
}
