using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using goa.Common;
using Octree;
using PubFuncWt;
using QuadTrees;

namespace MountSiteDesignAnaly
{
    class SingleCellEarthwork
    {
        internal BoundsOctree<Tri3dBounds> tri3DOctree;
        internal QuadTreeRectF<Tri3dBounds> qtree;
        internal IEnumerable<Rad3dBounds> rad3DBounds;
        internal Poly2dSampling poly2DSampling;

        internal double volumeDig;
        internal double volumeFill;

        internal List<Segment3d> earthworkDigSegment3ds;
        internal List<Segment3d> earthworkFillSegment3ds;

        internal List<Segment3d> earthworkSegment3ds
        {
            get
            {
                List<Segment3d> result = new List<Segment3d>();
                result.AddRange(this.earthworkDigSegment3ds);
                result.AddRange(this.earthworkFillSegment3ds);
                return result;
            }
        }

        internal List<TriangleInfo> triangleInfos;

        internal SingleCellEarthwork(BoundsOctree<Tri3dBounds> _tri3DOctree, QuadTreeRectF<Tri3dBounds> _qtree, Poly2dSampling _poly2DSampling, IEnumerable<Rad3dBounds> _rad3DBounds)
        {
            this.tri3DOctree = _tri3DOctree;
            this.rad3DBounds = _rad3DBounds;
            this.qtree = _qtree;

            this.poly2DSampling = _poly2DSampling;
        }
        /// <summary>
        /// 计算采样点与地形之间的高差关系
        /// </summary>
        internal void Computer()
        {
            this.triangleInfos = new List<TriangleInfo>();
            this.earthworkDigSegment3ds = new List<Segment3d>();
            this.earthworkFillSegment3ds = new List<Segment3d>();

            this.EarthworkDigOrFill(out this.volumeDig, out this.volumeFill);

        }

        void EarthworkDigOrFill(out double volumeDig, out double volumeFill)
        {
            double _volumeDigHeight = 0;
            double _volumeFillHeight = 0;

            // 分析所有的三维射线 方柱体
            foreach (var item in rad3DBounds)
            {
                List<Tri3dBounds> collidTri3d = new List<Tri3dBounds>();
                // 找到与方柱体相交的三角面
                this.tri3DOctree.GetColliding(collidTri3d, item.bounds);

                //collidTri3d = this.qtree.GetObjects(item.rectangleF);

                foreach (var tri3d in collidTri3d)// 通过八叉树找到相交的三角面
                {
                    // down为填方
                    IntrRay3Triangle3 intrRay3Triangle3 = new IntrRay3Triangle3(item.ray3dDown, tri3d.Triangle3d);
                    intrRay3Triangle3.Compute();
                    if (intrRay3Triangle3.Quantity == 1)
                    {
                        Vector3d intrPint = tri3d.Triangle3d.PointAt(intrRay3Triangle3.TriangleBaryCoords);
                        Segment3d segment3d = new Segment3d(item.origin, intrPint);

                        this.earthworkFillSegment3ds.Add(segment3d);

                        // 由于是垂线，直接使用z数据相减获取
                        // 填方为负值
                        _volumeFillHeight += ((intrPint.z - item.origin.z) * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet() * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()).NumDecimal(2);

                        // 绿为填方
                        // 一个采样网格有两个三角面
                        item.triangle3ds.ForEach(p =>
                        {
                            TriangleInfo triangleInfo = new TriangleInfo(p);
                            triangleInfo.g = 255;
                            triangleInfo.EarthworkVolume = ((intrPint.z - item.origin.z) * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet() * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()).NumDecimal(2);
                            triangleInfo.Retain_Wall_Elecation_Difference = (intrPint.z - item.origin.z).NumDecimal(2);
                            triangleInfo.Segment3d = segment3d;
                            triangleInfo.Triangle3d_terrain = tri3d.Triangle3d;

                            this.triangleInfos.Add(triangleInfo);
                        });
                        break;
                    }
                    else
                    {
                        // up为挖方
                        intrRay3Triangle3 = new IntrRay3Triangle3(item.ray3dUp, tri3d.Triangle3d);
                        intrRay3Triangle3.Compute();
                        if (intrRay3Triangle3.Quantity == 1)
                        {
                            Vector3d intrPint = tri3d.Triangle3d.PointAt(intrRay3Triangle3.TriangleBaryCoords);
                            Segment3d segment3d = new Segment3d(item.origin, intrPint);

                            this.earthworkDigSegment3ds.Add(segment3d);

                            // 由于是垂线，直接使用z数据相减获取
                            // 挖方为正值
                            _volumeDigHeight += ((intrPint.z - item.origin.z) * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet() * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()).NumDecimal(2);

                            // 红为挖方
                            item.triangle3ds.ForEach(p =>
                            {
                                TriangleInfo triangleInfo = new TriangleInfo(p);
                                triangleInfo.r = 255;
                                triangleInfo.EarthworkVolume = ((intrPint.z - item.origin.z) * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet() * ViewModel.Instance.SamplingAccuracy.MilliMeterToFeet()).NumDecimal(2);
                                triangleInfo.Retain_Wall_Elecation_Difference = (intrPint.z - item.origin.z).NumDecimal(2);
                                triangleInfo.Segment3d = segment3d;
                                triangleInfo.Triangle3d_terrain = tri3d.Triangle3d;
                                
                                this.triangleInfos.Add(triangleInfo);
                            });
                            break;
                        }
                    }
                }
            }
            // 
            volumeFill = _volumeFillHeight;
            volumeDig = _volumeDigHeight;
        }
    }
}
