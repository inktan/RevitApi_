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

namespace TransGroupParas
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        //声明全局静态变量
        public static DocumentSet documentSet = null;//文档集
        public static IList<string> openFileNames = new List<string>();//打开的文档列表
        public static string originFileName = "";//打开的文档列表
        public static string targetFileName = "";//打开的文档列表
        public static IList<string> selectedGroupNames = new List<string>();//选择的组列表


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
                Application app = uiapp.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;
                View activeView = doc.ActiveView;
                ElementId activeViewId = activeView.Id;

                //获取所有文档列表
                documentSet = app.Documents;//为已经打开的文档文件
                if (documentSet.Size > 1)
                {
                    foreach(Document document in documentSet)//获取所有的打开文档的标题列表
                    {
                        string docName = document.Title;
                        //TaskDialog.Show("Revit", docNmae);
                        openFileNames.Add(docName);
                    }

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
                else
                {
                    TaskDialog.Show("Revit", "当前打开的Reivt文件只有一个，无法执行该插件。");
                }

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
