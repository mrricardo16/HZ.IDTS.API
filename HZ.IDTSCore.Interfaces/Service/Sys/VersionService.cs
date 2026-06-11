using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class VersionService : BaseService<tn_dts_version>, IVersionService
    {
        public VersionService(SessionInfo session) : base(session)
        {

        }

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_version> GetListPages(PageParm parm)
        {
            return Db.Queryable<tn_dts_version>()
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除版本
        /// <summary>
        /// 删除版本
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_version>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 新增版本信息
        /// <summary>
        /// 新增版本信息
        /// </summary>
        /// <param name="versionList"></param>
        /// <returns></returns>
        public ReturnMessage AddPackages(List<tn_dts_version> versionList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();

            if (versionList == null || versionList.Count == 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "版本信息不能为空！";
                return returnMessage;
            }

            foreach (var version in versionList)
            {
                if (version == null || string.IsNullOrWhiteSpace(version.cn_s_ver_packagedate))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "更新包版本不能为空！";
                    return returnMessage;
                }

                version.cn_s_ver_packagedate = version.cn_s_ver_packagedate.Trim();
            }

            List<tn_dts_version> upsertVersionList = versionList
                .GroupBy(it => it.cn_s_ver_packagedate, StringComparer.OrdinalIgnoreCase)
                .Select(it => it.Last())
                .ToList();

            ApiResult res = UseTransaction(dbTran =>
            {
                foreach (var version in upsertVersionList)
                {
                    string packageDate = version.cn_s_ver_packagedate;
                    string lowerPackageDate = packageDate.ToLower();
                    tn_dts_version existing = dbTran.Queryable<tn_dts_version>()
                        .Where(it => it.cn_s_ver_packagedate != null && it.cn_s_ver_packagedate.Trim().ToLower() == lowerPackageDate)
                        .First();

                    tn_dts_logs versionLog = new tn_dts_logs();
                    versionLog.cn_guid = Guid.NewGuid().ToString();
                    versionLog.cn_s_logs_type = "操作";
                    versionLog.cn_t_create = DateTime.Now;

                    if (existing == null)
                    {
                        if (!version.cn_s_ver_isupdated.HasValue)
                        {
                            version.cn_s_ver_isupdated = 0;
                        }

                        dbTran.Insertable<tn_dts_version>(version).ExecuteCommand();
                        versionLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用上传更新包功能向tn_dts_version表中新增一条版本信息记录，详细信息为" + JsonConvert.SerializeObject(version);
                    }
                    else
                    {
                        existing.cn_s_ver_number = version.cn_s_ver_number;
                        existing.cn_s_ver_packagetype = version.cn_s_ver_packagetype;
                        existing.cn_s_ver_updatecontent = version.cn_s_ver_updatecontent;
                        existing.cn_s_ver_remarks = version.cn_s_ver_remarks;
                        existing.cn_s_modify = user.UserCode;
                        existing.cn_s_modify_by = user.UserName;
                        existing.cn_t_modify = DateTime.Now;

                        dbTran.Updateable<tn_dts_version>(existing).ExecuteCommand();
                        versionLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用上传更新包功能发现相同包版本已存在，覆盖更新tn_dts_version表中一条版本信息记录，详细信息为" + JsonConvert.SerializeObject(version);
                    }

                    logList.Add(versionLog);
                }

                if (logList.Count > 0)
                {
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                }
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "19.2升级（Version）记录管理上传更新包接口新增或更新版本信息失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "新增或更新失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增或更新成功！";
            return returnMessage;
        }
        #endregion

        #region 读取一个产品版本号里所有的更新包版本
        /// <summary>
        /// 读取一个产品版本号里所有的更新包版本
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<string> GetAllPackageDateByNumber(string number)
        {
            return Db.Queryable<tn_dts_version>().Where(it => it.cn_s_ver_number == number).Select(it => it.cn_s_ver_packagedate).ToList();
        }
        #endregion

        #region 修改指定更新包版本的备份信息
        /// <summary>
        /// 修改指定更新包版本的备份信息
        /// </summary>
        /// <param name="backupsPackage"></param>
        /// <returns></returns>
        public ReturnMessage BackUpPackage(BackupsPackage backupsPackage)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            if (backupsPackage == null || string.IsNullOrWhiteSpace(backupsPackage.packageDate))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "备份对应更新包版本不能为空。";
                return returnMessage;
            }

            string packageDate = backupsPackage.packageDate.Trim();
            string lowerPackageDate = packageDate.ToLower();
            tn_dts_version itemNumber = Db.Queryable<tn_dts_version>()
                .Where(it => it.cn_s_ver_packagedate != null && it.cn_s_ver_packagedate.Trim().ToLower() == lowerPackageDate)
                .First();
            if (itemNumber == null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = $"备份对应版本记录不存在，PackageDate={packageDate}";
                return returnMessage;
            }

            itemNumber.cn_s_var_backupfilename = backupsPackage.backupfilename;
            itemNumber.cn_s_var_backupfiletype = backupsPackage.backupfiletype;
            itemNumber.cn_s_var_backupfilepath = backupsPackage.backupfilepath;
            var res = Db.Updateable<tn_dts_version>(itemNumber).ExecuteCommand();
            if (res <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "备份文件数据修改失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "备份文件数据修改成功！";
            return returnMessage;
        }
        #endregion

        #region 修改指定更新包版本的更新信息
        /// <summary>
        /// 修改指定更新包版本的更新信息
        /// </summary>
        /// <param name="executeUpdatePackage"></param>
        /// <returns></returns>
        public ReturnMessage ExecuteUpdatePackage(ExecuteUpdatePackage executeUpdatePackage)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            if (executeUpdatePackage == null || string.IsNullOrWhiteSpace(executeUpdatePackage.PackageDate))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "更新包版本不能为空。";
                return returnMessage;
            }

            string packageDate = executeUpdatePackage.PackageDate.Trim();
            string lowerPackageDate = packageDate.ToLower();
            tn_dts_version itemNumber = Db.Queryable<tn_dts_version>()
                .Where(it => it.cn_s_ver_packagedate != null && it.cn_s_ver_packagedate.Trim().ToLower() == lowerPackageDate)
                .First();
            if (itemNumber == null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = $"更新包版本记录不存在，PackageDate={packageDate}";
                return returnMessage;
            }

            itemNumber.cn_s_ver_isupdated = executeUpdatePackage.IsUpdated;
            itemNumber.cn_s_ver_update = executeUpdatePackage.UpdateTime;
            itemNumber.cn_s_ver_updateman = executeUpdatePackage.UpdateMan;
            var res = Db.Updateable<tn_dts_version>(itemNumber).ExecuteCommand();
            if (res <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "更新包状态写入失败，数据库无影响行数。";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "更新包记录已覆盖为目标状态。";
            return returnMessage;
        }
        #endregion

        #region 获取前端最新版本
        /// <summary>
        /// 获取前端最新版本
        /// </summary>
        /// <returns></returns>
        public string GetLatestUIVersion()
        {
            string latestUIVersion = Db.Queryable<tn_dts_version>().Where(it => it.cn_s_ver_packagedate.Contains("idts_ui") && it.cn_s_ver_isupdated == 1).OrderBy(it => it.cn_s_ver_update, SqlSugar.OrderByType.Desc).Select(it => it.cn_s_ver_packagedate).First();
            if (latestUIVersion is null)
            {
                latestUIVersion = "无";
            }
            return latestUIVersion;
        }
        #endregion

        #region 获取后端最新版本
        /// <summary>
        /// 获取后端最新版本
        /// </summary>
        /// <returns></returns>
        public string GetLatestAPIVersion()
        {
            string latestAPIVersion = Db.Queryable<tn_dts_version>().Where(it => it.cn_s_ver_packagedate.Contains("idts_api") && it.cn_s_ver_isupdated == 1).OrderBy(it => it.cn_s_ver_update, SqlSugar.OrderByType.Desc).Select(it => it.cn_s_ver_packagedate).First();
            if (latestAPIVersion is null)
            {
                latestAPIVersion = "无";
            }
            return latestAPIVersion;
        }
        #endregion

        #region 获取最新产品版本号
        /// <summary>
        /// 获取最新产品版本号
        /// </summary>
        /// <returns></returns>
        public string GetLatestNumberVersion()
        {
            string latestAPIVersion = Db.Queryable<tn_dts_version>().Where(it => it.cn_s_ver_isupdated == 1).OrderBy(it => it.cn_s_ver_update, SqlSugar.OrderByType.Desc).Select(it => it.cn_s_ver_number).First();
            if (latestAPIVersion is null)
            {
                latestAPIVersion = "无";
            }
            return latestAPIVersion;
        }
        #endregion


        #region 删除一个产品版本号里所有的更新包版本
        /// <summary>
        /// 删除一个产品版本号里所有的更新包版本
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public ReturnMessage DeleteAllPackageDateByNumber(string number)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            List<tn_dts_version> versionNumberList = Db.Queryable<tn_dts_version>().Where(it => it.cn_s_ver_number == number).ToList();
            if (versionNumberList.Count == 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "传入的产品版本号在tn_dts_version表中没有更新包版本记录！";
                return returnMessage;
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_version>(versionNumberList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "升级记录管理删除一个产品版本号里所有的更新包版本接口删除更新包版本记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

    }
}
