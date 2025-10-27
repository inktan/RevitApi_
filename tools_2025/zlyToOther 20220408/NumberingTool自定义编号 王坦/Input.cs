using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberingTool
{
    internal class Input
    {
        internal string ParamName, Prefix, Surfix;
        internal Input(string pn, string prefix, string surfix)
        {
            this.ParamName = pn;
            this.Prefix = prefix;
            this.Surfix = surfix;
        }
    }
}
