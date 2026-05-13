using System;
using System.Collections.Generic;
using System.Text;
using static HZ.IDTSCore.Model.Entity.OpenApi.ItemRowRack;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class ItemRackInfo
    {
        /// <summary>
        /// 针对货位、托盘、料箱等返回的物料信息
        /// </summary>
        public List<ItemRowPlus> itemRow { get; set; }
       
        /// <summary>
        /// 针对料架的
        /// </summary>
        public RackInfoViewModel rackInfo { get; set; }
    }
}
