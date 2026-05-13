using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    /// <summary>
    /// 系统级全局全量，主要是将系统参数设置所有数据缓存到内存中。
    /// 在修改所有的参数设置时，刷新变量，实现内存数据与数据库设置数据
    /// 保持一致性，在其他线程或方法体可直接访问全局变量。
    /// </summary>
    public class SystemDriver
    {
        private static readonly SystemDriver instance = new SystemDriver();

        private SystemDriver() { }

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static SystemDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 全局系统参数
        /// </summary>
        public List<tn_dts_setting> SysSetList = new List<tn_dts_setting>();

        /// <summary>
        /// 获取分类下面的参数项
        /// </summary>
        /// <param name="classify">分类名称</param>
        /// <returns></returns>
        public List<tn_dts_setting> GetKeyListForClassify(string classify)
        {
            return SysSetList.Where(p => p.cn_s_setting_class == classify).ToList();
        }

        /// <summary>
        /// 通过关键字编码获取关键字值
        /// </summary>
        /// <param name="keyCode">关键字编码</param>
        /// <returns></returns>
        public tn_dts_setting GetKeyValueObjectForKeyCode(string keyCode)
        {
            return SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == keyCode);
        }

        /// <summary>
        /// 通过关键字编码获取关键字值
        /// </summary>
        /// <param name="keyCode">关键字编码</param>
        /// <returns></returns>
        public string GetKeyValueForKeyCode(string keyCode)
        {
            string result = "";
            var keyObject = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == keyCode);
            if (keyObject != null)
            {
                result = keyObject.cn_s_setting_keyvalue;
            }
            return result;
        }

        /// <summary>
        /// 根据关键字设置参数值
        /// </summary>
        /// <param name="keyCode">关键字</param>
        /// <param name="keyValue">关键字值</param>
        /// <returns></returns>
        public bool SetKeyValueForKeyCode(string keyCode,string keyValue)
        {
            bool result = false;
            foreach (var key in SysSetList)
            {
                if (key.cn_s_setting_keycode == keyCode)
                {
                    key.cn_s_setting_keyvalue = keyValue;
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
