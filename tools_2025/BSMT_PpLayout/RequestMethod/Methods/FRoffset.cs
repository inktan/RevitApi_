using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;
using goa.Common.g3InterOp;

namespace BSMT_PpLayout
{
    class FRoffset : RequestMethod
    {
        public FRoffset(UIApplication uiApp) : base(uiApp) { }

        internal override void Execute()
        {
            FilledRegion filledRegion = this.doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as FilledRegion;
            string typeName = this.doc.GetElement(filledRegion.GetTypeId()).Name;
            IList<CurveLoop> curveLoops = filledRegion.GetBoundaries();
            CurveLoop outtestO = curveLoops.First();
            Polygon2d o = outtestO.ToCurves().ToPolygon2d();

            double height = o.Height();
            double width = o.Width();
            if (height > Math.Abs(GlobalData.Instance.fROffsetDis_num) + Precision_.TheShortestDistance && width > Math.Abs(GlobalData.Instance.fROffsetDis_num) + Precision_.TheShortestDistance)
            {
                if (GlobalData.Instance.fROffsetDis_num < 0)
                {
                    o = o.InwardOffeet(Math.Abs(GlobalData.Instance.fROffsetDis_num));
                }
                else
                {
                    o = o.OutwardOffeet(Math.Abs(GlobalData.Instance.fROffsetDis_num));
                }

                curveLoops[0] = o.ToCurveLoop(); this.view.CreatRingFilledRegoin(this.doc, curveLoops.ToList(), typeName, 0);
            }
            this.doc.DelEleId(filledRegion.Id);



        }
    }
}
