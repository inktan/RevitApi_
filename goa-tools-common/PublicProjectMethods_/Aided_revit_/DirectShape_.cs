using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
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
        public static void CreateDirectShape(this Document _doc, IEnumerable<GeometryObject> _geometry, XYZ _translation)
        {
            DirectShape ds = DirectShape.CreateElement(_doc, new ElementId(BuiltInCategory.OST_GenericModel));
            ds.SetShape(_geometry.ToList());
            ElementTransformUtils.MoveElement(ds.Document, ds.Id, _translation);
        }
        public static void CreateDirectShapeWithNewTransaction(this Document _doc, IEnumerable<GeometryObject> _geometry)
        {
            double _ele = _doc.ActiveView.GenLevel.ProjectElevation;
            XYZ _xYZ = new XYZ(0, 0, _ele);
            using (Transaction trans = new Transaction(_doc, "test"))
            {
                trans.Start();
                CreateDirectShape(_doc, _geometry, _xYZ);
                trans.Commit();
            }

        }
        #endregion
    }
}
