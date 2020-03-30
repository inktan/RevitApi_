/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

//批量提资

namespace VIEW_INTF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //读取和过滤视图
            List<ViewPlan> viewplans = new List<ViewPlan>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            viewplans = collector.OfClass(typeof(ViewPlan)).Select(e => e as ViewPlan).ToList();
            IEnumerable<ViewPlan> selectiontemp =
                from view in viewplans
                where view.ViewType == ViewType.FloorPlan
                where view.Name.Contains("PLOT")
                where !view.IsTemplate
                select view;
            if (selectiontemp == null) { TaskDialog.Show("wrong", "未找到包含PLOT的楼层平面视图"); return Result.Failed; }
            List<ViewPlan> selection = selectiontemp.ToList();

            //检测是否已有INTF视图.
            List<ViewPlan> intfs = new List<ViewPlan>();
            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            intfs = collector2.OfClass(typeof(ViewPlan)).Select(e => e as ViewPlan).ToList();
            IEnumerable<ViewPlan> sel =
                from view in intfs
                where view.ViewType == ViewType.FloorPlan
                where view.Name.Contains("INTF")
                where !view.IsTemplate
                select view;
            if (sel != null)
            {
                string str = "";
                foreach (ViewPlan vp in sel)
                {
                    string intfname = vp.Name;
                    for (int i = selection.Count - 1; i >= 0; i--)
                    {
                        string plotname = selection[i].Name;
                        plotname = plotname.Replace("PLOT", "INTF");
                        if (intfname == plotname)
                        {
                            selection.RemoveAt(i);
                            str += vp.Name;
                            str += "\n";
                        }
                    }
                }
                TaskDialog.Show("GOA", "检测已有部分INTF视图.\n已存在的INTF视图有:\n" + str + "\n已排除.");
                //排除已有plot视图.
            }

            List<ViewPlan> plots = new List<ViewPlan>();
            foreach (ViewPlan plot in selection)
            {
                ParameterSet paras = plot.Parameters;
                Parameter para = null;
                foreach (Parameter tmp in paras)
                {
                    if (tmp.Definition.Name == "VIEW-Content")
                    { para = tmp; break; }
                }
                if (para != null && para.AsString() == "02.PLOT")
                { plots.Add(plot); }
            }
            if (plots == null) { TaskDialog.Show("wrong", "所有PLOT视图内未找到VIEW-Content值为02.PLOT的视图."); return Result.Failed; }

            //获取选中的视图
            UserControl1 usercontrol = new UserControl1();
            usercontrol.listbox.ItemsSource = plots;
            List<ViewPlan> selplots = new List<ViewPlan>();
            if (usercontrol.ShowDialog() == true)
            {
                var objs = usercontrol.listbox.SelectedItems;
                foreach (Object obj in objs)
                { selplots.Add(obj as ViewPlan); }
            }
            if (selplots.Count == 0)
            { TaskDialog.Show("wrong", "没有获取到视图"); return Result.Failed; }

            //开启事务
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("批量提资");
                //带细节复制View.Duplicate  + ViewDuplicateOption 
                List<ViewPlan> intfviews = new List<ViewPlan>();
                List<ElementId> intfviewids = new List<ElementId>(); //后面删除文字和注释的时候创建collector要用到.
                foreach (ViewPlan vp in selplots)
                {
                    ViewPlan intfview = null;
                    ElementId id = vp.Duplicate(ViewDuplicateOption.WithDetailing);
                    intfviewids.Add(id);
                    intfview = doc.GetElement(id) as ViewPlan;
                    intfviews.Add(intfview);
                    //重命名视图名字PLOT为INTF
                    intfview.Name = vp.Name.Replace("PLOT", "INTF");
                    //共享参数VIEW-Content中 PLOT修改为INTF
                    ParameterSet paras = intfview.Parameters;
                    Parameter para = null;
                    foreach (Parameter tmp in paras)
                    {
                        if (tmp.Definition.Name == "VIEW-Content")
                        { para = tmp; break; }
                    }
                    if (para == null) { continue; }
                    para.Set("03.INTF");
                }
                TaskDialog.Show("GOA", "已复制INTF视图,并重命名PLOT为INTF且将VIEW-Content中PLOT修改为INTF");


                //隐藏INTF视图中所有文字
                foreach (ViewPlan intf in intfviews)
                {
                    FilteredElementCollector txetnotecollector = new FilteredElementCollector(doc, intf.Id);
                    List<TextNote> textnotes = txetnotecollector.OfClass(typeof(TextNote)).Select(e => e as TextNote).ToList();
                    if (textnotes == null) { continue; }
                    List<ElementId> textids = new List<ElementId>();
                    foreach (TextNote tn in textnotes)
                    {
                        if (tn.CanBeHidden(intf) && tn.Name.Contains("HDDN")) { textids.Add(tn.Id); }
                    }
                    if (textids != null && textids.Count != 0) { intf.HideElements(textids); ; }

                }
                TaskDialog.Show("GOA", "隐藏INTF视图中所有文字");

                //隐藏INTF视图中常规注释
                foreach (ViewPlan intf in intfviews)
                {
                    FilteredElementCollector annotationcollector = new FilteredElementCollector(doc, intf.Id);
                    List<Element> familyinstances = annotationcollector.OfClass(typeof(Family)).OfCategory(BuiltInCategory.OST_GenericAnnotation).ToList();
                    if (familyinstances == null) { continue; }
                    List<ElementId> familyinstanceids = new List<ElementId>();
                    foreach (Element tn in familyinstances)
                    {
                        if (tn.CanBeHidden(intf) && tn.Name.Contains("HDDN")) { familyinstanceids.Add(tn.Id); }
                    }
                    if (familyinstanceids != null && familyinstanceids.Count != 0) { intf.HideElements(familyinstanceids); ; }
                }
                TaskDialog.Show("GOA", "隐藏INTF视图中所有文字");


                //隐藏INTF视图中ANNO标注
                foreach (ViewPlan intf in intfviews)
                {
                    FilteredElementCollector dimensioncollector = new FilteredElementCollector(doc, intf.Id);
                    List<Dimension> dimensions = dimensioncollector.OfClass(typeof(Dimension)).Select(e => e as Dimension).ToList();
                    if (dimensions == null) { continue; }
                    List<ElementId> dimensionids = new List<ElementId>();
                    foreach (Dimension ds in dimensions)
                    {
                        if (ds.Name.Contains("ANNO") && ds.CanBeHidden(intf)) { dimensionids.Add(ds.Id); }
                    }
                    if (dimensionids != null && dimensionids.Count != 0) { intf.HideElements(dimensionids); }
                }

                TaskDialog.Show("GOA", "隐藏INTF视图中所有ANNO标注");

                transaction.Commit();
            }


            return Result.Succeeded;
        }
    }
}

*/