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

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using goa.Common;

namespace BSMT_PpLayout
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SelLayer : Window
    {
        // 窗口采用单例模式
        private static SelLayer _instance;
        /// <summary>
        /// 通过属性实现单例模式
        /// </summary>
        internal static SelLayer Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new SelLayer();
                }
                return _instance;
            }
        }
        public SelLayer()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }//class
}//namespace
