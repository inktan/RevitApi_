using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace TYPE_ON_OFF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //get activeview
            View activeview = uidoc.ActiveView;

            //get SystemFamily typeids
            List<ElementId> typeids = new List<ElementId>();
            List<ElementId> filledregiontypes = GetElementTypeIds<FilledRegionType>(uidoc);
            List<ElementId> textnotetypes = GetElementTypeIds<TextNoteType>(uidoc);
            List<ElementId> dimensiontypes = GetElementTypeIds<DimensionType>(uidoc);
            List<ElementId> spotdimensiontypes = GetElementTypeIds<SpotDimensionType>(uidoc);


            //get FamilySymbol
            FilteredElementCollector familycollector = new FilteredElementCollector(doc);
            familycollector.OfClass(typeof(Family));
            List<ElementId> symbolids = new List<ElementId>();
            foreach (Family fam in familycollector)
            {
                if (fam.Name == "A-SYMB-TEXT" || fam.Name == "A-SYMB-TEXT-CLOT")
                { symbolids.AddRange(fam.GetFamilySymbolIds()); }
            }
            if (symbolids.Count == 0) { TaskDialog.Show("wrong", "没有找到可载入族类型"); }

            //fix
            typeids = filledregiontypes.Union(textnotetypes).Union(dimensiontypes).Union(symbolids).ToList();

            //get all instances of each type
            List<ElementId> allhidden = new List<ElementId>();
            List<ElementId> parthidden = new List<ElementId>();
            for (int i = typeids.Count() - 1; i >= 0; i--)
            {
                List<ElementId> instanceids = GetInstancesIdByElementTypeId(uidoc, typeids[i], activeview.Id);

                int a = 0;
                int count = instanceids.Count;
                foreach(ElementId id in instanceids)
                {
                    Element ele = doc.GetElement(id);
                    if (ele.IsHidden(activeview)) { a++; };
                }
                //no instance or no one is hidden
                if (a == 0) { continue; }
                else if(a == count){ allhidden.Add(typeids[i]); }
                else if(a>0 && a< count) { parthidden.Add(typeids[i]);}
            }
            //transform
            List<ElementType> allhiddentypes = new List<ElementType>();
            foreach(ElementId id in allhidden)
            { allhiddentypes.Add(doc.GetElement(id) as ElementType); }
            List<ElementType> parthiddentypes = new List<ElementType>();
            foreach (ElementId id in parthidden)
            { parthiddentypes.Add(doc.GetElement(id) as ElementType); }


            //window
            Window1 window = new Window1();
            List<ElementType> selected1 = new List<ElementType>();
            List<ElementType> selected2 = new List<ElementType>();
            window.listbox1.ItemsSource = allhiddentypes;
            window.listbox1.DisplayMemberPath = "Name";
            window.listbox2.ItemsSource = parthiddentypes;
            window.listbox2.DisplayMemberPath = "Name";
            if (window.ShowDialog() == true)
            {
                var objs1 = window.listbox1.SelectedItems;
                foreach (Object obj in objs1)
                { selected1.Add(obj as ElementType); }

                var objs2 = window.listbox2.SelectedItems;
                foreach (Object obj in objs2)
                { selected2.Add(obj as ElementType); }
            }
            if (selected1.Count == 0 && selected2.Count == 0)
            { TaskDialog.Show("wrong", "没有选择类型"); return Result.Failed; }

            // unhide
            using (Transaction transaction = new Transaction(doc))  //显示隐藏图元
            {
                transaction.Start("显示隐藏图元");

                foreach (ElementType type in selected1)
                {unhideinstances(uidoc, type);}

                foreach (ElementType type in selected2)
                {unhideinstances(uidoc, type);}

                transaction.Commit();
            }


            return Result.Succeeded;
        }

        public static List<ElementId> GetElementTypeIds<TSystemFamily>(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            FilteredElementCollector typecollector = new FilteredElementCollector(doc);
            typecollector.OfClass(typeof(TSystemFamily)).WhereElementIsElementType();
            if (typecollector.Count() == 0)
            {
                TaskDialog.Show("wrong", "此视图中没有找到" + typeof(TSystemFamily).ToString() + "类型");
                return new List<ElementId>();
            }
            List<ElementId> typeids = typecollector.ToElementIds().ToList();

            return typeids;
        }

        private static List<ElementId> GetInstancesIdByElementTypeId(UIDocument uidoc, ElementId typeid, ElementId activeviewid)
        {
            Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.WhereElementIsNotElementType().OwnedByView(activeviewid);
            IEnumerable<ElementId> enu = from tmp in collector
                                         where tmp.GetTypeId() == typeid
                                         let elementid = tmp.Id
                                         select elementid;
            List<ElementId> ids = enu.ToList();
            return ids;
        }

        private void unhideinstances(UIDocument uidoc, ElementType type)
        {
            View activeview = uidoc.ActiveView;
            List<ElementId> instanceids = GetInstancesIdByElementTypeId(uidoc, type.Id, activeview.Id);
            List<ElementId> ids = new List<ElementId>();
            foreach (ElementId id in instanceids)
            {
                Element instance = uidoc.Document.GetElement(id);
                if (instance.IsHidden(activeview))
                { ids.Add(instance.Id); }
            }
            activeview.UnhideElements(ids);
        }





    }
}
