using Autodesk.Revit.UI;
using goa.Common;

namespace DimensioningTools
{
    public class APP : IExternalApplication
    {
        internal static MainWindow MainWindow = null;
        internal static string Version = "v1.031";

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
