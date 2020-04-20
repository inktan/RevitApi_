using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

using goa.Common;
using System.Collections.ObjectModel;

namespace FakeGridNum
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        //声明全局静态变量
        //public static ObservableCollection<string> TestList = new ObservableCollection<string>();


        public static string changeCommand = null;//判断条件字段
        public static string partField = null;//输入分区字段
        public static string startGridName;//输入起始字段，为字母

        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            //throw new NotImplementedException();

            try
            {
                MainWindow form = APP.MainWindow;
                if (null != form && form.IsVisible == true)
                {
                    form.Activate();
                    return Result.Succeeded;
                }

                //show new window
                if (null == form || !form.IsLoaded)
                    form = new MainWindow();
                APP.MainWindow = form;
                form.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }// excute
    }//class
}//namespace
