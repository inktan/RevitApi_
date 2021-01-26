using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using ClipperLib;
using g3;
//using NetTopologySuite.Geometries;

namespace PublicProjectMethods_
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public static class _g3To
    {
        #region ToRevit
        public static IEnumerable<CurveLoop> ToCurveLoops(this IEnumerable<Polygon2d> polygon2ds)
        {
            foreach (var item in polygon2ds)
            {
                yield return item.ToCurveLoop();
            }
        }
        public static CurveLoop ToCurveLoop(this Polygon2d _polygon2d)
        {
            _polygon2d = _polygon2d.DelDuplicate();
            List<Line> lines = new List<Line>();
            foreach (var seg in _polygon2d.SegmentItr())
            {
                lines.Add(seg.ToLine());
            }
            CurveLoop curves = new CurveLoop();
            foreach (Line line in lines)
            {
                Curve _c = line as Curve;
                curves.Append(_c);
            }
            return curves;
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
        public static Line ToLine(this Segment2d segment2D)
        {
            return Line.CreateBound(segment2D.P0.ToXYZ(), segment2D.P1.ToXYZ());
        }

        public static Line ToLineUnBound(this Segment2d segment2D)
        {
            return Line.CreateUnbound(segment2D.P0.ToXYZ(), segment2D.P1.ToXYZ());
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
        public static XYZ ToXYZ(this Vector2d vector2D)
        {
            return new XYZ(vector2D.x, vector2D.y, 0);
        }
        public static XYZ ToXYZ(this Vector3d vector3D)
        {
            return new XYZ(vector3D.x, vector3D.y, vector3D.z);
        }
        #endregion

        #region ToClipper

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
            double x = vector2D.x * Precision_.clipperMultiple;
            double y = vector2D.y * Precision_.clipperMultiple;
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
