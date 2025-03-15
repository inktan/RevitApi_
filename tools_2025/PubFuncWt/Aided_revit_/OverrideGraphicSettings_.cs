using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class OverrideGraphicSettings_
    {
        #region OverrideGraphicSettings 视图专用图元图形
        /// <summary>
        /// 替换图元显示
        /// </summary>
        public static void SetFilledRegionColor(this View view, ElementId elementId, Color surfaceForegroundPatternColor, Color projectionLineColor, int transparency)
        {
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
            ogs.SetSurfaceForegroundPatternColor(surfaceForegroundPatternColor);
            ogs.SetProjectionLineColor(projectionLineColor);
            ogs.SetSurfaceTransparency(transparency);
            view.SetElementOverrides(elementId, ogs);
        }
        /// <summary>
        /// 替换图元显示 透明度
        /// </summary>
        public static void SetSurfaceTransparencyr(this View view, ElementId elementId, int transparency)
        {
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
            ogs.SetSurfaceTransparency(transparency);
            view.SetElementOverrides(elementId, ogs);
        }

        /// <summary>
        /// 替换图元显示 投影线
        /// </summary>
        public static void SetProjectionLineColor(this View view, ElementId elementId, Color projectionLineColor)
        {
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
            ogs.SetProjectionLineColor(projectionLineColor);
            view.SetElementOverrides(elementId, ogs);
        }
        #endregion
    }
}
