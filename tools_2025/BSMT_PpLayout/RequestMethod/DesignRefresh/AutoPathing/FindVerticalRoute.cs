using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 基于现有塔楼结构空间寻找竖向道路
    /// </summary>
    class FindVerticalRoute
    {
        List<BoundO> boundOs;
        List<AxisAlignedBox2d> axisAlignedBox2ds;

        internal FindVerticalRoute(List<BoundO> _boundOs)
        {
            this.boundOs = _boundOs;
            this.axisAlignedBox2ds = _boundOs.Select(p => p.polygon2d.GetBounds()).ToList();
        }

        internal void Execute()
        {
            Vector2d buffer = new Vector2d(0, 1);
            // 右侧的点
            List<Vector2d> rightPoints = this.axisAlignedBox2ds.Select(p => p.Max).ToList();
            List<Segment2d> rightSeg2ds = rightPoints.Select(p => new Segment2d(p - buffer, p + buffer)).ToList();
            rightSeg2ds = rightSeg2ds.Select(p => p.Move(new Vector2d(1, 0), 10)).ToList();
            // 左侧的点
            List<Vector2d> leftPoints = this.axisAlignedBox2ds.Select(p => p.Min).ToList();
            List<Segment2d> leftSeg2ds = leftPoints.Select(p => new Segment2d(p - buffer, p + buffer)).ToList();
            leftSeg2ds = leftSeg2ds.Select(p => p.Move(new Vector2d(-1, 0), 10)).ToList();

            int count = rightPoints.Count;

            for (int j = 0; j < 1; j++)
            {
                //List<Vector2d> _path_Vector2D = item.polygon2d.Vertices.ToList();
                //List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
                //IEnumerable<Line> _lines = _xYZs.ToLines();
                //CMD.Doc.CreateDirectShapeWithNewTransaction(rightSeg2ds.ToLines(), CMD.Doc.ActiveView);
                //CMD.Doc.CreateDirectShapeWithNewTransaction(leftSeg2ds.ToLines(), CMD.Doc.ActiveView);
            }

            List<Vector2d> allVec2ds = new List<Vector2d>();
            allVec2ds.AddRange(rightPoints);
            allVec2ds.AddRange(leftPoints);

            CMD.Doc.CreateDirectShapeWithNewTransaction(allVec2ds.Select(p => ConnectPoint(p, allVec2ds)).ToLines(), CMD.Doc.ActiveView);

        }
        /// <summary>
        /// 用垂直线连接点集
        /// </summary>
        /// <returns></returns>
        List<Segment2d> ConnectPointSet(List<Vector2d> allVec2ds)
        {
            List<Segment2d> result = new List<Segment2d>();

            int count = allVec2ds.Count;
            for (int i = 0; i < count; i++)
            {


                // 连线

            }
            return result;
        }
        /// <summary>
        /// 找到当前点与点集的最近点 并作垂直线
        /// </summary>
        /// <returns></returns>
        Segment2d ConnectPoint(Vector2d vector2d, List<Vector2d> allVec2ds)
        {
            // 距离最近

            Vector2d closestPoint = allVec2ds.Where(p => !p.EpsilonEqual(vector2d, Precision_.TheShortestDistance)).OrderBy(p => p.DistanceSquared(vector2d)).First();

            // 垂直线的创作原理


            // 做两个点的垂直线

            // 垂直线需要进行连接

            // 垂直线
            Vector2d bottom = new Vector2d(vector2d.x, closestPoint.y);

            return new Segment2d(vector2d, bottom);
        }
    }
}
