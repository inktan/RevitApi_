
///*******************************************************************************
//*                                                                              *
//* 全局刷新                                                                    *
//*                                                                                *
//*                                                                              *
//*******************************************************************************/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI.Selection;
//using System.Diagnostics;
//using PubFuncWt;
//using g3;
//using goa.Common;

//namespace BSMT_PpLayout
//{
//    class DesignRefresh: RequestMethod
//    {
//        public DesignRefresh(UIApplication uiApp) : base(uiApp) { }

//        internal override void Execute()
//        {
//            Stopwatch sw = new Stopwatch();// 耗时计算
//            sw.Restart();
//            string tempStr = "全局刷新";
//            int time = 0;

//            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
//            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
//            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

//            sw.Stop();
//            sw.Restart();

//            foreach (ElemsViewLevel  elemsViewLevel in elemsViewLevels)// 遍历单个视图
//            {
//                View nowView = elemsViewLevel.View;
//                elemsViewLevel.DelUnUsefulEles();// 清除未锁定 车位 族实例

//                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
//                {
//                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
//                        continue;
//                    bsmt.DelUnUsefulEles();// 清除与地库边界暧昧不清的图元
//                    bsmt.DelUnUsefulPillar();
//                    bsmt.ChangeSurfceTransparency();
//                    bsmt.Computer();

//                    BsmtClipByRoad bsmtClipByRoad = new BsmtClipByRoad(bsmt);// 算法启动 1- 划分区域     
//                    bsmtClipByRoad.CreatFilledRegion();// 创建新的子停车区域

//                    List<PPLocPoint> psLocPoints = new List<PPLocPoint>();// 停车位放置位置 停车位族类型 放置角度

//                    BsmtWallRing bsmtWallRing = bsmtClipByRoad.BsmtWallRing;// 地库外墙环状区域停车计算
//                    if (bsmtWallRing.Polygon2ds.Count ==2)
//                    {
//                        SubParkArea subParkArea = bsmtWallRing.SubParkArea;
//                        subParkArea.Computer();

//                        if (subParkArea.Area > 0)
//                        {
//                            FollowPathCutting followPathCutting = new FollowPathCutting(subParkArea);// 直接使用兜圈处理 不需要进入 RegionDivider
//                            List<SubCellArea> subCellAreas  = followPathCutting.Computer(true).ToList();

//                            foreach (SubCellArea subCellArea in subCellAreas)
//                            {
//                                subCellArea.Computer();
//                                PsCutClipFactory psArrayCutClip = new PsCutClipFactory(subCellArea, new PsArrCutClipVer());
//                                psArrayCutClip.Computer();
//                                psLocPoints.AddRange(psArrayCutClip.TarPpPoints);// 获取各个停车位的停车位置，以及旋转角度
//                            }
//                        }
//                    }

//                    sw.Stop();
//                    NLogger.Instance.Trace($"{tempStr}-{time += 1}耗时 " + (sw.ElapsedMilliseconds / 1000.0).ToString() + " 秒。");
//                    sw.Restart();

//                    int i = 0;
//                    List<SubParkArea> subParkAreas = bsmtClipByRoad.SubParkAreas;

//                    List<CellArea> allCellAreas = new List<CellArea>();
//                    foreach (SubParkArea subParkArea in subParkAreas)
//                    {
//                        subParkArea.Computer();

//                        // 什么情况下允许基于塔楼填充区域切割图形 ????
//                        DivideSubParkArea divideSubParkArea = new DivideSubParkArea(subParkArea);
//                        IEnumerable<CellArea> cellAreas = divideSubParkArea.Computer();
//                        allCellAreas.AddRange(divideSubParkArea.Computer());
//                    }

//                    foreach (SubParkArea subParkArea in subParkAreas)// singleLayoutRegions里面的数据与 Revit 元素无关，此时可以开启多线程进行工作
//                    {
//                        subParkArea.Computer();

//                        if (i == 0)// 单个子区域调试
//                        {// 单个子区域调试     
                           
//                        }// 单个子区域调试
//                        else
//                        {
//                            //continue;
//                        }
//                        i++;
                  
//                        int count01 = 0;
//                        List<PPLocPoint> vs01 = CalPs_EveryEdge(subParkArea, ref count01);

//                        // 兜圈机制           
//                        int count02 = 0;
//                        List<PPLocPoint> vs02 = CalPs_Backtrack(subParkArea, ref count02);

//                        // 取数量最大的
//                        if (count01 > count02)
//                            psLocPoints.AddRange(vs01);
//                        else
//                            psLocPoints.AddRange(vs02);
//                    }

//                    sw.Stop();
//                    NLogger.Instance.Trace($"{tempStr}-{time += 1}耗时 " + (sw.ElapsedMilliseconds / 1000.0).ToString() + " 秒。");
//                    sw.Restart();

//                    #region 开启停车事务

//                    (new PPLocPoint()).PointProjecToRevit(psLocPoints, doc, nowView);

//                    //string parkingTypeName = "停车位_" + App.MainWindow.pSWidth.Text + "*" + App.MainWindow.pSHeight.Text;//设定目标停车位族类型名字
//                    //FamilySymbol parkingType = doc.ParkingPlaceType(parkingTypeName);
//                    //foreach (var item in psLocAngPairs)
//                    //{
//                    //    Transcation_mehod.LayoutParking(doc, nowView, item.Key, item.Value, parkingType);// 当前放置族实例和旋转族实例分两步进行，不分开，会出现不明原因问题
//                    //    if (_progressWindow.Tracker.StopTask)
//                    //        break;
//                    //    _progressWindow.Tracker.Current++;

//                    //}
//                    #endregion
//                    sw.Stop();
//                    NLogger.Instance.Trace($"{tempStr}-{time += 1}耗时 " + (sw.ElapsedMilliseconds / 1000.0).ToString() + " 秒。");
//                    sw.Restart();

//                }
//            }

//        }
//        /// <summary>
//        /// 这里是对每个车道属性边的平行边处理，不需要累加，需要求出最大值（从这里入手，进行多方案对比）
//        /// </summary>
//        /// <returns></returns>
//        internal List<PPLocPoint> CalPs_EveryEdge(SubParkArea subParkArea, ref int count)
//        {
//            if (subParkArea.SelfBoundSegs.Count < 6 )// 1 == 大于5边的，全部采用兜圈处理;
//            {
//                if (subParkArea.RevitEleVeRas.Count < 1)// 2 == 里面包含坡道出入口的，也采用兜圈进行处理;
//                {
//                    count = 0;
//                    List<List<PPLocPoint>> pPLocPoints = new List<List<PPLocPoint>>();

//                    DeterBaseline areaDivider01 = new DeterBaseline(subParkArea);

//                    IEnumerable<SubCellArea> cellAreas = areaDivider01.Computer_Chd_EveryEdge();

//                    foreach (SubCellArea subCellArea in cellAreas)
//                    {
//                        subCellArea.Computer();

//                        PsCutClipFactory psArrayCutClip = new PsCutClipFactory(subCellArea, new PsArrCutClipVer());
//                        psArrayCutClip.Computer();
//                        pPLocPoints.Add(psArrayCutClip.TarPpPoints);// 获取各个停车位的停车位置，以及旋转角度
//                    }

//                    List<PPLocPoint> psLocPoints = pPLocPoints.OrderBy(p => p.Count).Last();
//                    count += psLocPoints.Count;
//                    return psLocPoints;
//                }
//                else
//                {
//                    count = 0;
//                    return new List<PPLocPoint>();
//                }
//            }
//            else
//            {
//                count = 0;
//                return new List<PPLocPoint>();
//            }
//        }
//        /// <summary>
//        /// 计算一个地库_子区域的停车数量 这里为兜圈 需要再次进行累加
//        /// </summary>
//        /// <returns></returns>
//        internal List<PPLocPoint> CalPs_Backtrack(SubParkArea subParkArea, ref int count)
//        {
//            List<PPLocPoint> psLocPoints = new List<PPLocPoint>();

//            DeterBaseline areaDivider02 = new DeterBaseline(subParkArea);
//            IEnumerable<SubCellArea> cellAreas = areaDivider02.Computer_Chd_Backtrack();

//            foreach (SubCellArea cellArea in cellAreas)
//            {
//                cellArea.Computer();
//                PsCutClipFactory psArrayCutClip = new PsCutClipFactory(cellArea, new PsArrCutClipLinearAuto());

//                psArrayCutClip.Computer();
//                psLocPoints.AddRange(psArrayCutClip.TarPpPoints);// 获取各个停车位的停车位置，以及旋转角度

//                count += psArrayCutClip.TarPpPoints.Count;// 车位数量 统计
//            }
//            return psLocPoints;
//        }
  
//    }
//}
