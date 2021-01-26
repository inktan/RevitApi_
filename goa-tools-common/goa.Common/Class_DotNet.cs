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
    public class RealNumDomain
    {
        public double Min, Max;
        public RealNumDomain(double _d1, double _d2)
        {
            this.Min = Math.Min(_d1, _d2);
            this.Max = Math.Max(_d1, _d2);
        }
        public bool Contains(double _d)
        {
            return this.Min <= _d && this.Max >= _d;
        }
        public override string ToString()
        {
            return this.Min.ToStringDigits(6) + "|" + this.Max.ToStringDigits(6);
        }
        public override bool Equals(object obj)
        {
            if (obj is RealNumDomain == false)
                return false;
            else
            {
                var other = obj as RealNumDomain;
                return this.ToString() == other.ToString();
            }
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
s