using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using goa.Common.Exceptions;

namespace goa.Common
{
    /// <summary>
    /// base class of all editor, creator and remover.
    /// </summary>
    public partial class ElementOperator
    {
        public virtual Element Elem { get; set; }

        public readonly Guid Guid = Guid.NewGuid(); //unique id
        public string TarDocId { get { return this.Context.TarDocId; } }
        public string RefDocId { get { return this.Context.RefDocId; } }
        public Document RefDoc { get { return this.Context.RefDoc; } }
        public Document TarDoc { get { return this.Context.TarDoc; } }
        public bool OperationFinished = false;
        public bool Cancel = false; //cancel flag
        public OperationCancelledCause CancelCause;
        public OpContext Context;
        public List<ElementId> Cuts = new List<ElementId>();
        public List<ElementId> CutBy = new List<ElementId>();
        public List<ElementId> Joins = new List<ElementId>();

        public virtual void PreProcess() { }
        public virtual void Execute() { }
        public virtual void PostProcess() { }

        protected void ThrowHostNotFound()
        {
            string refId = "";
            if (this.Context != null)
                refId = this.Context.RefElemId.ToString();
            throw new goa.Common.Exceptions.HostNotFoundException(refId);
        }
        protected void CutAndJoin()
        {
            if (this.Elem == null)
                return;
            Methods.SyncJoinCut(this.Elem, this.Cuts, this.CutBy, this.Joins);
        }
    }
}
