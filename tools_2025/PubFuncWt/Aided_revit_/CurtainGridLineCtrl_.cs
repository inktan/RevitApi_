using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common;

namespace PubFuncWt
{
    public class CurtainGridLineCtrl_
    {
        internal List<bool> SegmentPattern = new List<bool>();// 网格线上竖挺放置情况
        internal Dictionary<string, Curve> SegmentCurves = new Dictionary<string, Curve>(); //key: start + end
        internal bool IsUGrid;
        internal Dictionary<string, Mullion> Mullions = new Dictionary<string, Mullion>();

        internal CurtainGridLine GridLine;

        internal CurtainGridLineCtrl_(CurtainGridLine _gl)
        {
            this.GridLine = _gl;
            this.IsUGrid = _gl.IsUGridLine;
        }
        /// <summary>
        /// 两根幕墙网格线的竖梃放置情况是否一致
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        internal bool IsSamePattern(CurtainGridLineCtrl_ _other)
        {
            for (int i = 0; i < this.SegmentPattern.Count; i++)
            {
                if (this.SegmentPattern[i] != _other.SegmentPattern[i])
                    return false;
            }
            return true;
        }

        internal void Match(CurtainGridLineCtrl_ _source)
        {
            var allSegments = this.SegmentCurves.Values.ToList();
            for (int i = 0; i < allSegments.Count; i++)
            {
                var yes0 = _source.SegmentPattern[i];
                var yes1 = this.SegmentPattern[i];
                if (yes0 != yes1)
                {
                    var c = allSegments[i];
                    if (yes0 == true)
                    {
                        this.GridLine.AddSegment(c);
                    }
                    else
                    {
                        this.GridLine.RemoveSegment(c);
                    }
                }
            }
        }

        internal void GetSegments()
        {
            var allSegments = this.GridLine.AllSegmentCurves.ToList();
            List<string> allSegmentKeys =
                allSegments
                .Cast<Curve>()
                .Select(x => getStartEndKey(x))
                .ToList();
            List<string> existingSegmentKeys =
                this.GridLine.ExistingSegmentCurves
                .Cast<Curve>()
                .Select(x => getStartEndKey(x))
                .ToList();
            for (int i = 0; i < allSegmentKeys.Count; i++)
            {
                this.SegmentPattern.Add
                    (existingSegmentKeys.Contains(allSegmentKeys[i]));
            }
            for (int i = 0; i < allSegments.Count; i++)
            {
                this.SegmentCurves[allSegmentKeys[i]] = allSegments[i];
            }
        }

        internal string GetKey()
        {
            return getStartEndKey(this.GridLine.FullCurve);
        }

        private string getStartEndKey(Curve _c)
        {
            return
            _c.GetEndPoint(0).ToStringDigits(2)
            + "||"
            + _c.GetEndPoint(1).ToStringDigits(2);
        }
    }
}
