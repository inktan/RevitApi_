using Autodesk.Revit.UI;

namespace CommonLittleCommands
{
    class APP : IExternalApplication
    {

        //public static UIApplication UIApp;
        public static MainWindow MainWindow = MainWindow.Instance;
        public static string Version = "v1.00";

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
            //throw new NotImplementedException();
        }

        public Result OnStartup(UIControlledApplication application)
        {
            return Result.Succeeded;
            //throw new NotImplementedException();
        }
    }
}
