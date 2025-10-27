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

    public class CMD_elevation1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View activeview = uidoc.ActiveView;
          //  if(activeview.ViewType != ViewType.ThreeD) { TaskDialog.Show("wrong", "请在三维视图下操作");return Result.Failed; }


            //get geometry
            Reference refe = uidoc.Selection.PickObject(ObjectType.Element);
            Element elem = doc.GetElement(refe);
            GeometryElement geomelem = elem.get_Geometry(new Options());

            //make spacing
            double spacing = 5000 / 304.8;
            //make planes & reverse
            //double topo_highest = elem.get_BoundingBox(activeview).Max.Z+(10/304.8);
            //double topo_lowest = elem.get_BoundingBox(activeview).Min.Z-(10/304.8);
            double topo_highest = 400000 / 304.8;
            double topo_lowest = 200000 / 304.8;
            double plane_elevation = topo_lowest;
            List<Plane> planes_down = new List<Plane>(); 
            List<Plane> planes_up = new List<Plane>();
            XYZ plane_normal_down = new XYZ(0, 0, -1);
            XYZ plane_normal_up = new XYZ(0, 0, 1);
            do{
                planes_down.Add(Plane.CreateByNormalAndOrigin(plane_normal_down, new XYZ(0, 0, plane_elevation)));
                planes_up.Add(Plane.CreateByNormalAndOrigin(plane_normal_up, new XYZ(0, 0, plane_elevation)));
                plane_elevation += spacing;
              } while (plane_elevation <= topo_highest);
            

          //  TaskDialog.Show("goa", planes_down.Count.ToString());
          //  TaskDialog.Show("goa", planes_up.Count.ToString());


            //transaction
            Transaction trans = new Transaction(doc);
            trans.Start("高程分析");

            SpatialFieldManager sfm = SpatialFieldManager.GetSpatialFieldManager(doc.ActiveView);
            if (sfm == null)
            {sfm = SpatialFieldManager.CreateSpatialFieldManager(doc.ActiveView, 1);}
            else
            {sfm.Clear();}

            AnalysisResultSchema resultschema = new AnalysisResultSchema("goa schema", "高程分析");
            int schemaindex = sfm.RegisterResult(resultschema);

            int count = 0;
            int aacount = 0;

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
                    if (normal.Z < 0) { normal = normal.Negate(); }

                    CurveLoop cl = GetCurveLoop(vertex0, vertex1, vertex2);
                    List<CurveLoop> cls = new List<CurveLoop>();
                    cls.Add(cl);
                    Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(cls, normal, 1 / 304.8);

                    if(solid == null) { TaskDialog.Show("goa", "123"); }

                    double solid_highest = FindHighest(vertex0, vertex1, vertex2);
                    double solid_lowest = FindLowest(vertex0, vertex1, vertex2);

                    int start = (int)((solid_lowest - topo_lowest) / spacing);
                    int end = (int)((solid_highest - topo_lowest) / spacing)+1;

                    //TaskDialog.Show("goa", "topo_highest" + topo_highest.ToString() + "\n" + "topo_lowest" + topo_lowest.ToString()+"\n"+ "solid_highest" + solid_highest.ToString()+"\n"+
                    //  "solid_lowest" + solid_lowest.ToString()+"\n"+ "start"+ start.ToString()+"\n"+ "end" + end.ToString()+"\n"
                    //    );


                    if ((end - start) == 1)
                    {
                        Face face = solid.Faces.get_Item(0);
                        Reference facerefe = face.Reference;

                        List<UV> uvs = new List<UV>();
                        uvs.Add(new UV());
                        FieldDomainPointsByUV pnts = new FieldDomainPointsByUV(uvs);

                        List<Double> doublelist = new List<double>();
                        doublelist.Add(start);
                        List<ValueAtPoint> vallist = new List<ValueAtPoint>();
                        vallist.Add(new ValueAtPoint(doublelist));
                        FieldValues vals = new FieldValues(vallist);

                        int index = sfm.AddSpatialFieldPrimitive(face, Transform.Identity);

                        sfm.UpdateSpatialFieldPrimitive(index, pnts, vals, schemaindex);
                        continue;
                    }




                    for (int j = start; j < end - 1; j++)
                    {
                        if (j == planes_down.Count - 2) { continue; }

                        //try
                        //{
                        //    var aaaa = planes_down[j + 1];
                        //}
                        //catch
                        //{ TaskDialog.Show("goa", j.ToString()); }


                        Solid newsolid = BooleanOperationsUtils.CutWithHalfSpace(solid, planes_down[j+1]);
                        Face face = GetFace(newsolid, normal);
                        if (face == null) { aacount++; continue;  }
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

                        BooleanOperationsUtils.CutWithHalfSpaceModifyingOriginalSolid(solid, planes_up[j + 1]);
                    }

                }
            }


            TaskDialog.Show("accc", aacount.ToString());
            TaskDialog.Show("goa", count.ToString());


            trans.Commit();

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

        public Face GetFace(Solid solid,XYZ normal)
        {
            if(solid == null) { return null; }
            Face face = null;

            foreach (Face temp in solid.Faces)
            {
                PlanarFace pf = temp as PlanarFace;
                if (pf == null) { continue; }
                if (pf.FaceNormal.IsAlmostEqualTo(normal)) { face = temp; }
            }

            return face;
        }

        public double FindHighest(XYZ p1, XYZ p2,XYZ p3)
        {
            double a = p1.Z;
            if (a < p2.Z) { a = p2.Z; }
            if (a < p3.Z) { a = p3.Z; }
            return a;
        }
        public double FindLowest(XYZ p1, XYZ p2, XYZ p3)
        {
            double a = p1.Z;
            if (a > p2.Z) { a = p2.Z; }
            if (a > p3.Z) { a = p3.Z; }
            return a;
        }

    }
}






