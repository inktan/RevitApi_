using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class BasicWallCreator : ElementCreator
    {
        public string DesignOptionUid;
        public Wall NewWall { get { return base.Elem as Wall; } }
        protected WallType wt;
        protected Level baseLevel;
        protected Curve offsetLocCurve;
        protected double height;
        protected bool structural;
        public XYZ Orientation;
        protected bool[] joinEnd;
        protected Dictionary<string, ParameterEditRecorder> paramValues;

        public BasicWallCreator
            (
            WallType _wt,
            Level _baseLevel,
            Curve _offsetLocCurve,
            double _height,
            XYZ _orientation,
            bool _structural,
            bool[] _joinEnd,
            Dictionary<string, ParameterEditRecorder> _param,
            string _designOptionUid = null,
            OpContext _docOpsInfo = null)
        {
            this.wt = _wt;
            this.baseLevel = _baseLevel;
            this.offsetLocCurve = _offsetLocCurve;
            this.height = _height;
            this.Orientation = _orientation;
            this.structural = _structural;
            this.joinEnd = _joinEnd;
            this.paramValues = _param;
            this.DesignOptionUid = _designOptionUid;
            this.Context = _docOpsInfo;
        }
        public override void Execute()
        {
            var doc = this.wt.Document;
            var baseOffset = this.offsetLocCurve.GetEndPoint(0).Z - this.baseLevel.ProjectElevation;
            var tl = new XYZ(0, 0, baseOffset);
            var tf = Transform.CreateTranslation(tl);
            var locCurve = this.offsetLocCurve.CreateTransformed(tf);
            base.Elem = Wall.Create
                (doc, locCurve, this.wt.Id, this.baseLevel.Id, this.height, baseOffset, false, this.structural);
            if (this.NewWall != null && this.NewWall.IsValidObject)
            {
                BasicWallEditor.SetParams(this.NewWall, this.paramValues);
                BasicWallEditor.SetJoinEnd(this.NewWall, this.joinEnd);
            }
        }
        public override void PostProcess()
        {
            if (this.NewWall != null && this.NewWall.IsValidObject)
            {
                BasicWallEditor.SetOrientation(this.NewWall, this.Orientation);
            }
        }
    }
}
