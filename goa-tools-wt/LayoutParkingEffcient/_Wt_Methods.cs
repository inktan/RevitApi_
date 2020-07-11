using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using ClipperLib;
using wt_Common;
using LayoutParkingEffcient;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace wt_Common
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;
    /// <summary>
    /// 个人常用方法梳理
    /// </summary>
    public class _Methods
    {
        public static double PRECISION = 0.003;//曲线的最小容差为 0.00256026455729167foot*304.8mm

        #region 将线条临时显示在界面上

        /// <summary>
        /// 将线段列表输出为 directshape
        /// </summary>
        public static void ShowTempGeometry(Document doc, List<List<Line>> _lines)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果
            foreach (List<Line> lines in _lines)
            {
                _geometry.AddRange(lines);
            }
            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry);
                trans.Commit();
            }
        }
        /// <summary>
        /// 将线段列表输出为 directshape
        /// </summary>
        public static void ShowTempGeometry(Document doc, List<Line> _lines)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果
            _geometry.AddRange(_lines);
            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry);
                trans.Commit();
            }
        }
        /// <summary>
        /// 将线段列表输出为 directshape 需不需要将directShape移动到当前视图，取决于生成ds的线圈，有没有Z值，比如经过clipper转化的点，Z值均为零
        /// </summary>
        public static void ShowTempGeometry(Document doc, List<Line> _lines, XYZ moveDirection)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果
            _geometry.AddRange(_lines);
            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry, moveDirection);
                trans.Commit();
            }
        }
        /// <summary>
        /// 将 curveArray 输出为 directshape
        /// </summary>
        public static void ShowTempGeometry(Document doc, CurveArray curveArray)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果
            _geometry.AddRange(CurveArrayToListLine(curveArray));
            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry);
                trans.Commit();
            }
        }
        /// <summary>
        /// 将 curveArray 输出为 directshape
        /// </summary>
        public static void ShowTempGeometry(Document doc, CurveArray curveArray, XYZ moveDirection)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果
            _geometry.AddRange(CurveArrayToListLine(curveArray));
            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry, moveDirection);
                trans.Commit();
            }
        }
        public static void ShowTempGeometry(Document doc, CurveLoop curveLoop)
        {

            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果

            CurveLoopIterator iteraor = curveLoop.GetCurveLoopIterator();
            while (iteraor.MoveNext())
            {
                _geometry.Add(iteraor.Current as GeometryObject);
            }

            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry);
                trans.Commit();
            }
        }
        public static void ShowTempGeometry(Document doc, CurveLoop curveLoop, XYZ moveDirection)
        {

            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果

            CurveLoopIterator iteraor = curveLoop.GetCurveLoopIterator();
            while (iteraor.MoveNext())
            {
                _geometry.Add(iteraor.Current as GeometryObject);
            }

            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry, moveDirection);
                trans.Commit();
            }
        }
        /// <summary>
        /// 将 curveArray 输出为 directshape
        /// </summary>
        public static void ShowTempGeometry(Document doc, CurveArrArray curveArray)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果

            foreach (CurveArray _ces in curveArray)
            {
                _geometry.AddRange(_Methods.CurveArrayToListLine(_ces));
            }

            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry);
                trans.Commit();
            }
        }
        /// <summary>
        /// 将 curveArray 输出为 directshape
        /// </summary>
        public static void ShowTempGeometry(Document doc, CurveArrArray curveArray, XYZ moveDirection)
        {
            List<GeometryObject> _geometry = new List<GeometryObject>();//查看临时结果

            foreach (CurveArray _ces in curveArray)
            {
                _geometry.AddRange(_Methods.CurveArrayToListLine(_ces));
            }

            using (Transaction trans = new Transaction(doc))//生成临时辅助线
            {
                trans.Start("trans");
                Methods.CreateDirectShape(doc, _geometry, moveDirection);
                trans.Commit();
            }
        }
        #endregion

        #region 基础语法功能
        /// <summary>
        /// 具体保留小数点后几位数 , int 可取值 1-5
        /// </summary>
        /// <param name="_distance"></param>
        /// <returns></returns>
        public static double TakeNumberAfterDecimal(double _distance, int _int)
        {
            string _str = _distance.ToString();
            if (_int == 1)
            {
                _str = _distance.ToString("#0.0");
            }
            else if (_int == 2)
            {
                _str = _distance.ToString("#0.00");
            }
            else if (_int == 3)
            {
                _str = _distance.ToString("#0.000");
            }
            else if (_int == 4)
            {
                _str = _distance.ToString("#0.0000");
            }
            else if (_int == 5)
            {
                _str = _distance.ToString("#0.00000");
            }
            else
            {
                return _distance;
            }
            return Convert.ToDouble(_str);
        }
        #endregion

        #region transform 将transform应用于曲线数组
        /// <summary>
        /// 将transform应用于曲线数组的数组
        /// </summary>
        public static List<Curve> TransformCurves(List<Curve> nowRegionPropertyLines, Transform trans)
        {
            List<Curve> _curveArrArray = new List<Curve>();
            foreach (Curve _ces in nowRegionPropertyLines)
            {
                _curveArrArray.Add(_ces.CreateTransformed(trans));
            }
            return _curveArrArray;
        }
        /// <summary>
        /// 将transform应用于曲线数组的数组
        /// </summary>
        public static CurveArrArray TransformCurveArray(CurveArrArray curveArrArray, Transform trans)
        {
            CurveArrArray _curveArrArray = new CurveArrArray();
            foreach (CurveArray _ces in curveArrArray)
            {
                _curveArrArray.Append(TransformCurveArray(_ces, trans));
            }
            return _curveArrArray;
        }
        /// <summary>
        /// 将transform应用于曲线数组
        /// </summary>
        public static CurveArray TransformCurveArray(CurveArray curves, Transform trans)
        {
            CurveArray _curves = new CurveArray();
            foreach (Curve c in curves)
            {
                _curves.Append(c.CreateTransformed(trans));
            }
            return _curves;
        }
        /// <summary>
        /// 将transform应用于点集体
        /// </summary>
        /// <returns></returns>
        public static List<Point> TransformPath_xyz(List<Point> Max_tar_placeXYZs, Transform afterTransform)
        {
            List<Point> rotateTarPlaceXYZs = new List<Point>();//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            foreach (Point _point in Max_tar_placeXYZs)
            {
                XYZ _rotate = afterTransform.OfPoint(_point.Coord);
                rotateTarPlaceXYZs.Add(Point.Create(_rotate, _point.GraphicsStyleId));
            }
            return rotateTarPlaceXYZs;
        }
        #endregion

        #region 线组相关  绘制详图线/模型线组，并赋予指定线样式 判断组内线样式是否统一
        /// <summary>
        /// 判断组内线样式是否统一
        /// </summary>
        public static bool IsSameAllCurveLineStyleFromGroup(Document doc, Group _group, out string _GraphicsStyle_frist_name)
        {
            bool iSmae = false;
            List<ElementId> _EleIds = _group.GetMemberIds().ToList();
            int _count = _EleIds.Count;
            int __count = 0;

            List<CurveElement> curveElements = _Methods.EleIdsToEles(doc, _EleIds).Cast<CurveElement>().ToList();
            _GraphicsStyle_frist_name = curveElements.First().LineStyle.Name;
            string graphicsStyle_frist_name = _GraphicsStyle_frist_name;

            curveElements.ForEach(p => {
                if (graphicsStyle_frist_name == p.LineStyle.Name)
                    __count += 1;
            });

            if (_count == __count)
                iSmae = true;

            return iSmae;
        }
        /// <summary>
        /// 创建详图线 成组 并且赋予指定线样式
        /// </summary>
        public static Group CreatNewDetailLineAndCreatGroup(Document doc, CurveArray curveArray, string lineStyle)
        {
            View acvtiView = doc.ActiveView;

            List<ElementId> NewDetailCurveIds = new List<ElementId>();
            using (Transaction CreatNewDetailLine = new Transaction(doc))
            {
                CreatNewDetailLine.Start("CreatNewDetailLine");
                DetailCurveArray _DetailCurveArray = doc.Create.NewDetailCurveArray(acvtiView, curveArray);
                foreach (DetailCurve detailCurve in _DetailCurveArray)
                {
                    _Methods.SetLineStyle(doc, lineStyle, detailCurve);
                    NewDetailCurveIds.Add(detailCurve.Id);
                }
                CreatNewDetailLine.Commit();
            }

            Group _group;
            using (Transaction CreatNewDetailLineGroup = new Transaction(doc))
            {
                CreatNewDetailLineGroup.Start("CreatNewDetailLineGroup");
                _group = doc.Create.NewGroup(NewDetailCurveIds);
                _group.GroupType.Name = lineStyle;
                CreatNewDetailLineGroup.Commit();
            }

            return _group;
        }
        /// <summary>
        /// 创建模型线 成组 并且赋予指定线样式
        /// </summary>
        public static Group CreatNewModelLineAndCreatGroup(Document doc, CurveArray curveArray, string lineStyle)
        {
            View acvtiView = doc.ActiveView;
            //当前工作视图的高程
            Level level = acvtiView.GenLevel;
            double projectElevation = level.ProjectElevation;
            Plane plane = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, projectElevation));

            List<ElementId> NewModelCurveIds = new List<ElementId>();
            using (Transaction CreatNewDetailLine = new Transaction(doc))
            {
                CreatNewDetailLine.Start("CreatNewDetailLine");
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                ModelCurveArray _ModelCurveArray = doc.Create.NewModelCurveArray(curveArray, sketchPlane);
                foreach (ModelCurve modelCurve in _ModelCurveArray)
                {
                    _Methods.SetLineStyle(doc, lineStyle, modelCurve);
                    NewModelCurveIds.Add(modelCurve.Id);
                }
                CreatNewDetailLine.Commit();
            }

            Group _group;
            using (Transaction CreatNewDetailLineGroup = new Transaction(doc))
            {
                CreatNewDetailLineGroup.Start("CreatNewDetailLineGroup");
                _group = doc.Create.NewGroup(NewModelCurveIds);
                _group.GroupType.Name = lineStyle;
                CreatNewDetailLineGroup.Commit();
            }
            return _group;
        }
        #endregion

        #region 获取目标区域内的元素
        /// <summary>
        /// 获取线圈内的所有属性曲线
        /// </summary>
        public static List<Curve> GetCurvesInTarRegion(Document doc, CurveArray curveArray, List<Curve> allCurves)
        {
            List<Curve> intersectCurves = new List<Curve>();
            foreach (Curve curve in allCurves)
            {
                XYZ xYZ01 = curve.GetEndPoint(0);
                XYZ xYZ02 = curve.GetEndPoint(1);

                if (IsInsideOrOnEdgeOfPolygon(new XYZ(xYZ01.X, xYZ01.Y, 0), curveArray) || IsInsideOrOnEdgeOfPolygon(new XYZ(xYZ02.X, xYZ02.Y, 0), curveArray))
                {
                    intersectCurves.Add(curve);
                    continue;
                }

                Line line = Line.CreateBound(new XYZ(xYZ01.X, xYZ01.Y, 0), new XYZ(xYZ02.X, xYZ02.Y, 0));
                foreach (Curve _curve in curveArray)
                {
                    XYZ _xYZ01 = _curve.GetEndPoint(0);
                    XYZ _xYZ02 = _curve.GetEndPoint(1);
                    Line _line = Line.CreateBound(new XYZ(_xYZ01.X, _xYZ01.Y, 0), new XYZ(_xYZ02.X, _xYZ02.Y, 0));
                    SetComparisonResult intersectionResule = line.Intersect(_line);
                    // CMD.TestList.Add(intersectionResule.ToString());
                    if (intersectionResule == SetComparisonResult.Overlap
                        || intersectionResule == SetComparisonResult.Subset
                        || intersectionResule == SetComparisonResult.Equal
                        )
                    {
                        intersectCurves.Add(curve);
                        break;
                    }
                }
            }
            return intersectCurves;
        }
        /// <summary>
        /// 获取线圈内的所有基础元素，判断基础为boundingBox的中心点，在不在目标范围内 
        /// 该方法速度的快慢，依据范围的大小，以及当前视图相关元素的数量
        /// </summary>
        public static List<Element> GetTarlElesByRegion(Document doc, CurveArray curveArray, List<Element> selEles)
        {
            List<Element> befroeRegionDetailEles = new List<Element>();
            foreach (Element _ele in selEles)
            {
                if (!_ele.IsValidObject) continue;

                BoundingBoxXYZ boundingBoxXYZ = _ele.get_BoundingBox(doc.ActiveView);
                if (boundingBoxXYZ == null)
                    continue;

                if (_ele is FamilyInstance)//族实例 默认放置位置为当前视图的切割平面
                {
                    if (_ele.OwnerViewId != doc.ActiveView.Id)//根据所属视图进行判断
                        continue;
                }
                else if (_ele is FilledRegion || _ele is Group)//根据物体 Z 值，进行判断
                {
                    if (boundingBoxXYZ.Min.Z != doc.ActiveView.GenLevel.ProjectElevation)//找到当前可见视图元素的 z 值
                        continue;
                }
                #region 通过boundingBox进行双重判断，1， boundingBOX所在矩形5个点（4个角点、一个中心点）与目标区域的关系；2，boundingBOX所在矩形边框与目标区域是否有相交关系；两个方法同时进行，速度会大幅度降低
                if (isBoundingBoxInRegionByFivePoints(boundingBoxXYZ, curveArray))//5个点
                    befroeRegionDetailEles.Add(_ele);
                else if (isBoundingBoxInRegionByIntersection(doc, boundingBoxXYZ, curveArray))//矩形线框的相交情况
                    befroeRegionDetailEles.Add(_ele);
                #endregion

            }
            return befroeRegionDetailEles;
        }
        /// <summary>
        /// 判断落在目标区域内的所有线圈，只要线圈包含的线条出现相交的情况，便判定该线圈属于目标区域
        /// </summary>
        public static CurveArrArray GetTarCurveArrArrayInTarRegionByintersect(CurveArray _regionCurves, CurveArrArray beforeObstacleCurveArrArray)
        {
            bool isBreak = false;
            CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
            foreach (CurveArray _curves in beforeObstacleCurveArrArray)
            {
                foreach (Curve item in _curves)
                {
                    foreach (Curve _item in _regionCurves)//只要有一次相交，便进入下一个循环
                    {
                        SetComparisonResult intersectionResule = item.Intersect(_item);
                        // CMD.TestList.Add(intersectionResule.ToString());
                        if (intersectionResule == SetComparisonResult.Overlap
                            || intersectionResule == SetComparisonResult.Subset
                            || intersectionResule == SetComparisonResult.Equal)
                        {
                            nowObstacleCurveArrArray.Append(_curves);
                            isBreak = true;
                            break;
                        }
                    }
                    if (isBreak)//
                    {
                        isBreak = false;
                        break;
                    }
                }
            }
            return nowObstacleCurveArrArray;
        }
        /// <summary>
        /// 当前范围内的所有障碍物线圈 找到各个线圈的四个矩形角点 和 形心 只要其中一个点位于目标区域内（包括线上），则判定该线圈属于目标区域
        /// </summary>
        public static CurveArrArray GetTarRegionInTarCurveArrArray(CurveArray _regionCurves, CurveArrArray beforeObstacleCurveArrArray)
        {
            CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
            foreach (CurveArray _curves in beforeObstacleCurveArrArray)
            {
                List<Line> _tempLines = CurveArrayToListLine(_regionCurves);
                XYZ _tmepLeftDownXYZ = GetLeftDownXYZfromLines(_tempLines);
                XYZ _tmepLeftUpnXYZ = GetLeftUpXYZfromLines(_tempLines);
                XYZ _tmepRigthUpXYZ = GetRightUpXYZfromLines(_tempLines);
                XYZ _tmepRightDownXYZ = GetRightDownXYZfromLines(_tempLines);
                XYZ _tmepMiddleXYZ = GetMiddleXYZfromLines(_tempLines);
                if (IsInsideOrOnEdgeOfPolygon(_tmepLeftDownXYZ, _curves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepLeftUpnXYZ, _curves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepRigthUpXYZ, _curves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepRightDownXYZ, _curves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepMiddleXYZ, _curves))
                {
                    nowObstacleCurveArrArray.Append(_curves);
                }
            }
            return nowObstacleCurveArrArray;
        }
        /// <summary>
        /// 当前范围内的所有障碍物线圈 找到各个线圈的四个矩形角点 和 形心 只要其中一个点位于目标区域内（包括线上），则判定该线圈属于目标区域 小在大
        /// </summary>
        public static CurveArrArray GetTarCurveArrArrayInTarRegion(CurveArray _regionCurves, CurveArrArray beforeObstacleCurveArrArray)
        {
            CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
            foreach (CurveArray _curves in beforeObstacleCurveArrArray)
            {
                List<Line> _tempLines = CurveArrayToListLine(_curves);
                XYZ _tmepLeftDownXYZ = GetLeftDownXYZfromLines(_tempLines);
                XYZ _tmepLeftUpnXYZ = GetLeftUpXYZfromLines(_tempLines);
                XYZ _tmepRigthUpXYZ = GetRightUpXYZfromLines(_tempLines);
                XYZ _tmepRightDownXYZ = GetRightDownXYZfromLines(_tempLines);
                XYZ _tmepMiddleXYZ = GetMiddleXYZfromLines(_tempLines);
                if (IsInsideOrOnEdgeOfPolygon(_tmepLeftDownXYZ, _regionCurves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepLeftUpnXYZ, _regionCurves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepRigthUpXYZ, _regionCurves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepRightDownXYZ, _regionCurves)
                    || IsInsideOrOnEdgeOfPolygon(_tmepMiddleXYZ, _regionCurves))
                {
                    nowObstacleCurveArrArray.Append(_curves);
                }
            }
            return nowObstacleCurveArrArray;
        }
        /// <summary>
        /// 判断一个线圈是否在另一个线圈内
        /// </summary>
        public static bool isCurveArrayInCurveArrayByBoundingBox(CurveArray selCurves, CurveArray regionCurves)
        {
            List<Line> _tempLines = CurveArrayToListLine(selCurves);
            XYZ _tmepLeftDownXYZ = GetLeftDownXYZfromLines(_tempLines);
            XYZ _tmepLeftUpnXYZ = GetLeftUpXYZfromLines(_tempLines);
            XYZ _tmepRigthUpXYZ = GetRightUpXYZfromLines(_tempLines);
            XYZ _tmepRightDownXYZ = GetRightDownXYZfromLines(_tempLines);
            XYZ _tmepMiddleXYZ = GetMiddleXYZfromLines(_tempLines);

            if (IsInsideOrOnEdgeOfPolygon(_tmepLeftDownXYZ, regionCurves)
                && IsInsideOrOnEdgeOfPolygon(_tmepLeftUpnXYZ, regionCurves)
                && IsInsideOrOnEdgeOfPolygon(_tmepRigthUpXYZ, regionCurves)
                && IsInsideOrOnEdgeOfPolygon(_tmepRightDownXYZ, regionCurves)
                && IsInsideOrOnEdgeOfPolygon(_tmepMiddleXYZ, regionCurves))
                return true;

            return false;
        }
        /// <summary>
        /// 判断一个线圈是否在另一个线圈内
        /// </summary>
        public static bool isCurveArrayInCurveArrayByAllSingleCurves(Document doc, CurveArray selCurves, CurveArray regionCurves)
        {
            foreach (Curve curve in selCurves)
            {
                XYZ xYZ01 = curve.GetEndPoint(0);
                XYZ xYZ02 = curve.GetEndPoint(1);
                if (!IsInsideOrOnEdgeOfPolygon(xYZ01, regionCurves) || !IsInsideOrOnEdgeOfPolygon(xYZ02, regionCurves))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断一个BoundingBoxXYZ的中心在不在一个CurveArray内
        /// </summary>
        public static bool isBoundingBoxCenterInRegion(BoundingBoxXYZ _boundingBoxXYZ, CurveArray _curveArray)
        {
            bool isInRegion = false;
            XYZ min = _boundingBoxXYZ.Min;
            XYZ max = _boundingBoxXYZ.Max;
            XYZ middle = 0.5 * (min + max);
            //需要注意停车位详图项目的boundingbox具有z值，需要归零
            if (IsInsidePolygon(middle, _curveArray))
            {
                isInRegion = true;
            }
            return isInRegion;
        }
        /// <summary>
        /// 判断一个BoundingBoxXYZ的(四个矩形角点、一个中心点)在不在目标CurveArray线圈内，只要一个点在，即返回true
        /// </summary>
        public static bool isBoundingBoxInRegionByFivePoints(BoundingBoxXYZ _boundingBoxXYZ, CurveArray _curveArray)
        {
            bool isInRegion = false;
            if (_boundingBoxXYZ == null)
                return false;

            XYZ min = _boundingBoxXYZ.Min;
            XYZ max = _boundingBoxXYZ.Max;
            double wight = max.X - min.X;
            double height = max.Y - max.Y;
            XYZ _tmepLeftDownXYZ = new XYZ(min.X, min.Y, 0);
            XYZ _tmepLeftUpnXYZ = new XYZ(min.X, min.Y + height, 0);
            XYZ _tmepRigthUpXYZ = new XYZ(max.X, max.Y, 0);
            XYZ _tmepRightDownXYZ = new XYZ(min.X + wight, min.Y, 0);

            XYZ _tmepMiddleXYZ = (min + max) / 2;
            _tmepMiddleXYZ = new XYZ(_tmepMiddleXYZ.X, _tmepMiddleXYZ.Y, 0);

            if (IsInsideOrOnEdgeOfPolygon(_tmepLeftDownXYZ, _curveArray)
                || IsInsideOrOnEdgeOfPolygon(_tmepLeftUpnXYZ, _curveArray)
                || IsInsideOrOnEdgeOfPolygon(_tmepRigthUpXYZ, _curveArray)
                || IsInsideOrOnEdgeOfPolygon(_tmepRightDownXYZ, _curveArray)
                || IsInsideOrOnEdgeOfPolygon(_tmepMiddleXYZ, _curveArray))
            {
                isInRegion = true;
            }

            return isInRegion;
        }
        /// <summary>
        /// 判断一个BoundingBoxXYZ的(矩形的四个边)是否与控制区域相交，只要相交，即返回true
        /// </summary>
        public static bool isBoundingBoxInRegionByIntersection(Document doc, BoundingBoxXYZ _boundingBoxXYZ, CurveArray _curveArray)
        {
            bool isInRegion = false;
            if (_boundingBoxXYZ == null)
                return false;

            XYZ min = _boundingBoxXYZ.Min;
            XYZ max = _boundingBoxXYZ.Max;
            XYZ middle = (min + max) / 2;
            double wight = Math.Abs(max.X - min.X);
            double height = Math.Abs(max.Y - min.Y);
            XYZ _tmepLeftDownXYZ = new XYZ(middle.X - wight / 2, middle.Y - height / 2, 0);
            XYZ _tmepLeftUpXYZ = new XYZ(middle.X - wight / 2, middle.Y + height / 2, 0);
            XYZ _tmepRigthUpXYZ = new XYZ(middle.X + wight / 2, middle.Y + height / 2, 0);
            XYZ _tmepRightDownXYZ = new XYZ(middle.X + wight / 2, middle.Y - height / 2, 0);

            List<Line> tempLines = new List<Line>();
            if (_tmepLeftDownXYZ.DistanceTo(_tmepLeftUpXYZ) >= PRECISION)
            {
                Line leftLine = Line.CreateBound(_tmepLeftDownXYZ, _tmepLeftUpXYZ);
                tempLines.Add(leftLine);
            }
            if (_tmepLeftUpXYZ.DistanceTo(_tmepRigthUpXYZ) >= PRECISION)
            {
                Line upLine = Line.CreateBound(_tmepLeftUpXYZ, _tmepRigthUpXYZ);
                tempLines.Add(upLine);
            }
            if (_tmepRigthUpXYZ.DistanceTo(_tmepRightDownXYZ) >= PRECISION)
            {
                Line rightLine = Line.CreateBound(_tmepRigthUpXYZ, _tmepRightDownXYZ);
                tempLines.Add(rightLine);
            }
            if (_tmepLeftDownXYZ.DistanceTo(_tmepRightDownXYZ) >= PRECISION)
            {
                Line bottomLine = Line.CreateBound(_tmepLeftDownXYZ, _tmepRightDownXYZ);
                tempLines.Add(bottomLine);
            }
            if (tempLines.Count < 1)
                return false;

            foreach (Line line in tempLines)
            {
                foreach (Curve curve in _curveArray)
                {
                    XYZ xYZ01 = curve.GetEndPoint(0);
                    XYZ xYZ02 = curve.GetEndPoint(1);
                    Line _line = Line.CreateBound(new XYZ(xYZ01.X, xYZ01.Y, 0), new XYZ(xYZ02.X, xYZ02.Y, 0));
                    SetComparisonResult intersectionResule = line.Intersect(_line);
                    if (intersectionResule == SetComparisonResult.Overlap
                        || intersectionResule == SetComparisonResult.Subset
                        || intersectionResule == SetComparisonResult.Equal)
                    {
                        isInRegion = true;
                        break;
                    }
                }
            }
            return isInRegion;
        }
        #endregion

        #region 曲线数组转化为线段列表 曲线处理 将曲线转为闭合或者非闭合多段线 线圈与曲线数组之间的转换
        /// <summary>
        /// 曲线数组列表转化曲线数组数组
        /// </summary>
        public static List<CurveArray> ConvertCurveArrArrayToListCurveArray(CurveArrArray CurveArrays)
        {
            List<CurveArray> curveArrArray = new List<CurveArray>();
            foreach (CurveArray curveArray in CurveArrays)
            {
                curveArrArray.Add(curveArray);
            }
            return curveArrArray;
        }
        /// <summary>
        /// 曲线数组数组转化曲线数组列表
        /// </summary>
        public static CurveArrArray ConvertListCurveArrayToCurveArrArray(List<CurveArray> CurveArrays)
        {
            CurveArrArray curveArrArray = new CurveArrArray();
            foreach (CurveArray curveArray in CurveArrays)
            {
                curveArrArray.Append(curveArray);
            }
            return curveArrArray;
        }
        /// <summary>
        /// 线圈列表转化为曲线数组的数组
        /// </summary>
        public static CurveArrArray CurveLoopsToCurveArrArray(List<CurveLoop> curveLoops)
        {
            CurveArrArray curveArrArray = new CurveArrArray();
            foreach (CurveLoop curveLoop in curveLoops)
            {
                curveArrArray.Append(CurveLoopToCurveArray(curveLoop));
            }
            return curveArrArray;
        }
        /// <summary>
        /// 线圈转化为曲线数组
        /// </summary>
        public static CurveArray CurveLoopToCurveArray(CurveLoop curveLoop)
        {
            CurveArray curveArray = new CurveArray();
            CurveLoopIterator curveLoopIterator = curveLoop.GetCurveLoopIterator();
            while (curveLoopIterator.MoveNext())
            {
                curveArray.Append(curveLoopIterator.Current);
            }
            return curveArray;
        }
        /// <summary>
        /// 曲线数组转化为线圈
        /// </summary>
        public static CurveLoop CurveArrayToCurveLoop(CurveArray curveArray)
        {
            CurveLoop curves = new CurveLoop();
            foreach (Curve curve in curveArray)
            {
                curves.Append(curve);
            }
            return curves;
        }
        /// <summary>
        /// 将linelist_list转化为CurveArrArray
        /// </summary>
        public static CurveArrArray ListLinesToCurveArrArray(List<List<Line>> listLines)
        {
            CurveArrArray curveArrArray = new CurveArrArray();
            foreach (List<Line> lines in listLines)
            {
                curveArrArray.Append(LinesToCurveArray(lines));
            }
            return curveArrArray;
        }
        /// <summary>
        /// 线列表转化为曲线数组
        /// </summary>
        public static CurveArray LinesToCurveArray(List<Line> _lines)
        {
            CurveArray curveArray = new CurveArray();
            foreach (Line line in _lines)
            {
                Curve _c = line as Curve;
                curveArray.Append(_c);
            }
            return curveArray;
        }
        /// <summary>
        /// 将CurveArrArray转化为linelist_list
        /// </summary>
        public static List<List<Line>> CurveArrArrayToListLines(CurveArrArray curves)
        {
            List<List<Line>> listLines = new List<List<Line>>();
            foreach (CurveArray _curves in curves)
            {
                listLines.Add(CurveArrayToListLine(_curves));
            }
            return listLines;
        }
        /// <summary>
        /// 将curvearray转化为linelist
        /// </summary>
        public static List<Line> CurveArrayToListLine(CurveArray _curves)
        {
            List<Line> lines = new List<Line>();
            foreach (Curve c in _curves)
            {
                if (c is Line)
                {
                    Line line = c as Line;
                    lines.Add(line);
                }
                else if (c is Arc)
                {
                    Arc arc = c as Arc;
                    List<Line> _lines = GetunClosedLinesFromArc(arc);
                    lines.AddRange(_lines);
                }
            }
            return lines;
        }
        /// <summary>
        /// 将原弧线转化为多段线
        /// </summary>
        public static List<Line> GetClosedLinesFromArc(Curve _Curve)
        {
            int Numberofsegments = 3;
            List<Line> _Lines = new List<Line>();
            for (int i = 0; i <= Numberofsegments; i++)
            {
                double _Spacing = 1.0 / Numberofsegments;
                Line _LIne = null;
                if (i == Numberofsegments)
                {
                    double _NumSeg_0 = i * _Spacing;
                    double _NumSeg_1 = (0) * _Spacing;
                    XYZ _YXZ_0 = _Curve.Evaluate(_NumSeg_0, true);
                    XYZ _YXZ_1 = _Curve.Evaluate(_NumSeg_1, true);
                    _LIne = Line.CreateBound(_YXZ_0, _YXZ_1);
                }
                else
                {
                    double _NumSeg_0 = i * _Spacing;
                    double _NumSeg_1 = (i + 1) * _Spacing;
                    XYZ _YXZ_0 = _Curve.Evaluate(_NumSeg_0, true);
                    XYZ _YXZ_1 = _Curve.Evaluate(_NumSeg_1, true);
                    _LIne = Line.CreateBound(_YXZ_0, _YXZ_1);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        /// <summary>
        /// 将原弧线转化为多段线
        /// </summary>
        public static List<Line> GetunClosedLinesFromArc(Curve _Curve)
        {
            int Numberofsegments = 3;
            List<Line> _Lines = new List<Line>();
            for (int i = 0; i <= Numberofsegments; i++)
            {
                double _Spacing = 1.0 / Numberofsegments;
                Line _LIne = null;
                if (i == Numberofsegments)
                {
                    continue;
                }
                else
                {
                    double _NumSeg_0 = i * _Spacing;
                    double _NumSeg_1 = (i + 1) * _Spacing;
                    XYZ _YXZ_0 = _Curve.Evaluate(_NumSeg_0, true);
                    XYZ _YXZ_1 = _Curve.Evaluate(_NumSeg_1, true);
                    _LIne = Line.CreateBound(_YXZ_0, _YXZ_1);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        #endregion

        #region 判断多根线，或者，两根线是否首位相连 或 闭合 判断两根线段是否有重叠部分，包括完全100%重合
        /// <summary>
        /// 判断两根线是否重合，这里，只要满足，一根线段的两个点到一个线段的距离，均小于极限值，即确定两个线段重合
        /// </summary>
        public static bool IsCoincide(Line line1, Line line2)
        {
            bool isCoincide = false;
            XYZ endPointc100 = line1.GetEndPoint(0);
            XYZ endPointc101 = line1.GetEndPoint(1);

            XYZ endPointc200 = line2.GetEndPoint(0);
            XYZ endPointc201 = line2.GetEndPoint(1);

            if ((line1.Distance(endPointc200) < PRECISION && line1.Distance(endPointc201) < PRECISION)
                || (line2.Distance(endPointc100) < PRECISION && line2.Distance(endPointc101) < PRECISION)) //由于clipper之后的误差为0.2mm，因此，手动创建函数 线段点到直线的距离小于1mm则属于重合的情况
            {
                isCoincide = true;
            }

            return isCoincide;
        }
        /// <summary>
        /// 判断所有直线是否首位相连，该函数只能判断出每个线段的端点被重合一次，是不是属于曲线闭合需要与iscurveloop函数一起使用
        /// </summary>
        public static bool IsCurveLoop(CurveArray _curves, out CurveLoop _curveLoop)
        {
            _curveLoop = new CurveLoop();
            int count = _curves.Size * 2;//被判断交点的数量
            int _count = 0;//求出交点的数量
            bool _isLoop = false;
            foreach (Curve c in _curves)
            {
                XYZ endpoing_0 = c.GetEndPoint(0);
                XYZ endpoing_1 = c.GetEndPoint(1);
                foreach (Curve _c in _curves)
                {
                    if (!c.Equals(_c))
                    {
                        XYZ _endpoing_0 = _c.GetEndPoint(0);
                        XYZ _endpoing_1 = _c.GetEndPoint(1);

                        if (endpoing_0.DistanceTo(_endpoing_0) < PRECISION
                            || endpoing_1.DistanceTo(_endpoing_0) < PRECISION
                            || endpoing_0.DistanceTo(_endpoing_1) < PRECISION
                            || endpoing_1.DistanceTo(_endpoing_1) < PRECISION)//判断逻辑为，一个线段的两个点都会被重合一次，因此，重合总次数，为线段端点的2倍
                        {
                            _count += 1;
                        }
                    }
                }
            }
            //TaskDialogShowMessage(count.ToString());
            //TaskDialogShowMessage(_count.ToString());
            if (count == _count)
            {
                _isLoop = true;
                _curves = SortCurves(_curves);//线段需要sort，进行排序
                foreach (Curve c in _curves)
                {
                    _curveLoop.Append(c);
                }
            }
            else
            {
                //throw new InvalidOperationException("线圈存在不闭合的情况，当前容差值为 0.0005*25.4 = 0.0127mm.");
                //throw new InvalidOperationException("线圈存在不闭合的情况，当前容差值为 0.1*304 = 2.54mm.");
            }
            return _isLoop;
        }
        /// <summary>
        /// 判断两根相交线段 结果为1，则是 L型 or V型；为0，则是X型 or T型， 或不相交； 如果是方向一致的线，1则为首尾相连 线段，2则为重叠
        /// </summary>
        public static int is_L_or_V_twolinesoverlap(Line _line1, Line _line2)
        {

            int _intersectCount = 0;
            XYZ _line1_start = _line1.GetEndPoint(0);
            XYZ _line1_end = _line1.GetEndPoint(1);
            XYZ _line2_start = _line2.GetEndPoint(0);
            XYZ _line2_end = _line2.GetEndPoint(1);
            if (_line1_start.IsAlmostEqualTo(_line2_start, PRECISION))
            {
                _intersectCount += 1;
            }
            if (_line1_end.IsAlmostEqualTo(_line2_start, PRECISION))
            {
                _intersectCount += 1;
            }
            if (_line1_start.IsAlmostEqualTo(_line2_end, PRECISION))
            {
                _intersectCount += 1;
            }
            if (_line1_end.IsAlmostEqualTo(_line2_end, PRECISION))
            {
                _intersectCount += 1;
            }

            return _intersectCount;
        }
        #endregion

        #region 打断相交线段 相交线段（V字型、L型）倒角
        /// <summary>
        /// 对所有相交线段 中 处于V字型和L型的 线段进行倒角 输出倒角圆弧 线段列表引用不变
        /// </summary>
        public static List<Arc> ChamferAll_LorVLines(List<Line> _Lines, double radius)
        {
            List<Arc> _Arces = new List<Arc>();
            int _count = _Lines.Count;
            for (int i = 0; i < _count; i++)
            {
                for (int j = 0; j < 2; j++)//判断一根线与其他所有线段的关系，对两个端点，分别进行判断
                {
                    int temp = i;
                    XYZ _i_xyz = _Lines[i].GetEndPoint(j);
                    List<int> list_int = new List<int>();
                    int coincide_count = IsXYZcoincideAllLines(_i_xyz, _Lines, out list_int);//找到一个点与所有线段的重合端点线段，并输出重合端点的线段索引值
                    if (coincide_count == 1)//当目标点与所有线段只有一个重合的情况时，进行倒角，并输出倒角后的裁剪线段
                    {
                        int k = list_int[0];//此处需要判断 索引值是否相同
                        if (k != i)
                        {
                            Line _newline1 = null;
                            Line _newline2 = null;
                            Arc _arc = Chamfer(_Lines[i], _Lines[k], radius, out _newline1, out _newline2);
                            _Arces.Add(_arc);
                            _Lines[i] = _newline1;
                            _Lines[k] = _newline2;
                            i--;//当前 i 索引线段被更新 所以回滚重新判断 
                            continue;
                        }
                    }
                    else if (coincide_count == 2)//与自身端点相交一次，比其它线段相交一次
                    {
                        foreach (int m in list_int)
                        {
                            if (m != i)
                            {
                                Line _newline1 = null;
                                Line _newline2 = null;
                                Arc _arc = Chamfer(_Lines[i], _Lines[m], radius, out _newline1, out _newline2);
                                _Arces.Add(_arc);
                                _Lines[i] = _newline1;
                                _Lines[m] = _newline2;
                                i--;//当前 i 索引线段被更新 所以回滚重新判断 
                                continue;
                            }
                        }

                    }
                    if (temp != i)//此处需要注意：不管是 j 重合一次，还是重合两次，都只能回滚一次，因为，一根线段具有两个端点
                    {
                        break;
                    }
                }
            }

            return _Arces;
        }
        /// <summary>
        /// 找到一个点与所有线段的重合端点线段，并输出重合端点的线段索引值
        /// </summary>
        /// <param name="_i_xyz"></param>
        /// <param name="_Lines"></param>
        /// <param name="Lines"></param>
        /// <returns></returns>
        public static int IsXYZcoincideAllLines(XYZ _i_xyz, List<Line> _Lines, out List<int> list_int)
        {
            list_int = new List<int>();//收集与 i 线段的一个端点重合线段的索引值
            int _count = _Lines.Count;
            for (int k = 0; k < _count; k++)
            {
                XYZ _k_xyz_0 = _Lines[k].GetEndPoint(0);
                XYZ _k_xyz_1 = _Lines[k].GetEndPoint(1);

                if (_i_xyz.IsAlmostEqualTo(_k_xyz_0, PRECISION) || _i_xyz.IsAlmostEqualTo(_k_xyz_1, PRECISION))//判断线段端点是否重合
                {
                    list_int.Add(k);
                }
            }
            return list_int.Count;
        }
        /// <summary>
        /// 把两个相交L、V型线段，如果不是，则基于交点进行裁剪 只留下L、V型，进行倒角处理
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <param name="radius"></param>
        /// <param name="_newline1"></param>
        /// <param name="_newline2"></param>
        /// <returns></returns>
        public static Arc Chamfer(Line _line1, Line _line2, double radius, out Line _newline1, out Line _newline2)
        {
            List<Line> lines = CutOffTwoLineByIntersectionPoint(_line1, _line2);//把两个相交线段，基于交点进行裁剪 只留下L、V型，进行倒角处理 该处处理，可以保证线的起点与终点的先后位置
            XYZ intersectionPoint = GetIntersetionPointFromTwoLines(_line1, _line2);//求出L or V交点
            XYZ direction_line1 = lines[0].Direction;//求方向，再求角度
            XYZ direction_line2 = lines[1].Direction;
            double angle = direction_line1.AngleTo(direction_line2);
            if (angle > Math.PI)
            {
                angle = Math.PI * 2 - angle;
            }
            //求圆弧的三角函数问题，需要深究——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
            XYZ point_arc_start = intersectionPoint.Add(direction_line1.Multiply(radius / (Math.Tan(angle / 2))));//使用正切三角函数，求出，圆弧与倒角直线相切点的位置
            XYZ point_arc_end = intersectionPoint.Add(direction_line2.Multiply(radius / (Math.Tan(angle / 2))));

            XYZ direction_line3 = direction_line1.Add(direction_line2).Normalize();

            XYZ point_arc_middle = intersectionPoint.Add(direction_line3.Multiply((radius / Math.Sin(angle / 2)) - radius));// 使用正弦三角函数，求出，圆弧与倒角直线相切点的位置

            Arc arc = Arc.Create(point_arc_start, point_arc_end, point_arc_middle);
            _newline1 = Line.CreateBound(point_arc_start, lines[0].GetEndPoint(1));
            _newline2 = Line.CreateBound(point_arc_end, lines[1].GetEndPoint(1));
            return arc;
        }
        /// <summary>
        /// 将所有相交线段 基于交点 打断
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static List<Line> CutOffAllIntersetionLines(List<Line> _Lines)
        {
            for (int i = 0; i < _Lines.Count; i++)
            {
                for (int j = 0; j < _Lines.Count; j++)
                {
                    if (i != j)
                    {
                        SetComparisonResult _SetComparisonResult = _Lines[i].Intersect(_Lines[j]);//判断两根线段间的关系
                        if (_SetComparisonResult == SetComparisonResult.Overlap)
                        {
                            List<Line> temps = CutOffTwoIntersetionLines(_Lines[i], _Lines[j]);
                            if (temps.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                _Lines.RemoveAt(j);
                                _Lines.RemoveAt(i);
                                _Lines.AddRange(temps);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
            return _Lines;
        }
        /// <summary>
        /// 基于相交点，切断相交直线，各自留下长的那一段  将相交线打断，留下长的那一半，十字型相交线，切割成L V型相交线段，该处选择留下长的那一部分
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <returns></returns>
        public static List<Line> CutOffTwoLineByIntersectionPoint(Line _line1, Line _line2)
        {
            List<Line> lines = new List<Line>();
            XYZ intersection = GetIntersetionPointFromTwoLines(_line1, _line2);//获取两个线的交点
            if (intersection == null)
            {
                return lines;
            }
            XYZ xyz_start1 = _line1.GetEndPoint(0);
            XYZ xyz_end1 = _line1.GetEndPoint(1);
            XYZ xyz_start2 = _line2.GetEndPoint(0);
            XYZ xyz_end2 = _line2.GetEndPoint(1);
            if (intersection.DistanceTo(xyz_start1) > intersection.DistanceTo(xyz_end1))
            {
                Line _newline = Line.CreateBound(intersection, xyz_start1);
                lines.Add(_newline);
            }
            else
            {
                Line _newline = Line.CreateBound(intersection, xyz_end1);
                lines.Add(_newline);
            }
            if (intersection.DistanceTo(xyz_start2) > intersection.DistanceTo(xyz_end2))
            {
                Line _newline = Line.CreateBound(intersection, xyz_start2);
                lines.Add(_newline);
            }
            else
            {
                Line _newline = Line.CreateBound(intersection, xyz_end2);
                lines.Add(_newline);
            }
            return lines;
        }
        /// <summary>
        /// 打断两根相交线段 L型 V型除外
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public static List<Line> CutOffTwoIntersetionLines(Line _line1, Line _line2)
        {
            List<Line> lines = new List<Line>();

            int _intersectCount = is_L_or_V_twolinesoverlap(_line1, _line2);// 判断两根相交线段 结果为1，则是 L型 or V型；为0，则是X型 or T型
            if (_intersectCount == 0)
            {
                lines = CutOffTwoIntersetionLines_X_T(_line1, _line2);
            }
            return lines;
        }
        /// <summary>
        /// 打断两根 X型 T型 相交线段
        /// </summary>
        /// <param name="_line1"></param>
        /// <param name="_line2"></param>
        /// <returns></returns>
        public static List<Line> CutOffTwoIntersetionLines_X_T(Line _line1, Line _line2)
        {
            List<Line> lines = new List<Line>();
            XYZ _line1_start = _line1.GetEndPoint(0);
            XYZ _line1_end = _line1.GetEndPoint(1);
            XYZ _line2_start = _line2.GetEndPoint(0);
            XYZ _line2_end = _line2.GetEndPoint(1);
            XYZ intersectionPint = GetIntersetionPointFromTwoLines(_line1, _line2);
            //该处需要判断是源于 T型 的存在
            if (!intersectionPint.IsAlmostEqualTo(_line1_start, PRECISION))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line1_start));
            }
            if (!intersectionPint.IsAlmostEqualTo(_line1_end, PRECISION))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line1_end));
            }
            if (!intersectionPint.IsAlmostEqualTo(_line2_start, PRECISION))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line2_start));
            }
            if (!intersectionPint.IsAlmostEqualTo(_line2_end, PRECISION))
            {
                lines.Add(Line.CreateBound(intersectionPint, _line2_end));
            }
            return lines;
        }
        #endregion

        #region 曲线 涵盖线段 排序问题
        /// <summary>
        /// 闭合多线段圈排序
        /// </summary>
        public static List<Line> SortLines(List<Line> lines)
        {
            CurveArray _curves = new CurveArray();
            foreach (Line _line in lines)
            {
                Curve _curve = _line as Curve;
                _curves.Append(_curve);
            }
            CurveArray curves = SortCurves(_curves);//线段需要sort，进行排序 gao common 中的排序函数，可能存在问题
            lines = new List<Line>();//曲线分段过多，导致性能降低
            foreach (Curve _Curve in curves)
            {
                Line _line = _Curve as Line;
                lines.Add(_line);
            }
            return lines;
        }
        /// <summary>
        /// 闭合曲线圈排序
        /// </summary>
        public static CurveArray SortCurves(CurveArray curves)
        {
            CurveArray _curves = new CurveArray();
            XYZ temp = curves.get_Item(0).GetEndPoint(1);//拿出一根线段的一个终点
            Curve temCurve = curves.get_Item(0);//拿出一根线段
            _curves.Append(temCurve);//添加第一根线段
            while (_curves.Size != curves.Size)//循环停止条件
            {
                temCurve = GetNext(curves, temp, temCurve);//寻找相连线段
                if (Math.Abs(temp.X - temCurve.GetEndPoint(0).X) < PRECISION
                    && Math.Abs(temp.Y - temCurve.GetEndPoint(0).Y) < PRECISION)
                {
                    temp = temCurve.GetEndPoint(1);
                }
                else
                {
                    temp = temCurve.GetEndPoint(0);
                }
                _curves.Append(temCurve);
            }
            if (Math.Abs(temp.X - curves.get_Item(0).GetEndPoint(0).X) > PRECISION
                || Math.Abs(temp.Y - curves.get_Item(0).GetEndPoint(0).Y) > PRECISION
                || Math.Abs(temp.Z - curves.get_Item(0).GetEndPoint(0).Z) > PRECISION)//temp最后一次，应该就是选段闭合的起点
            {
                throw new InvalidOperationException("CurveLoop may be unclosed_Sort. 不满足curveLoop容差值 0.0005*25.4 = 0.0127mm.");
            }
            return _curves;
        }
        public static Curve GetNext(CurveArray curves, XYZ connected, Curve currentCurve)//找到连接线段 便有返回值
        {
            foreach (Curve c in curves)
            {
                if (c.Equals(currentCurve))//判断两根线段是否为同一物体
                {
                    continue;
                }
                if ((Math.Abs(c.GetEndPoint(0).X - currentCurve.GetEndPoint(1).X) < PRECISION
                    && Math.Abs(c.GetEndPoint(0).Y - currentCurve.GetEndPoint(1).Y) < PRECISION
                    && Math.Abs(c.GetEndPoint(0).Z - currentCurve.GetEndPoint(1).Z) < PRECISION)
                    && (Math.Abs(c.GetEndPoint(1).X - currentCurve.GetEndPoint(0).X) < PRECISION
                    && Math.Abs(c.GetEndPoint(1).Y - currentCurve.GetEndPoint(0).Y) < PRECISION
                    && Math.Abs(c.GetEndPoint(1).Z - currentCurve.GetEndPoint(0).Z) < PRECISION)
                    && 2 != curves.Size)//该条件判断 c 的 起点 终点 与 connect的 终点 起点 是否重合 判断两根线段是否重合
                {
                    continue;
                }
                if (c.GetEndPoint(0).DistanceTo(connected) < PRECISION)//该条件成立，则说明 c 的终点与 connect 的终点相连
                {
                    return c;
                }
                else if (c.GetEndPoint(1).DistanceTo(connected) < PRECISION)//该条件成立，则说明 c 的终点与 connect 的终点相连 需要将线段重新绘制
                {
                    if (c is Line)
                    {
                        XYZ start = c.GetEndPoint(1);
                        XYZ end = c.GetEndPoint(0);
                        return Line.CreateBound(start, end);
                    }
                    else if (c is Arc)
                    {
                        int size = c.Tessellate().Count;
                        XYZ start = c.GetEndPoint(1);
                        XYZ middle = c.Tessellate()[size / 2];
                        XYZ end = c.GetEndPoint(0);
                        return Arc.Create(start, end, middle);
                        //return Line.CreateBound(connected, end);

                    }
                }
            }
            throw new InvalidOperationException("CurveLoop may be unclosed_GetNext.");
        }
        #endregion

        #region  线段处理 求围合面积 列表所在矩形的四个角点 修改线样式 获取线的端点列表 求线段交点
        /// <summary>
        /// 获得线段集所在矩形的中心的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetMiddleXYZfromLines(CurveArray _curves)
        {
            Path_xyz points = GetUniqueXYZFromCurves(_curves);
            return GetMiddleXYZfromPoints(points);
        }
        /// <summary>
        /// 获得线段集所在矩形的中心的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetMiddleXYZfromLines(List<Line> _Lines)
        {
            Path_xyz points = GetUniqueXYZFromLines(_Lines);
            return GetMiddleXYZfromPoints(points);
        }
        /// <summary>
        /// 获得点集所在矩形的中心的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetMiddleXYZfromPoints(Path_xyz pathXyz)
        {
            XYZ leftDownXYZ = GetLeftDownXYZfromPoints(pathXyz);
            XYZ rigthUpXYZ = GetRightUpXYZfromPoints(pathXyz);

            return (leftDownXYZ + rigthUpXYZ) * 0.5;
        }
        /// <summary>
        /// 获得点集所在矩形的左下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftDownXYZfromPoints(Path_xyz pathXyz)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;

            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_min, _Y_min, 0.0);

            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的左下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftDownXYZfromLines(List<Line> _Lines)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetLeftDownXYZfromPoints(pathXyz);
        }
        /// <summary>
        /// 获得所有线段所在矩形的左下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftDownXYZfromLines(CurveArray curveArray)
        {
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetLeftDownXYZfromPoints(pathXyz);
        }
        /// <summary>
        /// 获得点集所在矩形的左上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftUpXYZfromPoints(Path_xyz pathXyz)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;

            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_min, _Y_max, 0.0);

            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的左上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftUpXYZfromLines(List<Line> _Lines)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetLeftUpXYZfromPoints(pathXyz);
        }
        /// <summary>
        /// 获得所有线段所在矩形的左上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftUpXYZfromLines(CurveArray curveArray)
        {
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetLeftUpXYZfromPoints(pathXyz);
        }
        /// <summary>
        /// 获得点集所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightUpXYZfromPoints(Path_xyz pathXyz)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;

            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_max, _Y_max, 0.0);

            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightUpXYZfromLines(List<Line> _Lines)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetRightUpXYZfromPoints(pathXyz);

        }
        /// <summary>
        /// 获得所有线段所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightUpXYZfromLines(CurveArray curveArray)
        {
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetRightUpXYZfromPoints(pathXyz);
        }

        /// <summary>
        /// 获得点集所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightDownXYZfromPoints(Path_xyz pathXyz)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;

            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_max, _Y_min, 0.0);

            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的右下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightDownXYZfromLines(List<Line> _Lines)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetRightDownXYZfromPoints(pathXyz);
        }
        /// <summary>
        /// 获得所有线段所在矩形的右下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightDownXYZfromLines(CurveArray curveArray)
        {
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            return GetRightDownXYZfromPoints(pathXyz);
        }
        /// <summary>
        /// 获得点集所在矩形的左下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftDownXYZfromPoints(Path_xyz pathXyz, out double distance_x, out double distance_y)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;
            distance_x = 0;
            distance_y = 0;
            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_min, _Y_min, 0.0);
            distance_x = _X_max - _X_min;
            distance_y = _Y_max - _Y_min;
            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的左下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftDownXYZfromLines(List<Line> _Lines, out double distance_x, out double distance_y)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            distance_x = 0;
            distance_y = 0;

            return GetLeftDownXYZfromPoints(pathXyz, out distance_x, out distance_y);
        }
        /// <summary>
        /// 获得点集所在矩形的左上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftUpXYZfromPoints(Path_xyz pathXyz, out double distance_x, out double distance_y)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;
            distance_x = 0;
            distance_y = 0;
            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_min, _Y_max, 0.0);
            distance_x = _X_max - _X_min;
            distance_y = _Y_max - _Y_min;
            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的左上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetLeftUpXYZfromLines(List<Line> _Lines, out double distance_x, out double distance_y)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            distance_x = 0;
            distance_y = 0;

            return GetLeftUpXYZfromPoints(pathXyz, out distance_x, out distance_y);
        }
        /// <summary>
        /// 获得点集所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightUpXYZfromPoints(Path_xyz pathXyz, out double distance_x, out double distance_y)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;
            distance_x = 0;
            distance_y = 0;
            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_max, _Y_max, 0.0);
            distance_x = _X_max - _X_min;
            distance_y = _Y_max - _Y_min;
            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRigthUpXYZfromLines(List<Line> _Lines, out double distance_x, out double distance_y)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            distance_x = 0;
            distance_y = 0;

            return GetRightUpXYZfromPoints(pathXyz, out distance_x, out distance_y);
        }
        /// <summary>
        /// 获得点集所在矩形的右上角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRigthDownXYZfromPoints(Path_xyz pathXyz, out double distance_x, out double distance_y)
        {
            double _X_min = pathXyz[0].X;
            double _Y_min = pathXyz[0].Y;
            double _X_max = pathXyz[0].X;
            double _Y_max = pathXyz[0].Y;
            distance_x = 0;
            distance_y = 0;
            foreach (XYZ _xyz in pathXyz)
            {
                //谁小选谁
                _X_min = _X_min <= _xyz.X ? _X_min : _xyz.X;

                _Y_min = _Y_min <= _xyz.Y ? _Y_min : _xyz.Y;
                //谁大选谁
                _X_max = _X_max >= _xyz.X ? _X_max : _xyz.X;

                _Y_max = _Y_max >= _xyz.Y ? _Y_max : _xyz.Y;
            }
            XYZ _XYZ = new XYZ(_X_max, _Y_min, 0.0);
            distance_x = _X_max - _X_min;
            distance_y = _Y_max - _Y_min;
            return _XYZ;
        }
        /// <summary>
        /// 获得所有线段所在矩形的右下角的点坐标 //LeftDown LeftUp RigthUp RightDown
        /// </summary>
        public static XYZ GetRightDownXYZfromLines(List<Line> _Lines, out double distance_x, out double distance_y)
        {
            CurveArray curveArray = LinesToCurveArray(_Lines);
            Path_xyz pathXyz = GetUniqueXYZFromCurves(curveArray);

            distance_x = 0;
            distance_y = 0;

            return GetRigthDownXYZfromPoints(pathXyz, out distance_x, out distance_y);
        }
        /// <summary>
        /// 获取一系列线中的最下部水平线 前提是，最下部为水平线
        /// </summary>
        /// <returns></returns>
        public static Line GetBottomLineFromLines(List<Line> _Lines)
        {
            List<XYZ> middleXYZs = new Path_xyz();

            foreach (Line line in _Lines)
            {
                middleXYZs.Add(GetLineMiddlePoint(line));
            }

            int count = _Lines.Count;

            int index = 0;
            double minY = middleXYZs[index].Y;
            for (int i = 0; i < count; i++)
            {
                if (minY >= middleXYZs[i].Y)
                {
                    minY = middleXYZs[i].Y;
                    index = i;
                }
            }
            return _Lines[index];
        }
        /// <summary>
        /// 获取一系列线中的最上部水平线 前提是，最上部为水平线
        /// </summary>
        /// <returns></returns>
        public static Line GetUpLineFromLines(List<Line> _Lines)
        {
            List<XYZ> middleXYZs = new Path_xyz();

            foreach (Line line in _Lines)
            {
                middleXYZs.Add(GetLineMiddlePoint(line));
            }

            int count = _Lines.Count;

            int index = 0;
            double minY = middleXYZs[index].Y;
            for (int i = 0; i < count; i++)
            {
                if (minY <= middleXYZs[i].Y)
                {
                    minY = middleXYZs[i].Y;
                    index = i;
                }
            }
            return _Lines[index];
        }
        public static XYZ GetLineMiddlePoint(Line line)
        {
            XYZ _startPoint = line.GetEndPoint(0);
            XYZ _endPoint = line.GetEndPoint(1);

            return (_startPoint + _endPoint) / 2;
        }
        /// <summary>
        /// 求线段列表所在的平面矩形
        /// </summary>
        public static List<Line> GetRectengleFromLines(List<Line> _Lines)
        {
            //不可以用 0 作为首次被比选数据
            Line _Line_0 = _Lines[0];
            double _X_min = _Line_0.GetEndPoint(0).X;
            double _Y_min = _Line_0.GetEndPoint(0).Y;
            double _X_max = _Line_0.GetEndPoint(0).X;
            double _Y_max = _Line_0.GetEndPoint(0).Y;

            foreach (Line _Line in _Lines)
            {
                XYZ _endpoing_0 = _Line.GetEndPoint(0);
                XYZ _endpoing_1 = _Line.GetEndPoint(1);
                //谁小选谁
                _X_min = _X_min <= _endpoing_0.X ? _X_min : _endpoing_0.X;
                _X_min = _X_min <= _endpoing_1.X ? _X_min : _endpoing_1.X;

                _Y_min = _Y_min <= _endpoing_0.Y ? _Y_min : _endpoing_0.Y;
                _Y_min = _Y_min <= _endpoing_1.Y ? _Y_min : _endpoing_1.Y;
                //谁大选谁
                _X_max = _X_max >= _endpoing_0.X ? _X_max : _endpoing_0.X;
                _X_max = _X_max >= _endpoing_1.X ? _X_max : _endpoing_1.X;

                _Y_max = _Y_max >= _endpoing_0.Y ? _Y_max : _endpoing_0.Y;
                _Y_max = _Y_max >= _endpoing_1.Y ? _Y_max : _endpoing_1.Y;
            }

            XYZ _XYZ = new XYZ(_X_max, _Y_min, 0.0);

            XYZ line_leftdown = new XYZ(_X_min, _Y_min, 0);
            XYZ line_leftup = new XYZ(_X_min, _Y_max, 0);
            XYZ line_rightup = new XYZ(_X_max, _Y_max, 0);
            XYZ line_rightdown = new XYZ(_X_max, _Y_min, 0);

            Line line_0 = Line.CreateBound(line_leftdown, line_leftup);
            Line line_1 = Line.CreateBound(line_leftup, line_rightup);
            Line line_2 = Line.CreateBound(line_rightup, line_rightdown);
            Line line_3 = Line.CreateBound(line_rightdown, line_leftdown);

            return new List<Line>() { line_0, line_1, line_2, line_3 };
        }
        /// <summary>
        /// 获取两根线段的交点
        /// </summary>
        public static XYZ GetIntersetionPointFromTwoLines(Line _line1, Line _line2)
        {
            IntersectionResultArray intersectionResultArray;
            XYZ intersection = null;
            SetComparisonResult _SetComparisonResult = _line1.Intersect(_line2, out intersectionResultArray);

            if (_SetComparisonResult == SetComparisonResult.Overlap)
            {
                intersection = intersectionResultArray.get_Item(0).XYZPoint;
            }
            else
            {
                return null;
                throw new NotImplementedException("线段无非水平相交点");
            }
            return intersection;
        }
        /// <summary>
        /// Shoelace公式  鞋带法计算面积，需要进行进一步研究
        /// </summary>
        public static double GetAreafromLines(CurveArray _curves)
        {
            //获取线段断点列表，并去重
            Path_xyz _XYZs = GetUniqueXYZFromCurves(_curves);
            //求面积
            return GetAreafromPoints(_XYZs);
        }
        /// <summary>
        /// 修改线样式
        /// </summary>
        public static void SetLineStyle(Document doc, string tarLineStyleName, CurveElement _CurveElement)
        {
            ICollection<ElementId> LineStyleIds = GetAllLineStyleIdsFromSetting(doc);
            foreach (ElementId _eleId in LineStyleIds)
            {
                GraphicsStyle _lineStyle = doc.GetElement(_eleId) as GraphicsStyle;
                string _name = _lineStyle.Name;
                if (_name == tarLineStyleName)
                {
                    _CurveElement.LineStyle = _lineStyle;
                }
            }
        }
        /// <summary>
        /// 获取所有线的排序后的端点，CurveArrArray
        /// </summary>
        public static Paths_xyz GetUniqueXYZFromCurves(CurveArrArray _curves)
        {
            Paths_xyz pathsXyz = new Paths_xyz();
            foreach (CurveArray _ces in _curves)
            {
                pathsXyz.Add(GetUniqueXYZFromCurves(_ces));
            }
            return pathsXyz;
        }
        /// <summary>
        /// 获取所有线的排序后的端点，要提前开展排序工作，借助于goa common 并去重
        /// </summary>
        public static Path_xyz GetUniqueXYZFromCurves(CurveArray _curves)
        {
            Path_xyz _xyzs = new Path_xyz();
            foreach (Curve c in _curves)
            {
                if (c.GetType().Name.Equals("Line"))
                {
                    _xyzs.Add(c.GetEndPoint(0));
                    _xyzs.Add(c.GetEndPoint(1));
                }
                else if (c.GetType().Name.Equals("Arc"))
                {
                    int size = c.Tessellate().Count;
                    _xyzs.Add(c.Tessellate()[0]);
                    _xyzs.Add(c.Tessellate()[size / 2]);
                    _xyzs.Add(c.Tessellate()[size - 1]);
                }
            }
            //列表去重
            return Pointlistdeduplication(_xyzs).ToList();
        }
        /// <summary>
        /// 获取所有线的排序后的端点，CurveArrArray
        /// </summary>
        public static Paths_xyz GetUniqueXYZFromLines(List<List<Line>> _lines)
        {
            Paths_xyz pathsXyz = new Paths_xyz();

            foreach (List<Line> lines in _lines)
            {
                pathsXyz.Add(GetUniqueXYZFromLines(lines));
            }
            return pathsXyz;
        }
        /// <summary>
        /// 获取所有线的排序后的端点，要提前开展排序工作，借助于goa common 并去重
        /// </summary>
        public static Path_xyz GetUniqueXYZFromLines(List<Line> _lines)
        {
            Path_xyz _xyzs = new Path_xyz();
            foreach (Line l in _lines)
            {
                _xyzs.Add(l.GetEndPoint(0));
                _xyzs.Add(l.GetEndPoint(1));
            }
            //列表去重
            return Pointlistdeduplication(_xyzs).ToList();
        }
        #endregion

        #region 图元获取 过滤锁定元素 图元与图元Id之间的转换

        /// <summary>
        /// 从单个 模型线组 元素中读取LineList 如果组中含有曲线，会把曲线转化为多线段
        /// </summary>
        public static CurveArray GetCurvesFromCurveGroup(Group curveGroup)
        {
            CurveArray _curves = new CurveArray();
            Document doc = curveGroup.Document;
            List<ElementId> _groupEleIds = curveGroup.GetMemberIds().ToList();
            List<Element> _groupEles = EleIdsToEles(doc, _groupEleIds);
            _curves = ConvetDocLinesTolines(_groupEles);
            return _curves;
        }
        /// <summary>
        /// 提取 详图线 或者 模型线 的 lines
        /// </summary>
        public static CurveArray ConvetDocLinesTolines(List<Element> _eles)
        {
            CurveArray curves = new CurveArray();
            foreach (Element _ele in _eles)
            {
                if (_ele is DetailLine)
                {
                    DetailLine _detailLine = _ele as DetailLine;
                    Curve _curve = _detailLine.GeometryCurve;
                    curves.Append(_curve);
                }
                else if (_ele is ModelLine)
                {
                    ModelLine _modelLine = _ele as ModelLine;
                    Curve _curve = _modelLine.GeometryCurve;
                    curves.Append(_curve);
                }
                else if (_ele is DetailCurve)//将圆弧转化为多段线，取列表并集
                {
                    DetailCurve _cetaillArc = _ele as DetailCurve;
                    Curve _curve = _cetaillArc.GeometryCurve;
                    curves.Append(_curve);
                }
                else if (_ele is ModelCurve)//将圆弧转化为多段线，取列表并集
                {
                    ModelCurve _modelArc = _ele as ModelCurve;
                    Curve _curve = _modelArc.GeometryCurve;
                    curves.Append(_curve);
                }
            }
            return curves;
        }
        /// <summary>
        /// 剔除锁定图元
        /// </summary>
        public static ICollection<ElementId> FileterPinnedElement(Document doc, ICollection<ElementId> allEleIds)
        {
            ICollection<ElementId> _eleIds = new List<ElementId>();
            List<Element> allEles = EleIdsToEles(doc, allEleIds.ToList());
            foreach (Element ele in allEles)
            {
                bool isPinned = ele.Pinned;
                if (!isPinned)
                {
                    _eleIds.Add(ele.Id);
                }
            }
            return _eleIds;
        }
        /// <summary>
        /// 元素Id列表转换为元素列表
        /// </summary>
        public static List<Element> EleIdsToEles(Document doc, List<ElementId> _EleIds)
        {
            List<Element> _Eles = new List<Element>();
            foreach (ElementId _EleId in _EleIds)
            {
                Element _Ele = doc.GetElement(_EleId);
                _Eles.Add(_Ele);
            }
            return _Eles;
        }
        /// <summary>
        /// 元素列表转换为元素Id列表
        /// </summary>
        public static List<ElementId> ElesToEleIds(List<Element> _Eles)
        {
            List<ElementId> _EleIds = new List<ElementId>();
            foreach (Element _Ele in _Eles)
            {
                _EleIds.Add(_Ele.Id);
            }
            return _EleIds;
        }
        #endregion

        #region 点集处理 英寸下 将坐标值规整到两位小数 求闭合或者非闭合多段线 求围合面积 扫描线处理 判断线段中，是否存在三点共线

        /// <summary>
        /// 将 列表 顺序点集 连接成非多段线 
        /// </summary>
        public static List<List<Line>> GetListClosedtLineFromListPoints(Paths_xyz listXyzs)
        {
            List<List<Line>> listLines = new List<List<Line>>();
            foreach (Path_xyz _XYZs in listXyzs)
            {
                listLines.Add(GetClosedLinesFromPoints(_XYZs));
            }
            return listLines;
        }
        /// <summary>
        /// 将 列表 顺序点集 连接成闭合非多段线 
        /// </summary>
        public static List<List<Line>> GetListunClosedtLineFromListPoints(Paths_xyz listXyzs)
        {
            List<List<Line>> listLines = new List<List<Line>>();
            foreach (Path_xyz _XYZs in listXyzs)
            {
                listLines.Add(GetunClosedLinesFromPoints(_XYZs));
            }
            return listLines;
        }
        /// <summary>
        /// 将顺序点集 连接成闭合多段线
        /// </summary>
        public static List<Line> GetClosedLinesFromPoints(Path_xyz XYZs)
        {
            List<Line> _Lines = new List<Line>();

            //点集需要去重
            Path_xyz _XYZs = Pointlistdeduplication(XYZs);

            int _count = _XYZs.Count;
            for (int i = 0; i < _count; i++)
            {
                Line _LIne = null;
                if (i == _count - 1)
                {
                    _LIne = Line.CreateBound(_XYZs[i], _XYZs[0]);
                }
                else
                {
                    _LIne = Line.CreateBound(_XYZs[i], _XYZs[i + 1]);
                }
                _Lines.Add(_LIne);
            }

            return _Lines;
        }
        /// <summary>
        /// 将顺序点集 连接成非闭合多段线
        /// </summary>
        public static List<Line> GetunClosedLinesFromPoints(Path_xyz XYZs)
        {
            List<Line> _Lines = new List<Line>();

            //点集需要去重
            Path_xyz _XYZs = Pointlistdeduplication(XYZs);

            int _count = _XYZs.Count;
            for (int i = 0; i < _count; i++)
            {
                Line _LIne = null;
                if (i == _count - 1)
                {
                    continue;
                }
                else
                {
                    _LIne = Line.CreateBound(_XYZs[i], _XYZs[0]);
                }
                _Lines.Add(_LIne);
            }
            return _Lines;
        }
        /// <summary>
        ///  Shoelace公式  鞋带法计算面积，需要进行进一步研究
        /// </summary>
        public static double GetAreafromPoints(Path_xyz _XYZs)
        {
            int count = _XYZs.Count;
            double area = _XYZs[count - 1].X * _XYZs[0].Y - _XYZs[0].X * _XYZs[count - 1].Y;
            for (int i = 1; i < count; i++)
            {
                int j = i - 1;
                area += _XYZs[j].X * _XYZs[i].Y;
                area -= _XYZs[i].X * _XYZs[j].Y;
            }
            return Math.Abs(0.5 * area);
        }
        /// <summary>
        /// 点坐标值 取 两位小数 为sweep做准备
        /// </summary>
        public static Path_xyz PointsCoordinateTakeTwoDecimal(Path_xyz _XYZs, int _decimal)
        {
            Path_xyz _path_Xyz = new Path_xyz();
            foreach (XYZ xyz in _XYZs)
            {
                double x = TakeNumberAfterDecimal(xyz.X, _decimal);
                double y = TakeNumberAfterDecimal(xyz.Y, _decimal);
                _path_Xyz.Add(new XYZ(x, y, 0));
            }
            return _path_Xyz;
        }
        /// <summary>
        /// 点列表去重
        /// </summary>
        public static List<XYZ> Pointlistdeduplication(Path_xyz _XYZs)
        {
            //列表去重
            List<XYZ> __XYZs = new List<XYZ>();
            foreach (XYZ _XYZ in _XYZs)
            {
                if (!IsInXYZlist(_XYZ, __XYZs))
                {
                    __XYZs.Add(_XYZ);
                }
            }
            return __XYZs;
        }
        /// <summary>
        /// 点列表去重
        /// </summary>
        public static List<XYZ> Pointlistdeduplication(List<XYZ> _XYZs, double torlerance_distance)
        {
            //列表去重
            List<XYZ> __XYZs = new List<XYZ>();
            foreach (XYZ _XYZ in _XYZs)
            {
                if (!IsInXYZlist(_XYZ, __XYZs, torlerance_distance))
                {
                    __XYZs.Add(_XYZ);
                }
            }
            return __XYZs;
        }
        /// <summary>
        /// 与一个列表的数据都不相等
        /// </summary>
        public static bool IsInXYZlist(XYZ _XYZ, IList<XYZ> _XYZs)
        {
            bool isIn = false;

            foreach (XYZ __XYZ in _XYZs)
            {
                double _distance = _XYZ.DistanceTo(__XYZ);
                if (_distance < PRECISION)
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        /// <summary>
        /// 与一个列表的数据都不相等
        /// </summary>
        public static bool IsInXYZlist(XYZ _XYZ, IList<XYZ> _XYZs, double torlerance_distance)
        {
            bool isIn = false;

            foreach (XYZ __XYZ in _XYZs)
            {
                double _distance = _XYZ.DistanceTo(__XYZ);
                if (_distance < torlerance_distance)//该处需要注意 Revit2020版本中 曲线的最小容差为 0.00256026455729167 Feet 0.7803686370625 MilliMeter

                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        #endregion

        #region 线圈投影

        public static List<Line> ProjectLines(List<Curve> lines)
        {
            List<Line> _lines = new List<Line>();
            foreach (Curve curve in lines)
            {
                XYZ end01 = curve.GetEndPoint(0);
                XYZ end02 = curve.GetEndPoint(1);
                Line line = Line.CreateBound(new XYZ(end01.X, end01.Y, 0), new XYZ(end02.X, end02.Y, 0));
                line.SetGraphicsStyleId(curve.GraphicsStyleId);
                _lines.Add(line);
            }
            return _lines;
        }

        public static List<List<Line>> ProjectCurveArrArray(CurveArrArray curveArrArray)
        {
            List<List<Line>> listLines = new List<List<Line>>();
            foreach (CurveArray curveArray in curveArrArray)
            {
                listLines.Add(ProjectCurveArray(curveArray));
            }
            return listLines;
        }

        public static List<Line> ProjectCurveArray(CurveArray curveArray)
        {
            List<Line> lines = new List<Line>();
            foreach (Curve curve in curveArray)
            {
                {
                    // 曲线投影为线段
                    XYZ end01 = curve.GetEndPoint(0);
                    XYZ end02 = curve.GetEndPoint(1);
                    Line line = Line.CreateBound(new XYZ(end01.X, end01.Y, 0), new XYZ(end02.X, end02.Y, 0));
                    line.SetGraphicsStyleId(curve.GraphicsStyleId);
                    lines.Add(line);
                }
            }
            return lines;
        }

        #endregion

        #region 点与线圈的关系 
        /// <summary>
        /// 不包含边界。线圈须为连续线段。
        /// </summary>
        public static bool IsInsidePolygon(XYZ _xyz, CurveArray curveArray)
        {
            bool inOron = _IsInsideOrOnEdgeOfPolygon(_xyz, curveArray);//第一步，判断点在总多边形内部部还是外部，在为true，不在为false；当前判断无法剔除，点落在多边形边界线上的情况，
            bool on = OnAnyCurves(_xyz, curveArray);//第二步，该函数判断点在不在边界线上，在为true，不在为false；
            return inOron && !on; //双重判断，不在线上，在多边形区域内
        }
        /// <summary>
        /// 包含边界。线圈须为连续线段。
        /// </summary>
        public static bool IsInsideOrOnEdgeOfPolygon(XYZ _xyz, CurveArray curveArray)
        {
            bool inOron = _IsInsideOrOnEdgeOfPolygon(_xyz, curveArray);//第一步，判断点在总多边形内部部还是外部，在为true，不在为false；当前判断无法剔除，点落在多边形边界线上的情况，
            bool on = OnAnyCurves(_xyz, curveArray);//第二步，该函数判断点在不在边界线上，在为true，不在为false；
            return inOron || on; //双重判断，不在线上，在多边形区域内
        }
        /// <summary>
        /// 包含边界。线圈须为连续线段。
        /// </summary>
        public static bool _IsInsideOrOnEdgeOfPolygon(XYZ _xyz, CurveArray curveArray)
        {
            bool _isOverlapPolygon = false;
            int intersectCount = 0;

            Line _LInebound = Line.CreateBound(_xyz, new XYZ(_xyz.X + 10000, _xyz.Y, 0));//求一个点的射线
            foreach (Curve _c in curveArray)
            {
                // 曲线做个投影
                XYZ end01 = _c.GetEndPoint(0);
                XYZ end02 = _c.GetEndPoint(1);
                Line line = Line.CreateBound(new XYZ(end01.X, end01.Y, 0), new XYZ(end02.X, end02.Y, 0));

                IntersectionResultArray results;
                SetComparisonResult result = _LInebound.Intersect(line, out results);
                if (result == SetComparisonResult.Overlap)//判断基准线是否与轴网相交
                {
                    if (results != null)
                    {
                        XYZ _LineendPoint_0 = line.GetEndPoint(0);
                        XYZ _LineendPoint_1 = line.GetEndPoint(1);

                        //下一步 判定假设 参看文章 https://blog.csdn.net/u283056051/article/details/53980925

                        if ((_LineendPoint_0.Y < _xyz.Y && _LineendPoint_1.Y >= _xyz.Y) || (_LineendPoint_1.Y < _xyz.Y && _LineendPoint_0.Y >= _xyz.Y))
                        {
                            intersectCount += results.Size;
                        }
                    }
                }
            }
            if (intersectCount % 2 != 0)//判断交点的数量是否为奇数或者偶数，奇数为内true，偶数为外false
            {
                _isOverlapPolygon = true;
            }
            //bool on = OnAnyCurves(_xyz, curveArray);//第二步，该函数判断点在不在边界线上，在为true，不在为false；

            return _isOverlapPolygon;
        }
        /// <summary>
        /// 判断一个点是不是与Polygon边界重合。
        /// </summary>
        public static bool OnAnyCurves(XYZ _xyz, CurveArray curveArray)
        {
            bool _isOnLine = false;
            foreach (Curve _c in curveArray)
            {
                if (_c.Distance(_xyz) < PRECISION)//该处需要注意 Revit2020版本中 曲线长度的最小极限小值为 0.00256026455729167 Feet 0.7803686370625 MilliMeter
                {
                    _isOnLine = true;
                    break;
                }
            }
            return _isOnLine;
        }
        #endregion

        #region 线样式 获取文档所有线样式 新建线样式
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Curve> ConverDetalLinesToPropertyLines(List<DetailLine> nowRegionDetailLines)
        {
            List<Curve> curves = new List<Curve>();
            nowRegionDetailLines.ForEach(p =>
            {
                Curve curve = p.GeometryCurve;
                curve.SetGraphicsStyleId(p.LineStyle.Id);
                curves.Add(curve);
            });
            return curves;
        }
        /// <summary>
        /// 获取所有线样式的name_list
        /// </summary>
        public static List<string> getAllLineGraphicsStylNames(Document doc)
        {
            List<string> AllLineGraphicsStylNames = new List<string>();
            List<GraphicsStyle> AllLineGraphicsStyls = getAllLineGraphicses(doc);
            foreach (GraphicsStyle _GraphicsStyle in AllLineGraphicsStyls)
            {
                AllLineGraphicsStylNames.Add(_GraphicsStyle.Name);
            }
            return AllLineGraphicsStylNames;
        }
        /// <summary>
        /// 获取所有线型类别 方法二：通过document setting
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<GraphicsStyle> getAllLineGraphicses(Document doc)
        {
            List<GraphicsStyle> lineStyles = new List<GraphicsStyle>();
            Settings documentSettings = doc.Settings;
            Categories ParentCategoyry = doc.Settings.Categories;
            Category ParentLineCategoyry = ParentCategoyry.get_Item(BuiltInCategory.OST_Lines);
            CategoryNameMap _CategoryNameMap = ParentLineCategoyry.SubCategories;
            foreach (Category lineStyle in _CategoryNameMap)
            {
                GraphicsStyle _GraphicsStyle = lineStyle.GetGraphicsStyle(GraphicsStyleType.Projection);
                lineStyles.Add(_GraphicsStyle);
            }
            return lineStyles;
        }
        /// <summary>
        /// 获取所有线性类别 方法一：通过创建实体线事务，通过事务回滚方式，获取
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ICollection<ElementId> GetAllLineStyleIdsFromTransaction(Document doc)
        {
            ICollection<ElementId> styles = new List<ElementId>();
            Transaction transaction = new Transaction(doc);
            transaction.Start("Create detail line");
            try
            {
                View view = doc.ActiveView;
                DetailCurve detailCurve = doc.Create.NewDetailCurve(view, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)));
                styles = detailCurve.GetLineStyleIds();

                transaction.RollBack();
            }
            catch (Exception ex)
            {
                transaction.RollBack();
            }
            return styles;
        }
        /// <summary>
        /// 获取所有线型类别Id 方法二：通过document setting
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ICollection<ElementId> GetAllLineStyleIdsFromSetting(Document doc)
        {
            ICollection<ElementId> styles = new List<ElementId>();
            List<GraphicsStyle> lineStyles = getAllLineGraphicses(doc);
            foreach (GraphicsStyle _GraphicsStyle in lineStyles)
            {
                styles.Add(_GraphicsStyle.Id);
            }
            return styles;
        }
        /// <summary>
        /// 对多个线圈设定线样式
        /// </summary>
        public static void SetCurveArrArrayLineStyle(CurveArrArray obstacleCurveArrArray, ElementId lineStyleId)
        {
            foreach (CurveArray curveArray in obstacleCurveArrArray)
            {
                SetCurveArrayLineStyle(curveArray, lineStyleId);
            }
        }
        /// <summary>
        /// 对一个线圈设定线样式
        /// </summary>
        public static void SetCurveArrayLineStyle(CurveArray tarRdLineLoop, ElementId lineStyleId)
        {
            foreach (Curve _c in tarRdLineLoop)
            {
                _c.SetGraphicsStyleId(lineStyleId);// 经过测试，代码内存线也可以赋予线样式Id
            }
        }
        /// <summary>
        /// 获取指定线样式的Id
        /// </summary>
        public static ElementId GetTarGraphicsStyleId(Document doc, string tarLineStyleName)
        {
            ICollection<ElementId> styles = new List<ElementId>();
            List<GraphicsStyle> lineStyles = getAllLineGraphicses(doc);
            foreach (GraphicsStyle _GraphicsStyle in lineStyles)
            {
                if (_GraphicsStyle.Name == tarLineStyleName)
                {
                    return _GraphicsStyle.Id;
                }
            }
            return new ElementId(-1);
        }
        /// <summary>
        /// 创建新的线样式,线宽为整数1-16
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Category CreatLineStyle(Document doc, string LineStyleName, int LineWeight, Color newColor)
        {
            Category newCategory = null;
            Categories categories = doc.Settings.Categories;
            Category lineCategory = categories.get_Item(BuiltInCategory.OST_Lines);
            using (Transaction _CreatNewLineCategory = new Transaction(doc))//创建新的线样式
            {
                _CreatNewLineCategory.Start("_CreatNewLineCategory");
                newCategory = doc.Settings.Categories.NewSubcategory(lineCategory, LineStyleName);//
                newCategory.LineColor = newColor;
                newCategory.SetLineWeight(LineWeight, GraphicsStyleType.Projection);
                _CreatNewLineCategory.Commit();
            }
            return newCategory;
        }
        /// <summary>
        /// 创建线型图案 CreatLinePattern
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="patternName"></param>
        /// <returns></returns>
        public static LinePatternElement CreatLinePatternElement(Document doc, string patternName)
        {
            List<LinePatternSegment> lstSegments = new List<LinePatternSegment>();
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.0));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.03));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));

            LinePattern _linePattern = new LinePattern(patternName);
            _linePattern.SetSegments(lstSegments);

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Create a linepattern element");
                LinePatternElement linePatternElement = LinePatternElement.Create(doc, _linePattern);
                trans.Commit();
                return linePatternElement;
            }
        }

        #endregion

        #region 监测消息 测试代码 弹窗处理
        /// <summary>
        /// Revit消息窗口弹出
        /// </summary>
        /// <param name="_str"></param>
        public static void TaskDialogShowMessage(string _str)
        {
            string _title = "Error";
            TaskDialog.Show(_title, _str);
        }
        #endregion

        #region 解决执行事务报错弹窗问题
        /// <summary>
        /// 尝试解决——弹窗提示的异常和错误
        /// </summary>
        /// <param name="copyDwellingsEleIds"></param>
        public static void DeleteErrOrWaringTaskDialog(Transaction copyDwellingsEleIds)
        {
            FailureHandlingOptions fho = copyDwellingsEleIds.GetFailureHandlingOptions();
            fho.SetFailuresPreprocessor(new FiluresPrecessor());
            copyDwellingsEleIds.SetFailureHandlingOptions(fho);
        }
        /// <summary>
        /// 可用于执行预处理步骤的接口，以过滤出预期的事务失败或将某些失败标记为不可持续的。
        /// </summary>
        public class FiluresPrecessor : IFailuresPreprocessor
        {
            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                IList<FailureMessageAccessor> listFma = failuresAccessor.GetFailureMessages();
                if (listFma.Count <= 0)//没有任何 警告 or 弹窗 弹窗提示
                {
                    return FailureProcessingResult.Continue;
                }
                foreach (FailureMessageAccessor fma in listFma)
                {
                    if (fma.GetSeverity() == FailureSeverity.Error)//判断弹窗消息，是否，为 错误 提示
                    {
                        if (fma.HasResolutions())
                        {
                            failuresAccessor.ResolveFailure(fma);
                        }
                    }
                    if (fma.GetSeverity() == FailureSeverity.Warning)//判断弹窗消息，是否，为 警告 提示
                    {
                        failuresAccessor.DeleteWarning(fma);
                    }
                }
                return FailureProcessingResult.ProceedWithCommit;
            }
        }
        #endregion

        #region 架构相关 entity schema externalStroge 

        public void SetEntity(Document doc)
        {

            DataStorage dataStorage = null;

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("start");
                dataStorage = DataStorage.Create(doc);
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("start");

                // 创建数据模式 相当于class
                SchemaBuilder schemaBulider = new SchemaBuilder(Guid.NewGuid());

                schemaBulider.SetReadAccessLevel(AccessLevel.Public);
                schemaBulider.SetWriteAccessLevel(AccessLevel.Public);
                schemaBulider.SetSchemaName("myFistSchema");
                schemaBulider.SetDocumentation("Data store for socket related info in a wall");

                // 相当于创建 class 里面的 字段
                FieldBuilder fieldBuilder01 = schemaBulider.AddSimpleField("SocketLocation", typeof(XYZ));
                fieldBuilder01.SetUnitType(UnitType.UT_Length);
                FieldBuilder fieldBuilder02 = schemaBulider.AddSimpleField("SocketNumber", typeof(string));

                Schema _schema = schemaBulider.Finish();

                // 创建数据实体
                Entity entity = new Entity(_schema);

                // 为实例字段赋值
                entity.Set("SocketNumber", dataStorage.Name);

                dataStorage.SetEntity(entity);

                trans.Commit();
            }

            List<Guid> listSchemaGuid = dataStorage.GetEntitySchemaGuids().ToList();
            Schema schema = Schema.Lookup(listSchemaGuid[0]);
            Entity selEntity = dataStorage.GetEntity(schema);

            string testStr = selEntity.Get<string>("SocketNumber");
            XYZ oriPoint = selEntity.Get<XYZ>("SocketLocation", DisplayUnitType.DUT_MILLIMETERS);

            _Methods.TaskDialogShowMessage(testStr);
            _Methods.TaskDialogShowMessage(oriPoint.ToString());

        }



        #endregion
    }
}
