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

using goa.Common;


namespace BSMT_PpLayout
{

    class RampGenerated : RequestMethod
    {
        public RampGenerated(UIApplication uiapp) : base(uiapp)
        {

        }

        internal override void Execute()
        {
            IList<Reference> references = sel.PickObjects(ObjectType.Element, new SelPickFilter_Line(), "请点选坡道中心线组成的线组合");

            List<Curve> centerLine = new List<Curve>();// 坡道中心线 需要进行排序
            foreach (var item in references)
            {
                Element element = doc.GetElement(item);
                if (element is CurveElement)
                {
                    centerLine.Add((element as CurveElement).GeometryCurve);
                }
            }

            double length = 0;
            foreach (var item in centerLine)
            {
                length += item.ApproximateLength.FeetToMilliMeter();
            }

            GlobalData.Instance.CurrentLength_num = length.NumDecimal(1);
            GlobalData.Instance.CurrentLength = length.NumDecimal(1).ToString();
            GlobalData.Instance.NeedLength =(GlobalData.Instance.TotalRampLength_num -length).NumDecimal(1).ToString();

        }

        internal void Computer()
        {
            var groupRamp = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_RampCurveGroup(), "请点选坡道中心线组成的线组合")) as Group;

            Curve curve = null; // 确定坡道宽度

            List<Curve> centerLine = new List<Curve>();// 坡道中心线 需要进行排序
            foreach (var item in groupRamp.GetMemberIds())
            {
                Element element = doc.GetElement(item);
                if (element is DetailCurve)
                {
                    DetailCurve detailCurve = element as DetailCurve;
                    string lineStyleName = detailCurve.LineStyle.Name;
                    if (lineStyleName == "细线")
                    {
                        curve = detailCurve.GeometryCurve;
                    }
                    else if (lineStyleName == "地库_坡道中心线")
                    {
                        centerLine.Add(detailCurve.GeometryCurve);
                    }
                }
            }

            // 中断命令
            if (curve == null)
            {
                throw new NotImplementedException("线组内不存在指定坡道宽度的细线");
            }
            if (centerLine.Count < 1)
            {
                throw new NotImplementedException("线组内不存在坡道中心线");
            }

            double width = curve.ApproximateLength;
            
            // 排序坡道中心线
            CurveLoop curveLoopNew = SortCurves(centerLine);

            GraphicsStyle graphicsStyle = doc.GetGraphicsStyleByName("细线");
            
            // 将坡道中心线，两侧进行偏移
            CurveLoop o1 = CurveLoop.CreateViaOffset(curveLoopNew, width / 2, new XYZ(0, 0, 1));
            _ = doc.DrawDetailCurvesWithNewTrans(o1.ToCurveArray(), graphicsStyle);
            CurveLoop o2 = CurveLoop.CreateViaOffset(curveLoopNew, width / -2, new XYZ(0, 0, 1));
            _ = doc.DrawDetailCurvesWithNewTrans(o2.ToCurveArray(), graphicsStyle);

            // 将坡道边缘 封边
            Line line01 = Line.CreateBound(o1.First().GetEndPoint(0), o2.First().GetEndPoint(0));
            Line line02 = Line.CreateBound(o1.Last().GetEndPoint(1), o2.Last().GetEndPoint(1));

            CurveArray curveArray = new CurveArray();
            curveArray.Append(line01);
            curveArray.Append(line02);

            _ = doc.DrawDetailCurvesWithNewTrans(curveArray, graphicsStyle);
        }
        CurveLoop SortCurves(List<Curve> curves)
        {
            if (curves.Count == 1)
            {
                return curves.ToCurveLoop();
            }
            // 要判断第一条线
            Curve tempC = null;
            XYZ tempP = null;// 连接点
            foreach (var item01 in curves)
            {

                XYZ p0 = item01.GetEndPoint(0);
                XYZ p1 = item01.GetEndPoint(1);

                int count0 = 0;
                int count1 = 0;

                foreach (var item02 in curves)
                {
                    if (item01 != item02)
                    {
                        XYZ _p0 = item02.GetEndPoint(0);
                        XYZ _p1 = item02.GetEndPoint(1);

                        if (p0.DistanceTo(_p0) < Precision_.TheShortestDistance || p0.DistanceTo(_p1) < Precision_.TheShortestDistance)
                        {
                            count0++;
                        }
                        if (p1.DistanceTo(_p0) < Precision_.TheShortestDistance || p1.DistanceTo(_p1) < Precision_.TheShortestDistance)
                        {
                            count1++;
                        }
                    }
                }

                if (count0 == 0 && count1 == 1)
                {
                    tempP = p1;
                    tempC = item01;
                    break;
                }
                else if (count0 == 1 && count1 == 0)
                {
                    tempP = p0;
                    tempC = item01;
                    break;
                }
            }

            // 基于第一根线排序坡道中心线
            List<Curve> result = new List<Curve>() { tempC.Clone() };

            curves.Remove(tempC);

            while (curves.Count != 0)//循环停止条件
            {
                tempC = GetNext(curves, tempP);//寻找相连线段

                // 寻找下一个连接点
                if (tempP.DistanceTo(tempC.GetEndPoint(1)) < Precision_.TheShortestDistance)
                {
                    tempP = tempC.GetEndPoint(0);
                }
                else if (tempP.DistanceTo(tempC.GetEndPoint(0)) < Precision_.TheShortestDistance)
                {
                    tempP = tempC.GetEndPoint(1);
                }

                result.Add(tempC.Clone());
                curves.Remove(tempC);
            }

            return result.ToCurveLoop();
        }
        /// <summary>
        /// 基于一个相交点，在一个曲线列表中找到与之相连的曲线
        /// </summary>
        Curve GetNext(List<Curve> curves, XYZ connected)//找到连接线段 便有返回值
        {
            foreach (Curve c in curves)
            {

                if (connected.DistanceTo(c.GetEndPoint(0)) < Precision_.TheShortestDistance)
                {
                    return c;
                }
                else if (connected.DistanceTo(c.GetEndPoint(1)) < Precision_.TheShortestDistance)
                {
                    return c.CreateReversed();
                }
            }
            throw new InvalidOperationException("当前线组中的线段不满足连续多段线条件");
        }
    }
}
