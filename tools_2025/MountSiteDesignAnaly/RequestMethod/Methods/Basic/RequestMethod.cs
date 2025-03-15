using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using goa.Common;
using System;

namespace MountSiteDesignAnaly
{
    abstract class RequestMethod
    {
        internal UIApplication uiApp { get; }
        internal UIDocument uiDoc => this.uiApp.ActiveUIDocument;
        internal Document doc => this.uiDoc.Document;
        internal Selection sel => this.uiDoc.Selection;
        internal View view => this.uiDoc.ActiveView;

        // 辅助信息
        internal Stopwatch sw;// 用于计时

        internal string AreaShowHideInfo = "0F092674-3F81-4CE9-998C-8F8FE6314BF0";



        public RequestMethod(UIApplication _uiApp)
        {
            this.uiApp = _uiApp;
            this.sw = new Stopwatch();
        }

        internal abstract void Execute();

    }
}
