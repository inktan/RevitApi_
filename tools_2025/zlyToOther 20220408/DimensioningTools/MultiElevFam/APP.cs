using Autodesk.Revit.UI;
using goa.Common;

namespace AutoFillUpLevelHeightAnnotation
{
    public class APP : IExternalApplication
    {
        public static MainWindow MainWindow;
        public static string Version = "v1.00";

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
