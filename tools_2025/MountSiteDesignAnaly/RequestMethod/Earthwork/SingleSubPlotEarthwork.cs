using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using Octree;
using PubFuncWt;
using QuadTrees;

namespace MountSiteDesignAnaly
{

    class SingleSubPlotEarthwork
    {
        internal BoundsOctree<Tri3dBounds> tri3DOctree;
        internal QuadTreeRectF<Tri3dBounds> qtree;
        internal Poly2dSampling poly2DSampling;

        internal string plotNum { get; set; }
        internal int subNum { get; set; }
        internal int Id { get; set; }

        internal EaTextIfo eaTextInfo
        {
            get
            {
                string _eaInfo = "挖方量" + this.volumeDig.ToString();
                _eaInfo += "\n";
                _eaInfo += "填方量" + this.volumeFill.ToString();
                _eaInfo += "\n";
                _eaInfo += "总土方量为" + this.volume.ToString();

                EaTextIfo eaTextIfo = new EaTextIfo();
                eaTextIfo.EaInfo = _eaInfo;
                eaTextIfo.Position = this.poly2DSampling.center;
                eaTextIfo.Id = this.Id;
                eaTextIfo.SerialNumber = this.subNum.ToString();

                return eaTextIfo;
            }
        }

        internal double volume { get { return volumeDig + volumeFill; } }
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

        internal List<SingleCellEarthwork> singleCellEarthworks;

        internal SingleSubPlotEarthwork(BoundsOctree<Tri3dBounds> _tri3DOctree, QuadTreeRectF<Tri3dBounds> _qtree, Poly2dSampling _poly2DSampling)
        {
            this.tri3DOctree = _tri3DOctree;
            this.qtree = _qtree;
            this.poly2DSampling = _poly2DSampling;
        }
        internal int maxSize;
        internal void SplitSelf()
        {
            this.singleCellEarthworks = ToSingleCellEarthworks().ToList();
        }
        /// <summary>
        /// 挡土墙采样的数据分割处理
        /// </summary>
        internal void SplitSelf_RetainWall(List<Polygon2d> polygon2ds)
        {
            this.singleCellEarthworks = ToSingleCellEarthworks_RetainWall(polygon2ds).ToList();
        }
        internal void SplitSelf_RetainWall_Site(List<Polygon2d> polygon2ds)
        {
            this.singleCellEarthworks = ToSingleCellEarthworks_RetainWall_Site(polygon2ds).ToList();
        }
        /// <summary>
        /// 获取可用于开启多线程的子细胞
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SingleCellEarthwork> ToSingleCellEarthworks()
        {
            // 将采样点分成maxSize的倍数，然后开启多线程工作
            foreach (var item in this.poly2DSampling.SplitSelf(this.maxSize))
            {
                yield return new SingleCellEarthwork(this.tri3DOctree, this.qtree, this.poly2DSampling, item);
            }
        }

        internal IEnumerable<SingleCellEarthwork> ToSingleCellEarthworks_RetainWall(List<Polygon2d> polygon2ds)
        {
            // 将采样点分成maxSize的倍数，然后开启多线程工作
            foreach (var item in this.poly2DSampling.SplitSelf_RetainWall(this.maxSize, polygon2ds))
            {
                yield return new SingleCellEarthwork(this.tri3DOctree, this.qtree, this.poly2DSampling, item);
            }
        }
        internal IEnumerable<SingleCellEarthwork> ToSingleCellEarthworks_RetainWall_Site(List<Polygon2d> polygon2ds)
        {
            // 将采样点分成maxSize的倍数，然后开启多线程工作
            foreach (var item in this.poly2DSampling.SplitSelf_RetainWall_Site(this.maxSize, polygon2ds))
            {
                yield return new SingleCellEarthwork(this.tri3DOctree, this.qtree, this.poly2DSampling, item);
            }
        }

        internal void EarthworkSum()
        {
            this.earthworkDigSegment3ds = new List<Segment3d>();
            this.earthworkFillSegment3ds = new List<Segment3d>();

            this.triangleInfos = new List<TriangleInfo>();

            foreach (var item in this.singleCellEarthworks)
            {
                this.volumeDig += item.volumeDig;
                this.volumeFill += item.volumeFill;

                if (ViewModel.Instance.ShowSamplingTris)
                {
                    this.triangleInfos.AddRange(item.triangleInfos);
                }

                if (ViewModel.Instance.ShowSamplingLine)
                {
                    this.earthworkDigSegment3ds.AddRange(item.earthworkDigSegment3ds);
                    this.earthworkFillSegment3ds.AddRange(item.earthworkFillSegment3ds);
                }
            }
        }
    }

}
