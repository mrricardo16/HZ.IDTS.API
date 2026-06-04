using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace HZ.CommonUtil.Helpers
{
    public static class EnumHelper
    {
        public static T StringToEnum<T>(string value) where T : Enum
        {
            T actionEnum = (T)Enum.Parse(typeof(T), value);
            return actionEnum;
        }

        /// <summary>
        /// 获取枚举的提示文本
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDescription<Enum>(this Enum @enum)
        {
            FieldInfo fieldInfo = @enum.GetType().GetField(@enum.ToString());
            DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : @enum.ToString();
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);  //获取描述属性
            if (objs == null || objs.Length == 0)  //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }

        /// <summary>
        /// 枚举字符串转int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetInt<T>(string name) where T : struct
        {
            return (int)Enum.Parse(typeof(T), name);
        }
    }
}
