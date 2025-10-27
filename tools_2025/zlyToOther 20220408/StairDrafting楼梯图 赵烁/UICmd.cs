using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using goa.Common;
using goa.Common.Exceptions;

namespace StairDrafting
{
    internal static class UICmd
    {
        internal static void drawStairStructLines()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new ElementClassSelectionFilter<Autodesk.Revit.DB.Architecture.StairsRun>();
            Form_cursorPrompt.Start("选择一个楼梯梯段。", APP.MainWindow);
            var pick = sel.PickObject(ObjectType.Element, filter);
            Form_cursorPrompt.Stop();
            var run = doc.GetElement(pick) as Autodesk.Revit.DB.Architecture.StairsRun;
            var view = uidoc.ActiveView;
            var offset = APP.MainWindow.Offset;
            var ls = APP.MainWindow.LineStyle;
            drawStairStructLines(run, offset, view, ls);
        }
        private static void drawStairStructLines(Autodesk.Revit.DB.Architecture.StairsRun _run, double _offset, View _view, GraphicsStyle _lineStyle)
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = _run.Document;
            //get path line
            var pathLine = _run.GetStairsPath().First() as Line;
            if (pathLine == null)
                throw new CommonUserExceptions("仅支持直线梯段。");
            //get path line project onto view plane
            var cutPlane = _view.GetViewCutPlane();
            var lineProj = pathLine.ProjectOntoPlane(cutPlane);
            var pathDir = lineProj.Direction;
            //get starting line
            double tread = lineProj.Length / (double)_run.ActualTreadsNumber;
            XYZ treadV = pathDir * tread;
            //get width from boundary
            //works for both sketch-based and rule-based run.
            var boundaryProj = _run.GetFootprintBoundary()
                .Where(x => x is Line)
                .Cast<Line>()
                .Select(x => x.ProjectOntoPlane(cutPlane))
                .ToList();
            var vertices = boundaryProj.Select(x => x.GetEndPoint(0)).ToList();
            var viewDir = _view.ViewDirection;
            var cross = viewDir.CrossProduct(pathDir);
            var width = vertices.DimensionAlong(cross);
            //get start line
            XYZ start = lineProj.GetEndPoint(0) - width * 0.5 * cross;
            if (!_run.BeginsWithRiser)
                start += treadV;
            start += pathDir * _offset;
            var startLine = Line.CreateBound(start, start + cross * width);
            //create array
            var opt = new Options();
            opt.ComputeReferences = true;
            using (Transaction trans = new Transaction(doc, "test"))
            {
                trans.Start();
                var dl = doc.Create.NewDetailCurve(_view, startLine);
                dl.LineStyle = _lineStyle;
                var array = LinearArray.Create(doc, _view, dl.Id, _run.ActualRisersNumber, treadV, ArrayAnchorMember.Second);
                var memberIds = array.GetAllMemberIds();
                memberIds = new List<ElementId>() { memberIds.First(), memberIds.Last() };
                var memberGroups = memberIds.Select(x => doc.GetElement(x)).Cast<Group>();
                var dlLines = memberGroups.Select(x => doc.GetElement(x.GetMemberIds().First()).get_Geometry(opt).First() as Line);
                var refs = dlLines.Select(x => x.Reference).ToReferenceArray();
                doc.Regenerate();
                Form_cursorPrompt.Start("选择一个点，放置尺寸标注。", APP.MainWindow);
                var sel = uidoc.Selection;
                Form_cursorPrompt.Stop();
                var dimPoint = sel.PickPoint();
                var dimLine = Line.CreateUnbound(dimPoint, pathDir);
                var dim = doc.Create.NewDimension(_view, dimLine, refs);
                //set value text override
                string dist = UnitUtils.ConvertFromInternalUnits(tread, DisplayUnitType.DUT_MILLIMETERS).ToString();
                string totalDist = UnitUtils.ConvertFromInternalUnits(lineProj.Length, DisplayUnitType.DUT_MILLIMETERS).ToString();
                string text = dist + "x" + _run.ActualRisersNumber + "=" + totalDist;
                dim.ValueOverride = text;
                trans.Commit();
            }
        }
    }
}
