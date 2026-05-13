using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IGoodscommandService : IBaseService<tn_dts_goodscommand>
    {
        /// <summary>
        /// 保存货位指令
        /// </summary>
        /// <param name="saveGoodscommand"></param>
        /// <returns></returns>
        public ReturnMessage SaveGoodscommand(SaveGoodscommand saveGoodscommand);
        /// <summary>
        /// 判断同步数据是否需要缓存（0：不需要；1：需要新增；2：需要修改；3：Json有问题）
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int DetermineSynchronizeExist(string virequi, string synchronizeJson, ref tn_dts_goodscommand updateGoodscommand);

        /// <summary>
        /// 批量添加货位指令
        /// </summary>
        /// <param name="batchAddDate"></param>
        /// <returns></returns>
        public ReturnMessage BatchAddGoodscommand(BatchAddGoodscommandDate batchAddDate);

        /// <summary>
        /// 修改货位指令
        /// </summary>
        /// <param name="goodscommand"></param>
        /// <returns></returns>
        public ReturnMessage UpdateGoodscommand(tn_dts_goodscommand goodscommand);

        /// <summary>
        /// 编辑模版
        /// </summary>
        /// <param name="editGoodscommandDate"></param>
        /// <returns></returns>
        public ReturnMessage EditGoodscommandJson(EditGoodscommandDate editGoodscommandDate);

        /// <summary>
        /// 按指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<GoodscommandPlus> GetPageList(PageParm parm);

        /// <summary>
        /// 复制货位指令
        /// </summary>
        /// <param name="copyGoodscommandDate"></param>
        /// <returns></returns>
        public ReturnMessage CopyGoodscommand(CopyGoodscommandDate copyGoodscommandDate);

        /// <summary>
        /// 批量删除货位指令
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteGoodscommand(List<string> guidList);

        /// <summary>
        /// 按指令编码和指令名称混合模糊分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<GoodscommandInformation> GetGoodscommandInformationPages(PageParm parm);

        /// <summary>
        /// 批量添加库位
        /// </summary>
        /// <param name="batchAddLocationDate"></param>
        /// <returns></returns>
        public ReturnMessage BatchAddLocation(BatchAddLocationDate batchAddLocationDate, MaxRowColLayer maxRowColLayer);

        /// <summary>
        /// 修改库位
        /// </summary>
        /// <param name="modifyLocationDate"></param>
        /// <returns></returns>
        public ReturnMessage ModifyLocation(ModifyLocationDate modifyLocationDate);

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="deleteLocationDate"></param>
        /// <returns></returns>
        public ReturnMessage DeleteLocation(DeleteLocationDate deleteLocationDate);
    }
}

