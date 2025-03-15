
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
    class HLevSubregion
    {
        internal Document Doc => Bsmt.Doc;
        internal Bsmt Bsmt;
        /// <summary>
        /// 当前停车子区域边界处的道路线
        /// </summary>
        internal List<Route> AdjacentRoads { get; set; }

        internal HLevSubregion(Bsmt _bsmt)
        {
            this.Bsmt = _bsmt;
        }
        internal HLevSubregion()
        {
        }
        /// <summary>
        /// 求停车子区域的边界道路中心线
        /// </summary>
        /// <returns></returns>
        protected List<Route> AdjacentRoadCenterLine(Polygon2d o)
        {
            List<Route> seg2ds = new List<Route>();
            foreach (var item in this.Bsmt.InBoundEleLines)// 所有的地库范围内所有的道路线
            {
                Segment2d seg2d = item.Segment2d;
                Polygon2d poly2d = seg2d.ToRenct2d(GlobalData.Instance.Wd_pri_num / 1.8);// 判断道路是否与当前停车区域相关

                if (o.Intersects(poly2d))
                {
                    seg2ds.Add(new Route(seg2d));
                }
            }
            return seg2ds;
        }
    }
}
