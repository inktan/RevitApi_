using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common
{
    public class Exception_UserInputInvalidFormat : Exception
    {
        public override string Message
        {
            get
            {
                return "输入条件格式有误。请检查。";
            }
        }
    }
}
