using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using goa.Common;

namespace MountSiteDesignAnaly
{
    /// <summary>
    /// 建设场地内的挡土墙分析，一般情况是底板高度差造成的情况
    /// </summary>
    class AdjacentFloor
    {
        public SingleSubPlotEarthwork singleSubPlotEarthwork01 { get; set; }
        public SingleSubPlotEarthwork singleSubPlotEarthwork02 { get; set; }
        public List<Segment2d> OverlapSegs { get; set; }
        internal double EleDifference { get; set; }
        internal double Length { get; set; }
        internal List<TriangleInfo> triangleInfos { get; set; }
        public AdjacentFloor(SingleSubPlotEarthwork _singleSubPlotEarthwork01, SingleSubPlotEarthwork _singleSubPlotEarthwork02)
        {
            singleSubPlotEarthwork01 = _singleSubPlotEarthwork01;
            singleSubPlotEarthwork02 = _singleSubPlotEarthwork02;

            EleDifference = singleSubPlotEarthwork01.poly2DSampling.elevation - singleSubPlotEarthwork02.poly2DSampling.elevation;
            EleDifference = Math.Abs(EleDifference);
        }
        /// <summary>
        /// 获取楼板的重叠边界
        /// </summary>
        internal void CalOverlaoBoundary()
        {
            Polygon2d poly01 = singleSubPlotEarthwork01.poly2DSampling.polygon2d;
            Polygon2d poly02 = singleSubPlotEarthwork02.poly2DSampling.polygon2d;
            OverlapSegs = poly01.OverlapSegs_ByDis(poly02).ToList();
            Length = 0.0;
            this.triangleInfos = new List<TriangleInfo>();
            foreach (var item in OverlapSegs)
            {
                Length += item.Length;
                // 将segment2d转化为三角面

                Vector2d p0 = item.P0;
                Vector2d dir = item.Direction;

                List<Vector2d> vs = new List<Vector2d>() { p0};
                double count = item.Length / ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet();
                for (int i = 1; i < count; i++)
                {
                    vs.Add(p0+dir*i* ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet());
                }
                foreach (var p in vs)
                {
                    List<Triangle3d> triangle3ds = Polygon2d.MakeRectangle(p, ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet(), ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()).Triangle3dsByRectangle().ToList();
                    foreach (var tri in triangle3ds)
                    {
                        TriangleInfo triangleInfo = new TriangleInfo(tri);
                        triangleInfo.Retain_Wall_Elecation_Difference = EleDifference;
                        this.triangleInfos.Add(triangleInfo);
                    }
                }
            }
        }
    }
}


