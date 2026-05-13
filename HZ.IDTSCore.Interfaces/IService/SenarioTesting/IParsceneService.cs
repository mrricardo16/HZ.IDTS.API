using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IParsceneService : IBaseService<tn_dts_parscene>
    {
        /// <summary>
        /// 按场景编码（模糊）、场景名称（模糊）、流程关系分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_parscene> GetListPages(PageParm parm);

        /// <summary>
        /// 保存场景
        /// </summary>
        /// <param name="saveSceneDate"></param>
        /// <returns></returns>
        public ReturnMessage SaveScene(SaveSceneDate saveSceneDate);

        /// <summary>
        /// 批量删除场景
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteScene(List<string> guidList);
    }
}
