using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class XYZ_
    {
        /// <summary>
        /// revit创建线段有最低长度要求（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Line> ToLines(this IEnumerable<XYZ> xYZs)
        {
            //【1】去除不符合距离的点——Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
            xYZs = xYZs.DelDuplicate(Precision_.TheShortestDistance).ToList();
            //【2】
            if (xYZs.Count() >= 2)
            {
                for (int i = 0; i < xYZs.Count(); ++i)
                    yield return Line.CreateBound(xYZs.ElementAt(i), xYZs.ElementAt((i + 1) % xYZs.Count()));
            }

        }
        /// <summary>
        /// 去重_—Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<XYZ> DelDuplicate(this IEnumerable<XYZ> xYZs, double _precison = 0.003)
        {
            List<Vector2d> vector2ds = xYZs.ToVector2ds().ToList();
            return vector2ds.DelDuplicate(_precison).ToXyzs();
        }

    }
}
