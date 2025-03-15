using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 非障碍物性质的柱子，可由插件-点柱子-功能生成
    /// </summary>
    class RevitEleCol : RevitEleFS
    {
        internal double Width { get { return this.FamilyInstance.GetParaByName_("Width").AsDouble(); } }
        internal double Height { get { return this.FamilyInstance.GetParaByName_("Height").AsDouble(); } }
        internal RevitEleCol(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        { }
        internal BoundO BoundO => new BoundO(this.Polygon2d(), this.EleProperty);
        internal Polygon2d Polygon2d()
        {
            XYZ p = XYZ.Zero;

            XYZ leftDownPoint = new XYZ(p.X - this.Width / 2, p.Y - this.Width / 2, 0);
            XYZ leftUpPoint = new XYZ(p.X - this.Width / 2, p.Y + this.Width / 2, 0);
            XYZ rightUpPoint = new XYZ(p.X + this.Width / 2, p.Y + this.Width / 2, 0);
            XYZ rightDownPoint = new XYZ(p.X + this.Width / 2, p.Y - this.Width / 2, 0);

            XYZ _leftDownPoint = this.Trans.OfPoint(leftDownPoint);
            XYZ _leftUpPoint = this.Trans.OfPoint(leftUpPoint);
            XYZ _rightUpPoint = this.Trans.OfPoint(rightUpPoint);
            XYZ _rightDownPoint = this.Trans.OfPoint(rightDownPoint);
            // 注意，此处与revit保持一致，统一使用逆时针写法

            List<Vector2d> vector2Ds = new List<Vector2d>();
            vector2Ds.Add(_leftDownPoint.ToUV().ToVector2d());
            vector2Ds.Add(_rightDownPoint.ToUV().ToVector2d());
            vector2Ds.Add(_rightUpPoint.ToUV().ToVector2d());
            vector2Ds.Add(_leftUpPoint.ToUV().ToVector2d());

            return new Polygon2d(vector2Ds);
        }

    }
}
