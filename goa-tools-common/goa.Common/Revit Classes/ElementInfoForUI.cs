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
    public class ElementInfoForUI
    {
        public Element Elem;
        public ElementId Id { get { return this.Elem.Id; } }
        public string Name { get { return this.Elem.Name; } }
        public string DisplayName { get; set; }
        public ElementInfoForUI(Element _elem)
        {
            this.Elem = _elem;
            this.DisplayName = this.Name;
        }
    }
}
