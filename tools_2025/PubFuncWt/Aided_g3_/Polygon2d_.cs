using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;
using goa.Common.g3InterOp;

namespace PubFuncWt
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 判断一个polygon2d的底边相邻的最近左右两个边是否为 Y 轴方向
    /// </summary>
    public enum LorRisRight : int
    {
        None,
        L,
        R,
        LR
    }
    public static class Polygon2d_
    {
        #region Mesh

        public static List<Triangle3d> Triangle3ds(this Polygon2d _o)
        {
            return _o.ToSimpleMesh().Triangle3ds().ToList();
        }
        public static IEnumerable<Triangle3d> Triangle3dsByRectangle(this Polygon2d _o, double _z = 0.0)
        {
            yield return new Triangle3d(_o.Vertices[0].ToVector3d(_z), _o.Vertices[1].ToVector3d(_z), _o.Vertices[2].ToVector3d(_z));
            yield return new Triangle3d(_o.Vertices[2].ToVector3d(_z), _o.Vertices[3].ToVector3d(_z), _o.Vertices[0].ToVector3d(_z));
        }

        public static DMesh3 ToDMesh3(this Polygon2d _o)
        {
            TriangulatedPolygonGenerator triangulatedPolygonGenerator = new TriangulatedPolygonGenerator();
            GeneralPolygon2d _polygon2d = new GeneralPolygon2d(_o);
            triangulatedPolygonGenerator.Polygon = _polygon2d;
            MeshGenerator meshGenerator = triangulatedPolygonGenerator.Generate();
            return meshGenerator.MakeDMesh();
        }

        public static NTMesh3 ToNTMesh3(this Polygon2d _o)
        {
            TriangulatedPolygonGenerator triangulatedPolygonGenerator = new TriangulatedPolygonGenerator();
            GeneralPolygon2d _polygon2d = new GeneralPolygon2d(_o);
            triangulatedPolygonGenerator.Polygon = _polygon2d;
            MeshGenerator meshGenerator = triangulatedPolygonGenerator.Generate();
            return meshGenerator.MakeNTMesh();
        }
        public static SimpleMesh ToSimpleMesh(this Polygon2d _o)
        {
            TriangulatedPolygonGenerator triangulatedPolygonGenerator = new TriangulatedPolygonGenerator();
            GeneralPolygon2d _polygon2d = new GeneralPolygon2d(_o);
            triangulatedPolygonGenerator.Polygon = _polygon2d;
            MeshGenerator meshGenerator = triangulatedPolygonGenerator.Generate();
            return meshGenerator.MakeSimpleMesh();
        }


        #endregion

        #region Clipper

        /// <summary>
        /// polygon向外偏移-创建新的线圈实例
        /// </summary>
        public static Polygon2d OutwardOffeet(this Polygon2d _polygon2d, double offsetDistane)
        {
            if (offsetDistane < Precision_.TheShortestDistance)
            {
                return _polygon2d;
            }

            Path path = _polygon2d.Vertices.ToPath();
            Paths paths = path.Offset(offsetDistane, Precision_.ClipperMultiple, EndType.etClosedLine);

            return paths.ToPolygon2ds().OrderBy(p => p.Area).Last();
        }
        public static IEnumerable<Polygon2d> OutwardOffeet(this IEnumerable<Polygon2d> _polygon2ds, double offsetDistane)
        {
            foreach (var item in _polygon2ds)
            {
                yield return item.OutwardOffeet(offsetDistane);
            }
        }
        /// <summary>
        /// polygon向内偏移-创建新的线圈实例--clipper的offset是双边偏移
        /// </summary>
        public static Polygon2d InwardOffeet(this Polygon2d _polygon2D, double offsetDistane)
        {
            if (offsetDistane < Precision_.TheShortestDistance)
            {
                return _polygon2D;
            }

            Path path = _polygon2D.Vertices.ToPath();
            Paths paths = path.Offset(offsetDistane, Precision_.ClipperMultiple, EndType.etClosedLine);
            return paths.ToPolygon2ds().OrderBy(p => p.Area).First();
        }
        public static IEnumerable<Polygon2d> InwardOffeet(this IEnumerable<Polygon2d> _polygon2ds, double offsetDistane)
        {
            foreach (var item in _polygon2ds)
            {
                yield return item.InwardOffeet(offsetDistane);
            }
        }
        public static IEnumerable<Polygon2d> UnionClipper(this IEnumerable<Polygon2d> oes)
        {
            Paths result = oes.Select(p => p.OutwardOffeet(Precision_.TheShortestDistance)).ToPaths().UnionClip();

            if (result.Count > 0)
            {
                foreach (var _nextItem in result)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        public static IEnumerable<Polygon2d> UnionClipper(this IEnumerable<Polygon2d> o1, IEnumerable<Polygon2d> o2)
        {
            Paths result = o1.ToPaths().UnionClip(o2.ToPaths());

            if (result.Count > 0)
            {
                foreach (var _nextItem in result)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        public static IEnumerable<Polygon2d> UnionClipper(this Polygon2d o1, IEnumerable<Polygon2d> o2)
        {
            Paths paths01 = new Paths() { o1.ToPath() };

            Paths result = paths01.UnionClip(o2.ToPaths());

            if (result.Count > 0)
            {
                foreach (var _nextItem in result)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        public static IEnumerable<Polygon2d> UnionClipper(this Polygon2d o1, Polygon2d o2)
        {
            Paths paths01 = new Paths() { o1.ToPath() };
            Paths paths02 = new Paths() { o2.ToPath() };

            Paths result = paths01.UnionClip(paths02);

            if (result.Count > 0)
            {
                foreach (var _nextItem in result)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        public static IEnumerable<Polygon2d> DifferenceClipper(this IEnumerable<Polygon2d> sub, Polygon2d clipper)
        {
            if (clipper.VertexCount < 3 || clipper == null)
            {
                foreach (var item in sub)
                {
                    yield return item;
                }
                yield break;
            }

            //【】subjs
            if (sub.Count() < 1)
            {
                foreach (var item in sub)
                {
                    yield return item;
                }
                yield break;
            }
            Paths subPaths = sub.ToPaths();
            //subPaths = subPaths.UnionClip(new Paths() { subPaths.First() });// 这里不可以union，要保证孔洞的嵌套关系
            //【】将裁剪区域放大一个极小数，保证Clipper完整裁剪——Q:代码浮点数计算会产生误差值1e-6

            Paths clipPaths = new Paths();
            clipPaths.Add(clipper.ToPath());

            //Paths paths01 = new Paths() { clipPaths.First() };
            //Paths union = paths01.UnionClip(clipPaths);// 这里不可以union，要保证孔洞的嵌套关系

            Paths nextPlacedRegions = subPaths.DifferenceClip(clipPaths);

            if (nextPlacedRegions.Count > 0)
            {
                foreach (var _nextItem in nextPlacedRegions)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        public static IEnumerable<Polygon2d> DifferenceClipper(this IEnumerable<Polygon2d> sub, IEnumerable<Polygon2d> clipper)
        {
            if (clipper.Count() < 1)
            {
                foreach (var item in sub)
                {
                    yield return item;
                }
                yield break;
            }

            //【】subjs
            if (sub.Count() < 1)
            {
                foreach (var item in sub)
                {
                    yield return item;
                }
                yield break;
            }
            Paths subPaths = sub.ToPaths();
            //subPaths = subPaths.UnionClip(new Paths() { subPaths.First() });
            //【】将裁剪区域放大一个极小数，保证Clipper完整裁剪——Q:代码浮点数计算会产生误差值1e-6

            Paths clipPaths = new Paths();
            foreach (var item in clipper)
            {
                Path nowRectangleIntPoints = item.ToPath();
                clipPaths.Add(nowRectangleIntPoints);
            }

            // 先把裁剪子集做一个union
            //Paths paths01 = new Paths() { clipPaths.First() };
            //Paths union = paths01.UnionClip(clipPaths);

            Paths nextPlacedRegions = subPaths.DifferenceClip(clipPaths);

            if (nextPlacedRegions.Count > 0)
            {
                foreach (var _nextItem in nextPlacedRegions)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        public static IEnumerable<Polygon2d> DifferenceClipper(this Polygon2d polygon2d01, IEnumerable<Polygon2d> polygon2d02)
        {
            if (polygon2d02.Count() < 1)
            {
                yield return polygon2d01;
                yield break;
            }

            //【】subjs
            Path path = polygon2d01.ToPath();
            Paths subPaths = new Paths() { path };
            //【】将裁剪区域放大一个极小数，保证Clipper完整裁剪——Q:代码浮点数计算会产生误差值1e-6

            Paths clipPaths = new Paths();
            foreach (var item in polygon2d02)
            {
                Path nowRectangleIntPoints = item.ToPath();
                clipPaths.Add(nowRectangleIntPoints);
            }

            // 先把裁剪子集做一个union
            //Paths paths01 = new Paths() { clipPaths.First() };
            //Paths union = paths01.UnionClip(clipPaths);

            Paths nextPlacedRegions = subPaths.DifferenceClip(clipPaths);

            if (nextPlacedRegions.Count > 0)
            {
                foreach (var _nextItem in nextPlacedRegions)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角

                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0.01)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }

                }
            }
        }
        /// <summary>
        /// 两个polygon相减
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Polygon2d> DifferenceClipper(this Polygon2d polygon2d01, Polygon2d polygon2d02)
        {
            //【】subjs
            Path path = polygon2d01.ToPath();
            Paths subPaths = new Paths() { path };
            //【】将裁剪区域放大一个极小数，保证Clipper完整裁剪——Q:代码浮点数计算会产生误差值1e-6

            Path nowRectangleIntPoints = polygon2d02.ToPath();
            Paths clipPaths = new Paths() { nowRectangleIntPoints };
            Paths nextPlacedRegions = subPaths.DifferenceClip(clipPaths);

            if (nextPlacedRegions.Count > 0)
            {
                foreach (var _nextItem in nextPlacedRegions)
                {
                    Polygon2d next_polygon2d = _nextItem.ToPolygon2d();// 经过clipper后的点需要去除尖角
                    next_polygon2d = next_polygon2d.DelDuplicate();
                    if (next_polygon2d.VertexCount < 3)
                    {
                        continue;
                    }
                    if (next_polygon2d.Area > 0)// 15 平方米=161.4586563 平方英尺 一个停车位的面积 2.5*6 = 15㎡
                    {
                        yield return next_polygon2d;
                    }
                }
            }
        }
        /// <summary>
        /// 两个线圈的相交并集区域面积
        /// </summary>
        /// <returns></returns>
        public static double IntersectionArea(this Polygon2d poly01, Polygon2d poly02)
        {
            Paths interSectPahts = new Paths() { poly01.ToPath() }.IntersectionClip(new Paths() { poly02.ToPath() });//【】 Clipper 结束
            double area = 0;
            interSectPahts.ForEach(p => { area += p.ToPolygon2d().Area; });
            return area;
        }
        /// <summary>
        /// 两个线圈的相交并集区域
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Polygon2d> Intersection(this Polygon2d poly01, Polygon2d poly02)
        {
            Paths interSectPahts = new Paths() { poly01.ToPath() }.IntersectionClip(new Paths() { poly02.ToPath() });//【】 Clipper 结束
            return interSectPahts.Select(p => p.ToPolygon2d()).Where(p => p.Area > 0);
        }
        /// <summary>
        /// 线圈的相交并集区域
        /// </summary>
        /// <returns></returns>
        public static List<Polygon2d> Intersection(this Polygon2d poly01, IEnumerable<Polygon2d> polys)
        {
            // 如果子集为 0 ，则相交结果为 0 
            if (polys.Count() < 1)
            {
                return new List<Polygon2d>() { };
            }
            // 先把裁剪子集做一个union
            Paths paths01 = new Paths() { polys.First().ToPath() };
            Paths union = paths01.UnionClip(polys.Select(p => p.ToPath()).ToList());

            Paths interSectPahts = new Paths() { poly01.ToPath() }.IntersectionClip(union);//【】 Clipper 结束
            return interSectPahts.Select(p => p.ToPolygon2d()).Where(p => p.Area > 0).ToList();
        }
        /// <summary>
        /// 线圈的相交并集区域
        /// </summary>
        /// <returns></returns>
        public static List<Polygon2d> Intersection(this IEnumerable<Polygon2d> polys, Polygon2d poly)
        {
            Paths interSectPahts = polys.ToPaths().IntersectionClip(new Paths() { poly.ToPath() });//【】 Clipper 结束
            return interSectPahts.Select(p => p.ToPolygon2d()).Where(p => p.Area > 0).ToList();
        }
        /// <summary>
        /// 线圈的相交并集区域
        /// </summary>
        /// <returns></returns>
        public static List<Polygon2d> Intersection(this IEnumerable<Polygon2d> polys01, IEnumerable<Polygon2d> polys02)
        {
            // 如果子集为 0 ，则相交结果为 0 
            if (polys02.Count() < 1)
            {
                return new List<Polygon2d>() { };
            }
            // 先把裁剪子集做一个union
            Paths paths01 = new Paths() { polys02.First().ToPath() };
            Paths union = paths01.UnionClip(polys02.ToPaths());

            Paths interSectPahts = polys01.ToPaths().IntersectionClip(union);//【】 Clipper 结束
            return interSectPahts.Select(p => p.ToPolygon2d()).Where(p => p.Area > 0).ToList();
        }
        #endregion

        #region InterSection

        public static bool Intersects(this Polygon2d o, IEnumerable<Segment2d> segs)
        {
            foreach (Segment2d seg in segs)
            {
                if (o.Intersects(seg))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool Intersects(this Polygon2d o, IEnumerable<Polygon2d> os)
        {
            foreach (Polygon2d _polygon2d in os)
            {
                if (o.Intersects(_polygon2d))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断一根线段延长一段距离后，与一个线圈的所有交点，延长距离可为0
        /// </summary>
        public static List<Vector2d> FindInterSectionPoint(this Polygon2d o, Segment2d seg, double minDis = 1.0)
        {
            seg = seg.TwoWayExtension(minDis);
            List<Vector2d> vector2ds = new List<Vector2d>();
            foreach (Segment2d _seg2d in o.SegmentItr())
            {
                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(seg, _seg2d);
                intrSegment2Segment2.Find();
                if (intrSegment2Segment2.Quantity == 1)
                {
                    vector2ds.Add(intrSegment2Segment2.Point0);
                }
            }
            return vector2ds;
        }

        #endregion

        #region Plane shape judgment
        /// <summary>
        /// 判断一个 Polygon2d 中底边相邻最近的两竖向边的方向是否为 Y 轴
        /// </summary>
        public static LorRisRight JudgRightEdge(this Polygon2d _polygon2d)
        {
            // 这么写可以求出线段的索引值 勿更改
            Vector2d lowestP = _polygon2d.SegmentItr().Select(p => p.Center).OrderBy(p => p.y).First();// 最低点的线段
            List<Segment2d> segs = _polygon2d.SegmentItr().ToList();

            int bottomSegIndex = 0;// 找到最低点线段的索引值
            for (int i = 0; i < segs.Count; i++)
            {
                if (lowestP.y.EqualPrecision(segs[bottomSegIndex].Center.y))// 求出落在 X轴 上的线段
                {
                    break;
                }
                bottomSegIndex++;
            }

            // 左相邻竖向边界
            Segment2d leftSeg = segs[bottomSegIndex];
            if (bottomSegIndex == 0)
                leftSeg = segs.Last();
            else
                leftSeg = segs[bottomSegIndex - 1];

            // 右相邻竖向边界
            Segment2d rightSeg = segs[(bottomSegIndex + 1) % segs.Count];

            // 直接判断左边界是否为竖线
            if (Math.Abs(leftSeg.Direction.y).EqualZreo() && !Math.Abs(rightSeg.Direction.y).EqualZreo())
            {
                return LorRisRight.L;
            }
            else if (!Math.Abs(leftSeg.Direction.y).EqualZreo() && Math.Abs(rightSeg.Direction.y).EqualZreo())
            {
                return LorRisRight.R;
            }
            else
            {
                return LorRisRight.LR;
            }
            // =====>


            // 底边界
            Segment2d bottomSeg = segs[bottomSegIndex];
            double dotL = leftSeg.Direction.Dot(bottomSeg.Direction);
            double dotR = rightSeg.Direction.Dot(bottomSeg.Direction);

            if (!dotL.EqualZreo() && dotR.EqualZreo())
            {
                return LorRisRight.R;
            }
            else if (dotL.EqualZreo() && !dotR.EqualZreo())
            {
                return LorRisRight.L;
            }
            else
            {
                return LorRisRight.LR;
            }
        }
        /// <summary>
        /// 求一个polygon2d的直角边
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Segment2d> AngleSeg(this Polygon2d _polygon2d)
        {
            // 找到所有的直角边
            List<Segment2d> segment2ds = _polygon2d.SegmentItr().ToList();
            int count = segment2ds.Count;
            for (int i = 0; i < count; i++)
            {
                if (segment2ds[i].Direction.Dot(segment2ds[(i + 1) % count].Direction).EqualZreo())
                    yield return segment2ds[i];
            }
        }

        /// <summary>
        /// 求一个polygon2d的直角顶点数量
        /// </summary>
        /// <returns></returns>
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
            for (int i = 0; i <4; i++)
            {
                if (!polygon2d.OpeningAngleDeg(i).EqualPrecision(90))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsLine(this Polygon2d polygon2d)
        {
            if (polygon2d.VertexCount > 2)
            {
                if (polygon2d.Area.EqualZreo())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        public static bool Is90DegPolygon(this Polygon2d polygon2d)
        {
            for (int i = 0; i < polygon2d.VertexCount; i++)
            {
                if (!polygon2d.OpeningAngleDeg(i).EqualPrecision(90))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region transform       

        public static Polygon2d Move(this Polygon2d polygon2d, Vector2d direction, double distance)
        {
            List<Vector2d> vector2ds = polygon2d.Vertices.ToList();

            for (int i = 0; i < vector2ds.Count; i++)
            {
                vector2ds[i] = vector2ds[i] + direction * distance;
            }
            return new Polygon2d(vector2ds);

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
        #endregion

        #region Reconstruction coil
        /// <summary>
        /// 将线圈处理为工字型 或者 倒 U 字型 可以输入系数
        /// 地库强排插件使用
        /// </summary>
        /// <returns></returns>
        public static List<Polygon2d> IshapedUshaped(this Polygon2d o, double parkSpaceHeight, double _N, double _S, double _L, double _R)
        {
            double height = o.Height();
            double width = o.Width();

            Vector2d ld = o.LDpOfBox2d();
            Vector2d rd = o.RDpOfBox2d();
            Vector2d lu = o.LUpOfBox2d();
            Vector2d ru = o.RUpOfBox2d();

            // 处理为倒 U 字型
            List<Polygon2d> polygon2ds = new List<Polygon2d>();
            if (_L > 0.01)
            {
                Vector2d middleLeftPoint = (lu + ld) / 2;
                Segment2d seg2dLeft = new Segment2d(middleLeftPoint + new Vector2d(parkSpaceHeight, 0), middleLeftPoint - new Vector2d(parkSpaceHeight, 0));
                Polygon2d clipperLeft = seg2dLeft.ToRenct2d(height * _L);
                polygon2ds.Add(clipperLeft);
            }
            if (_R > 0.01)
            {
                Vector2d middleRightoint = (ru + rd) / 2;
                Segment2d seg2dRight = new Segment2d(middleRightoint + new Vector2d(parkSpaceHeight, 0), middleRightoint - new Vector2d(parkSpaceHeight, 0));
                Polygon2d clipperRight = seg2dRight.ToRenct2d(height * _R);
                polygon2ds.Add(clipperRight);
            }

            if (height <= parkSpaceHeight)
            {
            
            }
            else if (height < parkSpaceHeight * 2 && height > parkSpaceHeight)
            {
                // 处理为倒 U 字型
                if (_S > 0.01)
                {
                    Vector2d middleDowmPoint = (ld + rd) / 2;
                    Segment2d seg2dDown = new Segment2d(middleDowmPoint + new Vector2d(0, parkSpaceHeight), middleDowmPoint - new Vector2d(0, parkSpaceHeight));
                    Polygon2d clipperDown = seg2dDown.ToRenct2d(width * _S);
                    polygon2ds.Add(clipperDown);
                }
            }
            else
            {
                // 处理为 工 字型
                if (_S > 0.01)
                {
                    Vector2d middleDowmPoint = (ld + rd) / 2;
                    Segment2d seg2dDown = new Segment2d(middleDowmPoint + new Vector2d(0, parkSpaceHeight), middleDowmPoint - new Vector2d(0, parkSpaceHeight));
                    Polygon2d clipperDown = seg2dDown.ToRenct2d(width * _S);
                    polygon2ds.Add(clipperDown);
                }
                if (_N > 0.01)
                {
                    Vector2d middleUpPoint = (lu + ru) / 2;
                    Segment2d seg2dUp = new Segment2d(middleUpPoint + new Vector2d(0, parkSpaceHeight), middleUpPoint - new Vector2d(0, parkSpaceHeight));
                    Polygon2d clipperUp = seg2dUp.ToRenct2d(width * _N);
                    polygon2ds.Add(clipperUp);
                }
            }

            return o.DifferenceClipper(polygon2ds).ToList();

        }


        /// <summary>
        /// 删除多边形的尖角 删除共线的点
        /// </summary>
        /// <returns></returns>
        public static Polygon2d RemoveSharpCcorners(this Polygon2d polygon2d, double AnglePRECISION = 0.1)
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
        public static Polygon2d DelDuplicate(this Polygon2d polygon2d)
        {
            return new Polygon2d(polygon2d.Vertices.DelDuplicate());
        }
        #endregion

        #region spatial location
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
        #endregion

        #region Rectangle of coil
        /// <summary>
        /// 线圈所在外包矩形 左上角点
        /// </summary>
        public static Vector2d LUpOfBox2d(this Polygon2d polygon2d)
        {
            return polygon2d.GetBounds().LUp();
        }
        /// <summary>
        /// 线圈所在外包矩形 右上角点
        /// </summary>
        public static Vector2d RUpOfBox2d(this Polygon2d polygon2d)
        {
            return polygon2d.GetBounds().RUp();
        }
        /// <summary>
        /// 线圈所在外包矩形 右下角点
        /// </summary>
        public static Vector2d RDpOfBox2d(this Polygon2d polygon2d)
        {
            return polygon2d.GetBounds().RDp();
        }
        /// <summary>
        /// 线圈所在外包矩形 左下角点
        /// </summary>
        public static Vector2d LDpOfBox2d(this Polygon2d polygon2d)
        {
            return polygon2d.GetBounds().LDp();
        }

        /// <summary>
        /// 线圈所在外包矩形的高度
        /// </summary>
        static public double Height(this Polygon2d polygon2d)
        {
            AxisAlignedBox2d axisAlignedBox2D = polygon2d.GetBounds();
            return axisAlignedBox2D.Height;
        }
        /// <summary>
        /// 线圈所在外包矩形的宽度
        /// </summary>
        static public double Width(this Polygon2d polygon2d)
        {
            AxisAlignedBox2d axisAlignedBox2D = polygon2d.GetBounds();
            return axisAlignedBox2D.Width;

        }
        /// <summary>
        /// 线圈所在外包矩形的形心
        /// </summary>
        static public Vector2d Center(this Polygon2d polygon2d)
        {
            AxisAlignedBox2d axisAlignedBox2D = polygon2d.GetBounds();
            return ((axisAlignedBox2D.Min + axisAlignedBox2D.Max) / 2);
        }
        /// <summary>
        /// 线圈所在外包矩形的形心
        /// </summary>
        static public Vector2d Center(this IEnumerable<Vector2d> vector2ds)
        {
            AxisAlignedBox2d axisAlignedBox2D = (new Polygon2d(vector2ds)).GetBounds();
            return ((axisAlignedBox2D.Min + axisAlignedBox2D.Max) / 2);
        }
        #endregion

        #region Others

        /// <summary>
        /// 求出所有线圈的长度方向
        /// </summary>
        /// <param name="polygon2ds"></param>
        /// <returns></returns>
        public static Segment2d FindDirection(this IEnumerable<Polygon2d> polygon2ds)
        {
            List<Segment2d> segment2ds = new List<Segment2d>();

            foreach (var poly in polygon2ds)
            {
                segment2ds.AddRange(poly.RemoveSharpCcorners().SegmentItr());
            }
            // 直接找到当前线圈的最长边方向

            if (segment2ds.Count > 0)
            {
                return segment2ds.OrderBy(p => p.Length).Last();
            }
            else
            {
                return new Segment2d();
            }


            // 基于方向进行排序
            //List<IGrouping<Vector2d, Segment2d>> groups = segment2ds.GroupBy(c => c.Direction).ToList();
            //if (groups.Count > 1)
            //{
            //    groups = groups.OrderBy(p => p.Count()).ToList();
            //    Segment2d seg01 = groups[0].OrderBy(p => p.Length).Last();
            //    Segment2d seg02 = groups[1].OrderBy(p => p.Length).Last();

            //    if (seg01.Length > seg02.Length)
            //    {
            //        return seg01;
            //    }
            //    else
            //    {
            //        return seg02;
            //    }
            //}
            //else
            //{
            //    return new Segment2d();
            //}
        }
        #endregion

        #region Spatial Relations
        /// <summary>
        /// polygon2d 是否存在 一个polygon2d列表中
        /// </summary>
        public static bool IsIn(this Polygon2d polygon2d, IEnumerable<Polygon2d> polygon2ds)
        {
            foreach (Polygon2d _polygon2d in polygon2ds)
            {
                if (_polygon2d.Contains(polygon2d))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断一个polygon2d是否与某一个polygon2d重合
        /// </summary>
        /// <param name="polygon2d"></param>
        /// <param name="polygon2ds"></param>
        /// <returns></returns>
        public static bool IsSameIn(this Polygon2d polygon2d, IEnumerable<Polygon2d> polygon2ds)
        {
            foreach (Polygon2d _polygon2d in polygon2ds)
            {
                if (_polygon2d.IsSame(polygon2d))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断两个polygon2d是位置相同
        /// </summary>
        public static bool IsSame(this Polygon2d polygon2d01, Polygon2d polygon2d02)
        {
            Vector2d _cp01 = polygon2d01.Vertices[0];

            List<Vector2d> points = polygon2d02.Vertices.ToList();
            int index = points.IndexOf(_cp01);
            if (index == -1) return false;

            polygon2d01 = polygon2d01.ReSortPolygon2dByPoint(_cp01);
            polygon2d02 = polygon2d02.ReSortPolygon2dByPoint(_cp01);

            if (polygon2d01.VertexCount == polygon2d02.VertexCount)
            {
                for (int i = 0; i < polygon2d01.VertexCount; i++)
                {
                    if (polygon2d01.Vertices[i].EpsilonEqual(polygon2d02.Vertices[i], Precision_.TheShortestDistance))
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
        /// 基于线圈上的一个点，对polygon2d的点集进行重新排序
        /// </summary>
        public static Polygon2d ReSortPolygon2dByPoint(this Polygon2d polygon2d, Vector2d _cp)
        {
            List<Vector2d> points = polygon2d.Vertices.ToList();
            int index = points.IndexOf(_cp);
            int count = points.Count;
            List<Vector2d> result = new List<Vector2d>();
            for (int i = index; i < count; i++)
            {
                result.Add(points[i]);
            }
            for (int i = 0; i < index; i++)
            {
                result.Add(points[i]);
            }
            return new Polygon2d(result);
        }
        /// <summary>
        /// 两个线圈重叠的边界线
        /// </summary>
        /// <returns></returns>
        public static List<Segment2d> OverlapSegs(this Polygon2d polygon2d01, Polygon2d polygon2d02)
        {
            return polygon2d01.SegmentItr().OverLapSegs(polygon2d02.SegmentItr(), 0.0).ToList();
        }
        /// <summary>
        /// 获取两根线段的重叠部分 通过距离判断
        /// </summary>
        /// <param name="polygon2d01"></param>
        /// <param name="polygon2d02"></param>
        /// <returns></returns>
        public static List<Segment2d> OverlapSegs_ByDis(this Polygon2d polygon2d01, Polygon2d polygon2d02)
        {
            return polygon2d01.SegmentItr().OverLapSegs_byDis(polygon2d02.SegmentItr(), 0.0).ToList();
        }
        #endregion

    }
}
