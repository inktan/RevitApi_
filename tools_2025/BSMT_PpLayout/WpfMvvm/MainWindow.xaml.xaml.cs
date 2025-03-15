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
using PubFuncWt;

using goa.Common;

namespace BSMT_PpLayout
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;

        Dictionary<string, List<CityData>> CityData;

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
        public MainWindow()
        {
            InitializeComponent();
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);

            // 界面首次显示 计算柱网距离
            string strColumnDistance = (GlobalData.Instance.pSWidth_num.FeetToMilliMeter() * 3 + GlobalData.Instance.ColumnWidth_num.FeetToMilliMeter() + GlobalData.Instance.ColumnBurfferDistance_num.FeetToMilliMeter()).ToString();
            this.ColumnDistance.Text = strColumnDistance;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            //this.Left = 10;
            //this.Top = 10;
            // 界面首次显示 显示省份和城市
            InputCitiesData();
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
        private void makeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            this.dozeOff();
        }
        #endregion

        //some method~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// 初始化 省份与城市
        /// </summary>
        private void InputCitiesData()
        {
            CityData city = new CityData();
            this.CityData = city.GetCityData();

            List<string> provinces = this.CityData.Keys.ToList();// 省份
            this.provinces.ItemsSource = this.CityData.Keys;// 省份
            this.provinces.SelectedIndex = provinces.IndexOf("国标");

            this.cities.ItemsSource = this.CityData["国标"].Select(p => p.CityName).ToList();// 城市
            this.cities.SelectedIndex = 0;

            DetermineParameters();
        }
        /// <summary>
        /// 省份切换
        /// </summary>
        private void provinces_SelectionChanged(object sender, SelectionChangedEventArgs e)// 切换省份
        {
            string selProvince = this.provinces.SelectedValue as string;
            List<CityData> cityDatas = this.CityData[selProvince].ToList();
            List<string> citiesName = cityDatas.Select(p => p.CityName).ToList();
            this.cities.ItemsSource = citiesName;
            this.cities.SelectedIndex = 0;

            //DetermineParameters();
        }
        /// <summary>
        /// 城市切换
        /// </summary>
        private void cities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetermineParameters();
        }
        void DetermineParameters()
        {
            string selProvince = this.provinces.SelectedValue as string;
            string selCiry = this.cities.SelectedValue as string;
            CityData cityData = this.CityData[selProvince].Where(p => p.CityName == selCiry).FirstOrDefault();
            if (cityData != null)
            {
                this.pSHeight.Text = cityData.pSHeight.ToString();
                this.pSWidth.Text = cityData.pSWidth.ToString();
                //this.bigPSHeight.Text
                //this.bigPSWidth.Text
                this.miniPSHeight.Text = cityData.miniPSHeight.ToString();
                this.miniPSWidth.Text = cityData.miniPSWidth.ToString();
                this.pSHeight_Hor.Text = cityData.pSHeight_Hor.ToString();
                this.pSWidth_Hor.Text = cityData.pSWidth_Hor.ToString();
                this.Wd_main.Text = cityData.Wd_pri.ToString();
                this.Wd.Text = cityData.Wd_sec.ToString();
            }
        }
        private void SetData()
        {
            // 提取坡道数据
            CalRamp();

            double pSHeight;
            double.TryParse(this.pSHeight.Text, out pSHeight);//车位 高度
            double pSWidth;
            double.TryParse(this.pSWidth.Text, out pSWidth);//车位 宽度     

            //double bigPSHeight;
            //double.TryParse(this.bigPSHeight.Text, out bigPSHeight);//大 车位 高度
            //double bigPSWidth;
            //double.TryParse(this.bigPSWidth.Text, out bigPSWidth);//大 车位 宽度         

            double miniPSHeight;
            double.TryParse(this.miniPSHeight.Text, out miniPSHeight);//迷你 车位 高度
            double miniPSWidth;
            double.TryParse(this.miniPSWidth.Text, out miniPSWidth);//迷你 车位 宽度      

            double pSHeight_Hor;
            double.TryParse(this.pSHeight_Hor.Text, out pSHeight_Hor);//平行式 停车 高度
            double pSWidth_Hor;
            double.TryParse(this.pSWidth_Hor.Text, out pSWidth_Hor);//平行式 停车 宽度

            double columnWidth;
            double.TryParse(this.columnWidth.Text, out columnWidth);//柱子 宽度
            double columnBurfferDistance;
            double.TryParse(this.columnBurfferDistance.Text, out columnBurfferDistance);//柱子缓冲距离
            double nmsPP;
            double.TryParse(this.NumsPP.Text, out nmsPP);//间隔车位数量 2/3，对应大小柱网问题

            double columnBackwardDistance;
            double.TryParse(this.ColumnBackwardDistance.Text, out columnBackwardDistance); // 柱后退车头距离

            double basementWall_offset_distance;
            double.TryParse(this.basementWall_offset_distance.Text, out basementWall_offset_distance);//停车位距离地库外墙线
            double tarParkingEfficiency;
            double.TryParse(this.tarParkingEfficiency.Text, out tarParkingEfficiency);//设定的目标停车效率

            double double_returnLengthEnd;
            double.TryParse(this.returnLengthEnd.Text, out double_returnLengthEnd);// 尽端回车长度
            double double_loopReturnLength;
            double.TryParse(this.loopReturnLength.Text, out double_loopReturnLength);// 环道回车长度

            double double_numberCarsAllocated;
            double.TryParse(this.NumberCarsAllocated.Text, out double_numberCarsAllocated);
            GlobalData.Instance.numberCarsAllocated_num = double_numberCarsAllocated;// 应配车位数

            double double_perOfDamagedPs;
            double.TryParse(this.PerOfDamagedPs.Text, out double_perOfDamagedPs);
            GlobalData.Instance.perOfDamagedPs_num = double_perOfDamagedPs;// 折损车位比例（建议10%）

            // 自动化 选项
            GlobalData.Instance.pSHeight_num = UnitUtils_.MilliMeterToFeet(pSHeight);// 普通
            GlobalData.Instance.pSWidth_num = UnitUtils_.MilliMeterToFeet(pSWidth);

            GlobalData.Instance.miniPSHeight_num = UnitUtils_.MilliMeterToFeet(miniPSHeight);// 微型
            GlobalData.Instance.miniPSWidth_num = UnitUtils_.MilliMeterToFeet(miniPSWidth);

            GlobalData.Instance.pSHeight_Hor_num = UnitUtils_.MilliMeterToFeet(pSHeight_Hor);// 平行式停车
            GlobalData.Instance.pSWidth_Hor_num = UnitUtils_.MilliMeterToFeet(pSWidth_Hor);

            GlobalData.Instance.ColumnWidth_num = UnitUtils_.MilliMeterToFeet(columnWidth);
            GlobalData.Instance.ColumnBurfferDistance_num = UnitUtils_.MilliMeterToFeet(columnBurfferDistance);
            GlobalData.Instance.NumOfIntervalPP_num = nmsPP;
            GlobalData.Instance.ColumnBackwardDistance_num = UnitUtils_.MilliMeterToFeet(columnBackwardDistance);

            GlobalData.Instance.bsmtWallThickness_num = UnitUtils_.MilliMeterToFeet(basementWall_offset_distance);
            GlobalData.Instance.tarParkingEfficiency_num = tarParkingEfficiency.SQUARE_METERStoSQUARE_FEET();

            GlobalData.Instance.endReturnLength_num = UnitUtils_.MilliMeterToFeet(double_returnLengthEnd);
            GlobalData.Instance.loopReturnLength_num = UnitUtils_.MilliMeterToFeet(double_loopReturnLength);

            double Wd_main;
            double.TryParse(this.Wd_main.Text, out Wd_main);//主车道宽度
            double Wd;
            double.TryParse(this.Wd.Text, out Wd);//通车道宽度
            double Wd_CustomWidth;
            double.TryParse(this.Wd_CustomWidth.Text, out Wd_CustomWidth);//通车道宽度

            GlobalData.Instance.Wd_pri_num = UnitUtils_.MilliMeterToFeet(Wd_main);
            GlobalData.Instance.Wd_sec_num = UnitUtils_.MilliMeterToFeet(Wd);
            GlobalData.Instance.Wd_CustomWidth_num = UnitUtils_.MilliMeterToFeet(Wd_CustomWidth);

            // 储藏间分割相关
            double walkwayWidth;
            double.TryParse(this.walkwayWidth.Text, out walkwayWidth);// 走道宽度
            double minArea;
            double.TryParse(this.minArea.Text, out minArea);// 走道宽度
            double minDepth;
            double.TryParse(this.minDepth.Text, out minDepth);// 最小进深
            double maxDepth;
            double.TryParse(this.maxDepth.Text, out maxDepth);// 最大进深
            double minWidth;
            double.TryParse(this.minWidth.Text, out minWidth);//最小开间
            double maxWidth;
            double.TryParse(this.maxWidth.Text, out maxWidth);//最大开间

            GlobalData.Instance.WalkwayWidth_num = UnitUtils_.MilliMeterToFeet(walkwayWidth);
            GlobalData.Instance.MinArea_num = minArea.SQUARE_METERStoSQUARE_FEET();
            GlobalData.Instance.MinDepth_num = UnitUtils_.MilliMeterToFeet(minDepth);
            GlobalData.Instance.MaxDepth_num = UnitUtils_.MilliMeterToFeet(maxDepth);
            GlobalData.Instance.MinWidth_num = UnitUtils_.MilliMeterToFeet(minWidth);
            GlobalData.Instance.MaxWidth_num = UnitUtils_.MilliMeterToFeet(maxWidth);

            // 详图区域偏移距离
            double double_offsetDis;
            double.TryParse(this.offsetDis.Text, out double_offsetDis);
            GlobalData.Instance.fROffsetDis_num = UnitUtils_.MilliMeterToFeet(double_offsetDis);

            // 线头检查
            double double_lineCheckDis;
            double.TryParse(this.lineCheckDis.Text, out double_lineCheckDis);
            GlobalData.Instance.lineCheckDis_num = UnitUtils_.MilliMeterToFeet(double_lineCheckDis);

            // 插车系数
            double northVehicleInsertionCoefficient;
            double.TryParse(this.NorthVehicleInsertionCoefficient.Text, out northVehicleInsertionCoefficient);
            GlobalData.Instance.NorthVehicleInsertionCoefficient_num = northVehicleInsertionCoefficient;

            double sorthVehicleInsertionCoefficient;
            double.TryParse(this.SorthVehicleInsertionCoefficient.Text, out sorthVehicleInsertionCoefficient);
            GlobalData.Instance.SorthVehicleInsertionCoefficient_num = sorthVehicleInsertionCoefficient;

            double leftVehicleInsertionCoefficient;
            double.TryParse(this.LeftVehicleInsertionCoefficient.Text, out leftVehicleInsertionCoefficient);
            GlobalData.Instance.LeftVehicleInsertionCoefficient_num = leftVehicleInsertionCoefficient;

            double rightVehicleInsertionCoefficient;
            double.TryParse(this.RightVehicleInsertionCoefficient.Text, out rightVehicleInsertionCoefficient);
            GlobalData.Instance.RightVehicleInsertionCoefficient_num = rightVehicleInsertionCoefficient;

            // 出图数据
            double chamferRadius;
            double.TryParse(this.ChamferRadius.Text, out chamferRadius);
            GlobalData.Instance.ChamferRadius_num = chamferRadius.MilliMeterToFeet();
            double endCirclePosition;
            double.TryParse(this.EndCirclePosition.Text, out endCirclePosition);
            GlobalData.Instance.EndCirclePosition_num = endCirclePosition.MilliMeterToFeet();
            double endCircleRadius;
            double.TryParse(this.EndCircleRadius.Text, out endCircleRadius);
            GlobalData.Instance.EndCircleRadius_num = endCircleRadius.MilliMeterToFeet();

            // 序号圆圈大小
            double numCircleNum;
            double.TryParse(this.NumCircleNum.Text, out numCircleNum);
            GlobalData.Instance.NumCircleRadiu_num = numCircleNum.MilliMeterToFeet();

            double numTextSize;
            double.TryParse(this.NumTextSize.Text, out numTextSize);
            GlobalData.Instance.NumTextSize_num = numTextSize.MilliMeterToFeet();

            // 是否创建表格
            if (this.wheGenerateTable.IsChecked == true)
            {
                GlobalData.Instance.wheGenerateTable = true;
            }
            else
            {
                GlobalData.Instance.wheGenerateTable = false;
            }

            // 阵列是否使用 机械车位
            if (this.wheMechanicalPs.IsChecked == true)
            {
                GlobalData.Instance.wheMechanicalPs = true;
            }
            else
            {
                GlobalData.Instance.wheMechanicalPs = false;
            }
        }

        /// <summary>
        /// 刷新边界 添加
        /// </summary>
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            GlobalData.Instance.isAddRemove = true;
            SetData();
            makeRequest(RequestId.SelBsmtExWall);//选择车库边界曲线
        }
        /// <summary>
        /// 刷新边界 删除
        /// </summary>
        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            GlobalData.Instance.isAddRemove = false;
            SetData();
            makeRequest(RequestId.SelBsmtExWall);//选择车库边界曲线 
        }
        /// <summary>
        /// 刷新边界 删除
        /// </summary>
        private void Button13_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.RestBsmtWallEles);//选择车库边界曲线
        }
        /// <summary>
        /// 车位刷新
        /// </summary>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.GlobalAroundBasedExistRoadSystem);
        }
        /// <summary>
        /// 全局寻路
        /// </summary>
        private void RoadGenerate_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Road2Growth);
        }

        /// <summary>
        /// 数据刷新
        /// </summary>
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.RefreshDataStatistics);
        }
        /// <summary>
        /// 点柱子
        /// </summary>
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();

            if (this.bigColumn.IsChecked == true)
                GlobalData.Instance.BigOrsmallColumn = BigOrSmallColumn.Big;
            else if (this.smallColumn.IsChecked == true)
                GlobalData.Instance.BigOrsmallColumn = BigOrSmallColumn.Small;
            else
                GlobalData.Instance.BigOrsmallColumn = BigOrSmallColumn.Big;

            SetData();
            makeRequest(RequestId.PointPillar);
        }
        /// <summary>
        /// 窗口是隐藏，而非关闭
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        /// <summary>
        /// 检测车道尽端问题
        /// </summary>
        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.LookForEndPS);
        }

        private void _TextChanged(object sender, TextChangedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            if (this.ColumnDistance != null)
            {
                SetColumnDistance();
            }
        }
        /// <summary>
        /// 计算柱距
        /// </summary>
        private void SetColumnDistance()
        {
            if (this.pSWidth != null && this.columnWidth != null && this.columnBurfferDistance != null)
            {
                if (this.pSWidth.Text != null && this.columnWidth.Text != null && this.columnBurfferDistance.Text != null
                   && this.pSWidth.Text != "" && this.columnWidth.Text != "" && this.columnBurfferDistance.Text != "")
                {
                    double a;
                    double.TryParse(this.pSWidth.Text, out a);
                    double n;
                    double.TryParse(this.NumsPP.Text, out n);
                    double c;
                    double.TryParse(this.columnWidth.Text, out c);
                    double d;
                    double.TryParse(this.columnBurfferDistance.Text, out d);
                    this.ColumnDistance.Text = ((int)a * (int)n + (int)c + (int)d).ToString();
                }
            }
        }
        /// <summary>
        /// 检查碰撞
        /// </summary>
        private void CheckCollisions_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.CheckCollisions);
        }
        /// <summary>
        /// 选择柱子
        /// </summary>
        private void Button14_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.SelColumsFs);
        }
        /// <summary>
        /// 选择停车位
        /// </summary>
        private void Button18_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.SelPSByLine);
        }
        /// <summary>
        /// 两点阵列
        /// </summary>
        private void Button17_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();

            SetData();
            makeRequest(RequestId.LineArray);
        }
        /// <summary>
        /// 添加轴网
        /// </summary>
        private void AddGrid_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.GridMarking);
        }
        /// <summary>
        /// 添加尺寸线
        /// </summary>
        private void Button20_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.DimensionMarking);
        }
        /// <summary>
        /// 排序号
        /// </summary>
        private void AddSequenceNum_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();

            if (this.AddCircle.IsChecked == true)
            {
                GlobalData.Instance.AddCircle = true;
            }
            else
            {
                GlobalData.Instance.AddCircle = false;
            }

            makeRequest(RequestId.AddSequenceNum);
        }

        /// <summary>
        /// 将方案居中显示
        /// </summary>
        private void Button25_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.ShowDesignLocation);
        }
        /// <summary>
        /// 倒角
        /// </summary>
        private void Button19_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.wheChamfer = true;
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.RoadIntersecIconChamfer);
        }
        /// <summary>
        /// 倒角复原
        /// </summary>
        private void Button21_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.wheChamfer = false;
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.RoadIntersecIconChamfer);
        }

        private void filledRegionOffest_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.FRoffset);
        }

        /// <summary>
        /// 调整柱子起点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void adjustStartPoint_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            GlobalData.Instance.AdjustStartPoint = true;

            makeRequest(RequestId.AdjustStartPoint);
        }
        /// <summary>
        /// 指定子区域，在水平方向进行车道优化排布
        /// </summary>
        private void SubAreaLaneGeneration_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            GlobalData.Instance.AdjustStartPoint = false;
            makeRequest(RequestId.SubAreaLaneGeneration);
        }
        /// <summary>
        /// 全局寻路
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Road2Growth_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            GlobalData.Instance.AdjustStartPoint = false;
            makeRequest(RequestId.Road2Growth);
        }
        /// <summary>
        /// 弧线阵列
        /// </summary>
        private void ArcArray_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.ArcArray);
        }
        private void temp_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Temp01);
        }
        private void Temp02_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Temp02);
        }
        private void Temp03_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Temp03);
        }
        private void Temp04_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Temp04);
        }
    
        private void Temp05_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Temp05);
        }
        private void Temp06_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.Temp06);
        }
        /// <summary>
        /// 坡道计算
        /// </summary>
        private void RampGenerate_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.RampGenerated);
        }

        /// <summary>
        /// 储藏间划分
        /// </summary>
        private void DivideStorageRoom_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.DivideStorageRoom);
        }
        /// <summary>
        /// 绘制地库外墙
        /// </summary>
        private void AddBsmtWallLines_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.DrawBsmtWallLine);
        }
        /// <summary>
        /// 读取面板数据
        /// </summary>
        /// <param name="e"></param>
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.ReadPanelData);
        }
        /// <summary>
        /// 刷新方案列表
        /// </summary>
        private void updateDesignList_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.UpdateDesignList);
        }
        /// <summary>
        /// 提取cad
        /// </summary>
        private void ExtractCad_Click(object sender, RoutedEventArgs e)
        {
            makeRequest(RequestId.ExtractCad);
        }
        /// <summary>
        /// 自动寻路
        /// </summary>
        private void AutoPathfinding_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.AutoPathfinding);
        }
        /// <summary>
        /// 刷图层
        /// </summary>
        private void BrushLayer_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.BrushLayer);
        }
        /// <summary>
        /// 检查车道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineCheck_Click(object sender, RoutedEventArgs e)
        {
            GlobalData.Instance.selViewNames = this.listBox1.SelectedItems.Cast<string>().ToList();
            SetData();
            makeRequest(RequestId.LineCheck);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalRamp();
        }
        void CalRamp()
        {
            /*
              * 当坡道纵向坡度大于10％时，坡道上、下端均应设缓坡坡段
              * 其直线缓坡段的水平长度不应小于3．6m，缓坡坡度应为坡道坡度的1/2 
              * 曲线缓坡段的水平长度不应小于2．4m
              */

            if (ReferenceEquals(this.rampFloorHeight, null)
                || ReferenceEquals(this.GentleSlope, null)
                || ReferenceEquals(this.SlowSlopeLength, null)
                || ReferenceEquals(this.PpHeightRequire, null)
                || ReferenceEquals(this.rampSlope, null))
            {

            }
            else
            {
                double rampFloorHeight;// 层高
                double.TryParse(this.rampFloorHeight.Text, out rampFloorHeight);

                double gentleSlope;// 缓坡坡度
                double.TryParse(this.GentleSlope.Text, out gentleSlope);

                double SlowSlopeLength;// 缓坡长度
                double.TryParse(this.SlowSlopeLength.Text, out SlowSlopeLength);

                double rampSlope;// 坡度
                double.TryParse(this.rampSlope.Text, out rampSlope);

                double ppHeightRequire;// 可停车最低净高
                double.TryParse(this.PpHeightRequire.Text, out ppHeightRequire);

                // 计算缓坡高度
                double gentleSlopeHeight = SlowSlopeLength * gentleSlope;
                GlobalData.Instance.GentleSlopeHeight = gentleSlopeHeight.NumDecimal(1).ToString();

                double remainHeight = rampFloorHeight - gentleSlopeHeight;
                // 中间段坡长
                double middleLength = remainHeight / rampSlope;

                GlobalData.Instance.MiddleRampLength = middleLength.NumDecimal(1).ToString();
                GlobalData.Instance.TotalRampLength = (middleLength + SlowSlopeLength * 2).NumDecimal(1).ToString();
                GlobalData.Instance.TotalRampLength_num = (middleLength + SlowSlopeLength * 2).NumDecimal(1);

                if (rampSlope <= 0.1)// 不需要缓坡
                {
                    GlobalData.Instance.GentleSlopeHeight = "0.0";

                    GlobalData.Instance.MiddleRampLength = (rampFloorHeight / rampSlope).NumDecimal(1).ToString();
                    GlobalData.Instance.TotalRampLength = (rampFloorHeight / rampSlope).NumDecimal(1).ToString();
                    GlobalData.Instance.TotalRampLength_num = (rampFloorHeight / rampSlope).NumDecimal(1);
                }

                GlobalData.Instance.CurrentLength = GlobalData.Instance.CurrentLength_num.NumDecimal(1).ToString();
                GlobalData.Instance.NeedLength = (GlobalData.Instance.TotalRampLength_num - GlobalData.Instance.CurrentLength_num).NumDecimal(1).ToString();

                // 计算坡道下不可停车长度
                if (rampSlope <= 0.1)
                {
                    // 可停车最低净高 / 坡度
                    GlobalData.Instance.NoStopLengthBottomOfRamp = (ppHeightRequire / rampSlope).NumDecimal(1).ToString();
                }
                else
                {
                    // 缓坡坡长 + (可停车最低净高 - 缓坡高度
                    GlobalData.Instance.NoStopLengthBottomOfRamp = (SlowSlopeLength + (ppHeightRequire - gentleSlopeHeight) / rampSlope).NumDecimal(1).ToString();
                }
            }

        }
    
    }//class
}//namespace
