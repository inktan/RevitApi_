using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    /// <summary>
    /// base class for all creators
    /// </summary>
    public class ElementCreator : ElementOperator
    {
        public ElementId RecreatedFrom = ElementId.InvalidElementId; //record for re-creation
    }
}
