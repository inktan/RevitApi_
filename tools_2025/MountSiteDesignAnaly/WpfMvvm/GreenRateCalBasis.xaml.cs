using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MountSiteDesignAnaly
{
    /// <summary>
    /// Interaction logic for GreenRateCalBasis.xaml
    /// </summary>
    public partial class GreenRateCalBasis : Window
    {

        public GreenRateCalBasis()
        {
            InitializeComponent();
            this.DataContext = ViewModel.Instance;
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (string.IsNullOrEmpty((e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor).Description))
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
            else
            {
                e.Column.Header = (e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor).Description;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
