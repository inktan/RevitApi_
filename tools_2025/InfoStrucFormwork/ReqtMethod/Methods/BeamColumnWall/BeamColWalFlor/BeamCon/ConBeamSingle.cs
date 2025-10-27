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

namespace InfoStrucFormwork

{
    internal class ConBeamSingle : EleCreatInfo
    {
        /// <summary>
        /// 混凝土梁
        /// </summary>
        public ConBeamSingle() : base()
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
            //Move();
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
            // 锁定图层的正则表达式
            List<string> patterns = new List<string>()
            {
                @".*dsptext_beam\z",
                @".*dsptext_wallbeam\z",
                @".*G-ANNO-NPLT",
                @".*尺寸标注-梁"
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
            //textInfos.Count.ToString().TaskDialogErrorMessage();

            foreach (var item in textInfos)
            {
                if (item.Text.Contains('*') || item.Text.Contains('x'))
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
            //this.ConBeamTypes.Count.ToString().TaskDialogErrorMessage();
        }
        /// <summary>
        /// 载入 混凝土-矩形梁 族类型
        /// </summary>
        FamilySymbol BeamType { get; set; }
        /// <summary>
        /// 提取梁类型
        /// </summary>
        internal override void GetFamilySymbols()
        {
            try
            {
                Family beamType = (new FilteredElementCollector(CMD.Doc))
                 .OfClass(typeof(Family))
                 .First(p => p.Name == "混凝土-矩形梁") as Family;
                this.BeamType = beamType.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).Where(p => p.Name == "矩形梁").FirstOrDefault();

                if (this.BeamType == null)
                {
                    throw new NotImplementedException("请联系BIM协调员，载入GOA专属 混凝土-矩形梁");
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("请联系BIM协调员，载入GOA专属 混凝土-矩形梁");
            }
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

            List<string> patternNames = new List<string>() { @".*s\d*-beam-hddn", @".*s\d*-beam" };
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
        }
        /// <summary>
        /// 将散落的线段配对成多线
        /// </summary>
        /// <param name="lineInfos"></param>
        /// <returns></returns>
        internal static IEnumerable<MlineInfo> LinePair(List<LineInfo> lineInfos)
        {
            List<LineInfo> collectLineInfos = new List<LineInfo>();
            foreach (var lineInfo01 in lineInfos)
            {
                if (lineInfo01.Line == null) continue;
                if (collectLineInfos.Contains(lineInfo01)) continue;

                MlineInfo mlineInfo = new MlineInfo();
                mlineInfo.LineInfo01 = lineInfo01;
                mlineInfo.LineInfo02 = lineInfo01;
                LineToMul(mlineInfo);

                collectLineInfos.Add(lineInfo01);

                yield return mlineInfo;
            }
        }
        static public void LineToMul(MlineInfo mlineInfo)
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
                List<ConBeamType> allParConBeamTypes = new List<ConBeamType>(); //获取所有平行文字
                foreach (var item in this.ConBeamTypes.Where(p => p.typeInt == 1))
                {
                    Vector2d p0 = new Vector2d(item.TextInfo.MinPoint.X, item.TextInfo.MinPoint.Y);
                    Vector2d p1 = new Vector2d(item.TextInfo.MaxPoint.X, item.TextInfo.MaxPoint.Y);

                    Vector2d textCenter = (p0 + p1) * 0.5;

                    if (Math.Abs(mlineInfo.Line.Direction.DotProduct(item.TextInfo.Direction)).EqualPrecision(1.0))
                    {
                        if (mlineInfo.polygon2d.OutwardOffeet(2.0).Contains(textCenter))
                        {
                            conBeamTypes.Add(item);
                        }
                        allParConBeamTypes.Add(item);
                    }
                }

                //如果没有相交的尺寸文字
                //则求距离最近的文字
                if (conBeamTypes.Count < 1)
                {
                    conBeamTypes = allParConBeamTypes.Select(item =>
                    {
                        Vector2d p0 = new Vector2d(item.TextInfo.MinPoint.X, item.TextInfo.MinPoint.Y);
                        double distane = mlineInfo.polygon2d.Center().Distance(p0);
                        return new { Item = item, Distance = distane };
                    }).OrderBy(x => x.Distance).Select(p => p.Item).ToList();
                };

                if (conBeamTypes.Count < 1)
                {
                    continue;
                };

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
                foreach (var p in this.beamInfos)
                {
                    try
                    {

                        // 首先创建普通梁
                        if (!this.BeamType.IsActive)
                        {
                            this.BeamType.Activate();
                        }

                        Curve curve = Line.CreateBound(p.Line.GetEndPoint(0) + CMD.PositonMoveDis, p.Line.GetEndPoint(1) + CMD.PositonMoveDis);

                        FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(curve, this.BeamType, CMD.Doc.ActiveView.GenLevel, StructuralType.Beam); ;
                        this.FamilyInstances.Add(familyInstance);
                        this.ElementIds.Add(familyInstance.Id);

                        if (p.ConBeamType.Width != 0)
                            familyInstance.LookupParameter("宽度").Set(Convert.ToDouble(p.ConBeamType.Width).MilliMeterToFeet());
                        if (p.ConBeamType.Height != 0)
                            familyInstance.LookupParameter("高度").Set(Convert.ToDouble(p.ConBeamType.Height).MilliMeterToFeet());

                        // 结构分析
                        Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);
                        if (parameter != null && !parameter.IsReadOnly)
                            parameter.Set(0);
                        // 全部刷成无连接模式
                        parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCT_FRAM_JOIN_STATUS);
                        if (parameter != null && !parameter.IsReadOnly)
                            parameter.Set(1);
                        try
                        {
                            StructuralFramingUtils.DisallowJoinAtEnd(familyInstance, 0);
                            StructuralFramingUtils.DisallowJoinAtEnd(familyInstance, 1);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //throw;
                    }
       
                }

                trans.Commit();
            }
        }
        internal override void SetLevel()
        {

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                foreach (var p in this.FamilyInstances)
                {
                    try
                    {
                        Parameter parameter = p.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //throw;
                    }
                }

                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                foreach (var p in this.FamilyInstances)
                {
                    try
                    {
                        if (!p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble().EqualZreo())
                        {
                            p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                        }
                        if (!p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble().EqualZreo())
                        {
                            p.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //throw;
                    }
                }

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
                foreach (var item in this.ElementIds)
                {
                    try
                    {
                        ElementTransformUtils.MoveElement(CMD.Doc, item, CMD.PositonMoveDis);

                    }
                    catch (Exception)
                    {
                        continue;
                        //throw;
                    }

                }

                //ElementTransformUtils.MoveElements(CMD.Doc, this.ElementIds, CMD.PositonMoveDis);
                transaction.Commit();
            }
            //throw new NotImplementedException();
        }
    }
}

