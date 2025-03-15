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
        List<TextInfo> textInfos { get; set; }
        List<LineInfo> lineInfos { get; set; }
        List<PolyLineInfo> polyLineInfos { get; set; }

        List<PolyLineInfo> infoPolyLineInfos { get; set; }
        List<LineInfo> infoLineInfos { get; set; }
        List<ArcInfo> arcInfos { get; set; }
        List<SplineInfo> infoPlineInfos { get; set; }
        /// <summary>
        /// 基准屋顶
        /// </summary>
        //RoofBase roofBase { get; set; }
        List<Face> roofFaces { get; set; }
        /// <summary>
        /// 提取所有钢梁的定位线-单线
        /// </summary>
        internal override void ExGeoInfo()
        {
            if (ViewModel.Instance.ConsiderRoof)// 屋顶特殊处理
            {
                //roofBase = new FilteredElementCollector(CMD.Doc).OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsNotElementType().FirstOrDefault(p => p.LevelId == CMD.Level.Id) as RoofBase;
                //Element element = new FilteredElementCollector(CMD.Doc).OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsNotElementType().FirstOrDefault();
                //roofBase = element as RoofBase;

                //// 获取屋顶的最低表面
                //List<Face> faces = roofBase.GetAllFaces().OrderByDescending(p => p.Area).ToList();
                //Face roofFace = faces[0];
                //if (faces[0].Triangulate().Vertices.OrderBy(p => p.Z).Last().Z > faces[1].Triangulate().Vertices.OrderBy(p => p.Z).Last().Z)
                //{
                //    roofFace = faces[1];
                //}
                roofFaces = new List<Face>();

                Reference reference = CMD.Sel.PickObject(ObjectType.PointOnElement);
                Element element = CMD.Doc.GetElement(reference.ElementId);
                if (element is RevitLinkInstance)
                {
                    RevitLinkInstance revitLinkInstance = element as RevitLinkInstance;
                    Document linkDoc = revitLinkInstance.GetLinkDocument();
                    Element linkEle = linkDoc.GetElement(reference.LinkedElementId);

                    Reference linkRefer = reference.CreateReferenceInLink();
                    Face face = linkEle.GetGeometryObjectFromReference(linkRefer) as Face;//通过reference获取几何

                    roofFaces.Add(face);
                }
                else
                {
                    roofFaces.Add(element.GetGeometryObjectFromReference(reference) as Face);
                }
            }

            List<string> patternTexts = new List<string>()
            {
                @"s\d*-bmht-text",
                @"s\d*-bmht-iden",
                @"s\d*-text",
                @"dsptext_beam\z",
            };

            textInfos = new List<TextInfo>();
            infoPolyLineInfos = new List<PolyLineInfo>();
            infoLineInfos = new List<LineInfo>();

            List<string> patternSteelBeams = new List<string>()
            {
                @"s\d*-gl",
                @"s\d*-gkl",
                @"s\d*-steel-cbeam",
                @"s\d*-beam-hddn"
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
                            textInfos.AddRange(this.DwgParser.TexLayertInfos[item]);
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

            foreach (var lineInfo in lineInfos)
            {
                SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                steelBeamInfo.TextInfos = new List<TextInfo>();
                steelBeamInfo.BaseLineInfo = lineInfo;
                steelBeamInfo.BaseLineInfo = lineInfo;
                steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(2.0);

                this.steelBeamInfos.Add(steelBeamInfo);
            }

            foreach (var item in arcInfos)
            {
                SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                steelBeamInfo.TextInfos = new List<TextInfo>();
                steelBeamInfo.ArcInfo = item;

                this.steelBeamInfos.Add(steelBeamInfo);
            }

            // 处理多段线-无加腋节点-一端有加腋节点-两端有加腋节点
            foreach (var item in polyLineInfos)
            {
                if (item.Pts.Count > 1)
                {
                    LineInfo lineInfo = new LineInfo();
                    lineInfo.Segment2d = new Segment2d(item.Polygon2d.Vertices[0], item.Polygon2d.Vertices[item.Polygon2d.VertexCount - 1]);
                    lineInfo.Line = lineInfo.Segment2d.ToLine();

                    SteelBeamInfo steelBeamInfo = new SteelBeamInfo();
                    steelBeamInfo.BaseLineInfo = lineInfo;
                    steelBeamInfo.BaseLineInfo = lineInfo;
                    steelBeamInfo.Polygon2d = lineInfo.Segment2d.ToRenct2d(2.0);

                    this.steelBeamInfos.Add(steelBeamInfo);
                }

            }

            //throw new NotImplementedException();
        }
        /// <summary>
        /// 辅助墙体用于提取屋顶弧梁的定位线
        /// </summary>
        List<ElementId> wallIds { get; set; }
        /// <summary>
        /// 与梁线有关的文字信息抓取
        /// </summary>
        void InforCapture()
        {
            wallIds = new List<ElementId>();

            foreach (var steelBeamInfo in this.steelBeamInfos)
            {
                if (steelBeamInfo.ArcInfo != null)
                {
                    steelBeamInfo.Excute(this.FamilySymbols);
                    steelBeamInfo.ProjectToRoof(roofFaces);
                    wallIds.Add(steelBeamInfo.Wall.Id);
                }
                else
                {

                    steelBeamInfo.TextInfos = new List<TextInfo>();
                    foreach (var item in textInfos)
                    {
                        Vector2d p0 = new Vector2d(item.MinPoint.X, item.MinPoint.Y);
                        Vector2d p1 = new Vector2d(item.MaxPoint.X, item.MaxPoint.Y);

                        Segment2d segment2d = new Segment2d(p0, p1);

                        if (Math.Abs(steelBeamInfo.BaseLineInfo.Line.Direction.DotProduct(item.Direction)).EqualPrecision(1.0))// 平行关系
                        {
                            // 找到与梁线有关的所有文字注释
                            if (steelBeamInfo.Polygon2d.Contains(segment2d) || steelBeamInfo.Polygon2d.Intersects(segment2d))
                            {
                                steelBeamInfo.TextInfos.Add(item);
                            }
                        }
                    }
                    steelBeamInfo.Excute(this.FamilySymbols);
                    steelBeamInfo.ProjectToRoof(roofFaces);
                    // 删除辅助墙体
                    if (steelBeamInfo.Wall != null && steelBeamInfo.Wall.IsValidObject)
                    {
                        wallIds.Add(steelBeamInfo.Wall.Id);
                    }
                    steelBeamInfo.HaunchedNodes.ForEach(p =>
                    {
                        p.ProjectToRoof(roofFaces);
                        if (p.Wall != null && p.Wall.IsValidObject)
                        {
                            wallIds.Add(p.Wall.Id);
                        }
                    });
                }
            }
            CMD.Doc.DelEleIds(this.wallIds);
        }

        List<FamilySymbol> FamilySymbols { get; set; }
        internal override void OpenTrans()
        {
            InforCapture();
            //return;

            using (Transaction trans = new Transaction(CMD.Doc, "创建族实例"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.steelBeamInfos.Where(p => p.Curve != null).ToList().ForEach(p =>
                    {
                        FamilySymbol familySymbol = this.FamilySymbols.First();
                        if (p.TextInfos != null)
                        {
                            familySymbol = this.FamilySymbols.Where(_p => _p.Name.Contains(p.typeName)).FirstOrDefault();
                            if (familySymbol == null)
                            {
                                familySymbol = this.FamilySymbols.First();
                            }
                        }
                        if (!familySymbol.IsActive)
                        {
                            familySymbol.Activate();
                        }
                        // 存在创建失败的情况
                        p.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(p.Curve, familySymbol, CMD.Level, StructuralType.Beam);
                        //CMD.Doc.CreateDirectShape(new List<GeometryObject>() { p.Curve });
                        if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                        {
                            this.ElementIds.Add(p.FamilyInstance.Id);
                        }

                        p.HaunchedNodes.Where(_p => _p.Curve != null).ToList().ForEach(_p =>
                            {
                                string haunchedNodeTypeName = p.typeName + @" " + _p.typeName;
                                if (haunchedNodeTypeName.Contains("变H"))
                                {
                                    haunchedNodeTypeName = _p.typeName;
                                }

                                // 创建加腋类型
                                FamilySymbol haunchedNodeFs = this.FamilySymbols.Where(fs => fs.Name == haunchedNodeTypeName).FirstOrDefault();
                                if (haunchedNodeFs == null)
                                {
                                    haunchedNodeFs = this.FamilySymbols.First(fs => fs.FamilyName == "钢梁加腋节点").Duplicate(haunchedNodeTypeName) as FamilySymbol;
                                    this.FamilySymbols.Add(haunchedNodeFs);

                                    haunchedNodeFs.LookupParameter("b").Set(familySymbol.LookupParameter("宽度").AsDouble());
                                    haunchedNodeFs.LookupParameter("h").Set(familySymbol.LookupParameter("高度").AsDouble());
                                    haunchedNodeFs.LookupParameter("tw").Set(familySymbol.LookupParameter("腹杆厚度").AsDouble());
                                    haunchedNodeFs.LookupParameter("tf").Set(familySymbol.LookupParameter("法兰厚度").AsDouble());

                                    if (_p.typeName.Contains("SA"))
                                    {
                                        haunchedNodeFs.LookupParameter("b1").Set(familySymbol.LookupParameter("宽度").AsDouble() + _p.HaunchedNodeSizeDifference * 2);
                                        haunchedNodeFs.LookupParameter("h1").Set(familySymbol.LookupParameter("高度").AsDouble());
                                    }
                                    else
                                    {
                                        haunchedNodeFs.LookupParameter("b1").Set(familySymbol.LookupParameter("宽度").AsDouble());
                                        haunchedNodeFs.LookupParameter("h1").Set(familySymbol.LookupParameter("高度").AsDouble() + _p.HaunchedNodeSizeDifference);
                                    }
                                    haunchedNodeFs.LookupParameter("tw1").Set(familySymbol.LookupParameter("腹杆厚度").AsDouble());
                                    haunchedNodeFs.LookupParameter("tf1").Set(familySymbol.LookupParameter("法兰厚度").AsDouble());
                                }

                                if (!haunchedNodeFs.IsActive)
                                {
                                    haunchedNodeFs.Activate();
                                }
                                _p.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(_p.Curve, haunchedNodeFs, CMD.Level, StructuralType.Beam);
                                //CMD.Doc.CreateDirectShape(new List<GeometryObject>() { _p.Curve });
                                if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                                {
                                    this.ElementIds.Add(_p.FamilyInstance.Id);
                                }
                            });
                    });

                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "修复创建失败的钢梁"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.steelBeamInfos.Where(p => p.Curve != null).ToList().ForEach(p =>
                {
                    FamilySymbol familySymbol = this.FamilySymbols.First();
                    if (p.TextInfos != null)
                    {
                        familySymbol = this.FamilySymbols.Where(_p => _p.Name.Contains(p.typeName + @" ")).FirstOrDefault();
                        if (familySymbol == null)
                        {
                            familySymbol = this.FamilySymbols.First();
                        }
                    }
                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }
                    // 存在创建失败的情况
                    if (p.FamilyInstance == null || !p.FamilyInstance.IsValidObject)
                    {
                        // 重建基准线
                        p.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(Line.CreateBound(p.Curve.GetEndPoint(0), p.Curve.GetEndPoint(1)), familySymbol, CMD.Level, StructuralType.Beam);
                    }
                    if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                    {
                        this.ElementIds.Add(p.FamilyInstance.Id);
                    }

                    p.HaunchedNodes.Where(_p => _p.Curve != null).ToList().ForEach(_p =>
                    {
                        string haunchedNodeTypeName = _p.typeName;

                        // 创建加腋类型
                        FamilySymbol haunchedNodeFs = this.FamilySymbols.Where(fs => fs.Name == haunchedNodeTypeName).FirstOrDefault();

                        if (!haunchedNodeFs.IsActive)
                        {
                            haunchedNodeFs.Activate();
                        }
                        if (_p.FamilyInstance == null || !_p.FamilyInstance.IsValidObject)
                        {
                            // 重建基准线
                            _p.FamilyInstance = CMD.Doc.Create.NewFamilyInstance(Line.CreateBound(_p.Curve.GetEndPoint(0), _p.Curve.GetEndPoint(1)), haunchedNodeFs, CMD.Level, StructuralType.Beam);
                        }
                        if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                        {
                            this.ElementIds.Add(_p.FamilyInstance.Id);
                        }
                    });

                });

                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "取消结构分析"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.steelBeamInfos.ForEach(p =>
                {
                    if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                    {
                        Parameter parameter = p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);// 取消结构分析模型
                        if (parameter != null && !parameter.IsReadOnly)
                        {
                            parameter.Set(0);
                        }
                    }

                    p.HaunchedNodes.ForEach(_p =>
                    {
                        if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                        {
                            Parameter parameter = _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL);// 取消结构分析模型
                            if (parameter != null && !parameter.IsReadOnly)
                            {
                                parameter.Set(0);
                            }
                        }
                    });
                });
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

                    foreach (var item02 in item.HaunchedNodes)
                    {
                        familyInstance = item02.FamilyInstance;
                        if (familyInstance != null && familyInstance.IsValidObject)
                        {
                            Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                            if (!parameter.IsReadOnly)
                            {
                                parameter.Set(CMD.Level.Id);
                            }
                        }
                    }
                }
                trans.Commit();
            }
            // 起点 终点 标高偏移 不考虑屋顶空间
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                if (!ViewModel.Instance.ConsiderRoof)
                {
                    this.steelBeamInfos.ForEach(p =>
                    {
                        if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                        {
                            if (!p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).IsReadOnly)
                            {
                                p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                            }
                            if (!p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).IsReadOnly)
                            {
                                p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                            }
                        }

                        p.HaunchedNodes.ForEach(_p =>
                        {
                            if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                            {
                                if (!_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).IsReadOnly)
                                {
                                    _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                                }
                                if (!_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).IsReadOnly)
                                {
                                    _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                                }
                            }
                        });
                    });
                }
                trans.Commit();
            }
            // 起点 终点 标高偏移 考虑屋顶空间 ==> 先归零，再设置原来的偏移数值 解决个别地方显示不正确的问题
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                if (ViewModel.Instance.ConsiderRoof)
                {
                    this.steelBeamInfos.ForEach(p =>
                    {
                        if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                        {
                            if (!p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).IsReadOnly)
                            {
                                p.END0 = p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
                                p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                            }
                            if (!p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).IsReadOnly)
                            {
                                p.END1 = p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();
                                //p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                            }
                        }

                        p.HaunchedNodes.ForEach(_p =>
                        {
                            if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                            {
                                if (!_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).IsReadOnly)
                                {
                                    _p.END0 = _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
                                    _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.0);
                                }
                                if (!_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).IsReadOnly)
                                {
                                    _p.END1 = _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();
                                    //_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.0);
                                }
                            }
                        });
                    });
                }
                trans.Commit();
            }
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                if (ViewModel.Instance.ConsiderRoof)
                {
                    this.steelBeamInfos.ForEach(p =>
                    {
                        if (p.FamilyInstance != null && p.FamilyInstance.IsValidObject)
                        {
                            if (!p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).IsReadOnly)
                            {
                                p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(p.END0);
                            }
                            //if (!p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).IsReadOnly)
                            //{
                            //    p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(p.END1);
                            //}
                        }

                        p.HaunchedNodes.ForEach(_p =>
                        {
                            if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                            {
                                if (!_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).IsReadOnly)
                                {
                                    _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(_p.END0);
                                }
                                //if (!_p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).IsReadOnly)
                                //{
                                //    _p.FamilyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(_p.END1);
                                //}
                            }
                        });
                    });
                }
                trans.Commit();
            }
            // 横截面旋转
            using (Transaction trans = new Transaction(CMD.Doc, "横截面旋转"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                foreach (var item in this.steelBeamInfos)
                {
                    FamilyInstance familyInstance = item.FamilyInstance;
                    if (familyInstance != null && familyInstance.IsValidObject)
                    {
                        Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);
                        if (!parameter.IsReadOnly)
                        {
                            parameter.Set(0.0);
                        }
                    }

                    foreach (var item02 in item.HaunchedNodes)
                    {
                        familyInstance = item02.FamilyInstance;
                        if (familyInstance != null && familyInstance.IsValidObject)
                        {
                            Parameter parameter;
                            if (familyInstance.Name.Contains("KD"))// KD加腋类型需要翻转
                            {
                                parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);
                                if (!parameter.IsReadOnly)
                                {
                                    parameter.Set(Math.PI);// 使用角度表达向上加腋
                                }
                                parameter = familyInstance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE);// 1、使用角度表达向上加腋 2、同时要降低一个梁自身高度
                                if (!parameter.IsReadOnly)
                                {
                                    parameter.Set(familyInstance.Symbol.LookupParameter("h").AsDouble());
                                }
                            }
                            else
                            {
                                parameter = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);
                                if (!parameter.IsReadOnly)
                                {
                                    parameter.Set(0.0);
                                }
                                parameter = familyInstance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE);// 1、使用角度表达向上加腋 2、同时要降低一个梁自身高度
                                if (!parameter.IsReadOnly)
                                {
                                    parameter.Set(0.0);
                                }
                            }
                        }
                    }
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(CMD.Doc, "修改翻高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                this.steelBeamInfos.ForEach(p =>
                {
                    //p.FamilyInstance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).Set(p.zValue.MilliMeterToFeet());
                });
                trans.Commit();
            }

            //throw new NotImplementedException();
        }
        internal override void Move()
        {
            List<ElementId> eleIds = new List<ElementId>();
            eleIds.AddRange(this.steelBeamInfos.Where(p => p.FamilyInstance != null && p.FamilyInstance.IsValidObject).Select(p => p.FamilyInstance.Id));
            eleIds.AddRange(this.steelBeamInfos.Where(p => p.HaunchedNodes.Count > 0).SelectMany(p => p.HaunchedNodes).Where(p => p.FamilyInstance != null && p.FamilyInstance.IsValidObject).Select(p => p.FamilyInstance.Id));

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
            Family Fa = (new FilteredElementCollector(CMD.Doc))
                .OfClass(typeof(Family))
                .First(p => p.Name == "热轧H型钢") as Family;//

            this.FamilySymbols = Fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).ToList();

            GetSteelInfoTable();
            foreach (var item in this.SteelTypeTable)
            {
                if (!this.FamilySymbols.Select(p => p.Name).Contains(item.DetailedName))
                {
                    using (Transaction transaction = new Transaction(CMD.Doc, "创建 梁 类型"))
                    {
                        transaction.Start();

                        FamilySymbol familySymbol = this.FamilySymbols.First().Duplicate(item.DetailedName) as FamilySymbol;
                        try
                        {
                            this.FamilySymbols.Add(familySymbol);

                            string specifications = item.Specifications;
                            List<string> sizeDouble = specifications.Substring(specifications.IndexOf('H') + 1, specifications.Length - specifications.IndexOf('H') - 1).Split('x').ToList();

                            familySymbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT).Set(Convert.ToDouble(sizeDouble[0]).MilliMeterToFeet());// 高度
                            familySymbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH).Set(Convert.ToDouble(sizeDouble[1]).MilliMeterToFeet());// 宽度
                            familySymbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_ISHAPE_WEBTHICKNESS).Set(Convert.ToDouble(sizeDouble[2]).MilliMeterToFeet());// 腹杆厚度
                            familySymbol.get_Parameter(BuiltInParameter.STRUCTURAL_SECTION_ISHAPE_FLANGETHICKNESS).Set(Convert.ToDouble(sizeDouble[3]).MilliMeterToFeet());// 法兰厚度
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                        transaction.Commit();
                    }
                }
            }
            Fa = (new FilteredElementCollector(CMD.Doc))
                .OfClass(typeof(Family))
                .First(p => p.Name == "钢梁加腋节点") as Family;

            this.FamilySymbols.AddRange(Fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol));
        }

        List<SteelTable> SteelTypeTable { get; set; }
        /// <summary>
        /// 提取钢结构类型表格
        /// </summary>
        void GetSteelInfoTable()
        {
            // [01] 获取钢构件规格表
            string steelIndexStr = "钢构件规格表";
            List<TextInfo> textInPoly = new List<TextInfo>();
            List<Segment2d> lineInPolyHor = new List<Segment2d>();
            List<Segment2d> lineInPolyVer = new List<Segment2d>();

            foreach (var textInfo in this.textInfos)
            {
                if (textInfo.Text.StartsWith(steelIndexStr))
                {
                    Vector2d position = textInfo.Position.ToVector2d();
                    PolyLineInfo poly = infoPolyLineInfos.Where(p => p.Polygon2d != null && p.Polygon2d.IsRectangle()).OrderBy(p => position.DistanceSquared(p.Polygon2d.LUpOfBox2d())).FirstOrDefault();
                    if (poly != null)
                    {
                        foreach (var text in textInfos)
                        {
                            if (poly.Polygon2d.Contains(text.Center.ToVector2d()))
                            {
                                textInPoly.Add(text);
                            }
                        }
                        foreach (var item in this.infoPolyLineInfos)
                        {
                            if (item.Pts.Count() == 2)
                            {
                                Segment2d tmp = new Segment2d(item.Pts[0].ToVector2d(), item.Pts[1].ToVector2d());
                                if (poly.Polygon2d.Contains(tmp.Center))
                                {
                                    if (tmp.Direction.y.EqualZreo())
                                    {
                                        lineInPolyHor.Add(tmp);
                                    }
                                    else
                                    {
                                        lineInPolyVer.Add(tmp);
                                    }
                                }
                            }
                        }

                        foreach (var line in infoLineInfos)
                        {
                            if (poly.Polygon2d.Contains(line.Center.ToVector2d()))
                            {
                                if (line.Segment2d.Direction.y.EqualZreo())
                                {
                                    lineInPolyHor.Add(line.Segment2d);
                                }
                                else
                                {
                                    lineInPolyVer.Add(line.Segment2d);
                                }
                            }
                        }
                    }
                    break;
                }
            }

            lineInPolyHor = lineInPolyHor
                .Where((x, i) => lineInPolyHor.FindIndex(s => s.Center.x.EqualPrecision(x.Center.x) && s.Center.y.EqualPrecision(x.Center.y)) == i)
                .OrderBy(p => p.Center.y)
                .ToList();
            lineInPolyVer = lineInPolyVer
                .Where((x, i) => lineInPolyVer.FindIndex(s => s.Center.x.EqualPrecision(x.Center.x) && s.Center.y.EqualPrecision(x.Center.y)) == i)
                .OrderBy(p => p.Center.x)
                .ToList();

            // 每一行中的列元素配对
            List<List<TextInfo>> textRranks = new List<List<TextInfo>>();
            List<TextInfo> textCol = new List<TextInfo>();

            for (int i = 0; i < lineInPolyHor.Count; i++)// 从表格中按顺序提取文字
            {
                List<TextInfo> temp = new List<TextInfo>();
                for (int j = 0; j < lineInPolyVer.Count; j++)
                {
                    foreach (var item in textInPoly)
                    {
                        if (textCol.Contains(item)) continue;// 过滤

                        if (item.Center.X < lineInPolyVer[j].Center.x && item.Center.Y < lineInPolyHor[i].Center.y)
                        {
                            temp.Add(item);
                            textCol.Add(item);
                        }
                    }
                }
                if (temp.Count == 0) continue;// 过滤
                textRranks.Add(temp);
            }

            SteelTypeTable = new List<SteelTable>();
            foreach (var item in textRranks)
            {
                SteelTable strucSteelTable = new SteelTable();
                SteelTypeTable.Add(strucSteelTable);

                strucSteelTable.TypeName = item[0].Text;// 钢梁字母型号
                strucSteelTable.Specifications = item[1].Text;// 钢梁尺寸型号
                strucSteelTable.DetailedName = item[0].Text + " " + item[1].Text;// 钢梁字母+尺寸型号
            }

            // [02] 继续处理非表格数据中的钢梁类型
            string pat01 = @"^h\d+x\d+x\d+x\d+";
            foreach (var text in textInfos)
            {
                if (Regex.IsMatch(text.Text, pat01, RegexOptions.IgnoreCase))
                {
                    SteelTable strucSteelTable = new SteelTable();
                    SteelTypeTable.Add(strucSteelTable);

                    strucSteelTable.TypeName = text.Text.Replace('X', 'x');// 钢梁字母型号
                    strucSteelTable.Specifications = text.Text.Replace('X', 'x');// 钢梁尺寸型号
                    strucSteelTable.DetailedName = text.Text.Replace('X', 'x');// 钢梁字母+尺寸型号
                }
            }
        }
    }
}

