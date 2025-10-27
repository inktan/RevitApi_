using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using goa.Common;

namespace TagTools
{
    internal class TagInfo
    {
        internal IndependentTag Tag;
        internal XYZ Pos { get { return this.Tag.TagHeadPosition; } }
        private Plane BasePlane;

        private BoundingBoxXYZ BB;
        private BoundingBoxUV BBUV;

        internal XYZ RightDir;
        private double size;
        private XYZ newPos;

        internal TagInfo(IndependentTag _tag, View _ownerView, Plane _basePlane)
        {
            this.Tag = _tag;
            this.BasePlane = _basePlane;
            this.BB = _tag.get_BoundingBox(_ownerView);
            this.BBUV = this.BB.GetVertices().Select(x => _basePlane.ProjectInto(x)).GetBoundingBox();
        }

        internal XYZ GetDirFromElem(IndependentTag _tag)
        {
            var doc = _tag.Document;
            var host = doc.GetElement(_tag.TaggedLocalElementId);
            if(host is FamilyInstance)
            {
                var fi = host as FamilyInstance;
                return fi.HandOrientation;
            }
            else
            {
                //try get dir from loc curve
                var lc = host.LocationCurve();
                if (lc == null)
                {
                    //get dir from owner view
                    var view = _tag.Document.GetElement(_tag.OwnerViewId) as View;
                    return view.RightDirection;
                }
                else
                    return lc.ComputeDerivatives(0.5, true).BasisX; //dir at center
            }
        }

        private double getSize(IndependentTag _tag, XYZ _dir)
        {
            var pos = _tag.TagHeadPosition;
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach(var v in this.BB.GetVertices())
            {
                double d = v.DotProduct(_dir);
                if (d < min)
                    min = d;
                if (d > max)
                    max = d;
            }
            return max - min;
        }

        internal bool Overlap(TagInfo _other)
        {
            return this.BBUV.Overlaps(_other.BBUV);
        }

        internal static List<TagInfo> GetOverlappingTags(TagInfo thisTag)
        {
            var doc = thisTag.Tag.Document;
            var view = doc.GetElement(thisTag.Tag.OwnerViewId) as View;
            //get all tags in view
            var allTags = new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(IndependentTag))
                .Cast<IndependentTag>()
                .Select(x => new TagInfo(x, view, thisTag.BasePlane))
                .ToList();
            return GetOverlappingTags(thisTag, allTags);
        }
        internal static List<TagInfo> GetOverlappingTags(TagInfo thisTag, List<TagInfo> allTags)
        {
            var overlap = new List<TagInfo>();
            //find overlaps
            foreach (var other in allTags)
            {
                if (thisTag.Overlap(other))
                    overlap.Add(other);
            }
            return overlap;
        }
        internal static void AutoSpaceOverlappingTags(List<TagInfo> _tags)
        {
            //get right dir and dim along it.
            foreach(var t in _tags)
            {
                t.RightDir = t.GetDirFromElem(t.Tag);
                t.size = t.getSize(t.Tag, t.RightDir);
            }
            var totalDim = _tags.Sum(x => x.size) + AppSetting.Spacing * (_tags.Count - 1);
            //get start pos
            var firstRightDir = _tags.First().RightDir;
            var newPos = getFirstTagPos(_tags, firstRightDir);
            _tags[0].newPos = newPos;
            //get pos for other tags
            for(int i = 1;i < _tags.Count;i++)
            {
                var thisTag = _tags[i];
                var previous = _tags[i - 1];
                thisTag.RightDir = firstRightDir.DotProduct(thisTag.RightDir) > 0.001
                    ? thisTag.RightDir
                    : thisTag.RightDir.Negate();
                newPos += thisTag.RightDir * (thisTag.size * 0.5 + AppSetting.Spacing + previous.size * 0.5);
                thisTag.newPos = newPos;
            }
            //move to new pos
            var doc = _tags.First().Tag.Document;
            foreach(var tag in _tags)
            {
                //get move vector along dir
                var v = tag.newPos - tag.Tag.TagHeadPosition;
                v = tag.RightDir * (v.DotProduct(tag.RightDir));
                ElementTransformUtils.MoveElement(doc, tag.Tag.Id, v);
            }
        }
        private static XYZ getFirstTagPos(List<TagInfo> _tags, XYZ _dir)
        {
            //get index of center tag
            int indexCenter = (int)(Math.Floor(_tags.Count * 0.5) - 1);
            XYZ centerPos = _tags[indexCenter].Tag.TagHeadPosition;
            if (_tags.Count % 2 == 0)
                centerPos -= _dir * (_tags[indexCenter].size * 0.5 + AppSetting.Spacing * 0.5);
            if (indexCenter == 0)
                return centerPos;
            XYZ pos = centerPos;
            for(int i = indexCenter;i>=1;i--)
            {
                var thisTag = _tags[i];
                var previous = _tags[i - 1];
                pos -= _dir * (thisTag.size * 0.5 + AppSetting.Spacing + previous.size * 0.5);
            }
            return pos;
        }
    }
}
