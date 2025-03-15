using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeighaNet;
using goa.Common;
using PubFuncWt;

namespace InfoStrucFormwork

{
    class ConBeamInfo
    {
        /// <summary>
        /// 创建的梁
        /// </summary>
        //public FamilyInstance FamilyInstance { get; internal set; }
        /// <summary>
        /// Cad mline定位
        /// </summary>
        public MlineInfo MlineInfo { get; set; }
        public Line Line { get; set; }
        /// <summary>
        /// 结构计算书
        /// </summary>
        public ConBeamType ConBeamType { get; internal set; }
        /// <summary>
        /// 翻边高度
        /// </summary>
        public double zValue = 0.0;
        /// <summary>
        /// 翻遍高度的文字信息
        /// </summary>
        public TextInfo BeamUpturinTextInfo { get; internal set; }

        public ConBeamInfo(MlineInfo mlineInfo)
        {
            this.MlineInfo = mlineInfo;
            this.Line = mlineInfo.Line;

            XYZ p0 = Line.GetEndPoint(0);
            XYZ p1 = Line.GetEndPoint(1);

            if (p0.X < p1.X)
            {
                this.Line = Line.CreateBound(p0, p1);
            }
            else if (p0.X > p1.X)
            {
                this.Line = Line.CreateBound(p1, p0);
            }
            else if (p0.X == p1.X)
            {
                if (p0.Y < p1.Y)
                {
                    this.Line = Line.CreateBound(p0, p1);
                }
                else
                {
                    this.Line = Line.CreateBound(p1, p0);

                }
            }

        }

    }
}
