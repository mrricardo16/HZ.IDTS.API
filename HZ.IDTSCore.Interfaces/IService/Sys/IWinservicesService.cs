using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface IWinservicesService : IBaseService<tn_dts_winservices>
    {
        /// <summary>
        /// 保存接口
        /// </summary>
        /// <param name="saveBackups"></param>
        /// <returns></returns>
        public ReturnMessage Save(SaveWinservices saveBackups);

        /// <summary>
        /// 分页获取所有后台服务管理数据（不含查询）接口
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_winservices> GetListPages(PageParm parm);

       /// <summary>
       /// 操作服务
       /// </summary>
       /// <param name="operateServiceParameter"></param>
       /// <returns></returns>
        public ReturnMessage OperateService(OperateServiceParameter operateServiceParameter);

        /// <summary>
        /// 根据备注读取服务名称
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public string GetWinServiceName(string remarks);
    }
}
