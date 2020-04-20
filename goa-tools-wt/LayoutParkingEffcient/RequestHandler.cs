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
using ClipperLib;
using wt_Common;

namespace LayoutParkingEffcient
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

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
                    case RequestId.SelGarageBoundary:
                        {
                            SelGarageBoundary(uiapp);
                            break;
                        }
                    case RequestId.CheckLineStyle:
                        {
                            CheckLineStyles(uiapp);
                            break;
                        }
                    case RequestId.CheckpolygonClosed:
                        {
                            CheckpolygonClosed(uiapp);
                            break;
                        }
                    case RequestId.CheckInGroupLineStyleIsSame:
                        {
                            CheckInGroupLineStyleIsSame(uiapp);
                            break;
                        }
                    case RequestId.TestOthers:
                        {
                            TestOthers(uiapp);
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

        //外部事件方法建立————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        public void TestOthers(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            double _ShortCurveTolerance = app.ShortCurveTolerance;

            _Methods.TaskDialogShowMessage(_ShortCurveTolerance.ToString());

            _ShortCurveTolerance = Methods.FeetToMilliMeter(_ShortCurveTolerance);

            _Methods.TaskDialogShowMessage(_ShortCurveTolerance.ToString());
        }
        /// <summary>
        /// 检测地库强排所需线型是否必备
        /// </summary>
        /// <param name="uiapp"></param>
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
                && AllLineGraphicsStylNames.Contains("地库_停车区域边界线")
                && AllLineGraphicsStylNames.Contains("地库_主车道中心线"))
            {
                TaskDialog.Show("正确", "地库强排所需线型，均已存在当前文档中");
            }
            else//逐个判断，线型是否存在
            {
                if (!AllLineGraphicsStylNames.Contains("地库_场地控制红线"))//线型不存在，便创建
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_场地控制红线", 2, new Color(255, 0, 0));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_障碍物边界线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_障碍物边界线", 2, new Color(0, 128, 255));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_停车区域边界线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_停车区域边界线", 2, new Color(255, 127, 0));
                }
                if (!AllLineGraphicsStylNames.Contains("地库_主车道中心线"))
                {
                    Category _category = _Methods.CreatLineStyle(doc, "地库_主车道中心线", 2, new Color(255, 0, 255));
                }
                //进行二次判断
                AllLineGraphicsStylNames = _Methods.getAllLineGraphicsStylNames(doc);
                if (AllLineGraphicsStylNames.Contains("地库_场地控制红线")
                && AllLineGraphicsStylNames.Contains("地库_障碍物边界线")
                && AllLineGraphicsStylNames.Contains("地库_停车区域边界线")
                && AllLineGraphicsStylNames.Contains("地库_主车道中心线"))
                {
                    TaskDialog.Show("正确", "地库强排所需线型，均已存在当前文档中");
                }
                else
                {
                    throw new NotImplementedException("地库必备线型，自动创建失败，请手动创建");
                }
            }
        }
        /// <summary>
        /// 检查组内线段是否闭合
        /// </summary>
        /// <param name="uiapp"></param>
        public void CheckpolygonClosed(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion
            List<Element> sel_groups = sel.PickElementsByRectangle(new SelPickFilter_DetailGroups(), "Select Region Boundings ").ToList();
            foreach (Element _ele in sel_groups)
            {
                if (_ele is Group)
                {
                    Group _group = _ele as Group;
                    List<Line> _lines = _Methods.GetLinesFromLineGroup(_group);
                    bool isLoop = _Methods._LinesIsLoop(_lines);
                    if (isLoop)
                    {
                        _Methods.TaskDialogShowMessage("闭合");
                    }
                    else
                    {
                        _Methods.TaskDialogShowMessage("未闭合");
                    }
                }
            }
        }
        /// <summary>
        /// 检查组内线段样式是否统一
        /// </summary>
        /// <param name="uiapp"></param>
        public void CheckInGroupLineStyleIsSame(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            List<Element> sel_groups = sel.PickElementsByRectangle(new SelPickFilter_DetailGroups(), "Select Region Boundings ").ToList();
            foreach (Element _ele in sel_groups)
            {
                if (_ele is Group)
                {
                    Group _group = _ele as Group;
                    string _GraphicsStyle_frist_name;
                    bool iSame = IsSameAllDetailCurveLineStyleFromGroup(doc, _group, out _GraphicsStyle_frist_name);
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
        /// 选择地库设计区域 框选红线内外所有物体
        /// </summary>
        /// <param name="uiapp"></param>
        public void SelGarageBoundary(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            # region 第一步 读取基本数据 红线和障碍物为组 主通道为独立详图曲线

            List<Element> sel_Eles = sel.PickElementsByRectangle("Select Region Boundings ").ToList();//UI交互

            // 如果存在车位，则强行重置删除
            using (Transaction _trans_dele = new Transaction(doc, "_trans_dele"))
            {
                ICollection<ElementId> _pplace = new List<ElementId>();
                foreach (Element _ele in sel_Eles)
                {
                    if (_ele.Name == "停车位_详图线")
                    {
                        _pplace.Add(_ele.Id);
                    }
                }
                _trans_dele.Start();
                doc.Delete(_pplace);
                _trans_dele.Commit();
            }

            List<Group> RedLine_Group = new List<Group>();
            List<Group> RedLine_Offset_Group = new List<Group>();
            List<Group> Obstacle_Group = new List<Group>();
            List<Element> MianRoad_DetailCurve = new List<Element>();

            foreach (Element _ele in sel_Eles)
            {
                if (_ele is Group)//筛选目标Group
                {
                    Group _group = _ele as Group;

                    //判断组内线条是否闭合
                    List<Line> _lines = _Methods.GetLinesFromLineGroup(_group);
                    bool isLoop = _Methods._LinesIsLoop(_lines);
                    if (!isLoop)
                    {
                        throw new NotImplementedException("部分详图组内，线条未闭合");
                    }

                    ICollection<ElementId> _EleIds = _group.GetMemberIds();

                    //判断组内线条样式是否相同
                    string _GraphicsStyle_frist_name;//得到Group内线样式
                    bool iSame = IsSameAllDetailCurveLineStyleFromGroup(doc, _group, out _GraphicsStyle_frist_name);
                    if (!iSame)//需要判断组内线样式是否一致
                    {
                        throw new NotImplementedException("部分详图组内，线样式不统一，请检查");
                    }
                    if (_GraphicsStyle_frist_name == "地库_场地控制红线")
                    {
                        RedLine_Group.Add(_group);
                    }
                    else if (_GraphicsStyle_frist_name == "地库_停车区域边界线")
                    {
                        RedLine_Offset_Group.Add(_group);
                    }
                    else if (_GraphicsStyle_frist_name == "地库_障碍物边界线")
                    {
                        Obstacle_Group.Add(_group);
                    }
                }
                else if (_ele is DetailCurve)//筛选目标 主车道 中心线
                {
                    DetailCurve _DetailCurve = _ele as DetailCurve;
                    GraphicsStyle _GraphicsStyle = _DetailCurve.LineStyle as GraphicsStyle;
                    if (_GraphicsStyle.Name == "地库_主车道中心线")
                    {
                        MianRoad_DetailCurve.Add(_ele);
                    }
                }
            }
            if (RedLine_Group.Count == 0)
            {
                throw new NotImplementedException("选择元素中，线型样式包含'地库_控制红线'的组不存在");
            }
            else if (RedLine_Group.Count > 1)
            {
                throw new NotImplementedException("选择元素中，线型样式包含'地库_控制红线'的组，不唯一");
            }
            #endregion

            #region 第二步 数据处理 提取线列表
            List<Line> RedLine_Line = new List<Line>();//一个组 控制红线 该处需要注意
            List<Line> RedLine_Offset_Line = new List<Line>();//如果，场地中同时存在 地库_场地控制红线 地库_停车区域边界线， 则直接使用 地库_停车区域边界线，不再对地库_场地控制红线进行退距偏移处理
            List<List<Line>> Obstacle_Line = new List<List<Line>>();//一个组 地库障碍物
            List<Line> _MianRoad_Line = new List<Line>();//一个列表 地库主车道

            foreach (Group _gorup in RedLine_Group)//红线可用于求面积 和 退距偏移
            {
                List<Line> temp_RedLine_Line = _Methods.GetLinesFromLineGroup(_gorup);//曲线分段过多，导致性能降低
                RedLine_Line = Methods.SortLinesContiguous(temp_RedLine_Line);//线段需要sort，进行排序
            }

            if (RedLine_Offset_Group.Count == 1)
            {
                foreach (Group _gorup in RedLine_Offset_Group)
                {
                    List<Line> temp_RedLine_Offset_Line = _Methods.GetLinesFromLineGroup(_gorup);//曲线分段过多，导致性能降低
                    RedLine_Offset_Line = Methods.SortLinesContiguous(temp_RedLine_Offset_Line);//线段需要sort，进行排序
                }
            }

            foreach (Group _gorup in Obstacle_Group)
            {
                List<Line> _temp_Obstacle_Line = _Methods.GetLinesFromLineGroup(_gorup);
                List<Line> __temp_Obstacle_Line = Methods.SortLinesContiguous(_temp_Obstacle_Line);
                Obstacle_Line.Add(__temp_Obstacle_Line);
            }
            _MianRoad_Line = _Methods.ConvetDocLinesTolines(MianRoad_DetailCurve);
            #endregion

            #region 第三步 处理红线退距问题 将退居线绘制成组 以便设计师进行操作 clipper内置偏移，定位为内外同时偏移，会有两个点集结果 索引值 0  为 外圈偏移结果 1 为 内圈偏移结果

            Paths Paths_redline_offest = new Paths();
            if (RedLine_Offset_Group.Count == 0)//如果，场地中同时存在 地库_场地控制红线 地库_停车区域边界线， 则直接使用 地库_停车区域边界线，不再对地库_场地控制红线进行退距偏移处理
            {
                Path_xyz Path_xyz_RedLine = _Methods.GetUniqueXYZFromLines(RedLine_Line);

                Paths_redline_offest = clipper_methods._GetOffsetPonts_clipper_Path(Path_xyz_RedLine, CMD.redline_offset_distance);

                Path_xyz _Path_xyz = clipper_methods.PathToPath_xyz(Paths_redline_offest[1]);//索引值 0  为 外圈偏移结果 1 为 内圈偏移结果

                double toleranceDistance = Methods.MilliMeterToFeet(12000);//此处需要注意，Methods.SortLinesContiguous()，函数，当前测试表现为，需要线段的长度满足在12000mm之上
                Path_xyz _unique_Path_xyz = _Methods.Pointlistdeduplication(_Path_xyz, toleranceDistance); //需要对点进行去重 解决invalid curve loop的问题；可能原因，sort排线段函数，对线段有最小距离要求

                List<Line> redline_offset_in = _Methods.GetClosedLinesfromPoints(_unique_Path_xyz);

                //将偏移线创建出来，成Group
                List<ElementId> NewDetailCurveIds = new List<ElementId>();
                using (Transaction CreatNewDetailLine = new Transaction(doc))
                {
                    CreatNewDetailLine.Start("CreatNewDetailLine");
                    foreach (Line _line in redline_offset_in)
                    {
                        CMD.TestList.Add(Methods.FeetToMilliMeter(_line.Length).ToString());
                        DetailCurve _DetailCurve = doc.Create.NewDetailCurve(acvtiView, _line);
                        CurveElement _CurveElement = _DetailCurve as CurveElement;
                        _Methods.SetLineStyle(doc, "地库_停车区域边界线", _CurveElement);

                        NewDetailCurveIds.Add(_DetailCurve.Id);
                    }
                    CreatNewDetailLine.Commit();
                }
                using (Transaction CreatNewDetailLineGroup = new Transaction(doc))
                {
                    CreatNewDetailLineGroup.Start("CreatNewDetailLineGroup");
                    Group _group = doc.Create.NewGroup(NewDetailCurveIds);
                    CreatNewDetailLineGroup.Commit();
                }
            }
            else if (RedLine_Offset_Group.Count == 1)//如果，场地中同时存在 地库_场地控制红线 地库_停车区域边界线， 则直接使用 地库_停车区域边界线，不再对地库_场地控制红线进行退距偏移处理
            {
                Path_xyz _Path_xyz = _Methods.GetUniqueXYZFromLines(RedLine_Offset_Line);
                Paths_xyz _Paths_xyz = new Paths_xyz() { _Path_xyz, _Path_xyz };//考虑到 clipper 偏移，会有 内-1 外-0 的区别，因此，这里直接添加两个相同元素
                Paths_redline_offest = clipper_methods.Paths_xyzToPaths(_Paths_xyz);
            }
            #endregion

            #region 第四步 处理住宅、地库出入口边界问题

            Paths_xyz _Paths_xyz_Obstacle = new Paths_xyz();//基础障碍物范围
            foreach (List<Line> _line_list in Obstacle_Line)
            {
                _Paths_xyz_Obstacle.Add(_Methods.GetUniqueXYZFromLines(_line_list));//添加各个障碍物边界的点集
            }
            Paths _Paths_Obstacle = clipper_methods.Paths_xyzToPaths(_Paths_xyz_Obstacle);

            #endregion

            #region 第五布 处理主通道中心线 得到主通道占用空间

            Paths _WdMainOffset = new Paths();//拿到所有的主车道偏移后的围合点，需要将围合区域做并集
            foreach (Line _line in _MianRoad_Line)
            {
                _WdMainOffset.Add(clipper_methods._GetOffsetPonts_clipper_line(_line, CMD.Wd_main / 2));//添加每根车道中心线偏移后的矩形点集
            }

            Paths Main_road_region = new Paths();//地库主车道占据空间
            Clipper c_temp = new Clipper();//开展 所有线段的偏移矩形点击进行 clipper 合并
            c_temp.AddPaths(_WdMainOffset, PolyType.ptSubject, true);
            c_temp.AddPath(_WdMainOffset[0], PolyType.ptClip, true);
            c_temp.Execute(ClipType.ctUnion, Main_road_region, PolyFillType.pftNonZero, PolyFillType.pftNonZero);//需要把每根主车道偏移出的空间，进行合并

            #endregion

            #region 第六步 基于 被裁剪区域（红退距后区域）和 裁剪区域 （住宅结构边界 车库出入口）进行 clipper 裁剪

            //红线退距
            Paths _redLineRegion = new Paths();
            _redLineRegion.Add(Paths_redline_offest[1]); //clipper内置偏移，定位为内外同时偏移，会有两个点集结果
            //障碍物
            Paths _obstalRegion = new Paths();
            //_obstalRegion.AddRange(_Paths_Obstacle);
            _obstalRegion.AddRange(Main_road_region);

            // 开展裁剪任务
            Paths _canPlacedRegion = new Paths();//得到可停车区域
            Clipper _end_c = new Clipper();
            _end_c.AddPaths(_redLineRegion, PolyType.ptSubject, true);
            _end_c.AddPaths(_obstalRegion, PolyType.ptClip, true);
            _end_c.Execute(ClipType.ctDifference, _canPlacedRegion);

            #endregion

            #region 第七步 计算车位中心点 放置位置 同时开启事务 放置车位

            int ParkingCounts = 0;//总车位数量
            foreach (Path _path in _canPlacedRegion)//
            {
                // 1 对 单个可停车区域 进行处理
                Path_xyz _Path_xyz = clipper_methods.PathToPath_xyz(_path);//将clipper 结果 转换到 Revit
                Path_xyz _unique_Path_xyz = _Methods.Pointlistdeduplication(_Path_xyz); //需要对点进行去重 防止后台无法生成曲线 而报错 该处需要注意 Revit2020版本中 曲线的最小容差为 0.00256026455729167 Feet 0.7803686370625 MilliMeter
                List<Line> _single_canPlacedRegion = _Methods.GetClosedLinesfromPoints(_unique_Path_xyz);//将点集 转换为 闭合多段线边界
                _single_canPlacedRegion = Methods.SortLinesContiguous(_single_canPlacedRegion);//将点集 转换为 闭合多段线边界

                //将裁减后的区域 显示出来
                //_Methods.ShowTempGometry(doc, _single_canPlacedRegion);

                ParkingAlgorithm _parkingAlgorithm = new ParkingAlgorithm();

                List<XYZ> Max_tar_placeXYZs = _parkingAlgorithm.LayoutParking(_single_canPlacedRegion, Obstacle_Line);//此处需要添加 障碍物过滤，源于 clipper裁剪的结果中 包含空洞的边界,该处偷懒，直接过滤了所有障碍物边界，

                ParkingCounts += _parkingAlgorithm.Max_XYZ_count;

                LayoutParking(doc, Max_tar_placeXYZs); //开启放置事务
                //if (true)
                //{
                //    break;
                //}
            }

            #endregion

            #region 第八步 数据统计

            string parkingplace_count = @"当前处于选择范围内的停车位数量为" + ParkingCounts.ToString() + @"个；";

            double _Area = _Methods.GetAreafromLines(RedLine_Line);//计算面积，鞋带法 对数据需要进行小数取位数
            _Area = UnitUtils.Convert(_Area, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);

            double _ParkingEfficiency = _Area / ParkingCounts;//计算停车效率
            _Area = _Methods.TakeNumberAfterDecimal(_Area, 2);
            _ParkingEfficiency = _Methods.TakeNumberAfterDecimal(_ParkingEfficiency, 2);


            string str_Area = @"当前地库红线内面积为" + _Area.ToString() + @"㎡；";
            string str_ParkingEfficiency = @"当前选择区域的停车效率为" + _ParkingEfficiency.ToString() + @"㎡/车；" + "\n";

            CMD.TestList.Add(str_Area);
            CMD.TestList.Add(parkingplace_count);
            CMD.TestList.Add(str_ParkingEfficiency);
            #endregion

            TaskDialog.Show("error", "测试成功");
        }

        //以下为各种method—————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        /// <summary>
        /// 排布车位
        /// </summary>
        /// <param name="uiapp"></param>
        public void LayoutParking(Document doc, List<XYZ> tar_placeXYZs)//全部转移到clipper中进行处理
        {
            View acvtiView = doc.ActiveView;
            FamilySymbol parkingType = null;

            string parkingTypeName = "停车位_详图线";//设定目标停车位族类型名字
            string FamilyFilePath = @"W:\BIM_ARCH\03.插件\地库强排\族\停车位_详图线.rfa";//停车位族文件所在位置
            parkingType = _FindFamilySymbolNeeded(doc, FamilyFilePath, parkingTypeName);

            #region 开展事务组 按照区域内的点，将车位族布置进去
            using (TransactionGroup transGroupCreateDwellingses = new TransactionGroup(doc, "地库强排"))
            {
                if (transGroupCreateDwellingses.Start() == TransactionStatus.Started)//开启事务组
                {
                    modifyParking_H_W(doc, parkingType, CMD.parkingPlaceHeight, CMD.parkingPlaceWight);//调整车位的尺寸

                    List<FamilyInstance> parkingplaceInstances = new List<FamilyInstance>();
                    if (CMD.layoutMethod == "垂直式_0°")
                    {
                        parkingplaceInstances = PlaceParkingPlaces_2D(doc, parkingType, tar_placeXYZs, acvtiView);//开启事务 放置车位
                    }
                    else if (CMD.layoutMethod == "垂直式_90°")
                    {
                        parkingplaceInstances = PlaceParkingPlaces_2D(doc, parkingType, tar_placeXYZs, acvtiView);
                        RotateFamilyInstances(doc, parkingplaceInstances, Math.PI / 2);//对所有车位族实例进行旋转
                    }
                    else
                    {
                        TaskDialog.Show("error", "未选择停车方式");
                    }
                    transGroupCreateDwellingses.Assimilate();
                }
                else
                {
                    transGroupCreateDwellingses.RollBack();
                }
            }
            #endregion
        } /// <summary>
          /// 判断组内线样式是否统一
          /// </summary>
          /// <param name="doc"></param>
          /// <param name="_group"></param>
          /// <param name="_GraphicsStyle_frist_name"></param>
          /// <returns></returns>
        public bool IsSameAllDetailCurveLineStyleFromGroup(Document doc, Group _group, out string _GraphicsStyle_frist_name)
        {
            bool iSmae = false;
            ICollection<ElementId> _EleIds = _group.GetMemberIds();
            int _count = _EleIds.Count;
            int __count = 0;

            DetailCurve _DetailCurve_frist = doc.GetElement(_EleIds.First()) as DetailCurve;//提取组内首根线 确定组内线样式
            GraphicsStyle _GraphicsStyle_frist = _DetailCurve_frist.LineStyle as GraphicsStyle;
            _GraphicsStyle_frist_name = _GraphicsStyle_frist.Name;
            foreach (ElementId _eleId in _EleIds)
            {
                DetailCurve _DetailCurve = doc.GetElement(_eleId) as DetailCurve;
                GraphicsStyle _GraphicsStyle = _DetailCurve.LineStyle as GraphicsStyle;
                if (_GraphicsStyle_frist_name == _GraphicsStyle.Name)
                {
                    __count += 1;
                }
            }
            if (_count == __count)
            {
                iSmae = true;
            }
            return iSmae;
        }
        /// <summary>
        /// 旋转族实例
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parkingplaceInstances"></param>
        /// <param name="_angle"></param>
        public void RotateFamilyInstances(Document doc, List<FamilyInstance> parkingplaceInstances, double _angle)
        {
            using (Transaction _rotateTrans = new Transaction(doc))
            {
                _rotateTrans.Start("_rotateTrans");
                foreach (FamilyInstance fs in parkingplaceInstances)
                {
                    LocationPoint _point = fs.Location as LocationPoint;
                    if (_point != null)
                    {
                        XYZ aa = _point.Point;
                        XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                        Line _axis = Line.CreateBound(aa, cc);
                        _point.Rotate(_axis, _angle);
                    }
                }
                _rotateTrans.Commit();
            }
        }
        /// <summary>
        /// 基于线的数组，进行楼板绘制，继而求出区域面积
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        public double GetAreafromNewfloor(Document doc, List<Line> _Lines)
        {
            double _area = 0;
            CurveArray linesArray = new CurveArray();
            foreach (Line _line in _Lines)
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
        /// 寻找对应高度数据的标高
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="findElevation"></param>
        /// <returns></returns>
        public Level FindLevel(Document doc, double findElevation)
        {
            Level level = null;
            double _elevation = UnitUtils.Convert(findElevation, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);

            List<Element> eles = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).WhereElementIsNotElementType().ToElements().ToList();
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

        #region 调整族 类型参数 ， 以及载入处理
        /// <summary>
        /// 创建停车位族实例2D
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="placePoint"></param>
        /// <param name="parkingType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<FamilyInstance> PlaceParkingPlaces_2D(Document doc, FamilySymbol parkingType, List<XYZ> placeXYZs, View activeview)
        {
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
            using (Transaction creatNewGroup = new Transaction(doc))
            {
                creatNewGroup.Start("placeParkingPlace");
                if (!parkingType.IsActive)//判断族类型是否被激活
                {
                    parkingType.Activate();
                }
                foreach (XYZ _xyz in placeXYZs)
                {
                    FamilyInstance parkingPlace = doc.Create.NewFamilyInstance(_xyz, parkingType, activeview);
                    //System.Windows.Forms.Application.DoEvents();
                    familyInstances.Add(parkingPlace);
                }
                creatNewGroup.Commit();
            }
            return familyInstances;
        }
        /// <summary>
        /// 创建停车位族实例3D
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="placePoint"></param>
        /// <param name="parkingType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<FamilyInstance> PlaceParkingPlaces_3D(Document doc, FamilySymbol parkingType, List<XYZ> placeXYZs, Level level)
        {
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
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
                    familyInstances.Add(parkingPlace);
                }
                creatNewGroup.Commit();
            }
            return familyInstances;
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
        #endregion

        #region 判断 停车位族 是否存在于 当前文档中，不存在，则载入
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
            ElementFilter parkingCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents);
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
        #endregion
    }  // public class RequestHandler : IExternalEventHandler
} // namespace

