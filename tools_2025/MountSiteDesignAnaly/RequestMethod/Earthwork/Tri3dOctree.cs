using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using PubFuncWt;
using goa.Common;
using Octree;
using QuadTrees.QTreeRectF;
using QuadTrees;

using System.Drawing;

namespace MountSiteDesignAnaly
{
    class Tri3dOctree
    {
        internal TopographySurface topographySurface;

        internal BoundsOctree<Tri3dBounds> boundsTree;
        internal QuadTreeRectF<Tri3dBounds> qtree;


        internal Tri3dOctree(TopographySurface _topographySurface)
        {
            this.topographySurface = _topographySurface;

            this.Computer();
        }
        internal void Computer()
        {
            Mesh mesh = topographySurface.ToMesh();

            List<MeshTriangle> meshTriangles = mesh.GetAllTriangles();
            List<Triangle3d> triangle3ds = meshTriangles.Select(p => p.ToTriangle3d()).ToList();

            // 构建八叉树
            this.boundsTree = new BoundsOctree<Tri3dBounds>(1, new Vector3(), 1, 1.25f);
            // 构建四叉树
            this.qtree = new QuadTreeRectF<Tri3dBounds>();

            triangle3ds.ForEach(p =>
            {
                Tri3dBounds tri3DBounds = new Tri3dBounds(p);
                this.boundsTree.Add(tri3DBounds, tri3DBounds.Bounds);
                this.qtree.Add(tri3DBounds);
            });


        }
    }

    class Tri3dBounds : IRectFQuadStorable
    {
        internal Bounds Bounds;
        internal AxisAlignedBox3d AxisAlignedBox3d;

        internal Triangle3d Triangle3d;

        internal Tri3dBounds(Triangle3d triangle3d)
        {
            this.Triangle3d = triangle3d;
            this.AxisAlignedBox3d = triangle3d.AxisAlignedBox3d();
            this.Bounds = this.AxisAlignedBox3d.ToBounds();

            //this._rect = ToRectangleF(this.Bounds);
        }
        private RectangleF _rect;

        public RectangleF Rect => this._rect;

        internal RectangleF ToRectangleF(Bounds boundingBoxXYZ)
        {
            Vector3 min = boundingBoxXYZ.Min;
            Vector3 max = boundingBoxXYZ.Max;
            return new RectangleF((float)min.X, (float)min.Y, (float)(max.X - min.X), (float)(max.Y - min.Y));
        }
    }
}
