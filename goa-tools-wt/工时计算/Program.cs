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
            double dayCount = 5.0;
            double result = 12 +30/ 60.0;// 所有工作量
            result += 13 + 51/ 60.0 ;// 所有工作量
            result += 13+ 7 / 60.0 ;// 所有工作量
            result += 13 + 17 / 60.0; ;// 所有工作量
            result += 12 + 30 / 60.0; ;// 所有工作量
            result -= 8.5 * dayCount;// 所有工时减去日要求工时
            result /= dayCount;// 得到平均每日加班工时数量
            Console.WriteLine(result);

            Console.ReadKey();
        }
    }

}
