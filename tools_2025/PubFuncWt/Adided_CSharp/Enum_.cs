using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{

    /// <summary>
    /// 带有文字描述的Enum
    /// </summary>
    public enum TesTEnum : int
    {
        [Description("数据异常，请手动查看")]
        None,
    }
    public static class Enum_
    {
        /// <summary>
        /// 读取枚举值的对应文字描述
        /// </summary>
        /// <returns></returns>
        public static string GetEnumDescriptionFromEnum(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfos = type.GetMember(en.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }
        /// <summary>
        /// 根据传入的str读取对应enum中的描述
        /// </summary>
        /// <returns></returns>
        public static string GetEnumDescriptionFromStr<T>(this string name)
        {
            return GetEnumDescriptionFromEnum((Enum)GetEnumFromStr<T>(name));
        }
        /// <summary>
        /// 根据传入的int读取对应enum中的描述
        /// </summary>
        /// <returns></returns>
        public static string GetEnumDescriptionFromInt<T>(this int value)
        {
            return GetEnumDescriptionFromEnum((Enum)GetEnumFromInt<T>(value));
        }
        /// <summary>
        /// 根据枚举值的整数值返回字符串
        /// </summary>
        /// <returns></returns>
        public static string GetStrFromInt<T>(this int value)
        {
            return Enum.GetName(typeof(T), value);
        }
        public static string GetStrFromEnum(Enum en)
        {
            return en.ToString();
        }
        /// <summary>
        /// 根据枚举值的字符串获取其整数值
        /// </summary>
        /// <returns></returns>
        public static int GetIntFromStr<T>(this string name)
        {
            return (GetEnumFromStr<T>(name)).GetHashCode();
        }

        /// <summary>
        /// 获取枚举值的整数值
        /// </summary>
        /// <returns></returns>
        public static int GetIntFromEnum(this Enum en)
        {
            return en.GetHashCode();
        }
        /// <summary>
        /// 根据字符串返回枚举值
        /// </summary>
        /// <returns></returns>
        private static Enum GetEnumFromStr<T>(this string name)
        {
            return (Enum)Enum.Parse(typeof(T), name);
        }
        /// <summary>
        /// 根据整数值返回枚举值
        /// </summary>
        /// <returns></returns>
        private static Enum GetEnumFromInt<T>(this int value)
        {
            return (Enum)Enum.Parse(typeof(T), Enum.GetName(typeof(T), value));
        }
    }
}
