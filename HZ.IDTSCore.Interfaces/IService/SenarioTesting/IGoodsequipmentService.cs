using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IGoodsequipmentService : IBaseService<tn_dts_goodsequipment>
    {
        /// <summary>
        /// 按货位设备、指令编码和指令名称混合模糊分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<GoodscommandInformation> GetGoodscommandInformationPages(PageParm parm);

        /// <summary>
        /// 获取所有货位设备信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<GoodsequipmentInformation> GetAllGoodsequipmentInformation(PageParm parm);

        /// <summary>
        /// 按货位设备编码和名称混合模糊分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_goodsequipment> GetListPages(PageParm parm);

        /// <summary>
        /// 获取指定货位设备信息
        /// </summary>
        /// <param name="goodsequipmentguid"></param>
        /// <returns></returns>
        public tn_dts_goodsequipment GetGoodsequipment(string goodsequipmentguid);

        /// <summary>
        /// 保存货位设备
        /// </summary>
        /// <param name="saveGoodsequipment"></param>
        /// <returns></returns>
        public ReturnMessage SaveGoodsequipment(SaveGoodsequipment saveGoodsequipment);

        /// <summary>
        /// 批量删除货位设备
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteGoodsequipment(List<string> guidList);

        /// <summary>
        /// 获取一个货位设备类型下所有货位设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<GoodsequipmentInformation> GetGoodsequipmentInformationByGoodsequitype(PageParm param);
    }
}
