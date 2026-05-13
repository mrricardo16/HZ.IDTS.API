using HZ.IDTSCore.Model.Entity.Sys;
using HZ.IDTSCore.Interfaces.IService.Sys;
using System;
using System.Collections.Generic;
using System.Text;
using HZ.DbHelper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.CommonUtil.Model;
using System.ServiceProcess;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class WinservicesService : BaseService<tn_dts_winservices>, IWinservicesService
    {
        public WinservicesService(SessionInfo session) : base(session)
        {

        }

        #region 重启服务
        /// <summary>
        /// 重启服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public ReturnMessage RestartService(string serviceName)
        {
            Process process = new Process();
            ReturnMessage returnMessage = new ReturnMessage();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c sc stop {serviceName} && sc start {serviceName}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"sudo systemctl restart {serviceName}\"";
            }
            else
            {
                throw new Exception("Unsupported operating system");
            }
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                //重启成功
                returnMessage.IsSuccess = true;
                returnMessage.Message = "重启成功！";
                return returnMessage;
            }
            else
            {
                // 重启失败
                returnMessage.IsSuccess = false;
                returnMessage.Message = "重启失败，" + error;
                return returnMessage;
            }
        }
        #endregion

        #region 保存接口
        /// <summary>
        /// 保存接口
        /// </summary>
        /// <param name="saveBackups"></param>
        /// <returns></returns>
        public ReturnMessage Save(SaveWinservices saveBackups)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_winservices winservices = Db.Queryable<tn_dts_winservices>().Where(it => it.cn_guid == saveBackups.guid).First();
            if (winservices is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "传入的唯一标识：" + saveBackups.guid + "不存在！";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_winservices>().Where(it => it.cn_s_winservices_name == saveBackups.name && it.cn_guid != saveBackups.guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "服务名称不能重复！";
                return returnMessage;
            }
            winservices.cn_s_winservices_name = saveBackups.name;
            winservices.cn_s_winservices_describe = saveBackups.describe;
            winservices.cn_s_modify = user.UserCode;
            winservices.cn_s_modify_by = user.UserName;
            winservices.cn_t_modify = DateTime.Now;
            int resWinservices = Db.Updateable(winservices).Where(it => it.cn_guid == saveBackups.guid).ExecuteCommand();
            if (resWinservices <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "更新tn_dts_winservices表失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 分页获取所有后台服务管理数据（不含查询）接口
        /// <summary>
        /// 分页获取所有后台服务管理数据（不含查询）接口
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_winservices> GetListPages(PageParm parm)
        {
            try
            {
                PagedInfo<tn_dts_winservices> winservicePagedList = Db.Queryable<tn_dts_winservices>()
               .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
               .ToPage(parm.PageIndex, parm.PageSize);
                //PagedInfo<tn_dts_winservices> returnPagedList = new PagedInfo<tn_dts_winservices>();
                List<tn_dts_winservices> winserviceList = winservicePagedList.DataSource;
                List<tn_dts_winservices> returnList = new List<tn_dts_winservices>();
                ServiceController[] serviceController = ServiceController.GetServices();
                foreach (var winservice in winserviceList)
                {
                    if (serviceController.Where(it => it.ServiceName == winservice.cn_s_winservices_name).ToList().Count == 0)
                    {
                        winservice.cn_s_winservices_status = "windows服务中找不到该服务名称服务!";
                        winservice.cn_s_winservices_fullpath = "";
                    }
                    else
                    {
                        using (ServiceController sc = new ServiceController(winservice.cn_s_winservices_name))
                        {
                            winservice.cn_s_winservices_status = GetStatus(sc.Status);
                            winservice.cn_s_winservices_fullpath = GetServicePath(winservice.cn_s_winservices_name);
                            winservice.cn_t_modify = DateTime.Now;
                            int res = Db.Updateable(winservice).ExecuteCommand();
                        }
                    }
                    returnList.Add(winservice);
                }
                winservicePagedList.DataSource = returnList;
                return winservicePagedList;
            }
            catch { return new PagedInfo<tn_dts_winservices>(); };
        }
        #endregion

        #region 操作服务
        /// <summary>
        /// 操作服务
        /// </summary>
        /// <param name="operateServiceParameter"></param>
        /// <returns></returns>
        public ReturnMessage OperateService(OperateServiceParameter operateServiceParameter)
        {
            //Process process = new Process();
            ReturnMessage returnMessage = new ReturnMessage();
            ServiceController[] serviceController = ServiceController.GetServices();
            string operation = operateServiceParameter.operation;
            string serviceName = operateServiceParameter.serviceName;
            if (serviceController.Where(it => it.ServiceName == serviceName).ToList().Count == 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "Windows系统服务中找不到名称为" + serviceName + "的服务！";
                return returnMessage;
            }
            if (operation == "start")
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "服务状态不为Stopped/未运行，无法启动！";
                        return returnMessage;
                    }
                    tn_dts_winservices winservices = Db.Queryable<tn_dts_winservices>().Where(it => it.cn_s_winservices_name == serviceName).First();
                    if (winservices is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "tn_dts_winservices中找不到服务名称为" + serviceName + "的服务";
                        return returnMessage;
                    }
                    if (winservices.cn_s_winservices_remarks == "IDTS_API_Service")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "IDTS自身服务不支持启动！";
                        return returnMessage;
                    }
                    try
                    {
                        sc.Start();
                        winservices.cn_s_winservices_status = GetStatus(sc.Status);
                        winservices.cn_t_modify = DateTime.Now;
                        int res = Db.Updateable(winservices).ExecuteCommand();
                        if (res <= 0)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "更新tn_dts_winservices表失败！";
                            return returnMessage;
                        }
                        returnMessage.IsSuccess = true;
                        returnMessage.Message = "启动成功";
                    }
                    catch (Exception ex)
                    {
                        winservices.cn_s_winservices_status = GetStatus(sc.Status);
                        //winservices.cn_s_winservices_remarks = ex.Message;
                        winservices.cn_t_modify = DateTime.Now;
                        int res = Db.Updateable(winservices).ExecuteCommand();
                        if (res <= 0)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "更新tn_dts_winservices表失败！";
                            return returnMessage;
                        }
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "启动失败";
                    }
                }
            }
            else if (operation == "stop")
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "服务状态不为Running/正在运行，无法关闭！";
                        return returnMessage;
                    }
                    tn_dts_winservices winservices = Db.Queryable<tn_dts_winservices>().Where(it => it.cn_s_winservices_name == serviceName).First();
                    if (winservices is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "tn_dts_winservices中找不到服务名称为" + serviceName + "的服务";
                        return returnMessage;
                    }
                    if (winservices.cn_s_winservices_remarks == "IDTS_API_Service")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "IDTS自身服务不支持启动！";
                        return returnMessage;
                    }
                    try
                    {
                        sc.Stop();
                        winservices.cn_s_winservices_status = GetStatus(sc.Status);
                        winservices.cn_t_modify = DateTime.Now;
                        int res = Db.Updateable(winservices).ExecuteCommand();
                        if (res <= 0)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "更新tn_dts_winservices表失败！";
                            return returnMessage;
                        }
                        returnMessage.IsSuccess = true;
                        returnMessage.Message = "关闭成功";
                    }
                    catch (Exception ex)
                    {
                        winservices.cn_s_winservices_status = GetStatus(sc.Status);
                        //winservices.cn_s_winservices_remarks = ex.Message;
                        winservices.cn_t_modify = DateTime.Now;
                        int res = Db.Updateable(winservices).ExecuteCommand();
                        if (res <= 0)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "更新tn_dts_winservices表失败！";
                            return returnMessage;
                        }
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "关闭失败";
                    }
                }
            }
            else if (operation == "restart")
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "服务状态不为Running/正在运行，无法重启！";
                        return returnMessage;
                    }
                    tn_dts_winservices winservices = Db.Queryable<tn_dts_winservices>().Where(it => it.cn_s_winservices_name == serviceName).First();
                    if (winservices is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "tn_dts_winservices中找不到服务名称为" + serviceName + "的服务";
                        return returnMessage;
                    }
                    if(winservices.cn_s_winservices_remarks == "IDTS_API_Service")
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.Arguments = $"/c net stop {serviceName} && net start {serviceName}";
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();
                        winservices.cn_s_winservices_status = GetStatus(sc.Status);
                        winservices.cn_t_modify = DateTime.Now;
                        int res = Db.Updateable(winservices).ExecuteCommand();
                        if (res <= 0)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "更新tn_dts_winservices表失败！";
                            return returnMessage;
                        }
                        returnMessage.IsSuccess = true;
                        returnMessage.Message = "重启成功";
                    }
                    else
                    {
                        try
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped);
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            winservices.cn_s_winservices_status = GetStatus(sc.Status);
                            winservices.cn_t_modify = DateTime.Now;
                            int res = Db.Updateable(winservices).ExecuteCommand();
                            if (res <= 0)
                            {
                                returnMessage.IsSuccess = false;
                                returnMessage.Message = "更新tn_dts_winservices表失败！";
                                return returnMessage;
                            }
                            returnMessage.IsSuccess = true;
                            returnMessage.Message = "重启成功";

                        }
                        catch (Exception ex)
                        {
                            winservices.cn_s_winservices_status = GetStatus(sc.Status);
                            //winservices.cn_s_winservices_remarks = ex.Message;
                            winservices.cn_t_modify = DateTime.Now;
                            int res = Db.Updateable(winservices).ExecuteCommand();
                            if (res <= 0)
                            {
                                returnMessage.IsSuccess = false;
                                returnMessage.Message = "更新tn_dts_winservices表失败！";
                                return returnMessage;
                            }
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "重启失败";
                        }
                    }                 
                }
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入接口的operation参数不在“start”、“stop”和“restart”之中！";
                return returnMessage;
            }


            return returnMessage;
        }
        #endregion

        #region 获取服务状态
        /// <summary>
        /// 获取服务状态
        /// </summary>
        /// <param name="scs"></param>
        /// <returns></returns>
        public string GetStatus(ServiceControllerStatus scs)
        {
            switch (scs)
            {
                case ServiceControllerStatus.Stopped:
                    return "未运行";
                case ServiceControllerStatus.StartPending:
                    return "正在启动";
                case ServiceControllerStatus.StopPending:
                    return "正在停止";
                case ServiceControllerStatus.Running:
                    return "正在运行";
                case ServiceControllerStatus.ContinuePending:
                    return "即将继续";
                case ServiceControllerStatus.PausePending:
                    return "即将暂停";
                case ServiceControllerStatus.Paused:
                    return "已暂停";
                default:
                    return "";
            }
        }
        #endregion

        #region 读取服务启动文件路径
        /// <summary>
        /// 读取服务启动文件路径
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public string GetServicePath(string serviceName)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serviceName}"))
            {
                string path = key?.GetValue("ImagePath").ToString();
                return path;
            }
        }
        #endregion

        #region 根据备注读取服务名称
        /// <summary>
        /// 根据备注读取服务名称
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public string GetWinServiceName(string remarks)
        {
            return Db.Queryable<tn_dts_winservices>().Where(it => it.cn_s_winservices_remarks == remarks).Select(it => it.cn_s_winservices_name).First();
        }
        #endregion

    }
}
