using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface IBackupsService : IBaseService<tn_dts_dbops>
    {
        /// <summary>
        /// 备份接口
        /// </summary>
        /// <param name="backupFilePath"></param>
        /// <returns></returns>
        public ReturnMessage BackUp(string backupFilePath, string category);

        /// <summary>
        /// 保存接口
        /// </summary>
        /// <param name="saveBackups"></param>
        /// <returns></returns>
        public ReturnMessage Save(SaveBackups saveBackups);

        /// <summary>
        /// 分页获取所有备份记录（不含查询）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_dbops> GetListPages(PageParm parm);

        /// <summary>
        /// 获取最近的自动备份配置接口
        /// </summary>
        /// <returns></returns>
        public SaveBackups GetLatestSaveBackups();
    }
}
