using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
//using goa.Common;

namespace FAKE_DIMS
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            try
            {
                //check domain
                //if (ADValidationCheck.GetDirectoryEntryForCurrentUser() == null)
                //{
                //    TaskDialog.Show("信息", "需要连接goa网络。");
                //    return Result.Failed;
                //}

                var uidoc = commandData.Application.ActiveUIDocument;

                //show new window
                //goa.Common.APP.RevitWindow = Methods.GetRevitWindow(commandData.Application);
                var form = new MainWindow();
                APP.MainWindow = form;
                form.Show(DimensioningTools.APP.MainWindow);
            }
            catch (Exception ex)
            {
                //UserMessages.ShowErrorMessage(ex, null);
            }
            return Result.Succeeded;
        }

        internal static void Run()
        {
            //show new window
            var form = new MainWindow();
            APP.MainWindow = form;
            form.Show(DimensioningTools.APP.MainWindow);
        }
    }
}
