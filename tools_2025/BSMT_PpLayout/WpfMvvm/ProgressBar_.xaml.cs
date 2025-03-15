using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
using System.Windows.Threading;
using System.Security.Permissions;

namespace BSMT_PpLayout
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBar_ : Window
    {
        internal bool IsBreak { get; set; }
        public ProgressBar_()
        {
            InitializeComponent();

            this.IsBreak = false;
        }

        internal void Computer(string title, double minimum,double maximun)
        {
            this.textContent.Content = title;

            this.ProgressBar01.Minimum = minimum;
            this.ProgressBar01.Maximum = maximun;
        }

        private void TempBreak_Click(object sender, RoutedEventArgs e)
        {
            IsBreak = true;
        }
    }

}
