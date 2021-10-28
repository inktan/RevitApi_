using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("依赖倒置原则");

            Customer customer = new Customer();
            customer.Shopping(new ShopA());
            customer.Shopping(new ShopB());
        }
    }

    interface Shop
    {
        String Sell();
    }

    class ShopA : Shop
    {
        public string Sell()
        {
            return "A特产";
        }
    }
    class ShopB : Shop
    {
        public string Sell()
        {
            return "A特产";
        }
    }

    class Customer
    {
        public void Shopping(Shop shop)
        {
            Console.WriteLine(shop.Sell());
        }
    }
}
