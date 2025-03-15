
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using g3;
using PubFuncWt;

using ClipperLib;
using System;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 基于边界投影线段进行首次求取最大面积，需要记录最大面积区域polyogn，以及该区域对应的投影线段
    /// </summary>
    class CellArea
    {
        //internal bool IsObstaclRegion = false;

        internal BoundSeg UpSeg;
        internal BoundSeg DownSeg;

        internal double ScanAngle = -1;//基于任一边界旋转至x轴正方向上所需角度 默认为-1
        internal Document Doc => this.SubParkArea.Doc;
        internal SubParkArea SubParkArea;

        internal Polygon2d Polygon2d;

        internal List<BoundSeg> SelfBoundSegs;// 具体为 subParkArea 的边界属性，用于该区域的兜圈处理
        internal CellArea(Polygon2d _polygon2d, SubParkArea _subParkArea)
        {
            this.Polygon2d = _polygon2d;
            this.SubParkArea = _subParkArea;

            this.SelfBoundSegs = selfBoundSegs(this.SubParkArea).ToList();

            HandleUpLowBoundary();
        }

        private IEnumerable<BoundSeg> selfBoundSegs(SubParkArea _subParkArea)
        {
            List<BoundSeg> selfBoundSegs = new List<BoundSeg>();

            IEnumerable<BoundSeg> boundSegs = _subParkArea.SelfBoundSegs.Where(p => p.EleProperty != EleProperty.Lane);
            foreach (Segment2d segment2d in this.Polygon2d.SegmentItr())
            {
                bool isCoincide = false;
                // 判断与上一层级的非车道属性线重叠情况
                foreach (BoundSeg boundSeg in boundSegs)
                {
                    IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(segment2d, boundSeg.Segment2d);
                    intrSegment2Segment2.Compute();
                    if (intrSegment2Segment2.Quantity == 2)// 重叠的情况
                    {
                        BoundSeg boundarySegment = new BoundSeg(segment2d, boundSeg.EleProperty);
                        selfBoundSegs.Add(boundarySegment);
                        isCoincide = true;
                        break;
                    }
                }

                if (!isCoincide)// 不重叠的情况
                {
                    BoundSeg boundarySegment = new BoundSeg(segment2d, EleProperty.Lane);
                    selfBoundSegs.Add(boundarySegment);
                }
            }
            return selfBoundSegs;
        }
        void HandleUpLowBoundary()
        {
            // 提取线圈
            Polygon2d polygon2d = this.Polygon2d;
            List<Polygon2d> polygon2ds = this.SubParkArea.ObstructBoundLoops.Select(p => p.polygon2d).ToList();

            // 找到最上、最下边界
            this.UpSeg = this.SelfBoundSegs.OrderBy(p => p.Segment2d.Center.y).Last();
            this.DownSeg = this.SelfBoundSegs.OrderBy(p => p.Segment2d.Center.y).First();

            // 重置属性
            if (this.UpSeg.EleProperty != EleProperty.BsmtWall)
            {
                if (this.UpSeg.Segment2d.ToRenct2d(5).Intersects(polygon2ds))
                {
                    this.UpSeg.EleProperty = EleProperty.Obstructive;
                }
                else
                {
                    this.UpSeg.EleProperty = EleProperty.Lane;
                }
            }
            if (this.DownSeg.EleProperty != EleProperty.BsmtWall)
            {
                if (this.DownSeg.Segment2d.ToRenct2d(5).Intersects(polygon2ds))
                {
                    this.DownSeg.EleProperty = EleProperty.Obstructive;
                }
                else
                {
                    this.DownSeg.EleProperty = EleProperty.Lane;
                }
            }


            // 需要对上下边界进行水平调整
            if (!this.UpSeg.Segment2d.Direction.y.EqualZreo())// 不平行
            {
                Segment2d segment2d = this.UpSeg.Segment2d;
                double y = segment2d.EndPoints().OrderBy(p => p.y).First().y;

                Vector2d p0 = segment2d.P0;
                Vector2d p1 = segment2d.P1;

                this.UpSeg = new BoundSeg(new Segment2d(new Vector2d(p0.x, y), new Vector2d(p1.x, y)), this.UpSeg.EleProperty);
            }
            if (!this.DownSeg.Segment2d.Direction.y.EqualZreo())// 不平行
            {
                Segment2d segment2d = this.DownSeg.Segment2d;
                double y = segment2d.EndPoints().OrderBy(p => p.y).Last().y;

                Vector2d p0 = segment2d.P0;
                Vector2d p1 = segment2d.P1;

                this.DownSeg = new BoundSeg(new Segment2d(new Vector2d(p0.x, y), new Vector2d(p1.x, y)), this.DownSeg.EleProperty);
            }
        }

        /// <summary>
        /// 从上下边界关系放大停车区域
        /// </summary>
        /// <returns></returns>
        internal Polygon2d ZoomUpLowBoundary()
        {
            List<Polygon2d> polygon2ds = new List<Polygon2d>();
            if (this.UpSeg.EleProperty != EleProperty.BsmtWall && this.UpSeg.EleProperty != EleProperty.Lane)
            {
                polygon2ds.Add(this.UpSeg.Segment2d.ToRenct2d(GlobalData.Instance.Wd_pri_num * 2));
            }
            if (this.DownSeg.EleProperty != EleProperty.BsmtWall && this.DownSeg.EleProperty != EleProperty.Lane)
            {
                polygon2ds.Add(this.DownSeg.Segment2d.ToRenct2d(GlobalData.Instance.Wd_pri_num * 2));
            }

            IEnumerable<Polygon2d> result = this.Polygon2d.UnionClipper(polygon2ds);

            if (result.Count() > 0)
            {
                return result.FirstOrDefault();
            }
            else
            {
                return new Polygon2d();
            }
        }
    }
}
