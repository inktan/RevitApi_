using System;
using System.Collections.Generic;
using Form_ = System.Windows.Forms;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.IFC;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LayoutParkingEffcient
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;
    using Path_line = List<Line>;
    using Paths_line = List<List<Line>>;

    class RequestHandlerDocumentChangeEvent : IExternalEventHandler
    {
        public string GetName()
        {
            return "RequestHandlerDocumentChangeEvent";
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 智能刷新
        /// </summary>
        /// <param name="uiapp"></param>
        public void Execute(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            ////需要通过外部事件，进行处理事务，一旦进入外部事件，需要首尾取消和重新注册 documentchange 事件
            //uiapp.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(RegionAlgorithm.MainRoadChanged);
            ////刷新函数
            //CurveArray _controlRegion = CMD.controlRegion;
            //CurveArrArray afterObstacleCurveArrArray = new CurveArrArray();
            //CurveArrArray afterCanPlacedRegion = RegionAlgorithm.CalculateTarPlaceParkingRegion(doc, _controlRegion, out afterObstacleCurveArrArray);
            //CMD.afterCanPlaceBoundaryPoints = RegionAlgorithm.canPlaceBoundaryPoints;

            //// 判断裁剪后的区域是否发生变化 对变化的区域 进行重新计算 sweep算法
            //// 判断两个paths_xyz是否相同，交集之外的before进行删除操作 after进行重新计算操作
            //Paths_xyz afterChangedRegionPoints = RegionAlgorithm.GetChangedRegion(CMD.afterCanPlaceBoundaryPoints, CMD.beforeCanPlaceBoundaryPoints);//得到的
            //if (afterChangedRegionPoints.Count < 1)
            //{
            //    return;
            //}
            //Paths_xyz beforeChangedRegionPoints = RegionAlgorithm.GetChangedRegion(CMD.beforeCanPlaceBoundaryPoints, CMD.afterCanPlaceBoundaryPoints);//之前的
            //Paths_line afterChangedRegionLines = _Methods.GetListClosedtLineFromListPoints(afterChangedRegionPoints);
            //Paths_line beforeChangedRegionLines = _Methods.GetListClosedtLineFromListPoints(beforeChangedRegionPoints);
            //CurveArrArray afterChangedRegionRegions = _Methods.ListLinesToCurveArrArray(afterChangedRegionLines);
            //CurveArrArray beforeChangedRegionRegions = _Methods.ListLinesToCurveArrArray(beforeChangedRegionLines);

            //// 开启事务 
            //// 删除所有停车位族实例 以及 directShape 该处 需要优化 需要调整的区域，才需要进行删除

            //TransactionNow _transaction = new TransactionNow(doc);
            //ParkingAlgorithm _parkingAlgorithm = new ParkingAlgorithm();

            //_transaction.DeleteAllParkingFsAndDirectShapeFromCurveArrArray(afterChangedRegionRegions);//删除被变动的区域
            //_transaction.DeleteAllParkingFsAndDirectShapeFromCurveArrArray(beforeChangedRegionRegions);//删除被变动的区域
            //                                                                                           //_transaction.DeleteAllParkingFsAndDirectShapeFromCurveArray(CMD.controlRegion);// 删除所有停车位族实例 以及 directShape 该处 需要优化 需要调整的区域，才需要进行删除
            //#region 停车位 requestHandler部分 与 RequestHandlerDocumentChangeEvent部分 相同
            //using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "自动计算排布车位"))//开展事务组 合并吸收
            //{
            //    if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
            //    {
            //        using (ParkingAlgorithm parkingAlgorithm = new ParkingAlgorithm())
            //        {
            //            int i = 0;
            //            foreach (CurveArray regionCurves in afterChangedRegionRegions)//针对单个区域进行停车位计算处理
            //            {
            //                CurveArray _regionCurves = regionCurves;
            //                _Methods.ShowTempGeometry(doc, _regionCurves);
            //                if (i == 11)
            //                {

            //                }
            //                //得到位于目标区域内的障碍物
            //                //存在一种情况，一个障碍物线圈的四个角点，以及中心点，都在目标区域外
            //                CurveArrArray nowObstacleCurveArrArray = _Methods.GetTarCurveArrArrayInTarRegion(_regionCurves, afterObstacleCurveArrArray);
            //                foreach (CurveArray curves in _Methods.GetTarRegionInTarCurveArrArray(_regionCurves, afterObstacleCurveArrArray))
            //                {
            //                    nowObstacleCurveArrArray.Append(curves);
            //                }
            //                foreach (CurveArray curves in _Methods.GetTarCurveArrArrayInTarRegionByintersect(_regionCurves, afterObstacleCurveArrArray))
            //                {
            //                    nowObstacleCurveArrArray.Append(curves);
            //                }
            //                //得到位于目标区域内的详图线
            //                List<DetailLine> nowRegionDetailLines = _Methods.GetAllDetailLinsInTarRegion(doc, _regionCurves);
            //                //将所有详图线转化为内存属性线
            //                List<Curve> nowRegionPropertyLines = _Methods.ConverDetalLinesToPropertyLines(nowRegionDetailLines);

            //                #region 对固定车位进行处理
            //                // 这里提取 已定位的车位区域，需要考虑 柱子的宽度 车道的宽度 
            //                List<Element> allFixedParkingFS = _transaction.GetAllFixedParkingFromCurveArray(doc, _regionCurves);

            //                Paths_xyz unionParkingSplace = RegionAlgorithm.GetParkingSpaceUnion(doc, allFixedParkingFS);

            //                List<List<Line>> allFixedParkingLines = _Methods.GetListClosedtLineFromListPoints(unionParkingSplace);
            //                CurveArrArray allFixedParkingCurveArray = _Methods.ListLinesToCurveArrArray(allFixedParkingLines);

            //                //将裁减出来的属性线添加进去，如何判断 基于车位裁剪后的边界线的属性
            //                foreach (CurveArray curves in allFixedParkingCurveArray)
            //                {
            //                    nowObstacleCurveArrArray.Append(curves);
            //                    //_Methods.ShowTempGeometry(doc, curves);
            //                }

            //                // 使用车位空间对目标区域进行裁剪

            //                Paths subjsPlace = clipper_methods.Paths_xyzToPaths(new Paths_xyz() { _Methods.GetUniqueXYZFromCurves(_regionCurves) });
            //                Paths clipsFiexedParking = clipper_methods.Paths_xyzToPaths(_Methods.GetUniqueXYZFromCurves(allFixedParkingCurveArray));

            //                Paths_xyz canPlacedRegion = clipper_methods.PathsToPaths_xyz(RegionAlgorithm.RegionCropctDifference(subjsPlace, clipsFiexedParking));

            //                if (canPlacedRegion.Count < 1)
            //                    continue;

            //                //_regionCurves = _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(canPlacedRegion.First()));
            //                #endregion

            //                List<Point> Max_tar_columnplaceXYZs = new List<Point>();
            //                List<Point> Max_tar_placeXYZs = parkingAlgorithm.Sweep(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            //                _transaction.LayoutParking(Max_tar_placeXYZs, Max_tar_columnplaceXYZs, CMD.layoutMethod);//开启放置事务
            //                i++;
            //                //break;
            //            }
            //        }
            //        transGroupCreateDwellingses.Assimilate();
            //    }
            //    else
            //    {
            //        transGroupCreateDwellingses.RollBack();
            //    }
            //}
            //#endregion

            ////停车区域点集需要更新
            ////CMD.DataStatistics(doc, _controlRegion);

            //CMD.beforeCanPlaceBoundaryPoints = CMD.afterCanPlaceBoundaryPoints;//停车区域点集需要更新
            //throw new NotImplementedException();
        }

        public void parkingAlgorithmCommon(UIApplication uiapp)
        {



        }

        //外部事件方法建立————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
    }
}
