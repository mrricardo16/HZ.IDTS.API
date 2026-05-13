using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 场景子表
    /// <summary>
    /// 场景子表
    /// </summary>
    [SugarTable("tn_dts_chiscene")]
    public class tn_dts_chiscene
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 场景唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_chiscene_parsceguid")]
        public string cn_s_chiscene_parsceguid { get; set; }
        /// <summary>
        ///  业务流程唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiscene_parproguid")]
        public string cn_s_chiscene_parproguid { get; set; }
        /// <summary>
        ///  流程次序
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_chiscene_sequence")]
        public int cn_n_chiscene_sequence { get; set; }
        /// <summary>
        ///  流程间隔
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_chiscene_interval")]
        public int cn_n_chiscene_interval { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiscene_remarks")]
        public string cn_s_chiscene_remarks { get; set; }
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

    #region 场景子表信息（包含流程信息）
    /// <summary>
    /// 场景子表信息（包含流程信息）
    /// </summary>
    public class ChisceneInformation
    {
        /// <summary>
        /// 场景-流程关系唯一标识
        /// </summary>
        public string ChisceneGuid { get; set; }
        /// <summary>
        /// 流程唯一标识
        /// </summary>
        public string ProcedureGuid { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string ProcedureNo { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcedureName { get; set; }
        /// <summary>
        /// 流程次序
        /// </summary>
        public int ChisceneSequence { get; set; }
        /// <summary>
        /// 流程间隔/ms
        /// </summary>
        public int ChisceneInterval { get; set; }
    }
    #endregion

    #region 场景子表信息（含流程信息和修改创建信息）
    /// <summary>
    /// 场景子表信息（含流程信息和修改创建信息）
    /// </summary>
    public class ChisceneInformationPlus
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        public string ProcedureNo { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcedureName { get; set; }
        /// <summary>
        /// 流程次序
        /// </summary>
        public int ChisceneSequence { get; set; }
        /// <summary>
        /// 流程间隔
        /// </summary>
        public int ChisceneInterval { get; set; }
        /// <summary>
        /// 修改人编码
        /// </summary>
        public string ModifyNo { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        public string ModifyName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string CreateNo { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
    #endregion

}
