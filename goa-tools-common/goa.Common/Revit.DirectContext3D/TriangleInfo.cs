using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;

namespace goa.Revit.DirectContext3D
{
    public class TriangleInfo
    {
        public TriangleInfo(Triangle3d triangle, XYZ normal, ColorWithTransparency color, XYZ _offset)
        {
            this.Triangle = triangle;
            Normal = normal;
            ColorWithTransparency = color;
            this.Offset = _offset;
        }

        public Triangle3d Triangle;
        public XYZ Normal;
        public ColorWithTransparency ColorWithTransparency;
        public XYZ Offset;
    }
}
