using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.OpenApi;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using MongoDB.Driver;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HZ.IDTSCore.Interfaces.Service.OpenApi
{
    //class DtsService : DbContext, IDtsService
    class DtsService : BaseService<tn_dts_logs>, IDtsService
    {
        private const int LargeJsonLogSampleCount = 3;

        /// <summary>
        /// 大集合日志只记录统计信息和少量样例，避免把货位完整 JSON 写入磁盘。
        /// </summary>
        private static string BuildStockLogSummary(List<StockViewModel> stocks)
        {
            int stockCount = stocks == null ? 0 : stocks.Count;
            int rackCount = stocks == null ? 0 : stocks.Count(it => it.rackInfo != null);
            int boxCount = stocks == null ? 0 : stocks.Sum(it => it.rackInfo == null || it.rackInfo.boxInfo == null ? 0 : it.rackInfo.boxInfo.Count);
            string sampleCodes = stocks == null ? "" : string.Join(",", stocks.Take(LargeJsonLogSampleCount).Select(it => it.stockCode + "/" + it.areaCode + "/" + it.locationCode));
            return "locationRealMonitorViewModel.stock摘要：货位数=" + stockCount + "，料架数=" + rackCount + "，料箱数=" + boxCount + "，样例=" + sampleCodes;
        }

        /// <summary>
        /// 大集合日志只记录统计信息和少量样例，避免把站点完整 JSON 写入磁盘。
        /// </summary>
        private static string BuildDataMemberLogSummary(List<DataMember> dataMemberList)
        {
            int totalCount = dataMemberList == null ? 0 : dataMemberList.Count;
            int showGoodsCount = dataMemberList == null ? 0 : dataMemberList.Count(it => it.showGoods == 1);
            string sampleCodes = dataMemberList == null ? "" : string.Join(",", dataMemberList.Take(LargeJsonLogSampleCount).Select(it => it.code));
            return "dataMemberList摘要：站点数=" + totalCount + "，显示货物站点数=" + showGoodsCount + "，样例=" + sampleCodes;
        }
        public DtsService(SessionInfo session) : base(session)
        {

        }

        #region 获取通用初始化信息V2
        /// <summary>
        /// 获取通用初始化信息
        /// </summary>
        /// <returns>通用初始化信息</returns>
        public ViewSystemconfigModel GetSystemconfigV2()
        {
            ViewSystemconfigModel viewSystemconfigModel = new ViewSystemconfigModel();
            try
            {
                List<tn_dts_stock3d> stock3dList = Db.Queryable<tn_dts_stock3d>().ToList();
                List<Region> regionList = new List<Region>();
                foreach (var stock3d in stock3dList)
                {
                    Region region = new Region();
                    region.name = stock3d.cn_s_location_areaname;
                    region.showGoods = stock3d.cn_s_location_isshow;
                    region.colNum = stock3d.cn_s_location_col;
                    region.levelNum = stock3d.cn_s_location_layer;
                    region.rowNum = stock3d.cn_s_location_row;
                    region.sizeX = stock3d.cn_s_location_length;
                    region.sizeY = stock3d.cn_s_location_height;
                    region.sizeZ = stock3d.cn_s_location_width;
                    region.rowGap = GetRowGap(stock3d.cn_s_location_gap);
                    region.origionPointX = stock3d.cn_s_location_xpos;
                    region.origionPointY = stock3d.cn_s_location_ypos;
                    region.invalid = GetNullify(stock3d.cn_s_location_stockcode, stock3d.cn_s_location_areacode);
                    regionList.Add(region);
                }
                viewSystemconfigModel.Regions = regionList;

                Ground ground = new Ground();
                ground.data = Db.Queryable<Model.Entity.location.tn_dts_siteinfo>()
                .Select(it => new DataMember
                {
                    showGoods = it.cn_s_siteinfo_isshow,
                    code = it.cn_s_siteinfo_code,
                    positionX = it.cn_s_siteinfo_xpos,
                    positionY = it.cn_s_siteinfo_ypos,
                    sizeX = it.cn_s_siteinfo_lenght,
                    sizeY = it.cn_s_siteinfo_height,
                    sizeZ = it.cn_s_siteinfo_width,
                    angle = it.cn_d_siteinfo_angle,
                    pileofland = it.cn_s_siteinfo_pileofland
                }).ToList();
                viewSystemconfigModel.Ground = ground;

                List<AGVMember> agvs = new List<AGVMember>();
                List<string> agvnos = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == "AGV" && it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常").Select(it => it.cn_s_equi_no).ToList();
                var queryAGV = Db.Queryable<tn_dts_equipment>()
                .FullJoin<tn_dts_equiobject>((e, o) => e.cn_guid == o.cn_s_object_equiguid)
                .FullJoin<tn_dts_equiobjectattr>((e, o, oa) => o.cn_guid == oa.cn_s_objectattr_guid)
                .Select((e, o, oa) => new
                {
                    cn_s_equi_no = e.cn_s_equi_no,
                    cn_s_equi_name = e.cn_s_equi_name,
                    cn_s_equi_type = e.cn_s_equi_type,
                    cn_s_object_equiguid = e.cn_guid,
                    cn_s_objectattr_guid = o.cn_guid,
                    cn_s_objectattr_attrname = oa.cn_s_objectattr_attrname,
                    cn_s_objectattr_attrvalue = oa.cn_s_objectattr_attrvalue
                });
                foreach (var agvno in agvnos)
                {
                    AGVMember agv = new AGVMember();
                    agv.Name = queryAGV.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "Name").Select(it => it.cn_s_objectattr_attrvalue).First();
                    string s = queryAGV.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "Speed").Select(it => it.cn_s_objectattr_attrvalue).First();
                    if (String.IsNullOrEmpty(s))
                    {
                        agv.Speed = null;
                    }
                    else
                    {
                        agv.Speed = double.Parse(s);
                    }
                    agv.Code = queryAGV.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "Code").Select(it => it.cn_s_objectattr_attrvalue).First();
                    string ros = queryAGV.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "RotateSpeed").Select(it => it.cn_s_objectattr_attrvalue).First();
                    if (String.IsNullOrEmpty(ros))
                    {
                        agv.RotateSpeed = null;
                    }
                    else
                    {
                        agv.RotateSpeed = double.Parse(ros);
                    }
                    string ix = queryAGV.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "InitPosX").Select(it => it.cn_s_objectattr_attrvalue).First();
                    if (String.IsNullOrEmpty(ix))
                    {
                        agv.InitPosX = null;
                    }
                    else
                    {
                        agv.InitPosX = double.Parse(ix);
                    }
                    string iy = queryAGV.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "InitPosY").Select(it => it.cn_s_objectattr_attrvalue).First();
                    if (String.IsNullOrEmpty(iy))
                    {
                        agv.InitPosY = null;
                    }
                    else
                    {
                        agv.InitPosY = double.Parse(iy);
                    }
                    if (!(agv.Name is null) || !(agv.Speed is null) || !(agv.Code is null) || !(agv.RotateSpeed is null) || !(agv.InitPosX is null) || !(agv.InitPosY is null))
                    {
                        agvs.Add(agv);
                    }
                }
                viewSystemconfigModel.AGV = agvs;

                List<ChargeMember> charges = new List<ChargeMember>();
                List<string> chargenos = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == "充电机" && it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常").Select(it => it.cn_s_equi_no).ToList();
                var query = Db.Queryable<tn_dts_equipment>()
                .FullJoin<tn_dts_equiobject>((e, o) => e.cn_guid == o.cn_s_object_equiguid)
                .FullJoin<tn_dts_equiobjectattr>((e, o, oa) => o.cn_guid == oa.cn_s_objectattr_guid)
                .Select((e, o, oa) => new
                {
                    cn_s_equi_no = e.cn_s_equi_no,
                    cn_s_equi_name = e.cn_s_equi_name,
                    cn_s_equi_type = e.cn_s_equi_type,
                    cn_s_object_equiguid = e.cn_guid,
                    cn_s_objectattr_guid = o.cn_guid,
                    cn_s_objectattr_attrname = oa.cn_s_objectattr_attrname,
                    cn_s_objectattr_attrvalue = oa.cn_s_objectattr_attrvalue
                });
                foreach (var chargeno in chargenos)
                {
                    ChargeMember charge = new ChargeMember();
                    charge.name = query.MergeTable().Where(it => it.cn_s_equi_no == chargeno && it.cn_s_objectattr_attrname == "name").Select(it => it.cn_s_objectattr_attrvalue).First();
                    charge.code = query.MergeTable().Where(it => it.cn_s_equi_no == chargeno && it.cn_s_objectattr_attrname == "code").Select(it => it.cn_s_objectattr_attrvalue).First();
                    string px = query.MergeTable().Where(it => it.cn_s_equi_no == chargeno && it.cn_s_objectattr_attrname == "PosX").Select(it => it.cn_s_objectattr_attrvalue).First();
                    if (String.IsNullOrEmpty(px))
                    {
                        charge.PosX = null;
                    }
                    else
                    {
                        charge.PosX = double.Parse(px);
                    }
                    string py = query.MergeTable().Where(it => it.cn_s_equi_no == chargeno && it.cn_s_objectattr_attrname == "PosY").Select(it => it.cn_s_objectattr_attrvalue).First();
                    if (String.IsNullOrEmpty(py))
                    {
                        charge.PosY = null;
                    }
                    else
                    {
                        charge.PosY = double.Parse(py);
                    }
                    if (!(charge.name is null) || !(charge.code is null) || !(charge.PosX is null) || !(charge.PosY is null))
                    {
                        charges.Add(charge);
                    }
                }
                viewSystemconfigModel.charge = charges;

                AutoView autoView = new AutoView();
                string av = Db.Queryable<tn_dts_setting>()
                .Where(it => it.cn_s_setting_keycode == "AutoView")
                .Select(it => new
                {
                    Interval = it.cn_s_setting_keyvalue
                }).MergeTable().Select(it => it.Interval).First();
                if (String.IsNullOrEmpty(av))
                {
                    autoView.interval = null;
                }
                else
                {
                    autoView.interval = double.Parse(av);
                }
                viewSystemconfigModel.AutoView = autoView;

                Position position = new Position();
                string ox = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroX").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ox))
                {
                    position.originX = null;
                }
                else
                {
                    position.originX = double.Parse(ox);
                }
                string oy = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroY").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(oy))
                {
                    position.originY = null;
                }
                else
                {
                    position.originY = double.Parse(oy);
                }
                viewSystemconfigModel.Position = position;

                SysConfig sysconfig = new SysConfig();
                sysconfig.systemName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemName").Select(it => it.cn_s_setting_keyvalue).First();
                sysconfig.systemLogoURL = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemLogoURL").Select(it => it.cn_s_setting_keyvalue).First();
                sysconfig.webSocketServer = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "WebSocketServer").Select(it => it.cn_s_setting_keyvalue).First();
                viewSystemconfigModel.SysConfig = sysconfig;

                SysAuthority sysAuthority = new SysAuthority();
                string ermm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ermm))
                {
                    sysAuthority.enabledRealMonitorMenu = null;
                }
                else
                {
                    sysAuthority.enabledRealMonitorMenu = bool.Parse(ermm);
                }
                string eddm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledDeviceDataMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(eddm))
                {
                    sysAuthority.enabledDeviceDataMenu = null;
                }
                else
                {
                    sysAuthority.enabledDeviceDataMenu = bool.Parse(eddm);
                }
                string ebmm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledBussMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ebmm))
                {
                    sysAuthority.enabledBussMonitorMenu = null;
                }
                else
                {
                    sysAuthority.enabledBussMonitorMenu = bool.Parse(ebmm);
                }
                string evvm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledVirtualVideoMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(evvm))
                {
                    sysAuthority.enabledVirtualVideoMenu = null;
                }
                else
                {
                    sysAuthority.enabledVirtualVideoMenu = bool.Parse(evvm);
                }

                BussMonitorMenuSett bussMonitorMenuSett = new BussMonitorMenuSett();
                string icm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "isCustomMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(icm))
                {
                    bussMonitorMenuSett.isCustomMenu = null;
                }
                else
                {
                    bussMonitorMenuSett.isCustomMenu = bool.Parse(icm);
                }
                bussMonitorMenuSett.menuName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "MenuName").Select(it => it.cn_s_setting_keyvalue).First();
                bussMonitorMenuSett.menuUrl = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "MenuUrl").Select(it => it.cn_s_setting_keyvalue).First();
                sysAuthority.bussMonitorMenuSett = bussMonitorMenuSett;
                string epm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledPlannedModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(epm))
                {
                    sysAuthority.enabledPlannedModule = null;
                }
                else
                {
                    sysAuthority.enabledPlannedModule = bool.Parse(epm);
                }
                string hdldf = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "HomeDeviceListDefaultFold").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(hdldf))
                {
                    sysAuthority.homeDeviceListDefaultFold = null;
                }
                else
                {
                    sysAuthority.homeDeviceListDefaultFold = bool.Parse(hdldf);
                }
                string ehdql = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledHomeDeviceQueryList").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ehdql))
                {
                    sysAuthority.enabledHomeDeviceQueryList = null;
                }
                else
                {
                    sysAuthority.enabledHomeDeviceQueryList = bool.Parse(ehdql);
                }
                string ertm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealTimeModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ertm))
                {
                    sysAuthority.enabledRealTimeModule = null;
                }
                else
                {
                    sysAuthority.enabledRealTimeModule = bool.Parse(ertm);
                }
                string ercsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRepairCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ercsm))
                {
                    sysAuthority.enabledRepairCountShowModule = null;
                }
                else
                {
                    sysAuthority.enabledRepairCountShowModule = bool.Parse(ercsm);
                }
                string eucsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledUpkeepCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(eucsm))
                {
                    sysAuthority.enabledUpkeepCountShowModule = null;
                }
                else
                {
                    sysAuthority.enabledUpkeepCountShowModule = bool.Parse(eucsm);
                }
                string eecsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledErrorsCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(eucsm))
                {
                    sysAuthority.enabledErrorsCountShowModule = null;
                }
                else
                {
                    sysAuthority.enabledErrorsCountShowModule = bool.Parse(eecsm);
                }
                string emqm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledMaterialQueryModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(emqm))
                {
                    sysAuthority.enabledMaterialQueryModule = null;
                }
                else
                {
                    sysAuthority.enabledMaterialQueryModule = bool.Parse(emqm);
                }
                viewSystemconfigModel.SysAuthority = sysAuthority;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
            return viewSystemconfigModel;
        }
        #endregion

        #region 获取通用初始化信息（旧）
        ///// <summary>
        ///// 获取通用初始化信息（旧）
        ///// </summary>
        ///// <returns>通用初始化信息</returns>
        //public Newtonsoft.Json.Linq.JObject GetSystemconfig()
        //{
        //    Newtonsoft.Json.Linq.JObject result = new Newtonsoft.Json.Linq.JObject();
        //    try
        //    {
        //        List<tn_dts_stock3d> stock3dList = Db.Queryable<tn_dts_stock3d>().ToList();
        //        List<Region> regionList = new List<Region>();
        //        foreach (var stock3d in stock3dList)
        //        {
        //            Region region = new Region();
        //            region.name = stock3d.cn_s_location_areaname;
        //            region.showGoods = stock3d.cn_s_location_isshow;
        //            region.colNum = stock3d.cn_s_location_col;
        //            region.levelNum = stock3d.cn_s_location_layer;
        //            region.rowNum = stock3d.cn_s_location_row;
        //            region.sizeX = stock3d.cn_s_location_length;
        //            region.sizeY = stock3d.cn_s_location_height;
        //            region.sizeZ = stock3d.cn_s_location_width;
        //            region.rowGap = GetRowGap(stock3d.cn_s_location_gap);
        //            region.origionPointX = stock3d.cn_s_location_xpos;
        //            region.origionPointY = stock3d.cn_s_location_ypos;
        //            region.invalid = GetNullify(stock3d.cn_s_location_stockcode, stock3d.cn_s_location_areacode);
        //            regionList.Add(region);
        //        }

        //        Newtonsoft.Json.Linq.JArray jObjRegion = Newtonsoft.Json.Linq.JArray.FromObject(regionList);
        //        result["regions"] = jObjRegion;

        //        Ground ground = new Ground();
        //        ground.data = Db.Queryable<Model.Entity.location.tn_dts_siteinfo>()
        //        .Select(it => new DataMember
        //        {
        //            showGoods = it.cn_s_siteinfo_isshow,
        //            code = it.cn_s_siteinfo_code,
        //            positionX = it.cn_s_siteinfo_xpos,
        //            positionY = it.cn_s_siteinfo_ypos,
        //            sizeX = it.cn_s_siteinfo_lenght,
        //            sizeY = it.cn_s_siteinfo_height,
        //            sizeZ = it.cn_s_siteinfo_width,
        //            angle = it.cn_d_siteinfo_angle,
        //            pileofland = it.cn_s_siteinfo_pileofland
        //        }).ToList();

        //        Newtonsoft.Json.Linq.JObject jObjGround = Newtonsoft.Json.Linq.JObject.FromObject(ground);
        //        result["ground"] = jObjGround;

        //        AutoView autoView = new AutoView();
        //        string av = Db.Queryable<tn_dts_setting>()
        //        .Where(it => it.cn_s_setting_keycode == "AutoView")
        //        .Select(it => new
        //        {
        //            Interval = it.cn_s_setting_keyvalue
        //        }).MergeTable().Select(it => it.Interval).First();
        //        if (String.IsNullOrEmpty(av))
        //        {
        //            autoView.interval = null;
        //        }
        //        else
        //        {
        //            autoView.interval = double.Parse(av);
        //        }

        //        Newtonsoft.Json.Linq.JObject jObjAutoView = Newtonsoft.Json.Linq.JObject.FromObject(autoView);
        //        result["autoView"] = jObjAutoView;

        //        Position position = new Position();
        //        string ox = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroX").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ox))
        //        {
        //            position.originX = null;
        //        }
        //        else
        //        {
        //            position.originX = double.Parse(ox);
        //        }
        //        string oy = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroY").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(oy))
        //        {
        //            position.originY = null;
        //        }
        //        else
        //        {
        //            position.originY = double.Parse(oy);
        //        }

        //        Newtonsoft.Json.Linq.JObject jObjPosition = Newtonsoft.Json.Linq.JObject.FromObject(position);
        //        result["position"] = jObjPosition;

        //        SysConfig sysconfig = new SysConfig();
        //        sysconfig.systemName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemName").Select(it => it.cn_s_setting_keyvalue).First();
        //        sysconfig.systemLogoURL = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemLogoURL").Select(it => it.cn_s_setting_keyvalue).First();
        //        sysconfig.webSocketServer = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "WebSocketServer").Select(it => it.cn_s_setting_keyvalue).First();


        //        Newtonsoft.Json.Linq.JObject jObjSysconfig = Newtonsoft.Json.Linq.JObject.FromObject(sysconfig);
        //        result["sysConfig"] = jObjSysconfig;

        //        SysAuthority sysAuthority = new SysAuthority();
        //        string ermm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ermm))
        //        {
        //            sysAuthority.enabledRealMonitorMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledRealMonitorMenu = bool.Parse(ermm);
        //        }
        //        string eddm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledDeviceDataMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(eddm))
        //        {
        //            sysAuthority.enabledDeviceDataMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledDeviceDataMenu = bool.Parse(eddm);
        //        }
        //        string ebmm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledBussMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ebmm))
        //        {
        //            sysAuthority.enabledBussMonitorMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledBussMonitorMenu = bool.Parse(ebmm);
        //        }
        //        string evvm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledVirtualVideoMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(evvm))
        //        {
        //            sysAuthority.enabledVirtualVideoMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledVirtualVideoMenu = bool.Parse(evvm);
        //        }

        //        BussMonitorMenuSett bussMonitorMenuSett = new BussMonitorMenuSett();
        //        string icm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "isCustomMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(icm))
        //        {
        //            bussMonitorMenuSett.isCustomMenu = null;
        //        }
        //        else
        //        {
        //            bussMonitorMenuSett.isCustomMenu = bool.Parse(icm);
        //        }
        //        bussMonitorMenuSett.menuName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "MenuName").Select(it => it.cn_s_setting_keyvalue).First();
        //        bussMonitorMenuSett.menuUrl = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "MenuUrl").Select(it => it.cn_s_setting_keyvalue).First();
        //        sysAuthority.bussMonitorMenuSett = bussMonitorMenuSett;
        //        string epm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledPlannedModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(epm))
        //        {
        //            sysAuthority.enabledPlannedModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledPlannedModule = bool.Parse(epm);
        //        }
        //        string hdldf = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "HomeDeviceListDefaultFold").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(hdldf))
        //        {
        //            sysAuthority.homeDeviceListDefaultFold = null;
        //        }
        //        else
        //        {
        //            sysAuthority.homeDeviceListDefaultFold = bool.Parse(hdldf);
        //        }
        //        string ehdql = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledHomeDeviceQueryList").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ehdql))
        //        {
        //            sysAuthority.enabledHomeDeviceQueryList = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledHomeDeviceQueryList = bool.Parse(ehdql);
        //        }
        //        string ertm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealTimeModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ertm))
        //        {
        //            sysAuthority.enabledRealTimeModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledRealTimeModule = bool.Parse(ertm);
        //        }
        //        string ercsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRepairCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ercsm))
        //        {
        //            sysAuthority.enabledRepairCountShowModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledRepairCountShowModule = bool.Parse(ercsm);
        //        }
        //        string eucsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledUpkeepCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(eucsm))
        //        {
        //            sysAuthority.enabledUpkeepCountShowModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledUpkeepCountShowModule = bool.Parse(eucsm);
        //        }
        //        string eecsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledErrorsCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(eucsm))
        //        {
        //            sysAuthority.enabledErrorsCountShowModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledErrorsCountShowModule = bool.Parse(eecsm);
        //        }
        //        string emqm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledMaterialQueryModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(emqm))
        //        {
        //            sysAuthority.enabledMaterialQueryModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.enabledMaterialQueryModule = bool.Parse(emqm);
        //        }

        //        Newtonsoft.Json.Linq.JObject jObjSysAuthority = Newtonsoft.Json.Linq.JObject.FromObject(sysAuthority);
        //        result["sysAuthority"] = jObjSysAuthority;

        //        //List<string> agvguids = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == "AGV" && it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常").Select(it => it.cn_guid).ToList();
        //        List<string> agvguids = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常").Select(it => it.cn_guid).ToList();
        //        List<Newtonsoft.Json.Linq.JObject> attrAGVList = new List<Newtonsoft.Json.Linq.JObject>();//AGV固定对象
        //        List<DynamicsObjectClass> ontherList = new List<DynamicsObjectClass>();//其他通用对象

        //        foreach (var guid in agvguids)
        //        {
        //            //tn_dts_equiobject //tn_dts_equiobjectattr
        //            var eqObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equiguid == guid).ToList();

        //            //取一个设备固定 AGV对象名下面的所有属性
        //            //var agvObj=eqObjectList.FirstOrDefault(p => p.cn_s_object_name == "AGV");
        //            //if (agvObj != null)
        //            //{
        //            //    string jsonString = "";
        //            //    var eqAttrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == agvObj.cn_guid).ToList();
        //            //    foreach (var attrModel in eqAttrList)
        //            //    {
        //            //        jsonString += "\"" + attrModel.cn_s_objectattr_attrname + "\":\"" + attrModel.cn_s_objectattr_attrvalue + "\",";

        //            //    }
        //            //    if (jsonString != "")
        //            //    {
        //            //        jsonString = jsonString.Substring(0, jsonString.Length - 1);

        //            //        jsonString = "{" + jsonString + "}";

        //            //        Newtonsoft.Json.Linq.JObject jsonArray = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
        //            //        attrAGVList.Add(jsonArray);
        //            //    }
        //            //}


        //            //其他通用设备对象(包含充电机对象)
        //            var ontherObjList = eqObjectList;// eqObjectList.Where(p => p.cn_s_object_name != "AGV").ToList();
        //            foreach (var ontherObj in ontherObjList)
        //            {
        //                var objModel = ontherList.FirstOrDefault(p => p.objectKeyName == ontherObj.cn_s_object_name);
        //                if (objModel == null)
        //                {
        //                    ontherList.Add(new DynamicsObjectClass() { objectKeyName = ontherObj.cn_s_object_name });
        //                }
        //            }

        //        }
        //        //result["AGV"] = Newtonsoft.Json.Linq.JArray.FromObject(attrAGVList);

        //        //List<Newtonsoft.Json.Linq.JObject> attrOntherList = new List<Newtonsoft.Json.Linq.JObject>();//其他通用对象
        //        //foreach (var dynamicsObject in ontherList)
        //        //{
        //        //    var eqObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == dynamicsObject.objectKeyName).ToList();
        //        //    foreach(var eqObject in eqObjectList)
        //        //    {
        //        //        string jsonString = "";
        //        //        var eqAttrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == eqObject.cn_guid).ToList();
        //        //        foreach (var attrModel in eqAttrList)
        //        //        {
        //        //            jsonString += "\"" + attrModel.cn_s_objectattr_attrname + "\":\"" + attrModel.cn_s_objectattr_attrvalue + "\",";
        //        //        }
        //        //        if (jsonString != "")
        //        //        {
        //        //            jsonString = jsonString.Substring(0, jsonString.Length - 1);
        //        //            jsonString = "{" + jsonString + "}";
        //        //            Newtonsoft.Json.Linq.JObject jsonArray = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
        //        //            attrOntherList.Add(jsonArray);
        //        //        }
        //        //    }
        //        //    dynamicsObject.objectValue = attrOntherList;
        //        //    result[""+ dynamicsObject.objectKeyName + ""] = Newtonsoft.Json.Linq.JArray.FromObject(dynamicsObject.objectValue);
        //        //}

        //        List<Newtonsoft.Json.Linq.JObject> attrOntherList = new List<Newtonsoft.Json.Linq.JObject>();//其他通用对象
        //        foreach (var dynamicsObject in ontherList)
        //        {
        //            var eqObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == dynamicsObject.objectKeyName).ToList();
        //            foreach (var eqObject in eqObjectList)
        //            {
        //                string jsonString = "";
        //                var eqAttrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == eqObject.cn_guid).ToList();
        //                foreach (var attrModel in eqAttrList)
        //                {
        //                    jsonString += "\"" + attrModel.cn_s_objectattr_attrname + "\":\"" + attrModel.cn_s_objectattr_attrvalue + "\",";
        //                }
        //                if (jsonString != "")
        //                {
        //                    jsonString = jsonString.Substring(0, jsonString.Length - 1);
        //                    jsonString = "{" + jsonString + "}";
        //                    Newtonsoft.Json.Linq.JObject jsonArray = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
        //                    attrOntherList.Add(jsonArray);
        //                }
        //            }
        //            dynamicsObject.objectValue = attrOntherList;
        //            result["" + dynamicsObject.objectKeyName + ""] = Newtonsoft.Json.Linq.JArray.FromObject(dynamicsObject.objectValue);
        //            attrOntherList.Clear();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(ex.Message);
        //    }
        //    return result;
        //}
        #endregion

        /// <summary>
        /// 获取通用初始化信息
        /// </summary>
        /// <param name="locationRealMonitorViewModel"></param>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JObject GetSystemconfig(LocationRealMonitorViewModel locationRealMonitorViewModel)
        {
            Newtonsoft.Json.Linq.JObject result = new Newtonsoft.Json.Linq.JObject();
            try
            {
                List<tn_dts_stock3d> stock3dList = Db.Queryable<tn_dts_stock3d>().ToList();
                List<Region> regionList = new List<Region>();
                foreach (var stock3d in stock3dList)
                {
                    Region region = new Region();
                    region.name = stock3d.cn_s_location_areaname;
                    region.showGoods = stock3d.cn_s_location_isshow;
                    region.colNum = stock3d.cn_s_location_col;
                    region.levelNum = stock3d.cn_s_location_layer;
                    region.rowNum = stock3d.cn_s_location_row;
                    region.sizeX = stock3d.cn_s_location_length;
                    region.sizeY = stock3d.cn_s_location_height;
                    region.sizeZ = stock3d.cn_s_location_width;
                    region.rowGap = GetRowGap(stock3d.cn_s_location_gap);
                    region.origionPointX = stock3d.cn_s_location_xpos;
                    region.origionPointY = stock3d.cn_s_location_ypos;
                    region.invalid = GetNullify(stock3d.cn_s_location_stockcode, stock3d.cn_s_location_areacode);
                    regionList.Add(region);
                }

                Newtonsoft.Json.Linq.JArray jObjRegion = Newtonsoft.Json.Linq.JArray.FromObject(regionList);
                result["regions"] = jObjRegion;

                
                Ground ground = new Ground();
                List<DataMember> dataMemberList = Db.Queryable<Model.Entity.location.tn_dts_siteinfo>()
                .Select(it => new DataMember
                {
                    showGoods = it.cn_s_siteinfo_isshow,
                    code = it.cn_s_siteinfo_code,
                    positionX = it.cn_s_siteinfo_xpos,
                    positionY = it.cn_s_siteinfo_ypos,
                    sizeX = it.cn_s_siteinfo_lenght,
                    sizeY = it.cn_s_siteinfo_height,
                    sizeZ = it.cn_s_siteinfo_width,
                    angle = it.cn_d_siteinfo_angle,
                    pileofland = it.cn_s_siteinfo_pileofland
                }).ToList();
                string groundJson = "";
                LogHelper.Info(BuildStockLogSummary(locationRealMonitorViewModel == null ? null : locationRealMonitorViewModel.stock));
                LogHelper.Info(BuildDataMemberLogSummary(dataMemberList));
                if (!(locationRealMonitorViewModel.stock is null))
                {
                    foreach (var dataMember in dataMemberList)
                    {
                        //string dataMemberJson = JsonConvert.SerializeObject(dataMember);
                        StockViewModel matchStock = locationRealMonitorViewModel.stock.FirstOrDefault(it => it.locationCode == dataMember.code);
                        if(!(matchStock is null))
                        {
                            dataMember.rackInfo = matchStock.rackInfo;
                        }
                        //if (!(matchStock is null))
                        //{
                        //    // 反序列化JSON到JObject  
                        //    JObject jObject = JObject.Parse(dataMemberJson);

                        //    // 移除rackInfo属性  
                        //    jObject.Remove("rackInfo");

                        //    // 将修改后的JObject序列化为JSON字符串  
                        //    string modifiedJson = jObject.ToString();

                        //    string jsonAB = "";

                        //    //去左右大括号和\r\n
                        //    string jsonA = modifiedJson.Substring(1, modifiedJson.Length - 4) + ",";

                        //    string rackInfoJson = JsonConvert.SerializeObject(matchStock.rackInfo);

                        //    string jsonB = rackInfoJson.Substring(1, rackInfoJson.Length - 2);

                        //    jsonAB = "{" + jsonA + jsonB + "}";

                        //    groundJson += jsonAB + ",";

                        //}
                        //else
                        //{
                        //    groundJson += dataMemberJson + ",";
                        //}
                    }
                    //if (dataMemberList.Count != 0)
                    //{
                    //    groundJson = groundJson.Substring(0, groundJson.Length - 1);
                    //    groundJson += "[" + groundJson + "]";
                    //}
                }
                //Newtonsoft.Json.Linq.JObject groundArray = Newtonsoft.Json.Linq.JObject.Parse(groundJson);
                //Newtonsoft.Json.Linq.JObject jObjGround = Newtonsoft.Json.Linq.JObject.FromObject(groundArray);
                //result["ground"] = jObjGround;
                ground.data = dataMemberList;
                Newtonsoft.Json.Linq.JObject jObjGround = Newtonsoft.Json.Linq.JObject.FromObject(ground);
                result["ground"] = jObjGround;

                AutoView autoView = new AutoView();
                string av = Db.Queryable<tn_dts_setting>()
                .Where(it => it.cn_s_setting_keycode == "AutoView")
                .Select(it => new
                {
                    Interval = it.cn_s_setting_keyvalue
                }).MergeTable().Select(it => it.Interval).First();
                if (String.IsNullOrEmpty(av))
                {
                    autoView.interval = null;
                }
                else
                {
                    autoView.interval = double.Parse(av);
                }

                Newtonsoft.Json.Linq.JObject jObjAutoView = Newtonsoft.Json.Linq.JObject.FromObject(autoView);
                result["autoView"] = jObjAutoView;

                Position position = new Position();
                string ox = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroX").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ox))
                {
                    position.originX = null;
                }
                else
                {
                    position.originX = double.Parse(ox);
                }
                string oy = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroY").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(oy))
                {
                    position.originY = null;
                }
                else
                {
                    position.originY = double.Parse(oy);
                }

                Newtonsoft.Json.Linq.JObject jObjPosition = Newtonsoft.Json.Linq.JObject.FromObject(position);
                result["position"] = jObjPosition;

                SysConfig sysconfig = new SysConfig();
                sysconfig.systemName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemName").Select(it => it.cn_s_setting_keyvalue).First();
                sysconfig.systemLogoURL = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemLogoURL").Select(it => it.cn_s_setting_keyvalue).First();
                sysconfig.webSocketServer = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "WebSocketServer").Select(it => it.cn_s_setting_keyvalue).First();


                Newtonsoft.Json.Linq.JObject jObjSysconfig = Newtonsoft.Json.Linq.JObject.FromObject(sysconfig);
                result["sysConfig"] = jObjSysconfig;

                SysAuthority sysAuthority = new SysAuthority();
                string ermm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ermm))
                {
                    sysAuthority.enabledRealMonitorMenu = null;
                }
                else
                {
                    sysAuthority.enabledRealMonitorMenu = bool.Parse(ermm);
                }
                string eddm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledDeviceDataMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(eddm))
                {
                    sysAuthority.enabledDeviceDataMenu = null;
                }
                else
                {
                    sysAuthority.enabledDeviceDataMenu = bool.Parse(eddm);
                }
                string ebmm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledBussMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ebmm))
                {
                    sysAuthority.enabledBussMonitorMenu = null;
                }
                else
                {
                    sysAuthority.enabledBussMonitorMenu = bool.Parse(ebmm);
                }
                string evvm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledVirtualVideoMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(evvm))
                {
                    sysAuthority.enabledVirtualVideoMenu = null;
                }
                else
                {
                    sysAuthority.enabledVirtualVideoMenu = bool.Parse(evvm);
                }

                BussMonitorMenuSett bussMonitorMenuSett = new BussMonitorMenuSett();
                string icm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "isCustomMenu").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(icm))
                {
                    bussMonitorMenuSett.isCustomMenu = null;
                }
                else
                {
                    bussMonitorMenuSett.isCustomMenu = bool.Parse(icm);
                }
                bussMonitorMenuSett.menuName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "MenuName").Select(it => it.cn_s_setting_keyvalue).First();
                bussMonitorMenuSett.menuUrl = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "MenuUrl").Select(it => it.cn_s_setting_keyvalue).First();
                sysAuthority.bussMonitorMenuSett = bussMonitorMenuSett;
                string epm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledPlannedModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(epm))
                {
                    sysAuthority.enabledPlannedModule = null;
                }
                else
                {
                    sysAuthority.enabledPlannedModule = bool.Parse(epm);
                }
                string hdldf = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "HomeDeviceListDefaultFold").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(hdldf))
                {
                    sysAuthority.homeDeviceListDefaultFold = null;
                }
                else
                {
                    sysAuthority.homeDeviceListDefaultFold = bool.Parse(hdldf);
                }
                string ehdql = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledHomeDeviceQueryList").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ehdql))
                {
                    sysAuthority.enabledHomeDeviceQueryList = null;
                }
                else
                {
                    sysAuthority.enabledHomeDeviceQueryList = bool.Parse(ehdql);
                }
                string ertm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealTimeModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ertm))
                {
                    sysAuthority.enabledRealTimeModule = null;
                }
                else
                {
                    sysAuthority.enabledRealTimeModule = bool.Parse(ertm);
                }
                string ercsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRepairCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(ercsm))
                {
                    sysAuthority.enabledRepairCountShowModule = null;
                }
                else
                {
                    sysAuthority.enabledRepairCountShowModule = bool.Parse(ercsm);
                }
                string eucsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledUpkeepCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(eucsm))
                {
                    sysAuthority.enabledUpkeepCountShowModule = null;
                }
                else
                {
                    sysAuthority.enabledUpkeepCountShowModule = bool.Parse(eucsm);
                }
                string eecsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledErrorsCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(eucsm))
                {
                    sysAuthority.enabledErrorsCountShowModule = null;
                }
                else
                {
                    sysAuthority.enabledErrorsCountShowModule = bool.Parse(eecsm);
                }
                string emqm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledMaterialQueryModule").Select(it => it.cn_s_setting_keyvalue).First();
                if (String.IsNullOrEmpty(emqm))
                {
                    sysAuthority.enabledMaterialQueryModule = null;
                }
                else
                {
                    sysAuthority.enabledMaterialQueryModule = bool.Parse(emqm);
                }

                Newtonsoft.Json.Linq.JObject jObjSysAuthority = Newtonsoft.Json.Linq.JObject.FromObject(sysAuthority);
                result["sysAuthority"] = jObjSysAuthority;

                //List<string> agvguids = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == "AGV" && it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常").Select(it => it.cn_guid).ToList();
                List<string> agvguids = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常").Select(it => it.cn_guid).ToList();
                List<Newtonsoft.Json.Linq.JObject> attrAGVList = new List<Newtonsoft.Json.Linq.JObject>();//AGV固定对象
                List<DynamicsObjectClass> ontherList = new List<DynamicsObjectClass>();//其他通用对象

                foreach (var guid in agvguids)
                {
                    //tn_dts_equiobject //tn_dts_equiobjectattr
                    var eqObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equiguid == guid).ToList();

                    //取一个设备固定 AGV对象名下面的所有属性
                    //var agvObj=eqObjectList.FirstOrDefault(p => p.cn_s_object_name == "AGV");
                    //if (agvObj != null)
                    //{
                    //    string jsonString = "";
                    //    var eqAttrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == agvObj.cn_guid).ToList();
                    //    foreach (var attrModel in eqAttrList)
                    //    {
                    //        jsonString += "\"" + attrModel.cn_s_objectattr_attrname + "\":\"" + attrModel.cn_s_objectattr_attrvalue + "\",";

                    //    }
                    //    if (jsonString != "")
                    //    {
                    //        jsonString = jsonString.Substring(0, jsonString.Length - 1);

                    //        jsonString = "{" + jsonString + "}";

                    //        Newtonsoft.Json.Linq.JObject jsonArray = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                    //        attrAGVList.Add(jsonArray);
                    //    }
                    //}


                    //其他通用设备对象(包含充电机对象)
                    var ontherObjList = eqObjectList;// eqObjectList.Where(p => p.cn_s_object_name != "AGV").ToList();
                    foreach (var ontherObj in ontherObjList)
                    {
                        var objModel = ontherList.FirstOrDefault(p => p.objectKeyName == ontherObj.cn_s_object_name);
                        if (objModel == null)
                        {
                            ontherList.Add(new DynamicsObjectClass() { objectKeyName = ontherObj.cn_s_object_name });
                        }
                    }

                }
                //result["AGV"] = Newtonsoft.Json.Linq.JArray.FromObject(attrAGVList);

                //List<Newtonsoft.Json.Linq.JObject> attrOntherList = new List<Newtonsoft.Json.Linq.JObject>();//其他通用对象
                //foreach (var dynamicsObject in ontherList)
                //{
                //    var eqObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == dynamicsObject.objectKeyName).ToList();
                //    foreach(var eqObject in eqObjectList)
                //    {
                //        string jsonString = "";
                //        var eqAttrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == eqObject.cn_guid).ToList();
                //        foreach (var attrModel in eqAttrList)
                //        {
                //            jsonString += "\"" + attrModel.cn_s_objectattr_attrname + "\":\"" + attrModel.cn_s_objectattr_attrvalue + "\",";
                //        }
                //        if (jsonString != "")
                //        {
                //            jsonString = jsonString.Substring(0, jsonString.Length - 1);
                //            jsonString = "{" + jsonString + "}";
                //            Newtonsoft.Json.Linq.JObject jsonArray = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                //            attrOntherList.Add(jsonArray);
                //        }
                //    }
                //    dynamicsObject.objectValue = attrOntherList;
                //    result[""+ dynamicsObject.objectKeyName + ""] = Newtonsoft.Json.Linq.JArray.FromObject(dynamicsObject.objectValue);
                //}

                List<Newtonsoft.Json.Linq.JObject> attrOntherList = new List<Newtonsoft.Json.Linq.JObject>();//其他通用对象
                foreach (var dynamicsObject in ontherList)
                {
                    var eqObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == dynamicsObject.objectKeyName).ToList();
                    foreach (var eqObject in eqObjectList)
                    {
                        string jsonString = "";
                        var eqAttrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == eqObject.cn_guid).ToList();
                        foreach (var attrModel in eqAttrList)
                        {
                            jsonString += "\"" + attrModel.cn_s_objectattr_attrname + "\":\"" + attrModel.cn_s_objectattr_attrvalue + "\",";
                        }
                        if (jsonString != "")
                        {
                            jsonString = jsonString.Substring(0, jsonString.Length - 1);
                            jsonString = "{" + jsonString + "}";
                            Newtonsoft.Json.Linq.JObject jsonArray = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                            attrOntherList.Add(jsonArray);
                        }
                    }
                    dynamicsObject.objectValue = attrOntherList;
                    result["" + dynamicsObject.objectKeyName + ""] = Newtonsoft.Json.Linq.JArray.FromObject(dynamicsObject.objectValue);
                    attrOntherList.Clear();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
            return result;
        }

        #region 获取通用初始化信息
        /// <summary>
        /// 获取通用初始化信息
        /// </summary>
        /// <returns>通用初始化信息</returns>
        //public ViewSystemconfigModel<T> GetSystemconfig<T>(string attrname)
        //{
        //    ViewSystemconfigModel<T> viewSystemconfigModel = new ViewSystemconfigModel<T>();
        //    try
        //    {
        //        List<tn_dts_stock3d> stock3dList = Db.Queryable<tn_dts_stock3d>().ToList();
        //        List<Region> regionList = new List<Region>();
        //        foreach (var stock3d in stock3dList)
        //        {
        //            Region region = new Region();
        //            region.Name = stock3d.cn_s_location_areaname;
        //            region.ShowGoods = stock3d.cn_s_location_isshow;
        //            region.ColNum = stock3d.cn_s_location_col;
        //            region.LevelNum = stock3d.cn_s_location_layer;
        //            region.RowNum = stock3d.cn_s_location_row;
        //            region.SizeX = stock3d.cn_s_location_length;
        //            region.SizeY = stock3d.cn_s_location_height;
        //            region.SizeZ = stock3d.cn_s_location_width;
        //            region.RowGap = stock3d.cn_s_location_gap;
        //            region.OrigionPointX = stock3d.cn_s_location_xpos;
        //            region.OrigionPointY = stock3d.cn_s_location_ypos;
        //            region.Invalid = GetNullify(stock3d.cn_s_location_stockcode, stock3d.cn_s_location_areacode);
        //            regionList.Add(region);
        //        }
        //        viewSystemconfigModel.Regions = regionList;

        //        Ground ground = new Ground();
        //        ground.Data = Db.Queryable<Model.Entity.location.tn_dts_siteinfo>()
        //        .Select(it => new DataMember
        //        {
        //            ShowGoods = it.cn_s_siteinfo_isshow,
        //            Code = it.cn_s_siteinfo_code,
        //            PositionX = it.cn_s_siteinfo_xpos,
        //            PositionY = it.cn_s_siteinfo_ypos,
        //            SizeX = it.cn_s_siteinfo_lenght,
        //            SizeY = it.cn_s_siteinfo_height,
        //            SizeZ = it.cn_s_siteinfo_width
        //        }).ToList();
        //        viewSystemconfigModel.Ground = ground;

        //        List<string> guidList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == attrname).Select(it => it.cn_guid).ToList();
        //        foreach (var guid in guidList)
        //        {
        //            List<tn_dts_equiobjectattr> objectattrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == guid).ToList();
        //            var nametypevalueList = objectattrList.Select(it => new
        //            {
        //                attrname = it.cn_s_objectattr_attrname,
        //                attrtype = it.cn_s_objectattr_attrtype,
        //                attrvalue = it.cn_s_objectattr_attrvalue
        //            });
        //            var list = new[] { new 
        //            {

        //            }}
        //            foreach (var nametypevalue in nametypevalueList)
        //            {
        //                var 
        //            }
        //        }
        //        List<string> agvnos = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == "AGV" && it.cn_s_equi_parttype == "整机").Select(it => it.cn_s_equi_no).ToList();
        //        var query = Db.Queryable<tn_dts_equipment>()
        //        .FullJoin<tn_dts_equiobject>((e, o) => e.cn_guid == o.cn_s_object_equiguid)
        //        .FullJoin<tn_dts_equiobjectattr>((e, o, oa) => o.cn_guid == oa.cn_s_objectattr_guid)
        //        //.Where((e, o, oa) => e.cn_s_equi_type == "AGV" && e.cn_s_equi_parttype == "整机")
        //        .Select((e, o, oa) => new
        //        {
        //            cn_s_equi_no = e.cn_s_equi_no,
        //            cn_s_equi_name = e.cn_s_equi_name,
        //            cn_s_equi_type = e.cn_s_equi_type,
        //            cn_s_object_equiguid = e.cn_guid,
        //            cn_s_objectattr_guid = o.cn_guid,
        //            cn_s_objectattr_attrname = oa.cn_s_objectattr_attrname,
        //            cn_s_objectattr_attrvalue = oa.cn_s_objectattr_attrvalue
        //        });
        //        foreach (var agvno in agvnos)
        //        {
        //            AGVMember agv = new AGVMember();
        //            agv.Name = agvno;
        //            string s = query.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "Speed").Select(it => it.cn_s_objectattr_attrvalue).First();
        //            if (String.IsNullOrEmpty(s))
        //            {
        //                agv.Speed = null;
        //            }
        //            else
        //            {
        //                agv.Speed = double.Parse(s);
        //            }
        //            string ros = query.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "RotateSpeed").Select(it => it.cn_s_objectattr_attrvalue).First();
        //            if (String.IsNullOrEmpty(ros))
        //            {
        //                agv.RotateSpeed = null;
        //            }
        //            else
        //            {
        //                agv.RotateSpeed = double.Parse(ros);
        //            }
        //            string ix = query.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "InitPosX").Select(it => it.cn_s_objectattr_attrvalue).First();
        //            if (String.IsNullOrEmpty(ix))
        //            {
        //                agv.InitPosX = null;
        //            }
        //            else
        //            {
        //                agv.InitPosX = double.Parse(ix);
        //            }
        //            string iy = query.MergeTable().Where(it => it.cn_s_equi_no == agvno && it.cn_s_objectattr_attrname == "InitPosY").Select(it => it.cn_s_objectattr_attrvalue).First();
        //            if (String.IsNullOrEmpty(iy))
        //            {
        //                agv.InitPosY = null;
        //            }
        //            else
        //            {
        //                agv.InitPosY = double.Parse(iy);
        //            }
        //            agvs.Add(agv);
        //        }
        //        viewSystemconfigModel.AGV = agvs;

        //        AutoView autoView = new AutoView();
        //        string av = Db.Queryable<tn_dts_setting>()
        //        .Where(it => it.cn_s_setting_keycode == "AutoView")
        //        .Select(it => new
        //        {
        //            Interval = it.cn_s_setting_keyvalue
        //        }).MergeTable().Select(it => it.Interval).First();
        //        if (String.IsNullOrEmpty(av))
        //        {
        //            autoView.Interval = null;
        //        }
        //        else
        //        {
        //            autoView.Interval = double.Parse(av);
        //        }
        //        viewSystemconfigModel.AutoView = autoView;

        //        Position position = new Position();
        //        string ox = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroX").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ox))
        //        {
        //            position.OriginX = null;
        //        }
        //        else
        //        {
        //            position.OriginX = double.Parse(ox);
        //        }
        //        string oy = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "ZeroY").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(oy))
        //        {
        //            position.OriginY = null;
        //        }
        //        else
        //        {
        //            position.OriginY = double.Parse(oy);
        //        }
        //        viewSystemconfigModel.Position = position;

        //        SysConfig sysconfig = new SysConfig();
        //        sysconfig.SystemName = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "SystemName").Select(it => it.cn_s_setting_keyvalue).First();
        //        sysconfig.WebSocketServer = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "WebSocketServer").Select(it => it.cn_s_setting_keyvalue).First();
        //        viewSystemconfigModel.SysConfig = sysconfig;

        //        SysAuthority sysAuthority = new SysAuthority();
        //        string ermm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ermm))
        //        {
        //            sysAuthority.EnabledRealMonitorMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledRealMonitorMenu = bool.Parse(ermm);
        //        }
        //        string eddm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledDeviceDataMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(eddm))
        //        {
        //            sysAuthority.EnabledDeviceDataMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledDeviceDataMenu = bool.Parse(eddm);
        //        }
        //        string ebmm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledBussMonitorMenu").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ebmm))
        //        {
        //            sysAuthority.EnabledBussMonitorMenu = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledBussMonitorMenu = bool.Parse(ebmm);
        //        }
        //        string epm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledPlannedModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(epm))
        //        {
        //            sysAuthority.EnabledPlannedModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledPlannedModule = bool.Parse(epm);
        //        }
        //        string hdldf = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "HomeDeviceListDefaultFold").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(hdldf))
        //        {
        //            sysAuthority.HomeDeviceListDefaultFold = null;
        //        }
        //        else
        //        {
        //            sysAuthority.HomeDeviceListDefaultFold = bool.Parse(hdldf);
        //        }
        //        string ehdql = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledHomeDeviceQueryList").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ehdql))
        //        {
        //            sysAuthority.EnabledHomeDeviceQueryList = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledHomeDeviceQueryList = bool.Parse(ehdql);
        //        }
        //        string ertm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRealTimeModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ertm))
        //        {
        //            sysAuthority.EnabledRealTimeModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledRealTimeModule = bool.Parse(ertm);
        //        }
        //        string ercsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledRepairCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(ercsm))
        //        {
        //            sysAuthority.EnabledRepairCountShowModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledRepairCountShowModule = bool.Parse(ercsm);
        //        }
        //        string eucsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledUpkeepCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(eucsm))
        //        {
        //            sysAuthority.EnabledUpkeepCountShowModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledUpkeepCountShowModule = bool.Parse(eucsm);
        //        }
        //        string eecsm = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledErrorsCountShowModule").Select(it => it.cn_s_setting_keyvalue).First();
        //        if (String.IsNullOrEmpty(eucsm))
        //        {
        //            sysAuthority.EnabledErrorsCountShowModule = null;
        //        }
        //        else
        //        {
        //            sysAuthority.EnabledErrorsCountShowModule = bool.Parse(eecsm);
        //        }
        //        viewSystemconfigModel.SysAuthority = sysAuthority;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(ex.Message);
        //    }
        //    return viewSystemconfigModel;
        //}
        #endregion

        #region 获取指定仓库，指定区域的报废货位列表字符串
        /// <summary>
        /// 获取指定仓库，指定区域的报废货位列表字符串
        /// </summary>
        /// <param name="stockcode"></param>
        /// <param name="areacode"></param>
        /// <returns></returns>
        public List<string> GetNullify(string stockcode, string areacode)
        {
            var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == stockcode && it.area_code == areacode && it.type == "货位" && it.location_state == "报废");
            List<LocationSiteInformation> locationSiteInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filter);
            List<string> stringList = new List<string>();
            foreach (var locationSiteInformation in locationSiteInformationList)
            {
                //stringList.Add(locationSiteInformation.locationCode);
                string rowcolfloor = string.Empty;
                string row = locationSiteInformation.row;
                if (string.IsNullOrEmpty(row))
                {
                    row = "0";
                }
                int rowValue = int.Parse(row);
                if (rowValue / 10 == 0)
                {
                    row = "0" + rowValue.ToString();
                }
                string col = locationSiteInformation.col;
                if (string.IsNullOrEmpty(col))
                {
                    col = "0";
                }
                int colValue = int.Parse(col);
                if (colValue / 10 == 0)
                {
                    col = "0" + colValue.ToString();
                }
                string floor = locationSiteInformation.floor;
                if (string.IsNullOrEmpty(floor))
                {
                    floor = "0";
                }
                int floorValue = int.Parse(floor);
                if (floorValue / 10 == 0)
                {
                    floor = "0" + floorValue.ToString();
                }
                rowcolfloor = row + "-" + col + "-" + floor;
                stringList.Add(rowcolfloor);
            }
            return stringList;
        }
        #endregion

        #region 获取零部件（只查一级）
        /// <summary>
        /// 获取零部件（只查一级）
        /// </summary>
        /// <param name="partDetail"></param>
        /// <returns></returns>
        public ReturnPartDetail GetPartDetail(PartDetail partDetail)
        {
            tn_dts_equipment completeMachine = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == partDetail.deviceCode).First();
            if (completeMachine is null)
            {
                return null;
            }
            ReturnPartDetail returnPartDetail = new ReturnPartDetail();
            returnPartDetail.deviceCode = completeMachine.cn_s_equi_no;
            returnPartDetail.deviceName = completeMachine.cn_s_equi_name;
            returnPartDetail.deviceType = completeMachine.cn_s_equi_type;
            if (completeMachine.cn_s_equi_parttype == "整机")
            {
                int one = 1;
                returnPartDetail.parttype = one.ToString();
            }
            else
            {
                int two = 2;
                returnPartDetail.parttype = two.ToString();
            }
            List<string> partNoList = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == partDetail.deviceCode).Select(it => it.cn_s_equibom_childno).ToList();
            List<PartDetailModel> partDetailModelList = new List<PartDetailModel>();
            foreach (var partNo in partNoList)
            {
                tn_dts_equipment part = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == partNo).First();
                PartDetailModel partDetailModel = new PartDetailModel();
                partDetailModel.deviceCode = part.cn_s_equi_no;
                partDetailModel.deviceName = part.cn_s_equi_name;
                partDetailModel.lightReminder = GetLightReminder(partNo);
                partDetailModelList.Add(partDetailModel);
            }
            returnPartDetail.partdetail = partDetailModelList;
            return returnPartDetail;
        }
        #endregion

        #region 通用日志接口
        /// <summary>
        /// 通用新增日志接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReturnMessage UniversallyAddLog(OpenLog model)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_logs logs = new tn_dts_logs();
            logs.cn_guid = Guid.NewGuid().ToString();
            logs.cn_s_logs_type = model.logtype;
            logs.cn_s_logs_clientip = model.clientip;
            logs.cn_s_logs_receiveurl = model.receiveurl;
            logs.cn_s_logs_receivepram = model.receivepram;
            logs.cn_s_logs_receiveresult = model.receiveresult;
            logs.cn_s_logs_requesturl = model.requesturl;
            logs.cn_s_logs_requestpram = model.requestpram;
            logs.cn_s_logs_requestresult = model.requestresult;
            logs.cn_s_logs_optionpath = model.optionpath;
            logs.cn_s_logs_errorsinfo = model.errorsinfo;
            logs.cn_s_logs_remarks = model.remarks;
            logs.cn_s_creator = user.UserCode;
            logs.cn_s_creator_by = user.UserName;
            logs.cn_t_create = DateTime.Now;
            int reslogs = Db.Insertable(logs).ExecuteCommand();
            if (reslogs <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "日志信息插入tn_dts_logs表失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "插入成功";
            return returnMessage;
        }
        #endregion

        #region 获取每排间隔数组
        /// <summary>
        /// 获取每排间隔数组
        /// </summary>
        /// <param name="locationgap"></param>
        /// <returns></returns>
        public List<double> GetRowGap(string locationgap)
        {
            List<double> doubleList = new List<double>();
            if (!string.IsNullOrEmpty(locationgap))
            {
                string gap = locationgap.Remove(locationgap.Length - 1, 1).Remove(0, 1);
                String[] doubleItemList = gap.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var doubleItem in doubleItemList)
                {
                    string item = doubleItem;
                    if (doubleItem[0] == '\"')
                    {
                        item = doubleItem.Remove(doubleItem.Length - 1, 1).Remove(0, 1);
                    }
                    double outdouble = 0;
                    double.TryParse(item, out outdouble);
                    doubleList.Add(outdouble);
                }
            }
            return doubleList;
        }
        #endregion

        #region 获取菜单
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public MenuAuthorityModel GetMenu()
        {
            MenuAuthorityModel menuAuthorityModel = new MenuAuthorityModel();
            List<tn_dts_3dviewmenu> threedviewmenuList = Db.Queryable<tn_dts_3dviewmenu>().OrderBy(it => it.cn_s_3dview_menusort).ToList();
            List<Navigate> navigateList = new List<Navigate>();
            foreach (var threedviewmenu in threedviewmenuList)
            {
                Navigate navigate = new Navigate();
                navigate.ParentMenuid = threedviewmenu.cn_s_3dview_parentmenuid;
                navigate.Menuid = threedviewmenu.cn_s_3dview_menuid;
                navigate.MenuName = threedviewmenu.cn_s_3dview_menuname;
                navigate.MenuIco = threedviewmenu.cn_s_3dview_menuico;
                navigate.MenuUrl = threedviewmenu.cn_s_3dview_menuurl;
                navigate.Sort = threedviewmenu.cn_s_3dview_menusort;
                if (threedviewmenu.cn_s_3dview_menuisshow == 0)
                {
                    navigate.IsShow = false;
                }
                else
                {
                    navigate.IsShow = true;
                }
                navigate.ModuleAuthority = JsonConvert.DeserializeObject<List<Authority>>(threedviewmenu.cn_s_3dview_menumoduleauth);
                navigateList.Add(navigate);
            }
            menuAuthorityModel.Navigate = navigateList;
            return menuAuthorityModel;
        }
        #endregion

        #region 获取首页视角区域
        /// <summary>
        /// 获取首页视角区域
        /// </summary>
        /// <returns></returns>
        public HomeAngleViewAreaModel GetHomeAngle()
        {
            HomeAngleViewAreaModel homeAngleViewAreaModel = new HomeAngleViewAreaModel();
            HomeAngle homeAngle = new HomeAngle();
            string enabledAutoView = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "EnabledAutoView").Select(it => it.cn_s_setting_keyvalue).First();
            if (string.IsNullOrEmpty(enabledAutoView))
            {
                throw new Exception("tn_dts_setting表中找不到关键字编码为“EnabledAutoView”的数据或是关键字编码为“EnabledAutoView”的设置数据其设置值为空。");
            }
            homeAngle.EnabledAutoView = Convert.ToBoolean(enabledAutoView);
            string autoViewInterval = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "AutoView").Select(it => it.cn_s_setting_keyvalue).First();
            if (string.IsNullOrEmpty(autoViewInterval))
            {
                throw new Exception("tn_dts_setting表中找不到关键字编码为“AutoView”的数据或是关键字编码为“AutoView”的设置数据其设置值为空。");
            }
            homeAngle.AutoViewInterval = Convert.ToInt32(autoViewInterval);
            List<tn_dts_3danglearea> angleareaList = Db.Queryable<tn_dts_3danglearea>().OrderBy(it => it.cn_s_3danglearea_serial).ToList();
            List<AngleView> angleViewList = new List<AngleView>();
            foreach (var anglearea in angleareaList)
            {
                AngleView angleView = new AngleView();
                angleView.AreaCode = anglearea.cn_s_3danglearea_code;
                angleView.AreaName = anglearea.cn_s_3danglearea_name;
                if (!anglearea.cn_s_3danglearea_serial.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_serial项数据为空。");
                }
                angleView.Sort = anglearea.cn_s_3danglearea_serial.Value;
                if (!anglearea.cn_s_3danglearea_posX.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_posX项数据为空。");
                }
                angleView.PosX = anglearea.cn_s_3danglearea_posX.Value;
                if (!anglearea.cn_s_3danglearea_posY.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_posY项数据为空。");
                }
                angleView.PosY = anglearea.cn_s_3danglearea_posY.Value;
                if (!anglearea.cn_s_3danglearea_posZ.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_posZ项数据为空。");
                }
                angleView.PosZ = anglearea.cn_s_3danglearea_posZ.Value;
                if (!anglearea.cn_s_3danglearea_angleX.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_angleX项数据为空。");
                }
                angleView.AngleX = anglearea.cn_s_3danglearea_angleX.Value;
                if (!anglearea.cn_s_3danglearea_angleY.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_angleY项数据为空。");
                }
                angleView.AngleY = anglearea.cn_s_3danglearea_angleY.Value;
                if (!anglearea.cn_s_3danglearea_angleZ.HasValue)
                {
                    throw new Exception("tn_dts_3danglearea表中唯一标识为" + anglearea.cn_guid + "的视角区域记录中cn_s_3danglearea_angleZ项数据为空。");
                }
                angleView.AngleZ = anglearea.cn_s_3danglearea_angleZ.Value;
                angleViewList.Add(angleView);
            }
            homeAngle.AngleViewInfo = angleViewList;
            homeAngleViewAreaModel.HomeAngle = homeAngle;
            return homeAngleViewAreaModel;
        }
        #endregion

        #region 获取爆炸图保养提醒灯颜色
        /// <summary>
        /// 获取爆炸图保养提醒灯颜色
        /// </summary>
        /// <param name="equino"></param>
        /// <returns></returns>
        public int GetLightReminder(string equino)
        {
            tn_dts_equipment equipmentNo = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == equino).First();
            tn_dts_equiupkeep upkeepNo = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equino).OrderBy(it => it.cn_t_equiupkeep_date, SqlSugar.OrderByType.Desc).First();
            if (equipmentNo is null || equipmentNo.cn_s_equi_buydate is null || equipmentNo.cn_s_equi_defentperiod is null)
            {
                LogHelper.Info("tn_dts_equipment表中设备编号为：" + equino + "的设备不存在或其购买日期、保养周期为空。");
                return 1;
            }
            string lightReminderTimeString = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "LightReminderTime").Select(it => it.cn_s_setting_keyvalue).First();
            if (lightReminderTimeString is null)
            {
                LogHelper.Info("tn_dts_setting表中没有关键字编码为LightReminderTime的记录或其对应的设置值为空。");
                return 1;
            }
            int lightReminderTime = int.Parse(lightReminderTimeString);
            if (upkeepNo is null)
            {
                DateTime upkeepTime = DateTime.Parse(equipmentNo.cn_s_equi_buydate.Value.AddDays(equipmentNo.cn_s_equi_defentperiod.Value).ToString());
                TimeSpan difference = upkeepTime - DateTime.Now;
                if (difference.Days > lightReminderTime)
                {
                    return 1;
                }
                else if (difference.Days > 0)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                if (upkeepNo.cn_t_equiupkeep_date is null)
                {
                    LogHelper.Info("tn_dts_equiupkeep表中设备编码为：" + equino + "的保养记录中有cn_t_equiupkeep_date字段值为空。");
                    return 1;
                }
                DateTime upkeepTime = DateTime.Parse(upkeepNo.cn_t_equiupkeep_date.Value.AddDays(equipmentNo.cn_s_equi_defentperiod.Value).ToString());
                TimeSpan difference = upkeepTime - DateTime.Now;
                if (difference.Days > lightReminderTime)
                {
                    return 1;
                }
                else if (difference.Days > 0)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }
        #endregion
    }
}
