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

namespace COLR_SWIT
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
                    case RequestId.ChangeRgbToBlack:
                        {
                            ChangeRgbToBlack(uiapp);
                            break;
                        }
                    case RequestId.ChangeBlackToRgb:
                        {
                            ChangeBlackToRgb(uiapp);
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
        public void ChangeRgbToBlack(UIApplication uiapp)
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
                if (CMD.changStr == "_actiView")
                {
                        ChangeRgbToBlack(doc, activeView.Id);
                }
                else if (CMD.changStr == "_work")
                {
                    ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.workViewIds);
                    foreach (ElementId eleId in selPlotViewIds)
                    {
                        ChangeRgbToBlack(doc, eleId);
                    }
                }

                else if (CMD.changStr == "_plot")
                {
                    ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.plotViewIds);
                    foreach (ElementId eleId in selPlotViewIds)
                    {
                        ChangeRgbToBlack(doc, eleId);
                    }
                }
                else if (CMD.changStr == "_INTF")
                {
                    ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.INTFViewIds);
                    foreach (ElementId eleId in selPlotViewIds)
                    {
                        ChangeRgbToBlack(doc, eleId);
                    }
                }
                showSuccess("Rgb To Black");

                CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题
                //所有视图列表需要设置尾部更新-窗口列表更新
                CMD.GetViews_All(doc);//获取视图列表合辑
            }
        } //main method
        public void ChangeBlackToRgb(UIApplication uiapp)
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
                if (CMD.changStr == "_actiView")
                {
                        ChangeBlackToRgb(doc, activeView.Id);
                }
                else if (CMD.changStr == "_work")
                {
                    ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.workViewIds);
                    foreach (ElementId eleId in selPlotViewIds)
                    {
                        ChangeBlackToRgb(doc, eleId);
                    }
                }
                else if (CMD.changStr == "_plot")
                {
                    ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.plotViewIds);
                    foreach (ElementId eleId in selPlotViewIds)
                    {
                        ChangeBlackToRgb(doc, eleId);
                    }
                }
                else if (CMD.changStr == "_INTF")
                {
                    ICollection<ElementId> selPlotViewIds = getSelPlotViewIds(doc, CMD.selectedViewNames, CMD.INTFViewIds);
                    foreach (ElementId eleId in selPlotViewIds)
                    {
                        ChangeBlackToRgb(doc, eleId);
                    }
                }
                showSuccess("Black To Rgb");

                CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题
                //所有视图列表需要设置尾部更新-窗口列表更新
                CMD.GetViews_All(doc);//获取视图列表合辑
            }
        } //main method

        //以下为各种method----------------------------------------
        //成功界面显示
        public void showSuccess(string str)
        {
            TaskDialog mainDialog = new TaskDialog("Revit2020");
            mainDialog.MainInstruction = "Revit2020";
            mainDialog.MainContent = "You successfully changed all the elements from " + str;
            mainDialog.Show();
        }
        //全局元素改为黑色
        public void ChangeRgbToBlack(Document doc, ElementId eleId)
        {
            ICollection<ElementId> ele2DIds01 = getEleIdsInView(doc, eleId);//获取视图中所有可见元素
            //剔除填充区域
            IEnumerable<ElementId> eleIds01 = from ele in ele2DIds01
                                              where doc.GetElement(ele).GetType().Name != "FilledRegion"
                                              select ele;
            //设置过滤器
            ICollection<ElementId> ele2DIds02 = getEleIdsInView(doc, eleId);//获取视图中所有可见元素
            //选出填充区域
            IEnumerable<ElementId> eleIds02 = from ele in ele2DIds02
                                              where doc.GetElement(ele).GetType().Name == "FilledRegion"
                                              select ele;

            OverrideGraphicSettings ogs01 = new OverrideGraphicSettings();//设置投影线、截面线颜色
            ogs01.SetCutLineColor(new Color(0, 0, 0));
            ogs01.SetProjectionLineColor(new Color(0, 0, 0));
            OverrideGraphicSettings ogs02 = new OverrideGraphicSettings();//设置填充颜色
            ogs02.SetProjectionLineColor(new Color(60, 60, 60));

            FilteredElementCollector elements = new FilteredElementCollector(doc);

            using (Transaction changeClorRGB = new Transaction(doc, "changeClorRGB"))
            {
                changeClorRGB.Start("changeClorRGB");
                ViewElementOverride(doc, eleId, eleIds01.ToList(), ogs01);//非填充图元替换颜色为黑色
                ViewElementOverride(doc, eleId, eleIds02.ToList(), ogs02);//填充图元替换颜色为灰色
                changeClorRGB.Commit();
            }
        }
        //全局元素改为Category
        public void ChangeBlackToRgb(Document doc, ElementId eleId)
        {
            ICollection<ElementId> ele2DIds = getEleIdsInView(doc, eleId);//获取视图中所有可见元素
            OverrideGraphicSettings ogs01 = new OverrideGraphicSettings();//设置投影线、截面线颜色
            using (Transaction changeClorRGB = new Transaction(doc, "changeClorRGB"))
            {
                changeClorRGB.Start("changeClorRGB");
                ViewElementOverride(doc, eleId, ele2DIds.ToList(), ogs01);//替换颜色为按类别
                changeClorRGB.Commit();
            }
        }
        //通过视图Id过滤，获取需要从目标视图图元替换的可见图元
        public ICollection<ElementId> getEleIdsInView(Document doc, ElementId eleId)
        {
            FilteredElementCollector elementsCollector = new FilteredElementCollector(doc, eleId);

            IEnumerable<ElementId> vieEleIds = from elementId in elementsCollector.ToElementIds()
                                               where doc.GetElement(elementId).GetType().Name != "ReferencePlane"//对收集器进行首次过滤
                                               select elementId;
            return vieEleIds.ToList();
        }
        //改变图元 投影线/截面线 的颜色_当前视图
        public void ViewElementOverride(Document doc, ElementId eleId, ICollection<ElementId> eleIds, OverrideGraphicSettings ogs)
        {
            Autodesk.Revit.DB.View vie = doc.GetElement(eleId) as Autodesk.Revit.DB.View;
            foreach (ElementId tempEId in eleIds)
            {
                vie.SetElementOverrides(tempEId, ogs);
            }
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
        //过滤PLOT视图（不包括视图样板）
        public ICollection<ElementId> filteredPLOTview(Document doc, string str)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);//收集器本省不能被遍历
            IList<Element> views = viewCollector.OfClass(typeof(Autodesk.Revit.DB.View)).ToElements();//过滤器一旦被过滤，该过滤器本身只保存过滤后的元素数据
            //寻找包含指定字段的视图
            IEnumerable<ElementId> eleIds =
                from ele in views
                let viw = ele as Autodesk.Revit.DB.View
                where viw != null
                where !viw.IsTemplate//过滤掉视图样板
                where viw.Name.Contains(str)//指定视图名称包含字段
                let objTyp = doc.GetElement(viw.GetTypeId()) as ElementType
                where objTyp != null
                where !objTyp.Name.Equals("schedule") //过滤掉明细表
                where !objTyp.Name.Equals("drawing sheet")
                let viwId = viw.Id
                select viwId;
            ICollection<ElementId> _eleIds = eleIds.ToList();
            return _eleIds;
        }
        //过滤PLOT视图样板
        public ICollection<ElementId> filteredPLOTviewTemplate(Document doc)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);//收集器本省不能被遍历
            IList<Element> views = viewCollector.OfClass(typeof(Autodesk.Revit.DB.View)).ToElements();//过滤器一旦被过滤，该过滤器本身只保存过滤后的元素数据
            //寻找包含指定字段的视图样板
            IEnumerable<ElementId> eleIds =
                from ele in views
                let viw = ele as Autodesk.Revit.DB.View
                where viw != null
                where viw.IsTemplate//过滤掉视图样板
                where viw.Name.Contains("PLOT")//指定视图名称包含字段
                let viwId = viw.Id
                select viwId;
            ICollection<ElementId> _eleIds = eleIds.ToList();
            return _eleIds;
        }
        //打印列表中所有元素的名字
        public void elementNameListOut(Document doc, ICollection<ElementId> eleIds, string str)
        {
            string Names = null;
            foreach (ElementId eId in eleIds)
            {
                Element ele = doc.GetElement(eId);
                Names += ele.Name + "\n";
            }
            Names += str + eleIds.Count().ToString();
            string activiewName = doc.ActiveView.Name;
            Names += "\n\n当前活动视图为" + activiewName;
            TaskDialog.Show("Revit2020", Names);
        }
        //获取视图列表名称Ilist<string>
        public IList<string> getIcollecionNames(Document doc, ICollection<ElementId> eleIds)
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
    }  // public class RequestHandler : IExternalEventHandler
} // namespace

