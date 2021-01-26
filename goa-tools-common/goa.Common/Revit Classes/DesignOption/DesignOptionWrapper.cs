using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using System.DirectoryServices;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;

namespace goa.Common
{
    public class DesignOptionWrapper
    {
        public DesignOptionSet DesignOptionSet;
        public DesignOption DesignOption;
        public string LongName { get { return DesignOptionSet.Name + " : " + DesignOption.Name; } }
        public DesignOptionWrapper(DesignOptionSet _set, DesignOption _deOp)
        {
            this.DesignOptionSet = _set;
            this.DesignOption = _deOp;
        }
    }
}
