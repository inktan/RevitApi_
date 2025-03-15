using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using PubFuncWt;

namespace TeighaNet
{
    public class SplineInfo : EntityInfo
    {
        public Autodesk.Revit.DB.Curve Curve { get; set; }
        Spline Td_Spline { get; set; }
        public SplineInfo(Entity entity) : base(entity)
        {
            this.Td_Spline = entity as Teigha.DatabaseServices.Spline;
        }

        public override void Parse()
        {
            List<double> weights = new List<double>();
            List<XYZ> controlPoints = new List<XYZ>();

            for (int i = 0; i < this.Td_Spline.NumControlPoints; i++)
            {
                XYZ temp = new XYZ(
                        this.Td_Spline.GetControlPointAt(i).X.MilliMeterToFeet(),
                        this.Td_Spline.GetControlPointAt(i).Y.MilliMeterToFeet(),
                        this.Td_Spline.GetControlPointAt(i).Z.MilliMeterToFeet());
                if (!temp.PointCoinPoints(controlPoints))
                {
                    controlPoints.Add(temp);
                    weights.Add(this.Td_Spline.WeightAt(i));
                }
            }
            try
            {
                this.Curve = NurbSpline.CreateCurve(HermiteSpline.Create(controlPoints, false));
            }
            catch (Exception)
            {
                //throw;
            }

            //throw new NotImplementedException();
        }
    }
}
