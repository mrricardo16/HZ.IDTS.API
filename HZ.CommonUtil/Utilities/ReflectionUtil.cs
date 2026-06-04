using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Utilities
{
    public static class ReflectionUtil
    {
        /// <summary>
        /// 获取类中的属性值
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetModelValue(string FieldName, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                object o = Ts.GetProperty(FieldName).GetValue(obj, null);
                return o;
                //string Value = Convert.ToString(o);
                //if (string.IsNullOrEmpty(Value)) return null;
                //return Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 设置类中的属性值
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Value"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SetModelValue(string FieldName, string Value, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                object v = Convert.ChangeType(Value, Ts.GetProperty(FieldName).PropertyType);
                Ts.GetProperty(FieldName).SetValue(obj, v, null);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
