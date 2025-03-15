using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;

using goa.Common;
using Autodesk.Revit.DB.Architecture;

namespace MountSiteDesignAnaly
{
    internal enum PlaneType
    {
        None,
        Level,
        Inclined,
        ZAxisDown,
    }
    internal class BaseFace
    {
        internal Face baseFace;
        internal PlaneType planeType;

        internal BaseFace(Face _baseFace, PlaneType _planeType)
        {
            this.baseFace = _baseFace;
            this.planeType = _planeType;
        }
    }
    /// <summary>
    /// 土方平衡的基准面提取
    /// </summary>
    class SinglePlot
    {

        internal Element element;
        internal SinglePlot(Element _element)
        {
            this.element = _element;

            string plotNumTmp = this.element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
            this.plotNum = plotNumTmp.IsNullOrEmpty() ? CMD.PlotNum.ToString() : plotNumTmp;
        }

        internal List<SingleSubPlotEarthwork> singleSubPlotEarthworks = new List<SingleSubPlotEarthwork>();

        internal string plotNum { get; set; }
        internal double area = 0;
        internal double volumeDig = 0;
        internal double volumeFill = 0;
        internal double volume = 0;
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
                eaTextIfo.Position = this.element.get_BoundingBox(this.element.Document.ActiveView).GetCentroid().ToVector3d();
                eaTextIfo.Id = this.element.Id.IntegerValue;

                eaTextIfo.SerialNumber = plotNum;

                return eaTextIfo;
            }
        }

        internal void Sum()
        {
            foreach (var item in singleSubPlotEarthworks)
            {
                area += item.poly2DSampling.polygon2d.Area;
                volumeDig += item.volumeDig;
                volumeFill += item.volumeFill;
                volume += item.volume;
            }
        }

        internal List<BaseFace> baseFaces;

        internal void SolidHeightExtraction()
        {
            if (this.element is Floor || this.element is BuildingPad)
            {
                Computer_Floor_BuildingPad();
            }
            else
            {
                ComputerGM();
            }
        }
        /// <summary>
        /// 楼板直接提取上表面，上表面可为平面，也可为斜面
        /// 楼板面需要合并
        /// </summary>
        internal void Computer_Floor_BuildingPad()
        {
            this.baseFaces = new List<BaseFace>();

            List<Solid> solids = this.element.GetAllSolids();
            foreach (var p in solids.SelectMany(p0 => p0.Faces.ToIEnumerable()))
            {
                if (p is PlanarFace)
                {
                    PlanarFace planarFace = p as PlanarFace;
                    if (planarFace.IsFaceVerticalUp())// 找到垂直向上的水平面
                    {
                        // 水平面
                        this.baseFaces.Add(new BaseFace(planarFace, PlaneType.Level));
                    }
                    else if (planarFace.IsFaceFacingUp())
                    {
                        // 倾斜面
                        this.baseFaces.Add(new BaseFace(planarFace, PlaneType.Inclined));
                    }
                    else
                    {
                        // Z轴负方向的面
                        this.baseFaces.Add(new BaseFace(planarFace, PlaneType.ZAxisDown));
                    }
                }
                else if (p is RuledFace)
                {
                    BoundingBoxUV boundingBoxUV = p.GetBoundingBox();
                    XYZ normal = p.ComputeNormal((boundingBoxUV.Min + boundingBoxUV.Max) / 2);
                    if (normal.Z > 0)// 粗略判断其为向上的柱曲面
                    {
                        this.baseFaces.Add(new BaseFace((p as RuledFace), PlaneType.Inclined));// 倾斜面处理
                    }
                }
            }
        }
        /// <summary>
        /// 常规模型考虑是否包含地下室，包含取下表面，不包含取上表面
        /// </summary>
        internal void ComputerGM()
        {
            List<Solid> solids = this.element.GetAllSolids();

            // 地下室高度限值
            double minHeight = (95000.0).MilliMeterToFeet();

            // 识别体量是否包含地下室
            List<Solid> basementVolume = solids.Where(p => p.GetHeight() < minHeight).ToList();
            List<Solid> noBasementVolume = solids.Where(p => p.GetHeight() >= minHeight).ToList();

            List<BaseFace> pFs =
                basementVolume.SelectMany(
                    p0 =>
                    p0.Faces
                    .ToIEnumerable()
                    .Where(p => p is PlanarFace)
                    .Cast<PlanarFace>()
                    .Where(p => p.IsFaceVerticalDown())// 面垂直朝下
                    .Select(p => new BaseFace(p, PlaneType.Level))
                    ).ToList();

            pFs.AddRange(
                 noBasementVolume.SelectMany(
                    p0 =>
                    p0.Faces
                    .ToIEnumerable()
                    .Where(p => p is PlanarFace)
                    .Cast<PlanarFace>()
                    .Where(p => p.IsFaceVerticalUp())// 面垂直朝上
                    .Select(p => new BaseFace(p, PlaneType.Level))
                    ).ToList()
                );

            this.baseFaces = pFs;
        }
    }
}
