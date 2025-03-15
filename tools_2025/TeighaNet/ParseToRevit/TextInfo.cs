using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using Teigha.Geometry;

using goa.Common;
using Autodesk.Revit.DB;
using PubFuncWt;

namespace TeighaNet
{
    public class TextInfo : EntityInfo
    {

        /// <summary>
        /// 该定位点为文字图元的左下角
        /// </summary>
        public XYZ Position { get; set; }
        public XYZ Normal { get; private set; }
        public XYZ Direction { get; set; }
        public g3.Vector2d Direction2d { get; set; }

        public double Rotation { get; set; }
        public string Text { get; set; }
        /// <summary>
        /// 已经 MilliMeterToFeet
        /// </summary>
        public double Value = 0.0;

        DBText DBText { get; set; }
        public TextInfo(Entity entity) : base(entity)
        {
            this.DBText = this.Entity as DBText;
        }

        public override void Parse()
        {
            this.Position = new XYZ(this.DBText.Position.X.MilliMeterToFeet(), this.DBText.Position.Y.MilliMeterToFeet(), this.DBText.Position.Z.MilliMeterToFeet());
            this.Normal = new XYZ(this.DBText.Normal.X, this.DBText.Normal.Y, this.DBText.Normal.Z);
            this.Text = this.DBText.TextString;

            if (this.Text.StartsWith("H"))
            {
                double.TryParse(this.Text.Substring(1, this.Text.Length - 1), out Value);
                this.Value = Value.MilliMeterToFeet();
            }
            else
            {
                double.TryParse(this.Text, out Value);
                this.Value = Value.MilliMeterToFeet();
            }

            this.Rotation = this.DBText.Rotation;
            this.Direction2d = new g3.Vector2d(1, 0).Rotate(g3.Vector2d.Zero, this.Rotation);
            this.Direction = this.Direction2d.ToXYZ();

            //CMD.Doc.CreateDirectShapeWithNewTransaction(new List<GeometryObject>() { Autodesk.Revit.DB.Line.CreateBound(this.Position, this.Position + this.Direction2d.ToXYZ() * 200) });

            //throw new NotImplementedException();
        }
    }
}
