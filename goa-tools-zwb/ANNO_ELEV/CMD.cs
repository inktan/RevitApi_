using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

//楼板标注

namespace ANNO_ELEV
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //get activeview
            View activeview = uidoc.ActiveView;
            if (!(activeview is ViewPlan)) { TaskDialog.Show("wrong", "仅支持在平面内运行"); return Result.Failed; }

            //get type
            FilteredElementCollector typecollector = new FilteredElementCollector(doc);
            typecollector.OfClass(typeof(SpotDimensionType));
            SpotDimensionType type = null;
            foreach (Element ele in typecollector)
            {
                SpotDimensionType tmp = ele as SpotDimensionType;
                if (tmp == null) { continue; }
                if (tmp.Name == "A-LEVL-SLAB-2.5")
                { type = tmp; break; }
            }
            if (type == null)
            { TaskDialog.Show("GOA", "未找到<A-LEVL-SLAB-2.5>类型."); return Result.Failed; }

            List<SpotDimension> allsds = new List<SpotDimension>();
            string report = " ";

            //transaction
            using (Transaction transaction1 = new Transaction(doc))
            {
                transaction1.Start("creat elevation");
                //get floor
                FilteredElementCollector collector = new FilteredElementCollector(doc, activeview.Id);
                collector.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType();
                List<ElementId> floorids = collector.ToElementIds().ToList();
                allsds.AddRange(CreatSpotDimensionOnFloor(floorids, activeview, doc, ref report));
                transaction1.Commit();
            }

            using (Transaction transaction2 = new Transaction(doc))
            {
                transaction2.Start("creat elevation in link");
                FilteredElementCollector linkcollector = new FilteredElementCollector(doc);
                linkcollector.OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType();
                if (linkcollector != null && linkcollector.Count() != 0)
                {
                    foreach (RevitLinkInstance rli in linkcollector)
                    {
                        if (rli == null) { continue; }
                        FilteredElementCollector floorcollector = new FilteredElementCollector(rli.GetLinkDocument());
                        floorcollector.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType();
                        if (floorcollector == null || floorcollector.Count() == 0) { continue; }
                        List<ElementId> floorids = floorcollector.ToElementIds().ToList();
                        allsds.AddRange(CreatSpotDimensionOnFloorInLink(floorids, activeview, doc, rli, ref report));
                    }
                }
                transaction2.Commit();
            }

            using (Transaction transaction3 = new Transaction(doc))
            {
                transaction3.Start("±");
                foreach (SpotDimension sd in allsds)
                {
                    //set type and ±
                    sd.ChangeTypeId(type.Id);
                    Parameter para = sd.get_Parameter(BuiltInParameter.SPOT_ELEV_SINGLE_OR_UPPER_VALUE);
                    Parameter para2 = sd.get_Parameter(BuiltInParameter.SPOT_ELEV_SINGLE_OR_UPPER_PREFIX);
                    if (para.AsValueString() == "0")
                    {
                        para2.Set("±");
                    }
                    Parameter para3 = sd.get_Parameter(BuiltInParameter.SPOT_ELEV_DISPLAY_ELEVATIONS);
                    para3.Set(3);
                }
                uidoc.RefreshActiveView();
                transaction3.Commit();
            }

            TaskDialog.Show("report", report);
            return Result.Succeeded;
        }

        public List<SpotDimension> CreatSpotDimensionOnFloor(List<ElementId> floorids, View activeview, Document doc, ref string report)
        {
            List<SpotDimension> sds = new List<SpotDimension>();
            int errnum = 0;
            foreach (ElementId id in floorids)
            {
                Floor floor = doc.GetElement(id) as Floor;
                Options option = new Options();
                option.ComputeReferences = true;
                option.View = activeview;
                option.IncludeNonVisibleObjects = false;
                GeometryElement geometry = floor.get_Geometry(option);
                if (geometry == null) { continue; }
                foreach (GeometryObject geomobj in geometry)
                {
                    Solid gemosolid = geomobj as Solid;
                    if (gemosolid != null)
                    {
                        XYZ center = gemosolid.ComputeCentroid();
                        foreach (Face geomface in gemosolid.Faces)
                        {
                            PlanarFace plane = geomface as PlanarFace;
                            if (plane != null)
                            {
                                if (plane.FaceNormal.X == 0 && plane.FaceNormal.Y == 0 && plane.FaceNormal.Z == 1)
                                {
                                    //get reference for creation
                                    Reference refe = geomface.Reference;
                                    //get points for creation
                                    XYZ origin = center;
                                    XYZ bend = origin.Add(new XYZ(0, 1, 0));
                                    XYZ end = origin.Add(new XYZ(1, 1, 0));
                                    XYZ refpt = origin;
                                    try
                                    {
                                        SpotDimension sd = doc.Create.NewSpotElevation(activeview, refe, origin, bend, end, refpt, false);
                                        sds.Add(sd);
                                    }
                                    catch (Autodesk.Revit.Exceptions.InvalidOperationException e)
                                    {
                                        errnum++;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            report += "\n" + "当前项目有" + errnum.ToString() + "个楼板没有生成高程点.";
            return sds;
        }
        public List<SpotDimension> CreatSpotDimensionOnFloorInLink(List<ElementId> floorids, View activeview, Document doc, RevitLinkInstance rli, ref string report)
        {
            List<SpotDimension> sds = new List<SpotDimension>();
            var a = rli.GetTotalTransform();
            int errnum = 0;
            foreach (ElementId id in floorids)
            {
                Floor floor = rli.GetLinkDocument().GetElement(id) as Floor;

                Options option = new Options();
                option.ComputeReferences = true;
                option.View = activeview;
                option.IncludeNonVisibleObjects = false;
                GeometryElement geometry = floor.get_Geometry(option);
                if (geometry == null) { continue; }
                foreach (GeometryObject geomobj in geometry)
                {
                    Solid gemosolid = geomobj as Solid;
                    if (gemosolid != null)
                    {
                        XYZ center = rli.GetTotalTransform().OfPoint(gemosolid.ComputeCentroid());
                        foreach (Face geomface in gemosolid.Faces)
                        {
                            PlanarFace plane = geomface as PlanarFace;
                            if (plane != null)
                            {
                                if (plane.FaceNormal.X == 0 && plane.FaceNormal.Y == 0 && plane.FaceNormal.Z == 1)
                                {
                                    //get reference for creation
                                    Reference refe = geomface.Reference.CreateLinkReference(rli);
                                    //get points for creation
                                    XYZ origin = center;
                                    XYZ bend = origin.Add(new XYZ(0, 1, 0));
                                    XYZ end = origin.Add(new XYZ(1, 1, 0));
                                    XYZ refpt = origin;
                                    try
                                    {
                                        SpotDimension sd = doc.Create.NewSpotElevation(activeview, refe, origin, bend, end, refpt, false);
                                        sds.Add(sd);
                                    }
                                    catch (Autodesk.Revit.Exceptions.InvalidOperationException e)
                                    {
                                        errnum++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            report += "\n" + "在" + rli.Name + "有" + errnum.ToString() + "个楼板没有生成高程点.";
            return sds;
        }


    }
}
