using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.SenarioTesting
{
    public interface IEquicommandService : IBaseService<tn_dts_equicommand>
    {
        /// <summary>
        /// 按设备唯一标识、指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<EquicommandPlus> GetListPages(PageParm parm);

        /// <summary>
        /// 批量添加设备指令
        /// </summary>
        /// <param name="batchAddDate"></param>
        /// <returns></returns>
        public ReturnMessage BatchAddEquicommand(BatchAddEquicommandDate batchAddDate);

        /// <summary>
        /// 修改设备指令
        /// </summary>
        /// <param name="equicommand"></param>
        /// <returns></returns>
        public ReturnMessage UpdateEquicommand(tn_dts_equicommand equicommand);

        /// <summary>
        /// 编辑模版
        /// </summary>
        /// <param name="editEquicommandDate"></param>
        /// <returns></returns>
        public ReturnMessage EditEquicommandJson(EditEquicommandDate editEquicommandDate);

        /// <summary>
        /// 按指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<EquicommandInformation> GetEquicommandInformationPages(PageParm parm);

        /// <summary>
        /// 复制设备指令
        /// </summary>
        /// <param name="copyEquicommandDate"></param>
        /// <returns></returns>
        public ReturnMessage CopyEquicommand(CopyEquicommandDate copyEquicommandDate);

        /// <summary>
        /// 批量删除设备指令
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteEquicommand(List<string> guidList);

        /// <summary>
        /// 按有无通配符、指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<EquicommandInformation> GetEquicommandPlusInformationPages(PageParm parm);
    }
}
