using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common;

using g3;

namespace goa.Revit.DirectContext3D
{
    /// <summary>
    /// Inputs needed by geometry draw server. 
    /// Use AddFaceToBuffer() and AddEdgeToBuffer().
    /// </summary>
    public class GeometryDrawServerInputs
    {
        public RenderingPassBufferStorage
            NonTransFaceBuffer, TransFaceBuffer,
            NonTransTriangleBuffer, TransTriangleBuffer,
            EdgeBuffer, PointBuffer;
        public Outline Outline;
        public bool EnableFaceNormal = false;

        /// <summary>
        /// Create input with empty geometry inputs.
        /// </summary>
        /// <param name="_outline">3D space for Revit to re-draw screen.</param>
        public GeometryDrawServerInputs(Outline _outline, bool _enableFaceNormal = false)
        {
            this.NonTransFaceBuffer = new RenderingPassBufferStorage();
            this.TransFaceBuffer = new RenderingPassBufferStorage();
            this.NonTransTriangleBuffer = new RenderingPassBufferStorage();
            this.TransTriangleBuffer = new RenderingPassBufferStorage();
            this.EdgeBuffer = new RenderingPassBufferStorage();
            this.PointBuffer = new RenderingPassBufferStorage();
            this.Outline = _outline;
            this.EnableFaceNormal = _enableFaceNormal;
        }

        public void AddTriangleToBuffer
            (g3.Triangle3d _triangle,
            XYZ _normal,
            ColorWithTransparency _cwt,
            XYZ _offset,
            bool _transparent)
        {
            var transBuffer = this.TransTriangleBuffer;
            var nonTransBuffer = this.NonTransTriangleBuffer;
            TriangleInfo triangleInfo = new TriangleInfo(_triangle, _normal, _cwt, _offset);

            if (_transparent)
            {
                transBuffer.Triangles.Add(triangleInfo);
                transBuffer.VertexBufferCount += 3;
                transBuffer.PrimitiveCount += 1;
            }
            else
            {
                nonTransBuffer.Triangles.Add(triangleInfo);
                nonTransBuffer.VertexBufferCount += 3;
                nonTransBuffer.PrimitiveCount += 1;
            }
        }

        public void AddSolidToBuffer
        (Solid _solid,
            ColorWithTransparency _cwt,
            XYZ _offset,
            bool _transparent)
        {
            foreach (Face face in _solid.Faces)
            {
                var normal = face.Evaluate(new UV(0, 0));
                AddFaceToBuffer(face, normal, _cwt, _offset, _transparent);
            }
        }

        public void AddFaceToBuffer
            (Face _face,
            XYZ _normal,
            ColorWithTransparency _cwt,
            XYZ _offset,
            bool _transparent)
        {
            Mesh mesh = _face.Triangulate();
            var transFaceBuffer = this.TransFaceBuffer;
            var nonTransFaceBuffer = this.NonTransFaceBuffer;
            MeshInfo meshInfo = new MeshInfo(mesh, _normal, _cwt, _offset);

            if (_transparent)
            {
                transFaceBuffer.Meshes.Add(meshInfo);
                transFaceBuffer.VertexBufferCount += mesh.Vertices.Count;
                transFaceBuffer.PrimitiveCount += mesh.NumTriangles;
            }
            else
            {
                nonTransFaceBuffer.Meshes.Add(meshInfo);
                nonTransFaceBuffer.VertexBufferCount += mesh.Vertices.Count;
                nonTransFaceBuffer.PrimitiveCount += mesh.NumTriangles;
            }
        }

        public void AddEdgeToBuffer
            (Edge _edge,
            ColorWithTransparency _color,
            XYZ _offset)
        {
            var xyzs = _edge.Tessellate();
            var edgeInfo = new EdgeInfo(xyzs, _color, _offset);
            var edgeBuffer = this.EdgeBuffer;
            edgeBuffer.Edges.Add(edgeInfo);
            edgeBuffer.VertexBufferCount += xyzs.Count;
            edgeBuffer.PrimitiveCount += xyzs.Count - 1;
        }

        public void AddCurveToBuffer
            (Curve _curve,
            ColorWithTransparency _color,
            XYZ _offset)
        {   
            var xyzs = _curve.Tessellate();
            AddTessellatedCurveToBuffer(xyzs, _color, _offset);
        }

        public void AddTessellatedCurveToBuffer
            (IList<XYZ> _vertices,
            ColorWithTransparency _color,
            XYZ _offset)
        {
            var xyzs = _vertices;
            var edgeInfo = new EdgeInfo(xyzs, _color, _offset);
            var edgeBuffer = this.EdgeBuffer;
            edgeBuffer.Edges.Add(edgeInfo);
            edgeBuffer.VertexBufferCount += xyzs.Count;
            edgeBuffer.PrimitiveCount += xyzs.Count - 1;
        }

        public void AddPointToBuffer
            (XYZ _point,
            ColorWithTransparency _color,
            XYZ _offset)
        {
            var pointinfo = new PointInfo(_point, _color, _offset);
            var buffer = this.PointBuffer;
            buffer.Points.Add(pointinfo);
            buffer.VertexBufferCount++;
            buffer.PrimitiveCount++;
        }

        public static GeometryDrawServerInputs Merge(IEnumerable<GeometryDrawServerInputs> _inputs)
        {
            var outlines = _inputs.Select(x => x.Outline.ToBoundingBox()).GetBoundingBox().ToOutLine();
            var input = new GeometryDrawServerInputs(outlines, _inputs.First().EnableFaceNormal);
            input.NonTransFaceBuffer = RenderingPassBufferStorage.Merge(_inputs.Select(x => x.NonTransFaceBuffer).ToList());
            input.TransFaceBuffer = RenderingPassBufferStorage.Merge(_inputs.Select(x => x.TransFaceBuffer).ToList());
            input.NonTransTriangleBuffer = RenderingPassBufferStorage.Merge(_inputs.Select(x => x.NonTransTriangleBuffer).ToList());
            input.TransTriangleBuffer = RenderingPassBufferStorage.Merge(_inputs.Select(x => x.TransTriangleBuffer).ToList());
            input.EdgeBuffer = RenderingPassBufferStorage.Merge(_inputs.Select(x => x.EdgeBuffer).ToList());
            input.PointBuffer = RenderingPassBufferStorage.Merge(_inputs.Select(x => x.PointBuffer).ToList());
            return input;
        }

        public GeometryDrawServerInputs CreateMerged(GeometryDrawServerInputs _other)
        {
            return Merge(new List<GeometryDrawServerInputs>() { this, _other });
        }
        
        public List<GeometryDrawServerInputs> SplitSelf()
        {
            int maxSize = 10000;
            //for each storage type, split stored geometry
            var nonTransMeshLists = this.NonTransFaceBuffer.Meshes.DivideListMaxSize(maxSize);
            var transMeshLists = this.TransFaceBuffer.Meshes.DivideListMaxSize(maxSize);
            var nonTransTriangleLists = this.NonTransTriangleBuffer.Triangles.DivideListMaxSize(maxSize);
            var transTriangleLists = this.TransTriangleBuffer.Triangles.DivideListMaxSize(maxSize);
            var pointLists = this.PointBuffer.Points.DivideListMaxSize(maxSize);
            var edgeLists = this.EdgeBuffer.Edges.DivideListMaxSize(maxSize);

            var maxCount = new List<int>()
            { nonTransMeshLists.Count, transMeshLists.Count,
                nonTransTriangleLists.Count, transTriangleLists.Count,
                pointLists.Count, edgeLists.Count}
            .Max();

            var inputList = new List<GeometryDrawServerInputs>();

            for(int i = 0;i < maxCount;i++)
            {
                var nonTransMesh = i > nonTransMeshLists.Count - 1
                    ? new List<MeshInfo>()
                    : nonTransMeshLists[i];
                var transMesh = i > transMeshLists.Count - 1
                    ? new List<MeshInfo>()
                    : transMeshLists[i];
                var nonTransTriangle = i > nonTransTriangleLists.Count - 1
                    ? new List<TriangleInfo>()
                    : nonTransTriangleLists[i];
                var transTriangle = i > transTriangleLists.Count - 1
                    ? new List<TriangleInfo>()
                    : transTriangleLists[i];
                var pointList = i > pointLists.Count - 1
                    ? new List<PointInfo>()
                    : pointLists[i];
                var edgeList = i > edgeLists.Count - 1
                    ? new List<EdgeInfo>()
                    : edgeLists[i];

                var newInput = new GeometryDrawServerInputs(this.Outline, this.EnableFaceNormal);

                newInput.NonTransFaceBuffer = new RenderingPassBufferStorage(nonTransMesh);
                newInput.TransFaceBuffer = new RenderingPassBufferStorage(transMesh);
                newInput.NonTransTriangleBuffer = new RenderingPassBufferStorage(nonTransTriangle);
                newInput.TransTriangleBuffer = new RenderingPassBufferStorage(transTriangle);
                newInput.PointBuffer = new RenderingPassBufferStorage(pointList);
                newInput.EdgeBuffer = new RenderingPassBufferStorage(edgeList);

                inputList.Add(newInput);
            }
            return inputList;
        }
    }
}
