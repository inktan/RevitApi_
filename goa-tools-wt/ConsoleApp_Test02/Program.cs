using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_Test02
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        private static List<string> findLinesByLabel(string textFilePath, string label)
        {
            List<string> list = new List<string>();
            using (StreamReader streamReader = new StreamReader(textFilePath))
            {
                while (true)
                {
                    string str = null;
                    string str1 = streamReader.ReadLine();
                    string str2 = str1;
                    if (str1 == null)
                    {
                        break;
                    }
                    if (str2.Contains(label))
                    {
                        str = str2;
                        str = str.Remove(0, label.Length);
                        list.Add(str);
                    }
                }
            }
            return list;
        }
    }
}
