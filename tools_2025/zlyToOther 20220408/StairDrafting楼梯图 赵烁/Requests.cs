using System.Collections;
using System.Threading;

namespace StairDrafting
{
    public enum RequestId : int
    {
        None,
        close,

        drawStructTreadInPlan,
    }

    public class Request
    {
        private Queue m_requests;
        internal Request()
        {
            var q = new Queue();
            m_requests = Queue.Synchronized(q);
        }
        public RequestId Peek()
        {
            if (m_requests.Count > 0)
                return (RequestId)m_requests.Peek();
            else
                return RequestId.None;
        }
        public RequestId Take()
        {
            if (m_requests.Count > 0)
                return (RequestId)m_requests.Dequeue();
            else
                return RequestId.None;
        }
        public void Make(RequestId request)
        {
            m_requests.Enqueue(request);
        }
    }
}
