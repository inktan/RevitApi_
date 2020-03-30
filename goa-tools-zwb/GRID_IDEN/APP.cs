using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GOA.GRID_IDEN
{
    public class APP : IExternalApplication
    {
        public static Autodesk.Revit.UI.UIApplication UIApp;
        public static Autodesk.Revit.ApplicationServices.Application Application;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public virtual Result OnStartup(UIControlledApplication _uiCtrlApp)
        {
            StartUpSetup(_uiCtrlApp);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication _uiCtrlApp)
        {
            return Result.Succeeded;
        }

        public void StartUpSetup(UIControlledApplication _uiCtrlApp)
        {
            string tabName = "goa tools";
            string panelName = "轴网工具";
            string buttonName = "标头线";
            addTab(_uiCtrlApp, tabName);
            var panel = addPanel(_uiCtrlApp, panelName, tabName);
            addButton(panel, buttonName);
        }

        private void addTab(UIControlledApplication _uiCtrlApp, string _tabName)
        {
            try
            {
                _uiCtrlApp.CreateRibbonTab(_tabName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                //tab already exists
            }
        }

        private RibbonPanel addPanel(UIControlledApplication _uiCtrlApp, string _panelName, string _tabName)
        {
            //try find panel
            var panels = _uiCtrlApp.GetRibbonPanels(_tabName);
            RibbonPanel panel = null;
            foreach (var p in panels)
            {
                if (p.Name == _panelName)
                    panel = p;
            }
            if (null == panel)
                panel = _uiCtrlApp.CreateRibbonPanel(_tabName, _panelName);
            return panel;
        }

        private void addButton(RibbonPanel _panel, string _buttonName)
        {
            PushButtonData b1 =
                new PushButtonData("GOA.GRID_IDEN",
                                    _buttonName,
                                    addinPath(),
                                    "GOA.GRID_IDEN.CMD");

            var iconBitmap = global::GOA.GRID_IDEN.Properties.Resources.GRID_IDEN_32;
            BitmapSource icon = iconBitmap.BitmapImage();
            b1.LargeImage = icon;

            PushButton b = _panel.AddItem(b1) as PushButton;
        }

        private string addinPath()
        {
            Assembly assembly = Assembly.GetAssembly(this.GetType());
            return assembly.Location;
        }
    }

    public static class BitmapEx
    {
        public static BitmapImage BitmapImage(this Bitmap _bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                _bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}
