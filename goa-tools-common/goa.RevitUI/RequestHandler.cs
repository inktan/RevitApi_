using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Threading;
using goa.Common;

namespace goa.RevitUI
{
    public enum RequestId : int
    {
        None,
        //postCMD
        API_move,
        API_copy,
        API_rotate,
        API_mirrorPick,
        API_mirrorDraw,
        PostCmd_move,
        PostCmd_copy,
        PostCmd_MirrorPick,
        PostCmd_MirrorDraw,
        PostCmd_Rotate,

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

    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName() { return "Overlapping Elements Clean Up Request Handler"; }
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }

                    //利用系统命令
                    case RequestId.PostCmd_move:
                        {
                            Methods.PostCmd_move(app);
                            Methods.makeRequest(RequestId.API_move);
                            break;
                        }
                    case RequestId.PostCmd_copy:
                        {
                            Methods.PostCmd_copy(app);
                            Methods.makeRequest(RequestId.API_copy);
                            break;
                        }
                    case RequestId.PostCmd_MirrorPick:
                        {
                            Methods.PostCmd_MirrorPick(app);
                            Methods.makeRequest(RequestId.API_mirrorPick);

                            break;
                        }

                    case RequestId.PostCmd_Rotate:
                        {
                            Methods.PostCmd_rotate(app);
                            Methods.makeRequest(RequestId.API_rotate);
                            break;
                        }
                    case RequestId.API_move:
                        {
                            Methods.API_move(doc);
                            break;
                        }
                    case RequestId.API_copy:
                        {
                            Methods.API_copy(app);
                            break;
                        }

                    case RequestId.API_mirrorPick:
                        {
                            Methods.API_mirrorPick(app);
                            break;
                        }

                    case RequestId.API_rotate:
                        {
                            Methods.API_rotate(doc);
                            break;
                        }


                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return;
            }

            catch (Exception ex)
            {
                goa.Common.UserMessages.ShowErrorMessage(ex, null);
            }

            finally
            {
                //window.WakeUp();
                //window.Activate();
            }
        }
    }
}
