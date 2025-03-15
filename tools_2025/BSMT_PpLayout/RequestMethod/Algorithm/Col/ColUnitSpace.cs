using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using ClipperLib;

namespace BSMT_PpLayout
{
    
    class ColUnitSpace
    {
        internal Polygon2d polygon2d { get; set; }
        internal ColLocPoint ColumnLocationPoint { get; set; }
        internal ColUnitSpace(Polygon2d _polygon2d , ColLocPoint _colLocPoint)
        {
            this.polygon2d = _polygon2d;
            this.ColumnLocationPoint = _colLocPoint;
        }

    }
}
