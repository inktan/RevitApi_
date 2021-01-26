using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class FilledRegion_
    {
        #region FilledRegion
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
