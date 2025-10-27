using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;

namespace BSMT_Regions
{
    internal static class myMethods
    {
        internal static List<Area> PickAreas(UIDocument _uidoc)
        {
            var doc = _uidoc.Document;
            var sel = _uidoc.Selection;
            var filter = new AreaSelectionFilter();
            var pickrefs = sel.PickObjects(ObjectType.Element, filter);
            var areas = pickrefs.Select(x => doc.GetElement(x) as Area).Cast<Area>().ToList();
            return areas;
        }
        internal static void CreateFromArea(List<Area> _areas, double _offset, bool _inward, View _view)
        {
            var doc = _areas.First().Document;
            var allBoundaries = new List<Curve>();
            foreach (var area in _areas)
            {
                var boundaries = area
                    .GetBoundarySegments(new SpatialElementBoundaryOptions());
                var validSorted = new List<CurveLoop>();
                foreach (var b in boundaries)
                {
                    try
                    {
                        var curves = b.Select(x => x.GetCurve()).ToList();
                        bool isOpen;
                        var cl = curves.SortCurvesContiguousAsCurveLoop(out isOpen);
                        if (cl.IsOpen())
                            continue;
                        else
                            validSorted.Add(cl);
                    }
                    catch (InvalidCurveLoopException ex)
                    {
                        continue;
                    }
                }
                foreach (var cl in validSorted)
                {
                    bool ccw = cl.IsCounterclockwise(_view.ViewDirection);
                    if (!ccw)
                        cl.Flip();
                    var offset = getOffsetCurves(cl.ToList(), _offset, _view.ViewDirection, _inward);
                    if (offset != null)
                        allBoundaries.AddRange(offset);
                }
            }
            if (allBoundaries.Count == 0)
            {
                return;
            }
            var lineStyle = new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle))
                .First(x => x.Name == "地库_防火分区")
                as GraphicsStyle;
            using (TransactionGroup tg = new TransactionGroup(doc, "创建面积边界"))
            {
                tg.Start();
                List<ElementId> dls = new List<ElementId>();
                using (Transaction trans = new Transaction(doc, "create detail curves"))
                {
                    trans.Start();
                    foreach (var c in allBoundaries)
                    {
                        var dl = doc.Create.NewDetailCurve(_view, c);
                        dl.LineStyle = lineStyle;
                        dls.Add(dl.Id);
                    }
                    trans.Commit();
                }
                using (Transaction trans = new Transaction(doc, "create group"))
                {
                    trans.Start();
                    var group = doc.Create.NewGroup(dls);
                    trans.Commit();
                }
                tg.Assimilate();
            }
        }
        private static List<Curve> getOffsetCurves(List<Curve> _input, double _offset, XYZ _n, bool _inward)
        {
            var cl = CurveLoop.Create(_input);
            var plane = cl.GetPlane();
            double o = _offset;
            if (_inward)
                o *= -1;
            //try offset, if internal error, reduce offset by 0.7, try until succeed.
            bool succeed = false;
            double ratio = 1;
            CurveLoop offset = null;
            for (int i = 0; i < 99; i++)
            {
                try
                {
                    offset = CurveLoop.CreateViaOffset(cl, o * ratio, _n);
                    break;
                }
                catch (Autodesk.Revit.Exceptions.InternalException ex)
                {
                    ratio *= 0.7;
                }
            }
            return offset.ToList();
        }
    }
}
