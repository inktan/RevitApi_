using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PubFuncWt
{
    public static class String_
    {
        public static char[] TwentySxLetters =new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static string GetChinese(this string str)
        {
            string mystr = "", ResultString = "";
            for (int i = 0; i <= str.Length - 1; i++)
            {
                mystr = str.Substring(i, 1);
                //Console.WriteLine(mystr);
                if (str != "")
                {
                    if (Regex.IsMatch(mystr, "[\u4e00-\u9fa5]"))
                        ResultString += mystr;
                }
            }
            return ResultString;
        }
        /// <summary>
        /// 使用正则表达式，取出所有中文字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveChinese(this string str)
        {
            string mystr = "", ResultString = "";
            for (int i = 0; i <= str.Length - 1; i++)
            {
                mystr = str.Substring(i, 1);
                //Console.WriteLine(mystr);
                if (str != "")
                {
                    if (!Regex.IsMatch(mystr, "[\u4e00-\u9fa5]") && mystr != @"/" && mystr != @"【" && mystr != @"】")
                    {
                        ResultString += mystr;

                    }
                }
            }
            return ResultString;
        }
        /// <summary>
        /// 判断一个字符串是否包含列表内所有字符串
        /// </summary>
        /// <returns></returns>
        public static bool ContainAllListString(this string str, List<string> strEs)
        {
            int count = 0;
            int _count = strEs.Count;
            foreach (string tempStr in strEs)
            {
                if (str.Contains(tempStr))
                {
                    count++;
                }
            }


            if (_count == count)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 判断一个字符串是否包含列表内任一所有字符串
        /// </summary>
        /// <returns></returns>
        public static bool ContainAnyListString(this string str, List<string> strEs)
        {
            int count = 0;
            foreach (string tempStr in strEs)
            {
                if (str.Contains(tempStr))
                {
                    count++;
                }
            }

            if (count > 0)
                return true;
            else
                return false;
        }
        //public static bool IsNullOrEmpty(this string s)
        //{
        //    return string.IsNullOrEmpty(s);
        //}
        public static int SubstringCount(this string str, string substring)
        {
            if (str.Contains(substring))
            {
                string strReplaced = str.Replace(substring, "");
                return (str.Length - strReplaced.Length) / substring.Length;
            }
            return 0;
        }

        public static void MessageShow(this string s)
        {
            MessageBox.Show(s);

        }
    }
}
