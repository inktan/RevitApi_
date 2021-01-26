using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

namespace MODELESS_PROJECT_TEMPLATE_WPF
{
    abstract class RequestMethod
    {
        internal UIApplication uiapp;
        public RequestMethod(UIApplication uiapp)
        {
            this.uiapp = uiapp;
        }

        internal abstract void Execute();

    }
}
