using ClipperLib;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class FollowPathCutting
    {
        internal CellArea CellArea { get; }
        internal Polygon2d Polygon2d => this.CellArea.Polygon2d;
        internal Paths Paths => new Paths() { this.Polygon2d.Vertices.ToPath() };

        /// <summary>
        /// 兜圈
        /// </summary>
        internal FollowPathCutting(CellArea cellAreaa)
        {
            this.CellArea = cellAreaa;
        }

        /// <summary>
        /// 从长边开始，依次使用距停车位长度进行切割异性区域——用于地库最外墙线圈的兜圈排车位处理
        /// --选择是否摒弃掉非主车道边界
        /// --选择是否只保留左右边界
        /// --输入参数改为枚举值
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SubCellArea> Computer(FollowPathCutType followPathCutType)
        {
            IEnumerable<Segment2d> segment2ds = new List<Segment2d>();

            switch (followPathCutType)
            {
                case FollowPathCutType.AllBoundary:
                    {
                        segment2ds = this.Polygon2d.SegmentItr().OrderByDescending(p => p.Length);
                        break;
                    }
                case FollowPathCutType.Just_no_BsmeWall:
                    {
                        segment2ds = CellArea.SelfBoundSegs.Where(p => p.EleProperty != EleProperty.BsmtWall).Select(p => p.Segment2d).OrderByDescending(p => p.Length);
                        break;
                    }
                case FollowPathCutType.Just_Lane:
                    {
                        segment2ds = CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.Lane).Select(p => p.Segment2d).OrderByDescending(p => p.Length);
                        break;
                    }
                case FollowPathCutType.Just_x_Lane:
                    {
                        segment2ds = CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.Lane).Select(p => p.Segment2d).Where(p => p.Direction.y.EqualZreo()).OrderByDescending(p => p.Length);
                        break;
                    }
                case FollowPathCutType.Just_y_Lane:
                    {
                        segment2ds = CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.Lane).Select(p => p.Segment2d).Where(p => !p.Direction.y.EqualZreo());
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            Polygon2d polygon2d = CellArea.Polygon2d;
            //【】原始线圈区域
            Paths paths = this.Paths;
            foreach (Segment2d segment2d in segment2ds)
            {
                // 问题：兜圈的顺序具有不可控性
                // 目前使用是否与塔楼投影区域碰撞进行判断
                //bool isContinue = false;
                //foreach (var item in this.CellArea.SubParkArea.ObstructBoundLoops.Where(p=>p.EleProperty==EleProperty.ResidenStruRegion))// 不够严谨
                //{
                //    Polygon2d poly2d = item.polygon2d.InwardOffeet(1.0);

                //    // 找到边界上的分段点
                //    List<Vector2d> vector2ds = segment2d.DivideBySegNums().ToList();
                //    int isIn = 0;
                //    vector2ds.ForEach(p=> 
                //    {
                //        if(poly2d.Contains(p))
                //        {
                //            isIn++;
                //        }
                //    });

                //    if (isIn> vector2ds.Count/2.0)// 如果障碍物线圈与道路相交，如果道路空间超过一半位于障碍物线圈空间中，则取消该线段的兜圈处理
                //    {
                //        isContinue = true;
                //        break;
                //    }
                //}
                //if (isContinue)
                //{
                //    continue;
                //}

                Paths pathsLeft;

                Paths cliperPath = Paths_(segment2d, polygon2d);

                SubCellArea subCellArea = IntersectMAR(cliperPath, segment2d, paths, out pathsLeft);
                if (subCellArea == null || subCellArea.Polygon2d == null || subCellArea.Polygon2d.Area < 100)
                    continue;
                paths = pathsLeft;

                // 这里需要判断是否

                yield return subCellArea;
            }
        }
        /// <summary>
        /// 注意：这里不使用线段直接两侧偏移，由于图形复杂，两侧同时偏移，会干扰别的地方的几何形体
        /// </summary>
        internal Paths Paths_(Segment2d segment2d, Polygon2d polygon2d)
        {
            //【】基于线做矩形处理 1 求移动方向 
            double height = GlobalData.Instance.pSHeight_num;

            Vector2d center2d = segment2d.Center;
            Vector2d dir01 = segment2d.Rotate(center2d, Math.PI / 2).Direction;
            Vector2d dir02 = dir01 * (-1);

            Vector2d sideVec01 = center2d + dir01;
            Vector2d sideVec02 = center2d + dir02;

            Paths cliperPath = new Paths();  // 直接使用线段偏移成矩形

            if (polygon2d.Contains(sideVec01))
            {
                Segment2d _segment2d = segment2d.Move(dir01, height);
                List<Vector2d> vector2ds = new List<Vector2d>() { segment2d.P0, segment2d.P1, _segment2d.P1, _segment2d.P0 };
                cliperPath = new Paths() { vector2ds.ToPath() };
            }
            else if (polygon2d.Contains(sideVec02))
            {
                Segment2d _segment2d = segment2d.Move(dir02, height);
                List<Vector2d> vector2ds = new List<Vector2d>() { segment2d.P0, segment2d.P1, _segment2d.P1, _segment2d.P0 };
                cliperPath = new Paths() { vector2ds.ToPath() };
            }
            return cliperPath;
        }

        internal SubCellArea IntersectMAR(Paths cliperPath, Segment2d segment2d, Paths paths, out Paths pathsLeft)
        {
            //【】停车位偏移距离
            Paths interSectPahts = paths.IntersectionClip(cliperPath);//【】 Clipper 结束

            // 裁剪需要放大一个极小值 0.003约等于1mm
            pathsLeft = paths.DifferenceClip(cliperPath.ToPolygon2ds().OutwardOffeet(Precision_.TheShortestDistance).ToPaths());//【】 Clipper 结束

            List <Polygon2d> polygon2ds = new List<Polygon2d>();
            if (interSectPahts.Count > 0)
            {
                foreach (var item in interSectPahts)
                {
                    Polygon2d polygon2d = item.ToPolygon2d();
                    
                    if (polygon2d.VertexCount >= 4)
                    {
                        polygon2ds.Add(polygon2d);
                    }
                }
                if (polygon2ds.Count < 1) return null;

                SubCellArea cellArea = new SubCellArea(polygon2ds.OrderBy(p => p.Area).Last(), this.CellArea, segment2d);
                return cellArea;
            }
            else
            {
                return null;
            }
        }

    }
}
