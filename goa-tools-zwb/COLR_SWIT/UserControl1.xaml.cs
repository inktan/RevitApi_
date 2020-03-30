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

namespace COLR_SWIT
{

    public partial class Window1 : Window
    {
        public Window1()
        {
            i = 0;
            InitializeComponent();
        }

        public int i ;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            i = 1;
            this.DialogResult = true;
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            i = 2;
            this.DialogResult = true;
            this.Close();
        }
    }
}
