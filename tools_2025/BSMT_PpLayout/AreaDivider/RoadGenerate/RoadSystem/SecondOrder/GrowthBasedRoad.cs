using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using g3;
using ClipperLib;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 基于南北关系，设计4种平行车道规则
    /// </summary>
    class GrowthBasedRoad
    {
        internal List<BoundO> ObstructBoundOs;
        internal List<Polygon2d> ObstalPolygon2ds;

        internal List<Route> AdjacentRoutes;

        internal Document Document;

        internal Polygon2d Region;//环状取其外圈
        internal Polygon2d BsmtPolygon;//地库外墙范围

        internal GrowthBasedRoad(SubParkArea subParkArea, List<BoundO> obstructBoundOs, Polygon2d region, List<Route> adjacentRoutes, Document document)
        {
            this.ObstructBoundOs = obstructBoundOs;
            this.ObstalPolygon2ds = this.ObstructBoundOs
                 .Where(p => p.EleProperty == EleProperty.ResidenStruRegion).Select(p => p.polygon2d)
                 .ToList();
            this.AdjacentRoutes = adjacentRoutes;

            this.Region = region;
            this.BsmtPolygon = subParkArea.Bsmt.BsmtBound.Polygon2d;

            this.Document = document;
        }

        /// <summary>
        /// 给出多套道路方案
        /// </summary>
        /// <returns></returns>
        internal List<SchemeInfo> Computer()
        {
            /*
             * 道路如何生长==>
             * 
             * 坡道：
             * 1、从入口方向延伸一条道路与现在道路相连
             * 2、与入口方向垂直放置条道路
             * 
             * 塔楼间
             * 1、计算塔楼所在矩形到相邻道路的垂直投影点；
             * 2、基于上步计算点设计道路
             * 
             * 障碍物中存在坡道：
             * 
             * 不存在障碍物的情况
             * 1、如果是环形，判断环状区域的厚度
             * 2、非环形，采用阵列道路线的策略
             * 
             */

            // 求坡道数量 暂时不考虑坡道对车道生成的影响
            List<BoundO> vehicleOs = this.ObstructBoundOs
                .Where(p => p.EleProperty == EleProperty.VehicleRamp
                || p.EleProperty == EleProperty.VehicleRamp_Arc_A
                || p.EleProperty == EleProperty.VehicleRamp_Arc_B
                || p.EleProperty == EleProperty.VehicleRamp_UpDown)
                .ToList();

            // 罗列 除坡道的不可停车区域
            //List<BoundO> obstructBoundO = this.ObstructBoundOs
            //     .Where(p => p.EleProperty != EleProperty.VehicleRamp
            //     && p.EleProperty != EleProperty.VehicleRamp_Arc_A
            //     && p.EleProperty != EleProperty.VehicleRamp_Arc_B
            //     && p.EleProperty != EleProperty.VehicleRamp_UpDown)
            //     .ToList();                  

            Segment2d segment2d = ObstalPolygon2ds.FindDirection();
            Frame3d frame3d = segment2d.P0.CreatFrame3d(segment2d.Direction);

            // toFrame
            // 塔楼投影区域为道路生长的基准线
            this.ObstalPolygon2ds = this.ObstalPolygon2ds.Select(p => frame3d.ToFrame2dPoly(p)).ToList();
            this.Region = frame3d.ToFrame2dPoly(this.Region);// 实际停车计算区域
            this.BsmtPolygon = frame3d.ToFrame2dPoly(this.BsmtPolygon);
            this.AdjacentRoutes = this.AdjacentRoutes.Select(p => new Route(frame3d.ToFrame2dSeg(p.Segment2d))).ToList();

            List<SchemeInfo> result = ComputerBaseSingleRoute_Ver(this.ObstalPolygon2ds);
            // fromFrame
            result.ForEach(p => p.FromFrame2d(frame3d));
            return result;
        }
        /// <summary>
        /// x轴方向车道生长 处理辅助线
        /// </summary>
        List<SchemeInfo> ComputerBaseSingleRoute_Ver(List<Polygon2d> obstructBoundO)
        {
            List<Vector2d> roadDesignAuxiliarySegCenters = new List<Vector2d>();
            foreach (var o in obstructBoundO)
            {
                Vector2d p0 = o.LUpOfBox2d();
                Vector2d p1 = o.LDpOfBox2d();
                roadDesignAuxiliarySegCenters.Add(HandleAuxiliarySeg(p0));
                roadDesignAuxiliarySegCenters.Add(HandleAuxiliarySeg(p1));
            }

            // 取到所有的 障碍物 基准线
            Vector2d tempExtent = new Vector2d(2000.0, 0);// 构造道路辅助线
            List<Segment2d> roadDesignAuxiliarySegs = roadDesignAuxiliarySegCenters.DelDuplicate().Select(p => new Segment2d(p - tempExtent, p + tempExtent)).ToList();
            //CMD.Doc.CreateDirectShapeWithNewTransaction(roadDesignAuxiliarySegs.ToLines(), CMD.Doc.ActiveView);// 打印所有的基准辅助线

            // 排序 + 过滤（横向并列两个障碍物时，处理较麻烦）
            roadDesignAuxiliarySegs = roadDesignAuxiliarySegs.OrderByDescending(p => p.Center.y).ToList();// 由高到低进行排序

            return ComputerBaseSingleRoute_Ver(roadDesignAuxiliarySegs);
        }
        /// <summary>
        /// 处理辅助线
        /// </summary>
        private Vector2d HandleAuxiliarySeg(Vector2d p0)
        {
            Vector2d tempExtent = new Vector2d(2000.0, 0);// 构造道路辅助线
            Segment2d up_now = new Segment2d(p0 - tempExtent, p0 + tempExtent);
            List<Vector2d> tempPoints = this.Region.ToRectangle().ToPolygon2d().FindInterSectionPoint(up_now).OrderBy(p => p.x).ToList();// 道路辅助线与地库边界的交点

            if (tempPoints.Count < 2)
            {
                return p0;
            }
            return (tempPoints.First() + tempPoints.Last()) / 2;
        }
        /// <summary>
        /// x轴方向车道生长
        /// </summary>
        List<SchemeInfo> ComputerBaseSingleRoute_Ver(List<Segment2d> roadDesignAuxiliarySegs)
        {
            List<SchemeInfo> schemeInfos = new List<SchemeInfo>();
            // 首、尾辅助线对应空间的道路可能性均为3种
            int count = roadDesignAuxiliarySegs.Count;
            if (count == 0)
            {
                //SchemeInfo schemeInfo = new SchemeInfo();
                //schemeInfo.Designs.Add(new List<Route>());
                //return new List<SchemeInfo>() { schemeInfo };
                return schemeInfos;
            }
            // 区域上下，需要增加一个车长的空间
            Vector2d vector2d = new Vector2d(0, GlobalData.Instance.pSHeight_num);
            for (int i = 0; i < count; i++)
            {
                if (i == 0)// 障碍物最上边界线
                {
                    SchemeInfo schemeInfo = new SchemeInfo();

                    Segment2d upNow = roadDesignAuxiliarySegs[i];
                    double height = this.Region.RUpOfBox2d().y - upNow.Center.y;// 高度
                    IEnumerable<Route> routesUp01 = Desion01(upNow, height, new Vector2d(0, 1));
                    IEnumerable<Route> routesUp02 = Desion02(upNow, height, new Vector2d(0, 1));
                    IEnumerable<Route> routesUp03 = Desion03(upNow, height, new Vector2d(0, 1));

                    List<Route> routes = routesUp01.ToList();
                    if (routes.Count != 0)
                        schemeInfo.Designs.Add(routes);
                    routes = routesUp02.ToList();
                    if (routes.Count != 0)
                        schemeInfo.Designs.Add(routes);
                    routes = routesUp03.ToList();
                    if (routes.Count != 0)
                        schemeInfo.Designs.Add(routes);

                    schemeInfo.Designs.Add(new List<Route>());// 加一个空值，兜圈处理
                    schemeInfo.Region = new Polygon2d(new List<Vector2d>() { upNow.P0 - vector2d, upNow.P1 - vector2d, upNow.P1 + new Vector2d(0, 1000), upNow.P0 + new Vector2d(0, 1000) });// 这里Y轴上移1000，是为了保证能够完整的裁剪定位线与地库边界之间的区域

                    schemeInfos.Add(schemeInfo);
                }
                else if (i == count - 1)//障碍物最下边界线
                {
                    SchemeInfo schemeInfo = new SchemeInfo();

                    Segment2d downNow = roadDesignAuxiliarySegs[i];
                    double height = downNow.Center.y - this.Region.LDpOfBox2d().y;// 高度
                    IEnumerable<Route> routesDown01 = Desion01(downNow, height, new Vector2d(0, -1));
                    IEnumerable<Route> routesDown02 = Desion02(downNow, height, new Vector2d(0, -1));
                    IEnumerable<Route> routesDown03 = Desion03(downNow, height, new Vector2d(0, -1));

                    List<Route> routes = routesDown01.ToList();
                    if (routes.Count != 0)
                    {
                        schemeInfo.Designs.Add(routes);
                    }
                    routes = routesDown02.ToList();
                    if (routes.Count != 0)
                    {
                        schemeInfo.Designs.Add(routes);
                    }
                    routes = routesDown03.ToList();
                    if (routes.Count != 0)
                    {
                        schemeInfo.Designs.Add(routes);
                    }
                    schemeInfo.Designs.Add(new List<Route>());// 加一个空值，兜圈处理
                    schemeInfo.Region = new Polygon2d(new List<Vector2d>() { downNow.P1 + vector2d, downNow.P0 + vector2d, downNow.P0 - new Vector2d(0, 1000), downNow.P1 - new Vector2d(0, 1000) });// 这里Y轴下移1000，是为了保证完整的裁剪最下部区域

                    schemeInfos.Add(schemeInfo);
                }
                else// 夹心空间
                {
                    for (int temp_i = 0; temp_i < 1; temp_i++)
                    {
                        SchemeInfo schemeInfo = new SchemeInfo();

                        Segment2d upNow = roadDesignAuxiliarySegs[i + 1];// 夹心-下边界
                        double height = roadDesignAuxiliarySegs[i].Center.y - roadDesignAuxiliarySegs[i + 1].Center.y;// 高度

                        IEnumerable<Route> routesUp01 = Desion01(upNow, height, new Vector2d(0, 1));
                        IEnumerable<Route> routesUp02 = Desion02(upNow, height, new Vector2d(0, 1));
                        IEnumerable<Route> routesUp03 = Desion03(upNow, height, new Vector2d(0, 1));

                        List<Route> routes = routesUp01.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesUp02.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesUp03.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);

                        Segment2d downNow = roadDesignAuxiliarySegs[i];// 夹心-上边界
                        IEnumerable<Route> routesDown01 = Desion01(downNow, height, new Vector2d(0, -1));
                        IEnumerable<Route> routesDown02 = Desion02(downNow, height, new Vector2d(0, -1));
                        IEnumerable<Route> routesDown03 = Desion03(downNow, height, new Vector2d(0, -1));

                        routes = routesDown01.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesDown02.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesDown03.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);

                        schemeInfo.Designs.Add(new List<Route>());// 加一个空值，兜圈处理

                        schemeInfo.Region = new Polygon2d(new List<Vector2d>() { upNow.P0 - vector2d, upNow.P1 - vector2d, downNow.P1 + vector2d, downNow.P0 + vector2d });

                        schemeInfos.Add(schemeInfo);
                    }

                    for (int temp_i = 0; temp_i < 1; temp_i++)
                    {
                        SchemeInfo schemeInfo = new SchemeInfo();

                        Segment2d upNow = roadDesignAuxiliarySegs[i];// 夹心-下边界
                        double height = roadDesignAuxiliarySegs[i-1].Center.y - roadDesignAuxiliarySegs[i].Center.y;// 高度

                        IEnumerable<Route> routesUp01 = Desion01(upNow, height, new Vector2d(0, 1));
                        IEnumerable<Route> routesUp02 = Desion02(upNow, height, new Vector2d(0, 1));
                        IEnumerable<Route> routesUp03 = Desion03(upNow, height, new Vector2d(0, 1));

                        List<Route> routes = routesUp01.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesUp02.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesUp03.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);

                        Segment2d downNow = roadDesignAuxiliarySegs[i - 1];// 夹心-上边界
                        IEnumerable<Route> routesDown01 = Desion01(downNow, height, new Vector2d(0, -1));
                        IEnumerable<Route> routesDown02 = Desion02(downNow, height, new Vector2d(0, -1));
                        IEnumerable<Route> routesDown03 = Desion03(downNow, height, new Vector2d(0, -1));

                        routes = routesDown01.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesDown02.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);
                        routes = routesDown03.ToList();
                        if (routes.Count != 0)
                            schemeInfo.Designs.Add(routes);

                        schemeInfo.Designs.Add(new List<Route>());// 加一个空值，兜圈处理

                        schemeInfo.Region = new Polygon2d(new List<Vector2d>() { upNow.P0 - vector2d, upNow.P1 - vector2d, downNow.P1 + vector2d, downNow.P0 + vector2d });

                        schemeInfos.Add(schemeInfo);
                    }
                    i++;
                }
            }
            return schemeInfos;
        }
        #region 3种基本排布规则

        /*
         * 
         * 基于边界第一排为车道
         * 
         * 基于边界第二排为车道
         * 
         * 基于边界第三排为车道 
         * 
         */

        IEnumerable<Route> Desion01(Segment2d seg2d, double height, Vector2d moveDirection)
        {
            double spacingDistance = GlobalData.Instance.pSHeight_num * 2 + GlobalData.Instance.Wd_pri_num;// 车道每次上移距离
            double distance = GlobalData.Instance.Wd_pri_num / 2;// 当前车道以及车道间空间在y方向上的距离

            Segment2d tempSeg2d = seg2d;
            int i = 0;
            while (true)
            {
                if (height - distance < GlobalData.Instance.Wd_pri_num / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    tempSeg2d = tempSeg2d.Move(moveDirection, GlobalData.Instance.Wd_pri_num / 2);
                }
                else
                {
                    tempSeg2d = tempSeg2d.Move(moveDirection, spacingDistance);
                }

                // 循环中断条件
                distance += spacingDistance;

                i++;

                Segment2d segReDraw = RoadRedrawing(tempSeg2d);
                // 判断是否为有效道路
                if (IsValidRoadSpace(segReDraw))
                {
                    yield return new Route(segReDraw);
                }
            }
        }
        IEnumerable<Route> Desion02(Segment2d seg2d, double height, Vector2d moveDirection)
        {
            double spacingDistance = GlobalData.Instance.pSHeight_num * 2 + GlobalData.Instance.Wd_pri_num;// 车道每次上移距离
            double distance = GlobalData.Instance.pSHeight_num + GlobalData.Instance.Wd_pri_num / 2;// 当前车道以及车道间空间在y方向上的距离

            Segment2d tempSeg2d = seg2d;
            int i = 0;
            while (true)
            {
                if (height - distance < GlobalData.Instance.Wd_pri_num / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    tempSeg2d = tempSeg2d.Move(moveDirection, GlobalData.Instance.pSHeight_num + GlobalData.Instance.Wd_pri_num / 2);
                }
                else
                {
                    tempSeg2d = tempSeg2d.Move(moveDirection, spacingDistance);
                }

                // 循环中断条件
                distance += spacingDistance;// 距离判断出现了问题

                i++;

                Segment2d segReDraw = RoadRedrawing(tempSeg2d);
                // 判断是否为有效道路
                if (IsValidRoadSpace(segReDraw))
                {
                    yield return new Route(segReDraw);
                }
            }
        }
        IEnumerable<Route> Desion03(Segment2d seg2d, double height, Vector2d moveDirection)
        {
            double spacingDistance = GlobalData.Instance.pSHeight_num * 2 + GlobalData.Instance.Wd_pri_num;// 车道每次上移距离
            double distance = GlobalData.Instance.pSHeight_num * 2 + GlobalData.Instance.Wd_pri_num / 2;// 当前车道以及车道间空间在y方向上的距离

            Segment2d tempSeg2d = seg2d;
            int i = 0;
            while (true)
            {
                if (height - distance < GlobalData.Instance.Wd_pri_num / 2 - Precision_.Precison)
                {
                    break;
                }
                if (i == 0)
                {
                    tempSeg2d = tempSeg2d.Move(moveDirection, GlobalData.Instance.pSHeight_num * 2 + GlobalData.Instance.Wd_pri_num / 2);
                }
                else
                {
                    tempSeg2d = tempSeg2d.Move(moveDirection, spacingDistance);
                }
                // 循环中断条件
                distance += spacingDistance;// 距离判断出现了问题

                i++;

                Segment2d segReDraw = RoadRedrawing(tempSeg2d);
                // 判断是否为有效道路
                if (IsValidRoadSpace(segReDraw))
                {
                    yield return new Route(segReDraw);
                }
            }
        }
        /// <summary>
        /// 判断是否为有效路径，当道路空间的4个角点，判断是否满足 >= 三个位于地库外墙范围内
        /// </summary>
        bool IsValidRoadSpace(Segment2d segReDraw)
        {
            // 当当前停车区域向外偏移一个路宽，进行判定道路是否有效
            Polygon2d validRegion = this.Region.OutwardOffeet(GlobalData.Instance.Wd_pri_num);

            // 0 判断道路是否与障碍物碰撞
            foreach (var item in this.ObstalPolygon2ds)
            {
                if (item.Intersects(segReDraw))
                {
                    return false;
                }
            }
            // 1 对道路进行分段取样，判断是否所有样本点，落在可停车空间
            List<Vector2d> vector2ds = segReDraw.DivideByDis(1.0).ToList();
            foreach (var item in vector2ds)
            {
                if (!validRegion.Contains(item))
                {
                    return false;
                }
            }
            // 2 道路的四个角点，满足3个落在可停车空间就可以
            Polygon2d poly2d = segReDraw.ToRenct2d(GlobalData.Instance.Wd_pri_num / 2);
            int isIn = 0;
            foreach (var item in poly2d.Vertices)
            {
                if (validRegion.Contains(item))
                {
                    isIn++;
                }
            }
            if (isIn > 2)
            {
                return true;
            }
            return false;
        }
        Segment2d RoadRedrawing(Segment2d seg2d)
        {
            List<Vector2d> vector2ds = new List<Vector2d>();
            foreach (var item in this.AdjacentRoutes.Select(p => p.Segment2d).ToList())// 道路辅助线与计算区域周边道路的相交情况
            {
                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(seg2d, item);
                intrSegment2Segment2.Compute();
                if (intrSegment2Segment2.Quantity == 1)
                {
                    vector2ds.Add(intrSegment2Segment2.Point0);
                }
            }

            vector2ds.AddRange(this.BsmtPolygon.FindInterSectionPoint(seg2d, 2.0));// 道路辅助线与地库边界的交点

            // 把点集左右分为两个部分

            // 辅助中心点，需要明确
            Vector2d center = seg2d.Center;
            //

            List<Vector2d> vec2dsLeft = vector2ds.Where(p => p.x < center.x).OrderBy(p => p.Distance(center)).ToList();
            List<Vector2d> vec2dsRight = vector2ds.Where(p => p.x > center.x).OrderBy(p => p.Distance(center)).ToList();

            if (vec2dsLeft.Count >= 1 && vec2dsRight.Count >= 1)
            {
                return new Segment2d(vec2dsLeft[0], vec2dsRight[0]);
            }
            else
            {
                return seg2d;
            }
        }

        #endregion
    }
}
