using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Common;
using HZ.IDTSCore.Model.Entity.Basic;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Common
{
    public class BcService : BaseService<tn_wms_bc>, IBcService
    {
        public BcService(SessionInfo session) : base(session) { }

        //public List<tn_wms_bc> GetChildBc(List<tn_wms_bc> bcs)
        //{
        //    //Db.Queryable<tn_wms_bc>().Where(bcs.Sele)
        //}

        private List<tn_wms_bc> GetSon(List<tn_wms_bc> bc)
        {
            var list = Db.Queryable<tn_wms_bc>().Where(x => bc.Select(y => y.cn_s_bc).Contains(x.cn_s_pbc)).ToList();

            bc.ForEach(x =>
            {
                x.children = list.Where(x => x.cn_s_pbc == x.cn_s_bc).ToList();
            });
            return bc;
        }

        public List<tn_wms_bc> GetTreeSon(List<tn_wms_bc> bc)
        {
            var list = Db.Queryable<tn_wms_bc>().Where(x => bc.Select(y => y.cn_s_bc).Contains(x.cn_s_pbc)).ToList();

            int count = 0; ;
            foreach (var m in bc)
            {
                m.children = list.Where(x => x.cn_s_pbc == m.cn_s_bc).ToList();

                count += m.children.Count;

                if (m.children.Any())
                    return GetTreeSon(m.children);
            }
            if (count == 0)
                return bc;
            return bc;
        }

        public List<tn_wms_bc> GetAllChild(List<tn_wms_bc> allbc, List<tn_wms_bc> child)
        {
            var list = Db.Queryable<tn_wms_bc>().Where(x => child.Select(y => y.cn_s_bc).Contains(x.cn_s_pbc)).ToList();

            if (list.Any())
                allbc.AddRange(list);

            int count = 0; ;
            foreach (var m in child)
            {
                m.children = list.Where(x => x.cn_s_pbc == m.cn_s_bc).ToList();

                count += m.children.Count;

                if (m.children.Any())
                {
                    return GetAllChild(allbc, m.children);
                }
            }
            if (count == 0)
                return allbc;
            return allbc;
        }

        public List<tn_wms_bc> GetAllChild(List<string> bc)
        {
            var list = Db.Queryable<tn_wms_bc>().Where(x => bc.Contains(x.cn_s_bc)).ToList();
            List<tn_wms_bc> allBc = new List<tn_wms_bc>();
            GetAllChild(allBc, list);
            return allBc;
        }

        public List<tn_wms_bc> GetOwnAndAllChild(List<string> bc)
        {
            List<tn_wms_bc> allBc = new List<tn_wms_bc>();
            var list = Db.Queryable<tn_wms_bc>().Where(x => bc.Contains(x.cn_s_bc)).ToList();
            allBc.AddRange(list);
            GetAllChild(allBc, list);
            return allBc;
        }

        public ApiResult BatchUpdateOutQty(List<tn_wms_bc> bcs, SqlSugarClient dbTran)
        {

            string sql = string.Empty;
            foreach (var model in bcs)
            {
                sql += $@"update tn_wms_bc 
                        set  
                            cn_f_qty = cn_f_qty - {model.cn_f_pick_qty},
                            cn_b_outpack = {model.cn_b_outpack},
                            cn_b_unpack= {model.cn_b_unpack}
                        where 
                            cn_s_bc = '{model.cn_s_bc}'; ";
            }

            int affected = dbTran.Ado.ExecuteCommand(sql, new SugarParameter[] { });
            if (affected != bcs.Count)
                throw new Exception($"更新标签{string.Join(",", bcs.Select(e => e.cn_s_bc))}的拣货数据失败！");
            return
                ApiResult.Success();
        }
    }
}
