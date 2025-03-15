using Autodesk.Revit.DB;
using g3;
//using goa.Common.g3InterOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;

namespace TOPO_ANLS
{
    class ElevationTriangle
    {
        internal Triangle3d Triangle3d;
        internal ColorWithTransparency ColorWithTransparency;

        // 高程
        internal double zValue;
        // 坡向
        internal double aspectValue;
        // 坡度
        internal double slopeValue;


        internal ElevationTriangle(Triangle3d _triangle3d)
        {
            this.Triangle3d = _triangle3d;

            // 三角面默认颜色
            this.ColorWithTransparency = new ColorWithTransparency(0, 250, 0, 0);
        }
    }
}
