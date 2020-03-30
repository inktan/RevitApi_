using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

using goa.Common;

namespace VIEW_Duplicate
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName()
        {
            return "Overlapping Elements Clean Up Request Handler";
        }
        public void Execute(UIApplication uiapp)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.Duplicate:
                        {
                            viewduplicate_duplicate(uiapp, ViewDuplicateOption.Duplicate);
                            break;
                        }
                    case RequestId.AsDependent:
                        {
                            viewduplicate_duplicate(uiapp, ViewDuplicateOption.AsDependent);
                            break;
                        }
                    case RequestId.WithDetailing:
                        {
                            viewduplicate_duplicate(uiapp, ViewDuplicateOption.WithDetailing);
                            break;
                        }
                    case RequestId.UpdateVIews_List:
                        {
                            UpdateVIews_List(uiapp);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //外部事件方法建立
        //更细视图列表视图
        public void UpdateVIews_List(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题

            //所有视图列表需要设置尾部更新-窗口列表更新
            CMD.GetViews_All(doc);//获取视图列表合辑
        }
        //视图复制
        public void viewduplicate_duplicate(UIApplication uiapp, ViewDuplicateOption Duplicate)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Autodesk.Revit.DB.View activeView = doc.ActiveView;

            //让用户选择哪几个PLOT视图需要复制生成
            if (CMD.selectedViewNames.Count() <= 0)//判断是否选择不存在INTF视图的PLOT视图
            {
                TaskDialog.Show("Revit2020", "未选择任何视图");
            }
            else
            {
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("view_duplicate");
                    if (CMD.changStr == "_actiView")
                    {
                        Autodesk.Revit.DB.View destinationView = CreateDependentCopy(doc, activeView.Id, Duplicate);//复制PLOT视图为指定字段视图
                    }
                    else if (CMD.changStr == "_work")
                    {
                        ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.workViewIds);
                        foreach (ElementId eleId in selPlotViewIds)
                        {
                            Autodesk.Revit.DB.View destinationView = CreateDependentCopy(doc, eleId, Duplicate);//复制PLOT视图为指定字段视图
                        }
                    }
                    else if (CMD.changStr == "_plot")
                    {
                        ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.plotViewIds);
                        foreach (ElementId eleId in selPlotViewIds)
                        {
                            Autodesk.Revit.DB.View destinationView = CreateDependentCopy(doc, eleId, Duplicate);//复制PLOT视图为指定字段视图
                        }
                    }
                    else if (CMD.changStr == "_INTF")
                    {
                        ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.INTFViewIds);
                        foreach (ElementId eleId in selPlotViewIds)
                        {
                            Autodesk.Revit.DB.View destinationView = CreateDependentCopy(doc, eleId, Duplicate);//复制PLOT视图为指定字段视图
                        }
                    }
                    trans.Commit();
                }
                showSuccess(" !");

                CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题
                CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题

                //所有视图列表需要设置尾部更新-窗口列表更新
                CMD.GetViews_All(doc);//获取视图列表合辑
            }
        }//main

        //以下为各种method----------------------------------------
        //复制视图
        public Autodesk.Revit.DB.View CreateDependentCopy(Document doc, ElementId eleId, ViewDuplicateOption Duplicate)
        {
            Autodesk.Revit.DB.View vie = doc.GetElement(eleId) as Autodesk.Revit.DB.View;
            Autodesk.Revit.DB.View dependView = null;
            ElementId newViewId = null;
            if (vie.CanViewBeDuplicated(Duplicate))
            {
                newViewId = vie.Duplicate(Duplicate);
                dependView = doc.GetElement(newViewId) as Autodesk.Revit.DB.View;
                return dependView;
            }
            else
            {
                TaskDialog.Show("Wrong",vie.Name+"视图不可以被复制");
                return dependView;
            }
        }
        //成功界面显示
        public void showSuccess(string str)
        {
            TaskDialog mainDialog = new TaskDialog("Revit2020");
            mainDialog.MainInstruction = "Revit2020";
            mainDialog.MainContent = "You successfully copied all the views" + str;
            mainDialog.Show();
        }
        //将所选视图字符串列表转化为视图元素Id
        public ICollection<ElementId> getSelPlotViewIds(Document doc, IList<string> selectedPlotView, ICollection<ElementId> PlotViewIds)
        {
            ICollection<ElementId> selPlotViewIds = new List<ElementId>();
            foreach (string selectedstr in selectedPlotView)
            {
                foreach (ElementId eleId in PlotViewIds)
                {
                    Autodesk.Revit.DB.View vie = doc.GetElement(eleId) as Autodesk.Revit.DB.View;
                    string vieName = vie.Name;
                    if (selectedstr == vieName)
                    {
                        selPlotViewIds.Add(eleId);
                    }
                }
            }
            return selPlotViewIds;
        }
    }  // public class RequestHandler : IExternalEventHandler
} // namespace

