using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLittleCommands
{
    public enum RequestId : int
    {
        None,
        SelDieectionShpaesByRectangle,// 框选DirectShape
        SetSurfaceTransparency,// 设置表面透明度
        CommentElementId,// 解锁所有已锁定物体
        Test,// 普通测试

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
