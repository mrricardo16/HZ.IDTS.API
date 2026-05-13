using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface IThreeDimensionAngleAreaService : IBaseService<tn_dts_3danglearea>
    {
        /// <summary>
        /// 保存3D建模视角区域
        /// </summary>
        /// <param name="threeDimensionAngleArea"></param>
        /// <returns></returns>
        public ReturnMessage SaveThreeDimensionAngleArea(SaveThreeDimensionAngleArea saveThreeDimensionAngleArea);

        /// <summary>
        /// 分页查询3D建模视角区域
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_3danglearea> GetListPages(PageParm parm);

        /// <summary>
        /// 删除3D建模视角区域
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteThreeDimensionAngleArea(List<string> guidList);
    }
}
