using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;

namespace PublicProjectMethods_
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public static class Polygon2d_
    {

        public static int AngleNum(this Polygon2d polygon2d, double tarAngle = 90)
        {
            int angleCount = 0;
            for (int i = 0; i < polygon2d.VertexCount; i++)
            {
                if (polygon2d.OpeningAngleDeg(i).EqualPrecision(tarAngle))
                {
                    angleCount += 1;
                }
            }
            return angleCount;
        }
        /// <summary>
        /// 线圈是否为三角形
        /// </summary>
        /// <param name="polygon2d"></param>
        /// <returns></returns>
        public static bool IsTriangle(this Polygon2d polygon2d)
        {
            int count = polygon2d.VertexCount;
            if (count != 3)
            {
                return false;
            }
            double angle = 0;
            for (int i = 0; i < polygon2d.VertexCount; i++)
            {
                angle += polygon2d.OpeningAngleDeg(i);
            }
            if (angle.EqualPrecision(180))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 线圈是否为直角梯形
        /// </summary>
        /// <returns></returns>
        public static bool IsTrapezoid_RightAngle(this Polygon2d polygon2d)
        {
            int count = polygon2d.VertexCount;
            if (count != 4)
            {
                return false;
            }
            int calAngleCounr = 0;
            for (int i = 0; i < polygon2d.VertexCount; i++)
            {
                if (polygon2d.OpeningAngleDeg(i).EqualPrecision(90))
                {
                    calAngleCounr += 1;
                }
            }
            if (calAngleCounr != 2)
            {
                return false;
            }
            IEnumerable<Segment2d> segment2ds = polygon2d.SegmentItr();
            int segmntCount = segment2ds.Count();
            for (int i = 0; i < segmntCount; i++)
            {
                for (int j = 0; j < segmntCount; j++)
                {
                    if (i <= j) continue;
                    Segment2d segment2d01 = segment2ds.ElementAt(i);
                    Segment2d segment2d02 = segment2ds.ElementAt(j);
                    if (segment2d01.Direction.Dot(segment2d02.Direction).EqualPrecision(-1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 线圈是否为梯形 ——原则：四个角点，一对平行边
        /// </summary>
        /// <returns></returns>
        public static bool IsTrapezoid(this Polygon2d polygon2d)
        {
            int count = polygon2d.VertexCount;
            if (count != 4)
            {
                return false;
            }
            IEnumerable<Segment2d> segment2ds = polygon2d.SegmentItr();

            int segmntCount = segment2ds.Count();
            int pedCount = 0;
            for (int i = 0; i < segmntCount; i++)
            {
                for (int j = 0; j < segmntCount; j++)
                {
                    if (i <= j) continue;
                    Segment2d segment2d01 = segment2ds.ElementAt(i);
                    Segment2d segment2d02 = segment2ds.ElementAt(j);
                    if (segment2d01.Direction.Dot(segment2d02.Direction).EqualPrecision(-1))
                    {
                        if (segment2d01.Length > segment2d02.Length)
                        {
                            pedCount += 1;
                        }
                    }
                }
            }
            if (pedCount == 1)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 线圈是否为矩形
        /// </summary>
        /// <returns></returns>
        public static bool IsRectangle(this Polygon2d polygon2d)
        {
            int count = polygon2d.VertexCount;
            if (count != 4)
            {
                return false;
            }
            for (int i = 0; i < polygon2d.VertexCount; i++)
            {
                if (!polygon2d.OpeningAngleDeg(i).EqualPrecision(90))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 旋转Polygon2d
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Polygon2d> Rotate(this IEnumerable<Polygon2d> polygon2ds, Vector2d origin, double angle)
        {
            foreach (var item in polygon2ds)
            {
                yield return item.Rotate(origin, angle);
            }
        }
        /// <summary>
        /// 旋转Polygon2d
        /// </summary>
        /// <returns></returns>
        public static Polygon2d Rotate(this Polygon2d polygon2d, Vector2d origin, double angle)
        {
            if (angle == 0) return polygon2d;
            Matrix2d rotationTransform = new Matrix2d(angle);
            Polygon2d _polygon2D = new Polygon2d(polygon2d);// 避免自身图形被修改
            return _polygon2D.Rotate(rotationTransform, origin);
        }
        /// <summary>
        /// 镜像 polygon
        /// </summary>
        public static IEnumerable<Polygon2d> Mirror(this IEnumerable<Polygon2d> polygon2ds, Vector2d origin, Vector2d direction)
        {
            foreach (var item in polygon2ds)
            {
                yield return item.Mirror(origin, direction);
            }
        }
        /// <summary>
        /// 镜像 polygon
        /// </summary>
        /// </summary>
        /// <returns></returns>
        public static Polygon2d Mirror(this Polygon2d polygon2D, Vector2d origin, Vector2d direction)
        {
            List<Vector2d> vertices = polygon2D.VerticesItr(false).ToList();
            int N = polygon2D.VertexCount;
            for (int i = 0; i < N; i++)
                vertices[i] = vertices[i].Mirror(origin, direction);
            return new Polygon2d(vertices);
        }
        /// <summary>
        /// 删除多边形的尖角 删除共线的点
        /// </summary>
        /// <returns></returns>
        public static Polygon2d RemoveSharpCcorners(this Polygon2d polygon2d, double AnglePRECISION = 1.0)
        {
            List<Vector2d> vector2ds = new List<Vector2d>();

            for (int i = 0; i < polygon2d.VertexCount; i++)
            {
                double openingAngle = polygon2d.OpeningAngleDeg(i);

                if (openingAngle >= AnglePRECISION && openingAngle <= 180 - AnglePRECISION)
                {
                    vector2ds.Add(polygon2d[i]);
                }
            }

            return new Polygon2d(vector2ds);
        }

        /// <summary>
        /// 去重——Revit中创建实体线的最短线段距离（the shortest distance 0.00256026455729167 foot * 304.8 = 0.7803686370624797 mm）
        /// </summary>
        /// <returns></returns>
        public static Polygon2d DelDuplicate(this Polygon2d polygon2d, double _precison = 1e-6)
        {
            return new Polygon2d(polygon2d.Vertices.DelDuplicate());
        }
        /// <summary>
        /// polygon向外偏移-创建新的线圈实例
        /// </summary>
        public static Polygon2d OutwardOffeet(this Polygon2d _polygon2D, double offsetDistane)
        {
            //Polygon2d polygon2d = new Polygon2d(_polygon2D.VerticesItr(false));
            //bool IsClockwise = polygon2d.IsClockwise;
            //if (IsClockwise)// 注意，此处与线圈向内偏移写法相反 // 该处为向内偏移，注意写法
            //{
            //    polygon2d.PolyOffset(-offsetDistane);
            //}
            //else
            //{
            //    polygon2d.PolyOffset(offsetDistane);
            //}
            //return polygon2d;
            Path path = _polygon2D.Vertices.ToPath();
            Paths paths = path.Offset(offsetDistane, Precision_.clipperMultiple, EndType.etClosedLine);
            return paths.ToPolygon2ds().OrderBy(p => p.Area).Last();
        }
        /// <summary>
        /// polygon向内偏移-创建新的线圈实例
        /// </summary>
        public static Polygon2d InwardOffeet(this Polygon2d _polygon2D, double offsetDistane)
        {
            //Polygon2d polygon2d = new Polygon2d(_polygon2D.VerticesItr(false));
            //bool IsClockwise = polygon2d.IsClockwise;
            //if (IsClockwise)// 注意，此处与线圈向内偏移写法相反 // 该处为向内偏移，注意写法
            //{
            //    polygon2d.PolyOffset(offsetDistane);
            //}
            //else
            //{
            //    polygon2d.PolyOffset(-offsetDistane);
            //}
            //return polygon2d;
            Path path = _polygon2D.Vertices.ToPath();
            Paths paths = path.Offset(offsetDistane, Precision_.clipperMultiple, EndType.etClosedLine);
            return paths.ToPolygon2ds().OrderBy(p => p.Area).First();
        }
        /// <summary>
        /// 对比两个polygon是否相同
        /// </summary>
        public static bool IsSame(this Polygon2d polygon2d01, Polygon2d polygon2d02)
        {
            List<Vector2d> vector2ds01 = polygon2d01.Vertices.ToList();
            List<Vector2d> vector2ds02 = polygon2d02.Vertices.ToList();

            return vector2ds01.IsSame(vector2ds02);
        }
        /// <summary>
        /// 找到线圈北侧的点
        /// </summary>
        /// <returns></returns>
        public static List<Vector2d> NourthVertices(this Polygon2d polygon2d)
        {
            if (polygon2d.IsClockwise)// 为true则是顺时针,
            {
                polygon2d.Reverse();// 强制所有线圈为逆时针方向
            }

            // 1 找到线圈点集的最左、最右，同时偏下的角点
            Vector2d leftUpPoint = polygon2d.MaxLeftMaxUpPoint();
            Vector2d rightUpPoint = polygon2d.MaxRightMaxUpPoint();

            // 2 基于点进行重新排序 排序后便于去点集的区间范围
            List<Vector2d> path_Vector2d = polygon2d.ReSortPolygon2dByPoint(rightUpPoint).Vertices.ToList();

            int leftUpIndex = path_Vector2d.IndexOf(leftUpPoint);
            int rightUpIndex = path_Vector2d.IndexOf(rightUpPoint);

            // 所有线圈遵循逆时针模式
            int length = leftUpIndex - rightUpIndex + 1;
            return path_Vector2d.GetRange(rightUpIndex, length);

        }
        /// <summary>
        /// 找到线圈南侧的点
        /// </summary>
        /// <returns></returns>
        public static List<Vector2d> SourthVertices(this Polygon2d polygon2d)
        {
            if (polygon2d.IsClockwise)// 为true则是顺时针,
            {
                polygon2d.Reverse();// 强制所有线圈为逆时针方向
            }

            // 1 找到线圈点集的最左、最右，同时偏下的角点
            Vector2d leftDownPoint = polygon2d.MaxLeftMaxDownPoint();
            Vector2d rightDownPoint = polygon2d.MaxRightMaxDownPoint();

            // 2 基于点进行重新排序
            List<Vector2d> path_Vector2d = polygon2d.ReSortPolygon2dByPoint(leftDownPoint).Vertices.ToList();

            int leftDownIndex = path_Vector2d.IndexOf(leftDownPoint);
            int rightDownIndex = path_Vector2d.IndexOf(rightDownPoint);

            int length = rightDownIndex - leftDownIndex + 1;
            return path_Vector2d.GetRange(leftDownIndex, length);

        }
        /// <summary>
        /// 求出线圈点集中最上最右的那个点
        /// </summary>
        /// <returns></returns>
        public static Vector2d MaxRightMaxUpPoint(this Polygon2d polygon2d)
        {
            List<Vector2d> vector2ds = polygon2d.Vertices.ToList();
            Vector2d maxUpVector2d = vector2ds.OrderBy(p => p.y).ToList().Last(); // 默认为升序，即为由小到大
            vector2ds = vector2ds.Where(p => p.y.EqualPrecision(maxUpVector2d.y)).ToList();
            return vector2ds.OrderBy(p => p.x).ToList().Last();
        }
        /// <summary>
        /// 求出线圈点集中最上最左的那个点
        /// </summary>
        /// <returns></returns>
        public static Vector2d MaxLeftMaxUpPoint(this Polygon2d polygon2d)
        {
            List<Vector2d> vector2ds = polygon2d.Vertices.ToList();
            Vector2d maUpVector2d = vector2ds.OrderBy(p => p.y).ToList().Last(); // 默认为升序，即为由小到大
            vector2ds = vector2ds.Where(p => p.y.EqualPrecision(maUpVector2d.y)).ToList();
            return vector2ds.OrderBy(p => p.x).ToList().First();
        }
        /// <summary>
        /// 求出线圈点集中最下最左的那个点
        /// </summary>
        /// <returns></returns>
        public static Vector2d MaxLeftMaxDownPoint(this Polygon2d polygon2d)
        {
            List<Vector2d> vector2ds = polygon2d.Vertices.ToList();
            Vector2d maxDownVector2d = vector2ds.OrderBy(p => p.y).ToList().First(); // 默认为升序，即为由小到大
            vector2ds = vector2ds.Where(p => p.y.EqualPrecision(maxDownVector2d.y)).ToList();
            return vector2ds.OrderBy(p => p.x).ToList().First();
        }
        /// <summary>
        /// 求出线圈点集中最下最右的那个点
        /// </summary>
        /// <param name="polygon2d"></param>
        /// <returns></returns>
        public static Vector2d MaxRightMaxDownPoint(this Polygon2d polygon2d)
        {
            List<Vector2d> vector2ds = polygon2d.Vertices.ToList();
            Vector2d maxDownVector2d = vector2ds.OrderBy(p => p.y).ToList().First(); // 默认为升序，即为由小到大
            vector2ds = vector2ds.Where(p => p.y.EqualPrecision(maxDownVector2d.y)).ToList();
            return vector2ds.OrderBy(p => p.x).ToList().Last();
        }
        /// <summary>
        /// 基于线圈上的一个点，对polygon2d的点集进行重新排序
        /// </summary>
        public static Polygon2d ReSortPolygon2dByPoint(this Polygon2d polygon2d, Vector2d _cp)
        {
            List<Vector2d> vector2ds = polygon2d.Vertices.ToList();
            vector2ds = vector2ds.ReSortPolygon2dByPoint(_cp);
            return new Polygon2d(vector2ds);
        }
        /// <summary>
        /// 线圈所在外包矩形 左下角点
        /// </summary>
        /// <returns></returns>
        public static Vector2d LUPointOfBoundingRectangle(this Polygon2d polygon2d)
        {
            return polygon2d.BoundingRectangle().Vertices[3];
        }
        /// <summary>
        /// 线圈所在外包矩形 左下角点
        /// </summary>
        /// <returns></returns>
        public static Vector2d RUPointOfBoundingRectangle(this Polygon2d polygon2d)
        {
            return polygon2d.BoundingRectangle().Vertices[2];
        }
        /// <summary>
        /// 线圈所在外包矩形 右下角点
        /// </summary>
        /// <returns></returns>
        public static Vector2d RDPointOfBoundingRectangle(this Polygon2d polygon2d)
        {
            return polygon2d.BoundingRectangle().Vertices[1];
        }
        /// <summary>
        /// 线圈所在外包矩形 左下角点
        /// </summary>
        /// <returns></returns>
        public static Vector2d LDPointOfBoundingRectangle(this Polygon2d polygon2d)
        {
            return polygon2d.BoundingRectangle().Vertices[0];
        }

        /// <summary>
        /// 线圈所在外包矩形
        /// </summary>
        /// <returns></returns>
        static public Polygon2d BoundingRectangle(this Polygon2d polygon2d)
        {
            //【】下方注释方法的时间需要进行测试
            //Box2d box2D = polygon2d.MinimalBoundingBox(double epsilon)

            double wight = 0;
            double height = 0;
            Vector2d center = polygon2d.Center(ref wight, ref height);

            return Polygon2d.MakeRectangle(center, wight, height);
        }
        /// <summary>
        /// 线圈所在外包矩形的形心
        /// </summary>
        /// <returns></returns>
        static public Vector2d Center(this Polygon2d polygon2d)
        {
            double wight = 0;
            double height = 0;
            return polygon2d.Center(ref wight, ref height);
        }
        /// <summary>
        /// 线圈所在外包矩形的高度
        /// </summary>
        /// <returns></returns>
        static public double Height(this Polygon2d polygon2d)
        {
            IEnumerable<Vector2d> vector2ds = polygon2d.Vertices;
            if (vector2ds.Count() < 3)
                return 0.0;
            vector2ds = vector2ds.OrderBy(p => p.y).ToList();
            Vector2d minYpoint = vector2ds.First();
            Vector2d maxYpoint = vector2ds.Last(); // 默认为升序，即为由小到大

            return maxYpoint.y - minYpoint.y;
        }
        /// <summary>
        /// 线圈所在外包矩形的宽度
        /// </summary>
        /// <returns></returns>
        static public double Width(this Polygon2d polygon2d)
        {
            IEnumerable<Vector2d> vector2ds = polygon2d.Vertices;
            if (vector2ds.Count() < 3)
                return 0.0;

            vector2ds = vector2ds.OrderBy(p => p.x).ToList();
            Vector2d minXpoint = vector2ds.First();
            Vector2d maxXpoint = vector2ds.Last();

            return maxXpoint.x - minXpoint.x;
        }
        /// <summary>
        /// 线圈所在外包矩形的形心
        /// </summary>
        /// <returns></returns>
        static public Vector2d Center(this Polygon2d polygon2d, ref double wight, ref double height)
        {
            IEnumerable<Vector2d> vector2ds = polygon2d.Vertices;
            return vector2ds.Center(ref wight, ref height);
        }
        /// <summary>
        /// 线圈所在外包矩形的形心
        /// </summary>
        /// <returns></returns>
        static public Vector2d Center(this IEnumerable<Vector2d> vector2ds, ref double wight, ref double height)
        {
            if (vector2ds.Count() < 3)
            {
                return new Vector2d(0, 0);
            }
            vector2ds = vector2ds.OrderBy(p => p.x).ToList();
            Vector2d minXpoint = vector2ds.First();
            Vector2d maxXpoint = vector2ds.Last();

            vector2ds = vector2ds.OrderBy(p => p.y).ToList();
            Vector2d minYpoint = vector2ds.First();
            Vector2d maxYpoint = vector2ds.Last(); // 默认为升序，即为由小到大

            double x_middle = (minXpoint.x + maxXpoint.x) / 2;
            double y_middle = (minYpoint.y + maxYpoint.y) / 2;
            wight = maxXpoint.x - minXpoint.x;
            height = maxYpoint.y - minYpoint.y;
            return new Vector2d(x_middle, y_middle);
        }
    }
}
