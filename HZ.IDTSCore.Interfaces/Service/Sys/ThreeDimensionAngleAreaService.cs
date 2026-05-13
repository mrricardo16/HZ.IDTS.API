using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class ThreeDimensionAngleAreaService : BaseService<tn_dts_3danglearea>, IThreeDimensionAngleAreaService
    {
        public ThreeDimensionAngleAreaService(SessionInfo session) : base(session)
        {

        }

        #region 保存3D建模视角区域
        /// <summary>
        /// 保存3D建模视角区域
        /// </summary>
        /// <param name="threeDimensionAngleArea"></param>
        /// <returns></returns>
        public ReturnMessage SaveThreeDimensionAngleArea(SaveThreeDimensionAngleArea saveThreeDimensionAngleArea)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            string addOrModify = saveThreeDimensionAngleArea.AddOrModify;
            tn_dts_3danglearea threeDimensionAngleArea = saveThreeDimensionAngleArea.NewThreeDimensionAngleArea;
            if (saveThreeDimensionAngleArea.NewThreeDimensionAngleArea.cn_s_3danglearea_angleX < -360 || saveThreeDimensionAngleArea.NewThreeDimensionAngleArea.cn_s_3danglearea_angleX > 360)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "角度X的范围为-360~360度，请重试!";
                return returnMessage;
            }
            if (saveThreeDimensionAngleArea.NewThreeDimensionAngleArea.cn_s_3danglearea_angleY < -360 || saveThreeDimensionAngleArea.NewThreeDimensionAngleArea.cn_s_3danglearea_angleY > 360)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "角度Y的范围为-360~360度，请重试!";
                return returnMessage;
            }
            if (saveThreeDimensionAngleArea.NewThreeDimensionAngleArea.cn_s_3danglearea_angleZ < -360 || saveThreeDimensionAngleArea.NewThreeDimensionAngleArea.cn_s_3danglearea_angleZ > 360)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "角度Z的范围为-360~360度，请重试!";
                return returnMessage;
            }
            if (addOrModify == "add")
            {
                if (!(Db.Queryable<tn_dts_3danglearea>().Where(it => it.cn_s_3danglearea_code == threeDimensionAngleArea.cn_s_3danglearea_code).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "视角区域编号不能重复，请重试!";
                    return returnMessage;
                }
                tn_dts_3danglearea newThreeDimensionAnglearea = new tn_dts_3danglearea();
                newThreeDimensionAnglearea.cn_guid = Guid.NewGuid().ToString();
                newThreeDimensionAnglearea.cn_s_3danglearea_serial = threeDimensionAngleArea.cn_s_3danglearea_serial;
                newThreeDimensionAnglearea.cn_s_3danglearea_code = threeDimensionAngleArea.cn_s_3danglearea_code;
                newThreeDimensionAnglearea.cn_s_3danglearea_name = threeDimensionAngleArea.cn_s_3danglearea_name;
                newThreeDimensionAnglearea.cn_s_3danglearea_posX = threeDimensionAngleArea.cn_s_3danglearea_posX;
                newThreeDimensionAnglearea.cn_s_3danglearea_posY = threeDimensionAngleArea.cn_s_3danglearea_posY;
                newThreeDimensionAnglearea.cn_s_3danglearea_posZ = threeDimensionAngleArea.cn_s_3danglearea_posZ;
                newThreeDimensionAnglearea.cn_s_3danglearea_angleX = threeDimensionAngleArea.cn_s_3danglearea_angleX;
                newThreeDimensionAnglearea.cn_s_3danglearea_angleY = threeDimensionAngleArea.cn_s_3danglearea_angleY;
                newThreeDimensionAnglearea.cn_s_3danglearea_angleZ = threeDimensionAngleArea.cn_s_3danglearea_angleZ;
                newThreeDimensionAnglearea.cn_s_3danglearea_remarks = threeDimensionAngleArea.cn_s_3danglearea_remarks;
                newThreeDimensionAnglearea.cn_s_creator = user.UserCode;
                newThreeDimensionAnglearea.cn_s_creator_by = user.UserName;
                newThreeDimensionAnglearea.cn_t_create = DateTime.Now;
                tn_dts_logs cameraAreaLog = new tn_dts_logs();
                cameraAreaLog.cn_guid = Guid.NewGuid().ToString();
                cameraAreaLog.cn_s_logs_type = "操作";
                cameraAreaLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_3danglearea表中新增一条建模视角区域记录，详细信息为" + JsonConvert.SerializeObject(newThreeDimensionAnglearea);
                cameraAreaLog.cn_t_create = DateTime.Now;
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_3danglearea>(newThreeDimensionAnglearea).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(cameraAreaLog).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "17.1建模（3danglearea）视角区域管理保存接口新增3D建模视角区域失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败!";
                    return returnMessage;
                }
            }
            else if (addOrModify == "modify")
            {
                if (!(Db.Queryable<tn_dts_3danglearea>().Where(it => it.cn_s_3danglearea_code == threeDimensionAngleArea.cn_s_3danglearea_code && it.cn_guid != threeDimensionAngleArea.cn_guid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "视角区域编号不能重复，请重试!";
                    return returnMessage;
                }
                tn_dts_3danglearea threeDimensionAngleAreaGuid = Db.Queryable<tn_dts_3danglearea>().Where(it => it.cn_guid == threeDimensionAngleArea.cn_guid).First();
                threeDimensionAngleAreaGuid.cn_s_3danglearea_serial = threeDimensionAngleArea.cn_s_3danglearea_serial;
                string oldAngleAreaCode = threeDimensionAngleAreaGuid.cn_s_3danglearea_code;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_code = threeDimensionAngleArea.cn_s_3danglearea_code;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_name = threeDimensionAngleArea.cn_s_3danglearea_name;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_posX = threeDimensionAngleArea.cn_s_3danglearea_posX;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_posY = threeDimensionAngleArea.cn_s_3danglearea_posY;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_posZ = threeDimensionAngleArea.cn_s_3danglearea_posZ;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_angleX = threeDimensionAngleArea.cn_s_3danglearea_angleX;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_angleY = threeDimensionAngleArea.cn_s_3danglearea_angleY;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_angleZ = threeDimensionAngleArea.cn_s_3danglearea_angleZ;
                threeDimensionAngleAreaGuid.cn_s_3danglearea_remarks = threeDimensionAngleArea.cn_s_3danglearea_remarks;
                threeDimensionAngleAreaGuid.cn_s_modify = user.UserCode;
                threeDimensionAngleAreaGuid.cn_s_modify_by = user.UserName;
                threeDimensionAngleAreaGuid.cn_t_modify = DateTime.Now;
                tn_dts_logs cameraAreaLog = new tn_dts_logs();
                cameraAreaLog.cn_guid = Guid.NewGuid().ToString();
                cameraAreaLog.cn_s_logs_type = "操作";
                cameraAreaLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户修改了tn_dts_3danglearea表中视角区域编号为：" + oldAngleAreaCode + "，详细信息为" + JsonConvert.SerializeObject(threeDimensionAngleAreaGuid);
                cameraAreaLog.cn_t_create = DateTime.Now;
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_3danglearea>(threeDimensionAngleAreaGuid).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(cameraAreaLog).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "17.1建模（3danglearea）视角区域管理保存接口修改3D建模视角区域失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败!";
                    return returnMessage;
                }
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的add_or_modify参数只能为add或modify！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 分页查询3D建模视角区域
        /// <summary>
        /// 分页查询3D建模视角区域
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_3danglearea> GetListPages(PageParm parm)
        {
            string cn_s_3danglearea_name = parm.Parms["cn_s_3danglearea_name"].ObjToString();
            return Db.Queryable<tn_dts_3danglearea>().WhereIF(!string.IsNullOrEmpty(cn_s_3danglearea_name), (it => it.cn_s_3danglearea_name.Contains(cn_s_3danglearea_name)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion


        #region 删除3D建模视角区域
        /// <summary>
        /// 删除3D建模视角区域
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteThreeDimensionAngleArea(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_3danglearea> threeDimensionAngleAreaList = new List<tn_dts_3danglearea>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var guid in guidList)
            {
                tn_dts_3danglearea threeDimensionAngleAreaGuid = Db.Queryable<tn_dts_3danglearea>().Where(it => it.cn_guid == guid).First();
                if (threeDimensionAngleAreaGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的3D建模视角区域中有区域的唯一标识不存在！";
                    return returnMessage;
                }
                threeDimensionAngleAreaList.Add(threeDimensionAngleAreaGuid);
                tn_dts_logs angleAreaLog = new tn_dts_logs();
                angleAreaLog.cn_guid = Guid.NewGuid().ToString();
                angleAreaLog.cn_s_logs_type = "操作";
                angleAreaLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户删除了tn_dts_3danglearea表中视角区域编号为：" + threeDimensionAngleAreaGuid.cn_s_3danglearea_code + "的3D建模视角区域，详细信息为" + JsonConvert.SerializeObject(threeDimensionAngleAreaGuid);
                angleAreaLog.cn_t_create = DateTime.Now;
                logList.Add(angleAreaLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_3danglearea>(threeDimensionAngleAreaList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "17.3建模（3danglearea）视角区域管理批量删除接口批量删除3D建模视角区域失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion
    }
}
