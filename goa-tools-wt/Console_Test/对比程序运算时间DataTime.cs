using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        /// <summary>
        /// 该方法计算时间不准确，推荐使用stopWatch
        /// </summary>
        /// <param name="args"></param>
        void Main(string[] args)
        {
            DateTime startTime = System.DateTime.Now;

            for (int i = 0; i < 100000; i++)
            {
                Math.Sqrt(4);
            }

            DateTime endTime = System.DateTime.Now;
            TimeSpan ts = endTime - startTime;

            Console.WriteLine("程序执行所用时间:" + ts.Milliseconds + " 毫秒");

            startTime = System.DateTime.Now;

            for (int i = 0; i < 100000; i++)
            {
                double dou = i * i;
            }

            endTime = System.DateTime.Now;
            ts = endTime - startTime;

            Console.WriteLine("程序执行所用时间:" + ts.Milliseconds + " 毫秒");
        }
    }
}
