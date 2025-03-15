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

namespace TeighaNet
{
    public class LineInfo : EntityInfo
    {
        public Autodesk.Revit.DB.Line Line { get; set; }
        public Segment2d Segment2d { get; set; }
        Teigha.DatabaseServices.Line Td_Line { get; set; }
        public Polygon2d Polygon2d { get; set; }

        public LineInfo(Entity entity) : base(entity)
        {
            this.Td_Line = entity as Teigha.DatabaseServices.Line;
        }
        public LineInfo()
        {
        }


        public override void Parse()
        {

            XYZ p0 = new XYZ(this.Td_Line.StartPoint.X.MilliMeterToFeet(), this.Td_Line.StartPoint.Y.MilliMeterToFeet(), this.Td_Line.StartPoint.Z.MilliMeterToFeet());
            XYZ p1 = new XYZ(this.Td_Line.EndPoint.X.MilliMeterToFeet(), this.Td_Line.EndPoint.Y.MilliMeterToFeet(), this.Td_Line.EndPoint.Z.MilliMeterToFeet());

            this.Segment2d = new Segment2d(p0.ToVector2d(), p1.ToVector2d());

            try
            {
                this.Line = Autodesk.Revit.DB.Line.CreateBound(p0, p1);
            }
            catch (Exception)
            {
                //throw;
            }

            //throw new NotImplementedException();
        }
    }
}
