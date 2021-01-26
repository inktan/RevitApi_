using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class ConvexHull2_
    {
        /// <summary>
        /// 获取点集合的凸包polygon2d
        /// </summary>
        /// <param name="vector2ds"></param>
        /// <returns></returns>
        public static Polygon2d ConvexHullPolygon2d(this IEnumerable<Vector2d> vector2ds)
        {
            ConvexHull2 convexHull2 = new ConvexHull2(vector2ds.ToList(), Precision_.Precison, QueryNumberType.QT_DOUBLE);
            return convexHull2.GetHullPolygon();
        }
    }
}
