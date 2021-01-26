using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Face_
    {

        #region Face

        public static Plane ToPlane(this PlanarFace planarFace)
        {
            XYZ origon = planarFace.Origin;
            XYZ normalXyz = planarFace.FaceNormal;
            return Plane.CreateByNormalAndOrigin(normalXyz, origon);
        }
        public static double DistanceTo(this Plane plane01, Plane plane02)
        {
            XYZ v = plane01.Origin;
            return v.DistanceTo(plane02);
        }
        public static double DistanceTo(this XYZ xYZ, Plane plane)
        {
            XYZ v = xYZ - plane.Origin;
            return plane.Normal.DotProduct(v);
        }
        #endregion
    }
}
