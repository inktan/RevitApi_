using System.Threading;

namespace TOPO_ANLS
{
    public enum RequestId : int
    {
        None,
        SlopeAnalysis,
        ElevationAnalysis,
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
