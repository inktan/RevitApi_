using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using goa.Common;

namespace AutoFillUpLevelHeightAnnotation
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName() { return "Overlapping Elements Clean Up Request Handler"; }
        public void Execute(UIApplication app)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.AutoFillUp:
                        {
                            window.AutoFillUp();
                            break;
                        }
                    case RequestId.Pick:
                        {
                            window.Pick();
                            break;
                        }
                }
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //user cancelled
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                Form_cursorPrompt.Stop();
                window.WakeUp();
                window.Activate();
            }
        }
    }
}
