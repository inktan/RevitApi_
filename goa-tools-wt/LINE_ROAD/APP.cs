﻿using Autodesk.Revit.UI;

namespace LINE_ROAD
{
    public class APP : IExternalApplication
    {
        public static UIApplication UIApp;
        public static MainWindow MainWindow;
        public static string Version = "v1.00";

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
            }
            return Result.Succeeded;
        }

    }
}
