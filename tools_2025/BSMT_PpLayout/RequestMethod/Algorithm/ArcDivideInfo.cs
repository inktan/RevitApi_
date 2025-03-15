using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using goa.Common;

namespace PLAN_DSGN_TOOLS
{
    internal class ArcDivideInfo
    {
        private static readonly double epsilonRatio = 3.0;

        internal Arc Arc;
        internal bool OpenArc;
        internal double EpsilonDist { get { return this.divideDist / epsilonRatio; } }//equality threshhold
        internal List<PointOnArc> Points;
        internal List<Line> Segments;
        internal List<PointOnArc> IntrPoints;

        private Guid uid = Guid.NewGuid();
        private double divideDist;
        internal ArcDivideInfo(Arc _arc, IEnumerable<Curve> _context)
        {
            this.Arc = _arc;
            this.divideDist = GetDivideDist(_arc);
            this.Points = divideArcByDist(_arc, this.divideDist, out this.OpenArc);
            this.Segments = this.Points.Select(x => x.Pos).ToList().ToLines(this.OpenArc);
            this.IntrPoints = getIntrPoints(_arc, _context);
        }

        internal List<PointOnArc> GetInternalPoints()
        {
            var points = this.Points.ToList();
            points.RemoveAt(0);
            if (this.OpenArc)
                points.RemoveAt(points.Count - 1);
            return points;
        }

        internal static readonly double minDivideDist = 0.123123; //37.5mm
        private static readonly double maxNumSeg = 32;
        internal static double GetDivideDist(Arc _arc)
        {
            var num = _arc.Length / minDivideDist;
            if (num > maxNumSeg)
            {
                return num / maxNumSeg * minDivideDist;
            }
            else
            {
                return minDivideDist;
            }
        }

        internal ArcDivideInfo[] CreateOffsets(double _dist)
        {
            var refV = this.Arc.XDirection.CrossProduct(this.Arc.YDirection);
            var offset0 = this.Arc.CreateOffset(_dist, refV) as Arc;
            var offset1 = this.Arc.CreateOffset(_dist, -refV) as Arc;
            var newInfo0 = new ArcDivideInfo(offset0, new List<Curve>());
            newInfo0.IntrPoints = projectPointsOntoArc(this.IntrPoints, newInfo0);
            newInfo0.Points = projectPointsOntoArc(this.Points, newInfo0);
            var newInfo1 = new ArcDivideInfo(offset1, new List<Curve>());
            newInfo1.IntrPoints = projectPointsOntoArc(this.IntrPoints, newInfo1);
            newInfo1.Points = projectPointsOntoArc(this.Points, newInfo1);
            return new ArcDivideInfo[2] { newInfo0, newInfo1 };
        }
        private List<PointOnArc> projectPointsOntoArc(List<PointOnArc> _points, ArcDivideInfo _info)
        {
            var list = new List<PointOnArc>();
            foreach (var p in _points)
            {
                var proj = _info.Arc.Project(p.Pos).XYZPoint;
                var newP = new PointOnArc(_info, proj);
                list.Add(newP);
            }
            return list;
        }
        private List<PointOnArc> divideArcByDist(Arc _arc, double _dist, out bool _open)
        {
            var currP = _arc.IsBound
                ? _arc.GetEndPoint(0)
                : _arc.Tessellate().First();
            var prevP = currP;
            var list = new List<XYZ>() { currP };
            var end = _arc.IsBound
                ? _arc.GetEndPoint(1)
                : currP;
            var count = Math.Ceiling(_arc.Length / _dist);

            for (int i = 0; i < count; i++)
            {
                var intrCircle = Arc.Create(currP, _dist, 0.0, MathUtils.FullCircle, XYZ.BasisX, XYZ.BasisY);
                IntersectionResultArray ra;
                var result = intrCircle.Intersect(_arc, out ra);
                if (result != SetComparisonResult.Overlap)
                    break;
                var intrR = ra.Cast<Autodesk.Revit.DB.IntersectionResult>()
                    .FirstOrDefault(x =>
                    x.XYZPoint.IsAlmostEqualToByDifference(prevP) == false
                    && x.XYZPoint.IsAlmostEqualToByDifference(end) == false);
                if (intrR == null)
                    break;
                else
                {
                    prevP = currP;
                    currP = intrR.XYZPoint;
                    list.Add(currP);
                }
            }

            _open = _arc.IsBound;
            if (_open)
            {
                if (list.Count > 1)
                    list.RemoveAt(list.Count - 1);
                list.Add(end);
            }

            return list.Select(x => new PointOnArc(this, x)).ToList();
        }
        private List<PointOnArc> getIntrPoints(Arc _arc, IEnumerable<Curve> _curves)
        {
            var list = new List<PointOnArc>();
            foreach (var c in _curves)
            {
                if (c.Equals(_arc))
                    continue;
                IntersectionResultArray ra;
                var result = c.Intersect(_arc, out ra);
                if (result == SetComparisonResult.Overlap)
                {
                    var points = ra.Cast<IntersectionResult>()
                        .Select(x => x.XYZPoint)
                        .Select(x => new PointOnArc(this, x));
                    list.AddRange(points);
                }

            }
            return list;
        }

        public override bool Equals(object obj)
        {
            if (obj is ArcDivideInfo == false)
                return false;
            else
            {
                var other = obj as ArcDivideInfo;
                return this.uid == other.uid;
            }
        }
        public override int GetHashCode()
        {
            return this.uid.GetHashCode();
        }
    }
}
