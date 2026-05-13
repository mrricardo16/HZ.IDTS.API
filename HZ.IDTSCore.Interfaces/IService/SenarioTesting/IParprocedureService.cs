using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IParprocedureService : IBaseService<tn_dts_parprocedure>
    {
        /// <summary>
        /// 按流程编码和流程名称混合模糊分页查询流程信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<ParprocedureInformation> GetProcedure(PageParm parm);

        /// <summary>
        /// 按流程编码和流程名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_parprocedure> GetListPages(PageParm parm);

        /// <summary>
        /// 按（立库/地堆）编码或（立库/地堆）名称混合模糊分页查询（立库/地堆）信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<StandardInformation> GetStockSite(PageParm parm);

        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="saveProcedureDate"></param>
        /// <returns></returns>
        public ReturnMessage SaveProcedure(SaveProcedureDate saveProcedureDate);

        /// <summary>
        /// 批量删除流程
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteProcedure(List<string> guidList);
    }
}
