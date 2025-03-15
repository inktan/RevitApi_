using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;

namespace BSMT_PpLayout
{

    class BatchBreakLine : RequestMethod
    {
        public BatchBreakLine(UIApplication uiapp) : base(uiapp)
        {
        }
        /// <summary>
        /// 批量打断车道中心线
        /// </summary>
        internal override void Execute()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();

            List<Element> elements = new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_Lines).WhereElementIsNotElementType().ToElements().ToList();
            IList<Element> selEles = sel.PickElementsByRectangle(new SelPickFilter_Line(),"请选择地库停车子区域边线");
            //List<Element> intrEles = new List<Element>();
            List<Segment2d> segment2ds = new List<Segment2d>();

            //【】找打需要被打断的线
            foreach (Element element01 in selEles)
            {
                if (!(element01 is CurveElement)) continue;

                CurveElement curveElement01 = element01 as CurveElement;
                Curve curve01 = curveElement01.GeometryCurve;
                Line line01 = Line.CreateBound(curve01.GetEndPoint(0), curve01.GetEndPoint(1));
                Segment2d segment2d01 = line01.ToSegment2d();

                List<Vector2d> vector2ds = new List<Vector2d>();

                //【】辅助线
                foreach (Element element02 in elements)
                {
                    if (!(element02 is CurveElement)) continue;

                    CurveElement curveElement02 = element02 as CurveElement;
                    Curve curve02 = curveElement02.GeometryCurve;
                    if (curve02.IsCyclic) continue;
                    Line line02 = Line.CreateBound(curve02.GetEndPoint(0), curve02.GetEndPoint(1));
                    Segment2d segment2d02 = line02.ToSegment2d();
                    //将线段延伸一个距离，保证能够相交

                    segment2d02 = segment2d02.TwoWayExtension(1.0); 

                    IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2d01, segment2d02);
                    intrSegment2Segment2.Compute();
                    if(intrSegment2Segment2.Quantity==1)
                    {
                        Vector2d vector2d = intrSegment2Segment2.Point0;
                        vector2ds.Add(vector2d);
                    }
                }
                if (vector2ds.Count > 0)
                {
                    //intrEles.Add(element01);
                    vector2ds.Add(segment2d01.P0);
                    vector2ds.Add(segment2d01.P1);
                    if(segment2d01.Direction.y.EqualPrecision(1)|| segment2d01.Direction.y.EqualPrecision(-1))
                    {
                        vector2ds = vector2ds.DelDuplicate().OrderBy(p => p.y).ToList();
                    }
                    else
                    {
                        vector2ds = vector2ds.DelDuplicate().OrderBy(p => p.x).ToList();
                    }
                    //【】拿到所有的断线
                    for (int i = 0; i < vector2ds.Count-1; ++i)
                        segment2ds.Add( new Segment2d(vector2ds[i], vector2ds[(i + 1) % vector2ds.Count]));
                }
            }

            if (segment2ds.Count < 1) return;

            CurveArray curveArray = segment2ds.ToLines().ToCurveArray();
            GraphicsStyle graphicsStyle = doc.GetGraphicsStyleByName("地库_主车道中心线");

            using (Transaction trans = new Transaction(doc, "creatLines"))
            {
                trans.Start();
                doc.Delete(selEles.Select(p => p.Id).ToList());// 删除已有的线
                DetailCurveArray detailCurveArray=  doc.Create.NewDetailCurveArray(view,curveArray);
                foreach (DetailCurve detailCurve in detailCurveArray)
                {
                    detailCurve.LineStyle = graphicsStyle;
                }
                trans.Commit();
            }
         

        }
    }
}
