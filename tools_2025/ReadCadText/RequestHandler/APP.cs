using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;
using PubFuncWt;

namespace ReadCadText
{
    public class App : IExternalApplication
    {

        //public static int Time = 0;
        //public static List<string> pathNames = new List<string>();

        /*
         * 族文件路径，可以整体迁移时使用
         */
        //public static string FamilyFilePath = @"W:\BIM_ARCH\03.插件\地库强排\族\";

        public Result OnShutdown(UIControlledApplication application)
        {
            //application.ControlledApplication.DocumentSynchronizedWithCentral -= OnDocumentSynced;
            //application.ControlledApplication.DocumentChanged -= ControlledApplication_DocumentChanged;

            return Result.Succeeded;
            //throw new NotImplementedException();
        }


        public Result OnStartup(UIControlledApplication application)
        {
            //application.ControlledApplication.DocumentSynchronizedWithCentral += OnDocumentSynced;
            //application.ControlledApplication.DocumentChanged += ControlledApplication_DocumentChanged;

            return Result.Succeeded;
            //throw new NotImplementedException();
        }

    }
}
