using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class PolyLine_
    {
        public static CurveLoop ToCurveLoop(this PolyLine polyLine)
        {
            List<Line> lines = polyLine.ToLines();
            CurveLoop curves = new CurveLoop();
            lines.ForEach(p =>
            {
                curves.Append(p);
            });
            return curves;
        }
    }
}
