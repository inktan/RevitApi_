using System.Threading;

namespace COLR_SWIT
{
    public enum RequestId : int
    {
        None = 0,
        ChangeRgbToBlack = 1,
        ChangeBlackToRgb = 2,
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
