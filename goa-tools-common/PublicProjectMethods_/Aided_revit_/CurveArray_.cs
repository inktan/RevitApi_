using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class CurveArray_
    {
        #region Array To List
        /// <summary>
        /// 线列表转化为曲线数组
        /// </summary>
        public static CurveArray ToCurveArray(this IEnumerable<Line> _lines)
        {
            CurveArray curveArray = new CurveArray();
            foreach (Line line in _lines)
            {
                Curve _c = line as Curve;
                curveArray.Append(_c);
            }
            return curveArray;
        }/// <summary>
         /// 线列表转化为曲线数组
         /// </summary>
        public static CurveArray ToCurveArray(this IEnumerable<Curve> _curves)
        {
            CurveArray curveArray = new CurveArray();
            foreach (Curve _c in _curves)
            {
                curveArray.Append(_c);
            }
            return curveArray;
        }
        public static IEnumerable<Edge> ToIEnumerable(this EdgeArray edgeArray)
        {
            foreach (var item in edgeArray)
            {
                yield return item as Edge;
            }
        }

        public static IEnumerable<Face> ToIEnumerable(this FaceArray faceArray)
        {
            foreach (var item in faceArray)
            {
                yield return item as Face;
            }
        }
        public static IEnumerable<Curve> ToIEnumerable(this CurveArray curveArray)
        {
            foreach (var item in curveArray)
            {
                yield return item as Curve;
            }
        }
        #endregion
    }
}
