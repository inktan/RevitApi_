using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

//批量区位图

namespace SHET_LOCA
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //get viewport
            TaskDialog.Show("GOA", "请选择视口作为样例.");
            Reference viewportref = uidoc.Selection.PickObject(ObjectType.Element, "选择图例.");
            Viewport viewport = doc.GetElement(viewportref.ElementId) as Viewport;
            if (viewport == null) // || viewport.Id.IntegerValue != (int)BuiltInCategory.OST_Viewports)
            { TaskDialog.Show("wrong", "未选中视口"); return Result.Failed; }
            ElementId legendid = viewport.ViewId;

            //get titleBlock & name
            TaskDialog.Show("GOA", "请选择图框作为样例.");
            Reference titleblockref = uidoc.Selection.PickObject(ObjectType.Element, "选择图框.");
            FamilyInstance titleblock = doc.GetElement(titleblockref.ElementId) as FamilyInstance;
            Category titleblockcategory = titleblock.Category;
            if (titleblock == null || titleblockcategory.Id.IntegerValue != (int)BuiltInCategory.OST_TitleBlocks)
            { TaskDialog.Show("wrong", "未选中图框"); return Result.Failed; }
            string titleblockname = titleblock.Symbol.Family.Name;

            //get viewsheet
            ElementId viewsheetid = titleblock.OwnerViewId;

            //get location(by boundingbox)
            XYZ viewportposition = viewport.GetBoxCenter();
            XYZ titleblockposition = titleblock.get_BoundingBox(uidoc.ActiveView).Max;
            double distanceX = titleblockposition.X - viewportposition.X;
            double distanceY = titleblockposition.Y - viewportposition.Y;

            //get all titleBlocks
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsNotElementType();
            if(collector == null) { TaskDialog.Show("wrond", "未找到图纸"); }

            //prompt
            string prompt = "水平偏移:" + (distanceX*304.8).ToString() + "\n垂直偏移:" + (distanceY*304.8).ToString() + "\n同类型图框有:\n";
            List<FamilyInstance> titleblocks = new List<FamilyInstance>();
            foreach(Element ele in collector)
            {
                FamilyInstance tb = ele as FamilyInstance;
                if (tb == null) { TaskDialog.Show("wrond", "未找到图框"); continue; }
                ViewSheet vs = doc.GetElement(tb.OwnerViewId) as ViewSheet;
                if (vs == null) { TaskDialog.Show("wrond", "未找到图纸"); continue; }
                if (tb.Symbol.Family.Name == titleblockname)
                {
                    titleblocks.Add(tb); prompt += vs.Name; prompt += "\n";
                }
            }
            TaskDialog.Show("GOA", prompt);

            //dele all viewport
            FilteredElementCollector viewports = new FilteredElementCollector(doc);
            viewports.OfCategory(BuiltInCategory.OST_Viewports).WhereElementIsNotElementType();
            List<ElementId> deleids = new List<ElementId>();
            foreach(Element ele in viewports)
            {
                Viewport vp = ele as Viewport;
                if(vp.ViewId == legendid) { deleids.Add(vp.Id); }
            }

            //get type
            FilteredElementCollector typecol = new FilteredElementCollector(doc);
            typecol.OfClass(typeof(ElementType));
            ElementType type = null;
            foreach(Element ele in typecol)
            {
                ElementType tmp = ele as ElementType;
                if(tmp == null) { continue; }
                if(tmp.Name == "A-VIEW-NONE") { type = tmp; break; }
            }
            if(type == null) { TaskDialog.Show("wrong", "未找到<A-VIEW-NONE>类型,请载入后重试."); }

            //create viewport and move 
            using (Transaction transaction = new Transaction(doc)) 
            {
                transaction.Start("creat legends");
                doc.Delete(deleids);
                foreach(FamilyInstance tb in titleblocks)
                {
                    //if(tb.OwnerViewId == viewsheetid) {continue; }
                    XYZ postiontmp = tb.get_BoundingBox(doc.GetElement(tb.OwnerViewId) as ViewSheet).Max;
                    XYZ postion = new XYZ(postiontmp.X - distanceX, postiontmp.Y - distanceY, postiontmp.Z);
                    if(Viewport.CanAddViewToSheet(doc, tb.OwnerViewId, legendid))
                    {
                        Viewport newvp = Viewport.Create(doc, tb.OwnerViewId, legendid, postion);
                        newvp.ChangeTypeId(type.Id);
                    }
                }

                transaction.Commit();
            }
            TaskDialog.Show("GOA", "生成成功.如果图框内已有一样的区位图会被删除并生成在新的位置.");


            return Result.Succeeded;
        }
    }
}
