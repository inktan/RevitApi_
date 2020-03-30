using Autodesk.Revit.UI;

namespace GRID_Number
{
    public class APP : IExternalApplication
    {
        public static UIApplication UIApp;
        public static MainWindow MainWindow;
        public static string Version = "v1.00";

        //声明全局静态变量
        public static string changeCommand = null;//判断条件字段
        public static string partField = null;//输入分区字段
        public static string startGridName;//输入起始字段，为字母

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
