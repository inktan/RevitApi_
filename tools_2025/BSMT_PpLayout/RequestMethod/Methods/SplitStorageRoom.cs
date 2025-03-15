using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;
using ClipperLib;
using goa.Common.g3InterOp;

namespace BSMT_PpLayout
{

    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class SplitStorageRoom : RequestMethod
    {
        public SplitStorageRoom(UIApplication uiApp) : base(uiApp) { }

        List<Segment2d> WallSeg2ds = new List<Segment2d>();
        List<Polygon2d> StorageRoomPolys = new List<Polygon2d>();

        internal override void Execute()
        {
            FilledRegion filledRegion = this.doc.GetElement(sel.PickObject(ObjectType.Element, new SelPickFilter_FilledRegion_StoreRoom() { Doc = this.doc })) as FilledRegion;

            List<Curve> curveElements = sel.PickObjects(ObjectType.Element, new SelPickFilter_Line()).Select(p => this.doc.GetElement(p) as CurveElement).Select(p => p.GeometryCurve).ToList();

            Polygon2d oriStorageSpace = GetStoreage(filledRegion);// 原始储藏空间
            List<Polygon2d> walkWay = WalkwaySpace(curveElements);// 道路空间

            List<Polygon2d> oriStorageSpaces = oriStorageSpace.DifferenceClipper(walkWay).ToList();

            // 找到道路的第一个边向对边作垂线
            oriStorageSpaces.ForEach(p =>
            {
                List<Polygon2d> polygon2ds = PrimaryDivStorage(p,GlobalData.Instance.MinArea_num);
                StorageRoomPolys.AddRange(polygon2ds);
                //string filledTypeName = "地库_储藏间_已分隔";
                //view.CreatFilledRegoins(this.doc, polygon2ds.ToCurveLoops().ToList(), filledTypeName);
            });

            this.WallSeg2ds = WallSeg2dMerge();

            WallType wallType = (new FilteredElementCollector(this.doc)).OfCategory(BuiltInCategory.OST_Walls).Where(p => p is WallType).Where(p => p.Name == "常规 - 200mm").FirstOrDefault() as WallType;

            using (Transaction trans = new Transaction(this.doc, "创建储藏室的墙体"))
            {
                trans.DeleteErrOrWaringTaskDialog();
                trans.Start();
                this.WallSeg2ds.ForEach(p =>
                {
                    //this.doc.CreateDirectShapeWithNewTransaction(new List<Line>() { p.ToLine() }, this.view);
                    if (wallType == null)
                    {
                        Wall.Create(this.doc, p.ToLine(), this.view.GenLevel.Id, false);
                    }
                    else
                    {
                        Wall.Create(this.doc, p.ToLine(), wallType.Id, this.view.GenLevel.Id, 100, 0.0, false, false);
                    }

                });
                trans.Commit();
            }



        }
        /// <summary>
        /// 将墙线进行几何合并
        /// </summary>
        /// <returns></returns>
        List<Segment2d> WallSeg2dMerge()
        {
            List<Segment2d> _seg2ds = new List<Segment2d>();

            this.StorageRoomPolys.ForEach(p =>
            {
                Polygon2d poly = p.OutwardOffeet(10.0.MilliMeterToFeet());
                _seg2ds.AddRange(poly.SegmentItr());
            });

            // 线段去重
            List<Segment2d> seg2ds = _seg2ds.DelDuplicate();

            int time = 0;
            while (true)
            {
                time++;
                if (time > 10)
                {
                    //break;
                }

                bool isbrea = true;
                int count = seg2ds.Count;
                for (int i = 0; i < count; i++)
                {
                    Segment2d seg01 = seg2ds[i];
                    Segment2d _seg01 = seg01.TwoWayExtension(1);
                    if (seg01.Length < Precision_.TheShortestDistance)
                    {
                        continue;
                    }
                    for (int j = 0; j < count; j++)
                    {
                        if (j > i)
                        {
                            Segment2d seg02 = seg2ds[j];
                            Segment2d _seg02 = seg02.TwoWayExtension(1);
                            if (seg02.Length < Precision_.TheShortestDistance)
                            {
                                continue;
                            }

                            IntrSegment2Segment2 intrSeg2d = new IntrSegment2Segment2(_seg01, _seg02);
                            intrSeg2d.Compute();
                            if (intrSeg2d.Quantity == 2)
                            {
                                List<Vector2d> vector2ds = new List<Vector2d>();
                                vector2ds.AddRange(seg01.EndPoints());
                                vector2ds.AddRange(seg02.EndPoints());

                                double distance = 0;
                                int a = 0;
                                int b = 0;
                                for (int _i = 0; _i < vector2ds.Count; _i++)
                                {
                                    for (int _j = 0; _j < vector2ds.Count; _j++)
                                    {
                                        if (_j > _i)
                                        {
                                            double newDis = vector2ds[_i].DistanceSquared(vector2ds[_j]);
                                            if (newDis > distance)
                                            {
                                                distance = newDis;
                                                a = _i;
                                                b = _j;
                                            }
                                        }
                                    }
                                }

                                seg2ds[i] = new Segment2d(vector2ds[a], vector2ds[b]);
                                seg2ds[j] = new Segment2d();
                                isbrea = false;
                            }
                        }

                    }
                }

                if (isbrea)
                {
                    break;
                }
            }
            return seg2ds.Where(p => p.Length > Precision_.TheShortestDistance).ToList();
        }
        /// <summary>
        /// 批量处理
        /// </summary>
        /// <returns></returns>
        List<Polygon2d> PrimaryDivStorage(Polygon2d polygon2d, double minArea)
        {
            List<Polygon2d> collectionMeet = new List<Polygon2d>();

            List<Polygon2d> priDivePolys = SecondaryDivStorage(polygon2d);

            int i = 0;
            while (true)
            {
                i++;
                if (i > 10)
                {
                    //break;
                }
                if (priDivePolys.Count < 1)
                {
                    break;
                }
                List<Polygon2d> secondaryDivePolys = new List<Polygon2d>();

                foreach (Polygon2d o in priDivePolys)
                {
                    List<Polygon2d> divePolys = SecondaryDivStorage(o);
                    foreach (Polygon2d poly2d in divePolys)
                    {
                        if (poly2d.Area <= minArea)
                        {
                            collectionMeet.Add(poly2d);
                        }
                        else
                        {
                            secondaryDivePolys.Add(poly2d);
                        }
                    }
                }
                // 重置
                priDivePolys = secondaryDivePolys;
            }
            return collectionMeet;
        }
        /// <summary>
        /// 划分原则
        /// </summary>
        List<Polygon2d> SecondaryDivStorage(Polygon2d polygon2d)
        {
            if (polygon2d.Area > 0)
            {
                // 切割基准线的设置问题
                Segment2d longestSeg = new Segment2d();
                // 01
                //List<Segment2d> segment2ds = polygon2d.SegmentItr().OrderBy(p => p.Length).ToList();
                //longestSeg = segment2ds.Last();

                // 02

                Polygon2d rectangle = polygon2d.GetBounds().ToPolygon();
                List<Segment2d> segment2ds = rectangle.SegmentItr().ToList();
                Segment2d bottom = segment2ds[0];
                Segment2d right = segment2ds[1];

                Vector2d center = rectangle.Center();
                if (bottom.Length > right.Length)// 水平边长
                {
                    longestSeg = new Segment2d(center - new Vector2d(1000, 0), center + new Vector2d(1000, 0));
                }
                else // 竖直边长 或者相等
                {
                    longestSeg = new Segment2d(center - new Vector2d(0, 1000), center + new Vector2d(0, 1000));
                }


                longestSeg = longestSeg.Rotate(longestSeg.Center, Math.PI / 2);

                Polygon2d wallSpaceClipper = longestSeg.ToRenct2d(10.0.MilliMeterToFeet());// 这里有个参数需要调整，分割图形使用的裁剪宽度

                return polygon2d.DifferenceClipper(wallSpaceClipper).ToList();
            }
            else
            {
                return new List<Polygon2d>();
            }
        }
        /// <summary>
        /// 获取原始储藏空间
        /// </summary>
        /// <returns></returns>
        Polygon2d GetStoreage(FilledRegion filledRegion)
        {
            IEnumerable<Curve> _curves = filledRegion.GetBoundaries().First().ToCurves();
            List<XYZ> points = new List<XYZ>();
            foreach (var c in _curves)
            {
                if (c is Line)
                {
                    points.Add(c.GetEndPoint(0));
                    points.Add(c.GetEndPoint(1));
                }
                else if (c is Curve)
                {
                    points.AddRange(c.Tessellate());
                }
            }
            return new Polygon2d(points.ToVector2ds().DelDuplicate());
        }
        /// <summary>
        /// 获取道路空间
        /// </summary>
        /// <returns></returns>
        List<Polygon2d> WalkwaySpace(List<Curve> curveElements)
        {
            List<Polygon2d> result = new List<Polygon2d>();
            Paths polyLing = new Paths();
            foreach (var item in curveElements)
            {
                if (item is Line)
                {
                    polyLing.Add((item as Line).ToSegment2d().EndPoints().ToPath());
                }
                else
                {
                    List<Vector2d> ends = new List<Vector2d>() { item.GetEndPoint(0).ToVector2d(), item.GetEndPoint(1).ToVector2d() };
                    polyLing.Add(ends.ToPath());
                }
            }

            result = polyLing.Offset(GlobalData.Instance.WalkwayWidth_num / 2, Precision_.ClipperMultiple, EndType.etOpenSquare).ToPolygon2ds().ToList();// 这里的道路空间-末端为直角，并且向路径外延伸一部分，与地库道路略有不同


            return result;
        }

       
    }
}
