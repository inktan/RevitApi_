using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 工时计算
{
    class Program
    {
        static void Main(string[] args)
        {
            // 工时计算
            int dayCount = 4;
            double result = 11 + 52 / 60 ;// 所有工作量
             result += 13 + 20 / 60; ;// 所有工作量
             result += 8 + 44 / 60; ;// 所有工作量
             result += 8 + 44 / 60; ;// 所有工作量
             result += 0 + 0 / 60; ;// 所有工作量
            result -= 8.5 * dayCount;// 所有工时减去日要求工时
            result /= dayCount;// 得到平均每日加班工时数量
            Console.WriteLine(result);

        }
    }
}
