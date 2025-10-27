using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using XYY_lib;



namespace TopoTools
{


    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Topocreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                goa.Common.APP.RevitWindow = Methods.GetRevitWindow(commandData.Application);

                //Get application and documnet objects
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uiapp.ActiveUIDocument.Document;

                Selection seledge = uiapp.ActiveUIDocument.Selection;

                Form_cursorPrompt.Start("选择多条边缘。点【完成】", goa.Common.APP.RevitWindow);
                IList<Reference> selectedRefs = seledge.PickObjects(ObjectType.Edge);
                Form_cursorPrompt.Stop();
                List<Curve> curves = new List<Curve>();

                foreach (Reference r in selectedRefs)
                {
                    Element e = doc.GetElement(r);
                    GeometryObject geo = e.GetGeometryObjectFromReference(r);
                    Edge edge = geo as Edge;
                    Curve c = edge.AsCurve();
                    curves.Add(c);
                }

                List<XYZ> points = new List<XYZ>();

                foreach (Curve c in curves)
                {
                    if (c.IfcurveIsLine() == true)
                    {
                        XYZ sp = c.GetEndPoint(0);
                        XYZ ep = c.GetEndPoint(1);

                        points.Add(sp);
                        points.Add(ep);
                    }

                    else
                    {
                        List<Transform> trans = new List<Transform>(DivideByDist(c, 10, true));
                        foreach (Transform tran in trans)
                        {
                            XYZ cp = tran.Origin;
                            points.Add(cp);
                        }
                    }
                }

                points = points.DistinctLowPt();

                using (Transaction createtopo = new Transaction(doc, "创建地形"))
                {
                    createtopo.Start();

                    TopographySurface topo = TopographySurface.Create(doc, points);

                    createtopo.Commit();
                }

                return Result.Succeeded;
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                return Result.Cancelled;
            }
            catch(Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, goa.Common.APP.RevitWindow);
                return Result.Failed;
            }
            finally
            {
                Form_cursorPrompt.Stop();
            }
        }


       


            

        public  List<Transform> DivideByDist( Curve _c, double _dist, bool _includeEnd)
        {
            List<Transform> list = new List<Transform>() { _c.ComputeDerivatives(0, true) };
            var steps = Math.Round(100 * _c.ApproximateLength / _dist);
            double inc = 1 / steps;
            double param = 0;
            double segment = 0;
            for (int i = 0; i < 999999; i++)
            {
                if (segment < _dist)
                {
                    XYZ previous = _c.Evaluate(param, true);
                    param += inc;
                    if (param > 1)
                        break;
                    XYZ current = _c.Evaluate(param, true);
                    var dist = current.DistanceTo(previous);
                    segment += dist;
                }
                else
                {
                    list.Add(_c.ComputeDerivatives(param, true));
                    segment = 0;
                }
            }
            if (_includeEnd)
            {
                list.Add(_c.ComputeDerivatives(1, true));
            }
            return list;
        }
       
        private  List<Transform> DivideByNum( Line _line, int _num, bool _includeEnd)
        {
            double stride = _num == 0
                ? 1.0
                : 1.0 / (double)_num;
            double current = 0.0;
            var list = new List<Transform>();
            for (int i = 0; i < 999999; i++)
            {
                if (current.IsAlmostEqualByDifference(1.0))
                    break;
                list.Add(_line.ComputeDerivatives(current, true));
                current += stride;
            }
            if (_includeEnd)
            {
                list.Add(_line.ComputeDerivatives(1.0, true));
            }
            return list;
        }
        private  List<Transform> DivideByNum( Curve _nonLine, int _num, bool _includeEnd)
        {
            double strideLength = _nonLine.ApproximateLength / _num;
            return _nonLine.DivideByDist(strideLength, _includeEnd);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

