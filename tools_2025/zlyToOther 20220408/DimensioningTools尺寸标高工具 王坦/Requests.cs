using System.Threading;

namespace DimensioningTools
{
    public enum RequestId : int
    {
        None,
        close,

        fakeDim,
        dimClosestPintInView,
        dimAvoid,

        windowSpotElevDim,
        multiFloorElev,
        multiElevFam,
        fakeSpotElevUpdate,
        absElev,

        multiLevelElev,
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
