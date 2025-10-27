using Autodesk.Revit.UI;
using goa.Common;

namespace CoordinationInfo
{
    public class APP : IExternalApplication
    {
        internal static MainWindow MainWindow;
        internal static string Version = "v1.00";

        public virtual Result OnStartup(UIControlledApplication _uiCtrlApp)
        {
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication _uiCtrlApp)
        {
            return Result.Succeeded;
        }
    }
}
