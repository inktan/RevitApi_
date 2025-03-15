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
using ClipperLib;
using goa.Revit.DirectContext3D;
using goa.Common.g3InterOp;

namespace BSMT_PpLayout
{

    /// <summary>
    /// 横向道路生成条件，每个子区域，在南北方向，不允许出现错位的情况
    /// </summary>
    class Road2Growth : Refresh
    {
        internal Road2Growth(BsmtClipByRoad bsmtClipByRoad, Document document) : base(bsmtClipByRoad, document) { }

        internal override void Execute()
        {
            if (this.OutWallRing != null && this.OutWallRing.Polygon2ds.Count == 2)// 环状区域 需要特殊处理
            {
                // 当前策略，从中间横切一刀，会造成中间遗漏车位的情况
                Vector2d center = this.OutWallRing.Polygon2ds.First().Center();
                Segment2d segm2d = new Segment2d(center + new Vector2d(0, 1000), center - new Vector2d(0, 1000));
                List<Polygon2d> polygon2ds = this.OutWallRing.Polygon2ds.DifferenceClipper(segm2d.ToRenct2d(0.5)).ToList();

                foreach (var item in polygon2ds)
                {
                    SubParkArea subParkArea = new SubParkArea(item, this.OutWallRing.Bsmt);
                    this.SubParkAreas.Add(subParkArea);
                }
            }

            int i = 0;
            foreach (var item in this.SubParkAreas)
            {
                if (i != 10000)
                {
                    //continue;
                }

                if (GlobalData.Instance.PartAnchorPoint.x != 0 || GlobalData.Instance.PartAnchorPoint.y != 0)// 判断是否激活指定区域
                {
                    if (item.Polygon2d.Contains(GlobalData.Instance.PartAnchorPoint))// 判断激活点位于哪个区域
                    {
                        item.Computer();
                        item.DeleteUnFixedPses();
                    }
                    else
                    {
                        continue;
                    }
                }

                item.Computer();
                GrowthBasedRoad roadBasedGrowth = new GrowthBasedRoad(item, item.ObstructBoundLoops.ToList(), item.Polygon2d, item.AdjacentRoads, this.Document);

                // 以障碍物为基准，切分当前子区域，如果无障碍物，采取兜圈处理
                List<SchemeInfo> schemeInfos = roadBasedGrowth.Computer();

                List<Route> routes = new List<Route>();
                foreach (SchemeInfo schemeInfo in schemeInfos)
                {
                    List<Polygon2d> polygon2ds = new List<Polygon2d>();
                    if (schemeInfo.Region != null)
                    {
                        polygon2ds.AddRange(item.Polygon2d.Intersection(schemeInfo.Region));// 这里有一个求最大区域的设置
                    }
                    else
                    {
                        polygon2ds.Add(item.Polygon2d);
                    }
                    List<Route> optimalSolution = GetRoads(polygon2ds, schemeInfo, item);
                    routes.AddRange(optimalSolution);
                }

                List<PPLocPoint> temp = CalPsLocation(new List<Polygon2d>() { item.Polygon2d }, routes, item);
                this.PpLocPoints.AddRange(temp);
                this.Roads.AddRange(routes);
            }
        }
        /// <summary>
        /// 从每一SchemeInfo中，选择最优道路集合
        /// </summary>
        internal List<Route> GetRoads(IEnumerable<Polygon2d> _os, SchemeInfo schemeInfo, SubParkArea subParkArea)
        {
            Dictionary<List<PPLocPoint>, List<Route>> dict = new Dictionary<List<PPLocPoint>, List<Route>>();
            foreach (var item in schemeInfo.Designs)
            {
                List<PPLocPoint> temp = CalPsLocation(_os, item, subParkArea);
                dict.Add(temp, item);
            }
            // 排序后，找打最优解
            if (dict.Count > 0)
            {
                var dictSort = from objDic in dict orderby objDic.Key.Count descending select objDic;// 没有找到最优解
                return dictSort.First().Value;
            }
            else
            {
                return new List<Route>();
            }
        }
    }
}
