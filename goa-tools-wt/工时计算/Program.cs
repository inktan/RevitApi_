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
            double result = 12 + 41 / 60 + 12 + 50 / 60 + 12 + 28 / 60 + 13 + 21 / 60 + 12 + 39 / 60;// 所有工作量
            result -= 8.5 * 5;// 所有工时减去日要求工时
            result /= 5;// 得到平均每日加班工时数量
            Console.WriteLine(result);

        }
    }
}
