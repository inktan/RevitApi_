using Autodesk.Revit.UI;
using goa.Common;
using goa.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDViewNavi
{
    public class RequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName() { return "revit addin Request Handler"; }
        public void Execute(UIApplication app)
        {
            goa.Common.APP.UIApp = app;
            var window = APP.MainWindow;
            if (window.IsAvailable())
                window.dozeOff();
            Registry.Register(false);

            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.close:
                        {
                            if (window.IsAvailable())
                                window.Close();
                            Registry.Register(false);
                            return;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //user cancelled
            }
            catch (goa.Common.Exceptions.IgnorableException ex)
            {
                //ignore
            }
            catch (InvalidCurveLoopException ex)
            {
                UserMessages.ShowMessage("线圈非完整连续，或未闭合。");
            }
            catch (goa.Common.Exceptions.CommonUserExceptions ex)
            {
                UserMessages.ShowMessage(ex.Message);
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                AssemblyLoader.Register(false);
                Form_cursorPrompt.Stop();
                if (window.IsAvailable())
                {
                    window.WakeUp();
                    window.Activate();
                    Registry.Register(true);
                }
            }
        }
    }
}
