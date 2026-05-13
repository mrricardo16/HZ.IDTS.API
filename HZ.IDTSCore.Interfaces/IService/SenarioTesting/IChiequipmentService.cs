using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IChiequipmentService : IBaseService<tn_dts_chiequipment>
    {
        ///// <summary>
        ///// 读取一个设备下货位指令的位置（0：该设备不含货位指令；1：只有一个货位指令在开头；2：只有一个货位指令在结尾；3：只有两个一个货位指令在开头一个货位指令在结尾；4：其他情况）；
        ///// </summary>
        ///// <param name="equiguid"></param>
        ///// <param name="startLocationCode"></param>
        ///// <param name="endLocationCode"></param>
        ///// <returns></returns>
        //public int GetGoodscommandLocation(string equiguid, ref string startLocationCode, ref string endLocationCode);

        /// <summary>
        /// 读取一个设备下所有货位指令
        /// </summary>
        /// <param name="equiguid"></param>
        /// <param name="startLocationCode"></param>
        /// <param name="endLocationCode"></param>
        /// <returns></returns>
        public List<tn_dts_goodscommand> GetGoodscommand(string equiguid, ref string startRowColLayer, ref string endRowColLayer);

        /// <summary>
        /// 保存设备
        /// </summary>
        /// <param name="saveEquipmentDate"></param>
        /// <returns></returns>
        public ReturnMessage SaveEquipment(SaveEquipmentDate saveEquipmentDate);

        /// <summary>
        /// 通过流程子表唯一标识获取所有设备子表信息（含指令信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiequipmentInformation> GetAllChiequipmentByChiprocedureguid(PageParm param);

        /// <summary>
        /// 通过流程子表唯一标识获取所有设备子表信息（含指令信息和修改创建信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiequipmentInformationPlus> GetAllChiequipmentPlusByChiprocedureguid(PageParm param);
    }
}
