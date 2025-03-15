using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;
using PubFuncWt;
using Autodesk.Revit.DB;
using QuadTrees;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 算法生成的 地库_子停车区域 
    /// </summary>
    class SubParkArea : HLevSubregion
    {
        internal Polygon2d Polygon2d { get; }
        internal double Area => this.Polygon2d.Area;
        internal QuadTreeRectF<QTreeRevitEleCtrl> Qtree { get; }
        internal List<QTreeRevitEleCtrl> QTreeRevitEleCtrls { get; }

        /// <summary>
        /// 使用Frame3d对场景进行变换 适用与寻路算法
        /// </summary>
        Frame3d Frame3d { get; set; }

        internal SubParkArea(Polygon2d _polygon2d, Bsmt _bsmt) : base(_bsmt)
        {
            this.Polygon2d = _polygon2d;
            this.Qtree = _bsmt.Qtree;
            this.QTreeRevitEleCtrls = this.Qtree.GetObjects(_polygon2d.GetBounds().ToRectangleF()).Where(p => p.RevitEleCtrl.Ele.IsValidObject).ToList();// 判断是否为有效物体——传输进来的物体，部分会删除——如未锁定-车位-

            this.AdjacentRoads = AdjacentRoadCenterLine(this.Polygon2d);
            //Computer();
        }
        internal SubParkArea(Polygon2d _polygon2d)
        {
            this.Polygon2d = _polygon2d;
        }

        internal void Computer()
        {
            this.SelfBoundSegs = selfBoundSegs();
            this._inBoundEleFrs = this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleFR).Select(p => p.RevitEleCtrl as RevitEleFR).ToList();
            this._revitElePs = this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitElePS).Select(p => p.RevitEleCtrl as RevitElePS).ToList();
            this._revitEleColumns = this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleCol).Select(p => p.RevitEleCtrl as RevitEleCol).ToList();
            this._revitEleVeRas = this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleVeRa).Select(p => p.RevitEleCtrl as RevitEleVeRa).ToList();
            this._revitEleWalls = this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleWall).Select(p => p.RevitEleCtrl as RevitEleWall).ToList();

            List<RevitElePS> revitElePses = new List<RevitElePS>();

            Polygon2d poly2d = this.Polygon2d.OutwardOffeet(1.0);

            foreach (var p in this._revitElePs)
            {
                if (poly2d.Contains(p.LocVector2d) || poly2d.Intersects(p.Polygon2d()))
                {
                    revitElePses.Add(p);
                }
            }

            //this._revitElePs.Where(p => this.Polygon2d.Contains(p.LocVector2d) || this.Polygon2d.Intersects(p.Polygon2d())).ToList();
            //List<RevitElePS> revitElePses = this._revitElePs.Where(p => this.Polygon2d.Contains(p.LocVector2d) || this.Polygon2d.Intersects(p.Polygon2d())).ToList();

            this._revitEleFsesFixed = revitElePses.Where(p => p.PinnedByTool).ToList();
            this._revitEleFsesUnFixed = revitElePses.Where(p => !p.PinnedByTool).ToList();

            List<RevitEleCol> revitEleCols = new List<RevitEleCol>();
            foreach (var p in this._revitEleColumns)
            {
                if (poly2d.Contains(p.LocVector2d) || poly2d.Intersects(p.Polygon2d()))
                {
                    revitEleCols.Add(p);
                }
            }

            this._revitEleColumnsFixed = revitEleCols.Where(p => p.PinnedByTool).ToList();
            this._revitEleColumnsUnFixed = revitEleCols.Where(p => !p.PinnedByTool).ToList();


            this.ObstructBoundLoops = obstructBoundLoops().ToList();// 该步骤需要 详图障碍物区域 坡道 锁定-车位

            // 打印线条
            //foreach (var item in this._obstructBoundLoops)
            //{
            //    List<Vector2d> _path_Vector2D = item.polygon2d.Vertices.ToList();
            //    List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
            //    IEnumerable<Line> _lines = _xYZs.ToLines();
            //    this.Doc.CreateDirectShapeWithNewTransaction(_lines, this.Doc.ActiveView);
            //}

        }
        internal List<BoundSeg> SelfBoundSegs { get; set; }
        /// <summary>
        /// 子停车区域的自身边界属性 主要区别：是否面对通车道空间
        /// 待解决问题：矩形切换对应的边界与地库外墙边界，不存在直接关系
        /// </summary>
        private List<BoundSeg> selfBoundSegs()
        {
            Polygon2d polygon2d = this.Bsmt.BsmtBound.Polygon2dInward;
            List<BoundSeg> selfBoundSegs = new List<BoundSeg>();
            foreach (Segment2d segment2d in this.Polygon2d.SegmentItr())
            {
                bool isCoincide = segment2d.IsCoincide(polygon2d);
                if (isCoincide)
                {
                    BoundSeg boundarySegment = new BoundSeg(segment2d, EleProperty.BsmtWall);
                    selfBoundSegs.Add(boundarySegment);
                }
                else if (!isCoincide)
                {
                    BoundSeg boundarySegment = new BoundSeg(segment2d, EleProperty.Lane);
                    selfBoundSegs.Add(boundarySegment);
                }
            }
            return selfBoundSegs;
        }
        internal List<BoundO> ObstructBoundLoops { get; set; }

        /// <summary>
        /// 相关 障碍物线圈 当前主要障碍物属性元素： 剪力墙、结构轮廓、设备用房、车库出入口坡道、已固定车位占据的空间、机械车位、采光井……
        /// </summary>
        private IEnumerable<BoundO> obstructBoundLoops()
        {
            // 0 不需要作为障碍物属性的填充区域
            // 1 详图填充区域 非塔楼结构区域 塔楼区域
            // 2 开间区域要在第二步骤进行特殊处理
            foreach (var item01 in this._inBoundEleFrs
                .Where(p => p.EleProperty != EleProperty.SubPsAreaExit
                && p.EleProperty != EleProperty.BsmtWall
                && p.EleProperty != EleProperty.ResidenStruRegion
                && p.EleProperty != EleProperty.UnitFoyer// 剔除单元门厅
                && p.EleProperty != EleProperty.ResidenOpenRoom))// openRoom是为了表达剪力墙开间，可停车区域
            {
                foreach (var item02 in item01.BoundOs())
                {
                    if (item02.polygon2d.Area > 0.1)
                    {
                        if (this.Polygon2d.Contains(item02.polygon2d) || this.Polygon2d.Intersects(item02.polygon2d) || item02.polygon2d.Contains(this.Polygon2d))
                            yield return item02;
                    }
                }
            }

            // 2 详图填充区域 特殊情况 需要考虑塔楼开间区域

            List<BoundO> openRoomOs = new List<BoundO>();// 所有的开间线圈
            this._inBoundEleFrs.Where(p => p.EleProperty == EleProperty.ResidenOpenRoom).ToList().ForEach(p =>
            {
                openRoomOs.AddRange(p.BoundOs());
            });

            IEnumerable<Polygon2d> polygon2ds = openRoomOs.Select(p => p.polygon2d);

            foreach (var item01 in this._inBoundEleFrs.Where(p => p.EleProperty == EleProperty.ResidenStruRegion))// 塔楼填充区域需要减去开间信息
            {
                foreach (var item02 in item01.BoundOs())// 每一个独立的塔楼投影线圈
                {
                    Polygon2d tarPolygon = item02.polygon2d;

                    if (this.Polygon2d.Contains(tarPolygon) || this.Polygon2d.Intersects(tarPolygon) || tarPolygon.Contains(this.Polygon2d))
                    {
                        foreach (var item in tarPolygon.DifferenceClipper(polygon2ds))
                        {
                            // 是否处理为 工 或 U 字型 放在subCellArea中进行判断
                            if (item.Area > 0.1)
                            {
                                yield return new BoundO(item, item02.EleProperty);
                            }
                        }
                    }
                }
            }

            // 3 坡道 族实例
            foreach (var item01 in this._revitEleVeRas.Select(p => p.BoundO))
            {
                if (item01.polygon2d.Area > 0.1)
                {
                    if (this.Polygon2d.Contains(item01.polygon2d) || this.Polygon2d.Intersects(item01.polygon2d) || item01.polygon2d.Contains(this.Polygon2d))
                        yield return item01;
                }
            }
            // 4 锁定 车位
            Paths paths = this._revitEleFsesFixed.Select(p => p.BoundO.polygon2d.ToPath()).ToList();
            paths = paths.UnionClip(paths);
            foreach (var item in paths)
            {
                Polygon2d o = item.ToPolygon2d();
                if (o.Area > 0.1)
                {
                    yield return new BoundO(o, EleProperty.LockParking);
                }
            }
            // 5 剪力墙
            foreach (var item01 in this._revitEleWalls.Select(p => p.BoundO))
            {
                if (item01.polygon2d.Area > 0.1)
                {
                    if (this.Polygon2d.Contains(item01.polygon2d) || this.Polygon2d.Intersects(item01.polygon2d) || item01.polygon2d.Contains(this.Polygon2d))
                        yield return item01;
                }
            }
            // 6 单元门厅前，空一个车位
            foreach (var item01 in this._inBoundEleFrs.Where(p => p.EleProperty == EleProperty.UnitFoyer))
            {
                foreach (var item02 in item01.BoundOs())// 每一个独立的单元门厅线圈
                {
                    Polygon2d tarPolygon = item02.polygon2d;
                    // 如何拉长单元门厅
                    yield return new BoundO(tarPolygon, item02.EleProperty);

                }
            }



        }
        private List<RevitEleFR> _inBoundEleFrs { get; set; }
        internal List<RevitEleFR> InBoundEleFrs => this._inBoundEleFrs;
        private List<RevitElePS> _revitElePs { get; set; }
        internal List<RevitElePS> RevitElePs => this._revitElePs;
        private List<RevitEleVeRa> _revitEleVeRas { get; set; }
        internal List<RevitEleVeRa> RevitEleVeRas => this._revitEleVeRas;
        private List<RevitElePS> _revitEleFsesFixed { get; set; }
        internal List<RevitElePS> RevitEleFsesFixed => this._revitEleFsesFixed;
        private List<RevitElePS> _revitEleFsesUnFixed { get; set; }
        internal List<RevitElePS> RevitEleFsesUnFixed => this._revitEleFsesUnFixed;

        private List<RevitEleCol> _revitEleColumns { get; set; }
        internal List<RevitEleCol> RevitEleColumns => this._revitEleColumns;
        private List<RevitEleCol> _revitEleColumnsFixed { get; set; }
        internal List<RevitEleCol> RevitEleColumnsFixed => this._revitEleColumnsFixed;
        private List<RevitEleCol> _revitEleColumnsUnFixed { get; set; }
        internal List<RevitEleCol> RevitEleColumnsUnFixed => this._revitEleColumnsUnFixed;

        private List<RevitEleWall> _revitEleWalls { get; set; }
        internal List<RevitEleWall> RevitEleWalls => this._revitEleWalls;
        /// <summary>
        /// 删除未锁定 车位
        /// </summary>
        internal void DeleteUnFixedPses()
        {
            using (var deleteTrans = new Transaction(this.Doc, "删除所有未锁定停车位"))
            {
                deleteTrans.Start();
                this.Doc.Delete(_revitEleFsesUnFixed.Select(p => p.Id).ToList());
                this.Doc.Delete(_revitEleColumnsUnFixed.Select(p => p.Id).ToList());
                deleteTrans.Commit();
            }
        }

    }
}
