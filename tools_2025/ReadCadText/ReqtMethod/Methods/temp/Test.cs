using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace ReadCadText
{
    internal class Test : RequestMethod
    {
        internal Test(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
