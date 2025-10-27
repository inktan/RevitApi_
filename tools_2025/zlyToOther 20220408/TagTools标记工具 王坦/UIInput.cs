using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTools
{
    internal enum PosToHost
    {
        上,
        下,
    }
    internal static class UIInput
    {
        internal static double DistMoveTowardHost, DistAtPosToHost;
        internal static PosToHost PosToHost;
        internal static bool AlignToHostCentroid = false;
        internal static bool PromptChangeAll = false;
    }
}
