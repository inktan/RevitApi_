
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
using goa.Revit.DirectContext3D;
using Autodesk.Revit.DB.ExtensibleStorage;
using goa.Common;

namespace BSMT_PpLayout
{
    class Temp05 : RequestMethod
    {

        public Temp05(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            DetailCurve detailCurve = this.doc.GetElement(this.sel.PickObject(ObjectType.Element)) as DetailCurve;
            Arc arc = detailCurve.GeometryCurve as Arc;


            XYZ center = arc.Center;
            XYZ center02 = arc.ComputeDerivatives(0.5, true).Origin;

            //List<Line> lines = arc.DivideToLinesByCyclic(8400.0.MilliMeterToFeet(), false);
            //lines.Count.ToString().TaskDialogErrorMessage();
            //arc.CreateOffset(3000.0.MilliMeterToFeet(), center - center02);
            if (arc.Normal.Z.EqualPrecision(1.0))
            {
                Curve arc01 = arc.CreateOffset(3000.0.MilliMeterToFeet(), new XYZ(0, 0, -1));

                this.doc.CreateDirectShapeWithNewTransaction(new List<Curve>() { Line.CreateBound(center, center02), arc01 });
            }
            else
            {
                Curve arc01 = arc.CreateOffset(3000.0.MilliMeterToFeet(), new XYZ(0, 0, 1));
                this.doc.CreateDirectShapeWithNewTransaction(new List<Curve>() { Line.CreateBound(center, center02), arc01 });
            }


            //int hight = 100;
            //Transform tsf = Transform.CreateTranslation(new XYZ(0, 0, 1).Multiply(hight));
            //Curve newCurve = oldCurve.CreateTransformed(tsf);


        }
    }
}
