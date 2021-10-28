using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Revit.DirectContext3D
{
    /// <summary>
    /// A container to hold information associated with a geometry edge.
    /// </summary>
    public class EdgeInfo
    {
        public IList<XYZ> Vertices;
        public ColorWithTransparency Color;
        public XYZ Offset;

        public EdgeInfo(IList<XYZ> _vertices, ColorWithTransparency _color, XYZ _offset)
        {
            this.Vertices = _vertices;
            this.Color = _color;
            this.Offset = _offset;
        }
    }
}
