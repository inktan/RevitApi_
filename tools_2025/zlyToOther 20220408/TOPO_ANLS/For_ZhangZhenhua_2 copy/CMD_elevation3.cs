using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Diagnostics;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Analysis;

//高程分析

namespace TOPO_ANLS
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD_elevation3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View activeview = uidoc.ActiveView;

            //get geometry
            Reference refe = uidoc.Selection.PickObject(ObjectType.Element);
            Element elem = doc.GetElement(refe);
            GeometryElement geomelem = elem.get_Geometry(new Options());

            //make spacing
            double spacing = 5000 / 304.8;
            //make planes & reverse
            double topo_highest = elem.get_BoundingBox(activeview).Max.Z + (50 / 304.8);
            double topo_lowest = elem.get_BoundingBox(activeview).Min.Z - (50 / 304.8);
            //double topo_highest = 400000 / 304.8;
            //double topo_lowest = 0;
            double plane_elevation = topo_lowest;
            List<Plane> planes_down = new List<Plane>();
            List<Plane> planes_up = new List<Plane>();
            XYZ plane_normal_down = new XYZ(0, 0, -1);
            XYZ plane_normal_up = new XYZ(0, 0, 1);
            do
            {
                planes_down.Add(Plane.CreateByNormalAndOrigin(plane_normal_down, new XYZ(0, 0, plane_elevation)));
                planes_up.Add(Plane.CreateByNormalAndOrigin(plane_normal_up, new XYZ(0, 0, plane_elevation)));
                plane_elevation += spacing;
            } while (plane_elevation <= topo_highest);



            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            if (sfm == null)
            { sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1); }
            else
            { sfm.Clear(); }

            AnalysisResultSchema resultschema = new AnalysisResultSchema("goa schema", "高程分析");
            int schemaindex = sfm.RegisterResult(resultschema);

            double trianglecount = 0;
            double faceerrcount = 0;


            foreach (GeometryObject geomobj in geomelem)
            {
                Mesh geommesh = geomobj as Mesh;
                if (geommesh == null) continue;
                for (int i = 0; i < geommesh.NumTriangles; i++)
                {
                    trianglecount++;
                    MeshTriangle triangular = geommesh.get_Triangle(i);
                    XYZ vertex0 = triangular.get_Vertex(0);
                    XYZ vertex1 = triangular.get_Vertex(1);
                    XYZ vertex2 = triangular.get_Vertex(2);
                    Plane plane = Plane.CreateByThreePoints(vertex0, vertex1, vertex2);
                    XYZ normal = plane.Normal;

                    CurveLoop cl = GetCurveLoop(vertex0, vertex1, vertex2);
                    List<CurveLoop> cls = new List<CurveLoop>();
                    cls.Add(cl);
                    Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(cls, normal, 1 / 304.8);


                    for (int j = 1; j < planes_down.Count; j++)
                    {

                        if (solid == null) { break; }
                        Solid solid_cut_down = BooleanOperationsUtils.CutWithHalfSpace(solid, planes_down[j]);
                        if (solid_cut_down == null) { continue; }
                        Solid solid_cut_up = BooleanOperationsUtils.CutWithHalfSpace(solid_cut_down, planes_up[j-1]);
                        if(solid_cut_up == null) { continue; }
                        Face face = null;
                        foreach (Face temp in solid_cut_up.Faces)
                        {
                            PlanarFace pf = temp as PlanarFace;
                            if (pf == null) { continue; }
                            if (pf.FaceNormal.IsAlmostEqualTo(normal)) { face = temp; break; }
                        }
                        if (face == null) { faceerrcount++; continue; }

                        Reference facerefe = face.Reference;

                        List<UV> uvs = new List<UV>();
                        uvs.Add(new UV());
                        FieldDomainPointsByUV pnts = new FieldDomainPointsByUV(uvs);

                        List<Double> doublelist = new List<double>();
                        doublelist.Add(j);
                        List<ValueAtPoint> vallist = new List<ValueAtPoint>();
                        vallist.Add(new ValueAtPoint(doublelist));
                        FieldValues vals = new FieldValues(vallist);

                        int index = sfm.AddSpatialFieldPrimitive(face, Transform.Identity);

                        sfm.UpdateSpatialFieldPrimitive(index, pnts, vals, schemaindex);

                    }

                }
            }

        //    TaskDialog.Show("triangle", trianglecount.ToString());
         //   TaskDialog.Show("faceerr", faceerrcount.ToString());


            return Result.Succeeded;
        }




        public CurveLoop GetCurveLoop(XYZ point0, XYZ point1, XYZ point2)
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



    }
}






