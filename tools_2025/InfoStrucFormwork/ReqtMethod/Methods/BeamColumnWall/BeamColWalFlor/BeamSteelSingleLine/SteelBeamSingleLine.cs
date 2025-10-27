using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TeighaNet;

namespace InfoStrucFormwork
{
    internal class SteelBeamSingleLine : EleCreatInfo
    {
        /// <summary>
        /// 混凝土梁
        /// </summary>
        public SteelBeamSingleLine() : base()
        {
        }

        internal override void Execute()
        {
            // 属性初始化
            this.ElementIds = new List<ElementId>();

            ExGeoInfo();
            // 梁类型确认
            GetFamilySymbols();
            OpenTrans();
            SetLevel();
            Move();
            System.Windows.Forms.Application.DoEvents();// 窗体重绘
        }

        List<SteelBeamInfo> steelBeamInfos { get; set; }
        List<TextInfo> TextInfos { get; set; }
        List<LineInfo> lineInfos { get; set; }
        List<PolyLineInfo> polyLineInfos { get; set; }

        List<PolyLineInfo> infoPolyLineInfos { get; set; }
        List<LineInfo> infoLineInfos { get; set; }
        List<ArcInfo> arcInfos { get; set; }
        /// <summary>
        /// 提取所有钢梁的定位线-单线
        /// </summary>
        internal override void ExGeoInfo()
        {
            List<string> patternTexts = new List<string>()
            {
                @"s\d*-bmht-text",
                @"s\d*-bmht-iden",
                @"s\d*-text",
                @"dsptext_beam\z",
            };

            TextInfos = new List<TextInfo>();
            infoPolyLineInfos = new List<PolyLineInfo>();
            infoLineInfos = new List<LineInfo>();

            List<string> patternSteelBeams = new List<string>()
            {
                @"s\d*-gl",
                @"s\d*-gkl",
                @"s\d*-steel-cbeam",
                @"s\d*-beam-gl"
            };

            lineInfos = new List<LineInfo>();
            polyLineInfos = new List<PolyLineInfo>();
            arcInfos = new List<ArcInfo>();

            foreach (var item in this.DwgParser.LayerNames)
            {
                foreach (var patternText in patternTexts)
                {
                    if (Regex.IsMatch(item, patternText, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.TexLayertInfos.ContainsKey(item))
                        {
                            TextInfos.AddRange(this.DwgParser.TexLayertInfos[item]);
                        }
                        if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                        {
                            infoPolyLineInfos.AddRange(this.DwgParser.PolyLineLayerInfos[item]);
                        }
                        if (this.DwgParser.LineLayerInfos.ContainsKey(item))
                        {
                            infoLineInfos.AddRange(this.DwgParser.LineLayerInfos[item]);
                        }
                        if (this.DwgParser.ArcLayerInfos.ContainsKey(item))
                        {
                            arcInfos.AddRange(this.DwgParser.ArcLayerInfos[item]);
                        }
                    }
                }

                foreach (var patternSteelBeam in patternSteelBeams)
                {
                    if (Regex.IsMatch(item, patternSteelBeam, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.LineLayerInfos.ContainsKey(item))
                        {
                            lineInfos.AddRange(this.DwgParser.LineLayerInfos[item]);
                        }
                        if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                        {
                            polyLineInfos.AddRange(this.DwgParser.PolyLineLayerInfos[item]);
                        }
                        if (this.DwgParser.ArcLayerInfos.ContainsKey(item))
                        {
                            arcInfos.AddRange(this.DwgParser.ArcLayerInfos[item]);
                        }
                    }
                }
            }

            // 基于基准线 提取梁的所有信息
            this.steelBeamInfos = new List<SteelBeamInfo>();

            foreach (var polyLineInfo in polyLineInfos)
            {
                LineInfo lineInfo = new LineInfo();
                XYZ p0 = polyLineInfo.Pts.First();
                XYZ p1 = polyLineInfo.Pts.Last();

                lineInfo.Line = Line.CreateBound(p0, p1);
                lineInfo.Segment2d = new Segment2d(p0.ToVector2d(), p1.ToVector2d());
                lineInfos.Add(lineInfo);
            }

            foreach (var lineInfo in lineInfos)
            {
                SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                steelBeamInfo.BaseLineInfo = lineInfo;
                steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(3.0);

                this.steelBeamInfos.Add(steelBeamInfo);
            }

            foreach (var item in arcInfos)
            {
                SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                steelBeamInfo.ArcInfo = item;

                this.steelBeamInfos.Add(steelBeamInfo);
            }

            //throw new NotImplementedException();
        }
        /// <summary>
        /// 与梁线有关的文字信息抓取
        /// </summary>
        void InforCapture()
        {
            foreach (var steelBeamInfo in this.steelBeamInfos)
            {
                List<TextInfo> textInfos = new List<TextInfo>();
                foreach (var item in TextInfos)
                {
                    string[] parts = item.Text.Split('x');
                    if (parts.Length < 4) continue;
                    Vector2d p0 = new Vector2d(item.MinPoint.X, item.MinPoint.Y);
                    Vector2d p1 = new Vector2d(item.MaxPoint.X, item.MaxPoint.Y);
                    Segment2d segment2d = new Segment2d(p0, p1);
                    if (Math.Abs(steelBeamInfo.BaseLineInfo.Line.Direction.DotProduct(item.Direction)) > Math.Cos(Math.PI / 4.0)) // 夹角小于45°
                    {
                        if (steelBeamInfo.Polygon2d.Contains(segment2d) || steelBeamInfo.Polygon2d.Intersects(segment2d))
                        {
                            textInfos.Add(item);
                        }
                    }
                }

                //则求距离最近的文字
                if (textInfos.Count >= 1)
                {
                    textInfos = textInfos.Select(item =>
                    {
                        Vector2d p0 = new Vector2d(item.MinPoint.X, item.MinPoint.Y);
                        double distane = steelBeamInfo.Polygon2d.Center().Distance(p0);
                        return new { Item = item, Distance = distane };
                    }).OrderBy(x => x.Distance).Select(p => p.Item).ToList();
                };

                steelBeamInfo.TextInfo = textInfos.FirstOrDefault();

            }
        }
        FamilySymbol fs { get; set; }
        internal override void OpenTrans()
        {
            InforCapture();
            //return;

            using (Transaction trans = new Transaction(CMD.Doc, "创建族实例"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.steelBeamInfos.ForEach(p =>
                {
                    FamilySymbol familySymbol = this.fs;
                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }
                    // 存在创建失败的情况
                    p.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(p.BaseLineInfo.Line, familySymbol, CMD.Level, StructuralType.Beam);
                    //CMD.Doc.CreateDirectShape(new List<GeometryObject>() { p.Curve });
                    string input = @"H600x200x12x20";
                    if (p.TextInfo != null)
                    {
                        input = p.TextInfo.Text;
                    }
                    string[] parts = input.Split('x');

                    if (parts.Length == 4)
                    {
                        string hStr = parts[0].Substring(1); // 去掉开头的 "H"
                        if (double.TryParse(hStr, out double h) &&
                            double.TryParse(parts[1], out double w) &&
                            double.TryParse(parts[2], out double t1) &&
                            double.TryParse(parts[3], out double t2))
                        {
                            if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                            {
                                this.ElementIds.Add(p.FamilyInstance.Id);
                                p.FamilyInstance.LookupParameter("高度").Set(h.MilliMeterToFeet());
                                p.FamilyInstance.LookupParameter("宽度").Set(w.MilliMeterToFeet());
                                p.FamilyInstance.LookupParameter("腹板").Set(t1.MilliMeterToFeet());
                                p.FamilyInstance.LookupParameter("翼缘").Set(t2.MilliMeterToFeet());
                            }
                        }
                    }
                });
                trans.Commit();
            }
        }
        internal override void SetLevel()
        {
            // 定位标高
            using (Transaction trans = new Transaction(CMD.Doc, "---"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                foreach (var item in this.steelBeamInfos)
                {
                    FamilyInstance familyInstance = item.FamilyInstance;
                    if (familyInstance != null && familyInstance.IsValidObject)
                    {
                        Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(CMD.Level.Id);
                        }
                    }
                }
                trans.Commit();
            }
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                foreach (var item in this.steelBeamInfos)
                {
                    try
                    {
                        if (!item.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble().EqualZreo())
                        {
                            item.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                        }
                        if (!item.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble().EqualZreo())
                        {
                            item.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                        }
                        item.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0);
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
        internal override void Move()
        {
            List<ElementId> eleIds = new List<ElementId>();
            eleIds.AddRange(this.steelBeamInfos.Where(p => p.FamilyInstance != null && p.FamilyInstance.IsValidObject).Select(p => p.FamilyInstance.Id));

            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                if (this.steelBeamInfos.Where(p => p.FamilyInstance != null).Count() > 0)
                {
                    ElementTransformUtils.MoveElements(CMD.Doc, eleIds, CMD.PositonMoveDis);
                }
                transaction.Commit();
            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 创建梁类型
        /// </summary>
        internal override void GetFamilySymbols()
        {
            this.fs = CMD.Doc.FamilySymbolByName("H型钢", "热轧H型钢");
        }
    }
}

