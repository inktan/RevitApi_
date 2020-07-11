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

namespace LayoutParkingEffcient
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;

        bool documentChangedRegistered = false;

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
            parkingMode.Add("水平");
            parkingMode.Add("垂直");
            //parkingMode.Add("平行式_0°");
            //parkingMode.Add("平行式_90°");
            //parkingMode.Add("斜列式-倾角30°");
            //parkingMode.Add("斜列式-倾角45°");
            //parkingMode.Add("斜列式-倾角60°");
            this.comboBox.ItemsSource = parkingMode;

            //禁止刷新按钮 禁止数据刷新按钮
            this.button.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.button5.IsEnabled = false;
            this.button3.IsEnabled = false;
            this.button9.IsEnabled = false;
            this.button10.IsEnabled = false;
            //this.button6.IsEnabled = false;
            this.checkBox.IsEnabled = false;
            //flag要倒下
            //makeRequest(RequestId.DocumentChangedEventUnRegister);
            documentChangedRegistered = false;
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
                //this.IsEnabled = status;
                //foreach (FrameworkElement control in this.myGrid.Children)
                //{
                //    control.IsEnabled = status;
                //}

                this.TabItem2.IsEnabled = status;
                this.TabItem2.IsEnabled = status;

                this.WrapPanel1.IsEnabled = status;
                this.Refresh.IsEnabled = status;

                this.button4.IsEnabled = status;
                this.button7.IsEnabled = status;
                this.button8.IsEnabled = status;
                this.button9.IsEnabled = status;
                this.button10.IsEnabled = status;
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
        private void SetData()
        {
            CMD.stopAlgorithm = true; // 将停止计算的flag倒下

            CMD.layoutMethod = this.comboBox.SelectedItem.ToString();//停车方式

            double parkingPlaceHeight = Convert.ToDouble(this.parkingPlaceHeight.Text);//车位 高度
            double parkingPlaceWight = Convert.ToDouble(this.parkingPlaceWight.Text);//车位 宽度
            double columnWidth = Convert.ToDouble(this.columnWidth.Text);//柱子 宽度
            double columnBurfferDistance = Convert.ToDouble(this.columnBurfferDistance.Text);//柱子缓冲距离
            double basementWall_offset_distance = Convert.ToDouble(this.basementWall_offset_distance.Text);//停车位距离地库外墙线

            CMD.parkingPlaceHeight = Methods.MilliMeterToFeet(parkingPlaceHeight);
            CMD.parkingPlaceWight = Methods.MilliMeterToFeet(parkingPlaceWight);
            CMD.columnWidth = Methods.MilliMeterToFeet(columnWidth);
            CMD.columnBurfferDistance = Methods.MilliMeterToFeet(columnBurfferDistance);
            CMD.basementWall_offset_distance = Methods.MilliMeterToFeet(basementWall_offset_distance);

            double Wd_main = Convert.ToDouble(this.Wd_main.Text);//主车道宽度
            double Wd = Convert.ToDouble(this.Wd.Text);//通车道宽度
            double redline_distance = Convert.ToDouble(this.redline_distance.Text);//红线退距

            CMD.Wd_main = Methods.MilliMeterToFeet(Wd_main);
            CMD.Wd = Methods.MilliMeterToFeet(Wd);
            CMD.redline_offset_distance = Methods.MilliMeterToFeet(redline_distance);
        }
        /// <summary>
        /// 刷新边界
        /// </summary>
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            SetData();
            this.button.IsEnabled = true;
            this.button2.IsEnabled = true;
            this.button5.IsEnabled = true;
            this.button3.IsEnabled = true;
            this.button6.IsEnabled = true;
            this.checkBox.IsEnabled = true;
            makeRequest(RequestId.SetControlRegionBoundary);//选择车库边界曲线
        }
        /// <summary>
        /// 强制全局刷新
        /// </summary>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            #region 计算控制
            //UserControl1 userControl1 = new UserControl1();
            //userControl1.Show();
            #endregion

            SetData();
            if (this.checkBox.IsChecked == true)
            {
                MessageBoxResult yesORnoResult = MessageBox.Show(@"是否确认启动最优解算法，如果启动，将会消耗较长计算时间，请耐心等待!!!", @"警告", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (yesORnoResult == MessageBoxResult.Yes)
                {
                    CMD.isOptimalAlgorithm = true;
                    this.checkBox.IsChecked = true;
                }
                else if (yesORnoResult == MessageBoxResult.No)
                {
                    CMD.isOptimalAlgorithm = false;
                    this.checkBox.IsChecked = false;
                }
            }
            else if (this.checkBox.IsChecked == false) CMD.isOptimalAlgorithm = false;

            makeRequest(RequestId.GlobalRefresh);
        }
        /// <summary>
        /// 智能刷新
        /// </summary>
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            SetData();
            if (this.checkBox.IsChecked == true)
            {
                MessageBoxResult yesORnoResult = MessageBox.Show(@"是否确认启动最优解算法，如果启动，将会消耗较长计算时间，请耐心等待!!!", @"警告", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                if (yesORnoResult == MessageBoxResult.Yes)
                {
                    CMD.isOptimalAlgorithm = true;
                    this.checkBox.IsChecked = true;
                }
                else if (yesORnoResult == MessageBoxResult.No)
                {
                    CMD.isOptimalAlgorithm = false;
                    this.checkBox.IsChecked = false;
                }
            }
            else if (this.checkBox.IsChecked == false) CMD.isOptimalAlgorithm = false;

            makeRequest(RequestId.IntelligentRefresh);
        }
        /// <summary>
        /// 指定区域
        /// </summary>
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            SetData();
            if (this.checkBox.IsChecked == true)
                CMD.isOptimalAlgorithm = true;
            else if (this.checkBox.IsChecked == false)
                CMD.isOptimalAlgorithm = false;
            makeRequest(RequestId.ChangeDirectionByRectange);
            this.comboBox.SelectedIndex = 0;
        }
        /// <summary>
        /// 数据刷新
        /// </summary>
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            //SetData();
            makeRequest(RequestId.RefreshDataStatistics);
        }

        private void Window_Closed(object sender, EventArgs e)//取消订阅全局监控事件
        {
            //makeRequest(RequestId.DocumentChangedEventUnRegister);
            //documentChangedRegistered = false;
            SetData();
        }
        /// <summary>
        /// 为一个区域指点起点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            SetData();
            makeRequest(RequestId.ChangeDirectionByPoint);
        }
        /// <summary>
        /// 选择未锁定停车位族实例
        /// </summary>
        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.SelunFixedParkingFs);
        }
        /// <summary>
        /// 选择锁定停车位族实例
        /// </summary>
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.SelFixedParkingFs);
        }
        /// <summary>
        /// 选择柱子族实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.SelColumsFs);
        }
        private void _checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (documentChangedRegistered == false)
            {
                makeRequest(RequestId.DocumentChangedEventRegister);
                documentChangedRegistered = true;
            }
        }

        private void _checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (documentChangedRegistered == true)
            {
                makeRequest(RequestId.DocumentChangedEventUnRegister);
                documentChangedRegistered = false;
            }
        }
        /// <summary>
        /// 检查线型是否必备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _button3_Click(object sender, RoutedEventArgs e)
        {

            makeRequest(RequestId.CheckLineStyle);//检查线类型是否存在
        }
        /// <summary>
        /// 检查线圈是否闭合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _button4_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.CheckpolygonClosed);//检查组内线条是否闭合
        }
        /// <summary>
        /// 检查组内线条线样式是否统一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _button5_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.CheckInGroupLineStyleIsSame);//检查组内线条样式是否统一
        }
        /// <summary>
        /// 检查两根线段的端点是否闭合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.CheckTwoCurveCoincidence);//检查两根线段的端点是否闭合
        }
        /// <summary>
        /// 一键隐藏 directShape，导出cad用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Button7_Click(object sender, RoutedEventArgs e)
        {
            CMD.hidenDirectShape = true;
            makeRequest(RequestId.HidenDirectShape);

        }
        /// <summary>
        /// 一键显示 directShape
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Button8_Click(object sender, RoutedEventArgs e)
        {
            CMD.hidenDirectShape = false;
            makeRequest(RequestId.HidenDirectShape);
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            SetData();
            makeRequest(RequestId.CutAlgorithm);
        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            SetData();
            makeRequest(RequestId.ChangeDirectionByBoundary);
        }
    }//class
}//namespace
