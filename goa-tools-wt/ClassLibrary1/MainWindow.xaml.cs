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

namespace ClassLibrary1
{

    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected int eventCounter = 0;

        private void SomethingClicked(object sender, RoutedEventArgs e)
        {
            eventCounter++;
            string message = "#" + eventCounter.ToString() + ":\n\r" +
                " Sender " + sender.ToString() + "\n\r" +
                "Original Source: " + e.OriginalSource;
            this.listBox.Items.Add(message);
            e.Handled = (bool)checkBox.IsChecked;
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.listBox.Items.Clear();
        }
        
    }
}
