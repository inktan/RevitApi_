using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class BoundingBox_
    {
        public static IEnumerable<Line> ToLines(this BoundingBoxXYZ boundingBoxXYZ)
        {
            if (boundingBoxXYZ == null) return new List<Line>();
            return boundingBoxXYZ.ToXyzs().ToLines();
        }
        /// <summary>
        /// BondingBox的四个角点（可能只存在两个点），当前工作平面为正投影平面图，点需要去重
        /// </summary>
        public static IEnumerable<XYZ> ToXyzs(this BoundingBoxXYZ boundingBoxXYZ)
        {
            if (boundingBoxXYZ == null) return new List<XYZ>();

            XYZ min = boundingBoxXYZ.Min;
            XYZ max = boundingBoxXYZ.Max;

            double wight = max.X - min.X;
            double height = max.Y - min.Y;
            XYZ _rd = new XYZ(min.X + wight, min.Y, 0);
            XYZ _ru = new XYZ(max.X, max.Y, 0);
            XYZ _lu = new XYZ(min.X, min.Y + height, 0);
            XYZ _ld = new XYZ(min.X, min.Y, 0);

            return new List<XYZ>() { _rd, _ru, _lu, _ld };
        }
        /// <summary>
        /// BondingBox的中心点
        /// </summary>
        public static XYZ CenterXyz(this BoundingBoxXYZ boundingBoxXYZ)
        {
            if (boundingBoxXYZ == null) return null;

            XYZ min = boundingBoxXYZ.Min;
            XYZ max = boundingBoxXYZ.Max;
            return (min + max) / 2;
        }
    }
}
