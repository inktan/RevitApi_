using System.Threading;

namespace TransGroupParas
{
    public enum RequestId : int
    {
        None = 0,
        TransferGroupFimalyParas = 1,
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
