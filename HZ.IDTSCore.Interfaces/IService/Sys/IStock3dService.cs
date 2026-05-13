using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface IStock3dService : IBaseService<tn_dts_stock3d>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_stock3d> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 获取指定仓库，指定区域的报废货位列表字符串
        /// </summary>
        /// <param name="stockcode"></param>
        /// <param name="areacode"></param>
        /// <returns></returns>
        public string GetNullify(string stockcode, string areacode);

        /// <summary>
        /// 新增立库
        /// </summary>
        /// <param name="stock3d"></param>
        /// <returns></returns>
        public ReturnMessage AddStock3d(tn_dts_stock3d stock3d);

        /// <summary>
        /// 修改立库
        /// </summary>
        /// <param name="stock3d"></param>
        /// <returns></returns>
        public ReturnMessage UpdateStock3d(tn_dts_stock3d stock3d);

        /// <summary>
        /// 删除立库
        /// </summary>
        /// <param name="stock3d"></param>
        /// <returns></returns>
        public ReturnMessage DeleteStock3d(List<tn_dts_stock3d> stock3dList);
    }
}
