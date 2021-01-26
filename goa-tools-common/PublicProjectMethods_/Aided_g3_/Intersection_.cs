using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Intersection_
    {
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
