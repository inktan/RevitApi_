using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class FilledRegion_
    {
        #region FilledRegion
        public static List<string> FillRegionTypeNames(this Document doc)
        {
            return (new FilteredElementCollector(doc))
                .OfCategory(BuiltInCategory.OST_DetailComponents)
                .OfClass(typeof(FilledRegionType))
                .Select(p => p.Name)
                .ToList();
        }
        /// <summary>
        /// 求出文档中符合目标字段的详图填充样式，如果不存在，则创建
        /// </summary>
        public static ElementId GetFilledRegionTypeId(this Document doc, string filledRegionTypeName)
        {
            var filledRegionTypes = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_DetailComponents).OfClass(typeof(FilledRegionType)).ToElements();
            var fillPatternTypes = (new FilteredElementCollector(doc)).OfClass(typeof(FillPatternElement)).ToElements();
            Element _filledRegionType = filledRegionTypes.First();
            Element _fillPatternTypes = fillPatternTypes.First(p => p.Name == "<实体填充>");
            List<string> filledRegionTypeNames = filledRegionTypes.Select(p => p.Name).ToList();
            if (filledRegionTypeNames.Contains(filledRegionTypeName))
            {
                _filledRegionType = filledRegionTypes.Where(p => p.Name == filledRegionTypeName).First();
                return _filledRegionType.Id;
            }
            else
            {
                FilledRegionType filledRegionType;
                using (Transaction creatFilledRegionType = new Transaction(doc, "creatFilledRegionType"))
                {
                    creatFilledRegionType.Start();
                    filledRegionType = (_filledRegionType as FilledRegionType).Duplicate(filledRegionTypeName) as FilledRegionType;
                    filledRegionType.ForegroundPatternId = _fillPatternTypes.Id;
                    filledRegionType.ForegroundPatternColor = new Color(0, 0, 0);
                    creatFilledRegionType.Commit();
                }
                return filledRegionType.Id;
            }
        }
        /// <summary>
        /// 批量创建只含有一个线圈的详图填充区域
        /// </summary>
        public static List<ElementId> CreatFilledRegoins(this View view, Document doc, List<CurveLoop> curves, string filledRegionTypeName, int surfaceTransparency)
        {
            List<ElementId> elementIds = new List<ElementId>();
            ElementId _filledRegionTypeId = GetFilledRegionTypeId(doc, filledRegionTypeName);// 求出目标填充样式
            using (var creatFilledRegion = new Transaction(doc, "creatFilledRegion"))
            {
                creatFilledRegion.Start();

                foreach (var item in curves)
                {
                    FilledRegion filledRegion = FilledRegion.Create(doc, _filledRegionTypeId, view.Id, new List<CurveLoop>() { item });
                    elementIds.Add(filledRegion.Id);
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
                    ogs.SetSurfaceTransparency(surfaceTransparency);
                    view.SetElementOverrides(filledRegion.Id, ogs);
                }
                creatFilledRegion.Commit();
            }

            return elementIds;
        }
        /// <summary>
        /// 创建包含多个线圈的详图填充区域
        /// </summary>
        public static List<ElementId> CreatRingFilledRegoin(this View view, Document doc, List<CurveLoop> curves, string filledRegionTypeName, int surfaceTransparency)
        {
            List<ElementId> elementIds = new List<ElementId>();
            ElementId _filledRegionTypeId = GetFilledRegionTypeId(doc, filledRegionTypeName);// 求出目标填充样式
            using (var creatFilledRegion = new Transaction(doc, "creatFilledRegion"))
            {
                creatFilledRegion.Start();
                FilledRegion filledRegion = null;
                filledRegion = FilledRegion.Create(doc, _filledRegionTypeId, view.Id, curves);
                elementIds.Add(filledRegion.Id);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
                ogs.SetSurfaceTransparency(surfaceTransparency);
                view.SetElementOverrides(filledRegion.Id, ogs);
                creatFilledRegion.Commit();
            }
            return elementIds;
        }
        /// <summary>
        /// 获取详图区域的详图线
        /// </summary>
        public static List<DetailCurve> DetailCurves(this FilledRegion filledRegion, Document doc)
        {
            ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(CurveElement));
            List<ElementId> detailLineIds = filledRegion.GetDependentElements(elementClassFilter).ToList();

            List<DetailCurve> detailCurves = new List<DetailCurve>();
            foreach (ElementId elementId in detailLineIds)
            {
                DetailCurve detailCurve = doc.GetElement(elementId) as DetailCurve;
                detailCurves.Add(detailCurve);
            }
            return detailCurves;
        }
        /// <summary>
        /// 获取详图区域的Edges
        /// </summary>
        public static List<Edge> Edges(this FilledRegion filledRegion, View _view)
        {
            Options options = new Options();
            options.View = _view;
            options.ComputeReferences = true;
            GeometryElement geometryElement = filledRegion.get_Geometry(options);

            Solid solid = geometryElement.First() as Solid;
            return solid.Edges.ToIEnumerable().ToList();
        }

        #endregion
    }
}
