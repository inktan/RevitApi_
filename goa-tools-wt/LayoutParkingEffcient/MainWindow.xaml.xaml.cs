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

using goa.Common;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace LayoutParkingEffcient
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

            IList<string> parkingMode = new List<string>();
            parkingMode.Add("垂直式_0°");
            parkingMode.Add("垂直式_90°");
            parkingMode.Add("平行式_0°");
            parkingMode.Add("平行式_90°");
            parkingMode.Add("斜列式-倾角30°");
            parkingMode.Add("斜列式-倾角45°");
            parkingMode.Add("斜列式-倾角60°");
            this.comboBox.ItemsSource = parkingMode;

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
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            double parkingPlaceHeight = Convert.ToDouble(this.parkingPlaceHeight.Text);//车位 高度
            double parkingPlaceWight = Convert.ToDouble(this.parkingPlaceWight.Text);//车位 宽度
            double Wd = Convert.ToDouble(this.Wd.Text);//通车道 宽度
            double columnWidth = Convert.ToDouble(this.columnWidth.Text);//柱子 宽度

            CMD.parkingPlaceHeight = Methods.MilliMeterToFeet(parkingPlaceHeight);
            CMD.parkingPlaceWight = Methods.MilliMeterToFeet(parkingPlaceWight);
            CMD.Wd = Methods.MilliMeterToFeet(Wd);
            CMD.columnWidth = Methods.MilliMeterToFeet(columnWidth);

            double Wd_main = Convert.ToDouble(this.Wd_main.Text);//主车道宽度
            double redline_distance = Convert.ToDouble(this.redline_distance.Text);//红线退距

            CMD.Wd_main = Methods.MilliMeterToFeet(Wd_main);
            CMD.redline_offset_distance = Methods.MilliMeterToFeet(redline_distance);

            CMD.layoutMethod = this.comboBox.SelectedItem.ToString();

            makeRequest(RequestId.SelGarageBoundary);//选择车库边界曲线
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            makeRequest(RequestId.CheckLineStyle);//检查线类型是否存在
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.CheckpolygonClosed);//检查组内线条是否闭合
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.CheckInGroupLineStyleIsSame);//检查组内线条样式是否统一
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.TestOthers);//检查组内线条样式是否统一
        }
    }//class
}//namespace
