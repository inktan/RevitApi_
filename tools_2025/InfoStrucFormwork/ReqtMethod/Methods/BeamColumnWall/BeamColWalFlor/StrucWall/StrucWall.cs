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
    internal class StrucWall_ : EleCreatInfo
    {

        public StrucWall_() : base()
        { }

        /// <summary>
        /// 创建柱子与剪力墙
        /// </summary>
        /// <param name="layerName"></param>
        internal override void Execute()
        {
            ExGeoInfo();
            GetFamilySymbols();
            OpenTrans();
            SetLevel();
            Move();
            System.Windows.Forms.Application.DoEvents();// 窗体重绘
        }

        List<Line> lines_200 { get; set; }
        List<Line> lines_250 { get; set; }
        List<Line> lines_300 { get; set; }

        internal override void ExGeoInfo()
        {
            List<PolyLineInfo> polyLineInfos = new List<PolyLineInfo>();

            string patternWall01 = @"a\d*-wall";
            string patternWall02 = @"s\d*-wall";

            foreach (var item in this.DwgParser.LayerNames)
            {
                if (Regex.IsMatch(item, patternWall01, RegexOptions.IgnoreCase))
                {
                    if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))// 墙体全部使用轮廓族
                    {
                        polyLineInfos.AddRange(this.DwgParser.PolyLineLayerInfos[item].Where(p => p.Polygon2d != null && p.Polygon2d.IsRectangle()));
                    }
                }
                if (Regex.IsMatch(item, patternWall02, RegexOptions.IgnoreCase))
                {
                    if (this.DwgParser.PolyLineLayerInfos.ContainsKey(item))// 墙体全部使用轮廓族
                    {
                        polyLineInfos.AddRange(this.DwgParser.PolyLineLayerInfos[item].Where(p => p.Polygon2d != null && p.Polygon2d.IsRectangle()));
                    }
                }
            }

            List<Segment2d> seg2ds_200 = new List<Segment2d>();
            List<Segment2d> seg2ds_250 = new List<Segment2d>();
            List<Segment2d> seg2ds_300 = new List<Segment2d>();

            foreach (var polyLineInfo in polyLineInfos)
            {
                seg2ds_200.AddRange(polyLineInfo.GetCenterline(100.0.MilliMeterToFeet()));
                seg2ds_250.AddRange(polyLineInfo.GetCenterline(125.0.MilliMeterToFeet()));
                seg2ds_300.AddRange(polyLineInfo.GetCenterline(150.0.MilliMeterToFeet()));
            }

            lines_200 = seg2ds_200.Select(p => p.ToLineBound()).ToList();
            lines_250 = seg2ds_250.Select(p => p.ToLineBound()).ToList();
            lines_300 = seg2ds_300.Select(p => p.ToLineBound()).ToList();

            //throw new NotImplementedException();
        }

        List<Wall> Walls { get; set; }

        internal override void OpenTrans()
        {
            Walls = new List<Wall>();
            using (Transaction trans = new Transaction(CMD.Doc, "创建剪力墙"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                lines_200.ForEach(p =>
                {
                    Wall wall = Wall.Create(CMD.Doc, p, CMD.Doc.ActiveView.GenLevel.Id, true);
                    Walls.Add(wall);
                    wall.WallType = this.WallType200;
                });

                lines_250.ForEach(p =>
                {
                    Wall wall = Wall.Create(CMD.Doc, p, CMD.Doc.ActiveView.GenLevel.Id, true);
                    Walls.Add(wall);
                    wall.WallType = this.WallType250;
                });

                lines_300.ForEach(p =>
                {
                    Wall wall = Wall.Create(CMD.Doc, p, CMD.Doc.ActiveView.GenLevel.Id, true);
                    Walls.Add(wall);
                    wall.WallType = this.WallType300;
                });
                // 待添加类型

                trans.Commit();
            }
            this.ElementIds = new List<ElementId>();
            this.ElementIds.AddRange(Walls.Select(p => p.Id));

            //throw new NotImplementedException();
        }
        internal override void SetLevel()
        {
            using (Transaction trans = new Transaction(CMD.Doc, "修改标高"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();

                this.Walls.ForEach(wall =>
                {
                    if (CMD.DownLevel != null)
                    {
                        wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(CMD.Level.Id);
                        wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(CMD.DownLevel.Id);
                    }
                    else
                    {
                        wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(CMD.Level.Id);
                        wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(CMD.Level.Id);

                        wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(-4000.0.MilliMeterToFeet());
                    }
                });

                trans.Commit();
            }
        }
        internal override void Move()
        {
            using (Transaction transaction = new Transaction(CMD.Doc, "移动"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                if (this.Walls.Count > 0)
                {
                    ElementTransformUtils.MoveElements(CMD.Doc, this.Walls.Select(p => p.Id).ToList(), CMD.PositonMoveDis);
                }
                transaction.Commit();
            }
            //throw new NotImplementedException();
        }

        WallType WallType200 { get; set; }
        WallType WallType250 { get; set; }
        WallType WallType300 { get; set; }

        internal override void GetFamilySymbols()
        {
            List<WallType> wallTypes = (new FilteredElementCollector(CMD.Doc))
              .OfCategory(BuiltInCategory.OST_Walls)
              .OfClass(typeof(WallType))
              .Cast<WallType>()
              .Where(p => p.Kind == WallKind.Basic)
              .Where(p => p.Function == WallFunction.Exterior)
              .ToList();

            this.WallType200 = wallTypes.Where(p => p.Name == "常规 - 200mm").FirstOrDefault();
            this.WallType250 = wallTypes.Where(p => p.Name == "常规 - 250mm").FirstOrDefault();
            this.WallType300 = wallTypes.Where(p => p.Name == "常规 - 300mm").FirstOrDefault();

            // 创建墙体构造
            CompoundStructure compoundStructure;
            using (Transaction trans = new Transaction(CMD.Doc, "创建 剪力墙 类型"))
            {
                trans.Start();
                List<CompoundStructureLayer> compoundStructureLayers = new List<CompoundStructureLayer>();
                CompoundStructureLayer compoundStructureLayer = new CompoundStructureLayer();
                compoundStructureLayer.Width = 250.0.MilliMeterToFeet();
                compoundStructureLayers.Add(compoundStructureLayer);
                compoundStructure = CompoundStructure.CreateSimpleCompoundStructure(compoundStructureLayers);

                trans.Commit();
            }

            if (this.WallType200 == null)
            {
                using (Transaction trans = new Transaction(CMD.Doc, "创建 剪力墙 类型"))
                {
                    trans.Start();
                    this.WallType200 = wallTypes.First().Duplicate("常规 - 200mm") as WallType;
                    compoundStructure.SetLayerWidth(0, 200.0.MilliMeterToFeet());
                    this.WallType200.SetCompoundStructure(compoundStructure);

                    trans.Commit();
                }
            }
            if (this.WallType250 == null)
            {
                using (Transaction trans = new Transaction(CMD.Doc, "创建 剪力墙 类型"))
                {
                    trans.Start();
                    this.WallType250 = wallTypes.First().Duplicate("常规 - 250mm") as WallType;
                    compoundStructure.SetLayerWidth(0, 250.0.MilliMeterToFeet());
                    this.WallType250.SetCompoundStructure(compoundStructure);

                    trans.Commit();
                }
            }
            if (this.WallType300 == null)
            {
                using (Transaction trans = new Transaction(CMD.Doc, "创建 剪力墙 类型"))
                {
                    trans.Start();
                    this.WallType300 = wallTypes.First().Duplicate("常规 - 300mm") as WallType;
                    compoundStructure.SetLayerWidth(0, 300.0.MilliMeterToFeet());
                    this.WallType300.SetCompoundStructure(compoundStructure);

                    trans.Commit();
                }
            }
        }
    }
}
