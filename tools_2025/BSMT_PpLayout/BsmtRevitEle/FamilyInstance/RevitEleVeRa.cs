using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common;
using g3;
using goa.Common.g3InterOp;
using ClipperLib;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 地下车库出入口坡道
    /// </summary>
    class RevitEleVeRa : RevitEleFS
    {
        internal XYZ UpDirection { get; }
        internal RevitEleVeRa(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        {
            this.UpDirection = this.Trans.OfVector(new XYZ(1, 0, 0));
        }
        internal BoundO BoundO { get { return new BoundO(this.Polygon2d(), this.EleProperty); } }
        internal Polygon2d Polygon2d()
        {
            if (this.EleProperty == EleProperty.VehicleRamp)
                return GetPolygon2d();

            else if (this.EleProperty == EleProperty.VehicleRamp_Arc_A)
                return GetPolygon2d_Arc_A();

            else if (this.EleProperty == EleProperty.VehicleRamp_Arc_B)
                return GetPolygon2d_Arc_B();

            else if (this.EleProperty == EleProperty.VehicleRamp_UpDown)
                return GetPolygon2d_UpDown();
            else if (this.EleProperty == EleProperty.VehicleRamp_UpDown_Arc)
                return GetPolygon2d_UpDown_Arc();

            else
                return new Polygon2d();
        }

        Polygon2d GetPolygon2d()
        {
            double width = this.FamilyInstance.LookupParameter("净宽").AsDouble() / 2;
            double height = this.FamilyInstance.LookupParameter("坡道底部可停车位置").AsDouble();// 坡道顶面标高距地面高度
            Transform transform = this.FamilyInstance.GetTransform();
            XYZ leftDownPoint = new XYZ(0, -width, 0);
            XYZ leftUpPoint = new XYZ(0, width, 0);
            XYZ rightUpPoint = new XYZ(height, width, 0);
            XYZ rightDownPoint = new XYZ(height, -width, 0);
            XYZ _leftDownPoint = transform.OfPoint(leftDownPoint);
            XYZ _leftUpPoint = transform.OfPoint(leftUpPoint);
            XYZ _rightUpPoint = transform.OfPoint(rightUpPoint);
            XYZ _rightDownPoint = transform.OfPoint(rightDownPoint);

            // 注意，此处与revit保持一致，统一使用逆时针写法

            List<Vector2d> vector2Ds = new List<Vector2d>();
            vector2Ds.Add(_leftDownPoint.ToUV().ToVector2d());
            vector2Ds.Add(_rightDownPoint.ToUV().ToVector2d());
            vector2Ds.Add(_rightUpPoint.ToUV().ToVector2d());
            vector2Ds.Add(_leftUpPoint.ToUV().ToVector2d());

            return new Polygon2d(vector2Ds);
        }
        Polygon2d GetPolygon2d_Arc_A()
        {
            double wight = this.FamilyInstance.LookupParameter("净宽").AsDouble() / 2;
            double canLength = this.FamilyInstance.LookupParameter("坡道底部可停车位置").AsDouble();

            double inwardRadius = this.FamilyInstance.LookupParameter("内弯半径").AsDouble();
            double radius = inwardRadius + wight;
            double cornerAngle = this.FamilyInstance.LookupParameter("弯道角度").AsDouble();//角度使用asdouble的方法，默认为弧度值

            // Revit 创建Arc的角度为四象限角度逆时针旋转，第一象限为0-90，第二象限为90-180
            double startAngl = Math.PI + Math.PI / 2;
            double endAngle = startAngl + cornerAngle;

            XYZ center = new XYZ(0, radius, 0);
            Arc arc = Arc.Create(center, radius, startAngl, endAngle, new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            Line line = null;
            if (canLength > arc.Length)
            {
                Transform transform1 = arc.ComputeDerivatives(1, true);
                XYZ xYZ = transform1.Origin;
                XYZ xYZ01 = xYZ + transform1.BasisX.Normalize() * (canLength - arc.Length);// 求出切线方向
                line = Line.CreateBound(xYZ, xYZ01);
            }

            Arc arc1 = arc.CreateTransformed(this.Trans) as Arc;
            List<XYZ> xYZs = arc1.SegmentationByMaxAngle(5);
            Line line01 = null;

            if (line != null)
            {
                line01 = line.CreateTransformed(this.Trans) as Line;
                xYZs.AddRange(new List<XYZ>() { line01.GetEndPoint(0), line01.GetEndPoint(1) });
            }

            List<XYZ> newXyzs = xYZs.DelAlmostEqualPoint().ToList();

            Path path = newXyzs.ToIntPoints().ToList();

            Paths _patch = path.Offset(wight, Precision_.ClipperMultiple, EndType.etOpenButt);// 这里的补丁，都处理为主车道宽度

            return _patch.FirstOrDefault().ToPolygon2d();
        }
        Polygon2d GetPolygon2d_Arc_B()//还需要进一步进行处理
        {
            double length01 = this.FamilyInstance.LookupParameter("起始直段长度").AsDouble();
            double wight = this.FamilyInstance.LookupParameter("净宽").AsDouble() / 2;
            double canLength = this.FamilyInstance.LookupParameter("坡道底部可停车位置").AsDouble();

            double inwardRadius = this.FamilyInstance.LookupParameter("中心半径").AsDouble();
            //double cornerAngle = this.FamilyInstance.LookupParameter("角度_车过弯的转头角度").AsDouble();//角度使用asdouble的方法，默认为弧度值
            double cornerAngle = this.FamilyInstance.LookupParameter("输入角度").AsDouble();//角度使用asdouble的方法，默认为弧度值

            // Revit 创建Arc的角度为四象限角度逆时针旋转，第一象限为0-90，第二象限为90-180
            double startAngl = Math.PI + Math.PI / 2;
            double endAngle = startAngl + cornerAngle;
            // 弧段的前半段
            Line lineFirst = null;
            if (length01 > 0)
            {
                XYZ xYZ001 = new XYZ(0, 0, 0);
                XYZ xYZ002 = xYZ001 + new XYZ(length01, 0, 0);
                lineFirst = Line.CreateBound(xYZ001, xYZ002);
            }
            // 弧段
            XYZ center = new XYZ(length01, inwardRadius, 0);
            Arc arc = Arc.Create(center, inwardRadius, startAngl, endAngle, new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            Line line = null;
            // 弧段的后半段
            if (canLength > arc.Length + length01)
            {
                Transform transform1 = arc.ComputeDerivatives(1, true);
                XYZ xYZ = transform1.Origin;
                XYZ xYZ01 = xYZ + transform1.BasisX.Normalize() * (canLength - arc.Length);
                line = Line.CreateBound(xYZ, xYZ01);
            }

            List<XYZ> xYZs = new List<XYZ>();
            // 弧段的前半段
            Line lineFirst01 = null;
            if (lineFirst != null)
            {
                lineFirst01 = lineFirst.CreateTransformed(this.Trans) as Line;
                xYZs.AddRange(new List<XYZ>() { lineFirst01.GetEndPoint(0), lineFirst01.GetEndPoint(1) });
            }
            // 弧段
            Arc arc1 = arc.CreateTransformed(this.Trans) as Arc;
            xYZs.AddRange(arc1.SegmentationByMaxAngle(5));
            // 弧段的后半段
            Line line01 = null;
            if (line != null)
            {
                line01 = line.CreateTransformed(this.Trans) as Line;
                xYZs.AddRange(new List<XYZ>() { line01.GetEndPoint(0), line01.GetEndPoint(1) });
            }

            List<XYZ> newXyzs = xYZs.DelAlmostEqualPoint().ToList();

            Path path = newXyzs.ToIntPoints().ToList();
            Paths _patch = path.Offset(wight, Precision_.ClipperMultiple, EndType.etOpenButt);// 这里的补丁，都处理为主车道宽度

            Polygon2d polygon2d = _patch.FirstOrDefault().ToPolygon2d();

            return _patch.FirstOrDefault().ToPolygon2d();
        }
        Polygon2d GetPolygon2d_UpDown()
        {
            double wight = this.FamilyInstance.LookupParameter("净宽").AsDouble() / 2;
            double height = this.FamilyInstance.LookupParameter("总长").AsDouble();
            Transform transform = this.FamilyInstance.GetTransform();
            XYZ leftDownPoint = new XYZ(0, -wight, 0);
            XYZ leftUpPoint = new XYZ(0, wight, 0);
            XYZ rightUpPoint = new XYZ(height, wight, 0);
            XYZ rightDownPoint = new XYZ(height, -wight, 0);
            XYZ _leftDownPoint = transform.OfPoint(leftDownPoint);
            XYZ _leftUpPoint = transform.OfPoint(leftUpPoint);
            XYZ _rightUpPoint = transform.OfPoint(rightUpPoint);
            XYZ _rightDownPoint = transform.OfPoint(rightDownPoint);

            // 注意，此处与revit保持一致，统一使用逆时针写法

            List<Vector2d> vector2Ds = new List<Vector2d>();
            vector2Ds.Add(_leftDownPoint.ToUV().ToVector2d());
            vector2Ds.Add(_rightDownPoint.ToUV().ToVector2d());
            vector2Ds.Add(_rightUpPoint.ToUV().ToVector2d());
            vector2Ds.Add(_leftUpPoint.ToUV().ToVector2d());

            return new Polygon2d(vector2Ds);
        }
        /// <summary>
        /// 上下坡-弧段
        /// </summary>
        /// <returns></returns>
        private Polygon2d GetPolygon2d_UpDown_Arc()
        {
            double length01 = this.FamilyInstance.LookupParameter("起始直段长度").AsDouble();
            double length02 = this.FamilyInstance.LookupParameter("末段长度").AsDouble();
            double wight = this.FamilyInstance.LookupParameter("净宽").AsDouble() / 2;
            //double canLength = this.FamilyInstance.LookupParameter("坡道底部可停车位置").AsDouble();

            double radius = this.FamilyInstance.LookupParameter("半径").AsDouble();
            //double cornerAngle = this.FamilyInstance.LookupParameter("角度_车过弯的转头角度").AsDouble();//角度使用asdouble的方法，默认为弧度值
            double cornerAngle = this.FamilyInstance.LookupParameter("输入角度").AsDouble();//角度使用asdouble的方法，默认为弧度值

            // Revit 创建Arc的角度为四象限角度逆时针旋转，第一象限为0-90，第二象限为90-180
            double startAngl = Math.PI + Math.PI / 2;
            double endAngle = startAngl + cornerAngle;
            // 弧段的前半段
            Line line01 = null;
            if (length01 > 0)
            {
                XYZ xYZ002 = XYZ.Zero - new XYZ(length01, 0, 0);
                line01 = Line.CreateBound(xYZ002, XYZ.Zero);
            }
            // 弧段
            XYZ center = new XYZ(0, radius, 0);
            Arc arc = Arc.Create(center, radius, startAngl, endAngle, new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            Line line02 = null;
            // 弧段的后半段
            if (length02 > 0)
            {
                Transform transform1 = arc.ComputeDerivatives(1, true);
                XYZ xYZ = transform1.Origin;
                XYZ xYZ01 = xYZ + transform1.BasisX.Normalize() * length02;
                line02 = Line.CreateBound(xYZ, xYZ01);
            }

            List<XYZ> xYZs = new List<XYZ>();
            // 弧段的前半段
            Line line03 = null;
            if (line01 != null)
            {
                line03 = line01.CreateTransformed(this.Trans) as Line;
                xYZs.AddRange(new List<XYZ>() { line03.GetEndPoint(0), line03.GetEndPoint(1) });
            }
            // 弧段
            Arc arc1 = arc.CreateTransformed(this.Trans) as Arc;
            xYZs.AddRange(arc1.SegmentationByMaxAngle(5));
            // 弧段的后半段
            Line line04 = null;
            if (line02 != null)
            {
                line04 = line02.CreateTransformed(this.Trans) as Line;
                xYZs.AddRange(new List<XYZ>() { line04.GetEndPoint(0), line04.GetEndPoint(1) });
            }

            List<XYZ> newXyzs = xYZs.DelAlmostEqualPoint().ToList();

            Path path = newXyzs.ToIntPoints().ToList();
            Paths _patch = path.Offset(wight, Precision_.ClipperMultiple, EndType.etOpenButt);// 这里的补丁，都处理为主车道宽度

            Line line00 = Line.CreateBound(XYZ.Zero, XYZ.Zero + new XYZ(100, 0, 0));
            Line line0001 = line00.CreateTransformed(this.Trans) as Line;

            Line line0002 = Line.CreateBound(this.FamilyInstance.LocationPoint(), this.FamilyInstance.LocationPoint() + new XYZ(200, 200, 0));

            //CMD.Doc.CreateDirectShapeWithNewTransaction(new List<GeometryObject>() { line00, line0001, line0002 });
            //CMD.Doc.CreateDirectShapeWithNewTransaction(new List<GeometryObject>() { line01, arc, line02, line03, arc1, line04 });

            Polygon2d polygon2d = _patch.FirstOrDefault().ToPolygon2d();

            return _patch.FirstOrDefault().ToPolygon2d();

        }
    }
}
