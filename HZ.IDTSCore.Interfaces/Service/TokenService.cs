using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Common.Const;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.View;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace HZ.IDTSCore.Interfaces.Service
{
    public class TokenService : BaseService<object>, ITokenService
    {
        /// <summary>
        /// 缓存组件
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// HTTP上下文
        /// </summary>
        //private readonly IHttpContextAccessor _accessor;

        public TokenService(IMemoryCache memoryCache, SessionInfo session) :base(session)
        {
            _memoryCache = memoryCache;
        }

        #region 创建 Session
        /// <summary>
        /// 创建 Session
        /// </summary>
        public string CreateSession(UserSession userInfo, string source)
        {
            var userSession = Guid.NewGuid().ToString().ToUpper();
            //判断用户是否只允许等于一次
            if (userInfo.OneSession)
            {
                RemoveAllSession(userInfo.UserCode);
            }

            var expireTime = DateTime.Now.AddHours(SysConst.DefaultSessionExpire);
            var timeSpan = new TimeSpan(SysConst.DefaultSessionExpire, 0, 0);

            //将 Session 添加用户 Session 列表
            RedisServer.Session.HSet(userInfo.UserCode, userSession, expireTime);
            RedisServer.Session.Expire(userInfo.UserCode, timeSpan);

            //设置 Session 信息
            //var userSessionVM = new UserSession()
            //{
            //    UserCode = userInfo.UserCode,
            //    UserName = userInfo.UserName,
            //    DepartmentCode = "00",
            //    OrgCode = "00",
            //    TokenId = userInfo.TokenId
            //};

            RedisServer.Session.HSet(userSession, "UserInfo", userInfo);
            RedisServer.Session.Expire(userSession, timeSpan);

            //添加在线记录表

            return userSession;
        }
        #endregion

        #region 更新Session
        /// <summary>
        /// 更新Session
        /// </summary>
        /// <param name="userSession">用户Session</param>
        /// <param name="timeSpan">过期时间</param>
        public void UpdateSession(string userSession, TimeSpan timeSpan)
        {

            DateTime expireTime = DateTime.Now.Add(timeSpan);

            DateTime lastUpdateTime = _memoryCache.Get<DateTime>(userSession);

            if (lastUpdateTime == null || (Convert.ToDateTime(lastUpdateTime).AddMinutes(2) < DateTime.Now))
            {
                // 记录本次更新时间

                _memoryCache.Set(userSession, DateTime.Now);

                if (!string.IsNullOrEmpty(userSession))
                {
                    //更新在线用户记录最后操作时间

                    //根据 Session 取出 UserInfo
                    var userInfo = GetSessionItem<UserSession>(userSession, "UserInfo");

                    //更新 Session 列表中的 Session 过期时间
                    RedisServer.Session.HSet(userInfo.UserCode, userSession, expireTime);
                    //更新 Session 列表过期时间
                    RedisServer.Session.Expire(userInfo.UserCode, timeSpan);
                    //更新 Session 过期时间
                    RedisServer.Session.Expire(userSession, timeSpan);
                }
            }
        }
        #endregion

        #region 刷新用户所有Session信息
        /// <summary>
        /// 刷新用户所有Session信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void RefreshSession(string userId)
        {
            if (!RedisServer.Session.Exists(userId))
            {
                return;
            }

            //取出 Session 列表所有 Key
            var keys = RedisServer.Session.HKeys(userId);

            if (keys.Length <= 0)
            {
                return;
            }

            var userInfo = Db.Queryable<SYS_USER>().First(f => f.UserCode == userId);
            if (userInfo == null)
                throw new Exception("未找到用户：" + userId);

            foreach (var key in keys)
            {
                //根据 Session 取出 UserInfo
                var redisUserInfo = GetSessionItem<UserSession>(key, "UserInfo");

                //设置 Session 信息
                var userSessionVM = new UserSession()
                {
                    UserCode = userInfo.UserCode,
                };

                RedisServer.Session.HSet(key, "UserInfo", userSessionVM);
            }
        }
        #endregion

        #region 清除指定 Session
        /// <summary>
        /// 清除指定 Session
        /// </summary>
        public void RemoveSession(string userSession)
        {
            if (!string.IsNullOrEmpty(userSession))
            {
                //根据 Session 删除在线用户记录

                //根据 Session 取出 UserInfo
                var UserInfo = GetSessionItem<UserSession>(userSession, "UserInfo");

                //删除用户 Session 列表中的 Session
                RedisServer.Session.HDel(UserInfo.UserCode, userSession);

                //删除 Session 
                RedisServer.Session.Del(userSession);
            }
        }
        #endregion

        #region 清除用户所有 Session
        /// <summary>
        /// 清除用户所有 Session
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveAllSession(string userId)
        {
            if (RedisServer.Session.Exists(userId))
            {
                //取出 Session 列表所有 Key
                var keys = RedisServer.Session.HKeys(userId);

                foreach (var key in keys)
                {
                    //删除 Session 
                    RedisServer.Session.Del(key);

                    //删除用户 Session 列表中的 Session
                    RedisServer.Session.HDel(userId, key);
                }
            }

            //删除在线记录
        }
        #endregion

        /// <summary>
        /// 判断用户是否登录
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            return true;
            if (!string.IsNullOrEmpty(GetCurrSession().token))
            {
                if (!RedisServer.Session.Exists(GetCurrSession().token))
                {
                    //根据 Session 删除在线用户记录
                }
                else
                {
                    var expireTime = new TimeSpan(SysConst.DefaultSessionExpire, 0, 0);
                    UpdateSession(GetCurrSession().token, expireTime);
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 获取 Session 内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetSessionItem<T>(string session, string key)
        {
            if (!RedisServer.Session.Exists(session))
            {
                throw new Exception(string.Format("GetSessionItem : {0} has Exception", key));
            }

            return RedisServer.Session.HGet<T>(session, key);
        }

    }
}
