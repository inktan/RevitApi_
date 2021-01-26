using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using ClipperLib;
//using NetTopologySuite.Geometries;

namespace PublicProjectMethods_
{
    public static class RevitTo_
    {
        #region g3
        public static Vector2d ToVector2d(this XYZ xyz)
        {
            return new Vector2d(xyz.X, xyz.Y);
        }
        public static Vector3d ToVector3d(this XYZ xyz)
        {
            return new Vector3d(xyz.X, xyz.Y, xyz.Z);
        }
        public static Segment2d ToSegment2d(this Line line)
        {
            return new Segment2d(line.GetEndPoint(0).ToVector2d(), line.GetEndPoint(1).ToVector2d());
        }
        public static Line2d ToLine2d(this Line line)
        {
            return new Line2d(line.GetEndPoint(0).ToVector2d(), line.Direction.ToVector2d());
        }
        public static Arc2d ToArc2d(this Arc arc)
        {
            Vector2d center = arc.Center.ToVector2d();

            Vector2d vStart = arc.GetEndPoint(0).ToVector2d();
            Vector2d vEnd = arc.GetEndPoint(1).ToVector2d();

            return new Arc2d(center, vStart, vEnd);
        }
        public static Polygon2d ToPolygon2d(IEnumerable<XYZ> xYZs)
        {
            return new Polygon2d(xYZs.ToVector2ds());
        }
        public static IEnumerable<Vector2d> ToVector2ds(this IEnumerable<XYZ> xYZs)
        {
            foreach (var p in xYZs)
                yield return p.ToVector2d();
        }
        public static IEnumerable<Vector3d> ToVector3ds(this IEnumerable<XYZ> xYZs)
        {
            foreach (var p in xYZs)
                yield return p.ToVector3d();
        }

        #endregion

        #region cliper
        public static IntPoint ToIntPoint(this XYZ xyz)
        {
            double x = xyz.X * Precision_.clipperMultiple;
            double y = xyz.Y * Precision_.clipperMultiple;
            return new IntPoint((Int64)x, (Int64)y);
        }
        public static IEnumerable<IntPoint> ToIntPoints(this IEnumerable<XYZ> xYZs)
        {
            foreach (var item in xYZs)
            {
                yield return item.ToIntPoint();
            }
        }
        #endregion

        #region NetTopologySuiteTo_

        //public static Coordinate ToCoordinate(this XYZ xYZ)
        //{
        //    return new Coordinate(xYZ.X,xYZ.Y);
        //}

        #endregion

    }





}
