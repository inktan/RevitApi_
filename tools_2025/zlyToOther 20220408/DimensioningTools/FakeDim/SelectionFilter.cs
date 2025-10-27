using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAKE_DIMS
{
    public class FakeDimensionFamilyInstanceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                var fi = elem as FamilyInstance;
                var fa = fi.Symbol.Family;
                if (fa.Name.Contains("尺寸标注-毫米"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class LinearDimensionSelectionFitler : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Dimension == false)
                return false;
            var di = elem as Dimension;
            return di.DimensionType.StyleType == DimensionStyleType.Linear;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

}
