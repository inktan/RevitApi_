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
    class EaCalBy_GM_Floor : RequestMethod
    {
        internal EaCalBy_GM_Floor(UIApplication _uiApp) : base(_uiApp)
        {

        }
        TopographySurface topographySurface { get; set; }
        internal override void Execute()
        {
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

            // 添加体量族
            //List<Face> planarFaces = this.doc.GetElement(this.sel.PickObject(ObjectType.Element)).GetAllFaces();
            //List<PlanarFace> planarFaces = this.doc.GetElement(this.sel.PickObject(ObjectType.Element)).GetAllFacesFacingUp().ToList();
            //planarFaces.Count.ToString().TaskDialogErrorMessage();

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
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="tri3DOctree"></param>
        void Computer(List<Element> elements, Tri3dOctree tri3DOctree)
        {
            // 每个地块
            List<SinglePlot> singlePlots = new List<SinglePlot>();

            // 每个子地块
            List<SingleSubPlotEarthwork> singleSubPlotEarthworks = new List<SingleSubPlotEarthwork>();
            // 每个子地块中的子细胞-为了开启多线程
            List<SingleCellEarthwork> singleCellEarthworks = new List<SingleCellEarthwork>();

            if (elements.Count < 1) return;

            CMD.PlotNum = 0;
            CMD.SubPlotNum = 0;
            // 一个元素一个统计统计
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
                // 有效地块进行统计

                // 各个子细胞的序号
                List<BaseFaceCoil> panarFaceCoils =
                    singlePlot
                    .baseFaces
                    .Where(p => p.planeType != PlaneType.ZAxisDown)
                    .Select(p1 => new BaseFaceCoil(p1))
                    .ToList();

                // 楼板的数据要进行归纳
                panarFaceCoils.ForEach(p2 =>
                {
                    Poly2dSampling poly2DSampling = p2.GetPoly2dSampling();

                    var singleSubPlotEarthwork = new SingleSubPlotEarthwork(tri3DOctree.boundsTree, tri3DOctree.qtree, poly2DSampling);

                    CMD.SubPlotNum++;
                    singleSubPlotEarthwork.Id = singlePlot.element.Id.IntegerValue;
                    singleSubPlotEarthwork.plotNum = singlePlot.plotNum;
                    singleSubPlotEarthwork.subNum = CMD.SubPlotNum;
                    singleSubPlotEarthwork.maxSize = 10000;// 测试多线程耗时
                    singleSubPlotEarthwork.SplitSelf();

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
            DataPartitionStatistics(singlePlots);

            this.doc.SaveCsv(Variable_.CsvInfo, "土方平衡计算");

            string filePath = "土方平衡计算完成，数据存放在" + doc.PathName;
            int length = filePath.Length;
            filePath = filePath.Substring(0, length - 4) + "土方平衡计算" + @".csv";
            filePath.TaskDialogErrorMessage();
        }
        void ShowTris(List<TriangleInfo> triangleInfos)
        {
            GeometryDrawServerInputs geometryDrawServerInputs = new GeometryDrawServerInputs();

            //挖方
            List<TriangleInfo> triangleInfosDigs = triangleInfos.Where(p => p.EarthworkVolume > 0).ToList();
            System.Drawing.Color[] tmpColors01 = { System.Drawing.Color.Yellow, System.Drawing.Color.Red };

            double min = triangleInfosDigs.First().EarthworkVolume;
            double max = triangleInfosDigs.Last().EarthworkVolume;
            double interval = max - min;

            List<System.Drawing.Color> colors = Color_.GetColorListRedToMagenta(tmpColors01, triangleInfosDigs.Count);

            for (int i = 0; i < triangleInfosDigs.Count; i++)
            {
                if (ReferenceEquals(triangleInfosDigs[i], null))
                {
                    continue;
                }
                int index = Convert.ToInt32((triangleInfosDigs[i].EarthworkVolume - min) / interval * (triangleInfosDigs.Count - 1));

                Triangle3d triangle3d = triangleInfosDigs[i].Triangle3d;

                triangle3d.V0 =new Vector3d(triangle3d.V0.x, triangle3d.V0.y,300.0);
                triangle3d.V1 =new Vector3d(triangle3d.V1.x, triangle3d.V1.y,300.0);
                triangle3d.V2 =new Vector3d(triangle3d.V2.x, triangle3d.V2.y,300.0);

                geometryDrawServerInputs.AddTriangleToBuffer(triangle3d, new XYZ(0, 0, 10), new ColorWithTransparency((uint)colors[index].R, (uint)colors[index].G, (uint)colors[index].B, 0), new XYZ(0, 0, 50), false);
            }

            //填方
            List<TriangleInfo> triangleInfosFills = triangleInfos.Where(p => p.EarthworkVolume <= 0).ToList();
            System.Drawing.Color[] tmpColors02 = { System.Drawing.Color.Blue, System.Drawing.Color.Cyan };

            min = triangleInfosFills.First().EarthworkVolume;
            max = triangleInfosFills.Last().EarthworkVolume;
            interval = max - min;

            colors = Color_.GetColorListRedToMagenta(tmpColors02, triangleInfosFills.Count);

            for (int i = 0; i < triangleInfosFills.Count; i++)
            {
                if (ReferenceEquals(triangleInfosFills[i], null))
                {
                    continue;
                }
                int index = Convert.ToInt32((triangleInfosFills[i].EarthworkVolume - min) / interval * (triangleInfosFills.Count - 1));

                Triangle3d triangle3d = triangleInfosFills[i].Triangle3d;

                triangle3d.V0 = new Vector3d(triangle3d.V0.x, triangle3d.V0.y, 300.0);
                triangle3d.V1 = new Vector3d(triangle3d.V1.x, triangle3d.V1.y, 300.0);
                triangle3d.V2 = new Vector3d(triangle3d.V2.x, triangle3d.V2.y, 300.0);

                geometryDrawServerInputs.AddTriangleToBuffer(triangle3d, new XYZ(0, 0, 10), new ColorWithTransparency((uint)colors[index].R, (uint)colors[index].G, (uint)colors[index].B, 0), new XYZ(0, 0, 50), false);
            }

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
            Variable_.CsvInfo.Add(new object[] { "-", "-", "地块编号", "平整区域水平投影面积-㎡", "挖方量（正值）-m³", "填方量（负值）-m³", "土方量-m³" });
            double areaSum = 0;
            double volumeDigSum = 0;
            double volumeFillSum = 0;
            double volumeSum = 0;
            List<EaTextIfo> textIfos = new List<EaTextIfo>();
            foreach (var item in singlePlots)
            {
                item.Sum();
                Variable_.CsvInfo.Add(new object[]
                 {
                           "-",
                           "-",
                           item.plotNum,
                          item. area.SQUARE_FEETtoSQUARE_METERS().NumDecimal().ToString(),
                           item.volumeDig.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                          item. volumeFill.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                          item. volume.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                 });

                areaSum += item.area;
                volumeDigSum += item.volumeDig;
                volumeFillSum += item.volumeFill;
                volumeSum += item.volume;
                textIfos.Add(item.eaTextInfo);
            }

            Variable_.CsvInfo.Add(new object[]
               {
                           "-",
                           "-",
                           "合计",
                           areaSum.SQUARE_FEETtoSQUARE_METERS().NumDecimal().ToString(),
                           volumeDigSum.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                           volumeFillSum.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                           volumeSum.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
               });

            // 大地块也要进行文字输出
            AddText(textIfos);
        }
        /// <summary>
        /// 统计子地块
        /// </summary>
        /// <param name="singleSubPlotEarthworks"></param>
        void DataPartitionStatistics(List<SingleSubPlotEarthwork> singleSubPlotEarthworks)
        {
            Variable_.CsvInfo.Add(new object[] { "-", "地块Id", "分地块序号", "平整区域水平投影面积-㎡", "挖方量（正值）-m³", "填方量（负值）-m³", "土方量-m³" });

            List<EaTextIfo> textIfos = new List<EaTextIfo>();
            List<TriangleInfo> triangleInfos = new List<TriangleInfo>();
            List<Segment3d> showLines = new List<Segment3d>();

            double areaSum = 0;
            double volumeDig = 0;
            double volumeFill = 0;
            double volumeSum = 0;

            // 对各个面域的数据进行统计
            singleSubPlotEarthworks.ForEach(item =>
            {
                item.EarthworkSum();

                areaSum += item.poly2DSampling.polygon2d.Area;
                volumeDig += item.volumeDig;
                volumeFill += item.volumeFill;
                volumeSum += item.volume;

                Variable_.CsvInfo.Add(new object[]
                {
                        item.plotNum,
                        item.Id,
                        item.subNum,
                        item.poly2DSampling.polygon2d.Area.SQUARE_FEETtoSQUARE_METERS().NumDecimal().ToString(),
                        item.volumeDig.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                        item.volumeFill.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                        item.volume.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                });

                triangleInfos.AddRange(item.triangleInfos);

                // 不能直接对属性中的类字段进行外部赋值
                textIfos.Add(item.eaTextInfo);
                showLines.AddRange(item.earthworkSegment3ds);
            });
            Variable_.CsvInfo.Add(new object[]
                  {
                           "合计",
                           "-",
                           "-",
                           areaSum.SQUARE_FEETtoSQUARE_METERS().NumDecimal().ToString(),
                           volumeDig.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                           volumeFill.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                           volumeSum.CUBIC_FEETtoCUBIC_METERS().NumDecimal().ToString(),
                  });

            AddText(textIfos);

            if (ViewModel.Instance.ShowSamplingTris)
            {
                triangleInfos = triangleInfos.OrderBy(p => p.EarthworkVolume).ToList();
                // 显示颜色
                ShowTris(triangleInfos);
                // 添加图例
                DrawText("土方（m³）", triangleInfos.Last().EarthworkVolume.CUBIC_FEETtoCUBIC_METERS(), triangleInfos.First().EarthworkVolume.CUBIC_FEETtoCUBIC_METERS());

            }

            if (ViewModel.Instance.ShowSamplingLine)
            {
                this.doc.CreateDirectShapeWithNewTransaction(showLines.Where(p => p.Length > 0.01).Select(p => p.ToLine()));
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
            double rate = max / (max - min);
            // 绘制颜色梯度样式
            this.DrawRectColorByTri3d(PubFuncWt.RevitTo_.ToVector3d((location + new XYZ(0, -buffer, 0))), height, width, rate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startP"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="count">组成矩形的三角形总个数</param>
        /// <returns></returns>
        internal void DrawRectColorByTri3d(Vector3d startP, double height, double width, double rate)
        {
            List<Triangle3d> results = new List<Triangle3d>();

            double heightInterval = height / 200;

            for (int i = 0; i < 200; i++)
            {
                Vector3d nowStartP = startP + new Vector3d(0, (-1) * heightInterval * (i), 0);
                Triangle3d triangle3d01 = new Triangle3d(nowStartP, nowStartP + new Vector3d(0, (-1) * heightInterval, 0), nowStartP + new Vector3d(width, 0, 0));
                Triangle3d triangle3d02 = new Triangle3d(nowStartP + new Vector3d(0, (-1) * heightInterval, 0), nowStartP + new Vector3d(width, (-1) * heightInterval, 0), nowStartP + new Vector3d(width, 0, 0));

                results.Add(triangle3d01);
                results.Add(triangle3d02);
            }

            List<ElevationTriangle> eTs = results.Select(p => new ElevationTriangle(p)).ToList();


            System.Drawing.Color[] tmpColors01 = { System.Drawing.Color.Yellow, System.Drawing.Color.Red };
            List<System.Drawing.Color> colors01 = Color_.GetColorListRedToMagenta(tmpColors01, (int)(eTs.Count * (rate)), false);

            System.Drawing.Color[] tmpColors02 = { System.Drawing.Color.Blue, System.Drawing.Color.Cyan };
            List<System.Drawing.Color> colors02 = Color_.GetColorListRedToMagenta(tmpColors02, (int)(eTs.Count * (1 - rate)), false);

            for (int i = 0; i < eTs.Count; i++)
            {
                try
                {
                    if (i < eTs.Count * (rate))
                    {
                        eTs[i].ColorWithTransparency = new ColorWithTransparency((uint)colors01[i].R, (uint)colors01[i].G, (uint)colors01[i].B, 0);
                    }
                    else
                    {

                        eTs[i].ColorWithTransparency = new ColorWithTransparency((uint)colors02[(int)(i - eTs.Count * (rate))].R, (uint)colors02[(int)(i - eTs.Count * (rate))].G, (uint)colors02[(int)(i - eTs.Count * (rate))].B, 0);
                    }
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
