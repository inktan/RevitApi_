using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace ThreeDViewNavi
{
    internal class ViewNaviHistory
    {
        internal Document Doc;
        internal ElementId ViewId;
        internal List<ViewOrientation3D> History = new List<ViewOrientation3D>();

        internal ViewNaviHistory(Document _doc, ElementId _viewId)
        {
            this.Doc = _doc;
            this.ViewId = _viewId;
        }

        internal void Save(ViewOrientation3D _vo)
        {
            this.History.Add(_vo);
            if (this.History.Count > 199)
                this.History.RemoveAt(0);
        }

        internal ViewOrientation3D Reverse()
        {
            if (this.History.Count == 0)
                return null;
            var vo = this.History.Last();
            this.History.RemoveAt(this.History.Count - 1);
            return vo;
        }
    }
}
