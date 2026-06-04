using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HZ.CommonUtil.Utilities
{
    public static class StringUtil
    {
        public static int BoolConvetArr(object bv)
        {
            if (bv is int)
            {
                return (int)bv;
            }
            else
                return Convert.ToInt16(bv);
        }
        public static bool NumTrue(int i)
        {
            return i == 1;
        }
        public static string IsNull(object o)
        {
            return o == null ? "" : o.ToString();
        }
        public static bool IsNull(DateTime dt)
        {
            if (dt == new DateTime(1, 1, 1))
                return true;
            return false;
        }

        public static object IsNullConvert(DateTime? dt)
        {
            if (dt == null)
                return "";
            return dt;
        }
        public static string Trim(string obj)
        {
            if (string.IsNullOrEmpty(obj))
                return "";
            return obj.Trim();
        }
        public static string ToStringTrim(JToken obj)
        {
            if (obj == null ||  string.IsNullOrEmpty(obj.ToString()))
                return "";
            return obj.ToString().Trim();
        }

        public static int ToInt(JToken obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return 0;
            return Convert.ToInt16(obj);
        }
        public static float ToSingle(JToken obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return 0;
            return Convert.ToSingle(obj);
        }
        public static bool ToBool(JToken obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return false;
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// JToken转List<string>
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-08-19 14:30
        /// </remarks>
        /// <param name="jToken"></param>
        /// <returns></returns>
        public static List<string> JTokenToArr(JToken jToken)
        {
            if (jToken == null)
                return new List<string>();
            try
            {
                if (jToken.Type == JTokenType.Array)
                {
                    var value = jToken.ToObject<List<string>>();
                    if (value == null)
                        return new List<string>();
                    return value;
                }
                else if (jToken.Type == JTokenType.String)
                {
                    string[] s = jToken.ToString().Split(',',StringSplitOptions.RemoveEmptyEntries);
                    return new List<string>(s);
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                return new List<string>() { jToken.ToString() };
            }
        }

        /// <summary>
        /// 获取通道编码及方向
        /// </summary>
        /// <param name="sortDirection"></param>
        /// <param name="passagewayCode"></param>
        /// <returns></returns>
        public static KeyValuePair<string, int> ConvertPassageway(string sortDirection, string passagewayCode)
        {
            try
            {
                string passageway = "";
                string[] arr = passagewayCode.Split('-');
                if (arr.Length ==2)
                {
                    passageway = arr[0];

                    if (sortDirection == "列从小到大")
                        return new KeyValuePair<string, int>(passageway, arr[1]=="正"?1:0);
                    else if (sortDirection == "列从大到小")
                        return new KeyValuePair<string, int>(passageway, arr[1] == "正" ? 0 :1);
                }
                return new KeyValuePair<string, int>();
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, int>(); ;
            }
        }

        /// <summary>
        /// 获取通道编码
        /// </summary>
        /// <param name="passagewayCode"></param>
        /// <returns></returns>
        public static string ConvertPassagewayCode(string passagewayCode)
        {
            //通道01排1层-反
            string passageway = "";
            string[] arr = passagewayCode.Split('-');
            if (arr.Length ==2)
            {
                return passageway = arr[0];
            }
            return "";
        }


        public static string GetDescriptionFromEnum(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }


        public static Enum GetEnumFromStr<T>(this string name)
        {

            return (Enum)Enum.Parse(typeof(T),name);
        }

        public static Enum GetEnumFromInt<T>(this int value)
        {
            return (Enum)Enum.Parse(typeof(T), Enum.GetName(typeof(T), value));
        }

        public static string SplitSingleMark(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"'([^']*)'"); // 匹配单引号内的内容
            if (matches.Count > 0)
                return matches[0].Groups[1].Value;
            //    foreach (Match match in matches)
            //{
            //    Console.WriteLine(match.Groups[1].Value); // 输出匹配到的内容
            //}
            return "";
        }

        /// <summary>
        /// 截取字符串中中括号[]中的数据
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-11-25 22:11
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] GetTextInBrackets(string input)
        {
            Regex regex = new Regex(@"\[(.*?)\]");
            MatchCollection matches = regex.Matches(input);

            string[] results = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                results[i] = matches[i].Groups[1].Value;
            }

            return results;
        }
    }
}
