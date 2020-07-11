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

    class RegionAlgorithm
    {
        #region 输出数据
        /// <summary>
        /// 经过计算后的可停车区域 线圈点集List
        /// </summary>
        public Paths_xyz parkingRegionsPoints { get; set; }
        /// <summary>
        /// 经过计算后的可停车区域 线圈List
        /// </summary>
        public CurveArrArray parkingCurveArrArray { get; set; }
        /// <summary>
        /// 地库外墙线圈内的所有属性线
        /// </summary>
        public List<Curve> allCurves { get; set; }
        /// <summary>
        /// 拿到该范围框内所有的锁定车位 柱子
        /// </summary>
        public List<ElementId> fixedParkingFS { get; set; }
        /// <summary>
        /// 拿到该范围框内所有的未锁定车位 柱子
        /// </summary>
        public List<ElementId> unfixedParkingFS { get; set; }
        /// <summary>
        /// 拿到该范围框内所有的 常规模型 directshape
        /// </summary>
        public List<ElementId> directShapes { get; set; }
        /// <summary>
        /// 拿到该范围框内所有的 常规模型 directshape
        /// </summary>
        public List<ElementId> filledRegions { get; set; }
        /// <summary>
        /// 地库_单个填充区域填充线框
        /// </summary>
        public List<ElementId> filledRegions_singleParkignPlace { get; set; }
        /// <summary>
        /// 拿到该范围框内所有的 特殊障碍物，如线型坡道
        /// </summary>
        public List<ElementId> othersObstals { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 根据 地库_地库外墙线 计算可停车区域
        /// </summary>
        public RegionAlgorithm(Document doc, CurveArray controlRegionBaseWall)
        {

            RegionCalcution(doc, controlRegionBaseWall);//默认自动计算 输出数据字段

        }
        #endregion

        /// <summary>
        ///  1 过滤器筛选元素 2 通过区域锁定元素 3 使用clipper进行裁剪
        /// </summary>
        public void RegionCalcution(Document doc, CurveArray controlRegionBaseWall)
        {
            #region 获取目标region内的元素
            ElementCategoryFilter detailGroupFilter = new ElementCategoryFilter(BuiltInCategory.OST_IOSDetailGroups);
            //ElementCategoryFilter modelGroupFilter = new ElementCategoryFilter(BuiltInCategory.OST_IOSModelGroups);
            ElementCategoryFilter detailComponentsFilter = new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents);
            ElementCategoryFilter linesFilter = new ElementCategoryFilter(BuiltInCategory.OST_Lines);
            ElementCategoryFilter genericModel = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(new List<ElementFilter>() { detailGroupFilter, detailComponentsFilter, linesFilter, genericModel });

            List<Element> allTarEles = (new FilteredElementCollector(doc, doc.ActiveView.Id)).WherePasses(logicalOrFilter).WhereElementIsNotElementType().ToElements().ToList();
            List<Element> selEles = _Methods.GetTarlElesByRegion(doc, controlRegionBaseWall, allTarEles);
            #endregion

            #region 筛选 地库_地库外墙线圈中的目标元素 障碍物 主次道路中线 处理地库_地库外墙线退距 默认800mm

            #region 基于各属性进行目标元素抓取 
            CurveArray roadCurves = new CurveArray();
            CurveArray mainRoadCurves = RegionConditionGrab(doc, selEles, out roadCurves);

            #endregion

            CurveLoop baseWallLoop = new CurveLoop();
            bool isLoop = _Methods.IsCurveLoop(controlRegionBaseWall, out baseWallLoop);

            CurveArrArray baseWallCurveArrArray = new CurveArrArray();
            XYZ normal = new XYZ(0, 0, 1);
            double offsetDistance = 0;
            bool isCounterclockwise = baseWallLoop.IsCounterclockwise(normal);//curveloop内外偏移的依据
            if (isCounterclockwise)
                offsetDistance = -CMD.basementWall_offset_distance;
            else if (!isCounterclockwise)
                offsetDistance = CMD.basementWall_offset_distance;

            CurveLoop curveLoop = CurveLoop.CreateViaOffset(baseWallLoop, offsetDistance, normal);
            CurveArray _controlRegion = _Methods.CurveLoopToCurveArray(curveLoop);
            baseWallCurveArrArray.Append(_controlRegion);
            #endregion

            #region clipper最后图形处理 得到各个可停车区域region
            Paths basementWallPath = clipper_methods.Paths_xyzToPaths(_Methods.GetUniqueXYZFromCurves(baseWallCurveArrArray));
            Paths roadRegion = HandleMainRoadSpace(mainRoadCurves, roadCurves, CMD.Wd_main, CMD.Wd);
            Paths canPlacedRegion = clipper_methods.RegionCropctDifference(basementWallPath, roadRegion);//得到可停车区域
            this.parkingRegionsPoints = clipper_methods.PathsToPaths_xyz(canPlacedRegion);

            List<List<Line>> listLines = _Methods.GetListClosedtLineFromListPoints(this.parkingRegionsPoints);
            this.parkingCurveArrArray = _Methods.ListLinesToCurveArrArray(listLines);
            #endregion

        }
        /// <summary>
        /// 根据 地库外墙线线圈 找到 圈内的 障碍物区域 主次车道中心线
        /// </summary>
        public CurveArray RegionConditionGrab(Document doc, List<Element> selEles, out CurveArray roadCurves)
        {
            var obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");

            CurveArray mianRoadCurves = new CurveArray();
            List<CurveLoop> ObstacleLoops = new List<CurveLoop>();
            roadCurves = new CurveArray();
            List<ElementId> fixedParkingFS = new List<ElementId>();
            List<ElementId> unfixedParkingFS = new List<ElementId>();
            List<ElementId> othersObstals = new List<ElementId>();
            List<ElementId> directShapes = new List<ElementId>();
            List<ElementId> filledRegions = new List<ElementId>();
            List<ElementId> filledRegions_singleParkignPlace = new List<ElementId>();
            List<Curve> allCurves = new List<Curve>();

            #region 找到地库_地库外墙线的填充区域Id
            //ElementId baseWallFilledRegionId = new ElementId(-1);
            //foreach (Element _ele in selEles)
            //{
            //    if (_ele is Group)
            //    {
            //        Group _group = _ele as Group;
            //        if (_group.Name.Contains("地库外墙线"))
            //        {
            //            List<ElementId> elementIds = _group.GetMemberIds().ToList();
            //            List<Element> elements = _Methods.EleIdsToEles(doc, elementIds);
            //            elements.ForEach(p =>
            //            {
            //                if (p is FilledRegion) baseWallFilledRegionId = p.Id ;
            //            });
            //        }
            //    }
            //}
            #endregion

            foreach (Element _ele in selEles)
            {
                if (_ele is DetailCurve)
                {
                    DetailCurve detailCurve = _ele as DetailCurve;
                    Curve _c = detailCurve.GeometryCurve;
                    GraphicsStyle graphicsStyle = detailCurve.LineStyle as GraphicsStyle;
                    if (graphicsStyle.Name == "地库_主车道中心线")
                    {
                        _c.SetGraphicsStyleId(graphicsStyle.Id);
                        mianRoadCurves.Append(_c);
                    }
                    else if (graphicsStyle.Name == "地库_次车道中心线")
                    {
                        _c.SetGraphicsStyleId(graphicsStyle.Id);
                        roadCurves.Append(_c);
                    }
                }
                else if (_ele is FilledRegion)
                {
                    string filledRegionTypeName = doc.GetElement(_ele.GetTypeId()).Name;
                    if (filledRegionTypeName == "地库_单个停车区域")
                    {
                        filledRegions_singleParkignPlace.Add(_ele.Id);
                    }
                    else if (filledRegionTypeName.Contains("结构轮廓"))
                    {
                        FilledRegion filledRegion = _ele as FilledRegion;
                        filledRegions.Add(_ele.Id);
                        ObstacleLoops.AddRange(filledRegion.GetBoundaries());
                    }
                }
                else if (_ele is FamilyInstance)
                {
                    if (_ele.Name == "停车位_详图线")
                    {
                        if (_ele.get_Parameter(CMD.parkingFixedGid).AsValueString() == "是")
                        {
                            fixedParkingFS.Add(_ele.Id);
                        }
                        else if (_ele.get_Parameter(CMD.parkingFixedGid).AsValueString() == "否")
                        {
                            unfixedParkingFS.Add(_ele.Id);
                        }
                    }
                    else if (_ele.Name.Contains("坡道-直线"))
                    {
                        othersObstals.Add(_ele.Id);

                        double wight = Methods.MilliMeterToFeet(6000) / 2;
                        double height = Methods.MilliMeterToFeet(12000);
                        Transform transform = (_ele as FamilyInstance).GetTransform();
                        XYZ leftDownPoint = new XYZ(0, -wight, 0);
                        XYZ leftUpPoint = new XYZ(0, wight, 0);
                        XYZ rightUpPoint = new XYZ(height, wight, 0);
                        XYZ rightDownPoint = new XYZ(height, -wight, 0);
                        XYZ _leftDownPoint = transform.OfPoint(leftDownPoint);
                        XYZ _leftUpPoint = transform.OfPoint(leftUpPoint);
                        XYZ _rightUpPoint = transform.OfPoint(rightUpPoint);
                        XYZ _rightDownPoint = transform.OfPoint(rightDownPoint);

                        Curve curve = Line.CreateBound(_leftDownPoint, _leftUpPoint) as Curve;
                        Curve curve1 = Line.CreateBound(_leftUpPoint, _rightUpPoint) as Curve;
                        Curve curve2 = Line.CreateBound(_rightUpPoint, _rightDownPoint) as Curve;
                        Curve curve3 = Line.CreateBound(_rightDownPoint, _leftDownPoint) as Curve;

                        curve.SetGraphicsStyleId(obstacleLineStyleId);
                        curve1.SetGraphicsStyleId(obstacleLineStyleId);
                        curve2.SetGraphicsStyleId(obstacleLineStyleId);
                        curve3.SetGraphicsStyleId(obstacleLineStyleId);

                        allCurves.Add(curve);
                        allCurves.Add(curve);
                        allCurves.Add(curve);
                        allCurves.Add(curve);

                    }
                }
                else if (_ele is DirectShape)
                    directShapes.Add(_ele.Id);
            }

            #region 抓取属性线 将数据反馈给类字段
            foreach (Curve item in mianRoadCurves) allCurves.Add(item);
            foreach (Curve item in roadCurves) allCurves.Add(item);

            this.fixedParkingFS = fixedParkingFS;
            this.unfixedParkingFS = unfixedParkingFS;
            this.filledRegions = filledRegions;
            this.filledRegions_singleParkignPlace = filledRegions_singleParkignPlace;
            this.othersObstals = othersObstals;
            this.directShapes = directShapes;
            #endregion

            #region 设置obstal curve的属性
            foreach (CurveLoop curveLoop in ObstacleLoops)
            {
                foreach (Curve curve in curveLoop)
                {
                    curve.SetGraphicsStyleId(obstacleLineStyleId);
                    allCurves.Add(curve);
                }
            }
            this.allCurves = allCurves;
            #endregion

            return mianRoadCurves;
        }

        /// <summary>
        /// 处理主通道占据空间 直接 转义为 clipper 语境
        /// </summary>
        public Paths HandleMainRoadSpace(CurveArray mianRoadCurves, CurveArray roadCurves, double MainRoadWidth, double roadWidth)
        {
            int mainRoadCurvesCount = mianRoadCurves.Size;
            Paths mainRoadRegion = new Paths();//地库主车道占据空间
            if (mainRoadCurvesCount == 0)
            {
                return mainRoadRegion;
            }
            else if (mainRoadCurvesCount > 0)
            {
                Paths wdMainOffset = new Paths();//拿到所有的主车道偏移后的围合点，需要将围合区域做并集
                foreach (Curve _curve in mianRoadCurves)
                {
                    wdMainOffset.Add(clipper_methods._GetOffsetPonts_clipper_line(_curve, MainRoadWidth / 2));//添加每根车道中心线偏移后的矩形点集
                }
                foreach (Curve _curve in roadCurves)
                {
                    wdMainOffset.Add(clipper_methods._GetOffsetPonts_clipper_line(_curve, roadWidth / 2));//添加每根车道中心线偏移后的矩形点集
                }
                Clipper cTemp = new Clipper();//开展 所有线段的偏移矩形点击进行 clipper 合并
                cTemp.AddPaths(wdMainOffset, PolyType.ptSubject, true);
                cTemp.AddPath(wdMainOffset.First(), PolyType.ptClip, true);
                cTemp.Execute(ClipType.ctUnion, mainRoadRegion, PolyFillType.pftNonZero, PolyFillType.pftNonZero);//需要把每根主车道偏移出的空间，进行合并
            }
            return mainRoadRegion;
        }
        /// <summary>
        /// 在有元素内 抓取所有障碍物线圈
        /// </summary>
        public List<CurveLoop> RegionGrabobstaclesLineLoop(Document doc, List<Element> selEles)
        {
            List<CurveLoop> obstacleLineLoops = new List<CurveLoop>();

            foreach (Element _ele in selEles)
            {
                if (_ele is FilledRegion)
                {
                    if (_ele.Name == "详图填充区域")
                    {
                        FilledRegion filledRegion = _ele as FilledRegion;
                        obstacleLineLoops.AddRange(filledRegion.GetBoundaries());
                    }
                }
            }
            return obstacleLineLoops;
        }

        /// <summary>
        /// 详图线在区域内 或 与区域边界线相交
        /// </summary>
        public bool isDetaillineInorIntersectRegion(DetailLine detailLine, CurveArray _curveArray)
        {
            bool isInRegion = false;

            XYZ endPoint01 = detailLine.GeometryCurve.GetEndPoint(0);
            XYZ endPoint02 = detailLine.GeometryCurve.GetEndPoint(1);
            // z值 归零
            XYZ _endPoint01 = new XYZ(endPoint01.X, endPoint01.Y, 0);
            XYZ _endPoint02 = new XYZ(endPoint02.X, endPoint02.Y, 0);
            if (_Methods.IsInsidePolygon(_endPoint01, _curveArray) || _Methods.IsInsidePolygon(_endPoint02, _curveArray))
            {
                isInRegion = true;
            }
            return isInRegion;
        }
    }//class
}//namespace
