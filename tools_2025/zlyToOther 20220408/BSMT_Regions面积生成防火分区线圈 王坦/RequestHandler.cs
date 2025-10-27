using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using goa.Common;

namespace BSMT_Regions
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName() { return "Overlapping Elements Clean Up Request Handler"; }
        public void Execute(UIApplication app)
        {
            var window = APP.MainWindow;
            var uidoc = app.ActiveUIDocument;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.PickElement:
                        {
                            var areas = myMethods.PickAreas(uidoc);
                            myMethods.CreateFromArea(areas, 3, true, uidoc.ActiveView);
                            break;
                        }
                }
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
