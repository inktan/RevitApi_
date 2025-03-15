using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClipperLib;
using g3;

namespace PubFuncWt
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    public static class Clipper_
    {
        /// <summary>
        /// 交集，AND (intersection) -主体和裁剪多边形相交的区域，即求交集
        /// </summary>
        public static Paths IntersectionClip(this Paths subjs, Paths clips)
        {
            Paths solution = new Paths();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctIntersection, solution);
            // CleanPolygons函数效果不理想，需写函数进行处理
            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
        /// <summary>
        ///并集，OR (union) - 主体和裁剪多边形两者合并的区域，即求并集
        /// </summary>
        public static Paths UnionClip(this Paths subjs, Paths clips)
        {
            Paths solution = new Paths();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            //solution = Clipper.SimplifyPolygons(solution);

            return solution;
        }
        public static Paths UnionClip(this Paths sub)
        {
            Paths cli = new Paths();
            if(sub.Count>1)
            {
                cli.Add(sub.First());
                sub.RemoveAt(0);
            }
            else
            {
                return sub;
            }

            Paths solution = new Paths();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(sub, PolyType.ptSubject, true);
            c.AddPaths(cli, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            //solution = Clipper.SimplifyPolygons(solution);

            return solution;
        }
        /// <summary>
        /// 非/差，NOT (difference) - 求出 裁剪多边形 之外 主体的区域 
        /// </summary>
        /// <returns></returns>
        public static Paths DifferenceClip(this Paths subjs, Paths clips)
        {
            Paths solution = new Paths();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solution);
            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
        /// <summary>
        /// 非/差，NOT (difference) - 求出 裁剪多边形 之外 主体的区域 
        /// </summary>
        /// <returns></returns>
        public static PolyTree Difference_PolyTree(this Paths subjs, Paths clips)
        {
            PolyTree solution =new PolyTree();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solution);
            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
        /// <summary>
        /// 提取图形裁剪后的最外层环状区域 针对情况：地库被道路空间裁剪后，存在唯一的最外层环状空间
        /// </summary>
        /// <returns></returns>
        public static List<Polygon2d> ExtractPolyTreeRing(this PolyTree polyTree)
        {
            List<Polygon2d> polygon2ds = new List<Polygon2d>();
            polyTree.ExtractPolyTreeRing(ref polygon2ds);
            return polygon2ds;
        }
        /// <summary>
        /// 只取出停车区域被道路空间裁剪后的子空间，不包含最层环状区域
        /// </summary>
        /// <returns></returns>
        public static Paths ExtractPolyTreeChildren(this PolyTree polyTree)
        {
            List<Polygon2d> polygon2ds = new List<Polygon2d>();
            return polyTree.ExtractPolyTreeRing(ref polygon2ds);
        }
        /// <summary>
        /// 当前适用情况为 图形裁剪后的最外圈是环状——对应情况比较单一
        /// </summary>
        /// <returns></returns>
        public static Paths ExtractPolyTreeRing(this PolyTree polyTree, ref List<Polygon2d> polygon2ds)
        {
            Path intPoints = new Path();
            if (polyTree.ChildCount == 1)//clipper裁剪结果存在最外层线圈
            {
                PolyNode polyNode01 = polyTree.GetFirst();// 最外层线圈 01
                if (polyNode01.ChildCount == 1)
                {
                    PolyNode polyNode02 = polyNode01.Childs[0];// 次外层线圈 02

                    polygon2ds.Add(polyNode01.Contour.ToPolygon2d());
                    polygon2ds.Add(polyNode02.Contour.ToPolygon2d());

                    List<PolyNode> polyNodes = polyNode02.Childs;
                    return polyNode02.Childs.Select(p => p.Contour).ToList();
                }
                else
                    return new Paths() { polyNode01.Contour } ;// 舍弃最外层单线圈
            }
            else if (polyTree.ChildCount > 1)
                return polyTree.Childs.Select(p => p.Contour).ToList();

            return new Paths();
        }

        /// <summary>
        /// 异或，XOR (exclusive or) - 主体和裁剪多边形两者相交以外的区域  
        /// </summary>
        /// <returns></returns>
        public static Paths ExclusiveOr(this Paths subjs, Paths clips)
        {
            Paths solution = new Paths();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctXor, solution);
            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
        /// <summary>
        /// 线两侧偏移后为矩形，偏移距离须与点的放大倍数一致
        /// </summary>
        public static PolyTree Offset_PolyTree(this Path _Path, double _offset,double magnification ,EndType endType = EndType.etOpenButt)
        {
            PolyTree solution = new PolyTree();
            ClipperOffset _Co = new ClipperOffset();
            _Co.MiterLimit = 3;
            _Co.AddPath(_Path, JoinType.jtMiter, endType);
            _Co.Execute(ref solution, (_offset + Precision_.Precison) * magnification);
            //solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * (0.003));
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
        /// <summary>
        /// 单根线段的端点点集进行偏移，在后台，默认为偏移距离，增加一个缓冲值，以保证后续，能够顺利将图形裁剪干净
        /// </summary>
        public static Paths Offset(this Path _Path, double _offset, double magnification, EndType endType = EndType.etOpenButt)
        {
            Paths solution = new Paths();
            ClipperOffset _Co = new ClipperOffset();
            _Co.MiterLimit = 3;
            _Co.AddPath(_Path, JoinType.jtMiter, endType);
            _Co.Execute(ref solution, (_offset + Precision_.Precison) * magnification);
            //solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * (0.003));
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
        /// <summary>
        /// 将多个线段端点组成的点集进行偏移，在后台，默认为偏移距离，增加一个缓冲值，以保证后续，能够顺利将图形裁剪干净
        /// </summary>
        public static Paths Offset(this Paths _Paths, double _offset, double magnification, EndType endType = EndType.etOpenButt)
        {
            Paths solution = new Paths();
            ClipperOffset _Co = new ClipperOffset();
            _Co.MiterLimit = 3;
            _Co.AddPaths(_Paths, JoinType.jtMiter, endType);
            _Co.Execute(ref solution, (_offset + Precision_.Precison) * magnification);
            //solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * (0.003));
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
    }
}
