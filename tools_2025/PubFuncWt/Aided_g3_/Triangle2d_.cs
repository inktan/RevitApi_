using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    static public class Triangle2d_
    {
        #region Type conversion

        static public Triangle3d ToTriangle3d(this Triangle2d triangle2d, double _z = 0.0)
        {
            return new Triangle3d(triangle2d[0].ToVector3d(_z), triangle2d[1].ToVector3d(_z), triangle2d[2].ToVector3d(_z));
        }
        #endregion

    }
}
