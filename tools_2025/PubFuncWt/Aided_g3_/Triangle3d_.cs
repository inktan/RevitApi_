using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{

    public static class Triangle3d_
    {
        /// <summary>
        /// 高程
        /// </summary>
        public static double zValue(this Triangle3d triangle3d)
        {
         return triangle3d.Center().z;
        }

        /// <summary>
        /// 坡向
        /// </summary>
        public static double aspectValue(this Triangle3d triangle3d)
        {
            Vector3d normal = triangle3d.NormalReverse();
            double y = normal.y;
            double x = normal.x;
            if (normal.y.EqualZreo())
            {
                y = 0;
            }
            if (normal.x.EqualZreo())
            {
                x = 0;
            }
            return MathUtil.Atan2Positive(y, x) * MathUtil.Rad2Deg;
        }

        /// <summary>
        /// 坡度
        /// </summary>
        public static double slopeValue(this Triangle3d triangle3d)
        {
            Vector3d normal = triangle3d.NormalReverse();
            Vector3d auxiliaryVector3d = new Vector3d(normal.x, normal.y, 0);// 水平投影
            //double rad = normal.Normalized.AngleR(auxiliaryVector3d.Normalized);
            double rad = auxiliaryVector3d.Normalized.AngleR(normal.Normalized);

            if (rad.EqualPrecision(Math.PI / 2))
            {
                return Math.PI / 2 * MathUtil.Rad2Deg;
            }
            else
            {
                return rad * MathUtil.Rad2Deg;
            }

        }


        public static AxisAlignedBox3d AxisAlignedBox3d(this Triangle3d triangle3d)
        {
            List<double> xs = new List<double>() { triangle3d.V0.x, triangle3d.V1.x, triangle3d.V2.x };
            List<double> ys = new List<double>() { triangle3d.V0.y, triangle3d.V1.y, triangle3d.V2.y };
            List<double> zs = new List<double>() { triangle3d.V0.z, triangle3d.V1.z, triangle3d.V2.z };

            return new AxisAlignedBox3d(triangle3d.Center(), (xs.Max() - xs.Min()) / 2, (ys.Max() - ys.Min()) / 2, (zs.Max() - zs.Min()) / 2);
        }

        public static PolyLine3d ToPolyLine3d(this Triangle3d triangle3d)
        {
            Vector3d[] vector3ds = new Vector3d[] { triangle3d.V0, triangle3d.V1, triangle3d.V2 };
            return new PolyLine3d(vector3ds);

        }
        public static Triangle3d Move(this Triangle3d triangle3d, Vector3d dis)
        {

            Vector3d v0 = triangle3d.V0.Move(dis);
            Vector3d v1 = triangle3d.V1.Move(dis);
            Vector3d v2 = triangle3d.V2.Move(dis);

            return new Triangle3d(v0, v1, v2);
        }
        /// <summary>
        /// Triangle3d的点集为逆时针时，Noraml默认指向Z轴负方向，该函数进行方向纠正
        /// 可以理解三角面的法向量默认指向为山体内部，因此这里做一次反向
        /// </summary>
        /// <param name="triangle3d"></param>
        /// <returns></returns>
        public static Vector3d NormalReverse(this Triangle3d triangle3d)
        {
            return triangle3d.Normal * (-1);
        }
        public static Vector3d Center(this Triangle3d triangle3d)
        {
            return (triangle3d.V0 + triangle3d.V1 + triangle3d.V2) / 3;
        }

        public static IEnumerable<Segment3d> SegmentItr(this Triangle3d triangle3d)
        {
            Vector3d[] vector3ds = new Vector3d[] { triangle3d.V0, triangle3d.V1, triangle3d.V2 };
            int count = vector3ds.Count();
            for (int i = 0; i < count; ++i)
                yield return new Segment3d(vector3ds[i], vector3ds[(i + 1) % count]);
        }

        public static Segment3d LongestSide(this Triangle3d triangle3d)
        {
            return triangle3d.SegmentItr().OrderBy(p => p.Length).Last();
        }

        public static List<Vector3d> Vertices(this Triangle3d triangle3d)
        {
            return new List<Vector3d>() { triangle3d.V0, triangle3d.V1, triangle3d.V2 };
        }

        /// <summary>
        /// 三角形最长边对应的面
        /// </summary>
        /// <param name="triangle3d"></param>
        /// <returns></returns>
        public static Vector3d VertexOppositeLongestSide(this Triangle3d triangle3d)
        {
            Segment3d segment3d = triangle3d.SegmentItr().OrderBy(p => p.Length).Last();

            return triangle3d.Vertices().OrderBy(p => segment3d.DistanceSquared(p)).Last();
        }

        /// <summary>
        /// 根据面积划分三角形面，划分方式，使用最长边的中线进行划分
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Triangle3d> DivideTriangleByMidline(this Triangle3d triangle3d, double minArea)
        {
            // 使用队列
            Queue<Triangle3d> triQueue = new Queue<Triangle3d>();
            triQueue.Enqueue(triangle3d);

            while (true)
            {
                if (triQueue.Count == 0) break;

                Triangle3d head = triQueue.Dequeue();

                if (head.Area > minArea)
                {
                    Segment3d segment3d = head.LongestSide();

                    Vector3d p0 = segment3d.P0;
                    Vector3d p1 = segment3d.P1;

                    Vector3d center = segment3d.Center;

                    Vector3d vertexOppositeLongestSide = triangle3d.Vertices().OrderBy(p => segment3d.DistanceSquared(p)).Last();

                    triQueue.Enqueue(new Triangle3d(p0, center, vertexOppositeLongestSide));
                    triQueue.Enqueue(new Triangle3d(center, p1, vertexOppositeLongestSide));
                }
                else
                {
                    yield return head;
                }
            }
        }

        // 



    }
}
