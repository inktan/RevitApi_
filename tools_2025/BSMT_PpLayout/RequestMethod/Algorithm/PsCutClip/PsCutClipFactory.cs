using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using goa.Common.g3InterOp;
using ClipperLib;
using Autodesk.Revit.DB;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class AuxiliaryFrame3d
    {
        internal Frame3d Frame3d;
        internal int MirrorXcount;
        internal void RreverseX()
        {
            this.MirrorXcount++;
            if (MirrorXcount % 2 == 1)// x轴镜像次数不同，y轴的选装角度不同
            {
                this.Frame3d = this.Frame3d.RreverseX(-90);
            }
            else
            {
                this.Frame3d = this.Frame3d.RreverseX(90);
            }
        }
    }

    /// <summary>
    /// 标准阵列式停车布局
    /// </summary>
    class PsCutClipFactory
    {
        internal Document Doc => this.SubCellArea.Doc;
        internal SubCellArea SubCellArea { get; }
        internal Segment2d DatumLine { get; set; }
        internal PsCutClipPattern PsCutClipPattern { get; set; }// 排车位模式
        internal PsCutClipFactory(SubCellArea _cellArea, PsCutClipPattern _psCutClipPattern)
        {
            this.SubCellArea = _cellArea;
            this.PsCutClipPattern = _psCutClipPattern;
            this.DatumLine = this.SubCellArea.DatumLine;
        }

        internal void Computer()
        {
            ExtractData();
            SetFrame3d();
            SpatialRecognition();
        }

        private Polygon2d _polygon2d;
        private Polygon2d _bsmtPoly2d;
        private List<BoundO> _obstacleOs;// 所有的障碍物线圈，车位不可与之碰撞
        //private List<Segment2d> _obstacleSegs;// 用于判断车头是否与地库外墙线产生碰撞
        private List<Route> _allRoutes;// 用于判断地库子区域只有一边有道路时，调整柱子起点位置
        private AuxiliaryFrame3d _auxiliaryFrame3d;

        /// <summary>
        /// 0 提取数据
        /// </summary>
        private void ExtractData()
        {
            this._polygon2d = this.SubCellArea.Polygon2d;
            this._bsmtPoly2d = this.SubCellArea.CellArea.SubParkArea.Bsmt.BsmtBound.Polygon2dInward;
            this._obstacleOs = this.SubCellArea.ObstructBoundLoops;
            //this._obstacleOs = this.SubCellArea.ObstructBoundLoops.Select(p => p.polygon2d).UnionClipper().ToList();
            //this._obstacleSegs = this.SubCellArea.CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.BsmtWall).Select(p => p.Segment2d).ToList();// 该处障碍物属性线应对情况：车头面对地库外墙线
            this._allRoutes = this.SubCellArea.CellArea.SubParkArea.Bsmt.InBoundEleLines.Select(p => new Route(p.Segment2d)).ToList();
            this._auxiliaryFrame3d = new AuxiliaryFrame3d() { Frame3d = this.SubCellArea.Frame3d, MirrorXcount = 0 };
        }
        /// <summary>
        /// 需要对Frame3d进行定位处理 
        /// 1、将基准线线方向视为X轴正方向 
        /// 2、对齐柱子起点——背靠背相邻两排停车位
        /// 策略：如果该区域两侧都有道路，则依左侧为柱子起点；否则，为道路侧
        /// </summary>
        private void SetFrame3d()
        {
            // 1、横向长矩形上边界需要镜像，调整柱子起点
            Vector2d dirction = this.DatumLine.Direction;
            double angle = dirction.AngleRadToX();
            if (angle > Math.PI * 0.5 && angle < Math.PI * 1.5)// 基准线的方向位于第二与第三坐标系
            {
                this._auxiliaryFrame3d.RreverseX();
            }

            // 2、根据左右道路关系，判断是否需要镜像，调整柱子起点
            bool supplyMirror = JudgRightEdge();
            if (supplyMirror)
            {
                this._auxiliaryFrame3d.RreverseX();
            }
            // 3、存在手动调节的情况
            if (GlobalData.Instance.AdjustStartPoint)
            {
                Vector2d selPoint = this._auxiliaryFrame3d.Frame3d.ToFrameP(GlobalData.Instance.SelPointToAdjustStartPoint.ToVector3d()).ToVector2d();
                Segment2d segment2d = this._auxiliaryFrame3d.Frame3d.ToFrame2dSeg(this.DatumLine);

                double dis01 = selPoint.Distance(segment2d.P0);
                double dis02 = selPoint.Distance(segment2d.P1);

                if (angle > Math.PI * 0.5 && angle < Math.PI * 1.5)
                {
                    if (supplyMirror)
                    {
                        if (dis01 < dis02)
                        {
                        }
                        else
                        {
                            this._auxiliaryFrame3d.RreverseX();
                        }
                    }
                    else
                    {
                        if (dis01 < dis02)
                        {
                            this._auxiliaryFrame3d.RreverseX();
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                    if (supplyMirror)
                    {
                        if (dis01 < dis02)
                        {
                            this._auxiliaryFrame3d.RreverseX();
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        if (dis01 < dis02)
                        {
                        }
                        else
                        {
                            this._auxiliaryFrame3d.RreverseX();
                        }
                    }
                }
            }
        
            this._polygon2d = this._auxiliaryFrame3d.Frame3d.ToFrame2dPoly(this._polygon2d);
            this._bsmtPoly2d = this._auxiliaryFrame3d.Frame3d.ToFrame2dPoly(this._bsmtPoly2d);
            this._obstacleOs = this._obstacleOs.Select(p => new BoundO(this._auxiliaryFrame3d.Frame3d.ToFrame2dPoly(p.polygon2d), p.EleProperty)).ToList();
            //this._obstacleSegs = this._auxiliaryFrame3d.Frame3d.ToFrame2dSeg(this._obstacleSegs).ToList();
            this._allRoutes = this._allRoutes.Select(p => new Route(this._auxiliaryFrame3d.Frame3d.ToFrame2dSeg(p.Segment2d))).ToList();
            this.DatumLine = this._auxiliaryFrame3d.Frame3d.ToFrame2dSeg(this.DatumLine);
        }

        private IEnumerable<PPLocPoint> _tarPpPoints { get; set; }
        internal List<PPLocPoint> TarPpPoints => this._tarPpPoints.ToList();
        /// <summary>
        /// 图形空间分解
        /// 判断当前区域， 普通情况-1- 垂直or平行式 排车位；-2- 将当前区域划分为不同的分段，分别适合普通车位、大车位、微型车位
        /// 如果图形厚度小于4000，则采用 平行式 排车位
        /// </summary>
        private void SpatialRecognition()
        {
            // 4 启动排车位算法
            this._tarPpPoints = this.PsCutClipPattern.Sweep(this._polygon2d, this._bsmtPoly2d, this._obstacleOs.Select(p=>p.polygon2d).ToList());
            // 5 将停车位定位点返回到 世界 Frame3d
            this._tarPpPoints = PPLocPoint.FromFrame2d(this._tarPpPoints, this._auxiliaryFrame3d.Frame3d).ToList();
        }
        /// <summary>
        /// 插车处理==>处理障碍物为 工 或 U 字型
        /// </summary>
        /// <returns></returns>
        //IEnumerable<Polygon2d> PlugInProcessing()
        //{
        //    foreach (var p in this._obstacleOs)
        //    {
        //        if (p.EleProperty == EleProperty.ResidenStruRegion)
        //        {
        //            if (p.polygon2d.InwardOffeet(1.0).Intersects(this.DatumLine))
        //            {
        //                yield return p.polygon2d;
        //            }
        //            else
        //            {
        //                List<Polygon2d> shapedPolys = p.polygon2d.IshapedUshaped(GlobalData.Instance.pSHeight_num, GlobalData.Instance.NorthVehicleInsertionCoefficient_num / 2, GlobalData.Instance.SorthVehicleInsertionCoefficient_num / 2);
        //                foreach (var shapedPoly in shapedPolys)
        //                {
        //                    yield return shapedPoly;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            yield return p.polygon2d;
        //        }
        //    }
        //}
        private bool JudgRightEdge()
        {
            // 基准线 两侧延长 判断与道路线的相交关系
            Segment2d segment2d = this._auxiliaryFrame3d.Frame3d.ToFrame2dSeg(this.DatumLine).TwoWayExtension(GlobalData.Instance.Wd_pri_num);
            List<Vector2d> intrs = new List<Vector2d>();
            foreach (var item in this._allRoutes)
            {
                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2d, this._auxiliaryFrame3d.Frame3d.ToFrame2dSeg(item.Segment2d));
                intrSegment2Segment2.Compute();
                if (intrSegment2Segment2.Quantity == 1)
                {
                    intrs.Add(intrSegment2Segment2.Point0);
                }
            }

            if (intrs.Count == 1)// 只有一个交点
            {
                // 将基准线旋转90°
                //segment2d = segment2d.Rotate(segment2d.Center, Math.PI / 2);
                //int whichSide = segment2d.WhichSide(intrs[0]);
                //if (whichSide == -1)// 右为 1
                //{
                //    return true;
                //}
                return true;

            }
            return false;
        }
    }
}
