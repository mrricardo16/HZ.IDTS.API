using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces.Service.Equipment;
using HZ.IDTSCore.Interfaces.Service.SenarioTesting;
using HZ.IDTSCore.Interfaces.Service.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.iWCS.MData.Core;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Global
{
    public class SenarioTestingProcess
    {
        public static List<Task> _startTaskList;
        private static ParsceneService _parsceneService;
        private static ChisceneService _chisceneService;
        private static ChiprocedureService _chiprocedureService;
        private static EquipmentService _equipmentService;
        private static SiteinfoService _siteinfoService;
        private static Stock3dService _stock3dService;
        private static ChiequipmentService _chiequipmentService;
        private static EquicommandService _equicommandService;
        private static GoodscommandService _goodscommandService;
        private static GoodsequipmentService _goodsequipmentService;
        public static ManualResetEvent manualResetEvent;
        public static CancellationTokenSource tokensource;
        /// <summary>
        /// 是否退出循环推送
        /// </summary>
        public static bool IsExitLoop;
        /// <summary>
        /// （循环推送）是否执行过货位同步
        /// </summary>
        public static bool HasSynchronizeStock;
        ///// <summary>
        ///// 场景编码
        ///// </summary>
        //public static string Sceneno;
        ///// <summary>
        ///// 是否货位同步
        ///// </summary>
        //public static bool IsSynchronizeStock;
        ///// <summary>
        ///// 是否循环推送
        ///// </summary>
        //public static bool IsCirculate;
        ///// <summary>
        ///// 货位设备编码
        ///// </summary>
        //public static string Goodsequipmentno;

        static SenarioTestingProcess()
        {
            _parsceneService = new Interfaces.Service.SenarioTesting.ParsceneService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _chisceneService = new Interfaces.Service.SenarioTesting.ChisceneService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _chiprocedureService = new Interfaces.Service.SenarioTesting.ChiprocedureService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _equipmentService = new Interfaces.Service.Equipment.EquipmentService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _siteinfoService = new Interfaces.Service.Sys.SiteinfoService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _stock3dService = new Interfaces.Service.Sys.Stock3dService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _chiequipmentService = new Interfaces.Service.SenarioTesting.ChiequipmentService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _equicommandService = new Interfaces.Service.SenarioTesting.EquicommandService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _goodscommandService = new Interfaces.Service.SenarioTesting.GoodscommandService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _goodsequipmentService = new Interfaces.Service.SenarioTesting.GoodsequipmentService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            _startTaskList = new List<Task>();
            manualResetEvent = new ManualResetEvent(true);
            tokensource = new CancellationTokenSource();
            IsExitLoop = true;
            HasSynchronizeStock = false;
            //Sceneno = "";
            //IsSynchronizeStock = false;
            //IsCirculate = false;
            //Goodsequipmentno = "";
        }

        #region 开始测试
        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ReturnMessageViewModel StartTesting(StartTestingViewModel data)
        {
            ReturnMessageViewModel result = new ReturnMessageViewModel();
            result.ReturnMessage = new List<ReturnMessageItem>();
            StartTestingItem model = data.StartTesting[0];
            if (model.IsSynchronizeStock && !HasSynchronizeStock)
            {
                ReturnMessage returnMessage = GoodscommandDriver.Instance.SynchronizeStock(model.Goodsequipmentno);
                if (!returnMessage.IsSuccess)
                {
                    ReturnMessageItem returnMessageItem = new ReturnMessageItem();
                    returnMessageItem.Source = "start";
                    returnMessageItem.IsSuccess = false;
                    returnMessageItem.Message = returnMessage.Message;
                    result.ReturnMessage.Add(returnMessageItem);
                    return result;
                }
                HasSynchronizeStock = true;
            }
            MongoDBSingleton.Instance.DeleteAll<MongoCommand>();

            tn_dts_parscene parsceneNo = _parsceneService.GetFirst(it => it.cn_s_parscene_no == model.Sceneno);
            if (parsceneNo is null)
            {
                ReturnMessageItem returnMessageItem = new ReturnMessageItem();
                returnMessageItem.Source = "start";
                returnMessageItem.IsSuccess = false;
                returnMessageItem.Message = "tn_dts_parscene里没有场景编码为：" + model.Sceneno + "的场景记录，请重试！";
                result.ReturnMessage.Add(returnMessageItem);
                return result;
            }
            string guidScene = parsceneNo.cn_guid;
            string prorelationshipScene = parsceneNo.cn_s_parscene_prorelationship;//流程关系
            List<tn_dts_chiscene> chisceneParsceList = new List<tn_dts_chiscene>();
            if (prorelationshipScene == "串行")
            {
                chisceneParsceList = _chisceneService.GetWhere(it => it.cn_s_chiscene_parsceguid == guidScene).OrderBy(it => it.cn_n_chiscene_sequence).ToList();
            }
            else//并行
            {
                chisceneParsceList = _chisceneService.GetWhere(it => it.cn_s_chiscene_parsceguid == guidScene).ToList();
            }
            int number = 1;
            int procedureOrder = 0;
            foreach (var chisceneParsce in chisceneParsceList)
            {
                DateTime now = DateTime.Now;
                string taskNo = now.Hour.ToString("D2") + now.Minute.ToString("D2") + now.Second.ToString("D2") + now.Millisecond.ToString("D3");
                string parproguid = chisceneParsce.cn_s_chiscene_parproguid;//流程唯一标识
                int procedureSequence = chisceneParsce.cn_n_chiscene_sequence;//流程次序
                int procedureInterval = chisceneParsce.cn_n_chiscene_interval;//流程间隔
                if (prorelationshipScene == "串行")
                {
                    procedureOrder = 1;
                }
                else//并行
                {
                    procedureOrder++;
                }
                List<tn_dts_chiprocedure> chiprocedureParproList = _chiprocedureService.GetWhere(it => it.cn_s_chiprocedure_parproguid == parproguid).OrderBy(it => it.cn_n_chiprocedure_sequence).ToList();
                foreach (var chiprocedureParpro in chiprocedureParproList)
                {

                    string equiguid = chiprocedureParpro.cn_s_chiprocedure_equiguid;//设备档案唯一标识
                    string chiproguid = chiprocedureParpro.cn_guid;
                    int equipmentSequence = chiprocedureParpro.cn_n_chiprocedure_sequence;//设备次序
                    int equipmentInterval = chiprocedureParpro.cn_n_chiprocedure_interval;//设备间隔
                    string startguid = chiprocedureParpro.cn_s_chiprocedure_startguid;//起点唯一标识
                    string startStockCode = string.Empty;
                    string startAreaCode = string.Empty;
                    string startLocationCode = string.Empty;
                    string startRowColLayer = string.Empty;//起点排列层
                    if (chiprocedureParpro.cn_s_chiprocedure_startcategory == "立库")
                    {
                        startRowColLayer = chiprocedureParpro.cn_s_chiprocedure_startrcl;
                    }
                    string startLocationType = chiprocedureParpro.cn_s_chiprocedure_startcategory;//起点类别（立库/地堆）
                    tn_dts_equipment equipment = _equipmentService.GetWhere(it => it.cn_guid == equiguid).First();
                    string equipmentNo = equipment.cn_s_equi_no;//设备编码
                    string equipmentName = equipment.cn_s_equi_name;//设备名称
                    if (chiprocedureParpro.cn_s_chiprocedure_startcategory == "地堆")
                    {
                        tn_dts_siteinfo siteinfo = _siteinfoService.GetWhere(it => it.cn_guid == startguid).First();
                        var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "站点" && it.locationCode == siteinfo.cn_s_siteinfo_code);
                        LocationSiteInformation siteMongo = MongoDBSingleton.Instance.FindOneFilter<LocationSiteInformation>(filter);
                        if (siteMongo is null)
                        {
                            ReturnMessageItem returnMessageItem = new ReturnMessageItem();
                            returnMessageItem.Source = "start";
                            returnMessageItem.IsSuccess = false;
                            returnMessageItem.Message = "MongoDB的LocationSiteInfo表中没有locationCode为：" + siteinfo.cn_s_siteinfo_code + "，请检查mdg后重试！";
                            result.ReturnMessage.Add(returnMessageItem);
                            return result;
                        }
                        else
                        {
                            startStockCode = siteMongo.stockCode;//仓库编码
                            startAreaCode = siteMongo.area_code;//库区编码
                            startLocationCode = siteMongo.locationCode;//货位编码
                        }

                    }
                    else
                    {
                        tn_dts_stock3d stock3d = _stock3dService.GetWhere(it => it.cn_guid == startguid).First();
                        startStockCode = stock3d.cn_s_location_stockcode;// 仓库编码
                        startAreaCode = stock3d.cn_s_location_areacode;//货位编码
                    }
                    string endguid = chiprocedureParpro.cn_s_chiprocedure_endguid;
                    string endStockCode = string.Empty;
                    string endAreaCode = string.Empty;
                    string endLocationCode = string.Empty;
                    string endRowColLayer = string.Empty;
                    if (chiprocedureParpro.cn_s_chiprocedure_endcategory == "立库")
                    {
                        endRowColLayer = chiprocedureParpro.cn_s_chiprocedure_endrcl;
                    }
                    string endLocationType = chiprocedureParpro.cn_s_chiprocedure_endcategory;
                    if (chiprocedureParpro.cn_s_chiprocedure_endcategory == "地堆")
                    {
                        tn_dts_siteinfo siteinfoGuid = _siteinfoService.GetWhere(it => it.cn_guid == endguid).First();
                        var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "站点" && it.locationCode == siteinfoGuid.cn_s_siteinfo_code);
                        LocationSiteInformation siteMongo = MongoDBSingleton.Instance.FindOneFilter<LocationSiteInformation>(filter);
                        if (siteMongo is null)
                        {
                            ReturnMessageItem returnMessageItem = new ReturnMessageItem();
                            returnMessageItem.Source = "start";
                            returnMessageItem.IsSuccess = false;
                            returnMessageItem.Message = "MongoDB的LocationSiteInfo表中没有locationCode为：" + siteinfoGuid.cn_s_siteinfo_code + "，请检查mdg后重试！";
                            result.ReturnMessage.Add(returnMessageItem);
                            return result;
                        }
                        else
                        {
                            endStockCode = siteMongo.stockCode;
                            endAreaCode = siteMongo.area_code;
                            endLocationCode = siteMongo.locationCode;
                        }
                    }
                    else
                    {
                        tn_dts_stock3d stock3dGuid = _stock3dService.GetWhere(it => it.cn_guid == endguid).First();
                        endStockCode = stock3dGuid.cn_s_location_stockcode;
                        endAreaCode = stock3dGuid.cn_s_location_areacode;
                    }
                    List<tn_dts_chiequipment> chiequipmentEquiList = _chiequipmentService.GetWhere(it => it.cn_s_chiequipment_equiguid == chiproguid).OrderBy(it => it.cn_n_chiequipment_sequence).ToList();
                    //List<tn_dts_goodscommand> goodscommandList = _IChiequipmentService.GetGoodscommand(chiproguid, ref startRowColLayer, ref endRowColLayer);
                    foreach (var chiequipmentEqui in chiequipmentEquiList)
                    {
                        if (chiequipmentEqui.cn_s_chiequipment_type == "设备指令管理")
                        {
                            continue;
                        }
                        string comguid = chiequipmentEqui.cn_s_chiequipment_comguid;
                        int commandSequence = chiequipmentEqui.cn_n_chiequipment_sequence;
                        int commandInterval = chiequipmentEqui.cn_n_chiequipment_interval;
                        string json = string.Empty;
                        if (chiequipmentEqui.cn_s_chiequipment_category == "设备")
                        {
                            tn_dts_equicommand equicommandGuid = _equicommandService.GetWhere(it => it.cn_guid == comguid).First();
                            json = equicommandGuid.cn_s_equicommand_json;
                            if (equicommandGuid.cn_n_equicommand_haswildcard == 1)
                            {
                                json = json.Replace("{4}", equipmentName).Replace("{5}", equipmentNo).Replace("{6}", startLocationCode).Replace("{7}", endLocationCode)
                                .Replace("{8.1}", startStockCode + "-" + startAreaCode + "-" + startRowColLayer).Replace("{8.2}", startStockCode + "-" + startAreaCode + "-" + endRowColLayer)
                                .Replace("{9.1}", endStockCode + "-" + endAreaCode + "-" + startRowColLayer).Replace("{9.2}", endStockCode + "-" + endAreaCode + "-" + endRowColLayer)
                                .Replace("{TaskNo}", taskNo);
                            }

                        }
                        else//货位
                        {
                            tn_dts_goodscommand goodscommandGuid = _goodscommandService.GetWhere(it => it.cn_guid == comguid).First();
                            json = goodscommandGuid.cn_s_goodscommand_json;

                            if (goodscommandGuid.cn_s_goodscommand_type == "执行" && goodscommandGuid.cn_n_goodscommand_haswildcard == 1)
                            {
                                json = json.Replace("{0.1}", startStockCode).Replace("{0.2}", endStockCode).Replace("{1.1}", startAreaCode).Replace("{1.2}", endAreaCode)
                                    //.Replace("{2}", goodscommandGuid.cn_s_goodscommand_rowcollayer)
                                    .Replace("{3.1}", startLocationType).Replace("{3.2}", endLocationType);
                            }

                            tn_dts_goodscommand updateGoodsCommand = new tn_dts_goodscommand();
                            if (model.IsSynchronizeStock && _goodscommandService.DetermineSynchronizeExist(model.Goodsequipmentno, json, ref updateGoodsCommand) != 0)
                            {
                                int synchronizeResult = _goodscommandService.DetermineSynchronizeExist(model.Goodsequipmentno, json, ref updateGoodsCommand);
                                if (synchronizeResult == 3)
                                {
                                    ReturnMessageItem returnMessageItem = new ReturnMessageItem();
                                    returnMessageItem.IsSuccess = true;
                                    returnMessageItem.Message = "tn_dts_goodscommand表中货位设备编码为：" + model.Goodsequipmentno + "的货位指令中有指令Json被损坏，请检查后重试！";
                                    result.ReturnMessage.Add(returnMessageItem);
                                    return result;
                                }
                                SaveGoodscommand saveGoodscommand = new SaveGoodscommand();
                                if (synchronizeResult == 1)
                                {
                                    tn_dts_goodsequipment goodsequipmentNo = _goodsequipmentService.GetWhere(it => it.cn_s_goodsequipment_no == model.Goodsequipmentno).First();
                                    saveGoodscommand.AddOrModify = "add";
                                    tn_dts_goodscommand newGoodscommand = new tn_dts_goodscommand();
                                    newGoodscommand.cn_guid = Guid.NewGuid().ToString();
                                    newGoodscommand.cn_s_goodscommand_goodsequipguid = goodsequipmentNo.cn_guid;
                                    newGoodscommand.cn_s_goodscommand_no = newGoodscommand.cn_guid.Substring(0, 32);
                                    newGoodscommand.cn_s_goodscommand_name = goodscommandGuid.cn_s_goodscommand_name;
                                    newGoodscommand.cn_s_goodscommand_type = "业务";
                                    //newGoodscommand.cn_n_goodscommand_haswildcard = 0;
                                    newGoodscommand.cn_s_goodscommand_json = json;
                                    newGoodscommand.cn_s_creator = "IDTS_SenarioTesting";
                                    newGoodscommand.cn_s_creator_by = "数字孪生_场景测试";
                                    newGoodscommand.cn_t_create = DateTime.Now;
                                    saveGoodscommand.NewGoodscommand = newGoodscommand;
                                }
                                else//2
                                {
                                    saveGoodscommand.AddOrModify = "modify";
                                    tn_dts_goodscommand goodscommand = updateGoodsCommand;
                                    goodscommand.cn_s_goodscommand_json = json;
                                    goodscommand.cn_s_modify = "IDTS_SenarioTesting";
                                    goodscommand.cn_s_modify_by = "数字孪生_场景测试";
                                    goodscommand.cn_t_modify = DateTime.Now;
                                    saveGoodscommand.NewGoodscommand = goodscommand;
                                }

                                ReturnMessage returnMessage = _goodscommandService.SaveGoodscommand(saveGoodscommand);
                                if (!returnMessage.IsSuccess)
                                {
                                    ReturnMessageItem returnMessageItem = new ReturnMessageItem();
                                    returnMessageItem.IsSuccess = true;
                                    returnMessageItem.Message = "使用货位同步功能缓存货位数据失败，详细信息：" + returnMessage.Message;
                                    result.ReturnMessage.Add(returnMessageItem);
                                    return result;
                                }
                            }
                        }
                        MongoCommand mongoCommand = new MongoCommand();
                        mongoCommand.number = number++;
                        mongoCommand.procedureOrder = procedureOrder;
                        mongoCommand.commandJson = json;
                        mongoCommand.interval = commandInterval;
                        MongoDBSingleton.Instance.Add<MongoCommand>(mongoCommand);
                    }
                    var filterEqui = Builders<MongoCommand>.Filter.Where(it => it.number == (number - 1));
                    List<MongoCommand> equiMongoCommandList = MongoDBSingleton.Instance.FindList<MongoCommand>(filterEqui);
                    if (equiMongoCommandList.Count > 0)
                    {
                        MongoCommand equiMongoCommand = equiMongoCommandList[0];
                        equiMongoCommand.interval = equipmentInterval;
                        MongoDBSingleton.Instance.UpdateFilter(equiMongoCommand, filterEqui);
                    }
                }

                var filterPro = Builders<MongoCommand>.Filter.Where(it => it.number == (number - 1));
                List<MongoCommand> proMongoCommandList = MongoDBSingleton.Instance.FindList<MongoCommand>(filterPro);
                if (proMongoCommandList.Count > 0)
                {
                    MongoCommand proMongoCommand = proMongoCommandList[0];
                    proMongoCommand.interval = procedureInterval;
                    MongoDBSingleton.Instance.UpdateFilter(proMongoCommand, filterPro);
                }
            }

            //if (model.IsCirculate)
            //{
            //    while(true)
            //    {
            //        ExecuteWebSocket(prorelationshipScene, chisceneParsceList);
            //        Task.WaitAll(_startTaskList.ToArray());
            //        _startTaskList.Clear();
            //    }
            //}
            if (prorelationshipScene == "串行")
            {
                var filterAll = Builders<MongoCommand>.Filter.Where(it => it.number != 0);
                List<MongoCommand> mongoCommandList = MongoDBSingleton.Instance.FindList(filterAll).OrderBy(it => it.number).ToList();

                Task task = Task.Run(async () =>
                    AsyncWebSocket(mongoCommandList, tokensource, manualResetEvent)
                );
                _startTaskList.Add(task);
            }
            else//并行
            {
                for (int i = 1; i <= chisceneParsceList.Count; i++)
                {
                    var filterPro = Builders<MongoCommand>.Filter.Where(it => it.procedureOrder == i);
                    List<MongoCommand> mongoCommandProList = MongoDBSingleton.Instance.FindList(filterPro).OrderBy(it => it.number).ToList();
                    Task task = Task.Run(async () =>
                        AsyncWebSocket(mongoCommandProList, tokensource, manualResetEvent)
                    );
                    _startTaskList.Add(task);
                }
            }
            //ExecuteWebSocket(prorelationshipScene, chisceneParsceList);
            Task.WaitAll(_startTaskList.ToArray());
            ReturnMessageItem returnMessageItemEnd = new ReturnMessageItem();
            returnMessageItemEnd.Source = "start";
            returnMessageItemEnd.IsSuccess = true;
            returnMessageItemEnd.Message = "成功！";
            //returnMessageItemEnd.IsSuccess = false;
            //returnMessageItemEnd.Message = "自定义错误！";
            result.ReturnMessage.Add(returnMessageItemEnd);
            return result;
        }
        #endregion

        //private static void ExecuteWebSocket(string prorelationshipScene, List<tn_dts_chiscene> chisceneParsceList)
        //{
        //    if (prorelationshipScene == "串行")
        //    {
        //        var filterAll = Builders<MongoCommand>.Filter.Where(it => it.number != 0);
        //        List<MongoCommand> mongoCommandList = MongoDBSingleton.Instance.FindList(filterAll).OrderBy(it => it.number).ToList();

        //        Task task = Task.Run(async () =>
        //            AsyncWebSocket(mongoCommandList, tokensource, manualResetEvent)
        //        );
        //        _startTaskList.Add(task);
        //    }
        //    else//并行
        //    {
        //        for (int i = 1; i <= chisceneParsceList.Count; i++)
        //        {
        //            var filterPro = Builders<MongoCommand>.Filter.Where(it => it.procedureOrder == i);
        //            List<MongoCommand> mongoCommandProList = MongoDBSingleton.Instance.FindList(filterPro).OrderBy(it => it.number).ToList();
        //            Task task = Task.Run(async () =>
        //                AsyncWebSocket(mongoCommandProList, tokensource, manualResetEvent)
        //            );
        //            _startTaskList.Add(task);
        //        }
        //    }
        //}

        #region 广播指令
        /// <summary>
        /// 广播指令
        /// </summary>
        /// <param name="mongoCommandList"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="manualResetEvent"></param>
        private static void AsyncWebSocket(List<MongoCommand> mongoCommandList, CancellationTokenSource cancellationToken, ManualResetEvent manualResetEvent)
        {
            foreach (var mongoCommand in mongoCommandList)
            {
                //Task task = Task.Run(async () => { WebSocketServer.SessionInstance.Instance.PLCSendAll(mongoCommand.commandJson); }) ;
                //_startTaskList.Add(task);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                manualResetEvent.WaitOne();
                WebSocketServer.SessionInstance.Instance.PLCSendAll(mongoCommand.commandJson);
                Thread.Sleep(mongoCommand.interval);
            }
        }
        #endregion

        /// <summary>
        /// 广播货位
        /// </summary>
        /// <param name="locationRealMonitorViewModelList"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="manualResetEvent"></param>
        /// <param name="interval"></param>
        public static void AsyncWebSocketStock(List<LocationRealMonitorViewModel> locationRealMonitorViewModelList, CancellationTokenSource cancellationToken, ManualResetEvent manualResetEvent, int interval)
        {
            foreach (var locationRealMonitorViewModel in locationRealMonitorViewModelList)
            {
                //Task task = Task.Run(async () => { WebSocketServer.SessionInstance.Instance.PLCSendAll(mongoCommand.commandJson); }) ;
                //_startTaskList.Add(task);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                manualResetEvent.WaitOne();
                string sendJSONString = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                Thread.Sleep(interval);
            }
        }

        #region 停止测试
        /// <summary>
        /// 停止测试
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ReturnMessageViewModel StopTesting(StopTestingViewModel data)
        {
            ReturnMessageViewModel result = new ReturnMessageViewModel();
            result.ReturnMessage = new List<ReturnMessageItem>();
            tokensource.Cancel();
            SenarioTestingProcess.manualResetEvent = new ManualResetEvent(true);
            SenarioTestingProcess.tokensource = new CancellationTokenSource();
            ReturnMessageItem returnMessageItem = new ReturnMessageItem();
            returnMessageItem.Source = "stop";
            returnMessageItem.IsSuccess = true;
            returnMessageItem.Message = "成功！";
            result.ReturnMessage.Add(returnMessageItem);
            return result;
        }
        #endregion

        #region 暂停测试
        /// <summary>
        /// 暂停测试
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ReturnMessageViewModel PauseTesting(PauseTestingViewModel data)
        {
            ReturnMessageViewModel result = new ReturnMessageViewModel();
            result.ReturnMessage = new List<ReturnMessageItem>();
            manualResetEvent.Reset();
            ReturnMessageItem returnMessageItem = new ReturnMessageItem();
            returnMessageItem.Source = "pause";
            returnMessageItem.IsSuccess = true;
            returnMessageItem.Message = "成功！";
            result.ReturnMessage.Add(returnMessageItem);
            return result;
        }
        #endregion

        #region 继续测试
        /// <summary>
        /// 继续测试
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ReturnMessageViewModel ContinueTesting(ContinueTestingViewModel data)
        {
            ReturnMessageViewModel result = new ReturnMessageViewModel();
            result.ReturnMessage = new List<ReturnMessageItem>();
            manualResetEvent.Set();
            ReturnMessageItem returnMessageItem = new ReturnMessageItem();
            returnMessageItem.Source = "continue";
            returnMessageItem.IsSuccess = true;
            returnMessageItem.Message = "成功！";
            result.ReturnMessage.Add(returnMessageItem);
            return result;
        }
        #endregion

    }
}
