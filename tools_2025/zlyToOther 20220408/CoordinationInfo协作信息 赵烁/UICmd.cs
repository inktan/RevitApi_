
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using goa.Common.Exceptions;

namespace CoordinationInfo
{
    internal static class UICmd
    {
        internal static void ElemInLinkId()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var sel = uidoc.Selection;
            var pick = sel.PickObject(ObjectType.LinkedElement);
            var id = pick.LinkedElementId.ToString();
            UserMessages.ShowMessage(id);
        }
        internal static List<Level> Levels;
        internal static void PickLevels()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var allLevels = doc.FindAll<Level>()
                .Select(x => new ElementInfoForUI(x))
                .ToList();
            var form = new Form_MultiElementSelection
                (allLevels,
                null,
                "选择楼层");
            var result = form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var levels = form.SelectedElements
                    .Cast<Level>()
                    .ToList();
                Levels = levels;
            }
        }
        internal static void SetAll()
        {
            if (Levels.Count == 0)
                throw new CommonUserExceptions("请选择楼层。");
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var elemFilter = new ElementMulticategoryFilter(supportedCats);
            var all = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(elemFilter)
                .ToList();
            setElems(all, Levels);
        }

        internal static void SetPicked()
        {
            if (Levels.Count == 0)
                throw new CommonUserExceptions("请选择楼层。");
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new ElementMultiCategorySelectionFilter(supportedCats);
            string m = "选择图元。将填写信息。点【完成】。";
            Form_cursorPrompt.Start(m, APP.MainWindow);
            var picks = sel.PickObjects(ObjectType.Element, filter, m);
            Form_cursorPrompt.Stop();
            var elems = picks.Select(x => doc.GetElement(x));
            setElems(elems, Levels);
        }
        private static void setElems(IEnumerable<Element> _elems, List<Level> _levels)
        {
            var doc = _elems.First().Document;
            using (Transaction trans = new Transaction(doc, "写入协作信息"))
            {
                trans.Start();
                foreach (var elem in _elems)
                {
                    setOneElem(elem, _levels);
                }
                trans.Commit();
            }
        }
        private static void setOneElem(Element _elem, List<Level> _levels)
        {
            var bb = _elem.GetBoundingBoxInModelCS(null);
            if (bb == null)
                return;
            var centroid = bb.GetCentroid();
            var coordLevel = centroid.Z.GetFirstLevelBelow(_levels);
            if (coordLevel == null)
                coordLevel = bb.Min.Z.GetClosestLevel(_levels);
            var param = _elem.LookupParameter("协作_楼层");
            if (param == null || param.StorageType != StorageType.String)
                return;
            else
                param.Set(coordLevel.Name);
        }
        private static void setOneElem_backup(Element _elem, List<Level> _levels)
        {
            Level coordLevel = null;
            if (_elem is Wall)
            {
                var lc = _elem.LocationCurve();
                var offset = _elem.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
                var z = lc.GetEndPoint(0).Z + offset;
                coordLevel = z.GetClosestLevel(_levels);
            }
            else if (_elem is Floor)
            {
                var doc = _elem.Document;
                var level = doc.GetElement(_elem.LevelId) as Level;
                var offset = _elem.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();
                var z = level.ProjectElevation + offset;
                coordLevel = z.GetClosestLevel(_levels);
            }
            else if (_elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
            {
                var doc = _elem.Document;
                var level = doc.GetElement(_elem.LevelId) as Level;
                var offset = _elem.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble();
                var z = level.ProjectElevation + offset;
                coordLevel = z.GetClosestLevel(_levels);
            }
            else if (_elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
            {
                var lc = _elem.LocationCurve();
                var level = lc.GetEndPoint(0).GetClosestLevel(_levels);
                //level below
                coordLevel = getLevelBelow(level, Levels);
                if (coordLevel == null)
                    coordLevel = level;
            }
            else if (_elem is Ceiling)
            {
                var doc = _elem.Document;
                var level = doc.GetElement(_elem.LevelId) as Level;
                var offset = _elem.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM).AsDouble();
                var z = level.ProjectElevation + offset;
                level = z.GetClosestLevel(_levels);
                //level below
                coordLevel = getLevelBelow(level, Levels);
                if (coordLevel == null)
                    coordLevel = level;
            }
            var param = _elem.LookupParameter("协作_楼层");
            if (param == null || param.StorageType != StorageType.String)
                return;
            else
                param.Set(coordLevel.Name);
        }
        private static Level getLevelBelow(Level _thisLevel, IEnumerable<Level> _levels)
        {
            Level found = null;
            double maxOffset = double.MinValue;

            foreach (var l in _levels)
            {
                if (l.Id == _thisLevel.Id)
                    continue;
                var offset = l.ProjectElevation - _thisLevel.ProjectElevation;
                if (offset < -0.001 && offset > maxOffset)
                {
                    found = l;
                    maxOffset = offset;
                }
            }

            return found;
        }
        private static List<BuiltInCategory> supportedCats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Roofs,

            BuiltInCategory.OST_Stairs,
            /*
            BuiltInCategory.OST_StairsRuns,
            BuiltInCategory.OST_StairsLandings,
            BuiltInCategory.OST_StairsStringerCarriage,
            */
            BuiltInCategory.OST_Railings,
            BuiltInCategory.OST_StairsRailing,
            BuiltInCategory.OST_Ramps,

            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_SpecialityEquipment,
            BuiltInCategory.OST_GenericModel,

            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming,
        };
    }
}
