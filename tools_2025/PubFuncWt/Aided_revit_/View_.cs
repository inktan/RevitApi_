using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt.Aided_revit_
{
    public static class View_
    {
        /// <summary>
        /// 获取剖切面偏移当前视图标高的距离
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static double cutPlanOffset(this View view)
        {
            ViewPlan viewPlan = view as ViewPlan;
            PlanViewRange planViewRange = viewPlan.GetViewRange();
            return planViewRange.GetOffset(PlanViewPlane.CutPlane);// 注意：详图项目的族实例在平面视图放置高度与视图范围中的剖切面保持一致 
        }
    }
}
