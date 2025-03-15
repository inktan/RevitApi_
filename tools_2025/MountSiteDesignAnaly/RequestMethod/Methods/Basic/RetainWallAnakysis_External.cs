using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MountSiteDesignAnaly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using g3;
using goa.Common;
using goa.Revit.DirectContext3D;
using goa.Common.g3InterOp;
using TOPO_ANLS;

namespace MountSiteDesignAnaly
{
    class RetainWallAnakysis_External : RequestMethod
    {
        internal RetainWallAnakysis_External(UIApplication _uiApp) : base(_uiApp)
        {

        }
        TopographySurface topographySurface { get; set; }
        internal override void Execute()
        {
            // 数据记录
            Variable_.CsvInfo = new List<object[]>();
            Variable_.CsvInfo.Add(new object[] { DateTime.Now });

            GeometryDrawServersMgr.ClearAllServers();

            // 抓取注释为 work 地形
            List<TopographySurface> topographySurfaces = new FilteredElementCollector(this.doc)
                            .OfCategory(BuiltInCategory.OST_Topography)
                            .WhereElementIsNotElementType()
                            .Cast<TopographySurface>()
                            .Where(p => !p.IsHidden(this.view))
                            .ToList();

            if (topographySurfaces.Count < 1)
            {
                "当前文件不存在地形。".TaskDialogErrorMessage();
                return;
            }

            // 锁定原始地形
            List<TopographySurface> topographySurfacesWork = topographySurfaces
                           .Where(p => p.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == "work" || p.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == "WORK")
                           .ToList();

            if (topographySurfacesWork.Count > 0)
            {
                topographySurface = topographySurfacesWork.FirstOrDefault();
            }
            else
            {
                topographySurface = topographySurfaces.FirstOrDefault();
            }

            if (topographySurface == null)
            {
                throw new NotImplementedException("不存在地形文件");
            }

            // 构建三角面的八叉树
            Tri3dOctree tri3DOctree = new Tri3dOctree(topographySurface);

            List<Element> elements = new List<Element>();

            // 抓取当前视图的 常规模型 图元
            List<Element> gms = new FilteredElementCollector(this.doc, this.view.Id)
                      .OfCategory(BuiltInCategory.OST_GenericModel)
                      .WhereElementIsNotElementType()
                      .Where(p => p is FamilyInstance)
                      .Where(p => !p.IsHidden(this.view))
                      .Where(p => p.get_BoundingBox(this.view) != null)
                      .ToList();
            if (ViewModel.Instance.CalGM)
            {
                elements.AddRange(gms);
            }

            // 抓取当前视图的 楼板
            List<Floor> floors = new FilteredElementCollector(this.doc)
                   .OfCategory(BuiltInCategory.OST_Floors)
                   .WhereElementIsNotElementType()
                   .Cast<Floor>()
                   .Where(p => p.Name.Contains("土方"))
                   .ToList();
            if (ViewModel.Instance.CalFloor)
            {
                elements.AddRange(floors);
            }

            // 抓取当前 建筑地坪-水 图元
            // 需要筛选地形范围内的建筑地坪
            List<BuildingPad> buildingPads = new FilteredElementCollector(this.doc, this.view.Id)
                     .OfCategory(BuiltInCategory.OST_BuildingPad)
                     .WhereElementIsNotElementType()
                     .Cast<BuildingPad>()
                     .Where(p => p.Name.Contains("水")
                        || p.Name.Contains("water")
                        || p.Name.Contains("Water")
                        || p.Name.Contains("湖")
                        || p.Name.Contains("海")
                        || p.Name.Contains("江"))
                     .Where(p => !p.IsHidden(this.view))
                     .ToList();

            if (ViewModel.Instance.CalPad)
            {
                elements.AddRange(buildingPads);
            }

            if (elements.Count < 1) return;

            // 楼板碰撞检测
            //CollisionDetection(elements);
            // 
            Computer(elements, tri3DOctree);
        }

        private void CollisionDetection(List<Element> elements)
        {

            List<FloorInfo> floors = elements.Where(p => p is Floor).Select(p => new FloorInfo(p)).Where(p => p.comments != "x" || p.comments != "X").ToList();

            foreach (var item01 in floors)
            {
                if (item01.Poly.Area.EqualZreo())
                {
                    continue;
                }

                bool isBreak = false;
                foreach (var item02 in floors)
                {
                    if (item01.Element.Id == item02.Element.Id)
                        continue;

                    if (item02.Poly.Area.EqualZreo())
                    {
                        continue;
                    }

                    Polygon2d poly01 = item01.Poly.InwardOffeet(Precision_.TheShortestDistance);
                    Polygon2d poly02 = item02.Poly.InwardOffeet(Precision_.TheShortestDistance);

                    if (poly01.Intersects(poly02) || poly02.Intersects(poly01) || poly01.Contains(poly02) || poly02.Contains(poly01))
                    {
                        this.uiDoc.ShowElements(new List<ElementId>() { item01.Element.Id, item02.Element.Id });// 窗口中心显示碰撞
                        this.sel.SetElementIds(new List<ElementId>() { item01.Element.Id, item02.Element.Id });// 窗口高亮显示碰撞

                        "计算土方的楼板，存在重叠区域，请检查".TaskDialogErrorMessage();

                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }

            // throw new NotImplementedException();
        }

        /// <summary>
        /// 收集所有元素的线圈
        /// </summary>
        void Computer(List<Element> elements, Tri3dOctree tri3DOctree)
        {
            List<Polygon2d> allPolygon2ds = new List<Polygon2d>();

            // 每个地块
            List<SinglePlot> singlePlots = new List<SinglePlot>();

            // 每个子地块
            List<SingleSubPlotEarthwork> singleSubPlotEarthworks = new List<SingleSubPlotEarthwork>();
            // 每个子地块中的子细胞-为了开启多线程
            List<SingleCellEarthwork> singleCellEarthworks = new List<SingleCellEarthwork>();

            if (elements.Count < 1) return;

            CMD.PlotNum = 0;
            CMD.SubPlotNum = 0;

            // 只是为了收集所有线圈
            foreach (var p0 in elements)
            {
                SinglePlot singlePlot = new SinglePlot(p0);
                singlePlot.SolidHeightExtraction();
                if (singlePlot.baseFaces.Count < 1) continue;

                List<BaseFaceCoil> panarFaceCoils = singlePlot.baseFaces
                    .Where(p => p.planeType != PlaneType.ZAxisDown)
                    .Select(p1 => new BaseFaceCoil(p1))
                    .ToList();

                // 收集楼板的所有面域边界
                panarFaceCoils.ForEach(p2 =>
                {
                    Poly2dSampling poly2DSampling = p2.GetPoly2dSampling();
                    allPolygon2ds.Add(poly2DSampling.polygon2d);
                });
            };
            //合并线圈
            allPolygon2ds = allPolygon2ds.UnionClipper().ToList();

            //this.doc.CreateDirectShapeWithNewTransaction(AllPolygon2ds.SelectMany(p=>p.ToLines()));

            foreach (var p0 in elements)
            {
                CMD.PlotNum++;
                SinglePlot singlePlot = new SinglePlot(p0);
                singlePlots.Add(singlePlot);
                singlePlot.SolidHeightExtraction();

                if (singlePlot.baseFaces.Count < 1)
                {
                    CMD.PlotNum--;
                    continue;
                }

                List<BaseFaceCoil> panarFaceCoils = singlePlot.baseFaces
                    .Where(p => p.planeType != PlaneType.ZAxisDown)
                    .Select(p1 => new BaseFaceCoil(p1))
                    .ToList();

                // 收集楼板的所有面域边界
                panarFaceCoils.ForEach(p2 =>
                {
                    Poly2dSampling poly2DSampling = p2.GetPoly2dSampling();
                    var singleSubPlotEarthwork = new SingleSubPlotEarthwork(tri3DOctree.boundsTree, tri3DOctree.qtree, poly2DSampling);

                    CMD.SubPlotNum++;
                    singleSubPlotEarthwork.Id = singlePlot.element.Id.IntegerValue;
                    singleSubPlotEarthwork.plotNum = singlePlot.plotNum;
                    singleSubPlotEarthwork.subNum = CMD.SubPlotNum;
                    singleSubPlotEarthwork.maxSize = 10000;// 测试多线程耗时

                    // 这里取的采样点为各个子细胞边界采样点，只是为了计算挡土墙
                    singleSubPlotEarthwork.SplitSelf_RetainWall(allPolygon2ds);

                    singleCellEarthworks.AddRange(singleSubPlotEarthwork.singleCellEarthworks);
                    singleSubPlotEarthworks.Add(singleSubPlotEarthwork);
                    singlePlot.singleSubPlotEarthworks.Add(singleSubPlotEarthwork);
                });
            };

            if (singleSubPlotEarthworks.Count < 1) return;
            if (singleCellEarthworks.Count < 1) return;

            // 数据并行计算 多线程
            Variable_.CsvInfo.Add(new object[] { "并行计算耗时", DateTime.Now });

            Parallel.ForEach(singleCellEarthworks, item =>
            {
                item.Computer();
            });

            DataPartitionStatistics(singleSubPlotEarthworks);
            //DataPartitionStatistics(singlePlots);

            this.doc.SaveCsv(Variable_.CsvInfo, "挡土墙-外部-高度计算");

            string filePath = "挡土墙高度计算完成，数据存放在" + doc.PathName;
            int length = filePath.Length;
            filePath = filePath.Substring(0, length - 4) + "挡土墙高度计算" + @".csv";
            filePath.TaskDialogErrorMessage();
        }
        void ShowTris(List<TriangleInfo> triangleInfos)
        {
            GeometryDrawServerInputs geometryDrawServerInputs = new GeometryDrawServerInputs();

            //List<TriangleInfo> triangleInfosFills = triangleInfos.Where(p => p.EarthworkVolume < 0).ToList();
            //System.Drawing.Color[] tmpColors = { System.Drawing.Color.Blue, System.Drawing.Color.Cyan };
            System.Drawing.Color[] tmpColors = { System.Drawing.Color.Blue, System.Drawing.Color.Cyan, System.Drawing.Color.Yellow, System.Drawing.Color.Red };

            List<System.Drawing.Color> colors = Color_.GetColorListRedToMagenta(tmpColors, triangleInfos.Count);

            double min = triangleInfos.First().Retain_Wall_Elecation_Difference;
            double max = triangleInfos.Last().Retain_Wall_Elecation_Difference;

            double interval = max - min;

            for (int i = 0; i < triangleInfos.Count; i++)
            {
                if (ReferenceEquals(triangleInfos[i], null))
                {
                    continue;
                }
                if (interval != 0)
                {
                    int index = Convert.ToInt32((triangleInfos[i].Retain_Wall_Elecation_Difference - min) / interval * (triangleInfos.Count - 1));
                    geometryDrawServerInputs.AddTriangleToBuffer(triangleInfos[i].Triangle3d, new XYZ(0, 0, 10), new ColorWithTransparency((uint)colors[index].R, (uint)colors[index].G, (uint)colors[index].B, 0), new XYZ(0, 0, 150), false);
                }
                else
                {
                    geometryDrawServerInputs.AddTriangleToBuffer(triangleInfos[i].Triangle3d, new XYZ(0, 0, 10), new ColorWithTransparency(255, 0, 0, 0), new XYZ(0, 0, 150), false);
                
                    //增加竖直面 
                }
            }

            //List<TriangleInfo> triangleInfosDigs = triangleInfos.Where(p => p.EarthworkVolume > 0).ToList();
            //System.Drawing.Color[] tmpColors02 = { System.Drawing.Color.Yellow, System.Drawing.Color.Red };

            //colors = Color_.GetColorListRedToMagenta(tmpColors02, triangleInfosDigs.Count);
            //for (int i = 0; i < triangleInfosDigs.Count; i++)
            //{
            //    if (ReferenceEquals(triangleInfosDigs[i], null))
            //    {
            //        continue;
            //    }
            //    geometryDrawServerInputs.AddTriangleToBuffer(triangleInfosDigs[i].Triangle3d, new XYZ(0, 0, 10), new ColorWithTransparency((uint)colors[i].R, (uint)colors[i].G, (uint)colors[i].B, 0), new XYZ(0, 0, 50), false);
            //}

            //foreach (var item in triangleInfos)
            //{
            //    if (ReferenceEquals(item, null))
            //    {
            //        continue;
            //    }
            //    geometryDrawServerInputs.AddTriangleToBuffer(item.Triangle3d, new XYZ(0, 0, 10), new ColorWithTransparency(item.r, item.g, item.b, item.a), new XYZ(0, 0, 50), false);
            //    // 取点会快一些，显示为点阵，效果差强人意
            //    //geometryDrawServerInputs.AddPointToBuffer(item.Triangle3d.Center().ToXYZ(), new ColorWithTransparency(item.r, item.g, item.b, item.a), new XYZ(0, 0, 200));
            //}
            GeometryDrawServersMgr.ShowGraphics(geometryDrawServerInputs, Guid.NewGuid().ToString());
        }
        void AddText(List<EaTextIfo> textIfos)
        {
            TextNoteType textNoteType = this.doc.FindTxType("宋体 12mm", 12);

            // 文字
            using (Transaction trans = new Transaction(doc, "creatText"))
            {
                trans.Start();

                List<ElementId> elementIds = new List<ElementId>();
                textIfos.ForEach(p =>
                {
                    TextNote textNote = TextNote.Create(doc, this.view.Id, p.Position.ToXYZ(), p.SerialNumber.ToString(), textNoteType.Id);
                    textNote.HorizontalAlignment = HorizontalTextAlignment.Left;

                    elementIds.Add(textNote.Id);
                });

                this.doc.Create.NewGroup(elementIds);

                trans.Commit();
            }
        }
        /// <summary>
        /// 统计大地块
        /// </summary>
        /// <param name="singleRegionEarthworks"></param>
        void DataPartitionStatistics(List<SinglePlot> singlePlots)
        {
            //Variable_.CsvInfo.Add(new object[] { "-", "-", "地块编号", "平整区域水平投影面积-㎡", "挖方量（正值）-m³", "填方量（负值）-m³", "土方量-m³" });
            //double areaSum = 0;
            //double volumeDigSum = 0;
            //List<EaTextIfo> textIfos = new List<EaTextIfo>();
            //foreach (var item01 in singlePlots)
            //{
            //    foreach (var item02 in item.singleSubPlotEarthworks)
            //    {

            //    }                

            //    Variable_.CsvInfo.Add(new object[]
            //     {
            //               "-",
            //               "-",
            //               item.plotNum,
            //              item. area.SQUARE_FEETtoSQUARE_METERS().NumDecimal().ToString(),
            //               item.volumeDig.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
            //              item. volumeFill.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
            //              item. volume.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
            //     });

            //    areaSum += item.area;
            //    volumeDigSum += item.volumeDig;
            //    textIfos.Add(item.eaTextInfo);
            //}

            //Variable_.CsvInfo.Add(new object[]
            //   {
            //               "-",
            //               "-",
            //               "合计",
            //               areaSum.SQUARE_FEETtoSQUARE_METERS().NumDecimal().ToString(),
            //               volumeDigSum.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
            //   });

            //// 大地块也要进行文字输出
            //AddText(textIfos);
        }
        /// <summary>
        /// 统计子地块
        /// </summary>
        /// <param name="singleSubPlotEarthworks"></param>
        void DataPartitionStatistics(List<SingleSubPlotEarthwork> singleSubPlotEarthworks)
        {
            Variable_.CsvInfo.Add(new object[] { "-", "地块Id", "分地块序号", "挡土墙（0-3m）", "挡土墙（3-6m）", "挡土墙（6-8m）", "挡土墙（8-10m）", "挡土墙（10-20m）" });

            List<EaTextIfo> textIfos = new List<EaTextIfo>();
            List<TriangleInfo> triangleInfos = new List<TriangleInfo>();
            List<Segment3d> showLines = new List<Segment3d>();

            double count = 0;// 总取点数
            double length01 = 0.0;
            double length02 = 0.0;
            double length03 = 0.0;
            double length04 = 0.0;
            double length05 = 0.0;

            List<TriangleInfo> triInfo_0_3 = new List<TriangleInfo>();
            List<TriangleInfo> triInfo_3_6 = new List<TriangleInfo>();
            List<TriangleInfo> triInfo_6_8 = new List<TriangleInfo>();
            List<TriangleInfo> triInfo_8_10 = new List<TriangleInfo>();
            List<TriangleInfo> triInfo_10_20 = new List<TriangleInfo>();

            // 对各个面域的数据进行统计
            singleSubPlotEarthworks.ForEach(item =>
            {
                item.EarthworkSum();

                List<TriangleInfo> _0_3 = new List<TriangleInfo>();
                List<TriangleInfo> _3_6 = new List<TriangleInfo>();
                List<TriangleInfo> _6_8 = new List<TriangleInfo>();
                List<TriangleInfo> _8_10 = new List<TriangleInfo>();
                List<TriangleInfo> _10_20 = new List<TriangleInfo>();

                count += item.triangleInfos.Count;

                foreach (var triInfos in item.triangleInfos)
                {
                    if (triInfos.Retain_Wall_Elecation_Difference > 0.0)
                    {
                        triangleInfos.Add(triInfos);
                        showLines.Add(triInfos.Segment3d);

                        if (triInfos.Retain_Wall_Elecation_Difference > 0 && triInfos.Retain_Wall_Elecation_Difference < 3000.0.MilliMeterToFeet())
                        {
                            _0_3.Add(triInfos);
                        }
                        else if (triInfos.Retain_Wall_Elecation_Difference >= 3000.0.MilliMeterToFeet() && triInfos.Retain_Wall_Elecation_Difference < 6000.0.MilliMeterToFeet())
                        {
                            _3_6.Add(triInfos);
                        }
                        else if (triInfos.Retain_Wall_Elecation_Difference >= 6000.0.MilliMeterToFeet() && triInfos.Retain_Wall_Elecation_Difference < 8000.0.MilliMeterToFeet())
                        {
                            _6_8.Add(triInfos);
                        }
                        else if (triInfos.Retain_Wall_Elecation_Difference >= 8000.0.MilliMeterToFeet() && triInfos.Retain_Wall_Elecation_Difference < 10000.0.MilliMeterToFeet())
                        {
                            _8_10.Add(triInfos);
                        }
                        else if (triInfos.Retain_Wall_Elecation_Difference >= 10000.0.MilliMeterToFeet() && triInfos.Retain_Wall_Elecation_Difference < 20000.0.MilliMeterToFeet())
                        {
                            _10_20.Add(triInfos);
                        }
                    }
                }
                int tempCount = item.triangleInfos.Count;
                double tempLength = item.poly2DSampling.polygon2d.ArcLength.FeetToMilliMeter() / 1000.0;

                if (tempCount != 0)
                {
                    double tempLength01 = (_0_3.Count / (double)tempCount) * tempLength;
                    double tempLength02 = (_3_6.Count / (double)tempCount) * tempLength;
                    double tempLength03 = (_6_8.Count / (double)tempCount) * tempLength;
                    double tempLength04 = (_8_10.Count / (double)tempCount) * tempLength;
                    double tempLength05 = (_10_20.Count / (double)tempCount) * tempLength;

                    length01 += tempLength01;
                    length02 += tempLength02;
                    length03 += tempLength03;
                    length04 += tempLength04;
                    length05 += tempLength05;

                    Variable_.CsvInfo.Add(new object[]
                    {
                        item.plotNum,
                        item.Id,
                        item.subNum,
                        item.poly2DSampling.polygon2d.Area.SQUARE_FEETtoSQUARE_METERS().NumDecimal(),
                        tempLength.NumDecimal(),
                        tempLength01.NumDecimal(),
                        tempLength02.NumDecimal(),
                        tempLength03.NumDecimal(),
                        tempLength04.NumDecimal(),
                        tempLength05.NumDecimal(),
                    });
                }

                triInfo_0_3.AddRange(_0_3);
                triInfo_3_6.AddRange(_3_6);
                triInfo_6_8.AddRange(_6_8);
                triInfo_8_10.AddRange(_8_10);
                triInfo_10_20.AddRange(_10_20);

                // 添加子地块编号
                textIfos.Add(item.eaTextInfo);

            });

            Variable_.CsvInfo.Add(new object[]
            {
                "合计",
                "-",
                "-",
                length01.NumDecimal(),
                length02.NumDecimal(),
                length03.NumDecimal(),
                length04.NumDecimal(),
                length05.NumDecimal(),
            });

            AddText(textIfos);

            if (ViewModel.Instance.ShowSamplingTris)
            {
                triangleInfos = triangleInfos.OrderBy(p => p.Retain_Wall_Elecation_Difference).ToList();
                // 显示颜色
                ShowTris(triangleInfos);
                // 添加图例
                DrawText("挡土墙（m）", triangleInfos.Last().Retain_Wall_Elecation_Difference.FeetToMilliMeter() / 1000, triangleInfos.First().Retain_Wall_Elecation_Difference.FeetToMilliMeter() / 1000);
            }

            if (ViewModel.Instance.ShowSamplingLine)
            {
                this.doc.CreateDirectShapeWithNewTransaction(showLines.Select(p => p.ToLine()));
            }
        }
        internal void DrawText(string title, double max, double min)
        {
            TextNoteType textNoteType = this.doc.FindTxType("宋体 12mm", 12);

            // 图例左上角定位点
            XYZ location = this.sel.PickPoint("选择图例定位点");

            // 将文字id存储下来
            List<ElementId> eleIds = new List<ElementId>();
            // 文字
            using (Transaction trans = new Transaction(doc, "creatText"))
            {
                trans.Start();
                TextNote textNote = TextNote.Create(doc, this.view.Id, location, title, textNoteType.Id);
                textNote.HorizontalAlignment = HorizontalTextAlignment.Left;
                eleIds.Add(textNote.Id);
                trans.Commit();
            }

            // 图例的高度
            //double height = (this.max.Y - this.min.Y);
            //double width = (this.max.Y - this.min.Y) / 5;

            BoundingBoxXYZ boundingBoxXYZ = topographySurface.get_BoundingBox(this.view);
            double height = (boundingBoxXYZ.Max.Y - boundingBoxXYZ.Min.Y) / 3;
            double width = height / 5;

            double buffer = width * 1.05;

            int legendTextCount = 6;
            int interval = (Int32)(height / legendTextCount);

            double intervalValue = (max - min) / legendTextCount;
            if (intervalValue == 0)
            {
                intervalValue = max / legendTextCount;
            }
            // 文字
            using (Transaction trans = new Transaction(doc, "creatText"))
            {
                trans.Start();
                for (int i = 0; i < legendTextCount + 1; i++)
                {
                    TextNote textNote = TextNote.Create(doc, this.view.Id, location + new XYZ(buffer * 1.2, -buffer / 1.2 - (interval * i), 0), (max - intervalValue * i).NumDecimal(0).ToString(), textNoteType.Id);
                    textNote.HorizontalAlignment = HorizontalTextAlignment.Left;

                    eleIds.Add(textNote.Id);
                }

                //TextNote _textNote = TextNote.Create(doc, this.view.Id, location + new XYZ(buffer * 1.2, -buffer / 1.5 - 1 * (height + intervalValue), 0), min.NumDecimal(0).ToString(), textNoteType.Id);
                //_textNote.HorizontalAlignment = HorizontalTextAlignment.Left;

                //eleIds.Add(_textNote.Id);

                AutoGeneratedElementMgr autoGeneratedElementMgr = new AutoGeneratedElementMgr(this.doc, "TopographicAnalysisLegend");// id用于存储地形分析图例的文字
                autoGeneratedElementMgr.SaveIds(eleIds.Select(p => p.ToString()));

                trans.Commit();
            }

            // 绘制颜色梯度样式
            this.DrawRectColorByTri3d(PubFuncWt.RevitTo_.ToVector3d((location + new XYZ(0, -buffer, 0))), height, width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startP"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="count">组成矩形的三角形总个数</param>
        /// <returns></returns>
        internal void DrawRectColorByTri3d(Vector3d startP, double height, double width, int count = 200)
        {
            List<Triangle3d> results = new List<Triangle3d>();

            double heightInterval = height / count;

            for (int i = 0; i < count; i++)
            {
                Vector3d nowStartP = startP + new Vector3d(0, (-1) * heightInterval * (i), 0);
                Triangle3d triangle3d01 = new Triangle3d(nowStartP, nowStartP + new Vector3d(0, (-1) * heightInterval, 0), nowStartP + new Vector3d(width, 0, 0));
                Triangle3d triangle3d02 = new Triangle3d(nowStartP + new Vector3d(0, (-1) * heightInterval, 0), nowStartP + new Vector3d(width, (-1) * heightInterval, 0), nowStartP + new Vector3d(width, 0, 0));

                results.Add(triangle3d01);
                results.Add(triangle3d02);
            }

            List<ElevationTriangle> eTs = results.Select(p => new ElevationTriangle(p)).ToList();

            System.Drawing.Color[] tmpColors = { System.Drawing.Color.Blue, System.Drawing.Color.Cyan, System.Drawing.Color.Yellow, System.Drawing.Color.Red };
            List<System.Drawing.Color> colors = Color_.GetColorListRedToMagenta(tmpColors, eTs.Count, false);
            for (int i = 0; i < eTs.Count; i++)
            {
                try
                {
                    eTs[i].ColorWithTransparency = new ColorWithTransparency((uint)colors[i].R, (uint)colors[i].G, (uint)colors[i].B, 0);
                }
                catch (Exception)
                {
                }
            }

            GeometryDrawServerInputs geometryDrawServerInputs = new GeometryDrawServerInputs();
            foreach (var item in eTs)
            {
                geometryDrawServerInputs.AddTriangleToBuffer(item.Triangle3d, new XYZ(0, 0, 1), item.ColorWithTransparency, new XYZ(0, 0, 1), false);
            }

            GeometryDrawServersMgr.ShowGraphics(geometryDrawServerInputs, Guid.NewGuid().ToString());
        }

    }
}
