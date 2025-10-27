using Autodesk.Revit.DB;
using goa.Common;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CollisionDetection
{
    internal static class CollisionEleVsLinkedMod
    {
        //public static bool Excute(RevitLinkInstance revitLinkInstance, Element elem1)
        //{
        //    List<Solid> solids01 = elem1.GetAllSolids();

        //    Document linkDoc = revitLinkInstance.GetLinkDocument();

        //    List<Element> elements = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType()
        //        .Where(p => p.GetAllSolids().Count > 0)
        //        .ToList();
        //    elements.Count.ToString().TaskDialogErrorMessage();

        //    return false;
        //}
        public static List<Solid> GetSolidsFromLink(RevitLinkInstance rvtLink)
        {
            Document linkDoc = rvtLink.GetLinkDocument();
            List<Solid> solids = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType()
                .SelectMany(p => p.GetAllSolids())
                .ToList();
            solids.Count.ToString().TaskDialogErrorMessage();

            return solids;
        }

        public static List<Solid> GetTransformedSolidsFromLink(RevitLinkInstance rvtLink)
        {
            List<Solid> transformedSolids = new List<Solid>();
            Document linkDoc = rvtLink.GetLinkDocument();
            List<Solid> solids = new FilteredElementCollector(linkDoc).WhereElementIsNotElementType()
                .SelectMany(p => p.GetAllSolids())
                .ToList();

            // 获取链接实例的总变换矩阵
            Transform transform = rvtLink.GetTotalTransform();

            foreach (Solid solid in solids)
            {
                Solid transformedSolid = SolidUtils.CreateTransformed(solid, transform);
                transformedSolids.Add(transformedSolid);
            }

            return transformedSolids;
        }
    }
}
