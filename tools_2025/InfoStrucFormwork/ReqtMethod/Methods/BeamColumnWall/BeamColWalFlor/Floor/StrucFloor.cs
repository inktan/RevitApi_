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
//using DataStructures.Trees;
using System.Threading;

namespace InfoStrucFormwork
{
    internal class StrucFloor : EleCreatInfo
    {

        public StrucFloor() : base()
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
        //List<FloorInfo> floorInfoHoles { get; set; }

        List<PolyLineInfo> polyLineInfos = new List<PolyLineInfo>();// 最外层板边线，可能有多个
        List<PolyLineInfo> polyLineInfoDescends = new List<PolyLineInfo>();// 所有降板，包含嵌套降板
        //List<PolyLineInfo> polyLineInfoHoles = new List<PolyLineInfo>();
        //List<LineInfo> floorTableLineInfos = new List<LineInfo>();// 结构楼板表格的线
        List<TextInfo> textInfos = new List<TextInfo>();

        internal override void ExGeoInfo()
        {
            // 1 获取最外层楼板板边线
            List<string> patternFloor = new List<string>() { @"a\d*-slab-otln", @"a\d*-flor-otln" , @"s\d*-slab-otln", @"s\d*-flor-otln" };
            // 2 获取洞口
            //string patternHole = @"s\d*-hole";
            // 3 获取降板以及高度数据
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
                //if (Regex.IsMatch(item, patternHole, RegexOptions.IgnoreCase))
                //{
                //    if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                //    {
                //        polyLineInfoHoles.AddRange(this.DwgParser.PolyLineLayerInfos[item]);
                //    }
                //}
                if (Regex.IsMatch(item, patternText, RegexOptions.IgnoreCase))
                {
                    //if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))
                    //{
                    //    polyLineInfoDescends.AddRange(this.DwgParser.PolyLineLayerInfos[item]);// 降板与轮廓线，不在一个图层
                    //}
                    if (this.DwgParser.TexLayertInfos.ContainsKey(item))
                    {
                        textInfos.AddRange(this.DwgParser.TexLayertInfos[item]);
                    }
                    //if (this.DwgParser.LineLayerInfos.ContainsKey(item))
                    //{
                    //    floorTableLineInfos.AddRange(this.DwgParser.LineLayerInfos[item]);
                    //}
                }
            }

            this.floorInfos = polyLineInfos.Where(p => p.Polygon2d != null).Where(p => p.Pts.Count > 3).Where(p => p.CurveArray != null).Select(p => new FloorInfo(p)).ToList();
            floorDescendInfos = polyLineInfoDescends.Where(p => p.Polygon2d != null).Where(p => p.Pts.Count > 3).Where(p => p.CurveArray != null).Select(p => new FloorInfo(p)).ToList();
            //this.floorInfoHoles = polyLineInfoHoles.Where(p => p.Polygon2d != null).Where(p => p.Pts.Count > 3).Where(p => p.CurveArray != null).Select(p => new FloorInfo(p)).ToList();

            // 给每个洞口设定对应的标高
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
            action01(floorDescendInfos);

            // 获取楼板上的开洞和降板
            //this.floorInfos.ForEach(p => p.ObtainDesendFloorHole(floorDescendInfos, this.floorInfoHoles));
            // 获取降板上的开洞
            //floorDescendInfos.ForEach(p => p.ObtainDesendFloorHole(new List<FloorInfo>(), this.floorInfoHoles));

            //GetFloorInfoTable();// 获取楼板类型表格
        }

        internal override void OpenTrans()
        {
            this.ElementIds = new List<ElementId>();
            // 创建最外层楼板
            using (Transaction transaction = new Transaction(CMD.Doc, "创建楼板"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                this.floorInfos.ForEach(p =>
                {
                    if (p.CurveArray.Size > 3)
                    {
                        try
                        {
                            // 创建楼板
                            Floor floor = CMD.Doc.Create.NewFloor(p.CurveArray, this.FloorTypes.Where(_p => _p.Name == MainWindow.Instance.FloorTypeSel.SelectedValue.ToString()).First(), CMD.Doc.ActiveView.GenLevel, true);
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
            // 创建一级洞口和降板
            //using (Transaction transaction = new Transaction(CMD.Doc, "创建降板及洞口"))
            //{
            //    transaction.Start();
            //    transaction.DeleteErrOrWaringTaskDialog();
            //    foreach (var p in this.floorInfos)
            //    {
            //        foreach (var _p in p.DescendFloors)// 每个楼板上对应的降板族
            //        {
            //            if (_p.CurveArray.Size > 3)
            //            {
            //                //CMD.Doc.CreateDirectShape(_p.PolyLineInfo.Polygon2d.ToCurveLoop().ToList());
            //                try
            //                {
            //                    XYZ position = _p.PolyLineInfo.PolyLine.GetOutline().MinimumPoint;
            //                    FamilyInstance familyInstance = PlaeUniverslHole(p.Floor, position, _p.PolyLineInfo.Polygon2d);
            //                    familyInstance.LookupParameter("建筑板厚度").Set(150.0.MilliMeterToFeet());
            //                    _p.FamilyInstance = familyInstance;
            //                    this.ElementIds.Add(_p.FamilyInstance.Id);
            //                }
            //                catch (Exception)
            //                {
            //                    //throw;
            //                }
            //            }
            //        }
            //        foreach (var _p in p.Holes)// 每个降板族上对应的开洞
            //        {
            //            if (_p.CurveArray.Size > 3)
            //            {
            //                try
            //                {
            //                    XYZ position = _p.PolyLineInfo.PolyLine.GetOutline().MinimumPoint;
            //                    FamilyInstance familyInstance = PlaeUniverslHole(p.Floor, position, _p.PolyLineInfo.Polygon2d);
            //                    _p.FamilyInstance = familyInstance;
            //                    this.ElementIds.Add(_p.FamilyInstance.Id);
            //                }
            //                catch (Exception)
            //                {
            //                    //throw;
            //                }
            //            }
            //        }
            //    }
            //    transaction.Commit();
            //}

            using (Transaction transaction = new Transaction(CMD.Doc, "-"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                CMD.Doc.Regenerate();
                transaction.Commit();
            }

            System.Windows.Forms.Application.DoEvents();// 窗体重绘
            CMD.UIDoc.UpdateAllOpenViews();
        }
        /// <summary>
        /// 设置控制点坐标
        /// </summary>
        public static void Set32Points(XYZ position, FamilySymbol familySymbol, Polygon2d polygon2d)
        {

            List<Vector2d> points = polygon2d.Vertices.ToList();
            if (points.Count < 32)
            {
                Vector2d p_end = points.Last();
                Vector2d p_start = points.First();

                Segment2d segment2d = new Segment2d(p_end, p_start);
                int count = 32 - points.Count;
                double length = segment2d.Length / (count + 1);

                for (int i = 1; i < count + 1; i++)
                {
                    points.Add(p_end + segment2d.Direction * i * length);
                }
            }

            //Polygon2d pol = new Polygon2d(points);
            //CMD.Doc.CreateDirectShape(pol.ToCurveLoop().ToList());

            for (int i = 1; i <= 32; i++)
            {
                Parameter parameter = familySymbol.LookupParameter("x" + i);
                if (!parameter.IsReadOnly)
                {
                    parameter.Set(points[i - 1].x - position.X);
                }
                parameter = familySymbol.LookupParameter("y" + i);
                if (!parameter.IsReadOnly)
                {
                    parameter.Set(points[i - 1].y - position.Y);
                }
            }
        }
        internal override void SetLevel()
        {
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                this.floorInfos.ForEach(p =>// 楼板
                {
                    if (p.Floor != null && p.Floor.IsValidObject)
                    {
                        p.GetzValue();
                        p.Floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(p.zValue * 1000.0);
                    }
                    //p.Holes.ForEach(_p =>
                    //{
                    //    if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                    //    {
                    //        _p.FamilyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(CMD.Doc.ActiveView.GenLevel.Id);
                    //    }
                    //});

                    //p.DescendFloors.ForEach(_p =>
                    //{
                    //    if (_p.FamilyInstance != null && _p.FamilyInstance.IsValidObject)
                    //    {
                    //        _p.GetzValue();
                    //        _p.FamilyInstance.LookupParameter("板顶与基面距离").Set(_p.zValue * 1000.0);
                    //    }

                    //    _p.Holes.ForEach(__p =>
                    //    {
                    //        if (__p.FamilyInstance != null && __p.FamilyInstance.IsValidObject)
                    //        {
                    //            __p.FamilyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(CMD.Doc.ActiveView.GenLevel.Id);
                    //        }
                    //    });

                    //});

                });
                trans.Commit();
            }

            //throw new NotImplementedException();
        }

        List<FloorType> FloorTypes { get; set; }
        //public static List<FamilySymbol> FloorHoleFss { get; set; }
        //List<FamilySymbol> FloorHoleFss { get; set; }
        internal override void GetFamilySymbols()
        {
            this.FloorTypes = (new FilteredElementCollector(CMD.Doc))
              .OfCategory(BuiltInCategory.OST_Floors)
              .OfClass(typeof(FloorType))
              .Cast<FloorType>()
              .ToList();
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
