using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


namespace CADToRevit_Wall
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            View view = uidoc.ActiveView;
            ElementId levelid = view.GenLevel.Id;

            bool iscontinue = true;

            do
            {
                List<Element> eles = new List<Element>();
                try
                {
                    eles = GetSelected(uidoc);
                }
                catch
                {
                    iscontinue = false;
                    continue;
                }
                if(eles == null) { TaskDialog.Show("wrong", "选择的符号线的数量不为2.");  continue; }


                //get line_closer & line_farther
                Line line1 = (eles[0] as DetailLine).GeometryCurve as Line;
                Line line2 = (eles[1] as DetailLine).GeometryCurve as Line;
                if (line1 == null || line2 == null) { TaskDialog.Show("wrong", "仅支持直线."); continue; }
                Line line1unbound = (eles[0] as DetailLine).GeometryCurve as Line;
                line1unbound.MakeUnbound();
                Line line2unbound = (eles[1] as DetailLine).GeometryCurve as Line;
                line2unbound.MakeUnbound();
                //get distance from origin
                IntersectionResult result1 = line1unbound.Project(new XYZ(-1000/304.8, -1000 / 304.8, line1.GetEndPoint(0).Z));
                double distance1 = result1.Distance;
                IntersectionResult result2 = line2unbound.Project(new XYZ(-1000 / 304.8, -1000 / 304.8, line2.GetEndPoint(0).Z));
                double distance2 = result2.Distance;
                if (IsAlmostEqual(distance1, distance2)) { TaskDialog.Show("wrong", "两条线间距太小."); continue; }
                Line line_closer = null;
                Line line_farther = null;
                Line line_closer_unbound = null;
                Line line_farther_unbound = null;
                if (distance1 > distance2)
                {
                    line_closer = line2;
                    line_farther = line1;
                    line_closer_unbound = line2unbound;
                    line_farther_unbound = line1unbound;
                }
                else
                {
                    line_closer = line1;
                    line_farther = line2;
                    line_closer_unbound = line1unbound;
                    line_farther_unbound = line2unbound;
                }

                //Determine parallel
                XYZ dir1 = line_closer.Direction;
                XYZ dir2 = line_farther.Direction;
                bool isparallel = dir1.IsAlmostEqualTo(dir2) || dir1.IsAlmostEqualTo(-dir2);
                if (!isparallel) { TaskDialog.Show("wrong", "请选择平行的线."); continue; }

                //get points
                //makeunbound
                XYZ startpoint_closerline = line_closer.GetEndPoint(0);
                XYZ endpoint_closerline = line_closer.GetEndPoint(1);
                XYZ startpoint_fartherline = line_farther.GetEndPoint(0);
                XYZ endpoint_fartherline = line_farther.GetEndPoint(1);

                //get width of wall
                IntersectionResult intersection = line_closer_unbound.Project(startpoint_fartherline);
                double distance = intersection.Distance;
                XYZ point_intersection = intersection.XYZPoint;
                Line normalline = Line.CreateBound(point_intersection, startpoint_fartherline);
                XYZ normallinedir = normalline.Direction;

                //offset
                Line wallline = null;
                if (line_closer.Length > line_farther.Length)
                {
                    wallline = OffsetLine(line_closer, normallinedir, distance / 2);
                }
                else
                {
                    wallline = OffsetLine(line_farther, -normallinedir, distance / 2);
                }

                //get material
                FilteredElementCollector materialcollector = new FilteredElementCollector(doc);
                materialcollector.OfCategory(BuiltInCategory.OST_Materials).OfClass(typeof(Material));
                IEnumerable<Material> materials = from ele in materialcollector
                                                  let material = ele as Material
                                                  where material.Name == "BRIK"
                                                  select material;
                if (materials.Count() == 0) { TaskDialog.Show("wrong", "未找到BRIK材质"); continue; }
                ElementId materialid = materials.First().Id;

                //get walltype
                FilteredElementCollector walltypecollector = new FilteredElementCollector(doc);
                walltypecollector.OfClass(typeof(WallType)).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();
                IEnumerable<WallType> walltypes = from element in walltypecollector
                                                  let walltypetemp = element as WallType
                                                  let para = walltypetemp.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM)
                                                  where walltypetemp.Name.Contains("A-WALL-INTR")
                                                  where walltypetemp.Kind == WallKind.Basic
                                                  //  where para.AsDouble() == distance 
                                                  where IsAlmostEqual(para.AsDouble(), distance)
                                                  select walltypetemp;
                if (walltypecollector.Count() == 0) { TaskDialog.Show("wrong", "请先随意新建一个类型."); continue; }
                WallType typetemp = null;
                foreach(WallType type in walltypecollector)
                {
                    if(type.Kind == WallKind.Basic) { typetemp = type;break; }
                }
                if(typetemp == null) { TaskDialog.Show("wrong", "请先随意新建一个内墙类型."); continue; }
                WallType walltype = null;
                if (walltypes.Count() == 0)
                {
                    TaskDialog.Show("wrong", "未找到合适的类型,将新建一个类型");
                    using (Transaction createwalltype = new Transaction(doc))
                    {
                        createwalltype.Start("新建墙类型");

                        walltype = typetemp.Duplicate("A-WALL-INTR-" + Convert.ToInt32((distance * 304.8)).ToString()) as WallType;
                        CompoundStructure compoundstructure = CompoundStructure.CreateSingleLayerCompoundStructure(MaterialFunctionAssignment.Structure, distance, materialid);
                        walltype.SetCompoundStructure(compoundstructure);

                        createwalltype.Commit();
                    }

                }
                else
                { walltype = walltypes.First(); }


                //transaction

                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("生成墙");

                    Wall wall = Wall.Create(doc, wallline, walltype.Id, levelid, 10, 0, false, false);

                    transaction.Commit();
                }




            }
            while (iscontinue);





            return Result.Succeeded;
        }


        public List<Element> GetSelected(UIDocument uidoc)
        {
            List<Element> eles = uidoc.Selection.PickElementsByRectangle(new SelectionFilter(), "框选两条线以生成墙.").ToList();
            if (eles.Count != 2) {  return null; }
            return eles;
        }

        public bool IsAlmostEqual(double a, double b)
        {
            if (Math.Abs(a - b) <= 0.0001) { return true; }
            else { return false; }
        }
        public Line OffsetLine(Line line, XYZ dir, double distance)
        {
            XYZ s = line.GetEndPoint(0);
            XYZ e = line.GetEndPoint(1);
            XYZ news = s + (dir * distance);
            XYZ newe = e + (dir * distance);
            return Line.CreateBound(news, newe);
        }

        public string GetXYZString(XYZ xyz)
        {
            return "X:" + xyz.X.ToString() + " Y:" + xyz.Y.ToString() + " Z:" + xyz.Z.ToString();
        }
        public string GetLineString(Line line)
        {
            XYZ s = line.GetEndPoint(0);
            XYZ e = line.GetEndPoint(1);
            return GetXYZString(s) + "\n" + GetXYZString(e);
        }

    }


    public class SelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if ((BuiltInCategory)elem.Category.Id.IntegerValue == BuiltInCategory.OST_Lines && elem.ViewSpecific)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }


}
