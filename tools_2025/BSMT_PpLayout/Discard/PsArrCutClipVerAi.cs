//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using g3;
//using goa.Common.g3InterOp;
//using ClipperLib;
//using Autodesk.Revit.DB;
//using PubFuncWt;

//namespace BSMT_PpLayout
//{
//    using cInt = Int64;
//    using Path = List<IntPoint>;
//    using Paths = List<List<IntPoint>>;

//    /// <summary>
//    /// 阵列式停车位布局 - 垂直式
//    /// </summary>
//    class PsArrCutClipVerAi : PsCutClipPattern
//    {
//        /// <summary>
//        /// 阵列式停车位布局 - 垂直式
//        /// </summary>
//        internal PsArrCutClipVerAi( )
//        {        }
//        /// <summary>
//        /// 递归函数sweep，需要明确当前车位位于总体计算空间的第几行，以及车位相邻线的属性,由于使用的g3部分为二维坐标系，这里不再涉及投影处理
//        ///// </summary>
//        internal override List<PPLocPoint> Sweep(Polygon2d polygon2d, List<Polygon2d> obstacleOs, List<Segment2d> obstacleSegs,double rotateAngle)
//        {
//            #region  辅助条件设置
//            int sweepRowCount = 0;// sweep运行到第几行，0为第一行，数据需要连续，保证最外层整个区域的完整性
//            int parkingCount = 0;// parkingCount为该行的第几个裁剪车位，数据需要连续，保证最外层整个区域的完整性
//            // 函数迭代使用，不可以删除！！！
//            Vector2d startLDVec2d = polygon2d.LDpOfBox2d();
//            #endregion

//            // 输出数据-停车位位置点集
//            return SweepRecursion(startLDVec2d, polygon2d, obstacleOs, obstacleSegs, sweepRowCount, parkingCount, rotateAngle);
//        }

//        /// <summary>
//        /// 递归函数部分参量需要保证一致性，因此从原始线圈获取的部分初始条件，要一直传值给递归函数
//        /// </summary>
//        internal List<PPLocPoint> SweepRecursion(Vector2d startLeftDownVector2d, Polygon2d polygon2d, List<Polygon2d> obstacleOs, List<Segment2d> obstacleSegs, int sweepRowCount, int parkingCount, double rotateAngle)
//        {          
//            Polygon2d circularPolygon2d = polygon2d;//更新每次剩余线圈的空间范围
//            Vector2d moveHeidht_leftDownVector2d = startLeftDownVector2d;// 用于停车位定位点的高度数据更新，基于sweepRowCount与parkingCount判断行数与列数
//            List<PPLocPoint> tarPPLocPoints = new List<PPLocPoint>();// 输出数据-停车位位置点集

//            int i = 0;
//            while (i < 1e4)//无线循环，子区域裁剪结束，会自动跳出循环
//            {

//                #region  判断裁剪车位当前位于第几行-第几列-默认循环顺序为：由左至右-由上至下
//                Vector2d newVector2d = circularPolygon2d.LDpOfBox2d();//当前停车区域所在矩形的左下角点坐标

//                if (Math.Abs(newVector2d.y - startLeftDownVector2d.y - Height) <= Precision_.Precison)// 向上移动一行，行高为停车位高度
//                {
//                    sweepRowCount += 1;
//                    parkingCount = 0;
//                    moveHeidht_leftDownVector2d = moveHeidht_leftDownVector2d + new Vector2d(0,Height);// 处理裁剪车位Z轴高度移动问题
//                }
//                else if (Math.Abs(newVector2d.y - startLeftDownVector2d.y - Height - RoadWidth) <= Precision_.Precison)// 向上移动一行，行高为停车位高度+道路宽度
//                {
//                    sweepRowCount += 1;
//                    parkingCount = 0;
//                    moveHeidht_leftDownVector2d = moveHeidht_leftDownVector2d + new Vector2d(0, Height + RoadWidth);
//                }

//                startLeftDownVector2d = AddWifhtAndColumnParkingPoint(moveHeidht_leftDownVector2d, Width, ColumnWidth, ColumnBufferDistance, parkingCount);// 基于每一行的最左边起点，计算当前裁剪车位的位置

//                #endregion

//                #region 停车位机器人登场
//                ParkingPP_base parkingPP_Base = new ParkingPP_base(startLeftDownVector2d, Height, Width);// 基础停车位
//                ParkingPP_robot_withRoad parkingPP_robot = new ParkingPP_robot_withRoad(parkingPP_Base, sweepRowCount, RoadWidth, parkingCount, ColumnWidth, ColumnBufferDistance);// 停车位机器人 基于行列关系，判断柱子和车道的空间
//                bool collision01 = parkingPP_robot.Collision(obstacleOs);// 车位是否被障碍物线圈包含
//                bool collision02 = parkingPP_robot.Collision(obstacleSegs);// 判断车位前的道路空间是否与障碍物碰撞
//                bool isIn = parkingPP_robot.isInPolygon(circularPolygon2d);
//                if (!collision01 && !collision02 && isIn)
//                {
//                    PPLocPoint tarPPLocPoint = new PPLocPoint(parkingPP_robot.Robot.PlacePoint, HeadOfCarFacing.North, this.PsTypeName, rotateAngle);//停车位的实际放置点
//                    if (sweepRowCount % 2 == 0)//余数为零，为偶数次排布车位 车位放置的下边界为 车道边界
//                    {
//                        tarPPLocPoint.HeadOfCarFacing = HeadOfCarFacing.South;//车头在南侧elementId -122543112
//                    }
//                    else if (sweepRowCount % 2 == 1)//余数为一，为奇数次排布车位 车位放置的下边界为 障碍物-已停车位边界
//                    {
//                        tarPPLocPoint.HeadOfCarFacing = HeadOfCarFacing.North;//车头在北侧 elementId -21136
//                    }
//                    tarPPLocPoints.Add(tarPPLocPoint);
//                }
//                #endregion

//                #region 进行裁剪

//                ParkingPP_clipper parkingPP_Clipper = new ParkingPP_clipper(parkingPP_robot);
//                Paths singParkingPlace = new Paths() { parkingPP_Clipper.Clipper.Polygon2d.ToPath() };// 车位区域
//                Paths singleCanPlancedRegion = new Paths() { circularPolygon2d.Vertices.ToPath() };// 被裁剪区域
//                singleCanPlancedRegion = singleCanPlancedRegion.DifferenceClip(singParkingPlace);
//                Clipper.CleanPolygons(singleCanPlancedRegion);

//                if (singleCanPlancedRegion.Count == 1)//问题 两个相等区域相减 减到最后一个 便结束循环
//                {
//                    Polygon2d newPolygon2d = singleCanPlancedRegion.First().ToPolygon2d();//点集需要去重
//                    if (newPolygon2d.VertexCount < 3)
//                        break;
//                    circularPolygon2d = newPolygon2d;
//                }
//                else if (singleCanPlancedRegion.Count == 2)//问题 两个相等区域相减 减到最后一个 便结束循环
//                {
//                    Vector2d center_01 = singleCanPlancedRegion[0].ToPolygon2d().Center();
//                    Vector2d center_02 = singleCanPlancedRegion[1].ToPolygon2d().Center();

//                    // 取位置较高的图形
//                    Polygon2d circularPolygon2d_ = new Polygon2d();
//                    if (center_01.y <= center_02.y)
//                    {
//                        circularPolygon2d = singleCanPlancedRegion[1].ToPolygon2d();
//                        circularPolygon2d_ = singleCanPlancedRegion[0].ToPolygon2d();
//                    }
//                    else
//                    {
//                        circularPolygon2d = singleCanPlancedRegion[0].ToPolygon2d();
//                        circularPolygon2d_ = singleCanPlancedRegion[1].ToPolygon2d();
//                    }

//                    if (circularPolygon2d_.VertexCount >= 3)
//                    {
//                        // 要传递车位的定位点，该处使用递归算法
//                        List<PPLocPoint> parkingLocationPoints = SweepRecursion(moveHeidht_leftDownVector2d, circularPolygon2d_, obstacleOs, obstacleSegs, sweepRowCount, parkingCount, rotateAngle);
//                        tarPPLocPoints.AddRange(parkingLocationPoints);
//                    }

//                    if (circularPolygon2d.VertexCount < 3)
//                        break;
//                }
//                else
//                    break;
//                #endregion

//                // 判断得到区域点集所在矩形的高度，如果高度小于车位高度 则停止循环
//                double height_ = circularPolygon2d.Height();
//                if (height_ < Height - 1e-6)
//                    break;
//                //循环更新条件
//                i++;
//                parkingCount += 1; // 裁剪车位向右移动 +1
//            }

//            //InputParameter.Instance.strBackgroundMonitorDta += "while+i=" + i.ToString() + "\n";
//            return tarPPLocPoints;
//        }

//        /// <summary>
//        /// 判断车位的左下定位点
//        /// </summary>
//        internal Vector2d AddWifhtAndColumnParkingPoint(Vector2d _leftDownVector2d, double wight, double columnWidth, double columnBufferDistance, int parkingCount)
//        {
//            if (parkingCount > 0)
//            {
//                return _leftDownVector2d + new Vector2d(wight * parkingCount, 0) + new Vector2d((columnWidth + columnBufferDistance) * ((parkingCount - 1) / GlobalData.NumOfIntervalPP + 1), 0);
//            }
//            else
//            {
//                return _leftDownVector2d;
//            }
//        }
//    }
//}
