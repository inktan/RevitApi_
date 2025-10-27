using System.Threading;
using goa.Common;

namespace FAKE_DIMS
{
    public enum RequestId : int
    {
        None,
        test1,
        ConvertSingleView,
        ConvertMultiViews,
        ConvertSelected,
        Select,
        Check,
    }

    public class Request
    {
        private int m_request = (int)RequestId.None;
        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }
        public void Make(RequestId request)
        {
            AssemblyLoader.Register(true);
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
