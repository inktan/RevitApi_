using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace BSMT_PpLayout
{
    public class App : IExternalApplication
    {

        public static int Time = 0;
        public static List<string> pathNames = new List<string>();

        /*
         * 族文件路径，可以整体迁移时使用
         */
        public static string FamilyFilePath = @"W:\BIM_ARCH\02.族库\地库强排\";

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
            //throw new NotImplementedException();
        }

        public Result OnStartup(UIControlledApplication application)
        {
            return Result.Succeeded;
            //throw new NotImplementedException();
        }
    }
}
