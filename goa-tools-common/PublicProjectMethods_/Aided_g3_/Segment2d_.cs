using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PublicProjectMethods_
{
    public class Segment2d_class
    {
        public Segment2d Segment2d { get; set; }
    }
    public static class Segment2d_
    {
        #region Segment2d

        public static bool IsIn(this Segment2d segment2d01, List<Segment2d> segment2ds)
        {
            foreach (Segment2d segment2d in segment2ds)
            {
                if (segment2d01.IsSmae(segment2d))
                    return true;
            }
            return false;
        }

        public static bool IsSmae(this Segment2d segment2d01, Segment2d segment2d02)
        {
            if (!segment2d01.Center.Distance(segment2d02.Center).EqualZreo())
                return false;
            if (!Math.Abs(segment2d01.Direction.Dot(segment2d02.Direction)).EqualPrecision(1))
                return false;
            if (!segment2d01.Extent.EqualPrecision(segment2d02.Extent))
                return false;

            return true;
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
            //Vector2d point_arc_middle = intersection + (direction_radius.Normalized * (radius / Math.Sin(angle / 2) - radius));// 使用正弦三角函数，求出，圆弧与倒角直线相切点的位置
            Vector2d _point_arc_middle = intersection + (direction_radius.Normalized * (radius / Math.Sin(angle / 2)));// 使用正弦三角函数，求出，圆弧与倒角直线相切点的位置

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
        /// 如果两根线段的交点为1，说明，两根线段的关系为 L V 一 三种形态
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
        public static Segment2d TwoWayExtension(this Segment2d _segment2d, double distance = 1.0)
        {
            Vector2d _center = _segment2d.Center;
            Vector2d _direction = _segment2d.Direction;
            double length = _segment2d.Length;
            Vector2d p0 = _center + _direction * (length / 2 + distance);
            Vector2d p1 = _center + -_direction * (length / 2 + distance);
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
        public static Segment2d Rotate(this Segment2d _segment2d, Vector2d origin, double angle)
        {
            if (angle == 0) return _segment2d;

            Vector2d p0 = _segment2d.P0;
            Vector2d p1 = _segment2d.P1;
            p0 = p0.Rotate(origin, angle);
            p1 = p1.Rotate(origin, angle);
            return new Segment2d(p0, p1);
        }
        public static Segment2d Mirror(this Segment2d _segment2d, Vector2d origin, Vector2d direction)
        {
            Vector2d p0 = _segment2d.P0;
            Vector2d p1 = _segment2d.P1;

            p0 = p0.Mirror(origin, direction);
            p1 = p1.Mirror(origin, direction);

            return new Segment2d(p0, p1);
        }
        /// <summary>
        /// segmentToLine
        /// </summary>
        public static Line2d ToLine2d(this Segment2d _segment2d)
        {
            return new Line2d(_segment2d.P0, _segment2d.Direction);
        }
        /// <summary>
        /// 判断一根线段与一个线圈的所有交点
        /// </summary>
        public static List<Vector2d> FindInterSectionPoint(this Segment2d _segment2d, Polygon2d _polygon2d)
        {
            List<Vector2d> vector2ds = new List<Vector2d>();
            foreach (Segment2d _seg2d in _polygon2d.SegmentItr())
            {
                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(_segment2d, _seg2d);
                intrSegment2Segment2.Find();
                if (intrSegment2Segment2.Quantity == 1)
                {
                    vector2ds.Add(intrSegment2Segment2.Point0);
                }
            }
            return vector2ds;
        }

        /// <summary>
        /// 判断线段两个端点是否落在polygon2d上，收集器中数量可能为0，1，2——允许点到线段有一段缓冲距离
        /// </summary>
        public static List<Vector2d> FindInterSectionPoint(this Segment2d _segment2d, Polygon2d _polygon2d, double _minDistance)
        {
            List<Vector2d> vector2ds = new List<Vector2d>();
            Vector2d p0 = _segment2d.P0;
            Vector2d p1 = _segment2d.P1;

            foreach (Segment2d _seg2d in _polygon2d.SegmentItr())
            {

                if (!vector2ds.Contains(p0))
                {
                    double distanceP0 = _seg2d.DistanceSquared(p0);
                    if (distanceP0 <= _minDistance * _minDistance)
                    {
                        vector2ds.Add(p0);
                    }
                }
                if (!vector2ds.Contains(p1))
                {
                    double distanceP1 = _seg2d.DistanceSquared(p1);
                    if (distanceP1 <= _minDistance * _minDistance)
                    {
                        vector2ds.Add(p1);
                    }
                }
                if (vector2ds.Contains(p0) && vector2ds.Contains(p1))
                    break;

            }
            return vector2ds;
        }
        #endregion

    }
}
