using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 全局使用兜圈处理-基于原有道路兜圈处理
    /// </summary>
    class GlobalCircle : Refresh
    {
        /// <summary>
        /// 基于现有道路空间进行兜圈处理
        /// </summary>
        /// <param name="subParkAreas"></param>
        internal GlobalCircle(BsmtClipByRoad bsmtClipByRoad, Document document) : base(bsmtClipByRoad, document) { }

        internal override void Execute()
        {
            // 环状区域
            if (this.OutWallRing != null && this.OutWallRing.Polygon2ds.Count == 2)
            {
                Polygon2d minArea = this.OutWallRing.Polygon2ds.OrderBy(p => p.Area).First();
                Polygon2d largeArea = minArea.OutwardOffeet(GlobalData.Instance.pSHeight_num); // 找出内圈 向外偏移一个停车距离，作为停车边界，但是该部分停车，需要原地旋转180°
                SubParkArea temp = new SubParkArea(largeArea, this.OutWallRing.Bsmt);
                temp.Computer();

                var cellArea = new CellArea(temp.Polygon2d, temp);
                var result = CalPs_Backtrack(cellArea, FollowPathCutType.AllBoundary);// 环状内圈兜圈处理，与地库外墙线无关
                this.PpLocPoints.AddRange(result);
            }

            // 所有子区域

            foreach (var item in this.SubParkAreas)
            {
                if (GlobalData.Instance.SelPointToAdjustStartPoint.x != 0 || GlobalData.Instance.SelPointToAdjustStartPoint.y != 0)// 判断是否激活指定区域
                {
                    if (item.Polygon2d.Contains(GlobalData.Instance.SelPointToAdjustStartPoint))// 判断激活点位于哪个区域
                    {
                        item.Computer();
                        //item.DeleteUnFixedPses();
                    }
                    else
                    {
                        continue;
                    }
                }

                item.Computer();
                var cellArea = new CellArea(item.Polygon2d, item);
                var result = CalPs_Backtrack(cellArea, FollowPathCutType.Just_no_BsmeWall);// 兜圈
                this.PpLocPoints.AddRange(result);
            }
        }
    }
}
