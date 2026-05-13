using HZ.IDTSCore.Model.Entity;
using HZ.IDTSCore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HZ.IDTSCore.Interfaces
{
    public static class SystemCache
    {


        /// <summary>
        /// 对照码
        /// </summary>
        public static List<tn_wms_outside_code> outsideCodeList = new List<tn_wms_outside_code>();

        /// <summary>
        /// 对照配置
        /// </summary>
        public static List<tn_wms_outside_sys> outsideSysList = new List<tn_wms_outside_sys>();


        public static void Add_LightMapCache(tn_wms_location location, string opType, string opNo, string trayCode)
        {
            //if (location == null)
            //    return;

            //LightMapDto dto = LightMapCache.Find(e => e.location.cn_s_location_code.Equals(location.cn_s_location_code));

            //if (dto != null)
            //    LightMapCache.Remove(dto);
            //LightMapCache.Add(new LightMapDto(location, opType, opNo, trayCode));
        }

        /// <summary>
        /// 关闭货位灯，并返回货位灯所属巷道
        /// </summary>
        /// <param name="locationCode"></param>
        //public static void Del_LightMapCache(string locationCode)
        //{
        //    LightMapDto dto = LightMapCache.Find(e => e.location.cn_s_location_code == locationCode);
        //    if (dto == null)
        //        return;

        //    LightMapCache.Remove(dto);
        //}
    }
}
