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
using goa.Common;

namespace GRID_Number
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                ////check domain
                //if (ADValidationCheck.GetDirectoryEntryForCurrentUser() == null)
                //{
                //    TaskDialog.Show("信息", "需要连接goa网络。");
                //    return Result.Failed;
                //}

                APP.UIApp = commandData.Application;
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;//Revit文档数据读取
                Autodesk.Revit.DB.View activeView = doc.ActiveView;
                ElementId activeViewId = activeView.Id;

                //check opened window
                MainWindow form = APP.MainWindow;
                if (null != form && form.Visible == true)
                {
                    form.Activate();
                    return Result.Succeeded;
                }

                //show new window
                var p = Process.GetCurrentProcess();
                var revitWindow = new WindowHandle(p.MainWindowHandle);
                if (null == form || form.IsDisposed)
                    form = new MainWindow(uidoc);
                APP.MainWindow = form;
                form.Show(revitWindow);
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
            }
            return Result.Succeeded;
        }// excute

        //以下为各种method---------------------------------分割线---------------------------------

    }
}
