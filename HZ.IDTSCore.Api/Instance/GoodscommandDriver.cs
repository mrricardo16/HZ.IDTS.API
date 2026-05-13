using HZ.IDTSCore.Api.Controllers;
using HZ.IDTSCore.Api.Global;
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

        //        #region 判定货位信息是否修改
        //        /// <summary>
        //        /// 判定货位信息是否修改
        //        /// </summary>
        //        /// <param name="stockItemInformationList"></param>
        //        /// <param name="stockViewModelList"></param>
        //        /// <param name="goodsequipmentGuid"></param>
        //        /// 只考虑增改，没考虑删
        //        /// <returns></returns>
        //        public bool WatchStockItemInformation(out List<StockItemInformation> stockItemInformationList, List<StockViewModel> stockViewModelList, string goodsequipmentGuid)
        //        {
        //            bool isChange = false;
        //            stockItemInformationList = new List<StockItemInformation>();
        //            foreach (var stockViewModel in stockViewModelList)
        //            {
        //                var stockItemInformationBuilder = Builders<StockItemInformation>.Filter;
        //                var updateFilter = stockItemInformationBuilder.And(stockItemInformationBuilder.Eq("goodsequipmentGuid", goodsequipmentGuid), stockItemInformationBuilder.Eq("locationCode", stockViewModel.locationCode));
        //                StockItemInformation updatestock = MongoDBSingleton.Instance.FindOneFilter(updateFilter);
        //                if (updatestock is null)
        //                {
        //                    StockItemInformation stockItemInformation = new StockItemInformation();
        //                    stockItemInformation.goodsequipmentGuid = goodsequipmentGuid;
        //                    stockItemInformation.stockCode = stockViewModel.stockCode;
        //                    stockItemInformation.areaCode = stockViewModel.areaCode;
        //                    stockItemInformation.locationCode = stockViewModel.locationCode;
        //                    stockItemInformation.locationType = stockViewModel.locationType;
        //                    stockItemInformation.state = stockViewModel.state;
        //                    stockItemInformation.storageState = stockViewModel.storageState;
        //                    List<ItemRowInformation> itemRowInformationList = new List<ItemRowInformation>();
        //                    foreach (var item in stockViewModel.itemRow)
        //                    {
        //                        ItemRowInformation itemRowInformation = new ItemRowInformation();
        //                        itemRowInformation.goodsequipmentGuid = goodsequipmentGuid;
        //                        itemRowInformation.rowColLayer = stockViewModel.locationCode;
        //                        itemRowInformation.itemCode = item.itemCode;
        //                        itemRowInformation.itemName = item.itemName;
        //                        itemRowInformation.trayCode = item.trayCode;
        //                        itemRowInformation.remarks = item.remarks;
        //                        itemRowInformation.ext1 = item.ext1;
        //                        itemRowInformation.ext2 = item.ext2;
        //                        itemRowInformationList.Add(itemRowInformation)
        //;
        //                    }
        //                    stockItemInformation.itemRow = itemRowInformationList;
        //                    stockItemInformationList.Add(stockItemInformation);
        //                    isChange = true;
        //                }
        //                else
        //                {
        //                    if (updatestock.state != stockViewModel.state)
        //                    {
        //                        updatestock.state = stockViewModel.state;
        //                        isChange = true;
        //                    }
        //                    if (updatestock.storageState != stockViewModel.storageState)
        //                    {
        //                        updatestock.storageState = stockViewModel.storageState;
        //                        isChange = true;
        //                    }
        //                    List<ItemRowInformation> itemRowViewModelList = updatestock.itemRow;
        //                    if (WatchItemRow(ref itemRowViewModelList, stockViewModel.itemRow, goodsequipmentGuid, stockViewModel.locationCode))
        //                    {
        //                        updatestock.itemRow = itemRowViewModelList;
        //                        isChange = true;
        //                    }
        //                    stockItemInformationList.Add(updatestock);
        //                }

        //            }
        //            return isChange;
        //        }
        //        #endregion

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

        //        #region 判定货位信息是否修改
        //        /// <summary>
        //        /// 判定货位信息是否修改
        //        /// </summary>
        //        /// <param name="stockItemInformationList"></param>
        //        /// <param name="stockViewModelList"></param>
        //        /// <param name="goodsequipmentGuid"></param>
        //        /// <returns></returns>
        //        public bool WatchStockItemInformation(out List<StockItemInformation> stockItemInformationList, List<StockViewModel> stockViewModelList, string goodsequipmentGuid)
        //        {
        //            bool isChange = false;
        //            var stockItemInformationBuilder = Builders<StockItemInformation>.Filter;
        //            stockItemInformationList = new List<StockItemInformation>();
        //            List<StockItemInformation> stockItemInformationNoDeleteList = new List<StockItemInformation>();
        //            var aGoodsequipmentFilter = stockItemInformationBuilder.Eq("goodsequipmentGuid", goodsequipmentGuid);
        //            List<StockItemInformation> oldGoodsequipmentStockList = MongoDBSingleton.Instance.FindList(aGoodsequipmentFilter);
        //            foreach (var stockViewModel in stockViewModelList)
        //            {
        //                var updateFilter = stockItemInformationBuilder.And(stockItemInformationBuilder.Eq("goodsequipmentGuid", goodsequipmentGuid), stockItemInformationBuilder.Eq("locationCode", stockViewModel.locationCode));
        //                StockItemInformation updatestock = MongoDBSingleton.Instance.FindOneFilter(updateFilter);
        //                if (updatestock is null)
        //                {
        //                    StockItemInformation stockItemInformation = new StockItemInformation();
        //                    stockItemInformation.goodsequipmentGuid = goodsequipmentGuid;

        //                    stockItemInformation.stockCode = stockViewModel.stockCode;
        //                    stockItemInformation.areaCode = stockViewModel.areaCode;
        //                    stockItemInformation.locationCode = stockViewModel.locationCode;
        //                    stockItemInformation.locationType = stockViewModel.locationType;
        //                    stockItemInformation.state = stockViewModel.state;
        //                    stockItemInformation.storageState = stockViewModel.storageState;
        //                    List<ItemRowInformation> itemRowInformationList = new List<ItemRowInformation>();
        //                    foreach (var item in stockViewModel.itemRow)
        //                    {
        //                        ItemRowInformation itemRowInformation = new ItemRowInformation();
        //                        itemRowInformation.goodsequipmentGuid = goodsequipmentGuid;
        //                        itemRowInformation.rowColLayer = stockViewModel.locationCode;
        //                        itemRowInformation.itemCode = item.itemCode;
        //                        itemRowInformation.itemName = item.itemName;
        //                        itemRowInformation.trayCode = item.trayCode;
        //                        itemRowInformation.remarks = item.remarks;
        //                        itemRowInformation.ext1 = item.ext1;
        //                        itemRowInformation.ext2 = item.ext2;
        //                        itemRowInformationList.Add(itemRowInformation)
        //;
        //                    }
        //                    stockItemInformation.itemRow = itemRowInformationList;
        //                    stockItemInformationList.Add(stockItemInformation);

        //                    isChange = true;
        //                }
        //                else
        //                {
        //                    if (updatestock.state != stockViewModel.state)
        //                    {
        //                        updatestock.state = stockViewModel.state;
        //                        isChange = true;
        //                    }
        //                    if (updatestock.storageState != stockViewModel.storageState)
        //                    {
        //                        updatestock.storageState = stockViewModel.storageState;
        //                        isChange = true;
        //                    }
        //                    List<ItemRowInformation> itemRowViewModelList = updatestock.itemRow;
        //                    if (WatchItemRow(ref itemRowViewModelList, stockViewModel.itemRow, goodsequipmentGuid, stockViewModel.locationCode))
        //                    {
        //                        updatestock.itemRow = itemRowViewModelList;
        //                        isChange = true;
        //                    }
        //                    stockItemInformationList.Add(updatestock);
        //                }
        //            }
        //            foreach (var oldGoodsequipmentStock in oldGoodsequipmentStockList)
        //            {
        //                StockItemInformation stockItemInformationLocationCode = stockItemInformationList.FirstOrDefault(it => it.locationCode == oldGoodsequipmentStock.locationCode);
        //                if (stockItemInformationLocationCode is null)
        //                {
        //                    MongoDBSingleton.Instance.Delete<StockItemInformation>(oldGoodsequipmentStock._id.ToString());
        //                }
        //            }
        //            return isChange;
        //        }
        //        #endregion

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
