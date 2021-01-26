using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using g3;

namespace goa.Common.g3InterOp
{
    public static class Method_g3Revit
    {
        #region plane & frame
        public static Frame3f ToFrame3f(this Plane _plane)
        {
            return new Frame3f
                (_plane.Origin.ToVector3f(),
                _plane.XVec.ToVector3f(),
                _plane.YVec.ToVector3f(),
                _plane.Normal.ToVector3f());
        }
        #endregion

        #region Vector
        public static Vector3d ToVector3d(this XYZ _xyz)
        {
            return new Vector3d(_xyz.X, _xyz.Y, _xyz.Z);
        }
        public static Vector3d ToVector3d(this Vector2d _xy)
        {
            return new Vector3d(_xy.x, _xy.y, 0);
        }
        public static Vector3f ToVector3f(this XYZ _xyz)
        {
            return new Vector3f(_xyz.X, _xyz.Y, _xyz.Z);
        }
        public static XYZ ToXYZ(this Vector3d _v)
        {
            return new XYZ(_v.x, _v.y, _v.z);
        }
        public static XYZ ToXYZ(this Vector3f _v)
        {
            return new XYZ(_v.x, _v.y, _v.z);
        }
        public static Vector2d ToVector2d(this UV _uv)
        {
            return new Vector2d(_uv.U, _uv.V);
        }
        public static UV ToUV(this Vector2d _v)
        {
            return new UV(_v.x, _v.y);
        }
        #endregion

        #region BoundingBox
        public static AxisAlignedBox2d ToAABox2d(this BoundingBoxXYZ _bb, Plane3d _plane)
        {
            var min = _plane.ProjectInto(_bb.Min.ToVector3d());
            var max = _plane.ProjectInto(_bb.Max.ToVector3d());
            return new AxisAlignedBox2d(min, max);
        }
        public static AxisAlignedBox2d ToAABox2d(this BoundingBoxXYZ _bb, Frame3f _frame)
        {
            var min = _frame.ToPlaneUV((Vector3f)_bb.Min.ToVector3d(), 2);
            var max = _frame.ToPlaneUV((Vector3f)_bb.Max.ToVector3d(), 2);
            return new AxisAlignedBox2d(min, max);
        }
        #endregion  

        #region Line & Ray
        public static Segment2d ProjectIntoPlane(this Line _revitLine, Plane3d _p3d)
        {
            var inP0 = _p3d.ProjectInto(_revitLine.GetEndPoint(0).ToVector3d());
            var inP1 = _p3d.ProjectInto(_revitLine.GetEndPoint(1).ToVector3d());
            return new Segment2d(inP0, inP1);
        }
        public static Line ToLine(this Segment2d seg)
        {
            return Line.CreateBound(seg.P0.ToVector3d().ToXYZ(), seg.P1.ToVector3d().ToXYZ());
        }
        public static Line ToLine(this Segment3d seg)
        {
            return Line.CreateBound(seg.P0.ToXYZ(), seg.P1.ToXYZ());
        }
        public static Line ToLine(this Segment3f seg)
        {
            return Line.CreateBound(seg.P0.ToXYZ(), seg.P1.ToXYZ());
        }
        public static Segment3d ToSegment3d(this Line _l)
        {
            return new Segment3d(_l.GetEndPoint(0).ToVector3d(), _l.GetEndPoint(1).ToVector3d());
        }
        public static Segment3f ToSegment3f(this Line _l)
        {
            return new Segment3f(_l.GetEndPoint(0).ToVector3f(), _l.GetEndPoint(1).ToVector3f());
        }
        #endregion

        #region Triangle & Polygon
        public static List<Triangle3f> TriangulateG3(this Autodesk.Revit.DB.Face _face)
        {
            var mesh = _face.Triangulate();
            var count = mesh.NumTriangles;
            var list = new List<Triangle3f>();
            for (int i = 0; i < count; i++)
            {
                var t = mesh.get_Triangle(i);
                var triangle = new Triangle3f
                    (t.get_Vertex(0).ToVector3f(),
                    t.get_Vertex(1).ToVector3f(),
                    t.get_Vertex(2).ToVector3f());
                list.Add(triangle);
            }
            return list;
        }
        public static List<Triangle3f> ToTriangles(this List<Face> _faces)
        {
            List<Triangle3f> list = new List<Triangle3f>();
            foreach (var face in _faces)
                list.AddRange(face.TriangulateG3());
            return list;
        }
        public static Polygon2d ToPolygon2d(this IEnumerable<UV> _list)
        {
            var points = _list.Select(x => x.ToVector2d());
            return new Polygon2d(points);
        }
        /// <summary>
        /// Assume curves are XY-plane-based. Tessellate curves by max angle of 10 degree.
        /// </summary>
        public static Polygon2d ToPolygon2d(this IEnumerable<Curve> _curves)
        {
            return _curves.ToPolygon2d(10);
        }
        /// <summary>
        /// Assume curves are XY-plane-based. Tessellate curves by input max angle.
        /// </summary>
        public static Polygon2d ToPolygon2d(this IEnumerable<Curve> _curves, double _maxAngleInDeg)
        {
            List<XYZ> points = new List<XYZ>();
            foreach (var c in _curves)
            {
                var newPoints = c.SegmentationByMaxAngle(_maxAngleInDeg);
                if (points.Count > 0)
                {
                    if (points.Last().IsAlmostEqualToByDifference(newPoints.First(), 0.0001))
                        newPoints.RemoveAt(0);
                    if (newPoints.Last().IsAlmostEqualToByDifference(points.First(), 0.0001))
                        newPoints.RemoveAt(newPoints.Count - 1);
                }
                points.AddRange(newPoints);
            }
            var uvs = points.Select(x => x.ToUV());
            return uvs.ToPolygon2d();
        }
        public static IEnumerable<UV> ToUVList(this Polygon2d _polygon)
        {
            return _polygon.Vertices.Select(x => x.ToUV());
        }
        public static IEnumerable<Line> ToLines(this Polygon2d _polygon)
        {
            List<Line> lines = new List<Line>();
            foreach (var seg in _polygon.SegmentItr())
            {
                lines.Add(seg.ToLine());
            }
            return lines;
        }
        #endregion
    }
    public static class VectorToFloat
    {
        public static float[] convertToFloat(Vector3f _v)
        {
            return new float[3] { _v.x, _v.y, _v.z };
        }
        public static float[,] convertToFloat(Vector3f[] _input, int _size)
        {
            var array = new float[_size, 3];
            for (int i = 0; i < _size; i++)
            {
                var v = _input[i];
                array[i, 0] = v.x;
                array[i, 1] = v.y;
                array[i, 2] = v.z;
            }
            return array;
        }
        public static float[] convertToFloatOneD(Vector3f[] _input, int _size)
        {
            var array = convertToFloat(_input, _size);
            var convert = convertToOneD(array);
            return convert;
        }
        public static float[,,] convertToFloat(Vector3f[,] _triangles, int _size)
        {
            var array = new float[_size, 3, 3];
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var v = _triangles[i, j];
                    array[i, j, 0] = v.x;
                    array[i, j, 1] = v.y;
                    array[i, j, 2] = v.z;
                }
            }
            return array;
        }
        public static float[] convertToFloatOneD(Vector3f[,] _triangles, int _size)
        {
            var array = convertToFloat(_triangles, _size);
            var convert = convertToOneD(array);
            return convert;
        }
        private static float[] convertToOneD(float[,] input)
        {
            var size0 = input.GetLength(0);
            var size1 = input.GetLength(1);
            var a = new float[size0 * size1];
            int index = 0;
            for (int i = 0; i < size0; i++)
            {
                for (int j = 0; j < size1; j++)
                {
                    a[index] = input[i, j];
                    index++;
                }
            }
            return a;
        }

        private static float[] convertToOneD(float[,,] input)
        {
            var size0 = input.GetLength(0);
            var size1 = input.GetLength(1);
            var size2 = input.GetLength(2);
            var a = new float[size0 * size1 * size2];
            int index = 0;
            for (int i = 0; i < size0; i++)
            {
                for (int j = 0; j < size1; j++)
                {
                    for (int k = 0; k < size2; k++)
                    {
                        a[index] = input[i, j, k];
                        index++;
                    }
                }
            }
            return a;
        }
    }
}
