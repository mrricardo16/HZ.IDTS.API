using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// 获取载具上的物料详情信息视图
    /// </summary>
    public class NewGoodsInfoViewModel
    {
        public List<ItemRowViewModel> itemRow = new List<ItemRowViewModel>();

        public RackInfoViewModel rackInfo = new RackInfoViewModel();
    }
}
