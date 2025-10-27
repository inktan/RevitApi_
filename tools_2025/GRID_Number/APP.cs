using Autodesk.Revit.UI;

namespace GRID_Number
{
    public class APP : IExternalApplication
    {
        public static UIApplication UIApp;
        //public static MainWindow MainWindow;

        

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
