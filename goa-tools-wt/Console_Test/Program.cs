using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Console_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            A a1 = new B();
            Console.WriteLine(a1.GetType().ToString());

        }
    }
    class A
    {
        public  void Print()
        {
            Console.WriteLine("A");
        }
    }
    class B : A
    {
        public new void Print()
        {
            Console.WriteLine("B");
        }
    }
    //class C : A
    //{
    //    public override void Print()
    //    {
    //        Console.WriteLine("C");
    //    }
    //}
}
