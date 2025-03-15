using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeighaNet;
using PubFuncWt;

//using goa.Common;

namespace InfoStrucFormwork
{
    class ColumnInfo
    {
        public FamilyInstance FamilyInstance { get; internal set; }
        Polygon2d polygon2d { get; set; }

        internal XYZ positon { get; set; }

        public ColumnInfo(PolyLineInfo polyLineInfo)
        {
            this.PolyLineInfo = polyLineInfo;
            this.polygon2d = this.PolyLineInfo.Polygon2d;
        }
        internal ColType ColType;
        public ColumnInfo(CircleInfo circleInfo)
        {
            this.CircleInfo = circleInfo;
            this.positon = circleInfo.Center;
            this.colTypeName = ((int)(circleInfo.Radius.FeetToMilliMeter() * 2)).ToString() + @"mm";

            this.ColType = ColType.Circle;
        }

        public CircleInfo CircleInfo { get; }
        public PolyLineInfo PolyLineInfo { get; }

        internal void Excute()
        {

            GeoRec();
            //GeoCruciform();

            //throw new NotImplementedException();
        }

        internal double width = 0.0;
        internal double length = 0.0;
        internal string colTypeName = "";

        internal double rotateAngle = 0.0;

        /// <summary>
        /// 判断矩形柱
        /// </summary>
        internal void GeoRec()
        {
            if (this.polygon2d == null || !this.polygon2d.IsRectangle())
            {
                return;
            }

            width = this.polygon2d.Segment(0).Length;
            length = this.polygon2d.Segment(1).Length;

            if (width > length)
            {
                width = this.polygon2d.Segment(1).Length;
                length = this.polygon2d.Segment(0).Length;
            }

            if (!width.EqualZreo() && !length.EqualZreo())
            {
                colTypeName = ((int)(width.FeetToMilliMeter())).ToString() + "*" + ((int)(length.FeetToMilliMeter())).ToString();
            }

            this.positon = this.polygon2d.Center().ToXYZ();
            this.ColType = ColType.Rect;

            // 进一步计算旋转角度



            //throw new NotImplementedException();
        }

        /// <summary>
        /// 判断十字形
        /// </summary>
        internal void GeoCruciform()
        {
            if (this.polygon2d.IsRectangle())
            {
                return;
            }

            //throw new NotImplementedException();
        }
        internal void GeoCircle()
        {
            if (this.polygon2d == null || !this.polygon2d.IsRectangle())
            {
                return;
            }
        }
    }
    internal enum ColType : int
    {
        Rect,// 矩形
        Circle,// 圆形
        Cruciform,// 十字型
        Others,
    }
}
