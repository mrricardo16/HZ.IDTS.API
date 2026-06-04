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

        

        public BaseService(SessionInfo session):base(session)
        {

        }

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

        #region 添加操作V2
        /// <summary>
        /// 批量添加数据V2。
        /// 时间：2026-05-29
        /// 优化内容：大批量插入时按batchSize分批提交，避免单次SQL过大、锁时间过长或连接长时间占用。
        /// 注意：如果传入tranDb，则所有分批仍在调用方事务中执行；如果不传，SqlSugar按IsAutoCloseConnection自动释放连接。
        /// </summary>
        /// <param name="parm">待添加数据</param>
        /// <param name="batchSize">每批数量，默认500</param>
        /// <param name="tranDb">事务数据库对象</param>
        /// <returns>影响行数</returns>
        public int AddBatchV2(List<T> parm, int batchSize = 500, SqlSugarClient tranDb = null)
        {
            if (parm == null || parm.Count == 0)
            {
                return 0;
            }

            int resultV2 = 0;
            int pageSizeV2 = NormalizeBatchSizeV2(batchSize);
            SqlSugarClient dbV2 = tranDb ?? Db;
            for (int i = 0; i < parm.Count; i += pageSizeV2)
            {
                List<T> batchListV2 = parm.Skip(i).Take(pageSizeV2).ToList();
                resultV2 += dbV2.Insertable(batchListV2).ExecuteCommand();
            }
            return resultV2;
        }
        #endregion

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

        #region 查询操作V2
        /// <summary>
        /// 查询指定数量数据V2。
        /// 时间：2026-05-29
        /// 优化内容：替代GetAll无上限查询，默认最多返回1000条，避免大表一次性ToList造成内存和数据库IO压力。
        /// </summary>
        /// <param name="take">返回数量上限，默认1000</param>
        /// <param name="order">排序表达式</param>
        /// <param name="orderEnum">Asc/Desc</param>
        /// <returns>数据列表</returns>
        public List<T> GetAllV2(int take = 1000, Expression<Func<T, object>> order = null, string orderEnum = "Asc")
        {
            var queryV2 = ApplyExpressionOrderV2(Db.Queryable<T>(), order, orderEnum);
            return queryV2.Take(NormalizeTakeV2(take)).ToList();
        }
        #endregion

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

        #region 分页查询V2
        /// <summary>
        /// 根据条件查询分页数据V2。
        /// 时间：2026-05-29
        /// 优化内容：对分页参数做兜底限制，并对OrderBy做字段白名单校验，避免前端传入非法排序字段导致慢SQL或SQL注入风险。
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="parm">分页参数</param>
        /// <param name="allowOrderFields">允许排序字段；为空时默认使用实体属性名作为白名单</param>
        /// <returns>分页数据</returns>
        public PagedInfo<T> GetPagesV2(Expression<Func<T, bool>> where, PageParm parm, List<string> allowOrderFields = null)
        {
            PageParm pageParmV2 = NormalizePageParmV2(parm);
            var sourceV2 = Db.Queryable<T>().Where(where);
            sourceV2 = ApplyStringOrderV2(sourceV2, pageParmV2.OrderBy, pageParmV2.Sort, allowOrderFields);
            return sourceV2.ToPage(pageParmV2.PageIndex, pageParmV2.PageSize);
        }
        #endregion

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

        #region 条件查询V2
        /// <summary>
        /// 根据条件查询指定数量数据V2。
        /// 时间：2026-05-29
        /// 优化内容：替代GetWhere直接ToList全量返回，默认限制1000条，适合高并发接口或大表查询。
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="take">返回数量上限，默认1000</param>
        /// <param name="order">排序表达式</param>
        /// <param name="orderEnum">Asc/Desc</param>
        /// <returns>数据列表</returns>
        public List<T> GetWhereV2(Expression<Func<T, bool>> where, int take = 1000, Expression<Func<T, object>> order = null, string orderEnum = "Asc")
        {
            var queryV2 = ApplyExpressionOrderV2(Db.Queryable<T>().Where(where), order, orderEnum);
            return queryV2.Take(NormalizeTakeV2(take)).ToList();
        }
        #endregion

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

        #region 修改操作V2
        /// <summary>
        /// 批量修改V2。
        /// 时间：2026-05-29
        /// 优化内容：大批量更新时按batchSize拆分执行，降低单次SQL大小和长事务锁表风险。
        /// 注意：如果传入tranDb，则所有分批仍在调用方事务中执行。
        /// </summary>
        /// <param name="parm">待修改数据</param>
        /// <param name="batchSize">每批数量，默认500</param>
        /// <param name="tranDb">事务数据库对象</param>
        /// <returns>影响行数</returns>
        public int UpdateBatchV2(List<T> parm, int batchSize = 500, SqlSugarClient tranDb = null)
        {
            if (parm == null || parm.Count == 0)
            {
                return 0;
            }

            int resultV2 = 0;
            int pageSizeV2 = NormalizeBatchSizeV2(batchSize);
            SqlSugarClient dbV2 = tranDb ?? Db;
            for (int i = 0; i < parm.Count; i += pageSizeV2)
            {
                List<T> batchListV2 = parm.Skip(i).Take(pageSizeV2).ToList();
                resultV2 += dbV2.Updateable(batchListV2).ExecuteCommand();
            }
            return resultV2;
        }
        #endregion

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

        #region 删除操作V2
        /// <summary>
        /// 批量删除V2。
        /// 时间：2026-05-29
        /// 优化内容：大量主键删除时按batchSize拆分，避免单次IN条件过长，降低SQL解析和锁等待压力。
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="batchSize">每批数量，默认500</param>
        /// <param name="tranDb">事务数据库对象</param>
        /// <returns>影响行数</returns>
        public int DeleteBatchV2(object[] ids, int batchSize = 500, SqlSugarClient tranDb = null)
        {
            if (ids == null || ids.Length == 0)
            {
                return 0;
            }

            int resultV2 = 0;
            int pageSizeV2 = NormalizeBatchSizeV2(batchSize);
            SqlSugarClient dbV2 = tranDb ?? Db;
            for (int i = 0; i < ids.Length; i += pageSizeV2)
            {
                object[] batchIdsV2 = ids.Skip(i).Take(pageSizeV2).ToArray();
                resultV2 += dbV2.Deleteable<T>().In(batchIdsV2).ExecuteCommand();
            }
            return resultV2;
        }
        #endregion

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

        #region 数据库访问辅助方法V2
        /// <summary>
        /// 规范批量操作每批数量V2。
        /// </summary>
        private int NormalizeBatchSizeV2(int batchSize)
        {
            if (batchSize <= 0)
            {
                return 500;
            }
            if (batchSize > 5000)
            {
                return 5000;
            }
            return batchSize;
        }

        /// <summary>
        /// 规范查询返回数量V2。
        /// </summary>
        private int NormalizeTakeV2(int take)
        {
            if (take <= 0)
            {
                return 1000;
            }
            if (take > 10000)
            {
                return 10000;
            }
            return take;
        }

        /// <summary>
        /// 规范分页参数V2。
        /// </summary>
        private PageParm NormalizePageParmV2(PageParm parm)
        {
            PageParm pageParmV2 = parm ?? new PageParm();
            if (pageParmV2.PageIndex <= 0)
            {
                pageParmV2.PageIndex = 1;
            }
            if (pageParmV2.PageSize <= 0)
            {
                pageParmV2.PageSize = 20;
            }
            if (pageParmV2.PageSize > 1000)
            {
                pageParmV2.PageSize = 1000;
            }
            return pageParmV2;
        }

        /// <summary>
        /// 表达式排序V2。
        /// </summary>
        private ISugarQueryable<T> ApplyExpressionOrderV2(ISugarQueryable<T> query, Expression<Func<T, object>> order, string orderEnum)
        {
            if (order == null)
            {
                return query;
            }
            return query.OrderByIF(orderEnum == "Asc", order, OrderByType.Asc).OrderByIF(orderEnum == "Desc", order, OrderByType.Desc);
        }

        /// <summary>
        /// 字符串排序V2。
        /// </summary>
        private ISugarQueryable<T> ApplyStringOrderV2(ISugarQueryable<T> query, string orderBy, string sort, List<string> allowOrderFields)
        {
            if (!IsSafeOrderByV2(orderBy, allowOrderFields))
            {
                return query;
            }

            string orderTypeV2 = sort == "descending" ? "desc" : "asc";
            return query.OrderBy($"{orderBy} {orderTypeV2}");
        }

        /// <summary>
        /// 校验排序字段V2。
        /// </summary>
        private bool IsSafeOrderByV2(string orderBy, List<string> allowOrderFields)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return false;
            }

            List<string> fieldListV2 = allowOrderFields;
            if (fieldListV2 == null || fieldListV2.Count == 0)
            {
                fieldListV2 = typeof(T).GetProperties().Select(it => it.Name).ToList();
            }

            return fieldListV2.Any(it => string.Equals(it, orderBy, StringComparison.OrdinalIgnoreCase));
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
