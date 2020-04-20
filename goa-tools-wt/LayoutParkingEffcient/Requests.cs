using System.Threading;

namespace LayoutParkingEffcient
{
    public enum RequestId : int
    {
        None = 0,
        LayoutParking = 1,
        SelGarageBoundary = 2,
        CheckpolygonClosed = 3,
        CheckLineStyle = 4,
        CheckInGroupLineStyleIsSame = 5,
        TestOthers = 6,
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
