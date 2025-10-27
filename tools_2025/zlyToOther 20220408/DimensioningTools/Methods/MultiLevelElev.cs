using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using goa.Common;
using goa.Common.Exceptions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace DimensioningTools
{
    internal static partial class Methods
    {
        internal static void MultiLevelElev()
        {
            UIDocument activeUIDocument = goa.Common.APP.UIApp.ActiveUIDocument;
            Document document = activeUIDocument.Document;
            var filter = new ElementClassSelectionFilter<Level>();
            Form_cursorPrompt.Start("左键按下框选多个标高.", APP.MainWindow);
            IEnumerable<Level> levels =
                from elem in activeUIDocument.Selection.PickElementsByRectangle(filter)
                let level = elem as Level
                orderby level.Elevation
                select level;
            string str = null;
            foreach (Level level1 in levels)
            {
                double elevation = level1.Elevation * 304.8 / 1000;
                string str1 = elevation.ToString("f3");
                string str2 = level1.Name.Substring(2, 2);
                if (str2[0] == '0')
                {
                    str2 = str2.Substring(1, 1);
                }
                str = string.Concat(new string[] { str, str1, " (", str2, "F)\n" });
            }
            TaskDialog.Show("goa", str);
            View activeView = activeUIDocument.ActiveView;
            XYZ xYZ = null;
            Form_cursorPrompt.Start("选择标注放置位置", APP.MainWindow);
            try
            {
                xYZ = activeUIDocument.Selection.PickPoint();
            }
            catch
            {
                using (Transaction transaction = new Transaction(activeUIDocument.Document, "调整工作平面"))
                {
                    transaction.Start();
                    Plane plane = Plane.CreateByNormalAndOrigin(activeUIDocument.ActiveView.ViewDirection, activeUIDocument.ActiveView.Origin);
                    SketchPlane sketchPlane = SketchPlane.Create(document, plane);
                    activeUIDocument.ActiveView.SketchPlane = sketchPlane;
                    activeUIDocument.ActiveView.HideActiveWorkPlane();
                    transaction.Commit();
                }
                xYZ = activeUIDocument.Selection.PickPoint();
            }
            ElementId id = null;
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
            filteredElementCollector.OfClass(typeof(TextNoteType)).WhereElementIsElementType();
            foreach (Element element in filteredElementCollector)
            {
                if (element.Name == "Arial Narrow 2.5mm 1.00")
                {
                    id = element.Id;
                }
            }
            if (id == null)
            {
                id = document.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            }
            using (Transaction transaction1 = new Transaction(document))
            {
                transaction1.Start("批量楼层标高");
                if (str != null)
                {
                    TextNote.Create(document, activeView.Id, xYZ, str, id);
                }
                transaction1.Commit();
            }
        }
    }
}
