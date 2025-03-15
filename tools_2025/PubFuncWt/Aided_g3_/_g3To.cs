using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using ClipperLib;
using g3;
using goa.Common.g3InterOp;
using Octree;
//using NetTopologySuite.Geometries;

namespace PubFuncWt
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public static class _g3To
    {
        #region ToOctree
        public static Bounds ToBounds(this AxisAlignedBox3d axisAlignedBox3d)
        {
            Vector3 size = new Vector3(axisAlignedBox3d.Width, axisAlignedBox3d.Height, axisAlignedBox3d.Depth);
            return new Bounds(axisAlignedBox3d.Center.ToVector3(), size);
        }

        public static Vector3 ToVector3(this Vector3d vector3d)
        {
            return new Vector3(vector3d.x, vector3d.y, vector3d.z);
        }

        #endregion

        #region ToQuadTree
        public static Polygon2d ToPolygon2d(this RectangleF rectangleF)
        {
            Vector2d v00 = new Vector2d((double)rectangleF.Location.X, (double)rectangleF.Location.Y);
            Vector2d v01 = v00 + new Vector2d(rectangleF.Width, 0.0);
            Vector2d v02 = v01 + new Vector2d(0.0, rectangleF.Height);
            Vector2d v03 = v00 + new Vector2d(0.0, rectangleF.Height);

            List<Vector2d> vector2ds = new List<Vector2d>() { v00, v01, v02, v03 };

            return new Polygon2d(vector2ds);
        }
        public static RectangleF ToRectangle(this Polygon2d polygon2d)
        {
            AxisAlignedBox2d axisAlignedBox2D = polygon2d.GetBounds();
            return axisAlignedBox2D.ToRectangleF();
        }

        public static RectangleF ToRectangleF(this AxisAlignedBox2d axisAlignedBox2d)
        {
            Vector2d min = axisAlignedBox2d.Min;
            return new RectangleF((float)min.x, (float)min.y, (float)axisAlignedBox2d.Length(), (float)axisAlignedBox2d.Width());
        }
        #endregion

        #region ToRevit

        //public static Line ToLine(this Segment3d segment3d)
        //{
        //    return Line.CreateBound(segment3d.P0.ToXYZ(), segment3d.P1.ToXYZ());
        //}

        public static IEnumerable<CurveLoop> ToCurveLoops(this IEnumerable<Polygon2d> polygon2ds)
        {
            foreach (var item in polygon2ds)
            {
                CurveLoop curves = item.ToCurveLoop();
                if (curves.NumberOfCurves() >= 3)
                    yield return item.ToCurveLoop();
            }
        }
        public static CurveLoop ToCurveLoop(this Polygon2d _polygon2d)
        {
            Polygon2d polygon2d = _polygon2d.DelDuplicate();
            if (polygon2d.VertexCount >= 3 && polygon2d.Area > Precision_.Precison)
            {
                List<Line> lines = polygon2d.SegmentItr().Select(p => p.ToLine()).ToList();
                CurveLoop curves = new CurveLoop();
                foreach (Line line in lines)
                {
                    curves.Append(line);
                }
                return curves;
            }
            else
            {
                return new CurveLoop();
            }
        }

        public static Arc ToArc(this Arc2d arc2d)
        {
            XYZ center = arc2d.Center.ToXYZ();
            double radius = arc2d.Radius;
            double AngleStartDeg = (Math.PI / 180) * arc2d.AngleStartDeg;
            double AngleEndDeg = (Math.PI / 180) * arc2d.AngleEndDeg;
            XYZ xAxis = new XYZ(1, 0, 0);
            XYZ yAxis = new XYZ(0, 1, 0);

            return Arc.Create(center, radius, AngleStartDeg, AngleEndDeg, xAxis, yAxis);
        }

        public static IEnumerable<Line> ToLines(this IEnumerable<Segment2d> segment2ds)
        {
            foreach (var item in segment2ds)
                yield return item.ToLine();
        }

        /// <summary>
        /// segmentToLine
        /// </summary>
        public static Line2d ToLine2d(this Segment2d _segment2d)
        {
            return new Line2d(_segment2d.P0, _segment2d.Direction);
        }
        public static Line ToLineUnBound(this Segment2d segment2D)
        {
            return Line.CreateUnbound(segment2D.P0.ToXYZ(), segment2D.P1.ToXYZ());
        }
        public static Line ToLineBound(this Segment2d segment2D)
        {
            return Line.CreateBound(segment2D.P0.ToXYZ(), segment2D.P1.ToXYZ());
        }

        public static IEnumerable<List<XYZ>> ToXyzses(this IEnumerable<IEnumerable<Vector2d>> vector2dses)
        {
            foreach (var item in vector2dses)
            {
                yield return item.ToXyzs().ToList();

            }
        }
        public static IEnumerable<XYZ> ToXyzs(this IEnumerable<Vector2d> vector2ds)
        {
            foreach (var v in vector2ds)
                yield return v.ToXYZ();
        }
        public static IEnumerable<XYZ> ToXyzs(this IEnumerable<Vector3d> vector3ds)
        {
            foreach (var v in vector3ds)
                yield return v.ToXYZ();
        }
        public static XYZ ToXYZ(this Vector2d vector2D,double z=0.0)
        {
            return new XYZ(vector2D.x, vector2D.y, z);
        }
        //public static XYZ ToXYZ(this Vector3d vector3D)
        //{
        //    return new XYZ(vector3D.x, vector3D.y, vector3D.z);
        //}
        #endregion

        #region ToClipper

        public static Paths ToPaths(this IEnumerable<Polygon2d> polygon2ds)
        {
            Paths _Paths = new Paths();
            foreach (var item in polygon2ds)
            {
                Path _Path = item.ToPath();
                _Paths.Add(_Path);
            }
            return _Paths;
        }
        public static Paths ToPaths(this IEnumerable<List<Vector2d>> vector2Dses)
        {
            Paths _Paths = new Paths();
            foreach (List<Vector2d> vector2Ds in vector2Dses)
            {
                Path _Path = vector2Ds.ToPath();
                _Paths.Add(_Path);
            }
            return _Paths;
        }
        public static Path ToPath(this Polygon2d polygon2d)
        {
            return polygon2d.Vertices.ToPath();
        }
        public static Path ToPath(this IEnumerable<Vector2d> vector2Ds)
        {
            Path _Path = new Path();
            foreach (Vector2d vector2d in vector2Ds)
            {
                IntPoint _IntPoint = vector2d.ToIntPoint();
                _Path.Add(_IntPoint);
            }
            return _Path;
        }
        /// <summary>
        /// g3 vector2d to clipper intpoint
        /// </summary>
        public static IntPoint ToIntPoint(this Vector2d vector2D)
        {
            double x = vector2D.x * Precision_.ClipperMultiple;
            double y = vector2D.y * Precision_.ClipperMultiple;
            return new IntPoint((cInt)x, (cInt)y);
        }
        #endregion

        #region NetTopologySuiteTo_

        //public static Polygon ToPolygon(this Polygon2d polygon2d)
        //{
        //    ICollection<Vector2d> vector2ds = polygon2d.Vertices;
        //    return vector2ds.ToPolygon();
        //}
        //public static Polygon ToPolygon(this IEnumerable<Vector2d> vector2ds)
        //{
        //    IEnumerable<Coordinate> coordinates = vector2ds.ToCoordinates();
        //    Coordinate[] _coordinates = coordinates.ToArray();
        //    //【】LinearRing的构造点集，必要闭合的首尾点
        //    _coordinates =_coordinates.Append(_coordinates.First()).ToArray();
        //    LinearRing linearRing = new LinearRing(_coordinates);
        //    return new Polygon(linearRing);
        //}


        //public static IEnumerable<Coordinate> ToCoordinates(this IEnumerable<Vector2d> vector2ds)
        //{
        //    foreach (var item in vector2ds)
        //    {
        //        yield return item.ToCoordinate();
        //    }
        //}

        //public static Coordinate ToCoordinate(this Vector2d vector2d)
        //{
        //    return new Coordinate(vector2d.x,vector2d.y);
        //}

        #endregion
    }

}
