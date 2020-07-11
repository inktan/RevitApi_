using Autodesk.Revit.UI;

namespace LayoutParkingEffcient
{
    public class APP : IExternalApplication
    {
        public static UIApplication UIApp;
        public static MainWindow MainWindow;
        public static string Version = "v1.00";
        public static double PRECISION = 0.001;//0.001foot*304.8mm = 0.003048mm

        public virtual Result OnStartup(UIControlledApplication application)
        {
            MainWindow = null;   // no dialog needed yet; the command will bring it
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            if (MainWindow != null)
            {
                MainWindow.Close();
                MainWindow = null;
            }
            return Result.Succeeded;
        }
    }
}
