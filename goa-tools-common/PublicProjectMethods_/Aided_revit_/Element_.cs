using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Element_
    {
        #region ElementIds Elements Convert 
        /// <summary>
        /// 元素Id列表转换为元素列表
        /// </summary>
        public static IEnumerable<Element> ToElements(this IEnumerable<ElementId> _EleIds, Document doc)
        {
            List<Element> _Eles = new List<Element>();
            foreach (ElementId _EleId in _EleIds)
            {
                Element _Ele = doc.GetElement(_EleId);
                _Eles.Add(_Ele);
            }
            return _Eles;
        }
        /// <summary>
        /// 元素列表转换为元素Id列表
        /// </summary>
        public static IEnumerable<ElementId> ToElementIds(this IEnumerable<Element> _Eles)
        {
            List<ElementId> _EleIds = new List<ElementId>();
            foreach (Element _Ele in _Eles)
            {
                _EleIds.Add(_Ele.Id);
            }
            return _EleIds;
        }
        #endregion
    }
}
