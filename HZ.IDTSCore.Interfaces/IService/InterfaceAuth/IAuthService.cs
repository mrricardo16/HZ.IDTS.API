using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.InterfaceAuth
{
    public interface IAuthService
    {
        ApiResult SendRequest(tn_wms_interface_def model);

        string DoSendRequest(tn_wms_interface_def model, out string errMsg);
        ApiResult SendRequest(string intfCode,string jsonData,string user,string pwd);
    }
}
