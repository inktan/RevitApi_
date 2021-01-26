using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using System.DirectoryServices;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;

namespace goa.Common
{
    public class DesignOptionSet
    {
        public Element DesignOptionSet_revit;
        public string Name { get { return DesignOptionSet_revit.Name; } }
        public IList<DesignOption> DesignOptions { get; set; }

        public static IList<DesignOptionSet> GetDesignOptionSets(Document document)
        {
            Dictionary<ElementId, List<DesignOption>> dic = new Dictionary<ElementId, List<DesignOption>>();
            var allDesignOptions = new FilteredElementCollector(document).OfClass(typeof(DesignOption)).Cast<DesignOption>();
            foreach (DesignOption dOpt in allDesignOptions)
            {
                Element dosElem = document.GetElement(dOpt.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId());
                dic.TryAddValue(dosElem.Id, dOpt);
            }
            List<DesignOptionSet> list = new List<DesignOptionSet>();
            foreach (var pair in dic)
            {
                var dos = new DesignOptionSet();
                dos.DesignOptionSet_revit = document.GetElement(pair.Key);
                dos.DesignOptions = pair.Value.Cast<DesignOption>().ToList();
                list.Add(dos);
            }
            return list;
        }
    }
}
