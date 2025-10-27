using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices; 
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;

using goa.Common;
using System.Data;
using System.Diagnostics;

namespace SHET_NEW
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
                if (ADValidationCheck.GetDirectoryEntryForCurrentUser() == null)
                {
                    TaskDialog.Show("信息", "需要连接goa网络。");
                    return Result.Failed;
                }

                var doc = commandData.Application.ActiveUIDocument.Document;
                var form = new Form1(doc);
                var p = Process.GetCurrentProcess();
                var revitWindow = new WindowHandle(p.MainWindowHandle);
                form.ShowDialog(revitWindow);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
                return Result.Failed;
            }
        }
    }
}
