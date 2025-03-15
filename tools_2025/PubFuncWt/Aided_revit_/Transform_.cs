using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Transform_
    {
        public static IEnumerable<XYZ> OfPoints(this Transform transform,IEnumerable<XYZ> xYZs)
        {
            foreach (var item in xYZs)
            {
                yield return transform.OfPoint(item);
            }
        }
    }
}
