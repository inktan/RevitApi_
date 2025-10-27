using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;
using goa.Common.Exceptions;

namespace AutoFillUpLevelHeightAnnotation
{
    internal static class CMD
    {
        internal static void Run()
        {
            //check opened window
            MainWindow form = APP.MainWindow;
            if (form.IsAvailable())
            {
                form.Activate();
                return;
            }

            //show new window
            if (null == form || form.IsDisposed)
                form = new MainWindow();
            APP.MainWindow = form;
            form.Show(DimensioningTools.APP.MainWindow);
        }
    }
}
