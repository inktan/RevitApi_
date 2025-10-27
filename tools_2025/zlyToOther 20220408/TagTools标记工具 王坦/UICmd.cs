using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using goa.Common.Exceptions;

namespace TagTools
{
    public static class UICmd
    {
        public static void SpaceOutSelectedTags()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new ElementClassSelectionFilter<IndependentTag>();
            string m = "选择一个标记。将查找重叠的标记，并自动偏移避让。";
            Form_cursorPrompt.Start(m, goa.Common.APP.RevitWindow);
            var pick = sel.PickObject(ObjectType.Element, filter, m);
            Form_cursorPrompt.Stop();
            var tag = doc.GetElement(pick) as IndependentTag;
            var view = doc.GetElement(tag.OwnerViewId) as View;
            var plane = view.GetViewCutPlane();
            if (plane == null)
                throw new CommonUserExceptions("不支持的视图类型。");
            var thisTag = new TagInfo(tag, view, plane);
            var overlap = TagInfo.GetOverlappingTags(thisTag);
            if (overlap.Count == 1) //self
                return;
            using (Transaction trans = new Transaction(doc, "选择标记自动避让"))
            {
                trans.Start();
                TagInfo.AutoSpaceOverlappingTags(overlap);
                trans.Commit();
            }
        }

        public static void FindOverlap()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var view = uidoc.ActiveView;
            var plane = view.GetViewCutPlane();
            if (plane == null)
                throw new CommonUserExceptions("不支持的视图类型。");
            var allTags = new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(IndependentTag))
                .Cast<IndependentTag>()
                .Select(x => new TagInfo(x, view, plane))
                .ToList();
            var overlap = allTags.FirstOrDefault(x => TagInfo.GetOverlappingTags(x, allTags).Count > 1);
            if (overlap == null)
                throw new CommonUserExceptions("未找到重叠标记。");
            else
            {
                uidoc.ShowElements(overlap.Tag);
            }
        }

        public static void SpaceOutAllTagsInView()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var view = uidoc.ActiveView;
            var plane = view.GetViewCutPlane();
            if (plane == null)
                throw new CommonUserExceptions("不支持的视图类型。");
            var allTags = new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .OfClass(typeof(IndependentTag))
                .Cast<IndependentTag>()
                .Select(x => new TagInfo(x, view, plane))
                .ToList();
            var overlaps = new List<List<TagInfo>>();
            foreach (var t in allTags)
            {
                var overlap = TagInfo.GetOverlappingTags(t, allTags);
                if (overlap.Count > 1)
                    overlaps.Add(overlap);
            }
            using (Transaction trans = new Transaction(doc, "自动避让视图中重叠标记"))
            {
                trans.Start();
                foreach (var overlap in overlaps)
                    TagInfo.AutoSpaceOverlappingTags(overlap);
                trans.Commit();
            }
        }

        internal static void ShowHost()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new ElementClassSelectionFilter<IndependentTag>();
            string m = "选择一个标记。将显示它的主体。";
            Form_cursorPrompt.Start(m, goa.Common.APP.RevitWindow);
            var pick = sel.PickObject(ObjectType.Element, filter, m);
            Form_cursorPrompt.Stop();
            var tag = doc.GetElement(pick) as IndependentTag;
            var host = tag.GetTaggedLocalElement();
            if (host == null)
                throw new CommonUserExceptions("未找到主体图元。");
            else
            {
                uidoc.ShowElements(host);
                sel.SetElementIds(new List<ElementId>() { host.Id });
            }
        }

        internal static void MoveTowardHost()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new ElementClassSelectionFilter<IndependentTag>();
            string m = "选择一个标记。";
            Form_cursorPrompt.Start(m, goa.Common.APP.RevitWindow);
            var pick = sel.PickObject(ObjectType.Element, filter, m);
            Form_cursorPrompt.Stop();
            var tag = doc.GetElement(pick) as IndependentTag;
            var view = doc.GetElement(tag.OwnerViewId) as View;
            var plane = view.GetViewCutPlane();
            if (plane == null)
                throw new CommonUserExceptions("不支持的视图类型。");

            var dist = UIInput.DistMoveTowardHost;

            using (TransactionGroup tg = new TransactionGroup(doc, "贴合主体"))
            {
                tg.Start();

                using (Transaction trans = new Transaction(doc, "贴合主体"))
                {
                    trans.Start();
                    moveTowardHost(tag, view, plane, dist);
                    trans.Commit();
                }
                //change all?
                if (UIInput.PromptChangeAll == false)
                {
                    tg.Assimilate();
                    return;
                }
                var result = UserMessages.ShowYesNoDialog("是否要修改视图中所有相同标记？");
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    tg.Assimilate();
                    return;
                }

                var typeId = tag.GetTypeId();
                var allOthers = new FilteredElementCollector(doc, view.Id)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(IndependentTag))
                    .Where(x => x.GetTypeId() == typeId)
                    .Where(x => x.Id != tag.Id)
                    .Cast<IndependentTag>()
                    .ToList();

                using (Transaction trans = new Transaction(doc, "贴合主体"))
                {
                    trans.Start();
                    foreach (var other in allOthers)
                        moveTowardHost(other, view, plane, dist);
                    trans.Commit();
                }

                tg.Assimilate();
            }
        }

        internal static void TagsAtPosRelativeToHost()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new ElementClassSelectionFilter<IndependentTag>();
            string m = "选择一个标记。";
            Form_cursorPrompt.Start(m, goa.Common.APP.RevitWindow);
            var pick = sel.PickObject(ObjectType.Element, filter, m);
            Form_cursorPrompt.Stop();
            var tag = doc.GetElement(pick) as IndependentTag;
            var view = doc.GetElement(tag.OwnerViewId) as View;
            var plane = view.GetViewCutPlane();
            if (plane == null)
                throw new CommonUserExceptions("不支持的视图类型。");
            var dist = UIInput.DistAtPosToHost;
            var posToHost = UIInput.PosToHost;

            using (TransactionGroup tg = new TransactionGroup(doc, "移动标记到相对主体位置"))
            {
                tg.Start();

                using (Transaction trans = new Transaction(doc, "移动单个标记"))
                {
                    trans.Start();
                    tagPosRelativeToHost(tag, view, plane, posToHost, dist);
                    trans.Commit();
                }

                //change all?
                if (UIInput.PromptChangeAll == false)
                {
                    tg.Assimilate();
                    return;
                }
                var result = UserMessages.ShowYesNoDialog("是否要修改视图中所有相同标记？");
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    tg.Assimilate();
                    return;
                }

                var typeId = tag.GetTypeId();
                var allOthers = new FilteredElementCollector(doc, view.Id)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(IndependentTag))
                    .Where(x => x.GetTypeId() == typeId)
                    .Where(x => x.Id != tag.Id)
                    .Cast<IndependentTag>()
                    .ToList();

                using (Transaction trans = new Transaction(doc, "移动所有标记"))
                {
                    trans.Start();
                    foreach (var other in allOthers)
                        tagPosRelativeToHost(other, view, plane, posToHost, dist);
                    trans.Commit();
                }

                tg.Assimilate();
            }
        }

        private static void tagPosRelativeToHost
            (IndependentTag _tag, View _view, Plane _plane, 
            PosToHost _pos, double _dist)
        {
            var thisTag = new TagInfo(_tag, _view, _plane);
            var host = _tag.GetTaggedLocalElement();
            if (host == null)
                return;
            var hostSolids = host.GetAllSolids();
            if (hostSolids.Count == 0)
                return;
            var vertices = hostSolids.SelectMany(s => s.GetVertices()).ToList();
            thisTag.RightDir = thisTag.GetDirFromElem(thisTag.Tag);
            var dir = _view.ViewDirection.CrossProduct(thisTag.RightDir);
            if (_pos == PosToHost.下)
                dir = dir.Negate();
            var hostCentroid = vertices.GetBoundingBox().GetCentroid();
            XYZ vertex = null;
            double max = double.MinValue;
            foreach (var vert in vertices)
            {
                var d = dir.DotProduct(vert - hostCentroid);
                if (d > max)
                {
                    max = d;
                    vertex = vert;
                }
            }
            //get new pos from host's centroid
            XYZ newPos = null;
            if (UIInput.AlignToHostCentroid)
            {
                var vertexProj = (vertex - hostCentroid).DotProduct(dir) * dir + hostCentroid;
                newPos = vertexProj + dir * _dist;
            }
            else
            {
                var oldPosProj = (vertex - thisTag.Pos).DotProduct(dir) * dir + thisTag.Pos;
                newPos = oldPosProj + dir * _dist;
            }
            //move
            ElementTransformUtils.MoveElement(_tag.Document, _tag.Id, newPos - thisTag.Pos);
        }

        private static void moveTowardHost(IndependentTag _tag, View _view, Plane _plane, double _dist)
        {
            var thisTag = new TagInfo(_tag, _view, _plane);
            var host = _tag.GetTaggedLocalElement();
            if (host == null)
                return;
            var hostSolids = host.GetAllSolids();
            if (hostSolids.Count == 0)
                return;
            var vertices = hostSolids.SelectMany(s => s.GetVertices()).ToList();
            thisTag.RightDir = thisTag.GetDirFromElem(thisTag.Tag);
            var upDir = _view.ViewDirection.CrossProduct(thisTag.RightDir);
            XYZ vertex = null;
            double max = double.MinValue;
            foreach (var vert in vertices)
            {
                var d = Math.Abs(upDir.DotProduct(vert - thisTag.Pos));
                if (d > max)
                {
                    max = d;
                    vertex = vert;
                }
            }
            var v = vertex - thisTag.Pos;
            v = upDir * upDir.DotProduct(v);
            var moveV = _dist * v.Normalize();
            ElementTransformUtils.MoveElement(_tag.Document, _tag.Id, moveV);
        }

    }
}
