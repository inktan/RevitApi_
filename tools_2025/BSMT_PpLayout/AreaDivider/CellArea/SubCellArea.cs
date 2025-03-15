
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using g3;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 基于边界投影线段进行首次求取最大面积，需要记录最大面积区域polyogn，以及该区域对应的投影线段
    /// </summary>
    class SubCellArea
    {
        internal Document Doc => this.CellArea.Doc;
        internal CellArea CellArea { get; }
        internal Polygon2d Polygon2d { get; }
        internal List<BoundO> ObstructBoundLoops;
        /// <summary>
        /// 基准线旋转至x轴正方向上所需角度
        /// </summary>
        //internal double ScanAngle { get; }
        // 不再使用角度，使用向量表达停车位的方向
        internal Segment2d DatumLine { get; }

        /// <summary>
        /// 用于
        /// </summary>
        internal Frame3d Frame3d { get; set; }

        internal SubCellArea(Polygon2d _polygon2d, CellArea _cellArea, Segment2d _datumLine)
        {
            this.Polygon2d = _polygon2d;
            this.CellArea = _cellArea;
            this.DatumLine = _datumLine;
            //this.ScanAngle = _datumLine.Direction.AngleToX() * (-1);// 乘以 -1 是为了 满足图形以基准线 旋转至水平面上
            this.ObstructBoundLoops = GetObstructBoundLoops().ToList();

            this.Frame3d = this.DatumLine.P0.CreatFrame3d(this.DatumLine.Direction);
        }

        /// <summary>
        /// 相关 障碍物属性线圈
        /// </summary>
        private IEnumerable<BoundO> GetObstructBoundLoops()
        {
            Polygon2d poly2d = this.Polygon2d.OutwardOffeet(5);
            foreach (var p in this.CellArea.SubParkArea.ObstructBoundLoops)
            {
                if (poly2d.Intersects(p.polygon2d) || poly2d.Contains(p.polygon2d) || p.polygon2d.Contains(poly2d))// 偏移处理，应对的情况为，停车区域与障碍物线圈存在贴边的情况
                {
                    // 这里需要判断 是否处理障碍物为 工 或 U 字型

                    //int interCount = 0;
                    //foreach (var item in p.polygon2d.SegmentItr())
                    //{
                    //    if (this.ScanSse2d.Intersects(item))
                    //    {
                    //        interCount++;
                    //    }
                    //}

                    if (p.EleProperty == EleProperty.ResidenStruRegion)
                    {
                        if (p.polygon2d.InwardOffeet(1.0).Intersects(this.DatumLine))
                        {
                            yield return p;
                        }
                        else
                        {
                            List<Polygon2d> shapedPolys = 
                                p.polygon2d.IshapedUshaped(
                                    GlobalData.Instance.pSHeight_num, 
                                    GlobalData.Instance.NorthVehicleInsertionCoefficient_num / 2,
                                    GlobalData.Instance.SorthVehicleInsertionCoefficient_num / 2,
                                    GlobalData.Instance.LeftVehicleInsertionCoefficient_num / 2,
                                    GlobalData.Instance.RightVehicleInsertionCoefficient_num / 2);

                            foreach (var shapedPoly in shapedPolys)
                            {
                                //CMD.Doc.CreateDirectShapeWithNewTransaction(shapedPoly.ToCurveLoop());

                                yield return new BoundO(shapedPoly, p.EleProperty);
                            }
                        }
                    }
                    else
                    {
                        yield return p;
                    }
                }
            }
        }

        /// <summary>
        /// 删除未锁定 车位
        /// </summary>
        internal void DeleteUnFixedPses()
        {
            using (var deleteTrans = new Transaction(this.Doc, "删除所有未锁定停车位"))
            {
                deleteTrans.Start();
                this.Doc.Delete(this.CellArea.SubParkArea.RevitEleFsesUnFixed.Where(p=>this.Polygon2d.Contains(p.Polygon2d().Center())).Select(p => p.Id).ToList());
                this.Doc.Delete(this.CellArea.SubParkArea.RevitEleColumnsUnFixed.Where(p => this.Polygon2d.Contains(p.Polygon2d().Center())).Select(p => p.Id).ToList());
                deleteTrans.Commit();
            }
        }
    }
}
