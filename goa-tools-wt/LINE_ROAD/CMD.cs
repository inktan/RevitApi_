using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LINE_ROAD
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        //声明全局静态变量
        public static ObservableCollection<string> LineStylesName = new ObservableCollection<string>();
        public static ObservableCollection<GraphicsStyle> LineStyles = new ObservableCollection<GraphicsStyle>();

        public static GraphicsStyle _tarGraphicsStyle = null;//给新创建的线条线设置线型
        public static double Radius = 0;
        public static double Offset = 0;

        //public static 
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //throw new NotImplementedException();
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                //
                List<GraphicsStyle> lineStyles = _Methods.getAllLineGraphicsStyl(doc);
                foreach (GraphicsStyle gs in lineStyles)
                {
                    LineStyles.Add(gs);
                }
                //
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
