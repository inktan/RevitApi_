using Autodesk.Revit;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ThreeDViewNavi
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD_ground : IExternalCommand
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

                goa.Common.APP.UIApp = commandData.Application;
                Navi.FeetOnGround();
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
            }
            return Result.Succeeded;
        }
    }
}
