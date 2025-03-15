using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using ClipperLib;
using System.Drawing;
using Autodesk.Revit.UI.Selection;
using Octree;
using goa.Common;
//using NetTopologySuite.Geometries;

namespace PubFuncWt
{
    public static class RevitTo_
    {
        #region ToOctree
        public static Vector3 ToVector3(this XYZ xYZ)
        {
            return new Vector3(xYZ.X, xYZ.Y, xYZ.Z);
        }


        public static Bounds ToBounds(this BoundingBoxXYZ boundingBoxXYZ)
        {
            Vector3 center = boundingBoxXYZ.GetCentroid().ToVector3();

            double length = boundingBoxXYZ.Width();
            double width = boundingBoxXYZ.Height();
            double height = boundingBoxXYZ.Depth();
            Vector3 size = new Vector3(length, width, height);
            return new Bounds(center, size);
        }
        #endregion

        #region ToQuadTree
        /// <summary>
        /// RectangleF在官方中定义的坐标系，为左手坐标系，Revit界面中显示为右手坐标系
        /// </summary>
        public static RectangleF ToRectangleF(this BoundingBoxXYZ boundingBoxXYZ)
        {
            XYZ min = boundingBoxXYZ.Min;
            return new RectangleF((float)min.X, (float)min.Y, (float)boundingBoxXYZ.Width(), (float)boundingBoxXYZ.Height());
        }
        #endregion

        #region g3
        /// <summary>
        /// Triangle3d的点集为逆时针时，Noraml默认指向Z轴负方向，该函数进行方向纠正
        /// </summary>
        /// <param name="meshTriangle"></param>
        /// <returns></returns>
        public static Triangle3d ToTriangle3d(this MeshTriangle meshTriangle)
        {
            return new Triangle3d(meshTriangle.get_Vertex(0).ToVector3d(), meshTriangle.get_Vertex(1).ToVector3d(), meshTriangle.get_Vertex(2).ToVector3d());
        }

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
        public static Segment2d ToSegment2d(this Curve line)
        {
            return new Segment2d(line.GetEndPoint(0).ToVector2d(), line.GetEndPoint(1).ToVector2d());
        }



        public static Arc2d ToArc2d(this Arc arc)
        {
            Vector2d center = arc.Center.ToVector2d();

            Vector2d vStart = arc.GetEndPoint(0).ToVector2d();
            Vector2d vEnd = arc.GetEndPoint(1).ToVector2d();

            return new Arc2d(center, vStart, vEnd);
        }
        public static Polygon2d ToPolygon2d(this BoundingBoxXYZ boundingBoxXYZ)
        {
            return new Polygon2d(boundingBoxXYZ.BottomXyzs().ToVector2ds());
        }
        public static Polygon2d ToPolygon2d(this PickedBox pickedBox)
        {
            return new Polygon2d(pickedBox.BottomXyzs().ToVector2ds());
        }
        public static Polygon2d ToPolygon2d(this PolyLine polyLinex)
        {
            return new Polygon2d(polyLinex.GetCoordinates().ToVector2ds().DelDuplicate());
        }

        // 寻找 goa.common
        //public static Polygon2d ToPolygon2d(this IEnumerable<XYZ> xYZs)
        //{
        //    return new Polygon2d(xYZs.ToVector2ds());
        //}
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
            double x = xyz.X * Precision_.ClipperMultiple;
            double y = xyz.Y * Precision_.ClipperMultiple;
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
