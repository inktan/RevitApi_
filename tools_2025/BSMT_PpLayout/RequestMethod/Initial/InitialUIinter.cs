using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 从UI界面获取的初始数据
    /// </summary>
    class InitialUIinter : RequestMethod
    {
        internal InitialUIinter(UIApplication uiapp) : base(uiapp)
        {
        }
     

        /// <summary>
        /// 初始化操作
        /// </summary>
        internal override void Execute()
        {
        }
        /// <summary>
        /// UI已经存在的方案Ids
        /// </summary>
        internal List<ElementId> UiBsmtWallIds()
        {
            List<ElementId> baseWallIds = new List<ElementId>();
            foreach (string str in GlobalData.basementWallOutlineNames)
            {
                string[] strs = str.Split(':');
                string strElementId = strs.Last();
                strElementId = strElementId.Remove(strElementId.Length - 1, 1);
                int elementInt = Convert.ToInt32(strElementId);
                baseWallIds.Add(new ElementId(elementInt));
            }
            return baseWallIds;
        }
        /// <summary>
        /// UI✔的目标方案范围
        /// </summary>
        internal List<ElementId> SelBsmtWallIds()
        {
            // 使用ui选中的数据进行下一步操作
            List<ElementId> selBaseWallIds = new List<ElementId>();
            foreach (string str in GlobalData.Instance.selViewNames)
            {
                string[] strs = str.Split(':');
                string strElementId = strs.Last();
                strElementId = strElementId.Remove(strElementId.Length - 1, 1);
                int elementInt = Convert.ToInt32(strElementId);
                selBaseWallIds.Add(new ElementId(elementInt));
            }

            if (selBaseWallIds.Count < 1) throw new NotImplementedException("UI界面上未选方案");

            return selBaseWallIds;
        }

        /// <summary>
        /// 与UI界面勾选方案对应的视图层级
        /// </summary>
        internal List<ElemsViewLevel> ElemsViewLevels()
        {
            return this.SelBsmtWallIds().Select(p => doc.GetElement(p).OwnerViewId).Distinct().Select(p => new ElemsViewLevel(doc.GetElement(p))).ToList();
        }

    }
}
