using Autodesk.Revit.DB;
using g3;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using goa.Common;

namespace MountSiteDesignAnaly
{
    /// <summary>
    /// 绿化信息
    /// </summary>
    class SingleGreenInfor
    {
        internal string Name => this.coverFloor.Element.Name;
        internal int Id { get; set; }
        internal FloorInfo coverFloor { get; set; }
        internal FloorInfo bsmtTopFloor { get; set; }
        internal FilledRegionInfo filledRegionInfo { get; set; }
        public SingleGreenInfor(FloorInfo _coverFloor, FloorInfo _bsmeTopFloor)
        {
            this.coverFloor = _coverFloor;
            this.bsmtTopFloor = _bsmeTopFloor;
        }
        internal ColorWithTransparency colorWithTransparency
        {
            get
            {
                return new ColorWithTransparency(ColorWpf.R, ColorWpf.G, ColorWpf.B, 0);
            }
        }
        internal System.Windows.Media.Color ColorWpf;
        /// <summary>
        /// 去掉退距后的有效绿化面积
        /// </summary>
        internal List<Polygon2d> EffectiveGreenPolygons
        {
            get
            {
                List<Polygon2d> polygon2ds = new List<Polygon2d>();
                if (this.bsmtTopFloor != null)
                {
                    polygon2ds = this.coverFloor.Poly.Intersection(this.bsmtTopFloor.Poly).ToList();
                }
                else
                {
                    polygon2ds = new List<Polygon2d>() { this.coverFloor.Poly };
                }
                if (this.filledRegionInfo != null)
                {
                    polygon2ds = polygon2ds.DifferenceClipper(this.filledRegionInfo.Poly).ToList();
                }
                return polygon2ds;
            }
        }

        //=>
        //this.bsmtTopFloor != null
        //?
        //this.coverFloor.Poly.Intersection(this.bsmtTopFloor.Poly).ToList()
        //:
        //new List<Polygon2d>() { this.coverFloor.Poly };

        internal double EffectiveArea
        {
            get
            {
                double area = 0.0;
                if (this.bsmtTopFloor != null)
                {


                    foreach (var item in this.coverFloor.Poly.Intersection(this.bsmtTopFloor.Poly).ToList())
                    {
                        area += item.Area;
                    }
                }
                else
                {
                    return this.coverFloor.Poly.Area;
                }
                return area;
            }
        }
        /// <summary>
        /// 折算后的绿化厚度/系数
        /// </summary>
        internal double GreenRate { get; set; }
        /// <summary>
        /// 计算出来的实际绿化厚度
        /// </summary>
        internal double CoverThickness
            =>
            this.bsmtTopFloor != null
            ?
            this.coverFloor.elevation - this.bsmtTopFloor.elevation
            :
            this.coverFloor.elevation;

        /// <summary>
        /// 计算当前覆土厚土的有效绿化系数
        /// </summary>
        /// <param name="allCoverThickness"></param>
        internal void AssignEffectiveGreenValue()
        {
            foreach (var item in ViewModel.Instance.CoverThickGreenFactors)
            {
                if (this.CoverThickness.FeetToMilliMeter() >= item.CoverThicknessBottom * 1000 && this.CoverThickness.FeetToMilliMeter() < item.CoverThicknessTop * 1000)
                {
                    if (this.Name.Contains(item.GreenProperity))// 暂时不分绿地性质，只考虑厚度
                    {

                    }

                    this.Id = item.Id;
                    this.GreenRate = item.GreenFactor;
                    return;
                }
            }
            this.GreenRate = 0.0;
        }

        Level level = (new FilteredElementCollector(CMD.Doc))
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType().OfClass(typeof(Level))
            .Cast<Level>()
            .Where(p => p.Elevation >= 0.0)
            .First();

        internal void CreatFloor(List<FloorType> greenFloorTypes)
        {
            List<CurveArray> curveArrays = this.EffectiveGreenPolygons.Where(p => p.Area > 0).Select(p => p.ToCurveLoop().ToCurveArray()).ToList();

            using (Transaction tarns = new Transaction(CMD.Doc, "创建绿化系数楼板"))
            {
                tarns.DeleteErrOrWaringTaskDialog();
                tarns.Start();
                switch (this.GreenRate)
                {
                    case 1.00:
                        {
                            curveArrays.ForEach(p => CMD.Doc.Create.NewFloor(p, greenFloorTypes[0], level, false));
                            break;
                        }
                    case 0.8:
                        {
                            curveArrays.ForEach(p => CMD.Doc.Create.NewFloor(p, greenFloorTypes[1], level, false));
                            break;
                        }
                    case 0.5:
                        {
                            curveArrays.ForEach(p => CMD.Doc.Create.NewFloor(p, greenFloorTypes[2], level, false));
                            break;
                        }
                    case 0.3:
                        {
                            curveArrays.ForEach(p => CMD.Doc.Create.NewFloor(p, greenFloorTypes[3], level, false));
                            break;
                        }
                    case 0.1:
                        {
                            curveArrays.ForEach(p => CMD.Doc.Create.NewFloor(p, greenFloorTypes[4], level, false));
                            break;
                        }
                    case 0:
                        {
                            break;
                        }
                    default:
                        break;
                }
                tarns.Commit();

            }



        }

    }
}
