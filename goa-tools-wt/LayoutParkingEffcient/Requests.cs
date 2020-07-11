using System.Threading;

namespace LayoutParkingEffcient
{
    public enum RequestId : int
    {
        None,
        LayoutParking,
        SetControlRegionBoundary,
        CheckpolygonClosed,
        CheckLineStyle,
        CheckInGroupLineStyleIsSame,
        CheckTwoCurveCoincidence,
        DocumentChangedEventRegister,
        DocumentChangedEventUnRegister,
        GlobalRefresh,
        IntelligentRefresh,
        ChangeDirectionByRectange,
        ChangeDirectionByPoint,
        RefreshDataStatistics,
        HidenDirectShape,
        //OpenWidndowParameterSet ,
        //OpenWidndowMiscellaneousFunction ,
        //CloseOthersWindows ,
        SelunFixedParkingFs,
        SelFixedParkingFs,
        SelColumsFs,
        CutAlgorithm,
        ChangeDirectionByBoundary,
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
