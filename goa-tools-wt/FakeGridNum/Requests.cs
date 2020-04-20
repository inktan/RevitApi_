using System.Threading;

namespace FakeGridNum
{
    public enum RequestId : int
    {
        None = 0,
        GridReName = 1,
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
