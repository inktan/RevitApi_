using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PubFuncWt
{
    public static class Vector3f_
    {

        public static Vector3d To3d(this Vector3f vector3f)
        {
            return new Vector3d(vector3f.x, vector3f.y, vector3f.z);
        }

    }
}
