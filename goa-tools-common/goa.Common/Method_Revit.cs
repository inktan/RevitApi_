using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Exceptions;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using ClipperLib;

namespace goa.Common
{
    public static partial class Methods
    {
        #region BoundingBox
        public static XYZ GetCentroid(this BoundingBoxXYZ _box)
        {
            return (_box.Max + _box.Min) / 2;
        }
        public static Solid ToSolid(this BoundingBoxXYZ _box)
        {
            var loop = getCurveLoopAtBottom(_box);

            var z = _box.Max.Z - _box.Min.Z;
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry
                    (new List<CurveLoop>() { loop }, XYZ.BasisZ, z);
            return SolidUtils.CreateTransformed(solid, _box.Transform);
        }
        public static bool IsInside(this BoundingBoxUV _boxUV, UV _uv)
        {
            return rd(_uv.U) >= rd(_boxUV.Min.U) && rd(_uv.V) >= rd(_boxUV.Min.V)
                && rd(_uv.U) <= rd(_boxUV.Max.U) && rd(_uv.V) <= rd(_boxUV.Max.V);
        }
        public static bool IsInside(this BoundingBoxXYZ _box, XYZ _p)
        {
            return rd(_p.X) >= rd(_box.Min.X)
                && rd(_p.Y) >= rd(_box.Min.Y)
                && rd(_p.Z) >= rd(_box.Min.Z)
                && rd(_p.X) <= rd(_box.Max.X)
                && rd(_p.Y) <= rd(_box.Max.Y)
                && rd(_p.Z) <= rd(_box.Max.Z);
        }
        private static int rounding = 5;
        private static double rd(this double d)
        {
            return Math.Round(d, rounding);
        }
        public static UV[] GetVertices(this BoundingBoxUV _boxUV)
        {
            var p0 = _boxUV.Min;
            var p1 = new UV(_boxUV.Max.U, _boxUV.Min.V);
            var p2 = _boxUV.Max;
            var p3 = new UV(_boxUV.Min.U, _boxUV.Max.V);
            var array = new UV[4] { p0, p1, p2, p3 };
            return array;
        }
        public static XYZ[] GetVertices(this BoundingBoxXYZ _box)
        {
            var p0 = _box.Min;
            var p1 = new XYZ(_box.Max.X, _box.Min.Y, _box.Min.Z);
            var p2 = new XYZ(_box.Max.X, _box.Max.Y, _box.Min.Z);
            var p3 = new XYZ(_box.Min.X, _box.Max.Y, _box.Min.Z);
            var p4 = new XYZ(_box.Min.X, _box.Min.Y, _box.Max.Z);
            var p5 = new XYZ(_box.Max.X, _box.Min.Y, _box.Max.Z);
            var p6 = _box.Max;
            var p7 = new XYZ(_box.Min.X, _box.Max.Y, _box.Max.Z);
            var array = new XYZ[8] { p0, p1, p2, p3, p4, p5, p6, p7 };
            return array;
        }
        public static List<Line> GetBoundaryLines(this BoundingBoxUV _boxUV, Plane _plane)
        {
            var uvs = _boxUV.GetVertices();
            var points = uvs.Select(x => _plane.Evaluate(x)).ToList();
            var l0 = Line.CreateBound(points[0], points[1]);
            var l1 = Line.CreateBound(points[1], points[2]);
            var l2 = Line.CreateBound(points[2], points[3]);
            var l3 = Line.CreateBound(points[3], points[0]);
            return new List<Line>() { l0, l1, l2, l3 };
        }
        public static bool Overlaps(this BoundingBoxUV _box1, BoundingBoxUV _box2)
        {
            var vertices1 = _box1.GetVertices();
            var vertices2 = _box2.GetVertices();
            var b1 = vertices1.Any(x => _box2.IsInside(x));
            var b2 = vertices2.Any(x => _box1.IsInside(x));
            return b1 || b2;
        }
        public static bool Overlaps(this BoundingBoxUV _box1, BoundingBoxUV _box2, out double _area)
        {
            //get all limitations
            var minU = Math.Max(_box1.Min.U, _box2.Min.U);
            var maxU = Math.Min(_box1.Max.U, _box2.Max.U);
            var minV = Math.Max(_box1.Min.V, _box2.Min.V);
            var maxV = Math.Min(_box1.Max.V, _box2.Max.V);
            if (minU > maxU || minV > maxV)
            {
                _area = 0;
                return false;
            }
            else
            {
                _area = (maxU - minU) * (maxV - minV);
                return true;
            }
        }
        public static bool Overlaps(this BoundingBoxXYZ _box1, BoundingBoxXYZ _box2)
        {
            var vertices1 = _box1.GetVertices();
            var vertices2 = _box2.GetVertices();
            var b1 = vertices1.Any(x => _box2.IsInside(x));
            var b2 = vertices2.Any(x => _box1.IsInside(x));
            return b1 || b2;
        }
        public static double GetArea(this BoundingBoxUV _box)
        {
            return (_box.Max.U - _box.Min.U) * (_box.Max.V - _box.Min.V);
        }
        #endregion

        #region Document
        public static string Identifier(this Document _doc)
        {
            return _doc.Title + "|" + _doc.PathName;
        }
        #endregion

        #region Design Option
        public static bool IsInMainModelOrDesignOption(this Element _elem, ElementId _designOptionId)
        {
            if (_designOptionId == ElementId.InvalidElementId)
                return true;
            return _elem.DesignOption == null || _elem.DesignOption.Id == _designOptionId;
        }
        #endregion

        #region Element
        public static List<Solid> GetAllSolids(this Element _elem)
        {
            List<Solid> list = new List<Solid>();
            var opt = new Options();
            opt.ComputeReferences = true;
            var geomElem = _elem.get_Geometry(opt);
            return getAllSolids(geomElem);
        }
        private static List<Solid> getAllSolids(GeometryElement _geomElem)
        {
            List<Solid> list = new List<Solid>();
            if (_geomElem == null)
                return list;
            foreach (GeometryObject geomObj in _geomElem)
            {
                if (geomObj is Solid)
                {
                    Solid geomSolid = geomObj as Solid;
                    if (geomSolid == null
                        || geomSolid.Volume.IsAlmostEqualByDifference(0))
                        continue;
                    list.Add(geomSolid);
                }
                else if (geomObj is GeometryInstance)
                {
                    GeometryInstance gi = geomObj as GeometryInstance;
                    var ge = gi.GetInstanceGeometry();
                    list.AddRange(getAllSolids(ge));
                }
            }
            return list;
        }
        public static List<Solid> GetAllSolids(this Element _elem, Autodesk.Revit.DB.View _view)
        {
            List<Solid> list = new List<Solid>();
            var opt = new Options();
            opt.View = _view;
            opt.IncludeNonVisibleObjects = false;
            opt.ComputeReferences = true;
            var geomElem = _elem.get_Geometry(opt);
            return getAllSolids(geomElem);
        }
        public static void CopyParameterValuesFrom(this Element _elem, Element _source)
        {
            foreach (Parameter p1 in _elem.Parameters)
            {
                if (p1.IsReadOnly)
                    continue;
                Parameter p2;
                if (p1.IsShared)
                {
                    p2 = _source.get_Parameter(p1.GUID);
                }
                else
                {
                    p2 = _source.get_Parameter(p1.Definition);
                }
                var v1 = p1.GetMeaningfulValue();
                p2.SetValue(v1);
            }

        }
        public static Dictionary<ElementId, List<FamilyInstance>> GetHostDependentMap(Document _doc)
        {
            var dic = new Dictionary<ElementId, List<FamilyInstance>>();
            var allDpnts = new FilteredElementCollector(_doc)
              .OfClass(typeof(FamilyInstance))
              .Cast<FamilyInstance>()
              .Where(x => x.Host != null);
            foreach (var d in allDpnts)
            {
                var hostId = d.Host.Id;
                if (dic.ContainsKey(hostId) == false)
                {
                    dic[hostId] = new List<FamilyInstance>() { d };
                }
                else
                {
                    dic[hostId].Add(d);
                }
            }
            return dic;
        }
        #endregion

        #region Family
        public static List<FamilyInstance> GetAllInstances(this FamilySymbol _fs)
        {
            var doc = _fs.Document;
            var instances = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(x => x.Symbol.Id == _fs.Id)
                .ToList();
            return instances;
        }
        /// <summary>
        /// Duplicate Existing family, with given name.
        /// </summary>
        public static Family DuplicateFamily(Document _parentDoc, Document _famDoc, string _newFamName)
        {
            //save as new family into temp folder
            var opt = new SaveAsOptions();
            opt.OverwriteExistingFile = true;
            var filePath = Path.Combine(Path.GetTempPath(), _newFamName + ".rfa");
            //rename family type to match new name
            var mgr = _famDoc.FamilyManager;
            using (Transaction trans = new Transaction(_famDoc, "rename"))
            {
                trans.Start();
                mgr.RenameCurrentType(_newFamName);
                trans.Commit();
            }
            //save family and close
            _famDoc.SaveAs(filePath, opt);
            _famDoc.Close();
            //load into project doc
            Family newFam;
            using (Transaction trans = new Transaction(_parentDoc, "复制族"))
            {
                trans.Start();
                _parentDoc.LoadFamily(filePath, out newFam);
                trans.Commit();
            }
            return newFam;
        }
        /// <summary>
        /// Duplicate existing family, with input name.
        /// </summary>
        public static Family DuplicateFamily(Document _parentDoc, Document _famDoc)
        {
            try
            {
                var name = _famDoc.Title;
                var newName = InputUniqueName(_parentDoc, name);
                if (newName == "")
                    return null;
                return DuplicateFamily(_parentDoc, _famDoc, newName);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                return null;
            }
        }
        public static string InputUniqueName(Document _parentDoc, string _defaultName)
        {
            //get all existing family's names
            var names = new FilteredElementCollector(_parentDoc)
                .OfClass(typeof(Family))
                .Select(x => x.Name).ToList();

        //input new name,check name
        open_form:
            var form = new Form_SingleLineTextInput("新族名：", _defaultName);
            var result = form.ShowDialog();
            if (result == DialogResult.Cancel) return "";
            if (names.Contains(form.Input))
            {
                TaskDialog.Show("信息", "族名与现有族重复。");
                goto open_form;
            }
            //check valid file name
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
                if (invalidChars.Any(x => form.Input.Any(y => y == x)))
                {
                    TaskDialog.Show("消息", "不能使用以下字符：\r\n\r\n" + "\\<>|:*?/");
                    goto open_form;
                }
            return form.Input;
        }
        #endregion

        #region Filled Region
        public static List<ElementId> GetCurveElements(this FilledRegion _fr)
        {
            List<ElementId> list = new List<ElementId>();
            var doc = _fr.Document;
            int filledRegionId = _fr.Id.IntegerValue;
            for (int i = filledRegionId - 1; filledRegionId - 50 < i; i--)
            {
                ElementId id = new ElementId(i);
                Sketch boundary = doc.GetElement(id) as Sketch;
                if (null == boundary) continue;
                foreach (CurveArray crr in boundary.Profile)
                {
                    foreach (Curve curve in crr)
                    {
                        list.Add(curve.Reference.ElementId);
                    }
                }
                break;
            }
            return list;
        }
        #endregion

        #region Geometry_Vector
        public static UV ToUV(this XYZ _xyz)
        {
            return new UV(_xyz.X, _xyz.Y);
        }
        public static XYZ ToXYZ(UV _uv)
        {
            return new XYZ(_uv.U, _uv.V, 0);
        }
        /// <summary>
        /// Project given 3D XYZ point onto plane.
        /// </summary>
        public static XYZ ProjectOnto(this Plane plane, XYZ p)
        {
            double d = plane.SignedDistanceTo(p);
            XYZ q = p - d * plane.Normal;
            return q;
        }
        /// <summary>
        /// Project given 3D XYZ point into plane, 
        /// returning the UV coordinates of the result 
        /// in the local 2D plane coordinate system.
        /// </summary>
        public static UV ProjectInto(this Plane plane, XYZ p)
        {
            XYZ q = plane.ProjectOnto(p);
            XYZ o = plane.Origin;
            XYZ d = q - o;
            double u = d.DotProduct(plane.XVec);
            double v = d.DotProduct(plane.YVec);
            return new UV(u, v);
        }
        public static XYZ Evaluate(this Plane plane, UV uv)
        {
            var p = plane.Origin;
            p = p + plane.XVec * uv.U;
            p = p + plane.YVec * uv.V;
            return p;
        }
        /// <summary>
        /// Return signed distance from plane to a given point.
        /// </summary>
        public static double SignedDistanceTo(this Plane plane, XYZ p)
        {
            XYZ v = p - plane.Origin;
            return plane.Normal.DotProduct(v);
        }
        public static bool IsAlmostEqualTo(this XYZ _this, XYZ _other, double _e)
        {
            if (Math.Abs(_this.X - _other.X) > _e
                || Math.Abs(_this.Y - _other.Y) > _e
                || Math.Abs(_this.Z - _other.Z) > _e)
                return false;
            return true;
        }
        public static bool IsParallel(this XYZ _xyz, XYZ _xyz2)
        {
            return _xyz.CrossProduct(_xyz2).GetLength().IsAlmostEqualByDifference(0);
        }
        private static double lineIntersection(XYZ planePoint, XYZ planeNormal, XYZ linePoint, XYZ lineDirection)
        {
            if (planeNormal.DotProduct(lineDirection) == 0)
            {
                return double.MinValue;
            }
            double t = (planeNormal.DotProduct(planePoint) - planeNormal.DotProduct(linePoint)) / planeNormal.DotProduct(lineDirection);
            return t;
        }
        /// <summary>
        /// angle between two 2d vectors CCW 0 to 180 degrees
        /// </summary>
        public static double AngleToInDegree(this UV v1, UV v2)
        {
            double rad = v1.AngleTo(v2);
            return RadToDegree(rad);
        }
        public static double RadToDegree(double _d)
        {
            return _d * 360 / (2 * Math.PI);
        }
        public static double DegreeToRad(double _r)
        {
            return _r * 2 * Math.PI / 360;
        }
        public static double SqDist(this XYZ _p0, XYZ _p1)
        {
            return Math.Pow(_p1.X - _p0.X, 2)
                + Math.Pow(_p1.Y - _p0.Y, 2)
                + Math.Pow(_p1.Z - _p0.Z, 2);
        }
        public static double SqDistXY(this XYZ _p0, XYZ _p1)
        {
            return Math.Pow(_p1.X - _p0.X, 2)
                + Math.Pow(_p1.Y - _p0.Y, 2);
        }
        public static XYZ MiddlePoint(this XYZ _p1, XYZ _p2)
        {
            return (_p1 + _p2) / 2;
        }
        public static double GetSignedPolygonArea(this IList<UV> p)
        {
            int n = p.Count;
            double sum = p[0].U * (p[1].V - p[n - 1].V);
            for (int i = 1; i < n - 1; ++i)
            {
                sum += p[i].U * (p[i + 1].V - p[i - 1].V);
            }
            sum += p[n - 1].U * (p[0].V - p[n - 2].V);
            return Math.Abs(0.5 * sum);
        }
        /// <summary>
        /// Angle from this vector to the other vector, 
        /// measure the projection on XY plane. 
        /// CW from -2PI to 2PI
        /// </summary>
        /// <param name="_v0"></param>
        /// <param name="_v1"></param>
        /// <returns></returns>
        public static double AngleToOnXY(this XYZ _v0, XYZ _v1)
        {
            double angle = Math.Atan2(_v0.X, _v0.Y) - Math.Atan2(_v1.X, _v1.Y);
            return angle;
        }
        public static string ToStringDigits(this UV _uv, int _digits)
        {
            var u = Math.Round(_uv.U, _digits).ToString();
            var v = Math.Round(_uv.V, _digits).ToString();
            return u + " , " + v;
        }
        public static string ToStringDigits(this XYZ _xyz, int _digits)
        {
            var x = Math.Round(_xyz.X, _digits).ToString();
            var y = Math.Round(_xyz.Y, _digits).ToString();
            var z = Math.Round(_xyz.Z, _digits).ToString();
            return x + " , " + y + " , " + z;
        }
        public static double[] ToArrayD(this XYZ _xyz)
        {
            return new double[3] { _xyz.X, _xyz.Y, _xyz.Z };
        }
        public static List<XYZ> OrderAlong(this List<XYZ> _inputs, XYZ _dir)
        {
            //get starting and end line from far
            XYZ start = _dir * -9999;
            XYZ end = _dir * 9999;
            var line = Line.CreateBound(start, end);
            //get projected length for each point
            Dictionary<double, List<XYZ>> distMap = new Dictionary<double, List<XYZ>>();
            foreach (var p in _inputs)
            {
                var closest = p.ClosestPointOnLine(line);
                var dist = closest.SqDist(start);
                if (distMap.ContainsKey(dist))
                {
                    distMap[dist].Add(p);
                }
                else
                {
                    distMap[dist] = new List<XYZ>() { p };
                }
            }
            var ordered = distMap.OrderBy(x => x.Key);
            return ordered.SelectMany(x => x.Value).ToList();
        }
        public static XYZ GetMean(IEnumerable<XYZ> _points)
        {
            var mean = XYZ.Zero;
            int count = 0;
            foreach (var p in _points)
            {
                mean += p;
                count++;
            }
            mean /= count;
            return mean;
        }
        #endregion

        #region Geometry_Line
        public static Line ProjectOnto(this Line _line, Plane _plane)
        {
            var p0 = _plane.ProjectOnto(_line.GetEndPoint(0));
            var p1 = _plane.ProjectOnto(_line.GetEndPoint(1));
            if (p0.IsAlmostEqualTo(p1))
            {
                return null;
            }
            try
            {
                return Line.CreateBound(p0, p1);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException ex)
            {
                return null;
            }
        }
        public static List<XYZ> DivideByDistance(this Line _line, double _interval, bool _includeStart, bool _includeEnd)
        {
            List<XYZ> points = new List<XYZ>();
            var start = _line.GetEndPoint(0);
            var end = _line.GetEndPoint(1);
            if (_includeStart)
                points.Add(start);
            var dir = (end - start).Normalize() * _interval;
            var current = start;
            for (int i = 0; i < 10000; i++)
            {
                current = current + dir;
                var project = _line.Project(current);
                var rawP = project.Parameter;
                var nP = _line.ComputeNormalizedParameter(rawP);
                if (nP.IsAlmostEqualByScale(1, 0.0001))
                    break;
                else
                    points.Add(current);
            }
            if (_includeEnd)
                points.Add(end);
            return points;
        }
        public static bool IsAlmostIdentical(this Line _line, Line _other, double _e)
        {
            return
                _line.GetEndPoint(0).IsAlmostEqualTo(_other.GetEndPoint(0), _e)
               && _line.GetEndPoint(1).IsAlmostEqualTo(_other.GetEndPoint(1), _e);
        }
        public static bool IsOnExtensionOf(this Line _baseLine, Line _line)
        {
            var parallel = _baseLine.Direction.IsParallel(_line.Direction);
            if (parallel == false)
                return false;
            //check overlap is zero
            var p0 = _baseLine.GetEndPoint(0).ClosestPointOnLine(_line);
            var p1 = _baseLine.GetEndPoint(1).ClosestPointOnLine(_line);
            return p0.SqDist(p1).IsAlmostEqualByDifference(0);
        }
        /// <summary>
        /// The line stays within, or overlap, the other line's bound.
        /// </summary>
        public static bool IsInside(this Line _line, Line _testLine)
        {
            //end points both on test line
            var p0 = _line.GetEndPoint(0);
            var p1 = _line.GetEndPoint(1);
            var proj0 = _testLine.Project(p0);
            var proj1 = _testLine.Project(p1);
            if (proj0.Distance.IsAlmostEqualByDifference(0) == false
                || proj1.Distance.IsAlmostEqualByDifference(0) == false)
                return false;
            //end points both within bound
            var f0 = getProjectionParamNormalized(p0, _testLine);
            var f1 = getProjectionParamNormalized(p1, _testLine);
            if ((f0 < 0 && f0.IsAlmostEqualByDifference(0) == false)
                || (f0 > 1 && f0.IsAlmostEqualByDifference(1) == false)
                || (f1 < 0 && f1.IsAlmostEqualByDifference(0) == false)
                || (f1 > 1 && f1.IsAlmostEqualByDifference(1) == false))
            {
                return false;
            }

            return true;
        }
        public static Line ExtendBy(this Line _line, double _length)
        {
            var newEnd = _line.GetEndPoint(1) + _line.Direction * _length;
            return Line.CreateBound(_line.GetEndPoint(0), newEnd);
        }
        public static bool Intersects(this Line _line, Plane _plane)
        {
            var length = lineIntersection(_plane.Origin, _plane.Normal, _line.Origin, _line.Direction);
            if (length < 0 || length > _line.Length)
                return false;
            else
                return true;
        }
        public static Line TrimExtend(this Line _line, Plane _plane)
        {
            var length = lineIntersection(_plane.Origin, _plane.Normal, _line.Origin, _line.Direction);
            var p0 = _line.Origin;
            var p1 = p0 + _line.Direction * length;
            return Line.CreateBound(p0, p1);
        }
        /// <summary>
        /// return intersection point following, or against, a direction.
        /// </summary>
        public static XYZ GetIntersection(this Line _line, Plane _plane)
        {
            var length = lineIntersection(_plane.Origin, _plane.Normal, _line.Origin, _line.Direction);
            var p0 = _line.GetEndPoint(0);
            return p0 + _line.Direction * length;
        }
        /// <summary>
        /// shortest distance value from point to a list of lines, within line bound.
        /// </summary>
        public static double MinDistanceToLines(this XYZ _point, List<Line> _lines)
        {
            List<double> measures = new List<double>();
            foreach (var line in _lines)
            {
                measures.Add(_point.MinDistanceToLine(line));
            }
            return measures.Min();
        }
        /// <summary>
        /// distance to closest point on line, within line bound.
        /// </summary>
        public static double MinDistanceToLine(this XYZ _p, Line _line)
        {
            XYZ closest = _p.ClosestPointOnLine(_line);
            return _p.DistanceTo(closest);
        }
        public static double MinDistanceToLine(this UV _p, UVLine _line)
        {
            UV closest = _p.ClosestPointOnLine(_line);
            return _p.DistanceTo(closest);
        }
        /// <summary>
        /// find closest point on line, within line bound.
        /// </summary>
        public static XYZ ClosestPointOnLine(this XYZ _p, Line _line)
        {
            var f = getProjectionParamNormalized(_p, _line);
            if (f < 0) f = 0;
            else if (f > 1) f = 1;
            return _line.Evaluate(f, true);
        }
        private static double getProjectionParamNormalized(XYZ _p, Line _line)
        {
            XYZ p1 = _line.GetEndPoint(0);
            XYZ p2 = _line.GetEndPoint(1);
            XYZ p0 = _p;
            //https://www.codeproject.com/Questions/184403/How-to-determine-the-shortest-distance-from-a-poin
            //f = [(q-p1).(p2-p1)]÷|p2-p1|²
            double f =
                ((p0 - p1).DotProduct(p2 - p1)) / Math.Pow((p2 - p1).GetLength(), 2);
            return f;
        }
        private static double getProjectionParamNormalized(UV _uv, UVLine _line)
        {
            UV p1 = _line.p0;
            UV p2 = _line.p1;
            UV p0 = _uv;
            //https://www.codeproject.com/Questions/184403/How-to-determine-the-shortest-distance-from-a-poin
            //f = [(q-p1).(p2-p1)]÷|p2-p1|²
            double f =
                ((p0 - p1).DotProduct(p2 - p1)) / Math.Pow((p2 - p1).GetLength(), 2);
            return f;
        }
        /// <summary>
        /// projection point on line, could be outside line bound.
        /// </summary>
        public static UV ProjectOnLine(this UV _uv, UVLine _line)
        {
            var f = getProjectionParamNormalized(_uv, _line);
            return _line.Evaluate(f);
        }
        /// <summary>
        /// closest point on line, within line bound.
        /// </summary>
        public static UV ClosestPointOnLine(this UV _uv, UVLine _line)
        {
            var f = getProjectionParamNormalized(_uv, _line);
            if (f <= 0) return _line.p0;
            if (f >= 1) return _line.p1;
            else return _line.Evaluate(f);
        }
        public static Line ProjectOnto(this Line _line, Line _lineOnto)
        {
            var p1 = _line.GetEndPoint(0).ClosestPointOnLine(_lineOnto);
            var p2 = _line.GetEndPoint(1).ClosestPointOnLine(_lineOnto);
            //check length
            if (p1.IsAlmostEqualTo(p2))
                return null;
            //can still throw "segment too small" exception
            try
            {
                return Line.CreateBound(p1, p2);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException ex)
            {
                return null;
            }
        }
        public static UVLine ProjectOnto(this UVLine _line, UVLine _lineOnto)
        {
            var p1 = _line.p0.ClosestPointOnLine(_lineOnto);
            var p2 = _line.p1.ClosestPointOnLine(_lineOnto);
            //check length
            if (p1.IsAlmostEqualTo(p2))
                return null;
            else
                return new UVLine(p1, p2);
        }
        public static double MinDistanceToLine(this Line _line, Line _otherLine)
        {
            var dists = new List<double>();
            dists.Add(_line.GetEndPoint(0).MinDistanceToLine(_otherLine));
            dists.Add(_line.GetEndPoint(1).MinDistanceToLine(_otherLine));
            dists.Add(_otherLine.GetEndPoint(0).MinDistanceToLine(_line));
            dists.Add(_otherLine.GetEndPoint(1).MinDistanceToLine(_line));
            return dists.Min();
        }
        public static double MinDistanceToLine(this UVLine _line, UVLine _otherLine)
        {
            var dists = new List<double>();
            dists.Add(_line.p0.MinDistanceToLine(_otherLine));
            dists.Add(_line.p1.MinDistanceToLine(_otherLine));
            dists.Add(_otherLine.p0.MinDistanceToLine(_line));
            dists.Add(_otherLine.p1.MinDistanceToLine(_line));
            return dists.Min();
        }
        public static string ToStringDigits(this Line _line, int _digits)
        {
            return
                _line.GetEndPoint(0).ToStringDigits(_digits)
                + "||"
                + _line.GetEndPoint(1).ToStringDigits(_digits);
        }
        #endregion

        #region Geometry_Curve
        public static XYZ ClosestPointOnCurve(this XYZ _p, Curve _curveOnto)
        {
            var result1 = _curveOnto.Project(_p);
            var param1 = _curveOnto.ComputeNormalizedParameter(result1.Parameter);
            if (param1 >= 1)
            {
                return _curveOnto.GetEndPoint(1);
            }
            else if (param1 <= 0)
            {
                return _curveOnto.GetEndPoint(0);
            }
            else
            {
                return result1.XYZPoint;
            }
        }
        public static List<Transform> DivideByDist(this Curve _c, double _dist, bool _includeEnd)
        {
            List<Transform> list = new List<Transform>() { _c.ComputeDerivatives(0, true) };
            var steps = Math.Round(100 * _c.ApproximateLength / _dist);
            double inc = 1 / steps;
            double param = 0;
            double segment = 0;
            for (int i = 0; i < 9999; i++)
            {
                if (segment < _dist)
                {
                    XYZ previous = _c.Evaluate(param, true);
                    param += inc;
                    if (param > 1)
                        break;
                    XYZ current = _c.Evaluate(param, true);
                    var dist = current.DistanceTo(previous);
                    segment += dist;
                }
                else
                {
                    list.Add(_c.ComputeDerivatives(param, true));
                    segment = 0;
                }
            }
            if (_includeEnd)
            {
                list.Add(_c.ComputeDerivatives(1, true));
            }
            return list;
        }
        public static List<XYZ> SegmentationByMaxAngle(this Curve _c, double _angleInDeg)
        {
            if (_c is Line)
            {
                var l = _c as Line;
                return new List<XYZ>() { l.GetEndPoint(0), l.GetEndPoint(1) };
            }

            List<XYZ> list = new List<XYZ>();
            double maxAngle = _angleInDeg / g3.MathUtil.Rad2Deg;
            int numSteps = 300;
            double pEpsilon = 1.0d / 300.0d;
            double p = 0;
            var startDeri = _c.ComputeDerivatives(p, true);
            XYZ baseTgt = startDeri.BasisX;
            XYZ preTgt = baseTgt;
            XYZ prePoint = startDeri.Origin;
            list.Add(prePoint);
            for (int i = 1; i < numSteps; i++)
            {
                p += pEpsilon;
                var derivatives = _c.ComputeDerivatives(p, true);
                XYZ currTgt = derivatives.BasisX;
                var currAngle = currTgt.AngleTo(baseTgt);
                if (currAngle > maxAngle && i > 1)
                {
                    list.Add(prePoint);
                    baseTgt = preTgt;
                }
                preTgt = currTgt;
                prePoint = derivatives.Origin;
            }
            var end = _c.GetEndPoint(1);
            if (list.Last().IsAlmostEqualTo(end) == false)
                list.Add(end);
            return list;
        }
        #endregion

        #region Geometry_CurveLoop
        public static List<Line> SortLinesContiguous(this List<Line> _lines)
        {
            var curves = _lines.Cast<Curve>().ToList();
            bool open;
            var loop = curves.SortCurvesContiguousAsCurveLoop(out open);
            return loop.Cast<Line>().ToList();
        }
        public static List<Curve> SortCurvesContiguous(this List<Curve> _curves)
        {
            bool open;
            var cl = _curves.SortCurvesContiguousAsCurveLoop(out open);
            return cl.ToList();
        }
        public static List<Curve> SortCurvesContiguous(this List<Curve> _curves, out bool open)
        {
            var cl = _curves.SortCurvesContiguousAsCurveLoop(out open);
            return cl.Cast<Curve>().ToList();
        }
        /// <summary>
        /// Sort list of curves to be contiguous, create curveloop, 
        /// orient it to CW, return curve loop. Throw InvalidCurveLoopException
        /// if curve loop is not a single loop, or has more than two segmetns 
        /// from one point.
        /// </summary>
        /// <param name="_curves"></param>
        /// <param name="_isOpenLoop"></param>
        /// <returns></returns>
        public static CurveLoop SortCurvesContiguousAsCurveLoop(this List<Curve> _curves, out bool _isOpenLoop)
        {
            CurveLoopType type = GetLoopType(_curves);
            CurveLoop loop = new CurveLoop();
            if (type == CurveLoopType.Closed)
            {
                _isOpenLoop = false;
                loop = SortClosedLoopCurvesContiguous(_curves);
            }
            else if (type == CurveLoopType.Open)
            {
                _isOpenLoop = true;
                loop = SortOpenLoopCurvesContiguous(_curves);
            }
            else
            {
                string message = "Invalid curve loop.";
                throw new InvalidCurveLoopException(message);
            }
            if (!loop.IsOpen() && !loop.IsCounterclockwise(XYZ.Zero))
                loop.Flip();
            //orientCurveLoopToClockWise(loop);
            return loop;
        }
        public static CurveLoop SortCurveArrayContiguous(this CurveArray _ca, out bool _isOpenLoop)
        {
            List<Curve> list = _ca.ToList();
            return SortCurvesContiguousAsCurveLoop(list, out _isOpenLoop);
        }
        /// <summary>
        /// Curves need to be valid closed loop
        /// </summary>
        /// <param name="_curves"></param>
        /// <returns></returns>
        private static CurveLoop SortClosedLoopCurvesContiguous(List<Curve> _curves)
        {
            Curve curve = _curves.First();
            return CreateContiguousLoopFromCurveIterative(curve, _curves);
        }
        /// <summary>
        /// check if curves form a single loop by starting at each curve and 
        /// create curve loop, then check the max number of curves in one 
        /// loop. If is smaller than total number of curves, there is more 
        /// than one loop.
        /// </summary>
        /// <param name="_curves"></param>
        /// <returns></returns>
        public static bool IsSingleLoop(this List<Curve> _curves)
        {
            int maxCount = 0;
            foreach (Curve curve in _curves)
            {
                try
                {
                    CurveLoop loop = CreateContiguousLoopFromCurveIterative(curve, _curves);
                    int loopCount = loop.Count();
                    maxCount = Math.Max(loopCount, maxCount);
                }
                catch (InvalidCurveLoopException ex)
                {
                    return false;
                }
            }

            if (maxCount != _curves.Count)
                return false;
            else
                return true;
        }
        /// <summary>
        /// loop through all curves end points, put them 
        /// into a dictionary of list of points with coordiantes as key. 
        /// Closed loop has two curves in each value; open loop has two 
        /// values with just one curve; invalid loop might have more than 
        /// two values with just one curve, or more than two curves in 
        /// any one value.
        /// </summary>
        /// <param name="_curves"></param>
        /// <returns></returns>
        public static CurveLoopType GetLoopType(this List<Curve> _curves)
        {
            //check if is a single loop
            if (!IsSingleLoop(_curves))
                return CurveLoopType.Invalid;

            //add all end points to dictionary
            Dictionary<string, List<XYZ>> dic = new Dictionary<string, List<XYZ>>();
            foreach (Curve curve in _curves)
            {
                XYZ p0 = curve.GetEndPoint(0);
                XYZ p1 = curve.GetEndPoint(1);
                string key0 = GetApproximateCoordinatesAsString(p0, 2);
                string key1 = GetApproximateCoordinatesAsString(p1, 2);
                bool containsKey0 = dic.ContainsKey(key0);
                bool containsKey1 = dic.ContainsKey(key1);

                if (dic.ContainsKey(key0))
                {
                    var value = dic[key0];
                    value.Add(p0);
                }
                else
                {
                    dic.Add(key0, new List<XYZ>() { p0 });
                }

                if (dic.ContainsKey(key1))
                {
                    var value = dic[key1];
                    value.Add(p1);
                }
                else
                {
                    dic.Add(key1, new List<XYZ>() { p1 });
                }
            }

            //check each value
            int numberOfListWithOnePoint = 0;
            foreach (var valuePair in dic)
            {
                var list = valuePair.Value;
                int count = list.Count;
                if (count == 1)
                {
                    numberOfListWithOnePoint++;
                    if (numberOfListWithOnePoint > 2)
                    {
                        return CurveLoopType.Invalid;
                    }
                }
                else if (count > 2)
                {
                    return CurveLoopType.Invalid;
                }
            }

            if (numberOfListWithOnePoint == 0)
                return CurveLoopType.Closed;
            else
                return CurveLoopType.Open;
        }
        private static string GetApproximateCoordinatesAsString(this XYZ _xyz, int _digits)
        {
            string x = Math.Round(_xyz.X, _digits).ToString();
            string y = Math.Round(_xyz.Y, _digits).ToString();
            string z = Math.Round(_xyz.Z, _digits).ToString();
            string s = string.Format("{0},{1},{2}", x, y, z);
            return s;
        }
        private static CurveArray SortClosedCurveArrayContiguous(CurveArray _ca)
        {
            List<Curve> list = new List<Curve>();
            foreach (Curve c in _ca)
                list.Add(c);
            CurveLoop resultLoop = SortClosedLoopCurvesContiguous(list);

            CurveArray ca = new CurveArray();
            foreach (Curve c in resultLoop)
                ca.Append(c);
            return ca;
        }
        /// <summary>
        /// Curves need to be valid open loop
        /// </summary>
        /// <param name="_curves"></param>
        /// <returns></returns>
        private static CurveLoop SortOpenLoopCurvesContiguous(List<Curve> _curves)
        {
            //figure out which curve is at the starting end
            Curve startingCurve = null;
            CurveLoop loop = new CurveLoop();
            foreach (Curve thisCurve in _curves)
            {
                Curve startMatchCurve = null;
                Curve endMatchCurve = null;
                foreach (Curve nextCurve in _curves)
                {
                    //skip identical/reversal curve
                    if (CurvesAreIdentical(thisCurve, nextCurve)
                        || CurvesAreReversed(thisCurve, nextCurve))
                        continue;

                    if (thisCurve.GetEndPoint(0).IsAlmostEqualTo(nextCurve.GetEndPoint(0))
                        || thisCurve.GetEndPoint(0).IsAlmostEqualTo(nextCurve.GetEndPoint(1)))
                    {
                        startMatchCurve = nextCurve;
                    }
                    else if (thisCurve.GetEndPoint(1).IsAlmostEqualTo(nextCurve.GetEndPoint(0))
                        || thisCurve.GetEndPoint(1).IsAlmostEqualTo(nextCurve.GetEndPoint(1)))
                    {
                        endMatchCurve = nextCurve;
                    }
                }

                //skip if this curve is in the middle of loop
                if (null != startMatchCurve
                    && null != endMatchCurve)
                    continue;
                //if next curve start/end at this curve's start point
                //reverse this curve, set as start curve
                else if (null != startMatchCurve)
                {
                    startingCurve = thisCurve.CreateReversed();
                }
                //if next curve start/end at this curve's end point
                //set this curve as start curve
                else
                {
                    startingCurve = thisCurve;
                }
            }

            if (null == startingCurve)
                throw new Exception("Failed to find starting curve for open loop.");

            return CreateContiguousLoopFromCurveIterative(startingCurve, _curves);
        }
        private static CurveLoop CreateContiguousLoopFromCurveIterative(Curve _startCurve, List<Curve> _curves)
        {
            CurveLoop resultLoop = new CurveLoop();

            resultLoop.Append(_startCurve);

            List<Curve> curvesToCheck = new List<Curve>();
            curvesToCheck.AddRange(_curves);

            if (curvesToCheck.Contains(_startCurve))
                curvesToCheck.Remove(_startCurve);

            int total = curvesToCheck.Count;
            for (int i = 0; i < total; i++)
            {
                bool found = false;
                Curve thisCurve = resultLoop.Last();

                XYZ end1 = thisCurve.GetEndPoint(1);
                XYZ start1 = thisCurve.GetEndPoint(0);

                foreach (Curve c in curvesToCheck)
                {
                    if (CurvesAreIdentical(thisCurve, c)
                        || CurvesAreReversed(thisCurve, c))
                        continue;

                    XYZ start2 = c.GetEndPoint(0);
                    XYZ end2 = c.GetEndPoint(1);
                    if (end1.IsAlmostEqualTo(start2))
                    {
                        //if already found, throw exception
                        if (found)
                            throw new InvalidCurveLoopException("Invalid curve loop.");
                        //add to loop
                        resultLoop.Append(c);
                        found = true;
                    }
                    else if (end1.IsAlmostEqualTo(end2))
                    {
                        //if already found, throw exception
                        if (found)
                            throw new InvalidCurveLoopException("Invalid curve loop.");
                        //reverse next curve, add to loop
                        Curve reversed = c.CreateReversed();
                        resultLoop.Append(reversed);
                        found = true;
                    }
                }

                //if this curve is the starting curve, and its end point is not connected to other curves
                //test its start point, if find connection, reverse and replace it in result loop
                if (found == false && resultLoop.Count() == 1)
                {
                    foreach (Curve c in curvesToCheck)
                    {
                        XYZ start2 = c.GetEndPoint(0);
                        XYZ end2 = c.GetEndPoint(1);

                        if (start1.IsAlmostEqualTo(start2))
                        {
                            //revert this curve, replace start curve in result loop
                            var reversedThisCurve = thisCurve.CreateReversed();
                            resultLoop = new CurveLoop();
                            resultLoop.Append(reversedThisCurve);
                            //add to loop
                            resultLoop.Append(c);
                            found = true;
                        }
                        else if (start1.IsAlmostEqualTo(end2))
                        {
                            //revert this curve, replace start curve in result loop
                            var reversedThisCurve = thisCurve.CreateReversed();
                            resultLoop = new CurveLoop();
                            resultLoop.Append(reversedThisCurve);
                            //reverse next curve, add to loop
                            Curve reversed = c.CreateReversed();
                            resultLoop.Append(reversed);
                            found = true;
                        }
                    }
                }

                if (found && curvesToCheck.Contains(thisCurve))
                    curvesToCheck.Remove(thisCurve);
            }
            return resultLoop;
        }
        /// <summary>
        /// Checks two end points only
        /// </summary>
        /// <param name="_curve1"></param>
        /// <param name="_curve2"></param>
        /// <returns></returns>
        private static bool CurvesAreIdentical(Curve _curve1, Curve _curve2)
        {
            XYZ start1 = _curve1.GetEndPoint(0);
            XYZ end1 = _curve1.GetEndPoint(1);
            XYZ start2 = _curve2.GetEndPoint(0);
            XYZ end2 = _curve2.GetEndPoint(1);
            if (start1.IsAlmostEqualTo(start2)
                && end1.IsAlmostEqualTo(end2))
            {
                return true;
            }
            else
                return false;
        }
        private static bool CurvesAreReversed(Curve _curve1, Curve _curve2)
        {
            XYZ start1 = _curve1.GetEndPoint(0);
            XYZ end1 = _curve1.GetEndPoint(1);
            XYZ start2 = _curve2.GetEndPoint(0);
            XYZ end2 = _curve2.GetEndPoint(1);
            if (start1.IsAlmostEqualTo(end2)
                && end1.IsAlmostEqualTo(start2))
            {
                return true;
            }
            else
                return false;
        }
        /*
        internal static void orientCurveLoopToClockWise(CurveLoop _loop)
        {
            Curve curve = _loop.First();
            if (IsCurveCounterClockWise(curve))
            {
                _loop.Flip();
            }
        }
        private static bool IsCurveCounterClockWise(Curve _curve)
        {
            XYZ p0 = _curve.GetEndPoint(0);
            XYZ p1 = _curve.GetEndPoint(1);
            double angle = p0.AngleToOnXY(p1);
            if ((0 < angle && angle < Math.PI)
                || (-2 * Math.PI < angle && angle < -1 * Math.PI))
                return true;
            else
                return false;
        }
        */
        public static CurveArrArray SortClosedCurveArrArrayContiguous(this CurveArrArray _caa)
        {
            CurveArrArray caa = new CurveArrArray();
            foreach (CurveArray ca in _caa)
                caa.Append(SortClosedCurveArrayContiguous(ca));
            return caa;
        }
        public static List<Curve> ToList(this CurveArray _ca)
        {
            List<Curve> list = new List<Curve>();
            foreach (Curve c in _ca)
                list.Add(c);
            return list;
        }
        public static List<CurveLoop> ToLoopList(this CurveArrArray _caa)
        {
            List<CurveLoop> result = new List<CurveLoop>();
            foreach (CurveArray ca in _caa)
            {
                bool b;
                var loop = ca.SortCurveArrayContiguous(out b);
                result.Add(loop);
            }
            return result;
        }
        public static XYZ GetCentroid(this CurveLoop _loop)
        {
            XYZ centroid = XYZ.Zero;
            foreach (Curve c in _loop)
            {
                centroid += c.GetEndPoint(0);
            }
            centroid /= _loop.Count();
            return centroid;
        }
        #endregion

        #region Geometry_Solid
        /// <summary>
        /// Return multiple if any error.
        /// </summary>
        public static List<Solid> UnionAll(this List<Solid> _solids)
        {
            if (_solids.Count == 0 || _solids.Count == 1)
                return _solids;
            List<Solid> list = new List<Solid>();
            list.Add(_solids[0]);
            //union each solid against each solid in result list
            //if succeed, replace result solid with a union solid
            //if fail, add new solid into result list
            for (int i = 1; i < _solids.Count; i++)
            {
                var s2 = _solids[i]; //solid in input list
                bool succeeded = false;
                for (int j = 0; j < list.Count; j++)
                {
                    var s1 = list[j]; //solid in result list
                    try
                    {
                        list[j] = BooleanOperationsUtils.ExecuteBooleanOperation
                        (s1, s2, BooleanOperationsType.Union);
                        //flag
                        succeeded = true;
                    }
                    //failed to union, continue
                    catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
                    {
                        continue;
                    }
                }
                //if not found intersection, add new solid to result list
                if (!succeeded)
                {
                    list.Add(s2);
                }
            }
            return list;
        }

        ///https://forums.autodesk.com/t5/revit-api-forum/hot-to-knows-if-a-point-is-inside-a-mass-and-or-solid/td-p/8570689
        ///points on the surface is considered to be inside
        public static bool IsInsideSolid(this XYZ point, Solid solid)
        {
            SolidCurveIntersectionOptions sco = new SolidCurveIntersectionOptions();
            sco.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;

            //for some reason, the direction of increment vector could affect result
            //for now, basis X works
            Line line1 = Line.CreateBound(point, point.Add(new XYZ(1, 0, 0)));
            Line line2 = Line.CreateBound(point, point.Add(new XYZ(-1, 0, 0)));

            double tolerance = 0.000001;

            SolidCurveIntersection sci1 = solid.IntersectWithCurve(line1, sco);
            SolidCurveIntersection sci2 = solid.IntersectWithCurve(line2, sco);

            bool inside1 = false;
            bool inside2 = false;

            for (int i = 0; i < sci1.SegmentCount; i++)
            {
                Curve c = sci1.GetCurveSegment(i);

                if (point.IsAlmostEqualTo(c.GetEndPoint(0), tolerance) || point.IsAlmostEqualTo(c.GetEndPoint(1), tolerance))
                {
                    inside1 = true;
                }
            }

            for (int i = 0; i < sci2.SegmentCount; i++)
            {
                Curve c = sci2.GetCurveSegment(i);

                if (point.IsAlmostEqualTo(c.GetEndPoint(0), tolerance) || point.IsAlmostEqualTo(c.GetEndPoint(1), tolerance))
                {
                    inside2 = true;
                }
            }

            if (inside1 || inside2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// points on the surface is considered to be outside
        /// </summary>
        public static bool IsOutsideSolid(this XYZ point, Solid solid)
        {
            SolidCurveIntersectionOptions sco = new SolidCurveIntersectionOptions();
            sco.ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside;

            //for some reason, the direction of increment vector could affect result
            //for now, 1 1 1 works
            Line line1 = Line.CreateBound(point, point.Add(new XYZ(1, 1, 1)));
            Line line2 = Line.CreateBound(point, point.Add(new XYZ(-1, -1, -1)));

            double tolerance = 0.000001;

            SolidCurveIntersection sci1 = solid.IntersectWithCurve(line1, sco);
            SolidCurveIntersection sci2 = solid.IntersectWithCurve(line2, sco);
            bool outside1 = false;
            bool outside2 = false;

            for (int i = 0; i < sci1.SegmentCount; i++)
            {
                Curve c = sci1.GetCurveSegment(i);

                if (point.IsAlmostEqualTo(c.GetEndPoint(0), tolerance) || point.IsAlmostEqualTo(c.GetEndPoint(1), tolerance))
                {
                    outside1 = true;
                }
            }

            for (int i = 0; i < sci2.SegmentCount; i++)
            {
                Curve c = sci2.GetCurveSegment(i);

                if (point.IsAlmostEqualTo(c.GetEndPoint(0), tolerance) || point.IsAlmostEqualTo(c.GetEndPoint(1), tolerance))
                {
                    outside2 = true;
                }
            }

            if (outside1 || outside2)
                return true;
            else
                return false;
        }
        #endregion

        #region Geometry_Face
        public static XYZ ClosestPointOnFace(this XYZ _p0, Face _face)
        {
            var result = _face.Project(_p0);
            if (result != null)
            {
                return result.XYZPoint;
            }
            else
            {
                //loop all edges, find closest point on them
                double minDist = double.MaxValue;
                XYZ closest = null;
                foreach (var loop in _face.GetEdgesAsCurveLoops())
                {
                    foreach (Curve c in loop)
                    {
                        var resultCurve = c.Project(_p0);
                        if (resultCurve != null)
                            return resultCurve.XYZPoint;
                        //result shouldn't be null, below are just in case
                        else
                        {
                            //get either of two end points
                            var end0 = c.GetEndPoint(0);
                            var dist0 = _p0.SqDist(end0);
                            var end1 = c.GetEndPoint(1);
                            var dist1 = _p0.SqDist(end1);
                            if (dist0 < minDist)
                            {
                                minDist = dist0;
                                closest = end0;
                            }
                            if (dist1 < minDist)
                            {
                                minDist = dist1;
                                closest = end1;
                            }
                        }
                    }
                }
                return closest;
            }
        }
        public static XYZ GetCentroid(this Face _face)
        {
            var boxUV = _face.GetBoundingBox();
            var centroidUV = (boxUV.Max + boxUV.Min) / 2;
            return _face.Evaluate(centroidUV);
        }
        public static PlanarFace Push(this PlanarFace _face, double _distance)
        {
            var curveLoop = _face.GetEdgesAsCurveLoops().First();
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry
                (new List<CurveLoop>() { curveLoop },
                _face.FaceNormal, _distance);
            return solid.Faces.get_Item(0) as PlanarFace;
        }
        #endregion

        #region Group
        public static XYZ GetLocationPoint(this Group _group)
        {
            return ((LocationPoint)_group.Location).Point;
        }
        public static IEnumerable<Group> GetAllInstances(this GroupType _gt)
        {
            var doc = _gt.Document;
            var instances = new FilteredElementCollector(doc)
                .OfClass(typeof(Group))
                .Cast<Group>()
                .Where(x => x.GroupType.Id == _gt.Id);
            return instances;
        }
        public static bool MemberOrderConsistent(Group _g1, Group _g2)
        {
            var doc = _g1.Document;
            var ids1 = _g1.GetMemberIds();
            var ids2 = _g2.GetMemberIds();
            for (int i = 0; i < ids1.Count; i++)
            {
                var id1 = ids1[i];
                var id2 = ids2[i];
                var elem1 = doc.GetElement(id1);
                if (elem1 is SketchPlane)
                    continue;
                var elem2 = doc.GetElement(id2);
                if (elem1.Name != elem2.Name)
                    return false;
            }
            return true;
        }
        public static bool CheckModelCompliancy(Document _doc)
        {
            var grouptypes = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_IOSModelGroups)
                .WhereElementIsElementType()
                .Cast<GroupType>();

            //check nested group
            bool noNestedGroup = true;
            string s1 =
                "以下模型组内发现嵌套模型组。清除嵌套组之后才能正常修改组内构件。"
                + Environment.NewLine + Environment.NewLine
                + "模型组："
                + Environment.NewLine + Environment.NewLine;

            foreach (var gt in grouptypes)
            {
                var ids = gt.Groups;
                var g = gt.Groups.Cast<Group>().FirstOrDefault();
                if (g == null)
                    continue;
                foreach (var memberId in g.GetMemberIds())
                {
                    var member = _doc.GetElement(memberId);
                    if (member is Group)
                    {
                        noNestedGroup = false;
                        s1 += g.Name + "  ID: " + g.Id.ToString()
                             + Environment.NewLine;
                        break;
                    }
                }
            }

            if (!noNestedGroup)
            {
                TaskDialog.Show("消息", s1);
                return false;
            }

            //check member order consistency
            bool allConsistent = true;
            string s2 =
                "以下模型组的成员顺序不统一，插件将会忽略这些组的成员修改。重新编组可以解决这个问题。"
                + Environment.NewLine + Environment.NewLine
                + "模型组："
                + Environment.NewLine + Environment.NewLine;

            using (Transaction trans = new Transaction(_doc, "检查组"))
            {
                trans.Start();
                foreach (var gt in grouptypes)
                {
                    var ids = gt.Groups;
                    var g = gt.Groups.Cast<Group>().FirstOrDefault();
                    if (g == null)
                        continue;
                    var newInstance = _doc.Create.PlaceGroup(new XYZ(500, 500, 500), gt);
                    var consistent = MemberOrderConsistent(g, newInstance);
                    _doc.Delete(newInstance.Id);
                    if (!consistent)
                    {
                        allConsistent = false;
                        s2 += g.Name + "  ID: " + g.Id.ToString()
                             + Environment.NewLine;
                    }
                }
                trans.RollBack();
            }

            if (!allConsistent)
            {
                TaskDialog.Show("消息", s2);
                return false;
            }

            TaskDialog.Show("消息", "未发现模型组异常， 可以正常使用插件。");
            return true;
        }
        #endregion

        #region Material
        public static Material TransferMaterial(this Material _source, Document _targetDoc)
        {
            string name = _source.Name;
            Material targetMat = null;
            if (!Material.IsNameUnique(_targetDoc, name))
            {
                targetMat = new FilteredElementCollector(_targetDoc)
                    .OfClass(typeof(Material))
                    .First(x => x.Name == name) as Material;
            }
            else
            {
                var targetMatId = Material.Create(_targetDoc, name);
                targetMat = _targetDoc.GetElement(targetMatId) as Material;
            }
            copyMaterial(_source, targetMat);
            return targetMat;
        }
        private static void copyMaterial(Material _source, Material _target)
        {
            //no need to include everything since matieral inside family will be overridden by 
            //material of the same name inside project.
            _target.Color = _source.Color;
            _target.CutBackgroundPatternColor = _source.CutBackgroundPatternColor;
            _target.CutForegroundPatternColor = _source.CutForegroundPatternColor;
            _target.MaterialCategory = _source.MaterialCategory;
            _target.MaterialClass = _source.MaterialClass;
            _target.Shininess = _source.Shininess;
            _target.Smoothness = _source.Smoothness;
            _target.SurfaceBackgroundPatternColor = _source.SurfaceBackgroundPatternColor;
            _target.SurfaceForegroundPatternColor = _source.SurfaceForegroundPatternColor;
            _target.Transparency = _source.Transparency;
            _target.UseRenderAppearanceForShading = _source.UseRenderAppearanceForShading;
        }
        #endregion

        #region Parameter
        /// <summary>
        /// Read parameter value in its native storage data type.
        /// </summary>
        public static object GetValue(this Parameter _p)
        {
            if (_p == null || !_p.HasValue)
            {
                //family type parameter could return -1 id and Not has value
                if (_p.StorageType == StorageType.ElementId)
                    return ElementId.InvalidElementId;
                else
                    return null;
            }

            switch (_p.StorageType)
            {
                case StorageType.None:
                    {
                        return null;
                    }
                case StorageType.ElementId:
                    {
                        return _p.AsElementId();
                    }
                case StorageType.Integer:
                    {
                        return _p.AsInteger();
                    }
                case StorageType.String:
                    {
                        return _p.AsString();
                    }
                case StorageType.Double:
                    {
                        return _p.AsDouble();
                    }
                default:
                    {
                        return "< Error >";
                    }
            }
        }
        /// <summary>
        /// Get the meaningful value from parameter.
        /// Return Element if storage type is ElementId
        /// Return "error" text if any exception.
        /// Some value retrieved this way could not be use to set paramter value.
        /// If setting value is priority, use "GetValue" instead.
        /// </summary>
        /// <param name="_p"></param>
        /// <returns></returns>
        public static object GetMeaningfulValue(this Parameter _p)
        {
            if (_p == null || !_p.HasValue)
            {
                //family type parameter could return -1 id and Not has value
                if (_p.StorageType == StorageType.ElementId)
                    return ElementId.InvalidElementId;
                else
                    return null;
            }

            switch (_p.StorageType)
            {
                case StorageType.None:
                    {
                        return null;
                    }
                case StorageType.ElementId:
                    {
                        var doc = _p.Element.Document;
                        var id = _p.AsElementId();
                        //some id parameter does not return value
                        if (id == null)
                            return _p.AsValueString();
                        //key parameter can return -1 id and has value
                        //showing "(none)" on UI
                        if (id.IntegerValue == -1)
                            return id;
                        else
                        {
                            var elem = doc.GetElement(id);
                            if (elem == null)
                                return _p.AsValueString();
                            else return elem;
                        }
                    }
                case StorageType.Integer:
                    {
                        return _p.AsValueString();
                    }
                case StorageType.String:
                    {
                        return _p.AsString();
                    }
                case StorageType.Double:
                    {
                        return _p.AsValueString(); //value with unit if applicable
                    }
                default:
                    {
                        return "< Error >";
                    }
            }
        }
        public static void SetValue(this Parameter p, object value)
        {
            //special case for key parameter
            if (p.StorageType == StorageType.ElementId
                && value is ElementId)
            {
                var id = ((ElementId)value).IntegerValue;
                if (id == -1)
                {
                    p.Set((ElementId)value);
                    return;
                }
            }

            if (value == null)
                value = "";

            if (value.GetType().Equals(typeof(string)))
            {
                if (p.SetValueString(value as string))
                    return;
            }

            switch (p.StorageType)
            {
                case StorageType.None:
                    break;
                case StorageType.Double:
                    if (value is string)
                    {
                        var valueString = value as string;
                        if (string.IsNullOrEmpty(valueString))
                            p.Set(0.0);
                        else
                            p.Set(double.Parse(valueString));
                    }
                    else
                    {
                        p.Set(Convert.ToDouble(value));
                    }
                    break;
                case StorageType.Integer:
                    if (value is string)
                    {
                        if (p.Definition.ParameterType == ParameterType.YesNo)
                        {
                            string s = (string)value;
                            var b = s.ToBoolean();
                            if (b) p.Set(1);
                            else p.Set(0);
                        }
                        else
                            p.Set(int.Parse(value as string));
                    }
                    else
                    {
                        p.Set(Convert.ToInt32(value));
                    }
                    break;
                case StorageType.ElementId:
                    if (value is ElementId)
                    {
                        p.Set(value as ElementId);
                    }
                    else if (value is string)
                    {
                        p.Set(new ElementId(int.Parse(value as string)));
                    }
                    else if (value is Element)
                    {
                        p.Set(((Element)value).Id);
                    }
                    else
                    {
                        p.Set(new ElementId(Convert.ToInt32(value)));
                    }
                    break;
                case StorageType.String:
                    p.Set(value.ToString());
                    break;
            }
        }
        public static string GetId(this Parameter p)
        {
            string id = p.GetUniqueId();
            if (id == null)
                id = p.Id.ToString();
            return id;
        }
        public static string GetId(this FamilyParameter p)
        {
            string id = p.GetUniqueId();
            if (id == null)
                id = p.Id.ToString();
            return id;
        }
        /// <summary>
        /// one method for all types of parameters. 
        /// Project parameter does not have a unique identity, return null;
        /// </summary>
        public static string GetUniqueId(this Parameter p)
        {
            //shared parameter
            if (p.IsShared)
                return p.GUID.ToString();
            var iDef = p.Definition as InternalDefinition;
            //built-in parameter
            if (iDef.BuiltInParameter != BuiltInParameter.INVALID)
                return p.Id.ToString();
            //project parameter dose not have a unique identifier
            else
                return null;
        }
        /// <summary>
        /// family parameter, the same as parameter.
        /// </summary>
        public static string GetUniqueId(this FamilyParameter p)
        {
            //shared parameter
            if (p.IsShared)
                return p.GUID.ToString();
            var iDef = p.Definition as InternalDefinition;
            //built-in parameter
            if (iDef.BuiltInParameter != BuiltInParameter.INVALID)
                return p.Id.ToString();
            //project parameter dose not have a unique identifier
            else
                return null;
        }
        public static Parameter GetParameter(this Element _elem, string _id)
        {
            //try built_in parameter
            BuiltInParameter bip;
            var tryBip = Enum.TryParse<BuiltInParameter>(_id, out bip);
            if (tryBip)
            {
                return _elem.get_Parameter(bip);
            }
            //try shared parameter
            Guid guid;
            var tryGuid = Guid.TryParse(_id, out guid);
            if (tryGuid)
            {
                return _elem.get_Parameter(guid);
            }
            //loop through parameters, find the correct id number
            foreach (Parameter p in _elem.Parameters)
            {
                if (p.Definition is InternalDefinition || p.IsShared)
                    continue;
                if (p.Id.ToString() == _id)
                    return p;
            }
            return null;
        }
        public static Parameter GetParameterByName(this Element _elem, string _name)
        {
            foreach (Parameter p in _elem.Parameters)
            {
                if (p.Definition.Name == _name)
                    return p;
            }
            return null;
        }
        #endregion

        #region Polygon
        public static double GetArea(this List<XYZ> _points)
        {
            var intrPs = _points.Select(x => x.ToClipperPoint()).ToList();
            var area = Clipper.Area(intrPs);
            return area / Math.Pow(ClipperConverter._feet_to_mm, 2);
        }
        public static IntPoint ToClipperPoint(this XYZ _xyz)
        {
            return ClipperConverter.GetIntPoint(_xyz);
        }
        public static XYZ ToXYZ(this IntPoint _p)
        {
            return ClipperConverter.GetXyzPoint(_p);
        }
        public static double GetArea(this List<Curve> _boundary, Document _doc)
        {
            var v0 = (_boundary[0].GetEndPoint(1) - _boundary[0].GetEndPoint(0)).Normalize();
            var c1 = _boundary
                .First(x => (x.GetEndPoint(1) - x.GetEndPoint(0)).Normalize().IsAlmostEqualTo(v0) == false);
            var v1 = (c1.GetEndPoint(1) - c1.GetEndPoint(0)).Normalize();
            var dir = v0.CrossProduct(v1).Normalize();
            var cls = new List<CurveLoop>() { CurveLoop.Create(_boundary) };
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(cls, dir, 1);
            var face = solid.Faces.get_Item(0);
            return face.Area;
        }
        #endregion

        #region Polygon_insideOutside
        /// <summary>
        /// 不包含边界。线圈须为连续线段。
        /// </summary>
        public static bool IsInsidePolygon(this XYZ _XYZ, List<Line> _lines)
        {
            //投形到XY平面
            _XYZ = new XYZ(_XYZ.X, _XYZ.Y, 0);
            _lines = _lines
                .Select(x => x.ProjectToXY())
                .ToList();

            bool inOrOn = _XYZ.IsInsideOrOnEdgeOfPolygon(_lines);//第一步，判断点在总多边形内部部还是外部，在为true，不在为false；当前判断无法剔除，点落在多边形边界线上的情况，
            bool on = _XYZ.OnAnyLine(_lines);//第二步，该函数判断点在不在边界线上，在为true，不在为false；
            return inOrOn && !on; //双重判断，不在线上，在多边形区域内
        }
        public static XYZ ProjectToXY(this XYZ p)
        {
            return new XYZ(p.X, p.Y, 0);
        }
        public static Line ProjectToXY(this Line l)
        {
            var p0 = l.GetEndPoint(0).ProjectToXY();
            var p1 = l.GetEndPoint(1).ProjectToXY();
            return Line.CreateBound(p0, p1);
        }
        /// <summary>
        /// 包含边界。线圈须为连续线段。
        /// </summary>
        public static bool IsInsideOrOnEdgeOfPolygon(this XYZ _XYZ, List<Line> _lines)
        {
            bool inOrOn = false;
            int intersectCount = 0;

            //投形到XY平面
            _XYZ = _XYZ.ProjectToXY();
            _lines = _lines.Select(x => x.ProjectToXY()).ToList();

            Line _LInebound = Line.CreateBound(_XYZ, new XYZ(_XYZ.X + 100000, _XYZ.Y, 0));//求一个点的射线
            foreach (Line _Line in _lines)
            {
                IntersectionResultArray results;
                SetComparisonResult result = _LInebound.Intersect(_Line, out results);
                if (result == SetComparisonResult.Overlap)//判断基准线是否与轴网相交
                {
                    if (results != null)
                    {
                        XYZ _LineendPoint_0 = _Line.GetEndPoint(0);
                        XYZ _LineendPoint_1 = _Line.GetEndPoint(1);

                        //下一步 判定假设 参看文章 https://blog.csdn.net/u283056051/article/details/53980925

                        if ((_LineendPoint_0.Y < _XYZ.Y && _LineendPoint_1.Y >= _XYZ.Y) || (_LineendPoint_0.Y > _XYZ.Y && _LineendPoint_1.Y <= _XYZ.Y))
                        {
                            intersectCount += results.Size;
                        }
                    }
                }
            }
            if (intersectCount % 2 != 0)//判断交点的数量是否为奇数或者偶数，奇数为内true，偶数为外false
            {
                inOrOn = true;
            }
            return inOrOn;
        }
        /// <summary>
        /// 判断一个点是不是与Polygon边界重合。
        /// </summary>
        public static bool OnAnyLine(this XYZ _XYZ, IList<Line> _lines)
        {
            bool _isOnLine = false;
            foreach (Line _Line in _lines)
            {
                if (_Line.Distance(_XYZ) < 0.003)//该处需要注意 Revit2020版本中 曲线长度的最小极限小值为 0.00256026455729167 Feet 0.7803686370625 MilliMeter
                {
                    _isOnLine = true;
                    break;
                }
            }
            return _isOnLine;
        }
        #endregion

        #region Polygon_booleanWithClipper
        public static List<CurveLoop> UnionAll(this List<CurveLoop> _loops)
        {
            var paths = _loops.Select(x => GetPathFromCurveLoop(x)).ToList();
            Clipper clipper = new Clipper();
            clipper.AddPaths(paths, PolyType.ptSubject, true);
            PolyTree resultTree = new PolyTree();
            clipper.Execute(ClipType.ctUnion, resultTree, PolyFillType.pftNonZero);
            var caa = CurveArrArrayFromPolyTree(resultTree, true);

            List<CurveLoop> result = new List<CurveLoop>();
            foreach (CurveArray ca in caa)
            {
                var loop = new CurveLoop();
                foreach (Curve c in ca)
                {

                    try { loop.Append(c); }
                    catch (Autodesk.Revit.Exceptions.ArgumentException ex) { }
                }
                result.Add(loop);
            }
            return result;
        }

        /// <summary>
        /// Extension method for CurveLoop class. clip operation on two curve loops,
        /// using clipper library. Input loops needs to be contiguous.
        /// </summary>
        public static List<CurveLoop> ClipBy(this CurveLoop _subLoop, CurveLoop _cutLoop, ClipType _clipType)
        {
            List<CurveLoop> result = new List<CurveLoop>();

            var clipPath = GetPathFromCurveLoop(_cutLoop);
            var subPath = GetPathFromCurveLoop(_subLoop);
            if (!_subLoop.IsOpen())
                subPath.Add(subPath.First());

            Clipper clipper = new Clipper();
            clipper.AddPath(subPath, PolyType.ptSubject, false); //sub path needs to be set as open to excute as curve, not polygon
            clipper.AddPath(clipPath, PolyType.ptClip, true);

            PolyTree resultTree = new PolyTree();
            clipper.Execute(_clipType, resultTree, PolyFillType.pftEvenOdd);
            var caa = CurveArrArrayFromPolyTree(resultTree, false);

            //sort each curve array to be contiguous
            foreach (CurveArray ca in caa)
            {
                var curves = ca.ToList();
                bool isOpen;
                var loop = curves.SortCurvesContiguousAsCurveLoop(out isOpen);
                result.Add(loop);
            }

            return result;
        }

        public static List<CurveLoop> ClipBy(this IList<CurveLoop> _subLoops, CurveLoop _cutLoop, ClipType _clipType)
        {
            List<CurveLoop> result = new List<CurveLoop>();
            foreach (CurveLoop cl in _subLoops)
            {
                result.AddRange(cl.ClipBy(_cutLoop, _clipType));
            }
            return result;
        }

        /// <summary>
        /// Cut a set of open/closed curveloops by a another set of closed curveloops. 
        /// Return a list of list of curveloops of the same structure of the input 
        /// subject curveloops.
        /// </summary>
        public static List<List<CurveLoop>> ClipBy
            (this IList<CurveLoop> _subLoops, IList<CurveLoop> _cutLoops, ClipType _clipType)
        {
            List<List<CurveLoop>> result = new List<List<CurveLoop>>();

            var clipPaths = new List<List<IntPoint>>();
            foreach (var cl in _cutLoops)
            {
                clipPaths.Add(GetPathFromCurveLoop(cl));
            }
            var subPaths = new List<List<IntPoint>>();
            foreach (var cl in _subLoops)
            {
                subPaths.Add(GetPathFromCurveLoop(cl));
                if (!cl.IsOpen()) //if subject loop is closed loop, add the starting point at the end 
                    subPaths.Last().Add(subPaths.Last().First());
            }

            foreach (var subPath in subPaths)
            {
                Clipper clipper = new Clipper();
                //bool subClosed = !_subLoops.First().IsOpen();
                bool cutClosed = !_cutLoops.First().IsOpen();
                clipper.AddPath(subPath, PolyType.ptSubject, false); //sub path needs to be set as open to excute as curve, not polygon
                clipper.AddPaths(clipPaths, PolyType.ptClip, cutClosed);

                PolyTree resultTree = new PolyTree();
                clipper.Execute(_clipType, resultTree, PolyFillType.pftEvenOdd);

                var caa = CurveArrArrayFromPolyTree(resultTree, false);
                var loopList = caa.ToLoopList();
                result.Add(loopList);
            }

            return result;
        }

        /// <summary>
        /// clip a list of lists of curveloops with a list of curveloops
        /// return a list of lists of curveloops that maintain the structure of first dimension
        /// </summary>
        public static List<List<CurveLoop>> ClipBy
            (this IList<List<CurveLoop>> _subListsOfLoops,
            IList<CurveLoop> _cutLoops,
            ClipperLib.ClipType _clipType)
        {
            List<List<CurveLoop>> result = new List<List<CurveLoop>>();
            foreach (var curveLoopList in _subListsOfLoops)
            {
                var clipResult = curveLoopList.ClipBy(_cutLoops, _clipType);
                //flatten result to one-dimension list
                var newCurveLoopList = new List<CurveLoop>();
                foreach (var list in clipResult)
                {
                    foreach (var cl in list)
                        newCurveLoopList.Add(cl);
                }
                //add this row's baselines to level
                result.Add(newCurveLoopList);
            }
            return result;
        }

        /// <summary>
        /// Extension method for CurveLoop class.
        /// </summary>
        /// <param name="_loop"></param>
        /// <returns></returns>
        private static List<IntPoint> GetPathFromCurveLoop(this CurveLoop _loop)
        {
            List<IntPoint> list = new List<IntPoint>();
            List<XYZ> vertices = GetVerticesFromCurveLoop(_loop);
            foreach (XYZ p in vertices)
            {
                IntPoint ip = ClipperConverter.GetIntPoint(p);
                list.Add(ip);
            }
            return list;
        }

        private static List<XYZ> GetVerticesFromCurveLoop(CurveLoop _loop)
        {
            List<XYZ> list = new List<XYZ>();
            foreach (var curve in _loop)
            {
                list.Add(curve.GetEndPoint(0));
            }
            if (_loop.IsOpen())
            {
                list.Add(_loop.Last().GetEndPoint(1));
            }
            return list;
        }

        private static CurveArrArray CurveArrArrayFromPolyTree(PolyTree _tree, bool _closed)
        {
            CurveArrArray caa = new CurveArrArray();
            PolyNode polynode = _tree.GetFirst();
            while (null != polynode)
            {
                List<XYZ> list = new List<XYZ>();
                foreach (var ip in polynode.Contour)
                {
                    XYZ p = ClipperConverter.GetXyzPoint(ip);
                    list.Add(p);
                }
                list = cleanUpOverlaps(list);
                CurveArray ca = CurveArrayFromListOfPoints(list, _closed);
                if (ca.Size > 0)
                    caa.Append(ca);
                polynode = polynode.GetNext();
            }
            return caa;
        }

        private static CurveArray CurveArrayFromListOfPoints(List<XYZ> _points, bool _closed)
        {
            CurveArray ca = new CurveArray();
            int n = _points.Count;
            for (int i = 0; i < n - 1; i++)
            {
                XYZ p0 = _points[i];
                XYZ p1 = _points[i + 1];
                try
                {
                    Line line = Line.CreateBound(p0, p1);
                    ca.Append(line);
                }
                catch (ArgumentsInconsistentException ex)
                {
                    continue;
                }
            }
            if (_closed && _points.Last().IsAlmostEqualTo(_points.First()) == false)
            {
                Line line = null;
                try
                {
                    line = Line.CreateBound(_points.Last(), _points.First());
                    ca.Append(line);
                }
                catch { }
            }
            return ca;
        }

        private static List<XYZ> cleanUpOverlaps(List<XYZ> _input)
        {
            List<XYZ> output = new List<XYZ>();
            if (_input.Count == 0)
                return output;
            output.Add(_input[0]);
            for (int i = 1; i < _input.Count - 1; i++)
            {
                var p0 = _input[i - 1];
                var p1 = _input[i];
                var p2 = _input[i + 1];
                var dir0 = (p0 - p1).Normalize();
                var dir1 = (p2 - p1).Normalize();
                if (dir0.IsAlmostEqualTo(dir1) == false)
                    output.Add(p1);
            }
            output.Add(_input.Last());
            return output;
        }

        public static class ClipperConverter
        {
            /// <summary>
            /// Consider a Revit length zero 
            /// if is smaller than this.
            /// </summary>
            public const double _eps = 1.0e-4;

            /// <summary>
            /// Conversion factor from feet to millimetres.
            /// </summary>
            public const double _feet_to_mm = 25.4 * 12 * 10; //multiplier should not be too big

            /// <summary>
            /// Conversion a given length value 
            /// from feet to millimetres.
            /// </summary>
            public static long ConvertFeetToMillimetres(double d)
            {
                return (long)(_feet_to_mm * d);

                if (0 < d)
                {
                    return _eps > d
                      ? 0
                      : (long)(_feet_to_mm * d + 0.5);

                }
                else
                {
                    return _eps > -d
                      ? 0
                      : (long)(_feet_to_mm * d - 0.5);

                }

            }

            /// <summary>
            /// Conversion a given length value 
            /// from millimetres to feet.
            /// </summary>
            static double ConvertMillimetresToFeet(long d)
            {
                return d / _feet_to_mm;
            }

            /// <summary>
            /// Return a clipper integer point 
            /// from a Revit model space one.
            /// Do so by dropping the Z coordinate
            /// and converting from imperial feet 
            /// to millimetres.
            /// </summary>
            public static IntPoint GetIntPoint(XYZ p)
            {
                return new IntPoint(
                  ConvertFeetToMillimetres(p.X),
                  ConvertFeetToMillimetres(p.Y));
            }

            /// <summary>
            /// Return a Revit model space point 
            /// from a clipper integer one.
            /// Do so by adding a zero Z coordinate
            /// and converting from millimetres to
            /// imperial feet.
            /// </summary>
            public static XYZ GetXyzPoint(IntPoint p)
            {
                return new XYZ(
                  ConvertMillimetresToFeet(p.X),
                  ConvertMillimetresToFeet(p.Y),
                  0.0);
            }
        }
        #endregion

        #region Test
        public static void ShowOrigin(Document _doc)
        {
            using (Transaction trans = new Transaction(_doc, "show origin"))
            {
                trans.Start();
                var lineX = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 0, 0));
                var lineY = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 1, 0));
                DirectShape ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(new List<GeometryObject>() { lineX, lineY });
                trans.Commit();
            }
        }
        /// <summary>
        /// need open transaction.
        /// </summary>
        public static void CreateDirectShape(Document _doc, List<GeometryObject> _geometry, ref DirectShape _ds)
        {
            _ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
            _ds.SetShape(_geometry);
        }
        /// <summary>
        /// need open transaction.
        /// </summary>
        public static void CreateDirectShape(Document _doc, List<GeometryObject> _geometry)
        {
            DirectShape ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
            ds.SetShape(_geometry);
        }
        /// <summary>
        /// need open transaction.
        /// </summary>
        public static void CreateDirectShape(Document _doc, List<GeometryObject> _geometry, XYZ _translation)
        {
            DirectShape ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
            ds.SetShape(_geometry);
            ElementTransformUtils.MoveElement(ds.Document, ds.Id, _translation);
        }
        #endregion

        #region Topo
        public static List<Face> GetFaces(this TopographySurface _topo, Autodesk.Revit.DB.View _view)
        {
            List<Face> list = new List<Face>();
            //get all triangles
            var opt = new Options();
            opt.View = _view;
            opt.IncludeNonVisibleObjects = false;
            var geoElem = _topo.get_Geometry(new Options());
            Mesh mesh = null;
            foreach (var go in geoElem)
            {
                if (go is Mesh)
                {
                    mesh = go as Mesh;
                    break;
                }
            }
            if (mesh == null)
                return list;

            for (int i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                //get curve loops
                var curves = new List<Curve>();
                curves.Add(Line.CreateBound(triangle.get_Vertex(0), triangle.get_Vertex(1)));
                curves.Add(Line.CreateBound(triangle.get_Vertex(1), triangle.get_Vertex(2)));
                curves.Add(Line.CreateBound(triangle.get_Vertex(2), triangle.get_Vertex(0)));
                var curveloop = CurveLoop.Create(curves);

                //get normal
                var plane = curveloop.GetPlane();
                var normal = plane.Normal;

                //extrude inward
                var solid = GeometryCreationUtilities.CreateExtrusionGeometry
                    (new List<CurveLoop>() { curveloop },
                    -1 * normal, 1);

                var face = solid.Faces.get_Item(1);
                list.Add(face);
            }

            return list;
        }
        #endregion

        #region Transform
        public static Transform GetTransform(Group _refGroup, Group _targetGroup)
        {
            var refIds = _refGroup.GetMemberIds();
            var targetIds = _targetGroup.GetMemberIds();
            var doc = _refGroup.Document;
            for (int i = 0; i < refIds.Count; i++)
            {
                var refElem = doc.GetElement(refIds[i]);
                if (refElem is Wall)
                {
                    var refWall = refElem as Wall;
                    var targetWall = doc.GetElement(targetIds[i]) as Wall;
                    return GetTransform(refWall, targetWall);
                }
                else if (refElem is FamilyInstance)
                {
                    var refInstance = refElem as FamilyInstance;
                    var targetInstance = doc.GetElement(targetIds[i]) as FamilyInstance;
                    return GetTransform(refInstance, targetInstance);
                }
            }
            return null;
        }
        public static Transform GetTransform(Wall _refWall, Wall _targetWall)
        {
            var refT = GetTransform(_refWall);
            var targetT = GetTransform(_targetWall);
            var t = targetT * refT.Inverse;
            return t;
        }
        public static Transform GetTransform(Wall _wall)
        {
            if (_wall.WallType.Kind == WallKind.Basic)
            {
                Line l = (_wall.Location as LocationCurve).Curve as Line;
                if (l != null)
                {
                    var Y = _wall.Orientation;
                    var Z = XYZ.BasisZ;
                    var X = l.Direction;

                    var t = Transform.Identity;
                    t.BasisX = X;
                    t.BasisY = Y;
                    t.BasisZ = Z;
                    t.Origin = l.Origin;
                    return t;
                }
                else return null;
            }
            //叠层墙或curtain
            else
            {
                var Y = _wall.Orientation;
                if (_wall.WallType.Kind != WallKind.Basic && _wall.Flipped)
                    Y *= -1;
                var Z = XYZ.BasisZ;
                var X = _wall.Orientation.CrossProduct(Z);

                var t = Transform.Identity;
                t.BasisX = X;
                t.BasisY = Y;
                t.BasisZ = Z;
                if (_wall.WallType.Kind == WallKind.Curtain)
                {
                    t.Origin = ((LocationCurve)_wall.Location).Curve.GetEndPoint(0);
                }
                else
                {
                    Line l = (_wall.Location as LocationCurve).Curve as Line;
                    if (l != null)
                        t.Origin = l.Origin;
                    else return null;
                }
                return t;
            }

        }
        /*
        public static Transform GetTransform(Wall _wall)
        {
            var Y = _wall.Orientation;
            if (_wall.Flipped)
                Y *= -1;
            var Z = XYZ.BasisZ;
            var X = _wall.Orientation.CrossProduct(Z);

            var t = Transform.Identity;
            t.BasisX = X;
            t.BasisY = Y;
            t.BasisZ = Z;
            t.Origin = ((LocationCurve)_wall.Location).Curve.GetEndPoint(0);
            return t;
        }
        */
        public static Transform GetTransform(FamilyInstance _refInstance, FamilyInstance _targetInstance)
        {
            var refT = GetTransform(_refInstance);
            var targetT = GetTransform(_targetInstance);
            return targetT * refT.Inverse;
        }
        public static Transform GetAllTransform(this FamilyInstance _fi)
        {
            return GetTransform(_fi);
        }
        public static Transform GetTransform(FamilyInstance _fi)
        {
            var t = Transform.Identity;
            t.BasisX = _fi.HandOrientation;
            t.BasisY = _fi.FacingOrientation;
            t.BasisZ = t.BasisX.CrossProduct(t.BasisY);
            if (_fi.Mirrored)
                t.BasisZ *= -1;
            t.Origin = _fi.getOrigin();
            //t.Origin = _fi.GetTotalTransform().Origin;
            return t;
        }
        private static XYZ getOrigin(this FamilyInstance _fi)
        {
            if (_fi.Location is LocationPoint)
            {
                return ((LocationPoint)_fi.Location).Point;
            }
            else
            {
                var curve = ((LocationCurve)_fi.Location).Curve;
                return curve.GetEndPoint(0);
            }
        }
        public static string ToStringDigits(this Transform _transform, int _digits)
        {
            string s = "";
            s += _transform.Origin.ToStringDigits(_digits) + "||";
            s += _transform.BasisX.ToStringDigits(_digits) + "||";
            s += _transform.BasisY.ToStringDigits(_digits) + "||";
            s += _transform.BasisZ.ToStringDigits(_digits) + "||";
            s += _transform.Scale.ToStringDigits(_digits);
            return s;
        }
        #endregion

        #region UnitConvertion
        public static double MilliMeterToFeet(this double _mm)
        {
            return UnitUtils.ConvertToInternalUnits(_mm, DisplayUnitType.DUT_MILLIMETERS);
        }
        public static double FeetToMilliMeter(this double _feet)
        {
            return UnitUtils.ConvertFromInternalUnits(_feet, DisplayUnitType.DUT_MILLIMETERS);
        }
        #endregion

        #region View
        public static Plane GetViewBasePlane(this Autodesk.Revit.DB.View _view)
        {
            if (_view is ViewPlan
                || _view is ViewSection
                || _view is View3D)
            {
                //get plane from view origin and direction
                var origin = _view.Origin;
                //if plane view, add base level height
                if (_view is ViewPlan)
                {
                    var vp = _view as ViewPlan;
                    var range = vp.GetViewRange();
                    var levelId = range.GetLevelId(PlanViewPlane.CutPlane);
                    var level = _view.Document.GetElement(levelId) as Level;
                    origin = new XYZ(0, 0, level.ProjectElevation);
                }
                return Plane.CreateByOriginAndBasis(origin, _view.RightDirection, _view.UpDirection);
            }
            else
                return null;
        }
        public static Plane GetViewCutPlane(this Autodesk.Revit.DB.View _view)
        {
            if (_view is ViewPlan
                || _view is ViewSection
                || _view is View3D)
            {
                //get plane from view origin and direction
                var origin = _view.Origin;
                //if plane view, add cut height
                if (_view is ViewPlan)
                {
                    var vp = _view as ViewPlan;
                    var range = vp.GetViewRange();
                    var levelId = range.GetLevelId(PlanViewPlane.CutPlane);
                    var level = _view.Document.GetElement(levelId) as Level;
                    origin = new XYZ(0, 0, level.ProjectElevation);
                    var offset = range.GetOffset(PlanViewPlane.CutPlane);
                    origin += new XYZ(0, 0, offset);
                }
                return Plane.CreateByOriginAndBasis(origin, _view.RightDirection, _view.UpDirection);
            }
            else
                return null;
        }
        public static Solid GetCropBoxAsSolid(this Autodesk.Revit.DB.View _view)
        {
            if (_view is ViewPlan)
            {
                var vp = _view as ViewPlan;
                return vp.GetCropBoxAtCutPlane();
            }
            else if (_view is ViewSection)
            {
                var vs = _view as ViewSection;
                return vs.CropBox.ToSolid();
            }
            else
            {
                throw new Exception("View type not supported by this method.");
            }
        }
        public static Solid GetCropBoxAtCutPlane(this ViewPlan _planView)
        {
            var doc = _planView.Document;

            var viewRange = _planView.GetViewRange();
            var cutLevelId = viewRange.GetLevelId(PlanViewPlane.CutPlane);
            var level = doc.GetElement(cutLevelId) as Level;
            var zLevel = level.ProjectElevation;
            var cutOffset = viewRange.GetOffset(PlanViewPlane.CutPlane);
            var zCut = zLevel + cutOffset;
            var viewDepthLevelId = viewRange.GetLevelId(PlanViewPlane.ViewDepthPlane);
            double zDepth;
            if (viewDepthLevelId != ElementId.InvalidElementId)
            {
                var viewDepthLevel = doc.GetElement(viewDepthLevelId) as Level;
                //view depth level id could be -4, meanning level below, could be null
                if (viewDepthLevel == null)
                {
                    zDepth = zCut - 1000;
                }
                else
                {
                    var viewDepthOffset = viewRange.GetOffset(PlanViewPlane.ViewDepthPlane);
                    zDepth = viewDepthLevel.ProjectElevation + viewDepthOffset;
                }
            }
            else
            {
                zDepth = zCut - 1000;
            }

            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, zDepth));
            var height = zCut - zDepth;

            var cl = getCurveLoopAtBottom(_planView.CropBox);
            var originZ = cl.GetPlane().Origin.Z;
            var transform = Transform.CreateTranslation(new XYZ(0, 0, zDepth - originZ));
            var projectedCL = CurveLoop.CreateViaTransform(cl, transform);
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry
                 (new List<CurveLoop>() { projectedCL }, XYZ.BasisZ, height);
            return solid;
        }
        private static CurveLoop getCurveLoopAtBottom(BoundingBoxXYZ _box)
        {
            var p1 = new XYZ(_box.Min.X, _box.Min.Y, _box.Min.Z);
            var p2 = new XYZ(_box.Max.X, _box.Max.Y, _box.Min.Z);

            var minX = Math.Min(p1.X, p2.X);
            var maxX = Math.Max(p1.X, p2.X);
            var minY = Math.Min(p1.Y, p2.Y);
            var maxY = Math.Max(p1.Y, p2.Y);
            var minZ = _box.Min.Z;
            CurveLoop loop;
            try
            {
                loop = CurveLoop.Create(new List<Curve>()
                {
                    Line.CreateBound(new XYZ(minX, maxY, minZ), new XYZ(maxX, maxY, minZ)),
                    Line.CreateBound(new XYZ(maxX, maxY, minZ), new XYZ(maxX, minY, minZ)),
                    Line.CreateBound(new XYZ(maxX, minY, minZ), new XYZ(minX, minY, minZ)),
                    Line.CreateBound(new XYZ(minX, minY, minZ), new XYZ(minX, maxY, minZ))
                });
            }
            catch (ArgumentsInconsistentException ex)
            {
                //one or more edge is too short
                return null;
            }
            return loop;
        }
        #endregion
    }

    public enum CurveLoopType
    {
        Open,
        Closed,
        Invalid
    }
    public class InvalidCurveLoopException : Exception
    {
        public InvalidCurveLoopException()
        {
        }
        public InvalidCurveLoopException(string message)
        : base(message)
        {
        }
        public InvalidCurveLoopException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
