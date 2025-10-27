using System.Threading;

namespace TagTools
{
    public enum RequestId : int
    {
        None,
        close,

        showHost,

        findOverlap,
        spaceOutSelectedTags,
        spaceOutAllTags,

        moveTowardHost,
        tagAtPosRelativeToHost,
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
