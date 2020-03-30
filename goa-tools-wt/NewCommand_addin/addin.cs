using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace NewCommand_addin
{
    public class addin : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel_NewCommand = application.CreateRibbonPanel("New Command");
            
            PushButton pushButton_COLR_SWIT = ribbonPanel_NewCommand.AddItem(new PushButtonData("颜色切换", "COLR_SWIT", @"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\COLR_SWIT\bin\Debug\COLR_SWIT.dll", "COLR_SWIT.CDM")) as PushButton;
            Uri uriImage_COLR_SWIT = new Uri(@"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\颜色切换.png");

            BitmapImage largeImage_COLR_SWIT = new BitmapImage(uriImage_COLR_SWIT);
            pushButton_COLR_SWIT.LargeImage = largeImage_COLR_SWIT;


            PushButton pushButton_VIEW_INTF = ribbonPanel_NewCommand.AddItem(new PushButtonData("视图提资", "VIEW_INTF", @"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\VIEW_INTF\bin\Debug\VIEW_INTF.dll", "VIEW_INTF.CDM")) as PushButton;
            Uri uriImage_VIEW_INTF = new Uri(@"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\视图提资.png");
            BitmapImage largeImage_VIEW_INTF = new BitmapImage(uriImage_VIEW_INTF);
            pushButton_VIEW_INTF.LargeImage = largeImage_VIEW_INTF;
            //
            //
            //
            //RibbonPanel ribbonPanel_NewCommand = application.CreateRibbonPanel("New Command");

            //PushButton pushButton_COLR_SWIT = ribbonPanel_NewCommand.AddItem(new PushButtonData("颜色切换", "COLR_SWIT", @"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\COLR_SWIT\bin\Debug\COLR_SWIT.dll", "COLR_SWIT.CDM")) as PushButton;
            //Uri uriImage_COLR_SWIT = new Uri(@"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\颜色切换.png");
            //BitmapImage largeImage_COLR_SWIT = new BitmapImage(uriImage_COLR_SWIT);
            //pushButton_COLR_SWIT.LargeImage = largeImage_COLR_SWIT;

            //PushButton pushButton_VIEW_INTF = ribbonPanel_NewCommand.AddItem(new PushButtonData("视图提资", "VIEW_INTF", @"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\VIEW_INTF\bin\Debug\VIEW_INTF.dll", "VIEW_INTF.CDM")) as PushButton;
            //Uri uriImage_VIEW_INTF = new Uri(@"H:\Users\wang.tan\Documents\GitHub\goa-tools-wt\视图提资.png");
            //BitmapImage largeImage_VIEW_INTF = new BitmapImage(uriImage_VIEW_INTF);
            //pushButton_VIEW_INTF.LargeImage = largeImage_VIEW_INTF;
            //
            //
            //
            return Result.Succeeded;
        }
    }
}
