using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace AutoFillUpLevelHeightAnnotation
{
    public class GenericAnnotationSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return
                elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericAnnotation;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
