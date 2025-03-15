
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;
using g3;

namespace BSMT_PpLayout
{

    class CheckCollisions : RequestMethod
    {
        public CheckCollisions(UIApplication uiapp) : base(uiapp)
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

                    elementIds = new List<ElementId>();
                    // 停车位碰撞
                    checkCollisions(bsmt);
                    // 停车位与路的关系
                    CarHitRoad(bsmt);

                    if (elementIds.Count > 0)
                    {
                        this.uiDoc.ShowElements(elementIds);// 窗口中心显示碰撞
                        sel.SetElementIds(elementIds);// 窗口高亮显示碰撞
                    }
                }
            }
        }

        List<ElementId> elementIds { get; set; }

        /// <summary>
        /// 检查碰撞问题
        /// </summary>
        internal void checkCollisions(Bsmt bsmt)
        {
            List<RevitElePS> elePses = bsmt.InBoundElePses;
            if (elePses.Count > 0)
            {

                // 创建kDtree 创建一次用于检索即可
                List<Vector2d> vector2Ds = elePses.Select(p => p.LocVector2d).ToList();
                alglib.kdtree kDTree = KdTree.ConstructTree<RevitElePS>(elePses, vector2Ds);

                // 使用KDTree进行检索
                foreach (var item in elePses)
                {
                    Polygon2d polygon2d = item.Polygon2d();
                    Polygon2d polygon2dOffeet = polygon2d.InwardOffeet(Precision_.TheShortestDistance*10);

                    List<RevitElePS> parkingUnitExits1 = KdTree.SearchByCoord<RevitElePS>(item.LocVector2d, elePses, kDTree, 3);
                    for (int i = 0; i < parkingUnitExits1.Count; i++)
                    {
                        if (item.Ele.Id != parkingUnitExits1[i].Ele.Id)
                        {
                            RevitElePS _parkingUnitExit = parkingUnitExits1[i];
                            Polygon2d _polygon2d = _parkingUnitExit.Polygon2d();
                            Polygon2d _polygon2dOffeet = _polygon2d.InwardOffeet(Precision_.TheShortestDistance*10);

                            if (polygon2dOffeet.Intersects(_polygon2dOffeet))
                            {
                                if (!elementIds.Contains(item.Ele.Id))
                                {

                                    //for (int j = 0; j < 1; j++)
                                    //{
                                    //    List<Vector2d> _path_Vector2D = polygon2dOffeet.Vertices.ToList();
                                    //    List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
                                    //    IEnumerable<Line> _lines = _xYZs.ToLines();
                                    //    CMD.Doc.CreateDirectShapeWithNewTransaction(_lines, CMD.Doc.ActiveView);
                                    //}
                                    //for (int j = 0; j < 1; j++)
                                    //{
                                    //    List<Vector2d> _path_Vector2D = _polygon2dOffeet.Vertices.ToList();
                                    //    List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
                                    //    IEnumerable<Line> _lines = _xYZs.ToLines();
                                    //    CMD.Doc.CreateDirectShapeWithNewTransaction(_lines, CMD.Doc.ActiveView);
                                    //}

                                    elementIds.Add(item.Ele.Id);
                                }
                                if (!elementIds.Contains(_parkingUnitExit.Id))
                                {
                                    elementIds.Add(_parkingUnitExit.Id);
                                }
                            }
                        }
                    }
                }

            }
        }
        /// <summary>
        /// 汽车撞路
        /// </summary>
        internal void CarHitRoad(Bsmt bsmt)
        {
            foreach (var item in bsmt.InBoundElePses)
            {
                Polygon2d carPolygon2d = item.Polygon2d().InwardOffeet(Precision_.TheShortestDistance);
                Polygon2d roadPolygon2d = new Polygon2d();
                foreach (var p in bsmt.InBoundEleLines)
                {
                    if (p.EleProperty == EleProperty.PriLane)
                    {
                        roadPolygon2d = p.Segment2d.ToRenct2d(GlobalData.Instance.Wd_pri_num / 2);
                    }
                    else if (p.EleProperty == EleProperty.SecLane)
                    {
                        roadPolygon2d = p.Segment2d.ToRenct2d(GlobalData.Instance.Wd_sec_num / 2);
                    }
                    else if (p.EleProperty == EleProperty.CusLane)
                    {
                        roadPolygon2d = p.Segment2d.ToRenct2d(GlobalData.Instance.Wd_CustomWidth_num / 2);
                    }

                    if (carPolygon2d.Intersects(roadPolygon2d))
                    {
                        elementIds.Add(item.Id);
                        break;
                    }
                }
            }
        }
    }
}
