using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService
{
    public interface ITokenService
    {
        //创建 Session
        string CreateSession(UserSession userInfo, string source);
        //更新Session
        void UpdateSession(string userSession, TimeSpan timeSpan);
        //刷新用户所有Session信息
        void RefreshSession(string userId);
        //清除指定 Session
        void RemoveSession(string userSession);
        //清除用户所有 Session
        void RemoveAllSession(string userId);
        //当前登录用户信息
        UserSession GetSessionInfo();
        //判断用户是否登录
        bool IsAuthenticated();
        //获取 Session 内容
        //T GetSessionItem<T>(string key);
        //获取 Session 内容
        T GetSessionItem<T>(string session, string key);
    }
}
