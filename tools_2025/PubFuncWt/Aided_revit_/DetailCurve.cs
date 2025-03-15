using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class DetailCurve_
    {
        public static DetailCurve DrawDetailCurveWithNewTrans(this Document document, Curve curve, Element lineStyle)
        {
            DetailCurve detailCurve;

            using (Transaction transaction = new Transaction(document, "创建一根详图线"))
            {
                transaction.Start();
                detailCurve = document.Create.NewDetailCurve(document.ActiveView, curve);
                detailCurve.LineStyle = lineStyle;
                transaction.Commit();
            }

            return detailCurve;
        }
        public static DetailCurveArray DrawDetailCurvesWithNewTrans(this Document document, CurveArray curveArray, Element lineStyle)
        {
            DetailCurveArray detailCurveArray;

            using (Transaction transaction = new Transaction(document, "创建一根详图线"))
            {
                transaction.Start();
                detailCurveArray = document.Create.NewDetailCurveArray(document.ActiveView, curveArray);
                foreach (DetailCurve item in detailCurveArray)
                {
                    item.LineStyle = lineStyle;
                }
                transaction.Commit();
            }

            return detailCurveArray;
        }

    }
}
