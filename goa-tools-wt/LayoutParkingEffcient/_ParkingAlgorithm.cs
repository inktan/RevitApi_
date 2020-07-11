using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using wt_Common;
using ClipperLib;
using goa.Common;

namespace LayoutParkingEffcient
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;
    class ParkingAlgorithm : IDisposable
    {
        public int MyProperty { get; set; }
        //构造函数
        public ParkingAlgorithm() { }
        #region sweep
        public List<Point> tarSweep(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints)
        {
            tarColumnPoints = new List<Point>();
            List<Point> tarPlaceXYZs = new List<Point>();

            if (CMD.layoutMethod == "水平")
            {
                tarPlaceXYZs = SweepRecursion(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            else if (CMD.layoutMethod == "垂直")
            {
                tarPlaceXYZs = SweepVertiacl(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            return tarPlaceXYZs;
        }
        public List<Point> tarMaxSweep(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints)
        {
            List<Point> tarPlaceXYZs = new List<Point>();
            List<Point> tarPlaceXYZsVertiacl = new List<Point>();

            List<Point> tarMaxColumnPoints = new List<Point>();
            List<Point> tarMaxColumnPointsVertiacl = new List<Point>();

            tarColumnPoints = new List<Point>();

            tarPlaceXYZs = SweepMax(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarMaxColumnPoints, new SweepDelegate(SweepRecursion));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            tarPlaceXYZsVertiacl = SweepMax(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarMaxColumnPointsVertiacl, new SweepDelegate(SweepVertiacl));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，

            if (tarPlaceXYZs.Count >= tarPlaceXYZsVertiacl.Count)
            {
                CMD.layoutMethod = "水平";
                tarColumnPoints = tarMaxColumnPoints;
                return tarPlaceXYZs;
            }
            else
            {
                CMD.layoutMethod = "垂直";
                tarColumnPoints = tarMaxColumnPointsVertiacl;
                return tarPlaceXYZsVertiacl;
            }
        }
        /// <summary>
        /// 委托函数，常用
        /// </summary>
        /// <returns></returns>
        public delegate List<Point> SweepDelegate(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints);
        /// <summary>
        /// 基于方向的选择 进行车位最大排布
        /// </summary>
        /// <returns></returns>
        public List<Point> tarMaxSweepByDirection(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints, string layoutMethod)
        {
            List<Point> tarPlaceXYZs = new List<Point>();
            tarColumnPoints = new List<Point>();

            if (CMD.layoutMethod == "水平")
            {
                tarPlaceXYZs = SweepMax(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepRecursion));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            else if (CMD.layoutMethod == "垂直")
            {
                tarPlaceXYZs = SweepMax(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepVertiacl));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            else
            {
                TaskDialog.Show("error", "未选择停车方式");
            }
            return tarPlaceXYZs;
        }
        /// <summary>
        /// 通过判断选择点与选择停车区域角点的关系进行判断 目的旨在调整停车位排布的起点位置
        /// </summary>
        /// <returns></returns>
        public List<Point> tarMaxSweepByPointInTarRegion(Document doc, XYZ selPoint, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints, string layoutMethod)
        {
            List<Point> tarPlaceXYZs = new List<Point>();
            tarColumnPoints = new List<Point>();

            // 找到最近的角点
            XYZ leftDownPoint = _Methods.GetLeftDownXYZfromLines(_regionCurves);
            XYZ leftUpPoint = _Methods.GetLeftUpXYZfromLines(_regionCurves);
            XYZ rightUpPoint = _Methods.GetRightUpXYZfromLines(_regionCurves);
            XYZ rightDownPoint = _Methods.GetRightDownXYZfromLines(_regionCurves);

            List<XYZ> points = new Path_xyz() { leftDownPoint, leftUpPoint, rightUpPoint, rightDownPoint };

            // 求距离
            double leftDownDistance = selPoint.DistanceTo(leftDownPoint);
            double leftUpDistance = selPoint.DistanceTo(leftUpPoint);
            double rightUpDistance = selPoint.DistanceTo(rightUpPoint);
            double rightDownDistance = selPoint.DistanceTo(rightDownPoint);

            List<double> distances = new List<double>() { leftDownDistance, leftUpDistance, rightUpDistance, rightDownDistance };

            // 求最小距离
            int minDistance = distances.IndexOf(distances.Min());

            // 函数输入
            switch (minDistance)
            {
                case 0:
                    {
                        if (CMD.layoutMethod == "水平")
                        {
                            //tarPlaceXYZs = Sweep(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，

                            tarPlaceXYZs = SweepMaxLeftDown(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepRecursion));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else if (CMD.layoutMethod == "垂直")
                        {
                            tarPlaceXYZs = SweepMaxLeftDown(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepVertiacl));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else
                        {
                            TaskDialog.Show("error", "未选择停车方式");
                        }
                        break;
                    }
                case 1:
                    {
                        if (CMD.layoutMethod == "水平")
                        {
                            tarPlaceXYZs = SweepMaxLeftUpMirrorX(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepRecursion));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else if (CMD.layoutMethod == "垂直")
                        {
                            tarPlaceXYZs = SweepMaxLeftUpMirrorX(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepVertiacl));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else
                        {
                            TaskDialog.Show("error", "未选择停车方式");
                        }

                        break;
                    }
                case 2:
                    {
                        if (CMD.layoutMethod == "水平")
                        {
                            tarPlaceXYZs = SweepMaxRightUpMirrorXY(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepRecursion));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else if (CMD.layoutMethod == "垂直")
                        {
                            tarPlaceXYZs = SweepMaxRightUpMirrorXY(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepVertiacl));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else
                        {
                            TaskDialog.Show("error", "未选择停车方式");
                        }
                        break;
                    }
                case 3:
                    {
                        if (CMD.layoutMethod == "水平")
                        {
                            tarPlaceXYZs = SweepMaxLeftDownMirrorY(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepRecursion));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else if (CMD.layoutMethod == "垂直")
                        {
                            tarPlaceXYZs = SweepMaxLeftDownMirrorY(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarColumnPoints, new SweepDelegate(SweepVertiacl));//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                        }
                        else
                        {
                            TaskDialog.Show("error", "未选择停车方式");
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return tarPlaceXYZs;
        }
        /// <summary>
        /// 从区域四个角点 进行水平排布 从中提取出最大数量的车位排布布局
        /// </summary>
        public List<Point> SweepMax(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarMaxColumnPoints, SweepDelegate sweepDelegate)
        {
            List<Point> Max_tar_placeXYZs = new List<Point>();
            tarMaxColumnPoints = new List<Point>();
            // 排车位01 左下角点

            List<Point> _leftDownTarColumnPoints = new List<Point>();
            List<Point> _leftDowntarParkingPoints = SweepMaxLeftDown(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out _leftDownTarColumnPoints, sweepDelegate);

            //_leftDownTarColumnPoints = new Path_xyz();
            //List<Point> _leftDowntarParkingPoints = new List<Point>();

            // 排车位02 基于 左下角 进行 Y 轴镜像

            List<Point> _mirrorLefDownTarColumnPoints = new List<Point>();
            List<Point> _mirrorLefDowntarParkingPoints = SweepMaxLeftDownMirrorY(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out _mirrorLefDownTarColumnPoints, sweepDelegate);

            //_mirrorLefDownTarColumnPoints = new Path_xyz();
            //List<Point> _mirrorLefDowntarParkingPoints = new List<Point>();

            // 排车位03  基于 左上角 进行 X 轴镜像

            List<Point> _mirrorLefUpTarColumnPoints = new List<Point>();
            List<Point> _mirrorLefUptarParkingPoints = SweepMaxLeftUpMirrorX(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out _mirrorLefUpTarColumnPoints, sweepDelegate);

            //_mirrorLefUpTarColumnPoints = new Path_xyz();
            //List<Point> _mirrorLefUptarParkingPoints = new List<Point>();

            // 排车位04  基于 右上角 进行 X 轴 镜像 Y 轴 镜像

            List<Point> _mirrorrightUpTarColumnPoints = new List<Point>();
            List<Point> _mirrorrightUptarParkingPoints = SweepMaxRightUpMirrorXY(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out _mirrorrightUpTarColumnPoints, sweepDelegate);

            //_mirrorrightUpTarColumnPoints = new Path_xyz();
            //List<Point> _mirrorrightUptarParkingPoints = new List<Point>();

            //
            //求出所有方案的最大值
            //
            List<List<Point>> tarParkingPointses = new List<List<Point>>() { _leftDowntarParkingPoints, _mirrorLefDowntarParkingPoints, _mirrorLefUptarParkingPoints, _mirrorrightUptarParkingPoints };
            List<List<Point>> tarColumnPointses = new List<List<Point>>() { _leftDownTarColumnPoints, _mirrorLefDownTarColumnPoints, _mirrorLefUpTarColumnPoints, _mirrorrightUpTarColumnPoints };
            List<int> tarParkingPointsesount = new List<int>() { _leftDowntarParkingPoints.Count, _mirrorLefDowntarParkingPoints.Count, _mirrorLefUptarParkingPoints.Count, _mirrorrightUptarParkingPoints.Count };

            //List<Path_xyz> tarParkingPointses = new List<Path_xyz>() { _leftDowntarParkingPoints };
            //List<Path_xyz> tarColumnPointses = new List<Path_xyz>() { _leftDownTarColumnPoints  };
            //List<int> tarParkingPointsesount = new List<int>() { _leftDowntarParkingPoints.Count };

            int maxIndex = tarParkingPointsesount.IndexOf(tarParkingPointsesount.Max());
            Max_tar_placeXYZs = tarParkingPointses[maxIndex];
            tarMaxColumnPoints = tarColumnPointses[maxIndex];

            return Max_tar_placeXYZs;
        }

        #region 从四个角点排车位
        /// <summary>
        /// 从左下角进行排车位
        /// </summary>
        /// <returns></returns>
        public List<Point> SweepMaxLeftDown(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarMaxColumnPoints, SweepDelegate sweepDelegate)
        {
            tarMaxColumnPoints = new List<Point>();
            return sweepDelegate(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out tarMaxColumnPoints);
        }
        /// <summary>
        /// 基于 左下角 进行 Y 轴镜像
        /// </summary>
        /// <returns></returns>
        public List<Point> SweepMaxLeftDownMirrorY(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarMaxColumnPoints, SweepDelegate sweepDelegate)
        {
            tarMaxColumnPoints = new List<Point>();

            XYZ _leftDownPoint = _Methods.GetLeftDownXYZfromLines(_regionCurves);
            Plane _leftDownMirrorPlane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), _leftDownPoint);
            Transform mirrorleftDownTransform = Transform.CreateReflection(_leftDownMirrorPlane);

            CurveArray _mirrorleftDownRegionCurves = _Methods.TransformCurveArray(_regionCurves, mirrorleftDownTransform);
            CurveArrArray _mirrorleftDownnowObstacleCurves = _Methods.TransformCurveArray(nowObstacleCurveArrArray, mirrorleftDownTransform);

            List<Curve> _mirrorleftDownnowRegionPropertyLines = _Methods.TransformCurves(nowRegionPropertyLines, mirrorleftDownTransform);

            List<Point> _mirrorLefDownTarColumnPoints = new List<Point>();
            List<Point> _mirrorLefDowntarParkingPoints = sweepDelegate(doc, _mirrorleftDownRegionCurves, _mirrorleftDownnowObstacleCurves, _mirrorleftDownnowRegionPropertyLines, out _mirrorLefDownTarColumnPoints);

            tarMaxColumnPoints = _Methods.TransformPath_xyz(_mirrorLefDownTarColumnPoints, mirrorleftDownTransform);
            _mirrorLefDowntarParkingPoints = _Methods.TransformPath_xyz(_mirrorLefDowntarParkingPoints, mirrorleftDownTransform);

            return _mirrorLefDowntarParkingPoints;
        }
        /// <summary>
        /// 基于 左上角 进行 X 轴镜像
        /// </summary>
        /// <returns></returns>
        public List<Point> SweepMaxLeftUpMirrorX(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarMaxColumnPoints, SweepDelegate sweepDelegate)
        {
            tarMaxColumnPoints = new List<Point>();

            XYZ _leftUpPoint = _Methods.GetLeftUpXYZfromLines(_regionCurves);
            Plane _leftUpMirrorPlane = Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), _leftUpPoint);
            Transform mirrorleftUpTransform = Transform.CreateReflection(_leftUpMirrorPlane);

            CurveArray _mirrorleftUpRegionCurves = _Methods.TransformCurveArray(_regionCurves, mirrorleftUpTransform);
            CurveArrArray _mirrorleftUpNowObstacleCurves = _Methods.TransformCurveArray(nowObstacleCurveArrArray, mirrorleftUpTransform);

            List<Curve> _mirrorleftUpRegionPropertyLines = _Methods.TransformCurves(nowRegionPropertyLines, mirrorleftUpTransform);

            List<Point> _mirrorLefUpTarColumnPoints = new List<Point>();
            List<Point> _mirrorLefUptarParkingPoints = sweepDelegate(doc, _mirrorleftUpRegionCurves, _mirrorleftUpNowObstacleCurves, _mirrorleftUpRegionPropertyLines, out _mirrorLefUpTarColumnPoints);

            tarMaxColumnPoints = _Methods.TransformPath_xyz(_mirrorLefUpTarColumnPoints, mirrorleftUpTransform);
            _mirrorLefUptarParkingPoints = _Methods.TransformPath_xyz(_mirrorLefUptarParkingPoints, mirrorleftUpTransform);

            return _mirrorLefUptarParkingPoints;
        }
        /// <summary>
        ///  基于 右上角 进行 X 轴 镜像 Y 轴 镜像
        /// </summary>
        /// <returns></returns>
        public List<Point> SweepMaxRightUpMirrorXY(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarMaxColumnPoints, SweepDelegate sweepDelegate)
        {
            tarMaxColumnPoints = new List<Point>();

            XYZ _rightUpPoint = _Methods.GetRightUpXYZfromLines(_regionCurves);
            Plane _rightUpMirrorPlane_X = Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), _rightUpPoint);
            Plane _rightUpMirrorPlane_Y = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), _rightUpPoint);
            Transform mirrorrightUpTransform_X = Transform.CreateReflection(_rightUpMirrorPlane_X);
            Transform mirrorrightUpTransform_Y = Transform.CreateReflection(_rightUpMirrorPlane_Y);

            CurveArray _mirrorrightUpRegionCurves_X = _Methods.TransformCurveArray(_regionCurves, mirrorrightUpTransform_X);
            CurveArrArray _mirrorrightUpNowObstacleCurves_X = _Methods.TransformCurveArray(nowObstacleCurveArrArray, mirrorrightUpTransform_X);
            CurveArray _mirrorrightUpRegionCurves_Y = _Methods.TransformCurveArray(_mirrorrightUpRegionCurves_X, mirrorrightUpTransform_Y);
            CurveArrArray _mirrorrightUpNowObstacleCurves_Y = _Methods.TransformCurveArray(_mirrorrightUpNowObstacleCurves_X, mirrorrightUpTransform_Y);

            //_Methods.ShowTempGeometry(doc, _mirrorrightUpRegionCurves_Y);
            List<Curve> _mirrorrightUpRegionPropertyLines_X = _Methods.TransformCurves(nowRegionPropertyLines, mirrorrightUpTransform_X);
            List<Curve> _mirrorrightUpRegionPropertyLines_Y = _Methods.TransformCurves(_mirrorrightUpRegionPropertyLines_X, mirrorrightUpTransform_Y);

            List<Point> _mirrorrightUpTarColumnPoints = new List<Point>();
            List<Point> _mirrorrightUptarParkingPoints = sweepDelegate(doc, _mirrorrightUpRegionCurves_Y, _mirrorrightUpNowObstacleCurves_Y, _mirrorrightUpRegionPropertyLines_Y, out _mirrorrightUpTarColumnPoints);

            List<Point> _mirrorrightUpTarColumnPoints_X = _Methods.TransformPath_xyz(_mirrorrightUpTarColumnPoints, mirrorrightUpTransform_Y);
            List<Point> _mirrorrightUptarParkingPoints_X = _Methods.TransformPath_xyz(_mirrorrightUptarParkingPoints, mirrorrightUpTransform_Y);

            tarMaxColumnPoints = _Methods.TransformPath_xyz(_mirrorrightUpTarColumnPoints_X, mirrorrightUpTransform_X);
            _mirrorrightUptarParkingPoints = _Methods.TransformPath_xyz(_mirrorrightUptarParkingPoints_X, mirrorrightUpTransform_X);

            return _mirrorrightUptarParkingPoints;
        }
        #endregion

        /// <summary>
        /// 对目标进行 swepp 进而进行车位排布 垂直式
        /// </summary>
        public List<Point> SweepVertiacl(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints)
        {
            View acvtiView = doc.ActiveView;
            double rotateAngle = Math.PI / 2;

            XYZ _leftUpPoint = _Methods.GetLeftUpXYZfromLines(_regionCurves);
            Transform beforeTransform = Transform.CreateRotationAtPoint(new XYZ(0, 0, 1), rotateAngle, _leftUpPoint);//逆时针旋转90°
            Transform afterTransform = Transform.CreateRotationAtPoint(new XYZ(0, 0, 1), -rotateAngle, _leftUpPoint);//顺时针旋转90°

            //进行transform
            CurveArray afterCurves = _Methods.TransformCurveArray(_regionCurves, beforeTransform);//transform后的停车区域
            CurveArrArray afterObstacleCurves = _Methods.TransformCurveArray(nowObstacleCurveArrArray, beforeTransform);//旋转障碍物

            //_Methods.ShowTempGeometry(doc, afterObstacleCurves);

            List<Curve> afterRegionPropertyLines = _Methods.TransformCurves(nowRegionPropertyLines, beforeTransform);

            //基于 sweep 求出停车位中心点
            List<Point> tarPlaceXYZs = SweepRecursion(doc, afterCurves, afterObstacleCurves, afterRegionPropertyLines, out tarColumnPoints);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，

            //将计算好的车位排布点 transfrom 回来
            tarPlaceXYZs = _Methods.TransformPath_xyz(tarPlaceXYZs, afterTransform);
            tarColumnPoints = _Methods.TransformPath_xyz(tarColumnPoints, afterTransform);

            return tarPlaceXYZs;
        }

        #region sweepRecursion sweep函数递归
        /// <summary>
        /// 递归函数sweep，需要明确当前车位位于总体计算空间的第几行，以及车位相邻线的属性
        /// </summary>
        public List<Point> SweepRecursion(Document doc, CurveArray _regionCurves, CurveArrArray nowObstacleCurveArrArray, List<Curve> nowRegionPropertyLines, out List<Point> tarColumnPoints)
        {
            #region 输出数据-停车位位置点集-柱网位置点集
            List<Point> tarParkingPoints = new List<Point>();
            tarColumnPoints = new List<Point>();
            #endregion

            #region 辅助循环条件设置
            int sweepRowCount = 0;        // sweep运行到第几行，0为第一行，数据需要连续，保证最外层整个区域的完整性
            int parkingCount = 0;         // parkingCount为该行的第几个裁剪车位，数据需要连续，保证最外层整个区域的完整性
            #endregion

            #region 求出最原始条件，投影处理……

            #region 明确输入条件 1 计算边界、各个障碍物边界、所有的属性线（用于sweepLine相交处理）进行投影处理，投影函数会把Curve处理为Line，同时，要保证线圈的闭合性，用以判断车位空间与障碍物线圈间的关系；
            List<Line> projectRegionLines = _Methods.ProjectCurveArray(_regionCurves);
            List<List<Line>> projectObstalLines = _Methods.ProjectCurveArrArray(nowObstacleCurveArrArray);
            List<Line> nowProJectRegionPropertyLines = _Methods.ProjectLines(nowRegionPropertyLines);
            CurveArray nowProjectRegionLines = _Methods.LinesToCurveArray(projectRegionLines);//用于判断车位是否与障碍物边界冲突
            CurveArrArray nowProjectObstalCurveArrArray = _Methods.ListLinesToCurveArrArray(projectObstalLines);//用于判断车位是否与障碍物边界冲突

            Path_xyz circularPoints = _Methods.GetUniqueXYZFromLines(projectRegionLines);
            circularPoints = _Methods.Pointlistdeduplication(clipper_methods.RemoveSingleLineInPoints(circularPoints));// 起始边界点集 ***
            XYZ startLeftDownXYZ = _Methods.GetLeftDownXYZfromPoints(circularPoints);// 点集所在矩形，左下角point

            #endregion

            #region 明确最下和最上位置边界线的属性，由于边界线源于clipper裁剪，不存在属性，因此需要判断边界线的属性

            Line basicDownHorizantalLine = _Methods.GetBottomLineFromLines(projectRegionLines);
            //Line basicUpHorizantalLine = _Methods.GetBottomLineFromLines(projectRegionLines);

            ConditionLine basicDownHorizantalCLine = GetCLine(doc, basicDownHorizantalLine, nowProJectRegionPropertyLines);//获取基准起始属性线，问题为地库_地库外墙真实边界线为退距线（800mm），会导致找不到与其重叠的属性线
            //ConditionLine basicUpHorizantalCLine = GetCLine(doc, basicUpHorizantalLine, nowProJectRegionPropertyLines);//获取基准起始属性线
            #endregion

            #endregion

            tarParkingPoints = Sweep(doc, startLeftDownXYZ, nowProjectRegionLines, nowProjectObstalCurveArrArray, nowProJectRegionPropertyLines, basicDownHorizantalCLine, sweepRowCount, parkingCount, out tarColumnPoints);

            return tarParkingPoints;
        }
        #endregion

        /// <summary>
        /// 对目标进行 swepp 进而进行车位排布 水平式。在clipper进行中，一个图形会被裁剪成两个部分，需要把多余的那个部分拿出来进行处理，遇到的问题（新的图形起点水平线属性需要判断），将行数数据进行传递
        /// </summary>
        /// <param name="doc">当前活跃文档</param>
        /// <param name="_regionCurves"></param>
        /// <param name="nowObstacleCurveArrArray"></param>
        /// <param name="nowRegionPropertyLines"></param>
        /// <param name="tarColumnPoints"></param>
        /// <returns></returns>
        public List<Point> Sweep(Document doc, XYZ startLeftDownXYZ, CurveArray nowProjectRegionLines, CurveArrArray nowProjectObstalCurveArrArray, List<Line> nowProJectRegionPropertyLines, ConditionLine basicDownHorizantalCLine, int sweepRowCount, int parkingCount, out List<Point> tarColumnPoints)
        {
            #region 输出数据-停车位位置点集-柱网位置点集
            List<Point> tarParkingPoints = new List<Point>();
            tarColumnPoints = new List<Point>();
            #endregion

            #region 从静态数据中，提取数据
            double height = CMD.parkingPlaceHeight;
            double wight = CMD.parkingPlaceWight;
            double roadWight = CMD.Wd;
            double columnHeight = CMD.columnWidth;
            double columnWidth = CMD.columnWidth;
            double columnBufferDistance = CMD.columnBurfferDistance;
            #endregion

            #region 明确输入条件 1 计算边界、各个障碍物边界、所有的属性线（用于sweepLine相交处理）进行投影处理，投影函数会把Curve处理为Line，同时，要保证线圈的闭合性，用以判断车位空间与障碍物线圈间的关系；

            Path_xyz circularPoints = _Methods.GetUniqueXYZFromLines(_Methods.CurveArrayToListLine(nowProjectRegionLines));
            circularPoints = _Methods.Pointlistdeduplication(clipper_methods.RemoveSingleLineInPoints(circularPoints));// 起始边界点集 ***

            #endregion

            #region 辅助循环条件设置
            XYZ moveHeidht_leftDownXYZ = startLeftDownXYZ;// moveHeidht_leftDownXYZ 目的为，让车位可以往上移动，该xyz需要递归传递，保证递归新区域的起点位置（x，y）和height的正确性，用于符合 sweepRowCount,与 parkingCount的数值
            ParkingSpaceUnit clipperParkingSpaceUnit = new ParkingSpaceUnit(startLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, false); // 该变量用于裁剪
            ParkingSpaceUnit _parkingSpaceUnit = clipperParkingSpaceUnit; // 该变量用于停车
            bool doesHasColumn = false;   // 需要不要柱子，默认不加柱子
            #endregion

            #region 启动For循环 循环次数给极大值，单个区域裁剪结束，会自动跳出循环

            for (int i = 0; i < 20000; i++)
            {
                #region 停止控制
                if (CMD.stopAlgorithm == false) break;
                #endregion
                //CMD.TestList.Add(i.ToString());
                #region 更新循环条件 判断裁剪车位当前位于第几行 第几列 由左至右 由上至下

                XYZ circularLeftDownXYZ = _Methods.GetLeftDownXYZfromPoints(circularPoints);// 左下角裁剪位置更新 该数据用于判断裁剪高度，与放置车位class无关
                List<Line> circularRegionLines = _Methods.GetClosedLinesFromPoints(circularPoints);// 被裁剪区域得到更新

                if (Math.Abs(circularLeftDownXYZ.Y - startLeftDownXYZ.Y - height) <= _Methods.PRECISION)
                {
                    sweepRowCount += 1;
                    parkingCount = 0;
                    moveHeidht_leftDownXYZ = moveHeidht_leftDownXYZ + new XYZ(0, height, 0);// 处理裁剪车位Z轴高度移动问题
                }
                else if (Math.Abs(circularLeftDownXYZ.Y - startLeftDownXYZ.Y - height - roadWight) <= _Methods.PRECISION)
                {
                    sweepRowCount += 1;
                    parkingCount = 0;
                    moveHeidht_leftDownXYZ = moveHeidht_leftDownXYZ + new XYZ(0, height + roadWight, 0);
                }

                startLeftDownXYZ = AddWifhtAndColumnParkingPoint(moveHeidht_leftDownXYZ, wight, columnWidth, columnBufferDistance, parkingCount);// 基于每一行的最左边起点在该行的列数parkingCount，计算当前裁剪车位的位置
                #endregion

                #region 创建sweepLine，得到相交列表（包括相交点，相见线段）
                XYZ sweepUpPoint = new XYZ(0, 2500, 0);//垂直sweepLine上、下端点高度
                XYZ sweepDownPoint = new XYZ(0, -2500, 0);

                XYZ leftBuffer = new XYZ(_Methods.PRECISION, 0, 0);
                XYZ rightBuffer = new XYZ(wight - _Methods.PRECISION, 0, 0);
                XYZ columnBuffer = new XYZ(wight + columnWidth + columnBufferDistance * 2, 0, 0);

                Line sweepLeftLine = Line.CreateBound(startLeftDownXYZ + sweepUpPoint + leftBuffer, startLeftDownXYZ + sweepDownPoint + leftBuffer);
                Line sweepLeftRight = Line.CreateBound(startLeftDownXYZ + sweepUpPoint + rightBuffer, startLeftDownXYZ + sweepDownPoint + rightBuffer);
                if (parkingCount % 3 == 0)
                    sweepLeftRight = Line.CreateBound(startLeftDownXYZ + sweepUpPoint + rightBuffer + columnBuffer, startLeftDownXYZ + sweepDownPoint + rightBuffer + columnBuffer);

                List<ConditionLine> intersectBoundriesAndPointsVerticalLeft = GetIntersectBoundriesCLlines(doc, sweepLeftLine, circularRegionLines, nowProJectRegionPropertyLines, "Vertical");// 被裁减边界线与sweppLine的相交情况
                //List<ConditionLine> intersectBoundriesAndPointsVerticalRight = GetIntersectBoundriesCLlines(doc, sweepLeftRight, circularRegionLines, nowProJectRegionPropertyLines, "Horizantal");// 被裁减边界线与sweppLine的相交情况
                #endregion

                if (intersectBoundriesAndPointsVerticalLeft.Count > 1)// 此处判断，是因为sweepLine与计算区域无相交线产生
                {
                    #region 判断当前被裁减区域，与sweepLine相交最下边界属性线
                    ConditionLine downClLine = intersectBoundriesAndPointsVerticalLeft.First();
                    ConditionLine upClLine = intersectBoundriesAndPointsVerticalLeft.Last();
                    double upDownDistance = upClLine.conditionPoint.xyz.Y - downClLine.conditionPoint.xyz.Y;//底部 边界与紧挨边界的距离
                    #endregion

                    if (upDownDistance > height - _Methods.PRECISION)// 上下距离大于停车位高度
                    {
                        #region 判断停车位相邻下边界线属性
                        if (basicDownHorizantalCLine.lineStyleId == LineStyleId.MainRoadId)//最下水平边界属性决定第一排车位的形式 是单独车位 还是车位+车道
                        {
                            if (sweepRowCount % 2 == 0)//余数为零，为偶数次排布车位 车位放置的下边界为 车道边界
                            {
                                downClLine.lineStyleId = LineStyleId.MainRoadId;
                            }
                            else if (sweepRowCount % 2 == 1)//余数为一，为奇数次排布车位 车位放置的下边界为 障碍物-已停车位边界
                            {
                                downClLine.lineStyleId = LineStyleId.CarTailId;
                            }
                        }
                        else if (basicDownHorizantalCLine.lineStyleId == LineStyleId.basementWallId || basicDownHorizantalCLine.lineStyleId == LineStyleId.ObstacleCurveId)
                        {
                            if (sweepRowCount % 2 == 0)//余数为零，为偶数次排布车位 车位放置的下边界为 车道边界
                            {
                                downClLine.lineStyleId = LineStyleId.CarTailId;
                            }
                            else if (sweepRowCount % 2 == 1)//余数为一，为奇数次排布车位 车位放置的下边界为 障碍物-已停车位边界
                            {
                                downClLine.lineStyleId = LineStyleId.MainRoadId;
                            }
                        }
                        #endregion

                        #region 基于sweepLine与被裁减区域的相交情况，确定是单独车位，还是车位+通道

                        XYZ newStartLeftDownXYZ = startLeftDownXYZ;// 由于车位的位置可能会发生移动，因此，这里使用临时变量存储数据

                        if (downClLine.lineStyleId == LineStyleId.MainRoadId)//首 相交点
                        {
                            _parkingSpaceUnit = AddColumnParking(startLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);

                            if (upClLine.lineStyleId == LineStyleId.MainRoadId)
                            {
                                if (Math.Abs(upClLine.line.Direction.X) > 1 - _Methods.PRECISION && Math.Abs(upClLine.line.Direction.Y) < _Methods.PRECISION)//判断顶端是不是水平线，不是水平线，不做车位向上移动处理
                                {
                                    if (upDownDistance < height * 2 - _Methods.PRECISION && upDownDistance > height - _Methods.PRECISION)
                                    {
                                        newStartLeftDownXYZ = startLeftDownXYZ + new XYZ(0, upDownDistance - height, 0);
                                        _parkingSpaceUnit = AddColumnParking(newStartLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);
                                    }
                                }
                            }
                        }
                        else if (downClLine.lineStyleId == LineStyleId.CarTailId)//首 相交点
                        {
                            _parkingSpaceUnit = AddColumnParkingAndMainRoad(startLeftDownXYZ, height, wight, roadWight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);

                            if (upClLine.lineStyleId == LineStyleId.MainRoadId) //最下边界为 障碍物 属性 通车道 最上为 通车道 
                            {
                                //开展 间距问题 此处设置为水平线，才需要处理间距问题
                                if (Math.Abs(upClLine.line.Direction.X) > 1 - _Methods.PRECISION && Math.Abs(upClLine.line.Direction.Y) < _Methods.PRECISION)
                                {
                                    if (upDownDistance < height * 2 - _Methods.PRECISION && upDownDistance > height - _Methods.PRECISION)
                                    {
                                        newStartLeftDownXYZ = startLeftDownXYZ + new XYZ(0, upDownDistance - height, 0);
                                        _parkingSpaceUnit = AddColumnParking(newStartLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);
                                    }
                                    else if (upDownDistance < height * 3 - _Methods.PRECISION && upDownDistance > height * 2 - _Methods.PRECISION)
                                    {
                                        newStartLeftDownXYZ = startLeftDownXYZ + new XYZ(0, upDownDistance - height, 0);
                                        _parkingSpaceUnit = AddColumnParking(newStartLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);
                                    }
                                }
                            }
                        }
                        else//需要注意，当出现不符合上述属性线组合的情况下，要进行报错提示
                        {
                            _Methods.TaskDialogShowMessage("部分区域，车位排布出项错误，请设计师查看线样式是否设置正确。");
                            break;
                        }
                        #endregion

                        #region 判断当前车位的下方，是不是有障碍物存在，该处比较特殊

                        bool isNotBottomHasObstalLeft = false;
                        bool isNotBottomHasObstalRight = false;

                        if (basicDownHorizantalCLine.lineStyleId == LineStyleId.MainRoadId)//最下水平边界属性决定第一排车位的形式 是单独车位 还是车位+车道
                        {
                            if (sweepRowCount % 2 == 0)//余数为零，为偶数次排布车位 车位放置的下边界为 车道边界
                            {
                                List<ConditionLine> AllIntersectPropertyLinesAndPointsVerticalLeft = GetAllIntersectPropertyLinesAndPoints(doc, sweepLeftLine, nowProJectRegionPropertyLines, "Vertical");// 被裁减边界线与sweppLine的相交情况
                                List<ConditionLine> AllIntersectPropertyLinesAndPointsVerticalRight = GetAllIntersectPropertyLinesAndPoints(doc, sweepLeftRight, nowProJectRegionPropertyLines, "Horizantal");// 被裁减边界线与sweppLine的相交情况

                                if (upDownDistance > height * 2 - _Methods.PRECISION)//需要在属性线列表中，找到与停车位基点，相邻最近的上下两个点
                                {
                                    isNotBottomHasObstalLeft = isObstalUnderParking(startLeftDownXYZ, AllIntersectPropertyLinesAndPointsVerticalLeft, roadWight, height);
                                    isNotBottomHasObstalRight = isObstalUnderParking(startLeftDownXYZ, AllIntersectPropertyLinesAndPointsVerticalRight, roadWight, height);
                                }
                            }
                        }
                        else if (basicDownHorizantalCLine.lineStyleId == LineStyleId.basementWallId
                            || basicDownHorizantalCLine.lineStyleId == LineStyleId.ObstacleCurveId)// 与上条件判断结果刚好相反
                        {
                            if (sweepRowCount % 2 == 1)
                            {
                                List<ConditionLine> AllIntersectPropertyLinesAndPointsVerticalLeft = GetAllIntersectPropertyLinesAndPoints(doc, sweepLeftLine, nowProJectRegionPropertyLines, "Vertical");// 被裁减边界线与sweppLine的相交情况
                                List<ConditionLine> AllIntersectPropertyLinesAndPointsVerticalRight = GetAllIntersectPropertyLinesAndPoints(doc, sweepLeftRight, nowProJectRegionPropertyLines, "Horizantal");// 被裁减边界线与sweppLine的相交情况

                                if (upDownDistance > height * 2 - _Methods.PRECISION)//需要在属性线列表中，找到与停车位基点，相邻最近的上下两个点
                                {
                                    isNotBottomHasObstalLeft = isObstalUnderParking(startLeftDownXYZ, AllIntersectPropertyLinesAndPointsVerticalLeft, roadWight, height);
                                    isNotBottomHasObstalRight = isObstalUnderParking(startLeftDownXYZ, AllIntersectPropertyLinesAndPointsVerticalRight, roadWight, height);
                                }
                            }
                        }

                        #endregion

                        #region 判断当前停车区域最上线段是否满足停车位宽度
                        bool isLastLineLength = false;
                        if (upClLine.line.Length - wight > -_Methods.PRECISION) //判断最上方边界线，是否满足一个车的宽度
                            isLastLineLength = true;
                        #endregion

                        #region 取出停车位所在位置中心点
                        if (isLastLineLength && !isNotBottomHasObstalLeft && !isNotBottomHasObstalRight)//最上边界不够停车宽度
                        {
                            using (ParkingPlaceVertical parkingPlaceVertical = new ParkingPlaceVertical(_parkingSpaceUnit))
                            {
                                if (parkingPlaceVertical.isCarFourcornerInCanPlacedRegion(nowProjectRegionLines, nowProjectObstalCurveArrArray)) // 判断车位空间，是否与障碍物产生冲突
                                {
                                    #region 提取停车位放置点和车位朝向
                                    if (basicDownHorizantalCLine.lineStyleId == LineStyleId.MainRoadId)//最下水平边界属性决定第一排车位的形式 是单独车位 还是车位+车道
                                    {
                                        if (sweepRowCount % 2 == 0)//余数为零，为偶数次排布车位 车位放置的下边界为 车道边界
                                        {
                                            tarParkingPoints.Add(Point.Create(_parkingSpaceUnit.middlePoint, new ElementId(-122543112)));//车头在南侧elementId -122543112
                                        }
                                        else if (sweepRowCount % 2 == 1)//余数为一，为奇数次排布车位 车位放置的下边界为 障碍物-已停车位边界
                                        {
                                            tarParkingPoints.Add(Point.Create(_parkingSpaceUnit.middlePoint, new ElementId(-21136)));//车头在北侧 elementId -21136
                                        }
                                    }
                                    else if (basicDownHorizantalCLine.lineStyleId == LineStyleId.basementWallId
                                        || basicDownHorizantalCLine.lineStyleId == LineStyleId.ObstacleCurveId)// 与上条件判断结果刚好相反
                                    {
                                        if (sweepRowCount % 2 == 0)
                                        {
                                            tarParkingPoints.Add(Point.Create(_parkingSpaceUnit.middlePoint, new ElementId(-21136)));
                                        }
                                        else if (sweepRowCount % 2 == 1)
                                        {
                                            tarParkingPoints.Add(Point.Create(_parkingSpaceUnit.middlePoint, new ElementId(-122543112)));
                                        }
                                    }
                                    #endregion

                                    #region 当前判断，如果车位无法放置，则相邻的左下角柱子，也无法放置
                                    if (doesHasColumn == true)
                                    {
                                        if (downClLine.lineStyleId == LineStyleId.MainRoadId)
                                        {
                                            _parkingSpaceUnit.upColunmPoint = _parkingSpaceUnit.upColunmPoint + new XYZ(0, columnHeight / 2, 0);
                                        }
                                        else if (downClLine.lineStyleId == LineStyleId.CarTailId)
                                        {
                                            _parkingSpaceUnit.bottomColunmPoint = _parkingSpaceUnit.bottomColunmPoint - new XYZ(0, columnHeight / 2, 0);
                                        }
                                        tarColumnPoints.Add(Point.Create(_parkingSpaceUnit.upColunmPoint, new ElementId(-1)));
                                        tarColumnPoints.Add(Point.Create(_parkingSpaceUnit.bottomColunmPoint, new ElementId(-1)));
                                        //需要将xyz转变为Point
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion
                    }

                }

                #region 确认裁剪所用区域，整个图形的布尔裁剪行为，一直进行直到结束

                if (basicDownHorizantalCLine.lineStyleId == LineStyleId.MainRoadId)// 判断裁剪起始形态：车位、车位+车道
                {
                    if (sweepRowCount % 2 == 0)// 判断裁剪位于第几行
                        clipperParkingSpaceUnit = AddColumnParking(startLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);

                    else if (sweepRowCount % 2 == 1)
                        clipperParkingSpaceUnit = AddColumnParkingAndMainRoad(startLeftDownXYZ, height, wight, roadWight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);

                }
                else if (basicDownHorizantalCLine.lineStyleId == LineStyleId.basementWallId
                    || basicDownHorizantalCLine.lineStyleId == LineStyleId.ObstacleCurveId
                    || basicDownHorizantalCLine.lineStyleId == LineStyleId.OffsetRedLineId)
                {
                    if (sweepRowCount % 2 == 0)
                        clipperParkingSpaceUnit = AddColumnParkingAndMainRoad(startLeftDownXYZ, height, wight, roadWight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);

                    else if (sweepRowCount % 2 == 1)
                        clipperParkingSpaceUnit = AddColumnParking(startLeftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, parkingCount, out doesHasColumn);
                }
                #endregion

                #region 进行裁剪

                Paths singleParking = new Paths() { clipperParkingSpaceUnit.path };// 车位区域
                Paths singleCanPlancedRegion = new Paths() { clipper_methods.Path_xyzToPath(circularPoints) };// 被裁剪区域
                singleCanPlancedRegion = clipper_methods.RegionCropctDifference(singleCanPlancedRegion, singleParking);// 裁剪结果

                if (singleCanPlancedRegion.Count == 1)//问题 两个相等区域相减 减到最后一个 便结束循环
                {
                    circularPoints = clipper_methods.PathToPath_xyz(singleCanPlancedRegion.First());//点集需要去重
                    Clipper.CleanPolygons(singleCanPlancedRegion);

                    if (circularPoints.Count < 3)
                        break;

                    circularPoints = _Methods.Pointlistdeduplication(clipper_methods.RemoveSingleLineInPoints(circularPoints));

                    if (circularPoints.Count < 3)
                        break;
                }
                else if (singleCanPlancedRegion.Count == 2)
                {
                    Path_xyz newRegionPoints_01 = clipper_methods.PathToPath_xyz(singleCanPlancedRegion.First());//点集需要去重
                    Path_xyz newRegionPoints_02 = clipper_methods.PathToPath_xyz(singleCanPlancedRegion.Last());//点集需要去重

                    XYZ upXYZ_01 = _Methods.GetLeftUpXYZfromPoints(newRegionPoints_01);
                    XYZ upXYZ_02 = _Methods.GetLeftUpXYZfromPoints(newRegionPoints_02);

                    CurveArray curveArrayRecursion = new CurveArray();
                    if (upXYZ_01.Y >= upXYZ_02.Y)
                    {
                        circularPoints = newRegionPoints_01;// 用于自身循环
                        curveArrayRecursion = _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(newRegionPoints_02));// 用于多出的区域循环，要保持停车的行、列数以及基准线属性正确 basicDownHorizantalCLine, sweepRowCount, parkingCountRecursion, 
                    }
                    else
                    {
                        circularPoints = newRegionPoints_02;
                        curveArrayRecursion = _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(newRegionPoints_01));
                    }

                    #region 采用递归函数，明确是第几行，以及该行相邻属性线为障碍物，还是通车道

                    List<Point> tarParkingPointsRecursion = new List<Point>();
                    List<Point> tarColumnPointsRecursion = new List<Point>();

                    int parkingCountRecursion = parkingCount + 1;

                    //double moveDownDistance = doc.ActiveView.GenLevel.ProjectElevation;
                    //_Methods.ShowTempGeometry(doc, curveArrayRecursion, new XYZ(0, 0, moveDownDistance));

                    tarParkingPointsRecursion = Sweep(doc, moveHeidht_leftDownXYZ, curveArrayRecursion, nowProjectObstalCurveArrArray, nowProJectRegionPropertyLines, basicDownHorizantalCLine, sweepRowCount, parkingCountRecursion, out tarColumnPointsRecursion);

                    tarParkingPoints.AddRange(tarParkingPointsRecursion);
                    tarColumnPoints.AddRange(tarColumnPointsRecursion);

                    #endregion

                    if (circularPoints.Count < 3)
                        break;

                    circularPoints = _Methods.Pointlistdeduplication(clipper_methods.RemoveSingleLineInPoints(circularPoints));

                    if (circularPoints.Count < 3)
                        break;
                }
                else
                    break;//区域被裁剪结束 则跳出循环
                #endregion

                #region 判断得到区域点集所在矩形的高度，如果高度小于车位高度 则停止循环
                XYZ _leftUpPoint = _Methods.GetLeftUpXYZfromPoints(circularPoints);
                XYZ _leftDownPoint = _Methods.GetLeftDownXYZfromPoints(circularPoints);

                if (_leftUpPoint.Y - _leftDownPoint.Y < height - _Methods.PRECISION)
                    break;
                #endregion

                #region 裁剪车位向右移动 +1
                parkingCount += 1;
                #endregion

            }//for循环结束

            #endregion

            return tarParkingPoints;
        }

        /// <summary>
        /// 判断车位的下方，是不是障碍物属性线，如果是，则判断车头与障碍物属性线的距离，是否满足车道宽度的要求 车位类参数，用来控制，当前车位是车，还是车+次通车道宽
        /// </summary>
        public bool isObstalUnderParking(XYZ _leftDownXYZ, List<ConditionLine> clintersectLinesAndPointsVerticalLeft, double roadWight, double height)
        {
            bool isNotBottomHasObstal = false;

            if (clintersectLinesAndPointsVerticalLeft.Count < 1)
                return false;

            //首先判断 基点下方有没有障碍物
            ConditionLine downClLine = clintersectLinesAndPointsVerticalLeft.First();
            ConditionLine upClLine = clintersectLinesAndPointsVerticalLeft.Last();

            double upDistance = _leftDownXYZ.Y - upClLine.conditionPoint.xyz.Y;//上部分为负值 当前为最小值 
            double downDistance = _leftDownXYZ.Y - downClLine.conditionPoint.xyz.Y;//下部分为正值 当前为最大值

            if (downDistance > 0)
            {
                foreach (ConditionLine _clLine in clintersectLinesAndPointsVerticalLeft)
                {
                    //找到上下属性线
                    double _upDistance = _leftDownXYZ.Y - _clLine.conditionPoint.xyz.Y;//上部分为负值 当前为最小值 
                    double _downDistance = _leftDownXYZ.Y - _clLine.conditionPoint.xyz.Y;//下部分为正值 当前为最大值
                    if (_upDistance <= _Methods.PRECISION)
                    {
                        if (_upDistance > upDistance)
                        {
                            upClLine = _clLine;
                            upDistance = _upDistance;
                        }
                    }
                    if (_downDistance >= -_Methods.PRECISION)
                    {
                        if (_downDistance < downDistance)
                        {
                            downClLine = _clLine;
                            downDistance = _downDistance;
                        }
                    }
                }

                if (downClLine.lineStyleId == LineStyleId.ObstacleCurveId)//首先判断底部是不是障碍物属性
                {

                    if (downDistance < height + roadWight - _Methods.PRECISION)//其次判断底部距离够不够停车
                    {
                        isNotBottomHasObstal = true;
                    }
                }
            }

            return isNotBottomHasObstal;
        }
        public ConditionLine FindBenchmarkclLine(XYZ _leftDownXYZ, List<ConditionLine> clintersectLinesAndPointsVerticalLeft, ConditionLine firstClLine)
        {
            ConditionLine benchMarkclLine = firstClLine;
            foreach (ConditionLine _clLine in clintersectLinesAndPointsVerticalLeft)
            {
                if (Math.Abs(_leftDownXYZ.Y - _clLine.conditionPoint.xyz.Y) < _Methods.PRECISION)//判断sweep初始点 在不在sweep交点列表中 如果不在，则说明，是一个倾斜三角形
                {
                    benchMarkclLine = _clLine;
                }
            }
            return benchMarkclLine;
        }
        /// <summary>
        /// 是否增加柱网
        /// </summary>
        public ParkingSpaceUnit AddColumnParkingAndMainRoad(XYZ _leftDownXYZ, double height, double wight, double mainRoadWight, double columnHeight, double columnWidth, double columnBufferDistance, int parkingCount, out bool doesHasColumn)
        {
            doesHasColumn = false;

            if (parkingCount - 3 >= 0 || parkingCount == 0)
            {
                if (parkingCount % 3 == 0 || parkingCount == 0)
                {
                    doesHasColumn = true;
                    return new ParkingSpaceUnit(_leftDownXYZ, height, wight, mainRoadWight, columnHeight, columnWidth, columnBufferDistance, true);//创建车位+车道+柱子
                }
                else
                {
                    return new ParkingSpaceUnit(_leftDownXYZ, height, wight, mainRoadWight, columnHeight, columnWidth, columnBufferDistance, false);//创建车位+车道
                }
            }
            else
            {
                return new ParkingSpaceUnit(_leftDownXYZ, height, wight, mainRoadWight, columnHeight, columnWidth, columnBufferDistance, false);//创建车位+车道
            }

        }
        /// <summary>
        /// 判断车位石头添加柱距和主车道 此处为 在柱子干扰的情况下 车位左下角点的位置移动
        /// </summary>
        public ParkingSpaceUnit AddColumnParking(XYZ _leftDownXYZ, double height, double wight, double columnHeight, double columnWidth, double columnBufferDistance, int parkingCount, out bool doesHasColumn)
        {
            doesHasColumn = false;

            if (parkingCount - 3 >= 0 || parkingCount == 0)
            {
                if (parkingCount % 3 == 0 || parkingCount == 0)
                {
                    doesHasColumn = true;
                    return new ParkingSpaceUnit(_leftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, true);//创建车位+柱子
                }
                else
                {
                    return new ParkingSpaceUnit(_leftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, false);//创建车位
                }
            }
            else
            {
                return new ParkingSpaceUnit(_leftDownXYZ, height, wight, columnHeight, columnWidth, columnBufferDistance, false);//创建车位
            }
        }
        /// <summary>
        /// 判断车位石头添加柱距
        /// </summary>
        public XYZ AddWifhtAndColumnParkingPoint(XYZ _leftDownXYZ, double wight, double columnWidth, double columnBufferDistance, int parkingCount)
        {
            if (parkingCount > 0)
            {
                return _leftDownXYZ + new XYZ(wight * parkingCount, 0, 0) + new XYZ((columnWidth + columnBufferDistance * 2) * ((parkingCount - 1) / 3 + 1), 0, 0);
            }
            else
            {
                return _leftDownXYZ;
            }
        }
        /// <summary>
        /// 获取所有sweep线上的点集
        /// </summary>
        public List<ConditionLine> GetAllIntersectPropertyLinesAndPoints(Document doc, Line sweepLineVertical, List<Line> nowRegionPropertyLines, string sweepLineDirection)
        {
            List<ConditionLine> clintersectLines = new List<ConditionLine>();
            foreach (Line _line in nowRegionPropertyLines)//这个为边界线
            {
                IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
                SetComparisonResult setComparisonResult = sweepLineVertical.Intersect(_line, out intersectionResultArray);
                if (setComparisonResult == SetComparisonResult.Overlap)
                {
                    //创建属性线
                    ConditionLine clLine = new ConditionLine(doc, _line);
                    //创建属性点
                    ConditionPoint clPoint = new ConditionPoint(intersectionResultArray.get_Item(0).XYZPoint, clLine.lineStyleId);
                    clLine.conditionPoint = clPoint;
                    clintersectLines.Add(clLine);
                }
            }

            if (clintersectLines.Count <= 1)
                return clintersectLines;
            else
            {
                // 将相交线集基于点集进行排序 左为小，右为大；注意，此处的写法
                if (sweepLineDirection == "Vertical")
                {
                    clintersectLines.Sort((ConditionLine line01, ConditionLine line02) => line01.conditionPoint.xyz.Y.CompareTo(line02.conditionPoint.xyz.Y));
                }
                else if (sweepLineDirection == "Horizantal")
                {
                    clintersectLines.Sort((ConditionLine line01, ConditionLine line02) => line01.conditionPoint.xyz.X.CompareTo(line02.conditionPoint.xyz.X));
                }
            }
            return clintersectLines;
        }
        /// <summary>
        /// 获取与扫描线相交的属性线，并排序， 左为小，右为大；注意此处sort排序函数的写法；sweepLineDirection为Vertical垂直与Horizantal水平，两种做法
        /// </summary>
        public List<ConditionLine> GetIntersectBoundriesCLlines(Document doc, Line sweepLine, List<Line> _lines, List<Line> nowRegionPropertyLines, string sweepLineDirection)
        {
            List<ConditionLine> clintersectLines = new List<ConditionLine>();
            foreach (Line _line in _lines)//这个为边界线
            {
                IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
                SetComparisonResult setComparisonResult = sweepLine.Intersect(_line, out intersectionResultArray);
                if (setComparisonResult == SetComparisonResult.Overlap)
                {
                    //创建属性线
                    ConditionLine clLine = GetCLine(doc, _line, nowRegionPropertyLines);
                    //创建属性点
                    ConditionPoint clPoint = new ConditionPoint(intersectionResultArray.get_Item(0).XYZPoint, clLine.lineStyleId);
                    clLine.conditionPoint = clPoint;
                    clintersectLines.Add(clLine);
                }
            }

            if (clintersectLines.Count <= 1)
                return clintersectLines;
            else
            {
                //将相交线集基于点集进行排序 左为小，右为大；注意，此处的写法
                if (sweepLineDirection == "Vertical")
                {
                    clintersectLines.Sort((ConditionLine line01, ConditionLine line02) => line01.conditionPoint.xyz.Y.CompareTo(line02.conditionPoint.xyz.Y));
                }
                else if (sweepLineDirection == "Horizantal")
                {
                    clintersectLines.Sort((ConditionLine line01, ConditionLine line02) => line01.conditionPoint.xyz.X.CompareTo(line02.conditionPoint.xyz.X));
                }
            }
            return clintersectLines;
        }
        #endregion
        /// <summary>
        /// 获取属性线
        /// </summary>
        public ConditionLine GetCLine(Document doc, Line _line, List<Line> nowRegionPropertyLines)
        {
            ConditionLine clLine = new ConditionLine();
            Line propertyLine = null;
            Line line = _line as Line;
            bool isCoincide = IsCoincideWithAllPropertyLines(_line, nowRegionPropertyLines, out propertyLine);
            if (isCoincide)
            {
                clLine = new ConditionLine(doc, propertyLine);
            }
            else if (!isCoincide)
            {
                clLine = new ConditionLine(doc, _line);
            }
            return clLine;
        }
        /// <summary>
        /// 判断一个线是否与目标详图线集合中线条重合，如果重合，提取重合线
        /// </summary>
        public bool IsCoincideWithAllPropertyLines(Line _line, List<Line> nowRegionPropertyLines, out Line _propertyLine)
        {
            bool isCoincide = false;
            _propertyLine = null;
            foreach (Line propertyLine in nowRegionPropertyLines)
            {
                if (_Methods.IsCoincide(_line, propertyLine))//由于clipper之后的误差为0.2mm，因此，手动创建函数 线段点到直线的距离小于1mm则属于重合的情况
                {
                    isCoincide = true;
                    _propertyLine = propertyLine;
                    break;
                }
            }

            return isCoincide;
        }
        /// <summary>
        /// 该函数为该class 使用using语句，结束的时候，会释放的函数
        /// </summary>
        public void Dispose()
        {
            //_Methods.TaskDialogShowMessage("该区域计算结束");
            //throw new NotImplementedException();
        }

    }//class
} //namespace