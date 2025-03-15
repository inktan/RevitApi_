using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 使用g3构建一个停车位
    /// </summary>
    internal class ParkingPP_base : ICloneable
    {
        #region 四个角点，以及self polygon2d
        internal double Height { get; set; }
        internal double Width { get; set; }
        internal Vector2d LDpoint { get; set; }
        internal Vector2d LUpoint { get; set; }
        internal Vector2d RUpoint { get; set; }
        internal Vector2d RDpoint { get; set; }
        internal Vector2d PlacePoint { get; set; }// 放置点位于裁剪机器人的下边界中点
        internal Polygon2d Polygon2d { get; set; }//需要手动进行深拷贝

        // internal Polygon2d InwardPolygon2d { get; set; }//将线圈向内收缩一个精度，保证碰撞检测能够通过
        internal BoxMethod InwarBoxMehthod { get; set; }
        internal BoxMethod InwarBoxMehthod_road { get; set; }// 加上车道空间
        #endregion

        internal ParkingPP_base(Vector2d _leftDownPoint, double _wight, double _height)
        {
            this.Height = _height;
            this.Width = _wight;
            this.LDpoint = _leftDownPoint;
            this.RDpoint = new Vector2d(_leftDownPoint.x + _wight, _leftDownPoint.y);
            this.RUpoint = new Vector2d(_leftDownPoint.x + _wight, _leftDownPoint.y + _height);
            this.LUpoint = new Vector2d(_leftDownPoint.x, _leftDownPoint.y + _height);
            this.PlacePoint = new Vector2d(_leftDownPoint.x + _wight / 2, _leftDownPoint.y);

            UpdatePolygon2d();
        }
        /// <summary>
        /// 车位机器人四个角点移动后，需要生成对应的新的线圈
        /// </summary>
        internal void UpdatePolygon2d()
        {
            this.Polygon2d = new Polygon2d(new List<Vector2d>() { this.LDpoint, this.RDpoint, this.RUpoint, this.LUpoint });
            Polygon2d tmep = this.Polygon2d.InwardOffeet(Precision_.TheShortestDistance * 10);
            this.InwarBoxMehthod = new BoxMethod(tmep);

            // 这里的车道空间需注意：是矩形的下部端点，向下移动一个路宽
            Polygon2d polyRoad = new Polygon2d(new List<Vector2d>() { this.LDpoint - new Vector2d(0, GlobalData.Instance.Wd_pri_num*0.8), this.RDpoint - new Vector2d(0, GlobalData.Instance.Wd_pri_num), this.RUpoint , this.LUpoint });
            polyRoad = polyRoad.InwardOffeet(Precision_.TheShortestDistance * 10);
            this.InwarBoxMehthod_road = new BoxMethod(polyRoad);
        }
        /// <summary>
        /// 默认为浅拷贝，手动辅助深拷贝
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            ParkingPP_base parkingPP_Base = (ParkingPP_base)this.MemberwiseClone();

            //需要手动深度拷贝的内容
            parkingPP_Base.Polygon2d = new Polygon2d(this.Polygon2d);
            return parkingPP_Base;

            //throw new NotImplementedException();
        }

    }
}
