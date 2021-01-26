using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClipperLib;

namespace PublicProjectMethods_
{
    using cInt = Int64;
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    public static class Clipper_
    {
        /// <summary>
        /// 交集，AND (intersection) -主体和裁剪多边形相交的区域，即求交集
        /// </summary>
        public static Paths Intersection(this Paths subjs, Paths clips)
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
        public static Paths Union(this Paths subjs, Paths clips)
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
        /// <summary>
        /// 非/差，NOT (difference) - 求出 裁剪多边形 之外 主体的区域 
        /// </summary>
        /// <returns></returns>
        public static Paths Difference(this Paths subjs, Paths clips)
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
            PolyTree solution = new PolyTree();//得到可停车区域
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solution);
            // solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * Precision_.TheShortestDistance);
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
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
        public static PolyTree Offset_PolyTree(this Path _Path, double _offset, double magnification, EndType endType)
        {
            PolyTree solution = new PolyTree();
            ClipperOffset _Co = new ClipperOffset();
            _Co.MiterLimit = 3;
            _Co.AddPath(_Path, JoinType.jtMiter, endType);
            _Co.Execute(ref solution, _offset * magnification);
            //solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * (0.003));
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        } /// <summary>
          /// 线两侧偏移后为矩形，偏移距离须与点的放大倍数一致
          /// </summary>
        public static Paths Offset(this Path _Path, double _offset, double magnification, EndType endType)
        {
            Paths solution = new Paths();
            ClipperOffset _Co = new ClipperOffset();
            _Co.MiterLimit = 3;
            _Co.AddPath(_Path, JoinType.jtMiter, endType);
            _Co.Execute(ref solution, _offset * magnification);
            //solution = Clipper.CleanPolygons(solution, Precision_.clipperMultiple * (0.003));
            //solution = Clipper.SimplifyPolygons(solution);
            return solution;
        }
    }
}
