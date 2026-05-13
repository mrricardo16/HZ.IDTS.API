using HZ.CommonUtil.ExceptionExtend;
using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Basic;
using HZ.IDTSCore.Model.InterfaceParams;
using HZ.IDTSCore.Model.SerializeEntity;
using HZ.Redis.RedisUserModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace HZ.IDTSCore.Interfaces.Service
{
    public class UserService : BaseService<SYS_USER>, IUserService
    {
        public UserService(SessionInfo session) : base(session) { }

        public AccountEntity Login(string username, string password)
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");

            var entity = new
            {
                userCode = username,
                userPwd = password,
                appCode = "WMS",
                ip = ""
            };
            string jsonString = JsonConvert.SerializeObject(entity);
            ApiResult model = new ApiResult();
            string result = WebApiManager.HttpPost(mdg, "api/Account/Login", jsonString, ref model);
            if (!model.IsSuccess)
            {
                return new AccountEntity()
                {
                    Data = model.Message,
                    Code = "1"
                };
            }
            model = JsonConvert.DeserializeObject<ApiResult>(result);

            if (model.IsSuccess)
            {
                JObject obj = JsonConvert.DeserializeObject<JObject>(model.Data.ToString());
                string token = obj["token"].ToString();

                RedisMsgEntity user = HZ.Redis.RedisUserinfoHelper.GetUserInfo(token);
                if (!user.Success)
                {
                    return new AccountEntity()
                    {
                        Data = user.Msg,
                        Code = "1"
                    };
                }
                return new AccountEntity()
                {
                    Data = model.Message,
                    Code = "0",
                    Login = user.userInfo.userCode,
                    UserName = user.userInfo.userName,
                    Token = token,
                    //SplitDbCode=user.userInfo.orgFlag,
                    SplitDbList = JsonConvert.DeserializeObject<List<SplitDb>>(obj["splitDbOrg"].ToString()),
                    userExt = new UserExt()
                    {
                        stockCode = user.userInfo.stockCode,
                        OrgCode = user.userInfo.orgCode,
                    }
                };
            }
            else
            {
                return new AccountEntity()
                {
                    Data = model.Message,
                    Code = "1"
                };
            }
        }

        /// <summary>
        /// 添加用户
        /// 咸阳项目
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userName"></param>
        /// <param name="phoneId">手机ID</param>
        /// <returns></returns>
        public ApiResult AddUser(string userCode, string userName, string phoneId)
        {
            var user = Db.Deleteable<tn_wms_user>().Where(e => e.cn_s_user_code == userCode || e.cn_s_phone_id == phoneId).ExecuteCommand();
            Db.Insertable(new tn_wms_user
            {
                cn_s_user_code = userCode,
                cn_s_user_name = userName,
                cn_s_phone_id = phoneId,
                cn_t_create = DateTime.Now,
                cn_t_modify = DateTime.Now
            }).ExecuteCommand();
            return ApiResult.Success();
        }

        /// <summary>
        /// 根据角色编号获取角色用户
        /// </summary>
        /// <param name="roleCode">角色编号</param>
        /// <returns></returns>
        public List<RoleUser> GetRoleUser(string roleCode)
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi; //AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult res = new ApiResult();
            string result = WebApiManager.HttpGet(mdg, "api/EmployeeWork/GetUserByRole?roleCode=" + roleCode, ref res, "", LoginToken());
            if (!res.IsSuccess)
                throw new WebException(res.Message);
            res = JsonConvert.DeserializeObject<ApiResult>(result);
            if (res.IsSuccess)
            {
                return JsonConvert.DeserializeObject<List<RoleUser>>(res.Data.ToString());
            }
            throw new Exception(res.Message);
        }

        public string LoginToken()
        {
            IUserService _IUserService = ServiceLocator.GetService<IUserService>(GetCurrSession());

            AccountEntity account = _IUserService.Login("hz", "123456");
            if (account == null)
            {
                throw new Exception("请检查MDG登录接口！");
            }
            if (account.Code == "0")
            {
                UserSession userSeesion = new UserSession()
                {
                    OrgCode = account.userExt.OrgCode,
                    OrgFlag = account.userExt.OrgFlag,
                    OrgName = account.userExt.OrgName,
                    TokenId = account.Token,
                    UserCode = account.Login,
                    UserName = account.UserName
                };
                ITokenService _ITokenService = ServiceLocator.GetService<ITokenService>(GetCurrSession());

                bool newMdg = AppSettings.GetValue<bool>("AppSet:NewMdg");
                if (newMdg)
                    return account.Token;
                else
                {
                    var token = _ITokenService.CreateSession(userSeesion, "HZ.WMSCore");
                    return token;
                }
            }
            throw new PowerException(account.Data);
            return "0";
        }
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="opNo">单号</param>
        /// <param name="stockCode">仓库号</param>
        /// <param name="opName">业务类型</param>
        /// <param name="orderType">单据名</param>
        /// <returns></returns>
        public ApiResult StartFlow(string opNo, string stockCode, string opName, string orderType)
        {
            string stockLevel = stockCode == "X" ? "支队" : "大队";
            string endStr = "";
            if (orderType == "入库单")
                endStr = "入库";
            if (orderType == "出库单")
            {
                if (stockCode == "X")
                {
                    endStr = opName;
                }
                else
                    endStr = "出库";
            }
            if (orderType == "出库申请")
            {
                if (stockCode != "X")
                {
                    endStr = opName;
                }
                else
                    endStr = "申请";
            }

            //IUniAppService _IUniAppService = ServiceLocator.GetService<IUniAppService>(GetCurrSession());
            
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi; //AppSettings.GetValue<string>("SysInterface:Mdg");
            var data = new
            {
                opNo = opNo,
                routeInfo = stockLevel + endStr,
                opTitle = opName
            };
            UserSession user = GetSessionInfo();
            ApiResult res = new ApiResult();
            LogHelper.Info(JsonConvert.SerializeObject(data));
            string result = WebApiManager.HttpPost(mdg, "api/BillPublic/Submit", JsonConvert.SerializeObject(data),ref res, user.TokenId);
            if (!res.IsSuccess)
                return res;
            LogHelper.Info(result);

            res = JsonConvert.DeserializeObject<ApiResult>(result);
            if (res.IsSuccess)
            {
                try
                {
                    JObject resData = JsonConvert.DeserializeObject<JObject>(res.Data.ToString());
                    List<RoleUser> users = JsonConvert.DeserializeObject<List<RoleUser>>(resData.Value<object>("lstMessageUser").ToString());
                    users.ForEach(x =>
                    {
                        //_IUniAppService.PushData(x.userCode, orderType+"审核", $"{opNo}需要审核！", orderType+"审核");
                    });
                }
                catch (Exception ex)
                {
                    
                }
            }
            return res;
        }
        public ApiResult PassFlow(string context, string opType, string opNo, ref bool isLaseFlow)
        {
            //IUniAppService _IUniAppService = ServiceLocator.GetService<IUniAppService>(GetCurrSession());

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            var data = new
            {
                billNo = opNo
            };
            UserSession user = GetSessionInfo();
            ApiResult res = new ApiResult();
            LogHelper.Info("调用MDG流程接口:" + JsonConvert.SerializeObject(data));
            string result = WebApiManager.HttpPost(mdg, "api/BillPublic/Approved", JsonConvert.SerializeObject(data), ref res, user.TokenId);
            if (!res.IsSuccess)
                return res;
            LogHelper.Info("MDG流程接口返回:" + result);
            res = JsonConvert.DeserializeObject<ApiResult>(result);
            if (res.IsSuccess)
            {
                try
                {
                    JObject resData = JsonConvert.DeserializeObject<JObject>(res.Data.ToString());
                    isLaseFlow = resData.Value<bool>("isLaseFlow");
                    List<RoleUser> users = JsonConvert.DeserializeObject<List<RoleUser>>(resData.Value<object>("lstMessageUser").ToString());
                    users.ForEach(x =>
                    {
                        //_IUniAppService.PushData(x.userCode, $"{opType}审核", $"{opType}单{opNo}需要审核！", context);
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, ex);
                }
            }
            return res;
        }
        public ApiResult RejectFlow(string opNo, string remark,ref bool isFirstFlow)
        {
            //IUniAppService _IUniAppService = ServiceLocator.GetService<IUniAppService>(GetCurrSession());

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            var data = new
            {
                billNo = opNo,
                matterNote= remark
            };
            UserSession user = GetSessionInfo();

            ApiResult res = new ApiResult();
            string result = WebApiManager.HttpPost(mdg, "api/BillPublic/Reject", JsonConvert.SerializeObject(data),ref res, user.TokenId);
            if (!res.IsSuccess)
                return res;
            res = JsonConvert.DeserializeObject<ApiResult>(result);
            if (res.IsSuccess)
            {
                try
                {
                    JObject resData = JsonConvert.DeserializeObject<JObject>(res.Data.ToString());
                    isFirstFlow = resData.Value<bool>("isFirstFlow");
                    List<RoleUser> users = JsonConvert.DeserializeObject<List<RoleUser>>(resData.Value<object>("lstMessageUser").ToString());
                    users.ForEach(x =>
                    {
                        //_IUniAppService.PushData(x.userCode, "入库单审核", $"入库单{opNo}需要审核！", "入库单审核");
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, ex);
                }
            }
            return res;
        }

       
        public ApiResult GetTodoWaitItem()
        {
            //IUniAppService _IUniAppService = ServiceLocator.GetService<IUniAppService>(GetCurrSession());

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");

            UserSession user = GetSessionInfo();

            ApiResult res = new ApiResult();
            string result = WebApiManager.HttpGet(mdg, "api/FlowMatter/GetBillSummary", ref res, "billType=&billNo=billState=待办", user.TokenId);

            if (!res.IsSuccess)
                return res;
            LogHelper.Info("billType=&billNo=billState=待办" + result);
            res = JsonConvert.DeserializeObject<ApiResult>(result);
            if (res.IsSuccess)
            {
                List<JObject> data = JsonConvert.DeserializeObject<List<JObject>>(res.Data.ToString());
                return ApiResult.Success("", data);
            }
            else 
            {
                res.Message = "MDG:" + res.Message;
            }
            return res;
        }
        public ApiResult GetFlowItem(PageParm parm)
        {
            //IUniAppService _IUniAppService = ServiceLocator.GetService<IUniAppService>(GetCurrSession());

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");

            UserSession user = GetSessionInfo();

            ApiResult res = new ApiResult();
            string result = WebApiManager.HttpPost(mdg, "api/FlowMatter/GetPageList", JsonConvert.SerializeObject(parm), ref res, user.TokenId);

            if (!res.IsSuccess)
                return res;
            LogHelper.Info(JsonConvert.SerializeObject(parm)+":" + result);
            res = JsonConvert.DeserializeObject<ApiResult>(result);
            
            return res;
        }


        public ApiResult GetTryToken(string userCode, string oldToken)
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");

            ApiResult model = new ApiResult();
            string result = WebApiManager.HttpGet(mdg, $"api/User/GetTryToken?userCode={userCode}&oldToken="+ oldToken,ref model);

            if (!model.IsSuccess)
                return model;

            return JsonConvert.DeserializeObject<ApiResult>(result);
        }
    }
}
