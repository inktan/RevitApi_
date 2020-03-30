using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace GeometricSurfaceArea
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            Selection sel = uiapp.ActiveUIDocument.Selection;
            Reference pickedref = sel.PickObject(ObjectType.Face, "Please select a  face");
            Element elem = doc.GetElement(pickedref);
            Face face = elem.GetGeometryObjectFromReference(pickedref) as Face;

            double convertFaceArea = UnitUtils.Convert(face.Area, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);

            TaskDialog.Show("Revit 2018", "The area of the selected faces is " + convertFaceArea.ToString() + "㎡.");
            //TaskDialog.Show("revit", DateTime.Now.ToString());

            return Result.Succeeded;

        }

    }
}
