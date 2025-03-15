using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using goa.Common;
using PubFuncWt;

namespace BSMT_PpLayout
{
    class CalPsTypeCanPlaced
    {
        /// <summary>
        /// 当前停车类型
        /// </summary>
        internal string psTypeName { get; set; }
        internal double width { get; set; }// 
        internal double height { get; set; }
        internal Vector2d lDpoint { get; set; }// 停车位定位点-实时移动点
        internal Vector2d recordLdpoint { get; }
        internal int parkingCount { get; set; }

        internal bool canPlaced = false;
        internal bool addColumn = false;

        Polygon2d outwardO { get; set; }// 当前停车子区域
        Polygon2d bsmtPoly2d { get; set; }// 地库边界范围
        List<BoxMethod> obstacleBoxes { get; set; }// 所有的障碍物线圈

        internal ParkingPP_robot parkingPP_robot;//车位机器人
        internal ParkingPP_base parkingPP;//车位数据

        /// <summary>
        /// 车型试错法：每前进一步，判断一次
        /// </summary>
        Vector2d step = new Vector2d(50.0.MilliMeterToFeet(), 0); // 该位置为瞧你根据试错法，每次前进的距离可以继续精细化

        internal CalPsTypeCanPlaced(string _psTypeName, double _width, double _height, Vector2d _lDpoint, int _parkingCount, Polygon2d _outwardO, Polygon2d _bsmtPoly2d, List<BoxMethod> _obstacleBoxes)
        {
            psTypeName = _psTypeName;
            width = _width;
            height = _height;
            this.lDpoint = _lDpoint;
            this.recordLdpoint = _lDpoint;

            parkingCount = _parkingCount;

            outwardO = _outwardO;
            bsmtPoly2d = _bsmtPoly2d;
            obstacleBoxes = _obstacleBoxes;
        }

        /// <summary>
        /// 在有限距离里内，判断能够放置当前车型
        /// 不能则返回true，进入下一轮试错循环
        /// </summary>
        internal void NextLoop(bool next)
        {
            int loopCount = 0;// 记录试错次数，可用于断点debug

            while (true)// 循环一个平行停车的长度，判断中间能否停一个普通停车位
            {
                // 
                lDpoint = loopCount > 0 ? lDpoint += step : lDpoint; // 停车位定位点-移动-更新，首次移动距离为0

                parkingPP = new ParkingPP_base(lDpoint, width, height);// 普通停车位

                // 添加条件，是否计算柱子距离
                if (parkingCount % 3 == 0 && loopCount == 0)
                {
                    addColumn = true;
                }

                parkingPP_robot = new ParkingPP_robot(parkingPP, parkingCount, GlobalData.Instance.ColumnWidth_num, GlobalData.Instance.ColumnBurfferDistance_num, addColumn);// 该处默认不考虑柱子空间

                if (parkingPP_robot.Collision(obstacleBoxes))// 每移动一步距离仍停不进普通停车位
                {
                    // 如果狭长空间的长度比平行式车位长，则进一步计算是否需要平行式停车
                    if (lDpoint.x - this.recordLdpoint.x > GlobalData.Instance.pSHeight_Hor_num - (GlobalData.Instance.ColumnWidth_num + GlobalData.Instance.ColumnBurfferDistance_num) * 2)// 减去双柱宽度
                    {
                        break;
                    }
                }
                else// 移动到某一步距离，停进普通停车位
                {
                    bool isIn = parkingPP_robot.IsInPolygon(outwardO, bsmtPoly2d);// 会默认给停车位的车头区域，加上一个通车道空间进行判断

                    if (!isIn)
                    {
                        // 如果狭长空间的长度比平行式车位长，则进一步计算是否需要平行式停车
                        if (lDpoint.x - this.recordLdpoint.x > GlobalData.Instance.pSHeight_Hor_num - (GlobalData.Instance.ColumnWidth_num + GlobalData.Instance.ColumnBurfferDistance_num * 2) * 2)// 减去双柱宽度
                        {
                            break;
                        }
                    }
                    else
                    {
                        this.canPlaced = true;
                        break;// 找到了当前车型可以放置的位置
                    }
                }
                if (!next)
                {
                    break;
                }
                loopCount++;// 循环条件，不可无
            }
        }
    }
}
