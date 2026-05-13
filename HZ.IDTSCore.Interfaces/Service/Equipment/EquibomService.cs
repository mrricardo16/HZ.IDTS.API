using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
namespace HZ.IDTSCore.Interfaces.Service.Equipment
{
    class EquibomService : BaseService<tn_dts_equibom>, IEquibomService
    {
        public EquibomService(SessionInfo session) : base(session)
        {

        }

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equibom> GetListPages(PageParm parm)
        {
            return Db.Queryable<tn_dts_equibom>().OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除设备结构
        /// <summary>
        /// 删除设备结构
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_equibom>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion
    }
}
