using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
   public static class GroupType_
    {
        /// <summary>
        /// 清除无效详图组类型
        /// </summary>
        public static void DeleteInvalidDetailGroupTypes(this Document doc)
        {
            List<ElementId> elementIds = new List<ElementId>();
            foreach (var iGroup in (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_IOSDetailGroups).ToElements().GroupBy(p => p.Name))
            {
                if (iGroup.Count() == 1)
                {
                    foreach (var item in iGroup)
                    {
                        elementIds.Add(item.Id);
                    }
                }
            }
            doc.DelEleIds(elementIds);
        }
    }
}
