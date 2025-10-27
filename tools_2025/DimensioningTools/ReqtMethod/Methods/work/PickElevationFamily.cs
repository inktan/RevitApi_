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

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
using goa.Common.Exceptions;
//using NetTopologySuite.Geometries;

namespace DimensioningTools
{
    internal class PickElevationFamily : RequestMethod
    {
        internal PickElevationFamily(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new GenericAnnotationSelectionFilter();
            var pickRef = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, filter);
            var fi = doc.GetElement(pickRef) as FamilyInstance;
            if (!isValidFamilyInstance(fi))
            {
                TaskDialog.Show("错误", "选择的族无效。");
                return;
            }
            else
            {
                CMD.fi = fi;
            }
        }

        private bool isValidFamilyInstance(FamilyInstance _fi)
        {
            var p = _fi.GetParameterByName("标高 1");
            return p != null;
        }
    }
}
