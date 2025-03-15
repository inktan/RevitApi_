using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class DirectShape_
    {
        #region DirectShape 显示原生几何形体
        /// <summary>
        /// 使用 DirectShape 显示原点位置
        /// </summary>
        public static void ShowOrigin(this Document _doc)
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
        public static void CreateDirectShape(this Document _doc, IEnumerable<GeometryObject> _geometry)
        {
            DirectShape ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
            ds.SetShape(_geometry.ToList());
        }
        /// <summary>
        /// need open transaction.
        /// </summary>
        public static DirectShape CreateDirectShape(this Document _doc, IEnumerable<GeometryObject> _geometry, XYZ _translation)
        {
            DirectShape ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
            ds.SetShape(_geometry.ToList());
            ElementTransformUtils.MoveElement(ds.Document, ds.Id, _translation);

            return ds;
        }
        /// <summary>
        /// 由于视图的裁切面具有高度
        /// </summary>
        public static DirectShape CreateDirectShapeWithNewTransaction(this Document _doc, IEnumerable<GeometryObject> _geometries,View view)
        {
            double _ele = view.GenLevel.ProjectElevation;
            XYZ _xYZ = new XYZ(0, 0, _ele);
            DirectShape ds = null;
            using (Transaction trans = new Transaction(_doc, "将几何物体打印显示"))
            {
                trans.Start();
                
                ds= CreateDirectShape(_doc, _geometries, _xYZ);
                trans.Commit();
            }
            return ds;
        }
        /// <summary>
        /// 由于视图的裁切面具有高度
        /// </summary>
        public static void CreateDirectShapeWithNewTransaction(this Document _doc, IEnumerable<GeometryObject> _geometries, View view,XYZ xYZ)
        {
            double _ele = view.GenLevel.ProjectElevation;
            XYZ _xYZ = new XYZ(0, 0, _ele);
            if (xYZ != null)
            {
                _xYZ += xYZ;
            }
            using (Transaction trans = new Transaction(_doc, "将几何物体打印显示"))
            {
                trans.Start();
                CreateDirectShape(_doc, _geometries, _xYZ);
                trans.Commit();
            }
        }
        public static void CreateDirectShapeWithNewTransaction(this Document _doc, IEnumerable<GeometryObject> _geometry)
        {
            using (Transaction trans = new Transaction(_doc, "将几何物体打印显示"))
            {
                trans.Start();
                CreateDirectShape(_doc, _geometry);
                trans.Commit();
            }
        }
        #endregion
    }
}
