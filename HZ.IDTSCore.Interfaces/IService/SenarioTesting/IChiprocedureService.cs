using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IChiprocedureService : IBaseService<tn_dts_chiprocedure>
    {
        /// <summary>
        /// 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息和终点信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiprocedureInformation> GetAllChiprocedureByParprocedureguid(PageParm param);

        /// <summary>
        /// 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息、终点信息和修改创建信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiprocedureInformationPlus> GetAllChiprocedurePlusByParprocedureguid(PageParm param);
    }
}
