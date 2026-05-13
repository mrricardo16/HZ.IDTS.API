using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class BackupsController : BaseController
    {
        private IBackupsService _IBackupsService;

        public BackupsController()
        {
            _IBackupsService = ServiceLocator.GetService<IBackupsService>(HttpContextSession());
        }

        #region 手工备份
        /// <summary>
        /// 手工备份
        /// </summary>
        /// <param name="backupFilePath"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BackUp(string backupFilePath)
        {
            ReturnMessage res = _IBackupsService.BackUp(backupFilePath,"人工");
            ApiResult resultReturn = new ApiResult();
            if (res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = res.Message;
            }
            else
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
            }
            return toResponse(resultReturn);
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="saveBackups"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(SaveBackups saveBackups)
        {
            ReturnMessage res = _IBackupsService.Save(saveBackups);
            ApiResult resultReturn = new ApiResult();
            if (res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = res.Message;
                BackupsDriver.Instance.DBackups = _IBackupsService.GetLatestSaveBackups();
                BackupsDriver.Instance.LastBackups = DateTime.Now;
            }
            else
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
            }
            return toResponse(resultReturn);
        }
        #endregion

        #region 分页获取所有备份记录（不含查询）
        /// <summary>
        /// 分页获取所有备份记录（不含查询）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm parm)
        {
            var res = _IBackupsService.GetListPages(parm);
            return toResponse(res);
        }
        #endregion

        #region 获取最近的自动备份配置
        /// <summary>
        /// 获取最近的自动备份配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetLatestSaveBackups()
        {
            var res = _IBackupsService.GetLatestSaveBackups();
            return toResponse(res);
        }
        #endregion
    }
}
