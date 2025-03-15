using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using Teigha.Geometry;

using PubFuncWt;
using Autodesk.Revit.DB;
using goa.Common;
using g3;

namespace TeighaNet
{
    /// <summary>
    /// 目前应对单个mline
    /// </summary>
    public class MlineInfo : EntityInfo
    {

        Mline Mline { get; set; }
        /// <summary>
        /// 已经 MilliMeterToFeet
        /// </summary>
        public PolyLine PolyLine { get; set; }
        public Autodesk.Revit.DB.Line Line { get; set; }
        /// <summary>
        /// 单个多线对应的线圈
        /// </summary>
        public Polygon2d polygon2d { get; set; }
        g3.Vector2d[] vects = new g3.Vector2d[4];

        /// <summary>
        /// mline中心基准线
        /// </summary>
        public List<XYZ> Pts { get; set; }
        public MlineInfo(Entity entity) : base(entity)
        {
            this.Mline = this.Entity as Mline;
            this.Pts = new List<XYZ>();
        }
        public MlineInfo()
        {
        }

        public LineInfo LineInfo01 { get; set; }
        public LineInfo LineInfo02 { get; set; }

        public override void Parse()
        {
            if (this.Mline.NumberOfVertices == 2)
            {
                Section();
            }
            else
            {
                Sections();
            }
        }
        /// <summary>
        /// 多段多线
        /// </summary>
        void Sections()
        {

        }
        /// <summary>
        /// 一段多线
        /// </summary>
        void Section()
        {
            double scale = this.Mline.Scale.MilliMeterToFeet();// 双线宽度
            MlineJustification mlineJustification = this.Mline.Justification;// 线的偏移方向

            for (int i = 0; i < this.Mline.NumberOfVertices; i++)
            {
                Point3d point3d = this.Mline.VertexAt(i);
                this.Pts.Add(new XYZ(point3d.X.MilliMeterToFeet(), point3d.Y.MilliMeterToFeet(), point3d.Z.MilliMeterToFeet()));
            }

            Segment2d segment2d = Autodesk.Revit.DB.Line.CreateBound(this.Pts[0], this.Pts[1]).ToSegment2d();
            g3.Vector2d dir = segment2d.Direction.Rotate(g3.Vector2d.Zero, Math.PI / 2);

            switch (mlineJustification)
            {
                case MlineJustification.Zero:// 基准线在中心
                    segment2d = segment2d.Move(dir * -1, scale * 0.5);
                    vects[0] = segment2d.P0;
                    vects[1] = segment2d.P1;
                    segment2d = segment2d.Move(dir * 1, scale);
                    vects[2] = segment2d.P1;
                    vects[3] = segment2d.P0;

                    break;
                case MlineJustification.Bottom:// 基准线在下部
                    Segment2d basedSeg01 = segment2d.Move(dir, scale * 0.5);
                    this.Pts[0] = basedSeg01.P0.ToXYZ();
                    this.Pts[1] = basedSeg01.P1.ToXYZ();

                    vects[0] = segment2d.P0;
                    vects[1] = segment2d.P1;

                    segment2d = segment2d.Move(dir, scale);

                    vects[2] = segment2d.P1;
                    vects[3] = segment2d.P0;

                    break;
                case MlineJustification.Top:// 基准线在上部
                    Segment2d basedSeg02 = segment2d.Move(dir, scale * 0.5);
                    this.Pts[0] = basedSeg02.P0.ToXYZ();
                    this.Pts[1] = basedSeg02.P1.ToXYZ();

                    vects[0] = segment2d.P1;
                    vects[1] = segment2d.P0;

                    segment2d = segment2d.Move(dir * -1, scale);

                    vects[2] = segment2d.P0;
                    vects[3] = segment2d.P1;

                    break;
                default:
                    break;
            }

            this.Line = Autodesk.Revit.DB.Line.CreateBound(this.Pts[0], this.Pts[1]);
            this.polygon2d = new Polygon2d(this.vects);

            //throw new NotImplementedException();
        }
    }
}
