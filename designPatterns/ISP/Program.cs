using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("里氏替换原则");


            InputModule input = StuScoreList.getInputModule();
            CountModule count = StuScoreList.getCountModule();
            PrintModule print = StuScoreList.getPrintModule();
            input.Insert();
            count.CountTotalScore();
            print.PrintStuInfo();
        }
    }
    interface InputModule
    {
        void Insert();
        void Delete();
        void Modify();
    }
    interface CountModule
    {
        void CountTotalScore();
        void CountAverage();

    }
    interface PrintModule
    {
        void PrintStuInfo();
        void QueryStuInfo();
    }

    class StuScoreList : InputModule, CountModule, PrintModule
    {
        internal StuScoreList()
        {
        }
        public static InputModule getInputModule()
        {
            return (InputModule)new StuScoreList();
        }
        public static CountModule getCountModule()
        {
            return (CountModule)new StuScoreList();
        }
        public static PrintModule getPrintModule()
        {
            return (PrintModule)new StuScoreList();
        }

        public void CountAverage()
        {
            Console.WriteLine("输入模块的CountAverage()方法被调用！");
        }

        public void CountTotalScore()
        {
            Console.WriteLine("输入模块的CountTotalScore()方法被调用！");
        }

        public void Delete()
        {
            Console.WriteLine("输入模块的Delete()方法被调用！");
        }

        public void Insert()
        {
            Console.WriteLine("输入模块的insert()方法被调用！");
        }

        public void Modify()
        {
            Console.WriteLine("输入模块的Modify()方法被调用！");
        }

        public void PrintStuInfo()
        {
            Console.WriteLine("输入模块的PrintStuInfo()方法被调用！");
        }

        public void QueryStuInfo()
        {
            Console.WriteLine("输入模块的QueryStuInfo()方法被调用！");
        }
    }
}
