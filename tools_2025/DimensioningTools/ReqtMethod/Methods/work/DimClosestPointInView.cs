using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using goa.Common.Exceptions;
using Autodesk.Revit.UI;

namespace DimensioningTools
{
    internal   class DimClosestPointInView : RequestMethod
    {
        internal DimClosestPointInView(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute() { 
            //get selected dim type
            DimensionType dt = null;
            if (sel.GetElementIds().Count > 0)
            {
                var item = doc.GetElement(sel.GetElementIds().First()) as Dimension;
                if (item != null)
                    dt = item.DimensionType;
            }

            //select two elems
            var pick1 = sel.PickObject(ObjectType.Element, "选择第一个图元");
            var pick2 = sel.PickObject(ObjectType.Element, "选择第二个图元");
            Form_cursorPrompt.Stop();
            var elem1 = doc.GetElement(pick1);
            var elem2 = doc.GetElement(pick2);
            //get vertical edges
            var plane = view.GetViewBasePlane();
            var dir = plane.Normal;
            var edges1 = elem1.GetAllSolids(true, view).SelectMany(x => getStraightEdgesOfDir(x, dir)).ToList();
            var edges2 = elem2.GetAllSolids(true, view).SelectMany(x => getStraightEdgesOfDir(x, dir)).ToList();
            if (edges1.Count == 0 || edges2.Count == 0)
                throw new CommonUserExceptions("某个图元未找到垂直于视图的边。");
            //find two closest
            double min = double.MaxValue;
            Edge[] pair = new Edge[2];
            XYZ p1, p2;

            foreach (var e1 in edges1)
            {
                p1 = plane.ProjectOnto(e1.AsCurve().GetEndPoint(0));
                foreach (var e2 in edges2)
                {
                    p2 = plane.ProjectOnto(e2.AsCurve().GetEndPoint(0));
                    var dist = p1.SqDist(p2);
                    if (dist < min)
                    {
                        min = dist;
                        pair[0] = e1;
                        pair[1] = e2;
                    }
                }
            }
            //create dimension
            p1 = plane.ProjectOnto(pair[0].AsCurve().GetEndPoint(0));
            p2 = plane.ProjectOnto(pair[1].AsCurve().GetEndPoint(0));
            var line = Line.CreateBound(p1, p2);
            var ra = new ReferenceArray();
            ra.Append(pair[0].Reference);
            ra.Append(pair[1].Reference);
            //debug
            var edge1Line = pair[0].AsCurve() as Line;
            var edge2Line = pair[1].AsCurve() as Line;
            var ref1 = pair[0].Reference;
            var ref2 = pair[1].Reference;
            Debug.WriteLine("edge1: " + edge1Line.ToStringEnds(2));
            Debug.WriteLine("edge2: " + edge2Line.ToStringEnds(2));
            Debug.WriteLine("p1: " + p1.ToStringDigits(2));
            Debug.WriteLine("p2: " + p2.ToStringDigits(2));
            Debug.WriteLine("ref1: " + ref1.ConvertToStableRepresentation(doc));
            Debug.WriteLine("ref2: " + ref2.ConvertToStableRepresentation(doc));
            //debug end
            int id;
            using (Transaction trans = new Transaction(doc, "标注最近点"))
            {
                trans.Start();
                if (dt == null)
                {
                    var newDim = doc.Create.NewDimension(view, line, ra);
                    id = newDim.Id.IntegerValue;
                }
                else
                {
                    var newDim = doc.Create.NewDimension(view, line, ra, dt);
                    id = newDim.Id.IntegerValue;
                }
                trans.Commit();
            }
        }

        private  List<Edge> getStraightEdgesOfDir(Solid _solid, XYZ _dir)
        {
            var list = new List<Edge>();
            if (_solid == null || _solid.Volume == 0)
                return list;
            foreach (Edge edge in _solid.Edges)
            {
                var c = edge.AsCurve();
                if (c is Line)
                {
                    var l = c as Line;
                    if (l.Direction.IsParallel(_dir))
                        list.Add(edge);
                }
            }
            return list;
        }
    }
}
