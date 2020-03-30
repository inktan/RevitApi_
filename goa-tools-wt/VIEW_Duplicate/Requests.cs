using System.Threading;

namespace VIEW_Duplicate
{
    public enum RequestId : int
    {
        None = 0,
        Duplicate = 1,
        AsDependent = 2,
        WithDetailing = 3,

        UpdateVIews_List = 4
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
