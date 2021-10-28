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

namespace goa.Common
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD_hostId : IExternalCommand
    {
        internal static IList<FamilyInstance> allFi;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var ids = sel.GetElementIds().ToList();
            string m = "";
            if (ids.Count == 0)
            {
                var pick = sel.PickObject(ObjectType.Element, "选择一个构件");
                ids.Add(pick.ElementId);
            }
            var fi = doc.GetElement(ids.First()) as FamilyInstance;
            if (fi == null)
                m = "not a family instance.";
            else
            {
                var host = fi.Host;
                if (host == null)
                    m = "no host.";
                else
                    m = fi.Host.Id.ToString();
            }

            UserMessages.ShowMessage(m);
            int idInt;
            bool b = int.TryParse(m, out idInt);
            if(b)
            {
                System.Windows.Forms.Clipboard.SetText(m);
                sel.SetElementIds(new List<ElementId>() { new ElementId(idInt) });
            }
            return Result.Succeeded;
        }
    }
}

