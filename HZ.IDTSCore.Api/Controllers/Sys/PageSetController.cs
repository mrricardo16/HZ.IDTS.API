using System;
using System.Collections.Generic;
using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.View;
using Microsoft.AspNetCore.Mvc;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class PageSetController : BaseController
    {
        private readonly ISystemService _ISystemService;


        public PageSetController()
        {
            _ISystemService = ServiceLocator.GetService<ISystemService>(HttpContextSession());

        }

        [HttpGet]
        public IActionResult GetTableColumns()
        {
            UserSession user = GetSessionInfo();
            var res = _ISystemService.GetTableColumns(user.UserCode);
            return toResponse(res);
        }

        [HttpGet]
        public IActionResult GetColumnsByPowerCode(string powerCode)
        {
            UserSession user = GetSessionInfo();
            var res = _ISystemService.GetTableColumns(user.UserCode, powerCode);
            return toResponse(res);
        }

        [HttpGet]
        public IActionResult GetConfList(string powerCode)
        {
            var res = _ISystemService.GetConfList(powerCode);
            return toResponse(res);
        }

        [HttpPost]
        public IActionResult SaveCusColumns(List<tn_wms_view_table_conf> confModels)
        {
            var res = _ISystemService.SaveCusColumns(confModels);
            return toResponse(res);
        }

        [HttpGet]
        public IActionResult GetPageDefTree()
        {
            var res = _ISystemService.GetPageDefTree();
            return toResponse(res);
        }

        [HttpPost]
        public IActionResult SavePageBasicSet(tn_wms_view_table_conf model)
        {
            var res = _ISystemService.SavePageBasicSet(model);
            return new JsonResult(res);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add(tn_wms_view_table_conf model)
        {
            UserSession user = GetSessionInfo();
            model.cn_s_creator = user.UserCode;
            model.cn_s_creator_by = user.UserName;
            model.cn_t_create = DateTime.Now;
            var res = _ISystemService.Add(model);
            return toResponse(res);
        }

        [HttpPost]
        public IActionResult SaveNode(tn_wms_view_table_def model)
        {
            UserSession user = GetSessionInfo();
            model.cn_s_creator = user.UserCode;
            model.cn_s_creator_by = user.UserName;
            model.cn_t_create = DateTime.Now;
            var res = _ISystemService.SaveNode(model);
            return toResponse(res);
        }


        [HttpGet]
        public IActionResult DeleteNode(string guid)
        {
            var res = _ISystemService.DeleteNode(guid);
            return toResponse(res);
        }

        [HttpGet]
        public IActionResult PushMdg()
        {
            var res = _ISystemService.GetConfList("");
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult result = ApiResult.Success();
            WebApiManager.HttpPost(mdg, "api/ViewTable/Init", Newtonsoft.Json.JsonConvert.SerializeObject(res), ref result);
            return toResponse(result);
        }
    }
}
