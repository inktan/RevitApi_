using System.Threading;

namespace WIND_NUMB_new
{
    public enum RequestId : int
    {
        None = 0,
        CalAllFiMarks = 1,
        CommandMarks = 2,
        CommandSingleMark = 3
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
