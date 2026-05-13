using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquibomService : IBaseService<tn_dts_equibom>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equibom> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);
    }
}
