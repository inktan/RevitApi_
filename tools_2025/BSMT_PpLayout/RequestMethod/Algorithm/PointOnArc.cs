using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace PLAN_DSGN_TOOLS
{
    internal class PointOnArc
    {
        internal ArcDivideInfo ParentArc;
        internal XYZ Pos;

        internal PointOnArc(ArcDivideInfo _parent, XYZ _pos)
        {
            this.ParentArc = _parent;
            this.Pos = _pos;
        }
    }
}
