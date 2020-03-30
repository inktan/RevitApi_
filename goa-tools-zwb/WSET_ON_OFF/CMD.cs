using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

//改变工作集可见性

namespace WSET_ON_OFF
{
    [TransactionAttribute(TransactionMode.Manual)]


    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            View activeview = uidoc.ActiveView;

            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);

            List<Workset> list_visible = new List<Workset>();
            List<Workset> list_invisible = new List<Workset>();
            foreach(Workset ws in collector)
            {
                if (activeview.IsWorksetVisible(ws.Id)) { list_visible.Add(ws); }
                else{ list_invisible.Add(ws); }
            }


            //window
            Window1 window = new Window1();
            window.listbox_visible.ItemsSource = list_visible;
            window.listbox_invisible.ItemsSource = list_invisible;
            
            //selected
            List<Workset> seleted_visible = new List<Workset>();
            List<Workset> seleted_invisible = new List<Workset>();
            if (window.ShowDialog() == true)
            {
                var objs1 = window.listbox_visible.SelectedItems;
                foreach (Object obj in objs1)
                { seleted_visible.Add(obj as Workset); }

                var objs2 = window.listbox_invisible.SelectedItems;
                foreach (Object obj in objs2)
                { seleted_invisible.Add(obj as Workset); }
            }

            //change visible
            using (Transaction transaction = new Transaction(doc))  
            {
                transaction.Start("改变工作集可见性");

                foreach (Workset ws in seleted_visible)
                { activeview.SetWorksetVisibility(ws.Id, WorksetVisibility.Hidden); }

                foreach (Workset ws in seleted_invisible)
                { activeview.SetWorksetVisibility(ws.Id, WorksetVisibility.Visible); }

                transaction.Commit();
            }


            return Result.Succeeded;
        }
    }
}
