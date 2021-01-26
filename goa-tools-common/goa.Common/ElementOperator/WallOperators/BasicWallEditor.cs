using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class BasicWallEditor : ElementEditor
    {
        public Wall Wall;
        protected WallType WT;
        protected XYZ orientation;
        protected XYZ translation;
        protected Curve newOffsetLocCurve;
        protected bool[] joinEnd;
        protected double height;
        public Dictionary<string, ParameterEditRecorder> paramValues;

        private bool needDisjoinEnds = false;

        public BasicWallEditor
            (Wall _wall,
            WallType _wt,
            Curve _offsetLocCurve,
            double _height,
            XYZ _orientation,
            bool _needDisjoinEnds,
            bool[] _joinEnd,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _docOpsInfo)
        {
            this.Wall = _wall;
            this.WT = _wt;
            this.newOffsetLocCurve = _offsetLocCurve;
            this.orientation = _orientation;
            this.height = _height;
            this.paramValues = _params == null
                ? new Dictionary<string, ParameterEditRecorder>()
                : _params;
            this.needDisjoinEnds = _needDisjoinEnds;
            this.joinEnd = _joinEnd == null
                ? getWallJoinEndStates(_wall)
                : _joinEnd;
            this.Context = _docOpsInfo;
        }
        public override void PreProcess()
        {
            if (needDisjoinEnds)
            {
                WallUtils.DisallowWallJoinAtEnd(this.Wall, 0);
                WallUtils.DisallowWallJoinAtEnd(this.Wall, 1);
            }
        }
        public override void Execute()
        {
            if (this.WT != null)
                this.Wall.WallType = this.WT;
            if (this.newOffsetLocCurve != null)
            {
                editByOffsetCurve();
            }
            if (double.IsNaN(this.height) == false)
            {
                adjustHeight();
            }
            SetParams(this.Wall, this.paramValues);
        }
        public override void PostProcess()
        {
            SetOrientation(this.Wall, this.orientation);
            if (needDisjoinEnds)
                SetJoinEnd(this.Wall, this.joinEnd);
            this.OperationFinished = true;
        }
        public static void SetOrientation(Wall _wall, XYZ _orientation)
        {
            if (_orientation != null
                && _wall.Orientation.IsAlmostEqualToByDifference(_orientation, 0.0001) == false)
            {
                _wall.Flip();
            }
        }
        public static void SetParams(Wall _wall, Dictionary<string, ParameterEditRecorder> _params)
        {
            if (_params == null)
                return;
            foreach (var key in _params.Keys)
            {
                var p = _wall.GetParameterByUniqueIdOrByName(key);
                if (p != null)
                {
                    var refP = _params[key];
                    if (refP.StorageType == p.StorageType)
                        p.SetValue(refP.Value);
                }
            }
        }
        public static void SetJoinEnd(Wall _wall, bool[] _joinEnd)
        {
            if (_joinEnd[0])
                WallUtils.AllowWallJoinAtEnd(_wall, 0);
            else
                WallUtils.DisallowWallJoinAtEnd(_wall, 0);
            if (_joinEnd[1])
                WallUtils.AllowWallJoinAtEnd(_wall, 1);
            else
                WallUtils.DisallowWallJoinAtEnd(_wall, 1);
        }
        private void editByOffsetCurve()
        {
            var doc = this.Wall.Document;
            var locCurve = this.Wall.LocationCurve();
            var pos = locCurve.GetEndPoint(0);
            var offsetPos = this.newOffsetLocCurve.GetEndPoint(0);
            var deltaZ = (offsetPos - pos).Z;
            var p = this.Wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
            this.paramValues[BuiltInParameter.WALL_BASE_OFFSET.GetId()] = new ParameterEditRecorder(p, deltaZ);
            XYZ tl = null;
            if (this.newOffsetLocCurve.Translational(locCurve, ref tl))
            {
                tl = new XYZ(tl.X, tl.Y, 0.0);
                ElementTransformUtils.MoveElement(doc, this.Wall.Id, tl);
            }
            else
            {
                tl = new XYZ(0, 0, deltaZ * -1.0);
                var tf = Transform.CreateTranslation(tl);
                var newLocCurve = this.newOffsetLocCurve.CreateTransformed(tf);
                ((LocationCurve)this.Wall.Location).Curve = newLocCurve;
            }
        }
        private void adjustHeight()
        {
            var topOffsetP = this.Wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET);
            if (topOffsetP.IsReadOnly)
            {
                var heightP = this.Wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                heightP.Set(this.height);
            }
            else
            {
                var offsetPos = this.newOffsetLocCurve.GetEndPoint(0);
                var tarZ = offsetPos.Z + this.height;
                var topLevelId = this.Wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
                var doc = this.Wall.Document;
                var topLevel = doc.GetElement(topLevelId) as Level;
                var topOffset = tarZ - topLevel.ProjectElevation;
                topOffsetP.Set(topOffset);
            }
        }
        private bool[] getWallJoinEndStates(Wall _wall)
        {
            bool b0 = WallUtils.IsWallJoinAllowedAtEnd(_wall, 0);
            bool b1 = WallUtils.IsWallJoinAllowedAtEnd(_wall, 1);
            return new bool[2] { b0, b1 };
        }
    }
}
