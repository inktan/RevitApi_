using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using PubFuncWt;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
//using goa.Common;


namespace BSMT_PpLayout
{
    class TowerDistanceCheck : RequestMethod
    {
        public TowerDistanceCheck(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            "塔楼距离检查，该功能正在优化，待释放".TaskDialogErrorMessage();
            return;
            #region 手动

            if (GlobalData.Instance.isAutoTowerDistanceCheck ==false)
            {
            login:
                List<FilledRegion> filledRegions = new List<FilledRegion>();
                FilledRegion filledRegion01 = doc.GetElement(sel.PickObject(ObjectType.Element ,new SelPickFilter_FilledRegion(),"请选择详图填充区域")) as FilledRegion;
                FilledRegion filledRegion02 = doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion(), "请选择详图填充区域")) as FilledRegion;

                filledRegions.Add(filledRegion01);
                filledRegions.Add(filledRegion02);

                if (filledRegions.Count !=2)
                {
                    string errorMessage = "当前选择详图区域不足两个，请重新进行选择。";
                    errorMessage.TaskDialogErrorMessage();
                    goto login;
                }
                GetDimensions(doc, filledRegions);
                return;
            }

            #endregion

            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            #region 对单个视图层级进行车位强排计算

            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                View nowView = elemsViewLevel.View;
                elemsViewLevel.DelUnUsefulEles();// 清除未锁定 车位 族实例

                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;

                    // 提取所有的塔楼区域
                    //List<BoundO> filledRegions =
                    //    bsmt.InBoundEleFRes.Where(p=>p.EleProperty==EleProperty.ResidenStruRegion || p.EleProperty == EleProperty.ResidenStruRegionDepth).Select(p=>p.FilledRegion).ToList();
                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();
                    // 开展计算
                    // 删除相关尺寸标注后 进行重置
                    GetDimensions(doc, new List<FilledRegion>());
                }
            }
            #endregion

            //throw new NotImplementedException();
        }
        /// <summary>
        /// 对多个塔楼的相邻对边进行标注计算
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filledRegions"></param>
        internal void GetDimensions(Document doc, List<FilledRegion> filledRegions)
        {
            // 找到合适的尺寸标注类型
            string unUsefulDimensionType = "线性尺寸标注样式";
            string usefulDimensionType = "地库强排TowerDistanceCheck";
            IEnumerable<DimensionType> dimensionTypes = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).WhereElementIsElementType().Cast<DimensionType>().Where(p => p.FamilyName == unUsefulDimensionType && p.Name != unUsefulDimensionType);
            DimensionType dimensionType = null;

            foreach (var item in dimensionTypes)
            {
                if(item.Name.Contains("地库强排TowerDistanceCheck"))
                {
                    dimensionType = item;
                    break;
                }
            }

            if(dimensionType == null)
            {
                using (Transaction dupli = new Transaction(doc))
                {
                    dupli.Start("Duplicate");
                    dimensionType = dimensionTypes.First().Duplicate(usefulDimensionType) as DimensionType;

                    Parameter readRule = dimensionType.get_Parameter(BuiltInParameter.DIM_STYLE_READ_CONVENTION);
                    readRule.Set(0);

                    Parameter textSize = dimensionType.get_Parameter(BuiltInParameter.TEXT_SIZE);
                    textSize.Set(5.0.MilliMeterToFeet());

                    dupli.Commit();
                }
            }

            int count = filledRegions.Count;
            //for (int i = 0; i < count; i++)
            for (int i = 0; i < count; i++)
            {
                List<Edge> twoEdges01 = FindTwoEdges(filledRegions[i], doc);
                Edge edge01 = twoEdges01[0];
                Edge edge02 = twoEdges01[1];
                Line line01 = edge01.AsCurve() as Line;
                Line line02 = edge02.AsCurve() as Line;
                
                //【】假定南北向关系 求出塔楼 一 的两个边界平行关系的长边
                Segment2d segment2d01 = line01.ToSegment2d();
                Segment2d segment2d02 = line02.ToSegment2d();

                for (int j = 0; j < count; j++)
                {
                    if (j > i)
                    {
                        List<Edge> twoEdges02 = FindTwoEdges(filledRegions[j], doc);
                        Edge edge03 = twoEdges02[0];
                        Edge edge04 = twoEdges02[1];
                        Line line03 = edge03.AsCurve() as Line;
                        Line line04 = edge04.AsCurve() as Line;

                        //【】求出塔楼 二 的两个边界平行关系的长边
                        Segment2d segment2d03 = line03.ToSegment2d();
                        Segment2d segment2d04 = line04.ToSegment2d();

                        //【】首先判断两个塔楼是否平行
                        if (!Math.Abs(segment2d01.Direction.Dot(segment2d03.Direction)).EqualPrecision(1))
                            continue;

                        //【】假定南北向关系：塔楼 一 与塔楼 二 需要满足上下面对面关系
                        //【】求出所有垂线 8条 
                        Segment2d pedalLine01 = segment2d01.P0.PedalLine(segment2d03.ToLine2d());
                        Segment2d pedalLine02 = segment2d01.P0.PedalLine(segment2d04.ToLine2d());

                        Segment2d pedalLine03 = segment2d02.P0.PedalLine(segment2d03.ToLine2d());
                        Segment2d pedalLine04 = segment2d02.P0.PedalLine(segment2d04.ToLine2d());

                        Segment2d pedalLine05 = segment2d03.P0.PedalLine(segment2d01.ToLine2d());
                        Segment2d pedalLine06 = segment2d03.P0.PedalLine(segment2d02.ToLine2d());

                        Segment2d pedalLine07 = segment2d04.P0.PedalLine(segment2d01.ToLine2d());
                        Segment2d pedalLine08 = segment2d04.P0.PedalLine(segment2d02.ToLine2d());

                        //List<Line> _lines = new List<Line>() { pedalLine01.ToLine(), pedalLine02.ToLine(), pedalLine03.ToLine(), pedalLine04.ToLine(), pedalLine05.ToLine(), pedalLine06.ToLine(), pedalLine07.ToLine(), pedalLine08.ToLine() };
                        //double _ele = doc.ActiveView.GenLevel.ProjectElevation;
                        //XYZ _xYZ = new XYZ(0, 0, _ele);
                        //doc.CreateDirectShapeWithNewTransaction(_lines);


                        //【】基于垂线方向的叉积 判断当前两个塔楼呈现 平行上下错位 关系 
                        double tempDouble = pedalLine01.Direction.Dot(pedalLine02.Direction);
                        if (!pedalLine01.Direction.Dot(pedalLine02.Direction).EqualPrecision(1, 0.001))
                            continue;
                         tempDouble = pedalLine03.Direction.Dot(pedalLine04.Direction);
                        if (!pedalLine03.Direction.Dot(pedalLine04.Direction).EqualPrecision(1, 0.001))
                            continue;
                         tempDouble = pedalLine05.Direction.Dot(pedalLine06.Direction);
                        if (!pedalLine05.Direction.Dot(pedalLine06.Direction).EqualPrecision(1, 0.001))
                            continue;
                         tempDouble = pedalLine07.Direction.Dot(pedalLine08.Direction);
                        if (!pedalLine07.Direction.Dot(pedalLine08.Direction).EqualPrecision(1, 0.001))
                            continue;

                        //【】判断左右错位关系
                        double _distance = pedalLine01.P0.CalcPedalDistanceToLine(pedalLine05.ToLine2d());
                        if(_distance>60000.0.MilliMeterToFeet())
                            continue;

                        //基于最短垂线数据 求出对应的两个edge
                        List<Segment2d> segment2ds = new List<Segment2d>() { pedalLine01, pedalLine02, pedalLine03, pedalLine04, pedalLine05, pedalLine06, pedalLine07, pedalLine08 };
                        Segment2d tarSegment2d = segment2ds.OrderBy(p => p.Length).First();

                        // 手动测试，不设置距离限制
                        if (GlobalData.Instance.isAutoTowerDistanceCheck == true)
                        {
                            if (tarSegment2d.Length > 120000.0.MilliMeterToFeet() || tarSegment2d.Length < 1.0.MilliMeterToFeet())
                                continue;
                        }

                        List<Edge> edges = new List<Edge>() { edge01, edge02, edge03, edge04 };
                        int _count = edges.Count;
                        for (int _i = 0; _i < _count; _i++)
                        {
                            bool isBreak = false;
                            for (int _j = 0; _j < _count; _j++)
                            {
                                if (_j > _i)
                                {
                                    Edge edge_0 = edges[_i];
                                    Edge edge_1 = edges[_j];

                                    Segment2d segment2D_00 = (edge_0.AsCurve() as Line).ToSegment2d();
                                    Segment2d segment2D_01 = (edge_1.AsCurve() as Line).ToSegment2d();
                                    double distance = segment2D_00.P0.CalcPedalDistanceToLine(segment2D_01.ToLine2d());

                                    if (Math.Abs(distance - tarSegment2d.Length).EqualZreo())
                                    {
                                        Dimension dimension = GetDimension(edge_0, edge_1, doc, dimensionType);
                                        isBreak = true;
                                        break;
                                    }
                                }
                            }
                            if (isBreak) break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取塔楼区域的两较长对边
        /// </summary>
        /// <returns></returns>
        internal List<Edge> FindTwoEdges(FilledRegion _filledRegion01, Document doc)
        {
            List<Edge> edges = _filledRegion01.Edges(doc.ActiveView);
            //【】找到最长的Edge
            edges = edges.OrderBy(p => p.AsCurve().Length).ToList();
            Edge longestEdge01 = edges.Last();
            //Line longestLine01 = longestEdge01.AsCurve() as Line;

            //【】找到与最长Edge线方向平行，且距离最远的对边
            edges = edges.GetRange(0, edges.Count - 1);
            // 求平行边
            List<Edge> edgesParallel = new List<Edge>();
            foreach (Edge edge in edges)
            {
                double dotResult = (edge.AsCurve() as Line).Direction.DotProduct((longestEdge01.AsCurve() as Line).Direction);
                if (Math.Abs(dotResult).EqualPrecision(1,0.001))
                {
                    edgesParallel.Add(edge);
                }
            }
            // 求距离最远边
            Segment2d segment2d = (longestEdge01.AsCurve() as Line).ToSegment2d();
            Edge longestEdge02 = edgesParallel.OrderBy(p => p.AsCurve().GetEndPoint(0).ToVector2d().CalcPedalDistanceToLine(segment2d.ToLine2d())).Last();
            //Line longestLine02 = longestEdge02.AsCurve() as Line;
            return new List<Edge>() { longestEdge01, longestEdge02 };
        }

        internal Dimension GetDimension(Edge longestEdge01, Edge longestEdge02, Document doc, DimensionType dimensionType)
        {
            //【】获取尺寸标注参照
            ReferenceArray referenceArray = new ReferenceArray();
            referenceArray.Append(longestEdge01.Reference);
            referenceArray.Append(longestEdge02.Reference);

            //【】获取参照所在矩形中心线
            Line dimensionLine = FindMindLine(longestEdge01.AsCurve() as Line, longestEdge02.AsCurve() as Line);

            #region 需要对距离进行停车可行性分析

            double L = dimensionLine.Length;// 楼间距
            double b = GlobalData.Instance.pSHeight_num;// 车位长度
            double w = GlobalData.Instance.Wd_pri_num;// 该处使用的是主车道宽度

            double doubleOverride = L / 1000;
            string valueOverride ="楼间距为 " + doubleOverride.FeetToMilliMeter().NumDecimal().ToString() + "米";
            string _Above = "";// 上部
            TowerDistanceJudgment towerDistanceJudgment = TowerDistanceJudgment.None;
            //string _Above = towerDistanceJudgment.GetEnumDescriptionFromEnum();// 上部
            int index = 0;
            if (w <= L && L < b + 2 * w)
            {
                towerDistanceJudgment = TowerDistanceJudgment.First;
                index = towerDistanceJudgment.GetIntFromEnum();
                _Above = towerDistanceJudgment.GetEnumDescriptionFromEnum();// 上部
            }
            else if (b + 2 * w <= L && L < 2 * b + 2 * w)
            {
                towerDistanceJudgment = TowerDistanceJudgment.Second;
                index = towerDistanceJudgment.GetIntFromEnum();
                _Above = towerDistanceJudgment.GetEnumDescriptionFromEnum();// 上部
            }
            else if (2 * b + 2 * w <= L && L < (2 * b + w) + (b + 2 * w))
            {
                towerDistanceJudgment = TowerDistanceJudgment.Third;
                index = towerDistanceJudgment.GetIntFromEnum();
                _Above = towerDistanceJudgment.GetEnumDescriptionFromEnum();// 上部
            }
            else if ((2 * b + w) + (b + 2 * w) <= L && L < (2 * b + w) + (2 * b + 2 * w))
            {
                towerDistanceJudgment = TowerDistanceJudgment.Fourth;
                index = towerDistanceJudgment.GetIntFromEnum();
                _Above = towerDistanceJudgment.GetEnumDescriptionFromEnum();// 上部
            }
            else if ((2 * b + w) + (2 * b + 2 * w) <= L && L < 2 * (2 * b + w) + (b + 2 * w))
            {
                towerDistanceJudgment = TowerDistanceJudgment.Fifth;
                index = towerDistanceJudgment.GetIntFromEnum();
                _Above = towerDistanceJudgment.GetEnumDescriptionFromEnum();// 上部
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (w + i * (2 * b + w) <= L && L < (b + 2 * w) + i * (2 * b + w))
                    {
                        index = 1;
                        _Above = "可设置 " + "(" + (i + 1).ToString() + ")" + " 条车道 + 最多" + "(" + (2 * (i + 1)).ToString() + ")" + " 排车位";
                    }
                    else if (i * (2 * b + w) + (b + 2 * w) <= L && L < (i + 1) * (b + 2 * w) + w)
                    {
                        index = 2;
                        _Above = "可设置 " + "(" + (i + 2).ToString() + ")" + " 条车道 + " + "(" +  (2 * i + 3).ToString() + ")" + " 排车位";
                    }
                }
            }

            #endregion
            string _Below = "最佳值：";// 底部
            string _Prefix = "楼间距为 ";// 前缀
            string _Suffix = "mm";// 后缀

            Dimension dimension = null;
            using (Transaction trans = new Transaction(doc, "添加塔楼间距标注"))
            {
                trans.Start();
                if(dimensionType == null)
                {
                    dimension = doc.Create.NewDimension(doc.ActiveView, dimensionLine, referenceArray);
                }
                else
                {
                    dimension = doc.Create.NewDimension(doc.ActiveView, dimensionLine, referenceArray, dimensionType);
                }

                dimension.Above = _Above;// 上部
                // dimension.Below = _Below;// 底部
                // dimension.Prefix = _Prefix;// 前缀
                // dimension.Suffix = _Suffix;// 后缀
                dimension.ValueOverride = valueOverride;

                // 较为经济的间距区域
                if (index == 1|| index == 3|| index == 5)
                {
                    doc.ActiveView.SetProjectionLineColor(dimension.Id, new Color(0, 255, 0));
                }
                // 不经济的间距区域
                else if(index == 2|| index == 4)
                {
                    doc.ActiveView.SetProjectionLineColor(dimension.Id, new Color(255, 0, 0));
                }
                trans.Commit();
            }

            return dimension;
        }
        /// <summary>
        /// 找到两条平行线段的中点连线 1 将平行线旋转至水平位置 2 求出它们所在矩形的上下边中点连线
        /// </summary>
        /// <returns></returns>
        internal Line FindMindLine(Line line01, Line line02)
        {
            Segment2d segment2D01 = line01.ToSegment2d();
            Segment2d segment2D02 = line02.ToSegment2d();

            double rotateAngle = segment2D01.Direction.AngleR(new Vector2d(1, 0));
            Vector2d rotateOrigin = segment2D01.P0;
            segment2D01 = segment2D01.Rotate(rotateOrigin, rotateAngle);
            segment2D02 = segment2D02.Rotate(rotateOrigin, rotateAngle);

            Vector2d center01 = segment2D01.Center;
            Vector2d center02 = segment2D02.Center;

            double x = (center01.x + center02.x) / 2;

            Segment2d segment2D = new Segment2d(new Vector2d(x, center01.y), new Vector2d(x, center02.y));
            segment2D = segment2D.Rotate(rotateOrigin, -rotateAngle);

            return segment2D.ToLine();
        }

    }
}
