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

//等高线


namespace TOPO_ANLS
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD_contour : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View activeview = uidoc.ActiveView;

            Reference refe1 = uidoc.Selection.PickObject(ObjectType.Element);
            Reference refe2 = uidoc.Selection.PickObject(ObjectType.Element);







            return Result.Succeeded;
        }


        public void DrewContour(Element elem1,Element elem2)
        {
            Document doc = elem1.Document;
            View activeview = doc.ActiveView;

            Options opt = new Options();
            opt.View = activeview;

            GeometryElement geom1 = elem1.get_Geometry(opt);


            Solid solid1 = null;
            foreach (GeometryObject geomobj in geom1)
            {
                GeometryInstance geomins = geomobj as GeometryInstance;
                foreach (GeometryObject geomobj2 in geomins.SymbolGeometry)
                {
                    Solid temp1 = geomobj2 as Solid;
                    if (temp1 != null && temp1.Faces.Size != 0 && temp1.Edges.Size != 0) { solid1 = temp1; }
                }

            }



            GeometryElement geom2 = elem2.get_Geometry(opt);
            Face face2 = null;
            foreach (GeometryObject geomobj in geom2)
            {
                Solid temp2 = geomobj as Solid;
                if (temp2 == null) { continue; }
                foreach (Face facetemp in temp2.Faces)
                {
                    if (facetemp != null) { face2 = facetemp; }
                }
            }

            PlanarFace pface2 = face2 as PlanarFace;
            Surface surface2 = pface2.GetSurface();
            Plane plane2 = surface2 as Plane;



            List<EdgeArrayArray> list_edgeaa = new List<EdgeArrayArray>();
            Solid solid = BooleanOperationsUtils.CutWithHalfSpace(solid1, plane2);

            if (solid == null) { TaskDialog.Show("goa", "solid wrong"); }

            foreach (Face face in solid.Faces)
            {
                list_edgeaa.Add(face.EdgeLoops);

            }

            TaskDialog.Show("goa", list_edgeaa.Count.ToString() + "list_edgeaa");


            List<Curve> list_curve = new List<Curve>();
            foreach (EdgeArrayArray edgeaa in list_edgeaa)
            {
                foreach (EdgeArray edgea in edgeaa)
                {
                    foreach (Edge edge in edgea)
                    {
                        list_curve.Add(edge.AsCurve());
                    }
                }
            }

            TaskDialog.Show("goa", list_curve.Count.ToString() + "list_curve");

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("start");

                SketchPlane sketchplane = SketchPlane.Create(doc, plane2);
                int j = 0;
                foreach (Curve curve in list_curve)
                {
                    //  DetailCurve detailcurve = doc.Create.NewDetailCurve(activeview, curve);
                    try
                    {
                        ModelCurve modelcurve = doc.Create.NewModelCurve(curve, sketchplane);

                    }

                    catch
                    {
                        continue;
                    }
                    j++;
                }
                TaskDialog.Show("gha", j.ToString());
                trans.Commit();
            }
        }



    }
}
