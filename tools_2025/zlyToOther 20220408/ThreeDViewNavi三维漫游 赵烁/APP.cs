using Autodesk.Revit.UI;
using goa.Common;

namespace ThreeDViewNavi
{
    public class APP : IExternalApplication
    {
        public static UIApplication UIApp { get { return goa.Common.APP.UIApp; } set { goa.Common.APP.UIApp = value; } }
        internal static MainWindow MainWindow;
        internal static string Version = "v1.01";

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
