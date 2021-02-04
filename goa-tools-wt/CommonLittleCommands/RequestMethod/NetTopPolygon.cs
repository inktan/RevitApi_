using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using NetTopologySuite.Geometries;
using PublicProjectMethods_;

namespace CommonLittleCommands
{
    class NetTopPolygon : RequestMethod
    {
        public NetTopPolygon(UIApplication uiapp) : base(uiapp)
        { }

        internal override void Execute()
        {
            #region 与revit文档交互入口
            UIDocument uidoc = this.uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View _view = doc.ActiveView;
            #endregion

            Coordinate[] points = new Coordinate[] { new Coordinate(0, 0), new Coordinate(100, 100), new Coordinate(100, 0), new Coordinate(0, 100), new Coordinate(0, 0) };
            LinearRing linearRing = new LinearRing(points);
            Polygon polygon01 = new Polygon(linearRing);
            polygon01.Coordinates.Count().ToString().TaskDialogErrorMessage();


            Coordinate[] points02 = new Coordinate[] { new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(10, 10), new Coordinate(0, 10), new Coordinate(0, 0) };
            LinearRing linearRing02 = new LinearRing(points02);
            Polygon polygon02 = new Polygon(linearRing02);
            polygon02.Coordinates.Count().ToString().TaskDialogErrorMessage();

            Geometry geometry = polygon01.Difference(polygon02);

            if (geometry.OgcGeometryType == OgcGeometryType.Polygon)
            {
                Polygon polygon = geometry as Polygon;
                Coordinate[] coordinates = polygon.Coordinates;//点集需要去重


            }


        }
    }



}
