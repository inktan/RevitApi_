using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class CurveLoop_
    {

        public static IEnumerable<Curve> ToCurves(this CurveLoop curveLoop)
        {
            CurveLoopIterator curveLoopIterator = curveLoop.GetCurveLoopIterator();
            while (curveLoopIterator.MoveNext())
            {
                yield return curveLoopIterator.Current;
            }
        }
       
  

    }
}
