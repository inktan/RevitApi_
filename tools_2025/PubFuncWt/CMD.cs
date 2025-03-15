
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using System;
using System.Diagnostics;

namespace PubFuncWt
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            #region 与revit文档交互入口
            UIApplication uiApp = commandData.Application;
            #endregion

            Document Doc = uiApp.ActiveUIDocument.Document;
            Selection Sel = uiApp.ActiveUIDocument.Selection;

            string msg = "test\r\n\test";
            var ex = new Exception(msg);

            string email = "wang.tan@goa.com.cn";
            UserMessages.SendMessageToWeChat(msg, email, "addin name");
            UserMessages.SendErrorMessageToWeChat(ex, email, "addin name");


            // tmep
      
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();

            "end123".TaskDialogErrorMessage();

            return Result.Succeeded;
        }// excute

    }//class
}//namespace
