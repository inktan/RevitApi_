using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PubFuncWt
{
    public static class Vector2d_
    {
        public static Polygon2d MakeRectangle(this Vector2d center, double width, double height)
        {
            return Polygon2d.MakeRectangle(center, width, height);
        }
        /// <summary>
        /// 构建网格点
        /// </summary>
        /// <returns></returns>
        public static List<Vector2d> GetMatrixVerts(this Vector2d _min, double _xLength, double _yLength, double _xCellSize, double _yCellSize)
        {
            List<Vector2d> points = new List<Vector2d>();
            //计算X和Y方向的数量
            int xNum = (int)Math.Ceiling(_xLength / _xCellSize) + 1;
            int yNum = (int)Math.Ceiling(_yLength / _yCellSize) + 1;
            //得到矩阵

            Vector2d p = _min + new Vector2d(_xCellSize / 2, _yCellSize / 2);
            points.Add(p);

            for (int i = 0; i < xNum; i++)
            {
                for (int j = 0; j < yNum; j++)
                {
                    p += new Vector2d(0.0, _yCellSize);
                    points.Add(p);
                }
                p = new Vector2d(p.x, _min.y);
                p += new Vector2d(_xCellSize, 0.0);
                points.Add(p);
            }
            return points;
        }
        /// <summary>
        /// x轴正方向，逆时针旋转至目标向量，需要转的角度
        /// </summary>
        /// <returns></returns>
        public static double AngleRadToX(this Vector2d vector2d)
        {
            return MathUtil.Atan2Positive(vector2d.y, vector2d.x);
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

        /// <summary>
        /// 判断点在不在线段上
        /// </summary>
        /// <param name="p"></param>
        /// <param name="segment2d"></param>
        /// <returns></returns>
        public static bool OnSeg2d(this Vector2d p, Segment2d segment2d)
        {
            double t = (p - segment2d.Center).Dot(segment2d.Direction);
            if (t < segment2d.Extent && t > segment2d.Extent * (-1))
            {
                Vector2d proj = segment2d.Center + t * segment2d.Direction;

                if (proj.EpsilonEqual(p, MathUtil.Epsilonf))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 点落在线圈边界上
        /// </summary>
        public static bool OnPolygon2d(this Vector2d vector2d, Polygon2d polygon2d)
        {
            //【】判断点是否落在多边形线段上
            foreach (Segment2d segment2d in polygon2d.SegmentItr())
            {
                if (vector2d.OnSeg2d(segment2d))
                {
                    return true;
                }
            }
            return false;
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
        /// 去重——Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static List<Vector2d> DelDuplicate(this IEnumerable<Vector2d> vector2ds, double _precison = 3e-3)
        {
            List<Vector2d> result = new List<Vector2d>();

            foreach (Vector2d v01 in vector2ds)
            {
                bool isIn = true;
                foreach (Vector2d v02 in result)
                {
                    if (v01.EpsilonEqual(v02, _precison))
                    {
                        isIn = false;
                        break;
                    }
                }

                if (isIn)
                {
                    result.Add(v01);
                }
            }
            return result;
        }

        public static Vector3d ToVector3d(this Vector2d vector2d, double _z = 0.0)
        {
            return new Vector3d(vector2d.x, vector2d.y, _z);
        }
        public static Vector3f ToVector3f(this Vector2d vector2d, double _z = 0.0)
        {
            return new Vector3f(vector2d.x, vector2d.y, _z);
        }

    }





}
