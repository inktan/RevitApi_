using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using g3;
using ClipperLib;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 用于碰撞分析 考虑柱子与道路的关系 基于基础停车位进行下一步操作
    /// </summary>
    internal class ParkingPP_robot
    {
        #region information

        internal ParkingPP_base Base { get; }// 保留底稿
        internal ParkingPP_base Robot { get; }//机器人用来检测障碍物干扰情况 因此，机器人的空间柱网空间

        #endregion

        /// <summary>
        /// 构造函数 sweepRowCount的参数很重要
        /// </summary>
        /// <param name="_parkingPP_base">基础停车位</param>
        /// <param name="_roadWidth">通车到宽度</param>
        /// <param name="sweepRowCount">当前位第几排停车</param>
        /// <param name="columnWidth">柱宽</param>
        /// <param name="columnBurfferDistance">停车位与柱子的缓冲距离</param>
        /// <param name="parkingCount">当前第几列停车</param>
        internal ParkingPP_robot(ParkingPP_base _parkingPP_base, int parkingCount, double columnWidth, double columnBurfferDistance, bool addColumn)
        {
            this.Base = _parkingPP_base;//基于基础停车位进行复制，引用型类型的新变量指向可能为原指针，需要判断是否需要new处理，值类型的新变量默认指定新指针
            this.Robot = (ParkingPP_base)_parkingPP_base.Clone();//基于基础停车位进行复制，引用型类型的新变量指向可能为原指针，需要判断是否需要new处理，值类型的新变量默认指定新指针

            if (parkingCount % GlobalData.Instance.NumOfIntervalPP_num == 0 && addColumn)//第0列附带柱子空间，第?列附带柱子空间-由全局参数控制-……
                AddColumn(columnWidth, columnBurfferDistance);

            this.Robot.UpdatePolygon2d();
        }
        /// <summary>
        /// 添加柱子占据空间
        /// </summary>
        void AddColumn(double columnWidth, double columnBurfferDistance)
        {
            Vector2d columnMove = new Vector2d(columnWidth + columnBurfferDistance, 0);
            this.Robot.RDpoint += columnMove;
            this.Robot.RUpoint += columnMove;
            this.Robot.PlacePoint += columnMove;

        }

        internal bool Collision(IEnumerable<BoxMethod> boxMethods)
        {
            foreach (BoxMethod boxMethod in boxMethods)
            {
                // 优化方案 先判断矩形是否碰撞，如果碰撞，再进行polygon是否碰撞相碰 ==> 优化不理想，需要想其它办法
                //if (this.Robot.InwarBoxMehthod.Box2d.Intersects(boxMethod.Box2d))
                //{
                //    if (this.Robot.InwarBoxMehthod.O.Intersects(boxMethod.O)
                //        || boxMethod.O.Contains(this.Robot.InwarBoxMehthod.O)
                //        || this.Robot.InwarBoxMehthod.O.Contains(boxMethod.O))
                //    {
                //        return true;
                //    }
                //}
                if (this.Robot.InwarBoxMehthod_road.O.Intersects(boxMethod.O)
                    || boxMethod.O.Contains(this.Robot.InwarBoxMehthod_road.O)
                    || this.Robot.InwarBoxMehthod_road.O.Contains(boxMethod.O))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断基础停车位是否同时，落在子停车区域和地库边界范围内
        /// </summary>
        /// <param name="polygon2d">子停车空间</param>
        /// <param name="bsmtPoly2d">地库边界区域</param>
        /// <returns></returns>
        internal bool IsInPolygon(Polygon2d polygon2d, Polygon2d bsmtPoly2d)
        {
            Polygon2d inO = this.Robot.InwarBoxMehthod.O;
            // 优化方案 先判断是否位于矩形内是否碰撞，如果碰撞，再进行polygon是否碰撞相碰

            int count = 0;
            foreach (var item in this.Robot.InwarBoxMehthod_road.O.Vertices)// 判断车头前面加上道路后，是否还位于可停车区域内
            {
                if (bsmtPoly2d.Contains(item))
                {
                    count++;
                }
            }
            //CMD.Doc.CreateDirectShapeWithNewTransaction(polygon2d.ToCurveLoop().ToList());
            //CMD.Doc.CreateDirectShapeWithNewTransaction(this.Robot.InwarBoxMehthod_road.O.ToCurveLoop().ToList());

            return count > 2 ? polygon2d.Contains(inO) : false;
        }
    }
}
