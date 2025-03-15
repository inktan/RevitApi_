using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 主车道中心线
    /// </summary>
    internal class RevitEleCurve : RevitEleCtrl
    {
        internal CurveElement CurveEle { get; }
        internal Curve Curve => this.CurveEle.GeometryCurve;

        internal RevitEleCurve(Element ele, EleProperty eleProperty) : base(ele, eleProperty)
        {
            this.CurveEle = ele as CurveElement;
        }

        private Segment2d Segment2d()
        {
            if (this.Curve is Line)
            {
                return (this.Curve as Line).ToSegment2d();
            }
            else if (this.Curve is Arc)// 如果是曲线，该处直接投影为线段
            {
                if (this.Curve.IsBound == true)// true为边界线 非圆形
                {
                    XYZ xYZ01 = this.Curve.GetEndPoint(0);
                    XYZ xYZ02 = this.Curve.GetEndPoint(1);
                    return new Segment2d(xYZ01.ToVector2d(), xYZ02.ToVector2d());
                }
                else
                    return new Segment2d();
            }
            else
                return new Segment2d();
        }

        // 是否需要追加 RevitEleLine 的信息
        // 还是直接把曲线转换为多段线        
        internal BoundSeg BoundSeg { get { return new BoundSeg(this.Segment2d(), this.EleProperty); } }
        /// <summary>
        /// 线线型
        /// </summary>
        //internal string TypeName
        //{
        //    get
        //    {
        //        return this.CurveEle.LineStyle.Name;
        //    }
        //}
        //internal EleProperty RevitEleProperty
        //{
        //    get
        //    {
        //        if (TypeName == "地库_主车道中心线") return EleProperty.PriLane;
        //        else if (TypeName == "地库_次车道中心线") return EleProperty.SecLane;
        //        else if (TypeName == "地库_自定义宽度车道中心线") return EleProperty.CusLane;

        //        return EleProperty.None;
        //    }
        //}


    }
}
