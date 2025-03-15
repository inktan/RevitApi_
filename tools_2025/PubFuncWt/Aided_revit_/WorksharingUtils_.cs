using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;

namespace PubFuncWt
{
    public static class WorksharingUtils_
    {
        public static string Occupied(this Document doc, ElementId elementId)
        {

            CheckoutStatus cks = WorksharingUtils.GetCheckoutStatus(doc, elementId);
            if (cks == CheckoutStatus.OwnedByOtherUser)
            {
                WorksharingTooltipInfo worksharingTooltipInfo = WorksharingUtils.GetWorksharingTooltipInfo(doc, elementId);
                return worksharingTooltipInfo.Owner;
            }
            return "";
        }
        /// <summary>
        /// 判断要操作的图元是否别其他用户占用
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementIds"></param>
        public static void Occupied(this Document doc, IEnumerable<ElementId> elementIds)
        {
            string info = "需要计算门窗编号的图元被下列用户占用：\n";
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
            int index = 0;
            foreach (var item in elementIds)
            {
                string singleInfo = Occupied(doc, item);
                if (!singleInfo.IsNullOrEmpty())
                {
                    if (!keyValuePairs.ContainsKey(singleInfo))
                    {
                        index++;
                        keyValuePairs.Add(singleInfo, index);
                    }
                }
            }

            if (keyValuePairs.Count < 1)
            {
                return;
            }

            foreach (var item in keyValuePairs)
            {
                info += item.Key + "、";
            }
            info += "请通知上述用户释放权限";
            throw new NotImplementedException(info);
        }
    }
}
