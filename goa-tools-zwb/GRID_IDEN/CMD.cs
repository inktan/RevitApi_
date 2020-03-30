using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

//添加轴号符号线

namespace GRID_IDEN
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //window
            Window1 window = new Window1();
            window.ShowDialog();
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            TaskDialog.Show("GOA","选择需要添加符号线的轴网点击左上角完成.");

            //get line length & offset
            double length = Convert.ToInt32(window.tb_length.Text)/304.8;

            //get activeview
            View activeview = uidoc.ActiveView;

            //get grids
            IList<Reference> selection = uidoc.Selection.PickObjects(ObjectType.Element, "选择需要添加符号线的轴网");

            // creat a list for group
            //List<DetailCurve> detaillines = new List<DetailCurve>();

            //get linestyle
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(GraphicsStyle));
            IEnumerable<GraphicsStyle> tmpgraphicsstyles = null;
            tmpgraphicsstyles = from tmp in collector
                                let gs = tmp as GraphicsStyle
                                where gs.Name == "G-GRID-IDEN"
                                select gs;
            if(tmpgraphicsstyles.Count() == 0) { TaskDialog.Show("wrong", "未找到名为G-GRID-IDEN的轴网");return Result.Failed; }
            GraphicsStyle graphicsstyle = tmpgraphicsstyles.ToList().First();

            //creat lines
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("批量添加轴号线");

                foreach (Reference refe in selection)
                {
                    Grid grid = doc.GetElement(refe.ElementId) as Grid;
                    if(grid == null) { continue; }
                    //exclude curve
                    if (grid.IsCurved) { TaskDialog.Show("wrong", "目前暂不支持曲线轴网"); return Result.Failed; }
                    Line gridline = grid.GetCurvesInView(DatumExtentType.ViewSpecific, activeview).First() as Line;
                    XYZ direction = gridline.Direction;
                    XYZ backdirection = direction.Multiply(-1);
                    XYZ origin = gridline.GetEndPoint(0);
                    XYZ origin2 = gridline.GetEndPoint(1);
                    XYZ linepoint1 = origin + (direction * length);
                    XYZ linepoint2 = origin2 + (backdirection * length);
                    Line line1 = Line.CreateBound(origin, linepoint1);
                    Line line2 = Line.CreateBound(origin2, linepoint2);
                    //creat line by IsBubbleVisibleInView
                    if (grid.IsBubbleVisibleInView(DatumEnds.End0, activeview))
                    {
                        DetailCurve detailline1 = doc.Create.NewDetailCurve(activeview, line1);
                        detailline1.LineStyle = graphicsstyle;
                        //detaillines.Add(detailline1);
                    }
                    if (grid.IsBubbleVisibleInView(DatumEnds.End1, activeview))
                    {
                        DetailCurve detailline2 = doc.Create.NewDetailCurve(activeview, line2);
                        detailline2.LineStyle = graphicsstyle;
                        //detaillines.Add(detailline2);
                    }
                }
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
