using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Revit.DirectContext3D
{
    public class PointInfo
    {
        public XYZ Vertex;
        public ColorWithTransparency Color;
        public XYZ Offset;

        public PointInfo(XYZ _vertex, ColorWithTransparency _color, XYZ _offset)
        {
            this.Vertex = _vertex;
            this.Color = _color;
            this.Offset = _offset;
        }
    }
}
