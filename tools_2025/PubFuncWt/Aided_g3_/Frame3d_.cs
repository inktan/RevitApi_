using g3;
using goa.Common.g3InterOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Frame3d_
    {
        /// <summary>
        /// 构建Frame3d
        /// </summary>
        /// <returns></returns>
        public static Frame3d CreatFrame3d(this Vector2d o,Vector2d direction)
        {
            Vector3d origin = o.ToVector3d();
            Vector3d x = direction.Normalized.ToVector3d();
            Vector3d y = x.Rotate(new Vector3d(0, 0, 1), 90);
            Vector3d z = x.Cross(y).Normalized;// 叉积符合右手定则

            return new Frame3d(origin, x, y, z);
        }

        /// <summary>
        /// 将X轴反向 如何判断是第几次镜像x轴
        /// </summary>
        /// <returns></returns>
        public static Frame3d RreverseX(this Frame3d frame3d, double angleDeg = 90)
        {
            Vector3d origin = frame3d.Origin;
            Vector3d x = frame3d.X * (-1.0);
            Vector3d y = x.Rotate(new Vector3d(0, 0, 1), angleDeg);// 镜像关系需要明确
            Vector3d z = x.Cross(y).Normalized;// 叉积符合右手定则

            return new Frame3d(origin, x, y, z);
        }
        public static Frame3d MirrorY(this Frame3d frame3d, double scale)
        {
            Vector3d x = frame3d.X;
            Vector3d y = frame3d.Y * scale;
            Vector3d z = frame3d.Z;

            return new Frame3d(frame3d.Origin, x, y, z);
        }
        public static Frame3d MirrorZ(this Frame3d frame3d, double scale)
        {
            Vector3d x = frame3d.X;
            Vector3d y = frame3d.Y;
            Vector3d z = frame3d.Z * scale;

            return new Frame3d(frame3d.Origin, x, y, z);
        }
        public static IEnumerable<Polygon2d> ToFrame2dPoly(this Frame3d frame3d, IEnumerable<Polygon2d> polygon2ds)
        {
            foreach (var item in polygon2ds)
            {
                yield return frame3d.ToFrame2dPoly(item);
            }
        }
        /// <summary>
        /// 转到指定Frame 框架只是用新的坐标系来描述曾经的那个点，点的位置不曾移动，不过是新的坐标系有一套新的描述
        /// </summary>
        /// <returns></returns>
        public static Polygon2d ToFrame2dPoly(this Frame3d frame3d, Polygon2d o)
        {
            List<Vector2d> vec2ds = new List<Vector2d>();
            foreach (var item in o.Vertices)
            {
                Vector2d vec = frame3d.ToFrameP(item.ToVector3d()).ToVector2d();
                vec2ds.Add(vec);
            }
            return new Polygon2d(vec2ds);
        }
        public static IEnumerable<Polygon2d> FromFrame2dPoly(this Frame3d frame3d, IEnumerable<Polygon2d> polygon2ds)
        {
            foreach (var item in polygon2ds)
            {
                yield return frame3d.FromFrame2dPoly(item);
            }
        }
        /// <summary>
        /// 从指定Frame转到世界坐标系
        /// </summary>
        /// <returns></returns>
        public static Polygon2d FromFrame2dPoly(this Frame3d frame3d, Polygon2d o)
        {
            List<Vector2d> vec2ds = new List<Vector2d>();
            foreach (var item in o.Vertices)
            {
                Vector2d vec = frame3d.FromFrameP(item.ToVector3d()).ToVector2d();
                vec2ds.Add(vec);
            }
            return new Polygon2d(vec2ds);
        }
        /// <summary>
        /// 转到指定Frame
        /// </summary>
        /// <returns></returns>
        public static Segment2d ToFrame2dSeg(this Frame3d frame3d, Segment2d seg2d)
        {
            Vector2d vec00 = frame3d.ToFrameP(seg2d.P0.ToVector3d()).ToVector2d();
            Vector2d vec01 = frame3d.ToFrameP(seg2d.P1.ToVector3d()).ToVector2d();

            return new Segment2d(vec00, vec01);
        }
        public static IEnumerable<Segment2d> ToFrame2dSeg(this Frame3d frame3d, IEnumerable<Segment2d> seg2ds)
        {
            foreach (var item in seg2ds)
            {
                Vector2d vec00 = frame3d.ToFrameP(item.P0.ToVector3d()).ToVector2d();
                Vector2d vec01 = frame3d.ToFrameP(item.P1.ToVector3d()).ToVector2d();
                yield return new Segment2d(vec00, vec01);
            }
        }
        /// <summary>
        /// 从指定Frame转到世界坐标系
        /// </summary>
        /// <returns></returns>
        public static Segment2d FromFrame2dSeg(this Frame3d frame3d, Segment2d seg2d)
        {
            Vector2d vec00 = frame3d.FromFrameP(seg2d.P0.ToVector3d()).ToVector2d();
            Vector2d vec01 = frame3d.FromFrameP(seg2d.P1.ToVector3d()).ToVector2d();

            return new Segment2d(vec00, vec01);
        }
        public static IEnumerable<Segment2d> FromFrame2dSeg(this Frame3d frame3d, IEnumerable<Segment2d> seg2ds)
        {
            foreach (var item in seg2ds)
            {
                Vector2d vec00 = frame3d.FromFrameP(item.P0.ToVector3d()).ToVector2d();
                Vector2d vec01 = frame3d.FromFrameP(item.P1.ToVector3d()).ToVector2d();
                yield return new Segment2d(vec00, vec01);
            }
        }


        public static Frame3f ToFrame3f(this Frame3d frame3d)
        {
            return new Frame3f(frame3d.Origin.To3f(), frame3d.X.To3f(), frame3d.Y.To3f(), frame3d.Z.To3f());
        }
    }
}
