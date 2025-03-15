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
using goa.Common;

namespace MountSiteDesignAnaly
{
    internal class Test : RequestMethod
    {
        internal Test(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            List<Face> planarFaces = this.doc.GetElement(this.sel.PickObject(ObjectType.Element)).GetAllFaces();
            //List<PlanarFace> planarFaces = this.doc.GetElement(this.sel.PickObject(ObjectType.Element)).GetAllFacesFacingUp().ToList();
            planarFaces.Count.ToString().TaskDialogErrorMessage();


            new FilteredElementCollector(this.doc, this.view.Id).OfClass(typeof(FilledRegion)).ToElements().Count.ToString().TaskDialogErrorMessage();

            //sw.ElapsedMilliseconds.ToString().TaskDialogErrorMessage();
        }
    }

}
