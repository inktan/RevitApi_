using System.Threading;

namespace VIEW_INTF
{
    public enum RequestId : int
    {
        None = 0,
        NewBuiltViews = 1,
        UpdateVIews = 2,
        UpdateVIews_List = 3
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
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
