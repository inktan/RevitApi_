using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public enum HostCondition
    {
        NoHostHorizontal,
        NoHostVertical,
        NoHostOther,
        Level,
        HorizontalFace,
        VerticalFace,
        HostedOther,
    }
    /// <summary>
    /// extract information from element for 
    /// other methods of line based family instance.
    /// </summary>
    public class LineBasedFamilyInfo
    {
        public FamilyInstance FI;
        public Line LocLine;
        internal Line OffsetLocLine; //location line + offset from base plane
        public HostCondition HostCondition;
        public LineBasedFamilyInfo(FamilyInstance fi)
        {
            this.FI = fi;
            this.LocLine = fi.LocationLine();
            this.OffsetLocLine = LineBasedFamilyUtils.GetOffsetLocLine(fi);

            getHostCondition(fi);
        }
        internal static bool IsLineBasedInstance(Element _elem)
        {
            if (_elem is FamilyInstance == false
                || _elem.Location is LocationCurve == false)
                return false;
            var lc = _elem.Location as LocationCurve;
            return lc.Curve is Line;
        }

        private void getHostCondition(FamilyInstance _fi)
        {
            XYZ norm = _fi.HandOrientation.CrossProduct(_fi.FacingOrientation);
            bool horizontal = Math.Abs(norm.Z).IsAlmostEqualByDifference(1.0);
            bool vertical = norm.Z.IsAlmostEqualByDifference(0.0);
            if (_fi.Host == null)
            {
                if (horizontal)
                    this.HostCondition = HostCondition.NoHostHorizontal;
                else if (vertical)
                    this.HostCondition = HostCondition.NoHostVertical;
                else
                    this.HostCondition = HostCondition.NoHostOther;
            }
            else if (_fi.Host is Level)
                this.HostCondition = HostCondition.Level;
            else if (_fi.HostFace != null)
            {
                var face = _fi.Host.GetGeometryObjectFromReference(_fi.HostFace);
                if (face is PlanarFace)
                {
                    var pf = face as PlanarFace;
                    if (pf.FaceNormal.Z.IsAlmostEqualByDifference(0))
                        this.HostCondition = HostCondition.VerticalFace;
                    else if (Math.Abs(pf.FaceNormal.Z).IsAlmostEqualByDifference(1))
                        this.HostCondition = HostCondition.HorizontalFace;
                }
            }
            else
                this.HostCondition = HostCondition.HostedOther;
        }
    }
}
