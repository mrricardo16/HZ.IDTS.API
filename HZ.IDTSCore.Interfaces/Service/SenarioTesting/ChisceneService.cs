using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class ChisceneService : BaseService<tn_dts_chiscene>, IChisceneService
    {
        public ChisceneService(SessionInfo session) : base(session)
        {

        }

        #region  通过场景主表唯一标识获取所有场景子表信息（含流程信息）
        /// <summary>
        /// 通过场景主表唯一标识获取所有场景子表信息（含流程信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChisceneInformation> GetAllChisceneByParsceneguid(PageParm param)
        {
            string parsceguid = param.Parms["cn_s_chiscene_parsceguid"].ObjToString();
            List<tn_dts_chiscene> chisceneList = Db.Queryable<tn_dts_chiscene>().InnerJoin<tn_dts_parprocedure>((ch, pa) => ch.cn_s_chiscene_parproguid == pa.cn_guid)
                .WhereIF(!string.IsNullOrEmpty(parsceguid), (ch, pa) => ch.cn_s_chiscene_parsceguid == parsceguid)
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_n_chiscene_sequence asc" : param.OrderBy)
                .ToList();
            List<ChisceneInformation> chisceneInformationList = new List<ChisceneInformation>();
            foreach (var chiscene in chisceneList)
            {
                ChisceneInformation chisceneInformation = Db.Queryable<tn_dts_chiscene>().InnerJoin<tn_dts_parprocedure>((ch, pa) => ch.cn_s_chiscene_parproguid == pa.cn_guid)
                    .Where((ch, pa) => ch.cn_guid == chiscene.cn_guid)
                    .Select((ch, pa) => new ChisceneInformation
                    {
                        ChisceneGuid = ch.cn_guid,
                        ProcedureGuid = pa.cn_guid,
                        ProcedureNo = pa.cn_s_parprocedure_no,
                        ProcedureName = pa.cn_s_parprocedure_name,
                        ChisceneSequence = ch.cn_n_chiscene_sequence,
                        ChisceneInterval = ch.cn_n_chiscene_interval
                    }).First();
                chisceneInformationList.Add(chisceneInformation);
            }
            return chisceneInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 通过场景主表唯一标识获取所有场景子表信息（含流程信息和修改创建信息），按流程编码和流程名称分页模糊查询
        /// <summary>
        /// 通过场景主表唯一标识获取所有场景子表信息（含流程信息和修改创建信息），按流程编码和流程名称分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChisceneInformationPlus> GetAllChiscenePlusByParsceneguid(PageParm param)
        {
            string parsceguid = param.Parms["cn_s_chiscene_parsceguid"].ObjToString();
            string parprono = param.Parms["cn_s_parprocedure_no"].ObjToString();
            string parproname = param.Parms["cn_s_parprocedure_name"].ObjToString();
            List<tn_dts_chiscene> chisceneList = Db.Queryable<tn_dts_chiscene>().InnerJoin<tn_dts_parprocedure>((ch, pa) => ch.cn_s_chiscene_parproguid == pa.cn_guid)
               .WhereIF(!string.IsNullOrEmpty(parsceguid), (ch, pa) => ch.cn_s_chiscene_parsceguid == parsceguid)
               .WhereIF(!string.IsNullOrEmpty(parprono), (ch, pa) => pa.cn_s_parprocedure_no.Contains(parprono))
               .WhereIF(!string.IsNullOrEmpty(parproname), (ch,pa) => pa.cn_s_parprocedure_name.Contains(parproname))
               .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_n_chiscene_sequence asc" : param.OrderBy)
               .ToList();
            List<ChisceneInformationPlus> chisceneInformationPlusList = new List<ChisceneInformationPlus>();
            foreach (var chiscene in chisceneList)
            {
                ChisceneInformationPlus chisceneInformationPlus = Db.Queryable<tn_dts_chiscene>().InnerJoin<tn_dts_parprocedure>((ch, pa) => ch.cn_s_chiscene_parproguid == pa.cn_guid)
                    .Where((ch, pa) => ch.cn_guid == chiscene.cn_guid)
                    .Select((ch, pa) => new ChisceneInformationPlus
                    {
                        ProcedureNo = pa.cn_s_parprocedure_no,
                        ProcedureName = pa.cn_s_parprocedure_name,
                        ChisceneSequence = ch.cn_n_chiscene_sequence,
                        ChisceneInterval = ch.cn_n_chiscene_interval,
                        ModifyNo = ch.cn_s_modify,
                        ModifyName = ch.cn_s_modify_by,
                        ModifyTime = ch.cn_t_modify,
                        CreateNo = ch.cn_s_creator,
                        CreateName = ch.cn_s_creator_by,
                        CreateTime = ch.cn_t_create
                    }).First();
                chisceneInformationPlusList.Add(chisceneInformationPlus);
            }
            return chisceneInformationPlusList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion
    }
}
