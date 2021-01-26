using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Math_
    {
        /// <summary>
        /// 判断两个数据是否相等 误差精确度设置为 1e-6
        /// </summary>
        /// <returns></returns>
        public static bool EqualPrecision(this double _d01, double _d02, double _Precison = 1e-6)
        {
            if (Math.Abs(_d01 - _d02) <= _Precison)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 判断一个浮点数是否接近于零--1e-6;
        /// </summary>
        /// <returns></returns>
        public static bool EqualZreo(this double _d01, double _Precison = 1e-6)
        {
            if (Math.Abs(_d01) <= _Precison)
                return true;
            else
                return false;
        }
        public static string ToPercent(this double _d)
        {
            return (_d).ToString("P");
        }
        /// <summary>
        /// double取小数点后固定位数
        /// </summary>
        /// <returns></returns>
        public static double NumDecimal(this double _double, int _int = 2)
        {
            string _str = _double.ToString();

            switch (_int)
            {
                case 0:
                    _str = _double.ToString("#0.");
                    break;
                case 1:
                    _str = _double.ToString("#0.0");
                    break;
                case 2:
                    _str = _double.ToString("#0.00");
                    break;
                case 3:
                    _str = _double.ToString("#0.000");
                    break;
                case 4:
                    _str = _double.ToString("#0.0000");
                    break;
                case 5:
                    _str = _double.ToString("#0.00000");
                    break;
                case 6:
                    _str = _double.ToString("#0.000000");
                    break;
                case 7:
                    _str = _double.ToString("#0.0000000");
                    break;
                case 8:
                    _str = _double.ToString("#0.00000000");
                    break;
                case 9:
                    _str = _double.ToString("#0.000000000");
                    break;
                case 10:
                    _str = _double.ToString("#0.0000000000");
                    break;
                default:
                    break;
            }
            return Convert.ToDouble(_str);
        }
        public static double Angle2Radians(this double angle)
        {
            return angle * (Math.PI / 180);
        }
        public static double Radians2Angle(this double radian)
        {
            return radian * (180 / Math.PI);
        }
    }
}
