using Autodesk.Revit.DB;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using goa.Common;

namespace PubFuncWt
{
    public static class OctreeTo_
    {
      

        public static XYZ ToXyz(this Vector3 vector3)
        {
            return new XYZ(vector3.X, vector3.Y, vector3.Z);
        }


    }
}
