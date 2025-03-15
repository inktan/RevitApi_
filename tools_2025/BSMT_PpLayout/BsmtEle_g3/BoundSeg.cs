using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using ClipperLib;
using PubFuncWt;
using goa.Common;

namespace BSMT_PpLayout
{
    class BoundSeg
    {
        internal EleProperty EleProperty { get; set; }
        internal Segment2d Segment2d { get; }
        internal BoundSeg(Segment2d segment2d, EleProperty _revitEleProperty)
        {
            this.Segment2d = segment2d;
            this.EleProperty = _revitEleProperty;
        }
   
    }
}
