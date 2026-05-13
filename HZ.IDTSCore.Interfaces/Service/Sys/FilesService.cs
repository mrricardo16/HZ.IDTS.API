using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class FilesService : BaseService<tn_dts_files>, IFilesService
    {
        public FilesService(SessionInfo session) : base(session)
        {

        }

        #region 分页模糊查询来源Guid和文件名称
        /// <summary>
        /// 分页模糊查询来源Guid和文件名称
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_files> GetListPages(PageParm parm)
        {
            string cn_s_files_source_guid = parm.Parms["cn_s_files_source_guid"].ObjToString();
            string cn_s_files_name = parm.Parms["cn_s_files_name"].ObjToString();
            return Db.Queryable<tn_dts_files>().WhereIF(!string.IsNullOrEmpty(cn_s_files_source_guid), (s => s.cn_s_files_source_guid.Contains(cn_s_files_source_guid)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_files_name), (s => s.cn_s_files_name.Contains(cn_s_files_name)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除附件
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_files>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        //#region 批量删除区域静态图附件表信息
        ///// <summary>
        ///// 批量删除区域静态图附件表信息
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteCameraArea(List<string> guidList)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    foreach (string guid in guidList)
        //    {
        //        if (Db.Queryable<tn_dts_files>().Where(it => it.cn_guid == guid).First() is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的相机区域中有区域静态图的相机区域中有不在服务器上传文件夹中的区域静态图！";
        //            return returnMessage;
        //        }
        //    }

        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        foreach (string guid in guidList)
        //        {
        //            dbTran.Deleteable<tn_dts_files>().Where(it => it.cn_guid == guid).ExecuteCommand();
        //        }
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除区域静态图附件表信息失败!";
        //        return returnMessage;
        //    }

        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "批量删除区域静态图附件表信息成功！";
        //    return returnMessage;
        //}
        //#endregion

    }
}
