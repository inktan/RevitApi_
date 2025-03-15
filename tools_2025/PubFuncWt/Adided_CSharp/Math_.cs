using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace PubFuncWt
{
    public static class Math_
    {
        /// <summary>
        /// 判断两个数据是否相等 误差精确度设置为 1e-6
        /// </summary>
        /// <returns></returns>
        public static bool EqualPrecision(this double _d01, double _d02, double _Precison = 1e-3)
        {
            return Math.Abs(_d01 - _d02) <= _Precison;
        }
        /// <summary>
        /// 判断一个浮点数是否接近于零--1e-6;
        /// </summary>
        /// <returns></returns>
        public static bool EqualZreo(this double _d01, double _Precison = 1e-3)
        {
            return Math.Abs(_d01) <= _Precison;
        }
        public static string ToPercent(this double _d, int v)
        {
            return (_d).ToString("P");
        }
        /// <summary>
        /// double取小数点后固定位数 四舍五入
        /// </summary>
        /// <returns></returns>
        public static double NumDecimal(this double _double, int _int = 2)
        {
            return Convert.ToDouble(_double.ToStringDigits(_int));
        }
        /// <summary>
        /// double取小数点后固定位数 直接抹去后面位数，不需要四舍五入
        /// </summary>
        /// <returns></returns>
        public static double _DoubleDecimal(this double _double, int _int = 2)
        {
            string temp = _double.ToString();

            // 求小数点索引值
            int index = temp.IndexOf('.');
            if (index == -1)// 不存在小数点
            {
                return _double;
            }
            // 求出小数点后的尾巴
            string[] strArr = temp.Split('.');
            string strLast = strArr.Last();
            strLast = strLast.Substring(index - 1, _int);// 取需要的位数

            string result = strArr.First() + '.' + strLast;// 重组小数
            return Convert.ToDouble(result);
        }
    }
}
