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
using System.IO;
using System.Windows.Controls.Primitives;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace dll_wpf
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;

        public MainWindow()
        {
            InitializeComponent();
            this.Title += " " + APP.Version;
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);

            //窗口初始化大小
            //this.Width = 330;
            //this.Height = 800;

            //窗口初始化后即加载所有数据
            this.listBox.ItemsSource = CMD.TestList;
        }
        #region modeless form related
        public void WakeUp()
        {
            enableCommands(true);
        }
        private void dozeOff()
        {
            enableCommands(false);
        }
        private void enableCommands(bool status)
        {
            try
            {
                foreach (System.Windows.FrameworkElement control in this.myGrid.Children)
                {
                    control.IsEnabled = status;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void makeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            this.dozeOff();
        }
        #endregion
        //some method~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.TestMethod_temp);//与创建模型命令进行对接
        }
    }//class
}//namespace
