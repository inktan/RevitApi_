using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PubFuncWt
{
    public static class Curve_
    {

        /// <summary>
        /// 将曲线打成多段线，使用Tessellate为粗略算法
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static List<Line> DivideToLinesByTessellate(this Arc arc, double _dist, bool closed = false)
        {
            var pts = arc.Tessellate();

            List<XYZ> xYZs = new List<XYZ>();
            xYZs.Add(pts.First());

            int current = 0;
            int next = 1;

            bool jumpOut = false;

            while (true)
            {
                for (int i = next; i < pts.Count; i++)
                {
                    double dis = pts[current].DistanceTo(pts[next]);
                    if (dis < _dist)
                    {
                        next++;
                        if (next == pts.Count - 1)
                        {
                            xYZs.Add(pts[next]);
                            jumpOut = true;
                            break;
                        }
                    }
                    else
                    {
                        xYZs.Add(pts[next]);
                        current = next;

                        next++;
                        if (next == pts.Count - 1)
                        {
                            xYZs.Add(pts[next]);
                            jumpOut = true;
                            break;
                        }
                        break;
                    }
                }
                if (jumpOut)
                {
                    break;
                }
            }
            return xYZs.ToLines(closed).ToList();
        }

        public static List<Line> DivideToLinesByCyclic(this Arc _arc, double _dist, bool closed = true)
        {
            var currP = _arc.IsBound
                ? _arc.GetEndPoint(0)
                : _arc.Tessellate().First();
            var prevP = currP;
            var list = new List<XYZ>() { currP };
            var end = _arc.IsBound
                ? _arc.GetEndPoint(1)
                : currP;
            var count = Math.Ceiling(_arc.Length / _dist);

            for (int i = 0; i < count; i++)
            {
                var intrCircle = Arc.Create(currP, _dist, 0.0, MathUtils.FullCircle, XYZ.BasisX, XYZ.BasisY);
                IntersectionResultArray ra;
                var result = intrCircle.Intersect(_arc, out ra);
                if (result != SetComparisonResult.Overlap)
                    break;
                var intrR = ra.Cast<IntersectionResult>()
                    .FirstOrDefault(x =>
                    x.XYZPoint.IsAlmostEqualToByDifference(prevP) == false
                    && x.XYZPoint.IsAlmostEqualToByDifference(end) == false);
                if (intrR == null)
                    break;
                else
                {
                    prevP = currP;
                    currP = intrR.XYZPoint;
                    list.Add(currP);
                }
            }

            //_open = _arc.IsBound;
            //if (_open)
            //{
            //    if (list.Count > 1)
            //        list.RemoveAt(list.Count - 1);
            list.Add(end);
            //}

            return list.ToLines(closed).ToList();
        }

    }
}
