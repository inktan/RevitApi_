using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class EdgeArray_
    {

        public static CurveLoop ToCurveLoop(this EdgeArray edgeArray)
        {
            CurveLoop curves = new CurveLoop();
            foreach (Edge edge in edgeArray)
            {
                Curve curve = edge.AsCurve();
                curves.Append(curve);
            }
            return curves;
        }

        public static IEnumerable<XYZ> ToXyzs(this EdgeArray edgeArray)
        {
            foreach (Edge edge in edgeArray)
            {
                yield return edge.AsCurve().GetEndPoint(0);
            }
        }

        
    }
}
