using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary_test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIDocument uidoc;
        private Document doc;
        private ExternalEvent m_ExEvent;
        //private MainWindowRequestHandler m_Handler;

        public MainWindow(UIDocument _uidoc)
        {
            InitializeComponent();

            this.Title = "Revit2020";//WPF窗口标题
            this.Title += " " + APP.Version;
            //this.m_Handler = new MainWindowRequestHandler();
            //this.m_ExEvent = ExternalEvent.Create(m_Handler);
            this.uidoc = _uidoc;
            this.doc = _uidoc.Document;

            string pngDirectory = @"I:\WangTan\14-parametric study for dwellings\分析图\temp_01.png";
            this.image.Source = new BitmapImage(new Uri(pngDirectory));
            this.image_Copy.Source = new BitmapImage(new Uri(pngDirectory));
            this.image_Copy1.Source = new BitmapImage(new Uri(pngDirectory));
            this.image_Copy2.Source = new BitmapImage(new Uri(pngDirectory));

            this.image.ToolTip = "建筑面积：";
            this.image.ToolTip += "\n套内面积：";
            this.image.ToolTip += "\n得房率：";
            this.image_Copy.ToolTip = "建筑面积：";
            this.image_Copy.ToolTip += "\n套内面积：";
            this.image_Copy.ToolTip += "\n得房率：";
            this.image_Copy1.ToolTip = "建筑面积：";
            this.image_Copy1.ToolTip += "\n套内面积：";
            this.image_Copy1.ToolTip += "\n得房率：";
            this.image_Copy2.ToolTip = "建筑面积：";
            this.image_Copy2.ToolTip += "\n套内面积：";
            this.image_Copy2.ToolTip += "\n得房率：";

            GetEdition();
            this.button.ToolTip = GetEdition().ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.label6.Content = "建筑面积：";
            this.label10.Content = "公摊面积：";
            this.label11.Content = "得房率：";
        }
        public static Version GetEdition()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
