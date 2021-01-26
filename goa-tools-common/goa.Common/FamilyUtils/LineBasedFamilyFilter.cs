using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace goa.Common
{
    public class ClassSelectionFilter<T> : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is T;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class CategorySelectionFilter : ISelectionFilter
    {
        private int bic;
        public CategorySelectionFilter(BuiltInCategory _bic)
        {
            this.bic = (int)_bic;
        }
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.IntegerValue == this.bic;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class LineBasedElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return IsLineBasedFamilyInstance(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
        public static bool IsLineBasedFamilyInstance(Element elem)
        {
            return elem.LocationCurve() is Line;
        }
    }
}
