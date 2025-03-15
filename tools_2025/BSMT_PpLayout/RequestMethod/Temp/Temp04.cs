
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;
using g3;
using System.Collections.Concurrent;
using goa.Common.g3InterOp;
using Autodesk.Revit.DB.ExternalService;

using goa.Common;
using goa.Revit.DirectContext3D;

namespace BSMT_PpLayout
{

    /// <summary>
    /// 测试goa-common中的DirectContext3D 
    /// </summary>
    class Temp04 : RequestMethod
    {
        public Temp04(UIApplication uiapp) : base(uiapp)
        { }

        internal override void Execute()
        {
            List<PolyLine> polyLines = PolylineInCAD();
            polyLines.ForEach(p =>
            {
                List<Line> lines = p.ToLines();

                for (int j = 0; j < 1; j++)
                {
                    CMD.Doc.CreateDirectShapeWithNewTransaction(lines, CMD.Doc.ActiveView);
                }
            });
        }
        /// <summary>
        /// 根据选择框，选择链接cad中的多段线
        /// </summary>
        /// <returns></returns>
        internal List<PolyLine> PolylineInCAD()
        {
            List<PolyLine> result = new List<PolyLine>();
            // 更改操作方式
            var pickBox = this.sel.PickBox(PickBoxStyle.Crossing);
            Polygon2d region = pickBox.ToBoundingBox().ToPolygon2d();

            List<ImportInstance> cadlinks = new FilteredElementCollector(this.doc).OfClass(typeof(ImportInstance)).Cast<ImportInstance>().ToList();

            foreach (ImportInstance cadlink in cadlinks)
            {
                BoundingBoxXYZ boundingBoxXYZ = cadlink.get_BoundingBox(this.view);
                Polygon2d cadLinkPoly2d = boundingBoxXYZ.ToPolygon2d();
                if (region.Contains(cadLinkPoly2d) || region.Intersects(cadLinkPoly2d) || cadLinkPoly2d.Contains(region))
                {
                    Options opt = new Options();
                    opt.IncludeNonVisibleObjects = false;

                    List<PolyLine> temp = cadlink.GetGeometryObjectOfType<PolyLine>(opt).Where(x => isPolylineInsideBox(x, pickBox)).ToList();
                    result.AddRange(temp);
                }
            }
            return result;
        }
        /// <summary>
        /// polyling是否选择框内
        /// </summary>
        /// <returns></returns>
        private bool isPolylineInsideBox(PolyLine _pl, PickedBox _pb)
        {
            var bb = _pb.ToBoundingBoxUV();
            var coords = _pl.GetCoordinates().Select(x => x.ProjectToXY().ToUV());
            //return coords.All(x => bb.IsInside(x));
            foreach (var item in coords)
            {
                if (bb.IsInside(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
