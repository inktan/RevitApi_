using Autodesk.Revit.UI;
using goa.Common;
using System.Threading;

namespace NumberingTool
{
    internal static class RequestMaker
    {
        private static RequestHandler m_Handler;
        private static ExternalEvent m_ExEvent;
        /// <summary>
        /// need to run within API context
        /// </summary>
        internal static void Init()
        {
            if (m_Handler == null)
            {
                m_Handler = new RequestHandler();
                m_ExEvent = ExternalEvent.Create(m_Handler);
            }
        }
        internal static void makeRequest(RequestId request)
        {
            AssemblyLoader.Register(true);
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
        }
    }
}
