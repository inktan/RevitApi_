using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using g3;
using ClipperLib;
using goa.Common;
using goa.Common.g3InterOp;
using PubFuncWt;

namespace BSMT_PpLayout
{
    using cInt = Int64;

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    /// <summary>
    /// 地区区域划分计算
    /// </summary>
    class BsmtClipByRoad
    {
        internal Document Doc => this.Bsmt.BsmtBound.Doc;
        internal Bsmt Bsmt { get; set; }
        internal PolyTree PolyTree { get; }
        internal List<SubParkArea> SubParkAreas { get; }
        internal OutWallRing OutWallRing { get; }// 最外层的环状区域 如果不存在环形道路，则不存在环状区域
        internal BsmtClipByRoad(Bsmt baseMent)
        {
            this.Bsmt = baseMent;
            this.PolyTree = polyTree();

            // 被道路空间切分出来的各个子停车区域，可能存在最外层环状的情况
            this.SubParkAreas = this.PolyTree.ExtractPolyTreeChildren().Select(p => p.ToPolygon2d()).Where(p => p.Area > 0).Select(p => new SubParkArea(p, this.Bsmt)).ToList();// 子区域需要包含支持排车位算法的所有图元

            List<Polygon2d> oes = this.PolyTree.ExtractPolyTreeRing();
            if (oes.Count == 2)
            {
                this.OutWallRing = new OutWallRing(this.PolyTree.ExtractPolyTreeRing(), this.Bsmt);
            }
        }

        /// <summary>
        /// 裁剪后得到的clipper树
        /// </summary>
        internal PolyTree polyTree()
        {
            Paths paths = new Paths() { this.Bsmt.Polygon2dInward.ToPath() };// 地库区域退墙厚后的范围
            return paths.Difference_PolyTree(RoadSpace());
        }
        /// <summary>
        /// 获取道路空间 -重构1延长-重构2尖角
        /// </summary>
        internal Paths RoadSpace()
        {
            Paths result = new Paths();
            Paths temp = new Paths();

            // 需要判断车道是否与坡道相连
            List<RevitEleVeRa> revitEleVeRas = this.Bsmt.InBoundEleVeRas;
            foreach (var p in this.Bsmt.InBoundEleLines)
            {
                bool isBreak = true;
                foreach (RevitEleVeRa revitEleVeRa in revitEleVeRas)
                {
                    Vector2d locV2d = revitEleVeRa.LocVector2d;
                    Vector2d direction = revitEleVeRa.UpDirection.ToVector2d();

                    Segment2d segment2d = new Segment2d(locV2d - direction * 5.0, locV2d + direction * 5.0);
                    double width = revitEleVeRa.FamilyInstance.LookupParameter("净宽").AsDouble();
                    double wallThick = revitEleVeRa.FamilyInstance.LookupParameter("墙厚").AsDouble();
                    if (segment2d.ToRenct2d(width / 2.0).Intersects(p.Segment2d))
                    {
                        // 1 与坡道相连的道路线的宽度与坡道宽度设置一致 取消
                        //temp = p.Segment2d.EndPoints().ToPath().Offset(width / 2 + wallThick, Precision_.ClipperMultiple, EndType.etOpenButt);// 末端为直角，且没有延伸
                        //result.AddRange(temp);

                        // 2 需要额外增加坡道空间，进行裁剪，处理坡道空间 起始直段坡道需要切割停车区域
                        double height = 0.0;
                        if (revitEleVeRa.EleProperty == EleProperty.VehicleRamp)
                        {
                            height = revitEleVeRa.FamilyInstance.LookupParameter("坡道底部可停车位置").AsDouble();// 坡道顶面标高距地面高度
                        }
                        else if (revitEleVeRa.EleProperty == EleProperty.VehicleRamp_Arc_A)
                        {
                        }
                        else if (revitEleVeRa.EleProperty == EleProperty.VehicleRamp_Arc_B)
                        {
                            height = revitEleVeRa.FamilyInstance.LookupParameter("起始直段长度").AsDouble();
                        }
                        else if (revitEleVeRa.EleProperty == EleProperty.VehicleRamp_UpDown)
                        {
                            height = revitEleVeRa.FamilyInstance.LookupParameter("总长").AsDouble();
                        }
                        Segment2d veRaSeg2d = new Segment2d(locV2d - direction * 5.0, locV2d + direction * height);// 这里有人为数据设置，需要后期debug
                        Paths paths = veRaSeg2d.EndPoints().ToPath().Offset(width / 2 + wallThick, Precision_.ClipperMultiple, EndType.etOpenButt);// 末端为直角，且没有延伸
                        result.AddRange(paths);

                        isBreak = false;
                        break;
                    }
                }
                if (isBreak)
                {

                }

                if (p.EleProperty == EleProperty.PriLane)
                {
                    temp = p.Segment2d.EndPoints().ToPath().Offset(GlobalData.Instance.Wd_pri_num / 2, Precision_.ClipperMultiple, EndType.etOpenButt);// 末端为直角，且没有延伸
                }
                else if (p.EleProperty == EleProperty.SecLane)
                {
                    temp = p.Segment2d.EndPoints().ToPath().Offset(GlobalData.Instance.Wd_sec_num / 2, Precision_.ClipperMultiple, EndType.etOpenButt);// 末端为直角，且没有延伸
                }
                else if (p.EleProperty == EleProperty.CusLane)
                {
                    temp = p.Segment2d.EndPoints().ToPath().Offset(GlobalData.Instance.Wd_CustomWidth_num / 2, Precision_.ClipperMultiple, EndType.etOpenButt);// 末端为直角，且没有延伸
                }
                result.AddRange(temp);
            }

            // 尖角补丁
            Paths sharpPatch =
                PatchSharpCorners(this.Bsmt.InBoundEleLines
                .Where(p => p.EleProperty == EleProperty.PriLane
                || p.EleProperty == EleProperty.SecLane
                || p.EleProperty == EleProperty.CusLane
                || p.EleProperty == EleProperty.Lane)
                .Select(p => p.Segment2d).ToList());

            sharpPatch = sharpPatch.Offset(GlobalData.Instance.Wd_pri_num / 2, Precision_.ClipperMultiple, EndType.etOpenButt);

            return result.UnionClip(sharpPatch);
        }
        /// <summary>
        /// 重构道路中心线——线段距离线圈一个缓冲距离时，延长该线段
        /// </summary>
        Segment2d RestructSeg_extend(Segment2d segment2d)
        {
            // 出现不知名 bug---需要像更好的策略

            Polygon2d polygon2d = this.Bsmt.BsmtBound.Polygon2dInward;
            double minDistance = 500.0.MilliMeterToFeet();// 判断线段端点是否落在线圈上，设定缓冲值
            List<Vector2d> vector2ds = polygon2d.FindInterSectionPoint(segment2d, minDistance);
            int count = vector2ds.Count;
            if (count == 1)
            {
                Vector2d vector2d = vector2ds.First();

                double disP0 = segment2d.P0.Distance(vector2d);
                double disP1 = segment2d.P1.Distance(vector2d);

                if (disP0 < disP1)
                {
                    Vector2d direction = vector2d - segment2d.P1;// 延长方向
                    return new Segment2d(segment2d.P1, vector2d + direction.Normalized * minDistance);
                }
                else
                {
                    Vector2d direction = vector2d - segment2d.P0;// 延长方向
                    return new Segment2d(segment2d.P0, vector2d + direction.Normalized * minDistance);
                }
            }
            else if (vector2ds.Count == 2)// 如果是同侧的两个点，要怎么判定
            {
                double distance = vector2ds[0].Distance(vector2ds[1]);

                if (distance <= minDistance)
                {
                    Vector2d center = segment2d.Center;
                    Vector2d vector2d = vector2ds.First();
                    double dis01 = center.Distance(vector2ds[0]);
                    double dis02 = center.Distance(vector2ds[1]);

                    if (dis01 > dis02)
                    {
                        vector2d = vector2ds[1];
                    }
                    else
                    {
                        vector2d = vector2ds[0];
                    }
                    double disP0 = segment2d.P0.Distance(vector2d);
                    double disP1 = segment2d.P1.Distance(vector2d);

                    if (disP0 < disP1)
                    {
                        Vector2d direction = vector2d - segment2d.P1;// 延长方向
                        return new Segment2d(segment2d.P1, vector2d + direction.Normalized * minDistance);
                    }
                    else
                    {
                        Vector2d direction = vector2d - segment2d.P0;// 延长方向
                        return new Segment2d(segment2d.P0, vector2d + direction.Normalized * minDistance);
                    }
                }

                Vector2d direct01 = vector2ds[0] - vector2ds[1]; ;// 求的延长方向
                Vector2d direct02 = vector2ds[1] - vector2ds[0]; ;// 求的延长方向
                return new Segment2d(vector2ds[0] + direct01.Normalized * minDistance, vector2ds[1] + direct02.Normalized * minDistance);
            }
            else
                return segment2d;
        }
        /// <summary>
        /// 重构道路中心线——两线段呈现夹角时，需要为其打补丁，补丁处理为主通车道宽度
        /// </summary>
        Paths PatchSharpCorners(List<Segment2d> segment2ds)
        {
            Paths paths = new Paths();

            List<Vector2d> vector2ds = new List<Vector2d>();
            segment2ds.ForEach(p =>
            {
                vector2ds.Add(p.P0);
                vector2ds.Add(p.P1);
            });

            foreach (Vector2d p in vector2ds.DelDuplicate())// 取出所有的不重合车道端点
            {
                List<Segment2d> _segment2ds = new List<Segment2d>();

                foreach (Segment2d seg2d in segment2ds)
                {
                    Vector2d p0 = seg2d.P0;
                    Vector2d p1 = seg2d.P1;

                    if (p.Distance(p0).EqualZreo())
                    {
                        _segment2ds.Add(seg2d);
                    }
                    else if (p.Distance(p1).EqualZreo())
                    {
                        _segment2ds.Add(seg2d);
                    }
                }

                int interSectCount = _segment2ds.Count;// 如果一个点端点找到两根车道，则说明多根线段呈现夹角形式
                if (interSectCount >= 2)// 需要判断超过2根的情况
                {
                    Segment2d segment2d01 = _segment2ds[0];
                    Segment2d segment2d02 = _segment2ds[1];
                    Vector2d direction01 = (segment2d01.Center - p).Normalized;
                    Vector2d direction02 = (segment2d02.Center - p).Normalized;

                    if (interSectCount > 2)
                    {
                        double angle = direction01.Dot(direction02);
                        for (int j = 0; j < interSectCount; j++)
                        {
                            for (int q = 0; q < interSectCount; q++)
                            {
                                if (q > j)
                                {
                                    Vector2d _direction01 = (_segment2ds[j].Center - p).Normalized;
                                    Vector2d _direction02 = (_segment2ds[q].Center - p).Normalized;

                                    double _angle = _direction01.Dot(_direction02);
                                    if (_angle < angle)// 找到最大夹角对应的两根车道线，进行打补丁
                                    {
                                        angle = _angle;
                                        direction01 = _direction01;
                                        direction02 = _direction02;
                                    }
                                }
                            }
                        }
                    }

                    Vector2d center01 = p + direction01 * (GlobalData.Instance.Wd_pri_num / 2);//【】转角只处理道路一半宽度的空间
                    Vector2d center02 = p + direction02 * (GlobalData.Instance.Wd_pri_num / 2);
                    List<Vector2d> path_Vector2D = new List<Vector2d>() { center01, p, center02 };

                    paths.Add(path_Vector2D.ToPath());
                }
            }

            return paths;
        }

        /// <summary>
        /// 创建子停车区域域
        /// </summary>
        internal void CreatFilledRegion()
        {
            View view = this.Bsmt.BsmtBound.View;
            string filledTypeName = "地库_子停车区域";
            try
            {
                // 外部环状区间
                if (this.OutWallRing != null && this.OutWallRing.Polygon2ds.Count == 2)
                {
                    view.CreatRingFilledRegoin(this.Doc, this.OutWallRing.Polygon2ds.ToCurveLoops().ToList(), filledTypeName, 0);
                }
            }
            catch (Exception)
            {
                "请检查车道中心线是否设置正确".TaskDialogErrorMessage();
                //throw new NotImplementedException("请检查车道中心线是否设置正确");
            }
            // 内部各个子区域
            List<Polygon2d> polygon2ds = this.SubParkAreas.Select(p => p.Polygon2d).ToList();

            foreach (var item in polygon2ds)
            {
                try
                {
                    view.CreatFilledRegoins(this.Doc, new List<CurveLoop>() { item.ToCurveLoop() }, filledTypeName, 0);
                }
                catch (Exception)
                {
                    "请检查车道中心线是否设置正确".TaskDialogErrorMessage();
                    //throw new NotImplementedException("请检查车道中心线是否设置正确");
                }
            }
        }

        /// <summary>
        /// 基于点选矩形，找到目标停车区域
        /// </summary>
        internal IEnumerable<SubParkArea> SubParkAreasByRec(Polygon2d polygon2d)
        {
            List<Polygon2d> polygon2ds = new List<Polygon2d>();
            Paths selPaths = new Paths() { polygon2d.VerticesItr(false).ToList().ToPath() };
            Paths tarPlacedRegions = this.SubParkAreas.Select(p => p.Polygon2d.ToPath()).ToList().IntersectionClip(selPaths);//得到裁剪后的可停车区域
            List<Polygon2d> _polygon2ds = tarPlacedRegions.ToPolygon2ds().ToList();

            List<ElementId> needDelUnfixedParkingIds = new List<ElementId>();
            foreach (Polygon2d o in _polygon2ds)
            {
                SubParkArea subParkArea = new SubParkArea(o, this.Bsmt);
                yield return subParkArea;
            }
        }
        /// <summary>
        /// 基于选择边界，找到目标停车区域
        /// </summary>
        internal IEnumerable<SubParkArea> SubParkAreasByBoundary(Segment2d seg)
        {
            Polygon2d o1 = Polygon2d.MakeRectangle(seg.Center, 1.0, 1.0);

            foreach (var item in this.SubParkAreas)
            {
                Polygon2d o2 = item.Polygon2d;
                if (o1.Intersects(o2))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 兜圈
        /// </summary>
        internal IEnumerable<SubParkArea> SubParkAreasByBacktrack(Polygon2d o)
        {
            foreach (var item in this.SubParkAreas)
            {
                Polygon2d polygon2d = item.Polygon2d;
                if (polygon2d.Contains(o) || polygon2d.Intersects(o))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 划线阵列
        /// </summary>
        internal IEnumerable<SubParkArea> SubParkAreasByLineArray(Segment2d seg)
        {

            double height = GlobalData.Instance.pSHeight_num;

            List<Polygon2d> _polygon2ds = new List<Polygon2d>();

            // 基于划线 偏移出一排车的空间
            Vector2d vecStart = seg.P0;
            Vector2d vecEnd = seg.P1;

            Vector2d pedl = vecStart.Rotate(vecEnd, -Math.PI / 2);
            Vector2d direction = (pedl - vecEnd).Normalized;
            Vector2d vecEnd01 = vecEnd + direction * height;
            Vector2d vecStart01 = vecStart + direction * height;
            Polygon2d rec = new Polygon2d(new List<Vector2d>() { vecStart, vecEnd, vecEnd01, vecStart01 });

            _polygon2ds.Add(rec);

            foreach (Polygon2d o in _polygon2ds)
            {

                SubParkArea subParkArea = new SubParkArea(o.InwardOffeet(Precision_.TheShortestDistance), this.Bsmt);
                yield return subParkArea;
            }

        }

    }
}
