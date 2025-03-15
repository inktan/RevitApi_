//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI.Selection;
//using System.Diagnostics;
//using PubFuncWt;
//using g3;
//using goa.Common;
//using ClipperLib;

//namespace BSMT_PpLayout
//{
//    using cInt = Int64;

//    using Path = List<IntPoint>;
//    using Paths = List<List<IntPoint>>;

//    /// <summary>
//    /// 使用障碍物切割子区域===>基于南北关系，设计4种平行车道规则，取最优
//    /// </summary>
//    class ObstacleCutter : Refresh
//    {
//        internal ObstacleCutter(BsmtClipByRoad bsmtClipByRoad, Document document ) : base(bsmtClipByRoad, document) { }

//        internal override void Execute()
//        {
//            // 环状区域
         
//            DivideSubParkArea divideSubParkArea;

//            if (this.OutWallRing != null&& this.OutWallRing.Polygon2ds.Count == 2)
//            { 
//                divideSubParkArea = new DivideSubParkArea(this.OutWallRing);
//                if (this.OutWallRing.ObstructBoundLoops.Count<1)// 无障碍物的环状
//                {
//                    foreach (var item in divideSubParkArea.Computer())// 直接进行环状兜圈处理
//                    {
//                        List<PPLocPoint> tmepResult = CalPs_Backtrack(item, FollowPathCutType.AllBoundary);// 环状内圈兜圈处理，与地库外墙线无关
//                        this.PpLocPoints.AddRange(tmepResult);
//                    }
//                }
//                else
//                {
//                    List<Route> roads = GetRoads(divideSubParkArea.Computer());
//                    for (int j = 0; j < 1; j++)
//                    {
//                        //List<Vector2d> _path_Vector2D = polygon2d.Vertices.ToList();
//                        //List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
//                        //IEnumerable<Line> _lines = _xYZs.ToLines();
//                        // this.Document.CreateDirectShapeWithNewTransaction(roads.ToLines(), this.Document.ActiveView);
//                    }
//                    List<PPLocPoint> pPLocPoints = CalPsLocation(this.OutWallRing.Polygon2ds, roads, this.OutWallRing.SubParkAreaOut);

//                    this.Roads.AddRange(roads);
//                    this.PpLocPoints.AddRange(pPLocPoints);
//                }
//            }

//            // 所有子区域

//            int i = 0;
//            foreach (var item in this.SubParkAreas)
//            {
//                if (i != 2135)
//                {
//                    //continue;
//                }
//                item.Computer();
//                /*
//                 * Y轴-方向通车道合理——基于塔楼填充区域切割图形
//                 * 对切割后的图形进行道路空间设计
//                 * 大区域分割成小区域 ==> 对小区域的最优道路求解 ==> 汇总所有小区域的道路反馈到大区域
//                 */
//                divideSubParkArea = new DivideSubParkArea(item);
//                List<Route> roads = GetRoads(divideSubParkArea.Computer());
//                for (int j = 0; j < 1; j++)
//                {
//                    //List<Vector2d> _path_Vector2D = polygon2d.Vertices.ToList();
//                    //List<XYZ> _xYZs = _path_Vector2D.ToXyzs().ToList();
//                    //IEnumerable<Line> _lines = _xYZs.ToLines();
//                    // this.Document.CreateDirectShapeWithNewTransaction(roads.ToLines(), this.Document.ActiveView);
//                }
//                List<PPLocPoint> pPLocPoints = CalPsLocation(new List<Polygon2d>() { item.Polygon2d }, roads, item);

//                this.Roads.AddRange(roads);
//                this.PpLocPoints.AddRange(pPLocPoints);
//            }
          
//        }
//        internal List<Route> GetRoads(IEnumerable<CellArea> cellAreas)
//        {

//            List<Route> roads = new List<Route>();
//            foreach (var cellArea in cellAreas)
//            {
//                Polygon2d polygon2d = cellArea.ZoomUpLowBoundary();

         
//                //continue;

//                FourRoadDistriNnorSou pathFinding = new FourRoadDistriNnorSou(cellArea);
//                SchemeInfo schemeInfo = pathFinding.Computer();// 会提出四种方案

//                // 求出最优方案路径-四选一
//                Dictionary<List<PPLocPoint>, List<Route>> dict = new Dictionary<List<PPLocPoint>, List<Route>>();
//                foreach (var item in schemeInfo.Designs)
//                {
//                    //if(item.Count==0)// 当前子区域在第一层级不存在设计道路线
//                    //{
//                    //    continue;
//                    //}

//                    // 需要将区域在y轴方向拉伸
//                    // 拉伸原则-非地库外墙线
//                    List<PPLocPoint> temp = CalPsLocation(new List<Polygon2d>() { polygon2d}, item, cellArea.SubParkArea);

//                    dict.Add(temp, item);
//                }

//                if (dict.Count > 0)
//                {
//                    var dictSort = from objDic in dict orderby objDic.Key.Count descending select objDic;
//                    roads.AddRange(dictSort.First().Value);
//                }
//            }
//            return roads;

//        }
        
//    }
//}
