//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.DB;

//using g3;
//using ClipperLib;
//using PublicProjectMethods_;

//namespace ParkingLayoutEfficientNewStructual
//{

//    using cInt = Int64;

//    using Path = List<IntPoint>;
//    using Paths = List<List<IntPoint>>;
//    using Path_Vector2d = List<Vector2d>;
//    using Paths_Vector2d = List<List<Vector2d>>;
//    /// <summary>
//    /// 用于裁剪计算
//    /// </summary>
//    class ParkingUnitSpace
//    {
//        #region 车位信息，用于裁剪停车区域
//        internal Vector2d LeftDownVector2d { get; set; }
//        internal Vector2d LeftUpVector2d { get; set; }
//        internal Vector2d RightUpVector2d { get; set; }
//        internal Vector2d RightDownVector2d { get; set; }
//        internal Vector2d MiddleVector2d { get; set; }
//        internal Path_Vector2d Path_Vector2d { get; set; }
//        internal Path Path { get; set; }
//        internal Polygon2d Polygon2D { get; set; }
//        #endregion

//        #region 车位自身四个角点，用来判断，车位族实例是否可以存在当前停车区域中
//        internal Vector2d self_LeftDownVector2d { get; set; }
//        internal Vector2d self_LeftUpVector2d { get; set; }
//        internal Vector2d self_RightUpVector2d { get; set; }
//        internal Vector2d self_RightDownVector2d { get; set; }
//        internal Vector2d self_MiddleVector2d { get; set; }

//        internal Path_Vector2d self_Path_Vector2d { get; set; }
//        internal Polygon2d self_Polygon2d { get; set; }

//        internal Path_Vector2d self_Path_Vector2d_Road { get; set; }
//        internal Polygon2d self_Polygon2d_Road { get; set; }

//        #endregion

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        internal ParkingUnitSpace()
//        {
//            this.LeftDownVector2d = Vector2d.Zero;
//            this.LeftUpVector2d = new Vector2d(10, 0);
//            this.RightUpVector2d = new Vector2d(10, 10);
//            this.RightDownVector2d = new Vector2d(10, 0);
//            this.MiddleVector2d = new Vector2d(5, 5);

//            this.Path_Vector2d = new Path_Vector2d() { this.LeftDownVector2d, this.LeftUpVector2d, this.RightUpVector2d, this.RightDownVector2d };
//            this.Polygon2D = new Polygon2d(this.Path_Vector2d);
//            this.Path = this.Path_Vector2d.ToPath();
//        }
//        /// <summary>
//        /// 构造函数， 裁剪单元停车空间无车道
//        /// </summary>
//        internal ParkingUnitSpace(Vector2d leftDown, double height, double wight, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn)
//        {
//            GetCarFourPoints( leftDown,  height,  wight,  columnHeight,  columnWidth,  columnBurfferDistance,  isHasCloumn);

//            if (!isHasCloumn)//无柱距
//            {
//                this.LeftDownVector2d = leftDown;
//                this.LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height);
//                this.RightUpVector2d = new Vector2d(leftDown.x + wight, leftDown.y + height);
//                this.RightDownVector2d = new Vector2d(leftDown.x + wight, leftDown.y);
//                this.MiddleVector2d = new Vector2d(leftDown.x + wight / 2, leftDown.y + height / 2);
//            }
//            else if (isHasCloumn)//有柱距
//            {
//                this.LeftDownVector2d = leftDown;
//                this.LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height);
//                this.RightUpVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance * 2, leftDown.y + height);
//                this.RightDownVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance * 2, leftDown.y);
//                this.MiddleVector2d = new Vector2d(leftDown.x + wight / 2 + columnWidth + columnBurfferDistance * 2, leftDown.y + height / 2);
//            }

//            this.Path_Vector2d = new Path_Vector2d() { leftDown - new Vector2d(0, 1) - new Vector2d(1, 0), this.LeftUpVector2d - new Vector2d(1, 0), this.RightUpVector2d, this.RightDownVector2d - new Vector2d(0, 1) };//为了满足能够完美切除，底部往下做一个缓冲值
//            this.Polygon2D = new Polygon2d(this.Path_Vector2d);
//            this.Path = this.Path_Vector2d.ToPath();
//        }
//        /// <summary>
//        /// 构造函数， 裁剪单元停车空间拥有车道
//        /// </summary>
//        internal ParkingUnitSpace(Vector2d leftDown, double height, double wight, double mainRoad, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn, string roadDirection)
//        {
//            GetCarFourPoints(leftDown, height, wight, columnHeight, columnWidth, columnBurfferDistance, isHasCloumn);
//            if(roadDirection == "N")
//            {
//                _RoadNorth(leftDown, height, wight, mainRoad, columnHeight, columnWidth, columnBurfferDistance, isHasCloumn);
//            }
//            else if(roadDirection == "S")
//            {
//                _RoadSouth(leftDown, height, wight, mainRoad, columnHeight, columnWidth, columnBurfferDistance, isHasCloumn);
//            }
//        }
//        /// <summary>
//        /// 裁剪车位空间 北侧开路
//        /// </summary>
//        internal void _RoadNorth(Vector2d leftDown, double height, double wight, double mainRoad, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn )
//        {

//            if (!isHasCloumn)//无柱距
//            {
//                this.LeftDownVector2d = leftDown;
//                this.LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height + mainRoad);
//                this.RightUpVector2d = new Vector2d(leftDown.x + wight, leftDown.y + height + mainRoad);
//                this.RightDownVector2d = new Vector2d(leftDown.x + wight, leftDown.y);

//                this.MiddleVector2d = new Vector2d(leftDown.x + wight / 2, leftDown.y + height / 2);
//            }
//            else if (isHasCloumn)//有柱距
//            {
//                this.LeftDownVector2d = leftDown;
//                this.LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height + mainRoad);
//                this.RightUpVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance , leftDown.y + height + mainRoad);
//                this.RightDownVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance , leftDown.y);

//                this.MiddleVector2d = new Vector2d(leftDown.x + wight / 2 + columnWidth + columnBurfferDistance , leftDown.y + height / 2);
//            }

//            this.Path_Vector2d = new Path_Vector2d() { this.LeftDownVector2d - new Vector2d(0, 1) - new Vector2d(1, 0), this.LeftUpVector2d - new Vector2d(1, 0) , this.RightUpVector2d , this.RightDownVector2d - new Vector2d(0, 1) };//为了满足能够完美切除，底部往下做一个缓冲值
//            this.Polygon2D = new Polygon2d(this.Path_Vector2d);
//            this.Path = this.Path_Vector2d.ToPath();
//        }
//        /// <summary>
//        /// 裁剪车位空间 南侧开路
//        /// </summary>
//        internal void _RoadSouth(Vector2d leftDown, double height, double wight, double mainRoad, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn)
//        {
//            if (!isHasCloumn)//无柱距
//            {
//                this.LeftDownVector2d = new Vector2d(leftDown.x, leftDown.y - mainRoad);
//                this.LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height);
//                this.RightUpVector2d = new Vector2d(leftDown.x + wight, leftDown.y + height);
//                this.RightDownVector2d = new Vector2d(leftDown.x + wight, leftDown.y - mainRoad);

//                this.MiddleVector2d = new Vector2d(leftDown.x + wight / 2, leftDown.y + height / 2);
//            }
//            else if (isHasCloumn)//有柱距
//            {
//                this.LeftDownVector2d = new Vector2d(leftDown.x, leftDown.y - mainRoad);
//                this.LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height);
//                this.RightUpVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance, leftDown.y + height);
//                this.RightDownVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance, leftDown.y - mainRoad);

//                this.MiddleVector2d = new Vector2d(leftDown.x + wight / 2 + columnWidth + columnBurfferDistance , leftDown.y + height / 2);
//            }

//            this.Path_Vector2d = new Path_Vector2d() { this.LeftDownVector2d - new Vector2d(0, 1) - new Vector2d(1, 0), this.LeftUpVector2d - new Vector2d(1, 0) , this.RightUpVector2d, this.RightDownVector2d - new Vector2d(0, 1) };//为了满足能够完美切除，底部往下做一个缓冲值
//            this.Polygon2D = new Polygon2d(this.Path_Vector2d);
//            this.Path = this.Path_Vector2d.ToPath();
//        }
//        /// <summary>
//        /// 获取停车位的四个角点
//        /// </summary>
//        internal void GetCarFourPoints(Vector2d leftDown, double height, double wight, double columnHeight, double columnWidth, double columnBurfferDistance, bool isHasCloumn)
//        {
//            if (!isHasCloumn)//无柱距
//            {
//                this.self_LeftDownVector2d = leftDown;
//                this.self_LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height);
//                this.self_RightUpVector2d = new Vector2d(leftDown.x + wight, leftDown.y + height);
//                this.self_RightDownVector2d = new Vector2d(leftDown.x + wight, leftDown.y);
//                this.self_MiddleVector2d = new Vector2d(leftDown.x + wight / 2, leftDown.y + height / 2);
//            }
//            else if (isHasCloumn)//有柱距
//            {
//                this.self_LeftDownVector2d = leftDown;
//                this.self_LeftUpVector2d = new Vector2d(leftDown.x, leftDown.y + height);
//                this.self_RightUpVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance, leftDown.y + height);
//                this.self_RightDownVector2d = new Vector2d(leftDown.x + wight + columnWidth + columnBurfferDistance, leftDown.y);
//                this.self_MiddleVector2d = new Vector2d(leftDown.x + wight / 2 + columnWidth + columnBurfferDistance, leftDown.y + height / 2);
//            }

//            this.self_Path_Vector2d = new Path_Vector2d() { this.self_LeftDownVector2d, this.self_LeftUpVector2d, this.self_RightUpVector2d, this.self_RightDownVector2d };
//            this.self_Polygon2d = new Polygon2d(this.self_Path_Vector2d);

//            this.self_Path_Vector2d_Road = new Path_Vector2d() { this.self_LeftDownVector2d, this.self_LeftUpVector2d + new Vector2d(0,InputParameter.pSHeight), this.self_RightUpVector2d + new Vector2d(0, InputParameter.pSHeight), this.self_RightDownVector2d };
//            this.self_Polygon2d_Road = new Polygon2d(this.self_Path_Vector2d_Road);
//        }

//    }
//}
