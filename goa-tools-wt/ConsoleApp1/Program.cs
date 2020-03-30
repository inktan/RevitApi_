using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    class Program
    {
        private static string assemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        static void Main(string[] args)
        {
            //double result01_abc = Math.Round(120 / 100.0, 5, MidpointRounding.AwayFromZero);
            //double result02_abc = Math.Round(123 / 100.0, 5, MidpointRounding.AwayFromZero);
            //double result03_abc = Math.Round(123 / 100.0, 0, MidpointRounding.AwayFromZero);
            //double result04_abc = Math.Round(156 / 100.0, 0, MidpointRounding.AwayFromZero);
            double result05_abc = Math.Ceiling(123 / 100.0);
            double result06_abc = Math.Ceiling(156 / 100.0);
            //Console.WriteLine(result01_abc.ToString());
            //Console.WriteLine(result02_abc.ToString());
            //Console.WriteLine(result03_abc.ToString());
            //Console.WriteLine(result04_abc.ToString());
            Console.WriteLine(result05_abc.ToString());
            Console.WriteLine(result06_abc.ToString());
            //string value = "";
            //double result02_abc = Math.Round(-10000 / 100.0, 5, MidpointRounding.AwayFromZero);
            //string _str = result02_abc.ToString();
            //string str = "abcdefghijklmn";
            //int index = str.IndexOf("b");
            //string _str = str.Substring(1);
            //Form1 form1 = new Form1();
            //form1.ShowDialog();

            //Line line = new Line();
            //line.setLength(2.1);
            //Console.WriteLine(assemblyDirectory);
            //Console.WriteLine(line.getLength().ToString());
            Console.ReadKey();
        }


    }
    class Line
    {
        private double length;
        public Line()
        {
            Console.WriteLine("对象已经创建");
        }
        public void setLength(double len)
        {
            length = len;
        }
        public double getLength()
        {
            return length;
        }
        ~Line()
        {
            Console.WriteLine("对象已经销毁");

        }
    }

}
