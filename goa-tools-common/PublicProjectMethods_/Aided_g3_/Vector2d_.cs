using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PublicProjectMethods_
{
    public static class Vector2d_
    {

        /// <summary>
        /// 判断一个点是否为点集的边界点
        /// </summary>
        /// <param name="vector2ds"></param>
        /// <returns></returns>
        public static bool IsBoundaryVector2d(this Vector2d _vector2d, IEnumerable<Vector2d> vector2ds, double distance = 100.0)
        {
            //【】找到四象限
            List<Polygon2d> polygon2ds = _vector2d.GetFourQuadrants(distance);
            int count = 0;
            foreach (Polygon2d polygon2d in polygon2ds)
            {
                foreach (Vector2d vector2d in vector2ds)
                {
                    if (vector2d.InPolygon2d(polygon2d))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count < 4)
                return true;
            else
                return false;
        }
        #region 基于点创建四象限线圈
        public static List<Polygon2d> GetFourQuadrants(this Vector2d vector2d, double distance = 100.0)
        {
            Polygon2d polygon2d01 = GetFirstQuadrant(vector2d, distance);
            Polygon2d polygon2d02 = GetSecondQuadrant(vector2d, distance);
            Polygon2d polygon2d03 = GetThirdQuadrant(vector2d, distance);
            Polygon2d polygon2d04 = GetFourthQuadrant(vector2d, distance);

            return new List<Polygon2d>() { polygon2d01, polygon2d02, polygon2d03, polygon2d04 };
        }

        public static Polygon2d GetFirstQuadrant(this Vector2d vector2d, double distance)
        {
            Vector2d vector2d_RD = new Vector2d(distance, 0);
            Vector2d vector2d_RU = new Vector2d(distance, distance);
            Vector2d vector2d_LU = new Vector2d(0, distance);
            return new Polygon2d(new List<Vector2d>() { vector2d, vector2d + vector2d_RD, vector2d + vector2d_RU, vector2d + vector2d_LU });
        }

        public static Polygon2d GetSecondQuadrant(this Vector2d vector2d, double distance)
        {
            Vector2d vector2d_RU = new Vector2d(0, distance);
            Vector2d vector2d_LU = new Vector2d(-distance, distance);
            Vector2d vector2d_LD = new Vector2d(-distance, 0);

            return new Polygon2d(new List<Vector2d>() { vector2d, vector2d + vector2d_RU, vector2d + vector2d_LU, vector2d + vector2d_LD });
        }

        public static Polygon2d GetThirdQuadrant(this Vector2d vector2d, double distance)
        {
            Vector2d vector2d_LU = new Vector2d(-distance, 0);
            Vector2d vector2d_LD = new Vector2d(-distance, -distance);
            Vector2d vector2d_RD = new Vector2d(0, -distance);

            return new Polygon2d(new List<Vector2d>() { vector2d, vector2d + vector2d_LU, vector2d + vector2d_LD, vector2d + vector2d_RD });
        }

        public static Polygon2d GetFourthQuadrant(this Vector2d vector2d, double distance)
        {
            Vector2d vector2d_LD = new Vector2d(0, -distance);
            Vector2d vector2d_RD = new Vector2d(distance, -distance);
            Vector2d vector2d_RU = new Vector2d(distance, 0);

            return new Polygon2d(new List<Vector2d>() { vector2d, vector2d + vector2d_LD, vector2d + vector2d_RD, vector2d + vector2d_RU });
        }
        #endregion

        public static bool OnPolygon2d(this Vector2d vector2d, Polygon2d polygon2d)
        {
            return polygon2d.Contains(vector2d);
        }
        public static bool InPolygon2d(this Vector2d vector2d, Polygon2d polygon2d)
        {
            //【】判断点是否落在多边形线段上
            foreach (Segment2d segment2d in polygon2d.SegmentItr())
            {
                double distance = segment2d.DistanceSquared(vector2d);
                if (distance.EqualZreo())
                {
                    return false;
                }
            }
            return polygon2d.Contains(vector2d);
        }
        public static Vector2d Rotate(this Vector2d _vector2d, Vector2d origin, double angle)
        {
            if (angle == 0) return _vector2d;

            Matrix2d rotationTransform = new Matrix2d(angle);
            return rotationTransform * (_vector2d - origin) + origin;
        }

        /// <summary>
        /// 计算一个点到直线的垂直距离 
        /// </summary>
        public static double PedalDistanceToLine(this Vector2d tarVector2d, Vector2d origin, Vector2d _direction)
        {
            Line2d line2D = new Line2d(origin, _direction);
            double dis = line2D.DistanceSquared(tarVector2d);
            return (double)Math.Sqrt(dis);
        }
        /// <summary>
        /// 计算一个点到直线的垂直距离 
        /// </summary>
        public static double CalcPedalDistanceToLine(this Vector2d tarVector2d, Line2d line2D)
        {
            double dis = line2D.DistanceSquared(tarVector2d);
            return (double)Math.Sqrt(dis);
        }
        /// <summary>
        /// 计算vector2d到线段的垂线，如果垂足落在线段的延长线上，则垂线为目标点与线段端点的连线
        /// </summary>
        public static Segment2d PedalSegment2d(this Vector2d p, Segment2d segment2d)
        {
            Vector2d P0 = segment2d.P0;
            Vector2d P1 = segment2d.P1;
            Vector2d center = segment2d.Center;
            Vector2d direcion = segment2d.Direction;
            double Extent = segment2d.Length / 2;

            double t = (p - center).Dot(direcion);
            if (t >= Extent)// 线段距离的一半
                return new Segment2d(p, P1);
            if (t <= -Extent)// 线段距离的一半
                return new Segment2d(p, P0);
            return new Segment2d(p, center + t * direcion);
        }
        /// <summary>
        /// 计算vector2d到直线的垂线，该方式基于向量方式
        /// </summary>
        public static Segment2d PedalLine(this Vector2d tarVector2d, Vector2d origin, Vector2d _direction)
        {
            Vector2d pedal = tarVector2d.Pedal(origin, _direction);
            return new Segment2d(tarVector2d, pedal);
        }
        /// <summary>
        /// vector2d到直线的垂线
        /// </summary>
        public static Segment2d PedalLine(this Vector2d tarVector2d, Line2d line2D)
        {
            return tarVector2d.PedalLine(line2D.p0, line2D.Direction);
        }
        /// <summary>
        /// 镜像点
        /// </summary>
        public static Vector2d Mirror(this Vector2d tarVector2d, Vector2d origin, Vector2d direction)
        {
            Vector2d pedal = tarVector2d.Pedal(origin, direction);
            return new Vector2d(2 * pedal.x - tarVector2d.x, 2 * pedal.y - tarVector2d.y);
        }
        /// <summary>
        /// 镜像点
        /// </summary>
        public static Vector2d Mirror(this Vector2d tarVector2d, Line2d line2d)
        {
            Vector2d origin = line2d.p0;
            Vector2d direction = line2d.Direction;
            Vector2d pedal = tarVector2d.Pedal(origin, direction);
            return new Vector2d(2 * pedal.x - tarVector2d.x, 2 * pedal.y - tarVector2d.y);
        }

        /// <summary>
        /// vector2d到直线的垂足
        /// </summary>
        public static Vector2d Pedal(this Vector2d tarVector2d, Vector2d origin, Vector2d _direction)
        {
            Vector2d direction = _direction.Normalized;
            double t = (tarVector2d - origin).Dot(direction);
            return origin + t * direction;
        }
        /// <summary>
        /// 计算vector2d到直线的垂足，该方式基于向量方式
        /// </summary>
        public static Vector2d CalcPedal(this Vector2d tarVector2d, Line2d line2d)
        {
            Vector2d origin = line2d.p0;
            Vector2d direction = line2d.Direction;
            double t = (tarVector2d - origin).Dot(direction);
            return origin + t * direction;
        }
        /// <summary>
        /// Vector2d 是否在 Vector2ds——点是否与点集中的某点距离为零
        /// </summary>
        /// <returns></returns>
        public static bool InVector2ds(this Vector2d vector2d, IEnumerable<Vector2d> vector2Ds, double _precison = 1e-6)
        {
            bool isIn = false;

            foreach (Vector2d vector2D1 in vector2Ds)
            {
                double _distance = vector2d.DistanceSquared(vector2D1);
                if (_distance <= Math.Pow(_precison, 2))
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        /// <summary>
        /// 去重——Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Vector2d> DelDuplicate(this IEnumerable<Vector2d> vector2ds, double _precison = 1e-6)
        {
            List<Vector2d> _vector2ds = new List<Vector2d>();

            foreach (Vector2d vector2d in vector2ds)
            {
                if (!vector2d.InVector2ds(_vector2ds, _precison))
                {
                    _vector2ds.Add(vector2d);
                }
            }
            return _vector2ds;
        }
        /// <summary>
        /// 判断一个 Path_Vector2d 在不在 Paths_Vector2d
        /// </summary>
        public static bool InVector2dses(this List<Vector2d> vector2ds, List<List<Vector2d>> vector2dses)
        {
            bool isIn = false;
            foreach (List<Vector2d> _vector2ds in vector2dses)
            {
                if (vector2ds.IsSame(_vector2ds))
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }

        /// <summary>
        /// 对比两个 List Vector2d 是否相同
        /// </summary>
        public static bool IsSame(this List<Vector2d> vector2ds01, List<Vector2d> vector2ds02)
        {
            Vector2d _cp01 = vector2ds01[0];
            Vector2d _cp02 = vector2ds02[0];

            vector2ds01 = vector2ds01.ReSortPolygon2dByPoint(_cp01);
            vector2ds02 = vector2ds02.ReSortPolygon2dByPoint(_cp02);

            int count01 = vector2ds01.Count;
            int count02 = vector2ds02.Count;
            if (count01 == count02)
            {
                for (int i = 0; i < count01; i++)
                {
                    if (vector2ds01[i].DistanceSquared(vector2ds02[i]) < Math.Pow(Precision_.TheShortestDistance, 2))
                        continue;
                    else
                        return false;
                }
            }
            else
                return false;

            return true;
        }
        /// <summary>
        /// 对 List Vector2d重新排序
        /// </summary>
        public static List<Vector2d> ReSortPolygon2dByPoint(this List<Vector2d> vector2ds01, Vector2d _cp)
        {
            int index = vector2ds01.IndexOf(_cp);
            int count = vector2ds01.Count;
            List<Vector2d> vector2Ds = new List<Vector2d>();
            for (int i = index; i < count; i++)
            {
                vector2Ds.Add(vector2ds01[i]);
            }
            for (int i = 0; i < index; i++)
            {
                vector2Ds.Add(vector2ds01[i]);
            }
            return vector2Ds;
        }
    }





}
