using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClipperLib;
using g3;
using PubFuncWt;

namespace BSMT_PpLayout
{
    class BoundO
    {
        internal Polygon2d polygon2d { get;  }
        internal EleProperty EleProperty { get; }
        internal List<BoundSeg> boundSegs
        { 
            get
            {
                List<BoundSeg> boundSegs = new List<BoundSeg>();
                foreach (var item in polygon2d.SegmentItr())
                    boundSegs.Add(new BoundSeg(item, this.EleProperty));
                return boundSegs;
            }
        }

        internal BoundO(Polygon2d polygon2D, EleProperty _revitEleProperty)
        {
            this.polygon2d = polygon2D;
            this.EleProperty = _revitEleProperty;
        }
        internal double Area => this.polygon2d.Area;

        internal double Length => this.polygon2d.ArcLength;
       
    }
}
