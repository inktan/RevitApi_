using g3;
using goa.Common.g3InterOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Frame3f_
    {
        public static Frame3d ToFrame3d(this Frame3f frame3f)
        {
            return new Frame3d(frame3f.Origin.To3d(), frame3f.X.To3d(), frame3f.Y.To3d(), frame3f.Z.To3d());
        }

    }
}
