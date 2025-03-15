using Autodesk.Revit.DB;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountSiteDesignAnaly
{
    internal class RevitEleInfo
    {
        internal string comments => this.Element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();

        internal Element Element;
        //internal List<Curve> curves => this.Element.SketchLines();

        internal List<Curve> curves => this.planarFace!=null ? this.planarFace.GetEdgeCurves() :new List<Curve>();

        internal PlanarFace planarFace => this.Element.TopPlanarFace();

        // 对于非平整楼板，要做进一步细分处理
        internal Polygon2d Poly { get; set; }
        public RevitEleInfo(Element element)
        {
            this.Element = element;

            List<Vector2d> result = new List<Vector2d>();
            foreach (var item in this.curves)
            {
                if (item is Line)
                {
                    result.Add(item.GetEndPoint(0).ToVector2d());
                }
                else if (item is Arc)
                {
                    List<XYZ> xYZs = item.Tessellate().ToList();
                    if (xYZs.Count>1)
                    {
                        //xYZs.RemoveAt(xYZs.Count-1);
                        result.AddRange(xYZs.Select(p => p.ToVector2d()));
                    }
                }
            }
            this.Poly = new Polygon2d(result);
        }
    }
}
