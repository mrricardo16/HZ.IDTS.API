using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using MongoDB.Driver;
using HZ.iWCS.MData.Core;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class GoodscommandService : BaseService<tn_dts_goodscommand>, IGoodscommandService
    {
        public GoodscommandService(SessionInfo session) : base(session)
        {

        }

        #region 保存货位指令
        /// <summary>
        /// 保存货位指令
        /// </summary>
        /// <param name="saveGoodscommand"></param>
        /// <returns></returns>
        public ReturnMessage SaveGoodscommand(SaveGoodscommand saveGoodscommand)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            string addOrModify = saveGoodscommand.AddOrModify;
            tn_dts_goodscommand goodscommand = saveGoodscommand.NewGoodscommand;
            if (addOrModify == "add")
            {
                if (!(Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_no == goodscommand.cn_s_goodscommand_no).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "指令编码不能重复，请重试!";
                    return returnMessage;
                }
                tn_dts_goodscommand newGoodscommand = new tn_dts_goodscommand();
                newGoodscommand.cn_guid = Guid.NewGuid().ToString();
                newGoodscommand.cn_s_goodscommand_goodsequipguid = goodscommand.cn_s_goodscommand_goodsequipguid;
                newGoodscommand.cn_s_goodscommand_no = goodscommand.cn_s_goodscommand_no;
                newGoodscommand.cn_s_goodscommand_name = goodscommand.cn_s_goodscommand_name;
                newGoodscommand.cn_s_goodscommand_type = goodscommand.cn_s_goodscommand_type;
                //newGoodscommand.cn_n_goodscommand_haswildcard = goodscommand.cn_n_goodscommand_haswildcard;
                newGoodscommand.cn_s_goodscommand_json = goodscommand.cn_s_goodscommand_json;
                newGoodscommand.cn_s_goodscommand_remarks = goodscommand.cn_s_goodscommand_remarks;
                newGoodscommand.cn_s_creator = user.UserCode;
                newGoodscommand.cn_s_creator_by = user.UserName;
                newGoodscommand.cn_t_create = DateTime.Now;
                tn_dts_logs goodscommandLog = new tn_dts_logs();
                goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                goodscommandLog.cn_s_logs_type = "操作";
                goodscommandLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_goodscommand表中新增一条货位指令记录，详细信息为" + JsonConvert.SerializeObject(newGoodscommand);
                goodscommandLog.cn_t_create = DateTime.Now;
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_goodscommand>(newGoodscommand).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(goodscommandLog).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "保存接口新增货位指令失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败!";
                    return returnMessage;
                }
            }
            else if (addOrModify == "modify")
            {
                if (!(Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_no == goodscommand.cn_s_goodscommand_no && it.cn_guid != goodscommand.cn_guid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "指令编码不能重复，请重试!";
                    return returnMessage;
                }
                tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == goodscommand.cn_guid).First();
                goodscommandGuid.cn_s_goodscommand_goodsequipguid = goodscommand.cn_s_goodscommand_goodsequipguid;
                string oldGoodscommandNo = goodscommandGuid.cn_s_goodscommand_no;
                goodscommandGuid.cn_s_goodscommand_no = goodscommand.cn_s_goodscommand_no;
                goodscommandGuid.cn_s_goodscommand_name = goodscommand.cn_s_goodscommand_name;
                goodscommandGuid.cn_s_goodscommand_type = goodscommand.cn_s_goodscommand_type;
                //goodscommandGuid.cn_n_goodscommand_haswildcard = goodscommand.cn_n_goodscommand_haswildcard;
                goodscommandGuid.cn_s_goodscommand_json = goodscommand.cn_s_goodscommand_json;
                goodscommandGuid.cn_s_goodscommand_remarks = goodscommand.cn_s_goodscommand_remarks;
                goodscommandGuid.cn_s_modify = user.UserCode;
                goodscommandGuid.cn_s_modify_by = user.UserName;
                goodscommandGuid.cn_t_modify = DateTime.Now;
                tn_dts_logs goodscommandLog = new tn_dts_logs();
                goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                goodscommandLog.cn_s_logs_type = "操作";
                goodscommandLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户修改了tn_dts_goodscommand表中货位指令唯一标识为：" + oldGoodscommandNo + "，详细信息为" + JsonConvert.SerializeObject(goodscommandGuid);
                goodscommandLog.cn_t_create = DateTime.Now;
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_goodscommand>(goodscommandGuid).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(goodscommandLog).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "保存接口修改货位指令失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败!";
                    return returnMessage;
                }
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的add_or_modify参数只能为add或modify！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 判断同步数据是否需要缓存（0：不需要；1：需要新增；2：需要修改；3：Json有问题）
        /// <summary>
        /// 判断同步数据是否需要缓存（0：不需要；1：需要新增；2：需要修改；3：Json有问题）
        /// </summary>
        /// <param name="virequi"></param>
        /// <param name="synchronizeJson"></param>
        /// <returns></returns>
        public int DetermineSynchronizeExist(string virequi, string synchronizeJson, ref tn_dts_goodscommand updateGoodscommand)
        {
            int returnResult = 0;
            updateGoodscommand = new tn_dts_goodscommand();
            tn_dts_goodsequipment goodsequipmentNo = Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_s_goodsequipment_no == virequi).First();
            tn_dts_goodscommand initGoodscommand = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentNo.cn_guid && it.cn_s_goodscommand_type == "初始化").First();
            List<tn_dts_goodscommand> goodscommandList = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentNo.cn_guid && it.cn_s_goodscommand_type == "业务").ToList();
            List<DetermineSynchronizeModel> determineSynchronizeModelList = new List<DetermineSynchronizeModel>();
            LocationRealMonitorViewModel initStock = new LocationRealMonitorViewModel();
            initStock.stock = new List<StockViewModel>();
            try
            {
                initStock = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(initGoodscommand.cn_s_goodscommand_json);
            }
            catch
            {
                returnResult = 3;
                return returnResult;
            }
            finally
            {
                initStock = new LocationRealMonitorViewModel();
                initStock.stock = new List<StockViewModel>();
            }
            foreach (var item in initStock.stock)
            {
                DetermineSynchronizeModel determineSynchronizeModel = new DetermineSynchronizeModel();
                determineSynchronizeModel.goodscommandGuid = initGoodscommand.cn_guid;
                determineSynchronizeModel.stockView = item;
                determineSynchronizeModelList.Add(determineSynchronizeModel);
            }
            foreach (var goodscommand in goodscommandList)
            {
                try
                {
                    initStock = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommand.cn_s_goodscommand_json);
                }
                catch
                {
                    returnResult = 3;
                    return returnResult;
                }
                finally
                {
                    if(initStock is null)
                    {
                        initStock = new LocationRealMonitorViewModel();
                        initStock.stock = new List<StockViewModel>();
                    }
                }
                if(initStock.stock.Count > 0)
                {
                    DetermineSynchronizeModel determineSynchronizeModel = new DetermineSynchronizeModel();
                    determineSynchronizeModel.goodscommandGuid = goodscommand.cn_guid;
                    determineSynchronizeModel.stockView = initStock.stock[0];
                    determineSynchronizeModelList.Add(determineSynchronizeModel);
                }          
            }
            try
            {
                initStock = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(synchronizeJson);
            }
            catch
            {
                returnResult = 3;
                return returnResult;
            }
            finally
            {
                initStock = new LocationRealMonitorViewModel();
                initStock.stock = new List<StockViewModel>();
            }
            if(initStock.stock.Count == 0)
            {
                returnResult = 0;
                return returnResult;
            }
            StockViewModel stock = initStock.stock[0];
            foreach (var determineSynchronizeModel in determineSynchronizeModelList)
            {
                StockViewModel stockViewModel = determineSynchronizeModel.stockView;
                if (stock.stockCode != stockViewModel.stockCode || stock.areaCode != stockViewModel.areaCode || stock.locationCode != stockViewModel.locationCode || stock.locationType != stockViewModel.locationType)
                {
                    continue;
                }
                if (stock.state == stockViewModel.state && stock.storageState == stockViewModel.storageState && DetermineitemRowSame(stock.itemRow, stockViewModel.itemRow))
                {
                    returnResult = 0;
                    return returnResult;
                }
                else
                {
                    returnResult = 2;
                    updateGoodscommand = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == determineSynchronizeModel.goodscommandGuid).First();
                    return returnResult;
                }
            }
            returnResult = 1;
            return returnResult;
        }
        #endregion

        #region 判断货位数据同步项中物料信息是否相同
        /// <summary>
        /// 判断货位数据同步项中物料信息是否相同
        /// </summary>
        /// <param name="itemRowViewListOne"></param>
        /// <param name="itemRowViewListTwo"></param>
        /// <returns></returns>
        public bool DetermineitemRowSame(List<ItemRowViewModel> itemRowViewListOne, List<ItemRowViewModel> itemRowViewListTwo)
        {
            bool returnResult = false;
            if (itemRowViewListOne.Count != itemRowViewListTwo.Count)
            {
                return returnResult;
            }
            for (int i = 0; i < itemRowViewListOne.Count; i++)
            {
                if (itemRowViewListOne[i].itemCode != itemRowViewListTwo[i].itemCode || itemRowViewListOne[i].itemName != itemRowViewListTwo[i].itemName || itemRowViewListOne[i].trayCode != itemRowViewListTwo[i].trayCode || itemRowViewListOne[i].remarks != itemRowViewListTwo[i].remarks || itemRowViewListOne[i].ext1 != itemRowViewListTwo[i].ext1 || itemRowViewListOne[i].ext2 != itemRowViewListTwo[i].ext2)
                {
                    return returnResult;
                }
            }
            returnResult = true;
            return returnResult;
        }
        #endregion

        #region 批量添加货位指令
        /// <summary>
        /// 批量添加货位指令
        /// </summary>
        /// <param name="batchAddDate"></param>
        /// <returns></returns>
        public ReturnMessage BatchAddGoodscommand(BatchAddGoodscommandDate batchAddDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<string> goodsequipmentGuidList = batchAddDate.SelectiveGoodsequipmentGuidList;
            foreach (var goodsequipmentGuid in goodsequipmentGuidList)
            {
                if (Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_guid == goodsequipmentGuid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "需要添加指令的设备中有唯一标识不存在的设备，请检查后重试！";
                    return returnMessage;
                }
            }
            if (batchAddDate.Haswildcard == 0 && goodsequipmentGuidList.Count > 1)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "不含通配符的货位指令无法批量（超过一个设备）添加，请重试！";
                return returnMessage;
            }
            List<tn_dts_goodscommand> goodscommandList = new List<tn_dts_goodscommand>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var goodsequipmentGuid in goodsequipmentGuidList)
            {
                string commandGuid = Guid.NewGuid().ToString();
                tn_dts_goodscommand goodscommand = new tn_dts_goodscommand();
                goodscommand.cn_guid = commandGuid;
                goodscommand.cn_s_goodscommand_goodsequipguid = goodsequipmentGuid;
                goodscommand.cn_s_goodscommand_no = commandGuid.Substring(0, 32);
                goodscommand.cn_s_goodscommand_name = batchAddDate.GoodscommandName;
                goodscommand.cn_s_goodscommand_type = batchAddDate.GoodscommandType;
                goodscommand.cn_n_goodscommand_haswildcard = batchAddDate.Haswildcard;
                goodscommand.cn_s_goodscommand_json = batchAddDate.Json;
                goodscommand.cn_s_creator = user.UserCode;
                goodscommand.cn_s_creator_by = user.UserName;
                goodscommand.cn_t_create = DateTime.Now;
                goodscommandList.Add(goodscommand);
                tn_dts_logs goodscommandLog = new tn_dts_logs();
                goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                goodscommandLog.cn_s_logs_type = "操作";
                goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令管理批量增加功能向tn_dts_goodscommand表中新增一条货位指令，详细信息为：" + JsonConvert.SerializeObject(goodscommand);
                goodscommandLog.cn_t_create = DateTime.Now;
                logList.Add(goodscommandLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_goodscommand>(goodscommandList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("2.7货位指令（Goodscommand）管理批量添加货位指令失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量增加失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量增加成功！";
            return returnMessage;
        }
        #endregion

        #region 修改货位指令
        /// <summary>
        /// 修改货位指令
        /// </summary>
        /// <param name="goodscommand"></param>
        /// <returns></returns>
        public ReturnMessage UpdateGoodscommand(tn_dts_goodscommand goodscommand)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == goodscommand.cn_guid).First();
            if (goodscommandGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_goodscommand表中找不到唯一标识为：" + goodscommand.cn_guid + "的货位指令记录，请检查后重试！";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_no == goodscommand.cn_s_goodscommand_no && it.cn_guid != goodscommand.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "指令编码不能重复，请重试！";
                return returnMessage;
            }
            goodscommandGuid.cn_s_goodscommand_no = goodscommand.cn_s_goodscommand_no;
            goodscommandGuid.cn_s_goodscommand_name = goodscommand.cn_s_goodscommand_name;
            goodscommandGuid.cn_s_goodscommand_type = goodscommand.cn_s_goodscommand_type;
            goodscommandGuid.cn_n_goodscommand_haswildcard = goodscommand.cn_n_goodscommand_haswildcard;
            goodscommandGuid.cn_s_goodscommand_json = goodscommand.cn_s_goodscommand_json;
            goodscommandGuid.cn_s_goodscommand_remarks = goodscommand.cn_s_goodscommand_remarks;
            goodscommandGuid.cn_s_modify = user.UserCode;
            goodscommandGuid.cn_s_modify_by = user.UserName;
            goodscommandGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs goodscommandLog = new tn_dts_logs();
            goodscommandLog.cn_guid = Guid.NewGuid().ToString();
            goodscommandLog.cn_s_logs_type = "操作";
            goodscommandLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用修改货位指令功能修改了一条货位指令，详细信息：" + JsonConvert.SerializeObject(goodscommandGuid);
            goodscommandLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_goodscommand>(goodscommandGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(goodscommandLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("2.8货位指令（Goodscommand）管理修改货位指令失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改货位指令失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改货位指令成功！";
            return returnMessage;
        }
        #endregion

        #region 编辑模版
        /// <summary>
        /// 编辑模版
        /// </summary>
        /// <param name="editGoodscommandDate"></param>
        /// <returns></returns>
        public ReturnMessage EditGoodscommandJson(EditGoodscommandDate editGoodscommandDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == editGoodscommandDate.GoodscommandGuid).First();
            if (goodscommandGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_goodscommand表中找不到唯一标识为：" + editGoodscommandDate.GoodscommandGuid + "的货位指令记录，请检查后重试！";
                return returnMessage;
            }
            goodscommandGuid.cn_n_goodscommand_haswildcard = editGoodscommandDate.Haswildcard;
            goodscommandGuid.cn_s_goodscommand_json = editGoodscommandDate.Json;
            goodscommandGuid.cn_s_modify = user.UserCode;
            goodscommandGuid.cn_s_modify_by = user.UserName;
            goodscommandGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs goodscommandLog = new tn_dts_logs();
            goodscommandLog.cn_guid = Guid.NewGuid().ToString();
            goodscommandLog.cn_s_logs_type = "操作";
            goodscommandLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用编辑模版功能修改了一条货位指令Json，详细信息：" + JsonConvert.SerializeObject(goodscommandGuid);
            goodscommandLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_goodscommand>(goodscommandGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(goodscommandLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("2.9货位指令（Goodscommand）管理编辑货位指令失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "编辑模版失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "编辑模版成功！";
            return returnMessage;
        }
        #endregion

        #region 按指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// <summary>
        /// 按指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<GoodscommandPlus> GetPageList(PageParm param)
        {
            string goodscommandNo = param.Parms["cn_s_goodscommand_no"].ObjToString();
            string goodscommandName = param.Parms["cn_s_goodscommand_name"].ObjToString();
            string goodscommandType = param.Parms["cn_s_goodscommand_type"].ObjToString();
            string goodsequipmentGuidListString = param.Parms["cn_s_goodsequipment_guid"].ObjToString();
            List<string> goodsequipmentGuidList = JsonConvert.DeserializeObject<List<string>>(goodsequipmentGuidListString);
            List<GoodscommandPlus> goodscommandAllList = new List<GoodscommandPlus>();
            foreach (var goodsequipmentGuid in goodsequipmentGuidList)
            {
                List<GoodscommandPlus> goodcommandList = Db.Queryable<tn_dts_goodscommand>()
                .InnerJoin<tn_dts_goodsequipment>((gc, ge) => gc.cn_s_goodscommand_goodsequipguid == ge.cn_guid)
                .Where(gc => gc.cn_s_goodscommand_goodsequipguid == goodsequipmentGuid)
                .WhereIF(!string.IsNullOrEmpty(goodscommandNo), gc => gc.cn_s_goodscommand_no.Contains(goodscommandNo))
                .WhereIF(!string.IsNullOrEmpty(goodscommandName), gc => gc.cn_s_goodscommand_name.Contains(goodscommandName))
                .WhereIF(!string.IsNullOrEmpty(goodscommandType), gc => gc.cn_s_goodscommand_type == goodscommandType)
                .Select((gc, ge) => new GoodscommandPlus
                {
                    cn_guid = gc.cn_guid,
                    cn_s_goodscommand_goodsequipmentno = ge.cn_s_goodsequipment_no,
                    cn_s_goodscommand_goodsequipguid = gc.cn_s_goodscommand_goodsequipguid,
                    cn_s_goodscommand_no = gc.cn_s_goodscommand_no,
                    cn_s_goodscommand_name = gc.cn_s_goodscommand_name,
                    cn_s_goodscommand_type = gc.cn_s_goodscommand_type,
                    cn_n_goodscommand_haswildcard = gc.cn_n_goodscommand_haswildcard,
                    cn_s_goodscommand_json = gc.cn_s_goodscommand_json,
                    cn_s_goodscommand_remarks = gc.cn_s_goodscommand_remarks,
                    cn_s_modify = gc.cn_s_modify,
                    cn_s_modify_by = gc.cn_s_modify_by,
                    cn_t_modify = gc.cn_t_modify,
                    cn_s_creator = gc.cn_s_creator,
                    cn_s_creator_by = gc.cn_s_creator_by,
                    cn_t_create = gc.cn_t_create
                })
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                .ToList();
                goodscommandAllList.AddRange(goodcommandList);
            }
            return goodscommandAllList.OrderByDescending(it => it.cn_t_modify).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 复制货位指令
        /// <summary>
        /// 复制货位指令
        /// </summary>
        /// <param name="copyGoodscommandDate"></param>
        /// <returns></returns>
        public ReturnMessage CopyGoodscommand(CopyGoodscommandDate copyGoodscommandDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<string> goodscommandguidList = copyGoodscommandDate.GoodscommandGuidList;
            List<string> goodsequipmentguidList = copyGoodscommandDate.GoodsequipmentGuidList;
            List<tn_dts_goodscommand> goodscommandGuidList = new List<tn_dts_goodscommand>();
            foreach (var goodscommandguid in goodscommandguidList)
            {
                tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == goodscommandguid).First();
                if (goodscommandGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选择需要复制的货位指令中有指令的唯一标识不存在，请检查后重试！";
                    return returnMessage;
                }
                if (goodscommandGuid.cn_n_goodscommand_haswildcard == 0)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选择需要复制的货位指令中有指令不含通配符，请检查后重试！";
                    return returnMessage;
                }
                goodscommandGuidList.Add(goodscommandGuid);
            }
            List<tn_dts_goodscommand> goodscommandList = new List<tn_dts_goodscommand>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var goodsequipmentguid in goodsequipmentguidList)
            {
                if (Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_guid == goodsequipmentguid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选择需要粘贴的货位设备中有设备的唯一标识不存在，请检查后重试！";
                    return returnMessage;
                }
                foreach (var goodscommandGuid in goodscommandGuidList)
                {
                    string commandGuid = Guid.NewGuid().ToString();
                    tn_dts_goodscommand goodscommand = new tn_dts_goodscommand();
                    goodscommand.cn_guid = commandGuid;
                    goodscommand.cn_s_goodscommand_goodsequipguid = goodsequipmentguid;
                    goodscommand.cn_s_goodscommand_no = commandGuid.Substring(0, 32);
                    goodscommand.cn_s_goodscommand_name = goodscommandGuid.cn_s_goodscommand_name;
                    goodscommand.cn_s_goodscommand_type = goodscommandGuid.cn_s_goodscommand_type;
                    goodscommand.cn_n_goodscommand_haswildcard = goodscommandGuid.cn_n_goodscommand_haswildcard;
                    goodscommand.cn_s_goodscommand_json = goodscommandGuid.cn_s_goodscommand_json;
                    goodscommand.cn_s_creator = user.UserCode;
                    goodscommand.cn_s_creator_by = user.UserName;
                    goodscommand.cn_t_create = DateTime.Now;
                    goodscommandList.Add(goodscommand);
                    tn_dts_logs goodscommandLog = new tn_dts_logs();
                    goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                    goodscommandLog.cn_s_logs_type = "操作";
                    goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令管理复制货位指令功能向tn_dts_goodscommand表中新增一条货位指令，详细信息为：" + JsonConvert.SerializeObject(goodscommand);
                    goodscommandLog.cn_t_create = DateTime.Now;
                    logList.Add(goodscommandLog);
                }
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_goodscommand>(goodscommandList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("2.11货位指令（Goodscommand）管理复制货位指令失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "复制货位指令失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "复制货位指令成功！";
            return returnMessage;
        }
        #endregion

        #region 批量删除货位指令
        ///// <summary>
        ///// 批量删除货位指令
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteGoodscommand(List<string> guidList)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    List<tn_dts_goodscommand> goodscommandGuidList = new List<tn_dts_goodscommand>();
        //    List<tn_dts_logs> logList = new List<tn_dts_logs>();
        //    foreach (var guid in guidList)
        //    {
        //        tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == guid).First();
        //        if (goodscommandGuid is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "已选货位指令中有货位指令的唯一标识不存在，请检查后重试！";
        //            return returnMessage;
        //        }
        //        goodscommandGuidList.Add(goodscommandGuid);
        //        tn_dts_logs goodscommandLog = new tn_dts_logs();
        //        goodscommandLog.cn_guid = Guid.NewGuid().ToString();
        //        goodscommandLog.cn_s_logs_type = "操作";
        //        goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令管理批量删除货位指令功能删除一条tn_dts_goodscommand货位指令，详细信息为：" + JsonConvert.SerializeObject(goodscommandGuid);
        //        goodscommandLog.cn_t_create = DateTime.Now;
        //        logList.Add(goodscommandLog);
        //    }
        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        dbTran.Deleteable<tn_dts_goodscommand>(goodscommandGuidList).ExecuteCommand();
        //        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        LogHelper.Error("2.12货位指令（Goodscommand）管理批量删除货位指令失败，详细信息为：" + res.Message);
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除货位指令失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "批量删除货位指令成功！";
        //    return returnMessage;
        //}

        /// <summary>
        /// 批量删除货位指令
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteGoodscommand(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令管理批量删除货位指令功能删除了" + guidList.Count + "条tn_dts_goodscommand货位指令";
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_goodscommand>().In(guidList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("2.12货位指令（Goodscommand）管理批量删除货位指令失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除货位指令失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量删除货位指令成功！";
            return returnMessage;
        }
        #endregion

        #region 按指令编码和指令名称混合模糊分页查询
        /// <summary>
        /// 按指令编码和指令名称混合模糊分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<GoodscommandInformation> GetGoodscommandInformationPages(PageParm param)
        {
            string goodsequipmentGuid = param.Parms["cn_s_goodsequipment_guid"].ObjToString();
            string goodscommandNoOrName = param.Parms["cn_s_goodscommand_no_or_name"].ObjToString();
            List<tn_dts_goodscommand> goodscommandList = Db.Queryable<tn_dts_goodscommand>()
                .Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentGuid && it.cn_s_goodscommand_type == "执行")
                .WhereIF(!string.IsNullOrEmpty(goodscommandNoOrName), it => it.cn_s_goodscommand_no.Contains(goodscommandNoOrName) || it.cn_s_goodscommand_name.Contains(goodscommandNoOrName))
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_create asc" : param.OrderBy).ToList();
            return goodscommandList.Select(it =>
            new GoodscommandInformation
            {
                GoodscommandGuid = it.cn_guid,
                GoodscommandNo = it.cn_s_goodscommand_no,
                GoodscommandName = it.cn_s_goodscommand_name,
            }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 批量添加库位
        /// <summary>
        /// 批量添加库位
        /// </summary>
        /// <param name="batchAddLocationDate"></param>
        /// <returns></returns>
        public ReturnMessage BatchAddLocation(BatchAddLocationDate batchAddLocationDate, MaxRowColLayer maxRowColLayer)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            string goodsequipmentguid = batchAddLocationDate.GoodsequipmentGuid;
            string addMode = batchAddLocationDate.AddMode;
            StockViewModel stockViewModel = batchAddLocationDate.StockViewModel;
            var builder = Builders<StockItemInformation>.Filter;
            List<StockViewModel> stockViewModelAddList = new List<StockViewModel>();
            List<StockViewModel> stockViewModelModifyInitList = new List<StockViewModel>();
            //List<StockViewModel> stockViewModelModifyBusList = new List<StockViewModel>();
            List<tn_dts_goodscommand> modifyList = new List<tn_dts_goodscommand>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            if (addMode == "整排")
            {
                string rowString = stockViewModel.locationCode.Split("-")[0];
                int row = 0;
                if (string.IsNullOrEmpty(rowString) || !int.TryParse(rowString, out row))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的排数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                int maxCol = int.Parse(maxRowColLayer.MaxCol);
                int maxLayer = int.Parse(maxRowColLayer.MaxLayer);
                for (int i = 1; i <= maxCol; i++)
                {
                    for (int j = 1; j <= maxLayer; j++)
                    {
                        string rowColLayer = row + "-" + i + "-" + j;
                        var filter = builder.And(builder.Eq("goodsequipmentGuid", goodsequipmentguid), builder.Eq("locationCode", rowColLayer));
                        StockItemInformation stockItemInformationLocationCode = MongoDBSingleton.Instance.FindOneFilter(filter);
                        if (stockItemInformationLocationCode is null)
                        {
                            StockViewModel stockAddViewModel = new StockViewModel();
                            stockAddViewModel.stockCode = stockViewModel.stockCode;
                            stockAddViewModel.areaCode = stockViewModel.areaCode;
                            stockAddViewModel.locationType = stockViewModel.locationType;
                            stockAddViewModel.locationCode = rowColLayer;
                            stockAddViewModel.state = stockViewModel.state;
                            stockAddViewModel.storageState = stockViewModel.storageState;
                            stockAddViewModel.itemRow = stockViewModel.itemRow;
                            stockViewModelAddList.Add(stockAddViewModel);
                        }
                        else if (stockItemInformationLocationCode.commandSource == "初始化")
                        {
                            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            try
                            {
                                if(!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                                {
                                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
                                }
                            }
                            catch
                            {

                            }
                            finally
                            {
                                if(locationRealMonitorViewModel is null)
                                {
                                    locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                                    locationRealMonitorViewModel.stock = new List<StockViewModel>();
                                }
                            }                    
                            List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                            if(initAddItemRowViewModel.Count == 0)
                            {
                                continue;
                            }
                            StockViewModel stockViewModelRowColLayer = initAddItemRowViewModel.First(it => it.locationCode == rowColLayer);
                            stockViewModelRowColLayer.state = stockViewModel.state;
                            stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                            stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                            stockViewModelModifyInitList.Add(stockViewModelRowColLayer);
                        }
                        else//业务
                        {
                            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "业务" && it.cn_guid == stockItemInformationLocationCode.busGuid).First();
                            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            try
                            {
                                if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                                {
                                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                            List<StockViewModel> busItemRowViewModel = locationRealMonitorViewModel.stock;
                            if(busItemRowViewModel.Count == 0)
                            {
                                continue;
                            }
                            StockViewModel stockViewModelRowColLayer = busItemRowViewModel.First(it => it.locationCode == rowColLayer);
                            stockViewModelRowColLayer.state = stockViewModel.state;
                            stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                            stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                            busItemRowViewModel.Clear();
                            busItemRowViewModel.Add(stockViewModelRowColLayer);
                            locationRealMonitorViewModel.stock = busItemRowViewModel;
                            goodscommandGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                            goodscommandGuid.cn_s_modify = user.UserCode;
                            goodscommandGuid.cn_s_modify_by = user.UserName;
                            goodscommandGuid.cn_t_modify = DateTime.Now;
                            modifyList.Add(goodscommandGuid);
                            tn_dts_logs goodscommandLog = new tn_dts_logs();
                            goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                            goodscommandLog.cn_s_logs_type = "操作";
                            goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条业务指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandGuid);
                            goodscommandLog.cn_t_create = DateTime.Now;
                            logList.Add(goodscommandLog);
                        }
                    }
                }
                if (stockViewModelAddList.Count > 0 || stockViewModelModifyInitList.Count > 0)
                {
                    tn_dts_goodscommand goodscommandInitGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                    LocationRealMonitorViewModel locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandInitGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewInitModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandInitGuid.cn_s_goodscommand_json);
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (locationRealMonitorViewInitModel is null)
                        {
                            locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                        }
                    }
                    List<StockViewModel> initItemRowViewModel = locationRealMonitorViewInitModel.stock;
                    foreach (var stockViewModelModifyInit in stockViewModelModifyInitList)
                    {
                        StockViewModel stockModify = initItemRowViewModel.First(it => it.locationCode == stockViewModelModifyInit.locationCode);
                        stockModify.state = stockViewModelModifyInit.state;
                        stockModify.storageState = stockViewModelModifyInit.storageState;
                        stockModify.itemRow = stockViewModelModifyInit.itemRow;
                    }
                    initItemRowViewModel.AddRange(stockViewModelAddList);
                    locationRealMonitorViewInitModel.stock = initItemRowViewModel;
                    goodscommandInitGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewInitModel);
                    goodscommandInitGuid.cn_s_modify = user.UserCode;
                    goodscommandInitGuid.cn_s_modify_by = user.UserName;
                    goodscommandInitGuid.cn_t_modify = DateTime.Now;
                    modifyList.Add(goodscommandInitGuid);
                    tn_dts_logs goodscommandInitLog = new tn_dts_logs();
                    goodscommandInitLog.cn_guid = Guid.NewGuid().ToString();
                    goodscommandInitLog.cn_s_logs_type = "操作";
                    goodscommandInitLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条初始化指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandInitGuid);
                    goodscommandInitLog.cn_t_create = DateTime.Now;
                    logList.Add(goodscommandInitLog);
                }
            }
            else if (addMode == "整列")
            {
                string colString = stockViewModel.locationCode.Split("-")[1];
                int col = 0;
                if (string.IsNullOrEmpty(colString) || !int.TryParse(colString, out col))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的列数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                int maxRow = int.Parse(maxRowColLayer.MaxRow);
                int maxLayer = int.Parse(maxRowColLayer.MaxLayer);
                for (int i = 1; i <= maxRow; i++)
                {
                    for (int j = 1; j <= maxLayer; j++)
                    {
                        string rowColLayer = i + "-" + col + "-" + j;
                        var filter = builder.And(builder.Eq("goodsequipmentGuid", goodsequipmentguid), builder.Eq("locationCode", rowColLayer));
                        StockItemInformation stockItemInformationLocationCode = MongoDBSingleton.Instance.FindOneFilter(filter);
                        if (stockItemInformationLocationCode is null)
                        {
                            StockViewModel stockAddViewModel = new StockViewModel();
                            stockAddViewModel.stockCode = stockViewModel.stockCode;
                            stockAddViewModel.areaCode = stockViewModel.areaCode;
                            stockAddViewModel.locationCode = rowColLayer;
                            stockAddViewModel.locationType = stockViewModel.locationType;
                            stockAddViewModel.state = stockViewModel.state;
                            stockAddViewModel.storageState = stockViewModel.storageState;
                            stockAddViewModel.itemRow = stockViewModel.itemRow;
                            stockViewModelAddList.Add(stockAddViewModel);
                        }
                        else if (stockItemInformationLocationCode.commandSource == "初始化")
                        {
                            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            try
                            {
                                if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                                {
                                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                            List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                            if(initAddItemRowViewModel.Count == 0)
                            {
                                continue;
                            }
                            StockViewModel stockViewModelRowColLayer = initAddItemRowViewModel.First(it => it.locationCode == rowColLayer);
                            stockViewModelRowColLayer.state = stockViewModel.state;
                            stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                            stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                            stockViewModelModifyInitList.Add(stockViewModelRowColLayer);
                        }
                        else//业务
                        {
                            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "业务" && it.cn_guid == stockItemInformationLocationCode.busGuid).First();
                            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            try
                            {
                                if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                                {
                                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                            List<StockViewModel> busItemRowViewModel = locationRealMonitorViewModel.stock;
                            if(busItemRowViewModel.Count == 0)
                            {
                                continue;
                            }
                            StockViewModel stockViewModelRowColLayer = busItemRowViewModel.First(it => it.locationCode == rowColLayer);
                            stockViewModelRowColLayer.state = stockViewModel.state;
                            stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                            stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                            busItemRowViewModel.Clear();
                            busItemRowViewModel.Add(stockViewModelRowColLayer);
                            locationRealMonitorViewModel.stock = busItemRowViewModel;
                            goodscommandGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                            goodscommandGuid.cn_s_modify = user.UserCode;
                            goodscommandGuid.cn_s_modify_by = user.UserName;
                            goodscommandGuid.cn_t_modify = DateTime.Now;
                            modifyList.Add(goodscommandGuid);
                            tn_dts_logs goodscommandLog = new tn_dts_logs();
                            goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                            goodscommandLog.cn_s_logs_type = "操作";
                            goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条业务指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandGuid);
                            goodscommandLog.cn_t_create = DateTime.Now;
                            logList.Add(goodscommandLog);
                        }
                    }
                }
                if (stockViewModelAddList.Count > 0 || stockViewModelModifyInitList.Count > 0)
                {
                    tn_dts_goodscommand goodscommandInitGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                    LocationRealMonitorViewModel locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandInitGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewInitModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandInitGuid.cn_s_goodscommand_json);
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (locationRealMonitorViewInitModel is null)
                        {
                            locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                        }
                    }
                    List<StockViewModel> initItemRowViewModel = locationRealMonitorViewInitModel.stock;
                    foreach (var stockViewModelModifyInit in stockViewModelModifyInitList)
                    {
                        StockViewModel stockModify = initItemRowViewModel.First(it => it.locationCode == stockViewModelModifyInit.locationCode);
                        stockModify.state = stockViewModelModifyInit.state;
                        stockModify.storageState = stockViewModelModifyInit.storageState;
                        stockModify.itemRow = stockViewModelModifyInit.itemRow;
                    }
                    initItemRowViewModel.AddRange(stockViewModelAddList);
                    locationRealMonitorViewInitModel.stock = initItemRowViewModel;
                    goodscommandInitGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewInitModel);
                    goodscommandInitGuid.cn_s_modify = user.UserCode;
                    goodscommandInitGuid.cn_s_modify_by = user.UserName;
                    goodscommandInitGuid.cn_t_modify = DateTime.Now;
                    modifyList.Add(goodscommandInitGuid);
                    tn_dts_logs goodscommandInitLog = new tn_dts_logs();
                    goodscommandInitLog.cn_guid = Guid.NewGuid().ToString();
                    goodscommandInitLog.cn_s_logs_type = "操作";
                    goodscommandInitLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条初始化指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandInitGuid);
                    goodscommandInitLog.cn_t_create = DateTime.Now;
                    logList.Add(goodscommandInitLog);
                }
            }
            else if (addMode == "整层")
            {
                string layerString = stockViewModel.locationCode.Split("-")[2];
                int layer = 0;
                if (string.IsNullOrEmpty(layerString) || !int.TryParse(layerString, out layer))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的层数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                int maxRow = int.Parse(maxRowColLayer.MaxRow);
                int maxCol = int.Parse(maxRowColLayer.MaxCol);
                for (int i = 1; i <= maxRow; i++)
                {
                    for (int j = 1; j <= maxCol; j++)
                    {
                        string rowColLayer = i.ToString() + "-" + j.ToString() + "-" + layer;
                        var filter = builder.And(builder.Eq("goodsequipmentGuid", goodsequipmentguid), builder.Eq("locationCode", rowColLayer));
                        StockItemInformation stockItemInformationLocationCode = MongoDBSingleton.Instance.FindOneFilter(filter);
                        if (stockItemInformationLocationCode is null)
                        {
                            StockViewModel stockAddViewModel = new StockViewModel();
                            stockAddViewModel.stockCode = stockViewModel.stockCode;
                            stockAddViewModel.areaCode = stockViewModel.areaCode;
                            stockAddViewModel.locationCode = rowColLayer;
                            stockAddViewModel.locationType = stockViewModel.locationType;
                            stockAddViewModel.state = stockViewModel.state;
                            stockAddViewModel.storageState = stockViewModel.storageState;
                            stockAddViewModel.itemRow = stockViewModel.itemRow;
                            stockViewModelAddList.Add(stockAddViewModel);
                        }
                        else if (stockItemInformationLocationCode.commandSource == "初始化")
                        {
                            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            try
                            {
                                if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                                {
                                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                            List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                            if(initAddItemRowViewModel.Count == 0)
                            {
                                continue;
                            }
                            StockViewModel stockViewModelRowColLayer = initAddItemRowViewModel.First(it => it.locationCode == rowColLayer);
                            stockViewModelRowColLayer.state = stockViewModel.state;
                            stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                            stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                            stockViewModelModifyInitList.Add(stockViewModelRowColLayer);
                        }
                        else//业务
                        {
                            tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "业务" && it.cn_guid == stockItemInformationLocationCode.busGuid).First();
                            LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewModel.stock = new List<StockViewModel>();
                            try
                            {
                                if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                                {
                                    locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                            List<StockViewModel> busItemRowViewModel = locationRealMonitorViewModel.stock;
                            if(busItemRowViewModel.Count == 0)
                            {
                                continue;
                            }
                            StockViewModel stockViewModelRowColLayer = busItemRowViewModel.First(it => it.locationCode == rowColLayer);
                            stockViewModelRowColLayer.state = stockViewModel.state;
                            stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                            stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                            busItemRowViewModel.Clear();
                            busItemRowViewModel.Add(stockViewModelRowColLayer);
                            locationRealMonitorViewModel.stock = busItemRowViewModel;
                            goodscommandGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                            goodscommandGuid.cn_s_modify = user.UserCode;
                            goodscommandGuid.cn_s_modify_by = user.UserName;
                            goodscommandGuid.cn_t_modify = DateTime.Now;
                            modifyList.Add(goodscommandGuid);
                            tn_dts_logs goodscommandLog = new tn_dts_logs();
                            goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                            goodscommandLog.cn_s_logs_type = "操作";
                            goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条业务指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandGuid);
                            goodscommandLog.cn_t_create = DateTime.Now;
                            logList.Add(goodscommandLog);
                        }
                    }
                }
                if (stockViewModelAddList.Count > 0 || stockViewModelModifyInitList.Count > 0)
                {
                    tn_dts_goodscommand goodscommandInitGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                    LocationRealMonitorViewModel locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandInitGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewInitModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandInitGuid.cn_s_goodscommand_json);
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (locationRealMonitorViewInitModel is null)
                        {
                            locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                        }
                    }
                    List<StockViewModel> initItemRowViewModel = locationRealMonitorViewInitModel.stock;
                    foreach (var stockViewModelModifyInit in stockViewModelModifyInitList)
                    {
                        StockViewModel stockModify = initItemRowViewModel.First(it => it.locationCode == stockViewModelModifyInit.locationCode);
                        stockModify.state = stockViewModelModifyInit.state;
                        stockModify.storageState = stockViewModelModifyInit.storageState;
                        stockModify.itemRow = stockViewModelModifyInit.itemRow;
                    }
                    initItemRowViewModel.AddRange(stockViewModelAddList);
                    locationRealMonitorViewInitModel.stock = initItemRowViewModel;
                    goodscommandInitGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewInitModel);
                    goodscommandInitGuid.cn_s_modify = user.UserCode;
                    goodscommandInitGuid.cn_s_modify_by = user.UserName;
                    goodscommandInitGuid.cn_t_modify = DateTime.Now;
                    modifyList.Add(goodscommandInitGuid);
                    tn_dts_logs goodscommandInitLog = new tn_dts_logs();
                    goodscommandInitLog.cn_guid = Guid.NewGuid().ToString();
                    goodscommandInitLog.cn_s_logs_type = "操作";
                    goodscommandInitLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条初始化指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandInitGuid);
                    goodscommandInitLog.cn_t_create = DateTime.Now;
                    logList.Add(goodscommandInitLog);
                }
            }
            else if (addMode == "单个")
            {
                string rowString = stockViewModel.locationCode.Split("-")[0];
                string colString = stockViewModel.locationCode.Split("-")[1];
                string layerString = stockViewModel.locationCode.Split("-")[2];
                int row = 0;
                int col = 0;
                int layer = 0;
                if (string.IsNullOrEmpty(rowString) || !int.TryParse(rowString, out row))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的排数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                if (string.IsNullOrEmpty(colString) || !int.TryParse(colString, out col))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的列数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                if (string.IsNullOrEmpty(layerString) || !int.TryParse(layerString, out layer))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的层数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                string rowColLayer = row + "-" + col + "-" + layer;
                var filter = builder.And(builder.Eq("goodsequipmentGuid", goodsequipmentguid), builder.Eq("locationCode", rowColLayer));
                StockItemInformation stockItemInformationLocationCode = MongoDBSingleton.Instance.FindOneFilter(filter);
                if (stockItemInformationLocationCode is null)
                {
                    StockViewModel stockAddViewModel = new StockViewModel();
                    stockAddViewModel.stockCode = stockViewModel.stockCode;
                    stockAddViewModel.areaCode = stockViewModel.areaCode;
                    stockAddViewModel.locationCode = rowColLayer;
                    stockAddViewModel.locationType = stockViewModel.locationType;
                    stockAddViewModel.state = stockViewModel.state;
                    stockAddViewModel.storageState = stockViewModel.storageState;
                    stockAddViewModel.itemRow = stockViewModel.itemRow;
                    stockViewModelAddList.Add(stockAddViewModel);
                }
                else if (stockItemInformationLocationCode.commandSource == "初始化")
                {
                    tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                    LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                    List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                    if(initAddItemRowViewModel.Count != 0)
                    {
                        StockViewModel stockViewModelRowColLayer = initAddItemRowViewModel.First(it => it.locationCode == rowColLayer);
                        stockViewModelRowColLayer.state = stockViewModel.state;
                        stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                        stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                        stockViewModelModifyInitList.Add(stockViewModelRowColLayer);
                    }  
                }
                else//业务
                {
                    tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "业务" && it.cn_guid == stockItemInformationLocationCode.busGuid).First();
                    LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                    List<StockViewModel> busItemRowViewModel = locationRealMonitorViewModel.stock;
                    if(busItemRowViewModel.Count != 0)
                    {
                        StockViewModel stockViewModelRowColLayer = busItemRowViewModel.First(it => it.locationCode == rowColLayer);
                        stockViewModelRowColLayer.state = stockViewModel.state;
                        stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                        stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                        busItemRowViewModel.Clear();
                        busItemRowViewModel.Add(stockViewModelRowColLayer);
                        locationRealMonitorViewModel.stock = busItemRowViewModel;
                        goodscommandGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                        goodscommandGuid.cn_s_modify = user.UserCode;
                        goodscommandGuid.cn_s_modify_by = user.UserName;
                        goodscommandGuid.cn_t_modify = DateTime.Now;
                        modifyList.Add(goodscommandGuid);
                        tn_dts_logs goodscommandLog = new tn_dts_logs();
                        goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                        goodscommandLog.cn_s_logs_type = "操作";
                        goodscommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条业务指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandGuid);
                        goodscommandLog.cn_t_create = DateTime.Now;
                        logList.Add(goodscommandLog);
                    }    
                }
                if (stockViewModelAddList.Count > 0 || stockViewModelModifyInitList.Count > 0)
                {
                    tn_dts_goodscommand goodscommandInitGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                    LocationRealMonitorViewModel locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandInitGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewInitModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandInitGuid.cn_s_goodscommand_json);
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (locationRealMonitorViewInitModel is null)
                        {
                            locationRealMonitorViewInitModel = new LocationRealMonitorViewModel();
                            locationRealMonitorViewInitModel.stock = new List<StockViewModel>();
                        }
                    }
                    List<StockViewModel> initItemRowViewModel = locationRealMonitorViewInitModel.stock;
                    foreach (var stockViewModelModifyInit in stockViewModelModifyInitList)
                    {
                        StockViewModel stockModify = initItemRowViewModel.First(it => it.locationCode == stockViewModelModifyInit.locationCode);
                        stockModify.state = stockViewModelModifyInit.state;
                        stockModify.storageState = stockViewModelModifyInit.storageState;
                        stockModify.itemRow = stockViewModelModifyInit.itemRow;
                    }
                    initItemRowViewModel.AddRange(stockViewModelAddList);
                    locationRealMonitorViewInitModel.stock = initItemRowViewModel;
                    goodscommandInitGuid.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewInitModel);
                    goodscommandInitGuid.cn_s_modify = user.UserCode;
                    goodscommandInitGuid.cn_s_modify_by = user.UserName;
                    goodscommandInitGuid.cn_t_modify = DateTime.Now;
                    modifyList.Add(goodscommandInitGuid);
                    tn_dts_logs goodscommandInitLog = new tn_dts_logs();
                    goodscommandInitLog.cn_guid = Guid.NewGuid().ToString();
                    goodscommandInitLog.cn_s_logs_type = "操作";
                    goodscommandInitLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条初始化指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommandInitGuid);
                    goodscommandInitLog.cn_t_create = DateTime.Now;
                    logList.Add(goodscommandInitLog);
                }
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的AddMode参数只能为“整排”、“整列”、“整层”和“单个”！";
                return returnMessage;
            }

            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_goodscommand>(modifyList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量增加货位失败！";
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量增加货位成功！";
            return returnMessage;
        }
        #endregion

        #region 修改库位
        /// <summary>
        /// 修改库位
        /// </summary>
        /// <param name="modifyLocationDate"></param>
        /// <returns></returns>
        public ReturnMessage ModifyLocation(ModifyLocationDate modifyLocationDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            string goodsequipmentguid = modifyLocationDate.GoodsequipmentGuid;
            StockViewModel stockViewModel = modifyLocationDate.StockViewModel;
            string rowString = stockViewModel.locationCode.Split("-")[0];
            string colString = stockViewModel.locationCode.Split("-")[1];
            string layerString = stockViewModel.locationCode.Split("-")[2];
            int row = 0;
            int col = 0;
            int layer = 0;
            tn_dts_goodscommand goodscommand = new tn_dts_goodscommand();
            if (string.IsNullOrEmpty(rowString) || !int.TryParse(rowString, out row))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的排数为空或无法转化为int，请检查后重试！";
                return returnMessage;
            }
            if (string.IsNullOrEmpty(colString) || !int.TryParse(colString, out col))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的列数为空或无法转化为int，请检查后重试！";
                return returnMessage;
            }
            if (string.IsNullOrEmpty(layerString) || !int.TryParse(layerString, out layer))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的层数为空或无法转化为int，请检查后重试！";
                return returnMessage;
            }
            string rowColLayer = row + "-" + col + "-" + layer;
            tn_dts_logs log = new tn_dts_logs();
            var builder = Builders<StockItemInformation>.Filter;
            var filter = builder.And(builder.Eq("goodsequipmentGuid", goodsequipmentguid), builder.Eq("locationCode", rowColLayer));
            StockItemInformation stockItemInformationLocationCode = MongoDBSingleton.Instance.FindOneFilter(filter);
            if (stockItemInformationLocationCode is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "当前货位设备修改库区的排列层在数据库中不存在，请检查后重试！";
                return returnMessage;
            }
            else if (stockItemInformationLocationCode.commandSource == "初始化")
            {
                goodscommand = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                locationRealMonitorViewModel.stock = new List<StockViewModel>();
                try
                {
                    if (!string.IsNullOrEmpty(goodscommand.cn_s_goodscommand_json))
                    {
                        locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommand.cn_s_goodscommand_json);
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
                List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                if(initAddItemRowViewModel.Count != 0)
                {
                    StockViewModel stockViewModelRowColLayer = initAddItemRowViewModel.First(it => it.locationCode == rowColLayer);
                    stockViewModelRowColLayer.state = stockViewModel.state;
                    stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                    stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                    locationRealMonitorViewModel.stock = initAddItemRowViewModel;
                    goodscommand.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                    goodscommand.cn_s_modify = user.UserCode;
                    goodscommand.cn_s_modify_by = user.UserName;
                    goodscommand.cn_t_modify = DateTime.Now;
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条初始化指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommand);
                    log.cn_t_create = DateTime.Now;
                }       
            }
            else//业务
            {
                goodscommand = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "业务" && it.cn_guid == stockItemInformationLocationCode.busGuid).First();
                LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                locationRealMonitorViewModel.stock = new List<StockViewModel>();
                try
                {
                    if (!string.IsNullOrEmpty(goodscommand.cn_s_goodscommand_json))
                    {
                        locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommand.cn_s_goodscommand_json);
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
                List<StockViewModel> busItemRowViewModel = locationRealMonitorViewModel.stock;
                if(busItemRowViewModel.Count != 0)
                {
                    StockViewModel stockViewModelRowColLayer = busItemRowViewModel.First(it => it.locationCode == rowColLayer);
                    stockViewModelRowColLayer.state = stockViewModel.state;
                    stockViewModelRowColLayer.storageState = stockViewModel.storageState;
                    stockViewModelRowColLayer.itemRow = stockViewModel.itemRow;
                    busItemRowViewModel.Clear();
                    busItemRowViewModel.Add(stockViewModelRowColLayer);
                    locationRealMonitorViewModel.stock = busItemRowViewModel;
                    goodscommand.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                    goodscommand.cn_s_modify = user.UserCode;
                    goodscommand.cn_s_modify_by = user.UserName;
                    goodscommand.cn_t_modify = DateTime.Now;
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量添加功能修改了一条业务指令Json，详细信息为：" + JsonConvert.SerializeObject(goodscommand);
                    log.cn_t_create = DateTime.Now;
                }        
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_goodscommand>(goodscommand).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改货位失败！";
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改货位成功！";
            return returnMessage;
        }
        #endregion

        #region 删除库位
        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="deleteLocationDate"></param>
        /// <returns></returns>
        public ReturnMessage DeleteLocation(DeleteLocationDate deleteLocationDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            string goodsequipmentguid = deleteLocationDate.GoodsequipmentGuid;
            List<string> LocationCodeList = deleteLocationDate.LocationCodeList;
            List<StockViewModel> stockViewModelInitDeleteList = new List<StockViewModel>();
            List<tn_dts_goodscommand> deleteGoodscommandList = new List<tn_dts_goodscommand>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            List<RowColLayer> rowColLayerList = new List<RowColLayer>();
            tn_dts_goodscommand modifyInit = new tn_dts_goodscommand();
            foreach (var LocationCode in LocationCodeList)
            {
                RowColLayer rowColLayer = new RowColLayer();
                string rowString = LocationCode.Split("-")[0];
                string colString = LocationCode.Split("-")[1];
                string layerString = LocationCode.Split("-")[2];
                int row = 0;
                int col = 0;
                int layer = 0;
                if (string.IsNullOrEmpty(rowString) || !int.TryParse(rowString, out row))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的排数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                if (string.IsNullOrEmpty(colString) || !int.TryParse(colString, out col))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的列数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                if (string.IsNullOrEmpty(layerString) || !int.TryParse(layerString, out layer))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的层数为空或无法转化为int，请检查后重试！";
                    return returnMessage;
                }
                rowColLayer.Row = row;
                rowColLayer.Col = col;
                rowColLayer.Layer = layer;
                rowColLayerList.Add(rowColLayer);
            }
            foreach (var rowColLayer in rowColLayerList)
            {
                string rowColLayerString = rowColLayer.Row.ToString() + "-" + rowColLayer.Col.ToString() + "-" + rowColLayer.Layer.ToString();
                tn_dts_logs log = new tn_dts_logs();
                var builder = Builders<StockItemInformation>.Filter;
                var filter = builder.And(builder.Eq("goodsequipmentGuid", goodsequipmentguid), builder.Eq("locationCode", rowColLayerString));
                StockItemInformation stockItemInformationLocationCode = MongoDBSingleton.Instance.FindOneFilter(filter);
                if (stockItemInformationLocationCode is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "当前货位设备删除库区的排列层在数据库中不存在，请检查后重试！";
                    return returnMessage;
                }
                else if (stockItemInformationLocationCode.commandSource == "初始化")
                {
                    tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                    LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                    locationRealMonitorViewModel.stock = new List<StockViewModel>();
                    try
                    {
                        if (!string.IsNullOrEmpty(goodscommandGuid.cn_s_goodscommand_json))
                        {
                            locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(goodscommandGuid.cn_s_goodscommand_json);
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
                    List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                    if(initAddItemRowViewModel.Count != 0)
                    {
                        StockViewModel stockViewModelRowColLayer = initAddItemRowViewModel.First(it => it.locationCode == rowColLayerString);
                        stockViewModelInitDeleteList.Add(stockViewModelRowColLayer);
                    }            
                }
                else//业务
                {
                    tn_dts_goodscommand goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "业务" && it.cn_guid == stockItemInformationLocationCode.busGuid).First();
                    deleteGoodscommandList.Add(goodscommandGuid);
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量删除功能删除了一条货位指令记录，详细信息为：" + JsonConvert.SerializeObject(goodscommandGuid);
                    log.cn_t_create = DateTime.Now;
                    logList.Add(log);
                }
            }
            if (stockViewModelInitDeleteList.Count > 0)
            {
                modifyInit = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == goodsequipmentguid && it.cn_s_goodscommand_type == "初始化").First();
                LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                locationRealMonitorViewModel.stock = new List<StockViewModel>();
                try
                {
                    if (!string.IsNullOrEmpty(modifyInit.cn_s_goodscommand_json))
                    {
                        locationRealMonitorViewModel = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(modifyInit.cn_s_goodscommand_json);
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
                List<StockViewModel> initAddItemRowViewModel = locationRealMonitorViewModel.stock;
                foreach (var stockViewModelInitDelete in stockViewModelInitDeleteList)
                {
                    StockViewModel deleteStockViewModel = initAddItemRowViewModel.First(it => it.locationCode == stockViewModelInitDelete.locationCode);
                    initAddItemRowViewModel.Remove(deleteStockViewModel);
                }
                locationRealMonitorViewModel.stock = initAddItemRowViewModel;
                modifyInit.cn_s_goodscommand_json = JsonConvert.SerializeObject(locationRealMonitorViewModel);
                modifyInit.cn_s_modify = user.UserCode;
                modifyInit.cn_s_modify_by = user.UserName;
                modifyInit.cn_t_modify = DateTime.Now;
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "操作";
                log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用货位指令初始化管理批量删除功能修改了一条初始化指令Json记录，详细信息为：" + JsonConvert.SerializeObject(modifyInit);
                log.cn_t_create = DateTime.Now;
                logList.Add(log);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                if (deleteGoodscommandList.Count > 0)
                {
                    dbTran.Deleteable<tn_dts_goodscommand>(deleteGoodscommandList).ExecuteCommand();
                }
                if (stockViewModelInitDeleteList.Count > 0)
                {
                    dbTran.Updateable<tn_dts_goodscommand>(modifyInit).ExecuteCommand();
                }
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除货位失败！";
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量删除货位成功！";
            return returnMessage;
        }
        #endregion
    }
}
