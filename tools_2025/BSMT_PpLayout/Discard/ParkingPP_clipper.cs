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
//    internal class ParkingPP_clipper 
//    {
//        #region information
//        internal ParkingPP_base Clipper { get; }//该数据用于图形切割
//        #endregion

//        internal ParkingPP_clipper(ParkingPP_robot_withRoad parkingPP_Robot )
//        {
//            this.Clipper = (ParkingPP_base)parkingPP_Robot.Robot.Clone();
//            ZoomBorder();
//        }

//        void ZoomBorder()
//        {
//            this.Clipper.LUpoint -= new Vector2d(10,0);
//            this.Clipper.LDpoint -= new Vector2d(10,00);
//            this.Clipper.RDpoint -= new Vector2d(0,10);

//            this.Clipper.UpdatePolygon2d();
//        }


//    }
//}
