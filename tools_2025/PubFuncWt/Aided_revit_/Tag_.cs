using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Tag_
    {

        #region Tag 标记
        /// <summary>
        /// 给图元注释标记族，默认位置为Boundingbox的Min，注释数据为图元实例标记值，是否显示引线（0-否，1-是）
        /// </summary>
        public static void SetTag(this View view, Document doc,  Element element, ElementId TagFamilySymbolId, int isHadLEADER_LINE)
        {
            string field = element.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();
            if (field == null || field == "")
            { }
            else
            {
                BoundingBoxXYZ boundingBoxXYZ = element.get_BoundingBox(view);
                if (boundingBoxXYZ != null)
                {
                    XYZ min = element.get_BoundingBox(view).Min;
                    XYZ max = element.get_BoundingBox(view).Max;
                    XYZ middleEleXYZ = (min + max) / 2;
                    IndependentTag independentTag = IndependentTag.Create(doc, TagFamilySymbolId, view.Id, new Reference(element), true, TagOrientation.Horizontal, min);
                    // 是否 取消标记引线
                    independentTag.get_Parameter(BuiltInParameter.LEADER_LINE).Set(isHadLEADER_LINE);
                }
            }
        }
        #endregion 
    }
}
