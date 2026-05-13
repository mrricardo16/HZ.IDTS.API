using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IChisceneService : IBaseService<tn_dts_chiscene>
    {
        /// <summary>
        /// 通过场景主表唯一标识获取所有场景子表信息（含流程信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChisceneInformation> GetAllChisceneByParsceneguid(PageParm param);
        /// <summary>
        /// 通过场景主表唯一标识获取所有场景子表信息（含流程信息和修改创建信息），按流程编码和流程名称分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChisceneInformationPlus> GetAllChiscenePlusByParsceneguid(PageParm param);
    }
}
