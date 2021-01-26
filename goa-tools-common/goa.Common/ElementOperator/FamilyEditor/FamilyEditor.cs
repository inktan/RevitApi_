using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class FamilyEditor : ElementEditor
    {
        public FamilyInstance FI
        {
            get { return base.Elem as FamilyInstance; }
            set { base.Elem = value; }
        }
        public Dictionary<string, ParameterEditRecorder> paramValues;

        protected FamilySymbol FS;
        protected XYZ hand, facing;
        protected bool cutHost;

        protected FamilyEditor(bool _cutHost)
        {
            this.cutHost = _cutHost;
        }

        protected virtual void executeChange()
        {
            if (this.FS != null)
                this.FI.Symbol = this.FS;
            SetParams(this.FI, this.paramValues);
        }
        public static void SetOrientation(FamilyInstance _fi, XYZ _hand, XYZ _facing)
        {
            if (_hand != null
                && _fi.HandOrientation.IsAlmostEqualToByDifference(_hand, 0.0001) == false)
            {
                _fi.flipHand();
            }
            if (_facing != null
                && _fi.FacingOrientation.IsAlmostEqualToByDifference(_facing, 0.0001) == false)
            {
                _fi.flipFacing();
            }
        }
        public static void SetParams(FamilyInstance _fi, Dictionary<string, ParameterEditRecorder> _params)
        {
            if (_params == null)
                return;
            foreach (var key in _params.Keys)
            {
                var p = _fi.GetParameterByUniqueIdOrByName(key);
                if (p != null)
                {
                    var refP = _params[key];
                    if (refP.StorageType == p.StorageType)
                    {
                        if (p.IsReadOnly && p.Definition.Name == "洞口宽度")
                        {
                            //for a WIP version of envi fam
                            p = _fi.GetParameters("实际洞口宽度").First();
                        }
                        p.SetValue(refP.Value);
                    }
                }
            }
        }
    }
}
