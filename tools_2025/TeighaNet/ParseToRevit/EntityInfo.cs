using Autodesk.Revit.DB;
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
    public abstract class EntityInfo
    {
        public string Layer { get; set; }
        public Entity Entity { get; set; }
        public XYZ MaxPoint { get; set; }
        public XYZ MinPoint { get; set; }
        public XYZ Center { get; set; }

        public abstract void Parse();
        public EntityInfo()
        { }
        public EntityInfo(Entity entity)
        {
            // 当前测试，脱离teighaNetService无法读取数据
            this.Entity = entity;
            this.Layer = this.Entity.Layer;

            try
            {
                Extents3d ext = this.Entity.GeometricExtents;
                this.MinPoint = new XYZ(ext.MinPoint.X.MilliMeterToFeet(), ext.MinPoint.Y.MilliMeterToFeet(), ext.MinPoint.Z.MilliMeterToFeet());
                this.MaxPoint = new XYZ(ext.MaxPoint.X.MilliMeterToFeet(), ext.MaxPoint.Y.MilliMeterToFeet(), ext.MaxPoint.Z.MilliMeterToFeet());

                this.Center = (this.MinPoint + this.MaxPoint) * 0.5;
            }
            catch (Exception)
            {
                this.MinPoint = XYZ.Zero;
                this.MaxPoint = XYZ.Zero;

                this.Center = XYZ.Zero; ;
                //"识别该文字的几何体失败。"
                //throw;
            }
        }
    }
}
