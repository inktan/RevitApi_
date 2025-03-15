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
    public class CircleInfo : EntityInfo
    {
        public Autodesk.Revit.DB.Arc Arc { get; set; }
        Teigha.DatabaseServices.Circle Td_Circle { get; set; }
        public double Radius { get; set; }
        public XYZ Center { get; set; }
        public CircleInfo(Entity entity) : base(entity)
        {
            this.Td_Circle = entity as Teigha.DatabaseServices.Circle;

        }

        public override void Parse()
        {
            this.Radius = this.Td_Circle.Radius.MilliMeterToFeet();
            this.Center = new XYZ(this.Td_Circle.Center.X.MilliMeterToFeet(), this.Td_Circle.Center.Y.MilliMeterToFeet(), this.Td_Circle.Center.Z.MilliMeterToFeet());
            try
            {
                this.Arc = Autodesk.Revit.DB.Arc.Create(Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), XYZ.Zero), this.Radius, 0, Math.PI * 2);
            }
            catch (Exception)
            {
                //throw;
            }
            //this.Arc = Autodesk.Revit.DB.Arc.Create(this.Center, this.Radius, 0, Math.PI * 2, new XYZ(1, 0, 0), new XYZ(0, 1, 0));

            //throw new NotImplementedException();
        }
    }
}
