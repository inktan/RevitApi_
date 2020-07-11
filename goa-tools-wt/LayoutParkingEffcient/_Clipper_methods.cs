using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using wt_Common;
using LayoutParkingEffcient;

namespace ClipperLib
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    public class clipper_methods
    {
        public static double _multiple = 100000000.0;

        /// <summary>
        /// clipper 减法
        /// </summary>
        public Paths RegionCropctDifference(Paths pathsRedlineOffest, Paths mainRoadRegion, Paths obstacleRegion)
        {
            Paths canPlacedRegion = new Paths();//得到可停车区域
            Clipper endC = new Clipper();
            endC.AddPaths(pathsRedlineOffest, PolyType.ptSubject, true);
            endC.AddPaths(mainRoadRegion, PolyType.ptClip, true);
            endC.AddPaths(obstacleRegion, PolyType.ptClip, true);
            endC.Execute(ClipType.ctDifference, canPlacedRegion);

            return canPlacedRegion;
        }
        /// <summary>
        /// clipper 减法
        /// </summary>
        public static Paths RegionCropctDifference(Paths subjs, Paths clips)
        {
            Paths canPlacedRegion = new Paths();//得到可停车区域
            Clipper endC = new Clipper();
            endC.AddPaths(subjs, PolyType.ptSubject, true);
            endC.AddPaths(clips, PolyType.ptClip, true);
            endC.Execute(ClipType.ctDifference, canPlacedRegion);

            return canPlacedRegion;
        }
        /// <summary>
        /// clipper 交集
        /// </summary>
        public static Paths RegionCropctIntersection(Paths subjs, Paths clips)
        {
            Paths canPlacedRegion = new Paths();//得到可停车区域
            Clipper endC = new Clipper();
            endC.AddPaths(subjs, PolyType.ptSubject, true);
            endC.AddPaths(clips, PolyType.ptClip, true);
            endC.Execute(ClipType.ctIntersection, canPlacedRegion);

            return canPlacedRegion;
        }
        /// <summary>
        /// clipper 并集
        /// </summary>
        public static Paths RegionCropctUnion(Paths subjs, Paths clips)
        {
            Paths canPlacedRegion = new Paths();//得到可停车区域
            Clipper endC = new Clipper();
            endC.AddPaths(subjs, PolyType.ptSubject, true);
            endC.AddPaths(clips, PolyType.ptClip, true);
            endC.Execute(ClipType.ctUnion, canPlacedRegion, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            return canPlacedRegion;
        }
        /// <summary>
        /// 对一个点集路径进行偏移 需要注意 对于一个闭合路径 使用ClipperOffset 得到的结果，会有两个点集，第一个结果为外圈点集，第二结果为内圈点集
        /// </summary>
        public static List<List<IntPoint>> _GetOffsetPonts_clipper_Path(Path_xyz _Path_xyz_RedLine, double _offset)
        {
            Paths solution = new Paths();
            ClipperOffset _Co = new ClipperOffset();//ClipperOffset构造函数。其包含可选参数
            Path _Path_RedLine = Path_xyzToPath(_Path_xyz_RedLine);
            _Co.AddPath(_Path_RedLine, JoinType.jtSquare, EndType.etClosedLine);
            _offset = _offset * 1000;//放大倍数与clipper_methods中放大倍数保持一致
            _Co.Execute(ref solution, _offset);
            return solution;//一根线偏移后，应该具有四个端点，为矩形
        }
        /// <summary>
        /// 使用clipper对 单根个线段进行偏移，其结果点集是一个矩形路径
        /// </summary>
        public static List<IntPoint> _GetOffsetPonts_clipper_line(Curve _curve, double _offset)
        {
            //拿出线段断点转化为 Intpont
            XYZ _startPoint = _curve.GetEndPoint(0);
            XYZ _endPoint = _curve.GetEndPoint(1);
            Path_xyz _Path_xyz = new Path_xyz() { _startPoint, _endPoint };
            Path _Path = Path_xyzToPath(_Path_xyz);
            Paths solution = new Paths();
            //开展偏移工作
            ClipperOffset _Co = new ClipperOffset();//ClipperOffset构造函数。其包含可选参数
            _Co.AddPath(_Path, JoinType.jtSquare, EndType.etClosedLine);
            _offset = _offset * _multiple;//放大倍数与clipper_methods中放大倍数保持一致
            _Co.Execute(ref solution, _offset);
            return solution[0]; //一根线偏移后，应该具有四个端点，为矩形
        }
        /// <summary>
        /// 排除clipper后出现的单根直线问题 该处为特殊情况，clipper后，下部出现一个孤立的点 排除方法，查看一个由一个端点引出的两根线的夹角极限值 优化：相邻的两根线进行判断
        /// </summary>
        public static Path_xyz RemoveSingleLineInPoints(Path_xyz _newRegionPoints)
        {
            Path_xyz needPathXyz = new Path_xyz();
            foreach (XYZ xyz in _newRegionPoints)
            {
                needPathXyz.Add(xyz);
            }

            List<Line> tempLines = _Methods.GetClosedLinesFromPoints(_newRegionPoints);
            int count = tempLines.Count;
            for (int i = 0; i < count; i++)
            {
                Line line01 = null;
                Line line02 = null;

                if (i == count - 1)
                {
                    line01 = tempLines[0];
                    line02 = tempLines[i];
                }
                else
                {
                    line01 = tempLines[i];
                    line02 = tempLines[i + 1];
                }

                double angle = line01.Direction.AngleTo(line02.Direction);

                if ((0 < angle && angle < _Methods.PRECISION) || (Math.PI > angle && angle > Math.PI - _Methods.PRECISION))
                {
                    //CMD.TestList.Add(angle.ToString());

                    //剔除V字型的角点
                    XYZ endPoint01 = line01.GetEndPoint(0);
                    XYZ endPoint02 = line01.GetEndPoint(1);

                    XYZ _endPoint01 = line02.GetEndPoint(0);
                    XYZ _endPoint02 = line02.GetEndPoint(1);

                    //删除 V 字型角点 注意 判断关系 由于点生成线，再求出线的端点，会发现，新得到的点与旧点产生区分度，因此，需要多重循环，通过距离进行判断，进行删除
                    if (endPoint01.DistanceTo(_endPoint01) < _Methods.PRECISION || endPoint01.DistanceTo(_endPoint02) < _Methods.PRECISION)
                    {
                        foreach (XYZ xyz in _newRegionPoints)
                        {
                            if (xyz.DistanceTo(endPoint01) < _Methods.PRECISION)
                            {
                                bool b01 = needPathXyz.Remove(xyz);
                            }
                        }
                    }
                    else if (endPoint02.DistanceTo(_endPoint01) < _Methods.PRECISION || endPoint02.DistanceTo(_endPoint02) < _Methods.PRECISION)
                    {
                        foreach (XYZ xyz in _newRegionPoints)
                        {
                            if (xyz.DistanceTo(endPoint02) < _Methods.PRECISION)
                            {
                                bool b01 = needPathXyz.Remove(xyz);
                            }
                        }
                    }
                }
            }
            return needPathXyz;
        }
        /// <summary>
        /// 将list<>list xyz 转化为 intpoint list<>list
        /// </summary>
        public static Paths_xyz PathsToPaths_xyz(Paths _Paths)
        {
            Paths_xyz _Paths_xyz = new Paths_xyz();
            foreach (Path _Path in _Paths)
            {
                Path_xyz _XYZs = PathToPath_xyz(_Path);
                _Paths_xyz.Add(_XYZs);
            }
            return _Paths_xyz;
        }
        /// <summary>
        /// 对比前后区域划分 得到变化的区域 变化后的重新计算 变化前的进行删除 得到前一个paths_xyz未更新的区域
        /// </summary>
        public static Paths_xyz GetChangedRegion(Paths_xyz afterCanPlaceBoundaryPoints, Paths_xyz beforeCanPlaceBoundaryPoints)
        {
            Paths_xyz changedRegion = new Paths_xyz();
            foreach (Path_xyz afterPathXyz in afterCanPlaceBoundaryPoints)
            {
                if (!PathXyzIsInPathsXyz(afterPathXyz, beforeCanPlaceBoundaryPoints))
                {
                    changedRegion.Add(afterPathXyz);
                }
            }
            return changedRegion;
        }
        /// <summary>
        /// 判断一个path_xyz在不在PahtsXyz
        /// </summary>
        public static bool PathXyzIsInPathsXyz(Path_xyz path_xyz, Paths_xyz paths_xyz)
        {
            bool isIn = false;
            foreach (Path_xyz _path_xyz in paths_xyz)
            {
                if (PathXyzIsSame(path_xyz, _path_xyz))
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        #region 判断两个线圈是否相同

        /// <summary>
        /// 对比两个区域是否相同
        /// </summary>
        public static bool PathXyzIsSame(Path_xyz afterCanPlaceBoundaryPoint, Path_xyz beforeCanPlaceBoundaryPoint)
        {
            Path_xyz afterPath_xyz = ReSortPointsByLeftDownPoint(afterCanPlaceBoundaryPoint);//重新排序
            Path_xyz beforePath_xyz = ReSortPointsByLeftDownPoint(beforeCanPlaceBoundaryPoint);//重新排序

            bool isSame = true;
            if (afterPath_xyz.Count == beforePath_xyz.Count)
            {
                for (int i = 0; i < afterPath_xyz.Count; i++)
                {
                    if (afterPath_xyz[i].DistanceTo(beforePath_xyz[i]) < _Methods.PRECISION)//该处需要注意 Revit2020版本中 曲线的最小容差为 0.00256026455729167 Feet 0.7803686370625 MilliMeter
                    {
                        continue;
                    }
                    else
                    {
                        isSame = false;
                        break;
                    }
                }
            }
            else
            {
                isSame = false;
            }
            return isSame;
        }
        /// <summary>
        /// 进行重新排序
        /// 求出与矩形左下角点平行的点中 距离其最近的点 基于该点 在该点基础下
        /// </summary>
        public static Path_xyz ReSortPointsByLeftDownPoint(Path_xyz path_xyz)
        {
            XYZ leftDownXyz = _Methods.GetLeftDownXYZfromPoints(path_xyz);
            //求出与矩形左下角点平行的点
            Path_xyz _pathXyz = new Path_xyz();
            foreach (XYZ xyz in path_xyz)
            {
                if (xyz.Y - leftDownXyz.Y < _Methods.PRECISION)
                {
                    _pathXyz.Add(xyz);
                }
            }
            //求出最下平行最近点
            XYZ closedPoint = GetCLosedPointNearestLeftDownPoint(leftDownXyz, _pathXyz);
            int index = path_xyz.IndexOf(closedPoint);

            //重新排序
            Path_xyz newPathXyz = new Path_xyz();
            int count = path_xyz.Count;
            for (int i = index; i < count; i++)
            {
                newPathXyz.Add(path_xyz[i]);
            }
            for (int i = 0; i < index; i++)
            {
                newPathXyz.Add(path_xyz[i]);
            }
            return newPathXyz;
        }
        /// <summary>
        /// 求出点集离矩形左下角点最近的最下位置的点 不要修改该函数
        /// </summary>
        public static XYZ GetCLosedPointNearestLeftDownPoint(XYZ leftDownXyz, Path_xyz path_xyz)//不要修改该函数
        {
            double temp = leftDownXyz.DistanceTo(path_xyz[0]);
            XYZ closedPoint = XYZ.Zero;

            foreach (XYZ xyz in path_xyz)
            {
                if (xyz.Y - leftDownXyz.Y < _Methods.PRECISION)
                {
                    temp = xyz.X - leftDownXyz.X;
                    closedPoint = xyz;
                    break;
                }
            }

            foreach (XYZ xyz in path_xyz)
            {
                if (xyz.Y == leftDownXyz.Y)
                {
                    double distance = xyz.X - leftDownXyz.X;
                    if (temp > distance)
                    {
                        closedPoint = xyz;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return closedPoint;
        }
        #endregion
        /// <summary>
        /// 将intpoint list 转化为 list xyz
        /// </summary>
        public static Path_xyz PathToPath_xyz(Path _Path)
        {
            Path_xyz _Path_xyz = new Path_xyz();
            foreach (IntPoint _IntPoint in _Path)
            {
                XYZ _xyz = IntPointToXYZ(_IntPoint);
                _Path_xyz.Add(_xyz);
            }
            return _Path_xyz;
        }
        /// <summary>
        /// 将list<>list xyz 转化为 intpoint list<>list
        /// </summary>
        public static Paths Paths_xyzToPaths(Paths_xyz _Paths_xyz)
        {
            Paths _Paths = new Paths();
            foreach (Path_xyz _Path_xyz in _Paths_xyz)
            {
                Path _Path = Path_xyzToPath(_Path_xyz);
                _Paths.Add(_Path);
            }
            return _Paths;
        }
        /// <summary>
        /// 将list xyz 转化为 intpoint list
        /// </summary>
        public static Path Path_xyzToPath(Path_xyz _Path_xyz)
        {
            Path _Path = new Path();
            foreach (XYZ _xyz in _Path_xyz)
            {
                IntPoint _IntPoint = XYZToIntPoint(_xyz);
                _Path.Add(_IntPoint);
            }
            return _Path;
        }
        /// <summary>
        /// Rveit XYZ convert to Clipper IntPoint
        /// </summary>
        public static IntPoint XYZToIntPoint(XYZ _xyz)
        {
            double x = _xyz.X * _multiple;
            double y = _xyz.Y * _multiple;
            return new IntPoint((cInt)x, (cInt)y);
        }
        /// <summary>
        /// Clipper IntPoint convert toRveit XYZ 
        /// </summary>
        public static XYZ IntPointToXYZ(IntPoint _intpoint)
        {
            double x = (double)_intpoint.X / _multiple;
            double y = (double)_intpoint.Y / _multiple;
            return new XYZ(x, y, 0.0);
        }
    }
}
