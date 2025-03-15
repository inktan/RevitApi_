using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

using ClipperLib;
//using goa.Common.g3InterOp;
using goa.Common;
using g3;
using PubFuncWt;
using System.Diagnostics;
using QuadTrees;
using System.Drawing;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 该类构造时，已经基于outline进行了图元清洗
    /// 基于polygon进行二次图元清洗
    /// </summary>
    class Bsmt
    {
        internal Document Doc => this.BsmtBound.Doc;
        // 地库外墙边界
        internal BsmtBound BsmtBound { get; set; }
        internal Polygon2d Polygon2dInward { get; set; }
        // 地库边界内所有元素
        internal QuadTreeRectF<QTreeRevitEleCtrl> Qtree { get; }
        internal List<QTreeRevitEleCtrl> QTreeRevitEleCtrls { get; set; }
        internal Bsmt(List<QTreeRevitEleCtrl> _qTreeRevitEles, BsmtBound _bsmtBoundary)
        {
            this.QTreeRevitEleCtrls = _qTreeRevitEles.Where(p => p.RevitEleCtrl.Ele.IsValidObject).ToList(); 
            this.BsmtBound = _bsmtBoundary;

            this.Qtree = new QuadTreeRectF<QTreeRevitEleCtrl>();
            this.Qtree.AddRange(this.QTreeRevitEleCtrls);

            this.Polygon2dInward = this.BsmtBound.Polygon2dInward;

            // 默认 自动 提取现有路网
            this._inBoundEleLines = inBoundEleLines().ToList();
            //Computer();
        }
        internal Bsmt()
        { }

        internal void Computer_VeRa()
        {
            this._inBoundEleVeRas = inBoundEleVeRas().ToList();
        }
        internal void Computer_Ps_Fr_Col_SubExit_Area()
        {
            this._inBoundElePses = inBoundElePses().ToList();
            this._inBoundEleFRes = inBoundEleFRes().ToList();
            this._inBoundEleCols = inBoundEleCols().ToList();
            this._subPsAreaEexists = subPsAreaEexists().ToList();
        }
        internal DataCollection DataCollection = null;
        internal void CalStatistics()
        {
            this.DataCollection = new DataCollection();
            this.DataCollection.Calculate(this);
        }

        private List<RevitElePS> _inBoundElePses;
        internal List<RevitElePS> InBoundElePses { get { return _inBoundElePses; } }
        /// <summary>
        /// 绝对位于地库边界内的 车位 族实例
        /// </summary>
        private IEnumerable<RevitElePS> inBoundElePses()
        {
            foreach (var item in this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject))
            {
                if (item.RevitEleCtrl is RevitElePS)
                {
                    RevitElePS revitElePS = item.RevitEleCtrl as RevitElePS;
                    if (this.Polygon2dInward.Contains(revitElePS.LocVector2d))
                    {
                        yield return revitElePS;
                    }
                }
            }
        }
        /// <summary>
        /// 函数弃用 revitElePS.Polygon2d()过于耗时
        /// </summary>
        bool Contain(RevitElePS revitElePS)
        {
            if (this.Polygon2dInward.Contains(revitElePS.LocVector2d) == false)
                return true;

            Polygon2d o = revitElePS.Polygon2d();
            int N = o.VertexCount;
            for (int i = 0; i < N; ++i)
            {
                if (this.Polygon2dInward.Contains(o[i]) == false)
                    return false;
            }

            return false;
        }
        private List<BoundO> _inBoundEleFRes { get; set; }
        internal List<BoundO> InBoundEleFRes => this._inBoundEleFRes;
        /// <summary>
        /// 与地库边界相关的详图填充区域
        /// </summary>
        private IEnumerable<BoundO> inBoundEleFRes()
        {
            foreach (var item01 in this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleFR))
            {
                RevitEleFR revitEleFR = item01.RevitEleCtrl as RevitEleFR;
                foreach (var item02 in revitEleFR.BoundOs())
                {
                    if (this.Polygon2dInward.Contains(item02.polygon2d) || this.Polygon2dInward.Intersects(item02.polygon2d))
                        yield return item02;
                }
            }
        }
        private List<RevitEleCol> _inBoundEleCols { get; set; }
        internal List<RevitEleCol> InBoundEleCols => this._inBoundEleCols;
        private IEnumerable<RevitEleCol> inBoundEleCols()
        {
            foreach (var item01 in this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleCol))
            {
                RevitEleCol revitEleCol = item01.RevitEleCtrl as RevitEleCol;

                if (this.Polygon2dInward.Contains(revitEleCol.BoundO.polygon2d) || this.Polygon2dInward.Intersects(revitEleCol.BoundO.polygon2d))
                    yield return revitEleCol;
            }
        }
        private List<RevitEleVeRa> _inBoundEleVeRas { get; set; }
        internal List<RevitEleVeRa> InBoundEleVeRas => this._inBoundEleVeRas;
        private IEnumerable<RevitEleVeRa> inBoundEleVeRas()
        {
            foreach (var item01 in this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl is RevitEleVeRa))
            {
                RevitEleVeRa revitEleVeRa = item01.RevitEleCtrl as RevitEleVeRa;
                if (this.Polygon2dInward.Contains(revitEleVeRa.BoundO.polygon2d) || this.Polygon2dInward.Intersects(revitEleVeRa.BoundO.polygon2d))
                    yield return revitEleVeRa;
            }
        }


        private List<SubPsAreaExist> _subPsAreaEexists;
        internal List<SubPsAreaExist> SubPsAreaEexists { get { return _subPsAreaEexists; } }
        /// <summary>
        /// 当前存在的各个地库子区域
        /// </summary>
        private IEnumerable<SubPsAreaExist> subPsAreaEexists()
        {
            foreach (var item in this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl.EleProperty == EleProperty.SubPsAreaExit))
                yield return new SubPsAreaExist(item.RevitEleCtrl as RevitEleFR, this.Qtree.GetObjects(item.Rect));

        }
        private List<BoundSeg> _inBoundEleLines;
        internal List<BoundSeg> InBoundEleLines => this._inBoundEleLines;
        /// <summary>
        /// 获取所有详图填充区域——主、次、自定义通车道中心线
        /// </summary>
        private IEnumerable<BoundSeg> inBoundEleLines()
        {
            // 删除重叠道路
            List<RevitEleCurve> revitEleLines = this.QTreeRevitEleCtrls
                .Where(p => p.RevitEleCtrl.Ele.IsValidObject)
                .Where(p => p.RevitEleCtrl is RevitEleCurve)
                .Select(p => p.RevitEleCtrl as RevitEleCurve)
                .Where(p=>p.EleProperty != EleProperty.Circle)
                .ToList();

            List<BoundSeg> collection = new List<BoundSeg>();

            //List<BoundSeg> priSeg2ds = revitEleLines.Where(p => p.EleProperty == EleProperty.PriLane).Select(p => p.BoundSeg).ToList();
            //List<BoundSeg> secSeg2ds = revitEleLines.Where(p => p.EleProperty == EleProperty.SecLane).Select(p => p.BoundSeg).ToList();
            //List<BoundSeg> cusSeg2ds = revitEleLines.Where(p => p.EleProperty == EleProperty.CusLane).Select(p => p.BoundSeg).ToList();

            //collection.AddRange(priSeg2ds);
            //collection.AddRange(secSeg2ds);
            //collection.AddRange(cusSeg2ds);

            // 临时做处理，将曲线处理为多段线
            double columnDis = GlobalData.Instance.pSWidth_num * 3 + GlobalData.Instance.ColumnWidth_num + GlobalData.Instance.ColumnBurfferDistance_num;
            foreach (var item in revitEleLines)
            {
                if (item.Curve is Line)
                {
                    collection.Add(item.BoundSeg);
                }
                else if (item.Curve is Arc)
                {
                    Arc arc = item.Curve as Arc;

                    double radius = arc.Radius;

                    // 求出弧线分段的长度，会因半径不同而不同
                    // 现在以车道两侧小边为定位线
                    // 先求出
                    XYZ vectorNom = new XYZ(0, 0, 1);
                    if (arc.Normal.Z.EqualPrecision(1.0))
                    {
                        vectorNom = new XYZ(0, 0, -1);
                    }
                    try
                    {
                        Arc arc01 = arc.CreateOffset(GlobalData.Instance.Wd_pri_num / 2, vectorNom) as Arc;
                        double radius01 = arc01.Radius;
                        // 三角形相似原理，求分弧线的线段长度
                        double columnDis01 = columnDis * radius / radius01;
                        List<Line> lines = arc.DivideToLinesByCyclic(columnDis01, false);

                        foreach (var line in lines)
                        {
                            collection.Add(new BoundSeg(line.ToSegment2d(), item.EleProperty));
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //throw;
                    }
                }
            }

            //if (priSeg2ds.Count() > 1)
            //{
            //    priSeg2ds = priSeg2ds.Select(p => p.Segment2d).OverLapSeg2ds().Select(p => new BoundSeg(p, EleProperty.PriLane)).ToList();
            //    collection.AddRange(priSeg2ds);
            //}

            //if (secSeg2ds.Count() > 1)
            //{
            //    secSeg2ds = secSeg2ds.Select(p => p.Segment2d).OverLapSeg2ds().Select(p => new BoundSeg(p, EleProperty.SecLane)).ToList();
            //    collection.AddRange(secSeg2ds);
            //}

            //if (cusSeg2ds.Count() > 1)
            //{
            //    cusSeg2ds = cusSeg2ds.Select(p => p.Segment2d).OverLapSeg2ds().Select(p => new BoundSeg(p, EleProperty.CusLane)).ToList();
            //    collection.AddRange(cusSeg2ds);
            //}

            return collection;
        }
        internal List<RevitEleCurve> GetLines()
        {
            List<RevitEleCurve> revitEleLines = 
                this.QTreeRevitEleCtrls
                .Where(p => p.RevitEleCtrl.Ele.IsValidObject)
                .Where(p => p.RevitEleCtrl is RevitEleCurve)
                .Select(p => p.RevitEleCtrl as RevitEleCurve)
                .ToList();
            return revitEleLines;
        }

        /// <summary>
        /// 清除未锁定 车位 族实例
        /// </summary>
        internal void DelUnUsefulParkSpaces()
        {
            List<ElementId> unFixedPsIds = 
                this.QTreeRevitEleCtrls
                .Where(p => p.RevitEleCtrl.Ele.IsValidObject)
                .Where(p => p.RevitEleCtrl is RevitElePS)
                .Select(p => p.RevitEleCtrl as RevitElePS)
                .Where(p => !p.PinnedByTool)
                .Select(p => p.Id)
                .ToList();// 删除未被插件锁定的 车位 族实例

            using (Transaction transDelete = new Transaction(this.Doc, "DeleteUnUsefulEleIds"))
            {
                transDelete.Start();
                this.Doc.Delete(unFixedPsIds);// 删除未被插件锁定的 车位 族实例
                transDelete.Commit();
            }
        }
        /// <summary>
        /// 清除未锁定 车位 族实例
        /// </summary>
        internal void DelUnUsefulSubPsAreaExits()
        {
            List<ElementId> subPsAreaExitIds = this.QTreeRevitEleCtrls.Where(p => p.RevitEleCtrl.Ele.IsValidObject).Where(p => p.RevitEleCtrl.EleProperty == EleProperty.SubPsAreaExit).Select(p => p.RevitEleCtrl.Id).ToList();// 删除当前存在的 地库-子停车区域

            using (Transaction transDelete = new Transaction(this.Doc, "DeleteUnUsefulEleIds"))
            {
                transDelete.Start();
                this.Doc.Delete(subPsAreaExitIds);// 删除当前存在的 地库-子停车区域
                transDelete.Commit();
            }
        }
        /// <summary>
        /// 删除已有的柱子
        /// </summary>
        internal void DelUnUsefulPillar()
        {
            List<ElementId> elementIds = this.QTreeRevitEleCtrls
                .Where(p => p.RevitEleCtrl.Ele.IsValidObject)
                .Where(p => p.RevitEleCtrl.EleProperty == EleProperty.ColumnUnit)
                .Select(p => p.RevitEleCtrl as RevitEleCol)
                .Where(p => !p.PinnedByTool)
                .Select(p => p.Id)
                .ToList();// 删除所有柱子族实例

            if (elementIds.Count > 0)
            {
                using (var deleteTrans = new Transaction(this.Doc, "删除所有柱子族实例"))
                {
                    deleteTrans.Start();
                    this.Doc.Delete(elementIds);// 删除所有柱子族实例
                    deleteTrans.Commit();
                }
            }

            //return layoutRegion.SingleLayoutRegions;
        }
        /// <summary>
        /// 删除现有道路体系
        /// </summary>
        internal void DelRevitLines()
        {
            List<ElementId> elementIds = this.QTreeRevitEleCtrls
                .Where(p => p.RevitEleCtrl.Ele.IsValidObject)
                .Where(p => p.RevitEleCtrl is RevitEleCurve)
                .Select(p => p.RevitEleCtrl.Id)
                .ToList();// 删除所有道路中心线

            if (elementIds.Count > 0)
            {
                using (var deleteTrans = new Transaction(this.Doc, "删除所有柱子族实例"))
                {
                    deleteTrans.Start();
                    this.Doc.Delete(elementIds);// 删除所有柱子族实例
                    deleteTrans.Commit();
                }
            }

            //return layoutRegion.SingleLayoutRegions;
        }
        //private void ChangeSurfceTransparency()
        //{
        //    using (var creatFilledRegion = new Transaction(this.Doc, "creatFilledRegion"))
        //    {
        //        creatFilledRegion.Start();
        //        OverrideGraphicSettings ogs = new OverrideGraphicSettings();//设置投影线、截面线颜色
        //        ogs.SetSurfaceTransparency(100);
        //        this.BsmtBound.View.SetElementOverrides(this.BsmtBound.Id, ogs);

        //        creatFilledRegion.Commit();
        //    }
        //}

    }// class
}// ns
