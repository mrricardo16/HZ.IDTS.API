using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquiupkeepService : IBaseService<tn_dts_equiupkeep>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equiupkeep> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 保存设备保养
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public ReturnMessage SaveDataUpkeep(SaveData_Upkeep saveData);

        /// <summary>
        /// 删除设备保养
        /// </summary>
        /// <param name="equiupkeepList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteDataUpkeep(List<tn_dts_equiupkeep> equiupkeepList);

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public tn_dts_equiupkeep Detail(string guid);

        /// <summary>
        /// 按事项名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<MatterInfo> GetMatterInfoPageList(PageParm parm);

        /// <summary>
        /// 删除保养项目
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ReturnMessage DeleteMatter(string guid);

        /// <summary>
        /// 返回指定设备的前指定个数个保养提醒项
        /// </summary>
        /// <param name="returnNum"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public List<EQUpkeepCollect> GetEQUpkeepCollectList(int returnNum, string deviceCode);

    }
} 
