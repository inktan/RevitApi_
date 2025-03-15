using Autodesk.Revit.DB;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;

namespace TeighaNet
{
    public class ArcInfo : EntityInfo
    {
        public Autodesk.Revit.DB.Arc Arc { get; set; }
        Teigha.DatabaseServices.Arc Td_Arc { get; set; }
        public ArcInfo(Entity entity) : base(entity)
        {
            this.Td_Arc = entity as Teigha.DatabaseServices.Arc;

        }

        public override void Parse()
        {
            double startAngle = this.Td_Arc.StartAngle;
            double endAngle = this.Td_Arc.EndAngle;

            if (startAngle > endAngle)
            {
                startAngle = this.Td_Arc.EndAngle;
                endAngle = this.Td_Arc.StartAngle;
            }
            Plane plane =
                Plane.CreateByNormalAndOrigin(
                    new XYZ(Td_Arc.Normal.X, Td_Arc.Normal.Y, Td_Arc.Normal.Z),
                    new XYZ(this.Td_Arc.Center.X.MilliMeterToFeet(), this.Td_Arc.Center.Y.MilliMeterToFeet(), this.Td_Arc.Center.Z.MilliMeterToFeet()));
            try
            {
                this.Arc = Autodesk.Revit.DB.Arc.Create(plane, this.Td_Arc.Radius.MilliMeterToFeet(), startAngle, endAngle);
            }
            catch (Exception)
            {
                //throw;
            }


            //throw new NotImplementedException();
        }
    }
}
