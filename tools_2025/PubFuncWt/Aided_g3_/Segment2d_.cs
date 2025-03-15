using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipperLib;
using g3;
using goa.Common;

namespace PubFuncWt
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;


    public static class Segment2d_
    {
        #region DataProcesse
        /// <summary>
        /// 删除重复线段
        /// </summary>
        /// <param name="segment2ds"></param>
        /// <returns></returns>
        public static List<Segment2d> DelDuplicate(this IEnumerable<Segment2d> segment2ds)
        {
            List<Segment2d> result = new List<Segment2d>();
            foreach (var seg01 in segment2ds)
            {
                bool isIn = true;
                foreach (var seg02 in result)// 判断收集器是否已存在
                {
                    if (seg01.Center.EpsilonEqual(seg02.Center, Precision_.TheShortestDistance))
                    {
                        if (seg01.Direction.EpsilonEqual(seg02.Direction, Precision_.TheShortestDistance) || seg01.Direction.EpsilonEqual(seg02.Direction * (-1), Precision_.TheShortestDistance))
                        {
                            if (seg01.Length.EqualPrecision(seg02.Length))
                            {
                                isIn = false;
                                break;
                            }
                        }
                    }
                }
                if (isIn)
                {
                    result.Add(seg01);
                }
            }
            return result;
        }

        #endregion
        /// <summary>
        /// 将一个线段分段成点集，包含首尾点，如10段为11个点
        /// </summary>
        public static IEnumerable<Vector2d> DivideBySegNums(this Segment2d segment2d, int segNumbers = 10, bool _includeStart = true, bool _includeEnd = true)
        {
            double intervalT = 1.0 / segNumbers;
            if (_includeStart)
            {
                for (int i = 0; i < segNumbers; i++)
                {

                    yield return segment2d.PointBetween(i * intervalT);
                }
            }
            else
            {
                for (int i = 1; i < segNumbers; i++)
                {

                    yield return segment2d.PointBetween(i * intervalT);
                }
            }

            if (_includeEnd)
            {
                yield return segment2d.PointBetween(1.0);
            }
        }
        /// <summary>
        /// 基于距离将线段分段，包含首尾点
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Vector2d> DivideByDis(this Segment2d segment2d, double distance = 1.0, bool _includeStart = true, bool _includeEnd = true)
        {
            int segNums = (int)(segment2d.Length / distance);
            return segment2d.DivideBySegNums(segNums, _includeStart, _includeEnd);
        }
        /// <summary>
        /// 将线段向两侧偏移
        /// </summary>
        /// <param name="segment2d"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static IEnumerable<Segment2d> Offset(this Segment2d seg2d, double width)
        {
            Vector2d dir = seg2d.Direction.Rotate(Vector2d.Zero, Math.PI / 2);
            yield return seg2d.Move(dir, width);
            yield return seg2d.Move(dir * -1, width);
        }
        /// <summary>
        /// 线段转为化矩形宽度为 width * 2，且线段两端头不进行延伸操作，请参看clipper—EndType.etOpenButt
        /// </summary>
        /// <returns></returns>
        public static Polygon2d ToRenct2d(this Segment2d segment2d, double width)
        {
            Paths paths = segment2d.EndPoints().ToPath().Offset(width, Precision_.ClipperMultiple, EndType.etOpenButt);

            if (paths.Count > 0)
            {
                Polygon2d o = paths.First().ToPolygon2d();
                if (o.Area > 0)
                    return o;
            }
            return new Polygon2d();
        }

        /// <summary>
        /// 基于相交点，打断所有线段
        /// </summary>
        /// <param name="segment2ds"></param>
        /// <returns></returns>
        public static List<Segment2d> BreakSegment2ds(this List<Segment2d> segment2ds)
        {
            List<Segment2d> colSegment2ds = new List<Segment2d>();

            int count = segment2ds.Count;

            for (int i = 0; i < count; i++)
            {
                List<Vector2d> vector2ds = new List<Vector2d>();// 收集当前线段的所有线段交点
                for (int j = 0; j < count; j++)
                {
                    if (i != j)// 自身不必检查相交
                    {
                        Segment2d _seg2d01 = segment2ds[i].TwoWayExtension(3.0);
                        Segment2d _seg2d02 = segment2ds[j].TwoWayExtension(3.0);

                        IntrSegment2Segment2 intrSeg2Seg2 = new IntrSegment2Segment2(_seg2d01, _seg2d02);
                        intrSeg2Seg2.Compute();

                        if (intrSeg2Seg2.Quantity == 1)
                        {
                            Vector2d vector2d = intrSeg2Seg2.Point0;
                            vector2ds.Add(vector2d);
                        }
                    }
                }

                vector2ds.Add(segment2ds[i].P0);
                vector2ds.Add(segment2ds[i].P1);
                if (segment2ds[i].Direction.y.EqualPrecision(1) || segment2ds[i].Direction.y.EqualPrecision(-1))
                {
                    vector2ds = vector2ds.DelDuplicate().OrderBy(p => p.y).ToList();
                }
                else
                {
                    vector2ds = vector2ds.DelDuplicate().OrderBy(p => p.x).ToList();
                }
                //【】拿到所有的断线
                for (int k = 0; k < vector2ds.Count - 1; ++k)
                {
                    Segment2d seg2d = new Segment2d(vector2ds[k], vector2ds[(k + 1) % vector2ds.Count]);
                    colSegment2ds.Add(seg2d);
                }
            }
            return colSegment2ds;
        }
        public static List<Vector2d> EndPoints(this Segment2d segment2d)
        {
            List<Vector2d> vector2ds = new List<Vector2d>();
            vector2ds.Add(segment2d.P0);
            vector2ds.Add(segment2d.P1);
            return vector2ds;
        }

        /// <summary>
        /// 判断一个线段是否与线圈的边界重合，两个端点，以及线段中心点
        /// </summary>
        /// <returns></returns>
        public static bool IsCoincide(this Segment2d segment2d, Polygon2d polygon2d)
        {
            bool isCoincide = false;

            Vector2d p0 = segment2d.P0;
            Vector2d p1 = segment2d.P1;
            Vector2d middlePoint = segment2d.Center;

            double distance00 = polygon2d.DistanceSquared(p0);// 距离平方
            double distance01 = polygon2d.DistanceSquared(p1);// 距离平方
            double distance02 = polygon2d.DistanceSquared(middlePoint);

            if (distance00 < Precision_.Precison && distance01 < Precision_.Precison && distance02 < Precision_.Precison)
            {
                isCoincide = true;
            }
            return isCoincide;
        }

        public static bool IsSmae(this Segment2d segment2d01, Segment2d segment2d02)
        {
            if (!segment2d01.Center.Distance(segment2d02.Center).EqualZreo())// 中点
                return false;
            if (!Math.Abs(segment2d01.Direction.Dot(segment2d02.Direction)).EqualPrecision(1))// 方向
                return false;
            if (!segment2d01.Extent.EqualPrecision(segment2d02.Extent))// 长度
                return false;

            return true;
        }
        /// <summary>
        /// 合并所有重叠线段
        /// </summary>
        /// <returns></returns>
        public static List<Segment2d> MergeSeg2ds(this IEnumerable<Segment2d> seg2ds, double exTension)
        {
            List<Segment2d> segment2ds = seg2ds.ToList();

            while (true)
            {
                int count = segment2ds.Count;// 记录当前 道路线 数量 

                bool noWhile = true;
                for (int i = 0; i < count; i++)
                {
                    bool isBreak = false;
                    for (int j = 0; j < count; j++)
                    {
                        if (j > i)
                        {
                            Segment2d seg2d01 = segment2ds[i];
                            Segment2d seg2d02 = segment2ds[j];
                            Segment2d overLap = seg2d01.MergeSeg2ds(seg2d02, exTension);// 提取重线
                            if (overLap.Length > Precision_.Precison)
                            {
                                segment2ds.Add(overLap);
                                segment2ds.RemoveAt(i);
                                segment2ds.RemoveAt(j);
                                isBreak = true;
                                noWhile = false;
                                break;
                            }
                        }
                    }
                    if (isBreak)
                    {
                        break;
                    }
                }

                if (noWhile)
                {
                    break;
                }
            }
            return segment2ds;
        }
        /// <summary>
        /// 合并重叠线段
        /// </summary>
        /// <returns></returns>
        public static Segment2d MergeSeg2ds(this Segment2d segment2d01, Segment2d segment2d02, double exTension)
        {
            if (segment2d01.OverLap(segment2d02, exTension))// 如果线段重叠
            {
                List<Vector2d> vector2ds = new List<Vector2d>();
                vector2ds.AddRange(segment2d01.EndPoints());
                vector2ds.AddRange(segment2d02.EndPoints());
                if (segment2d01.Direction.x.EqualZreo())
                {
                    vector2ds = vector2ds.OrderBy(p => p.y).ToList();
                }
                else
                {
                    vector2ds = vector2ds.OrderBy(p => p.x).ToList();
                }
                return new Segment2d(vector2ds.First(), vector2ds.Last());
            }

            return new Segment2d();
        }

        /// <summary>
        /// 判断两根线段是否重叠 可以对线段延长或缩短处理
        /// </summary>
        /// <returns></returns>
        public static bool OverLap(this Segment2d segment2d01, Segment2d segment2d02, double exTension)
        {
            if (Math.Abs(segment2d01.Direction.Normalized.Dot(segment2d02.Direction.Normalized)).EqualPrecision(1.0))// 判断是否平行
            {
                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2d01.TwoWayExtension(exTension), segment2d02.TwoWayExtension(exTension));
                intrSegment2Segment2.Compute();
                if (intrSegment2Segment2.Quantity == 2)// 判断是否重叠
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取两根线段的重叠部分
        /// </summary>
        /// <returns></returns>
        public static Segment2d OverLapSeg(this Segment2d segment2d01, Segment2d segment2d02, double exTension)
        {
            if (Math.Abs(segment2d01.Direction.Normalized.Dot(segment2d02.Direction.Normalized)).EqualPrecision(1.0))// 判断是否平行
            {
                IntrSegment2Segment2 intr = new IntrSegment2Segment2(segment2d01.TwoWayExtension(exTension), segment2d02.TwoWayExtension(exTension));
                intr.Compute();
                if (intr.Quantity == 2)// 判断是否重叠
                {
                    return new Segment2d(intr.Point0, intr.Point1);
                }
            }
            return new Segment2d();
        }
        /// <summary>
        /// 获取两根线段的重叠部分 通过距离判断
        /// </summary>
        /// <returns></returns>
        public static Segment2d OverLapSeg_ByDis(this Segment2d segment2d01, Segment2d segment2d02, double exTension)
        {
            Vector2d p0 = segment2d01.P0;
            Vector2d p1 = segment2d01.P1;
            Vector2d p2 = segment2d02.P0;
            Vector2d p3 = segment2d02.P0;

            List<Vector2d> vs = new List<Vector2d>();
            if (segment2d01.DistanceSquared(p2) < 1e-3)
            {
                vs.Add(p2);
            }
            if (segment2d01.DistanceSquared(p3) < 1e-3)
            {
                vs.Add(p3);
            }
            if (segment2d02.DistanceSquared(p0) < 1e-3)
            {
                vs.Add(p0);
            }
            if (segment2d02.DistanceSquared(p1) < 1e-3)
            {
                vs.Add(p1);
            }

            if (vs.Count > 1)
            {
                vs = vs.DelDuplicate();
                if (vs.Count==2)
                {
                    return new Segment2d(vs[0], vs[1]);
                }
            }

            return new Segment2d();
        }
        /// <summary>
        /// 获取两根线段的重叠部分
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Segment2d> OverLapSegs(this IEnumerable<Segment2d> segment2ds01, IEnumerable<Segment2d> segment2ds02, double exTension)
        {
            foreach (var seg01 in segment2ds01)
            {
                foreach (var seg02 in segment2ds02)
                {
                    Segment2d overlapSeg = seg01.OverLapSeg(seg02, exTension);

                    if (overlapSeg.Length > Precision_.TheShortestDistance)
                    {
                        yield return overlapSeg;
                    }
                }
            }
        }
        /// <summary>
        /// 获取两根线段的重叠部分 通过距离判断
        /// </summary>
        /// <param name="segment2ds01"></param>
        /// <param name="segment2ds02"></param>
        /// <param name="exTension"></param>
        /// <returns></returns>
        public static IEnumerable<Segment2d> OverLapSegs_byDis(this IEnumerable<Segment2d> segment2ds01, IEnumerable<Segment2d> segment2ds02, double exTension)
        {
            foreach (var seg01 in segment2ds01)
            {
                foreach (var seg02 in segment2ds02)
                {
                    Segment2d overlapSeg = seg01.OverLapSeg_ByDis(seg02, exTension);

                    if (overlapSeg.Length > Precision_.TheShortestDistance)
                    {
                        yield return overlapSeg;
                    }
                }
            }
        }
        /// <summary>
        /// 求线段的相交关系
        /// </summary>
        /// <returns></returns>
        public static bool WheIntr(this Segment2d segment201, IEnumerable<Segment2d> segment2ds)
        {
            foreach (var item in segment2ds)
            {
                if (item.WheIntr(segment201))
                    return true;
            }
            return false;
        }
        public static bool WheIntr(this Segment2d segment201, Segment2d segment2d02)
        {
            IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment201, segment2d02);
            intrSegment2Segment2.Compute();
            if (intrSegment2Segment2.Quantity > 0) return true;
            else return false;
        }
        public static bool IsIn(this Segment2d segment2d01, List<Segment2d> segment2ds)
        {
            foreach (Segment2d segment2d in segment2ds)
            {
                if (segment2d01.IsSmae(segment2d))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 将两根线段倒角 已经是LV相交状态 g3中创建圆弧的起点与重点，需要满足是逆时针方向
        /// </summary>
        /// <param name="segment2d01"></param>
        /// <param name="segment2d02"></param>
        public static Arc2d Chamfer(this ref Segment2d segment2d01, ref Segment2d segment2d02, double radius)
        {
            double dot = segment2d01.Direction.Dot(segment2d02.Direction);// 平行线
            if (dot.EqualPrecision(1)) return null;

            Line2d line2d01 = segment2d01.ToLine2d();
            Line2d line2d02 = segment2d02.ToLine2d();
            IntrLine2Line2 intrLine2Line2 = new IntrLine2Line2(line2d01, line2d02);
            intrLine2Line2.Compute();
            Vector2d intersection = intrLine2Line2.Point;

            //【】遇到问题，求不出当前相交线段的交点
            //IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2d01, segment2d02);
            //intrSegment2Segment2.Compute();
            //if (intrSegment2Segment2.Quantity != 1) return null;
            //Vector2d intersection = intrSegment2Segment2.Point0;

            Vector2d vectro2d_start1 = segment2d01.P0;
            Vector2d vectro2d_end1 = segment2d01.P1;

            double distance01_01 = vectro2d_start1.DistanceSquared(intersection);
            double distance01_02 = vectro2d_end1.DistanceSquared(intersection);

            if (distance01_01 < distance01_02)
                segment2d01 = new Segment2d(intersection, vectro2d_end1);
            else
                segment2d01 = new Segment2d(intersection, vectro2d_start1);

            Vector2d vectro2d_start2 = segment2d02.P0;
            Vector2d vectro2d_end2 = segment2d02.P1;

            double distance02_01 = vectro2d_start2.DistanceSquared(intersection);
            double distance02_02 = vectro2d_end2.DistanceSquared(intersection);

            if (distance02_01 < distance02_02)
                segment2d02 = new Segment2d(intersection, vectro2d_end2);
            else
                segment2d02 = new Segment2d(intersection, vectro2d_start2);

            Vector2d direction01 = segment2d01.Direction;
            Vector2d direction02 = segment2d02.Direction;

            double angle = direction01.AngleR(direction02);
            if (angle > Math.PI)
                angle = Math.PI * 2 - angle;

            Vector2d point_arc_start = intersection + (direction01 * (radius / (Math.Tan(angle / 2))));//使用正切三角函数，求出，圆弧与倒角直线相切点的位置
            Vector2d point_arc_end = intersection + (direction02 * (radius / (Math.Tan(angle / 2))));
            Vector2d direction_radius = direction01 + direction02;
            Vector2d _point_arc_middle = intersection + (direction_radius.Normalized * (radius / Math.Sin(angle / 2)));
            //Vector2d point_arc_middle = intersection + (direction_radius.Normalized * (radius / Math.Sin(angle / 2) - radius));// 使用正弦三角函数，求出，圆弧与倒角直线相切点的位置

            segment2d01 = new Segment2d(point_arc_start, segment2d01.P1);
            segment2d02 = new Segment2d(point_arc_end, segment2d02.P1);

            //【】该处需要判断圆弧的起点与终点的先后顺序

            Arc2d arc2d = new Arc2d(_point_arc_middle, point_arc_start, point_arc_end);

            if (Math.Abs(arc2d.AngleStartDeg - arc2d.AngleEndDeg) >= 180)
                return new Arc2d(_point_arc_middle, point_arc_end, point_arc_start);
            else
                return arc2d;
        }
        /// <summary>
        /// 如果两根线段的端点重合次数为1，说明，两根线段的关系为 L V 一 三种形态
        /// </summary>
        /// <returns></returns>
        public static bool IsLVintersec(this Segment2d segment2d01, Segment2d segment2d02, out Vector2d vector2D)
        {
            int _intersectCount = 0;

            Vector2d vector2D1 = segment2d01.P0;
            Vector2d vector2D2 = segment2d01.P1;
            Vector2d vector2D3 = segment2d02.P0;
            Vector2d vector2D4 = segment2d02.P1;

            vector2D = vector2D1;

            if (vector2D1.Distance(vector2D3).EqualZreo())
            {
                _intersectCount += 1;
                vector2D = vector2D1;
            }
            if (vector2D2.Distance(vector2D3).EqualZreo())
            {
                _intersectCount += 1;
                vector2D = vector2D2;
            }
            if (vector2D1.Distance(vector2D4).EqualZreo())
            {
                _intersectCount += 1;
                vector2D = vector2D1;
            }
            if (vector2D2.Distance(vector2D4).EqualZreo())
            {
                _intersectCount += 1;
                vector2D = vector2D2;
            }

            if (_intersectCount == 1)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 将线段双向延长指定距离
        /// </summary>
        /// <returns></returns>
        public static Segment2d TwoWayExtension(this Segment2d seg, double distance)
        {
            Vector2d dir = seg.Direction;
            Vector2d p0 = seg.P0 - dir * distance;
            Vector2d p1 = seg.P1 + dir * distance;
            return new Segment2d(p0, p1);
        }

        public static Segment2d Move(this Segment2d _segment2d, Vector2d direction, double distance)
        {
            Vector2d p0 = _segment2d.P0;
            Vector2d p1 = _segment2d.P1;

            p0 = p0 + direction * distance;
            p1 = p1 + direction * distance;

            return new Segment2d(p0, p1);
        }
        public static IEnumerable<Segment2d> Rotate(this IEnumerable<Segment2d> _segment2ds, Vector2d origin, double angle)
        {
            foreach (var item in _segment2ds)
                yield return item.Rotate(origin, angle);
        }
        public static Segment2d Rotate(this Segment2d _segment2d, Vector2d origin, double angle)
        {
            if (angle == 0) return _segment2d;

            Vector2d p0 = _segment2d.P0;
            Vector2d p1 = _segment2d.P1;
            p0 = p0.Rotate(origin, angle);
            p1 = p1.Rotate(origin, angle);
            return new Segment2d(p0, p1);
        }
        public static IEnumerable<Segment2d> Mirror(this IEnumerable<Segment2d> _segment2ds, Vector2d origin, Vector2d direction)
        {
            foreach (var item in _segment2ds)
                yield return item.Mirror(origin, direction);
        }
        public static Segment2d Mirror(this Segment2d _segment2d, Vector2d origin, Vector2d direction)
        {
            Vector2d p0 = _segment2d.P0;
            Vector2d p1 = _segment2d.P1;

            p0 = p0.Mirror(origin, direction);
            p1 = p1.Mirror(origin, direction);

            return new Segment2d(p0, p1);
        }

    }
}
