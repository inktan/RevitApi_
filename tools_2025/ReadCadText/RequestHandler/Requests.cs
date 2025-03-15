using System.Threading;

namespace ReadCadText
{
    internal enum RequestId : int
    {
        None,
        ExacText,
        RoomName,
        Test,
    }

    public class Request
    {
        private int m_request = (int)RequestId.None;
        internal RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        internal void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
