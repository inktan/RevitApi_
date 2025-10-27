using System.Threading;

namespace DimensioningTools
{
    internal enum RequestId : int
    {
        None,
        ConvertCurrentView,
        ConvertMultiViews,
        ConvertSelected,
        FrameSelectFakeDims,

        DimClosestPointInView,
        DimAvoid,
        FakeDimAvoid,
        WindowSpotElevDim,

        PickElevationFamily,
        AutoFillUp,

        FakeElev_Refresh_allviews,
        FakeElev_Refresh_activeview,

        DimOnFloors,
        AbsElev,

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
