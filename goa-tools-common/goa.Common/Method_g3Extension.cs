using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;

namespace goa.Common
{
    public static class Method_g3Extension
    {
        public static Segment3d OntoFrame(this Segment3d _seg3d, Frame3f _frame)
        {
            var seg3f = new Segment3f
                (_frame.ToFrameP((Vector3f)_seg3d.P0),
                _frame.ToFrameP((Vector3f)_seg3d.P1));
            return new Segment3d(seg3f.P0, seg3f.P1);
        }
        public static Segment2d IntoFrame(this Segment3d _seg3d, Frame3f _frame)
        {
            var seg2f = new Segment2f
                (_frame.ToPlaneUV((Vector3f)_seg3d.P0, 2),
                _frame.ToPlaneUV((Vector3f)_seg3d.P1, 2));
            return new Segment2d(seg2f.P0, seg2f.P1);
        }
        public static Segment3d OutFrame(this Segment2d _seg2d, Frame3f _frame)
        {
            var seg3f = new Segment3f
                (_frame.FromPlaneUV((Vector2f)_seg2d.P0, 2),
                _frame.FromPlaneUV((Vector2f)_seg2d.P1, 2));
            return new Segment3d(seg3f.P0, seg3f.P1);
        }
        public static Segment2d Project(this Segment2d _host, Segment2d _seg)
        {
            //project each end point
            double t0 = _host.Project(_seg.P0);
            double t1 = _host.Project(_seg.P1);
            t0 = MathUtil.Clamp(t0, -1.0 * _host.Extent, _host.Extent);
            t1 = MathUtil.Clamp(t1, -1.0 * _host.Extent, _host.Extent);
            if (t0.IsAlmostEqualByDifference(t1))
                return new Segment2d(Vector2d.Zero, Vector2d.Zero);
            var p0 = _host.PointAt(t0);
            var p1 = _host.PointAt(t1);
            return new Segment2d(p0, p1);
        }
        public static Segment3d Unproject(this Plane3d _plane, Segment2d _seg)
        {
            return new Segment3d(_plane.Evaluate(_seg.P0), _plane.Evaluate(_seg.P1));
        }
        public static Vector3f Rotate(this Vector3f _v, Vector3f _axis, float _deg)
        {
            Quaternionf ro = new Quaternionf(_axis, _deg);
            var trans = new TransformSequence();
            trans.AppendRotation(ro);
            return trans.TransformP(_v);
        }

        public static Polygon2d ToPolygon(this AxisAlignedBox2d _box)
        {
            var p0 = _box.Min;
            var p1 = new Vector2d(_box.Max.x, _box.Min.y);
            var p2 = _box.Max;
            var p3 = new Vector2d(_box.Min.x, _box.Max.y);
            return new Polygon2d(new List<Vector2d>() { p0, p1, p2, p3 });
        }
        /// <summary>
        /// Trim / split a segment by a polygon.
        /// </summary>
        /// <param name="_type">0 = inside; 1 = outside ; 2 = both sides</param>
        public static List<Segment2d> Trim(this Polygon2d _polygon, Segment2d _seg, int _type)
        {
            var list = new List<Segment2d>();
            //find intersections
            List<KeyValuePair<double, Vector2d>> intPs = new List<KeyValuePair<double, Vector2d>>();
            foreach (var edge in _polygon.SegmentItr())
            {
                var segIntr = new IntrSegment2Segment2(_seg, edge);
                bool b = segIntr.Find();
                if (b)
                {
                    intPs.Add(new KeyValuePair<double, Vector2d>(segIntr.Parameter0, segIntr.Point0));
                }
            }
            //if no intersection
            if (intPs.Count == 0)
            {
                if (_type == 1)
                {
                    //return empty list
                }
                else
                {
                    //check if segment is contained inside polygon
                    if (_polygon.Contains(_seg))
                        list.Add(_seg);
                }
                return list;
            }
            //sort intersection points by position
            var sortedIntPs = intPs.OrderBy(x => x.Key).Select(x => x.Value).ToList();

            //get two list of segments from intersections
            var list0 = new List<Segment2d>();
            var list1 = new List<Segment2d>() { new Segment2d(_seg.P0, sortedIntPs[0]) };
            for (int i = 1; i < sortedIntPs.Count; i += 2)
            {
                Vector2d p0, p1, p2;
                p0 = sortedIntPs[i - 1];
                p1 = sortedIntPs[i];
                list0.Add(new Segment2d(p0, p1));
                if (i + 1 < sortedIntPs.Count)
                {
                    p2 = sortedIntPs[i + 1];
                    list1.Add(new Segment2d(p1, p2));
                }
            }
            //use even/odd of intr num to decide the last segment
            var last = new Segment2d(sortedIntPs.Last(), _seg.P1);
            if (sortedIntPs.Count % 2 == 0)
                list1.Add(last);
            else
                list0.Add(last);

            //check starting point inside or outside
            bool startInside = _polygon.Contains(_seg.P0);
            //return coresponding list of segments
            if (_type == 0)
                return startInside ? list1 : list0;
            else if (_type == 1)
                return startInside ? list0 : list1;
            else
            {
                list.AddRange(list0);
                list.AddRange(list1);
                return list;
            }
        }
    }
}
