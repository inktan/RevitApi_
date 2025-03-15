using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using g3;
using PubFuncWt;
using System.Diagnostics;
using goa.Revit.DirectContext3D;
using goa.Common.g3InterOp;

namespace BSMT_PpLayout
{
    class PointColumn : RequestMethod
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        internal PointColumn(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                View nowView = elemsViewLevel.View;

                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;

                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();
                    #region 刷新 删除柱子
                    bsmt.DelUnUsefulPillar();// 全局刷新
                    #endregion

                    // 清除3D服务器
                    //GeometryDrawServersMgr.ClearAllServers();

                    //Outline outline = new Outline(bsmt.BsmtBound.Polygon2d.GetBounds().Min.ToXYZ() - new XYZ(100, 100, 10), bsmt.BsmtBound.Polygon2d.GetBounds().Max.ToXYZ() + new XYZ(100, 100, 10));
                    //GeometryDrawServerInputs geometryDrawServerInputs = new GeometryDrawServerInputs(outline);

                    List<ColLocPoint> tarColumnLocationPoint = ColumnFallingInGap(bsmt);

                    //【】经过二次筛选 将4个重合点过滤为两个点，再将两个点筛选为一个点
                    int beforeCount = tarColumnLocationPoint.Count;

                    while (true)
                    {
                        tarColumnLocationPoint = DelPossibleDuplicateColumnPoint(tarColumnLocationPoint);

                        int nowCount = tarColumnLocationPoint.Count;
                        if (nowCount == beforeCount)
                        {
                            break;
                        }
                        else
                        {
                            beforeCount = nowCount;
                        }
                    }
                    //tarColumnLocationPoint.ForEach(p =>
                    //{
                    //    Line line = new Segment2d(p.Vector2d, p.Vector2d + new Vector2d(10, 10)).ToLine();
                    //    geometryDrawServerInputs.AddCurveToBuffer(line, new ColorWithTransparency(255, 0, 0, 0), new XYZ(0, 0, 0));
                    //});
                    //GeometryDrawServersMgr.ShowGraphics(geometryDrawServerInputs, "_PointColumn_");
                    //this.uiDoc.UpdateAllOpenViews();
                    List<FamilyInstance> _columns = doc.LayoutColumn(nowView, tarColumnLocationPoint);
                }
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 获取落在空隙空间的柱子
        /// </summary>
        List<ColLocPoint> ColumnFallingInGap(Bsmt bsmt)
        {
            // 取出现存在的所有停车位
            List<RevitElePS> ppExits = bsmt.InBoundElePses;
            List<Polygon2d> ppExitsOs = ppExits.Select(p => p.Polygon2d()).ToList();

            // 创建kDtree 创建一次用于检索即可
            List<Vector2d> vector2ds = ppExits.Select(p => p.LocVector2d).ToList();
            alglib.kdtree kDTree = KdTree.ConstructTree<Polygon2d>(ppExitsOs, vector2ds);

            // 取出所有的可能性柱子位置
            List<ColUnitSpace> allColumnUnitSpace = new List<ColUnitSpace>();
            foreach (RevitElePS elePS in ppExits)
            {
                List<ColUnitSpace> columnUnitSpaces = elePS.ColumnUnitSpaces();
                allColumnUnitSpace.AddRange(columnUnitSpaces);
            }

            List<ColLocPoint> tarColumnLocationPoint = new List<ColLocPoint>();

            int i = 0;
            foreach (ColUnitSpace columnUnitSpace in allColumnUnitSpace)
            {
                if (i > 200)
                {
                    //break;
                }

                Polygon2d _polygon2d = columnUnitSpace.polygon2d.InwardOffeet(0.01);// 柱子线圈缩小一个缓冲距离
                //for (int j = 0; j < 1; j++)
                //{
                //    List<Vector2d> _path_Vector2D = _polygon2d.Vertices.ToList();
                //    List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
                //    IEnumerable<Line> _lines = _xYZs.ToLines();
                //    CMD.Doc.CreateDirectShapeWithNewTransaction(_lines, CMD.Doc.ActiveView);
                //}

                //【】寻找距离柱子最近的停车位
                List<Polygon2d> polygon2ds = KdTree.SearchByCoord<Polygon2d>(columnUnitSpace.ColumnLocationPoint.Vector2d, ppExitsOs, kDTree, 6).ToList();// 这里取6个，可以保证，找到该柱点周边所有的停车位线圈

                bool isIn = false;
                foreach (var item01 in polygon2ds)// 找到可以落在当前停车位空隙中的柱子
                {
                    foreach (var item02 in _polygon2d.Vertices)
                    {
                        if (item01.Contains(item02))
                        {
                            isIn = true;
                            break;
                        }
                    }
                    if (isIn)
                    {
                        break;
                    }
                }
                if (!isIn)
                {
                    tarColumnLocationPoint.Add(columnUnitSpace.ColumnLocationPoint);

                }
                i++;
            }
            return tarColumnLocationPoint;
        }

        /// <summary>
        /// 删除重复位置的柱子
        /// </summary>
        /// <param name="tarColumnLocationPoint"></param>
        /// <returns></returns>
        List<ColLocPoint> DelPossibleDuplicateColumnPoint(List<ColLocPoint> tarColumnLocationPoint)
        {
            // 创建kDtree 创建一次用于检索即可
            List<Vector2d> vector2ds = tarColumnLocationPoint.Select(p => p.Vector2d).ToList();
            alglib.kdtree kDTree = KdTree.ConstructTree<ColLocPoint>(tarColumnLocationPoint, vector2ds);

            //【】一次重复筛选 将两个点筛选为一个点，将4个点筛选为两个点

            List<ColLocPoint> delColumnLocationPoints = new List<ColLocPoint>();//
            List<ColLocPoint> unDelColumnLocationPoints = new List<ColLocPoint>();//

            // 每次只提取两个重复柱子位置
            foreach (ColLocPoint columnLocationPoint in tarColumnLocationPoint)
            {
                if (delColumnLocationPoints.Contains(columnLocationPoint) || unDelColumnLocationPoints.Contains(columnLocationPoint))
                {
                    continue;
                }
                //【】检索出的两个柱子，一个为自身，一个为其它
                List<ColLocPoint> twoColumnLocationPoints = KdTree.SearchByCoord<ColLocPoint>(columnLocationPoint.Vector2d, tarColumnLocationPoint, kDTree, 2).ToList();
                if (delColumnLocationPoints.Contains(twoColumnLocationPoints[0]) || delColumnLocationPoints.Contains(twoColumnLocationPoints[1]))
                {
                    continue;
                }
                if (unDelColumnLocationPoints.Contains(twoColumnLocationPoints[0]) || unDelColumnLocationPoints.Contains(twoColumnLocationPoints[1]))
                {
                    continue;
                }

                //【】判断两个柱子是否相交
                if (twoColumnLocationPoints[0].Vector2d.Distance(twoColumnLocationPoints[1].Vector2d).EqualZreo())// 两个柱子距离为0
                {
                    // 将重复的两个柱子，分别输送到不同的收集器
                    delColumnLocationPoints.Add(twoColumnLocationPoints[0]);
                    unDelColumnLocationPoints.Add(twoColumnLocationPoints[1]);
                }
            }
            return tarColumnLocationPoint.Where(p => !delColumnLocationPoints.Contains(p)).ToList();// 
        }
    }
}
