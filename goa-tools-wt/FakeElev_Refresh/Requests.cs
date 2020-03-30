using System.Threading;

namespace FakeElev_Refresh
{
    public enum RequestId : int
    {
        None = 0,
        FakeElev_Refresh_activeview = 1,
        FakeElev_Refresh_allviews = 2,
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
