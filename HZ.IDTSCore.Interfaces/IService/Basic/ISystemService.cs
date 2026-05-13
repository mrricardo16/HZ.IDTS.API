using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService
{
    public interface ISystemService
    {
        ApiResult AddMenus(sys_menu mode);
        ApiResult UpdateMenus(sys_menu mode);
        ApiResult IsCollection(string menuurlcode);
        List<sys_menu> GetAllMenus(string sourceTerminal);
        List<tn_wms_view_table_conf> GetTableColumns(string userCode);
        List<tn_wms_view_table_conf> GetTableColumns(string userCode,string powerCode);
        int SaveCusColumns(List<tn_wms_view_table_conf> columns);
        List<tn_wms_view_table_def> GetPageDefTree();
        List<tn_wms_view_table_conf> GetConfList(string powerCode);
        ApiResult SavePageBasicSet(tn_wms_view_table_conf model);
        List<sys_menu> GetPDAMenus();
        List<sys_menu> GetMenus(int type);
        ApiResult Add(tn_wms_view_table_conf model);
        ApiResult SaveNode(tn_wms_view_table_def model);
        ApiResult DeleteNode(string guid);
        List<string> GetButton(string pageName);

        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <param name="backupFilePath">备份的路径(绝对路径)</param>
        /// <returns></returns>
        ApiResult DatabaseBackup(string backupFilePath);

        ApiResult BackupRemoteDatabase(string remoteServer, string remoteUser, string remotePassword, string remoteDatabase, string dumpFilePath);
    }
}
