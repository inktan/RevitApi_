using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace goa.Common
{
    public class FamilyCreator : ElementCreator
    {
        public static List<Element> HostsWithInstanceFaceProblem = new List<Element>();
        public FamilyInstance NewFI { get { return base.Elem as FamilyInstance; } }
        public int HostLevel;
        protected FamilySymbol fs;
        public XYZ Hand, Facing;
        protected Dictionary<string, ParameterEditRecorder> paramValues;
        public FamilyCreator
            (FamilySymbol _fs,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
        {
            this.fs = _fs;
            this.paramValues = _params;
            this.Context = _opContext;
        }
        public override void PreProcess()
        {
            checkFSDoc();
            activateFS(this.fs);
            activeDesignOption();
        }
        public static FamilyCreator CreateSimilar(FamilyInstance _fi, Document _tarDoc, int _hostLevel)
        {
            var type = HostUtils.GetHostingType(_fi);
            if (type == HostingType.FaceBased)
            {
                return FaceBasedFamilyCreator.CreateSimilar(_fi, _tarDoc, _hostLevel);
            }
            else if (type == HostingType.LineBasedOnLevel)
            {
                return LineBasedFamilyCreator_onLevel.CreateSimilar(_fi, _tarDoc, _hostLevel);
            }
            else if (type == HostingType.LineBasedOnFace)
            {
                return LineBasedFamilyCreator_onPlanarFace.CreateSimilar(_fi, _tarDoc, _hostLevel);
            }
            else if (type == HostingType.WallBased)
            {
                return WallBasedFamilyCreator.CreateSimilar(_fi, _tarDoc, _hostLevel);
            }
            else
            {
                return null;
            }
        }
        protected static Dictionary<string, ParameterEditRecorder> getAllModifiableParams(FamilyInstance _fi)
        {
            var dic = new Dictionary<string, ParameterEditRecorder>();
            foreach (Parameter p in _fi.ParametersMap)
            {
                if (p.IsReadOnly || p.UserModifiable == false)
                    continue;
                else
                    dic[p.GetId()] = new ParameterEditRecorder(p, p.GetValue());
            }
            return dic;
        }
        protected void checkFSDoc()
        {
            if (this.Context == null
                || this.RefDocId == this.TarDocId)
                return;
            //try find a symbol of same name
            var tarFS = new FilteredElementCollector(this.TarDoc)
                .OfClass(typeof(FamilySymbol))
                .OfCategoryId(this.fs.Category.Id)
                .Where(x => x.Name == this.fs.Name)
                .Cast<FamilySymbol>()
                .FirstOrDefault(x => x.Family.Name == this.fs.Family.Name);
            if (tarFS != null)
            {
                this.fs = tarFS as FamilySymbol;
                return;
            }
            else
            {
                //if not found, copy from ref doc
                var newIds = ElementTransformUtils.CopyElements
                    (this.fs.Document,
                    new List<ElementId>() { this.fs.Id },
                    this.TarDoc,
                    null,
                    new CopyPasteOptions());
                this.TarDoc.Regenerate();
                this.fs = TarDoc.GetElement(newIds.First()) as FamilySymbol;
            }
        }
        protected void activeDesignOption()
        {
            if (this.Context == null)
                return;
            this.Cancel = false;
            var doc = this.fs.Document;
            var activeDopId = DesignOption.GetActiveDesignOptionId(doc);
            bool active = true;
            if (activeDopId == null || activeDopId == ElementId.InvalidElementId)
            {
                active = this.Context.DesignOptionId == null;
            }
            else
            {
                var activeDop = doc.GetElement(activeDopId);
                active = activeDop.UniqueId == this.Context.DesignOptionId;
            }
            if (!active)
            {
                this.Cancel = true;
                this.CancelCause = OperationCancelledCause.InactiveDesignOption;
            }
        }
        private void activateFS(FamilySymbol _fs)
        {
            if (_fs.IsActive == false)
            {
                _fs.Activate();
                _fs.Document.Regenerate();
            }
        }
    }
}
