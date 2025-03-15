using Autodesk.Revit.DB;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using goa.Common;

namespace PubFuncWt
{
    public static class Octree_
    {
    
        public static List<Line> ToBoundaryLines(this Bounds _bb)
        {
            var a = _bb.GetVertices();
            return new List<Line>()
            {
                Line.CreateBound(a[0], a[1]),
                Line.CreateBound(a[1], a[2]),
                Line.CreateBound(a[2], a[3]),
                Line.CreateBound(a[3], a[0]),
                Line.CreateBound(a[4], a[5]),
                Line.CreateBound(a[5], a[6]),
                Line.CreateBound(a[6], a[7]),
                Line.CreateBound(a[7], a[4]),
                Line.CreateBound(a[0], a[4]),
                Line.CreateBound(a[1], a[5]),
                Line.CreateBound(a[2], a[6]),
                Line.CreateBound(a[3], a[7]),
            };
        }
        public static XYZ[] GetVertices(this Bounds bounds)
        {
            var p0 = bounds.Min.ToXyz();
            var p1 = new XYZ(bounds.Max.X, bounds.Min.Y, bounds.Min.Z);
            var p2 = new XYZ(bounds.Max.X, bounds.Max.Y, bounds.Min.Z);
            var p3 = new XYZ(bounds.Min.X, bounds.Max.Y, bounds.Min.Z);
            var p4 = new XYZ(bounds.Min.X, bounds.Min.Y, bounds.Max.Z);
            var p5 = new XYZ(bounds.Max.X, bounds.Min.Y, bounds.Max.Z);
            var p6 = bounds.Max.ToXyz();
            var p7 = new XYZ(bounds.Min.X, bounds.Max.Y, bounds.Max.Z);
            var array = new XYZ[8] { p0, p1, p2, p3, p4, p5, p6, p7 };
            return array;
        }
    }
}
