using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace PubFuncWt
{
    public static class SimpleMesh_
    {

        public static IEnumerable<Triangle3d> Triangle3ds(this SimpleMesh simpleMesh)
        {
            int triangleCount = simpleMesh.TriangleCount;
            for (int i = 0; i < triangleCount; i++)
            {
                yield return simpleMesh.Triangle(i);
            }
        }

        public static Triangle3d Triangle(this SimpleMesh simpleMesh, int i)
        {
            Index3i index3I = simpleMesh.GetTriangle(i);
            return new Triangle3d(simpleMesh.GetVertex(index3I.a), simpleMesh.GetVertex(index3I.b), simpleMesh.GetVertex(index3I.c));
        }
    }
}
