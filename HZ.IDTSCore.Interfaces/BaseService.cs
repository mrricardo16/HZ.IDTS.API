using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using SqlSugar;
using Newtonsoft.Json;
using HZ.Redis.RedisUserModel;
using System.Net;
using static HZ.IDTSCore.Common.Const.SysKeyword;
using HZ.DbHelper;
using HZ.CommonUtil.Model;
using HZ.CommonUtil.ExceptionExtend;
using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Common.Helpers;

namespace HZ.IDTSCore.Interfaces
{
    /// <summary>
    /// 基础服务定义
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T> : DbContext, ISplitTableService, IBaseService<T> where T : class, new()
    {

        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        /// <returns></returns>
        public UserSession GetSessionInfo()
        {
            try
            {
                string token = GetCurrSession().token;// GetCurrSession();// this.HttpContext.Request.Headers["token"];
                if (token == "sys")
                {
                    return new UserSession()
                    {
                        //AuthorizeStock = user.userInfo.stockCode == null ? new List<string>() : new List<string>(user.userInfo.stockCode.Split(',')),
                        //OrgCode = user.userInfo.orgCode,
                        //OrgFlag = user.userInfo.orgFlag,
                        //OrgName = user.userInfo.orgName,
                        TokenId = token,
                        UserCode = "sys",
                        UserName = "sys",
                    };
                }
                RedisMsgEntity user = null;
                user = HZ.Redis.RedisUserinfoHelper.GetUserInfo(token);

                if (user == null || !user.Success)
                    throw new PowerException("请重新登录！");
                return new UserSession()
                {
                    AuthorizeStock = user.userInfo.stockCode == null ? new List<string>() : new List<string>(user.userInfo.stockCode.Split(',')),
                    OrgCode = user.userInfo.orgCode,
                    OrgFlag = user.userInfo.orgFlag,
                    OrgName = user.userInfo.orgName,
                    TokenId = user.userInfo.token,
                    UserCode = user.userInfo.userCode,
                    UserName = user.userInfo.userName,
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //public UserSession GetSessionInfoWithLog()
        //{
        //    TLog.Info("GetSessionInfo");
        //    string token = GetCurrSession();// this.HttpContext.Request.Headers["token"];
        //    TLog.Info("token:" + token);
        //    if (token == "sys")
        //    {
        //        return new UserSession()
        //        {
        //            //AuthorizeStock = user.userInfo.stockCode == null ? new List<string>() : new List<string>(user.userInfo.stockCode.Split(',')),
        //            //OrgCode = user.userInfo.orgCode,
        //            //OrgFlag = user.userInfo.orgFlag,
        //            //OrgName = user.userInfo.orgName,
        //            TokenId = token,
        //            UserCode = "sys",
        //            UserName = "sys",
        //        };
        //    }
        //    RedisMsgEntity user = null;
        //    user = HZ.Redis.RedisUserinfoHelper.GetUserInfo(token);
        //    TLog.Info("user:" + JsonConvert.SerializeObject(user));

        //    if (user == null || !user.Success)
        //        throw new PowerException("请重新登录！");
        //    return new UserSession()
        //    {
        //        AuthorizeStock = user.userInfo.stockCode == null ? new List<string>() : new List<string>(user.userInfo.stockCode.Split(',')),
        //        OrgCode = user.userInfo.orgCode,
        //        OrgFlag = user.userInfo.orgFlag,
        //        OrgName = user.userInfo.orgName,
        //        TokenId = user.userInfo.token,
        //        UserCode = user.userInfo.userCode,
        //        UserName = user.userInfo.userName,
        //    };
        //}

        public BaseService(SessionInfo session):base(session)
        {
            //this.SetUs(token,org);
        }

        //public SessionInfo GetCurrSession()
        //{
        //    return this.GetCurrSession()
        //}

        public string GetRuleNo(string ruleName, string otherSuffix = "")
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");

            SessionInfo session = GetCurrSession();
            //UserSession user = GetSessionInfo();

            ApiResult apiRes = new ApiResult();
            string str = WebApiManager.HttpGet(mdg+"/api/BasicWork/GenBillNo?ruleName=" + ruleName + "&otherSuffix=" + otherSuffix, ref apiRes, session.token, session.splitDbCode);
            if (!apiRes.IsSuccess)
                throw new WebException(apiRes.Message);

            apiRes = JsonConvert.DeserializeObject<ApiResult>(str);
            if (apiRes.IsSuccess)
            {
                string code = apiRes.Data.ToString();
                if(code.Length==0)
                    throw new Exception("未获取到[" + ruleName + "]号！");
                return code;
            }
            else
                throw new Exception("未获取到[" + ruleName + "]号！");
        }

        public List<string> GetRuleNos(string ruleName,int count, string otherSuffix = "")
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            UserSession user = GetSessionInfo();

            ApiResult apiRes = new ApiResult();
            string str = WebApiManager.HttpGet(mdg+"/api/BasicWork/GenBillNoList?ruleName=" + ruleName + "&orgFlag=&otherSuffix=" + otherSuffix+ "&codeNum="+ count, ref apiRes, user.TokenId, user.OrgFlag);
            if (!apiRes.IsSuccess)
                throw new WebException(apiRes.Message);

            apiRes = JsonConvert.DeserializeObject<ApiResult>(str);
            if (apiRes.IsSuccess)
            {
                var codes = JsonConvert.DeserializeObject<List<string>>(apiRes.Data.ToString());
                if (codes.Count!= count)
                    throw new Exception("未获取到[" + ruleName + "]号！");
                return codes;
            }
            else
                throw new Exception("未获取到[" + ruleName + "]号！");
        }

        public ApiResult GetUserByRole(string roleCode)
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            UserSession user = GetSessionInfo();

            ApiResult apiRes = new ApiResult();
            string str = WebApiManager.HttpGet(mdg+"/api/EmployeeWork/GetUserByRole?roleCode=" + roleCode, ref apiRes, user.TokenId, user.OrgFlag);
            if (!apiRes.IsSuccess)
                return apiRes;
            apiRes = JsonConvert.DeserializeObject<ApiResult>(str);
            return apiRes;
        }

        /// <summary>
        /// 查询任务的状态
        /// </summary>
        /// <param name="type">0：全部、1：执行中、2：完成、3：取消、</param>
        /// <returns></returns>
        public List<string> GetTaskState(int type)
        {
            List<string> taskStateList = new List<string>();
            switch (type)
            {
                case 0:
                case 1:
                    taskStateList.AddRange(new List<string> {
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.UnSend),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.UnExecute),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.DoExecute),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.PickStart),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.PickComplete),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.UnloadingStart),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.UnloadingComplete),
                    });
                    break;
                case 2:
                    taskStateList.AddRange(new List<string> {
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.Complete),
                    });
                    break;
                case 3:
                    taskStateList.AddRange(new List<string> {
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.Cancel),
                        EnumHelper.GetEnumDescription(ExecuteStateEnum.Error),
                    });
                    break;
            }

            return taskStateList;
        }

        /// <summary>
        /// 判断托盘号是否是虚拟托盘
        /// </summary>
        /// <param name="trayCode"></param>
        /// <returns></returns>
        public bool IsVirtualTray(string trayCode)
        {
            return trayCode.StartsWith("VT");
        }

        #region 事务

        /// <summary>
        /// 启用事务
        /// </summary>
        public void BeginTran()
        {
            Db.Ado.BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            Db.Ado.CommitTran();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            Db.Ado.RollbackTran();
        }

        #endregion

        #region 添加操作
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public int Add(T parm,SqlSugarClient tranDb=null)
        {
            if(tranDb==null)
                return Db.Insertable(parm).ExecuteCommand();
            return tranDb.Insertable(parm).ExecuteCommand();
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public int Add(List<T> parm, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Insertable(parm).ExecuteCommand();
            return tranDb.Insertable(parm).ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public T Saveable(T parm, Expression<Func<T, object>> uClumns = null, Expression<Func<T, object>> iColumns = null)
        {
            var command = Db.Saveable(parm);

            if (uClumns != null)
            {
                command = command.UpdateIgnoreColumns(uClumns);
            }

            if (iColumns != null)
            {
                command = command.InsertIgnoreColumns(iColumns);
            }

            return command.ExecuteReturnEntity();
        }

        /// <summary>
        /// 批量添加或更新数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public List<T> Saveable(List<T> parm, Expression<Func<T, object>> uClumns = null, Expression<Func<T, object>> iColumns = null)
        {
            var command = Db.Saveable(parm);

            if (uClumns != null)
            {
                command = command.UpdateIgnoreColumns(uClumns);
            }

            if (iColumns != null)
            {
                command = command.InsertIgnoreColumns(iColumns);
            }

            return command.ExecuteReturnList();
        }
        #endregion

        #region 查询操作

        /// <summary>
        /// 根据条件查询数据是否存在
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> where)
        {
            return Db.Queryable<T>().Any(where);
        }

        /// <summary>
        /// 根据主值查询单条数据
        /// </summary>
        /// <param name="pkValue">主键值</param>
        /// <returns>泛型实体</returns>
        public T GetId(object pkValue)
        {
            return Db.Queryable<T>().InSingle(pkValue);
        }

        /// <summary>
        /// 根据主键查询多条数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<T> GetIn(object[] ids)
        {
            return Db.Queryable<T>().In(ids).ToList();
        }

        /// <summary>
        /// 根据条件取条数
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> where)
        {
            return Db.Queryable<T>().Count(where);

        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用)
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            return Db.Queryable<T>().ToList();
        }

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public T GetFirst(Expression<Func<T, bool>> where)
        {
            return Db.Queryable<T>().Where(where).First();
        }

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public T GetFirst(string parm)
        {
            return Db.Queryable<T>().Where(parm).First();
        }


        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<T> GetPages(Expression<Func<T, bool>> where, PageParm parm)
        {
            var source = Db.Queryable<T>().Where(where).OrderByIF(!string.IsNullOrEmpty(parm.Sort), $"{parm.OrderBy} {(parm.Sort == "descending" ? "desc" : "asc")}");

            return source.ToPage(parm.PageIndex, parm.PageSize);
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parm"></param>
        /// <param name="totalField"></param>
        /// <returns></returns>
        public PagedInfo<T> GetPages(Expression<Func<T, bool>> where, PageParm parm, List<ITotalField> totalField)
        {
            var source = Db.Queryable<T>().Where(where).OrderByIF(!string.IsNullOrEmpty(parm.Sort), $"{parm.OrderBy} {(parm.Sort == "descending" ? "desc" : "asc")}");

            return source.ToPage(totalField, parm.PageIndex, parm.PageSize);
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        public List<T> GetWhere(Expression<Func<T, bool>> where)
        {
            var query = Db.Queryable<T>().Where(where);
            return query.ToList();
        }

        /// <summary>
		/// 根据条件查询数据
		/// </summary>
		/// <param name="where">条件表达式树</param>
		/// <returns></returns>
        public List<T> GetWhere(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, string orderEnum = "Asc")
        {
            if(order!=null)
                return Db.Queryable<T>().Where(where).OrderByIF(orderEnum == "Asc", order, SqlSugar.OrderByType.Asc).OrderByIF(orderEnum == "Desc", order, SqlSugar.OrderByType.Desc).ToList();
            else
                 return Db.Queryable<T>().Where(where).ToList();
        }

        #endregion

        #region 修改操作

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public int Update(T parm, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Updateable(parm).ExecuteCommand();
            return tranDb.Updateable(parm).ExecuteCommand();
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public int Update(List<T> parm, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Updateable(parm).ExecuteCommand();
            return tranDb.Updateable(parm).ExecuteCommand();
        }

        /// <summary>
        /// 按查询条件更新
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommand();
            return tranDb.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommand();
        }

        #endregion

        #region 删除操作

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public int Delete(object id, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Deleteable<T>(id).ExecuteCommand();
            return tranDb.Deleteable<T>(id).ExecuteCommand();
        }

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public int Delete(object[] ids, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Deleteable<T>().In(ids).ExecuteCommand();
            return tranDb.Deleteable<T>().In(ids).ExecuteCommand();
        }

        /// <summary>
        /// 根据条件删除一条或多条数据
        /// </summary>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
        public int Delete(Expression<Func<T, bool>> where, SqlSugarClient tranDb = null)
        {
            if (tranDb == null)
                return Db.Deleteable<T>().Where(where).ExecuteCommand();
            return tranDb.Deleteable<T>().Where(where).ExecuteCommand();
        }
        #endregion


        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            throw new NotImplementedException();
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo)
        {
            return entityInfo.DbTableName + "_First";
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType type)
        {
            return entityInfo.DbTableName + "_First";
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            return entityInfo.DbTableName + "_First";
        }

        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            throw new NotImplementedException();
        }
    }
}
