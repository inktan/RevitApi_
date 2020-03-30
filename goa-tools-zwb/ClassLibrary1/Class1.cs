//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.Attributes;


//namespace ClassLibrary1
//{
//    [TransactionAttribute(TransactionMode.Manual)]
//    [RegenerationAttribute(RegenerationOption.Manual)]

//    public class Class1 : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            UIDocument uidoc = commandData.Application.ActiveUIDocument;
//            Document doc = commandData.Application.ActiveUIDocument.Document;

//            View activeview = uidoc.ActiveView;

//            FilteredElementCollector floorcollector = new FilteredElementCollector(doc,activeview.Id);
//            floorcollector.OfCategory(BuiltInCategory.OST_Floors).OfClass(typeof(Floor)).WhereElementIsNotElementType();
//            IEnumerable<ElementId> floorids = from ele in floorcollector
//                                     let floor = ele as Floor
//                                     let type = doc.GetElement(floor.GetTypeId()) as FloorType
//                                    where type.Name.Contains("FNSH")
//                                    select floor.Id;
//            if(floorids.Count() == 0) { TaskDialog.Show("wrong", "未找到FNSH楼板"); }

//            //FilteredElementCollector linestylecollector = new FilteredElementCollector(doc);
//            //linestylecollector.OfClass(typeof(GraphicsStyle));
//            //IEnumerable<GraphicsStyle> tmpgraphicsstyles = null;
//            //tmpgraphicsstyles = from tmp in linestylecollector
//            //                    let gs = tmp as GraphicsStyle
//            //                    where gs.Name == "G-GRID-IDEN"
//            //                    select gs;
//            //if (tmpgraphicsstyles.Count() == 0) { TaskDialog.Show("wrong", "未找到名为G-GRID-IDEN的轴网"); return Result.Failed; }
//            //GraphicsStyle graphicsstyle = tmpgraphicsstyles.ToList().First();

//            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
//            ogs.SetProjectionLinePatternId();

//            using (Transaction transaction = new Transaction(doc))
//            {
//                transaction.Start("将楼板边线改成白色");
//                foreach (ElementId id in floorids)
//                {
//                    activeview.SetElementOverrides(id, ogs);
//                }
//                uidoc.RefreshActiveView();
//                transaction.Commit();
//            }




//            return Result.Succeeded;
//        }
//    }
//}
