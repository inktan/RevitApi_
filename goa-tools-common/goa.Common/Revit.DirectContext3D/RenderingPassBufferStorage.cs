using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;

using goa.Common;

namespace goa.Revit.DirectContext3D
{
    // A class that brings together all the data and rendering parameters that are needed to draw one sequence of primitives (e.g., triangles)
    // with the same format and appearance.
    public class RenderingPassBufferStorage
    {
        public RenderingPassBufferStorage()
        {
            Meshes = new List<MeshInfo>();
            Edges = new List<EdgeInfo>();
            Points = new List<PointInfo>();
            Triangles = new List<TriangleInfo>();
        }

        public RenderingPassBufferStorage(List<MeshInfo> _meshes)
        {
            Meshes = new List<MeshInfo>();
            Edges = new List<EdgeInfo>();
            Points = new List<PointInfo>();
            Triangles = new List<TriangleInfo>();

            foreach (var meshInfo in _meshes)
            {
                this.Meshes.Add(meshInfo);
                this.VertexBufferCount += meshInfo.Mesh.Vertices.Count;
                this.PrimitiveCount += meshInfo.Mesh.NumTriangles;
            }
        }

        public RenderingPassBufferStorage(List<EdgeInfo> _edges)
        {
            Meshes = new List<MeshInfo>();
            Edges = new List<EdgeInfo>();
            Points = new List<PointInfo>();
            Triangles = new List<TriangleInfo>();

            foreach (var edgeInfo in _edges)
            {
                this.Edges.Add(edgeInfo);
                var xyzs = edgeInfo.Vertices;
                this.VertexBufferCount += xyzs.Count;
                this.PrimitiveCount += xyzs.Count - 1;
            }
        }

        public RenderingPassBufferStorage(List<PointInfo> _points)
        {
            Meshes = new List<MeshInfo>();
            Edges = new List<EdgeInfo>();
            Points = new List<PointInfo>();
            Triangles = new List<TriangleInfo>();

            foreach (var pointInfo in _points)
            {
                this.Points.Add(pointInfo);
                this.VertexBufferCount++;
                this.PrimitiveCount++;
            }
        }

        public RenderingPassBufferStorage(List<TriangleInfo> _triangles)
        {
            Meshes = new List<MeshInfo>();
            Edges = new List<EdgeInfo>();
            Points = new List<PointInfo>();
            Triangles = new List<TriangleInfo>();

            foreach (var triangleInfo in _triangles)
            {
                this.Triangles.Add(triangleInfo);
                this.VertexBufferCount += 3;
                this.PrimitiveCount += 1;
            }
        }

        public RenderingPassBufferStorage Merge(RenderingPassBufferStorage _other)
        {
            var newBuffer = new RenderingPassBufferStorage();
            newBuffer.Meshes = this.Meshes.Combine(_other.Meshes).ToList();
            newBuffer.Edges = this.Edges.Combine(_other.Edges).ToList();
            newBuffer.Points = this.Points.Combine(_other.Points).ToList();
            newBuffer.Triangles = this.Triangles.Combine(_other.Triangles).ToList();
            newBuffer.VertexBufferCount = this.VertexBufferCount + _other.VertexBufferCount;
            newBuffer.PrimitiveCount = this.PrimitiveCount + _other.PrimitiveCount;
            return newBuffer;
        }

        public static RenderingPassBufferStorage Merge(IList<RenderingPassBufferStorage> _buffers)
        {
            if (_buffers.Count == 0)
            {
                return null;
            }
            else if (_buffers.Count == 1)
            {
                return _buffers.First();
            }
            else
            {
                var newBuffer = _buffers[0].Merge(_buffers[1]);
                for (int i = 2; i < _buffers.Count; i++)
                {
                    var buffer1 = _buffers[i];
                    newBuffer = newBuffer.Merge(buffer1);
                }
                return newBuffer;
            }
        }

        public VertexFormatBits FormatBits { get; set; }

        public List<MeshInfo> Meshes { get; set; }
        public List<EdgeInfo> Edges { get; set; }
        public List<PointInfo> Points { get; set; }
        public List<TriangleInfo> Triangles { get; set; }

        public int PrimitiveCount { get; set; }
        public int VertexBufferCount { get; set; }
        public int IndexBufferCount { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public VertexFormat VertexFormat { get; set; }
        public EffectInstance EffectInstance { get; set; }
    }
}
