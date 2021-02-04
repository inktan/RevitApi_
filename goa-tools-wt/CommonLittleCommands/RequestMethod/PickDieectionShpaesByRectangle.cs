using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using PublicProjectMethods_;

namespace CommonLittleCommands
{
    class PickDieectionShpaesByRectangle : RequestMethod
    {
        public PickDieectionShpaesByRectangle(UIApplication uiapp) : base(uiapp)
        { }

        internal override void Execute()
        {
            UIDocument uidoc = this.uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View actiView = doc.ActiveView;

            List<ElementId> selElementIds = sel.PickElementsByRectangle(new SelPickFilter_DirectShape(), "请点选详图区域，地库外墙线线圈（线样式为绿色）").Select(p => p.Id).ToList();

            sel.SetElementIds(selElementIds);

            //throw new NotImplementedException();
        }
    }
}
