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
using System.Windows.Shapes;

namespace CollisionDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        /// <summary>
        /// 关闭窗口后，该实例只是为隐藏模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        #region modeless form related
        public void WakeUp()
        {
            enableCommands(true);
        }
        public void DozeOff()
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

        #endregion


    }
}
