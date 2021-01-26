using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class String_
    {
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
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

    }
}
