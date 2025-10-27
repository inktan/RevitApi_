using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
using goa.Common.Exceptions;
//using NetTopologySuite.Geometries;

namespace DimensioningTools
{
    internal class ConvertMultiViews : RequestMethod
    {
        internal ConvertMultiViews(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            var doc = this.doc;
            var allViews = getAllValidViewsInDoc(doc).Select(x => new ElementInfoForUI(x)).ToList();
            foreach (var view in allViews)
            {
                var elemType = doc.GetElement(view.Elem.GetTypeId());
                var viewType = elemType as ElementType;
                view.DisplayName = viewType.FamilyName + ":" + view.Name;
            }
            allViews = allViews.OrderBy(x => x.DisplayName).ToList();
            var form = new Form_MultiElementSelection(allViews, new List<ElementId>(), "选择视图");
            var result = form.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
                return;
            var selectedViews = form.SelectedElements.Cast<View>().ToList();
            if (selectedViews.Count == 0)
                return;
            var fa = findFamilyInDoc(doc);
            //convert
            using (TransactionGroup tg = new TransactionGroup(doc, "转换尺寸标注"))
            {
                tg.Start();
                foreach (var view in selectedViews)
                {
                    convertOneView(view, fa);
                }
                tg.Assimilate();
            }
        }
        private void convertOneView(View _view, Family _fa)
        {
            //find dims inside view
            var doc = _view.Document;
            var dimCtrls = new FilteredElementCollector(doc, _view.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(Dimension))
                .Where(x => LinearDimensionSelectionFilter.IsLinearDimensionElem(x))
                .Cast<Dimension>()
                .Select(x => new DimCtrl(x))
                .ToList();
            convertDims(_view, _fa, dimCtrls);
        }
        internal static void convertDims(View _view, Family _fa, IEnumerable<DimCtrl> _dimCtrls)
        {
            var doc = _view.Document;

            using (Transaction trans = new Transaction(doc, "convert"))
            {
                trans.Start();
                //convert
                foreach (var ctrl in _dimCtrls)
                {
                    ctrl.ChangeToFamilyInstance(_fa, _view);
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "set text position"))
            {
                trans.Start();
                foreach (var ctrl in _dimCtrls)
                {
                    ctrl.SetTextToCenter();
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "delete original"))
            {
                trans.Start();
                foreach (var ctrl in _dimCtrls)
                {
                    ctrl.DeleteOriginalDim();
                }
                trans.Commit();
            }
        }
        internal static Family findFamilyInDoc(Document doc)
        {
            var family = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .FirstOrDefault(x => x.Name.Contains("详图-尺寸标注-毫米"));
            if (family == null)
            {
                throw new CommonUserExceptions
                    ("找不到以下族类型:"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "详图-尺寸标注-毫米");
            }

            return family as Family;
        }
        private List<View> getAllValidViewsInDoc(Document _doc)
        {
            var views = new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(x => x.IsTemplate == false
                && supportedViewTypes.Contains(x.ViewType))
                .ToList();
            return views;
        }

        private static readonly HashSet<ViewType> supportedViewTypes
            = new HashSet<ViewType>(new List<ViewType>()
            {
                ViewType.AreaPlan,
                ViewType.CeilingPlan,
                ViewType.Detail,
                ViewType.Elevation,
                ViewType.FloorPlan,
                ViewType.Section,
            });
    }
}
