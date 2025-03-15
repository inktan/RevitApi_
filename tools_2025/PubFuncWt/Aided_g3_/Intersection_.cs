using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Intersection_
    {
        public static bool IntrLine3Line3(this Line3d line01, Line3d line02, out Vector3d intersection)
        {
            intersection = Vector3d.Zero;
            bool inter = Math3d_.LineLineIntersection(out intersection, line01.Origin, line01.Direction, line02.Origin, line02.Direction);
            if (inter)
            {
                if (line01.DistanceSquared(intersection).EqualZreo(MathUtil.Epsilon) &&
                    line02.DistanceSquared(intersection).EqualZreo(MathUtil.Epsilon))
                {
                    return true;
                }
            }
            intersection = Vector3d.Zero;
            return false;
        }

        /// <summary>
        /// 直线与线圈相交
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Vector2d> IntrLine2Polygon(this Line2d segment2d_p0, Polygon2d regionPolygon2d)
        {
            IEnumerable<Segment2d> segment2ds = regionPolygon2d.SegmentItr();
            IntrLine2Segment2 intrLine2Segment2 = new IntrLine2Segment2(segment2d_p0, segment2ds.First());
            foreach (var item in segment2ds)
            {
                intrLine2Segment2.Segment = item;
                intrLine2Segment2.Compute();

                if (intrLine2Segment2.Quantity == 1)
                {
                    yield return intrLine2Segment2.Point;
                }
            }
        }
    }
}
