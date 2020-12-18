using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetTopologySuite.Geometries;
using g3;

namespace Console_Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Vector2d vector2d01 = new Vector2d(-1.00000000 - 0.00000032);
            Vector2d vector2d2 = new Vector2d(0.00000000 ,0.00000000);

            Console.WriteLine(vector2d01.Dot(vector2d2));
        }
    }
}
