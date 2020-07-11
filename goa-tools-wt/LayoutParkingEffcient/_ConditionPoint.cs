using System;
using System.Collections.Generic;
using Form_ = System.Windows.Forms;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.IFC;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LayoutParkingEffcient
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Path_xyz = List<XYZ>;
    using Paths_xyz = List<List<XYZ>>;

    class ConditionPoint
    {
        public LineStyleId xyzStyleId { get; set; }
        public XYZ xyz { get; set; }

        public ConditionPoint(XYZ _xyz, LineStyleId _xyzStyleId)
        {
            xyz = _xyz;
            xyzStyleId = _xyzStyleId;
        }
    }
}
