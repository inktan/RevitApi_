using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using g3;
using System.Text.RegularExpressions;
using goa.Common.g3InterOp;

namespace InfoStrucFormwork

{
    internal class SteelBeamDoubleLine : EleCreatInfo
    {
        /// <summary>
        /// 混凝土梁
        /// </summary>
        public SteelBeamDoubleLine() : base()
        {
        }

        internal override void Execute()
        {
            // 属性初始化
            this.ElementIds = new List<ElementId>();
            // 结构计算书配对
            ObtainBeamTypeFromStrucCalBook();
            // 
            if (ConBeamTypes == null || ConBeamTypes.Count < 1) return;

            // 梁类型确认
            GetFamilySymbols();
            ExGeoInfo();
            MatchBookPositionLine();

            OpenTrans();
            SetLevel();
            Move();
            System.Windows.Forms.Application.DoEvents();// 窗体重绘
        }
        public List<ConBeamType> ConBeamTypes { get; private set; }
        /// <summary>
        /// 从结构计算书获取所有的梁类型ObtainBeamTypeFromStrucCalBook
        /// </summary>
        void ObtainBeamTypeFromStrucCalBook()
        {
            // 结构计算书 文字
            List<TextInfo> textInfos = new List<TextInfo>();
            // 图层
            List<string> patterns = new List<string>()
            {
                @"dsptext_beam\z",
                @"dsptext_wallbeam\z",
            };

            // 添加额外图层
            //char[] vs = new char[] { ',', '，' };
            //patterns.AddRange(ViewModel.Instance.ConcLayernames.Split(vs));

            foreach (var pattern in patterns)
            {
                foreach (var item in this.DwgParser.LayerNames)
                {
                    if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.TexLayertInfos.ContainsKey(item))
                        {
                            textInfos.AddRange(this.DwgParser.TexLayertInfos[item]);
                        }
                    }
                }
            }
            this.ConBeamTypes = new List<ConBeamType>();

            foreach (var item in textInfos)
            {
                if (item.Text.Contains('*'))
                {
                    ConBeamType conBeamType = new ConBeamType(item);
                    conBeamType.TextInfo = item;
                    conBeamType.Excute();

                    if (conBeamType.beamTypeName != null)
                    {
                        this.ConBeamTypes.Add(conBeamType);
                    }
                }
            }
        }
        List<FamilySymbol> FamilySymbols { get; set; }
        /// <summary>
        /// 创建梁类型
        /// </summary>
        internal override void GetFamilySymbols()
        {
            // 钢梁
            Family steelBeamType = (new FilteredElementCollector(CMD.Doc))
                //.OfCategory(BuiltInCategory.OST_StructuralFraming)
                .OfClass(typeof(Family))
                .First(p => p.Name == "结构-梁-钢梁") as Family;
                //.First(p => p.Name == "结构梁_SUPER_实例") as Family;

            this.FamilySymbols = new List<FamilySymbol>();
            List<FamilySymbol> steelBeamTypes = steelBeamType.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).ToList();

            this.FamilySymbols.AddRange(steelBeamTypes);

            //foreach (var item in this.ConBeamTypes)
            //{
            //    using (Transaction transaction = new Transaction(CMD.Doc, "创建 钢梁 类型"))
            //    {
            //        transaction.Start();

            //        if (!this.FamilySymbols.Select(p => p.Name).Contains(item.beamTypeName))
            //        {
            //            if (item.B == 0 || item.H == 0 || item.U == 0 || item.T == 0 || item.D == 0 || item.F == 0) continue;
            //            FamilySymbol familySymbol = steelBeamTypes.First().Duplicate(item.beamTypeName) as FamilySymbol;
            //            this.FamilySymbols.Add(familySymbol);
            //            familySymbol.LookupParameter("B").Set(Convert.ToDouble(item.B).MilliMeterToFeet());
            //            familySymbol.LookupParameter("H").Set(Convert.ToDouble(item.H).MilliMeterToFeet());
            //            familySymbol.LookupParameter("U").Set(Convert.ToDouble(item.U).MilliMeterToFeet());
            //            familySymbol.LookupParameter("T").Set(Convert.ToDouble(item.T).MilliMeterToFeet());
            //            familySymbol.LookupParameter("D").Set(Convert.ToDouble(item.D).MilliMeterToFeet());
            //            familySymbol.LookupParameter("F").Set(Convert.ToDouble(item.F).MilliMeterToFeet());
            //        }
            //        transaction.Commit();
            //    }
            //}
        }
        List<ConBeamInfo> beamInfos { get; set; }
        List<MlineInfo> mlineInfos { get; set; }
        /// <summary>
        /// 抓取梁定位线与尺寸文字配对
        /// </summary>
        internal override void ExGeoInfo()
        {
            // 获取所有的梁的基准线 mline
            mlineInfos = new List<MlineInfo>();
            List<LineInfo> lineInfos = new List<LineInfo>();

            List<string> patternNames = new List<string>() { @"S\d*-STL-HDN" };
            foreach (var patternName in patternNames)
            {
                foreach (var item in this.DwgParser.LayerNames)
                {
                    if (Regex.IsMatch(item, patternName, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.MlineLayerInfos.ContainsKey(item))
                        {
                            mlineInfos.AddRange(this.DwgParser.MlineLayerInfos[item]);
                        }

                        if (this.DwgParser.LineLayerInfos.ContainsKey(item))
                        {
                            lineInfos.AddRange(this.DwgParser.LineLayerInfos[item]);
                        }

                        if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                        {
                            foreach (var plInfos in this.DwgParser.PolyLineLayerInfos[item])
                            {
                                if (plInfos.lineInfos != null && plInfos.lineInfos.Count > 0)
                                {
                                    lineInfos.AddRange(plInfos.lineInfos);
                                }
                            }
                        }
                    }
                }
            }
            // 将散落的线段配对成多线
            mlineInfos.AddRange(LinePair(lineInfos));
            mlineInfos = MultilingColJudge();
        }
        /// <summary>
        /// 合并重叠的定位线
        /// </summary>
        /// <returns></returns>
        List<MlineInfo> MultilingColJudge()
        {
            List<MlineInfo> result = new List<MlineInfo>();
            List<MlineInfo> collection = new List<MlineInfo>();

            int count = mlineInfos.Count;
            for (int i = 0; i < count; i++)
            {
                if (result.Contains(mlineInfos[i])) continue;
                if (collection.Contains(mlineInfos[i])) continue;

                result.Add(mlineInfos[i]);

                for (int j = 0; j < count; j++)
                {
                    if (j <= i) continue;

                    Segment2d seg01 = mlineInfos[i].Line.ToSegment2d();
                    Segment2d seg02 = mlineInfos[j].Line.ToSegment2d();

                    IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(seg01, seg02);
                    intrSegment2Segment2.Compute();

                    if (intrSegment2Segment2.Quantity == 2)
                    {
                        collection.Add(mlineInfos[j]);
                        List<Vector2d> pts = new List<Vector2d>() { seg01.P0, seg01.P1, seg02.P0, seg02.P1 };
                        pts.OrderByDescending(p => p.y).OrderBy(p => p.x).ToList();
                        mlineInfos[i].Line = new Segment2d(pts.First(), pts.Last()).ToLine();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将散落的线段配对成多线
        /// </summary>
        /// <param name="lineInfos"></param>
        /// <returns></returns>
        internal IEnumerable<MlineInfo> LinePair(List<LineInfo> lineInfos)
        {
            List<LineInfo> collectLineInfos = new List<LineInfo>();
            for (int i = 0; i < lineInfos.Count; i++)
            {
                LineInfo lineInfo01 = lineInfos[i];

                if (lineInfo01.Line == null) continue;
                if (collectLineInfos.Contains(lineInfo01)) continue;

                List<LineInfo> parrelLineInfo = new List<LineInfo>();

                for (int j = 0; j < lineInfos.Count; j++)
                {
                    if (j <= i) continue;
                    LineInfo lineInfo02 = lineInfos[j];

                    if (lineInfo02.Line == null) continue;
                    if (collectLineInfos.Contains(lineInfo02)) continue;

                    // 先找到所有的平行线
                    if (Math.Abs(lineInfo01.Line.Direction.DotProduct(lineInfo02.Line.Direction)).EqualPrecision(1.0))
                    {
                        parrelLineInfo.Add(lineInfo02);
                    }
                }

                LineInfo lineInfo = parrelLineInfo
                    .Where(p => new Line2d(p.Segment2d.P0, p.Segment2d.Direction).DistanceSquared(lineInfo01.Segment2d.P0) <= Math.Pow(50.0.MilliMeterToFeet(), 2))
                    .Where(p => new Line2d(p.Segment2d.P0, p.Segment2d.Direction).DistanceSquared(lineInfo01.Segment2d.P0) > Math.Pow(1.0.MilliMeterToFeet(), 2))
                    .OrderBy(p => p.Center.DistanceTo(lineInfo01.Center))
                    .FirstOrDefault();

                if (lineInfo == null) continue;

                MlineInfo mlineInfo = new MlineInfo();
                mlineInfo.LineInfo01 = lineInfo01;
                mlineInfo.LineInfo02 = lineInfo;
                LineToMul(mlineInfo);

                //CMD.Doc.CreateDirectShapeWithNewTransaction(new List<GeometryObject>() { mlineInfo.Line });

                collectLineInfos.Add(lineInfo01);
                collectLineInfos.Add(lineInfo);

                yield return mlineInfo;
            }
        }
        void LineToMul(MlineInfo mlineInfo)
        {
            Segment2d seg01 = mlineInfo.LineInfo01.Line.ToSegment2d();
            Segment2d seg02 = mlineInfo.LineInfo02.Line.ToSegment2d();

            Line2d line2d01 = new Line2d(seg01.P0, seg01.Direction);

            double distance = Math.Sqrt(line2d01.DistanceSquared(seg02.P0)) * 0.5;

            Segment2d result;
            if (seg01.Length > seg02.Length)// 留长边
            {
                Vector2d direction01 = seg01.Direction.Rotate(Vector2d.Zero, Math.PI * 0.5 * (-1));
                Vector2d direction02 = seg01.Direction.Rotate(Vector2d.Zero, Math.PI * 0.5);

                Segment2d segment2d01 = new Segment2d(seg01.Center, seg01.Center + direction01 * 100);
                Polygon2d polygon2d01 = segment2d01.ToRenct2d(1000.0);
                if (polygon2d01.Intersects(seg02) || polygon2d01.Contains(seg02))
                {
                    result = seg01.Move(direction01, distance);
                }
                else
                {
                    result = seg01.Move(direction02, distance);
                }
            }
            else
            {
                Vector2d direction01 = seg02.Direction.Rotate(Vector2d.Zero, Math.PI * 0.5 * (-1));
                Vector2d direction02 = seg02.Direction.Rotate(Vector2d.Zero, Math.PI * 0.5);

                Segment2d segment2d01 = new Segment2d(seg02.Center, seg02.Center + direction01 * 100);
                Polygon2d polygon2d01 = segment2d01.ToRenct2d(1000.0);
                if (polygon2d01.Intersects(seg01) || polygon2d01.Contains(seg01))
                {
                    result = seg02.Move(direction01, distance);
                }
                else
                {
                    result = seg02.Move(direction02, distance);
                }
            }
            mlineInfo.polygon2d = result.ToRenct2d(500.0.MilliMeterToFeet());
            mlineInfo.Line = Line.CreateBound(result.P0.ToXYZ(), result.P1.ToXYZ());
        }
        /// <summary>
        /// 配对梁定位线和尺寸信息
        /// </summary>
        internal void MatchBookPositionLine()
        {
            // 基于基准线 提取梁的所有信息
            this.beamInfos = new List<ConBeamInfo>();
            foreach (var mlineInfo in mlineInfos)
            {
                ConBeamInfo conBeamInfo = new ConBeamInfo(mlineInfo);
                List<ConBeamType> conBeamTypes = new List<ConBeamType>();
                foreach (var item in this.ConBeamTypes)
                {
                    Vector2d p0 = new Vector2d(item.TextInfo.MinPoint.X, item.TextInfo.MinPoint.Y);
                    Vector2d p1 = new Vector2d(item.TextInfo.MaxPoint.X, item.TextInfo.MaxPoint.Y);

                    Segment2d segment2d = new Segment2d(p0, p1);

                    if (Math.Abs(mlineInfo.Line.Direction.DotProduct(item.TextInfo.Direction)).EqualPrecision(1.0))
                    {
                        if (mlineInfo.polygon2d.Contains(segment2d) || mlineInfo.polygon2d.Intersects(segment2d))
                        {
                            conBeamTypes.Add(item);
                        }
                    }
                }
                if (conBeamTypes.Count < 1) continue;
                ConBeamType conBeamType = conBeamTypes.Where(p => p.TextInfo.Text.Contains("加腋")).FirstOrDefault();
                if (conBeamType == null)
                {
                    conBeamType = conBeamTypes.FirstOrDefault();
                }
                conBeamInfo.ConBeamType = conBeamType;
                this.beamInfos.Add(conBeamInfo);

                // 翻遍数据单独进行图层处理
                BeamFlanging(conBeamInfo);
            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 梁 翻边处理
        /// </summary>
        /// <param name="p"></param>
        void BeamFlanging(ConBeamInfo p)
        {
            List<TextInfo> textInfos = new List<TextInfo>();
            string pattern = @"xxxxxx";

            foreach (var item in this.DwgParser.LayerNames)
            {
                if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                {
                    if (this.DwgParser.TexLayertInfos.ContainsKey(item))
                    {
                        textInfos.AddRange(this.DwgParser.TexLayertInfos[item]);
                    }
                }
            }

            foreach (var item in textInfos)
            {
                string text = item.Text;

                if (text[text.Length - 1] != ')')// 翻高数据以括号为结尾
                {
                    continue;
                }

                Vector2d p0 = new Vector2d(item.MinPoint.X, item.MinPoint.Y);
                Vector2d p1 = new Vector2d(item.MaxPoint.X, item.MaxPoint.Y);

                Segment2d segment2d = new Segment2d(p0, p1);
                if (p.MlineInfo.polygon2d.Contains(segment2d) || p.MlineInfo.polygon2d.Intersects(segment2d))
                {
                    p.BeamUpturinTextInfo = item;

                    break;
                }
            }
        }
        List<FamilyInstance> FamilyInstances { get; set; }
        internal override void OpenTrans()
        {
            this.FamilyInstances = new List<FamilyInstance>();

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.beamInfos.ForEach(p =>
                {
                    // 首先创建普通梁
                    FamilySymbol familySymbol = this.FamilySymbols.First();
                    //if (p.ConBeamType != null)
                    //{
                    familySymbol = this.FamilySymbols.Where(_p => _p.Name == "工字钢").FirstOrDefault();
                    //}
                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }

                    FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(p.Line, familySymbol, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;
                    this.FamilyInstances.Add(familyInstance);
                    this.ElementIds.Add(familyInstance.Id);

                    Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                    if (parameter != null && !parameter.IsReadOnly)
                    {
                        parameter.Set(0);
                    }
                    // Z 轴对正
                    parameter = familyInstance.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);
                    if (parameter != null && !parameter.IsReadOnly)
                    {
                        parameter.Set(2);
                    }
                    familyInstance.LookupParameter("B").Set(Convert.ToDouble(p.ConBeamType.B).MilliMeterToFeet());
                    familyInstance.LookupParameter("H").Set(Convert.ToDouble(p.ConBeamType.H).MilliMeterToFeet());
                    familyInstance.LookupParameter("U").Set(Convert.ToDouble(p.ConBeamType.U).MilliMeterToFeet());
                    familyInstance.LookupParameter("T").Set(Convert.ToDouble(p.ConBeamType.T).MilliMeterToFeet());
                    familyInstance.LookupParameter("D").Set(Convert.ToDouble(p.ConBeamType.D).MilliMeterToFeet());
                    familyInstance.LookupParameter("F").Set(Convert.ToDouble(p.ConBeamType.F).MilliMeterToFeet());

                    //familyInstance.LookupParameter("梁宽").Set(Convert.ToDouble(p.ConBeamType.U).MilliMeterToFeet());
                    //familyInstance.LookupParameter("梁高").Set(Convert.ToDouble(p.ConBeamType.H).MilliMeterToFeet());
                    //familyInstance.LookupParameter("梁宽_终点叠加").Set(0.0);
                    //familyInstance.LookupParameter("梁高_终点叠加").Set(0.0);

                });

                trans.Commit();
            }
        }
        internal override void SetLevel()
        {

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.FamilyInstances.ForEach(p =>
                {
                    Parameter parameter = p.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                    if (!parameter.IsReadOnly)
                    {
                        parameter.Set(CMD.Level.Id);
                    }
                });
                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                this.FamilyInstances.ForEach(p =>
                {
                    if (!p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble().EqualZreo())
                    {
                        p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                    }
                    if (!p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble().EqualZreo())
                    {
                        p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                    }
                });
                trans.Commit();
            }

            //using (Transaction trans = new Transaction(CMD.Doc, "修改翻高"))
            //{
            //    trans.DeleteErrOrWaringTaskDialog();
            //    trans.Start();
            //    this.beamInfos.ForEach(p =>
            //    {
            //        p.UpturnExcute();
            //        p.FamilyInstance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).Set(p.zValue.MilliMeterToFeet());
            //    });
            //    trans.Commit();
            //}

            //throw new NotImplementedException();
        }

        internal override void Move()
        {
            if (this.ElementIds.Count < 1) return;

            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                ElementTransformUtils.MoveElements(CMD.Doc, this.ElementIds, CMD.PositonMoveDis);
                transaction.Commit();
            }
            //throw new NotImplementedException();
        }
    }
}

