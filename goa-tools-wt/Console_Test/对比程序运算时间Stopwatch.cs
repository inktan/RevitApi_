using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program2
    {
        void Main(string[] args)
        {
            //DateTime startTime = System.DateTime.Now;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < 100000000; i++)
            {
                Math.Sqrt(4);
            }
            sw.Stop();

            //DateTime endTime = System.DateTime.Now;
            //TimeSpan ts = endTime - startTime;

            Console.WriteLine("程序执行所用时间:" + sw.ElapsedMilliseconds.ToString() + " 毫秒");

            //startTime = System.DateTime.Now;

            sw.Start();
            for (int i = 0; i < 100000000; i++)
            {
                double dou = i * i;
            }
            sw.Stop();

            //endTime = System.DateTime.Now;
            //ts = endTime - startTime;

            Console.WriteLine("程序执行所用时间:" + sw.ElapsedMilliseconds.ToString() + " 毫秒");
        }
    }
}
