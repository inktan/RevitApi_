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
    internal class StrucFloor_backup : EleCreatInfo
    {

        public StrucFloor_backup() : base()
        {

        }

        internal override void Execute()
        {
            ExGeoInfo();
            GetFamilySymbols();
            OpenTrans();
            SetLevel();
            Move();
            //throw new NotImplementedException();
            System.Windows.Forms.Application.DoEvents();// 窗体重绘
        }

        List<FloorInfo> floorInfos { get; set; }
        List<FloorInfo> floorDescendInfos { get; set; }
        List<FloorInfo> floorInfoHoles { get; set; }

        List<PolyLineInfo> polyLineInfos = new List<PolyLineInfo>();
        List<PolyLineInfo> polyLineInfoDescends = new List<PolyLineInfo>();// 该列表包含降板线圈，结构楼板数据表格所在外框
        List<PolyLineInfo> polyLineInfoHoles = new List<PolyLineInfo>();
        List<LineInfo> floorTableLineInfos = new List<LineInfo>();// 结构楼板表格的线
        List<TextInfo> textInfos = new List<TextInfo>();

        internal override void ExGeoInfo()
        {
            List<string> patternFloor = new List<string>() { @"a\d*-slab-otln", @"a\d*-flor-otln" };

            string patternHole = @"s\d*-hole";
            string patternText = @"s\d*-slab-text";

            foreach (var item in this.DwgParser.LayerNames)
            {
                foreach (var pattern in patternFloor)
                {
                    if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                    {
                        if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                        {
                            polyLineInfos.AddRange(this.DwgParser.PolyLineLayerInfos[item]);
                        }
                    }
                }
                if (Regex.IsMatch(item, patternHole, RegexOptions.IgnoreCase))
                {
                    if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                    {
                        polyLineInfoHoles.AddRange(this.DwgParser.PolyLineLayerInfos[item]);
                    }
                }
                if (Regex.IsMatch(item, patternText, RegexOptions.IgnoreCase))
                {
                    if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                    {
                        polyLineInfoDescends.AddRange(this.DwgParser.PolyLineLayerInfos[item]);// 降板与轮廓线，不在一个图层
                    }
                    if (this.DwgParser.TexLayertInfos.ContainsKey(item))
                    {
                        textInfos.AddRange(this.DwgParser.TexLayertInfos[item]);
                    }
                    if (this.DwgParser.LineLayerInfos.ContainsKey(item))
                    {
                        floorTableLineInfos.AddRange(this.DwgParser.LineLayerInfos[item]);
                    }
                }
            }

            this.floorInfos = polyLineInfos.Where(p => p.Polygon2d != null).Where(p => p.Pts.Count > 3).Where(p => p.CurveArray != null).Select(p => new FloorInfo(p)).ToList();
            this.floorDescendInfos = polyLineInfoDescends.Where(p => p.Polygon2d != null).Where(p => p.Pts.Count > 3).Where(p => p.CurveArray != null).Select(p => new FloorInfo(p)).ToList();
            this.floorInfoHoles = polyLineInfoHoles.Where(p => p.Polygon2d != null).Where(p => p.Pts.Count > 3).Where(p => p.CurveArray != null).Select(p => new FloorInfo(p)).ToList();

            Action<List<FloorInfo>> action01 = (p) =>
            {
                foreach (var poly in p)
                {
                    poly.textInfos = new List<TextInfo>();

                    foreach (var text in textInfos)
                    {
                        Vector2d textNowPosition = new Vector2d(text.Center.X, text.Center.Y);
                        if (poly.PolyLineInfo.Polygon2d.Contains(textNowPosition))
                        {
                            poly.textInfos.Add(text);
                        }
                    }
                }
            };
            action01(this.floorInfos);
            action01(this.floorDescendInfos);

            //this.floorInfos.ForEach(p => p.ObtainDesendFloorHole(this.floorDescendInfos, this.floorInfoHoles));
            //this.floorDescendInfos.ForEach(p => p.ObtainDesendFloorHole(new List<FloorInfo>(), this.floorInfoHoles));

            GetFloorInfoTable();// 获取楼板类型表格
        }

        internal override void OpenTrans()
        {
            this.ElementIds = new List<ElementId>();

            using (Transaction transaction = new Transaction(CMD.Doc, "创建楼板"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();

                this.floorInfos.ForEach(p =>
                {
                    if (p.CurveArray.Size > 2)
                    {
                        try
                        {
                            Floor floor = CMD.Doc.Create.NewFloor(p.CurveArray, this.FloorTypes.Where(_p => _p.Name == MainWindow.Instance.FloorTypeSel.SelectedValue.ToString()).First(), CMD.Doc.ActiveView.GenLevel, false);
                            p.Floor = floor;
                            this.ElementIds.Add(p.Floor.Id);
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                });
                this.floorDescendInfos.ForEach(p =>
                {
                    if (p.CurveArray.Size > 2)
                    {
                        try
                        {
                            Floor floor = CMD.Doc.Create.NewFloor(p.CurveArray, this.FloorTypes.Where(_p => _p.Name == MainWindow.Instance.FloorTypeSel.SelectedValue.ToString()).First(), CMD.Doc.ActiveView.GenLevel, false);

                            p.Floor = floor;
                            this.ElementIds.Add(p.Floor.Id);
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                });

                transaction.Commit();
            }

            //List<StrucGenlProfileInfo> strucGenlProfileInfos = new List<StrucGenlProfileInfo>();
            //StrucGenlProfileInfo strucGenlProfileInfo;

            //using (Transaction transaction = new Transaction(CMD.Doc, "楼板降板及开洞"))
            //{
            //    transaction.Start();
            //    transaction.DeleteErrOrWaringTaskDialog();

            //if (!this.FloorHoleFs.IsActive)
            //{
            //    this.FloorHoleFs.Activate();
            //}

            //foreach (var p in this.floorInfos)
            //{
            //    if (p.Floor != null && p.Floor.IsValidObject)
            //    {
            // 将所有的碰撞洞口合并
            //List<Polygon2d> polyCol = new List<Polygon2d>();
            //foreach (var item in p.DescendFloors) // 放置楼板空心族-降板可见
            //{
            //    polyCol.Add(item.PolyLineInfo.Polygon2d);

            //FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(item.PolyLineInfo.Polygon2d.LDpOfBox2d().ToXYZ(), this.FloorHoleFs, p.Floor, CMD.Level, StructuralType.NonStructural);
            //item.Descend = familyInstance;
            //this.ElementIds.Add(familyInstance.Id);

            //strucGenlProfileInfo = new StrucGenlProfileInfo();
            //strucGenlProfileInfo.PolyLineInfo = item.PolyLineInfo;
            //strucGenlProfileInfo.FamilyInstance = familyInstance;
            //strucGenlProfileInfos.Add(strucGenlProfileInfo);

            // 可以采用挖洞口的形式
            //try
            //{
            //    Opening opening = CMD.Doc.Create.NewOpening(p.Floor, item.CurveArray, true);
            //    this.ElementIds.Add(opening.Id);
            //    //Floor floor = CMD.Doc.Create.NewFloor(item.CurveArray, this.FloorTypes.Where(_p => _p.Name == MainWindow.Instance.FloorTypeSel.SelectedValue.ToString()).First(), CMD.Level, false);
            //    //item.Floor = floor;
            //    //this.ElementIds.Add(floor.Id);
            //}
            //catch (Exception)
            //{
            //    //throw;
            //}
            //}
            //foreach (var item in p.Holes)
            //{
            //    polyCol.Add(item.PolyLineInfo.Polygon2d);
            //}
            //foreach (var item in polyCol.UnionClipper())
            //{
            //    // 可以采用挖洞口的形式
            //    try
            //    {
            //        Opening opening = CMD.Doc.Create.NewOpening(p.Floor, item.OutwardOffeet(0.01).ToCurveLoop().ToCurveArray(), true);
            //        this.ElementIds.Add(opening.Id);
            //    }
            //    catch (Exception)
            //    {
            //        //throw;
            //    }
            //}

            //foreach (var item in p.OtherTypeFloors)// 放置不同类型的楼板
            //{
            //FamilySymbol familySymbol = this.FloorHoleFss.Where(_p => _p.Name == item.StrucFloorTable.DetailedName).FirstOrDefault();
            //if (familySymbol == null)
            //{
            //    familySymbol = this.FloorHoleFs;
            //}
            //if (!familySymbol.IsActive)
            //{
            //    familySymbol.Activate();
            //}

            //FamilyInstance familyInstance = CMD.Doc.Create.NewFamilyInstance(item.PolyLineInfo.Polygon2d.LDpOfBox2d().ToXYZ(), familySymbol, p.Floor, CMD.Level, StructuralType.NonStructural);
            //item.OtherTypeFloor = familyInstance;
            //this.ElementIds.Add(familyInstance.Id);

            //strucGenlProfileInfo = new StrucGenlProfileInfo();
            //strucGenlProfileInfo.PolyLineInfo = item.PolyLineInfo;
            //strucGenlProfileInfo.FamilyInstance = familyInstance;
            //strucGenlProfileInfos.Add(strucGenlProfileInfo);
            //            }
            //        }
            //    }

            //    transaction.Commit();
            //}
            //using (Transaction transaction = new Transaction(CMD.Doc, "在降板中开洞"))
            //{
            //    transaction.Start();
            //    transaction.DeleteErrOrWaringTaskDialog();

            //    foreach (var item in this.floorDescendInfos) // 放置楼板空心族-降板可见
            //    {
            //        if (item.Floor != null && item.Floor.IsValidObject)
            //        {
            //            // 在降板上开洞
            //            foreach (var hole02 in item.Holes)
            //            {
            //                Opening opening = CMD.Doc.Create.NewOpening(item.Floor, hole02.CurveArray, true);
            //                this.ElementIds.Add(opening.Id);
            //            }
            //        }
            //    }

            //    transaction.Commit();
            //}

        }

        internal override void SetLevel()
        {
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                //this.floorInfos.ForEach(p =>
                //{
                //    if (p.Floor != null && p.Floor.IsValidObject)// 主体楼板
                //    {
                //        p.Floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0.0);
                //    }

                //    p.OtherTypeFloors.ForEach(_p =>// 其他类型楼板
                //    {
                //        if (_p.OtherTypeFloor != null && _p.OtherTypeFloor.IsValidObject)
                //        {
                //            _p.OtherTypeFloor.LookupParameter("楼板可见性").Set(1);
                //        }
                //    });
                //});

                this.floorInfos.ForEach(_p =>// 楼板
                {
                    if (_p.Floor != null && _p.Floor.IsValidObject)
                    {
                        _p.GetzValue();
                        _p.Floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0);
                    }
                });
                this.floorDescendInfos.ForEach(_p =>// 楼板上的降板
                {
                    if (_p.Floor != null && _p.Floor.IsValidObject)
                    {
                        _p.GetzValue();
                        _p.Floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(_p.zValue * 1000);
                    }
                });

                trans.Commit();
            }

            //throw new NotImplementedException();
        }

        List<FloorType> FloorTypes { get; set; }
        //FamilySymbol FloorHoleFs { get; set; }
        //List<FamilySymbol> FloorHoleFss { get; set; }
        internal override void GetFamilySymbols()
        {
            this.FloorTypes = (new FilteredElementCollector(CMD.Doc))
              .OfCategory(BuiltInCategory.OST_Floors)
              .OfClass(typeof(FloorType))
              .Cast<FloorType>()
              .ToList();

            //this.FloorHoleFs = CMD.Doc.FamilySymbolByName("楼板-开洞-v4", "楼板-开洞-v4");
            //// 创建楼板类型
            //Family fa = (new FilteredElementCollector(CMD.Doc))
            //       //.OfCategory(BuiltInCategory.OST_StructuralFraming)
            //       .OfClass(typeof(Family))
            //       .First(p => p.Name == "楼板-开洞-v4") as Family;
            //// 矩形
            //this.FloorHoleFss = fa.GetFamilySymbolIds().Select(p => CMD.Doc.GetElement(p) as FamilySymbol).ToList();

            //foreach (var item in this.FloorTypeTable)
            //{
            //    if (!this.FloorHoleFss.Select(p => p.Name).Contains(item.TypeName))
            //    {
            //        using (Transaction trans = new Transaction(CMD.Doc, "创建 楼板开洞 类型"))
            //        {
            //            trans.Start();
            //            FamilySymbol temp = this.FloorHoleFs.Duplicate(item.TypeName) as FamilySymbol;
            //            temp.LookupParameter("结构厚度").Set(item.Thickness.MilliMeterToFeet());
            //            this.FloorHoleFss.Add(temp);
            //            trans.Commit();
            //        }
            //    }
            //}

            foreach (var item in this.FloorTypeTable)
            {
                if (!this.FloorTypes.Select(p => p.Name).Contains(item.DetailedName))
                {
                    if (item.Thickness <= 0) continue;

                    using (Transaction trans = new Transaction(CMD.Doc, "创建 楼板开洞 类型"))
                    {
                        trans.Start();
                        FloorType temp = this.FloorTypes.First().Duplicate(item.DetailedName) as FloorType;

                        CompoundStructure compoundStructure;
                        int i = 0;
                        while (true)
                        {
                            compoundStructure = this.FloorTypes[i].GetCompoundStructure();
                            IList<CompoundStructureLayer> compoundStructureLayers = compoundStructure.GetLayers();
                            CompoundStructureLayer compoundStructureLayer = new CompoundStructureLayer();
                            compoundStructureLayer.Width = item.Thickness.MilliMeterToFeet();
                            compoundStructureLayers.Clear();
                            compoundStructureLayers.Add(compoundStructureLayer);
                            compoundStructure.SetLayers(compoundStructureLayers);

                            if (compoundStructure.IsValid(CMD.Doc, out IDictionary<int, CompoundStructureError> errMap, out IDictionary<int, int> twoLayerErrorsMap))
                            {
                                break;
                            }
                            i++;
                        }
                        temp.SetCompoundStructure(compoundStructure);
                        this.FloorTypes.Add(temp);
                        trans.Commit();
                    }
                }
            }

            //throw new NotImplementedException();
        }
        List<StrucFloorTable> FloorTypeTable { get; set; }
        /// <summary>
        /// 提取楼板类型表格
        /// </summary>
        void GetFloorInfoTable()
        {
            // 获取楼板配筋表

            string floorIndexStr = "楼板配筋表";
            List<TextInfo> textInPoly = new List<TextInfo>();
            List<Segment2d> lineInPolyHor = new List<Segment2d>();
            List<Segment2d> lineInPolyVer = new List<Segment2d>();

            foreach (var textInfo in textInfos)
            {
                if (textInfo.Text.StartsWith(floorIndexStr))
                {
                    Vector2d position = textInfo.Position.ToVector2d();
                    PolyLineInfo poly = polyLineInfoDescends.Where(p => p.Polygon2d != null && p.Polygon2d.IsRectangle()).OrderBy(p => position.DistanceSquared(p.Polygon2d.LUpOfBox2d())).FirstOrDefault();
                    if (poly != null)
                    {
                        foreach (var text in textInfos)
                        {
                            if (poly.Polygon2d.Contains(text.Center.ToVector2d()))
                            {
                                textInPoly.Add(text);
                            }
                        }

                        foreach (var item in this.polyLineInfoDescends)
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

                        foreach (var line in floorTableLineInfos)
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

            List<List<TextInfo>> textRranks = new List<List<TextInfo>>();
            List<TextInfo> textCol = new List<TextInfo>();

            for (int i = 0; i < lineInPolyHor.Count; i++)// 从表格中按顺序提取文字
            {
                List<TextInfo> temp = new List<TextInfo>();
                for (int j = 0; j < lineInPolyVer.Count; j++)
                {
                    foreach (var item in textInPoly)
                    {
                        if (textCol.Contains(item)) continue;

                        if (item.Center.X < lineInPolyVer[j].Center.x && item.Center.Y < lineInPolyHor[i].Center.y)
                        {
                            temp.Add(item);
                            textCol.Add(item);
                        }
                    }
                }
                if (temp.Count == 0) continue;
                textRranks.Add(temp);
            }

            FloorTypeTable = new List<StrucFloorTable>();
            foreach (var item in textRranks)
            {
                StrucFloorTable strucFloorTable = new StrucFloorTable();
                FloorTypeTable.Add(strucFloorTable);

                strucFloorTable.TypeName = item[0].Text;
                strucFloorTable.DetailedName = item[0].Text + " " + item[1].Text; ; ; ;

                double.TryParse(item[1].Text, out double thickness);

                if (thickness == 0)
                {
                    string widthTmp01 = "";
                    string widthTmp02 = "";

                    int i = 0;
                    while (i <= item[1].Text.Length)
                    {
                        char widthChar = item[1].Text.ElementAtOrDefault(i);

                        if (char.IsNumber(widthChar))
                        {
                            widthTmp01 += widthChar;
                        }
                        if (!char.IsNumber(widthChar)) break;
                        i++;
                    }
                    while (i <= item[1].Text.Length)
                    {
                        char widthChar = item[1].Text.ElementAtOrDefault(i);
                        if (!char.IsNumber(widthChar))
                        {
                            i++;
                            continue;
                        };

                        if (char.IsNumber(widthChar))
                        {
                            widthTmp02 += widthChar;
                        }
                        i++;
                    }

                    double.TryParse(widthTmp01, out double thickness01);
                    double.TryParse(widthTmp02, out double thickness02);

                    thickness = thickness01 + thickness02;
                }

                strucFloorTable.Thickness = thickness;
            }

        }

        internal override void Move()
        {
            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();

                //List<ElementId> elementIds = this.floorInfos.Where(p => p.Floor != null && p.Floor.IsValidObject).Select(p => p.Floor.Id).ToList();

                //this.floorInfos.ForEach(_p =>
                //{
                //    elementIds.AddRange(_p.DescendFloors.Where(p => p.Descend != null && p.Descend.IsValidObject).Select(p => p.Descend.Id));
                //    elementIds.AddRange(_p.Holes.Where(p => p.Hole != null && p.Hole.IsValidObject).Select(p => p.Hole.Id));
                //});

                if (this.ElementIds.Count() > 0)// 移动楼板
                {
                    ElementTransformUtils.MoveElements(CMD.Doc, this.ElementIds.Where(p => CMD.Doc.GetElement(p) != null && CMD.Doc.GetElement(p).IsValidObject).ToList(), CMD.PositonMoveDis);
                }
                // 移动 楼板-开洞 族

                transaction.Commit();
            }
            //throw new NotImplementedException();
        }
    }
}
