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
using Autodesk.Revit.DB.ExtensibleStorage;

namespace LayoutParkingEffcient
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        private Guid _schemaGuid = new Guid("15E4F70C-5E48-4169-9C66-7547F5885A6A");
        private string fieldBuilderName = "canParkingRegionXYZes";

        //private ProgressWindow m_progressWindow = new ProgressWindow();
        //private ProgressTracker m_progressTracker { get { return this.m_progressWindow.Tracker; } }

        public string GetName()
        {
            return "RequestHandler";
        }
        public void Execute(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion
            TransactionGroup transGroupParkingPlace = new TransactionGroup(doc);//开启事务组

            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.SetControlRegionBoundary:
                        {
                            transGroupParkingPlace.Start("选择地库外墙线圈");
                            SetControlRegionBoundary(uiapp);
                            break;
                        }
                    case RequestId.CheckLineStyle:
                        {
                            transGroupParkingPlace.Start("创建车位强排所有必备线型");
                            CheckLineStyles(uiapp);
                            break;
                        }
                    case RequestId.CheckpolygonClosed:
                        {
                            transGroupParkingPlace.Start("检查组内线圈是否闭合");
                            CheckpolygonClosed(uiapp);
                            break;
                        }
                    case RequestId.CheckInGroupLineStyleIsSame:
                        {
                            transGroupParkingPlace.Start("检查组内线样式是否统一");
                            CheckInGroupLineStyleIsSame(uiapp);
                            break;
                        }
                    case RequestId.CheckTwoCurveCoincidence:
                        {
                            transGroupParkingPlace.Start("检测两根曲线端点是否重合");
                            CheckTwoCurveCoincidence(uiapp);
                            break;
                        }
                    case RequestId.DocumentChangedEventRegister:
                        {
                            transGroupParkingPlace.Start("启动监测");
                            DocumentChangedEventRegister(uiapp);
                            break;
                        }
                    case RequestId.DocumentChangedEventUnRegister:
                        {
                            transGroupParkingPlace.Start("车位自动排布");
                            DocumentChangedEventUnRegister(uiapp);
                            break;
                        }
                    case RequestId.ChangeDirectionByRectange:
                        {
                            transGroupParkingPlace.Start("局部调整车位排布方向");
                            ChangeDirectionByRectange(uiapp);
                            break;
                        }
                    case RequestId.ChangeDirectionByPoint:
                        {
                            transGroupParkingPlace.Start("局部调整车位排布方向");
                            ChangeDirectionByPoint(uiapp);
                            break;
                        }
                    case RequestId.GlobalRefresh:
                        {
                            transGroupParkingPlace.Start("车位布局 全局刷新");
                            GlobalRefresh(uiapp);//该处暂定为
                            break;
                        }
                    case RequestId.IntelligentRefresh:
                        {
                            transGroupParkingPlace.Start("车位布局 智能刷新");
                            IntelligentRefresh(uiapp);
                            break;
                        }
                    case RequestId.RefreshDataStatistics:
                        {
                            transGroupParkingPlace.Start("刷新车位统计数据");
                            RefreshDataSatistis(uiapp);
                            break;
                        }
                    case RequestId.HidenDirectShape:
                        {
                            transGroupParkingPlace.Start("隐藏 / 显示 车道辅助线");
                            HidenDirectShape(uiapp);
                            break;
                        }
                    case RequestId.SelunFixedParkingFs:
                        {
                            transGroupParkingPlace.Start("框选停车位族实例");
                            SelunFixedParkingFs(uiapp);
                            break;
                        }
                    case RequestId.SelFixedParkingFs:
                        {
                            transGroupParkingPlace.Start("框选停车位族实例");
                            SelFixedParkingFs(uiapp);
                            break;
                        }
                    case RequestId.SelColumsFs:
                        {
                            transGroupParkingPlace.Start("框选柱子族实例");
                            SelColumsFs(uiapp);
                            break;
                        }
                    case RequestId.CutAlgorithm:
                        {
                            transGroupParkingPlace.Start("切分算法");
                            cutAlgorithm(uiapp);
                            break;
                        }
                    case RequestId.ChangeDirectionByBoundary:
                        {
                            transGroupParkingPlace.Start("指定区域边界");
                            ChangeDirectionByBoundary(uiapp);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                transGroupParkingPlace.Assimilate();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("error", ex.Message);
                transGroupParkingPlace.RollBack();
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //外部事件方法建立————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————

        /// <summary>
        /// 框选未锁定停车位族实例
        /// </summary>
        /// <param name="uiapp"></param>
        public void SelunFixedParkingFs(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> selEles = sel.PickElementsByRectangle(new SelPickFilter_FamilyInstance(), "请框选停车位族实例")
                .Where(p => p.Name == "停车位_详图线")
                .ToList();

            List<Element> unfixedParkingFs = selEles.Where(p => p.get_Parameter(CMD.parkingFixedGid).AsValueString() == "否").ToList();

            sel.SetElementIds(_Methods.ElesToEleIds(unfixedParkingFs));
        }
        /// <summary>
        /// 框选锁定停车位族实例
        /// </summary>
        /// <param name="uiapp"></param>
        public void SelFixedParkingFs(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> selEles = sel.PickElementsByRectangle(new SelPickFilter_FamilyInstance(), "请框选停车位族实例")
                .Where(p => p.Name == "停车位_详图线")
                .ToList();

            List<Element> fixedParkingFs = selEles.Where(p => p.get_Parameter(CMD.parkingFixedGid).AsValueString() == "是").ToList();

            sel.SetElementIds(_Methods.ElesToEleIds(fixedParkingFs));
        }
        /// <summary>
        /// 框选柱子族实例
        /// </summary>
        /// <param name="uiapp"></param>
        public void SelColumsFs(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> selEles = sel.PickElementsByRectangle(new SelPickFilter_FamilyInstance(), "请框选停车位族实例")
                .Where(p => p.Name == "柱子_详图线")
                .ToList();

            sel.SetElementIds(_Methods.ElesToEleIds(selEles));
        }
        /// <summary>
        /// 一键 隐藏 显示 directShape
        /// </summary>
        public void HidenDirectShape(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            ICollection<ElementId> _directShapeIds = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(DirectShape)).WhereElementIsNotElementType().ToElementIds();
            //CMD.TestList.Add(_directShapeIds.Count.ToString());
            using (Transaction hiddenDirectShape = new Transaction(doc, "一键隐藏directShape"))
            {
                hiddenDirectShape.Start();
                if (CMD.hidenDirectShape)
                {
                    acvtiView.HideElements(_directShapeIds);
                }
                else if (!CMD.hidenDirectShape)
                {
                    acvtiView.UnhideElements(_directShapeIds);
                }
                hiddenDirectShape.Commit();
            }

            return;
        }
        /// <summary>
        /// 检查两根线段的端点是否重合
        /// </summary>
        public void CheckTwoCurveCoincidence(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            IList<Element> eles = sel.PickElementsByRectangle();
            CurveArray curveArray = new CurveArray();
            foreach (Element ele in eles)
            {
                DetailCurve _detailCurve = ele as DetailCurve;
                Curve curve = _detailCurve.GeometryCurve;
                curveArray.Append(curve);
            }

            XYZ endpoing_0 = curveArray.get_Item(0).GetEndPoint(0);
            XYZ endpoing_1 = curveArray.get_Item(0).GetEndPoint(1);

            XYZ _endpoing_0 = curveArray.get_Item(1).GetEndPoint(0);
            XYZ _endpoing_1 = curveArray.get_Item(1).GetEndPoint(1);

            if (endpoing_0.DistanceTo(_endpoing_0) < _Methods.PRECISION
                               || endpoing_1.DistanceTo(_endpoing_0) < _Methods.PRECISION
                               || endpoing_0.DistanceTo(_endpoing_1) < _Methods.PRECISION
                               || endpoing_1.DistanceTo(_endpoing_1) < _Methods.PRECISION)//判断逻辑为，一个线段的两个点都会被重合一次，因此，重合总次数，为线段端点的2倍
            {
                _Methods.TaskDialogShowMessage("选择的两根详图线，首尾相交");
            }
        }
        /// <summary>
        /// 检查组内线段是否闭合
        /// </summary>
        public void CheckpolygonClosed(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> sel_groups = sel.PickElementsByRectangle(new SelPickFilter_Groups(), "Select Region Boundings ").ToList();
            foreach (Element _ele in sel_groups)
            {
                if (_ele is Group)
                {
                    Group _group = _ele as Group;
                    CurveArray _curves = _Methods.GetCurvesFromCurveGroup(_group);
                    CurveLoop curveLoop = new CurveLoop();
                    bool isLoop = _Methods.IsCurveLoop(_curves, out curveLoop);//该处比较严谨，必须满足线圈，才可以继续运行
                    if (isLoop)
                    {
                        _Methods.TaskDialogShowMessage("该线圈状态为闭合");
                    }
                    else
                    {
                        _Methods.TaskDialogShowMessage("该线圈状态为未闭合");
                    }
                }
            }
        }
        /// <summary>
        /// 检查组内线段样式是否统一
        /// </summary>
        public void CheckInGroupLineStyleIsSame(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> sel_groups = sel.PickElementsByRectangle(new SelPickFilter_Groups(), "Select Region Boundings ").ToList();
            foreach (Element _ele in sel_groups)
            {
                if (_ele is Group)
                {
                    Group _group = _ele as Group;
                    string _GraphicsStyle_frist_name;
                    bool iSame = _Methods.IsSameAllCurveLineStyleFromGroup(doc, _group, out _GraphicsStyle_frist_name);
                    if (iSame)
                    {
                        TaskDialog.Show("正确", "组内详图线样式统一 \n线样式为：" + _GraphicsStyle_frist_name);
                        continue;
                    }
                    else
                    {
                        throw new NotImplementedException("组内详图线样式不统一，请检查");
                    }
                }
            }
        }
        /// <summary>
        /// 定位一个停车区域 设定车位垂直车道方向
        /// </summary>
        public void ChangeDirectionByBoundary(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            //ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region UI交互 选择一个区域，选择一个边，filledregion的curveLoop顺序，在后台中，默认为逆时针。
            FilledRegion selFilledRegion = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as FilledRegion;
            CurveArray selCurveLoop = _Methods.CurveLoopToCurveArray(selFilledRegion.GetBoundaries().First());//选择的停车区域

            Reference reference = sel.PickObject(ObjectType.Edge, "请选择当前选择停车区域的任一边界，用以确认车位排布方向");
            Element filledRegionBoundary = doc.GetElement(reference);
            Line line = (filledRegionBoundary.GetGeometryObjectFromReference(reference) as Edge).AsCurve() as Line;

            double rotateAngle = line.Direction.AngleTo(new XYZ(1, 0, 0));

            XYZ selPoint = sel.PickPoint();// UI交互 指定起点
            #endregion

            #region 判断选择区域定位线的方向位于第几象限

            XYZ leftDownPoint = _Methods.GetLeftDownXYZfromLines(selCurveLoop);
            XYZ axis = new XYZ(0, 0, 1);

            Transform beforeTransform = Transform.Identity;
            Transform afterTransform = Transform.Identity;
            XYZ basicBoundary = line.Direction;
            double _x = basicBoundary.X;
            double _y = basicBoundary.Y;

            if (_x >= 0 && _y >= 0)// 第一象限quadrant 如果x ou y = 0，说明线为水平线/垂直线
            {
                rotateAngle = -rotateAngle;
            }
            else if (_x >= 0 && _y < 0)// 第四象限quadrant
            {
                //
            }
            else if (_x < 0 && _y >= 0)// 第二象限quadrant
            {
                rotateAngle = Math.PI - rotateAngle;
            }
            else if (_x < 0 && _y <= 0)// 第三象限quadrant
            {
                rotateAngle = -(Math.PI - rotateAngle);
            }
            beforeTransform = Transform.CreateRotationAtPoint(axis, rotateAngle, leftDownPoint);//当基准线方向在第一象限时，transfom角度为负值，顺时针旋转
            afterTransform = Transform.CreateRotationAtPoint(axis, -rotateAngle, leftDownPoint);//当基准线方向在第一象限时，transfom角度为负值，顺时针旋转

            #endregion

            SubChangeDirectionByPointAndTransform(doc, selCurveLoop, selPoint, beforeTransform, afterTransform, Math.PI - rotateAngle);

        }
        /// <summary>
        /// 定位一个停车区域 设定起点位置 子方法
        /// </summary>
        public void SubChangeDirectionByPointAndTransform(Document doc, CurveArray selCurveLoop, XYZ selPoint, Transform beforeTransform, Transform afterTransform, double rotateAngle)
        {
            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region 对选择区域做准备处理，删除 非固定停车位 柱子 族实例

            RegionAlgorithm regionAlgorithm = new RegionAlgorithm(doc, selCurveLoop);//该实例包含 地库_地库外墙线 内的所有元素Id
            CurveArrArray beforeCanPlacedRegions = regionAlgorithm.parkingCurveArrArray;
            List<Element> filledRegions = _Methods.EleIdsToEles(doc, regionAlgorithm.filledRegions);
            List<Element> allFixedParkingFS = _Methods.EleIdsToEles(doc, regionAlgorithm.fixedParkingFS);
            List<Element> othersObstalFs = _Methods.EleIdsToEles(doc, regionAlgorithm.othersObstals);
            List<Curve> allCurves = regionAlgorithm.allCurves;

            using (var deleteTrans = new Transaction(doc, "删除所有未锁定停车位、柱子以及diretShape。"))//启动事务 删除当前地库外墙线区域内的停车位和柱子族实例（未锁定）
            {
                deleteTrans.Start();
                doc.Delete(regionAlgorithm.unfixedParkingFS);
                //doc.Delete(regionAlgorithm.fixedParkingAndColumnFS);
                //doc.Delete(regionAlgorithm.directShapes);
                deleteTrans.Commit();
            }

            #endregion

            #region 获取与停车计算区域有关的障碍物区域

            CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
            filledRegions.ForEach(_p =>
            {
                FilledRegion p = _p as FilledRegion;
                foreach (CurveArray curveArray in _Methods.CurveLoopsToCurveArrArray(p.GetBoundaries().ToList()))
                    nowObstacleCurveArrArray.Append(curveArray);
            });

            #endregion

            #region 此处需要判断填充区域 需不需要考虑架空处理 使用transform进行转换
            CurveArrArray otherObstalLoops = GetOthersObstalRegions(doc, selCurveLoop, othersObstalFs);
            foreach (CurveArray curveArray1 in otherObstalLoops)
            {
                nowObstacleCurveArrArray.Append(curveArray1);
                #region 将新增属性线添加到allCurves中
                foreach (Curve curve in curveArray1)
                {
                    curve.SetGraphicsStyleId(obstacleLineStyleId);
                    allCurves.Add(curve);
                }
                #endregion
            }
            #endregion

            #region 对固定车位进行处理 这里提取 已定位的车位区域，需要考虑 柱子的宽度 车道的宽度 

            Paths_xyz unionParkingSplace = GetParkingSpaceUnion(doc, allFixedParkingFS);

            CurveArrArray allFixedParkingCurveArray = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(unionParkingSplace));
            foreach (CurveArray curveArray in allFixedParkingCurveArray)
                nowObstacleCurveArrArray.Append(curveArray);

            #endregion

            #region Transform处理

            if (beforeTransform != null)
            {
                if (!beforeTransform.IsIdentity)
                {
                    selPoint = beforeTransform.OfPoint(selPoint);
                    selCurveLoop = _Methods.TransformCurveArray(selCurveLoop, beforeTransform);
                    nowObstacleCurveArrArray = _Methods.TransformCurveArray(nowObstacleCurveArrArray, beforeTransform);
                    allCurves = _Methods.TransformCurves(allCurves, beforeTransform);
                }
            }

            #endregion

            #region 得到目标区域内的相关属性线条 开启停车sweep算法

            var transactionNow = new TransactionNow(doc);
            ParkingAlgorithm parkingAlgorithm = new ParkingAlgorithm();

            List<Point> Max_tar_columnplaceXYZs = new List<Point>();
            List<Point> Max_tar_placeXYZs = new List<Point>();

            if (CMD.isOptimalAlgorithm == true)
            {
                #region 此处需要给出指点起点后的最优解 ？？？？？？
                Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweepByPointInTarRegion(doc, selPoint, selCurveLoop, nowObstacleCurveArrArray, allCurves, out Max_tar_columnplaceXYZs, CMD.layoutMethod);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                #endregion
            }
            else if (CMD.isOptimalAlgorithm == false)
            {
                Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweepByPointInTarRegion(doc, selPoint, selCurveLoop, nowObstacleCurveArrArray, allCurves, out Max_tar_columnplaceXYZs, CMD.layoutMethod);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            #endregion

            #region Transform处理

            if (afterTransform != null)
            {
                if (!afterTransform.IsIdentity)
                {
                    Max_tar_columnplaceXYZs = _Methods.TransformPath_xyz(Max_tar_columnplaceXYZs, afterTransform);
                    Max_tar_placeXYZs = _Methods.TransformPath_xyz(Max_tar_placeXYZs, afterTransform);
                }
            }

            #endregion

            #region 开启放置车位事务
            transactionNow.LayoutParking(Max_tar_placeXYZs, Max_tar_columnplaceXYZs, rotateAngle, CMD.layoutMethod);//开启放置事务 
            #endregion
        }
        /// <summary>
        /// 切分算法，涉及到基于基准线旋转的问题。遗留问题，中间缝隙，会变成两个柱子（600mm * 2）的距离。
        /// </summary>
        public void cutAlgorithm(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            //ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region UI交互 选择一个区域 选择一个起点
            FilledRegion selFilledRegion = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as FilledRegion;
            CurveArray selCurveLoop = _Methods.CurveLoopToCurveArray(selFilledRegion.GetBoundaries().First());//选择的停车区域

            Path selCurveLoopPath = clipper_methods.Path_xyzToPath(_Methods.GetUniqueXYZFromCurves(selCurveLoop));
            #endregion

            #region 划一根基准线，该基准线为柱子所在位置，通过基准线做一个相对较长的矩形，用于裁剪当前区域

            XYZ selPointStart = sel.PickPoint("指定车位排布起点");// UI交互 指定起点
            XYZ selPointDirection = sel.PickPoint("指定方向");// UI交互 指定起点
            XYZ directionVector = selPointStart - selPointDirection;
            int multiple = 10;
            XYZ point01 = selPointStart + directionVector * multiple;
            XYZ point02 = selPointDirection - directionVector * multiple;
            Line basicLine = Line.CreateBound(point01, point02);
            Path basicLineRegion = clipper_methods._GetOffsetPonts_clipper_line((basicLine as Curve), CMD.columnWidth / 2 + CMD.columnBurfferDistance);
            Paths canPlacedRegion = clipper_methods.RegionCropctDifference(new Paths() { selCurveLoopPath }, new Paths() { basicLineRegion });//得到可停车区域

            Paths_xyz parkingRegionsPoints = clipper_methods.PathsToPaths_xyz(canPlacedRegion);
            #endregion

            #region 对clipper切分出的区域，做处理，如何区分切分出的区域与基准线的关系，谁在上，谁在下，谁在左，谁在右

            if (parkingRegionsPoints.Count < 3)
            {
                CurveArrArray firstCanPlacedRegions = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(parkingRegionsPoints));

                #region 对一个区域进行车位排布，基于一个点定位车位排布起点
                foreach (CurveArray _regionCurves in firstCanPlacedRegions)
                {
                    SubChangeDirectionByPoint(doc, _regionCurves, selPointStart);
                }
                #endregion
            }
            #endregion


            #region 1.从静态数据线圈Id 提取地库_地库外墙线 2.判断选择区域在不在已选择Id线圈，如果不在需要提醒设计师
            var basementWallRegion = GetRegionFromMemoryEleId(doc, CMD.baseMentWallLoopId);
            #endregion
            DataStatistics(doc, basementWallRegion);//停车区域点集需要更新
        }
        /// <summary>
        /// 定位一个停车区域 设定起点位置
        /// </summary>
        public void ChangeDirectionByPoint(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region UI交互 选择一个区域 选择一个起点
            //DirectShape selDs = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as DirectShape;
            //CurveArray selCurveLoop = GetCurveArrayFromDirectshape(selDs);//选择的停车区域
            FilledRegion selFilledRegion = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as FilledRegion;
            CurveArray selCurveLoop = _Methods.CurveLoopToCurveArray(selFilledRegion.GetBoundaries().First());//选择的停车区域

            XYZ selPoint = sel.PickPoint();// UI交互 指定起点
            #endregion

            #region 1.从静态数据线圈Id 提取地库_地库外墙线 2.判断选择区域在不在已选择Id线圈，如果不在需要提醒设计师
            var basementWallRegion = GetRegionFromMemoryEleId(doc, CMD.baseMentWallLoopId);
            //bool isInRegion = _Methods.isCurveArrayInCurveArrayByAllSingleCurves(doc, selCurveLoop, basementWallRegion);
            //if (!isInRegion)//如果不在区域内
            //{
            //    TaskDialog mainDialog = new TaskDialog("Revit2020 警告!");
            //    mainDialog.MainInstruction = "Revit2020 警告!";
            //    mainDialog.MainContent =
            //        "已点选线圈范围不在已记录地库外墙线圈范围内，请问是否继续执行？"
            //        + "如果执行,选择区域车位排布会更新；但是，已记录地库外墙线圈内的统计数据会出现异常。";
            //    mainDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
            //    mainDialog.DefaultButton = TaskDialogResult.No;

            //    TaskDialogResult tResult = mainDialog.Show();

            //    if (tResult == TaskDialogResult.Yes)
            //    {
            //        //
            //    }
            //    else if (tResult == TaskDialogResult.No)
            //        throw new NotImplementedException("请重新选择计算区域。");
            //}
            #endregion

            #region 对一个区域进行车位排布，基于一个点定位车位排布起点

            SubChangeDirectionByPoint(doc, selCurveLoop, selPoint);

            #endregion

            DataStatistics(doc, basementWallRegion);//停车区域点集需要更新
        }
        /// <summary>
        /// 定位一个停车区域 设定起点位置 子方法
        /// </summary>
        public void SubChangeDirectionByPoint(Document doc, CurveArray selCurveLoop, XYZ selPoint)
        {
            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region 对选择区域做准备处理，删除 非固定停车位 柱子 族实例

            RegionAlgorithm regionAlgorithm = new RegionAlgorithm(doc, selCurveLoop);//该实例包含 地库_地库外墙线 内的所有元素Id
            CurveArrArray beforeCanPlacedRegions = regionAlgorithm.parkingCurveArrArray;
            List<Element> filledRegions = _Methods.EleIdsToEles(doc, regionAlgorithm.filledRegions);
            List<Element> allFixedParkingFS = _Methods.EleIdsToEles(doc, regionAlgorithm.fixedParkingFS);
            List<Element> othersObstalFs = _Methods.EleIdsToEles(doc, regionAlgorithm.othersObstals);
            List<Curve> allCurves = regionAlgorithm.allCurves;

            using (var deleteTrans = new Transaction(doc, "删除所有未锁定停车位、柱子以及diretShape。"))//启动事务 删除当前地库外墙线区域内的停车位和柱子族实例（未锁定）
            {
                deleteTrans.Start();
                doc.Delete(regionAlgorithm.unfixedParkingFS);
                //doc.Delete(regionAlgorithm.fixedParkingAndColumnFS);
                //doc.Delete(regionAlgorithm.directShapes);
                deleteTrans.Commit();
            }

            #endregion

            #region 获取与停车计算区域有关的障碍物区域

            CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
            filledRegions.ForEach(_p =>
            {
                FilledRegion p = _p as FilledRegion;
                foreach (CurveArray curveArray in _Methods.CurveLoopsToCurveArrArray(p.GetBoundaries().ToList()))
                    nowObstacleCurveArrArray.Append(curveArray);
            });

            #endregion

            #region 此处需要判断填充区域 需不需要考虑架空处理 使用transform进行转换
            CurveArrArray otherObstalLoops = GetOthersObstalRegions(doc, selCurveLoop, othersObstalFs);
            foreach (CurveArray curveArray1 in otherObstalLoops)
            {
                nowObstacleCurveArrArray.Append(curveArray1);
                #region 将新增属性线添加到allCurves中
                foreach (Curve curve in curveArray1)
                {
                    curve.SetGraphicsStyleId(obstacleLineStyleId);
                    allCurves.Add(curve);
                }
                #endregion
            }
            #endregion

            #region 对固定车位进行处理 这里提取 已定位的车位区域，需要考虑 柱子的宽度 车道的宽度 

            Paths_xyz unionParkingSplace = GetParkingSpaceUnion(doc, allFixedParkingFS);

            CurveArrArray allFixedParkingCurveArray = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(unionParkingSplace));
            foreach (CurveArray curveArray in allFixedParkingCurveArray)
                nowObstacleCurveArrArray.Append(curveArray);

            #endregion

            #region 得到目标区域内的相关属性线条 开启停车sweep算法

            var transactionNow = new TransactionNow(doc);
            ParkingAlgorithm parkingAlgorithm = new ParkingAlgorithm();
            List<Point> Max_tar_columnplaceXYZs = new List<Point>();

            List<Point> Max_tar_placeXYZs = new List<Point>();

            if (CMD.isOptimalAlgorithm == true)
            {
                #region 此处需要给出指点起点后的最优解

                #endregion
                Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweepByPointInTarRegion(doc, selPoint, selCurveLoop, nowObstacleCurveArrArray, allCurves, out Max_tar_columnplaceXYZs, CMD.layoutMethod);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            else if (CMD.isOptimalAlgorithm == false)
            {
                Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweepByPointInTarRegion(doc, selPoint, selCurveLoop, nowObstacleCurveArrArray, allCurves, out Max_tar_columnplaceXYZs, CMD.layoutMethod);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }

            transactionNow.LayoutParking(Max_tar_placeXYZs, Max_tar_columnplaceXYZs, CMD.layoutMethod);//开启放置事务 
            #endregion
        }
        /// <summary>
        /// 定位一个停车区域 通过点选矩形做布尔交集 对单个停车区域的车位排布方向进行更改
        /// </summary>
        public void ChangeDirectionByRectange(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region UI交互 选择一个区域
            //DirectShape selDs = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as DirectShape;
            //CurveArray selCurveLoop = GetCurveArrayFromDirectshape(selDs);//选择的停车区域
            FilledRegion selFilledRegion = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion())) as FilledRegion;
            CurveArray selCurveLoop = _Methods.CurveLoopToCurveArray(selFilledRegion.GetBoundaries().First());//选择的停车区域

            XYZ rectanglePoint01 = sel.PickPoint();
            XYZ rectanglePoint02 = sel.PickPoint();
            CurveArray rectangle = GetRectangle(rectanglePoint01, rectanglePoint02);// UI交互 指定矩形区域
            #endregion

            #region 矩形与选择可停车区域 做交集运算

            Paths selPlacedParkingRegionPoints = new Paths() { clipper_methods.Path_xyzToPath(_Methods.GetUniqueXYZFromCurves(selCurveLoop)) };//点选可停车区域
            Paths rectanglePoints = new Paths() { clipper_methods.Path_xyzToPath(_Methods.GetUniqueXYZFromCurves(rectangle)) };

            Paths_xyz canPlacedRegion = clipper_methods.PathsToPaths_xyz(clipper_methods.RegionCropctIntersection(selPlacedParkingRegionPoints, rectanglePoints));//得到裁剪后的可停车区域
            CurveArray selIntersectRegion = _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(canPlacedRegion.First()));

            #endregion

            #region 1.从静态数据线圈Id 提取地库_地库外墙线 2.判断选择区域在不在已选择Id线圈，如果不在需要提醒设计师

            var basementWallRegion = GetRegionFromMemoryEleId(doc, CMD.baseMentWallLoopId);

            //bool isInRegion = _Methods.isCurveArrayInCurveArrayByAllSingleCurves(doc, selCurveLoop, basementWallRegion);
            //if (!isInRegion)//如果不在区域内
            //{
            //    TaskDialog mainDialog = new TaskDialog("Revit2020 警告!");
            //    mainDialog.MainInstruction = "Revit2020 警告!";
            //    mainDialog.MainContent =
            //        "已点选线圈范围不在已记录地库外墙线圈范围内，请问是否继续执行？"
            //        + "如果执行,选择区域车位排布会更新；但是，已记录地库外墙线圈内的统计数据会出现异常。";
            //    mainDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
            //    mainDialog.DefaultButton = TaskDialogResult.No;

            //    TaskDialogResult tResult = mainDialog.Show();

            //    if (tResult == TaskDialogResult.Yes)
            //    {
            //        //
            //    }
            //    else if (tResult == TaskDialogResult.No)
            //        throw new NotImplementedException("请重新选择计算区域。");
            //}

            #endregion

            #region 对选择区域做准备处理，删除 非固定停车位 柱子 族实例

            RegionAlgorithm regionAlgorithm = new RegionAlgorithm(doc, selIntersectRegion);//该实例包含 地库_地库外墙线 内的所有元素Id
            CurveArrArray beforeCanPlacedRegions = regionAlgorithm.parkingCurveArrArray;
            List<Element> filledRegions = _Methods.EleIdsToEles(doc, regionAlgorithm.filledRegions);
            List<Element> allFixedParkingFS = _Methods.EleIdsToEles(doc, regionAlgorithm.fixedParkingFS);
            List<Element> othersObstalFs = _Methods.EleIdsToEles(doc, regionAlgorithm.othersObstals);
            List<Curve> allCurves = regionAlgorithm.allCurves;

            using (var deleteTrans = new Transaction(doc, "删除所有未锁定停车位、柱子以及diretShape。"))//启动事务 删除当前地库外墙线区域内的停车位和柱子族实例（未锁定）
            {
                deleteTrans.Start();
                doc.Delete(regionAlgorithm.unfixedParkingFS);
                //doc.Delete(regionAlgorithm.fixedParkingAndColumnFS);
                //doc.Delete(regionAlgorithm.directShapes);
                deleteTrans.Commit();
            }

            #endregion

            #region 获取与停车计算区域有关的障碍物区域

            CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
            filledRegions.ForEach(_p =>
            {
                FilledRegion p = _p as FilledRegion;
                foreach (CurveArray curveArray in _Methods.CurveLoopsToCurveArrArray(p.GetBoundaries().ToList()))
                    nowObstacleCurveArrArray.Append(curveArray);
            });

            #endregion

            #region 对固定车位进行处理 这里提取 已定位的车位区域，需要考虑 柱子的宽度 车道的宽度 

            Paths_xyz unionParkingSplace = GetParkingSpaceUnion(doc, allFixedParkingFS);

            CurveArrArray allFixedParkingCurveArray = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(unionParkingSplace));
            foreach (CurveArray curveArray in allFixedParkingCurveArray)
                nowObstacleCurveArrArray.Append(curveArray);

            #endregion

            #region 此处需要判断填充区域 需不需要考虑架空处理 使用transform进行转换
            CurveArrArray otherObstalLoops = GetOthersObstalRegions(doc, selCurveLoop, othersObstalFs);
            foreach (CurveArray curveArray1 in otherObstalLoops)
            {
                nowObstacleCurveArrArray.Append(curveArray1);
                #region 将新增属性线添加到allCurves中
                foreach (Curve curve in curveArray1)
                {
                    curve.SetGraphicsStyleId(obstacleLineStyleId);
                    allCurves.Add(curve);
                }
                #endregion
            }
            #endregion

            #region 得到目标区域内的相关属性线条 开启停车sweep算法

            var transactionNow = new TransactionNow(doc);
            ParkingAlgorithm parkingAlgorithm = new ParkingAlgorithm();
            List<Point> Max_tar_columnplaceXYZs = new List<Point>();
            List<Point> Max_tar_placeXYZs = new List<Point>();
            if (CMD.isOptimalAlgorithm == true)
            {
                Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweep(doc, selIntersectRegion, nowObstacleCurveArrArray, allCurves, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }
            else if (CMD.isOptimalAlgorithm == false)
            {
                Max_tar_placeXYZs = parkingAlgorithm.tarSweep(doc, selIntersectRegion, nowObstacleCurveArrArray, allCurves, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
            }

            transactionNow.LayoutParking(Max_tar_placeXYZs, Max_tar_columnplaceXYZs, CMD.layoutMethod);//开启放置事务 

            #endregion
            DataStatistics(doc, basementWallRegion);
        }
        /// <summary>
        /// 基于最新场地条件 刷新停车布局
        /// </summary>
        public void IntelligentRefresh(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion
            #region 求出目标填充样式
            ElementId _filledRegionTypeId = GetTarFilledRegionTypeId(doc, "地库_单个停车区域");
            #endregion
            #region 从静态数据提取线圈Id读取地库外墙线圈的entity 并对组名、线圈是否闭合、线圈线样式是否统一 进行审查

            Paths_xyz firstParkingPlaces = ReadEntityFromBaseWallLoop(doc, _schemaGuid, CMD.baseMentWallLoopId);

            var basementWallRegion = GetRegionFromMemoryEleId(doc, CMD.baseMentWallLoopId);//地库外墙线圈
            #endregion

            #region 计算最新可停车区域，与entity数据进行对比，得到变化的区域

            RegionAlgorithm regionAlgorithm = new RegionAlgorithm(doc, basementWallRegion);//该实例包含 地库_地库外墙线 内的所有元素Id
            CurveArrArray nowCanPlacedRegions = regionAlgorithm.parkingCurveArrArray;
            List<Element> filledRegions = _Methods.EleIdsToEles(doc, regionAlgorithm.filledRegions);
            List<Element> allFixedParkingFS = _Methods.EleIdsToEles(doc, regionAlgorithm.fixedParkingFS);
            List<Element> allUnFixedParkingFS = _Methods.EleIdsToEles(doc, regionAlgorithm.unfixedParkingFS);
            List<Element> allDirectShapes = _Methods.EleIdsToEles(doc, regionAlgorithm.directShapes);
            List<Element> othersObstalFs = _Methods.EleIdsToEles(doc, regionAlgorithm.othersObstals);
            List<Curve> allCurves = regionAlgorithm.allCurves;

            Paths_xyz nowChangedRegionPoints = clipper_methods.GetChangedRegion(regionAlgorithm.parkingRegionsPoints, firstParkingPlaces);//变化的新增区域
            CurveArrArray nowChangedCanPlacedRegion = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(nowChangedRegionPoints));

            if (nowChangedRegionPoints.Count < 1)
                throw new NotImplementedException("未检测到可停车空间变化区域，如果需要重新计算，请点击全局计算。");

            Paths_xyz firstChangedRegionPoints = clipper_methods.GetChangedRegion(firstParkingPlaces, regionAlgorithm.parkingRegionsPoints);//被修改的区域
            CurveArrArray firstChangedCanPlacedRegion = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(firstChangedRegionPoints));
            #endregion

            #region 删除变化区域的未锁定车位，并重新绘制停车区域边界线 directshape。此处需要注意，删除物体，需要使用生成物体时范围框，勿反。

            using (var deleteTrans = new Transaction(doc, "删除所有未锁定停车位、柱子以及diretShape。"))//启动事务 删除当前地库外墙线区域内的停车位和柱子族实例（未锁定）
            {
                deleteTrans.Start();
                List<ElementId> needDelDsIds = new List<ElementId>();
                foreach (CurveArray _regionCurves in firstChangedCanPlacedRegion)//针对单个区域进行停车位计算处理
                {
                    List<Element> needDelUnFixedParkingFS = _Methods.GetTarlElesByRegion(doc, _regionCurves, allUnFixedParkingFS);
                    doc.Delete(_Methods.ElesToEleIds(needDelUnFixedParkingFS));

                    #region directshape比较特殊，需要判断线圈与线圈的关系
                    Path_xyz _regionCurvePoints = _Methods.GetUniqueXYZFromCurves(_regionCurves);
                    foreach (Element tempEle in allDirectShapes)
                    {
                        DirectShape tempDirectShape = tempEle as DirectShape;
                        CurveArray templCurveLoop = GetCurveArrayFromDirectshape(tempDirectShape);//选择的停车区域
                        Path_xyz templCurveLooppoints = _Methods.GetUniqueXYZFromCurves(templCurveLoop);
                        if (clipper_methods.PathXyzIsSame(_regionCurvePoints, templCurveLooppoints)) needDelDsIds.Add(tempEle.Id);
                    }
                    #endregion
                }
                doc.Delete(needDelDsIds);

                doc.Delete(regionAlgorithm.filledRegions_singleParkignPlace);

                deleteTrans.Commit();
            }

            #endregion

            #region 将停车区域打印出来
            //double moveDownDistance = doc.ActiveView.GenLevel.ProjectElevation;
            foreach (CurveArray _regionCurves in nowCanPlacedRegions)
            {
                //_Methods.ShowTempGeometry(doc, _regionCurves, new XYZ(0, 0, moveDownDistance));
                #region 绘制详图填充区域
                CreatFilledRegion(doc, _filledRegionTypeId, _regionCurves);
                #endregion
            }
            #endregion

            #region 停车位全局刷新requestHandler，与智能刷新RequestHandlerDocumentChangeEvent，在停车位算法上，使用一个方法
            using (ParkingAlgorithm parkingAlgorithm = new ParkingAlgorithm())
            {
                var transactionNow = new TransactionNow(doc);

                #region 计时器
                //this.m_progressTracker.StartTracking_TS("Getting Properties / Methods..", 1, nowChangedCanPlacedRegion.Size);
                #endregion

                foreach (CurveArray _regionCurves in nowChangedCanPlacedRegion)//针对单个区域进行停车位计算处理
                {

                    #region 1 设定停止计算Flag 2 将当前停车区域打印在当前视图
                    if (CMD.stopAlgorithm == false) break;
                    #endregion

                    #region 获取与停车计算区域有关的障碍物区域

                    CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
                    List<FilledRegion> needFilledRegions = _Methods.GetTarlElesByRegion(doc, _regionCurves, filledRegions).Cast<FilledRegion>().ToList();
                    needFilledRegions.ForEach(p =>
                    {
                        foreach (CurveArray curveArray in _Methods.CurveLoopsToCurveArrArray(p.GetBoundaries().ToList()))
                            nowObstacleCurveArrArray.Append(curveArray);
                    });

                    #endregion

                    #region 此处需要判断填充区域 需不需要考虑架空处理 使用transform进行转换
                    CurveArrArray otherObstalLoops = GetOthersObstalRegions(doc, _regionCurves, othersObstalFs);
                    foreach (CurveArray curveArray1 in otherObstalLoops)
                    {
                        nowObstacleCurveArrArray.Append(curveArray1);
                        #region 将新增属性线添加到allCurves中
                        foreach (Curve curve in curveArray1)
                        {
                            curve.SetGraphicsStyleId(obstacleLineStyleId);
                            allCurves.Add(curve);
                        }
                        #endregion
                    }
                    #endregion

                    #region 对固定车位进行处理 这里提取 已定位的车位区域，需要考虑 柱子的宽度 车道的宽度 

                    List<Element> needlFixedParkingFS = _Methods.GetTarlElesByRegion(doc, _regionCurves, allFixedParkingFS);
                    Paths_xyz unionParkingSplace = GetParkingSpaceUnion(doc, needlFixedParkingFS);

                    CurveArrArray allFixedParkingCurveArray = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(unionParkingSplace));
                    foreach (CurveArray curveArray in allFixedParkingCurveArray)
                        nowObstacleCurveArrArray.Append(curveArray);

                    #endregion

                    #region 用停车位空间进行裁剪 未采用
                    //Paths subjsPlace = clipper_methods.Paths_xyzToPaths(new Paths_xyz() { _Methods.GetUniqueXYZFromCurves(_regionCurves) });
                    //Paths clipsFiexedParking = clipper_methods.Paths_xyzToPaths(_Methods.GetUniqueXYZFromCurves(allFixedParkingCurveArray));
                    //Paths_xyz canPlacedRegion = clipper_methods.PathsToPaths_xyz(RegionAlgorithm.RegionCropctDifference(subjsPlace, clipsFiexedParking));

                    //if (canPlacedRegion.Count < 1)
                    //    continue;
                    //_regionCurves = _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(canPlacedRegion.First()));
                    #endregion

                    #region 得到目标区域内的相关属性线条 开启停车sweep算法
                    List<Curve> nowRegionPropertyLines = _Methods.GetCurvesInTarRegion(doc, _regionCurves, allCurves);

                    List<Point> Max_tar_columnplaceXYZs = new List<Point>(); //求出停车位的放置点
                    List<Point> Max_tar_placeXYZs = new List<Point>();
                    if (CMD.isOptimalAlgorithm == true)
                    {
                        Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweep(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                    }
                    else if (CMD.isOptimalAlgorithm == false)
                    {
                        Max_tar_placeXYZs = parkingAlgorithm.tarSweep(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                    }
                    transactionNow.LayoutParking(Max_tar_placeXYZs, Max_tar_columnplaceXYZs, CMD.layoutMethod);//开启放置事务  

                    #endregion

                    #region 计时器
                    //if (this.m_progressTracker.StopTask) break;
                    //this.m_progressTracker.Current++;
                    #endregion
                }
                //this.m_progressTracker.StopTracking_TS();

                #region GC 垃圾回收 Garbage Collector
                GC.Collect();
                #endregion
            }
            #endregion

            #region 将可停车region 点集 写入entity 记录数据 设置entity 将上述生成的各个停车区域点集 存入地库外墙线线圈group的entity

            SetEntityToBaseWallLoop(doc, _schemaGuid, CMD.baseMentWallLoopId, regionAlgorithm.parkingRegionsPoints); //Entity赋值

            #endregion
            DataStatistics(doc, basementWallRegion);
        }
        /// <summary>
        /// 选择地库设计区域 框选红线内外所有物体
        /// </summary>
        public void GlobalRefresh(UIApplication uiapp)
        {
            #region 计算控制
            //UserControl1 userControl1 = new UserControl1();
            //userControl1.Show();
            #endregion

            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            //ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region 求出目标填充样式
            ElementId _filledRegionTypeId = GetTarFilledRegionTypeId(doc, "地库_单个停车区域");
            #endregion

            #region 从静态数据提取线圈Id  对组名、线圈是否闭合、线圈线样式是否统一 进行审查
            var basementWallRegion = GetRegionFromMemoryEleId(doc, CMD.baseMentWallLoopId);
            #endregion

            #region 全局刷新，不需要读取entity，只需要写入entity

            RegionAlgorithm regionAlgorithm = new RegionAlgorithm(doc, basementWallRegion);//该实例包含 地库_地库外墙线 内的所有元素Id
            CurveArrArray firstCanPlacedRegions = regionAlgorithm.parkingCurveArrArray;
            List<Element> filledRegions = _Methods.EleIdsToEles(doc, regionAlgorithm.filledRegions);
            List<Element> allFixedParkingFS = _Methods.EleIdsToEles(doc, regionAlgorithm.fixedParkingFS);
            List<Element> othersObstalFs = _Methods.EleIdsToEles(doc, regionAlgorithm.othersObstals);
            List<Curve> allCurves = regionAlgorithm.allCurves;

            using (var deleteTrans = new Transaction(doc, "删除所有未锁定停车位、柱子以及diretShape。"))//启动事务 删除当前地库外墙线区域内的停车位和柱子族实例（未锁定）
            {
                deleteTrans.Start();
                doc.Delete(regionAlgorithm.unfixedParkingFS);
                //doc.Delete(regionAlgorithm.fixedParkingAndColumnFS);
                //doc.Delete(regionAlgorithm.directShapes);
                doc.Delete(regionAlgorithm.filledRegions_singleParkignPlace);
                deleteTrans.Commit();
            }
            #endregion

            #region 将停车区域打印出来
            //double moveDownDistance = doc.ActiveView.GenLevel.ProjectElevation;
            //_Methods.ShowTempGeometry(doc, _regionCurves, new XYZ(0, 0, moveDownDistance));
            foreach (CurveArray _regionCurves in firstCanPlacedRegions)
            {
                #region 绘制详图填充区域
                CreatFilledRegion(doc, _filledRegionTypeId, _regionCurves);
                #endregion
            }
            #endregion

            #region 停车位全局刷新requestHandler，与智能刷新RequestHandlerDocumentChangeEvent，在停车位算法上，使用一个方法
            using (ParkingAlgorithm parkingAlgorithm = new ParkingAlgorithm())
            {
                var transactionNow = new TransactionNow(doc);
                int i = 0;

                #region 计时器
                //this.m_progressTracker.StartTracking_TS("Getting Properties / Methods..",1, firstCanPlacedRegions.Size);
                #endregion

                foreach (CurveArray _regionCurves in firstCanPlacedRegions)//针对单个区域进行停车位计算处理
                {
                    if (i == 7)
                    {

                    }
                    #region 1 设定停止计算Flag 2 将当前停车区域打印在当前视图
                    if (CMD.stopAlgorithm == false) break;

                    #endregion

                    #region 获取与停车计算区域有关的障碍物区域
                    CurveArrArray nowObstacleCurveArrArray = new CurveArrArray();
                    List<FilledRegion> needFilledRegions = _Methods.GetTarlElesByRegion(doc, _regionCurves, filledRegions).Cast<FilledRegion>().ToList();
                    needFilledRegions.ForEach(p =>
                    {
                        foreach (CurveArray curveArray in _Methods.CurveLoopsToCurveArrArray(p.GetBoundaries().ToList()))
                            nowObstacleCurveArrArray.Append(curveArray);
                    });
                    #endregion

                    #region 此处需要判断填充区域 需不需要考虑架空处理 使用transform进行转换
                    CurveArrArray otherObstalLoops = GetOthersObstalRegions(doc, _regionCurves, othersObstalFs);
                    foreach (CurveArray curveArray1 in otherObstalLoops)
                    {
                        nowObstacleCurveArrArray.Append(curveArray1);
                        #region 将新增属性线添加到allCurves中
                        foreach (Curve curve in curveArray1)
                        {
                            curve.SetGraphicsStyleId(obstacleLineStyleId);
                            allCurves.Add(curve);
                        }
                        #endregion
                    }
                    #endregion

                    #region 对固定车位进行处理 这里提取 已定位的车位区域，需要考虑 柱子的宽度 车道的宽度 

                    List<Element> needlFixedParkingFS = _Methods.GetTarlElesByRegion(doc, _regionCurves, allFixedParkingFS);
                    Paths_xyz unionParkingSplace = GetParkingSpaceUnion(doc, needlFixedParkingFS);

                    CurveArrArray allFixedParkingCurveArray = _Methods.ListLinesToCurveArrArray(_Methods.GetListClosedtLineFromListPoints(unionParkingSplace));
                    foreach (CurveArray curveArray in allFixedParkingCurveArray)
                        nowObstacleCurveArrArray.Append(curveArray);

                    #endregion

                    #region 用停车位空间进行裁剪 未采用
                    //Paths subjsPlace = clipper_methods.Paths_xyzToPaths(new Paths_xyz() { _Methods.GetUniqueXYZFromCurves(_regionCurves) });
                    //Paths clipsFiexedParking = clipper_methods.Paths_xyzToPaths(_Methods.GetUniqueXYZFromCurves(allFixedParkingCurveArray));
                    //Paths_xyz canPlacedRegion = clipper_methods.PathsToPaths_xyz(RegionAlgorithm.RegionCropctDifference(subjsPlace, clipsFiexedParking));

                    //if (canPlacedRegion.Count < 1)
                    //    continue;
                    //_regionCurves = _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(canPlacedRegion.First()));
                    #endregion

                    #region 得到目标区域内的相关属性线条 开启停车sweep算法
                    List<Curve> nowRegionPropertyLines = _Methods.GetCurvesInTarRegion(doc, _regionCurves, allCurves);

                    List<Point> Max_tar_columnplaceXYZs = new List<Point>(); //求出停车位的放置点
                    List<Point> Max_tar_placeXYZs = new List<Point>();
                    if (CMD.isOptimalAlgorithm == true)
                    {
                        Max_tar_placeXYZs = parkingAlgorithm.tarMaxSweep(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                    }
                    else if (CMD.isOptimalAlgorithm == false)
                    {
                        Max_tar_placeXYZs = parkingAlgorithm.tarSweep(doc, _regionCurves, nowObstacleCurveArrArray, nowRegionPropertyLines, out Max_tar_columnplaceXYZs);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，
                    }
                    transactionNow.LayoutParking(Max_tar_placeXYZs, Max_tar_columnplaceXYZs, CMD.layoutMethod);//开启放置事务  
                    #endregion

                    //}
                    i++;

                    #region 计时器
                    //if (this.m_progressTracker.StopTask) break;
                    //this.m_progressTracker.Current++;
                    #endregion
                }
                //this.m_progressTracker.StopTracking_TS();
            }
            #endregion

            #region 将可停车region 点集 写入entity 记录数据 设置entity 将上述生成的各个停车区域点集 存入地库外墙线线圈group的entity

            SetEntityToBaseWallLoop(doc, _schemaGuid, CMD.baseMentWallLoopId, regionAlgorithm.parkingRegionsPoints); //Entity赋值

            #endregion
            DataStatistics(doc, basementWallRegion);

            #region GC 垃圾回收 Garbage Collector
            GC.Collect();
            #endregion

        }
        /// <summary>
        /// 选择地库设计区域 组名设置为 地库_地库外墙线
        /// </summary>
        public void SetControlRegionBoundary(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var acvtiView = doc.ActiveView;
            #endregion

            #region 点选地库外墙边界线 该处略作调整，需要把线组group组名需要包含字段 “地库_地库外墙线” 判断线圈是否闭合 判断线圈内所有线样式是否为 “地库_地库外墙线”记录数据 1. 记录选择地库_地库外墙线组Id
            var _filledRegion = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion(), "请点选详图区域，地库外墙线线圈（线样式为绿色）")) as FilledRegion;
            FilledRegionType _filledRegionType = doc.GetElement(_filledRegion.GetTypeId()) as FilledRegionType;
            if (!_filledRegionType.Name.Contains("地库外墙线")) throw new NotImplementedException("选择的详图区域类型名字不包含字段“地库外墙线”，请确认是否正确设置，或选择错误。");
            #endregion


            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region 求出组内唯一详图区域，作为地库_地库外墙线线圈
            //List<ElementId> elementIds = _group.GetMemberIds().ToList();
            //List<Element> elements = _Methods.EleIdsToEles(doc, elementIds);

            //List<FilledRegion> filledRegions = new List<FilledRegion>();

            //elements.ForEach(p =>
            //{
            //    if (p is FilledRegion) filledRegions.Add(p as FilledRegion);
            //});

            //int fRegions = filledRegions.Count;

            //if (fRegions > 1) throw new NotImplementedException("当前组内地库外墙线详图区域不唯一，请确认。");
            //else if (fRegions == 0) throw new NotImplementedException("当前组内不存在地库外墙线详图区域，请确认。");

            var basementWallLoop = _filledRegion.GetBoundaries().First();
            var basementWallCurves = _Methods.CurveLoopToCurveArray(basementWallLoop);

            #region 为地库_地库外墙线赋予线样式
            foreach (Curve curve in basementWallCurves)
                curve.SetGraphicsStyleId(baseWallLineStyleId);

            #endregion

            #endregion

            #region 点选地库外墙边界线 该处略作调整，需要把线组group组名需要包含字段 “地库_地库外墙线” 判断线圈是否闭合 判断线圈内所有线样式是否为 “地库_地库外墙线”记录数据 1. 记录选择地库_地库外墙线组Id
            //var basementWallRegion = _Methods.GetCurvesFromCurveGroup(_group);
            //var basementWallLoop = new CurveLoop();
            //bool isLoop = _Methods.IsCurveLoop(basementWallRegion, out basementWallLoop);
            //if (!isLoop) throw new NotImplementedException("当前线框，组名为" + _group.Name + "，该组内线圈未闭合。");

            //string _GraphicsStyle_frist_name;//得到Group内线样式
            //bool iSame = _Methods.IsSameAllCurveLineStyleFromGroup(doc, _group, out _GraphicsStyle_frist_name);
            //if (!iSame) throw new NotImplementedException("当前线框，组名为" + _group.Name + "，该组内所有线条的线样式未统一为 “地库_地库外墙线”。");
            //else if(_GraphicsStyle_frist_name != "地库_地库外墙线") throw new NotImplementedException("当前线框，组名为" + _group.Name + "，该组内所有线条的线样式未统一为 “地库_地库外墙线”。");

            CMD.baseMentWallLoopId = _filledRegion.Id;//记录选择的 地库_地库外墙线组Id 
            #endregion

            #region 求出当前可停车区域，以详图填充方式展现

            RegionAlgorithm regionAlgorithm = new RegionAlgorithm(doc, basementWallCurves);

            using (var deleteTrans = new Transaction(doc, "删除所有未锁定停车位、柱子以及diretShape。"))//启动事务 删除当前地库外墙线区域内的停车位和柱子族实例（未锁定）
            {
                deleteTrans.Start();
                doc.Delete(regionAlgorithm.unfixedParkingFS);
                //doc.Delete(regionAlgorithm.fixedParkingAndColumnFS);
                //doc.Delete(regionAlgorithm.directShapes);
                doc.Delete(regionAlgorithm.filledRegions_singleParkignPlace);
                deleteTrans.Commit();
            }
            #region 求出目标填充样式
            ElementId _filledRegionTypeId = GetTarFilledRegionTypeId(doc, "地库_单个停车区域");
            #endregion

            CurveArrArray firstCanPlacedRegions = regionAlgorithm.parkingCurveArrArray;//布尔计算后的目标停车区域
            //double moveDistance = doc.ActiveView.GenLevel.ProjectElevation;
            foreach (CurveArray curveArray in firstCanPlacedRegions)
            {
                //_Methods.ShowTempGeometry(doc, curveArray);//将可停车区域的边界线 打印出来
                #region 绘制详图填充区域
                CreatFilledRegion(doc, _filledRegionTypeId, curveArray);
                #endregion
            }

            #endregion

            #region 记录数据 2. 设置entity 将上述生成的各个停车区域点集 存入地库外墙线线圈group的entity

            SetEntityToBaseWallLoop(doc, _schemaGuid, CMD.baseMentWallLoopId, regionAlgorithm.parkingRegionsPoints); //Entity赋值

            #endregion

        }
        /// <summary>
        /// DocumentChanged事件注册
        /// </summary>
        public void DocumentChangedEventRegister(UIApplication uiapp)
        {
            uiapp.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(MainRoadChanged);
        }
        /// <summary>
        /// DocumentChanged事件取消注册
        /// </summary>
        public void DocumentChangedEventUnRegister(UIApplication uiapp)
        {
            uiapp.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(MainRoadChanged);
        }
        /// <summary>
        /// 1 监控函数 2 进入外部事件，需要首尾取消和重新注册 documentchange 事件 3 经测试 对线条的操作，并不能实时响应监控函数
        /// </summary>
        public void MainRoadChanged(Object sender, DocumentChangedEventArgs args)
        {
            Document doc = args.GetDocument();
            RequestHandlerDocumentChangeEvent rquestHandlerDocumentChangeEvent = new RequestHandlerDocumentChangeEvent();
            ExternalEvent externalEvent = ExternalEvent.Create(rquestHandlerDocumentChangeEvent);
            externalEvent.Raise();
        }
        /// <summary>
        /// 刷新统计数据
        /// </summary>
        public void RefreshDataSatistis(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 点选地库外墙边界线 该处略作调整，需要把线组group组名需要包含字段 “地库_地库外墙线” 判断线圈是否闭合 判断线圈内所有线样式是否为 “地库_地库外墙线”记录数据 1. 记录选择地库_地库外墙线组Id
            var _group = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_Groups(), "请点选详图区域，地库外墙线线圈（线央视为绿色）")) as Group;
            if (!_group.Name.Contains("地库外墙线")) throw new NotImplementedException("选择的线圈组名不包含字段“地库外墙线”，请确认是否正确设置组名，或选择错误。");
            #endregion

            #region 求出组内唯一详图区域，作为地库_地库外墙线线圈
            List<ElementId> elementIds = _group.GetMemberIds().ToList();
            List<Element> elements = _Methods.EleIdsToEles(doc, elementIds);

            List<FilledRegion> filledRegions = new List<FilledRegion>();

            elements.ForEach(p =>
            {
                if (p is FilledRegion) filledRegions.Add(p as FilledRegion);
            });

            int fRegions = filledRegions.Count;

            if (fRegions > 1) throw new NotImplementedException("当前组内地库外墙线详图区域不唯一，请确认。");
            else if (fRegions == 0) throw new NotImplementedException("当前组内不存在地库外墙线详图区域，请确认。");

            var basementWallLoop = filledRegions.First().GetBoundaries().First();
            var basementWallCurves = _Methods.CurveLoopToCurveArray(basementWallLoop);

            #endregion

            DataStatistics(doc, basementWallCurves);
        }
        /// <summary>
        /// 检测地库强排所需线型是否必备
        /// </summary>
        public void CheckLineStyles(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<string> AllLineGraphicsStylNames = _Methods.getAllLineGraphicsStylNames(doc);
            //判断必备线型是否存在
            if (AllLineGraphicsStylNames.Contains("地库_场地控制红线")
                && AllLineGraphicsStylNames.Contains("地库_障碍物边界线")
                && AllLineGraphicsStylNames.Contains("地库_地库外墙线")
                && AllLineGraphicsStylNames.Contains("地库_主车道中心线")
                && AllLineGraphicsStylNames.Contains("地库_次车道中心线")
                && AllLineGraphicsStylNames.Contains("地库_红线退距线"))
            {
                //TaskDialog.Show("正确", "地库强排所需线型，均已存在当前文档中");
            }
            else//逐个判断，线型是否存在
            {
                if (!AllLineGraphicsStylNames.Contains("地库_场地控制红线"))//线型不存在，便创建
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_场地控制红线", 16, new Color(255, 0, 0));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_障碍物边界线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_障碍物边界线", 2, new Color(0, 0, 0));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_地库外墙线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_地库外墙线", 16, new Color(0, 166, 0));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_主车道中心线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_主车道中心线", 2, new Color(255, 0, 0));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_次车道中心线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_次车道中心线", 2, new Color(204, 153, 102));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_红线退距线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_红线退距线", 16, new Color(255, 127, 0));
                }
                //if (!AllLineGraphicsStylNames.Contains("地库_工作框"))
                //{
                //    Category _category = _Methods.CreatLineStyle(doc, "地库_工作框", 2, new Color(0, 0, 0));
                //}
                //进行二次判断
                AllLineGraphicsStylNames = _Methods.getAllLineGraphicsStylNames(doc);
                if (AllLineGraphicsStylNames.Contains("地库_场地控制红线")
                && AllLineGraphicsStylNames.Contains("地库_障碍物边界线")
                && AllLineGraphicsStylNames.Contains("地库_地库外墙线")
                && AllLineGraphicsStylNames.Contains("地库_主车道中心线")
                && AllLineGraphicsStylNames.Contains("地库_红线退距线"))
                {
                    //TaskDialog.Show("正确", "地库强排所需线型，均已存在当前文档中");
                }
                else
                {
                    //throw new NotImplementedException("地库必备线型，自动创建失败，请手动创建");
                }
            }
        }
        //以下为各种method—————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————


        /// <summary>
        /// 获取其他障碍物，如直线型坡道架空空间，同时赋予线属性
        /// </summary>
        public CurveArrArray GetOthersObstalRegions(Document doc, CurveArray _regionCurves, List<Element> othersObstalFs)
        {
            List<FamilyInstance> othersObstalFses = _Methods.GetTarlElesByRegion(doc, _regionCurves, othersObstalFs).Cast<FamilyInstance>().ToList();
            List<CurveLoop> CurveLoops = new List<CurveLoop>();
            othersObstalFses.ForEach(p =>
            {
                double wight = Methods.MilliMeterToFeet(6000) / 2;
                double height = Methods.MilliMeterToFeet(12000);
                Transform transform = (p as FamilyInstance).GetTransform();
                XYZ leftDownPoint = new XYZ(0, -wight, 0);
                XYZ leftUpPoint = new XYZ(0, wight, 0);
                XYZ rightUpPoint = new XYZ(height, wight, 0);
                XYZ rightDownPoint = new XYZ(height, -wight, 0);
                XYZ _leftDownPoint = transform.OfPoint(leftDownPoint);
                XYZ _leftUpPoint = transform.OfPoint(leftUpPoint);
                XYZ _rightUpPoint = transform.OfPoint(rightUpPoint);
                XYZ _rightDownPoint = transform.OfPoint(rightDownPoint);
                CurveLoop curves = new CurveLoop();
                curves.Append(Line.CreateBound(_leftDownPoint, _leftUpPoint));
                curves.Append(Line.CreateBound(_leftUpPoint, _rightUpPoint));
                curves.Append(Line.CreateBound(_rightUpPoint, _rightDownPoint));
                curves.Append(Line.CreateBound(_rightDownPoint, _leftDownPoint));
                CurveLoops.Add(curves);
            });

            CurveArrArray curveArrArray = _Methods.CurveLoopsToCurveArrArray(CurveLoops);

            return curveArrArray;
        }
        /// <summary>
        /// 将curveArray线圈绘制为详图填充区域
        /// </summary>
        public void CreatFilledRegion(Document doc, ElementId filledRegionTypeId, CurveArray curveArray)
        {
            View acvtiView = doc.ActiveView;
            using (Transaction creatFilledRegion = new Transaction(doc, "creatFilledRegion"))
            {
                creatFilledRegion.Start();

                CurveLoop curves = _Methods.CurveArrayToCurveLoop(curveArray);
                FilledRegion filledRegion = FilledRegion.Create(doc, filledRegionTypeId, acvtiView.Id, new List<CurveLoop>() { curves });

                OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
                ogs.SetSurfaceTransparency(100);
                acvtiView.SetElementOverrides(filledRegion.Id, ogs);
                creatFilledRegion.Commit();
            }
        }
        /// <summary>
        /// 求出文档中符合目标字段的详图填充样式，如果不存在，则创建
        /// </summary>
        public ElementId GetTarFilledRegionTypeId(Document doc, string str)
        {
            var filledRegionTypes = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_DetailComponents).OfClass(typeof(FilledRegionType)).ToElements();
            var fillPatternTypes = (new FilteredElementCollector(doc)).OfClass(typeof(FillPatternElement)).ToElements();
            Element _filledRegionType = filledRegionTypes.First();
            Element _fillPatternTypes = fillPatternTypes.First(p => p.Name == "<实体填充>");
            List<string> filledRegionTypeNames = filledRegionTypes.Select(p => p.Name).ToList();
            if (filledRegionTypeNames.Contains(str))
            {
                _filledRegionType = filledRegionTypes.Where(p => p.Name == str).First();
            }
            else
            {
                using (Transaction creatFilledRegionType = new Transaction(doc, "creatFilledRegionType"))
                {
                    creatFilledRegionType.Start();
                    FilledRegionType filledRegionType = (_filledRegionType as FilledRegionType).Duplicate(str) as FilledRegionType;
                    filledRegionType.ForegroundPatternId = _fillPatternTypes.Id;
                    creatFilledRegionType.Commit();
                }
            }
            return _filledRegionType.Id;
        }
        /// <summary>
        /// 将directShape线圈转化为curveArray
        /// </summary>
        public CurveArray GetCurveArrayFromDirectshape(DirectShape selDs)
        {
            Options opts = new Options();
            List<GeometryObject> objs = selDs.get_Geometry(opts).ToList();
            CurveArray selCurveLoop = new CurveArray();//选择的停车区域
            foreach (GeometryObject geometryObject in objs)
            {
                if (geometryObject is Curve)
                {
                    selCurveLoop.Append(geometryObject as Curve);
                }
            }
            return selCurveLoop;
        }
        /// <summary>
        /// 给group中写入entity，本次读取数据为点集列表
        /// </summary>
        public Paths_xyz ReadEntityFromBaseWallLoop(Document doc, Guid guid, ElementId elementId)
        {
            Schema schema = Schema.Lookup(guid);//guid为固定guid
            Paths_xyz firstParkingPlaces = new Paths_xyz();
            Entity _entity = doc.GetElement(elementId).GetEntity(schema);
            for (int i = 0; i < schema.ListFields().Count; i++)
            {
                string _fieldBuilderName = fieldBuilderName + i.ToString();
                IList<XYZ> xYZs = _entity.Get<IList<XYZ>>(_fieldBuilderName, DisplayUnitType.DUT_MILLIMETERS);
                if (xYZs.Count > 2)
                {
                    firstParkingPlaces.Add(xYZs.ToList());
                }
            }
            return firstParkingPlaces;
        }
        /// <summary>
        /// 给group中写入entity，本次写入数据为点集列表
        /// </summary>
        public void SetEntityToBaseWallLoop(Document doc, Guid guid, ElementId elementId, Paths_xyz paths_Xyz)
        {
            #region 设置entity 将上述生成的各个停车区域点集 存入地库外墙线线圈group的entity

            Schema schema = Schema.Lookup(guid);//guid为固定guid
            if (schema == null)
            {
                SchemaBuilder schemaBulider = new SchemaBuilder(_schemaGuid);
                schemaBulider.SetReadAccessLevel(AccessLevel.Public);
                schemaBulider.SetWriteAccessLevel(AccessLevel.Public);
                schemaBulider.SetSchemaName("canParkingPlaces");
                schemaBulider.SetDocumentation("该类架构用于存储各个可停车区域的点集。");

                int i = 0;
                while (i < 256)
                {
                    string _fieldBuilderName = fieldBuilderName + i.ToString();
                    FieldBuilder fieldBuilder = schemaBulider.AddArrayField(_fieldBuilderName, typeof(XYZ));
                    fieldBuilder.SetUnitType(UnitType.UT_Length);
                    i++;
                }
                schema = schemaBulider.Finish();
            }
            else
            {
                //Schema.EraseSchemaAndAllEntities(schema, true);
                //_Methods.TaskDialogShowMessage(schema.GUID.ToString());
            }
            #endregion

            #region entity赋值

            Entity entity = new Entity(schema);

            for (int i = 0; i < paths_Xyz.Count; i++)
            {
                string _fieldBuilderName = fieldBuilderName + i.ToString();
                IList<XYZ> xYZs = paths_Xyz[i];
                entity.Set<IList<XYZ>>(_fieldBuilderName, xYZs, DisplayUnitType.DUT_MILLIMETERS);
            }
            using (var setGroupEntity = new Transaction(doc, "地库_地库外墙线线组 set entity"))//对一个元素 Set相同schema下的Entity，以最新entity的数据为准
            {
                setGroupEntity.Start();
                doc.GetElement(elementId).SetEntity(entity);
                //Schema.EraseSchemaAndAllEntities(schema, true);
                setGroupEntity.Commit();
            }
            #endregion
        }
        /// <summary>
        /// 从静态数据 读取地库_地库外墙线 对组名、线圈是否闭合、线圈线样式是否统一,进行审查；
        /// </summary>
        /// <returns></returns>
        public CurveArray GetRegionFromMemoryEleId(Document doc, ElementId eleId)
        {
            #region 点选地库外墙边界线 该处略作调整，需要把线组group组名需要包含字段 “地库_地库外墙线” 判断线圈是否闭合 判断线圈内所有线样式是否为 “地库_地库外墙线”记录数据 1. 记录选择地库_地库外墙线组Id
            var _filledRegion = doc.GetElement(eleId) as FilledRegion;

            FilledRegionType _filledRegionType = doc.GetElement(_filledRegion.GetTypeId()) as FilledRegionType;
            if (!_filledRegionType.Name.Contains("地库外墙线")) throw new NotImplementedException("选择的详图区域类型名字不包含字段“地库外墙线”，请确认是否正确设置，或选择错误。");
            #endregion

            #region 求出所有的线样式 Ids
            //ElementId redLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_场地控制红线");
            ElementId obstacleLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_障碍物边界线");
            //ElementId mainRoadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_主车道中心线");
            ElementId baseWallLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_地库外墙线");
            //ElementId roadLineStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_次车道中心线");
            //ElementId redLineOffsetStyleId = _Methods.GetTarGraphicsStyleId(doc, "地库_红线退距线");
            #endregion

            #region 求出组内唯一详图区域，作为地库_地库外墙线线圈

            var basementWallLoop = _filledRegion.GetBoundaries().First();
            var basementWallCurves = _Methods.CurveLoopToCurveArray(basementWallLoop);

            #region 为地库_地库外墙线赋予线样式
            foreach (Curve curve in basementWallCurves)
                curve.SetGraphicsStyleId(baseWallLineStyleId);

            #endregion
            #endregion

            return basementWallCurves;
        }
        /// <summary>
        /// 输出数据
        /// </summary>
        public void DataStatistics(Document doc, CurveArray controlRegion)
        {
            #region 计算 地库_地库外墙线 线圈面积
            CurveLoop baseMentWallLoop = new CurveLoop();
            bool isLoop = _Methods.IsCurveLoop(controlRegion, out baseMentWallLoop);
            if (!isLoop) throw new NotImplementedException("当前选择线圈组内，线圈未闭合。");
            double _Area = ExporterIFCUtils.ComputeAreaOfCurveLoops(new List<CurveLoop>() { baseMentWallLoop }); //计算面积，鞋带法 对数据需要进行小数取位数
            _Area = UnitUtils.Convert(_Area, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);
            #endregion

            List<FamilyInstance> parkingFses = (new FilteredElementCollector(doc, doc.ActiveView.Id))
                .OwnedByView(doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_DetailComponents)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<Element> selParkingFses = new List<Element>();
            parkingFses.ForEach(p =>
            {
                Parameter basementFamilyType = p.Symbol.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
                if (basementFamilyType != null)
                {
                    string basementFamilyTypeStr = basementFamilyType.AsString();
                    if (basementFamilyTypeStr != null && basementFamilyTypeStr != "")
                    {
                        if (FirmStandards.BasementFamilytypeID[basementFamilyTypeStr] == BasementFamilyType.ParkingSpace)
                        {
                            selParkingFses.Add(p as Element);
                        }
                    }
                }
            });

            selParkingFses = _Methods.GetTarlElesByRegion(doc, controlRegion, selParkingFses);

            double _ParkingEfficiency = _Area / selParkingFses.Count;//计算停车效率
            _Area = _Methods.TakeNumberAfterDecimal(_Area, 2);
            _ParkingEfficiency = _Methods.TakeNumberAfterDecimal(_ParkingEfficiency, 2);

            string str_Time = @"计算时间：" + DateTime.Now.ToString();
            string str_Area = @"建筑面积：" + _Area.ToString() + @" ㎡；";
            string parkingplace_count = @"停车位数辆：" + selParkingFses.Count.ToString() + @" 辆；";
            string str_ParkingEfficiency = @"停车效率：" + _ParkingEfficiency.ToString() + @" ㎡/车；" + "\n";

            CMD.TestList.Add(str_Time);
            CMD.TestList.Add(str_Area);
            CMD.TestList.Add(parkingplace_count);
            CMD.TestList.Add(str_ParkingEfficiency);
        }
        /// <summary>
        /// 获取Boundingbox的四个角点
        /// </summary>
        public List<XYZ> GetBoundingBoxFourCornerPoint(BoundingBoxXYZ _boundingBoxXYZ)
        {
            XYZ min = _boundingBoxXYZ.Min;
            XYZ max = _boundingBoxXYZ.Max;

            return GetRectangeFourCornerPointsFromTwoPoins(min, max);
        }
        /// <summary>
        /// 通过两个点求正交矩形线圈
        /// </summary>
        public CurveArray GetRectangle(XYZ point01, XYZ point02)
        {
            List<XYZ> controlRegionPoints = GetRectangeFourCornerPointsFromTwoPoins(point01, point02);
            return _Methods.LinesToCurveArray(_Methods.GetClosedLinesFromPoints(controlRegionPoints));
        }
        /// <summary>
        /// 通过对角线点获取矩形的四个角点
        /// </summary>
        public List<XYZ> GetRectangeFourCornerPointsFromTwoPoins(XYZ point01, XYZ point02)
        {
            double xPoint01 = point01.X;
            double yPoint01 = point01.Y;
            double xPoint02 = point02.X;
            double yPoint02 = point02.Y;
            XYZ leftDownPoint = XYZ.Zero;
            XYZ leftUpPoint = XYZ.Zero;
            XYZ rightUpPoint = XYZ.Zero;
            XYZ rightDownPoint = XYZ.Zero;
            if (xPoint01 < xPoint02 && yPoint01 < yPoint02)
            {
                leftDownPoint = new XYZ(xPoint01, yPoint01, 0);
                leftUpPoint = new XYZ(xPoint01, yPoint02, 0);
                rightUpPoint = new XYZ(xPoint02, yPoint02, 0);
                rightDownPoint = new XYZ(xPoint02, yPoint01, 0);
            }
            if (xPoint01 < xPoint02 && yPoint01 > yPoint02)
            {
                leftDownPoint = new XYZ(xPoint01, yPoint02, 0);
                leftUpPoint = new XYZ(xPoint01, yPoint01, 0);
                rightUpPoint = new XYZ(xPoint02, yPoint01, 0);
                rightDownPoint = new XYZ(xPoint02, yPoint02, 0);
            }
            if (xPoint01 > xPoint02 && yPoint01 > yPoint02)
            {
                leftDownPoint = new XYZ(xPoint02, yPoint02, 0);
                leftUpPoint = new XYZ(xPoint02, yPoint01, 0);
                rightUpPoint = new XYZ(xPoint01, yPoint01, 0);
                rightDownPoint = new XYZ(xPoint01, yPoint02, 0);
            }
            if (xPoint01 > xPoint02 && yPoint01 < yPoint02)
            {
                leftDownPoint = new XYZ(xPoint02, yPoint01, 0);
                leftUpPoint = new XYZ(xPoint02, yPoint02, 0);
                rightUpPoint = new XYZ(xPoint01, yPoint02, 0);
                rightDownPoint = new XYZ(xPoint01, yPoint01, 0);
            }
            if (xPoint01 == xPoint02 || yPoint01 == yPoint02)
            {
                throw new NotImplementedException("所选定位点，构不成成立矩形的条件");
            }
            return new List<XYZ>() { leftDownPoint, leftUpPoint, rightUpPoint, rightDownPoint };
        }

        #region 将定位车位空间 做clipper union处理
        /// <summary>
        /// 获取指定区域内的停车位占据空间
        /// </summary>
        /// <returns></returns>
        public Paths_xyz GetParkingSpaceUnion(Document doc, List<Element> allFixedParkingFS)
        {
            List<XYZ> northParkingLocation = GetNorthParkingFSLocationPoint(allFixedParkingFS);
            List<XYZ> sourthParkingLocation = GetSourthParkingFSLocationPoint(allFixedParkingFS);

            List<XYZ> eastParkingLocation = GetEastParkingFSLocationPoint(allFixedParkingFS);
            List<XYZ> westthParkingLocation = GetWestParkingFSLocationPoint(allFixedParkingFS);

            Paths northPointsToPaths = clipper_methods.Paths_xyzToPaths(NorthPointsToPaths_xyz(doc, northParkingLocation, CMD.parkingPlaceHeight - _Methods.PRECISION, CMD.parkingPlaceWight - _Methods.PRECISION, CMD.columnWidth - _Methods.PRECISION, CMD.columnBurfferDistance - _Methods.PRECISION));
            Paths sorthPointsToPaths = clipper_methods.Paths_xyzToPaths(SourthPointsToPaths_xyz(doc, sourthParkingLocation, CMD.parkingPlaceHeight - _Methods.PRECISION, CMD.parkingPlaceWight - _Methods.PRECISION, CMD.columnWidth - _Methods.PRECISION, CMD.columnBurfferDistance - _Methods.PRECISION));

            Paths eastPointsToPaths = clipper_methods.Paths_xyzToPaths(EastPointsToPaths_xyz(doc, eastParkingLocation, CMD.parkingPlaceHeight - _Methods.PRECISION, CMD.parkingPlaceWight - _Methods.PRECISION, CMD.columnWidth - _Methods.PRECISION, CMD.columnBurfferDistance - _Methods.PRECISION));
            Paths westPointsToPaths = clipper_methods.Paths_xyzToPaths(WestPointsToPaths_xyz(doc, westthParkingLocation, CMD.parkingPlaceHeight - _Methods.PRECISION, CMD.parkingPlaceWight - _Methods.PRECISION, CMD.columnWidth - _Methods.PRECISION, CMD.columnBurfferDistance - _Methods.PRECISION));

            northPointsToPaths.AddRange(sorthPointsToPaths);
            northPointsToPaths.AddRange(eastPointsToPaths);
            northPointsToPaths.AddRange(westPointsToPaths);

            if (northPointsToPaths.Count < 1)
            {
                return new Paths_xyz();
            }

            return clipper_methods.PathsToPaths_xyz(clipper_methods.RegionCropctUnion(northPointsToPaths, sorthPointsToPaths));
        }
        /// <summary>
        /// 将北向点集转化为线圈点
        /// </summary>
        public Paths_xyz EastPointsToPaths_xyz(Document doc, List<XYZ> northParkingLocation, double height, double width, double columnWidth, double columnBurfferDistance)
        {
            Paths_xyz temp = new Paths_xyz();
            northParkingLocation.ForEach(p => {

                //XYZ leftDown = new XYZ(p.X - height / 2, p.Y - width / 2 - columnWidth - columnBurfferDistance * 2, 0);
                //XYZ lefyUp = new XYZ(p.X + height / 2 + height, p.Y - width / 2 - columnWidth - columnBurfferDistance * 2, 0);
                //XYZ rightUp = new XYZ(p.X + height / 2 + height, p.Y + width / 2 + columnWidth + columnBurfferDistance * 2, 0);
                //XYZ rightDown = new XYZ(p.X - height / 2, p.Y + width / 2 + columnWidth + columnBurfferDistance * 2, 0);

                XYZ leftDown = new XYZ(p.X - height / 2, p.Y - width / 2, 0);
                XYZ lefyUp = new XYZ(p.X + height / 2 + height, p.Y - width / 2, 0);
                XYZ rightUp = new XYZ(p.X + height / 2 + height, p.Y + width / 2, 0);
                XYZ rightDown = new XYZ(p.X - height / 2, p.Y + width / 2, 0);

                temp.Add(new Path_xyz() { leftDown, lefyUp, rightUp, rightDown });

            });

            return temp;
        }
        /// <summary>
        /// 将北向点集转化为线圈点
        /// </summary>
        public Paths_xyz NorthPointsToPaths_xyz(Document doc, List<XYZ> northParkingLocation, double height, double width, double columnWidth, double columnBurfferDistance)
        {
            Paths_xyz temp = new Paths_xyz();
            northParkingLocation.ForEach(p => {

                //XYZ leftDown = new XYZ(p.X - width / 2 - columnWidth - columnBurfferDistance * 2, p.Y - height / 2, 0);
                //XYZ lefyUp = new XYZ(p.X - width / 2 - columnWidth - columnBurfferDistance * 2, p.Y + height / 2 + height, 0);
                //XYZ rightUp = new XYZ(p.X + width / 2 + columnWidth + columnBurfferDistance * 2, p.Y + height / 2 + height, 0);
                //XYZ rightDown = new XYZ(p.X + width / 2 + columnWidth + columnBurfferDistance * 2, p.Y - height / 2, 0);

                XYZ leftDown = new XYZ(p.X - width / 2, p.Y - height / 2, 0);
                XYZ lefyUp = new XYZ(p.X - width / 2, p.Y + height / 2 + height, 0);
                XYZ rightUp = new XYZ(p.X + width / 2, p.Y + height / 2 + height, 0);
                XYZ rightDown = new XYZ(p.X + width / 2, p.Y - height / 2, 0);
                temp.Add(new Path_xyz() { leftDown, lefyUp, rightUp, rightDown });

            });

            return temp;
        }
        /// <summary>
        /// 将南向点集转化为线圈点
        /// </summary>
        public Paths_xyz WestPointsToPaths_xyz(Document doc, List<XYZ> northParkingLocation, double height, double width, double columnWidth, double columnBurfferDistance)
        {
            Paths_xyz temp = new Paths_xyz();
            northParkingLocation.ForEach(p => {

                //XYZ leftDown = new XYZ(p.X - height / 2 - height, p.Y - width / 2 - columnWidth - columnBurfferDistance * 2, 0);
                //XYZ lefyUp = new XYZ(p.X + height / 2, p.Y - width / 2 - columnWidth - columnBurfferDistance * 2, 0);
                //XYZ rightUp = new XYZ(p.X + height / 2, p.Y + width / 2 + columnWidth + columnBurfferDistance * 2, 0);
                //XYZ rightDown = new XYZ(p.X - height / 2 - height, p.Y + width / 2 + columnWidth + columnBurfferDistance * 2, 0);

                XYZ leftDown = new XYZ(p.X - height / 2 - height, p.Y - width / 2, 0);
                XYZ lefyUp = new XYZ(p.X + height / 2, p.Y - width / 2 - columnWidth, 0);
                XYZ rightUp = new XYZ(p.X + height / 2, p.Y + width / 2 + columnWidth, 0);
                XYZ rightDown = new XYZ(p.X - height / 2 - height, p.Y + width / 2, 0);

                temp.Add(new Path_xyz() { leftDown, lefyUp, rightUp, rightDown });

            });
            return temp;
        }
        /// <summary>
        /// 将南向点集转化为线圈点
        /// </summary>
        public Paths_xyz SourthPointsToPaths_xyz(Document doc, List<XYZ> northParkingLocation, double height, double width, double columnWidth, double columnBurfferDistance)
        {
            Paths_xyz temp = new Paths_xyz();
            northParkingLocation.ForEach(p => {

                //XYZ leftDown = new XYZ(p.X - width / 2 - columnWidth - columnBurfferDistance * 2, p.Y - height / 2 - height, 0);
                //XYZ lefyUp = new XYZ(p.X - width / 2 - columnWidth - columnBurfferDistance * 2, p.Y + height / 2, 0);
                //XYZ rightUp = new XYZ(p.X + width / 2 + columnWidth + columnBurfferDistance * 2, p.Y + height / 2, 0);
                //XYZ rightDown = new XYZ(p.X + width / 2 + columnWidth + columnBurfferDistance * 2, p.Y - height / 2 - height, 0);

                XYZ leftDown = new XYZ(p.X - width / 2, p.Y - height / 2 - height, 0);
                XYZ lefyUp = new XYZ(p.X - width / 2, p.Y + height / 2, 0);
                XYZ rightUp = new XYZ(p.X + width / 2, p.Y + height / 2, 0);
                XYZ rightDown = new XYZ(p.X + width / 2, p.Y - height / 2 - height, 0);

                temp.Add(new Path_xyz() { leftDown, lefyUp, rightUp, rightDown });

            });
            return temp;
        }
        /// <summary>
        /// 获取所有东向车位的族实例的中心点
        /// </summary>
        /// <param name="allFixedParkingFS"></param>
        public List<XYZ> GetEastParkingFSLocationPoint(List<Element> allFixedParkingFS)
        {
            List<FamilyInstance> northParkingFS = new List<FamilyInstance>();

            allFixedParkingFS.ForEach(p => {
                FamilyInstance parkingFS = p as FamilyInstance;
                if (parkingFS.FacingOrientation.Y < _Methods.PRECISION
                && parkingFS.FacingOrientation.Y > -_Methods.PRECISION
                && parkingFS.FacingOrientation.X > 1 - _Methods.PRECISION)
                {
                    northParkingFS.Add(parkingFS);
                }
            });

            List<XYZ> northParkingLocation = new List<XYZ>();

            northParkingFS.ForEach(p => {
                LocationPoint locationPoint = p.Location as LocationPoint;
                northParkingLocation.Add(locationPoint.Point);
            });

            return northParkingLocation;
        }
        /// <summary>
        /// 获取所有西向车位的族实例的中心点
        /// </summary>
        /// <param name="allFixedParkingFS"></param>
        public List<XYZ> GetWestParkingFSLocationPoint(List<Element> allFixedParkingFS)
        {
            List<FamilyInstance> northParkingFS = new List<FamilyInstance>();

            allFixedParkingFS.ForEach(p => {
                FamilyInstance parkingFS = p as FamilyInstance;
                if (parkingFS.FacingOrientation.Y < _Methods.PRECISION
                && parkingFS.FacingOrientation.Y > -_Methods.PRECISION
                && parkingFS.FacingOrientation.X < _Methods.PRECISION - 1)
                {
                    northParkingFS.Add(parkingFS);
                }
            });

            List<XYZ> northParkingLocation = new List<XYZ>();

            northParkingFS.ForEach(p => {
                LocationPoint locationPoint = p.Location as LocationPoint;
                northParkingLocation.Add(locationPoint.Point);
            });

            return northParkingLocation;
        }
        /// <summary>
        /// 获取所有北向车位的族实例的中心点
        /// </summary>
        /// <param name="allFixedParkingFS"></param>
        public List<XYZ> GetNorthParkingFSLocationPoint(List<Element> allFixedParkingFS)
        {
            List<FamilyInstance> northParkingFS = new List<FamilyInstance>();

            allFixedParkingFS.ForEach(p => {
                FamilyInstance parkingFS = p as FamilyInstance;
                if (parkingFS.FacingOrientation.X < _Methods.PRECISION
                && parkingFS.FacingOrientation.X > -_Methods.PRECISION
                && parkingFS.FacingOrientation.Y > 1 - _Methods.PRECISION)
                {
                    northParkingFS.Add(parkingFS);
                }
            });

            List<XYZ> northParkingLocation = new List<XYZ>();

            northParkingFS.ForEach(p => {
                LocationPoint locationPoint = p.Location as LocationPoint;
                northParkingLocation.Add(locationPoint.Point);
            });

            return northParkingLocation;
        }
        /// <summary>
        /// 获取所有南向车位的族实例的中心点
        /// </summary>
        public List<XYZ> GetSourthParkingFSLocationPoint(List<Element> allFixedParkingFS)
        {
            List<FamilyInstance> northParkingFS = new List<FamilyInstance>();

            allFixedParkingFS.ForEach(p => {
                FamilyInstance parkingFS = p as FamilyInstance;
                if (parkingFS.FacingOrientation.X < _Methods.PRECISION
                && parkingFS.FacingOrientation.X > -_Methods.PRECISION
                && parkingFS.FacingOrientation.Y < _Methods.PRECISION - 1)
                {
                    northParkingFS.Add(parkingFS);
                }
            });

            List<XYZ> northParkingLocation = new List<XYZ>();

            northParkingFS.ForEach(p => {
                LocationPoint locationPoint = p.Location as LocationPoint;
                northParkingLocation.Add(locationPoint.Point);
            });

            return northParkingLocation;
        }
        #endregion


    }  // public class RequestHandler : IExternalEventHandler
} // namespace
