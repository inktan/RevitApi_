using Autodesk.Revit.UI;
using goa.Common;
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

namespace MountSiteDesignAnaly
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;

        public MainWindow()
        {
            InitializeComponent();

            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);
  
            this.DataContext = ViewModel.Instance;

        }

        /// <summary>
        /// 采用单例模式
        /// </summary>
        private static MainWindow _instance;
        /// <summary>
        /// 通过属性实现单例模式
        /// </summary>
        internal static MainWindow Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new MainWindow();
                }
                return _instance;
            }
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
                this.IsEnabled = status;
                //foreach (FrameworkElement control in this.myGrid.Children)
                //{
                //    control.IsEnabled = status;
                //}
            }
            catch (Exception ex)
            {
            }
        }
        private void makeRequest(RequestId request)
        {
            SetData();
         
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            this.dozeOff();
        }
        #endregion
        /// <summary>
        /// 提取输入数据
        /// </summary>
        private void SetData()
        {
     
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.Test);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.CalCoverGreen = false;
            makeRequest(RequestId.CoverHeightDetection);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.ClearCalculationResults);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.EaCalByGenericModel);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.Show);

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.Hide);

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.CalCoverGreen = true;
            makeRequest(RequestId.CalculateGreenSpaceRate);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.GenerateFloor);
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.BsmtSpaceHeightDetection);
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.RetainWallAnakysis_External);

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.RetainWallAnakysis_Internal);

        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.RetainWallAnakysis_Site);

        }
    }
}
