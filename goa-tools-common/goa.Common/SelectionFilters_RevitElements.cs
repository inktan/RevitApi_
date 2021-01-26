using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace goa.Common
{
    #region Selection Filter
    public class CurveElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurveElement)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class FilledRegionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FilledRegion)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class CurtainGridLineSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurtainGridLine)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class CurtainPanelSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Autodesk.Revit.DB.Panel)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class LineElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurveElement)
            {
                var ce = elem as CurveElement;
                if (ce.GeometryCurve is Line)
                    return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class GroupSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Group)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class FamilyLoadOptions : IFamilyLoadOptions
    {
        public bool OnFamilyFound(
          bool familyInUse,
          out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(
          Family sharedFamily,
          bool familyInUse,
          out FamilySource source,
          out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Wall)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class WallStraightLineSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Wall)
                return elem.LocationCurve() is Line;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class LinearDimensionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return IsLinearDimensionElem(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }

        public static bool IsLinearDimensionElem(Element elem)
        {
            if (elem is Dimension)
            {
                var di = elem as Dimension;
                var dt = di.DimensionType;
                return dt.StyleType == DimensionStyleType.Linear;
            }
            else
            {
                return false;
            }
        }
    }
    public class FamilyInstanceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class SitePlanFamilySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            SitePlanFamilyType type = SitePlanFamilyType.Block_Highrise;
            return elem is FamilyInstance
                && FirmStandards.IsSiteBlockFamily(elem, ref type);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class PlanarFaceSelectionFilter : ISelectionFilter
    {
        private Document doc;
        public PlanarFaceSelectionFilter(Document _doc)
        {
            this.doc = _doc;
        }
        public bool AllowElement(Element elem)
        {
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            var elem = this.doc.GetElement(reference);
            var geometryObject = elem.GetGeometryObjectFromReference(reference);
            return geometryObject is PlanarFace;
        }
    }

    public class StraightEdgeSelectionFilter : ISelectionFilter
    {
        private Document doc;
        public StraightEdgeSelectionFilter(Document _doc)
        {
            this.doc = _doc;
        }
        public bool AllowElement(Element elem)
        {
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            var elem = this.doc.GetElement(reference);
            var geometryObject = elem.GetGeometryObjectFromReference(reference);
            if (geometryObject is Edge == false)
                return false;
            else
            {
                var edge = geometryObject as Edge;
                return edge.AsCurve() is Line;
            }
        }
    }
    #endregion
}
