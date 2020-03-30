using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;


//将所有PLOT视图内的线转换成黑色或者变回"按类别".

namespace COLR_SWIT
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    //public class CMD : IExternalCommand
    //{
    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        UIDocument uidoc = commandData.Application.ActiveUIDocument;
    //        Document doc = commandData.Application.ActiveUIDocument.Document;

    //        View activeview = uidoc.ActiveView;

    //        //找plot视图
    //        FilteredElementCollector collector = new FilteredElementCollector(doc);
    //        IEnumerable<Element> views = collector.OfClass(typeof(View)).WhereElementIsNotElementType();
    //        IEnumerable<ElementId> viewids =
    //            from element in views
    //            let view = element as View
    //            //where view.Name.Contains("PLOT")
    //            let para = view.LookupParameter("VIEW-Content")
    //            where para != null
    //            where para.AsString() == "02.PLOT"
    //            where !view.IsTemplate
    //            select view.Id;
    //        if (viewids == null || viewids.Count() == 0) { TaskDialog.Show("wrong", "未找到包含PLOT的视图"); return Result.Failed; }

    //        OverrideGraphicSettings ogs = new OverrideGraphicSettings();

    //        Window1 window = new Window1();
    //        window.ShowDialog();
    //        if (window.i == 0) { return Result.Cancelled; }
    //        if (window.i == 1)
    //        {
    //            ogs.SetProjectionLineColor(new Color(0, 0, 0));
    //        }

    //        using (Transaction transaction = new Transaction(doc))
    //        {
    //            transaction.Start("将线改色");
    //            foreach (ElementId viewid in viewids)
    //            {
    //                View view = doc.GetElement(viewid) as View;
    //                //找线
    //                FilteredElementCollector linecollector = new FilteredElementCollector(doc);
    //                linecollector.OfCategory(BuiltInCategory.OST_Lines).OwnedByView(viewid).WhereElementIsNotElementType();
    //                List<ElementId> ids = linecollector.ToElementIds().ToList();
    //                foreach (ElementId id in ids)
    //                {
    //                    view.SetElementOverrides(id, ogs);
    //                }
    //            }
    //            transaction.Commit();
    //        }


    //        return Result.Succeeded;
    //    }
    //}


    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            View activeview = uidoc.ActiveView;

            //找plot视图
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IEnumerable<Element> views = collector.OfClass(typeof(View)).WhereElementIsNotElementType();
            IEnumerable<ElementId> viewids =
                from element in views
                let view = element as View
                //where view.Name.Contains("PLOT")
                let para = view.LookupParameter("VIEW-Content")
                where para != null
                where para.AsString() == "02.PLOT" || para.AsString() == "03.INTF"
                where !view.IsTemplate
                select view.Id;
            if (viewids == null || viewids.Count() == 0) { TaskDialog.Show("wrong", "未找到包含PLOT的视图"); return Result.Failed; }

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();

            Window1 window = new Window1();
            window.ShowDialog();
            if (window.i == 0) { return Result.Cancelled; }
            if (window.i == 1)
            {
                ogs.SetProjectionLineColor(new Color(0, 0, 0));
            }

            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("将线改色");
                foreach (ElementId viewid in viewids)
                {
                    View view = doc.GetElement(viewid) as View;
                    //找线
                    FilteredElementCollector linecollector = new FilteredElementCollector(doc);
                    linecollector.OfCategory(BuiltInCategory.OST_Lines).OwnedByView(viewid).WhereElementIsNotElementType();
                    List<ElementId> lineids = linecollector.ToElementIds().ToList();
                    //找填充
                    FilteredElementCollector filledregioncollector = new FilteredElementCollector(doc);
                    filledregioncollector.OfCategory(BuiltInCategory.OST_DetailComponents).OfClass(typeof(FilledRegion)).WhereElementIsNotElementType();
                    List<ElementId> filledregionids = filledregioncollector.ToElementIds().ToList();
                    for(int i = filledregionids.Count -1; i>=0;i--)
                    {
                        FilledRegion fr = doc.GetElement(filledregionids[i]) as FilledRegion;
                        FilledRegionType type_fr = doc.GetElement(fr.GetTypeId()) as FilledRegionType;
                        if(type_fr.Name == "A-MATE-CONC-PLAN") { filledregionids.RemoveAt(i);continue; }
                        if (type_fr.Name == "A-MATE-CONC-50") { filledregionids.RemoveAt(i); continue; }
                        if (type_fr.Name == "A-MATE-CONC-ARAT") { filledregionids.RemoveAt(i); continue; }
                        if (type_fr.Name == "A-MATE-CONC-100") { filledregionids.RemoveAt(i); continue; }
                    }

                    foreach (ElementId id in lineids)
                    {
                        view.SetElementOverrides(id, ogs);
                    }
                    foreach (ElementId id in filledregionids)
                    {
                        view.SetElementOverrides(id, ogs);
                    }

                }
                transaction.Commit();
            }





            return Result.Succeeded;
        }
    }
}
