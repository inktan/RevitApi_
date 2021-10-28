using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Polygonize;
using NetTopologySuite.Operation.Overlay;
using NetTopologySuite.Operation.Union;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.GeometriesGraph;
using NetTopologySuite.Simplify;
//using NetTopologySuite.Mathematics;
using NetTopologySuite.Precision;
using NetTopologySuite.Algorithm;

using g3;

namespace goa.Common.NtsInterOp
{
    public static class NTS_InterOp
    {
        //IMPORTANT: need to process Revit coordinates to a reduced precision
        //before using NTS, otherwise all kinds of problems.
        //Default reduce to 0.00001
        private static PrecisionModel pm = new PrecisionModel(1e5); //0.00001
        private static GeometryFactory gf = new GeometryFactory(pm);
        private static GeometryPrecisionReducer gpr = new GeometryPrecisionReducer(pm);
        public static LineString ProjectInto(this Line _line, Plane _plane)
        {
            var p0proj = _plane.ProjectInto(_line.GetEndPoint(0));
            var p1proj = _plane.ProjectInto(_line.GetEndPoint(1));
            var ls = new LineString(
                new Coordinate[2]
                {
                p0proj.ToNtsCoord(),
                p1proj.ToNtsCoord()
                });
            return ls;
        }
        public static IList<Line> ToLines(this Geometry _geo)
        {
            IList<Line> list = null;
            if (_geo is NetTopologySuite.Geometries.Point || _geo is MultiPoint)
                return null;
            else if (_geo is LineString)
            {
                return ((LineString)_geo).ToLines();
            }
            else if (_geo is Polygon)
            {
                list = ((Polygon)_geo).ToLines();
            }
            else
            {
                if (_geo is MultiLineString)
                {
                    list = ((MultiLineString)_geo).ToLines();
                }
                else if (_geo is MultiPolygon)
                {
                    list = ((MultiPolygon)_geo).ToLines();
                }
                else if (_geo is GeometryCollection)
                {
                    list = ((GeometryCollection)_geo).Geometries.SelectMany(x => x.ToLines()).ToList();
                }
            }
            list = list.Where(x => x.Length.IsAlmostEqualByDifference(0) == false).ToList();
            return list;
        }
        public static IList<Segment3d> ToSegments(this Geometry _geo)
        {
            IList<Segment3d> list = null;
            if (_geo is NetTopologySuite.Geometries.Point || _geo is MultiPoint)
                return null;
            else if (_geo is LineString)
            {
                return ((LineString)_geo).LineStringToSegments();
            }
            else if (_geo is Polygon)
            {
                list = ((Polygon)_geo).PolygonToSegments();
            }
            else
            {
                if (_geo is MultiLineString)
                {
                    list = ((MultiLineString)_geo).MultiLineStringToSegments();
                }
                else if (_geo is MultiPolygon)
                {
                    list = ((MultiPolygon)_geo).MultiPolygonToSegments();
                }
                else if (_geo is GeometryCollection)
                {
                    list = ((GeometryCollection)_geo).Geometries.SelectMany(x => x.ToSegments()).ToList();
                }
            }
            list = list.Where(x => x.Length.IsAlmostEqualByDifference(0) == false).ToList();
            return list;
        }
        public static Coordinate ToNtsCoord(this UV _uv)
        {
            return new Coordinate(_uv.U, _uv.V);
        }
        public static Coordinate ToNtsCoord(this XYZ _XYZ)
        {
            return new Coordinate(_XYZ.X, _XYZ.Y);
        }
        public static NetTopologySuite.Geometries.Point ToNtsPoint(this XYZ _xyz)
        {
            return new NetTopologySuite.Geometries.Point(_xyz.ToNtsCoord());
        }
        public static XYZ ToXYZ(this Coordinate _coord)
        {
            double z = double.IsNaN(_coord.Z) ? 0 : _coord.Z;
            return new XYZ(_coord.X, _coord.Y, z);
        }
        public static Vector3d ToVector3d(this Coordinate _coord)
        {
            return new Vector3d(_coord.X, _coord.Y, 0.0);
        }
        public static Vector2d ToVector2d(this Coordinate _coord)
        {
            return new Vector2d(_coord.X, _coord.Y);
        }
        public static Vector3d ToVector3f(this Coordinate _coord)
        {
            return new Vector3f(_coord.X, _coord.Y, 0.0);
        }
        public static XYZ ToXYZ(this NetTopologySuite.Geometries.Point _point)
        {
            double z = double.IsNaN(_point.Z) ? 0 : _point.Z;
            return new XYZ(_point.X, _point.Y, z);
        }
        public static UV ToUV(this Coordinate _coord)
        {
            return new UV(_coord.X, _coord.Y);
        }
        public static LineString ToNtsLineString(this Line _line)
        {
            var coords = new Coordinate[2]
            {
                _line.GetEndPoint(0).ToNtsCoord(),
                _line.GetEndPoint(1).ToNtsCoord()
            };
            var ls = new LineString(coords);
            return gpr.Reduce(ls) as LineString;
        }

        public static Line ToLine(this LineString _ls)
        {
            var p0 = _ls.GetCoordinateN(0).ToXYZ();
            var p1 = _ls.GetCoordinateN(1).ToXYZ();
            if (p0.IsAlmostEqualToByDifference(p1, 0.0001))
                return null;
            try
            {
                var line = Line.CreateBound(p0, p1);
                return line;
            }
            catch { return null; }
        }
        public static Segment3d ToSegment(this LineString _ls)
        {
            var p0 = _ls.GetCoordinateN(0).ToVector3d();
            var p1 = _ls.GetCoordinateN(1).ToVector3d();
            return new Segment3d(p0, p1);
        }
        public static IList<Line> ToLines(this LineString _ls)
        {
            var list = new List<Line>();
            var coords = _ls.Coordinates;
            for (int i = 0; i < coords.Count() - 1; i++)
            {
                var c0 = coords[i];
                var c1 = coords[i + 1];
                try
                {
                    var line = Line.CreateBound(c0.ToXYZ(), c1.ToXYZ());
                    list.Add(line);
                }
                catch { }
            }
            return list;
        }
        public static IList<Segment3d> LineStringToSegments(this LineString _ls)
        {
            var list = new List<Segment3d>();
            var coords = _ls.Coordinates;
            for (int i = 0; i < coords.Count() - 1; i++)
            {
                var c0 = coords[i];
                var c1 = coords[i + 1];
                try
                {
                    var line = new Segment3d(c0.ToVector3d(), c1.ToVector3d());
                    list.Add(line);
                }
                catch { }
            }
            return list;
        }
        public static CurveArray ToCurveArray(this LineString _ls)
        {
            var lines = _ls.ToLines();
            var ca = new CurveArray();
            foreach (var l in lines)
                ca.Append(l);
            return ca;
        }
        public static List<Line> ToLines(this MultiLineString _mls)
        {
            var list = new List<Line>();
            foreach (Geometry geo in _mls)
            {
                if (geo is LineString)
                {
                    var ls = geo as LineString;
                    list.AddRange(ls.ToLines());
                }
            }
            return list;
        }
        public static List<Segment3d> MultiLineStringToSegments(this MultiLineString _mls)
        {
            var list = new List<Segment3d>();
            foreach (Geometry geo in _mls)
            {
                if (geo is LineString)
                {
                    var ls = geo as LineString;
                    list.AddRange(ls.ToSegments());
                }
            }
            return list;
        }
        public static List<Line> ToLines(this MultiPolygon _multiPoly)
        {
            var list = new List<Line>();
            foreach (Geometry geo in _multiPoly)
            {
                if (geo is Polygon)
                {
                    var poly = geo as Polygon;
                    list.AddRange(poly.ToLines());
                }
            }
            return list;
        }
        public static List<Segment3d> MultiPolygonToSegments(this MultiPolygon _multiPoly, Frame3d _frame)
        {
            var list = new List<Segment3d>();
            foreach (Geometry geo in _multiPoly)
            {
                if (geo is Polygon)
                {
                    var poly = geo as Polygon;
                    list.AddRange(poly.PolygonToSegments(_frame));
                }
            }
            return list;
        }
        public static List<Segment3d> MultiPolygonToSegments(this MultiPolygon _multiPoly)
        {
            var list = new List<Segment3d>();
            foreach (Geometry geo in _multiPoly)
            {
                if (geo is Polygon)
                {
                    var poly = geo as Polygon;
                    list.AddRange(poly.PolygonToSegments());
                }
            }
            return list;
        }
        public static List<Line> ToLines(this Polygon poly, Frame _fromFrame = null)
        {
            var shell = poly.Shell.ToLines();
            //var holes = poly.Holes.SelectMany(x => x.ToLines());
            var interiorRings = poly.InteriorRings.SelectMany(x => x.ToLines());
            var all = new List<Line>();
            all.AddRange(shell);
            //all.AddRange(holes);
            all.AddRange(interiorRings);
            if (_fromFrame != null)
            {
                var newLines = new List<Line>();
                foreach (var line in all)
                {
                    var newLine = Line.CreateBound
                        (_fromFrame.PointFromFrame(line.GetEndPoint(0)),
                        _fromFrame.PointFromFrame(line.GetEndPoint(1)));
                    newLines.Add(newLine);
                }
                return newLines;
            }
            else
                return all;
        }
        public static List<Segment3d> PolygonToSegments(this Polygon poly, Frame3d _fromFrame)
        {
            var shell = poly.Shell.ToSegments();
            //var holes = poly.Holes.SelectMany(x => x.ToLines());
            var interiorRings = poly.InteriorRings.SelectMany(x => x.ToSegments());
            var all = new List<Segment3d>();
            all.AddRange(shell);
            //all.AddRange(holes);
            all.AddRange(interiorRings);

            var newLines = new List<Segment3d>();
            foreach (var line in all)
            {
                var newLine = new Segment3d
                    (_fromFrame.FromFrameP(line.P0),
                    _fromFrame.FromFrameP(line.P1));
                newLines.Add(newLine);
            }
            return newLines;
        }
        public static List<Segment3d> PolygonToSegments(this Polygon poly)
        {
            var shell = poly.Shell.ToSegments();
            //var holes = poly.Holes.SelectMany(x => x.ToLines());
            var interiorRings = poly.InteriorRings.SelectMany(x => x.ToSegments());
            var all = new List<Segment3d>();
            all.AddRange(shell);
            //all.AddRange(holes);
            all.AddRange(interiorRings);          
            return all;
        }
        public static Floor CreateFloor(this Polygon _polygon, Document _doc, Element _floorType, Level _level)
        {
            var shellLines = _polygon.Shell.ToLines();
            var shellCA = new CurveArray();
            foreach (var l in shellLines)
                shellCA.Append(l);
            Floor floor = _doc.Create.NewFloor(shellCA, _floorType as FloorType, _level, false);
            floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0.0);
            return floor;
        }
        public static List<Opening> AddOpenings(this Polygon _polygon, Floor _floor)
        {
            var holesLines = _polygon.Holes.Select(x => x.ToLines());
            List<CurveArray> holesCA = new List<CurveArray>();
            foreach (var h in holesLines)
            {
                var ca = new CurveArray();
                foreach (var l in h)
                    ca.Append(l);
                holesCA.Add(ca);
            }
            Document doc = _floor.Document;
            var list = new List<Opening>();
            foreach (var hole in holesCA)
            {
                var opening = doc.Create.NewOpening(_floor, hole, true);
                list.Add(opening);
            }
            return list;
        }
        /// <summary>
        /// Get all solids' mesh vertices projection on XY plane.
        /// precission: 0.0 - 1.0.
        /// </summary>
        public static Geometry GetConvexHullOnXY(this Element _elem, double _precision = 0.3)
        {
            var solids = _elem.GetAllSolids();
            var faces = solids.SelectMany(x => x.Faces.Cast<Face>());
            var meshes = faces.Select(x => x.Triangulate(_precision));
            var vertices = meshes.SelectMany(x => x.Vertices);
            var coords = vertices.Select(x => x.ToNtsCoord());
            var ch = new ConvexHull(coords, gf);
            var chGeom = ch.GetConvexHull();
            return chGeom;
        }
        public static CurveLoop ToCurveLoop(this LinearRing _ring)
        {
            return _ring.ToCurveLoop(Transform.Identity);
        }
        public static CurveLoop ToCurveLoop(this LinearRing _ring, Transform _tf)
        {
            var lines = _ring.ToLines();
            var cl = new CurveLoop();
            foreach (var line in lines)
            {
                cl.Append(line.CreateTransformed(_tf));
            }
            return cl;
        }
        public static LinearRing ToNtsLinearRing(this CurveLoop _cl)
        {
            var coords = new List<Coordinate>();
            foreach (var c in _cl)
            {
                var co = c.GetEndPoint(0).ToNtsCoord();
                coords.Add(co);
            }
            coords.Add(_cl.First().GetEndPoint(0).ToNtsCoord());
            var lr = new LinearRing(coords.ToArray());
            return gpr.Reduce(lr) as LinearRing;
        }
        public static LinearRing ToNtsLinearRing(this IEnumerable<Line> _lines)
        {
            var coords = new List<Coordinate>();
            foreach (var l in _lines)
            {
                var co = l.GetEndPoint(0).ToNtsCoord();
                coords.Add(co);
            }
            coords.Add(_lines.First().GetEndPoint(0).ToNtsCoord());
            var lr = new LinearRing(coords.ToArray());
            return gpr.Reduce(lr) as LinearRing;
        }
    }
}
