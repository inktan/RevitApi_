//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.DB;

//using g3;
//using ClipperLib;
//using PubFuncWt;

//namespace BSMT_PpLayout
//{

//    /// <summary>
//    /// 用于碰撞分析 考虑柱子与道路的关系 基于基础停车位进行下一步操作
//    /// </summary>
//    internal class ParkingPP_robot_withRoad 
//    {
//        #region information
//        internal ParkingPP_base Base { get; }//用来判断当前空间能够容纳一个基础停车位-考虑柱网
//        internal ParkingPP_base Robot { get; }//机器人用来检测障碍物干扰情况 因此，机器人的空间包含车道与柱网空间

//        #endregion

//        /// <summary>
//        /// 构造函数 sweepRowCount的参数很重要
//        /// </summary>
//        /// <param name="_parkingPP_base">基础停车位</param>
//        /// <param name="_roadWidth">通车到宽度</param>
//        /// <param name="sweepRowCount">当前位第几排停车</param>
//        /// <param name="columnWidth">柱宽</param>
//        /// <param name="columnBurfferDistance">停车位与柱子的缓冲距离</param>
//        /// <param name="parkingCount">当前第几列停车</param>
//        internal ParkingPP_robot_withRoad(ParkingPP_base _parkingPP_base, int sweepRowCount, double _roadWidth, int parkingCount, double columnWidth, double columnBurfferDistance)
//        {
//            this.Base = _parkingPP_base;
//            this.Robot = (ParkingPP_base)_parkingPP_base.Clone();//基于基础停车位进行复制，引用型类型的新变量指向可能为原指针，需要判断是否需要new处理，值类型的新变量默认指定新指针

//            if (sweepRowCount % 2 == 0)//第一排为南侧加路，第二排为北侧加路
//                AddSourthRoad(_roadWidth);   
//            else
//                AddNorthRoad(_roadWidth);

//            if (parkingCount % GlobalData.NumOfIntervalPP == 0)//第0列附带柱子空间，第?列附带柱子空间-由全局参数控制-……
//                AddColumn(columnWidth, columnBurfferDistance);

//            this.Robot.UpdatePolygon2d();
//            this.Base.UpdatePolygon2d();
//        }
//        /// <summary>
//        /// 添加柱子占据空间
//        /// </summary>
//        void AddColumn(double columnWidth, double columnBurfferDistance)
//        {
//            Vector2d columnMove = new Vector2d(columnWidth + columnBurfferDistance, 0);
//            this.Robot.RDpoint += columnMove;
//            this.Robot.RUpoint += columnMove;
//            this.Robot.PlacePoint += columnMove;

//            this.Base.RDpoint += columnMove;
//            this.Base.RUpoint += columnMove;
//            this.Base.PlacePoint += columnMove;
//        }

//        /// <summary>
//        /// 增加路宽 北侧
//        /// </summary>
//        void AddNorthRoad(double _roadWidth)
//        {
//            Vector2d northRoad = new Vector2d(0, _roadWidth);
//            this.Robot.LUpoint += northRoad;
//            this.Robot.RUpoint += northRoad;
//        } 
//        /// <summary>
//        /// 增加路宽 南侧
//        /// </summary>
//        void AddSourthRoad(double _roadWidth)
//        {
//            Vector2d northRoad = new Vector2d(0, _roadWidth);
//            this.Robot.LDpoint -= northRoad;
//            this.Robot.RDpoint -= northRoad;
//        }
//        /// <summary>
//        /// 检测停车空间是否与障碍物存在碰撞关系  用于判断车头是否与地库外墙线产生碰撞
//        /// </summary>
//        /// <returns></returns>
//        internal bool Collision(IEnumerable<Segment2d> segment2ds)
//        {
//            Polygon2d inO = this.Robot.InwardPolygon2d;
//            foreach (Segment2d seg in segment2ds)
//            {
//                if (inO.Intersects(seg)) return true;
//            }
//            return false;
//        }
//        internal bool Collision(IEnumerable<Polygon2d> polygon2ds)
//        {
//            Polygon2d inO = this.Robot.InwardPolygon2d;
//            foreach (Polygon2d polygon2d in polygon2ds)
//            {
//                if (inO.Intersects(polygon2d) || inO.Contains(polygon2d) || polygon2d.Contains(inO)) return true;
//            }
//            return false;
//        }
//        /// <summary>
//        /// 判断基础停车位是否落在停车区域中
//        /// </summary>
//        internal bool isInPolygon(Polygon2d polygon2d)
//        {
//            Polygon2d outO = polygon2d.OutwardOffeet(Precision_.TheShortestDistance*10);
//            Polygon2d inO = this.Base.InwardPolygon2d;
//            return outO.Contains(inO);
//        }
//    }
//}
