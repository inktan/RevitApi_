using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using goa.Common;

namespace MODELESS_PROJECT_TEMPLATE
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
                }
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }
    }
}
