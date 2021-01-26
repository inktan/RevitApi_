using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ClipperLib;
using g3;

namespace PublicProjectMethods_
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    public static class ClipperTo_
    {
        #region Tog3

        /// <summary>
        /// 用polygon2d收集点集是为了，删除异常点（单根线端点，共线点）
        /// </summary>
        public static IEnumerable<Polygon2d> ToPolygon2ds(this Paths paths)
        {
            foreach (var item in paths)
            {
                yield return item.ToPolygon2d();
            }
        }
        /// <summary>
        /// 用polygon2d收集点集是为了，删除异常点（单根线端点，共线点）
        /// </summary>
        public static Polygon2d ToPolygon2d(this Path path)
        {
            Polygon2d polygon2d = new Polygon2d(path.ToVector2ds());
            polygon2d = polygon2d.DelDuplicate();
            return polygon2d.RemoveSharpCcorners();
        }
        public static IEnumerable<Vector2d> ToVector2ds(this Path path)
        {
            foreach (IntPoint _IntPoint in path)
            {
                yield return _IntPoint.ToVector2d();
            }
        }
        /// <summary>
        /// clipper intpoint to g3 vector2d
        /// </summary>
        public static Vector2d ToVector2d(this IntPoint _intpoint)
        {
            double x = (double)_intpoint.X / Precision_.clipperMultiple;
            double y = (double)_intpoint.Y / Precision_.clipperMultiple;
            return new Vector2d(x, y);
        }
        #endregion

        #region ToRevit
        public static IEnumerable<List<XYZ>> ToXYZses(this Paths paths)
        {
            foreach (var item in paths)
            {
                yield return item.ToXYZs().ToList();
            }

        }
        public static IEnumerable<XYZ> ToXYZs(this Path path)
        {
            foreach (IntPoint _IntPoint in path)
            {
                XYZ xYZ = _IntPoint.ToXYZ();
                yield return xYZ;
            }
        }
        public static XYZ ToXYZ(this IntPoint _intpoint)
        {
            double x = (double)_intpoint.X / Precision_.clipperMultiple;
            double y = (double)_intpoint.Y / Precision_.clipperMultiple;
            double z = 0.0;
            return new XYZ(x, y, z);
        }
        #endregion

    }
}
