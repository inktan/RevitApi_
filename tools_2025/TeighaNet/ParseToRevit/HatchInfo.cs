using Autodesk.Revit.DB;
using g3;
using goa.Common;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace TeighaNet
{
    public class HatchInfo : EntityInfo
    {
        Teigha.DatabaseServices.Hatch Td_Hatch { get; set; }

        public HatchInfo(Entity entity) : base(entity)
        {
            this.Td_Hatch = entity as Teigha.DatabaseServices.Hatch;
        }
        public HatchInfo()
        {
        }
        public double Area { get; set; }
        public List<Polygon2d> Polygon2ds { get; set; }
        public override void Parse()
        {
            this.Area = this.Td_Hatch.Area;
            int index = 0;
            this.Polygon2ds = new List<Polygon2d>();
            while (true)
            {
                try
                {
                    HatchLoop hatchLoop = this.Td_Hatch.GetLoopAt(index);
                    Curve2dCollection curve2DCollection = hatchLoop.Curves;

                    List<g3.Vector2d> vector2ds = new List<g3.Vector2d>();
                    foreach (Curve2d item in curve2DCollection)
                    {
                        Point2d point2d = item.StartPoint;
                        vector2ds.Add(new g3.Vector2d(point2d.X.MilliMeterToFeet(), point2d.Y.MilliMeterToFeet()));
                    }
                    Polygon2d polygon2d = new Polygon2d(vector2ds);
                    this.Polygon2ds.Add(polygon2d);
                }
                catch (Exception)
                {
                    break;
                }
                index++;
            }

            //throw new NotImplementedException();
        }
    }
}
