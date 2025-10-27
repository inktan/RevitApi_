using System.Threading;

namespace CollisionDetection
{
    internal enum RequestId : int
    {
        None,
        //框选
        IncludingLinkedModels,
        ExcludingLinkedModels,
        FrameSel_LinkedStruModels,//只判断链接的结构模型
        //全局
        IncludingLinkedModelsAll,
        ExcludingLinkedModelsAll,
        AllSel_LinkedStruModels,//只判断链接的结构模型

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
