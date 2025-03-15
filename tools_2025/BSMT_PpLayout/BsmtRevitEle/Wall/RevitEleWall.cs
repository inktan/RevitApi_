using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 非障碍物性质的柱子，可由插件-点柱子-功能生成
    /// </summary>
    class RevitEleWall : RevitEleCtrl
    {
        internal RevitEleWall(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        { }
        internal BoundO BoundO => new BoundO(this.Polygon2d(), this.EleProperty);
        internal Polygon2d Polygon2d()
        {
            var opt = new Options();
            opt.View = this.View;
            opt.ComputeReferences = false;
            opt.IncludeNonVisibleObjects = false;
            GeometryElement geometryElement = (this.Ele as Wall).get_Geometry(opt);

            List<Solid> solids = new List<Solid>();

            foreach (GeometryObject geomObj in geometryElement)
            {
                if (geomObj is Solid)
                {
                    Solid geomSolid = geomObj as Solid;
                    if (geomSolid == null
                        || geomSolid.Volume.IsAlmostEqualByDifference(0))
                        continue;
                    solids.Add(geomSolid);
                }
            }

            Solid solid = solids.FirstOrDefault();
            FaceArray faceArray = solid.Faces;
            foreach (Face face in faceArray)
            {
                PlanarFace planarFace = face as PlanarFace;
                if(planarFace.FaceNormal.Z.EqualPrecision(1) || planarFace.FaceNormal.Z.EqualPrecision(-1))
                {
                    foreach (EdgeArray edgeArray in planarFace.EdgeLoops)
                    {
                        return new Polygon2d(edgeArray.ToXyzs().ToVector2ds());
                    }
                }
            }
            return new Polygon2d();
        }

    }
}
