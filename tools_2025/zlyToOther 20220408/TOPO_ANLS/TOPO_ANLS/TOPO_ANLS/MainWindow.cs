using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Analysis;

using goa.Common;

namespace TOPO_ANLS
{
    public partial class MainWindow : System.Windows.Forms.Form
    {
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;
        public MainWindow()
        {
            InitializeComponent();
            this.Text += " " + APP.Version;
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);
        }

        internal void ElevationAnalysis()
        {

        }

        internal void SlopeAnalysis()
        {
            UIDocument uidoc = APP.UIApp.ActiveUIDocument;
            Document doc = uidoc.Document;
            var activeview = uidoc.ActiveView;
            if (activeview.ViewType != ViewType.ThreeD)
            {
                TaskDialog.Show("wrong", "请在三维视图下操作");
                return;
            }

            Reference refe = uidoc.Selection.PickObject(ObjectType.Element);
            Element elem = doc.GetElement(refe);

            GeometryElement geomelem = elem.get_Geometry(new Options());

            StringBuilder sb = new StringBuilder("坡度数据:");

            Transaction trans = new Transaction(doc);
            trans.Start("坡度分析");

            //int resultSchemaIndex = 1;
            //var schemaName = "goa slope";
            //var schemaDescription = "goa slope analysis";
            //AnalysisResultSchema resultschema;
            //SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            ////var rs = sfm.GetResultSchema(0);
            //if (sfm == null)
            //{
            //    sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);

            //    //SpatialFieldManager sfm = SpatialFieldManager.CreateSpatialFieldManager(activeview, 1);
            //    resultschema = new AnalysisResultSchema(schemaName, schemaDescription);
            //    resultSchemaIndex = sfm.RegisterResult(resultschema);
            //}
            //else
            //{
            //    for(int i = 1; i < 99; i++)
            //    {
            //        try
            //        {
            //            resultschema = sfm.GetResultSchema(i);
            //            if(resultschema.Name == schemaName)
            //            {
            //                resultSchemaIndex = i;
            //                break;
            //            }
            //        }
            //        catch { continue; }
            //    }
            //}

            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            if (sfm == null)
            {
                sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);
            }
            else
            {
                sfm.Clear();
            }

            AnalysisResultSchema resultschema = new AnalysisResultSchema("goa schema", "坡度分析");
            int schemaindex = sfm.RegisterResult(resultschema);


            int count = 0;

            foreach (GeometryObject geomobj in geomelem)
            {
                Mesh geommesh = geomobj as Mesh;
                if (geommesh == null) continue;
                for (int i = 0; i < geommesh.NumTriangles; i++)
                {
                    count++;
                    MeshTriangle triangular = geommesh.get_Triangle(i);
                    XYZ vertex0 = triangular.get_Vertex(0);
                    XYZ vertex1 = triangular.get_Vertex(1);
                    XYZ vertex2 = triangular.get_Vertex(2);
                    Plane plane = Plane.CreateByThreePoints(vertex0, vertex1, vertex2);
                    XYZ normal = plane.Normal;
                    double angle = normal.AngleTo(new XYZ(0, 0, 1));

                    CurveLoop cl = getCurveLoop(vertex0, vertex1, vertex2);
                    List<CurveLoop> cls = new List<CurveLoop>();
                    cls.Add(cl);
                    Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(cls, normal, 5 / 304.8);
                    Face face = solid.Faces.get_Item(0);
                    Reference facerefe = face.Reference;

                    List<UV> uvs = new List<UV>();
                    uvs.Add(new UV());
                    FieldDomainPointsByUV pnts = new FieldDomainPointsByUV(uvs);

                    List<Double> doublelist = new List<double>();
                    doublelist.Add(angle);
                    List<ValueAtPoint> vallist = new List<ValueAtPoint>();
                    vallist.Add(new ValueAtPoint(doublelist));
                    FieldValues vals = new FieldValues(vallist);

                    int index = sfm.AddSpatialFieldPrimitive(face, Transform.Identity);

                    sfm.UpdateSpatialFieldPrimitive(index, pnts, vals, schemaindex);




                    sb.Append(angle.ToString()).Append("\n");
                }
            }

            TaskDialog.Show("goa", count.ToString());


            trans.Commit();












            TaskDialog.Show("goa", sb.ToString());
        }

        private CurveLoop getCurveLoop(XYZ point0, XYZ point1, XYZ point2)
        {
            Curve curve0 = Line.CreateBound(point0, point1);
            Curve curve1 = Line.CreateBound(point1, point2);
            Curve curve2 = Line.CreateBound(point2, point0);
            List<Curve> curves = new List<Curve>();
            curves.Add(curve0);
            curves.Add(curve1);
            curves.Add(curve2);
            CurveLoop cl = CurveLoop.Create(curves);
            return cl;

        }

        #region modeless form related
        public void WakeUp()
        {
            enableCommands(true);
        }
        private void dozeOff()
        {
            enableCommands(false);
        }
        private void enableCommands(bool status)
        {
            try
            {
                foreach (System.Windows.Forms.Control ctrl in this.Controls)
                {
                    ctrl.Enabled = status;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void makeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            //this.dozeOff();
        }
        #endregion

        private void button_elev_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.ElevationAnalysis);
        }

        private void button_slope_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.SlopeAnalysis);
        }
    }
}
