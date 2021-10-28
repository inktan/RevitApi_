using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Revit.DirectContext3D
{
    /// <summary>
    /// A container to hold information associated with a triangulated face.
    /// </summary>
    public class MeshInfo
    {
        public MeshInfo(Mesh mesh, XYZ normal, ColorWithTransparency color, XYZ _offset)
        {
            Mesh = mesh;
            Normal = normal;
            ColorWithTransparency = color;
            this.Offset = _offset;
        }

        public Mesh Mesh;
        public XYZ Normal;
        public ColorWithTransparency ColorWithTransparency;
        public XYZ Offset;
    }
}
