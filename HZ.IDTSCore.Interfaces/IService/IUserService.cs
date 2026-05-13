using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.InterfaceParams;
using HZ.IDTSCore.Model.SerializeEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService
{
    public interface IUserService
    {
        AccountEntity Login(string username, string password);

      
        /// <summary>
        /// 添加用户
        /// 咸阳项目
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userName"></param>
        /// <param name="phoneId">手机id</param>
        /// <returns></returns>
        ApiResult AddUser(string userCode, string userName, string phoneId);

        /// <summary>
        /// 根据角色编号获取角色用户
        /// </summary>
        /// <param name="roleCode">角色编号</param>
        /// <returns></returns>
        public List<RoleUser> GetRoleUser(string roleCode);
        public ApiResult StartFlow(string opNo, string stockCode, string opName, string orderType);
        public ApiResult PassFlow(string context, string opType, string opNo, ref bool isLaseFlow);
        public ApiResult RejectFlow(string opNo, string remark, ref bool isFirstFlow);
        ApiResult GetFlowItem(PageParm parm);
        ApiResult GetTodoWaitItem();


        public ApiResult GetTryToken(string userCode, string oldToken);
    }
}
