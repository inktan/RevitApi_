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
     * 自动换3种
     * 
     * 普通+微型+平行式停车 _GerV_MiNiV_GerH
     * 
     */
    /// <summary>
    /// 自动判定放置什么车位 针对一排车位进行处理
    /// </summary>
    class PsArrCutClipLinearAuto_GerV_MiNiV_GerH : PsCutClipPattern
    {
        /// <summary>
        /// 自动判定放置什么车位 针对一排车位进行处理
        /// </summary>
        internal PsArrCutClipLinearAuto_GerV_MiNiV_GerH()
        { }

        internal override List<PPLocPoint> Sweep(Polygon2d currentArea, Polygon2d bsmtPoly2d, List<Polygon2d> obstacleOs)
        {
            //obstacleOs.ForEach(p =>
            //{
            //    CMD.Doc.CreateDirectShapeWithNewTransaction(p.ToCurveLoop());
            //});

            // 对polygon进行封装
            Polygon2d outwardO = currentArea.OutwardOffeet(Precision_.TheShortestDistance * 10);
            List<BoxMethod> obstacleBoxes = obstacleOs.Select(p => new BoxMethod(p)).ToList();
            //BoxMethod main = new BoxMethod(outwardO);

            int parkingCount = 0;// 当前为第几个车位
            Vector2d lDpoint = currentArea.LDpOfBox2d();// 首个车位的起始点
            Vector2d rDpoint = currentArea.RDpOfBox2d();// 首个车位的起始点

            string preTarPsTypeName = PsTypeName;
            string nowTarPsTypeName = PsTypeName;

            List<PPLocPoint> tarPPLocPoints = new List<PPLocPoint>();// 输出数据-停车位位置点集
            int i = 0;


            while (i < 1e4)// 无限循环，子区域裁剪结束，会自动跳出循环
            {
                nowTarPsTypeName = PsTypeName;// 记录当前停车位类型

                CalPsTypeCanPlaced calPsTypeCanPlaced =
                    new CalPsTypeCanPlaced(
                        this.PsTypeName,
                        GlobalData.Instance.pSWidth_num,
                        GlobalData.Instance.pSHeight_num,
                        lDpoint,
                        parkingCount,
                        outwardO,
                        bsmtPoly2d,
                        obstacleBoxes);

                calPsTypeCanPlaced.NextLoop(true);

                if (!calPsTypeCanPlaced.canPlaced && GlobalData.Instance.IsItMiNi)
                {
                    calPsTypeCanPlaced.psTypeName = this.MiniPsTypeName;
                    calPsTypeCanPlaced.width = GlobalData.Instance.miniPSWidth_num;
                    calPsTypeCanPlaced.height = GlobalData.Instance.miniPSHeight_num;
                    calPsTypeCanPlaced.lDpoint = calPsTypeCanPlaced.recordLdpoint;

                    calPsTypeCanPlaced.NextLoop(true);
                }
                //平行车位不需要循环测试，只需要执行一次
                if (!calPsTypeCanPlaced.canPlaced && GlobalData.Instance.IsItHor)
                {
                    calPsTypeCanPlaced.psTypeName = this.HorPsTypeName;
                    calPsTypeCanPlaced.width = GlobalData.Instance.pSHeight_Hor_num;
                    calPsTypeCanPlaced.height = GlobalData.Instance.pSWidth_Hor_num;
                    calPsTypeCanPlaced.lDpoint = calPsTypeCanPlaced.recordLdpoint;

                    calPsTypeCanPlaced.NextLoop(false);
                }

                // 更新停车位类型名称
                nowTarPsTypeName = calPsTypeCanPlaced.psTypeName;

                if (calPsTypeCanPlaced.canPlaced)// 符合停车要求
                {
                    PPLocPoint tarPPLocPoint;
                    Vector2d moveCol = new Vector2d(GlobalData.Instance.ColumnWidth_num + GlobalData.Instance.ColumnBurfferDistance_num, 0);

                    if (nowTarPsTypeName == this.HorPsTypeName && preTarPsTypeName != nowTarPsTypeName)
                    {
                        tarPPLocPoint = new PPLocPoint(calPsTypeCanPlaced.parkingPP_robot.Robot.PlacePoint, nowTarPsTypeName, GlobalData.Instance.pSWidth_Hor_num, GlobalData.Instance.pSHeight_Hor_num);//停车位的实际放置点 // 车头在南侧elementId -122543112

                        lDpoint = calPsTypeCanPlaced.parkingPP_robot.Robot.RDpoint + moveCol;// 停车位定位点更新
                    }
                    else
                    {
                        tarPPLocPoint = new PPLocPoint(calPsTypeCanPlaced.parkingPP_robot.Robot.PlacePoint, nowTarPsTypeName, GlobalData.Instance.pSWidth_Hor_num, GlobalData.Instance.pSHeight_Hor_num);//停车位的实际放置点 // 车头在南侧elementId -122543112

                        lDpoint = calPsTypeCanPlaced.parkingPP_robot.Robot.RDpoint;// 停车位定位点更新 
                    }

                    // 判断车头方向
                    if (nowTarPsTypeName == this.HorPsTypeName)
                    {
                        tarPPLocPoint.Direction = new Vector2d(1, 0);
                        tarPPLocPoint.Vector2d += new Vector2d(-GlobalData.Instance.pSHeight_Hor_num / 2, GlobalData.Instance.pSWidth_Hor_num / 2);// 调整平行停车的位置
                    }
                    else
                    {
                        tarPPLocPoint.Direction = new Vector2d(0, 1);
                    }

                    tarPPLocPoints.Add(tarPPLocPoint);

                    parkingCount++;
                    if (parkingCount % 3 == 0) parkingCount = 0;// 该计数，决定了是否放置柱子空间
                }
                else
                {
                    lDpoint += new Vector2d(500.0.MilliMeterToFeet(), 0); // 停车位定位点-移动1000mm-更新 
                    parkingCount = 0;
                }

                preTarPsTypeName = nowTarPsTypeName;// 车位类型及时更新
                if (rDpoint.x - lDpoint.x < GlobalData.Instance.pSWidth_num + GlobalData.Instance.ColumnWidth_num + GlobalData.Instance.ColumnBurfferDistance_num) break;// 以x坐标值为基准，当起点超过当前计算区域的最右值的距离为一个柱宽时，则跳出循环

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
