using Autodesk.Revit.DB;
using g3;
using goa.Common;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeighaNet;

namespace InfoStrucFormwork
{
    class FloorInfo
    {
        internal FloorInfo OuterFloor { get; set; }
        internal PolyLineInfo PolyLineInfo { get; }

        //public StrucFloorTable StrucFloorTable { get; private set; }
        internal List<TextInfo> textInfos { get; set; }

        internal double zValue = 0.0;

        internal CurveArray CurveArray;

        public FloorInfo(PolyLineInfo polyLineInfo)
        {
            //this.DescendFloors = new List<FloorInfo>();
            //this.Holes = new List<FloorInfo>();
            //this.OtherTypeFloors = new List<FloorInfo>();

            this.PolyLineInfo = polyLineInfo;
            this.CurveArray = this.PolyLineInfo.CurveArray;
        }

        public Floor Floor { get; internal set; }
        //public FamilyInstance Descend { get; internal set; }
        //public FamilyInstance Hole { get; internal set; }
        //public FamilyInstance OtherTypeFloor { get; internal set; }// 使用楼板 开洞 族，来表达不同的楼板类型
        /// <summary>
        /// 降板
        /// </summary>
        //internal List<FloorInfo> DescendFloors { get; set; }
        /// <summary>
        /// 洞口
        /// </summary>
        //internal List<FloorInfo> Holes { get; set; }
        //internal List<FloorInfo> OtherTypeFloors { get; set; }

        ///// <summary>
        ///// 基于梁中线，获取其他类型的楼板区域，使用楼板-开洞族进行处理
        ///// </summary>
        ///// <param name="FloorTypeTable"></param>
        ///// <param name="textInfos"></param>
        ///// <param name="beamLineInfos"></param>
        //internal void ObtainOtherFloorType(List<StrucFloorTable> floorTypeTable, List<TextInfo> textInfos, List<LineInfo> beamLineInfos)
        //{

        //    List<string> tyneNames = floorTypeTable.Select(p => p.TypeName).ToList();
        //    List<Segment2d> segment2ds = beamLineInfos.Select(p => p.Line.ToSegment2d()).ToList();

        //    foreach (var item in textInfos)
        //    {
        //        if (tyneNames.Contains(item.Text))
        //        {
        //            Vector2d position = item.Center.ToVector2d();

        //            if (this.PolyLineInfo.Polygon2d.Contains(position))
        //            {
        //                FloorInfo floorInfo = new FloorInfo(new PolyLineInfo());
        //                floorInfo.StrucFloorTable = floorTypeTable.Where(p => p.TypeName == item.Text).FirstOrDefault();

        //                Segment2d ray2d = new Segment2d(position, position + new Vector2d(0, 1e6));
        //                IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(ray2d, ray2d);
        //                List<Vector2d> rectPointCol = new List<Vector2d>();
        //                foreach (var seg in segment2ds)
        //                {
        //                    intrSegment2Segment2.Segment2 = seg;
        //                    intrSegment2Segment2.Compute();
        //                    if (intrSegment2Segment2.Quantity == 1)
        //                    {
        //                        rectPointCol.Add(intrSegment2Segment2.Point0);
        //                    }
        //                }
        //                Vector2d verUp = rectPointCol.OrderBy(p => p.y).FirstOrDefault();

        //                ray2d = new Segment2d(position, position + new Vector2d(0, -1e6));
        //                intrSegment2Segment2.Segment1 = ray2d;
        //                rectPointCol = new List<Vector2d>();
        //                foreach (var seg in segment2ds)
        //                {
        //                    intrSegment2Segment2.Segment2 = seg;
        //                    intrSegment2Segment2.Compute();
        //                    if (intrSegment2Segment2.Quantity == 1)
        //                    {
        //                        rectPointCol.Add(intrSegment2Segment2.Point0);
        //                    }
        //                }
        //                Vector2d verDown = rectPointCol.OrderBy(p => p.y).LastOrDefault();

        //                ray2d = new Segment2d(position, position + new Vector2d(-1e6, 0));
        //                intrSegment2Segment2.Segment1 = ray2d;
        //                rectPointCol = new List<Vector2d>();
        //                foreach (var seg in segment2ds)
        //                {
        //                    intrSegment2Segment2.Segment2 = seg;
        //                    intrSegment2Segment2.Compute();
        //                    if (intrSegment2Segment2.Quantity == 1)
        //                    {
        //                        rectPointCol.Add(intrSegment2Segment2.Point0);
        //                    }
        //                }
        //                Vector2d verLeft = rectPointCol.OrderBy(p => p.x).LastOrDefault();

        //                ray2d = new Segment2d(position, position + new Vector2d(1e6, 0));
        //                intrSegment2Segment2.Segment1 = ray2d;
        //                rectPointCol = new List<Vector2d>();
        //                foreach (var seg in segment2ds)
        //                {
        //                    intrSegment2Segment2.Segment2 = seg;
        //                    intrSegment2Segment2.Compute();
        //                    if (intrSegment2Segment2.Quantity == 1)
        //                    {
        //                        rectPointCol.Add(intrSegment2Segment2.Point0);
        //                    }
        //                }
        //                Vector2d verRight = rectPointCol.OrderBy(p => p.x).FirstOrDefault();

        //                if (verUp != null && verDown != null && verLeft != null && verRight != null)
        //                {
        //                    List<Vector2d> vecs = new List<Vector2d>()
        //                    {
        //                        new Vector2d(verLeft.x, verDown.y),
        //                        new Vector2d(verRight.x, verDown.y),
        //                        new Vector2d(verRight.x, verUp.y),
        //                        new Vector2d(verLeft.x, verUp.y)
        //                    };
        //                    floorInfo.PolyLineInfo.Polygon2d = new Polygon2d(vecs);
        //                    floorInfo.PolyLineInfo.PolyLine = PolyLine.Create(vecs.Select(p => p.ToXYZ()).ToList());
        //                    //CMD.Doc.CreateDirectShapeWithNewTransaction(polyLineInfo.PolyLine.ToCurveLoop().ToList());
        //                    this.OtherTypeFloors.Add(floorInfo);
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取洞口和降板
        ///// </summary>
        ///// <param name="descendFloors"></param>
        ///// <param name="holes"></param>
        //internal void ObtainDesendFloorHole(List<FloorInfo> descendFloors, List<FloorInfo> holes)
        //{
        //    foreach (var floorInfo in descendFloors)
        //    {
        //        if (this != floorInfo)
        //        {
        //            foreach (var vec in floorInfo.PolyLineInfo.Polygon2d.InwardOffeet(0.2).Vertices)
        //            {
        //                if (this.PolyLineInfo.Polygon2d.Contains(vec))
        //                {
        //                    floorInfo.OuterFloor = this;
        //                    this.DescendFloors.Add(floorInfo);
        //                    break;
        //                }
        //            }
        //        }
        //    }

            //foreach (var floorInfo in holes)
            //{
            //    if (this != floorInfo)
            //    {
            //        foreach (var vec in floorInfo.PolyLineInfo.Polygon2d.Vertices)
            //        {
            //            if (this.PolyLineInfo.Polygon2d.Contains(vec))
            //            {
            //                this.Holes.Add(floorInfo);
            //                break;
            //            }
            //        }
            //    }
            //}

            //throw new NotImplementedException();
        //}

        /// <summary>
        /// 获取当前楼板的zvalue
        /// </summary>
        internal void GetzValue()
        {
            foreach (var item in textInfos)
            {
                if (item.Text.StartsWith("H"))
                {
                    double.TryParse(item.Text.Substring(1, item.Text.Length - 1), out zValue);
                    this.zValue = zValue.MilliMeterToFeet();
                    if (!zValue.EqualZreo())
                    {
                        break;
                    }
                }
                else
                {
                    double zTmp = 0.0;
                    double.TryParse(item.Text, out zTmp);
                    if (zTmp.EqualZreo()) continue;

                    this.zValue = zTmp.MilliMeterToFeet();
                }

            }
        }

    }
}
