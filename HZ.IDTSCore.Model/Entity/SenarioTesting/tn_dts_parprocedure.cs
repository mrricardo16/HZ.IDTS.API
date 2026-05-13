using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 业务流程主表
    /// <summary>
    /// 业务流程主表
    /// </summary>
    [SugarTable("tn_dts_parprocedure")]
    public class tn_dts_parprocedure
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_parprocedure_no")]
        public string cn_s_parprocedure_no { get; set; }
        /// <summary>
        ///  流程名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_parprocedure_name")]
        public string cn_s_parprocedure_name { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_parprocedure_remarks")]
        public string cn_s_parprocedure_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
    }
    #endregion

    #region 流程信息类
    /// <summary>
    /// 流程信息类
    /// </summary>
    public class ParprocedureInformation
    {
        /// <summary>
        /// 流程唯一标识
        /// </summary>
        public string ParprocedureGuid { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string ParprocedureNo { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ParprocedureName { get; set; }
    }
    #endregion

    #region 通用信息类
    /// <summary>
    /// 通用信息类
    /// </summary>
    public class StandardInformation
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
    #endregion

    #region 保存流程数据类
    /// <summary>
    /// 保存流程数据类
    /// </summary>
    public class SaveProcedureDate
    {
        /// <summary>
        /// 新增修改标识
        /// </summary>
        public string AddOrModify { get; set; }
        /// <summary>
        /// 删除流程-设备关系唯一标识列表
        /// </summary>
        public List<string> DeleteChiprocedureGuidList { get; set; }
        /// <summary>
        /// 流程唯一标识
        /// </summary>
        public string ParprocedureGuid { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string ParprocedureNo { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ParprocedureName { get; set; }
        /// <summary>
        /// 流程所包含设备信息列表
        /// </summary>
        public List<ChiprocedureInformation> ChiprocedureInformationList { get; set; }
    }
    #endregion
}
