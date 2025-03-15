using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using QuadTrees.QTreeRectF;

namespace BSMT_PpLayout
{
    public class QTreeRevitEleCtrl : IRectFQuadStorable
    {    

        private RectangleF _rect;
        private RevitEleCtrl _revitEleCtrl;

        public RectangleF Rect => this._rect;
        internal RevitEleCtrl RevitEleCtrl => this._revitEleCtrl;

        internal QTreeRevitEleCtrl(RectangleF rect, RevitEleCtrl revitEleCtrl)
        {
            _rect = rect;
            _revitEleCtrl = revitEleCtrl;
        }
    }
}
