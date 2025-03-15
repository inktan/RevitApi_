using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;

namespace BSMT_PpLayout
{

    class ColLocPoint
    {
        internal double RotateAngle { get; set; }
        internal Vector2d Vector2d { get; set; }

        internal ColLocPoint(Vector2d vector2d, double rotateAngle)
        {
            this.Vector2d = vector2d;
            this.RotateAngle = rotateAngle;
        }
    }
}
