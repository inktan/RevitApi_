using System;
using System.Threading;

namespace ModelessFormExternalEvent
{
    /// <summary>
    ///   A list of requests the dialog has available
    /// </summary>
    /// 
    public enum RequestId : int//为对话框中可用的请求列表，定义一个枚举
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// "Delete" request
        /// </summary>
        Delete = 1,
        /// <summary>
        /// "FlipLeftRight" request
        /// </summary>
        FlipLeftRight = 2,
        /// <summary>
        /// "FlipInOut" request
        /// </summary>
        FlipInOut = 3,
        /// <summary>
        /// "MakeRight" request
        /// </summary>
        MakeRight = 4,
        /// <summary>
        /// "MakeLeft" request
        /// </summary>
        MakeLeft = 5,
        /// <summary>
        /// "TurnOut" request
        /// </summary>
        TurnOut = 6,
        /// <summary>
        /// "TurnIn" request
        /// </summary>
        TurnIn = 7,
        /// <summary>
        /// "Rotate" request
        /// </summary>
        Rotate = 8
    }

    /// <summary>
    ///   A class around a variable holding the current request.
    /// </summary>
    /// <remarks>
    ///   Access to it is made thread-safe, even though we don't necessarily
    ///   need it if we always disable the dialog between individual requests.
    /// </remarks>
    /// 
    public class Request
    {
        // Storing the value as a plain Int makes using the interlocking mechanism simpler
        private int m_request = (int)RequestId.None;//将值存储为普通Int可以简化联锁机制的使用

        /// <summary>
        ///   Take - The Idling handler calls this to obtain the latest request. 
        /// </summary>
        /// <remarks>
        ///   This is not a getter! It takes the request and replaces it
        ///   with 'None' to indicate that the request has been "passed on".
        /// </remarks>
        /// 
        public RequestId Take()//Take-空闲处理程序调用它来获取最新的请求，这不是getter，他接受请求并替换它，用None表示请求已'传递'
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        /// <summary>
        ///   Make - The Dialog calls this when the user presses a command button there. 
        /// </summary>
        /// <remarks>
        ///   It replaces any older request previously made.
        /// </remarks>
        /// 
        public void Make(RequestId request)//当用户按下命令按钮是，对话框会调用这个函数，它代替了以前提出的任何旧的请求
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
