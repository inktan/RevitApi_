using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class CurveArray_
    {
        /// <summary>
        /// 闭合曲线圈排序
        /// </summary>
        public static List<Curve> SortCurves(this List<Curve> curves)
        {
            List<Curve> _curves = new List<Curve>();
            XYZ temp = curves[0].GetEndPoint(1);//拿出一根线段的一个终点
            Curve temCurve = curves[0];//拿出一根线段
            _curves.Append(temCurve);//添加第一根线段
            while (_curves.Count != curves.Count)//循环停止条件
            {
                temCurve = temCurve.GetNext(curves, temp);//寻找相连线段
                if (Math.Abs(temp.X - temCurve.GetEndPoint(0).X) < Precision_.TheShortestDistance
                    && Math.Abs(temp.Y - temCurve.GetEndPoint(0).Y) < Precision_.TheShortestDistance)
                {
                    temp = temCurve.GetEndPoint(1);
                }
                else
                {
                    temp = temCurve.GetEndPoint(0);
                }
                _curves.Append(temCurve);
            }
            if (Math.Abs(temp.X - curves[0].GetEndPoint(0).X) > Precision_.TheShortestDistance
                || Math.Abs(temp.Y - curves[0].GetEndPoint(0).Y) > Precision_.TheShortestDistance
                || Math.Abs(temp.Z - curves[0].GetEndPoint(0).Z) > Precision_.TheShortestDistance)//temp最后一次，应该就是选段闭合的起点
            {
                throw new InvalidOperationException("CurveLoop的起点与终点不闭合。");
            }
            return _curves;
        }
        /// <summary>
        /// 基于一个相交点，在一个曲线列表中找到与之相连的曲线
        /// </summary>
        /// <param name="currentCurve"></param>
        /// <param name="curves"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public static Curve GetNext(this Curve currentCurve, List<Curve> curves, XYZ connected)//找到连接线段 便有返回值
        {
            foreach (Curve c in curves)
            {
                if (c.Equals(currentCurve))//判断两根线段是否为同一物体
                {
                    continue;
                }
                if ((Math.Abs(c.GetEndPoint(0).X - currentCurve.GetEndPoint(1).X) < Precision_.TheShortestDistance
                    && Math.Abs(c.GetEndPoint(0).Y - currentCurve.GetEndPoint(1).Y) < Precision_.TheShortestDistance
                    && Math.Abs(c.GetEndPoint(0).Z - currentCurve.GetEndPoint(1).Z) < Precision_.TheShortestDistance)
                    && (Math.Abs(c.GetEndPoint(1).X - currentCurve.GetEndPoint(0).X) < Precision_.TheShortestDistance
                    && Math.Abs(c.GetEndPoint(1).Y - currentCurve.GetEndPoint(0).Y) < Precision_.TheShortestDistance
                    && Math.Abs(c.GetEndPoint(1).Z - currentCurve.GetEndPoint(0).Z) < Precision_.TheShortestDistance)
                    && 2 != curves.Count)//该条件判断 c 的 起点 终点 与 connect的 终点 起点 是否重合 判断两根线段是否重合
                {
                    continue;
                }
                if (c.GetEndPoint(0).DistanceTo(connected) < Precision_.TheShortestDistance)//该条件成立，则说明 c 的终点与 connect 的终点相连
                {
                    return c;
                }
                else if (c.GetEndPoint(1).DistanceTo(connected) < Precision_.TheShortestDistance)//该条件成立，则说明 c 的终点与 connect 的终点相连 需要将线段重新绘制
                {
                    if (c is Line)
                    {
                        XYZ start = c.GetEndPoint(1);
                        XYZ end = c.GetEndPoint(0);
                        return Line.CreateBound(start, end);
                    }
                    else if (c is Arc)
                    {
                        int size = c.Tessellate().Count;
                        XYZ start = c.GetEndPoint(1);
                        XYZ middle = c.Tessellate()[size / 2];
                        XYZ end = c.GetEndPoint(0);
                        return Arc.Create(start, end, middle);
                    }
                }
            }
            throw new InvalidOperationException("CurveLoop may be unclosed_GetNext.");
        }

        #region Array To List
        /// <summary>
        /// 曲线数组转化为线圈
        /// </summary>
        public static CurveLoop ToCurveLoop(this CurveArray curveArray)
        {
            CurveLoop curves = new CurveLoop();
            foreach (Curve curve in curveArray)
            {
                curves.Append(curve);
            }
            return curves;
        }
        /// <summary>
        /// 曲线数组转化为线圈
        /// </summary>
        //public static CurveLoop ToCurveLoop(this IEnumerable<Curve> curves)
        //{
        //    CurveArray curveArray = curves.ToCurveArray();

        //    return curveArray.ToCurveLoop();
        //}
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
