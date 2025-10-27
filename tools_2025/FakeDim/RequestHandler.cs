using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using goa.Common.Exceptions;

namespace FAKE_DIMS
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName() { return "Overlapping Elements Clean Up Request Handler"; }
        public void Execute(UIApplication app)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.test1:
                        {
                            test1(app.ActiveUIDocument);
                            break;
                        }
                    case RequestId.ConvertSingleView:
                        {
                            ConvertSingleView(app.ActiveUIDocument);
                            break;
                        }
                    case RequestId.ConvertMultiViews:
                        {
                            ConvertMultiViews(app.ActiveUIDocument);
                            break;
                        }
                    case RequestId.ConvertSelected:
                        {
                            ConvertSelected(app.ActiveUIDocument);
                            break;
                        }
                    case RequestId.Select:
                        {
                            multiSelect(app.ActiveUIDocument);
                            break;
                        }
                    case RequestId.Check:
                        {
                            check(app.ActiveUIDocument);
                            break;
                        }
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //ignore
            }
            catch (goa.Common.Exceptions.IgnorableException ex)
            {
                //ignore
            }
            //catch (goa.Common.Exceptions.CommonUserExceptions ex)
            //{
            //    UserMessages.ShowMessage(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    UserMessages.ShowErrorMessage(ex, window);
            //}
            finally
            {
                //AssemblyLoader.Register(false);
                //Form_cursorPrompt.Stop();
                window.WakeUp();
                window.Activate();
            }
        }

        private void test1(UIDocument _uidoc)
        {
            var doc = _uidoc.Document;
            var sel = _uidoc.Selection;
            var pickRef = sel.PickObject(ObjectType.Element);
            var dim = doc.GetElement(pickRef) as Dimension;
            var dimCtrl = new DimCtrl(dim);
            using (Transaction trans = new Transaction(doc, "test"))
            {
                trans.Start();
                var geom = dimCtrl.TEST_GetPos();
                var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(geom);
                trans.Commit();
            }
        }

        private void ConvertSingleView(UIDocument _uidoc)
        {
            var doc = _uidoc.Document;
            var sel = _uidoc.Selection;
            //get fake dim family symbol
            var fa = findFamilyInDoc(doc);
            //pick real dimensions
            var filter = new LinearDimensionSelectionFilter();
            IList<Reference> pickRefs;
            try
            {
                Form_cursorPrompt.Start("选择尺寸标注", APP.MainWindow);
                pickRefs = sel.PickObjects(ObjectType.Element, filter);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                Form_cursorPrompt.Stop();
                return;
            }
            var dimCtrls = pickRefs
                .Select(x => doc.GetElement(x) as Dimension)
                .Select(x => new DimCtrl(x)).ToList();

            //convert
            var view = _uidoc.ActiveView;
            using (TransactionGroup tg = new TransactionGroup(doc, "转换尺寸标注"))
            {
                tg.Start();
                convertDims(view, fa, dimCtrls);
                tg.Assimilate();
            }
        }

        private void ConvertMultiViews(UIDocument _uidoc)
        {
            var doc = _uidoc.Document;
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

        private void ConvertSelected(UIDocument _uidoc)
        {
            var doc = _uidoc.Document;
            var fa = findFamilyInDoc(doc);
            var sel = _uidoc.Selection;
            var filter = new LinearDimensionSelectionFilter();
            string m = "选择需要转换成假标注的标注。点【完成】";
            Form_cursorPrompt.Start(m, null);
            var picks = sel.PickObjects(ObjectType.Element, filter);
            Form_cursorPrompt.Stop();
            var dims = picks
                .Select(x => doc.GetElement(x) as Dimension);
            var view = doc.GetElement(dims.First().OwnerViewId) as View;
            var ctrls = dims.Select(x => new DimCtrl(x));
            using (TransactionGroup tg = new TransactionGroup(doc, "转换假标注"))
            {
                tg.Start();
                convertDims(view, fa, ctrls);
                tg.Assimilate();
            }
        }

        /// <summary>
        /// TRANSACTIONS INSIDE
        /// </summary>
        /// <param name="_view"></param>
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

        /// <summary>
        /// transactions inside.
        /// </summary>
        /// <param name="_view"></param>
        /// <param name="_dimCtrls"></param>
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

        private void check(UIDocument _uidoc)
        {
            var doc = _uidoc.Document;
            var view = _uidoc.ActiveView;
            if (!validViewType(view))
            {
                TaskDialog.Show("消息", "当前视图类型无效。");
                return;
            }
            // test: get all geom from custom exporter
            // show as directshape
            var context = new ExportContext();
            var exporter = new CustomExporter(doc, context);
            exporter.IncludeGeometricObjects = true;
            exporter.Export2DIncludingAnnotationObjects = true;
            exporter.ShouldStopOnError = false;
            exporter.Export(view);
            var list = new List<GeometryObject>();
            list.AddRange(context.PlanarFaces);
            list.AddRange(context.Lines);
            //debug
            TaskDialog.Show("test", list.Count.ToString());
            using (Transaction trans = new Transaction(doc, "test"))
            {
                trans.Start();
                DirectShape ds = DirectShape.CreateElement
                    (doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(list);
                trans.Commit();
            }
        }

        private bool validViewType(View _view)
        {
            var vt = _view.ViewType;
            if (vt == ViewType.FloorPlan
                || vt == ViewType.CeilingPlan
                || vt == ViewType.Elevation
                || vt == ViewType.DraftingView
                || vt == ViewType.AreaPlan
                || vt == ViewType.Section
                || vt == ViewType.Detail)
                return true;
            else
                return false;
        }

        private bool validElemTypeToCheck(Element _elem)
        {
            var cat = (BuiltInCategory)_elem.Category.Id.IntegerValue;
            if (categories.Contains(cat) == false)
                return false;
            else
                return true;
        }

        private static readonly List<BuiltInCategory> categories
            = new List<BuiltInCategory>()
        {
            //3d
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFoundation,
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_StructuralFramingSystem,
            BuiltInCategory.OST_StructuralTruss,
            BuiltInCategory.OST_StructuralStiffener,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Stairs,
            BuiltInCategory.OST_GenericModel,
            //2d
            BuiltInCategory.OST_DetailComponents,
            BuiltInCategory.OST_Lines,
        };

        private void multiSelect(UIDocument _uidoc)
        {
            var sel = _uidoc.Selection;
            var filter = new FakeDimensionFamilyInstanceSelectionFilter();
            var elems = new List<Element>();
            try
            {
                Form_cursorPrompt.Start("按下左键框选多个假尺寸。", APP.MainWindow);
                elems = sel.PickElementsByRectangle(filter).ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                return;
            }
            var doc = _uidoc.Document;
            var ids = elems.Select(x => x.Id).ToList();
            sel.SetElementIds(ids);
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

        internal static Family findFamilyInDoc(Document doc)
        {
            var family = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .FirstOrDefault(x => x.Name.Contains("A-DIM-LINEAR-FAKE"));
            if (family == null)
            {
                throw new CommonUserExceptions
                    ("找不到以下族类型:"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "A-DIM-LINEAR-FAKE");
            }

            return family as Family;
        }
    }
}
