using Autodesk.Revit.UI;

namespace MODELESS_PROJECT_TEMPLATE_WPF
{
    class APP : IExternalApplication
    {

        //public static UIApplication UIApp;
        public static MainWindow MainWindow = MainWindow.Instance;

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
