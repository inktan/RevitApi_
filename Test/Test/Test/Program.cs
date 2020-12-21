using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ////【】运行程序文件自身的全路径
            //string fileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //Console.WriteLine(fileName);

            ////【】运行程序文件所在文件夹
            //fileName = System.Environment.CurrentDirectory;
            //Console.WriteLine(fileName);

            ////【】运行程序文件所在文件夹
            //fileName = System.IO.Directory.GetCurrentDirectory();
            //Console.WriteLine(fileName);

            ////【】运行程序文件所在文件夹 路径后多个 \
            //fileName = System.AppDomain.CurrentDomain.BaseDirectory;
            //Console.WriteLine(fileName);

            ////【】运行程序文件所在文件夹 路径后多个 \
            //fileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //Console.WriteLine(fileName);

            ////【】运行程序文件所在文件夹
            //fileName = System.Windows.Forms.Application.StartupPath;
            //Console.WriteLine(fileName);

            ////【】【】运行程序文件自身的全路径
            //fileName = System.Windows.Forms.Application.ExecutablePath;
            //Console.WriteLine(fileName);

            string path = @"E:\upadate_work\OneDrive\03-Reivt Development\GitHub\RevitApi_\goa-tools-wt\";

            string[] directories = Directory.GetDirectories(path);
            foreach (string directoryName in directories)
            {
                string strOjb = directoryName + @"\obj";
                string strBin = directoryName + @"\bin";

                if (Directory.Exists(strOjb))
                {
                    Directory.Delete(strOjb, true);
                }
                if (Directory.Exists(strBin))
                {
                    Directory.Delete(strBin, true);
                }

                if (directoryName.Substring(directoryName.Count() - 3) == @".vs")
                {
                    Directory.Delete(directoryName, true);
                }
            }

        }
    }
}
