using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

namespace goa.Common
{
    public static class APP
    {
        //setup by ribbon.dll at start up
        public static WindowHandle RevitWindow;
        public static UIControlledApplication UICtrlApp;
        public static UIApplication UIApp;
        public static string Version = "1.02";

        //global flags
        public static bool CommunicatingWithCentral = false;
        public static bool PauseDMU = false;
        public static bool PauseEvents = false;
        public static bool AddinExecuting = false;
        public static HashSet<goaToolsAddin> AddinsExecuting = new HashSet<goaToolsAddin>();
    }
}
