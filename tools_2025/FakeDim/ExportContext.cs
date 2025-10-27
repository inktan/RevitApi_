using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

namespace FAKE_DIMS
{
    public class ExportContext : IExportContext2D
    {
        public List<PlanarFace> PlanarFaces = new List<PlanarFace>();
        public List<Line> Lines = new List<Line>();

        public void Finish()
        {
            return;
        }

        public bool IsCanceled()
        {
            return true;
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            return;
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            var face = node.GetFace();
            var pf = face as PlanarFace;
            if (pf != null)
                this.PlanarFaces.Add(pf);
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
            return;
        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            return;
        }

        public void OnLight(LightNode node)
        {
            return;
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            return;
        }

        public void OnMaterial(MaterialNode node)
        {
            return;
        }

        public void OnRPC(RPCNode node)
        {
            return;
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            return;
        }

        public bool Start()
        {
            return true;
        }

        public RenderNodeAction OnCurve(CurveNode node)
        {
            var curve = node.GetCurve();
            if(curve is Line)
            {
                var l = curve as Line;
                Lines.Add(l);
            }
            return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnPolyline(PolylineNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnPoint(PointNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnLineSegment(LineSegment segment)
        {
            return;
        }

        public void OnPolylineSegments(PolylineSegments segments)
        {
            return;
        }

        public void OnText(TextNode node)
        {
            return;
        }

        public RenderNodeAction OnElementBegin2D(ElementNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd2D(ElementNode node)
        {
            return;
        }

        public RenderNodeAction OnFaceEdge2D(FaceEdgeNode node)
        {

            return RenderNodeAction.Skip;
        }

        public RenderNodeAction OnFaceSilhouette2D(FaceSilhouetteNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            throw new NotImplementedException();
        }
    }
}
