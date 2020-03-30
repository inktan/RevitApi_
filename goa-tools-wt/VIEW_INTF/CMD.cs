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

namespace VIEW_INTF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {

        //声明全局静态变量
        public static IList<string> originViewNames_INTF = new List<string>();//指定字段视图列表---提资图
        public static IList<string> targetViewNames_INTF = new List<string>();//指定字段视图列表---提资图
        public static IList<string> getNotarget_ToNames_INTF = new List<string>();//指定字段视图列表---提资图
        public static IList<string> originViewNames_DD = new List<string>();//指定字段视图列表---方案图
        public static IList<string> targetViewNames_DD = new List<string>();//指定字段视图列表---方案图
        public static IList<string> getNotarget_ToNames_DD = new List<string>();//指定字段视图列表---方案图
        public static IList<string> originViewNames_BP = new List<string>();//指定字段视图列表---蓝图
        public static IList<string> targetViewNames_BP = new List<string>();//指定字段视图列表---蓝图
        public static IList<string> getNotarget_ToNames_BP = new List<string>();//指定字段视图列表---蓝图
        public static ICollection<ElementId> originViewIds_INTF = new List<ElementId>();//指定字段视图列表ids---提资图
        public static ICollection<ElementId> targetViewIds_INTF = new List<ElementId>();//指定字段视图列表---提资图
        public static ICollection<ElementId> getNotargetIds_INTF = new List<ElementId>();//指定字段视图列表---提资图
        public static ICollection<ElementId> originViewIds_DD = new List<ElementId>();//指定字段视图列表---方案图
        public static ICollection<ElementId> targetViewIds_DD = new List<ElementId>();//指定字段视图列表---方案图
        public static ICollection<ElementId> getNotargetIds_DD = new List<ElementId>();//指定字段视图列表---方案图
        public static ICollection<ElementId> originViewIds_BP = new List<ElementId>();//指定字段视图列表---蓝图
        public static ICollection<ElementId> targetViewIds_BP = new List<ElementId>();//指定字段视图列表---蓝图
        public static ICollection<ElementId> getNotargetIds_BP = new List<ElementId>();//指定字段视图列表---蓝图
        public static int noINTFcount = 0;
        public static int noDDcount = 0;
        public static int noBPcount = 0;

        public static string changStr;//主选项功能对应控制值
        public static IList<string> selectedViewNames = new List<string>();//静态变量用户从窗口界面选择的视图列表
        public static string actiViewName;//当前视图名称，是否显示，取决于是否符合试图列表要求

        public delegate ICollection<ElementId> myDelegate(Document doc, ElementId eleId);

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
                actiViewName = activeView.Name;
                ElementId activeViewId = activeView.Id;

                //所有视图列表需要设置尾部更新
                GetViews_All(doc, "PLOT-CD", "INTF-CD", "PLOT-DD", "PLOT-BP");//获取视图列表合辑

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
        public static void GetViews_All(Document doc, string strOrigin, string strINTF, string strDD, string strBP)
        {
            //所有视图列表需要设置尾部更新

            //过滤视图-提资
            originViewIds_INTF = filteredPLOTview(doc, strOrigin);//过滤PLOT视图元素Id（不包括PLOT视图样板)
            originViewNames_INTF = getIcollecionNames(doc, originViewIds_INTF);//PLOT视图name列表（不包括PLOT视图样板)_winform listbox 输出
            targetViewIds_INTF = filteredPLOTview(doc, strINTF);//设置目标视图变量//过滤指定字段视图元素Id（不包括PLOT视图样板)
            targetViewNames_INTF = getIcollecionNames(doc, targetViewIds_INTF);//INTF视图name列表（不包括PLOT视图样板)

            getNotarget_ToNames_INTF = GetNoTarget_OriginNames(originViewNames_INTF, targetViewNames_INTF, strOrigin, strINTF);//设置目标视图变量//获取PLOT视图无对应INTF视图的元素
             //过滤视图-方案图
             originViewIds_DD = filteredBPview(doc, strOrigin);//过滤PLOT视图元素Id（不包括PLOT视图样板)
            originViewNames_DD = getIcollecionNames(doc, originViewIds_DD);//PLOT视图name列表（不包括PLOT视图样板)_winform listbox 输出
            targetViewIds_DD = filteredBPview(doc, strDD);//设置目标视图变量//过滤指定字段视图元素Id（不包括PLOT视图样板)
            targetViewNames_DD = getIcollecionNames(doc, targetViewIds_DD);//INTF视图name列表（不包括PLOT视图样板)

            getNotarget_ToNames_DD = GetNoTarget_OriginNames(originViewNames_DD, targetViewNames_DD, strOrigin, strDD);//设置目标视图变量//获取PLOT视图无对应INTF视图的元素
                                                                                                                                            
            //过滤视图-方案图
            originViewIds_BP = filteredBPview(doc, strOrigin);//过滤PLOT视图元素Id（不包括PLOT视图样板)
            originViewNames_BP = getIcollecionNames(doc, originViewIds_BP);//PLOT视图name列表（不包括PLOT视图样板)_winform listbox 输出
            targetViewIds_BP = filteredBPview(doc, strBP);//设置目标视图变量//过滤指定字段视图元素Id（不包括PLOT视图样板)
            targetViewNames_BP = getIcollecionNames(doc, targetViewIds_BP);//INTF视图name列表（不包括PLOT视图样板)

            getNotarget_ToNames_BP = GetNoTarget_OriginNames(originViewNames_BP, targetViewNames_BP, strOrigin, strBP);//设置目标视图变量//获取PLOT视图无对应INTF视图的元素
        }


        //查找前者不包含后者的元素列表
        public static IList<string> GetNoTarget_OriginNames(IList<string> PLOTViewNamesTem, IList<string> INTFViewNamesTem, string str_origin, string str_target)
        {
            IList<string> getNofield_PLOTNames = new List<string>();
            //判断是否存在INTF视图的PLOT视图
            if (INTFViewNamesTem.Count > 0)
            {
                foreach (string str_temp in PLOTViewNamesTem)
                {
                    string str_repl = str_temp.Replace(str_origin, str_target);
                    if (!INTFViewNamesTem.Contains(str_repl))
                    {
                        getNofield_PLOTNames.Add(str_temp);//收集不存在INTF视图的PLOT视图
                    }
                }
                return getNofield_PLOTNames;
            }
            else
            {
                return PLOTViewNamesTem;
            }

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
                where viw.Name.Contains("PLAN")//指定视图名称包含字段
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
        //过滤指定字段视图（不包括视图样板）
        public static ICollection<ElementId> filteredBPview(Document doc, string str)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);//收集器本省不能被遍历
            IList<Element> views = viewCollector.OfClass(typeof(Autodesk.Revit.DB.View)).ToElements();//过滤器一旦被过滤，该过滤器本身只保存过滤后的元素数据

            //寻找包含指定字段的视图
            IEnumerable<Autodesk.Revit.DB.View> views_temp =
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

            foreach (Autodesk.Revit.DB.View view in views_temp)
            {
                if (view.get_Parameter(BuiltInParameter.VIEW_DEPENDENCY).AsString() == "主选项"
                    || view.get_Parameter(BuiltInParameter.VIEW_DEPENDENCY).AsString() == "不相关")
                {
                    _eleIds.Add(view.Id);
                }
            }
            return _eleIds;
        }

    }//class
}//namespace
