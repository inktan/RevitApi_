using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class PointBasedFamilyEditor : FamilyEditor
    {
        protected XYZ translation;

        public PointBasedFamilyEditor(
            FamilyInstance _fi,
            XYZ _translation,
            FamilySymbol _fs,
            XYZ _hand,
            XYZ _facing,
            bool _cutHost,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
            : base(_cutHost)
        {
            this.FI = _fi;
            this.translation = _translation;
            this.FS = _fs;
            this.hand = _hand;
            this.facing = _facing;
            this.paramValues = _params;
            this.Context = _opContext;
        }

        public override void Execute()
        {
            if (this.translation != null)
                ElementTransformUtils.MoveElement(this.FI.Document, this.FI.Id, this.translation);

            base.executeChange();
        }
        public override void PostProcess()
        {
            SetOrientation(this.FI, this.hand, this.facing);
            HostUtils.CutUncutHost(this.FI, base.cutHost);
            this.OperationFinished = true;
        }
    }
}
