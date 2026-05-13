using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Basic;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Common
{
    public interface IBcService : IBaseService<tn_wms_bc>
    {
        public List<tn_wms_bc> GetTreeSon(List<tn_wms_bc> bc);
        public List<tn_wms_bc> GetAllChild(List<tn_wms_bc> allbc,List<tn_wms_bc> nextBc);
        public List<tn_wms_bc> GetAllChild(List<string> bc);
        public List<tn_wms_bc> GetOwnAndAllChild(List<string> bc);
        public ApiResult BatchUpdateOutQty(List<tn_wms_bc> bcs,SqlSugarClient dbTran);
    }
}
