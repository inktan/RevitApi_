using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace ANNO_ELEV_MULT
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //找到所有标高
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            IList<Element> list_Elem = uiDoc.Selection.PickElementsByRectangle(new LevelSelection(), "选择标高.");
            IEnumerable<Level> enu_Elem = from elem in list_Elem
                                            let level = elem as Level
                                            orderby level.Elevation 
                                            select level;

            //生成string
            string str = default(string);
            foreach(Level temp in enu_Elem)
            {
                string height = (temp.Elevation * 304.8/1000).ToString("f3");
                string floor = temp.Name.Substring(2, 2);
                if(floor[0] == '0') { floor = floor.Substring(1, 1); }
                str = str + height + " (" + floor + "F)" + "\n";
            }

            TaskDialog.Show("goa", str);
            //生成TEXT

            View view_Active = uiDoc.ActiveView;
            XYZ position = null;
            try
            {
                position = uiDoc.Selection.PickPoint();
            }
            catch
            {
                using (Transaction transaction = new Transaction(uiDoc.Document, "tt"))
                {

                    transaction.Start();
                    Plane plane = Plane.CreateByNormalAndOrigin(uiDoc.ActiveView.ViewDirection, uiDoc.ActiveView.Origin);
                    SketchPlane sp = SketchPlane.Create(doc,plane);
                    uiDoc.ActiveView.SketchPlane = sp;
                    uiDoc.ActiveView.HideActiveWorkPlane();
                    transaction.Commit();
                }
                position = uiDoc.Selection.PickPoint();
            }

            ElementId id_Type = null;
            FilteredElementCollector collector_type = new FilteredElementCollector(doc);
            collector_type.OfClass(typeof(TextNoteType)).WhereElementIsElementType();
            foreach(Element elem in collector_type)
            {
                if(elem.Name == "Arial Narrow 2.5mm 1.00") { id_Type = elem.Id; }
            }
            if(id_Type == null) { id_Type = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType); }
            using (Transaction creattext = new Transaction(doc))
            {
                creattext.Start("start");
                if(str != null)
                {
                    TextNote.Create(doc, view_Active.Id, position, str, id_Type);
                }
                creattext.Commit();
            }


            return Result.Succeeded;
        }
    }


    public class LevelSelection : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if(elem is Level) { return true; }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
