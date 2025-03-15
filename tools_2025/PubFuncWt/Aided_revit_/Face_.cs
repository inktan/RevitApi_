using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
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
        /// <summary>
        /// 找到朝上的面
        /// </summary>
        public static bool IsFaceFacingUp(this PlanarFace _face)
        {
            //判断法向量Z值大于零还是小于零
            XYZ normal = _face.FaceNormal;
            //大于零，返回true
            return normal.Z > 0.0;
            //return normal.Z > 0.0 && !normal.Z.EqualZreo();
        }
        /// <summary>
        /// 找到垂直朝上的面
        /// </summary>
        public static bool IsFaceVerticalUp(this PlanarFace _face)
        {
            return _face.FaceNormal.Z.EqualPrecision(1, 1e-10);
        }
        /// <summary>
        /// 找到垂直朝上的面
        /// </summary>
        public static bool IsFaceVerticalDown(this PlanarFace _face)
        {
            return _face.FaceNormal.Z.EqualPrecision(-1,1e-10);
        }
        #endregion
    }
}
