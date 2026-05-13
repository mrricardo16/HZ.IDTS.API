using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System.Collections.Generic;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class CacheService : BaseService<tn_dts_cache>, ICacheService
    {
        public CacheService(SessionInfo session) : base(session)
        {

        }

        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_cache>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }

        public PagedInfo<tn_dts_cache> GetListPages(PageParm parm)
        {
            return Db.Queryable<tn_dts_cache>()
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }

        #region 递归获取全部字典信息
        /// <summary>
        /// 调用递归算法实现获取一组父项编号的全部子类字典
        /// </summary>
        /// <param name="parentnameList"></param>
        /// <returns></returns>
        public List<DataDictionary> GetAllDict(List<DataDictionary> inputDataDictionaryList)
        {
            List<string> parentnameList = new List<string>();
            foreach (DataDictionary inputDataDictionary in inputDataDictionaryList)
            {
                parentnameList.Add(inputDataDictionary.dicName);
            }
            List<DataDictionary> dataDictionaryList = new List<DataDictionary>();
            foreach (string parentname in parentnameList)
            {
                GetAllDict_Recrusion(parentname, ref dataDictionaryList);
            }
            return dataDictionaryList;
        }

        /// <summary>
        /// 递归查询一项全部子类字典信息
        /// </summary>
        /// <param name="parentname"></param>
        /// <param name="outDictionaryList"></param>
        public void GetAllDict_Recrusion(string parentname, ref List<DataDictionary> outDictionaryList)
        {
            if (GetDictListByWeb(parentname) is null)
            {
            }
            else
            {
                List<DataDictionary> resDictionaryList = GetDictListByWeb(parentname);
                foreach (DataDictionary dataDictionary in resDictionaryList)
                {
                    outDictionaryList.Add(dataDictionary);
                    GetAllDict_Recrusion(dataDictionary.dicCode, ref outDictionaryList);
                }
            }
        }

        /// <summary>
        /// 请求Websocket获取MDG字典信息
        /// </summary>
        /// <param name="parentname"></param>
        /// <returns></returns>
        public List<DataDictionary> GetDictListByWeb(string parentname)
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult apiResDictList = new ApiResult();
            UserSession user = GetSessionInfo();
            string pathDefault = "/api/BasicWork/GetDictList?dictName=";
            string strDictList = WebApiManager.HttpGet(mdg, pathDefault + parentname, ref apiResDictList, "", user.TokenId);
            dynamic dictListDynamic = JsonConvert.DeserializeObject<dynamic>(strDictList);
            List<DataDictionary> dictList = JsonConvert.DeserializeObject<List<DataDictionary>>(dictListDynamic.Data.ToString());
            return dictList;
        }
        #endregion

    }
}
