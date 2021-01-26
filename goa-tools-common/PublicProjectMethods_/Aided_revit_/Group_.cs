using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Group_
    {
        // <summary>
        /// 组内线样式是否统一
        /// </summary>
        public static bool IsSameAllCurveLineStyleFromGroup(this Group _group, Document doc, out string lineStyleName_First)
        {
            bool iSmae = false;
            List<ElementId> _EleIds = _group.GetMemberIds().ToList();
            int _count = _EleIds.Count;
            int __count = 0;

            List<CurveElement> curveElements = _EleIds.ToElements(doc).Cast<CurveElement>().ToList();
            lineStyleName_First = curveElements.First().LineStyle.Name;
            string graphicsStyle_frist_name = lineStyleName_First;

            curveElements.ForEach(p => {
                if (graphicsStyle_frist_name == p.LineStyle.Name)
                    __count += 1;
            });

            if (_count == __count)
                iSmae = true;

            return iSmae;
        }
    }
}
