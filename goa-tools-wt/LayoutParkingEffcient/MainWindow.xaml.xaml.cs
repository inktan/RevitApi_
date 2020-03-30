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
            parkingMode.Add("垂直式");
            parkingMode.Add("平行式");
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
        /// <summary>
        /// 点击我 功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CMD._selRegionBoundings.Count != 0)
            {
                //设置停车位 数据需要单位换算
                double parkingPlaceHeight = Convert.ToDouble(this.parkingPlaceHeight.Text);
                double parkingPlaceWight = Convert.ToDouble(this.parkingPlaceWight.Text);
                double Wd = Convert.ToDouble(this.Wd.Text);
                double columnWidth = Convert.ToDouble(this.columnWidth.Text);

                parkingPlaceHeight = UnitUtils.Convert(parkingPlaceHeight, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
                parkingPlaceWight = UnitUtils.Convert(parkingPlaceWight, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
                Wd = UnitUtils.Convert(Wd, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
                columnWidth = UnitUtils.Convert(columnWidth, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);

                CMD.parkingPlaceHeight = parkingPlaceHeight;
                CMD.parkingPlaceWight = parkingPlaceWight;
                CMD.Wd = Wd;
                CMD.columnWidth = columnWidth;

                makeRequest(RequestId.TestMethod_temp);//与创建模型命令进行对接
            }
            else
            {
                MessageBox.Show("请设计师选择闭合边界线，注意在选择之前，将所有线转化为模型线。");
            }
        }
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.SelectRegionalBoundary);//选择车库边界曲线
        }
    }//class
}//namespace
