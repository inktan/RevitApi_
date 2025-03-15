using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 地库外墙线 线圈
    /// </summary>
    class BsmtBound : RevitEleFR
    {
        internal string DesignName = null;
        private FilledRegion filledRegion { get; }
        internal BoundO BoundO { get; }
        internal BoundO BoundOInward { get; }
        internal BsmtBound(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        {
            this.filledRegion = ele as FilledRegion;
            this.Polygon2ds = polygon2ds();
            this.Polygon2d = this.Polygon2ds.First();

            this.Polygon2dInward = polygon2dInward();
            this.BoundO = new BoundO(this.Polygon2d, EleProperty.BsmtWall);
            this.BoundOInward = new BoundO(this.Polygon2dInward, EleProperty.BsmtWall);

            this.DesignName = this.filledRegion.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();

            this.Length = this._length;
            this.Area = this._area;
        }
        internal BsmtBound()
        { }
        internal Polygon2d Polygon2d { get; }
        internal List<Polygon2d> Polygon2ds { get; }
        private List<Polygon2d> polygon2ds()
        {
            List<Polygon2d> polygon2ds = new List<Polygon2d>();
            foreach (var item in this.filledRegion.GetBoundaries())
            {
                IEnumerable<Curve> _curves = item.ToCurves();

                List<XYZ> points = new List<XYZ>();
                foreach (var c in _curves)
                {
                    if (c is Line)
                    {
                        points.Add(c.GetEndPoint(0));
                        points.Add(c.GetEndPoint(1));
                    }
                    else if (c is Curve)
                    {
                        points.AddRange(c.Tessellate());
                    }
                }
                Polygon2d poly2d = new Polygon2d(points.ToVector2ds().DelDuplicate());
                polygon2ds.Add(poly2d);
            }

            return polygon2ds;
        }
        internal double Area { get; }
        /// <summary>
        /// 地库外墙面积，扣除内圈
        /// </summary>
        double _area
        {
            get
            {
                double area = this.Polygon2ds.First().Area;
                for (int i = 1; i < this.Polygon2ds.Count; i++)
                {
                    area -= this.Polygon2ds[i].Area;
                }
                return area;
            }
        }
        internal double Length { get; }
        /// <summary>
        /// 地库周长，包含内圈周长
        /// </summary>
        double _length
        {
            get
            {
                double length = this.Polygon2ds.First().ArcLength;
                for (int i = 1; i < this.Polygon2ds.Count; i++)
                {
                    length += this.Polygon2ds[i].ArcLength;
                }
                return length;
            }
        }

        /// <summary>
        /// 退距外墙厚度后的外墙线圈 用于停车的绝对区域
        /// </summary>
        internal Polygon2d Polygon2dInward { get; set; }
        private Polygon2d polygon2dInward()
        {
            return this.Polygon2d.InwardOffeet(GlobalData.Instance.bsmtWallThickness_num);
        }
    }
}
