
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 地库区域裁剪后 最外围的环状区域
    /// </summary>
    class OutWallRing: HLevSubregion
    {
        internal List<Polygon2d> Polygon2ds;
        internal SubParkArea SubParkAreaOut;
        internal SubParkArea SubParkAreaIn;

        internal List<BoundO> ObstructBoundLoops;

        // 环形内圈向外偏移一排车距的 SubParkArea
        internal SubParkArea SubParkAreaInSelf;

        internal OutWallRing(List<Polygon2d> _polygon2ds, Bsmt _bsmt):base(_bsmt)
        {
            this.Polygon2ds = _polygon2ds;

            this.AdjacentRoads = AdjacentRoadCenterLine(this.Polygon2ds.Last()); ;
            Computer();
        }

        void Computer()
        {
            this.SubParkAreaOut = new SubParkArea(this.Polygon2ds.First(), this.Bsmt);
            this.SubParkAreaOut.Computer();
            this.SubParkAreaIn = new SubParkArea(this.Polygon2ds.Last(), this.Bsmt);
            this.SubParkAreaIn.Computer();

            // 环状空间需要单独处理 ==》
            if (this.Polygon2ds.Count() == 2)
            {
                this.ObstructBoundLoops = new List<BoundO>(this.SubParkAreaOut.ObstructBoundLoops);
                Polygon2d inPoly = this.Polygon2ds.Last();

                for (int i = this.ObstructBoundLoops.Count - 1; i > -1; i--)
                {
                    if (this.SubParkAreaIn.Polygon2d.Contains(this.ObstructBoundLoops[i].polygon2d))// 内部范围包含外部范围的则移除
                    {
                        this.ObstructBoundLoops.RemoveAt(i);
                    }
                }
            }

            // 找出内圈 向外偏移一个停车距离，作为停车边界，但是该部分停车，需要原地旋转180°
            SubParkArea temp = new SubParkArea(this.SubParkAreaIn.Polygon2d.OutwardOffeet(GlobalData.Instance.pSHeight_num), this.Bsmt);
            temp.Computer();
            this.SubParkAreaInSelf = temp;
        }
        
    }
}
