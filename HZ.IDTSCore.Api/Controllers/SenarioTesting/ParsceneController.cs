using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HZ.IDTSCore.Api.Controllers.SenarioTesting
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class ParsceneController : BaseController
    {
        private IParsceneService _IParsceneService;
        private IParprocedureService _IProcedureService;
        private IChisceneService _IChisceneService;

        public ParsceneController()
        {
            _IParsceneService = ServiceLocator.GetService<IParsceneService>(HttpContextSession());
            _IProcedureService = ServiceLocator.GetService<IParprocedureService>(HttpContextSession());
            _IChisceneService = ServiceLocator.GetService<IChisceneService>(HttpContextSession());
        }

        #region 按场景编码（模糊）、场景名称（模糊）、流程关系分页查询
        /// <summary>
        /// 按场景编码（模糊）、场景名称（模糊）、流程关系分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IParsceneService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 按流程编码和流程名称混合模糊分页查询流程信息
        /// <summary>
        /// 按流程编码和流程名称混合模糊分页查询流程信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetProcedure(PageParm param)
        {
            var res = _IProcedureService.GetProcedure(param);
            return toResponse(res);
        }
        #endregion

        #region 通过场景主表唯一标识获取所有场景子表信息（含流程信息）
        /// <summary>
        /// 通过场景主表唯一标识获取所有场景子表信息（含流程信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllChisceneByParsceneguid(PageParm param)
        {
            var res = _IChisceneService.GetAllChisceneByParsceneguid(param);
            return toResponse(res);
        }
        #endregion

        #region 保存场景
        /// <summary>
        /// 保存场景
        /// </summary>
        /// <param name="saveSceneDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveScene(SaveSceneDate saveSceneDate)
        {
            ReturnMessage res = _IParsceneService.SaveScene(saveSceneDate);
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

        #region 批量删除场景
        /// <summary>
        /// 批量删除场景
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteScene([FromBody] List<string> guidList)
        {
            ReturnMessage res = _IParsceneService.DeleteScene(guidList);
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

        #region 通过场景主表唯一标识获取所有场景子表信息（含流程信息和修改创建信息），按流程编码和流程名称分页模糊查询
        /// <summary>
        /// 通过场景主表唯一标识获取所有场景子表信息（含流程信息和修改创建信息），按流程编码和流程名称分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllChiscenePlusByParsceneguid(PageParm param)
        {
            var res = _IChisceneService.GetAllChiscenePlusByParsceneguid(param);
            return toResponse(res);
        }
        #endregion
    }
}
