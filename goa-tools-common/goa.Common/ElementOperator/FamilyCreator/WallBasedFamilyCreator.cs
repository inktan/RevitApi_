using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace goa.Common
{
    public class WallBasedFamilyCreator : FamilyCreator
    {
        protected Wall hostWall;
        protected XYZ pos;
        protected StructuralType sType;
        public WallBasedFamilyCreator
            (FamilySymbol _fs,
            Wall _hostWall,
            XYZ _pos,
            XYZ _hand,
            XYZ _facing,
            StructuralType _sType,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
            : base(_fs, _params, _opContext)
        {
            this.hostWall = _hostWall;
            this.pos = _pos;
            this.Hand = _hand;
            this.Facing = _facing;
            this.sType = _sType;
        }
        public override void Execute()
        {
            var doc = this.fs.Document;
            if (this.hostWall != null)
            {
                var level = doc.GetElement(this.hostWall.LevelId) as Level;
                base.Elem = doc.Create.NewFamilyInstance(this.pos, this.fs, this.hostWall, level, this.sType);
                FamilyEditor.SetParams(this.NewFI, this.paramValues);
            }
            else
            {
                ThrowHostNotFound();
            }
        }
        /// <summary>
        /// after creation, need to regen doc to get 
        /// corrent orientation info.
        /// </summary>
        public override void PostProcess()
        {
            if (this.NewFI != null && this.NewFI.IsValidObject)
            {
                FamilyEditor.SetOrientation(this.NewFI, this.Hand, this.Facing);
                this.OperationFinished = true;
            }
        }
        public static WallBasedFamilyCreator CreateSimilar(FamilyInstance _fi, Document _tarDoc, double _hostSearchRange)
        {
            var refDoc = _fi.Document;
            var param = _fi.GetAllEditableParams();
            var refDocId = refDoc.Identifier();
            var tarDocId = _tarDoc.Identifier();
            DesignOption dop; string dopUid, dopName;
            OpContext.GetActiveDesignOptionInfo(_tarDoc, out dop, out dopUid, out dopName);
            var opContext = new OpContext(refDocId, tarDocId, refDoc, _tarDoc, dopUid, dopName, _fi.Id);
            var hostCat = HostUtils.GetHostCategory(_fi);
            var hostDir = HostUtils.GetHostDir(_fi);
            var bb = _fi.GetBoundingBoxInModelCS(null);
            var bbFilter = Methods.GetBBIntersectFilter(bb, _hostSearchRange);
            var posPoints = HostUtils.GetPointsInsideWallHost(_fi);
            var search = new HostSearchSession(_tarDoc, hostCat, bbFilter, hostDir, null, dop);
            var hostWall = search.FindHostWall(posPoints);
            var ctr = new WallBasedFamilyCreator
                (_fi.Symbol, hostWall, _fi.GetPos(), _fi.HandOrientation, _fi.FacingOrientation, _fi.StructuralType, param, opContext);
            return ctr;
        }
    }
}
