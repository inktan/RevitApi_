using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PubFuncWt
{
    public static class BoundingBox_
    {
        public static bool IsNullOrEmpty(this BoundingBoxXYZ boundingBoxXYZ)
        {
            return boundingBoxXYZ == null;
        }
        public static bool HeightIsZero(this BoundingBoxXYZ boundingBoxXYZ)
        {
            if (boundingBoxXYZ.Depth().EqualZreo()) return true;
            return false;
        }


        //public static Outline ToOutLine(this BoundingBoxXYZ boundingBoxXYZ)
        //{
        //    return new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
        //}
        /// <summary>
        /// 对外部盒子做了微放大处理
        /// </summary>
        /// <returns></returns>
        public static Outline ToOutLineBigger(this BoundingBoxXYZ boundingBoxXYZ, double z)
        {
            XYZ min = boundingBoxXYZ.Min;
            XYZ max = boundingBoxXYZ.Max;
            min = new XYZ(min.X, min.Y, z) - Precision_.ShortestXYZ;
            max = new XYZ(max.X, max.Y, z) + Precision_.ShortestXYZ;
            return new Outline(min, max);
        }
        /// <summary>
        /// 对外部盒子做了微缩小处理
        /// </summary>
        /// <returns></returns>
        public static Outline ToOutLineSmaller(this BoundingBoxXYZ boundingBoxXYZ, double z)
        {
            XYZ min = boundingBoxXYZ.Min;
            XYZ max = boundingBoxXYZ.Max;
            min = new XYZ(min.X, min.Y, z) + Precision_.ShortestXYZ;
            max = new XYZ(max.X, max.Y, z) - Precision_.ShortestXYZ;
            return new Outline(min, max);
        }
        /// <summary>
        /// 盒子对角线长度
        /// </summary>
        public static double DiagonalLengeth(this BoundingBoxXYZ boundingBoxXYZ)
        {
            return boundingBoxXYZ.Max.DistanceTo(boundingBoxXYZ.Min);
        }
        /// <summary>
        /// X轴 长度
        /// </summary>
        public static double Width(this BoundingBoxXYZ boundingBoxXYZ)
        {
            return boundingBoxXYZ.Max.X - boundingBoxXYZ.Min.X;
        }
        /// <summary>
        /// Y轴 长度
        /// </summary>
        public static double Height(this BoundingBoxXYZ boundingBoxXYZ)
        {
            return boundingBoxXYZ.Max.Y - boundingBoxXYZ.Min.Y;
        }
        /// <summary>
        /// Z轴 长度
        /// </summary>
        public static double Depth(this BoundingBoxXYZ boundingBoxXYZ)
        {
            return boundingBoxXYZ.Max.Z - boundingBoxXYZ.Min.Z;
        }
        /// <summary>
        /// BondingBox的底面线圈
        /// </summary>
        public static IEnumerable<Line> BottomLines(this BoundingBoxXYZ boundingBoxXYZ)
        {
            if (boundingBoxXYZ == null) return new List<Line>();
            return boundingBoxXYZ.BottomXyzs().ToLines();
        }
        /// <summary>
        /// BondingBox的底面线圈的四个角点
        /// </summary>
        public static IEnumerable<XYZ> BottomXyzs(this BoundingBoxXYZ boundingBoxXYZ)
        {
            if (boundingBoxXYZ == null) return new List<XYZ>();

            XYZ min = boundingBoxXYZ.Min;
            XYZ max = boundingBoxXYZ.Max;

            XYZ _rd = new XYZ(max.X, min.Y, 0);
            XYZ _lu = new XYZ(min.X, max.Y, 0);

            return new List<XYZ>() { min, _rd, max, _lu };
        }

    }
}
