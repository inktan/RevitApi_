
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

    class Temp02 : RequestMethod
    {
        public Temp02(UIApplication uiapp) : base(uiapp)
        { }

        internal override void Execute()
        {
            // 清除无效详图组类型

            FilledRegion filledRegion = this.doc.GetElement(this.sel.PickObject(ObjectType.Element)) as FilledRegion;

            Polygon2d polygon2d = filledRegion.GetBoundaries().First().ToList().ToPolygon2d();

            Frame3d frame3d = new Frame3d(new Vector3d(10, -10, 0), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, 1));

            polygon2d = frame3d.ToFrame2dPoly(polygon2d);

            for (int j = 0; j < 1; j++)
            {
                List<Vector2d> _path_Vector2D = polygon2d.Vertices.ToList();
                List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
                IEnumerable<Line> _lines = _xYZs.ToLines();
                CMD.Doc.CreateDirectShapeWithNewTransaction(_lines, CMD.Doc.ActiveView);
            }
        }
    }
}
