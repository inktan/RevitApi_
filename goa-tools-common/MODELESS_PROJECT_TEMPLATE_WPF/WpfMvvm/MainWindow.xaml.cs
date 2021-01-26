using System;
using System.Collections.Generic;
using System.Globalization;
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

using Autodesk.Revit.UI;

namespace MODELESS_PROJECT_TEMPLATE_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;

        #region 窗口采用单例模式
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
        /// <summary>
        /// 通过函数实现单例模式
        /// </summary>
        /// <returns></returns>
        internal static MainWindow GetInstance()
        {
            if (ReferenceEquals(_instance, null))
            {
                _instance = new MainWindow();
            }
            return _instance;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        private MainWindow()
        {
            InitializeComponent();
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);

            SeiBingdings();
            SetData();
            SetOtherData();
        }
        /// <summary>
        /// BindingUI控件和数据源
        /// </summary>
        private void SeiBingdings()
        {
        }
        private void SetLayOut()
        {
        }
        private void SetData()
        {
            InputParameter.Instance.intTransparency = Convert.ToInt32(this.SetSurfaceTransparency.Text);
        }
        private void SetOtherData()
        {
        }
        #region modeless form related RevitApiMakeQuest固定模板
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void makeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            this.dozeOff();
        }



        #endregion

        /// <summary>
        /// 框选directshapes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button01_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.SelDieectionShpaesByRectangle);
        }

        private void Button02_Click(object sender, RoutedEventArgs e)
        {
            SetData();
            makeRequest(RequestId.SetSurfaceTransparency);
        }

        /// <summary>
        /// 解锁所有已锁定物体
        /// </summary>
        private void Button03_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.CommentElementId);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.Test);

        }
    }
}
