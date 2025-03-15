using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using goa.Common;
using ClipperLib;
using Autodesk.Revit.DB;
using PubFuncWt;
using System.Diagnostics;

namespace BSMT_PpLayout
{
    /*
     * 
     * 自动换 1种
     * 
     * 普通+  _GerV 
     * 
     */
    /// <summary>
    /// 自动判定放置什么车位 针对一排车位进行处理
    /// </summary>
    class PsArrCutClipLinearAuto_GerV_Mechan : PsCutClipPattern
    {
        /// <summary>
        /// 自动判定放置什么车位 针对一排车位进行处理
        /// </summary>
        internal PsArrCutClipLinearAuto_GerV_Mechan()
        { }

        internal override List<PPLocPoint> Sweep(Polygon2d polygon2d, Polygon2d bsmtPolygon2d, List<Polygon2d> obstacleOs)
        {
            // 对polygon进行封装
            Polygon2d outwardO = polygon2d.OutwardOffeet(Precision_.TheShortestDistance * 10);
            bsmtPolygon2d = bsmtPolygon2d.OutwardOffeet(Precision_.TheShortestDistance * 10);
            List<BoxMethod> obstacleBoxes = obstacleOs.Select(p => new BoxMethod(p)).ToList();
            BoxMethod main = new BoxMethod(outwardO);

            int parkingCount = 0;// 当前为第几个车位
            Vector2d lDpoint = outwardO.LDpOfBox2d();// 首个车位的起始点
            Vector2d rDpoint = outwardO.RDpOfBox2d();// 首个车位的起始点

            ParkingPP_robot parkingPP_robot;//车位机器人
            ParkingPP_base parkingPP;
            bool collisionObstalO;// 停车位机器人检测障碍物
            bool isIn;// 当前车位是否全部落在可停车区域内

            List<PPLocPoint> tarPPLocPoints = new List<PPLocPoint>();// 输出数据-停车位位置点集
            int i = 0;
            bool addColumn = true;
            while (i < 1e4)// 无限循环，子区域裁剪结束，会自动跳出循环
            {
                bool MeetCollisionConditions = true;// 是否满足停车条件，每次都需要将其重置

                parkingPP = new ParkingPP_base(lDpoint, GlobalData.Instance.pSWidth_num, GlobalData.Instance.pSHeight_num);// 基础
                parkingPP_robot = new ParkingPP_robot(parkingPP, parkingCount, GlobalData.Instance.ColumnWidth_num, GlobalData.Instance.ColumnBurfferDistance_num, addColumn); ;// 普通
                collisionObstalO = parkingPP_robot.Collision(obstacleBoxes);// 停车位机器人检测障碍物
                isIn = parkingPP_robot.IsInPolygon(outwardO, bsmtPolygon2d);// 当前车位是否全部落在可停车区域内

                if (collisionObstalO || !isIn)  // 普通停车位不适合 - 与障碍物碰撞，或者不落在停车区域内
                {
                    MeetCollisionConditions = false; // 三种情况都与障碍物碰撞
                }

                if (MeetCollisionConditions)// 符合停车要求
                {
                    //PPLocPoint tarPPLocPoint = new PPLocPoint(parkingPP_robot.Robot.PlacePoint, new Vector2d(0,1), HeadOfCarFacing.South, nowTarPsTypeName, Width, Height);//停车位的实际放置点 // 车头在南侧elementId -122543112

                    // 停车位类型为机械车位
                    PPLocPoint tarPPLocPoint = new PPLocPoint(parkingPP_robot.Robot.PlacePoint, this.MechanPsName, GlobalData.Instance.pSWidth_num, GlobalData.Instance.pSHeight_num);//停车位的实际放置点 // 车头在南侧elementId -122543112
                    tarPPLocPoint.Vector2d -= new Vector2d(GlobalData.Instance.pSWidth_num / 3, 0);

                    tarPPLocPoint.Direction = new Vector2d(0, 1);
                    tarPPLocPoints.Add(tarPPLocPoint);

                    lDpoint = parkingPP_robot.Robot.RDpoint;// 停车位定位点更新 
                    //循环更新条件
                    addColumn = true;
                    parkingCount++;
                    parkingCount = 0;// 该处注意：机械车位为组合体模式，每个组合之间放一颗柱子
                }
                else
                {
                    lDpoint += new Vector2d(GlobalData.Instance.pSHeight_Hor_num - 50.0.MilliMeterToFeet(), 0); // 停车位定位点-移动-更新 
                    parkingCount = 0;
                    addColumn = false;
                }

                if (lDpoint.x - rDpoint.x >= 0.0) break;// 以x坐标值为基准，当起点超过当前计算区域的最右值时，则跳出循环

                i++;
            }

            return tarPPLocPoints;
        }

        /// <summary>
        /// 判断车位的左下定位点
        /// </summary>
        internal Vector2d AddWifhtAndColumnParkingPoint(Vector2d _leftDownVector2d, double wight, double columnWidth, double columnBufferDistance, int parkingCount)
        {
            if (parkingCount > 0)
            {
                return _leftDownVector2d + new Vector2d(wight * parkingCount, 0) + new Vector2d((columnWidth + columnBufferDistance) * ((parkingCount - 1) / GlobalData.Instance.NumOfIntervalPP_num + 1), 0);
            }
            else
            {
                return _leftDownVector2d;
            }
        }
    }
}
