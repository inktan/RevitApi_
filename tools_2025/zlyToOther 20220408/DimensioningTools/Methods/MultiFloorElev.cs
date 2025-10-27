using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using goa.Common.Exceptions;
using goa.Common;

using PubFuncWt;

namespace DimensioningTools
{
    internal static partial class Methods
    {
        internal static void DimOnFloors()
        {
            UIDocument activeUIDocument = goa.Common.APP.UIApp.ActiveUIDocument;
            Document document = activeUIDocument.Document;
            View activeView = activeUIDocument.ActiveView;
            if (activeView is ViewPlan)
            {
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
                filteredElementCollector.OfClass(typeof(SpotDimensionType));
                SpotDimensionType spotDimensionType = null;
                foreach (Element element in filteredElementCollector)
                {
                    SpotDimensionType spotDimensionType1 = element as SpotDimensionType;
                    if (spotDimensionType1 != null)
                    {
                        if (spotDimensionType1.Name == "A-LEVL-SLAB-2.5")
                        {
                            spotDimensionType = spotDimensionType1;
                            break;
                        }
                    }
                }
                if (spotDimensionType != null)
                {
                    List<SpotDimension> spotDimensions = new List<SpotDimension>();
                    string error = "";
                    using (TransactionGroup tg = new TransactionGroup(document, "创建标高"))
                    {
                        tg.Start();
                        using (Transaction transaction = new Transaction(document))
                        {
                            transaction.Start("creat elevation");
                            FilteredElementCollector filteredElementCollector1 = new FilteredElementCollector(document, activeView.Id);
                            filteredElementCollector1.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType();
                            List<ElementId> list = filteredElementCollector1.ToElementIds().ToList<ElementId>();
                            spotDimensions.AddRange(CreatSpotDimensionOnFloor(list, activeView, document, ref error));
                            transaction.Commit();
                        }
                        using (Transaction transaction1 = new Transaction(document))
                        {
                            transaction1.Start("creat elevation in link");
                            FilteredElementCollector filteredElementCollector2 = new FilteredElementCollector(document);
                            filteredElementCollector2.OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType();
                            if ((filteredElementCollector2 == null ? false : filteredElementCollector2.Count<Element>() != 0))
                            {
                                foreach (RevitLinkInstance revitLinkInstance in filteredElementCollector2)
                                {
                                    if (revitLinkInstance != null)
                                    {

                                        FilteredElementCollector filteredElementCollector3 = new FilteredElementCollector(revitLinkInstance.GetLinkDocument());
                                        filteredElementCollector3.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType();
                                        if ((filteredElementCollector3 == null ? false : filteredElementCollector3.Count<Element>() != 0))
                                        {
                                            List<ElementId> elementIds = filteredElementCollector3.ToElementIds().ToList<ElementId>();
                                            spotDimensions.AddRange(CreatSpotDimensionOnFloorInLink(elementIds, activeView, document, revitLinkInstance, ref error));
                                        }
                                    }
                                }
                            }
                            transaction1.Commit();
                        }
                        using (Transaction transaction2 = new Transaction(document))
                        {
                            transaction2.Start("±");
                            foreach (SpotDimension spotDimension in spotDimensions)
                            {
                                spotDimension.ChangeTypeId(spotDimensionType.Id);
                                Parameter parameter = spotDimension.get_Parameter(BuiltInParameter.SPOT_ELEV_SINGLE_OR_UPPER_VALUE);
                                Parameter parameter1 = spotDimension.get_Parameter(BuiltInParameter.SPOT_ELEV_SINGLE_OR_UPPER_PREFIX);
                                if (parameter.AsValueString() == "0")
                                {
                                    parameter1.Set("±");
                                }
                                spotDimension.get_Parameter(BuiltInParameter.SPOT_ELEV_DISPLAY_ELEVATIONS).Set(3);
                            }
                            activeUIDocument.RefreshActiveView();
                            transaction2.Commit();
                        }
                        tg.Assimilate();
                    }

                    if (error != "")
                    {
                        error = "以下楼板未能创建高程点：\r\n" + error;
                        FlexibleMessageBox.Show(error);
                    }
                }
                else
                {
                    throw new CommonUserExceptions("未找到<A-LEVL-SLAB-2.5>类型.");
                }
            }
            else
            {
                throw new CommonUserExceptions("仅支持在平面内运行");
            }
        }

        private static List<SpotDimension> CreatSpotDimensionOnFloor(List<ElementId> floorids, View activeview, Document doc, ref string error)
        {
            List<SpotDimension> spotDimensions = new List<SpotDimension>();
            foreach (ElementId floorid in floorids)
            {
                Floor element = doc.GetElement(floorid) as Floor;
                Options option = new Options()
                {
                    ComputeReferences = true,
                    View = activeview,
                    IncludeNonVisibleObjects = false
                };
                GeometryElement geometry = element.get_Geometry(option);
                if (geometry != null)
                {
                    foreach (GeometryObject geometryObject in geometry)
                    {
                        Solid solid = geometryObject as Solid;
                        if (solid != null)
                        {
                            XYZ xYZ = solid.ComputeCentroid();
                            Line line = Line.CreateBound(xYZ, xYZ + new XYZ(1000, 1000, 0));
                            doc.CreateDirectShape(new List<Line>() { line });

                            foreach (Face face in solid.Faces)
                            {
                                PlanarFace planarFace = face as PlanarFace;
                                if (planarFace != null)
                                {
                                    if ((planarFace.FaceNormal.X != 0 || planarFace.FaceNormal.Y != 0 ? false : planarFace.FaceNormal.Z == 1))
                                    {
                                        Reference reference = face.Reference;
                                        XYZ xYZ1 = xYZ;
                                        XYZ xYZ2 = xYZ1.Add(new XYZ(0, 1, 0));
                                        XYZ xYZ3 = xYZ1.Add(new XYZ(1, 1, 0));
                                        XYZ xYZ4 = xYZ1;
                                        //try
                                        //{
                                        //    SpotDimension spotDimension = doc.Create.NewSpotElevation(activeview, reference, xYZ1, xYZ2, xYZ3, xYZ4, false);
                                        //    spotDimensions.Add(spotDimension);
                                        //}
                                        //catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
                                        //{
                                        //    error += floorid.ToString() + "\r\n";
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return spotDimensions;
        }

        private static List<SpotDimension> CreatSpotDimensionOnFloorInLink(List<ElementId> floorids, View activeview, Document doc, RevitLinkInstance rli, ref string error)
        {
            List<SpotDimension> spotDimensions = new List<SpotDimension>();
            rli.GetTotalTransform();
            foreach (ElementId floorid in floorids)
            {
                Floor element = rli.GetLinkDocument().GetElement(floorid) as Floor;
                Options option = new Options()
                {
                    ComputeReferences = true,
                    View = activeview,
                    IncludeNonVisibleObjects = false
                };
                GeometryElement geometry = element.get_Geometry(option);
                if (geometry != null)
                {
                    foreach (GeometryObject geometryObject in geometry)
                    {
                        Solid solid = geometryObject as Solid;
                        if (solid != null)
                        {
                            XYZ xYZ = rli.GetTotalTransform().OfPoint(solid.ComputeCentroid());

                            Line line = Line.CreateBound(xYZ, xYZ + new XYZ(1000, 1000, 0));
                            doc.CreateDirectShape(new List<Line>() { line });

                            foreach (Face face in solid.Faces)
                            {
                                PlanarFace planarFace = face as PlanarFace;
                                if (planarFace != null)
                                {
                                    if ((planarFace.FaceNormal.X != 0 || planarFace.FaceNormal.Y != 0 ? false : planarFace.FaceNormal.Z == 1))
                                    {
                                        Reference reference = face.Reference.CreateLinkReference(rli);
                                        XYZ xYZ1 = xYZ;
                                        XYZ xYZ2 = xYZ1.Add(new XYZ(0, 1, 0));
                                        XYZ xYZ3 = xYZ1.Add(new XYZ(1, 1, 0));
                                        XYZ xYZ4 = xYZ1;
                                        //try
                                        //{
                                        //    SpotDimension spotDimension = doc.Create.NewSpotElevation(activeview, reference, xYZ1, xYZ2, xYZ3, xYZ4, false);
                                        //    spotDimensions.Add(spotDimension);
                                        //}
                                        //catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
                                        //{
                                        //    error += floorid.ToString() + "\r\n";
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return spotDimensions;
        }


    }
}