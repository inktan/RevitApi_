using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using goa.Common.NtsInterOp;
using NetTopologySuite.Geometries;

namespace InfoStrucFormwork
{
    internal class FloorDivision : RequestMethod
    {
        internal FloorDivision(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            ICollection<ElementId> elementIds = this.sel.GetElementIds();
            if (elementIds.Count < 1) return;

            //找到楼板
            List<Floor> floors = new List<Floor>();
            foreach (var item in elementIds)
            {
                Element element = this.doc.GetElement(item);
                if (element is Floor)
                {
                    floors.Add(element as Floor);
                }
            }
            if (floors.Count < 1) return;

            //找到线元素
            List<Line> lines = new List<Line>();
            foreach (var item in elementIds)
            {
                Element element = this.doc.GetElement(item);
                Autodesk.Revit.DB.Location location = element.Location;
                if (location is LocationCurve)
                {
                    Curve curve = (location as LocationCurve).Curve;
                    if (curve is Line)
                    {
                        lines.Add(curve as Line);
                    }
                }
            }
            if (lines.Count < 1) return;

            List<LineString> lineStrings = new List<LineString>();
            foreach (var item in lines)
            {
                var p0 = item.GetEndPoint(0);
                var p1 = item.GetEndPoint(1);

                Coordinate[] points = { new Coordinate(p0.X, p0.Y), new Coordinate(p1.X, p1.Y) };
                LineString lineString = new LineString(points);
                lineStrings.Add(lineString);
            }

            //List<Polygon> polygons = new List<Polygon>();
            foreach (var item in floors)
            {
                ElementClassFilter filter = new ElementClassFilter(typeof(CurveElement));
                List<ElementId> dependentIds = item.GetDependentElements(filter).ToList();
                var curves = dependentIds.Select(x => (this.doc.GetElement(x).Location as LocationCurve).Curve).ToList();

                List<Coordinate> points = new List<Coordinate>();

                foreach (var curve in curves)
                {
                    var p0 = curve.GetEndPoint(0);
                    //var p1 = curve.GetEndPoint(1);
                    points.Add(new Coordinate(p0.X, p0.Y));
                    //points.Add(new Coordinate(p1.X, p1.Y));
                }

                points.Add(points[0]);

                var shell = new LinearRing(points.ToArray());
                var polygon = new Polygon(shell);
                //polygons.Add(polygon);

                var res = SplitPolygonByLine(polygon, lineStrings.First());
                if (res.polygons.Count() < 1) continue;

                foreach (var poly in res.polygons)
                {
                    using (Transaction transaction = new Transaction(CMD.Doc, "创建楼板"))
                    {
                        transaction.Start();
                        transaction.DeleteErrOrWaringTaskDialog();
                        CurveArray curveArray = poly.ToLines().ToCurveArray();

                        Floor floor = CMD.Doc.Create.NewFloor(curveArray, item.FloorType, CMD.Doc.GetElement(item.LevelId) as Level, true);
                        floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0.0);
                        transaction.Commit();
                    }
                }
                using (Transaction transaction = new Transaction(CMD.Doc, "删除楼板"))
                {
                    transaction.Start();
                    transaction.DeleteErrOrWaringTaskDialog();
                    CMD.Doc.Delete(item.Id);
                    transaction.Commit();
                }

            }

            //foreach (var item in res.lineSegments)
            //{
            //    Line line = Line.CreateBound(item.P0.ToXYZ(), item.P1.ToXYZ());
            //    this.doc.CreateDirectShapeWithNewTransaction(new List<Line>() { line });
            //}

            //resultPolygons.Count.ToString().TaskDialogErrorMessage();
        }


        /// <summary>
        /// 使用一条线切割多边形,可得到多个切线端和多个多边形
        /// </summary>
        static (Polygon[] polygons, NetTopologySuite.Geometries.LineSegment[] lineSegments) SplitPolygonByLine(Polygon polygon, NetTopologySuite.Geometries.LineString lineSegment)
        {
            //var lineClip = new LineString(new[] { lineSegment.P0, lineSegment.P1 });
            var lineClip = lineSegment;
            var g = polygon.Intersection(lineClip);
            if (g is NetTopologySuite.Geometries.Point point) return (new[] { polygon }, new NetTopologySuite.Geometries.LineSegment[0]);
            if (g is NetTopologySuite.Geometries.MultiPoint multiPoint) return (new[] { polygon }, new NetTopologySuite.Geometries.LineSegment[0]);

            var lines = new List<NetTopologySuite.Geometries.LineSegment>();
            if (g is LineString lineString)
            {
                if (lineString.Count > 0) lines.Add(new NetTopologySuite.Geometries.LineSegment(lineString.Coordinates[0], lineString.Coordinates[1]));
            }
            if (g is MultiLineString multiLineString)
            {
                foreach (var item in multiLineString.Geometries)
                {
                    var tmp = item as LineString;
                    if (tmp.Count > 0) lines.Add(new NetTopologySuite.Geometries.LineSegment(tmp.Coordinates[0], tmp.Coordinates[1]));
                }
            }
            if (lines.Count == 0) return (new[] { polygon }, new NetTopologySuite.Geometries.LineSegment[0]);
            //进行切割
            var g2 = polygon.SymmetricDifference(lineClip);
            var pointPolygon = g2 as Polygon;
            if (pointPolygon == null)
            {
                var coll = g2 as GeometryCollection;

                if (coll != null)
                {
                    pointPolygon = coll.Where(i => i is Polygon).FirstOrDefault() as Polygon;
                }
            }
            var polygons = new List<Polygon>();
            if (pointPolygon != null)
            {
                polygons.Add(pointPolygon);
            }

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                var hasFind = false;
                for (int j = 0; j < polygons.Count; j++)
                {
                    var tmpPolygon = polygons[j];
                    var cs = tmpPolygon.Coordinates.ToList();
                    var startIndex = cs.FindIndex(p => p.Equals2D(line.P0));
                    var endIndex = cs.FindIndex(p => p.Equals2D(line.P1));
                    if (startIndex != -1 && endIndex != -1 && Math.Abs(startIndex - endIndex) == 1)
                    {
                        //和 tmpPolygon 的边重合,直接舍弃
                        break;
                    }
                    if (startIndex != -1 && endIndex != -1 && Math.Abs(startIndex - endIndex) > 1)
                    {
                        //在这个 tmpPolygon 上找到了交点
                        //分离两个polygon
                        if (startIndex > endIndex)
                        {
                            var t = startIndex;
                            startIndex = endIndex;
                            endIndex = t;
                        }
                        //一个
                        var tmpCs = cs.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();
                        tmpCs.Add(tmpCs[0]);
                        //二个
                        //var tmpCs2 = cs.Skip(endIndex).SkipLast(1).ToList();
                        var tmpCs2 = cs.Skip(endIndex).ToList();
                        if (tmpCs2.Count > 0)
                        {
                            tmpCs2.RemoveAt(tmpCs2.Count - 1);
                        }
                        tmpCs2.AddRange(cs.Take(startIndex + 1));
                        tmpCs2.Add(tmpCs2[0]);

                        polygons.RemoveAt(j);
                        polygons.Add(new Polygon(new LinearRing(tmpCs.ToArray())));
                        polygons.Add(new Polygon(new LinearRing(tmpCs2.ToArray())));

                        lines[i].SetCoordinates(cs[startIndex], cs[endIndex]);
                        hasFind = true;
                        break;
                    }
                }
                if (!hasFind) lines.RemoveAt(i);
            }

            return (polygons.ToArray(), lines.ToArray());
        }

    }
}


