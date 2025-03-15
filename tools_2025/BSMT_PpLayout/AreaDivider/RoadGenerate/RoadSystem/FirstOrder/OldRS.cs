using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using g3;
using ClipperLib;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class OldRS
    {
        internal List<BoundSeg> RevitEleLines { get; }
        internal OldRS(Bsmt bsmt)
        {
            this.RevitEleLines = bsmt.InBoundEleLines;
            this.BsmtPoly2d = bsmt.Polygon2dInward;

            this.Routes = this.AllBreakSeg2ds.Select(p => new Route(p)).ToList();
            this.LocationPoint = bsmt.BsmtBound.Polygon2dInward.Center().ToXYZ();
        }

        //internal List<ElementId> EleIds => this.RevitEleLines.Select(p => p.Id).ToList();
        internal XYZ LocationPoint { get; set; }
       
        internal List<XYZ> RoutePoints
        {
            get
            {
                List<XYZ> result = new List<XYZ>();
                this.RevitEleLines.ForEach(p =>
                {
                    result.AddRange(p.Segment2d.EndPoints().ToXyzs());
                    //Curve c = (p.Ele as DetailCurve).GeometryCurve;
                    //if (c is Line)
                    //{
                    //    result.Add(c.GetEndPoint(0));
                    //    result.Add(c.GetEndPoint(1));
                    //}
                    //else if (c is Arc)
                    //{
                    //    Arc arc = c as Arc;
                    //    if (!arc.IsCyclic)
                    //    {
                    //        result.Add(c.GetEndPoint(0));
                    //        result.Add(c.GetEndPoint(1));
                    //    }
                    //}
                });
                return result;
            }
        }
        internal List<Segment2d> OldSeg2ds => this.RevitEleLines.Select(p => p.Segment2d).ToList();
        internal List<Segment2d> CutSeg2ds => CutRoadNet();
        internal List<Segment2d> AllBreakSeg2ds => this.CutSeg2ds.BreakSegment2ds();
        /// <summary>
        /// 该路径为打断后的路径
        /// </summary>
        internal List<Route> Routes { get; set; }

        internal void UpdatePathProperties()
        {
            List<Vector2d> brokenRoadEndPoints = new List<Vector2d>();
            this.AllBreakSeg2ds.ForEach(p => { brokenRoadEndPoints.AddRange(p.EndPoints()); });// 获取所有线段的端点

            foreach (var item in this.Routes)
            {
                item.CalEendProperty(brokenRoadEndPoints);
            }
        }

        Polygon2d BsmtPoly2d { get; set; }
        /// <summary>
        /// 使用地库线圈裁剪当前路网体系
        /// </summary>
        /// <returns></returns>
        List<Segment2d> CutRoadNet()
        {
            List<Segment2d> result = new List<Segment2d>();

            foreach (var item in this.OldSeg2ds)
            {
                List<Vector2d> vector2ds = this.BsmtPoly2d.FindInterSectionPoint(item,2.0);
                Segment2d temp = new Segment2d();
                if (vector2ds.Count == 1)
                {
                    Vector2d intr = vector2ds[0];
                    bool isIn00 = this.BsmtPoly2d.Contains(item.P0);
                    bool isIn01 = this.BsmtPoly2d.Contains(item.P1);
                    if (isIn00 && isIn01)
                    {
                        double dis00 = intr.Distance(item.P0);
                        double dis01 = intr.Distance(item.P1);

                        if (dis00 > dis01)
                        {
                            temp = new Segment2d(intr, item.P0);
                        }
                        else
                        {
                            temp = new Segment2d(intr, item.P1);
                        }
                    }
                    else if (isIn00)
                    {
                        temp = new Segment2d(intr, item.P0);
                    }
                    else
                    {
                        temp = new Segment2d(intr, item.P1);
                    }
                }
                else if (vector2ds.Count == 2)
                {
                    temp = new Segment2d(vector2ds[0], vector2ds[1]);
                }
                else
                {
                    temp = item;
                }
                if (temp.Length > Precision_.TheShortestDistance)
                {
                    result.Add(temp);
                }
            }

            return result;
        }
    }
}
