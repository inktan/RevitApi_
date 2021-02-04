using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

namespace CommonLittleCommands
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
