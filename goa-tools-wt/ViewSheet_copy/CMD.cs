using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ViewSheet_copy
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            //下面代码可直接拷贝到外部事件excute的uiapp内，注释掉return；
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            View activeView = doc.ActiveView;
            ElementId activeViewId = doc.ActiveView.Id;
            string activeViewName = activeView.Name;

            FilteredElementCollector familySymbolSet = (new FilteredElementCollector(doc)).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
            if (familySymbolSet.Count() < 0)
            {
                TaskDialog.Show("Revit2020", "当前文档不存在标题栏");
            }
            FamilySymbol familySymbol = null;
            foreach (FamilySymbol fs in familySymbolSet)
            {
                string fsNmae = fs.Name;
                if (fsNmae == "A1 公制")
                {
                    familySymbol = fs;
                }
            }

            //寻找已经创建的视图
            FilteredElementCollector viewSheetSet = (new FilteredElementCollector(doc)).OfClass(typeof(ViewSheet)).WhereElementIsNotElementType();
            if (viewSheetSet.Count() < 0)
            {
                TaskDialog.Show("Revit2020", "当前文档不存在图纸");
            }
            ViewSheet viewSheet = null;
            foreach (ViewSheet vs in viewSheetSet)
            {
                string vsNmae_NUMBER = vs.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                string vsNmae_NAME = vs.get_Parameter(BuiltInParameter.SHEET_NAME).AsString();
                if (vsNmae_NUMBER == "A101" && vsNmae_NAME == "未命名")
                {
                    viewSheet = vs;
                }
            }
            ICollection<ElementId> viewportIds = viewSheet.GetAllViewports();
                   
            XYZ maximumpoint = null;
            XYZ minimumpoint = null;
            XYZ viewportXYz =  new XYZ(0,0,0) ;
            if (viewportIds.Count() <= 0)
            {
                TaskDialog.Show("Revit2020", viewSheet.Name + "不存在视口");
            }
            else
            {
                foreach (ElementId viewportId in viewportIds)
                {
                    Viewport viewport = doc.GetElement(viewportId) as Viewport;

                    //Outline outline = viewport.GetBoxOutline();
                    Outline outline = viewport.GetLabelOutline();
                    //获取源图纸上指定视口的位置
                    maximumpoint = outline.MaximumPoint;
                    minimumpoint = outline.MinimumPoint;

                    viewportXYz = viewport.GetBoxCenter();

                    viewportXYz = (maximumpoint + minimumpoint) /2;

                }
            }
            //创建目标图纸，添加视图
            ViewSheet _viewSheet = CreatViewSheetPort(doc, familySymbol, activeViewId, activeViewName, viewportXYz);
            //修改目标视图上的目标视口
            //ICollection<ElementId> _viewportIds = _viewSheet.GetAllViewports();

            //using (Transaction setViewportOutline = new Transaction(doc))
            //{
            //    if (_viewportIds.Count() <= 0)
            //    {
            //        TaskDialog.Show("Revit2020", _viewSheet.Name + "不存在视口");
            //    }
            //    else
            //    {
            //        TaskDialog.Show("Revit2020", "123");

            //        foreach (ElementId viewportId in _viewportIds)
            //        {
            //            Viewport viewport = doc.GetElement(viewportId) as Viewport;

            //            Outline outline = viewport.GetBoxOutline();
            //            TaskDialog.Show("Revit2020", "453");

            //            //获取源图纸上指定视口的位置
            //            outline.MaximumPoint = maximumpoint;
            //            outline.MinimumPoint = minimumpoint;
            //        }
            //    }
            //}
            return Result.Succeeded;
        }

        //以下为各种method---------------------------------分割线---------------------------------

        //外部事件方法建立
        //创建图纸，添加视图
        public ViewSheet CreatViewSheetPort(Document doc, FamilySymbol familySymbol, ElementId activeViewId, string activeViewName,XYZ xYZ)//参数说明：doc, 标题栏族， 需要创建视口的视图id，所选视图id的名字
        {
            using (Transaction ViewSheetViewPort_new = new Transaction(doc))
            {
                ViewSheetViewPort_new.Start("ViewSheetViewPort_new");
                ViewSheet viewSheet = ViewSheet.Create(doc, familySymbol.Id);
                if (viewSheet == null)
                {
                    TaskDialog.Show("Revit2020", "未能成功创建新图纸.");
                    return null; 
                }
                //设置视口位置
                //UV location = new UV((viewSheet.Outline.Max.U - viewSheet.Outline.Min.U) / 2, (viewSheet.Outline.Max.V - viewSheet.Outline.Min.V) / 2);

                if (Viewport.CanAddViewToSheet(doc, viewSheet.Id, activeViewId))//判断视图是否能够加入图纸
                {
                    Viewport.Create(doc, viewSheet.Id, activeViewId, xYZ);
                }
                else
                {
                    TaskDialog.Show("Revit2020", activeViewName + "已经存在与图纸中");
                }
                ViewSheetViewPort_new.Commit();
                return viewSheet;
            }
        }
    }
}
