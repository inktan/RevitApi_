using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using Teigha.Geometry;

using goa.Common;
using PubFuncWt;
using g3;
using PubFuncWt;

namespace TeighaNet
{
    public class PolyLineInfo : EntityInfo
    {
        public CurveArray CurveArray { get; set; }
        /// <summary>
        /// 已经 MilliMeterToFeet
        /// </summary>
        public PolyLine PolyLine { get; set; }
        /// <summary>
        /// 实际位置
        /// </summary>
        public List<XYZ> Pts { get; set; }
        public List<LineInfo> lineInfos { get; set; }
        //public bool Closed { get; set; }
        /// <summary>
        /// 依据 图形所在矩形左下角点 为原点
        /// </summary>
        public List<XYZ> PtsOriginByMin
        {
            get
            {
                List<XYZ> xYZs = new List<XYZ>();
                this.Pts.ForEach(p =>
                {
                    double x = p.X - this.PolyLine.GetOutline().MinimumPoint.X;
                    double y = p.Y - this.PolyLine.GetOutline().MinimumPoint.Y;
                    double z = p.Z - this.PolyLine.GetOutline().MinimumPoint.Z;

                    xYZs.Add(new XYZ(x, y, z));
                });
                return xYZs;
            }
        }

        Polyline Polyline { get; set; }
        public Polygon2d Polygon2d { get; set; }

        public PolyLineInfo(Entity entity) : base(entity)
        {
            this.Polyline = this.Entity as Polyline;
        }
        public PolyLineInfo()
        {
        }
        public PolyLineInfo(Polygon2d polygon2d)
        {
            this.Polygon2d = polygon2d;
            Parse(polygon2d);
        }
        public void Parse(Polygon2d polygon2d)
        {
            this.Pts = polygon2d.Vertices.Select(p => new XYZ(p.x, p.y, 0.0)).ToList();

            // 点集去重
            this.Pts = this.Pts.DelAlmostEqualPoint().ToList();
            //this.Polygon2d = new Polygon2d(this.Pts.ToVector2ds()).RemoveSharpCcorners();// 直线会消失
            this.Polygon2d = new Polygon2d(this.Pts.ToVector2ds());

            if (this.Polygon2d.IsClockwise)
            {
                this.Pts.Reverse();
            }
            this.Polygon2d = new Polygon2d(this.Pts.ToVector2ds());

            this.CurveArray = new CurveArray();
            if (this.Pts.Count() > 2)
            {
                // 多段线中包含的曲线需要进一步研究
                this.PolyLine = PolyLine.Create(this.Pts);
                for (int i = 0; i < this.Pts.Count(); ++i)
                {
                    this.CurveArray.Append(Autodesk.Revit.DB.Line.CreateBound(this.Pts[i], this.Pts[(i + 1) % this.Pts.Count()]));
                }
            }

            if (this.Pts.Count() > 1)
            {
                lineInfos = new List<LineInfo>();
                for (int i = 0; i < this.Pts.Count(); ++i)
                {
                    try
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.Segment2d = new Segment2d(this.Pts[i].ToVector2d(), this.Pts[i + 1].ToVector2d());
                        lineInfo.Line = Autodesk.Revit.DB.Line.CreateBound(this.Pts[i], this.Pts[i + 1]);
                        lineInfo.Center = (this.Pts[i] + this.Pts[i + 1]) * 0.5;
                        lineInfos.Add(lineInfo);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }
            }
            //throw new NotImplementedException();
        }

        public override void Parse()
        {
            //List<GeometryObject> gs = new List<GeometryObject>();
            //Autodesk.Revit.DB.Line line01 = this.Polyline.GetLineSegmentAt(0).ToLine();
            //Autodesk.Revit.DB.Line line02 = this.Polyline.GetLineSegmentAt(1).ToLine();
            //Autodesk.Revit.DB.Line line03 = this.Polyline.GetLineSegmentAt(2).ToLine();
            //Autodesk.Revit.DB.Line line04 = this.Polyline.GetLineSegmentAt(3).ToLine();

            ////gs.Add(line01);
            ////gs.Add(line02);
            //gs.Add(line03);
            ////gs.Add(line04);

            //CMD.Doc.CreateDirectShapeWithNewTransaction(gs);
            //this.Entity.Id.ToString().TaskDialogErrorMessage();
            //this.Entity.ObjectId.ToString().TaskDialogErrorMessage();

            //return;
            this.Pts = new List<XYZ>();
            int count = this.Polyline.NumberOfVertices;

            //double length01 = this.Polyline.Length;
            //double length02 = 0;
            //for (int i = 0; i < count; ++i)
            //    length02 += this.Polyline.GetPoint3dAt(i).DistanceTo(this.Polyline.GetPoint3dAt((i + 1) % count));
            //if (length01.EqualPrecision(length02) && count > 2)
            //{
            //    this.Closed = true;
            //}
            //else
            //{
            //    this.Closed = false;
            //}

            for (int i = 0; i < count; i++)
            {
                Point3d point3d = this.Polyline.GetPoint3dAt(i);
                this.Pts.Add(new XYZ(point3d.X.MilliMeterToFeet(), point3d.Y.MilliMeterToFeet(), point3d.Z.MilliMeterToFeet()));
            }

            // 点集去重
            this.Pts = this.Pts.DelAlmostEqualPoint().ToList();
            //this.Polygon2d = new Polygon2d(this.Pts.ToVector2ds()).RemoveSharpCcorners();// 直线会消失
            this.Polygon2d = new Polygon2d(this.Pts.ToVector2ds());

            if (this.Polygon2d.IsClockwise)
            {
                this.Pts.Reverse();
            }
            this.Polygon2d = new Polygon2d(this.Pts.ToVector2ds());

            this.CurveArray = new CurveArray();
            if (this.Pts.Count() > 2)
            {
                // 多段线中包含的曲线需要进一步研究
                this.PolyLine = PolyLine.Create(this.Pts);
                for (int i = 0; i < this.Pts.Count(); ++i)
                {
                    this.CurveArray.Append(Autodesk.Revit.DB.Line.CreateBound(this.Pts[i], this.Pts[(i + 1) % this.Pts.Count()]));
                }
            }

            if (this.Pts.Count() > 1)
            {
                lineInfos = new List<LineInfo>();
                for (int i = 0; i < this.Pts.Count(); ++i)
                {
                    try
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.Segment2d = new Segment2d(this.Pts[i].ToVector2d(), this.Pts[i + 1].ToVector2d());
                        lineInfo.Line = Autodesk.Revit.DB.Line.CreateBound(this.Pts[i], this.Pts[i + 1]);
                        lineInfo.Center = (this.Pts[i] + this.Pts[i + 1]) * 0.5;
                        lineInfos.Add(lineInfo);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }

                }
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 获取剪力墙的定位中心线
        /// </summary>
        /// <returns></returns>
        public List<Segment2d> GetCenterline(double width)
        {
            List<Segment2d> result = new List<Segment2d>();

            foreach (var item in this.Polygon2d.SegmentItr())
            {
                result.AddRange(item.Offset(width));
            }

            for (int i = 0; i < result.Count; i++)
            {
                bool isOverLap = false;

                for (int j = 0; j < result.Count; j++)
                {
                    if (j > i)
                    {
                        if (result[i].OverLap(result[j], -0.1))// 1=304.8
                        {
                            if (result[i].Length < result[j].Length)
                            {
                                result[i] = result[j];
                            }

                            result.RemoveAt(j);
                            j--;
                            isOverLap = true;
                        }
                    }
                }

                if (!isOverLap)
                {
                    result.RemoveAt(i);
                    i--;
                }
            }

            return result;
        }
    }

}
