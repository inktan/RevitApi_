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

using goa.Common;

namespace LayoutParkingEffcient
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();

        public string GetName()
        {
            return "Overlapping Elements Clean Up Request Handler";
        }
        public void Execute(UIApplication uiapp)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.TestMethod_temp:
                        {
                            TestMethod_temp(uiapp);
                            break;
                        }
                    case RequestId.SelectRegionalBoundary:
                        {
                            SelectRegionalBoundarys(uiapp);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                //UserMessages.ShowErrorMessage(ex, window);
                TaskDialog.Show("error", ex.Message);
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //外部事件方法建立
        /// <summary>
        /// 新建户型_1T2H_aa_1DY 函数架构 文档开启 在前 文档关闭 在后
        /// </summary>
        /// <param name="uiapp"></param>
        public void TestMethod_temp(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            CMD.TestList.Clear();
            #endregion

            #region 声明全局变量
            FamilySymbol parkingType = null;
            #endregion

            #region 寻找目标parkingPlace组类型
            string parkingTypeName = "垂直式_5100x2400mm";//设定目标停车位族类型名字
            string FamilyFilePath = @"D:\BIM_Model\停车位_强排\停车位_Basic.rfa";//停车位族文件所在位置
            parkingType = _FindFamilySymbolNeeded(doc, FamilyFilePath, parkingTypeName);
            #endregion


            #region 计算目标区域内的点
            double distance_x = 0;
            double distance_y = 0;
            //XYZ _XYZ = sel.PickPoint("Select one point");//选择一个点
            List<Line> SelRegionBoundings = Methods.SortLinesContiguous(CMD._selRegionBoundings);

            XYZ _XYZ = GetLeftDownXYZfromLines(CMD._selRegionBoundings, out distance_x, out distance_y);//目标区域所在矩形的左下角坐标，以及X方向长度以及Y方向长度


            //通过条件判断 选择布点方式
            IList<XYZ> tar_placeXYZs = new List<XYZ>();//通过判断求出落在目标区域内的点
            if (true)
            {
                tar_placeXYZs = GetTarPoints_vertical(_XYZ, CMD._selRegionBoundings, distance_x, distance_y);//通过判断求出落在目标区域内的点
            }

            string parkingplace_count = @"当前处于选择范围内的停车位数量为" + tar_placeXYZs.Count.ToString() + @"个；";
            double _x_mm = UnitUtils.Convert(_XYZ.X, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);//单位转换，英尺到毫米mm
            double _y_mm = UnitUtils.Convert(_XYZ.Y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
            double distance_x_mm = UnitUtils.Convert(distance_x, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
            double distance_y_mm = UnitUtils.Convert(distance_y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
            string str_origin01 = string.Format("选择线段所在矩形左下角X:{0};\n选择线段所在矩形左下角Y:{1};", _x_mm.ToString(), _y_mm.ToString());//数据输出查看
            string str_origin02 = string.Format("选择线段所在矩形X_siatance:{0};\nY_distance:{1};", distance_x_mm.ToString(), distance_y_mm.ToString());//数据输出查看

            //遗留问题 所选区域面积的计算
            //double _Area = GetAreafromLines(CMD._selRegionBoundings);//计算面积，鞋带法
            double _Area = GetAreafromNewfloor(doc, SelRegionBoundings);//计算面积，新建楼板事务法
            //double ParkingEfficiency = tar_placeXYZs.Count / _Area;//计算停车效率
            //string str_ParkingEfficiency = @"当前选择区域的面积为" + _Area.ToString();
            //string str_ParkingEfficiency = @"当前选择区域的停车效率为" + ParkingEfficiency.ToString() + @"㎡/车；" + "\n";
            CMD.TestList.Add(parkingplace_count);
            CMD.TestList.Add(str_origin01);
            CMD.TestList.Add(str_origin02);
            //CMD.TestList.Add(str_ParkingEfficiency);
            #endregion

            #region 开展事务组 按照区域内的点，将车位族布置进去
            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "地库强排"))
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    double _parkingPlaceWight = CMD.parkingPlaceWight;
                    double _parkingPlaceHeight = CMD.parkingPlaceHeight;
                    double _Wd = CMD.Wd;
                    double _columnWidthdth = CMD.columnWidth;

                    modifyParking_H_W(doc, parkingType, _parkingPlaceHeight, _parkingPlaceWight);//调整车位的尺寸

                    double tarElvevation = 0.0;
                    Level level_00 = FindLevel(doc, tarElvevation);//寻找目标标高
                    if (level_00 != null)
                    {
                        PlaceParkingPlaces_3D(doc, parkingType, tar_placeXYZs, level_00);
                    }
                    else
                    {
                        string _error_message = "不存在高程为" + tarElvevation.ToString() + "的标高";
                        TaskDialog.Show("error", _error_message);
                    }
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #endregion
        }

        /// <summary>
        /// 选择闭合区域边界线，当前判定条件为，是否为闭合边界
        /// </summary>
        /// <param name="uiapp"></param>
        public void SelectRegionalBoundarys(UIApplication uiapp)
        {
            CMD._selRegionBoundings = new List<Line>();//数据重置
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            LinePickFilter linePickFilter = new LinePickFilter();
            IList<Element> ele_lines = sel.PickElementsByRectangle(linePickFilter, "Select Region Boundings ");
            ICollection<ElementId> _linIds = new List<ElementId>();
            IList<ModelLine> _ModelLines = new List<ModelLine>();
            foreach (Element ele in ele_lines)
            {
                ModelLine _ModelLine = ele as ModelLine;
                _ModelLines.Add(_ModelLine);
                _linIds.Add(ele.Id);
            }
            //首先判断线与线是不是首尾闭合
            if (_LinesIsLoop(_ModelLines))//该函数判断线段端点的重合数量，如果为首尾相连，则函数求出的相交数量为线段端点数量的两倍
            {
                foreach (ModelLine _ModelLine in _ModelLines)
                {
                    Curve _Curve = _ModelLine.GeometryCurve;
                    //_curveloop.Append(_Curve);
                    Line _Line = _Curve as Line;
                    CMD._selRegionBoundings.Add(_Line);
                }

                //CurveLoop _curveloop = new CurveLoop();
                //if (_curveloop.IsOpen())//矩形出现问题
                //{
                //    CMD._selRegionBoundings = new List<Line>();//数据重置
                //    throw new NotImplementedException("已选择边界没有闭合");
                //}
                //else
                //{
                //    sel.SetElementIds(_linIds);
                //}
            }
            else
            {
                CMD._selRegionBoundings = new List<Line>();//数据重置
                throw new NotImplementedException("已选择边界没有闭合");
            }
        }

        //以下为各种method---------------------------------分割线---------------------------------
        /// <summary>
        /// 获取落在目标区域内的车位中心点，需要设定车位排布规则
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <returns></returns>
        public IList<XYZ> GetTarPoints_vertical(XYZ _origin_leftdown_XYZ, IList<Line> _Lines, double distance_x, double distance_y)
        {
            IList<XYZ> tar_placeXYZs = new List<XYZ>();
            double _originX = _origin_leftdown_XYZ.X;
            double _originY = _origin_leftdown_XYZ.Y;
            XYZ _newXYZ = new XYZ(0, 0, 0);

            //计算目标区域所在矩形满排车位的最大值，并进行缓冲值处理
            double _parkingPlaceWight = CMD.parkingPlaceWight;
            double _parkingPlaceHeight = CMD.parkingPlaceHeight;
            double _Wd = CMD.Wd;
            double _columnWidth = CMD.columnWidth;

            int x_count_temp = Convert.ToInt32(distance_x / _parkingPlaceWight + 10);
            int y_count_temp = Convert.ToInt32(distance_y / _parkingPlaceHeight + 10);

            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;

                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }

                    bool _isInLine = isInLine(_newXYZ, _Lines);//第一步，该函数判断点在不在边界线上，在为true，不在为false；
                    bool _isInRgion = isInRegion(_newXYZ, _Lines);//第二步，判断点在区域内部还是外部，在为true，不在为false；
                    if (_isInRgion && !_isInLine)//双重判断，不在线上，也不在选择区域内
                    {
                        tar_placeXYZs.Add(_newXYZ);
                    }
                }
            }
            return tar_placeXYZs;
        }
        /// <summary>
        /// 基于线的数组，进行楼板绘制，继而求出区域面积
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public double GetAreafromNewfloor(Document doc, IList<Line> _Lines)
        {
            double _area = 0;
            CurveArray linesArray = new CurveArray();
            foreach (Line _line in CMD._selRegionBoundings)
            {
                Curve _curve = _line as Curve;
                linesArray.Append(_curve);
            }
            Floor _newFloor = null;
            using (Transaction newFloor = new Transaction(doc))
            {
                newFloor.Start("newFloor");
                _newFloor = doc.Create.NewFloor(linesArray, false);
                newFloor.Commit();
            }

            Parameter _floorArea = _newFloor.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
            _area = _floorArea.AsDouble();
            return _area;
        }
        /// <summary>
        /// Shoelace公式  鞋带法计算面积，需要进行进一步研究
        /// </summary>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public double GetAreafromLines(IList<Line> _Lines)
        {
            IList<XYZ> _XYZs = new List<XYZ>();
            foreach (Line _Line in _Lines)
            {
                _XYZs.Add(_Line.GetEndPoint(0));
                _XYZs.Add(_Line.GetEndPoint(1));
            }
            //列表去重
            IList<XYZ> __XYZs = new List<XYZ>();
            foreach (XYZ _XYZ in _XYZs)
            {
                if (!IsInXYZlist(_XYZ, __XYZs))
                {
                    __XYZs.Add(_XYZ);
                }
            }
            TaskDialog.Show("error", "非重合断点数量为" + __XYZs.Count.ToString());

            //求面积
            int count = __XYZs.Count;
            double area = __XYZs[count - 1].X * __XYZs[0].Y - __XYZs[0].X * __XYZs[count - 1].Y;
            for (int i = 1; i < count; i++)
            {
                int j = i - 1;
                area += __XYZs[j].X * __XYZs[i].Y;
                area -= __XYZs[i].X * __XYZs[j].Y;
            }
            TaskDialog.Show("error", "区域面积为temp：" + area.ToString());

            return area;
        }
        /// <summary>
        /// 与一个列表的数据都不相等
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <param name="_XYZs"></param>
        /// <returns></returns>
        public bool IsInXYZlist(XYZ _XYZ, IList<XYZ> _XYZs)
        {
            bool isIn = false;

            foreach (XYZ __XYZ in _XYZs)
            {
                if (_XYZ.IsAlmostEqualTo(__XYZ, 0.001))
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Lines"></param>
        /// <param name="distance_x"></param>
        /// <param name="distance_y"></param>
        /// <returns></returns>
        public XYZ GetLeftDownXYZfromLines(IList<Line> _Lines, out double distance_x, out double distance_y)
        {
            //不可以用 0 作为首次被比选数据
            Line _Line_0 = _Lines[0];
            double _X_min = _Line_0.GetEndPoint(0).X;
            double _Y_min = _Line_0.GetEndPoint(0).Y;
            double _X_max = _Line_0.GetEndPoint(0).X;
            double _Y_max = _Line_0.GetEndPoint(0).Y;
            distance_x = 0;
            distance_y = 0;

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

            XYZ _XYZ = new XYZ(_X_min, _Y_min, 0.0);
            distance_x = _X_max - _X_min;
            distance_y = _Y_max - _Y_min;

            return _XYZ;
        }
        /// <summary>
        /// 使用射线法，求一个点是不是在区域内
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <param name="_selRegionBoundings"></param>
        /// <returns></returns>
        public bool isInRegion(XYZ _XYZ, IList<Line> _selRegionBoundings)
        {
            bool _isInRegion = false;
            int intersectCount = 0;

            Line _LIneUnbound = Line.CreateBound(_XYZ, new XYZ(_XYZ.X + 100000000, 0, 0));//求一个点的射线，不存在射线，给一个极大值
            foreach (Line _Line in _selRegionBoundings)
            {
                IntersectionResultArray results;
                SetComparisonResult result = _LIneUnbound.Intersect(_Line, out results);
                if (result == SetComparisonResult.Overlap)//判断基准线是否与轴网相交
                {
                    if (results != null)
                    {
                        XYZ _LineendPoint_0 = _Line.GetEndPoint(0);
                        XYZ _LineendPoint_1 = _Line.GetEndPoint(1);
                        if ((_LineendPoint_0.Y < _XYZ.Y && _LineendPoint_1.Y >= _XYZ.Y) || (_LineendPoint_0.Y > _XYZ.Y && _LineendPoint_1.Y <= _XYZ.Y))//判断是不是顶点穿越
                        {
                            intersectCount += results.Size;
                        }
                    }
                }
            }
            //TaskDialog.Show("error", intersectCount.ToString());
            if (intersectCount % 2 != 0)//判断交点的数量是否为奇数或者偶数，奇数为内true，偶数为外false
            {
                _isInRegion = true;
            }
            return _isInRegion;
        }
        /// <summary>
        /// 判断一个点是不是一个列表集合线段上
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <param name="_selRegionBoundings"></param>
        /// <returns></returns>
        public bool isInLine(XYZ _XYZ, IList<Line> _selRegionBoundings)
        {
            bool _isInLien = false;
            foreach (Line _Line in _selRegionBoundings)
            {
                if (_Line.Distance(_XYZ) < 0.001)
                {
                    _isInLien = true;
                    break;
                }
            }
            return _isInLien;
        }
        /// <summary>
        /// 判断所有直线是否首位相连，该函数只能判断出每个线段的端点被重合一次，是不是闭合需要与iscurveloop函数一起使用
        /// </summary>
        /// <param name="_ModelLines"></param>
        /// <returns></returns>
        public bool _LinesIsLoop(IList<ModelLine> _ModelLines)
        {
            int count = _ModelLines.Count * 2;//被判断交点的数量
            int _count = 0;//求出交点的数量
            bool _isLoop = false;
            foreach (ModelLine _ModelLine in _ModelLines)
            {
                Curve _Curve = _ModelLine.GeometryCurve;
                Line _Line = _Curve as Line;
                XYZ _endpoing_0 = _Line.GetEndPoint(0);
                XYZ _endpoing_1 = _Line.GetEndPoint(1);

                foreach (ModelLine __ModelLine in _ModelLines)
                {
                    if (_ModelLine.Id != __ModelLine.Id)
                    {
                        Curve __Curve = __ModelLine.GeometryCurve;
                        Line __Line = __Curve as Line;
                        XYZ __endpoing_0 = __Line.GetEndPoint(0);
                        XYZ __endpoing_1 = __Line.GetEndPoint(1);

                        double tolerance = UnitUtils.Convert(0.001, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);//容差在千分之一内点的就算重合
                        if (_endpoing_0.IsAlmostEqualTo(__endpoing_0, tolerance)
                            || _endpoing_1.IsAlmostEqualTo(__endpoing_0, tolerance)
                            || _endpoing_0.IsAlmostEqualTo(__endpoing_1, tolerance)
                            || _endpoing_1.IsAlmostEqualTo(__endpoing_1, tolerance))
                        {
                            _count += 1;
                        }
                    }
                }
            }
            //TaskDialog.Show("error",_count.ToString());
            if (count == _count)
            {
                _isLoop = true;
            }
            return _isLoop;
        }

        /// <summary>
        /// UI选择元素过滤器
        /// </summary>
        public class LinePickFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Lines));
            }
            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
            }
        }
        /// <summary>
        /// 寻找对应高度数据的标高
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="findElevation"></param>
        /// <returns></returns>
        public Level FindLevel(Document doc, double findElevation)
        {
            Level level = null;
            double _elevation = UnitUtils.Convert(findElevation, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);

            IList<Element> eles = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).WhereElementIsNotElementType().ToElements();
            foreach (Element ele in eles)
            {
                Level temp_level = ele as Level;
                if (temp_level.Elevation == findElevation)
                {
                    level = temp_level;
                    break;
                }
            }
            return level;
        }
        /// <summary>
        /// 创建停车位族实例3D
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="placePoint"></param>
        /// <param name="parkingType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public void PlaceParkingPlaces_3D(Document doc, FamilySymbol parkingType, IList<XYZ> placeXYZs, Level level)
        {
            using (Transaction creatNewGroup = new Transaction(doc))
            {
                creatNewGroup.Start("placeParkingPlace");
                if (!parkingType.IsActive)//判断族类型是否被激活
                {
                    parkingType.Activate();
                }
                foreach (XYZ _xyz in placeXYZs)
                {
                    FamilyInstance parkingPlace = doc.Create.NewFamilyInstance(_xyz, parkingType, level, StructuralType.NonStructural);
                    //System.Windows.Forms.Application.DoEvents();

                }
                creatNewGroup.Commit();
            }
        }
        /// <summary>
        /// 创建停车位族实例2D
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="placePoint"></param>
        /// <param name="parkingType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public FamilyInstance placeParkingPlace_2D(Document doc, XYZ placePoint, FamilySymbol parkingType, View view)
        {
            FamilyInstance familyInstance = null;

            using (Transaction creatNewGroup = new Transaction(doc))
            {
                creatNewGroup.Start("placeParkingPlace");
                if (!parkingType.IsActive)//判断族类型是否被激活
                {
                    parkingType.Activate();
                }
                FamilyInstance parkingPlace = doc.Create.NewFamilyInstance(placePoint, parkingType, view);
                creatNewGroup.Commit();
            }
            return familyInstance;
        }
        /// <summary>
        /// 修改停车位尺寸
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parkingType"></param>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        /// <returns></returns>
        public void modifyParking_H_W(Document doc, FamilySymbol parkingType, double Height, double Width)
        {
            using (Transaction modifyParking_H_W = new Transaction(doc))
            {
                modifyParking_H_W.Start("modifyParking_H_W");
                Parameter DWparametericfloor_length = parkingType.LookupParameter("Height");//查找族类型参数
                Parameter DWparametericfloor_width = parkingType.LookupParameter("Width");
                DWparametericfloor_length.Set(Height);//修改族类型参数
                DWparametericfloor_width.Set(Width);
                modifyParking_H_W.Commit();
            }
        }
        /// <summary>
        /// 通过两层方法(如果当前文档不存在目标name族，则载入族文件)，确定当前文档，存在停车位族
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="FamilyFilePath"></param>
        /// <param name="parkingTypeName"></param>
        /// <returns></returns>
        public FamilySymbol _FindFamilySymbolNeeded(Document doc, string FamilyFilePath, string parkingTypeName)
        {
            FamilySymbol parkingType = null;
            bool symbolFound = FindFamilySymbolNeeded(doc, parkingTypeName, out parkingType);//寻找目标停车位族类型
            if (!symbolFound)
            {
                Family parkFamily = null;
                bool loadFamily = reLoadFamily(doc, FamilyFilePath, out parkFamily);
                ICollection<ElementId> eleIds = parkFamily.GetValidTypes();//目前该函数无效，未查明原因
                if (loadFamily)
                {
                    symbolFound = FindFamilySymbolNeeded(doc, parkingTypeName, out parkingType);
                }
                else
                {
                    throw new NotImplementedException("基础停车位族载入失败。");
                    //TaskDialog.Show("error","基础停车位族载入失败。");
                }
            }
            return parkingType;
        }
        /// <summary>
        /// 通过name进行索引需要的族类型
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="TargetSymbolName"></param>
        /// <param name="targetFamilySymbal"></param>
        /// <returns></returns>
        public bool FindFamilySymbolNeeded(Document doc, string TargetSymbolName, out FamilySymbol targetFamilySymbal)
        {
            ElementFilter parkingCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Parking);
            //ElementFilter parkingCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_GenericAnnotation);
            ElementFilter familySymbolFilter = new ElementClassFilter(typeof(FamilySymbol));
            LogicalAndFilter andFilter = new LogicalAndFilter(parkingCategoryFilter, familySymbolFilter);

            FilteredElementCollector parkingSymbols = new FilteredElementCollector(doc);
            parkingSymbols.WherePasses(andFilter);
            bool symbolFound = false;
            targetFamilySymbal = null;
            foreach (FamilySymbol element in parkingSymbols)
            {
                if (element.Name == TargetSymbolName)
                {
                    symbolFound = true;
                    targetFamilySymbal = element;
                    break;
                }
            }
            return symbolFound;
        }
        /// <summary>
        /// 通过文件路径字符串，将族载入到当前文档
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="FamilyFilePath"></param>
        /// <param name="family"></param>
        /// <returns></returns>
        public bool reLoadFamily(Document doc, string FamilyFilePath, out Family family)
        {
            family = null;
            bool loadSuccess = false;
            using (Transaction loadFamily = new Transaction(doc))
            {
                loadFamily.Start("loadFamily");
                projectFamLoadOption pjflo = new projectFamLoadOption();
                loadSuccess = doc.LoadFamily(FamilyFilePath, pjflo, out family);//经过测试
                if (loadSuccess)
                {
                    foreach (ElementId parkingTypeId in family.GetValidTypes())//该函数无效，获取不出一个family的族类型；
                    {
                        FamilySymbol parkingTypeName = doc.GetElement(parkingTypeId) as FamilySymbol;
                        if (parkingTypeName != null)
                        {
                            //CMD.TestList.Add(parkingTypeName.Name);
                        }
                    }
                }
                loadFamily.Commit();
            }
            return loadSuccess;
        }
        /// <summary>
        /// 载入族提示是否要覆盖族参数
        /// </summary>
        public class projectFamLoadOption : IFamilyLoadOptions
        {
            bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                throw new NotImplementedException();
            }

            bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Project;
                overwriteParameterValues = true;
                throw new NotImplementedException();
            }
        }

    }  // public class RequestHandler : IExternalEventHandler
} // namespace

