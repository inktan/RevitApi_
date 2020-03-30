using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


using System.Diagnostics;

namespace LINE_ROAD
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View activeview = uiDoc.ActiveView;

            double radius = 12000 / 304.8;
            double offset = 2000 / 304.8;


            FilteredElementCollector collector_temp = new FilteredElementCollector(doc);
            collector_temp.OfCategory(BuiltInCategory.OST_Lines).WhereElementIsNotElementType();
            DetailLine dl_temp = null;
            foreach(Element elem in collector_temp)
            {
                if(elem is DetailLine) { dl_temp = elem as DetailLine; }
            }
            if(dl_temp == null) { TaskDialog.Show("goa", "请用符号线绘制路中线"); return Result.Failed; }
            ICollection<ElementId> collection_id_linestyle = dl_temp.GetLineStyleIds();
            List<GraphicsStyle> list_linestyle = new List<GraphicsStyle>();
            foreach (ElementId id in collection_id_linestyle)
            {
                list_linestyle.Add(doc.GetElement(id) as GraphicsStyle);
            }

            UI ui = new UI();
            ui.combobox.ItemsSource = list_linestyle;
            int l = -1;
            foreach (GraphicsStyle gs in list_linestyle)
            {
                l++;
                if (gs.Name == "C-FIRE-FLOW") { break; }
            }
            ui.combobox.SelectedIndex = l;
            ui.ShowDialog();

            GraphicsStyle graphicsstyle = ui.combobox.SelectedItem as GraphicsStyle;

            radius = Convert.ToDouble(ui.textbox_radius.Text) / 304.8;
            offset = Convert.ToDouble(ui.textbox_offset.Text) / 2 / 304.8;



            //选择所有线
            IList<Element> list_DetailLines = uiDoc.Selection.PickElementsByRectangle(new SelectionFilter_DetailLine(), "选择符号线");
            List<Line> list_line = new List<Line>();
            foreach (Element elem in list_DetailLines)
            {
                DetailLine dl = elem as DetailLine;
                Line line = dl.GeometryCurve as Line;
                list_line.Add(line);
            }

            // List<Line> list_line_split = new List<Line>();
            //先打散所有线段.
            for (int i = 0; i < list_line.Count; i++)
            {
                for (int j = i + 1; j < list_line.Count; j++)
                {
                    if (list_line[i].Intersect(list_line[j]) == SetComparisonResult.Overlap)
                    {
                        List<Line> temps = SplitLine(list_line[i], list_line[j]);
                        if (temps == null) { continue; }
                        list_line.RemoveAt(j);
                        list_line.RemoveAt(i);
                        list_line.AddRange(temps);
                        i--;
                        break;
                    }
                }
            }


            List<Arc> list_arc = new List<Arc>();

            #region
            //将两根线的转折点做倒角(排除3根及以上线汇到一个点的情况)
            //for (int i = 0; i < list_line.Count; i++)
            //{
            //    for (int j = i + 1; j < list_line.Count; j++)
            //    {
            //        bool findthirdline = false;
            //        if (list_line[i].Intersect(list_line[j]) == SetComparisonResult.Overlap)
            //        {
            //            XYZ point_start_i = list_line[i].GetEndPoint(0);
            //            XYZ point_end_i = list_line[i].GetEndPoint(1);
            //            XYZ point_start_j = list_line[j].GetEndPoint(0);
            //            XYZ point_end_j = list_line[j].GetEndPoint(1);

            //            XYZ point_intersection = null;
            //            if (point_start_i.IsAlmostEqualTo(point_start_j)) { point_intersection = point_start_i; }
            //            if (point_start_i.IsAlmostEqualTo(point_end_j)) { point_intersection = point_start_i; }
            //            if (point_end_i.IsAlmostEqualTo(point_start_j)) { point_intersection = point_end_i; }
            //            if (point_end_i.IsAlmostEqualTo(point_end_j)) { point_intersection = point_end_i; }
            //            if (point_intersection == null) { continue; }

            //            for (int k = 0; k < list_line.Count; k++)
            //            {
            //                if (k == i || k == j) { continue; }
            //                XYZ point_start_k = list_line[k].GetEndPoint(0);
            //                XYZ point_end_k = list_line[k].GetEndPoint(1);
            //                if (point_intersection.IsAlmostEqualTo(point_start_k) || point_intersection.IsAlmostEqualTo(point_end_k))
            //                { findthirdline = true; break; }
            //            }
            //        }
            //        if (findthirdline) { continue; }
            //        else
            //        {
            //            Line newline1;
            //            Line newline2;
            //            Arc arc = Chamfer(list_line[i], list_line[j], radius, out newline1, out newline2);
            //            list_arc.Add(arc);
            //            list_line[i] = newline1;
            //            list_line[j] = newline2;

            //        }
            //    }
            //}
            #endregion

            //单线模式下,将两根线的转折点做倒角(排除3根及以上线汇到一个点的情况)
            for (int i = 0; i < list_line.Count; i++)
            {
                for (int c = 0; c < 2; c++)
                {
                    int temp = i;
                    List<int> list_int = new List<int>();
                    XYZ point_i = list_line[i].GetEndPoint(c);
                    for (int j = 0; j < list_line.Count; j++)
                    {
                        if (j == i) { continue; }
                        XYZ point_start_j = list_line[j].GetEndPoint(0);
                        XYZ point_end_j = list_line[j].GetEndPoint(1);
                        if (point_i.IsAlmostEqualTo(point_start_j) || point_i.IsAlmostEqualTo(point_end_j))
                        {
                            list_int.Add(j);
                        }
                    }
                    if (list_int.Count == 1)
                    {
                        int j = list_int[0];
                        Line newline1;
                        Line newline2;
                        Arc arc = Chamfer(list_line[i], list_line[j], radius, out newline1, out newline2);
                        list_arc.Add(arc);
                        list_line[i] = newline1;
                        list_line[j] = newline2;
                        i--; continue;
                    }
                    if (temp != i) { break; }
                }
            }


            //偏移,从单线变为双线.
            List<Arc> list_temp = new List<Arc>();
            foreach (Arc arc in list_arc)
            {
                list_temp.Add(arc.CreateOffset(offset, new XYZ(0, 0, -1)) as Arc);
                list_temp.Add(arc.CreateOffset(offset, new XYZ(0, 0, 1)) as Arc);
            }
            list_arc = list_temp;

            List<Line> list_temp2 = new List<Line>();
            foreach (Line line in list_line)
            {
                list_temp2.Add(line.CreateOffset(offset, new XYZ(0, 0, -1)) as Line);
                list_temp2.Add(line.CreateOffset(offset, new XYZ(0, 0, 1)) as Line);
            }
            list_line = list_temp2;




            //将偏移后的线,倒角.
            List<Arc> list_arc_nooffset = new List<Arc>();
            for (int i = 0; i < list_line.Count; i++)
            {
                for (int j = 0; j < list_line.Count; j++)
                {
                    if (i == j) { continue; }

                    //排除没有交点的线
                    IntersectionResultArray intersectionResultArray;
                    XYZ intersection;
                    if (list_line[i].Intersect(list_line[j], out intersectionResultArray) == SetComparisonResult.Overlap)
                    { intersection = intersectionResultArray.get_Item(0).XYZPoint; }
                    else
                    {continue;}

                    //排除首尾相接的线.(因为之前单线打断了以后才偏移,存在很多首尾相接的线)
                    XYZ point_start_temp = list_line[i].GetEndPoint(0);
                    XYZ point_end_temp = list_line[i].GetEndPoint(1);
                    XYZ point_start_temp2 = list_line[j].GetEndPoint(0);
                    XYZ point_end_temp2 = list_line[j].GetEndPoint(1);
                    if (point_start_temp.IsAlmostEqualTo(point_start_temp2) || point_start_temp.IsAlmostEqualTo(point_end_temp2)
                        || point_end_temp.IsAlmostEqualTo(point_start_temp2) || point_end_temp.IsAlmostEqualTo(point_end_temp2))
                    {continue; }

                    Line newline1;
                    Line newline2;
                    Arc arc = Chamfer(list_line[i], list_line[j], radius, out newline1, out newline2);
                    if (arc == null) { continue; }
                    list_arc_nooffset.Add(arc);
                    list_line[i] = newline1;
                    list_line[j] = newline2;
                }
            }

            //foreach (Arc arc in list_arc_nooffset)
            //{
            //    XYZ point_arc = GetIntersectionByArcTangent(arc);
            //    for (int i = 0; i < list_line.Count; i++)
            //    {
            //        IntersectionResultArray intersectionResultArray;
            //        XYZ intersection;
            //        if (arc.Intersect(list_line[i], out intersectionResultArray) == SetComparisonResult.Overlap)
            //        { intersection = intersectionResultArray.get_Item(0).XYZPoint; }
            //        else
            //        { continue; }
            //        XYZ point_start_line = list_line[i].GetEndPoint(0);
            //        XYZ point_end_line = list_line[i].GetEndPoint(1);

            //        if (point_arc.IsAlmostEqualTo(point_start_line))
            //        { list_line[i] = Line.CreateBound(intersection, point_end_line); i--; }
            //        else if (point_arc.IsAlmostEqualTo(point_end_line))
            //        { list_line[i] = Line.CreateBound(intersection, point_start_line); i--; }

            //    }
            //}



            ////将路封闭
            //List<Line> list_line_Closed = new List<Line>();
            //for (int i = 0; i < list_line.Count-1; i++)
            //{
            //    for (int x = 0; x < 2; x++)
            //    {
            //        XYZ point_1 = list_line[i].GetEndPoint(x);

            //        for (int j = i + 1; j < list_line.Count; j++)
            //        {
            //            for (int y = 0; y < 2; y++)
            //            {
            //                XYZ point_2 = list_line[j].GetEndPoint(y);
            //                double distance_temp = point_1.DistanceTo(point_2);
            //                if (AlmostEqual(distance_temp, offset*2))
            //                {
            //                    Line line_close = Line.CreateBound(point_1, point_2);
            //                    list_line_Closed.Add(line_close);
            //                }
            //            }
            //        }
            //    }
            //}



            using (Transaction offset_line = new Transaction(doc))
            {
                offset_line.Start("start");
                foreach (Arc arc in list_arc_nooffset)
                {
                    DetailCurve dc1 = doc.Create.NewDetailCurve(activeview, arc);
                    DetailCurve dc2 = doc.Create.NewDetailCurve(activeview, arc);
                    dc1.LineStyle = graphicsstyle; 
                    dc2.LineStyle = graphicsstyle;
                }
                foreach (Arc arc in list_arc)
                {
                    DetailCurve dc1 = doc.Create.NewDetailCurve(activeview, arc);
                    DetailCurve dc2 = doc.Create.NewDetailCurve(activeview, arc);
                    dc1.LineStyle = graphicsstyle;
                    dc2.LineStyle = graphicsstyle;
                }
                foreach (Line line in list_line)
                {
                    DetailCurve dc1 = doc.Create.NewDetailCurve(activeview, line);
                    DetailCurve dc2 = doc.Create.NewDetailCurve(activeview, line);
                    dc1.LineStyle = graphicsstyle;
                    dc2.LineStyle = graphicsstyle;
                }

                offset_line.Commit();
            }
















            return Result.Succeeded;
        }

        public List<Curve> Offset(Curve curve, double distance)
        {
            List<Curve> curves = new List<Curve>();
            curves.Add(curve.CreateOffset(distance, new XYZ(0, 0, 1)) as Curve);
            curves.Add(curve.CreateOffset(distance, new XYZ(0, 0, -1)) as Curve);
            return curves;
        }

        public Arc Chamfer(Line line1, Line line2, double radius, out Line newline1, out Line newline2)
        {

            List<Line> lines = TwoLineOneOrigin(line1, line2);

            //get intersection
            XYZ intersection = GetIntersection(line1, line2);
            if (intersection == null)
            {
                newline1 = null;
                newline2 = null;
                return null;
            }

            XYZ direction_line1 = lines[0].Direction;
            XYZ direction_line2 = lines[1].Direction;
            double angle = direction_line1.AngleTo(direction_line2);
            if (angle > Math.PI) { angle = Math.PI * 2 - angle; }

            XYZ point_arc_start = intersection.Add(direction_line1.Multiply(radius / (Math.Tan(angle / 2))));
            XYZ point_arc_end = intersection.Add(direction_line2.Multiply(radius / (Math.Tan(angle / 2))));

            XYZ direction_line3 = direction_line1.Add(direction_line2).Normalize();

            XYZ point_arc_middle = intersection.Add(direction_line3.Multiply((radius / Math.Sin(angle / 2)) - radius));

            Arc arc = Arc.Create(point_arc_start, point_arc_end, point_arc_middle);

            newline1 = Line.CreateBound(point_arc_start, lines[0].GetEndPoint(1));
            newline2 = Line.CreateBound(point_arc_end, lines[1].GetEndPoint(1));

            return arc;
        }

        public List<Line> TwoLineOneOrigin(Line line1, Line line2)
        {
            List<Line> lines = new List<Line>();

            XYZ intersection = GetIntersection(line1, line2);
            if (intersection == null) { return null; }

            XYZ xyz_start1 = line1.GetEndPoint(0);
            XYZ xyz_end1 = line1.GetEndPoint(1);
            XYZ xyz_start2 = line2.GetEndPoint(0);
            XYZ xyz_end2 = line2.GetEndPoint(1);

            Line newline1 = null;
            Line newline2 = null;

            if (intersection.DistanceTo(xyz_start1) > intersection.DistanceTo(xyz_end1))
            {
                newline1 = Line.CreateBound(intersection, xyz_start1);
            }
            else
            {
                newline1 = Line.CreateBound(intersection, xyz_end1);
            }

            if (intersection.DistanceTo(xyz_start2) > intersection.DistanceTo(xyz_end2))
            {
                newline2 = Line.CreateBound(intersection, xyz_start2);
            }
            else
            {
                newline2 = Line.CreateBound(intersection, xyz_end2);
            }

            lines.Add(newline1);
            lines.Add(newline2);

            return lines;
        }

        public XYZ GetIntersection(Line line1, Line line2)
        {
            IntersectionResultArray intersectionResultArray;
            XYZ intersection;
            if (line1.Intersect(line2, out intersectionResultArray) == SetComparisonResult.Overlap)
            {
                intersection = intersectionResultArray.get_Item(0).XYZPoint;
            }
            else
            {
                return null;
            }
            return intersection;
        }

        //打散所有线段,此处解决两根线端点对端点连接的情况. L型的话,返回null,即表示不需要做处理.
        public List<Line> SplitLine(Line line1, Line line2)
        {
            List<Line> lines = new List<Line>();
            XYZ point_start_1 = line1.GetEndPoint(0);
            XYZ point_end_1 = line1.GetEndPoint(1);
            XYZ point_start_2 = line2.GetEndPoint(0);
            XYZ point_end_2 = line2.GetEndPoint(1);
            if (point_start_1.IsAlmostEqualTo(point_start_2) || point_start_1.IsAlmostEqualTo(point_end_2)
            || point_end_1.IsAlmostEqualTo(point_start_2) || point_end_1.IsAlmostEqualTo(point_end_2))
            {
                //lines.Add(line1);
                //lines.Add(line2);
                //return lines;
                return null;
            }


            XYZ intersection = GetIntersection(line1, line2);
            if (intersection == null) { return null; }
            if (!intersection.IsAlmostEqualTo(point_start_1))
            {
                lines.Add(Line.CreateBound(intersection, point_start_1));
            }
            if (!intersection.IsAlmostEqualTo(point_start_2))
            {
                lines.Add(Line.CreateBound(intersection, point_start_2));
            }
            if (!intersection.IsAlmostEqualTo(point_end_1))
            {
                lines.Add(Line.CreateBound(intersection, point_end_1));
            }
            if (!intersection.IsAlmostEqualTo(point_end_2))
            {
                lines.Add(Line.CreateBound(intersection, point_end_2));
            }
            return lines;

        }

        //求Arc的切线的交点.
        public XYZ GetIntersectionByArcTangent(Arc arc)
        {
            XYZ center = arc.Center;
            XYZ start = arc.GetEndPoint(0);
            XYZ end = arc.GetEndPoint(1);
            Line line_start = Line.CreateBound(center, start);
            Line line_end = Line.CreateBound(center, end);
            XYZ direction_start = line_start.Direction;
            XYZ direction_end = line_end.Direction;
            XYZ direction_center = direction_start.Add(direction_end).Normalize();
            double angle = direction_start.AngleTo(direction_center);
            double radius = arc.Radius;
            double length = radius / (Math.Cos(angle));
            XYZ result = center.Add(direction_center.Multiply(length));
            return result;
        }


        public const double precision = 0.000001;

        public bool AlmostEqual(double x, double y)
        {
            if (Math.Abs(x - y) <= precision)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AlmostEqual(XYZ point_1, XYZ point_2)
        {
            return point_1.IsAlmostEqualTo(point_2, precision);
        }
        public bool AlmostEqual(Line line1, Line line2)
        {
            XYZ point_start_1 = line1.GetEndPoint(0);
            XYZ point_end_1 = line1.GetEndPoint(1);
            XYZ point_start_2 = line2.GetEndPoint(0);
            XYZ point_end_2 = line2.GetEndPoint(1);

            if (AlmostEqual(point_start_1, point_start_2) && AlmostEqual(point_end_1, point_end_2))
            {
                return true;
            }
            else if (AlmostEqual(point_start_1, point_end_2) && AlmostEqual(point_end_1, point_start_2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

    public class SelectionFilter_DetailLine : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is DetailLine) { return true; }
            else { return false; }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
