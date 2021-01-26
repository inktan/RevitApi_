using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace goa.Common
{
    public class LineBasedFamilyCreator_onLevel : FamilyCreator
    {
        public Line offsetLocLine;
        protected Level hostLevel;

        private DirectShape tempDS;
        private ViewPlan viewPlan;
        private bool tempViewPlan = false;
        private StructuralType sType;
        /// <summary>
        /// create new instance on level.
        /// </summary>
        public LineBasedFamilyCreator_onLevel
            (FamilySymbol _fs,
            Line _offsetLocLine,
            XYZ _facing,
            Level _hostLevel,
            StructuralType _sType,
            Dictionary<string, ParameterEditRecorder> _params,
            OpContext _opContext)
            : base(_fs, _params, _opContext)
        {
            this.offsetLocLine = _offsetLocLine;
            this.Facing = _facing;
            this.hostLevel = _hostLevel;
            this.sType = _sType;
        }

        /// <summary>
        /// Document.NewFamilyInstance(Curve, FamilySymbol, Level, StructuralType). 
        /// This method does not work with structural foundation category.
        /// Need to create temp solid geoemtry, copy to level afterward.
        /// </summary>
        public override void PreProcess()
        {
            base.PreProcess();
            if (this.Cancel)
                return;
            if (this.sType != StructuralType.Footing)
            {
                return;
            }
            else
            {
                var doc = this.fs.Document;
                //create temp geom 
                var z = this.offsetLocLine.GetEndPoint(0).Z;
                var basePlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, z));
                var solid = basePlane.GetSolidFromBasePlane();
                Methods.CreateDirectShape(doc, new List<GeometryObject>() { solid }, ref this.tempDS);
                //create temp view if needed
                var vp = findViewPlanOfLevel(doc, this.hostLevel.Id);
                if (vp == null)
                {
                    this.tempViewPlan = true;
                    vp = createViewsOfLevel(doc, this.hostLevel.Id);
                }
                this.viewPlan = vp;
            }
        }
        /// <summary>
        /// need OPEN TRANSACTION
        /// </summary>
        public override void Execute()
        {
            var doc = this.fs.Document;
            if (this.sType != StructuralType.Footing)
            {
                //create new on level
                var locLine = getLocLine();
                base.Elem = doc.Create.NewFamilyInstance(locLine, this.fs, this.hostLevel, this.sType);
            }
            else
            {
                //find face on temp geom
                PlanarFace pf = this.tempDS.GetAllSolids()
                    .SelectMany(s => s.Faces.Cast<PlanarFace>())
                    .Where(f => f != null)
                    .FirstOrDefault(f => f.FaceNormal.IsAlmostEqualToByDifference(XYZ.BasisZ, 0.0001));

                var tempFi = doc.Create.NewFamilyInstance(pf, this.offsetLocLine, fs);
                //copy to level
                var cpo = new CopyPasteOptions();
                var newCopies = ElementTransformUtils.CopyElements
                    (this.viewPlan,
                    new ElementId[1] { tempFi.Id },
                    this.viewPlan, null, cpo);
                base.Elem = doc.GetElement(newCopies.First()) as FamilyInstance;
                //delete temp elems
                doc.Delete(tempFi.Id);
                if (this.tempDS != null)
                    doc.Delete(this.tempDS.Id);
            }

            if (this.NewFI == null)
                return;
            FamilyEditor.SetParams(this.NewFI, this.paramValues);

            calcOffset();
        }
        /// <summary>
        /// after creation, need to regen doc to get 
        /// corrent orientation info.
        /// </summary>
        public override void PostProcess()
        {
            var doc = this.fs.Document;
            //delete temp views          
            //delete here in case other creators use the same view
            if (this.tempViewPlan && this.viewPlan.IsValidObject)
                doc.Delete(this.viewPlan.Id);
            if (this.NewFI != null && this.NewFI.IsValidObject)
            {
                FamilyEditor.SetOrientation(this.NewFI, this.Hand, this.Facing);
                this.OperationFinished = true;
            }
        }
        public static LineBasedFamilyCreator_onLevel CreateSimilar(FamilyInstance _fi, Document _tarDoc, int _hostLevel)
        {
            var hostLevel = _fi.Host as Level;
            var offsetLocLine = LineBasedFamilyUtils.GetOffsetLocLine(_fi);
            var param = _fi.GetAllEditableParams();
            var refDoc = _fi.Document;
            var refDocId = _fi.Document.Identifier();
            var tarDocId = _tarDoc.Identifier();
            DesignOption dop; string dopUid, dopName;
            OpContext.GetActiveDesignOptionInfo(_tarDoc, out dop, out dopUid, out dopName);
            var opContext = new OpContext(refDocId, tarDocId, refDoc, _tarDoc, dopUid, dopName, _fi.Id);
            var ctr = new LineBasedFamilyCreator_onLevel
                (_fi.Symbol, offsetLocLine, _fi.FacingOrientation, hostLevel, _fi.StructuralType, param, opContext);
            return ctr;
        }
        private Line getLocLine()
        {
            Plane p = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, this.hostLevel.ProjectElevation));
            return this.offsetLocLine.ProjectOntoPlane(p);
        }
        private void calcOffset()
        {
            var tarZ = this.offsetLocLine.GetEndPoint(0).Z;
            var offset = tarZ - this.hostLevel.ProjectElevation;
            NewFI.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(offset);
        }
        private static ViewPlan findViewPlanOfLevel(Document doc, ElementId _levelId)
        {
            var allPlanViews = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(x => x.IsTemplate == false && x.GenLevel != null);
            var dic = new Dictionary<ElementId, ViewPlan>();
            foreach (var vp in allPlanViews)
            {
                if (vp.GenLevel.Id == _levelId)
                    return vp;
            }
            return null;
        }
        /// <summary>
        /// need open transaction.
        /// </summary>
        private static ViewPlan createViewsOfLevel(Document doc, ElementId _levelId)
        {
            var viewType = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .Where(x => x.ViewFamily == ViewFamily.FloorPlan
                || x.ViewFamily == ViewFamily.AreaPlan)
                .First();
            return ViewPlan.Create(doc, viewType.Id, _levelId);
        }
    }
}
