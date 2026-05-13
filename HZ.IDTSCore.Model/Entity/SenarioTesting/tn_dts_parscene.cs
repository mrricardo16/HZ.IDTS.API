using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 场景主表
    /// <summary>
    /// 场景主表
    /// </summary>
    [SugarTable("tn_dts_parscene")]
    public class tn_dts_parscene
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 场景编码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_parscene_no")]
        public string cn_s_parscene_no { get; set; }
        /// <summary>
        ///  场景名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_parscene_name")]
        public string cn_s_parscene_name { get; set; }
        /// <summary>
        ///  流程关系
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_parscene_prorelationship")]
        public string cn_s_parscene_prorelationship { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_parscene_remarks")]
        public string cn_s_parscene_remarks { get; set; }
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

    #region 保存场景数据类
    /// <summary>
    /// 保存场景数据类
    /// </summary>
    public class SaveSceneDate
    {
        /// <summary>
        /// 新增修改标识
        /// </summary>
        public string AddOrModify { get; set; }
        /// <summary>
        /// 删除场景-流程关系唯一标识列表
        /// </summary>
        public List<string> DeleteChisceneGuidList { get; set; }
        /// <summary>
        /// 场景唯一标识
        /// </summary>
        public string ParsceneGuid { get; set; }
        /// <summary>
        /// 场景编码
        /// </summary>
        public string ParsceneNo { get; set; }
        /// <summary>
        /// 场景名称
        /// </summary>
        public string ParsceneName { get; set; }
        /// <summary>
        /// 流程关系
        /// </summary>
        public string Prorelationship { get; set; }
        /// <summary>
        /// 场景所包含流程信息列表
        /// </summary>
        public List<ChisceneInformation> ChisceneInformationList { get; set; }
    }
    #endregion

    #region 开始测试
    /// <summary>
    /// 开始测试
    /// </summary>
    public class StartTestingViewModel
    {
        public List<StartTestingItem> StartTesting { get; set; }
    }

    /// <summary>
    /// 开始测试子项
    /// </summary>
    public class StartTestingItem
    {
        /// <summary>
        /// 场景编码
        /// </summary>
        public string Sceneno { get; set; }

        /// <summary>
        /// 是否启用货位同步
        /// </summary>
        public bool IsSynchronizeStock { get; set; }

        /// <summary>
        /// 同步货位的货位设备编码（不启用货位同步时，传空字符串）
        /// </summary>
        public string Goodsequipmentno { get; set; }

        /// <summary>
        /// 用户选择的客户端IP地址
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 是否循环推送
        /// </summary>
        public bool IsCirculate { get; set; }
    }
    #endregion

    #region 结束测试
    /// <summary>
    /// 结束测试
    /// </summary>
    public class StopTestingViewModel
    {
        public List<StopTestingViewItem> StopTesting { get; set; }
    }

    /// <summary>
    /// 结束测试子项
    /// </summary>
    public class StopTestingViewItem
    {

    }
    #endregion

    #region 暂停测试
    /// <summary>
    /// 暂停测试
    /// </summary>
    public class PauseTestingViewModel
    {
        public List<PauseTestingViewItem> PauseTesting { get; set; }
    }

    /// <summary>
    /// 暂停测试子项
    /// </summary>
    public class PauseTestingViewItem
    {

    }
    #endregion

    #region 继续测试
    /// <summary>
    /// 继续测试
    /// </summary>
    public class ContinueTestingViewModel
    {
        public List<ContinueTestingViewItem> ContinueTesting { get; set; }
    }

    /// <summary>
    /// 继续测试子项
    /// </summary>
    public class ContinueTestingViewItem
    {

    }
    #endregion

    #region 返回信息
    /// <summary>
    /// 返回信息
    /// </summary>
    public class ReturnMessageViewModel
    {
        public List<ReturnMessageItem> ReturnMessage { get; set; }
    }

    /// <summary>
    /// 返回信息子项
    /// </summary>
    public class ReturnMessageItem
    {
        /// <summary>
        /// 信息来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }
    #endregion

    #region 测试信息
    ///// <summary>
    ///// 测试信息
    ///// </summary>
    //public class TestInfo
    //{
    //    /// <summary>
    //    /// 场景编码
    //    /// </summary>
    //    public string Sceneno { get; set; }

    //    /// <summary>
    //    /// 是否同步货位
    //    /// </summary>
    //    public bool IsSynchronizeStock { get; set; }

    //    /// <summary>
    //    /// 是否循环推送
    //    /// </summary>
    //    public bool IsCirculate { get; set; }

    //    /// <summary>
    //    /// 货位设备编码
    //    /// </summary>
    //    public string Goodsequipmentno { get; set; }

    //    /// <summary>
    //    /// 推送是否停止
    //    /// </summary>
    //    public bool IsExit { get; set; }
    //}
    #endregion
}
