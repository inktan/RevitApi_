using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Diagnostics;

//提资

namespace VIEW_INTF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            ViewPlan activeview = uiDoc.ActiveView as ViewPlan;
            if (activeview == null) { TaskDialog.Show("wrong", "请在平面视图内运行."); return Result.Failed; }
            Parameter para = activeview.LookupParameter("VIEW-Content");
            if (para == null) { TaskDialog.Show("wrong", "未找到VIEW-Content参数"); return Result.Failed; }

            //找到提资视图样板
            ViewPlan template = null;
            FilteredElementCollector collector_template = new FilteredElementCollector(doc);
            collector_template.OfClass(typeof(ViewPlan));
            IEnumerable<ViewPlan> templates =
                                                from ele in collector_template
                                                let view = ele as ViewPlan
                                                where view.Name.Equals("A-PLAN-MECH-INTF")
                                                where view.IsTemplate
                                                select view;
            if (templates != null && templates.Count() != 0) { template = templates.First(); }
            

            //找到所有的非建筑链接文件
            FilteredElementCollector linkcollector = new FilteredElementCollector(doc);
            linkcollector.OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType();
            IEnumerable<RevitLinkInstance> links = from ele in linkcollector
                                                   let ins = ele as RevitLinkInstance
                                                   let name = ins.Name
                                                   where name[0] != 'A'
                                                   select ins;

            //获取所有PLOT视图
            List<ViewPlan> plots = GetPLOT(doc);
            if (plots.Count() == 0) { TaskDialog.Show("wrong", "未找到PLOT视图.确认视图名称内含PLOT,并且VIEW-Content参数设置为02.PLOT"); return Result.Failed; }

            //让用户选择哪几个PLOT视图需要生成INTF视图.
            List<ViewPlan> plots_selected = new List<ViewPlan>();
            MyWindow window = new MyWindow();
            window.listbox.ItemsSource = plots;
            if (window.ShowDialog() == true)
            {
                var objs = window.listbox.SelectedItems;
                foreach (Object obj in objs)
                { plots_selected.Add(obj as ViewPlan); }
            }
            if (plots_selected.Count == 0) { return Result.Failed; }

            //记录对应的intf视图.
            List<ViewPlan> intfs = new List<ViewPlan>();
            //记录没有对应intf视图的plot视图.
            List<ViewPlan> plots_withoutintf = new List<ViewPlan>();
            string name_intfs = " ";
            foreach (ViewPlan plot in plots_selected)
            {
                ViewPlan intf = FindINTFByPLOT(plot);
                if (intf == null)
                {
                    plots_withoutintf.Add(plot);
                }
                else
                {
                    intfs.Add(intf);

                    name_intfs = name_intfs + "\n" + intf.Name;
                }
            }
            name_intfs += "\n以上INTF视图已生成,将删除现有视图并重新生成.其间可能出现Revit未响应的情况,请不要惊慌,去倒杯茶回来就好了.";
            if (intfs.Count != 0) { TaskDialog.Show("goa", name_intfs); }

            //预先设置视图样板,以保证后面copy 2D图元的时候减少错误的发生.
            if(template != null)
            {
                using (Transaction transaction_settemplate = new Transaction(doc))
                {
                    transaction_settemplate.Start("set template");
                    foreach (View intf in intfs)
                    {
                        intf.ViewTemplateId = template.Id;
                    }
                    transaction_settemplate.Commit();
                }
            }


            //开启事务 将PLOT的2D图元更新到INTF
            using (Transaction transaction_delete2D = new Transaction(doc))
            {
                transaction_delete2D.Start("delete 2d ");
                List<ElementId> deleteids = new List<ElementId>();
                foreach (ViewPlan intf in intfs)
                {
                    deleteids.AddRange(FindAll2DElmentIds(intf));
                }
                doc.Delete(deleteids);
                transaction_delete2D.Commit();
            }
            using (Transaction transaction_copy2D = new Transaction(doc))
            {
                transaction_copy2D.Start("copy 2d ");
                string errinfo = "以下图元在无法复制:";
                foreach (ViewPlan intf in intfs)
                {
                    ViewPlan plot = FindPLOTByINTF(intf);
                    if (plot == null) { continue; }
                    List<ElementId> ids_2D = FindAll2DElmentIds(plot);
                    Transform transform = ElementTransformUtils.GetTransformFromViewToView(plot, intf);
                    //ElementTransformUtils.CopyElements(plot, ids_2D, intf, transform, new CopyPasteOptions());
                    foreach (ElementId id in ids_2D)
                    {
                        List<ElementId> ids = new List<ElementId>();
                        ids.Add(id);
                        try
                        {
                            ElementTransformUtils.CopyElements(plot, ids, intf, transform, new CopyPasteOptions());
                        }
                        catch
                        {
                            errinfo = errinfo + "\n ID:" + id.IntegerValue + "类别:" + doc.GetElement(id).Category.Name;
                            continue;
                        }
                    }
                }
                TaskDialog.Show("goa", errinfo);
                transaction_copy2D.Commit();
            }

            //开启事务 带细节复制plot视图
            using (Transaction transaction_duplicate = new Transaction(doc))
            {
                transaction_duplicate.Start("带细节复制plot视图");
                foreach (ViewPlan plot in plots_withoutintf)
                {
                    ElementId id_newintf = plot.Duplicate(ViewDuplicateOption.WithDetailing);
                    ViewPlan newintf = doc.GetElement(id_newintf) as ViewPlan;
                    //重命名视图名字
                    newintf.Name = plot.Name.Replace("PLOT", "INTF");
                    //共享参数VIEW-Content中 PLOT修改为INTF
                    Parameter view_content = newintf.LookupParameter("VIEW-Content");
                    view_content.Set("03.INTF");
                }
                transaction_duplicate.Commit();
            }

            string wrong = "以下PLOT视图似乎存在问题,请手动进行提资操作.";
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("deal");
                foreach (ViewPlan plot in plots_selected)
                {
                    ViewPlan intf = FindINTFByPLOT(plot);
                    if (intf == null) { wrong = wrong + "\n" + plot.Name; continue; }

                    HideHDDNTextNote(intf);
                    HideHDDNAnnotation(intf);
                    HideANNODimension(intf);

                    //应用视图样板
                    if(template != null)
                    {
                        intf.ViewTemplateId = template.Id;
                    }

                    //关闭非建筑链接
                    List<ElementId> links_tohidden = new List<ElementId>();
                    foreach (RevitLinkInstance rli in links)
                    {
                        if (rli == null) { continue; }
                        if (rli.CanBeHidden(intf))
                        {
                            links_tohidden.Add(rli.Id);
                        }
                    }
                    intf.HideElements(links_tohidden);
                }

                transaction.Commit();
            }

            return Result.Succeeded;
        }


        public List<ViewPlan> GetINTF(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewPlan)).OfCategory(BuiltInCategory.OST_Views);
            IEnumerable<ViewPlan> intfs =
                                            from elem in collector
                                            let view = elem as ViewPlan
                                            where view.ViewType == ViewType.FloorPlan
                                            where view.Name.Contains("INTF")
                                            where !view.IsTemplate
                                            let paras = view.GetParameters("VIEW-Content")
                                            let para = paras.First()
                                            where para.AsString() == "03.INTF"
                                            select view;
            return intfs.ToList();
        }

        public List<ViewPlan> GetPLOT(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewPlan)).OfCategory(BuiltInCategory.OST_Views);
            IEnumerable<ViewPlan> plots =
                                            from elem in collector
                                            let view = elem as ViewPlan
                                            where view.ViewType == ViewType.FloorPlan
                                            where view.Name.Contains("PLOT")
                                            where !view.IsTemplate
                                            let paras = view.GetParameters("VIEW-Content")
                                            let para = paras.First()
                                            where para.AsString() == "02.PLOT"
                                            select view;
            return plots.ToList();
        }

        public ViewPlan FindPLOTByINTF(ViewPlan intf)
        {
            List<ViewPlan> plots = GetPLOT(intf.Document);
            string plotname = intf.Name.Replace("INTF", "PLOT");
            foreach (ViewPlan plot in plots)
            {
                if (plot.Name == plotname && plot.LookupParameter("VIEW-Content").AsString() == "02.PLOT")
                { return plot; }
            }
            return null;
        }

        public ViewPlan FindINTFByPLOT(ViewPlan plot)
        {
            List<ViewPlan> intfs = GetINTF(plot.Document);
            string intfname = plot.Name.Replace("PLOT", "INTF");
            foreach (ViewPlan intf in intfs)
            {
                if (intf.Name == intfname && intf.LookupParameter("VIEW-Content").AsString() == "03.INTF")
                { return intf; }
            }
            return null;
        }

        public List<ElementId> FindAll2DElmentIds(ViewPlan view)
        {
            Document doc = view.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<ElementId> ids = collector.OwnedByView(view.Id).WhereElementIsNotElementType().ToElementIds().ToList();
            List<ElementId> todelete = new List<ElementId>();
            foreach (ElementId id in ids)
            {
                Element ele = doc.GetElement(id);
                if (!DocumentValidation.CanDeleteElement(doc, id)) { continue; }
                if (ele.Category == null) { continue; }
                if (ele is ViewPlan) { continue; }
                todelete.Add(id);
            }
            return todelete;
        }

        public void HideHDDNTextNote(ViewPlan intf)
        {
            //隐藏INTF视图中含HDDN所有文字
            FilteredElementCollector txetnotecollector = new FilteredElementCollector(intf.Document, intf.Id);
            List<TextNote> textnotes = txetnotecollector.OfClass(typeof(TextNote)).Select(e => e as TextNote).ToList();
            if (textnotes == null) { return; }
            List<ElementId> textids = new List<ElementId>();
            foreach (TextNote tn in textnotes)
            {
                if (tn.CanBeHidden(intf) && tn.Name.Contains("HDDN")) { textids.Add(tn.Id); }
            }
            if (textids.Count != 0) { intf.HideElements(textids); ; }
        }

        public void HideHDDNAnnotation(ViewPlan intf)
        {
            //隐藏INTF视图中含HDDN常规注释
            FilteredElementCollector annotationcollector = new FilteredElementCollector(intf.Document, intf.Id);
            List<Element> familyinstances = annotationcollector.OfClass(typeof(Family)).OfCategory(BuiltInCategory.OST_GenericAnnotation).ToList();
            if (familyinstances == null) { return; }
            List<ElementId> familyinstanceids = new List<ElementId>();
            foreach (Element anno in familyinstances)
            {
                if (anno.CanBeHidden(intf) && anno.Name.Contains("HDDN")) { familyinstanceids.Add(anno.Id); }
            }
            if (familyinstanceids.Count != 0) { intf.HideElements(familyinstanceids); ; }

        }

        public void HideANNODimension(ViewPlan intf)
        {
            //隐藏INTF视图中ANNO标注
            FilteredElementCollector dimensioncollector = new FilteredElementCollector(intf.Document, intf.Id);
            List<Dimension> dimensions = dimensioncollector.OfClass(typeof(Dimension)).Select(e => e as Dimension).ToList();
            if (dimensions == null) { return; }
            List<ElementId> dimensionids = new List<ElementId>();
            foreach (Dimension ds in dimensions)
            {
                if (ds.Name.Contains("ANNO") && ds.CanBeHidden(intf)) { dimensionids.Add(ds.Id); }
            }
            if (dimensionids != null && dimensionids.Count != 0) { intf.HideElements(dimensionids); }
        }


        public string Show(List<ViewPlan> vps)
        {
            string str = " ";
            foreach (ViewPlan vp in vps)
            {
                str = str + "\n" + vp.Name;
            }
            return str;
        }

    }

    public class ViewPair
    {
        public ViewPlan m_intf;
        public ViewPlan m_plot;

        public ViewPair(ViewPlan plot, ViewPlan intf)
        {
            m_plot = plot;
            m_intf = intf;
        }
    }

}
