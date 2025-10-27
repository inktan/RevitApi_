using System.Threading;

namespace AutoFillUpLevelHeightAnnotation
{
    public enum RequestId : int
    {
        None,
        Pick,
        AutoFillUp,
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
