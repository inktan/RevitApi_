using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class ElementRemover : ElementOperator
    {
        public ElementRemover(Element _elem, OpContext _docOpsInfo = null)
        {
            this.Elem = _elem;
            this.Context = _docOpsInfo;
        }
        public override void Execute()
        {
            if (this.Elem.IsValidObject)
                Elem.Document.Delete(this.Elem.Id);
            this.OperationFinished = true;
        }
    }
}
