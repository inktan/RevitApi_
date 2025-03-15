using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 路径类别：两头都是尽端，一头是尽端，非尽端路
    /// </summary>
    internal enum PathType : int
    {
        BothEnds,
        SingleEnd,
        Invalid,
        NoEnd,
        None,
    }

    internal enum RoadOnWheDiseOfTheCoil : int
    {
        Left,
        Right,
        Top,
        Bottom,
        None,
    }

    /// <summary>
    /// 一段路径
    /// </summary>
    internal class Route
    {
        #region 用于道路生成
        internal RoadOnWheDiseOfTheCoil JudgeLeftRight(Polygon2d o)
        {
            // 垂直道路的左支路与线圈碰撞，说明道路在线圈的右侧
            if (o.Intersects(this.CalVerBranch(-20.0).Select(p => p.Segment2d)))
            {
                return RoadOnWheDiseOfTheCoil.Right;
            }
            // 与一条相反，说明道路在线圈的左侧
            else if (o.Intersects(this.CalVerBranch(20.0).Select(p => p.Segment2d)))
            {
                return RoadOnWheDiseOfTheCoil.Left;
            }
            else
            {
                return RoadOnWheDiseOfTheCoil.None;
            }
        }
        internal RoadOnWheDiseOfTheCoil JudgeUpDown(Polygon2d o)
        {
            // 垂直道路的上支路与线圈碰撞，说明道路在线圈的下侧
            if (o.Intersects(this.CalHorBranch(20.0).Select(p => p.Segment2d)))
            {
                return RoadOnWheDiseOfTheCoil.Bottom;
            }
            // 与一条相反，说明道路在线圈的上侧
            else if (o.Intersects(this.CalHorBranch(-20.0).Select(p => p.Segment2d)))
            {
                return RoadOnWheDiseOfTheCoil.Bottom;
            }
            else
            {
                return RoadOnWheDiseOfTheCoil.None;
            }
        }
        /// <summary>
        /// 获取竖线的左右支路-可用于判断图形关系
        /// 正值为右支路
        /// 负值为左支路
        /// </summary>
        internal List<Route> CalVerBranch(double width = 20.0)
        {
            List<Route> routes = new List<Route>();
            //double distance = GlobalData.Instance.Wd_pri + GlobalData.Instance.pSHeight * 2;
            double distance = GlobalData.Instance.Wd_pri_num / 2;
            Vector2d bottom = this.Segment2d.EndPoints().OrderBy(p => p.y).First();// 取最下点

            //bottom += new Vector2d(0, GlobalData.Instance.Wd_pri / 2);
            double intervalDis = distance;
            while (true)
            {
                if (intervalDis >= this.Segment2d.Length)
                {
                    break;
                }
                Segment2d seg = new Segment2d(bottom, bottom + new Vector2d(width, 0));
                routes.Add(new Route(seg));

                bottom += new Vector2d(0, distance);
                intervalDis += distance;
            }
            return routes;
        }
        /// <summary>
        /// 获取横线的上下支路-可用于判断图形关系
        /// 正值为上支路
        /// 负值为下支路
        /// </summary>
        internal List<Route> CalHorBranch(double width = 20.0)
        {
            List<Route> routes = new List<Route>();
            double distance = GlobalData.Instance.Wd_pri_num + GlobalData.Instance.pSHeight_num * 2;
            Vector2d left = this.Segment2d.EndPoints().OrderBy(p => p.x).First();

            left += new Vector2d(GlobalData.Instance.Wd_pri_num / 2, 0);
            double intervalDis = GlobalData.Instance.Wd_pri_num / 2;
            while (true)
            {
                if (intervalDis >= this.Segment2d.Length)
                {
                    break;
                }
                Segment2d seg = new Segment2d(left, left + new Vector2d(0, width));
                routes.Add(new Route(seg));

                left += new Vector2d(distance, 0);
                intervalDis += distance;
            }
            return routes;
        }

        #endregion

        #region 回车查询功能
        // 如果路径为双尽端 or 非尽端，则两端点属性一致
        internal Vector2d EndPoint;// 尽端点
        internal Vector2d NoEndPoint;// 非尽端点

        internal Vector2d EndReturnLocation;// 按照尽端距离数据推算回车位置
        internal Vector2d LoopReturnLocation;// 按照85m位置推算竖向车道位置

        internal Vector2d EndCirclePosition01;// 尽端道路圆圈位置
        internal Vector2d EndCirclePosition02;// 尽端道路圆圈位置

        internal Vector2d Direction;

        internal PathType PathType;// 是否为尽端路径
        internal Segment2d Segment2d;
        internal double Length;
        #endregion

        internal Route(Segment2d _segment2d)
        {
            this.Segment2d = _segment2d;
            this.Length = _segment2d.Length;

        }

        /// <summary>
        /// 判断路径是否为尽端路径
        /// </summary>
        internal void CalEendProperty(List<Vector2d> vector2ds)
        {
            if (this.Length < Precision_.TheShortestDistance)
            {
                this.PathType = PathType.Invalid;// 无效路径
                return;
            }

            Vector2d p0 = this.Segment2d.P0;
            Vector2d p1 = this.Segment2d.P1;

            int countP0 = 0;
            int countP1 = 0;
            foreach (var item in vector2ds)// 判断一个线段的线段两端点在
            {
                if (item.DistanceSquared(p0).EqualZreo())
                {
                    countP0++;
                }
                if (item.DistanceSquared(p1).EqualZreo())
                {
                    countP1++;
                }
            }

            if (countP0 == 1 && countP1 == 1)// 两个端点皆为尽端
            {
                this.PathType = PathType.BothEnds;
                this.EndPoint = p0;
                this.NoEndPoint = p1;
            }
            else if (countP0 == 1 && countP1 > 1)// p0为尽端
            {
                this.PathType = PathType.SingleEnd;

                this.EndPoint = p0;
                this.NoEndPoint = p1;
            }
            else if (countP0 > 1 && countP1 == 1)// p1为尽端
            {
                this.PathType = PathType.SingleEnd;

                this.EndPoint = p1;
                this.NoEndPoint = p0;
            }
            else// 两端点皆不是尽端
            {
                this.PathType = PathType.NoEnd;
                this.EndPoint = p0;
                this.NoEndPoint = p1;
            }

            if (this.PathType == PathType.SingleEnd)
            {
                this.Direction = (this.EndPoint - this.NoEndPoint).Normalized;// 方向为指向尽端
            }
            else
            {
                this.Direction = this.Segment2d.Direction;// 如果为非尽端路，则统一由左指向右侧，由上指向下侧（该原则来自于上层线段获取逻辑）
            }
        }

        /// <summary>
        /// 计算尽端路线上的尽端点 基于回车数据
        /// </summary>
        internal void CalEndLocation()
        {
            if (this.PathType == PathType.SingleEnd || this.PathType == PathType.BothEnds)
            {
                double endLength = GlobalData.Instance.endReturnLength_num;
                if (this.Length < endLength)// 如果长度不满足，则不属于尽端回车性质
                {
                    this.PathType = PathType.Invalid;// 这里判定为无效
                }
                else
                {
                    //this.ReturnLocation = this.NoEndPoint + this.Direction * endLength;

                    // 说明，根据设计师经验，直接拿掉尽端一个车位，不需要基于回车距离进行推算，回车距离只是用来判断，是否为尽端路
                    this.EndReturnLocation = this.EndPoint;
                }
            }
            else if (this.PathType == PathType.NoEnd)// 非尽端路
            {
                double endLength = GlobalData.Instance.loopReturnLength_num;
                if (this.Length > endLength)// 需要环道回车
                {
                    this.LoopReturnLocation = this.EndPoint + this.Direction * endLength;
                }
            }
        }
        /// <summary>
        /// 计算尽端路线上的圆圈表达位置
        /// </summary>
        internal void CalEndCirclePosition()
        {
            if (this.PathType == PathType.SingleEnd)// 一端为尽端
            {
                if (this.Length < GlobalData.Instance.EndCirclePosition_num)
                {
                    this.PathType = PathType.None;
                }
                else
                {
                    this.EndCirclePosition01 = this.EndPoint - this.Direction * GlobalData.Instance.EndCirclePosition_num;
                }
            }
            else if (this.PathType == PathType.BothEnds)// 两点都是尽端
            {
                if (this.Length < GlobalData.Instance.EndCirclePosition_num * 2)
                {
                    this.PathType = PathType.None;
                }
                else
                {
                    this.EndCirclePosition01 = this.EndPoint - this.Direction * GlobalData.Instance.EndCirclePosition_num;
                    this.EndCirclePosition02 = this.NoEndPoint + this.Direction * GlobalData.Instance.EndCirclePosition_num;
                }
            }
        }
        internal Route Rotate(Vector2d origin, double angle)
        {
            Segment2d segment2d = this.Segment2d.Rotate(Vector2d.Zero, angle);
            return new Route(segment2d);
        }

        internal Route Mirror(Vector2d origin, Vector2d direction)
        {
            Segment2d segment2d = this.Segment2d.Mirror(origin, direction);
            return new Route(segment2d);
        }
    }
}
