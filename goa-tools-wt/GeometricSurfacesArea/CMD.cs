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

namespace GeometricSurfacesArea
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

            IList<Reference> pickedref = new List<Reference>();
            pickedref = sel.PickObjects(ObjectType.Face, "Pleae select one faces");
            double areas = 0;
            foreach (Reference pickedrefTemp in pickedref)
            {
                Face face = doc.GetElement(pickedrefTemp).GetGeometryObjectFromReference(pickedrefTemp) as Face;
                areas += face.Area;
            }
            double convertFaceArea = UnitUtils.Convert(areas, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);

            TaskDialog.Show("Revit 2018", "The area of these selected faces is " + convertFaceArea.ToString() + "㎡.");
            //TaskDialog.Show("revit", DateTime.Now.ToString());

            return Result.Succeeded;
        }

    }
}
