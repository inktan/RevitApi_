using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using goa.Common;

namespace COLR_SWIT
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {

        //声明全局静态变量
        public static IList<string> selectedViewNames = new List<string>();

        public static IList<string> plotViewNames = new List<string>();
        public static IList<string> INTFViewNames = new List<string>();
        public static IList<string> workViewNames = new List<string>();
        public static ICollection<ElementId> plotViewIds = new List<ElementId>();
        public static ICollection<ElementId> INTFViewIds = new List<ElementId>();
        public static ICollection<ElementId> workViewIds = new List<ElementId>();

        public static string changStr;//判断条件字段
        public static string actiViewName;

        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            try
            {
                ////check domain
                //if (ADValidationCheck.GetDirectoryEntryForCurrentUser() == null)
                //{
                //    TaskDialog.Show("信息", "需要连接goa网络。");
                //    return Result.Failed;
                //}

                APP.UIApp = commandData.Application;
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;//Revit文档数据读取
                Autodesk.Revit.DB.View activeView = doc.ActiveView;
                ElementId activeViewId = activeView.Id;

                //获取视图列表合辑
                GetViews_All(doc);

                //check opened window
                MainWindow form = APP.MainWindow;
                if (null != form && form.Visible == true)
                {
                    form.Activate();
                    return Result.Succeeded;
                }

                //show new window
                var p = Process.GetCurrentProcess();
                var revitWindow = new WindowHandle(p.MainWindowHandle);
                if (null == form || form.IsDisposed)
                    form = new MainWindow(uidoc);
                APP.MainWindow = form;
                form.Show(revitWindow);
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
            }
            return Result.Succeeded;
        }// excute

        //以下为各种method---------------------------------分割线---------------------------------

        //获取视图列表合辑
        public static void GetViews_All(Document doc)
        {
            actiViewName = doc.ActiveView.Name;
            //所有视图列表需要设置尾部更新
            //过滤视图
            plotViewIds = filteredPLOTview(doc, "PLOT");//设置目标视图变量//过滤指定字段视图元素Id（不包括PLOT视图样板)
            plotViewNames = getIcollecionNames(doc, plotViewIds);//INTF视图name列表（不包括PLOT视图样板)                
                                                                 
            //过滤视图
            INTFViewIds = filteredPLOTview(doc, "INTF");//设置目标视图变量//过滤指定字段视图元素Id（不包括PLOT视图样板)
            INTFViewNames = getIcollecionNames(doc, plotViewIds);//INTF视图name列表（不包括PLOT视图样板)                
                                                                 
            //过滤视图
            workViewIds = filteredPLOTview(doc, "WORK");//设置目标视图变量//过滤指定字段视图元素Id（不包括PLOT视图样板)
            workViewNames = getIcollecionNames(doc, plotViewIds);//INTF视图name列表（不包括PLOT视图样板)
        }
        //获取视图列表名称
        public static IList<string> getIcollecionNames(Document doc, ICollection<ElementId> eleIds)
        {
            IList<string> strs = new List<string>();
            foreach (ElementId vieId in eleIds)
            {
                Autodesk.Revit.DB.View vie = doc.GetElement(vieId) as Autodesk.Revit.DB.View;
                string vieName = vie.Name;
                strs.Add(vieName);
            }
            return strs;
        }
        //过滤指定字段视图（不包括视图样板）
        public static ICollection<ElementId> filteredPLOTview(Document doc, string str)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);//收集器本省不能被遍历
            IList<Element> views = viewCollector.OfClass(typeof(Autodesk.Revit.DB.View)).ToElements();//过滤器一旦被过滤，该过滤器本身只保存过滤后的元素数据

            //寻找包含指定字段的视图
            IEnumerable<Autodesk.Revit.DB.View> eleIds =
                from ele in views
                let viw = ele as Autodesk.Revit.DB.View
                where viw != null
                where !viw.IsTemplate//过滤掉视图样板
                where viw.Name.Contains(str)//指定视图名称包含字段
                let objTyp = doc.GetElement(viw.GetTypeId()) as ElementType
                where objTyp != null
                where !objTyp.Name.Equals("schedule") //过滤掉明细表
                where !objTyp.Name.Equals("drawing sheet")
                select viw;

            ICollection<ElementId> _eleIds = new List<ElementId>();

            foreach (Autodesk.Revit.DB.View view in eleIds)
            {
                if (view.get_Parameter(BuiltInParameter.VIEW_DEPENDENCY).AsString() == "主选项"
                    || view.get_Parameter(BuiltInParameter.VIEW_DEPENDENCY).AsString() == "不相关")
                {
                    _eleIds.Add(view.Id);
                }
            }
            return _eleIds;
        }

    }
}
