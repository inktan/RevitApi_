using System.Threading;

namespace InfoStrucFormwork
{
    internal enum RequestId : int
    {
        None,
        ConcWall,
        ConcCol,
        ConcBeamUi,
        ConcBeamFollowFaceUi,
        ConcSingleBeamFollowFaceUi,
        ConcColFollowFaceUi,
        FloorDivision,
        AlignToBoardTop,
        AlignToBoardBottom,
        BbCoverFb,
        CalRelativeH,
        ClearStruAna,
        EleSeparate,

        StoreySteelBeamDoubleLine,
        RoofSteelBeamDoubleLine,

        StoreySteelBeamSingleLine,
        RoofStelBeamSingleLine,

        StoreyFloor,
        StoreyFloorAux,
        BandingWhole,
        BandingCombination,
        BandingSel,

        SlopeRoofBeam,

        Test01,
        Test02,
        Test03,
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
