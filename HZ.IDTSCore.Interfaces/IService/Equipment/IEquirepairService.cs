using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquirepairService : IBaseService<tn_dts_equirepair>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equirepair> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 保存设备维修
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public ReturnMessage SaveDataRepair(SaveData_Repair saveData);

        /// <summary>
        /// 删除设备维修
        /// </summary>
        /// <param name="equirepair"></param>
        /// <returns></returns>
        public ReturnMessage DeleteDataRepair(List<tn_dts_equirepair> equirepair);

        ///// <summary>
        ///// 获取所有维修事项名称
        ///// </summary>
        ///// <returns></returns>
        //public List<MatterInfo> GetAllRepairMatters();

        ///// <summary>
        ///// 获取所有故障原因解决方案对
        ///// </summary>
        ///// <returns></returns>
        //public List<FaultSolutionPairInfo> GetAllFaultSolutionPairs();

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public tn_dts_equirepair Detail(string guid);

        /// <summary>
        /// 删除故障原因
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ReturnMessage DeleteReason(string guid);

        /// <summary>
        /// 删除维修项目
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ReturnMessage DeleteMatter(string guid);

        /// <summary>
        /// 按事项名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<MatterInfo> GetMatterInfoPageList(PageParm parm);

        /// <summary>
        /// 按故障原因分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<ReasonInfo> GetReasonInfoPageList(PageParm parm);

        /// <summary>
        /// 返回指定设备的前指定个数个维修提醒项
        /// </summary>
        /// <param name="returnNum"></param>
        /// <returns></returns>
        public List<EQRepairCollect> GetEQRepairCollectList(int returnNum ,string deviceCode);
    }
}
