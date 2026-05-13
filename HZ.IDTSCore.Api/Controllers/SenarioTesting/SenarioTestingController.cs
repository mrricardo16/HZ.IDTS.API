using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Global;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.SenarioTesting
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class SenarioTestingController : BaseController
    {
        private IParsceneService _IParsceneService;
        private IChisceneService _IChisceneService;
        private IParprocedureService _IParprocedureService;
        private IChiprocedureService _IChiprocedureService;
        private IEquipmentService _IEquipmentService;
        private IChiequipmentService _IChiequipmentService;
        private IEquicommandService _IEquicommandService;
        private IGoodscommandService _IGoodscommandService;
        private ISiteinfoService _ISiteinfoService;
        private IStock3dService _IStock3dService;
        private IGoodsequipmentService _IGoodsequipmentService;

        public SenarioTestingController()
        {
            _IParsceneService = ServiceLocator.GetService<IParsceneService>(HttpContextSession());
            _IChisceneService = ServiceLocator.GetService<IChisceneService>(HttpContextSession());
            _IParprocedureService = ServiceLocator.GetService<IParprocedureService>(HttpContextSession());
            _IChiprocedureService = ServiceLocator.GetService<IChiprocedureService>(HttpContextSession());
            _IEquipmentService = ServiceLocator.GetService<IEquipmentService>(HttpContextSession());
            _IChiequipmentService = ServiceLocator.GetService<IChiequipmentService>(HttpContextSession());
            _IEquicommandService = ServiceLocator.GetService<IEquicommandService>(HttpContextSession());
            _IGoodscommandService = ServiceLocator.GetService<IGoodscommandService>(HttpContextSession());
            _ISiteinfoService = ServiceLocator.GetService<ISiteinfoService>(HttpContextSession());
            _IStock3dService = ServiceLocator.GetService<IStock3dService>(HttpContextSession());
            _IGoodsequipmentService = ServiceLocator.GetService<IGoodsequipmentService>(HttpContextSession());
        }

        #region 开始测试
        ///// <summary>
        ///// 开始测试
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        ///// 注：有问题：（1）读取一个设备下所有货位指令由于tn_dts_goodscommand中排列层字段已删除GetGoodscommand
        //[HttpPost]
        //public IActionResult StartTest(StartTestModel model)
        //{
        //    ApiResult result = new ApiResult();
        //    UserSession user = GetSessionInfo();
        //    //if (string.IsNullOrEmpty(model.Virequino))
        //    //{
        //    //    result.IsSuccess = false;
        //    //    result.ErrCode = 0;
        //    //    result.Message = "没有选择客户端，请重试！";
        //    //    return toResponse(result);
        //    //}
        //    if (model.IsSynchronizeStock)
        //    {
        //        ReturnMessage returnMessage = GoodscommandDriver.Instance.SynchronizeStock(model.Goodsequipmentno);
        //        if (!returnMessage.IsSuccess)
        //        {
        //            result.IsSuccess = false;
        //            result.ErrCode = 0;
        //            result.Message = returnMessage.Message;
        //            return toResponse(result);
        //        }
        //    }
        //    MongoDBSingleton.Instance.DeleteAll<MongoCommand>();

        //    tn_dts_parscene parsceneNo = _IParsceneService.GetFirst(it => it.cn_s_parscene_no == model.Sceneno);
        //    if (parsceneNo is null)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrCode = 0;
        //        result.Message = "tn_dts_parscene里没有场景编码为：" + model.Sceneno + "的场景记录，请重试！";
        //        return toResponse(result);
        //    }
        //    string guidScene = parsceneNo.cn_guid;
        //    string prorelationshipScene = parsceneNo.cn_s_parscene_prorelationship;
        //    List<tn_dts_chiscene> chisceneParsceList = new List<tn_dts_chiscene>();
        //    if (prorelationshipScene == "串行")
        //    {
        //        chisceneParsceList = _IChisceneService.GetWhere(it => it.cn_s_chiscene_parsceguid == guidScene).OrderBy(it => it.cn_n_chiscene_sequence).ToList();
        //    }
        //    else//并行
        //    {
        //        chisceneParsceList = _IChisceneService.GetWhere(it => it.cn_s_chiscene_parsceguid == guidScene).ToList();
        //    }
        //    int number = 1;
        //    int procedureOrder = 0;
        //    foreach (var chisceneParsce in chisceneParsceList)
        //    {
        //        string parproguid = chisceneParsce.cn_s_chiscene_parproguid;
        //        int procedureSequence = chisceneParsce.cn_n_chiscene_sequence;
        //        int procedureInterval = chisceneParsce.cn_n_chiscene_interval;
        //        if (prorelationshipScene == "串行")
        //        {
        //            procedureOrder = 1;
        //        }
        //        else//并行
        //        {
        //            procedureOrder++;
        //        }
        //        List<tn_dts_chiprocedure> chiprocedureParproList = _IChiprocedureService.GetWhere(it => it.cn_s_chiprocedure_parproguid == parproguid).OrderBy(it => it.cn_n_chiprocedure_sequence).ToList();
        //        foreach (var chiprocedureParpro in chiprocedureParproList)
        //        {
        //            string equiguid = chiprocedureParpro.cn_s_chiprocedure_equiguid;
        //            string chiproguid = chiprocedureParpro.cn_guid;
        //            int equipmentSequence = chiprocedureParpro.cn_n_chiprocedure_sequence;
        //            int equipmentInterval = chiprocedureParpro.cn_n_chiprocedure_interval;
        //            string startguid = chiprocedureParpro.cn_s_chiprocedure_startguid;
        //            string startStockCode = string.Empty;
        //            string startAreaCode = string.Empty;
        //            string startLocationCode = string.Empty;
        //            string startRowColLayer = string.Empty;
        //            if (chiprocedureParpro.cn_s_chiprocedure_startcategory == "立库")
        //            {
        //                startRowColLayer = chiprocedureParpro.cn_s_chiprocedure_startrcl;
        //            }
        //            string startLocationType = chiprocedureParpro.cn_s_chiprocedure_startcategory;
        //            tn_dts_equipment equipment = _IEquipmentService.GetWhere(it => it.cn_guid == equiguid).First();
        //            string equipmentNo = equipment.cn_s_equi_no;
        //            string equipmentName = equipment.cn_s_equi_name;
        //            if (chiprocedureParpro.cn_s_chiprocedure_startcategory == "地堆")
        //            {
        //                tn_dts_siteinfo siteinfo = _ISiteinfoService.GetWhere(it => it.cn_guid == startguid).First();
        //                var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "站点" && it.locationCode == siteinfo.cn_s_siteinfo_code);
        //                LocationSiteInformation siteMongo = MongoDBSingleton.Instance.FindOneFilter<LocationSiteInformation>(filter);
        //                if (siteMongo is null)
        //                {
        //                    result.IsSuccess = false;
        //                    result.ErrCode = 0;
        //                    result.Message = "MongoDB的LocationSiteInfo表中没有locationCode为：" + siteinfo.cn_s_siteinfo_code + "，请检查mdg后重试！";
        //                    return toResponse(result);
        //                }
        //                else
        //                {
        //                    startStockCode = siteMongo.stockCode;
        //                    startAreaCode = siteMongo.area_code;
        //                    startLocationCode = siteMongo.locationCode;
        //                }

        //            }
        //            else
        //            {
        //                tn_dts_stock3d stock3d = _IStock3dService.GetWhere(it => it.cn_guid == startguid).First();
        //                startStockCode = stock3d.cn_s_location_stockcode;
        //                startAreaCode = stock3d.cn_s_location_areacode;
        //            }
        //            string endguid = chiprocedureParpro.cn_s_chiprocedure_endguid;
        //            string endStockCode = string.Empty;
        //            string endAreaCode = string.Empty;
        //            string endLocationCode = string.Empty;
        //            string endRowColLayer = string.Empty;
        //            if (chiprocedureParpro.cn_s_chiprocedure_endcategory == "立库")
        //            {
        //                endRowColLayer = chiprocedureParpro.cn_s_chiprocedure_endrcl;
        //            }               
        //            string endLocationType = chiprocedureParpro.cn_s_chiprocedure_endcategory;
        //            if (chiprocedureParpro.cn_s_chiprocedure_endcategory == "地堆")
        //            {
        //                tn_dts_siteinfo siteinfoGuid = _ISiteinfoService.GetWhere(it => it.cn_guid == endguid).First();
        //                var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "站点" && it.locationCode == siteinfoGuid.cn_s_siteinfo_code);
        //                LocationSiteInformation siteMongo = MongoDBSingleton.Instance.FindOneFilter<LocationSiteInformation>(filter);
        //                if (siteMongo is null)
        //                {
        //                    result.IsSuccess = false;
        //                    result.ErrCode = 0;
        //                    result.Message = "MongoDB的LocationSiteInfo表中没有locationCode为：" + siteinfoGuid.cn_s_siteinfo_code + "，请检查mdg后重试！";
        //                    return toResponse(result);
        //                }
        //                else
        //                {
        //                    endStockCode = siteMongo.stockCode;
        //                    endAreaCode = siteMongo.area_code;
        //                    endLocationCode = siteMongo.locationCode;
        //                }
        //            }
        //            else
        //            {
        //                tn_dts_stock3d stock3dGuid = _IStock3dService.GetWhere(it => it.cn_guid == endguid).First();
        //                endStockCode = stock3dGuid.cn_s_location_stockcode;
        //                endAreaCode = stock3dGuid.cn_s_location_areacode;
        //            }
        //            List<tn_dts_chiequipment> chiequipmentEquiList = _IChiequipmentService.GetWhere(it => it.cn_s_chiequipment_equiguid == chiproguid).OrderBy(it => it.cn_n_chiequipment_sequence).ToList();
        //            //List<tn_dts_goodscommand> goodscommandList = _IChiequipmentService.GetGoodscommand(chiproguid, ref startRowColLayer, ref endRowColLayer);
        //            foreach (var chiequipmentEqui in chiequipmentEquiList)
        //            {
        //                if (chiequipmentEqui.cn_s_chiequipment_type == "设备指令管理")
        //                {
        //                    continue;
        //                }
        //                string comguid = chiequipmentEqui.cn_s_chiequipment_comguid;
        //                int commandSequence = chiequipmentEqui.cn_n_chiequipment_sequence;
        //                int commandInterval = chiequipmentEqui.cn_n_chiequipment_interval;
        //                string json = string.Empty;
        //                if (chiequipmentEqui.cn_s_chiequipment_category == "设备")
        //                {
        //                    tn_dts_equicommand equicommandGuid = _IEquicommandService.GetWhere(it => it.cn_guid == comguid).First();
        //                    json = equicommandGuid.cn_s_equicommand_json;
        //                    if (equicommandGuid.cn_n_equicommand_haswildcard == 1)
        //                    {
        //                        json = json.Replace("{4}", equipmentName).Replace("{5}", equipmentNo).Replace("{6}", startLocationCode).Replace("{7}", endLocationCode)
        //                        .Replace("{8.1}", startStockCode + "-" + startAreaCode + "-" + startRowColLayer).Replace("{8.2}", startStockCode + "-" + startAreaCode + "-" + endRowColLayer)
        //                        .Replace("{9.1}", endStockCode + "-" + endAreaCode + "-" + startRowColLayer).Replace("{9.2}", endStockCode + "-" + endAreaCode + "-" + endRowColLayer);
        //                    }
        //                }
        //                else//货位
        //                {
        //                    tn_dts_goodscommand goodscommandGuid = _IGoodscommandService.GetWhere(it => it.cn_guid == comguid).First();
        //                    json = goodscommandGuid.cn_s_goodscommand_json;

        //                    if (goodscommandGuid.cn_s_goodscommand_type == "执行" && goodscommandGuid.cn_n_goodscommand_haswildcard == 1)
        //                    {
        //                        json = json.Replace("{0.1}", startStockCode).Replace("{0.2}", endStockCode).Replace("{1.1}", startAreaCode).Replace("{1.2}", endAreaCode)
        //                            //.Replace("{2}", goodscommandGuid.cn_s_goodscommand_rowcollayer)
        //                            .Replace("{3.1}", startLocationType).Replace("{3.2}", endLocationType);
        //                    }

        //                    tn_dts_goodscommand updateGoodsCommand = new tn_dts_goodscommand();
        //                    if (model.IsSynchronizeStock && _IGoodscommandService.DetermineSynchronizeExist(model.Goodsequipmentno, json, ref updateGoodsCommand) != 0)
        //                    {
        //                        int synchronizeResult = _IGoodscommandService.DetermineSynchronizeExist(model.Goodsequipmentno, json, ref updateGoodsCommand);
        //                        if (synchronizeResult == 3)
        //                        {
        //                            result.IsSuccess = false;
        //                            result.ErrCode = 0;
        //                            result.Message = "tn_dts_goodscommand表中货位设备编码为：" + model.Goodsequipmentno + "的货位指令中有指令Json被损坏，请检查后重试！";
        //                            return toResponse(result);
        //                        }
        //                        SaveGoodscommand saveGoodscommand = new SaveGoodscommand();
        //                        if (synchronizeResult == 1)
        //                        {
        //                            tn_dts_goodsequipment goodsequipmentNo = _IGoodsequipmentService.GetWhere(it => it.cn_s_goodsequipment_no == model.Goodsequipmentno).First();
        //                            saveGoodscommand.AddOrModify = "add";
        //                            tn_dts_goodscommand newGoodscommand = new tn_dts_goodscommand();
        //                            newGoodscommand.cn_guid = Guid.NewGuid().ToString();
        //                            newGoodscommand.cn_s_goodscommand_goodsequipguid = goodsequipmentNo.cn_guid;
        //                            newGoodscommand.cn_s_goodscommand_no = newGoodscommand.cn_guid.Substring(0,32);
        //                            newGoodscommand.cn_s_goodscommand_name = goodscommandGuid.cn_s_goodscommand_name;
        //                            newGoodscommand.cn_s_goodscommand_type = "业务";
        //                            //newGoodscommand.cn_n_goodscommand_haswildcard = 0;
        //                            newGoodscommand.cn_s_goodscommand_json = json;
        //                            newGoodscommand.cn_s_creator = user.UserCode;
        //                            newGoodscommand.cn_s_creator_by = user.UserName;
        //                            newGoodscommand.cn_t_create = DateTime.Now;
        //                            saveGoodscommand.NewGoodscommand = newGoodscommand;
        //                        }
        //                        else//2
        //                        {
        //                            saveGoodscommand.AddOrModify = "modify";
        //                            tn_dts_goodscommand goodscommand = updateGoodsCommand;
        //                            goodscommand.cn_s_goodscommand_json = json;
        //                            goodscommand.cn_s_modify = user.UserCode;
        //                            goodscommand.cn_s_modify_by = user.UserName;
        //                            goodscommand.cn_t_modify = DateTime.Now;
        //                            saveGoodscommand.NewGoodscommand = goodscommand;
        //                        }

        //                        ReturnMessage returnMessage = _IGoodscommandService.SaveGoodscommand(saveGoodscommand);
        //                        if (!returnMessage.IsSuccess)
        //                        {
        //                            result.IsSuccess = false;
        //                            result.ErrCode = 0;
        //                            result.Message = "使用货位同步功能缓存货位数据失败，详细信息：" + returnMessage.Message;
        //                            return toResponse(result);
        //                        }
        //                    }
        //                }
        //                MongoCommand mongoCommand = new MongoCommand();
        //                mongoCommand.number = number++;
        //                mongoCommand.procedureOrder = procedureOrder;
        //                mongoCommand.commandJson = json;
        //                mongoCommand.interval = commandInterval;
        //                MongoDBSingleton.Instance.Add<MongoCommand>(mongoCommand);
        //            }
        //            var filterEqui = Builders<MongoCommand>.Filter.Where(it => it.number == (number - 1));
        //            List<MongoCommand> equiMongoCommandList = MongoDBSingleton.Instance.FindList<MongoCommand>(filterEqui);
        //            if(equiMongoCommandList.Count > 0)
        //            {
        //                MongoCommand equiMongoCommand = equiMongoCommandList[0];
        //                equiMongoCommand.interval = equipmentInterval;
        //                MongoDBSingleton.Instance.UpdateFilter(equiMongoCommand, filterEqui);
        //            }    
        //        }
        //        var filterPro = Builders<MongoCommand>.Filter.Where(it => it.number == (number - 1));
        //        List<MongoCommand> proMongoCommandList = MongoDBSingleton.Instance.FindList<MongoCommand>(filterPro);
        //        if(proMongoCommandList.Count > 0)
        //        {
        //            MongoCommand proMongoCommand = proMongoCommandList[0];
        //            proMongoCommand.interval = procedureInterval;
        //            MongoDBSingleton.Instance.UpdateFilter(proMongoCommand, filterPro);
        //        }       
        //    }

        //    if (prorelationshipScene == "串行")
        //    {
        //        var filterAll = Builders<MongoCommand>.Filter.Where(it => it.number != 0);
        //        List<MongoCommand> mongoCommandList = MongoDBSingleton.Instance.FindList(filterAll).OrderBy(it => it.number).ToList();
        //        foreach (var mongoCommand in mongoCommandList)
        //        {
        //            Task.Run(async () => { WebSocketServer.SessionInstance.Instance.PLCSendAll(mongoCommand.commandJson); });
        //            Thread.Sleep(mongoCommand.interval);
        //        }
        //    }
        //    else//并行
        //    {
        //        for (int i = 1; i <= chisceneParsceList.Count; i++)
        //        {
        //            var filterPro = Builders<MongoCommand>.Filter.Where(it => it.procedureOrder == i);
        //            List<MongoCommand> mongoCommandProList = MongoDBSingleton.Instance.FindList(filterPro).OrderBy(it => it.number).ToList();
        //            ParameterizedThreadStart threadStart = mongoCommandList =>
        //            {
        //                foreach (var mongoCommand in mongoCommandList as List<MongoCommand>)
        //                {
        //                    Task.Run(async () => { WebSocketServer.SessionInstance.Instance.PLCSendAll(mongoCommand.commandJson); });
        //                    Thread.Sleep(mongoCommand.interval);
        //                }
        //            };
        //            Thread thread = new Thread(threadStart);
        //            thread.Start(mongoCommandProList);
        //        }
        //    }
        //    result.IsSuccess = true;
        //    result.ErrCode = 200;
        //    result.Message = "成功！";
        //    return toResponse(result);
        //}
        #endregion




        #region 获取连接列表
        /// <summary>
        /// 获取连接列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetCollectionList()
        {
            List<string> collectionList = new List<string>();
            collectionList.Add("连接IP1:port");
            collectionList.Add("连接IP2:port");
            collectionList.Add("连接IP3:port");
            return toResponse(collectionList);
        }
        #endregion

        #region 读取向指定IP进行WebSocket推送信息
        /// <summary>
        /// 读取向指定IP进行WebSocket推送信息
        /// </summary>
        /// <param name="collctionipport"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPostInformation([FromBody] string collctionipport)
        {
            List<string> postInformationList = new List<string>();
            if (collctionipport == "连接IP1:port")
            {
                postInformationList.Add("发送消息1");
                postInformationList.Add("发送消息2");
                postInformationList.Add("发送消息3");
            }
            else if (collctionipport == "连接IP2:port")
            {
                postInformationList.Add("发送消息1");
                postInformationList.Add("发送消息2");
                postInformationList.Add("发送消息3");
            }
            else if (collctionipport == "连接IP3:port")
            {
                postInformationList.Add("发送消息1");
                postInformationList.Add("发送消息2");
                postInformationList.Add("发送消息3");
            }
            else
            {
                return toResponse(new ApiResult()
                {
                    IsSuccess = false,
                    StatusCode = (int)StatusCodeType.Error,
                    Message = "传入的IP端口号不在连接列表内，请检查后重试！"
                });
            }
            return toResponse(postInformationList);
        }
        #endregion

        //#region 开始测试
        ///// <summary>
        ///// 开始测试
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult StartTest(StartTestModel model)
        //{
        //    string sceneNo = model.Sceneno;
        //    if (_IParsceneService.GetWhere(it => it.cn_s_parscene_no == sceneNo).Count == 0)
        //    {
        //        return toResponse(new ApiResult()
        //        {
        //            IsSuccess = false,
        //            StatusCode = (int)StatusCodeType.Error,
        //            Message = "传入的场景编码不在数据库中，请检查后重试！"
        //        });
        //    }
        //    if (model.IsSynchronizeStock == true && _IGoodsequipmentService.GetWhere(it => it.cn_s_goodsequipment_no == model.Goodsequipmentno).Count == 0)
        //    {
        //        return toResponse(new ApiResult()
        //        {
        //            IsSuccess = false,
        //            StatusCode = (int)StatusCodeType.Error,
        //            Message = "传入的货位设备编码不在数据库中，请检查后重试！"
        //        });
        //    }
        //    if (model.ClientIP != "连接IP1:port" && model.ClientIP != "连接IP2:port" && model.ClientIP != "连接IP3:port")
        //    {
        //        return toResponse(new ApiResult()
        //        {
        //            IsSuccess = false,
        //            StatusCode = (int)StatusCodeType.Error,
        //            Message = "传入的IP端口号不在连接列表内，请检查后重试！"
        //        });
        //    }
        //    return toResponse(new ApiResult()
        //    {
        //        IsSuccess = true,
        //        StatusCode = (int)StatusCodeType.Success,
        //        Message = "数据成功发送！"
        //    });
        //}
        //#endregion

        #region 停止测试
        /// <summary>
        /// 停止测试
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //public IActionResult StopTest()
        //{
        //    return toResponse(new ApiResult()
        //    {
        //        IsSuccess = true,
        //        StatusCode = (int)StatusCodeType.Success,
        //        Message = "已停止推送！"
        //    });
        //}
        #endregion

        #region 获取测试信息
        ///// <summary>
        ///// 获取测试信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult GetTestInfo()
        //{
        //    TestInfo testInfo = new TestInfo()
        //    {
        //        Sceneno = SenarioTestingProcess.Sceneno,
        //        IsSynchronizeStock = SenarioTestingProcess.IsSynchronizeStock,
        //        IsCirculate = SenarioTestingProcess.IsCirculate,
        //        Goodsequipmentno = SenarioTestingProcess.Goodsequipmentno,
        //        IsExit = SenarioTestingProcess.IsExitLoop
        //    };
        //    return toResponse(testInfo);
        //}
        #endregion
    }
}
