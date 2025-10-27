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
    internal class FrameSelectFakeDims : RequestMethod
    {
        internal FrameSelectFakeDims(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            var sel = this.sel;
            var filter = new FakeDimensionFamilyInstanceSelectionFilter();
            var elems = new List<Element>();
            try
            {
                elems = sel.PickElementsByRectangle(filter).ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                return;
            }
            var doc = this.doc;
            var ids = elems.Select(x => x.Id).ToList();
            sel.SetElementIds(ids);
        }

    }
}
