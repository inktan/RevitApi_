using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.DirectContext3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using goa.Common;
using goa.Common.Exceptions;
using goa.Common.g3InterOp;

using g3;

namespace goa.Revit.DirectContext3D
{
    /// <summary>
    /// Use this server to draw geometry on screen, without creating element in model.
    /// </summary>
    public class GeometryDrawServer : IDirectContext3DServer
    {
        public bool InputChanged = false;
        public GeometryDrawServer(UIDocument _uiDoc, string _addinId)
        {
            m_guid = Guid.NewGuid();
            AddinId = _addinId;
            m_uiDocument = _uiDoc;
            m_doc = _uiDoc.Document;
        }

        public GeometryDrawServer(UIDocument _uiDoc, string _addinId, GeometryDrawServerInputs _inputs)
            : this(_uiDoc, _addinId)
        { 
            this.Inputs = _inputs;
        }

        public System.Guid GetServerId() { return m_guid; }
        public System.String GetVendorId() { return "goa"; }
        public ExternalServiceId GetServiceId() { return ExternalServices.BuiltInExternalServices.DirectContext3DService; }
        public System.String GetName() { return "face offset draw Server"; }
        public System.String GetDescription() { return "draw faces and edges with an offset from original"; }
        // Corresponds to functionality that is not used in this sample.
        public System.String GetApplicationId() { return ""; }
        // Corresponds to functionality that is not used in this sample.
        public System.String GetSourceId() { return ""; }
        // Corresponds to functionality that is not used in this sample.
        public bool UsesHandles() { return false; }
        // Tests whether this server should be invoked for the given view.
        // The server only wants to be invoked for 3D views that are part of the document that contains the element in m_element.
        public bool CanExecute(Autodesk.Revit.DB.View view)
        {
            if (view is View3D == false
                && view is ViewPlan == false
                && view is ViewSection == false)
                return false;

            Document doc = view.Document;
            return doc.Equals(this.m_doc);
        }
        // Reports a bounding box of the geometry that this server submits for drawing.
        public Outline GetBoundingBox(Autodesk.Revit.DB.View view)
        {
            return this.Inputs.Outline;
        }
        // Indicates that this server will submit geometry during the rendering pass for transparent geometry.
        public bool UseInTransparentPass(Autodesk.Revit.DB.View view) { return true; }
        // Submits the geometry for rendering.
        public void RenderScene(Autodesk.Revit.DB.View view, DisplayStyle displayStyle)
        {
            try
            {
                //on request, process pre-stored graphic data
                //to get ready for submission for drawing.
                if (InputChanged)
                {
                    ProcessFaces(this.Inputs.NonTransFaceBuffer);
                    ProcessFaces(this.Inputs.TransFaceBuffer);
                    ProcessTriangles(this.Inputs.NonTransTriangleBuffer);
                    ProcessTriangles(this.Inputs.TransTriangleBuffer);
                    ProcessEdges(this.Inputs.EdgeBuffer);
                    ProcessPoints(this.Inputs.PointBuffer);
                }

                DrawContext.SetWorldTransform(this.WorldTransform);

                // Submit a subset of the geometry for drawing. Determine what geometry should be submitted based on
                // the type of the rendering pass (opaque or transparent) and DisplayStyle (wireframe or shaded).

                // If the server is requested to submit transparent geometry, DrawContext().IsTransparentPass()
                // will indicate that the current rendering pass is for transparent objects.
                RenderingPassBufferStorage faceBuffer =
                    DrawContext.IsTransparentPass()
                    ? this.Inputs.TransFaceBuffer
                    : this.Inputs.NonTransFaceBuffer;
                RenderingPassBufferStorage triangleBuffer =
                    DrawContext.IsTransparentPass()
                    ? this.Inputs.TransTriangleBuffer
                    : this.Inputs.NonTransTriangleBuffer;
                var edgeBuffer = this.Inputs.EdgeBuffer;
                var pointBuffer = this.Inputs.PointBuffer;

                // Conditionally submit triangle primitives (for non-wireframe views).
                if (displayStyle != DisplayStyle.Wireframe &&
                    faceBuffer.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(faceBuffer.VertexBuffer,
                                            faceBuffer.VertexBufferCount,
                                            faceBuffer.IndexBuffer,
                                            faceBuffer.IndexBufferCount,
                                            faceBuffer.VertexFormat,
                                            faceBuffer.EffectInstance, PrimitiveType.TriangleList, 0,
                                            faceBuffer.PrimitiveCount);

                if (displayStyle != DisplayStyle.Wireframe &&
                    triangleBuffer.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(triangleBuffer.VertexBuffer,
                                            triangleBuffer.VertexBufferCount,
                                            triangleBuffer.IndexBuffer,
                                            triangleBuffer.IndexBufferCount,
                                            triangleBuffer.VertexFormat,
                                            triangleBuffer.EffectInstance, PrimitiveType.TriangleList, 0,
                                            triangleBuffer.PrimitiveCount);

                // Conditionally submit line segment primitives.
                if (displayStyle != DisplayStyle.Shading &&
                    edgeBuffer.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(edgeBuffer.VertexBuffer,
                                            edgeBuffer.VertexBufferCount,
                                            edgeBuffer.IndexBuffer,
                                            edgeBuffer.IndexBufferCount,
                                            edgeBuffer.VertexFormat,
                                            edgeBuffer.EffectInstance, PrimitiveType.LineList, 0,
                                            edgeBuffer.PrimitiveCount);

                if (pointBuffer.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(pointBuffer.VertexBuffer,
                        pointBuffer.VertexBufferCount,
                        pointBuffer.IndexBuffer,
                        pointBuffer.IndexBufferCount,
                        pointBuffer.VertexFormat,
                        pointBuffer.EffectInstance, PrimitiveType.PointList, 0,
                        pointBuffer.PrimitiveCount);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                InputChanged = false;
            }
        }

        // Create and populate a pair of vertex and index buffers. Also update parameters associated with the format of the vertices.
        private void ProcessFaces(RenderingPassBufferStorage bufferStorage)
        {
            List<MeshInfo> meshes = bufferStorage.Meshes;
            if (meshes.Count == 0) return;
            List<int> numVerticesInMeshesBefore = new List<int>();

            bool useNormals = this.Inputs.EnableFaceNormal;

            // Vertex attributes are stored sequentially in vertex buffers. The attributes can include position, normal vector, and color.
            // All vertices within a vertex buffer must have the same format. Possible formats are enumerated by VertexFormatBits.
            // Vertex format also determines the type of rendering effect that can be used with the vertex buffer. In this sample,
            // the color is always encoded in the vertex attributes.

            bufferStorage.FormatBits = useNormals ? VertexFormatBits.PositionNormalColored : VertexFormatBits.PositionColored;

            // The format of the vertices determines the size of the vertex buffer.
            int vertexBufferSizeInFloats = (useNormals ? VertexPositionNormalColored.GetSizeInFloats() : VertexPositionColored.GetSizeInFloats()) *
               bufferStorage.VertexBufferCount;
            numVerticesInMeshesBefore.Add(0);

            bufferStorage.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            bufferStorage.VertexBuffer.Map(vertexBufferSizeInFloats);

            int numMeshes = meshes.Count;
            if (useNormals)
            {
                // A VertexStream is used to write data into a VertexBuffer.
                VertexStreamPositionNormalColored vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionNormalColored();
                for (int i = 0; i < numMeshes; i++)
                {
                    var meshInfo = meshes[i];
                    Mesh mesh = meshInfo.Mesh;
                    foreach (XYZ vertex in mesh.Vertices)
                    {
                        vertexStream.AddVertex(new VertexPositionNormalColored(vertex + meshInfo.Offset, meshInfo.Normal, meshInfo.ColorWithTransparency));
                    }

                    numVerticesInMeshesBefore.Add(numVerticesInMeshesBefore.Last() + mesh.Vertices.Count);
                }
            }
            else
            {
                // A VertexStream is used to write data into a VertexBuffer.
                VertexStreamPositionColored vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionColored();
                for (int i = 0; i < numMeshes; i++)
                {
                    var meshInfo = meshes[i];
                    Mesh mesh = meshInfo.Mesh;
                    // make the color of all faces white in HLR
                    ColorWithTransparency color = meshInfo.ColorWithTransparency;
                    foreach (XYZ vertex in mesh.Vertices)
                    {
                        vertexStream.AddVertex(new VertexPositionColored(vertex + meshInfo.Offset, color));
                    }

                    numVerticesInMeshesBefore.Add(numVerticesInMeshesBefore.Last() + mesh.Vertices.Count);
                }
            }

            bufferStorage.VertexBuffer.Unmap();

            // Primitives are specified using a pair of vertex and index buffers. An index buffer contains a sequence of indices into
            // the associated vertex buffer, each index referencing a particular vertex.

            int meshNumber = 0;
            bufferStorage.IndexBufferCount = bufferStorage.PrimitiveCount * IndexTriangle.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * bufferStorage.IndexBufferCount;
            bufferStorage.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            bufferStorage.IndexBuffer.Map(indexBufferSizeInShortInts);
            {
                // An IndexStream is used to write data into an IndexBuffer.
                IndexStreamTriangle indexStream = bufferStorage.IndexBuffer.GetIndexStreamTriangle();
                foreach (MeshInfo meshInfo in meshes)
                {
                    Mesh mesh = meshInfo.Mesh;
                    int startIndex = numVerticesInMeshesBefore[meshNumber];
                    for (int i = 0; i < mesh.NumTriangles; i++)
                    {
                        MeshTriangle mt = mesh.get_Triangle(i);
                        // Add three indices that define a triangle.
                        indexStream.AddTriangle(new IndexTriangle((int)(startIndex + mt.get_Index(0)),
                                                                  (int)(startIndex + mt.get_Index(1)),
                                                                  (int)(startIndex + mt.get_Index(2))));
                    }
                    meshNumber++;
                }
            }
            bufferStorage.IndexBuffer.Unmap();

            // VertexFormat is a specification of the data that is associated with a vertex (e.g., position).
            bufferStorage.VertexFormat = new VertexFormat(bufferStorage.FormatBits);
            // Effect instance is a specification of the appearance of geometry. For example, it may be used to specify color, if there is no color information provided with the vertices.
            bufferStorage.EffectInstance = new EffectInstance(bufferStorage.FormatBits);
        }

        // Create and populate a pair of vertex and index buffers. Also update parameters associated with the format of the vertices.
        private void ProcessTriangles(RenderingPassBufferStorage bufferStorage)
        {
            List<TriangleInfo> triangles = bufferStorage.Triangles;
            if (triangles.Count == 0) return;

            bool useNormals = this.Inputs.EnableFaceNormal;

            // Vertex attributes are stored sequentially in vertex buffers. The attributes can include position, normal vector, and color.
            // All vertices within a vertex buffer must have the same format. Possible formats are enumerated by VertexFormatBits.
            // Vertex format also determines the type of rendering effect that can be used with the vertex buffer. In this sample,
            // the color is always encoded in the vertex attributes.

            bufferStorage.FormatBits = useNormals ? VertexFormatBits.PositionNormalColored : VertexFormatBits.PositionColored;

            // The format of the vertices determines the size of the vertex buffer.
            int vertexBufferSizeInFloats = (useNormals ? VertexPositionNormalColored.GetSizeInFloats() : VertexPositionColored.GetSizeInFloats()) *
               bufferStorage.VertexBufferCount;

            bufferStorage.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            bufferStorage.VertexBuffer.Map(vertexBufferSizeInFloats);

            int numTriangles = triangles.Count;
            if (useNormals)
            {
                // A VertexStream is used to write data into a VertexBuffer.
                VertexStreamPositionNormalColored vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionNormalColored();
                for (int i = 0; i < numTriangles; i++)
                {
                    var triangleInfo = triangles[i];
                    g3.Triangle3d triangle = triangleInfo.Triangle;
                    vertexStream.AddVertex(new VertexPositionNormalColored(triangle.V0.ToXYZ() + triangleInfo.Offset, triangleInfo.Normal, triangleInfo.ColorWithTransparency));
                    vertexStream.AddVertex(new VertexPositionNormalColored(triangle.V1.ToXYZ() + triangleInfo.Offset, triangleInfo.Normal, triangleInfo.ColorWithTransparency));
                    vertexStream.AddVertex(new VertexPositionNormalColored(triangle.V2.ToXYZ() + triangleInfo.Offset, triangleInfo.Normal, triangleInfo.ColorWithTransparency));
                }
            }
            else
            {
                // A VertexStream is used to write data into a VertexBuffer.
                VertexStreamPositionColored vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionColored();
                for (int i = 0; i < numTriangles; i++)
                {
                    var triangleInfo = triangles[i];
                    g3.Triangle3d triangle = triangleInfo.Triangle;
                    // make the color of all faces white in HLR
                    ColorWithTransparency color = triangleInfo.ColorWithTransparency;
                    vertexStream.AddVertex(new VertexPositionColored(triangle.V0.ToXYZ() + triangleInfo.Offset, color));
                    vertexStream.AddVertex(new VertexPositionColored(triangle.V1.ToXYZ() + triangleInfo.Offset, color));
                    vertexStream.AddVertex(new VertexPositionColored(triangle.V2.ToXYZ() + triangleInfo.Offset, color));
                }
            }

            bufferStorage.VertexBuffer.Unmap();

            // Primitives are specified using a pair of vertex and index buffers. An index buffer contains a sequence of indices into
            // the associated vertex buffer, each index referencing a particular vertex.

            bufferStorage.IndexBufferCount = bufferStorage.PrimitiveCount * IndexTriangle.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * bufferStorage.IndexBufferCount;
            bufferStorage.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            bufferStorage.IndexBuffer.Map(indexBufferSizeInShortInts);
            // An IndexStream is used to write data into an IndexBuffer.
            IndexStreamTriangle indexStream = bufferStorage.IndexBuffer.GetIndexStreamTriangle();
            int currIndex = 0;
            for (int i = 0; i < numTriangles; i++)
            {
                // Add three indices that define a triangle.
                indexStream.AddTriangle(new IndexTriangle(currIndex + 0,currIndex + 1,currIndex + 2));
                currIndex += 3;
            }
            bufferStorage.IndexBuffer.Unmap();

            // VertexFormat is a specification of the data that is associated with a vertex (e.g., position).
            bufferStorage.VertexFormat = new VertexFormat(bufferStorage.FormatBits);
            // Effect instance is a specification of the appearance of geometry. For example, it may be used to specify color, if there is no color information provided with the vertices.
            bufferStorage.EffectInstance = new EffectInstance(bufferStorage.FormatBits);
        }

        // A helper function, analogous to ProcessFaces.
        private void ProcessEdges(RenderingPassBufferStorage bufferStorage)
        {
            var edges = bufferStorage.Edges;
            if (edges.Count == 0)
                return;

            // Edges are encoded as line segment primitives whose vertices contain only position information.
            bufferStorage.FormatBits = VertexFormatBits.PositionColored;

            //int edgeVertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * bufferStorage.VertexBufferCount;
            int edgeVertexBufferSizeInFloats = VertexPositionColored.GetSizeInFloats() * bufferStorage.VertexBufferCount;
            List<int> numVerticesInEdgesBefore = new List<int>();
            numVerticesInEdgesBefore.Add(0);

            int numEdges = edges.Count;
            bufferStorage.VertexBuffer = new VertexBuffer(edgeVertexBufferSizeInFloats);
            bufferStorage.VertexBuffer.Map(edgeVertexBufferSizeInFloats);

            var vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionColored();
            for (int i = 0; i < numEdges; i++)
            {
                var edgeInfo = edges[i];
                foreach (XYZ vertex in edgeInfo.Vertices)
                {
                    vertexStream.AddVertex(new VertexPositionColored(vertex + edgeInfo.Offset, edgeInfo.Color));
                }

                numVerticesInEdgesBefore.Add(numVerticesInEdgesBefore.Last() + edgeInfo.Vertices.Count);
            }

            bufferStorage.VertexBuffer.Unmap();

            int edgeNumber = 0;
            bufferStorage.IndexBufferCount = bufferStorage.PrimitiveCount * IndexLine.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * bufferStorage.IndexBufferCount;
            bufferStorage.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            bufferStorage.IndexBuffer.Map(indexBufferSizeInShortInts);
            {
                IndexStreamLine indexStream = bufferStorage.IndexBuffer.GetIndexStreamLine();
                foreach (var edgeInfo in edges)
                {
                    var xyzs = edgeInfo.Vertices;
                    int startIndex = numVerticesInEdgesBefore[edgeNumber];
                    for (int i = 1; i < xyzs.Count; i++)
                    {
                        // Add two indices that define a line segment.
                        indexStream.AddLine(new IndexLine((int)(startIndex + i - 1),
                                                          (int)(startIndex + i)));
                    }
                    edgeNumber++;
                }
            }
            bufferStorage.IndexBuffer.Unmap();

            bufferStorage.VertexFormat = new VertexFormat(bufferStorage.FormatBits);
            bufferStorage.EffectInstance = new EffectInstance(bufferStorage.FormatBits);
        }
        // A helper function, analogous to ProcessFaces.
        private void ProcessPoints(RenderingPassBufferStorage bufferStorage)
        {
            var points = bufferStorage.Points;
            if (points.Count == 0)
                return;

            // Edges are encoded as line segment primitives whose vertices contain only position information.
            bufferStorage.FormatBits = VertexFormatBits.PositionColored;

            //int edgeVertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * bufferStorage.VertexBufferCount;
            int vertexBufferSizeInFloats = VertexPositionColored.GetSizeInFloats() * bufferStorage.VertexBufferCount;

            int numPoints = points.Count;
            bufferStorage.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            bufferStorage.VertexBuffer.Map(vertexBufferSizeInFloats);

            var vertexStream = bufferStorage.VertexBuffer.GetVertexStreamPositionColored();
            for (int i = 0; i < numPoints; i++)
            {
                var pointInfo = points[i];
                var vertex = pointInfo.Vertex;
                vertexStream.AddVertex(new VertexPositionColored(vertex + pointInfo.Offset, pointInfo.Color));
            }

            bufferStorage.VertexBuffer.Unmap();

            bufferStorage.IndexBufferCount = bufferStorage.PrimitiveCount * IndexPoint.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * bufferStorage.IndexBufferCount;
            bufferStorage.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            bufferStorage.IndexBuffer.Map(indexBufferSizeInShortInts);
            {
                IndexStreamPoint indexStream = bufferStorage.IndexBuffer.GetIndexStreamPoint();
                for (int i = 0; i < numPoints; i++)
                {
                    indexStream.AddPoint(new IndexPoint(i));
                }
            }
            bufferStorage.IndexBuffer.Unmap();

            bufferStorage.VertexFormat = new VertexFormat(bufferStorage.FormatBits);
            bufferStorage.EffectInstance = new EffectInstance(bufferStorage.FormatBits);
        }
        public Document Document
        {
            get { return (m_uiDocument != null) ? m_uiDocument.Document : null; }
        }
        private Guid m_guid;
        internal string AddinId;

        //private List<FamilyInstance> fis;
        private UIDocument m_uiDocument;
        private Document m_doc;

        public GeometryDrawServerInputs Inputs;
        public Transform WorldTransform = Transform.Identity;
    }
}
