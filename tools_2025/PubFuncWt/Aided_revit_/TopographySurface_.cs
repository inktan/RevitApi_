using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class TopographySurface_
    {

        public static Mesh ToMesh(this TopographySurface topographySurface)
        {
            Options options = new Options();

            GeometryElement geometryElement = topographySurface.get_Geometry(options);
            foreach (var item in geometryElement)
            {
                if (item is Mesh)
                {
                    return item as Mesh;
                }
            }

            return null;
        }
    }
}
