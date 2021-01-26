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
    public class LineBasedFamilyCreator_onPlanarFace : FamilyCreator
    {
        public Line OffsetLocLine;
        public bool CutsHost;

        protected Element Host;
        protected PlanarFace hostFace;
        protected Reference hostFaceRef;
        protected string referenceString;

        protected XYZ facingRefPoint;
        protected bool downwardFacing;

        /// <summary>
        /// create new instance along a line, on face.
        /// </summary>
        /// <param name="_cutHost">need to cut host?</param>
        /// <param name="_facingRefPoint">adjust facing</param>
        /// <param name="faceOppositeDir">adjust facing to face the opposite direction of ref point?</param>
        public LineBasedFamilyCreator_onPlanarFace
            (FamilySymbol _fs,
            Line _offsetLocLine,
            Reference _hostFaceRef,
            XYZ _facingRefPoint,
            bool faceOppositeDir)
            : base(_fs, null, null)
        {
            this.OffsetLocLine = _offsetLocLine;
            this.hostFaceRef = _hostFaceRef;
            this.facingRefPoint = _facingRefPoint;
            this.downwardFacing = faceOppositeDir;
        }
        /// <summary>
        /// create new instance along a line, on face.
        /// </summary>
        /// <param name="_cutHost">need to cut host?</param>
        /// <param name="_facingRefPoint">adjust facing</param>
        /// <param name="faceOppositeDir">adjust facing to face the opposite direction of ref point?</param>
        public LineBasedFamilyCreator_onPlanarFace
            (FamilySymbol _fs,
            Line _offsetLocLine,
            Element _host,
            PlanarFace _hostFace,
            XYZ _facingRefPoint,
            bool faceOppositeDir)
            : base(_fs, null, null)
        {
            this.OffsetLocLine = _offsetLocLine;
            this.Host = _host;
            this.hostFace = _hostFace;
            this.facingRefPoint = _facingRefPoint;
            this.downwardFacing = faceOppositeDir;
            this.paramValues = new Dictionary<string, ParameterEditRecorder>();
        }
        /// <summary>
        /// create new instance on host face, with a face reference as input.
        /// </summary>
        public LineBasedFamilyCreator_onPlanarFace
            (FamilySymbol _fs,
            Line _offsetLocLine,
            XYZ _facing,
            Reference _hostFaceRef,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
            : base(_fs, _params, _opContext)
        {
            this.OffsetLocLine = _offsetLocLine;
            this.Facing = _facing;
            this.hostFaceRef = _hostFaceRef;
            this.referenceString = _hostFaceRef.ConvertToStableRepresentation(_fs.Document);
        }
        /// <summary>
        /// create new instance on host face.
        /// </summary>
        public LineBasedFamilyCreator_onPlanarFace
            (FamilySymbol _fs,
            Line _offsetLocLine,
            XYZ _facing,
            Element _host,
            PlanarFace _hostFace,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
            : base(_fs, _params, _opContext)
        {
            this.OffsetLocLine = _offsetLocLine;
            this.Host = _host;
            this.hostFace = _hostFace;
            this.Facing = _facing;
        }

        /// <summary>
        /// need OPEN TRANSACTION
        /// </summary>
        public override void Execute()
        {
            var doc = this.fs.Document;
            if (this.hostFace != null
                && this.hostFace.Reference != null)
                this.referenceString = this.hostFace.Reference.ConvertToStableRepresentation(doc);
            //project offset line onto host face
            if (this.hostFaceRef == null && this.hostFace == null)
                ThrowHostNotFound();
            var locLine = getLocLine();

            if (this.hostFaceRef != null)
                base.Elem = doc.Create.NewFamilyInstance(this.hostFaceRef, locLine, this.fs);
            else if (this.referenceString != null)
            {
                var hostFaceRef = Reference.ParseFromStableRepresentation(doc, this.referenceString);
                base.Elem = doc.Create.NewFamilyInstance(hostFaceRef, locLine, this.fs);
            }
            else if (this.hostFace != null)
            {
                try
                {
                    base.Elem = doc.Create.NewFamilyInstance(this.hostFace, locLine, this.fs);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException ex)
                {
                    FamilyCreator.HostsWithInstanceFaceProblem.Add(this.Host);
                }
                catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
                {
                    FamilyCreator.HostsWithInstanceFaceProblem.Add(this.Host);
                }
            }

            if (this.NewFI == null)
                return;
            FamilyEditor.SetParams(this.NewFI, this.paramValues);
            //offset from host
            calcOffset();
        }
        /// <summary>
        /// after creation, need to regen doc to get 
        /// corrent orientation info.
        /// </summary>
        public override void PostProcess()
        {
            if (this.NewFI != null && this.NewFI.IsValidObject)
            {
                if (facingRefPoint != null)
                    adjustFacingByRefPoint(this.NewFI);
                FamilyEditor.SetOrientation(this.NewFI, this.Hand, this.Facing);
                //cuts needs to be inside post process
                //otherwise face change might affect creation of other new instances.
                HostUtils.CutUncutHost(this.NewFI, this.CutsHost);
                base.CutAndJoin();
                this.OperationFinished = true;
            }
        }
        public static LineBasedFamilyCreator_onPlanarFace CreateSimilar(FamilyInstance _fi, Document _tarDoc, double _hostSearchRange)
        {
            var refDoc = _fi.Document;
            var refDocId = refDoc.Identifier();
            var tarDocId = _tarDoc.Identifier();
            var locLine = _fi.LocationLine();
            var offsetLocLine = LineBasedFamilyUtils.GetOffsetLocLine(_fi);
            var param = _fi.GetAllEditableParams();
            DesignOption dop; string dopUid, dopName;
            OpContext.GetActiveDesignOptionInfo(_tarDoc, out dop, out dopUid, out dopName);
            var opContext = new OpContext(refDocId, tarDocId, refDoc, _tarDoc, dopUid, dopName, _fi.Id);
            var hostCat = HostUtils.GetHostCategory(_fi);
            var hostDir = HostUtils.GetHostDir(_fi);
            var faceNorm = _fi.GetPlane().Normal;
            var bb = _fi.GetBoundingBoxInModelCS(null);
            var bbFilter = Methods.GetBBIntersectFilter(bb, _hostSearchRange);
            var search = new HostSearchSession(_tarDoc, hostCat, bbFilter, hostDir, faceNorm, dop);
            Element host;
            var hostFace = search.FindHostFace(locLine, out host);
            var ctr = new LineBasedFamilyCreator_onPlanarFace
                (_fi.Symbol, offsetLocLine, _fi.FacingOrientation, host, hostFace, param, opContext);
            Methods.GetCutsAndJoins(_fi, out ctr.Cuts, out ctr.CutBy, out ctr.Joins);
            ctr.CutsHost = HostUtils.CutsHost(_fi);
            return ctr;
        }
        private Line getLocLine()
        {
            var doc = this.fs.Document;
            XYZ norm, origin;
            if (this.hostFace != null)
            {
                norm = this.hostFace.FaceNormal;
                origin = this.hostFace.Origin;
            }
            else
            {
                var elem = doc.GetElement(this.hostFaceRef);
                var pf = elem.GetGeometryObjectFromReference(this.hostFaceRef) as PlanarFace;
                norm = pf.FaceNormal;
                origin = pf.Origin;
                if (this.hostFaceRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                {
                    var ins = elem as Instance;
                    var tf = ins.GetTotalTransform();
                    norm = tf.OfVector(norm);
                    origin = tf.OfPoint(origin);
                }
            }
            var plane = Plane.CreateByNormalAndOrigin(norm, origin);
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
            var norm = HostUtils.GetHostNormal(this.NewFI);
            var p0 = this.NewFI.LocationCurve().GetEndPoint(0);
            var pOffset = this.OffsetLocLine.GetEndPoint(0);
            var totalOffset = (pOffset - p0).DotProduct(norm);
            double symbolOffset = LineBasedFamilyUtils.GetSymbolOffset(this.fs);
            double profileOffset = LineBasedFamilyUtils.GetProfileOffset(this.fs);
            double instanceOffset = totalOffset - symbolOffset - profileOffset;
            this.NewFI.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(instanceOffset);
        }
        private void adjustFacingByRefPoint(FamilyInstance _fi)
        {
            var locLine = _fi.LocationLine();
            var endP = locLine.GetEndPoint(0);
            var facing = _fi.FacingOrientation;
            if (this.downwardFacing)
                facing *= -1.0;
            var dot = (this.facingRefPoint - endP).DotProduct(facing);
            if (dot.IsAlmostEqualByDifference(0) == false
                && dot < 0.0)
            {
                _fi.flipFacing();
            }
        }
    }
}
