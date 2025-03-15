
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClipperLib;
using g3;
using PubFuncWt;
using goa.Common;

using Autodesk.Revit.DB;

// Revit打印线条的方法

//for (int j = 0; j < 1; j++)
//{
//    List<Vector2d> _path_Vector2D = item.polygon2d.Vertices.ToList();
//    List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
//    IEnumerable<Line> _lines = _xYZs.ToLines();
//     CMD.Doc.CreateDirectShapeWithNewTransaction(_lines,   CMD.Doc.ActiveView);
//}

namespace BSMT_PpLayout
{
    /// <summary>
    /// 对每个地库_子停车区域，基于特定规则，进一步切分父子停车区域
    /// </summary>
    class DeterBaseline
    {
     
        internal Document Doc => this.CellArea.Doc;
        internal CellArea CellArea { get; set; }
        internal DeterBaseline(CellArea _cellArea)
        {
            this.CellArea = _cellArea;
        }

        /// <summary>
        /// 矩形切换
        /// </summary>
        internal IEnumerable<SubCellArea> Computer_Chd_Rectangle(Segment2d segment2d)
        {
            Polygon2d _polygon2d = this.CellArea.Polygon2d;
            SubCellArea cellArea = new SubCellArea(_polygon2d, this.CellArea, segment2d);
            yield return cellArea;
        }
        /// <summary>
        /// 指定平行边
        /// </summary>
        internal IEnumerable<SubCellArea> Computer_Chd_Boundary(Segment2d segment2d)
        {
            SubCellArea cellArea = new SubCellArea(this.CellArea.Polygon2d, this.CellArea, segment2d);
            yield return cellArea;
        }
 
        /// <summary>
        /// 线性阵列 在上一阶段已经求出停车空间
        /// </summary>
        internal IEnumerable<SubCellArea> Computer_Chd_LineArray(Segment2d segment2d)
        {
            SubCellArea cellArea = new SubCellArea(this.CellArea.Polygon2d, this.CellArea, segment2d);
            yield return cellArea;
        }

        /// <summary>
        /// 图形拆分原则 -三角形-不拆分-矩形与梯形-平行边中中较长车道中心线属性边-采用最大面积区域切割法（不稳定）——自动基于最长边
        /// 需要做进一步进行图形分析工作
        /// </summary>
        internal IEnumerable<SubCellArea> Computer_Chd_Longest()
        {
            Segment2d segment2d = this.CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.Lane).OrderBy(p => p.Segment2d.Length).Select(p => p.Segment2d).Last();
            SubCellArea cellArea = new SubCellArea(this.CellArea.Polygon2d, this.CellArea, segment2d);
            yield return cellArea;
        }
        /// <summary>
        /// 每个边都做平行处理 这个求出的每个排车区域没有相关性
        /// </summary>
        internal IEnumerable<SubCellArea> Computer_Chd_EveryEdge()
        {
            foreach (Segment2d segment2d in this.CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.Lane).Select(p => p.Segment2d))
            {
                SubCellArea cellArea = new SubCellArea(this.CellArea.Polygon2d, this.CellArea, segment2d);
                yield return cellArea;
            }
        }
        internal IEnumerable<SubCellArea> Computer_Chd_EveryEdge_UD()
        {
            foreach (Segment2d segment2d in this.CellArea.SelfBoundSegs.Where(p => p.EleProperty == EleProperty.Lane).Select(p => p.Segment2d).Where(p=>p.Direction.y.EqualZreo()))
            {
                SubCellArea cellArea = new SubCellArea(this.CellArea.Polygon2d, this.CellArea, segment2d);
                yield return cellArea;
            }
        }
        /// <summary>
        /// 兜圈
        /// </summary>
        internal IEnumerable<SubCellArea> Computer_Chd_Backtrack(FollowPathCutType followPathCutType)
        {
            FollowPathCutting followPathCutting = new FollowPathCutting(this.CellArea);
            return followPathCutting.Computer(followPathCutType).ToList();
        }
      
    }//
}//
