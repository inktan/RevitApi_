using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using goa.Common.Exceptions;

namespace NumberingTool
{
    internal static class UICmd
    {
        internal static List<Element> Elems = new List<Element>();
        internal static void AddCurrSelection()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var ids = sel.GetElementIds();
            if (ids.Count == 0)
                throw new CommonUserExceptions("请先选择需要编号的图元。");
            Elems = ids.Select(x => doc.GetElement(x)).ToList();
        }
        internal static void Numbering(Input _input)
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            string error = "";
            int i = 1;
            using(Transaction trans = new Transaction(doc, "自动编号"))
            {
                trans.Start();
                foreach (var e in Elems)
                {
                    var p = e.LookupParameter(_input.ParamName);
                    if (p == null)
                    {
                        error += e.Name + " " + e.Id + " 未找到参数。\r\n";
                        continue;
                    }
                    var value = _input.Prefix + i + _input.Surfix;
                    p.Set(value);
                    i++;
                }
                trans.Commit();
            }

            if(error != "")
            {
                FlexibleMessageBox.Show(error);
            }
        }
    }
}
