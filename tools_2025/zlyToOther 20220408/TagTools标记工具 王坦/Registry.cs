using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTools
{
    public static class Registry
    {
        internal static bool registered = false;
        public static void Register(bool _register)
        {
            if (_register == registered)
                return;

            registered = _register;
        }
    }
}
