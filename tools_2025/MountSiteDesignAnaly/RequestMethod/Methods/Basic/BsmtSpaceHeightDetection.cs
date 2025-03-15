using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using goa.Common;
using g3;
using goa.Revit.DirectContext3D;

namespace MountSiteDesignAnaly
{
    internal class BsmtSpaceHeightDetection : RequestMethod
    {
        internal BsmtSpaceHeightDetection(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            GeometryDrawServersMgr.ClearAllServers();

            // 提取地下室底板
            List<Floor> bsmtBottomFloors = new FilteredElementCollector(this.doc)
                   .OfCategory(BuiltInCategory.OST_Floors)
                   .WhereElementIsNotElementType()
                   .Cast<Floor>()
                   .Where(p => p.Name.Contains("地下室底板"))
                   .ToList();
            List<FloorInfo> bsmtBottomFloorInfos = bsmtBottomFloors.Select(p => new FloorInfo(p)).ToList();

            // 提取地下室底提取地下室顶板板板
            List<Floor> bsmtTopFloors = new FilteredElementCollector(this.doc)
                   .OfCategory(BuiltInCategory.OST_Floors)
                   .WhereElementIsNotElementType()
                   .Cast<Floor>()
                   .Where(p => p.Name.Contains("地下室顶板"))
                   .ToList();

            List<FloorInfo> bsmtTopFloorInfos = bsmtTopFloors.Select(p => new FloorInfo(p)).ToList();

            foreach (var item01 in bsmtBottomFloorInfos)
            {
                if (item01.Poly.Area.EqualZreo())
                    continue;

                bool isBreak = false;
                foreach (var item02 in bsmtTopFloorInfos)
                {
                    if (item01.Element.Id == item02.Element.Id)
                        continue;

                    if (item02.Poly.Area.EqualZreo())
                        continue;

                    Polygon2d poly01 = item01.Poly.InwardOffeet(Precision_.TheShortestDistance);
                    Polygon2d poly02 = item02.Poly.InwardOffeet(Precision_.TheShortestDistance);

                    if (poly01.Intersects(poly02) || poly02.Intersects(poly01) || poly01.Contains(poly02) || poly02.Contains(poly01))
                    {

                        if (item02.elevation - item01.elevation < ViewModel.Instance.MinimumClearHeight.MilliMeterToFeet() + ViewModel.Instance.TopPlateThickness.MilliMeterToFeet())
                        {
                            this.uiDoc.ShowElements(new List<ElementId>() { item01.Element.Id, item02.Element.Id });// 窗口中心显示碰撞
                            this.sel.SetElementIds(new List<ElementId>() { item01.Element.Id, item02.Element.Id });// 窗口高亮显示碰撞

                            "地下室存在不满足最小层高的区域，请检查".TaskDialogErrorMessage();

                            isBreak = true;
                            break;
                        }
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }



            //throw new NotImplementedException();
        }
    }
}
