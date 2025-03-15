using Autodesk.Revit.DB;
using g3;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class XYZ_
    {
        /// <summary>
        /// 从点画一根垂直线
        /// </summary>
        /// <param name="_point"></param>
        /// <returns></returns>
        public static Line VerticalLine(this XYZ _point)
        {
            XYZ p0 = _point - XYZ.BasisZ * 999;
            XYZ p1 = _point + XYZ.BasisZ * 999;
            return Line.CreateBound(p0, p1);
        }
        public static Line VerticalLineUp(this XYZ _point)
        {
            XYZ p1 = _point + XYZ.BasisZ * 999;
            return Line.CreateBound(_point, p1);
        }
        public static Line VerticalLineDown(this XYZ _point)
        {
            XYZ p0 = _point - XYZ.BasisZ * 999;
            return Line.CreateBound(_point, p0);
        }

        /// <summary>
        /// revit创建线段有最低长度要求（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Line> ToLines(this IEnumerable<XYZ> xYZs, bool closed = false)
        {
            //【1】去除不符合距离的点——Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
            xYZs = xYZs.DelAlmostEqualPoint(Precision_.TheShortestDistance).ToList();
            //【2】
            if (xYZs.Count() >= 2)
            {
                for (int i = 0; i < xYZs.Count(); ++i)
                {
                    if (closed)
                    {
                        yield return Line.CreateBound(xYZs.ElementAt(i), xYZs.ElementAt((i + 1) % xYZs.Count()));
                    }
                    else
                    {
                        if (i < xYZs.Count() - 1)
                        {
                            yield return Line.CreateBound(xYZs.ElementAt(i), xYZs.ElementAt((i + 1) % xYZs.Count()));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 删除最近距离点_—Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<XYZ> DelAlmostEqualPoint(this IEnumerable<XYZ> xYZs, double _precison = 0.003)
        {
            List<XYZ> _xYZs = new List<XYZ>();
            foreach (XYZ xYZ in xYZs)
            {
                if (!xYZ.PointCoinPoints(_xYZs, _precison))
                {
                    _xYZs.Add(xYZ);
                }
            }
            return _xYZs;
        }
        /// <summary>
        /// 判断点是否与点集中某个点重合
        /// </summary>
        public static bool PointCoinPoints(this XYZ p0, IEnumerable<XYZ> xYZs, double _precison = 3e-3)
        {
            foreach (XYZ p1 in xYZs)
            {
                if (p0.DistanceTo(p1) <= _precison)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
