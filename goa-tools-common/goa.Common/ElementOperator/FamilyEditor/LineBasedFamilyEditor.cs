using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class LineBasedFamilyEditor : FamilyEditor
    {
        public Line OffsetLocLine;
        public FamilyCreator SimilarCreator;

        public LineBasedFamilyEditor(
            FamilyInstance _fi,
            FamilySymbol _fs,
            Line _offsetLocLine,
            XYZ _facing,
            bool _cutHost,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
            : base(_cutHost)
        {
            this.FI = _fi;
            this.FS = _fs;
            this.OffsetLocLine = _offsetLocLine;
            this.facing = _facing;
            this.paramValues = _params;
            this.Context = _opContext;
            this.Context = _opContext;
        }
        public override void Execute()
        {
            if (this.OffsetLocLine != null)
            {
                var locLine = getLocLine();
                var lc = this.FI.Location as LocationCurve;
                lc.Curve = locLine;
                //Methods.CreateDirectShape(this.FI.Document, new List<GeometryObject>() { locLine });
            }
            base.executeChange();
            calcOffset();
        }
        public override void PostProcess()
        {
            SetOrientation(this.FI, this.hand, this.facing);
            HostUtils.CutUncutHost(this.FI, base.cutHost);
            base.CutAndJoin();
            this.OperationFinished = true;
        }

        public FamilyCreator GetSimilarCreator()
        {
            var info = new LineBasedFamilyInfo(this.FI);
            if (info.HostCondition == HostCondition.Level)
            {
                var level = this.FI.Host as Level;
                this.SimilarCreator = new LineBasedFamilyCreator_onLevel
                    (this.FI.Symbol, this.OffsetLocLine, this.facing, level, this.FI.StructuralType, this.paramValues, this.Context);
            }
            else
            {
                if (this.FI.HostFace != null
                    && this.FI.HostFace.ElementReferenceType != ElementReferenceType.REFERENCE_TYPE_NONE)
                {
                    this.SimilarCreator = new LineBasedFamilyCreator_onPlanarFace
                        (this.FI.Symbol, this.OffsetLocLine, this.facing, this.FI.HostFace, this.paramValues, this.Context);
                }
                else
                {
                    //search for host face on the run
                    var locLine = this.FI.LocationLine();
                    var hostCat = HostUtils.GetHostCategory(this.FI);
                    var bbFilter = Methods.GetBBIntersectFilter
                        (this.FI.GetBoundingBoxInModelCS(null),
                        HostSearchSession.HostSearchRange);
                    var hostDir = HostUtils.GetHostDir(this.FI);
                    var faceNorm = this.FI.GetPlane().Normal;
                    var dop = this.FI.DesignOption;
                    var search = new HostSearchSession
                        (this.FI.Document, hostCat, bbFilter, hostDir, faceNorm, dop);
                    Element host;
                    var hostFace = search.FindHostFace(locLine, out host);
                    if (hostFace == null)
                        throw new goa.Common.Exceptions.HostNotFoundException(this.FI.Id.ToString());

                    this.SimilarCreator = new LineBasedFamilyCreator_onPlanarFace
                        (this.FI.Symbol, this.OffsetLocLine, this.facing, host, hostFace, this.paramValues, this.Context);
                }
                Methods.GetCutsAndJoins
                    (this.FI,
                    out this.SimilarCreator.Cuts,
                    out this.SimilarCreator.CutBy,
                    out this.SimilarCreator.Joins);
            }
            return this.SimilarCreator;
        }
        private Line getLocLine()
        {
            var doc = this.FI.Document;
            var plane = this.FI.GetPlane();
            //project back to base plane
            var unProjected = this.OffsetLocLine.ProjectOntoPlane(plane);
            //un-extend both ends
            var startExtPKey = FirmStandards.SuperFacadeFamParam[12].ToString();
            var endExtPKey = FirmStandards.SuperFacadeFamParam[13].ToString();
            double extStart = this.paramValues.ContainsKey(startExtPKey)
                ? (double)this.paramValues[startExtPKey].Value
                : 0.0;
            double extEnd = this.paramValues.ContainsKey(endExtPKey)
                ? (double)this.paramValues[endExtPKey].Value
                : 0.0;
            var unExtended = unProjected.ExtendOneEndByDist(extStart, 0);
            unExtended = unExtended.ExtendOneEndByDist(-1.0 * extEnd, 1);
            return unExtended;
        }
        private void calcOffset()
        {
            if (this.OffsetLocLine == null)
                return;
            var norm = HostUtils.GetHostNormal(this.FI);
            var p0 = this.FI.LocationCurve().GetEndPoint(0);
            var pOffset = this.OffsetLocLine.GetEndPoint(0);
            var totalOffset = (pOffset - p0).DotProduct(norm);
            double symbolOffset = LineBasedFamilyUtils.GetSymbolOffset(this.FS);
            double profileOffset = LineBasedFamilyUtils.GetProfileOffset(this.FS);
            double instanceOffset = totalOffset - symbolOffset - profileOffset;
            this.FI.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(instanceOffset);
        }
    }
}
