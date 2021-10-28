using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("接口隔离原则");

            Bird bird01 = new Swallow();
            Animal bird02 = new BrownKiwi();
         
            Console.WriteLine("end");
            Console.ReadKey();
        
        }
    }
}
