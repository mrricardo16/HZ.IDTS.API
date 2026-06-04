using HZ.IDTSCore.Api.Controllers;
using HZ.IDTSCore.Api.Global;
using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Interfaces.Service.Equipment;
using HZ.IDTSCore.Interfaces.Service.SenarioTesting;
using HZ.IDTSCore.Interfaces.Service.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.iWCS.MData.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class GoodscommandDriver
    {
        private static readonly GoodscommandDriver instance = new GoodscommandDriver();
       
        private GoodscommandDriver()
        {
            
        }

        public static GoodscommandDriver Instance
        {
            get
            {
                return instance;
            }
        }

        #region 刷新指定货位设备Mongo库位信息
        /// <summary>
        /// 刷新指定货位设备Mongo库位信息
        /// </summary>
        /// <param name="goodsequipmentguid"></param>
        /// <returns></returns>
        public void RefreshStockItemInformation(string goodsequipmentguid)
        {
            List<tn_dts_goodscommand> goodscommandList = new Interfaces.Service.SenarioTesting.GoodscommandService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            }).GetWhere(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && (it.cn_s_goodscommand_type == "初始化" || it.cn_s_goodscommand_type == "业务"));
            List<RefreshStock> refreshStockList = new List<RefreshStock>();
            foreach (var goodscommand in goodscommandList)
            {
                string json = goodscommand.cn_s_goodscommand_json;
                LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                locationRealMonitorViewModel.stock = new List<StockViewModel>();
                try
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(json);
                    }   
                }
                catch
                {

                }
                finally
                {
                    if (locationRealMonitorViewModel is null)
                    {
                        locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                        locationRealMonitorViewModel.stock = new List<StockViewModel>();
                    }
                }         
                foreach (var item in locationRealMonitorViewModel.stock)
                {
                    RefreshStock refreshStock = new RefreshStock();
                    refreshStock.commandSource = goodscommand.cn_s_goodscommand_type;
                    if (goodscommand.cn_s_goodscommand_type == "初始化")
                    {
                        refreshStock.busGuid = "";
                    }
                    else
                    {
                        refreshStock.busGuid = goodscommand.cn_guid;
                    }
                    refreshStock.stockViewModel = item;
                    refreshStockList.Add(refreshStock);
                }
            }
            List<StockItemInformation> stockViewModelAddList = null;
            if (GoodscommandDriver.Instance.WatchStockItemInformation(out stockViewModelAddList, refreshStockList, goodsequipmentguid))
            {
                // var filterDelete = builder.Eq("goodsequipmentGuid", goodsequipmentguid);
                foreach (var stockViewModelAdd in stockViewModelAddList)
                {
                    if (stockViewModelAdd._id == new ObjectId("000000000000000000000000"))
                    {
                        MongoDBSingleton.Instance.Add<StockItemInformation>(stockViewModelAdd);
                    }
                    else
                    {
                        MongoDBSingleton.Instance.Update<StockItemInformation>(stockViewModelAdd, stockViewModelAdd._id.ToString());
                    }
                }
            }
        }
        #endregion

        #region 刷新指定货位设备Mongo库位信息V2
        /// <summary>
        /// 刷新指定货位设备Mongo库位信息V2。
        /// 时间：2026-05-29
        /// 优化内容：原版本在WatchStockItemInformation中按货位逐条FindOneFilter查询MongoDB，货位数量几万时会产生几万次MongoDB请求。
        /// V2版本一次性取出该货位设备已有货位数据，在内存中用Dictionary按locationCode匹配，只保留必要的新增、修改、删除操作。
        /// 注意：原RefreshStockItemInformation、WatchStockItemInformation、WatchItemRow均不改动，V2方法独立保留。
        /// </summary>
        /// <param name="goodsequipmentguid">货位设备唯一标识</param>
        public void RefreshStockItemInformationV2(string goodsequipmentguid)
        {
            List<tn_dts_goodscommand> goodscommandList = new Interfaces.Service.SenarioTesting.GoodscommandService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            }).GetWhere(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && (it.cn_s_goodscommand_type == "初始化" || it.cn_s_goodscommand_type == "业务"));

            List<RefreshStock> refreshStockList = new List<RefreshStock>();
            foreach (var goodscommand in goodscommandList)
            {
                LocationRealMonitorViewModel locationRealMonitorViewModel = DeserializeLocationRealMonitorV2(goodscommand.cn_s_goodscommand_json);
                foreach (var item in locationRealMonitorViewModel.stock)
                {
                    RefreshStock refreshStock = new RefreshStock();
                    refreshStock.commandSource = goodscommand.cn_s_goodscommand_type;
                    refreshStock.busGuid = goodscommand.cn_s_goodscommand_type == "初始化" ? "" : goodscommand.cn_guid;
                    refreshStock.stockViewModel = item;
                    refreshStockList.Add(refreshStock);
                }
            }

            List<StockItemInformation> stockItemInformationAddListV2 = null;
            List<StockItemInformation> stockItemInformationUpdateListV2 = null;
            List<ObjectId> stockItemInformationDeleteIdListV2 = null;
            if (WatchStockItemInformationV2(out stockItemInformationAddListV2, out stockItemInformationUpdateListV2, out stockItemInformationDeleteIdListV2, refreshStockList, goodsequipmentguid))
            {
                var stockItemInformationBuilder = Builders<StockItemInformation>.Filter;

                // 删除使用DeleteMany一次提交，避免几万个货位逐条Delete。
                if (stockItemInformationDeleteIdListV2.Count > 0)
                {
                    var deleteFilterV2 = stockItemInformationBuilder.In("_id", stockItemInformationDeleteIdListV2);
                    MongoDBSingleton.Instance.DeleteMany<StockItemInformation>(deleteFilterV2);
                }

                // 新增使用InsertMany一次批量写入，减少MongoDB网络往返次数。
                if (stockItemInformationAddListV2.Count > 0)
                {
                    MongoDBSingleton.Instance.InsertMany<StockItemInformation>(stockItemInformationAddListV2);
                }

                // 每个货位的更新内容不同，UpdateManay只能对同一批文档设置同一份UpdateDefinition，这里不能直接合并成一次UpdateManay。
                // V2已去掉逐货位FindOneFilter，剩余更新只对真正变化的数据执行，整体耗时会明显低于原版本。
                foreach (var stockItemInformationUpdateV2 in stockItemInformationUpdateListV2)
                {
                    MongoDBSingleton.Instance.Update<StockItemInformation>(stockItemInformationUpdateV2, stockItemInformationUpdateV2._id.ToString());
                }
            }
        }
        #endregion

        #region 反序列化货位同步ModelV2
        /// <summary>
        /// 反序列化货位同步ModelV2。
        /// </summary>
        /// <param name="json">货位同步Json</param>
        /// <returns>货位同步Model</returns>
        private LocationRealMonitorViewModel DeserializeLocationRealMonitorV2(string json)
        {
            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
            locationRealMonitorViewModel.stock = new List<StockViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(json);
                }
            }
            catch
            {

            }
            finally
            {
                if (locationRealMonitorViewModel is null)
                {
                    locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                }
                if (locationRealMonitorViewModel.stock is null)
                {
                    locationRealMonitorViewModel.stock = new List<StockViewModel>();
                }
            }
            return locationRealMonitorViewModel;
        }
        #endregion

        #region 判定货位信息是否修改V2
        /// <summary>
        /// 判定货位信息是否修改V2。
        /// 时间：2026-05-29
        /// 优化内容：原版本每个货位都调用FindOneFilter查询MongoDB；V2一次FindList取出旧数据，然后用Dictionary在内存中按locationCode匹配。
        /// </summary>
        /// <param name="stockItemInformationAddListV2">需要新增的货位列表</param>
        /// <param name="stockItemInformationUpdateListV2">需要更新的货位列表</param>
        /// <param name="stockItemInformationDeleteIdListV2">需要删除的MongoDB主键列表</param>
        /// <param name="refreshStockList">指令解析出的货位数据</param>
        /// <param name="goodsequipmentGuid">货位设备唯一标识</param>
        /// <returns>是否存在新增、修改或删除</returns>
        public bool WatchStockItemInformationV2(out List<StockItemInformation> stockItemInformationAddListV2, out List<StockItemInformation> stockItemInformationUpdateListV2, out List<ObjectId> stockItemInformationDeleteIdListV2, List<RefreshStock> refreshStockList, string goodsequipmentGuid)
        {
            stockItemInformationAddListV2 = new List<StockItemInformation>();
            stockItemInformationUpdateListV2 = new List<StockItemInformation>();
            stockItemInformationDeleteIdListV2 = new List<ObjectId>();

            if (refreshStockList is null)
            {
                refreshStockList = new List<RefreshStock>();
            }

            var stockItemInformationBuilder = Builders<StockItemInformation>.Filter;
            var aGoodsequipmentFilter = stockItemInformationBuilder.Eq("goodsequipmentGuid", goodsequipmentGuid);

            // 只查询一次MongoDB，避免货位数量大时出现几万次FindOneFilter。
            List<StockItemInformation> oldGoodsequipmentStockListV2 = MongoDBSingleton.Instance.FindList<StockItemInformation>(aGoodsequipmentFilter);
            if (oldGoodsequipmentStockListV2 is null)
            {
                oldGoodsequipmentStockListV2 = new List<StockItemInformation>();
            }

            Dictionary<string, StockItemInformation> oldStockDictionaryV2 = new Dictionary<string, StockItemInformation>();
            foreach (var oldGoodsequipmentStockV2 in oldGoodsequipmentStockListV2)
            {
                if (oldGoodsequipmentStockV2 is null || string.IsNullOrEmpty(oldGoodsequipmentStockV2.locationCode))
                {
                    continue;
                }
                if (!oldStockDictionaryV2.ContainsKey(oldGoodsequipmentStockV2.locationCode))
                {
                    oldStockDictionaryV2.Add(oldGoodsequipmentStockV2.locationCode, oldGoodsequipmentStockV2);
                }
            }

            Dictionary<string, StockItemInformation> addStockDictionaryV2 = new Dictionary<string, StockItemInformation>();
            HashSet<string> currentLocationCodeSetV2 = new HashSet<string>();
            HashSet<ObjectId> updateIdSetV2 = new HashSet<ObjectId>();

            foreach (var refreshStockV2 in refreshStockList)
            {
                if (refreshStockV2 is null || refreshStockV2.stockViewModel is null || string.IsNullOrEmpty(refreshStockV2.stockViewModel.locationCode))
                {
                    continue;
                }

                StockViewModel stockViewModelV2 = refreshStockV2.stockViewModel;
                currentLocationCodeSetV2.Add(stockViewModelV2.locationCode);

                StockItemInformation oldStockItemInformationV2 = null;
                if (oldStockDictionaryV2.TryGetValue(stockViewModelV2.locationCode, out oldStockItemInformationV2))
                {
                    if (ApplyStockItemInformationV2(oldStockItemInformationV2, refreshStockV2, goodsequipmentGuid, stockViewModelV2)
                        && !updateIdSetV2.Contains(oldStockItemInformationV2._id))
                    {
                        stockItemInformationUpdateListV2.Add(oldStockItemInformationV2);
                        updateIdSetV2.Add(oldStockItemInformationV2._id);
                    }
                    continue;
                }

                StockItemInformation addStockItemInformationV2 = null;
                if (addStockDictionaryV2.TryGetValue(stockViewModelV2.locationCode, out addStockItemInformationV2))
                {
                    // 同一批数据中同一个locationCode重复出现时，继续复用待新增对象，避免插入重复货位。
                    ApplyStockItemInformationV2(addStockItemInformationV2, refreshStockV2, goodsequipmentGuid, stockViewModelV2);
                    continue;
                }

                addStockItemInformationV2 = BuildStockItemInformationV2(refreshStockV2, goodsequipmentGuid, stockViewModelV2);
                stockItemInformationAddListV2.Add(addStockItemInformationV2);
                addStockDictionaryV2.Add(stockViewModelV2.locationCode, addStockItemInformationV2);
            }

            foreach (var oldGoodsequipmentStockV2 in oldGoodsequipmentStockListV2)
            {
                if (oldGoodsequipmentStockV2 is null || currentLocationCodeSetV2.Contains(oldGoodsequipmentStockV2.locationCode))
                {
                    continue;
                }
                stockItemInformationDeleteIdListV2.Add(oldGoodsequipmentStockV2._id);
            }

            return stockItemInformationAddListV2.Count > 0 || stockItemInformationUpdateListV2.Count > 0 || stockItemInformationDeleteIdListV2.Count > 0;
        }
        #endregion

        #region 构建货位信息V2
        /// <summary>
        /// 构建货位信息V2。
        /// </summary>
        private StockItemInformation BuildStockItemInformationV2(RefreshStock refreshStockV2, string goodsequipmentGuid, StockViewModel stockViewModelV2)
        {
            StockItemInformation stockItemInformationV2 = new StockItemInformation();
            stockItemInformationV2.goodsequipmentGuid = goodsequipmentGuid;
            stockItemInformationV2.busGuid = refreshStockV2.busGuid;
            stockItemInformationV2.commandSource = refreshStockV2.commandSource;
            stockItemInformationV2.stockCode = stockViewModelV2.stockCode;
            stockItemInformationV2.areaCode = stockViewModelV2.areaCode;
            stockItemInformationV2.locationCode = stockViewModelV2.locationCode;
            stockItemInformationV2.locationType = stockViewModelV2.locationType;
            stockItemInformationV2.state = stockViewModelV2.state;
            stockItemInformationV2.storageState = stockViewModelV2.storageState;
            stockItemInformationV2.itemRow = BuildItemRowInformationListV2(stockViewModelV2.itemRow, goodsequipmentGuid, stockViewModelV2.locationCode);
            return stockItemInformationV2;
        }
        #endregion

        #region 应用货位变更V2
        /// <summary>
        /// 应用货位变更V2。
        /// </summary>
        private bool ApplyStockItemInformationV2(StockItemInformation stockItemInformationV2, RefreshStock refreshStockV2, string goodsequipmentGuid, StockViewModel stockViewModelV2)
        {
            bool isChangeV2 = false;

            if (stockItemInformationV2.commandSource != refreshStockV2.commandSource && refreshStockV2.commandSource == "初始化")
            {
                stockItemInformationV2.commandSource = refreshStockV2.commandSource;
                stockItemInformationV2.busGuid = "";
                isChangeV2 = true;
            }
            if (stockItemInformationV2.commandSource != refreshStockV2.commandSource && refreshStockV2.commandSource == "业务")
            {
                stockItemInformationV2.commandSource = refreshStockV2.commandSource;
                stockItemInformationV2.busGuid = refreshStockV2.busGuid;
                isChangeV2 = true;
            }
            if (stockItemInformationV2.state != stockViewModelV2.state)
            {
                stockItemInformationV2.state = stockViewModelV2.state;
                isChangeV2 = true;
            }
            if (stockItemInformationV2.storageState != stockViewModelV2.storageState)
            {
                stockItemInformationV2.storageState = stockViewModelV2.storageState;
                isChangeV2 = true;
            }

            List<ItemRowInformation> itemRowInformationListV2 = stockItemInformationV2.itemRow;
            if (WatchItemRowV2(ref itemRowInformationListV2, stockViewModelV2.itemRow, goodsequipmentGuid, stockViewModelV2.locationCode))
            {
                stockItemInformationV2.itemRow = itemRowInformationListV2;
                isChangeV2 = true;
            }

            return isChangeV2;
        }
        #endregion

        #region 构建物料信息列表V2
        /// <summary>
        /// 构建物料信息列表V2。
        /// </summary>
        private List<ItemRowInformation> BuildItemRowInformationListV2(List<ItemRowViewModel> itemRowViewModelListV2, string goodsequipmentGuid, string rowColLayer)
        {
            List<ItemRowInformation> itemRowInformationListV2 = new List<ItemRowInformation>();
            if (itemRowViewModelListV2 is null)
            {
                return itemRowInformationListV2;
            }

            foreach (var itemV2 in itemRowViewModelListV2)
            {
                if (itemV2 is null)
                {
                    continue;
                }
                ItemRowInformation itemRowInformationV2 = new ItemRowInformation();
                itemRowInformationV2.goodsequipmentGuid = goodsequipmentGuid;
                itemRowInformationV2.rowColLayer = rowColLayer;
                itemRowInformationV2.itemCode = itemV2.itemCode;
                itemRowInformationV2.itemName = itemV2.itemName;
                itemRowInformationV2.trayCode = itemV2.trayCode;
                itemRowInformationV2.remarks = itemV2.remarks;
                itemRowInformationV2.ext1 = itemV2.ext1;
                itemRowInformationV2.ext2 = itemV2.ext2;
                itemRowInformationListV2.Add(itemRowInformationV2);
            }
            return itemRowInformationListV2;
        }
        #endregion
        #region 判定货位信息是否修改
        /// <summary>
        /// 判定货位信息是否修改
        /// </summary>
        /// <param name="stockItemInformationList"></param>
        /// <param name="refreshStockList"></param>
        /// <param name="goodsequipmentGuid"></param>
        /// <returns></returns>
        public bool WatchStockItemInformation(out List<StockItemInformation> stockItemInformationList, List<RefreshStock> refreshStockList, string goodsequipmentGuid)
        {
            bool isChange = false;
            var stockItemInformationBuilder = Builders<StockItemInformation>.Filter;
            stockItemInformationList = new List<StockItemInformation>();
            List<StockItemInformation> stockItemInformationNoDeleteList = new List<StockItemInformation>();
            var aGoodsequipmentFilter = stockItemInformationBuilder.Eq("goodsequipmentGuid", goodsequipmentGuid);
            List<StockItemInformation> oldGoodsequipmentStockList = MongoDBSingleton.Instance.FindList(aGoodsequipmentFilter);
            foreach (var refreshStock in refreshStockList)
            {
                StockViewModel stockViewModel = refreshStock.stockViewModel;
                var updateFilter = stockItemInformationBuilder.And(stockItemInformationBuilder.Eq("goodsequipmentGuid", goodsequipmentGuid), stockItemInformationBuilder.Eq("locationCode", stockViewModel.locationCode));
                StockItemInformation updatestock = MongoDBSingleton.Instance.FindOneFilter(updateFilter);
                if (updatestock is null)
                {
                    StockItemInformation stockItemInformation = new StockItemInformation();
                    stockItemInformation.goodsequipmentGuid = goodsequipmentGuid;
                    stockItemInformation.busGuid = refreshStock.busGuid;
                    stockItemInformation.commandSource = refreshStock.commandSource;
                    stockItemInformation.stockCode = stockViewModel.stockCode;
                    stockItemInformation.areaCode = stockViewModel.areaCode;
                    stockItemInformation.locationCode = stockViewModel.locationCode;
                    stockItemInformation.locationType = stockViewModel.locationType;
                    stockItemInformation.state = stockViewModel.state;
                    stockItemInformation.storageState = stockViewModel.storageState;
                    List<ItemRowInformation> itemRowInformationList = new List<ItemRowInformation>();
                    foreach (var item in stockViewModel.itemRow)
                    {
                        ItemRowInformation itemRowInformation = new ItemRowInformation();
                        itemRowInformation.goodsequipmentGuid = goodsequipmentGuid;
                        itemRowInformation.rowColLayer = stockViewModel.locationCode;
                        itemRowInformation.itemCode = item.itemCode;
                        itemRowInformation.itemName = item.itemName;
                        itemRowInformation.trayCode = item.trayCode;
                        itemRowInformation.remarks = item.remarks;
                        itemRowInformation.ext1 = item.ext1;
                        itemRowInformation.ext2 = item.ext2;
                        itemRowInformationList.Add(itemRowInformation)
;
                    }
                    stockItemInformation.itemRow = itemRowInformationList;
                    stockItemInformationList.Add(stockItemInformation);

                    isChange = true;
                }
                else
                {
                    if (updatestock.commandSource != refreshStock.commandSource && refreshStock.commandSource == "初始化")
                    {
                        updatestock.commandSource = refreshStock.commandSource;
                        updatestock.busGuid = "";
                        isChange = true;
                    }
                    if (updatestock.commandSource != refreshStock.commandSource && refreshStock.commandSource == "业务")
                    {
                        updatestock.commandSource = refreshStock.commandSource;
                        updatestock.busGuid = refreshStock.busGuid;
                        isChange = true;
                    }
                    if (updatestock.state != stockViewModel.state)
                    {
                        updatestock.state = stockViewModel.state;
                        isChange = true;
                    }
                    if (updatestock.storageState != stockViewModel.storageState)
                    {
                        updatestock.storageState = stockViewModel.storageState;
                        isChange = true;
                    }
                    List<ItemRowInformation> itemRowViewModelList = updatestock.itemRow;
                    if (WatchItemRow(ref itemRowViewModelList, stockViewModel.itemRow, goodsequipmentGuid, stockViewModel.locationCode))
                    {
                        updatestock.itemRow = itemRowViewModelList;
                        isChange = true;
                    }
                    stockItemInformationList.Add(updatestock);
                }
            }
            foreach (var oldGoodsequipmentStock in oldGoodsequipmentStockList)
            {
                StockItemInformation stockItemInformationLocationCode = stockItemInformationList.FirstOrDefault(it => it.locationCode == oldGoodsequipmentStock.locationCode);
                if (stockItemInformationLocationCode is null)
                {
                    MongoDBSingleton.Instance.Delete<StockItemInformation>(oldGoodsequipmentStock._id.ToString());
                }
            }
            return isChange;
        }
        #endregion

        #region 判定物料信息是否修改
        /// <summary>
        /// 判定物料信息是否修改
        /// </summary>
        /// <param name="itemRowInformationList"></param>
        /// <param name="itemRowViewModelList"></param>
        /// <param name="goodsequipmentGuid"></param>
        /// <param name="rowColLayer"></param>
        /// <returns></returns>
        public bool WatchItemRow(ref List<ItemRowInformation> itemRowInformationList, List<ItemRowViewModel> itemRowViewModelList, string goodsequipmentGuid, string rowColLayer)
        {
            bool isChange = false;
            List<ItemRowInformation> itemRowInformationNotDeleteList = new List<ItemRowInformation>();
            foreach (var item in itemRowViewModelList)
            {
                ItemRowInformation itemRowInformationItemcode = itemRowInformationList.FirstOrDefault(it => it.itemCode == item.itemCode);
                if (itemRowInformationItemcode is null)
                {
                    ItemRowInformation itemRowInformation = new ItemRowInformation();
                    itemRowInformation.goodsequipmentGuid = goodsequipmentGuid;
                    itemRowInformation.rowColLayer = rowColLayer;
                    itemRowInformation.itemCode = item.itemCode;
                    itemRowInformation.itemName = item.itemName;
                    itemRowInformation.trayCode = item.trayCode;
                    itemRowInformation.remarks = item.remarks;
                    itemRowInformation.ext1 = item.ext1;
                    itemRowInformation.ext2 = item.ext2;
                    itemRowInformationList.Add(itemRowInformation);
                    itemRowInformationNotDeleteList.Add(itemRowInformation);
                    isChange = true;
                }
                else
                {
                    itemRowInformationNotDeleteList.Add(itemRowInformationItemcode);
                    if (itemRowInformationItemcode.itemName != item.itemName)
                    {
                        itemRowInformationItemcode.itemName = item.itemName;
                        isChange = true;
                    }
                    if (itemRowInformationItemcode.trayCode != item.trayCode)
                    {
                        itemRowInformationItemcode.trayCode = item.trayCode;
                        isChange = true;
                    }
                    if (itemRowInformationItemcode.remarks != item.remarks)
                    {
                        itemRowInformationItemcode.remarks = item.remarks;
                        isChange = true;
                    }
                    if (itemRowInformationItemcode.ext1 != item.ext1)
                    {
                        itemRowInformationItemcode.ext1 = item.ext1;
                        isChange = true;
                    }
                    if (itemRowInformationItemcode.ext2 != item.ext2)
                    {
                        itemRowInformationItemcode.ext2 = item.ext2;
                        isChange = true;
                    }
                }
            }
            List<ItemRowInformation> itemRowInfromationDeletedList = new List<ItemRowInformation>();
            foreach (var item in itemRowInformationList)
            {
                ItemRowInformation itemRowInformationItemcode = itemRowInformationNotDeleteList.FirstOrDefault(it => it.itemCode == item.itemCode);
                if (itemRowInformationItemcode is null)
                {
                    isChange = true;
                    continue;
                }
                itemRowInfromationDeletedList.Add(item);
            }
            itemRowInformationList = itemRowInfromationDeletedList;
            return isChange;
        }
        #endregion

        #region 判定物料信息是否修改V2
        /// <summary>
        /// 判定物料信息是否修改V2。
        /// 时间：2026-05-29
        /// 优化内容：原版本每个物料都FirstOrDefault扫描旧物料列表；V2将旧物料按itemCode转成Dictionary，减少物料明细比对耗时。
        /// </summary>
        /// <param name="itemRowInformationList">MongoDB中已有物料信息</param>
        /// <param name="itemRowViewModelList">接口Json中的物料信息</param>
        /// <param name="goodsequipmentGuid">货位设备唯一标识</param>
        /// <param name="rowColLayer">货位编码</param>
        /// <returns>是否存在物料新增、修改或删除</returns>
        public bool WatchItemRowV2(ref List<ItemRowInformation> itemRowInformationList, List<ItemRowViewModel> itemRowViewModelList, string goodsequipmentGuid, string rowColLayer)
        {
            bool isChangeV2 = false;
            if (itemRowInformationList is null)
            {
                itemRowInformationList = new List<ItemRowInformation>();
            }
            if (itemRowViewModelList is null)
            {
                itemRowViewModelList = new List<ItemRowViewModel>();
            }

            Dictionary<string, ItemRowInformation> oldItemRowDictionaryV2 = new Dictionary<string, ItemRowInformation>();
            foreach (var itemRowInformationV2 in itemRowInformationList)
            {
                if (itemRowInformationV2 is null)
                {
                    continue;
                }
                string itemCodeV2 = itemRowInformationV2.itemCode ?? string.Empty;
                if (!oldItemRowDictionaryV2.ContainsKey(itemCodeV2))
                {
                    oldItemRowDictionaryV2.Add(itemCodeV2, itemRowInformationV2);
                }
            }

            List<ItemRowInformation> newItemRowInformationListV2 = new List<ItemRowInformation>();
            HashSet<string> currentItemCodeSetV2 = new HashSet<string>();
            foreach (var itemRowViewModelV2 in itemRowViewModelList)
            {
                if (itemRowViewModelV2 is null)
                {
                    continue;
                }

                string itemCodeV2 = itemRowViewModelV2.itemCode ?? string.Empty;
                currentItemCodeSetV2.Add(itemCodeV2);

                ItemRowInformation itemRowInformationV2 = null;
                if (oldItemRowDictionaryV2.TryGetValue(itemCodeV2, out itemRowInformationV2))
                {
                    if (itemRowInformationV2.itemName != itemRowViewModelV2.itemName)
                    {
                        itemRowInformationV2.itemName = itemRowViewModelV2.itemName;
                        isChangeV2 = true;
                    }
                    if (itemRowInformationV2.trayCode != itemRowViewModelV2.trayCode)
                    {
                        itemRowInformationV2.trayCode = itemRowViewModelV2.trayCode;
                        isChangeV2 = true;
                    }
                    if (itemRowInformationV2.remarks != itemRowViewModelV2.remarks)
                    {
                        itemRowInformationV2.remarks = itemRowViewModelV2.remarks;
                        isChangeV2 = true;
                    }
                    if (itemRowInformationV2.ext1 != itemRowViewModelV2.ext1)
                    {
                        itemRowInformationV2.ext1 = itemRowViewModelV2.ext1;
                        isChangeV2 = true;
                    }
                    if (itemRowInformationV2.ext2 != itemRowViewModelV2.ext2)
                    {
                        itemRowInformationV2.ext2 = itemRowViewModelV2.ext2;
                        isChangeV2 = true;
                    }
                    newItemRowInformationListV2.Add(itemRowInformationV2);
                    continue;
                }

                ItemRowInformation newItemRowInformationV2 = new ItemRowInformation();
                newItemRowInformationV2.goodsequipmentGuid = goodsequipmentGuid;
                newItemRowInformationV2.rowColLayer = rowColLayer;
                newItemRowInformationV2.itemCode = itemRowViewModelV2.itemCode;
                newItemRowInformationV2.itemName = itemRowViewModelV2.itemName;
                newItemRowInformationV2.trayCode = itemRowViewModelV2.trayCode;
                newItemRowInformationV2.remarks = itemRowViewModelV2.remarks;
                newItemRowInformationV2.ext1 = itemRowViewModelV2.ext1;
                newItemRowInformationV2.ext2 = itemRowViewModelV2.ext2;
                newItemRowInformationListV2.Add(newItemRowInformationV2);
                isChangeV2 = true;
            }

            foreach (var oldItemRowInformationV2 in itemRowInformationList)
            {
                if (oldItemRowInformationV2 is null)
                {
                    continue;
                }
                string oldItemCodeV2 = oldItemRowInformationV2.itemCode ?? string.Empty;
                if (!currentItemCodeSetV2.Contains(oldItemCodeV2))
                {
                    isChangeV2 = true;
                    break;
                }
            }

            itemRowInformationList = newItemRowInformationListV2;
            return isChangeV2;
        }
        #endregion
        #region 获取指定仓库库区最大排列层
        /// <summary>
        /// 获取指定仓库库区最大排列层
        /// </summary>
        /// <param name="getAreaMaxRowColLayer"></param>
        /// <returns></returns>
        public MaxRowColLayer GetAreaMaxRowcollayer(GetAreaMaxRowColLayer getAreaMaxRowColLayer)
        {
            MaxRowColLayer maxRowColLayer = new MaxRowColLayer();
            var filterLocation = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == getAreaMaxRowColLayer.stockCode && it.area_code == getAreaMaxRowColLayer.areaCode && it.type == "货位");
            List<LocationSiteInformation> locationSiteInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filterLocation);
            double maxRow = 0;
            double maxCol = 0;
            double maxFloor = 0;
            foreach (var locationSiteInformation in locationSiteInformationList)
            {
                string row = locationSiteInformation.row;
                string col = locationSiteInformation.col;
                string floor = locationSiteInformation.floor;
                double rowDouble = 0;
                double colDouble = 0;
                double floorDouble = 0;
                if (!String.IsNullOrEmpty(row))
                {
                    rowDouble = double.Parse(row);
                }
                if (!String.IsNullOrEmpty(col))
                {
                    colDouble = double.Parse(col);
                }
                if (!String.IsNullOrEmpty(floor))
                {
                    floorDouble = double.Parse(floor);
                }
                if (rowDouble > maxRow)
                {
                    maxRow = rowDouble;
                }
                if (colDouble > maxCol)
                {
                    maxCol = colDouble;
                }
                if (floorDouble > maxFloor)
                {
                    maxFloor = floorDouble;
                }
            }
            maxRowColLayer.MaxRow = maxRow.ToString();
            maxRowColLayer.MaxCol = maxCol.ToString();
            maxRowColLayer.MaxLayer = maxFloor.ToString();
            return maxRowColLayer;
        }
        #endregion

        #region 同步货位
        /// <summary>
        /// 同步货位
        /// </summary>
        /// <param name="goodsequipmentno"></param>
        /// <returns></returns>
        public ReturnMessage SynchronizeStock(string goodsequipmentno)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            //LocationRealMonitorViewModel locationRealMonitorView = new LocationRealMonitorViewModel();
            //locationRealMonitorView.stock = new List<StockViewModel>();
            List<LocationRealMonitorViewModel> locationRealMonitorViewModelList = new List<LocationRealMonitorViewModel>();
            if(string.IsNullOrEmpty(goodsequipmentno))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "用户启用货位同步后，没有选择货位设备无法同步！";
                return returnMessage;
            }
            GoodscommandService goodscommandService = new Interfaces.Service.SenarioTesting.GoodscommandService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            GoodsequipmentService goodsequipmentService = new Interfaces.Service.SenarioTesting.GoodsequipmentService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            SettingService settingService = new Interfaces.Service.Sys.SettingService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            });
            int websocketPostNumber = 100;
            int websocketPostInterval = 1000;
            string websocketPostNumberString = settingService.GetWhere(it => it.cn_s_setting_keycode == "WebsocketPostNumber").Select(it => it.cn_s_setting_keyvalue).FirstOrDefault();
            if(!(websocketPostNumberString is null))
            {
                websocketPostNumber = int.Parse(websocketPostNumberString);
            }
            string websocketPostIntervalString = settingService.GetWhere(it => it.cn_s_setting_keycode == "WebsocketPostInterval").Select(it => it.cn_s_setting_keyvalue).FirstOrDefault();
            if(!(websocketPostIntervalString is null))
            {
                websocketPostInterval = int.Parse(websocketPostIntervalString);
            }
            tn_dts_goodsequipment goodsequipment = goodsequipmentService.GetFirst(it => it.cn_s_goodsequipment_no == goodsequipmentno);
            tn_dts_goodscommand initgoods = goodscommandService.GetFirst(it => it.cn_s_goodscommand_goodsequipguid == goodsequipment.cn_guid && it.cn_s_goodscommand_type == "初始化");
            List<tn_dts_goodscommand> goodsList = goodscommandService.GetWhere(it => it.cn_s_goodscommand_goodsequipguid == goodsequipment.cn_guid && it.cn_s_goodscommand_type == "业务");
            //List<StockViewModel> stockViewModelList = new List<StockViewModel>();
            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
            locationRealMonitorViewModel.stock = new List<StockViewModel>();
            if (!(initgoods is null))
            {
                string initJson = initgoods.cn_s_goodscommand_json;

                if (!(initJson is null))
                {
                    LocationRealMonitorViewModel initStock = new LocationRealMonitorViewModel();
                    initStock.stock = new List<StockViewModel>();
                    try
                    {
                        initStock = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(initJson);
                    }
                    catch (Exception exception)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "tn_dts_goodscommand表中货位设备编码为：" + goodsequipment.cn_s_goodsequipment_no + "记录的初始化指令Json无法转化，请检查后重试！";
                        return returnMessage;
                    }
                    finally
                    {
                        if(initStock is null)
                        {
                            initStock = new LocationRealMonitorViewModel();
                            initStock.stock = new List<StockViewModel>();
                        }
                    }

                    if (initStock.stock.Count > 0)
                        StorageDriver.Instance.PublishStocksInfo(initStock.stock);

                    foreach (var stockView in initStock.stock)
                    {
                        //locationRealMonitorView.stock.Add(stockView);
                        if(locationRealMonitorViewModel.stock.Count < websocketPostNumber)
                        {
                            locationRealMonitorViewModel.stock.Add(stockView);
                        }
                        else
                        {
                            locationRealMonitorViewModelList.Add(locationRealMonitorViewModel);
                            locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                        }
                    }
                    if (goodsList.Count != 0)
                    {
                        foreach (var goods in goodsList)
                        {
                            string proJson = goods.cn_s_goodscommand_json;
                            if (proJson is null)
                            {
                                continue;
                            }

                            LocationRealMonitorViewModel proStock = new LocationRealMonitorViewModel();
                            proStock.stock = new List<StockViewModel>();
                            try
                            {
                                proStock = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(proJson);
                            }
                            catch(Exception exception)
                            {
                                returnMessage.IsSuccess = false;
                                returnMessage.Message = "tn_dts_goodscommand表中货位设备编码为：" + goodsequipment.cn_s_goodsequipment_no + "记录的业务指令Json无法转化，请检查后重试！";
                                return returnMessage;
                            }
                            finally
                            {
                                if(proStock is null)
                                {
                                    proStock = new LocationRealMonitorViewModel();
                                    proStock.stock = new List<StockViewModel>();
                                }
                            }

                            if (proStock.stock.Count > 0)
                                StorageDriver.Instance.PublishStocksInfo(proStock.stock);

                            foreach (var stockView in proStock.stock)
                            {
                                //locationRealMonitorView.stock.Add(stockView);
                                if (locationRealMonitorViewModel.stock.Count < websocketPostNumber)
                                {
                                    locationRealMonitorViewModel.stock.Add(stockView);
                                }
                                else
                                {
                                    locationRealMonitorViewModelList.Add(locationRealMonitorViewModel);
                                    locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                                    locationRealMonitorViewModel.stock = new List<StockViewModel>();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (goodsList.Count != 0)
                {
                    foreach (var goods in goodsList)
                    {
                        string proJson = goods.cn_s_goodscommand_json;
                        if (proJson is null)
                        {
                            continue;
                        }
                        LocationRealMonitorViewModel proStock = new LocationRealMonitorViewModel();
                        proStock.stock = new List<StockViewModel>();
                        try
                        {
                            proStock = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(proJson);
                        }
                        catch (Exception exception)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "tn_dts_goodscommand表中货位设备编码为：" + goodsequipment.cn_s_goodsequipment_no + "记录有业务指令Json无法转化，请检查后重试！";
                            return returnMessage;
                        }
                        finally
                        {
                            if(proStock is null)
                            {
                                proStock = new LocationRealMonitorViewModel();
                                proStock.stock = new List<StockViewModel>();
                            }
                        }

                        if (proStock.stock.Count > 0)
                            StorageDriver.Instance.PublishStocksInfo(proStock.stock);

                        foreach (var stockView in proStock.stock)
                        {
                            //locationRealMonitorView.stock.Add(stockView);
                            if (locationRealMonitorViewModel.stock.Count < websocketPostNumber)
                            {
                                locationRealMonitorViewModel.stock.Add(stockView);
                            }
                            else
                            {
                                locationRealMonitorViewModelList.Add(locationRealMonitorViewModel);
                                locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                                locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            }
                        }
                    }
                }
            }
            locationRealMonitorViewModelList.Add(locationRealMonitorViewModel);
            //foreach (var item in locationRealMonitorViewModelList)
            //{
            //    string sendJSONString = JsonConvert.SerializeObject(item);
            //    Task.Run(async () => { WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString); });
            //    Thread.Sleep(websocketPostInterval);
            //}
            SenarioTestingProcess.AsyncWebSocketStock(locationRealMonitorViewModelList, SenarioTestingProcess.tokensource, SenarioTestingProcess.manualResetEvent, websocketPostInterval);
            //if (locationRealMonitorView.stock.Count != 0)
            //{
            //    string sendJSONString = JsonConvert.SerializeObject(locationRealMonitorView);
            //    Task.Run(async () => { WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString); });
            //}
            returnMessage.IsSuccess = true;
            returnMessage.Message = "";
            return returnMessage;
        }
        #endregion
    }
}
