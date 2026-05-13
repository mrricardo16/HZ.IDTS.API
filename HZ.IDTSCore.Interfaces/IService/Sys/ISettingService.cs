using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface ISettingService : IBaseService<tn_dts_setting>
    {
        /// <summary>
        /// 分页查询数据(含模糊查询）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_setting> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public ApiResult BatchAdd(List<tn_dts_setting> listModel);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public List<tn_dts_setting> GroupByTest();

        /// <summary>
        /// 保存系统配置
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public ReturnMessage SaveSetting(SettingSavety ss);

        /// <summary>
        /// 下一步接口
        /// </summary>
        /// <param name="settingItemList"></param>
        /// <returns></returns>
        public ReturnMessage NextStep(List<SettingItem> settingItemList);

        /// <summary>
        /// 根据关键字编码读取设置值
        /// </summary>
        /// <param name="keycode"></param>
        /// <returns></returns>
        public string GetKeyvalueByKeycode(string keycode);
    }
}
