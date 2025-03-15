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
    /// 普通停车位
    /// </summary>
    class RevitElePS : RevitEleFS
    {
        // 宽和高两个数据来源于族类型参数
        internal double Width { get { return this.FamilyInstance.GetParaByName_("Width").AsDouble(); } }
        internal double Height { get { return this.FamilyInstance.GetParaByName_("Height").AsDouble(); } }
        internal RevitElePS(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        { }

        /// <summary>
        /// 收集当前离停车位最近的两个车位，如果不存在，则会复制自身数据
        /// </summary>
        internal List<RevitElePS> Last2PPsExit { get; set; }
        /// <summary>
        /// 如果该车位为端头位置车位，则求出该端头指向另一端头的方向
        /// </summary>
        internal Vector2d RowDirection { get; set; }
        /// <summary>
        /// 判断当前端头停车位是否为尽端车道停车位
        /// </summary>
        internal CarType RowEndType { get; set; }

        internal BoundO BoundO { get { return new BoundO(this.Polygon2d(), this.EleProperty); } }

        // 求四个角点


        internal Polygon2d Polygon2d()
        {
            if (this.EleProperty == EleProperty.AttachedPP
                || this.EleProperty == EleProperty.FastChargePP
                || this.EleProperty == EleProperty.EndPP
                || this.EleProperty == EleProperty.MechanicalPP
                || this.EleProperty == EleProperty.MiniParkSpace
                || this.EleProperty == EleProperty.ParkSpace
                || this.EleProperty == EleProperty.PublicPP
                || this.EleProperty == EleProperty.SlowChargePP)
                //return GetPolygon2d();
                return GetPolygon2dByRevit();

            else if (this.EleProperty == EleProperty.BarrierFreePP)
            {
                Polygon2d polygon2d = GetPolygon2d_BarrierFreePPByRevit();

                return polygon2d;
            }

            //else if (this.EleProperty == EleProperty.MechanicalPP)
            //    return GetPolygon2d_MechanicalPP();
            else
                return new Polygon2d();
        }
        internal double Count()
        {
            // 子母停车位
            if (this.EleProperty == EleProperty.AttachedPP)
                return 1.5;
            // 无障碍停车位
            else if (this.EleProperty == EleProperty.BarrierFreePP)
                return 1;
            // 快充电车位
            // 慢充电车位
            // 公共停车位
            else if (this.EleProperty == EleProperty.FastChargePP
                || this.EleProperty == EleProperty.SlowChargePP
                || this.EleProperty == EleProperty.PublicPP)
                return 1;
            // 机械车位 需要单独进行计算
            //else if (this.EleProperty == EleProperty.MechanicalPP)
            //{
            //    Parameter parameter = this.Ele.LookupParameter("机械车位数");
            //    return parameter.AsInteger();
            //}
            // 标准停车位
            else if (this.EleProperty == EleProperty.ParkSpace
                || this.EleProperty == EleProperty.BigParkSpace)
            {
                return 1;
            } // 微型停车位
            else if (this.EleProperty == EleProperty.MiniParkSpace)
            {
                return 0.7;
            }
            else
                return 1;
        }

        //Polygon2d GetPolygon2d()
        //{
        //    double x = this.LocVector2d.x;
        //    double y = this.LocVector2d.y;

        //    Vector2d lD = new Vector2d(x - this.Width / 2, y );
        //    Vector2d lU = new Vector2d(x - this.Width / 2, y + this.Height);
        //    Vector2d rU = new Vector2d(x + this.Width / 2, y + this.Height);
        //    Vector2d rD = new Vector2d(x + this.Width / 2, y );

        //    Polygon2d polygon2d = new Polygon2d(new List<Vector2d>() { lU, lD, rD, rU });

        //    double remainder = this.RotateAngle % (Math.PI * 2);

        //    if (remainder.EqualZreo())
        //        return polygon2d;
        //    else
        //        return polygon2d.Rotate(this.LocVector2d, this.RotateAngle);
        //}
        //Polygon2d GetPolygon2d_BarrierFreePP()
        //{
        //    double x = this.LocVector2d.x;
        //    double y = this.LocVector2d.y;

        //    Vector2d lD = new Vector2d(x - this.Width / 2 - 1200.0.MilliMeterToFeet(), y); // 无障碍车位需要减去安全距离
        //    Vector2d lU = new Vector2d(x - this.Width / 2 - 1200.0.MilliMeterToFeet(), y + this.Height);// 无障碍车位需要减去安全距离
        //    Vector2d rU = new Vector2d(x + this.Width / 2, y + this.Height);
        //    Vector2d rD = new Vector2d(x + this.Width / 2, y);

        //    Polygon2d polygon2d = new Polygon2d(new List<Vector2d>() { lU, lD, rD, rU });
        //    double remainder = this.RotateAngle % (Math.PI * 2);

        //    if (remainder.EqualZreo())
        //    {
        //        return polygon2d;
        //    }
        //    else
        //    {               
        //        return polygon2d.Rotate(this.LocVector2d, this.RotateAngle);
        //    }
        //}
        //Polygon2d GetPolygon2d_MechanicalPP()
        //{
        //    int count = Convert.ToInt32(Parameter_.GetParaByName_(this.FamilyInstance, "首层车位数").AsValueString());
        //    double x = this.LocVector2d.x;
        //    double y = this.LocVector2d.y;
        //    Vector2d lD = new Vector2d(x - this.Width / 2,y);
        //    Vector2d lU = new Vector2d(x - this.Width / 2, y + this.Height / 2);
        //    Vector2d rU = new Vector2d(x + this.Width * (count - 1 + 0.5), y + this.Height / 2);
        //    Vector2d rD = new Vector2d(x + this.Width * (count - 1 + 0.5), y);

        //    Polygon2d polygon2d = new Polygon2d(new List<Vector2d>() { lU, lD, rD, rU });
        //    double remainder = this.RotateAngle % (Math.PI * 2);

        //    if (remainder.EqualZreo())
        //        return polygon2d;
        //    else
        //        return polygon2d.Rotate(this.LocVector2d, this.RotateAngle);
        //}


        /// <summary>
        /// 子母车位——快充电位——普通车位——公共泊车位——慢充电位
        /// </summary>
        /// <returns></returns>
        Polygon2d GetPolygon2dByRevit()
        {
            XYZ leftDownPoint = new XYZ(-this.Width / 2, 0.0, 0.0);
            XYZ rightDownPoint = new XYZ(this.Width / 2, 0.0, 0.0);
            XYZ leftUpPoint = new XYZ(-this.Width / 2, this.Height, 0);
            XYZ rightUpPoint = new XYZ(this.Width / 2, this.Height, 0);

            List<XYZ> xYZs = new List<XYZ>() { leftUpPoint, leftDownPoint, rightDownPoint, rightUpPoint };

            List<Vector2d> vector2Ds = new List<Vector2d>();
            for (int i = 0; i < xYZs.Count; i++)
            {
                vector2Ds.Add(this.Trans.OfPoint(xYZs[i]).ToVector2d());
            }
            // 注意，此处与revit保持一致，统一使用逆时针写法
            return new Polygon2d(vector2Ds);
        }
        /// <summary>
        /// 无障碍车位——线圈
        /// </summary>
        /// <returns></returns>
        Polygon2d GetPolygon2d_BarrierFreePPByRevit()
        {
            XYZ p = XYZ.Zero;

            XYZ leftDownPoint = new XYZ(p.X - this.Width / 2 - 1200.0.MilliMeterToFeet(), p.Y, 0); // 无障碍车位需要减去安全距离
            XYZ leftUpPoint = new XYZ(p.X - this.Width / 2 - 1200.0.MilliMeterToFeet(), p.Y + this.Height, 0);// 无障碍车位需要减去安全距离
            XYZ rightUpPoint = new XYZ(p.X + this.Width / 2, p.Y + this.Height, 0);
            XYZ rightDownPoint = new XYZ(p.X + this.Width / 2, p.Y, 0);

            List<XYZ> xYZs = new List<XYZ>() { leftUpPoint, leftDownPoint, rightDownPoint, rightUpPoint };

            xYZs = this.Trans.OfPoints(xYZs).ToList();

            // 注意，此处与revit保持一致，统一使用逆时针写法
            return new Polygon2d(xYZs.ToVector2ds());
        }
        /// <summary>
        /// 机械车位
        /// </summary>
        /// <returns></returns>
        //Polygon2d GetPolygon2d_MechanicalPPByRevit()
        //{
        //    int count = Convert.ToInt32(Parameter_.GetParaByName_(this.FamilyInstance, "首层车位数").AsValueString());

        //    XYZ p = XYZ.Zero;

        //    XYZ leftDownPoint = new XYZ(p.X - this.Width / 2, p.Y - this.Height / 2, 0);
        //    XYZ leftUpPoint = new XYZ(p.X - this.Width / 2, p.Y + this.Height / 2, 0);
        //    XYZ rightUpPoint = new XYZ(p.X + this.Width * (count - 1 + 0.5), p.Y + this.Height / 2, 0);
        //    XYZ rightDownPoint = new XYZ(p.X + this.Width * (count - 1 + 0.5), p.Y - this.Height / 2, 0);

        //    List<XYZ> xYZs = new List<XYZ>() { leftUpPoint, leftDownPoint, rightDownPoint, rightUpPoint };

        //    // 判断是否为镜像关系
        //    bool isMirrored = this.FamilyInstance.IsMirriored(ref xYZs);

        //    List<Vector2d> vector2Ds = new List<Vector2d>();
        //    for (int i = 0; i < xYZs.Count; i++)
        //    {
        //        vector2Ds.Add(this.Transform.OfPoint(xYZs[i]).ToVector2d());
        //    }
        //    // 注意，此处与revit保持一致，统一使用逆时针写法
        //    return new Polygon2d(vector2Ds);
        //}

        internal List<ColUnitSpace> ColumnUnitSpaces()
        {
            ColGeneration colGeneration = new ColGeneration(this);
            return colGeneration.ColumnUnitSpaces;
        }

    }
}
