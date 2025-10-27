using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using goa.Common;
using System;
using Autodesk.Revit.ApplicationServices;

namespace DimensioningTools
{
    abstract class RequestMethod
    {
        internal UIApplication uiApp { get; }
        internal Application app => this.uiApp.Application;
        internal UIDocument uiDoc => this.uiApp.ActiveUIDocument;
        internal Document doc => this.uiDoc.Document;
        internal Selection sel => this.uiDoc.Selection;
        internal View view => this.uiDoc.ActiveView;

        // 辅助信息
        internal Stopwatch sw;// 用于计时

        public RequestMethod(UIApplication _uiApp)
        {
            this.uiApp = _uiApp;
            this.sw = new Stopwatch();
        }

        internal abstract void Execute();

    }
}
