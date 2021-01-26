using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace goa.Common
{
    /// <summary>
    /// face-based family can be craeted on face, or on a work plane, such as level, grid and reference plane.
    /// </summary>
    public class FaceBasedFamilyCreator : FamilyCreator
    {
        protected Element host;
        protected Face HostFace;
        protected Level level;
        protected Reference HostFaceRef;
        public XYZ Pos;
        protected bool flipFacing;
        public FaceBasedFamilyCreator
            (FamilySymbol _fs,
            Reference _hostFaceRef,
            XYZ _pos,
            XYZ _hand,
            XYZ _facing,
            Dictionary<string, ParameterEditRecorder> _params,
            bool _flipFacing,
            OpContext _opContext)
            : base(_fs, _params, _opContext)
        {
            this.HostFaceRef = _hostFaceRef;
            this.Pos = _pos;
            this.Hand = _hand;
            this.Facing = _facing;
            this.flipFacing = _flipFacing;
        }
        public FaceBasedFamilyCreator
            (FamilySymbol _fs,
            Element _host,
            Face _hostFace,
            XYZ _pos,
            XYZ _hand,
            XYZ _facing,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _docOpsInfo)
            : base(_fs, _params, _docOpsInfo)
        {
            this.host = _host;
            this.HostFace = _hostFace;
            this.Pos = _pos;
            this.Hand = _hand;
            this.Facing = _facing;
        }
        /// <summary>
        /// need OPEN TRANSACTION
        /// </summary>
        public override void Execute()
        {
            var doc = this.fs.Document;
            if (this.HostFaceRef != null)
            {
                base.Elem = doc.Create.NewFamilyInstance(this.HostFaceRef, this.Pos, this.Hand, this.fs);
            }
            else if (this.HostFace != null)
            {
                base.Elem = doc.Create.NewFamilyInstance(this.HostFace, this.Pos, this.Hand, this.fs);
            }
            else
            {
                ThrowHostNotFound();
            }
            if (this.flipFacing)
                this.NewFI.flipFacing();
            FamilyEditor.SetParams(NewFI, this.paramValues);
        }
        public override void PostProcess()
        {
            if (this.NewFI != null && this.NewFI.IsValidObject)
            {
                FamilyEditor.SetOrientation(this.NewFI, this.Hand, this.Facing);
                this.OperationFinished = true;
            }
        }
        public static FaceBasedFamilyCreator CreateSimilar(FamilyInstance _fi, Document _tarDoc, double _hostSearchRange)
        {
            var refDoc = _fi.Document;
            var pos = _fi.GetPos();
            var hostCat = HostUtils.GetHostCategory(_fi);
            var bb = _fi.GetBoundingBoxInModelCS(null);
            var bbFilter = Methods.GetBBIntersectFilter(bb, _hostSearchRange);
            XYZ faceNorm = null;
            DesignOption dop; string dopUid, dopName;
            OpContext.GetActiveDesignOptionInfo(_tarDoc, out dop, out dopUid, out dopName);
            var hostDir = HostUtils.GetHostDir(_fi);
            var search = new HostSearchSession(_tarDoc, hostCat, bbFilter, hostDir, faceNorm, dop);
            Element host;
            var hostFace = search.FindHostFace(pos, out host);
            var param = _fi.GetAllEditableParams();
            var refDocId = refDoc.Identifier();
            var tarDocId = _tarDoc.Identifier();

            var opContext = new OpContext(refDocId, tarDocId, refDoc, _tarDoc, dopUid, dopName, _fi.Id);
            var ctr = new FaceBasedFamilyCreator
                (_fi.Symbol, _fi.Host, hostFace, pos, _fi.HandOrientation, _fi.FacingOrientation, param, opContext);
            return ctr;
        }
    }
}
