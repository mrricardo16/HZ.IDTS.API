using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface IVersionService : IBaseService<tn_dts_version>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_version> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 新增版本信息
        /// </summary>
        /// <param name="versionList"></param>
        /// <returns></returns>
        public ReturnMessage AddPackages(List<tn_dts_version> versionList);

        /// <summary>
        /// 读取一个产品版本号里所有的更新包版本
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<string> GetAllPackageDateByNumber(string number);

        /// <summary>
        /// 修改指定更新包版本的备份信息
        /// </summary>
        /// <param name="backupsPackage"></param>
        /// <returns></returns>
        public ReturnMessage BackUpPackage(BackupsPackage backupsPackage);

        /// <summary>
        /// 修改指定更新包版本的更新信息
        /// </summary>
        /// <param name="executeUpdatePackage"></param>
        /// <returns></returns>
        public ReturnMessage ExecuteUpdatePackage(ExecuteUpdatePackage executeUpdatePackage);

        /// <summary>
        /// 获取前端最新版本
        /// </summary>
        /// <returns></returns>
        public string GetLatestUIVersion();

        /// <summary>
        /// 获取后端最新版本
        /// </summary>
        /// <returns></returns>
        public string GetLatestAPIVersion();

        /// <summary>
        /// 删除一个产品版本号里所有的更新包版本
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public ReturnMessage DeleteAllPackageDateByNumber(string number);

        /// <summary>
        /// 获取最新产品版本号
        /// </summary>
        /// <returns></returns>
        public string GetLatestNumberVersion();
    }
}
