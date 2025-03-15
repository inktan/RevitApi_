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
using goa.Revit.DirectContext3D;
using ClipperLib;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    abstract class Refresh
    {
        internal BsmtClipByRoad BsmtClipByRoad;

        internal Document Document;

        internal List<SubParkArea> SubParkAreas;
        internal OutWallRing OutWallRing;

        internal List<PPLocPoint> PpLocPoints;
        internal List<Route> Roads;

        internal Refresh(BsmtClipByRoad bsmtClipByRoad, Document document)
        {
            this.BsmtClipByRoad = bsmtClipByRoad;

            this.Roads = new List<Route>();
            this.PpLocPoints = new List<PPLocPoint>();

            this.SubParkAreas = bsmtClipByRoad.SubParkAreas;
            this.OutWallRing = bsmtClipByRoad.OutWallRing;

            this.Document = document;
        }
        internal abstract void Execute();// 
        /// <summary>
        /// 基于道路设计方案，求当前可停车区域的可停车数量
        /// </summary>
        /// <param name="polygon2d">当前可停车区域-该参数具有可变性质，勿更改</param>
        /// <param name="segment2ds">道路空间</param>
        /// <param name="subParkArea">设计环境</param>
        internal List<PPLocPoint> CalPsLocation(IEnumerable<Polygon2d> _polygon2ds, List<Route> segment2ds, SubParkArea subParkArea)
        {
            List<PPLocPoint> result = new List<PPLocPoint>();
            Paths paths = segment2ds.Select(p => p.Segment2d.EndPoints().ToPath()).ToList();
            paths = paths.Offset(GlobalData.Instance.Wd_pri_num / 2, Precision_.ClipperMultiple, EndType.etOpenButt);
            IEnumerable<Polygon2d> pathsRoad = paths.ToPolygon2ds();
            IEnumerable<Polygon2d> polygon2ds = _polygon2ds.DifferenceClipper(pathsRoad);// 减去道路空间

            foreach (var o in polygon2ds)
            {
                //for (int j = 0; j < 1; j++)
                //{
                //    List<Vector2d> _path_Vector2D = o.Vertices.ToList();
                //    List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
                //    IEnumerable<Line> _lines = _xYZs.ToLines();
                //    this.Document.CreateDirectShapeWithNewTransaction(_lines, this.Document.ActiveView);
                //}

                CellArea cellArea = new CellArea(o, subParkArea);
                var tmepResult = CalPs_Backtrack(cellArea, FollowPathCutType.Just_no_BsmeWall);// 塔楼结构投影区域的侧面空间进行兜圈停车处理                       
                result.AddRange(tmepResult);
            }
            return result;
        }
        /// <summary>
        /// 兜圈算法
        /// </summary>
        /// <returns></returns>
        internal List<PPLocPoint> CalPs_Backtrack(CellArea cellArea, FollowPathCutType followPathCutType)
        {
            List<PPLocPoint> psLocPoints = new List<PPLocPoint>();

            DeterBaseline areaDivider02 = new DeterBaseline(cellArea);
            var subCellAreas = areaDivider02.Computer_Chd_Backtrack(followPathCutType).ToList();

            foreach (SubCellArea subCellArea in subCellAreas)
            {
                // 这里做一个调整起点的判断
                if (GlobalData.Instance.SelPointToAdjustStartPoint.x != 0 || GlobalData.Instance.SelPointToAdjustStartPoint.y != 0)// 判断是否激活指定区域
                {
                    if (subCellArea.Polygon2d.Contains(GlobalData.Instance.SelPointToAdjustStartPoint))// 判断激活点位于哪个区域
                    {
                        subCellArea.DeleteUnFixedPses();
                    }
                    else
                    {
                        continue;
                    }
                }

                PsCutClipFactory psArrayCutClip= new PsCutClipFactory(subCellArea, new PsArrCutClipLinearAuto_GerV_MiNiV_GerH());

                psArrayCutClip.Computer();
                psLocPoints.AddRange(psArrayCutClip.TarPpPoints);// 获取各个停车位的停车位置，以及旋转角度
            }
            return psLocPoints;
        }

    }
}
