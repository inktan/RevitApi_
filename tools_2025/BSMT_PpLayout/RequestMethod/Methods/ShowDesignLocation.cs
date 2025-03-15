
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{

    class ShowDesignLocation : RequestMethod
    {
        public ShowDesignLocation(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds();
            this.uiDoc.ShowElements(selBaseWallIds);

            //throw new NotImplementedException();
        }
    }
}
 