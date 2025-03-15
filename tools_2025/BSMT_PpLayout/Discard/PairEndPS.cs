//using g3;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.DB;
//using PubFuncWt;

//namespace BSMT_PpLayout
//{
//    class PairEndPS
//    {
//        /// <summary>
//        /// 端头停车位01 默认为最左 否则为最下
//        /// </summary>
//        internal RevitElePS ParkingUnitExit01;
//        /// <summary>
//        /// 端头停车位02 右上角
//        /// </summary>
//        internal RevitElePS ParkingUnitExit02;
//        /// <summary>
//        /// 该排车的所有数据
//        /// </summary>
//        internal List<RevitElePS> RowParkingUnitExits;
//        /// <summary>
//        /// 是否为尽端式
//        /// </summary>
//        internal CarType NowRowCarEndType;
//        /// <summary>
//        /// 一排车位的方向 需要归一化
//        /// </summary>
//        internal Vector2d Direction;
//        /// <summary>
//        /// 这排车的长度
//        /// </summary>
//        internal double Length;

//        internal PairEndPS(RevitElePS parkingUnitExit01, RevitElePS parkingUnitExit02)
//        {
//            VectorNormalized(parkingUnitExit01, parkingUnitExit02);

//            GetRowLength();
//        }
//        /// <summary>
//        /// 求一排车的方向
//        /// </summary>
//        /// <param name="parkingUnitExit01"></param>
//        /// <param name="parkingUnitExit02"></param>
//        internal void VectorNormalized(RevitElePS parkingUnitExit01, RevitElePS parkingUnitExit02)
//        {
//            List<RevitElePS> parkingUnitExits = new List<RevitElePS>() { parkingUnitExit01 , parkingUnitExit02 };
//            this.Direction = parkingUnitExit01.LocVector2d - parkingUnitExit02.LocVector2d;
//            if (this.Direction.x == 0)
//            {
//                parkingUnitExits = parkingUnitExits.OrderBy(p => p.LocVector2d.y).ToList();// 次轮 y值
//            }
//            else
//            {
//                parkingUnitExits = parkingUnitExits.OrderBy(p => p.LocVector2d.x).ToList();// 首轮 x值
//            }
//            this.ParkingUnitExit01 = parkingUnitExits.First();
//            this.ParkingUnitExit02 = parkingUnitExits.Last();

//            this.Direction = this.ParkingUnitExit02.LocVector2d - this.ParkingUnitExit01.LocVector2d;
//            this.Direction = this.Direction.Normalized;
//        }
//        /// <summary>
//        /// 求一排车的长度
//        /// </summary>
//        internal void GetRowLength()
//        {
//            this.Length = this.ParkingUnitExit01.LocVector2d.Distance(this.ParkingUnitExit02.LocVector2d) + GlobalData.pSWidth;
//        }
//        /// <summary>
//        /// 找到每一排需要回车的回车处的位置
//        /// </summary>
//        /// <returns></returns>
//        internal List<FamilyInstance> FindTarInReturnEnd()
//        {    
//            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
//            // 确定一排车的哪个端头是尽端
//            RevitElePS _parkingUnitExit = this.ParkingUnitExit01;
//            if (this.ParkingUnitExit01.RowEndType == CarType.NoEndType)
//                _parkingUnitExit = this.ParkingUnitExit01;
//            else if (this.ParkingUnitExit02.RowEndType == CarType.NoEndType)
//                _parkingUnitExit = this.ParkingUnitExit02;

//            //int segments = Convert.ToInt32(this.Length / InputParameter.returnLengthEnd);// 该方法直接舍去小数
//            for (int i = 1; i <= 1; i++)
//            {

//                foreach (RevitElePS parkingUnitExit in this.RowParkingUnitExits)
//                {
//                    bool isContinue = false;
//                    double distance = _parkingUnitExit.LocVector2d.Distance(parkingUnitExit.LocVector2d);
//                    if (distance >= GlobalData.returnLengthEnd * i - GlobalData.pSWidth - Precision_.Precison
//                        && distance < GlobalData.returnLengthEnd * i - Precision_.Precison)
//                    {
//                        familyInstances.Add(parkingUnitExit.FamilyInstance);
//                        isContinue = true;
//                        break;
//                    }
//                    if (isContinue)
//                        break;
//                }
//            }
//            return familyInstances;
//        }
//        /// <summary>
//        /// 环道回车 elementId提醒
//        /// </summary>
//        /// <returns></returns>
//        internal List<FamilyInstance> FindTarInLoopReturn()
//        {
//            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
//            // 确定一排车的哪个端头是尽端

//            RevitElePS _parkingUnitExit = this.ParkingUnitExit01;
//            if (this.ParkingUnitExit01.RowEndType == CarType.NoEndType)
//                _parkingUnitExit = this.ParkingUnitExit01;
//            else if (this.ParkingUnitExit02.RowEndType == CarType.NoEndType)
//                _parkingUnitExit = this.ParkingUnitExit02;

//            int segments = Convert.ToInt32(this.Length / GlobalData.loopReturnLength);// 该方法直接舍去小数
//            for (int i = 1; i <= segments; i++)
//            {
//                foreach (RevitElePS parkingUnitExit in this.RowParkingUnitExits)
//                {
//                    double distance = _parkingUnitExit.LocVector2d.Distance(parkingUnitExit.LocVector2d);
//                    if (distance >= GlobalData.returnLengthEnd * i - GlobalData.pSWidth - Precision_.Precison
//                        && distance < GlobalData.returnLengthEnd * i - Precision_.Precison)
//                    {
//                        familyInstances.Add(parkingUnitExit.FamilyInstance);
//                        break;
//                    }
//                }
//            }
//            return familyInstances;
//        }
//    }
//}
