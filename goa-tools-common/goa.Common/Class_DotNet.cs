using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    /// <summary>
    /// Wrapper class for converting IntPtr to IWin32Window.
    /// </summary>
    public class WindowHandle : IWin32Window
    {
        IntPtr _hwnd;

        public WindowHandle(IntPtr h)
        {
            Debug.Assert(IntPtr.Zero != h,
              "expected non-null window handle");

            _hwnd = h;
        }

        public IntPtr Handle
        {
            get
            {
                return _hwnd;
            }
        }
    }
    public sealed class MyBinaryFormatterBinder
        : System.Runtime.Serialization.SerializationBinder
    {
        public override System.Type BindToType(
          string assemblyName,
          string typeName)
        {
            return Type.GetType(string.Format("{0}, {1}",
              typeName, assemblyName));
        }
    }
}
