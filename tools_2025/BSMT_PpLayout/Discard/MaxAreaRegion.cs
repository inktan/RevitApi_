//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using ClipperLib;
//using CommonMethod_g3;
//using g3;

//namespace ParkingLayoutEfficientNewStructual
//{

//    using cInt = Int64;

//    using Path = List<IntPoint>;
//    using Paths = List<List<IntPoint>>;
//    using Path_Vector2d = List<Vector2d>;
//    using Paths_Vector2d = List<List<Vector2d>>;

//    /// <summary>
//    /// 用于输出进行最终计算车位的最大面积区域，该区域已被临时车位排布临时计算过，满足最优解（停车效率、与车道相通）
//    /// </summary>
//    class MaxAreaRegion
//    {
//        // 需要定义面积区域边界的属性信息

//        //internal Path_Vector2d Path_Vector2d { get; }
//        //internal Polygon2d Polygon2d { get; }
//        internal MaxAreaPolygon MaxAreaPolygon { get; }
//        /// <summary>
//        /// 与该区域相关的所有障碍物线圈
//        /// </summary>
//        internal List<BoundaryLoop> BoundaryLoops { get; set; }
//        /// <summary>
//        /// 与该区域相关的所有属性线条
//        /// </summary>
//        internal List<BoundarySegment> BoundarySegments { get; set; }
//        /// <summary>
//        /// 长度与角度的键值对 首位最小，末尾最大
//        /// </summary>
//        internal List<KeyValuePair<Segment2d, double>> LengthAnglePairs { get; }
//        /// <summary>
//        /// 当前区域多边形最长边的旋转至X轴水平方向的角度
//        /// </summary>
//        internal double LongestSideAngle { get; }

//        internal MaxAreaRegion(MaxAreaPolygon maxAreaPolygon)
//        {
//            this.MaxAreaPolygon = maxAreaPolygon;
//            //this.Path_Vector2d = maxAreaRectangle.Polygon2d.VerticesItr(false).ToList();
//            this.LengthAnglePairs = GetRegionLineLength_Angle(this.MaxAreaPolygon.Polygon2d);
//            this.LongestSideAngle = this.LengthAnglePairs.Last().Value;// 获取当前边界最长边的旋转角度
//        }

//        /// <summary>
//        /// 找到各个边与x轴风方向的角度，求出transform，由clipper计算出来的线圈默认为逆时针旋转方向
//        /// 得到排序，首位最小，末尾最大
//        /// </summary>
//        internal List<KeyValuePair<Segment2d, double>> GetRegionLineLength_Angle(Polygon2d polygon2d)
//        {
//            Dictionary<Segment2d, double> lengthAnglePairs = new Dictionary<Segment2d, double>();

//            foreach (Segment2d segment2d in polygon2d.SegmentItr())
//            {
//                double rotateAngle = segment2d.AngleBetweenHorizontal();// 弧度

//                if (!lengthAnglePairs.ContainsKey(segment2d))
//                {
//                    lengthAnglePairs.Add(segment2d, rotateAngle);
//                }
//            }
//            List<KeyValuePair<Segment2d, double>> keyValuePairs = new List<KeyValuePair<Segment2d, double>>(lengthAnglePairs);
//            keyValuePairs.Sort(delegate (KeyValuePair<Segment2d, double> s1, KeyValuePair<Segment2d, double> s2) { return s1.Key.Length.CompareTo(s2.Key.Length); });// 左为小 右为大
//            lengthAnglePairs.Clear();

//            return keyValuePairs;
//        }
//    }//
//}//
