using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 详图区域
    /// 抽象为闭合线圈
    /// </summary>
    class RevitEleFR : RevitEleCtrl
    {

        internal FilledRegion FilledRegion { get; }
        internal RevitEleFR(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        {
            this.FilledRegion = ele as FilledRegion;
        }
        internal RevitEleFR()
        { }
        internal IList<CurveLoop> CurveLoops()
        {
            return this.FilledRegion.GetBoundaries();
        }
        internal List<BoundO> BoundOs()
        {
            return boundOs().ToList();
        }
        private IEnumerable<BoundO> boundOs()
        {
            foreach (var item in this.CurveLoops())
            {
                List<Curve> curves = item.ToCurves().ToList();
                Polygon2d polygon2d = curves.ToPolygon2d();
                yield return new BoundO(polygon2d, this.EleProperty);
            }
        }
        /// <summary>
        /// 填充区域类型
        /// </summary>
        internal string TypeName
        {
            get
            {
                return this.Doc.GetElement(this.FilledRegion.GetTypeId()).Name;
            }
        }

        internal double Area()
        {
            double area = 0;
            this.BoundOs().ForEach(p => area += p.Area);
            return area;
        }


    }
}
