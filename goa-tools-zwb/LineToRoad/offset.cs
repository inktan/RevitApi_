using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace LINE_ROAD
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class Offset : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View activeview = uiDoc.ActiveView;

            Reference refe = uiDoc.Selection.PickObject(ObjectType.Element);

            DetailArc da = doc.GetElement(refe) as DetailArc;

            Arc arc = da.GeometryCurve as Arc;

           Arc newarc1 =  arc.CreateOffset(500/304.8, new XYZ(0, 0, 1)) as Arc;
            Arc newarc2 = arc.CreateOffset(500/304.8, new XYZ(0, 0, -1)) as Arc;

            using (Transaction creat_line = new Transaction(doc))
            {
                creat_line.Start("start");

                    doc.Create.NewDetailCurve(activeview, newarc1);
                doc.Create.NewDetailCurve(activeview, newarc2);


                creat_line.Commit();
            }

            return Result.Succeeded;

        }
    }
}
