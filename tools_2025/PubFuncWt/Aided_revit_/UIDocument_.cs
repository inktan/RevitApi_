using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
   public static class UIDocument_
    {
        public static Element PickElement(this UIDocument _uidoc, string _prompt = "选择图元")
        {
            Reference pickRef = _uidoc.Selection.PickObject(ObjectType.Element, _prompt);
            Document doc = _uidoc.Document;
            Element elem = doc.GetElement(pickRef);
            return elem;
        }
        public static Element PickElement(this UIDocument _uidoc, ISelectionFilter selectionFilter,string _prompt = "选择图元")
        {
            Reference pickRef = _uidoc.Selection.PickObject(ObjectType.Element, selectionFilter, _prompt);
            Document doc = _uidoc.Document;
            Element elem = doc.GetElement(pickRef);
            return elem;
        }
        public static Face PickFace(this UIDocument _uidoc, string _prompt = "选择一个面")
        {
            Reference pick = _uidoc.Selection.PickObject(ObjectType.Face, _prompt);
            Document doc = _uidoc.Document;
            Element elem = doc.GetElement(pick);
            Face face = elem.GetGeometryObjectFromReference(pick) as Face;
            return face;
        }
    }
}
