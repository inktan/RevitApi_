using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class CurveLoop_
    {

        public static IEnumerable<Curve> ToCurves(this CurveLoop curveLoop)
        {
            List<Curve> curveArray = new List<Curve>();
            CurveLoopIterator curveLoopIterator = curveLoop.GetCurveLoopIterator();
            while (curveLoopIterator.MoveNext())
            {
                yield return curveLoopIterator.Current;
            }
        }
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
        /// 闭合曲线圈排序
        /// </summary>
        public static CurveArray SortCurves(this CurveArray curves)
        {
            CurveArray _curves = new CurveArray();
            XYZ temp = curves.get_Item(0).GetEndPoint(1);//拿出一根线段的一个终点
            Curve temCurve = curves.get_Item(0);//拿出一根线段
            _curves.Append(temCurve);//添加第一根线段
            while (_curves.Size != curves.Size)//循环停止条件
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
            if (Math.Abs(temp.X - curves.get_Item(0).GetEndPoint(0).X) > Precision_.TheShortestDistance
                || Math.Abs(temp.Y - curves.get_Item(0).GetEndPoint(0).Y) > Precision_.TheShortestDistance
                || Math.Abs(temp.Z - curves.get_Item(0).GetEndPoint(0).Z) > Precision_.TheShortestDistance)//temp最后一次，应该就是选段闭合的起点
            {
                throw new InvalidOperationException("CurveLoop的起点与终点不闭合。");
            }
            return _curves;
        }
        public static Curve GetNext(this Curve currentCurve, CurveArray curves, XYZ connected)//找到连接线段 便有返回值
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
                    && 2 != curves.Size)//该条件判断 c 的 起点 终点 与 connect的 终点 起点 是否重合 判断两根线段是否重合
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

    }
}
