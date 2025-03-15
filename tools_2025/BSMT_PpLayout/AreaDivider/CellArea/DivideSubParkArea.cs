using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{

    class DivideSubParkArea
    {
        internal SubParkArea SubParkArea;
        internal OutWallRing OutWallRing;

        /// <summary>
        /// 基于塔楼投影区域对地库子区域进行切割
        /// </summary>
        /// <param name="_subParkArea"></param>
        internal DivideSubParkArea(SubParkArea _subParkArea)
        {
            this.SubParkArea = _subParkArea;
        }

        internal DivideSubParkArea(OutWallRing _outWallRing)
        {
            this.OutWallRing = _outWallRing;
        }

        /// <summary>
        /// </summary>
        internal IEnumerable<CellArea> Computer()
        {
            if (this.OutWallRing != null)
            {
                // 为最外墙线圈 兜圈停车位空间 两层线圈 需要明确内层线圈的通车道属性 进行兜圈处理
                return GetParkArea(this.OutWallRing.ObstructBoundLoops, this.OutWallRing.Polygon2ds);
            }
            else if (this.SubParkArea != null)
            {
                // 前提是竖向主车道划分设计合理 基于-面积合适的塔楼填充区域-坡道空间-切分地库子停车区域-得到新一轮的停车子区域
                return GetParkArea(this.SubParkArea.ObstructBoundLoops, new List<Polygon2d>() { this.SubParkArea.Polygon2d });
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obstructBoundLoops">设计最外圈范围内包含的障碍物属性线圈</param>
        /// <param name="designScope">设计范围，可能为两线圈，eg-环状</param>
        /// <returns></returns>
        internal IEnumerable<CellArea> GetParkArea(IEnumerable<BoundO> obstructBoundLoops, IEnumerable<Polygon2d> designScope)
        {
            List<BoundO> ObstructBoundLoops = obstructBoundLoops
                .Where(p => p.EleProperty == EleProperty.ResidenStruRegion)
                .Where(p => p.Area > 100.0.SQUARE_METERStoSQUARE_FEET())// 这里使用面积进行区分塔楼投影是否切割停车区域，需要更改为使用类型进行区分
                .ToList();

            if (ObstructBoundLoops.Count < 1)
            {
                if (designScope.Count() == 2)// 环状区域
                {
                    // 找出内圈 向外偏移一个停车距离，作为停车边界，但是该部分停车，需要原地旋转180°

                    yield return new CellArea(this.OutWallRing.SubParkAreaInSelf.Polygon2d, this.OutWallRing.SubParkAreaInSelf);
                }
                else// 非环状区域
                {
                    yield return new CellArea(designScope.First(), this.SubParkArea);
                }
            }
            else
            {
                IEnumerable<Polygon2d> obstalPolys = ObstructBoundLoops.Select(p => p.polygon2d).UnionClipper();
                List<Polygon2d> transitionRegions = new List<Polygon2d>();
                foreach (var o in obstalPolys)// 设计一个合适的裁剪线圈
                {
                    Vector2d lu = o.LUpOfBox2d();
                    Vector2d ru = o.RUpOfBox2d();
                    Vector2d midUp = (lu + ru) / 2;

                    Vector2d ld = o.LDpOfBox2d();
                    Vector2d rd = o.RDpOfBox2d();
                    Vector2d midDown = (ld + rd) / 2;

                    Segment2d segLu = new Segment2d(midUp, midUp - new Vector2d(Precision_.LongestSingleDraw_feet / 30, 0));
                    Segment2d segLd = new Segment2d(midDown, midDown - new Vector2d(Precision_.LongestSingleDraw_feet / 30, 0));

                    Segment2d segRu = new Segment2d(midUp, midUp + new Vector2d(Precision_.LongestSingleDraw_feet / 30, 0));
                    Segment2d segRd = new Segment2d(midDown, midDown + new Vector2d(Precision_.LongestSingleDraw_feet / 30, 0));

                    double disLeft = 0.0;
                    double disRight = 0.0;

                    List<Vector2d> left = new List<Vector2d>();
                    List<Vector2d> right = new List<Vector2d>();

                    foreach (var item in designScope)
                    {
                        left.AddRange(item.FindInterSectionPoint(segLd,2.0));
                        left.AddRange(item.FindInterSectionPoint(segLu,2.0));

                        right.AddRange(item.FindInterSectionPoint(segRd,2.0));
                        right.AddRange(item.FindInterSectionPoint(segRu,2.0));
                    }
                    if (left.Count > 0)
                    {
                        disLeft = left.Select(p=>new Vector2d(p.x,ld.y)).Select(p => p.Distance(ld)).Min();// y值要统一
                    }
                    if (left.Count > 0)
                    {
                        disRight = right.Select(p => new Vector2d(p.x, rd.y)).Select(p => p.Distance(rd)).Min();// y值要统一
                    }

                    Polygon2d transiton = o.GetBounds().ScaleLeftRight(disLeft, disRight).ToPolygon();// ===> 该处放大指标很重要
                    transitionRegions.Add(transiton);
                }

                List<Polygon2d> polygon2ds = designScope.DifferenceClipper(transitionRegions).ToList();
                foreach (Polygon2d o in polygon2ds)
                {
                    if (designScope.Count() == 2)// 环状区域
                    {
                        yield return new CellArea(o, this.OutWallRing.SubParkAreaOut);
                    }
                    else if (this.SubParkArea != null)// 非环状区域
                    {
                        yield return new CellArea(o, this.SubParkArea);
                    }
                }
            }
        }
    }
}
