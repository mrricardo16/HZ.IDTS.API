using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.SenarioTesting
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class GoodscommandController : BaseController
    {
        private IGoodscommandService _IGoodscommandService;
        private IGoodsequipmentService _IGoodsequipmentService;

        public GoodscommandController()
        {
            _IGoodscommandService = ServiceLocator.GetService<IGoodscommandService>(HttpContextSession());
            _IGoodsequipmentService = ServiceLocator.GetService<IGoodsequipmentService>(HttpContextSession());
        }

        #region 获取一个货位设备类型下所有货位设备信息
        /// <summary>
        /// 获取一个货位设备类型下所有货位设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquipmentInformationByEquitype(PageParm param)
        {
            var res = _IGoodsequipmentService.GetGoodsequipmentInformationByGoodsequitype(param);
            return toResponse(res);
        }
        #endregion

        #region 批量添加设备指令
        /// <summary>
        /// 批量添加设备指令
        /// </summary>
        /// <param name="batchAddDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BatchAddGoodscommand(BatchAddGoodscommandDate batchAddDate)
        {
            ReturnMessage res = _IGoodscommandService.BatchAddGoodscommand(batchAddDate);
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

        #region 修改货位指令
        /// <summary>
        /// 修改货位指令
        /// </summary>
        /// <param name="goodscommand"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateGoodscommand(tn_dts_goodscommand goodscommand)
        {
            ReturnMessage res = _IGoodscommandService.UpdateGoodscommand(goodscommand);
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

        #region 编辑货位指令
        /// <summary>
        /// 编辑货位指令
        /// </summary>
        /// <param name="editGoodscommandDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditGoodscommandJson(EditGoodscommandDate editGoodscommandDate)
        {
            ReturnMessage res = _IGoodscommandService.EditGoodscommandJson(editGoodscommandDate);
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

        #region 按指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// <summary>
        /// 按指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm parm)
        {
            var res = _IGoodscommandService.GetPageList(parm);
            return toResponse(res);
        }
        #endregion

        #region 复制货位指令
        /// <summary>
        /// 复制货位指令
        /// </summary>
        /// <param name="copyGoodscommandDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CopyGoodscommand(CopyGoodscommandDate copyGoodscommandDate)
        {
            ReturnMessage res = _IGoodscommandService.CopyGoodscommand(copyGoodscommandDate);
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

        #region 批量删除货位指令
        /// <summary>
        /// 批量删除货位指令
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteGoodscommand([FromBody] List<string> guidList)
        {
            ReturnMessage res = _IGoodscommandService.DeleteGoodscommand(guidList);
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

        #region 按指令编码和指令名称混合模糊分页查询
        /// <summary>
        /// 按指令编码和指令名称混合模糊分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetGoodscommandInformationPages(PageParm param)
        {
            var res = _IGoodscommandService.GetGoodscommandInformationPages(param);
            return toResponse(res);
        }
        #endregion

        //[HttpPost]
        ////public IActionResult GetStockByGooodsequipmentGuid(GetReturnSite getReturnSite)
        //public IActionResult GetStockByGooodsequipmentGuid()
        //{
        //    List<StockItemInformation> stockViewModelList = new List<StockItemInformation>();
        //    //for (int i = 0; i < 100; i++)
        //    //{
        //    //    StockItemInformation stockItemInformation = new StockItemInformation();
        //    //    stockItemInformation.stockCode = "001";
        //    //    stockItemInformation.areaCode = "001";
        //    //    int row = i % 13;
        //    //    int col = i % 17;
        //    //    int layer = i % 23;
        //    //    stockItemInformation.locationCode = row.ToString() + "-" + col.ToString() + "-" + layer.ToString();
        //    //    stockItemInformation.locationType = "立库";
        //    //    if (i % 4 == 0)
        //    //    {
        //    //        stockItemInformation.state = "正常";
        //    //    }
        //    //    else if (i % 4 == 1)
        //    //    {
        //    //        stockItemInformation.state = "报废";
        //    //    }
        //    //    else if (i % 4 == 2)
        //    //    {
        //    //        stockItemInformation.state = "预入库锁定";
        //    //    }
        //    //    else
        //    //    {
        //    //        stockItemInformation.state = "预出库锁定";
        //    //    }
        //    //    if (i % 3 == 0)
        //    //    {
        //    //        stockItemInformation.storageState = "空";
        //    //    }
        //    //    else if (i % 3 == 1)
        //    //    {
        //    //        stockItemInformation.storageState = "满";
        //    //    }
        //    //    else
        //    //    {
        //    //        stockItemInformation.storageState = "空托盘";
        //    //    }
        //    //    List<ItemRowViewModel> itemRowViewModelList = new List<ItemRowViewModel>();
        //    //    for (int j = 1; j <= (i % 3) + 1; j++)
        //    //    {
        //    //        ItemRowViewModel itemRowViewModel = new ItemRowViewModel();
        //    //        itemRowViewModel.itemCode = row.ToString() + "-" + col.ToString() + "-" + layer.ToString() + "-" + j.ToString();
        //    //        itemRowViewModel.itemName = (j + 2).ToString();
        //    //        itemRowViewModel.trayCode = (j + i).ToString();
        //    //        itemRowViewModel.remarks = "";
        //    //        itemRowViewModel.ext1 = "";
        //    //        itemRowViewModel.ext2 = "";
        //    //        itemRowViewModelList.Add(itemRowViewModel);
        //    //    }
        //    //    stockItemInformation.itemRow = itemRowViewModelList;
        //    //    stockViewModelList.Add(stockItemInformation);
        //    //}
        //    //MongoDBSingleton.Instance.InsertMany<StockItemInformation>(stockViewModelList);

        //    var filterLocation = Builders<StockItemInformation>.Filter.Where(it => it.locationType == "立库");
        //    MongoDBSingleton.Instance.DeleteMany<StockItemInformation>(filterLocation);

        //    //List<Model.Entity.Sys.tn_dts_setting> SysSetList = new Interfaces.Service.Sys.SettingService(new DbHelper.SessionInfo()
        //    //{
        //    //    token = "",
        //    //    splitDbCode = ""
        //    //}).GetAll();
        //    //string ipAddress = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MIPAddress").cn_s_setting_keyvalue;
        //    //string port = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MPort").cn_s_setting_keyvalue;
        //    //string mongoDatabase = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MDatabaseName").cn_s_setting_keyvalue;
        //    //string userName = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MUserName").cn_s_setting_keyvalue;
        //    //string passWord = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MPassWord").cn_s_setting_keyvalue;
        //    //if (String.IsNullOrEmpty(ipAddress) && String.IsNullOrEmpty(port) && String.IsNullOrEmpty(mongoDatabase) && String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(passWord))
        //    //{
        //    //    throw new Exception();
        //    //}
        //    //string connectionString = "mongodb://" + ipAddress + ":" + port;
        //    //MongoClientSettings mongoClientSetting = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        //    //mongoClientSetting.Credential = MongoCredential.CreateCredential(mongoDatabase, userName, passWord);
        //    //MongoClient mongoClient = new MongoClient(mongoClientSetting);

        //    //var filter = Builders<StockItemInformation>.Filter.Eq("itemRow.itemCode", "7-0-16-1");
        //    //var builder = Builders<StockItemInformation>.Filter;
        //    //string row = "1";
        //    //string col = "5";
        //    //string layer = "9";
        //    //var filterRow = builder.Regex("locationCode", new BsonRegularExpression("/^" + row + "-" + "[0-9]+-[0-9]+" + "/"));
        //    //var filterCol = builder.Regex("locationCode", new BsonRegularExpression("/[0-9]+-" + col + "-[0-9]+" + "/"));
        //    //var filterLayer = builder.Regex("locationCode", new BsonRegularExpression("/[0-9]+-[0-9]+-" + layer + "$/"));
        //    //var filterState = builder.Eq("state", "正常");
        //    //var filterStorageState = builder.Eq("storageState", "空");
        //    //var filterItem = builder.Eq("locationCode", "5-15-14");
        //    ////var filterItemCode = builder.And(builder.Eq("locationCode", "5-15-14"), builder.Regex("itemRow.itemCode", new BsonRegularExpression("/" + "2" + "/")));
        //    ////var filterItemName = builder.And(builder.Eq("locationCode", "5-15-14"), builder.Regex("itemRow.itemName", new BsonRegularExpression("/" + "5" + "/")));
        //    ////var filterTrayCode = builder.And(builder.Eq("locationCode", "5-15-14"), builder.Regex("itemRow.trayCode", new BsonRegularExpression("/" + "8" + "/")));
        //    //List<StockItemInformation> stockItemInformationRow = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterRow).ToList();
        //    //List<StockItemInformation> stockItemInformationCol = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterCol).ToList();
        //    //List<StockItemInformation> stockItemInformationLayer = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterLayer).ToList();
        //    //List<StockItemInformation> stockItemInformationState = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterState).ToList();
        //    //List<StockItemInformation> stockItemInformationStorageState = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterStorageState).ToList();
        //    ////List <StockItemInformation> stockItemInformationItemCode = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterItemCode).ToList();
        //    ////List <StockItemInformation> stockItemInformationItemName = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterItemName).ToList();
        //    ////List <StockItemInformation> stockItemInformationTrayCode = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterTrayCode).ToList();
        //    //StockItemInformation stockItemInformationItem = mongoClient.GetDatabase(mongoDatabase).GetCollection<StockItemInformation>("StockItemInformation").FindSync(filterItem).First();
        //    //List<ItemRowInformation> itemRowInformationList = new List<ItemRowInformation>();
        //    //foreach (var item in stockItemInformationItem.itemRow)
        //    //{
        //    //    ItemRowInformation itemRowInformation = new ItemRowInformation();
        //    //    itemRowInformation.goodsequipmentGuid = "8191d9b7-a78e-4157-b42f-574947728801";
        //    //    itemRowInformation.rowColLayer = "5-15-14";
        //    //    itemRowInformation.itemCode = item.itemCode;
        //    //    itemRowInformation.itemName = item.itemName;
        //    //    itemRowInformation.trayCode = item.trayCode;
        //    //    itemRowInformation.remarks = item.remarks;
        //    //    itemRowInformation.ext1 = item.ext1;
        //    //    itemRowInformation.ext2 = item.ext2;
        //    //    itemRowInformationList.Add(itemRowInformation);
        //    //}
        //    //var filterDelete = Builders<ItemRowInformation>.Filter.Ne<ItemRowInformation>("itemCode", null);
        //    //MongoDBSingleton.Instance.DeleteMany<ItemRowInformation>(filterDelete);
        //    //MongoDBSingleton.Instance.InsertMany<ItemRowInformation>(itemRowInformationList);
        //    //var filterItemCode = Builders<ItemRowInformation>.Filter.Regex("itemCode", new BsonRegularExpression("/" + "2" + "/"));
        //    //var filterItemName = Builders<ItemRowInformation>.Filter.Regex("itemName", new BsonRegularExpression("/" + "5" + "/"));
        //    //var filterTrayCode = Builders<ItemRowInformation>.Filter.Regex("trayCode", new BsonRegularExpression("/" + "8" + "/"));
        //    //List<ItemRowInformation> itemRowInformationItemCode = MongoDBSingleton.Instance.FindList(filterItemCode);
        //    //List<ItemRowInformation> itemRowInformationItemName = MongoDBSingleton.Instance.FindList(filterItemName);
        //    //List<ItemRowInformation> itemRowInformationTrayCode = MongoDBSingleton.Instance.FindList(filterTrayCode);
        //    ////List<StockItemInformation> stockItemInformationList = MongoDBSingleton.Instance.;
        //    return toResponse(true);
        //}

        #region 根据货位设备唯一标识分页模糊查询货位信息
        /// <summary>
        /// 根据货位设备唯一标识分页模糊查询货位信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetStockInformationByGooodsequipmentGuid(PageParm param)
        {
            ApiResult resultReturn = new ApiResult();
            string goodsequipmentguid = param.Parms["cn_s_goodsequipment_guid"].ObjToString();
            string goodscommandRow = param.Parms["cn_s_goodscommand_row"].ObjToString();
            string goodscommandCol = param.Parms["cn_s_goodscommand_col"].ObjToString();
            string goodscommandLayer = param.Parms["cn_s_goodscommand_layer"].ObjToString();
            string goodscommandState = param.Parms["cn_s_goodscommand_state"].ObjToString();
            string goodscommandStoragestate = param.Parms["cn_s_goodscommand_storagestate"].ObjToString();
            var builder = Builders<StockItemInformation>.Filter;
            var filter = builder.Empty;
            if (_IGoodsequipmentService.GetFirst(it => it.cn_guid == goodsequipmentguid) is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "传入的货位设备唯一标识在tn_dts_goodsequipment表中不存在，请检查后重试！";
                return toResponse(resultReturn);
            }
            GoodscommandDriver.Instance.RefreshStockItemInformation(goodsequipmentguid);
            //List<tn_dts_goodscommand> goodscommandList = _IGoodscommandService.GetWhere(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && (it.cn_s_goodscommand_type == "初始化" || it.cn_s_goodscommand_type == "业务"));
            ////foreach (var goodscommand in goodscommandList)
            ////{
            ////    string json = goodscommand.cn_s_goodscommand_json;
            ////    LocationRealMonitorViewModel locationRealMonitorViewModel = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(json);
            ////    List<StockItemInformation> stockViewModelAddList = null;
            ////    if (GoodscommandDriver.Instance.WatchStockItemInformation(out stockViewModelAddList, locationRealMonitorViewModel.stock, goodsequipmentguid))
            ////    {
            ////       // var filterDelete = builder.Eq("goodsequipmentGuid", goodsequipmentguid);
            ////        foreach (var stockViewModelAdd in stockViewModelAddList)
            ////        {
            ////            if(stockViewModelAdd._id == new ObjectId("000000000000000000000000"))
            ////            {
            ////                MongoDBSingleton.Instance.Add<StockItemInformation>(stockViewModelAdd);
            ////            }
            ////            else
            ////            {
            ////                MongoDBSingleton.Instance.Update<StockItemInformation>(stockViewModelAdd, stockViewModelAdd._id.ToString());
            ////            }
            ////        } 
            ////    }
            ////}
            //List<StockViewModel> stockViewModelList = new List<StockViewModel>();
            //foreach (var goodscommand in goodscommandList)
            //{
            //    string json = goodscommand.cn_s_goodscommand_json;
            //    LocationRealMonitorViewModel locationRealMonitorViewModel = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(json);
            //    stockViewModelList.AddRange(locationRealMonitorViewModel.stock);
            //}
            //List<StockItemInformation> stockViewModelAddList = null;
            //if (GoodscommandDriver.Instance.WatchStockItemInformation(out stockViewModelAddList, stockViewModelList, goodsequipmentguid))
            //{
            //    // var filterDelete = builder.Eq("goodsequipmentGuid", goodsequipmentguid);
            //    foreach (var stockViewModelAdd in stockViewModelAddList)
            //    {
            //        if (stockViewModelAdd._id == new ObjectId("000000000000000000000000"))
            //        {
            //            MongoDBSingleton.Instance.Add<StockItemInformation>(stockViewModelAdd);
            //        }
            //        else
            //        {
            //            MongoDBSingleton.Instance.Update<StockItemInformation>(stockViewModelAdd, stockViewModelAdd._id.ToString());
            //        }
            //    }
            //}
            List<FilterDefinition<StockItemInformation>> queryList = new List<FilterDefinition<StockItemInformation>>();
            queryList.Add(builder.Eq("goodsequipmentGuid", goodsequipmentguid));
            if (!string.IsNullOrEmpty(goodscommandRow))
            {
                queryList.Add(builder.Regex("locationCode", new BsonRegularExpression("/^" + goodscommandRow + "-" + "[0-9]+-[0-9]+" + "/")));
            }
            if (!string.IsNullOrEmpty(goodscommandCol))
            {
                queryList.Add(builder.Regex("locationCode", new BsonRegularExpression("/[0-9]+-" + goodscommandCol + "-[0-9]+" + "/")));
            }
            if (!string.IsNullOrEmpty(goodscommandLayer))
            {
                queryList.Add(builder.Regex("locationCode", new BsonRegularExpression("/[0-9]+-[0-9]+-" + goodscommandLayer + "$/")));
            }
            if (!string.IsNullOrEmpty(goodscommandState))
            {
                queryList.Add(builder.Eq("state", goodscommandState));
            }
            if (!string.IsNullOrEmpty(goodscommandStoragestate))
            {
                queryList.Add(builder.Eq("storageState", goodscommandStoragestate));
            }
            if (queryList.Count > 0)
            {
                filter = builder.And(queryList);
            }
            return toResponse(MongoDBSingleton.Instance.ToPageMongo<StockItemInformation>(filter, param.PageIndex, param.PageSize));
        }
        #endregion

        #region 根据货位设备唯一标识和排列层分页模糊查询物料信息
        /// <summary>
        /// 根据货位设备唯一标识和排列层分页模糊查询物料信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetItemRowInformationByGooodsequipmentGuidRowcollayer(PageParm param)
        {
            ApiResult resultReturn = new ApiResult();
            string goodsequipmentguid = param.Parms["cn_s_goodsequipment_guid"].ObjToString();
            string rowcollayer = param.Parms["cn_s_goodscommand_rowcollayer"].ObjToString();
            string itemCode = param.Parms["cn_s_goodscommand_itemCode"].ObjToString();
            string itemName = param.Parms["cn_s_goodscommand_itemName"].ObjToString();
            string trayCode = param.Parms["cn_s_goodscommand_trayCode"].ObjToString();
            var builderStock = Builders<StockItemInformation>.Filter;
            var builderItem = Builders<ItemRowInformation>.Filter;
            var filter = builderItem.Empty;
            if (_IGoodsequipmentService.GetFirst(it => it.cn_guid == goodsequipmentguid) is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "传入的货位设备唯一标识在tn_dts_goodsequipment表中不存在，请检查后重试！";
                return toResponse(resultReturn);
            }
            
            var filterStock = builderStock.And(builderStock.Eq("goodsequipmentGuid", goodsequipmentguid), builderStock.Eq("locationCode", rowcollayer));
            StockItemInformation stockItemInformation = MongoDBSingleton.Instance.FindOneFilter(filterStock);
            if(stockItemInformation is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "传入的货位设备唯一标识或排列层在MongoDB中不存在，请检查后重试！";
                return toResponse(resultReturn);
            }
            var filterItemDelete = builderItem.And(builderItem.Eq("goodsequipmentGuid", goodsequipmentguid), builderItem.Eq("rowColLayer", rowcollayer));
            MongoDBSingleton.Instance.DeleteMany(filterItemDelete);
            MongoDBSingleton.Instance.InsertMany<ItemRowInformation>(stockItemInformation.itemRow);
            List<FilterDefinition<ItemRowInformation>> queryList = new List<FilterDefinition<ItemRowInformation>>();
            queryList.Add(filterItemDelete);
            if (!string.IsNullOrEmpty(itemCode))
            {
                queryList.Add(builderItem.Regex("itemCode", new BsonRegularExpression("/" + itemCode + "/")));
            }
            if (!string.IsNullOrEmpty(itemName))
            {
                queryList.Add(builderItem.Regex("itemName", new BsonRegularExpression("/" + itemName + "/")));
            }
            if (!string.IsNullOrEmpty(trayCode))
            {
                queryList.Add(builderItem.Regex("trayCode", new BsonRegularExpression("/" + trayCode + "/")));
            }
            if (queryList.Count > 0)
            {
                filter = builderItem.And(queryList);
            }
            return toResponse(MongoDBSingleton.Instance.ToPageMongo<ItemRowInformation>(filter, param.PageIndex, param.PageSize));
        }
        #endregion

        #region 批量添加库位
        /// <summary>
        /// 批量添加库位
        /// </summary>
        /// <param name="batchAddLocationDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BatchAddLocation(BatchAddLocationDate batchAddLocationDate)
        {
            ApiResult resultReturn = new ApiResult();
            tn_dts_goodsequipment goodsequipmentGuid = _IGoodsequipmentService.GetWhere(it => it.cn_guid == batchAddLocationDate.GoodsequipmentGuid).First();
            if (goodsequipmentGuid is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "传入的货位设备唯一标识在tn_dts_goodsequipment表中不存在，请检查后重试！";
                return toResponse(resultReturn);
            }
            GetAreaMaxRowColLayer getAreaMaxRowColLayer = new GetAreaMaxRowColLayer();
            getAreaMaxRowColLayer.stockCode = goodsequipmentGuid.cn_s_goodsequipment_stockcode;
            getAreaMaxRowColLayer.areaCode = goodsequipmentGuid.cn_s_goodsequipment_areacode;
            MaxRowColLayer maxRowColLayer = GoodscommandDriver.Instance.GetAreaMaxRowcollayer(getAreaMaxRowColLayer);
            GoodscommandDriver.Instance.RefreshStockItemInformation(batchAddLocationDate.GoodsequipmentGuid);
            ReturnMessage res = _IGoodscommandService.BatchAddLocation(batchAddLocationDate, maxRowColLayer);
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

        #region 修改库位
        /// <summary>
        /// 修改库位
        /// </summary>
        /// <param name="modifyLocationDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ModifyLocation(ModifyLocationDate modifyLocationDate)
        {
            ApiResult resultReturn = new ApiResult();
            tn_dts_goodsequipment goodsequipmentGuid = _IGoodsequipmentService.GetWhere(it => it.cn_guid == modifyLocationDate.GoodsequipmentGuid).First();
            if (goodsequipmentGuid is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "传入的货位设备唯一标识在tn_dts_goodsequipment表中不存在，请检查后重试！";
                return toResponse(resultReturn);
            }
            GoodscommandDriver.Instance.RefreshStockItemInformation(modifyLocationDate.GoodsequipmentGuid);
            ReturnMessage res = _IGoodscommandService.ModifyLocation(modifyLocationDate);
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

        #region 删除库位
        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="deleteLocationDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteLocation(DeleteLocationDate deleteLocationDate)
        {
            ApiResult resultReturn = new ApiResult();
            tn_dts_goodsequipment goodsequipmentGuid = _IGoodsequipmentService.GetWhere(it => it.cn_guid == deleteLocationDate.GoodsequipmentGuid).First();
            if (goodsequipmentGuid is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "传入的货位设备唯一标识在tn_dts_goodsequipment表中不存在，请检查后重试！";
                return toResponse(resultReturn);
            }
            GoodscommandDriver.Instance.RefreshStockItemInformation(deleteLocationDate.GoodsequipmentGuid);
            ReturnMessage res = _IGoodscommandService.DeleteLocation(deleteLocationDate);
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
    }
}
