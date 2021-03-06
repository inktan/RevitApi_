﻿using Autodesk.Revit.UI;

namespace VIEW_Duplicate
{
    public class APP : IExternalApplication
    {
        public static UIApplication UIApp;
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
