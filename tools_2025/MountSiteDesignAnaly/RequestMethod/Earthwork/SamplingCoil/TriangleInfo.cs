using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;

namespace MountSiteDesignAnaly
{
    class TriangleInfo
    {
        /// <summary>
        /// 对应的地形三角面
        /// </summary>
        internal Triangle3d Triangle3d_terrain { get; set; }
        internal Triangle3d Triangle3d { get; set; }
        internal UInt32 r;
        internal UInt32 g;
        internal UInt32 b;
        internal UInt32 a;

        /// <summary>
        /// 大于0为挖方，小于0为填方。
        /// </summary>
        internal double EarthworkVolume;
        /// <summary>
        /// 建设场地内的挡土墙分析高度
        /// </summary>
        internal double Retain_Wall_Elecation_Difference;
        /// <summary>
        /// 土方放样线条
        /// </summary>
        internal Segment3d Segment3d;
        internal EaTextIfo EaTextIfo;
        internal TriangleInfo(Triangle3d _triangle3d)
        {
            this.Triangle3d = _triangle3d;
            this.r = 0;
            this.g = 0;
            this.b = 0;
            this.a = 0;
        }
        /// <summary>
        /// 二维平面上距离最近的挡土墙数据
        /// </summary>
        //internal TriangleInfo triangleInfo_2d { get; set; }
        /// <summary>
        /// 三维平面上距离最近的挡土墙数据
        /// </summary>
        //internal TriangleInfo triangleInfo_3d { get; set; }
    }
}
