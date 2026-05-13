using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class ChiprocedureService : BaseService<tn_dts_chiprocedure>, IChiprocedureService
    {
        public ChiprocedureService(SessionInfo session) : base(session)
        {

        }

        #region 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息和终点信息）
        /// <summary>
        /// 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息和终点信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiprocedureInformation> GetAllChiprocedureByParprocedureguid(PageParm param)
        {
            string parproguid = param.Parms["cn_s_chiprocedure_parproguid"].ObjToString();
            List<tn_dts_chiprocedure> chiprocedureList = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                .FullJoin<tn_dts_stock3d>((ch, eq, sts) => ch.cn_s_chiprocedure_startguid == sts.cn_guid).FullJoin<tn_dts_siteinfo>((ch, eq, sts, sis) => ch.cn_s_chiprocedure_startguid == sis.cn_guid)
                .FullJoin<tn_dts_stock3d>((ch, eq, sts, sis, ste) => ch.cn_s_chiprocedure_endguid == ste.cn_guid).FullJoin<tn_dts_siteinfo>((ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_endguid == sie.cn_guid)
                .WhereIF(!string.IsNullOrEmpty(parproguid), (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_parproguid == parproguid)
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_n_chiprocedure_sequence asc" : param.OrderBy)
                .ToList();
            List<ChiprocedureInformation> chiprocedureInformationList = new List<ChiprocedureInformation>();
            foreach (var chiprocedure in chiprocedureList)
            {
                if (chiprocedure.cn_s_chiprocedure_startcategory == "立库" && chiprocedure.cn_s_chiprocedure_endcategory == "立库")
                {
                    ChiprocedureInformation chiprocedureInformation = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_stock3d>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_stock3d>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformation
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartGuid = st.cn_guid,
                            StartType = ch.cn_s_chiprocedure_startcategory,
                            StartNo = st.cn_s_location_areacode,
                            StartName = st.cn_s_location_areaname,
                            EndGuid = en.cn_guid,
                            EndType = ch.cn_s_chiprocedure_endcategory,
                            EndNo = en.cn_s_location_areacode,
                            EndName = en.cn_s_location_areaname,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval
                        }).First();
                    chiprocedureInformationList.Add(chiprocedureInformation);
                }
                else if (chiprocedure.cn_s_chiprocedure_startcategory == "立库" && chiprocedure.cn_s_chiprocedure_endcategory == "地堆")
                {
                    ChiprocedureInformation chiprocedureInformation = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_stock3d>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_siteinfo>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformation
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartGuid = st.cn_guid,
                            StartType = ch.cn_s_chiprocedure_startcategory,
                            StartNo = st.cn_s_location_areacode,
                            StartName = st.cn_s_location_areaname,
                            EndGuid = en.cn_guid,
                            EndType = ch.cn_s_chiprocedure_endcategory,
                            EndNo = en.cn_s_siteinfo_code,
                            EndName = en.cn_s_siteinfo_name,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval
                        }).First();
                    chiprocedureInformationList.Add(chiprocedureInformation);
                }
                else if (chiprocedure.cn_s_chiprocedure_startcategory == "地堆" && chiprocedure.cn_s_chiprocedure_endcategory == "立库")
                {
                    ChiprocedureInformation chiprocedureInformation = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_siteinfo>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_stock3d>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformation
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartGuid = st.cn_guid,
                            StartType = ch.cn_s_chiprocedure_startcategory,
                            StartNo = st.cn_s_siteinfo_code,
                            StartName = st.cn_s_siteinfo_name,
                            EndGuid = en.cn_guid,
                            EndType = ch.cn_s_chiprocedure_endcategory,
                            EndNo = en.cn_s_location_areacode,
                            EndName = en.cn_s_location_areaname,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval
                        }).First();
                    chiprocedureInformationList.Add(chiprocedureInformation);
                }
                else if (chiprocedure.cn_s_chiprocedure_startcategory == "地堆" && chiprocedure.cn_s_chiprocedure_endcategory == "地堆")
                {
                    ChiprocedureInformation chiprocedureInformation = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_siteinfo>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_siteinfo>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformation
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartGuid = st.cn_guid,
                            StartType = ch.cn_s_chiprocedure_startcategory,
                            StartNo = st.cn_s_siteinfo_code,
                            StartName = st.cn_s_siteinfo_name,
                            EndGuid = en.cn_guid,
                            EndType = ch.cn_s_chiprocedure_endcategory,
                            EndNo = en.cn_s_siteinfo_code,
                            EndName = en.cn_s_siteinfo_name,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval
                        }).First();
                    chiprocedureInformationList.Add(chiprocedureInformation);
                }
                else
                {

                }
            }
            return chiprocedureInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息、终点信息和修改创建信息）
        /// <summary>
        /// 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息、终点信息和修改创建信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiprocedureInformationPlus> GetAllChiprocedurePlusByParprocedureguid(PageParm param)
        {
            string parproguid = param.Parms["cn_s_chiprocedure_parproguid"].ObjToString();
            string equiguid = param.Parms["cn_s_chiprocedure_equiguid"].ObjToString();
            string starttype = param.Parms["cn_s_chiprocedure_starttype"].ObjToString();
            string startguid = param.Parms["cn_s_chiprocedure_startguid"].ObjToString();
            string endtype = param.Parms["cn_s_chiprocedure_endtype"].ObjToString();
            string endguid = param.Parms["cn_s_chiprocedure_endguid"].ObjToString();
            List<tn_dts_chiprocedure> chiprocedureList = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                .FullJoin<tn_dts_stock3d>((ch, eq, sts) => ch.cn_s_chiprocedure_startguid == sts.cn_guid).FullJoin<tn_dts_siteinfo>((ch, eq, sts, sis) => ch.cn_s_chiprocedure_startguid == sis.cn_guid)
                .FullJoin<tn_dts_stock3d>((ch, eq, sts, sis, ste) => ch.cn_s_chiprocedure_endguid == ste.cn_guid).FullJoin<tn_dts_siteinfo>((ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_endguid == sie.cn_guid)
                .WhereIF(!string.IsNullOrEmpty(parproguid), (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_parproguid == parproguid)
                .WhereIF(!string.IsNullOrEmpty(equiguid), (ch, eq, sts, sis, ste, sie) => eq.cn_guid == equiguid)
                .WhereIF(!string.IsNullOrEmpty(starttype) && string.IsNullOrEmpty(endtype) && string.IsNullOrEmpty(startguid)
                ,(ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_startcategory == starttype)
                .WhereIF(starttype == "立库" && string.IsNullOrEmpty(endtype) && !string.IsNullOrEmpty(startguid) 
                , (ch, eq, sts, sis, ste, sie) => sts.cn_guid == startguid)
                .WhereIF(starttype == "地堆" && string.IsNullOrEmpty(endtype) && !string.IsNullOrEmpty(startguid)
                , (ch, eq, sts, sis, ste, sie) => sis.cn_guid == startguid)
                .WhereIF(string.IsNullOrEmpty(starttype) && !string.IsNullOrEmpty(endtype) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_endcategory == endtype)
                .WhereIF(endtype == "立库" && string.IsNullOrEmpty(starttype) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ste.cn_guid == endguid)
                .WhereIF(endtype == "地堆" && string.IsNullOrEmpty(starttype) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sie.cn_guid == endguid)
                .WhereIF(starttype == "立库" && endtype == "立库" && !string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sts.cn_guid == startguid)
                .WhereIF(starttype == "立库" && endtype == "立库" && string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ste.cn_guid == endguid)
                .WhereIF(starttype == "立库" && endtype == "立库" && !string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sts.cn_guid == startguid && ste.cn_guid == endguid)
                .WhereIF(starttype == "立库" && endtype == "立库" && string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_startcategory == "立库" && ch.cn_s_chiprocedure_endcategory == "立库")
                .WhereIF(starttype == "立库" && endtype == "地堆" && !string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sts.cn_guid == startguid)
                .WhereIF(starttype == "立库" && endtype == "地堆" && string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sie.cn_guid == endguid)
                .WhereIF(starttype == "立库" && endtype == "地堆" && !string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sts.cn_guid == startguid && sie.cn_guid == endguid)
                .WhereIF(starttype == "立库" && endtype == "地堆" && string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_startcategory == "立库" && ch.cn_s_chiprocedure_endcategory == "地堆")
                .WhereIF(starttype == "地堆" && endtype == "立库" && !string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sis.cn_guid == startguid)
                .WhereIF(starttype == "地堆" && endtype == "立库" && string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ste.cn_guid == endguid)
                .WhereIF(starttype == "地堆" && endtype == "立库" && !string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sis.cn_guid == startguid && ste.cn_guid == endguid)
                .WhereIF(starttype == "地堆" && endtype == "立库" && string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_startcategory == "地堆" && ch.cn_s_chiprocedure_endcategory == "立库")
                .WhereIF(starttype == "地堆" && endtype == "地堆" && !string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sis.cn_guid == startguid)
                .WhereIF(starttype == "地堆" && endtype == "地堆" && string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sie.cn_guid == endguid)
                .WhereIF(starttype == "地堆" && endtype == "地堆" && !string.IsNullOrEmpty(startguid) && !string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => sis.cn_guid == startguid && sie.cn_guid == endguid)
                .WhereIF(starttype == "地堆" && endtype == "地堆" && string.IsNullOrEmpty(startguid) && string.IsNullOrEmpty(endguid)
                , (ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_startcategory == "地堆" && ch.cn_s_chiprocedure_endcategory == "地堆")
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_n_chiprocedure_sequence asc" : param.OrderBy)
                .ToList();
            List<ChiprocedureInformationPlus> chiprocedureInformationPlusList = new List<ChiprocedureInformationPlus>();
            foreach (var chiprocedure in chiprocedureList)
            {
                //ChiprocedureInformationPlus chiprocedureInformationPlus = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                //.FullJoin<tn_dts_stock3d>((ch, eq, sts) => ch.cn_s_chiprocedure_startguid == sts.cn_guid).FullJoin<tn_dts_siteinfo>((ch, eq, sts, sis) => ch.cn_s_chiprocedure_startguid == sis.cn_guid)
                //.FullJoin<tn_dts_stock3d>((ch, eq, sts, sis, ste) => ch.cn_s_chiprocedure_endguid == ste.cn_guid).FullJoin<tn_dts_siteinfo>((ch, eq, sts, sis, ste, sie) => ch.cn_s_chiprocedure_endguid == sie.cn_guid)
                //.Where((ch, eq, sts, sis, ste, sie) => ch.cn_guid == chiprocedure.cn_guid)
                //.Select((ch, eq, sts, sis, ste, sie) => new ChiprocedureInformationPlus
                //{
                //    ChiprocedureGuid = ch.cn_guid,
                //    EquipmentGuid = eq.cn_guid,
                //    EquipmentNo = eq.cn_s_equi_no,
                //    EquipmentName = eq.cn_s_equi_name,
                //    //StartNo = starttype == "立库" ? sts.cn_s_location_areacode : sis.cn_s_siteinfo_code,
                //    //StartName = starttype == "立库" ? sts.cn_s_location_areaname : sis.cn_s_siteinfo_name,
                //    //EndNo = endtype == "立库" ? ste.cn_s_location_areacode : sie.cn_s_siteinfo_code,
                //    //EndName = endtype == "立库" ? ste.cn_s_location_areaname : sie.cn_s_siteinfo_name,
                //    StartNo = !string.IsNullOrEmpty(sts.cn_s_location_areacode) ? sts.cn_s_location_areacode : sis.cn_s_siteinfo_code,
                //    StartName = !string.IsNullOrEmpty(sts.cn_s_location_areaname) ? sts.cn_s_location_areaname : sis.cn_s_siteinfo_name,
                //    EndNo = !string.IsNullOrEmpty(ste.cn_s_location_areacode) ? ste.cn_s_location_areacode : sie.cn_s_siteinfo_code,
                //    EndName = !string.IsNullOrEmpty(ste.cn_s_location_areaname) ? ste.cn_s_location_areaname : sie.cn_s_siteinfo_name,
                //    ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                //    ChiprocedureInterval = ch.cn_n_chiprocedure_interval,
                //    ModifyNo = ch.cn_s_modify,
                //    ModifyName = ch.cn_s_modify_by,
                //    ModifyTime = ch.cn_t_modify,
                //    CreateNo = ch.cn_s_creator,
                //    CreateName = ch.cn_s_creator_by,
                //    CreateTime = ch.cn_t_create
                //}).First();
                //chiprocedureInformationPlusList.Add(chiprocedureInformationPlus);
                if(chiprocedure.cn_s_chiprocedure_startcategory == "立库" && chiprocedure.cn_s_chiprocedure_endcategory == "立库")
                {
                    ChiprocedureInformationPlus chiprocedureInformationPlus = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_stock3d>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_stock3d>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformationPlus
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartNo = st.cn_s_location_areacode,
                            StartName = st.cn_s_location_areaname,
                            EndNo = en.cn_s_location_areacode,
                            EndName = en.cn_s_location_areaname,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval,
                            ModifyNo = ch.cn_s_modify,
                            ModifyName = ch.cn_s_modify_by,
                            ModifyTime = ch.cn_t_modify,
                            CreateNo = ch.cn_s_creator,
                            CreateName = ch.cn_s_creator_by,
                            CreateTime = ch.cn_t_create
                        }).First();
                    chiprocedureInformationPlusList.Add(chiprocedureInformationPlus);
                }
                else if(chiprocedure.cn_s_chiprocedure_startcategory == "立库" && chiprocedure.cn_s_chiprocedure_endcategory == "地堆")
                {
                    ChiprocedureInformationPlus chiprocedureInformationPlus = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_stock3d>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_siteinfo>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformationPlus
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartNo = st.cn_s_location_areacode,
                            StartName = st.cn_s_location_areaname,
                            EndNo = en.cn_s_siteinfo_code,
                            EndName = en.cn_s_siteinfo_name,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval,
                            ModifyNo = ch.cn_s_modify,
                            ModifyName = ch.cn_s_modify_by,
                            ModifyTime = ch.cn_t_modify,
                            CreateNo = ch.cn_s_creator,
                            CreateName = ch.cn_s_creator_by,
                            CreateTime = ch.cn_t_create
                        }).First();
                    chiprocedureInformationPlusList.Add(chiprocedureInformationPlus);
                }
                else if(chiprocedure.cn_s_chiprocedure_startcategory == "地堆" && chiprocedure.cn_s_chiprocedure_endcategory == "立库")
                {
                    ChiprocedureInformationPlus chiprocedureInformationPlus = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_siteinfo>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_stock3d>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformationPlus
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartNo = st.cn_s_siteinfo_code,
                            StartName = st.cn_s_siteinfo_name,
                            EndNo = en.cn_s_location_areacode,
                            EndName = en.cn_s_location_areaname,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval,
                            ModifyNo = ch.cn_s_modify,
                            ModifyName = ch.cn_s_modify_by,
                            ModifyTime = ch.cn_t_modify,
                            CreateNo = ch.cn_s_creator,
                            CreateName = ch.cn_s_creator_by,
                            CreateTime = ch.cn_t_create
                        }).First();
                    chiprocedureInformationPlusList.Add(chiprocedureInformationPlus);
                }
                else if(chiprocedure.cn_s_chiprocedure_startcategory == "地堆" && chiprocedure.cn_s_chiprocedure_endcategory == "地堆")
                {
                    ChiprocedureInformationPlus chiprocedureInformationPlus = Db.Queryable<tn_dts_chiprocedure>().InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiprocedure_equiguid == eq.cn_guid)
                        .InnerJoin<tn_dts_siteinfo>((ch, eq, st) => ch.cn_s_chiprocedure_startguid == st.cn_guid).InnerJoin<tn_dts_siteinfo>((ch, eq, st, en) => ch.cn_s_chiprocedure_endguid == en.cn_guid)
                        .Where((ch, eq, st, en) => ch.cn_guid == chiprocedure.cn_guid)
                        .Select((ch, eq, st, en) => new ChiprocedureInformationPlus
                        {
                            ChiprocedureGuid = ch.cn_guid,
                            EquipmentGuid = eq.cn_guid,
                            EquipmentNo = eq.cn_s_equi_no,
                            EquipmentName = eq.cn_s_equi_name,
                            StartNo = st.cn_s_siteinfo_code,
                            StartName = st.cn_s_siteinfo_name,
                            EndNo = en.cn_s_siteinfo_code,
                            EndName = en.cn_s_siteinfo_name,
                            ChiprocedureSequence = ch.cn_n_chiprocedure_sequence,
                            ChiprocedureInterval = ch.cn_n_chiprocedure_interval,
                            ModifyNo = ch.cn_s_modify,
                            ModifyName = ch.cn_s_modify_by,
                            ModifyTime = ch.cn_t_modify,
                            CreateNo = ch.cn_s_creator,
                            CreateName = ch.cn_s_creator_by,
                            CreateTime = ch.cn_t_create
                        }).First();
                    chiprocedureInformationPlusList.Add(chiprocedureInformationPlus);
                }
                else
                {

                }
            }
            return chiprocedureInformationPlusList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

    }
}
