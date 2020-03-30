using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ModelessFormExternalEvent
{
    public class RequestHandler : IExternalEventHandler
    {
        // A trivial delegate, but handy
        private delegate void DoorOperation(FamilyInstance e);

        // The value of the latest request made by the modeless form 
        private Request m_request = new Request();//由非模态窗体发出的最新请求的值

        /// <summary>
        /// A public property to access the current request value
        /// </summary>
        public Request Request
        {
            get { return m_request; }//访问当前请求值的公共属性
        }

        /// <summary>
        ///   A method to identify this External Event Handler
        /// </summary>
        public String GetName()
        {
            return "R2014 External Event Sample";
        }


        /// <summary>
        ///   The top method of the event handler.
        /// </summary>
        /// <remarks>
        ///   This is called by Revit after the corresponding
        ///   external event was raised (by the modeless form)
        ///   and Revit reached the time at which it could call
        ///   the event's handler (i.e. this object)
        /// </remarks>
        /// 
        public void Execute(UIApplication uiapp)
        {
            try
            {
                switch (Request.Take())//空闲处理程序调用它来获取最新的请求
                {
                    case RequestId.None://直接调用的RequestID的枚举数据
                        {
                            return;  // no request at this time -> we can leave immediately
                        }
                    case RequestId.Delete:
                        {
                            TaskDialog.Show("Revit2020", "Delete");
                            break;
                        }
                    case RequestId.FlipLeftRight:
                        {
                            TaskDialog.Show("Revit2020", "FlipLeftRight");
                            break;
                        }
                    case RequestId.FlipInOut:
                        {
                            TaskDialog.Show("Revit2020", "FlipInOut");
                            break;
                        }
                    case RequestId.MakeLeft:
                        {
                            TaskDialog.Show("Revit2020", "MakeLeft");
                            break;
                        }
                    case RequestId.MakeRight:
                        {
                            TaskDialog.Show("Revit2020", "MakeRight");
                            break;
                        }
                    case RequestId.TurnOut:
                        {
                            TaskDialog.Show("Revit2020", "TurnOut");
                            break;
                        }
                    case RequestId.TurnIn:
                        {
                            TaskDialog.Show("Revit2020", "TurnIn");
                            break;
                        }
                    case RequestId.Rotate:
                        {
                            TaskDialog.Show("Revit2020", "Rotate");
                            break;
                        }
                    default:
                        {
                            // some kind of a warning here should
                            // notify us about an unexpected request 
                            break;
                        }
                }
            }
            finally
            {
                APP.thisApp.WakeFormUp();
            }

            return;
        }

    }  // class
}
