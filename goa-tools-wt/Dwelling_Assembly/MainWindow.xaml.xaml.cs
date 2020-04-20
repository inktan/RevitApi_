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
using System.Collections.ObjectModel;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Dwelling_Assembly
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;

        //所有HX
        public List<string> huxingNameList = new List<string>();
        public List<string> huxingPathList = new List<string>();
        //所有HX_iamge_namelist
        public List<string> huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307 = new List<string>();//根据核心筒对应的套型组合模式进行分类
        public List<string> huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405 = new List<string>();
        public List<string> huxingNameList_samlleftpng_CORE_335406 = new List<string>();
        public List<string> huxingNameList_samlleftpng_CORE_5401_5402_5403 = new List<string>();
        public List<string> huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408 = new List<string>();
        //所有HXT_拿出所有image图像的文件名list
        List<string> huxingNameList_samllpng = new List<string>();
        //所有户型组合示意图
        public List<string> huxingZH_NameList = new List<string>();
        public List<string> huxingZH_PathList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            this.Title += " " + APP.Version;
            this.textBox.Text = "GOA 大象设计";
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);

            //窗口初始化大小
            this.Width = 330;
            this.Height = 800;

            string str_logo = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\Resource_PNG\Logo\logo_small.png";//设置界面logo
            this.main_logo.Source = ReturnBitmap(str_logo);//获取图像需要完整的路径

            //窗口初始化后即加载所有数据
            //所有户型组合示意图路径
            string allhuxingZH_Path = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\Resource_PNG\单元组合示意";
            huxingZH_PathList = GetDirectory(allhuxingZH_Path, out huxingZH_NameList);//文件名字列表

            //所有户型文件路径
            string allHXFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT";
            huxingPathList = GetDirectory(allHXFilePath, out huxingNameList);//户型组合路径列表

            //拿出所有image图像的文件名list
            foreach (string str_huxingName in huxingNameList)
            {
                if (str_huxingName.Contains(".png") && str_huxingName.Contains("small") && str_huxingName.Contains("Left"))
                {
                    huxingNameList_samllpng.Add(str_huxingName);
                }
            }

            //房型选项
            this.comboBox1.ItemsSource = CMD.houseType;
            this.comboBox2.ItemsSource = CMD.houseType;
            this.comboBox7.ItemsSource = CMD.houseType;
            this.comboBox10.ItemsSource = CMD.houseType;
            //开间选项
            this.comboBox3.ItemsSource = CMD.kaiJianNum;
            this.comboBox4.ItemsSource = CMD.kaiJianNum;
            this.comboBox8.ItemsSource = CMD.kaiJianNum;
            this.comboBox11.ItemsSource = CMD.kaiJianNum;
            //边厅选项
            this.comboBox5.ItemsSource = CMD.ketingweizhi;
            this.comboBox6.ItemsSource = CMD.ketingweizhi;
            this.comboBox9.ItemsSource = CMD.ketingweizhi;
            this.comboBox12.ItemsSource = CMD.ketingweizhi;

            //对户型小缩略图 进行HXT分类
            foreach (string str_huxingPngName in huxingNameList_samllpng)
            {
                if (str_huxingPngName.Contains("_CORE_3301")
                    || str_huxingPngName.Contains("_CORE_3302")
                    || str_huxingPngName.Contains("_CORE_3303")
                    || str_huxingPngName.Contains("_CORE_3304")
                    || str_huxingPngName.Contains("_CORE_3305")
                    || str_huxingPngName.Contains("_CORE_3306")
                    || str_huxingPngName.Contains("_CORE_3307"))//此处需要说明_CORE_33系列的核心筒不可以出现54编号
                {
                    huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307.Add(str_huxingPngName);
                }
                if (str_huxingPngName.Contains("_CORE_335401")
                    || str_huxingPngName.Contains("_CORE_335402")
                    || str_huxingPngName.Contains("_CORE_335403")
                    || str_huxingPngName.Contains("_CORE_335404")
                    || str_huxingPngName.Contains("_CORE_335405"))
                {
                    huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405.Add(str_huxingPngName);
                }
                if (str_huxingPngName.Contains("_CORE_335406"))
                {
                    huxingNameList_samlleftpng_CORE_335406.Add(str_huxingPngName);
                }
                if (str_huxingPngName.Contains("_CORE_5401")
                    || str_huxingPngName.Contains("_CORE_5402")
                    || str_huxingPngName.Contains("_CORE_5403"))
                {
                    huxingNameList_samlleftpng_CORE_5401_5402_5403.Add(str_huxingPngName);
                }
                if (str_huxingPngName.Contains("_CORE_5404")
                    || str_huxingPngName.Contains("_CORE_5405")
                    || str_huxingPngName.Contains("_CORE_5406")
                    || str_huxingPngName.Contains("_CORE_5407")
                    || str_huxingPngName.Contains("_CORE_5408")
                    || str_huxingPngName.Contains("_CORE_5409")
                    || str_huxingPngName.Contains("_CORE_5410")
                    || str_huxingPngName.Contains("_CORE_5411")
                    || str_huxingPngName.Contains("_CORE_5412")
                    || str_huxingPngName.Contains("_CORE_5413")
                    || str_huxingPngName.Contains("_CORE_5414")
                    || str_huxingPngName.Contains("_CORE_5415")
                    || str_huxingPngName.Contains("_CORE_5416")
                    || str_huxingPngName.Contains("_CORE_5417")
                    || str_huxingPngName.Contains("_CORE_5418")
                    || str_huxingPngName.Contains("_CORE_5419")
                    || str_huxingPngName.Contains("_CORE_5420"))
                {
                    huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408.Add(str_huxingPngName);
                }
            }
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

        //户型组合示意图选择判断
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.button_Copy.IsEnabled = true;
        }
        //重设住选项窗口数据-主
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.groupBox1.IsEnabled = true;
            this.groupBox_Copy2.IsEnabled = true;
            this.groupBox_Copy3.IsEnabled = true;

            //复原所有选项
            this.radioButton.IsChecked = false;
            this.radioButton1.IsChecked = false;
            this.radioButton2.IsChecked = false;
            this.radioButton3.IsChecked = false;
            this.radioButton4.IsChecked = false;
            this.radioButton5.IsChecked = false;
            this.radioButton5_Copy.IsChecked = false;
            this.radioButton6.IsChecked = false;
            this.radioButton7.IsChecked = false;
            this.radioButton8.IsChecked = false;
            //禁用所有套型按钮
            this.TXA.IsEnabled = false;
            this.TXB.IsEnabled = false;
            this.TXC.IsEnabled = false;
            this.TXD.IsEnabled = false;
            //
            //窗口初始化大小
            this.Height = 800;
            this.Width = 330;
            ResetListboxSource();
            RestTX();
        }
        //重设窗口数据-次
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //禁用所有套型按钮
            this.TXA.IsEnabled = false;
            this.TXB.IsEnabled = false;
            this.TXC.IsEnabled = false;
            this.TXD.IsEnabled = false;

            this.button_Copy.IsEnabled = true;
            this.button2.IsEnabled = true;
            this.listBox.IsEnabled = true;
            this.Width = 330;
            RestTX();
        }
        //重设套型设计参数
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            List<Image> nullImageList = new List<Image>();
            this.listBox1.ItemsSource = nullImageList;
            RestTX();
            this.Width = 1000;
        }
        //所有套型数据复原-四个套型参数全部复原
        private void RestTX()
        {
            listBox1.IsEnabled = false;

            //重设所有套型条件-房型-开间-客厅位置
            this.comboBox2.SelectedIndex = -1;
            this.comboBox4.SelectedIndex = -1;
            this.comboBox5.SelectedIndex = -1;
            this.comboBox1.SelectedIndex = -1;
            this.comboBox3.SelectedIndex = -1;
            this.comboBox6.SelectedIndex = -1;
            this.comboBox7.SelectedIndex = -1;
            this.comboBox8.SelectedIndex = -1;
            this.comboBox9.SelectedIndex = -1;
            this.comboBox10.SelectedIndex = -1;
            this.comboBox11.SelectedIndex = -1;
            this.comboBox12.SelectedIndex = -1;
            //重设所有套型条件-面积段
            this.textBox1.Text = "0";
            this.textBox2.Text = "0";
            this.textBox3.Text = "0";
            this.textBox4.Text = "0";
        }
        //单元形式对应的单元数量设定
        private void radioButton3_Checked(object sender, RoutedEventArgs e)
        {
            this.radioButton8.IsEnabled = true;
        }

        private void radioButton4_Checked(object sender, RoutedEventArgs e)
        {
            this.radioButton8.IsEnabled = false;
        }
        //加载界面主选项内容-户型组合示意图
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (radioButton.IsChecked == true)//H < 33 读取 1T2H
            {
                if (radioButton3.IsChecked == true)//1T2H
                {
                    if (radioButton6.IsChecked == true)//1单元
                    {
                        SettargetImage("_1T2H_", "_1DY_");
                    }
                    else if (radioButton7.IsChecked == true)//2单元
                    {
                        SettargetImage("_1T2H_", "_2DY_");
                    }
                    else if (radioButton8.IsChecked == true)//3单元
                    {
                        SettargetImage("_1T2H_", "_3DY_");
                    }
                    else
                        ResetListboxSource();
                }
                else
                    ResetListboxSource();
            }
            else if (radioButton1.IsChecked == true)// 33 H 54 读取 1T2H 1T4H
            {
                if (radioButton3.IsChecked == true)//1T2H
                {
                    if (radioButton6.IsChecked == true)//1单元
                    {
                        SettargetImage("_1T2H_", "_1DY_");
                    }
                    else if (radioButton7.IsChecked == true)//2单元
                    {
                        SettargetImage("_1T2H_", "_2DY_");
                    }
                    else if (radioButton8.IsChecked == true)//3单元
                    {
                        SettargetImage("_1T2H_", "_3DY_");
                    }
                    else
                        ResetListboxSource();
                }
                else if (radioButton4.IsChecked == true)//1T4H
                {
                    if (radioButton6.IsChecked == true)//1单元
                    {
                        SettargetImage("_1T4H_", "_1DY_");
                    }
                    else if (radioButton7.IsChecked == true)//2单元
                    {
                        SettargetImage("_1T4H_", "_2DY_");
                    }
                    else
                        ResetListboxSource();
                }
                else
                    ResetListboxSource();
            }
            else if (radioButton2.IsChecked == true)// H > 54 读取 2T2H 2T4H
            {
                if (radioButton5.IsChecked == true)//2T2H
                {
                    if (radioButton6.IsChecked == true)//1单元
                    {
                        SettargetImage("_2T2H_", "_1DY_");
                    }
                    else if (radioButton7.IsChecked == true)//2单元
                    {
                        SettargetImage("_2T2H_", "_2DY_");
                    }
                    else if (radioButton8.IsChecked == true)//3单元
                    {
                        SettargetImage("_2T2H_", "_3DY_");
                    }
                    else
                        ResetListboxSource();
                }
                else if (radioButton5_Copy.IsChecked == true)//2T4H
                {
                    if (radioButton6.IsChecked == true)//1单元
                    {
                        SettargetImage("_2T4H_", "_1DY_");
                    }
                    else if (radioButton7.IsChecked == true)//2单元
                    {
                        SettargetImage("_2T4H_", "_2DY_");
                    }
                    else
                        ResetListboxSource();
                }
                else
                    ResetListboxSource();
            }
            else
                ResetListboxSource();
        }
        //设置 listbox 的源伟 null
        public void ResetListboxSource()
        {
            List<string> targetName = new List<string>();
            this.listBox.ItemsSource = targetName;
            this.listBox.IsEnabled = true;
        }
        //设置listbox的图像列表源
        public void SettargetImage(string _str, string __str)
        {
            List<string> targetName = new List<string>();
            foreach (string str in huxingZH_NameList)
            {
                if (str.Contains(".png") && str.Contains(_str) && str.Contains(__str))
                {
                    targetName.Add(str);
                }
            }
            //基于name获取fullpath
            List<string> targeFullPath = GetTaegetFileFullPath(targetName, huxingZH_PathList);
            List<Image> targetImage = new List<Image>();
            foreach (string tmep_str in targeFullPath)
            {
                Image image = new Image();
                image.Source = ReturnBitmap(tmep_str);
                foreach (string temp__str in targetName)//找到图片的名字
                {
                    if (tmep_str.Contains(temp__str))
                    {
                        int length = temp__str.Length;
                        string _name = temp__str.Substring(0, length - 4);
                        image.Name = _name;
                        CreatImageTootip(image, _name);
                    }
                }
                targetImage.Add(image);
            }
            this.listBox.ItemsSource = targetImage;
            ResetWin();
        }
        //重设窗口
        public void ResetWin()
        {
            if (radioButton.IsChecked == true || radioButton1.IsChecked == true || radioButton2.IsChecked == true)
            {
                if (radioButton3.IsChecked == true || radioButton4.IsChecked == true)
                {
                    if (radioButton6.IsChecked == true || radioButton7.IsChecked == true || radioButton8.IsChecked == true)
                    {
                        this.button_Copy.IsEnabled = false;
                        this.listBox.IsEnabled = true;

                        //窗口初始化大小
                        this.Width = 330;
                        this.Height = 800;
                    }
                }
            }
        }
        //获取目标户型组合示意图
        public List<Image> GetTargetImage(List<Image> image_lst, string str_01, string str_02)
        {
            List<Image> targetImage = new List<Image>();
            foreach (Image _image in image_lst)
            {
                if (_image.Name.Contains(str_01) && _image.Name.Contains(str_02))
                {
                    targetImage.Add(_image);
                }
            }
            return targetImage;
        }

        //按钮-生成套型设计的套型设置界面
        //套型的各个参数更改会伴随着Hxzuhe_auto方法，等同于参数改变跟踪
        //
        //分三个步骤-01识别套型组合模式-02通过面积参数>0，等套型参数设定获取符合的套型库-03对套型库进行自由组合，获取核心筒编码一致的组合模式
        //
        private void Hxzuhe_auto()
        {
            //生成模型按钮的 状态切换
            this.button3.IsEnabled = false;
            this.button3.Content = "确认";
            //该if判断用来设定详图户型拼接的宽度
            this.listBox1.IsEnabled = true;
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            List<Image> nullImageList = new List<Image>();
            this.listBox1.ItemsSource = nullImageList;
            if (this.listBox.SelectedIndex >= 0)
            {
                Image selImage = this.listBox.SelectedItem as Image;
                string _TAarea = textBox1.Text;//套型A面积参数
                string _TBarea = textBox2.Text;//套型B面积参数
                string _TCarea = textBox3.Text;//套型C面积参数
                string _TDarea = textBox4.Text;//套型D面积参数
                //首先判断户型的具体组合方式
                if (selImage.Name == "HXZH_1T2H_aa_1DY_A_" || selImage.Name == "HXZU_2T2H_aa_1DY_A_")//只有套型A
                {
                    //此处适用于33 1T2H、3354 1T2H、54 2T2H；
                    //需要注意的是，2T4H的连廊式核心筒及其对应的套型 会 出项在2T2H的模式中，此处需要对核心筒进行分类；
                    GETHXZH_HXZH_1T2H_aa_1DY_A_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_aaaa_2DY_A_" || selImage.Name == "HXZU_2T2H_aaaa_2DY_A_")//只有套型A
                {
                    GETHXZH_HXZU_1T2H_aaaa_2DY_A_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_aaaaaa_3DY_A_" || selImage.Name == "HXZU_2T2H_aaaaaa_3DY_A_")//只有套型A
                {
                    GETHXZH_HXZU_1T2H_aaaaaa_3DY_A_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_ab_1DY_AB_" || selImage.Name == "HXZU_2T2H_ab_1DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_ab_1DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abba_2DY_AB_" || selImage.Name == "HXZU_2T2H_abba_2DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_abba_2DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_aabbaa_3DY_AB_" || selImage.Name == "HXZU_2T2H_aabbaa_3DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_aabbaa_3DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abbbba_3DY_AB_" || selImage.Name == "HXZU_2T2H_abbbba_3DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_abbbba_3DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abbc_2DY_ABC_" || selImage.Name == "HXZU_2T2H_abbc_2DY_ABC_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_abbc_2DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_aabbac_3DY_ABC_" || selImage.Name == "HXZU_2T2H_aabbac_3DY_ABC_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_aabbac_3DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abbbbc_3DY_ABC_" || selImage.Name == "HXZU_2T2H_abbbbc_3DY_ABC_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_abbbbc_3DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abccba_3DY_ABC_" || selImage.Name == "HXZU_2T2H_abccba_3DY_ABC_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_abccba_3DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abccbd_3DY_ABCD_" || selImage.Name == "HXZU_2T2H_abccbd_3DY_ABCD_")//套型ABCD
                {
                    GETHXZH_HXZU_1T2H_abccbd_3DY_ABCD_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_2T4H_abba_1DY_AB_" || selImage.Name == "HXZU_1T4H_abba_1DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_2T4H_abba_1DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_2T4H_abbaabba_2DY_AB_" || selImage.Name == "HXZU_1T4H_abbaabba_2DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_2T4H_abbaabba_2DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_2T4H_abbc_1DY_ABC_" || selImage.Name == "HXZU_1T4H_abbc_1DY_ABC_")//只有套型AB
                {//
                    GETHXZH_HXZU_2T4H_abbc_1DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_2T4H_abbccbba_2DY_ABC_" || selImage.Name == "HXZU_1T4H_abbccbba_2DY_ABC_")//只有套型AB
                {
                    GETHXZH_HXZU_2T4H_abbccbba_2DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_2T4H_abbaabbc_2DY_ABC_" || selImage.Name == "HXZU_1T4H_abbaabbc_2DY_ABC_")//只有套型AB
                {
                    GETHXZH_HXZU_2T4H_abbaabbc_2DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_2T4H_abbccbbd_2DY_ABCD_" || selImage.Name == "HXZU_1T4H_abbccbbd_2DY_ABCD_")//只有套型AB
                {
                    GETHXZH_HXZU_2T4H_abbccbbd_2DY_ABCD_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abaaba_3DY_AB_" || selImage.Name == "HXZU_2T2H_abaaba_3DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_abaaba_3DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_abaabc_3DY_ABC_" || selImage.Name == "HXZU_2T2H_abaabc_3DY_ABC_")//只有套型ABC
                {
                    GETHXZH_HXZU_1T2H_abaabc_3DY_ABC_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_aaab_2DY_AB_" || selImage.Name == "HXZU_2T2H_aaab_2DY_AB_")//只有套型AB
                {
                    GETHXZH_HXZU_1T2H_aaab_2DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
                else if (selImage.Name == "HXZU_1T2H_aaaaab_3DY_AB_" || selImage.Name == "HXZU_2T2H_aaaaab_3DY_AB_")//只有套型ABC
                {
                    GETHXZH_HXZU_1T2H_aaaaab_3DY_AB_(_TAarea, _TBarea, _TCarea, _TDarea);
                }
            }
        }

        #region 按钮-生成套型设计，由套型缩略图组成的户型组合示意图
        //读取套型参数
        //将户型image组合 HXZH_1T2H_aa_1DY_A_ 模式，求出对应户型库
        public void GETHXZH_HXZH_1T2H_aa_1DY_A_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            if (double.TryParse(_TAarea, out TAarea))
            {
                if (TAarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0)//套型A条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZH_1T2H_aa_1DY_A_(TXANameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_aaaa_2DY_A_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_aaaa_2DY_A_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            if (double.TryParse(_TAarea, out TAarea))
            {
                if (TAarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0)//套型A条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_aaaa_2DY_A_(TXANameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_aaaaaa_3DY_A_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_aaaaaa_3DY_A_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            if (double.TryParse(_TAarea, out TAarea))
            {
                if (TAarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0)//套型A条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_aaaaaa_3DY_A_(TXANameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_ab_1DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_ab_1DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_abba_2DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abba_2DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abba_2DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_aabbaa_3DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_aabbaa_3DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_aabbaa_3DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_abbbba_3DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abbbba_3DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abbbba_3DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_abbc_2DY_ABC_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abbc_2DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abbc_2DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_aabbac_3DY_ABC_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_aabbac_3DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_aabbac_3DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_abbbbc_3DY_ABC_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abbbbc_3DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abbbbc_3DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_abccba_3DY_ABC_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abccba_3DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abccba_3DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abccbd_3DY_ABCD_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            double TDarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea) && double.TryParse(_TDarea, out TDarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0 && TDarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0
                        && comboBox10.SelectedIndex >= 0 && comboBox11.SelectedIndex >= 0 && comboBox12.SelectedIndex >= 0)//套型ABCD条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        List<string> TXDNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXDNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox10, comboBox11, textBox4, comboBox12, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXDNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox10, comboBox11, textBox4, comboBox12, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXDNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox10, comboBox11, textBox4, comboBox12, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abccbd_3DY_ABCD_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs, TXDNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abba_1DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_2T4H_abba_1DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_MID_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335406, "UNIT_MID_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_MID_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_2T4H_abba_1DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbaabba_2DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_2T4H_abbaabba_2DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_MID_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335406, "UNIT_MID_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_MID_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_2T4H_abbaabba_2DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbc_1DY_ABC_ 模式，求出对应户型库
        public void GETHXZH_HXZU_2T4H_abbc_1DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335406, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_2T4H_abbc_1DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbccbba_2DY_ABC_ 模式，求出对应户型库
        public void GETHXZH_HXZU_2T4H_abbccbba_2DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335406, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_2T4H_abbccbba_2DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbaabbc_2DY_ABCD_ 模式，求出对应户型库
        public void GETHXZH_HXZU_2T4H_abbaabbc_2DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335406, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_2T4H_abbaabbc_2DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbccbbd_2DY_ABCD_ 模式，求出对应户型库
        public void GETHXZH_HXZU_2T4H_abbccbbd_2DY_ABCD_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            double TDarea;
            if (double.TryParse(_TAarea, out TAarea)
                && double.TryParse(_TBarea, out TBarea)
                && double.TryParse(_TCarea, out TCarea)
                && double.TryParse(_TDarea, out TDarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0 && TDarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0
                        && comboBox10.SelectedIndex >= 0 && comboBox11.SelectedIndex >= 0 && comboBox12.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        List<string> TXDNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXDNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox10, comboBox11, textBox4, comboBox12, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335406, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                            TXDNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox10, comboBox11, textBox4, comboBox12, huxingNameList_samlleftpng_CORE_335406, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_MID_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                            TXDNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox10, comboBox11, textBox4, comboBox12, huxingNameList_samlleftpng_CORE_5404_5405_5406_5407_5408, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_2T4H_abbccbbd_2DY_ABCD_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs, TXDNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbccbbd_2DY_ABCD_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abaaba_3DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea)
                && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abaaba_3DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_2T4H_abbccbbd_2DY_ABCD_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_abaabc_3DY_ABC_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            double TCarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea) && double.TryParse(_TCarea, out TCarea))
            {
                if (TAarea > 0 && TBarea > 0 && TCarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0
                        && comboBox7.SelectedIndex >= 0 && comboBox8.SelectedIndex >= 0 && comboBox9.SelectedIndex >= 0)//套型ABC条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        List<string> TXCNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXCNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox7, comboBox8, textBox3, comboBox9, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_abaabc_3DY_ABC_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs, TXCNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_aaab_2DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_aaab_2DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_aaab_2DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(0);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        //将户型image组合 HXZU_1T2H_aaaaab_3DY_AB_ 模式，求出对应户型库
        public void GETHXZH_HXZU_1T2H_aaaaab_3DY_AB_(string _TAarea, string _TBarea, string _TCarea, string _TDarea)
        {
            double TAarea;
            double TBarea;
            if (double.TryParse(_TAarea, out TAarea) && double.TryParse(_TBarea, out TBarea))
            {
                if (TAarea > 0 && TBarea > 0)//面积输入参数确定
                {
                    if (comboBox2.SelectedIndex >= 0 && comboBox4.SelectedIndex >= 0 && comboBox5.SelectedIndex >= 0
                        && comboBox1.SelectedIndex >= 0 && comboBox3.SelectedIndex >= 0 && comboBox6.SelectedIndex >= 0)//套型AB条件
                    {
                        //与两个套型宽度相比较，得出合适的窗口宽度
                        List<string> TXANameList_samlleftpngs = new List<string>();
                        List<string> TXBNameList_samlleftpngs = new List<string>();
                        //判断高度区间
                        if (radioButton.IsChecked == true)//33
                        {
                            //获得目标户型文件fullpathes
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_3301_3302_3303_3303_3304_3305_3306_3307, "UNIT_END_LEFT_");
                        }
                        else if (radioButton1.IsChecked == true)//33-54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_335401_335402_335403_335403_335404_335405, "UNIT_END_LEFT_");
                        }
                        else if (radioButton2.IsChecked == true)//54
                        {
                            TXANameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox2, comboBox4, textBox1, comboBox5, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                            TXBNameList_samlleftpngs = GetTX_A_B_C_D_names(comboBox1, comboBox3, textBox2, comboBox6, huxingNameList_samlleftpng_CORE_5401_5402_5403, "UNIT_END_LEFT_");
                        }
                        //将目标户型image组合为listbox下拉列表
                        int zhCount = CombboxItem_HXZU_1T2H_aaaaab_3DY_AB_(TXANameList_samlleftpngs, TXBNameList_samlleftpngs);
                        if (zhCount > 0)
                        {
                            ModifyWindows_TXZH(1);
                        }
                        else
                        {
                            ModifyWindows_TXZH(0);
                        }
                    }
                }
            }
        }
        #endregion

        //根据套型参数生成套型的图片组合
        #region 将户型image组合 模式，转换为combbox的item
        //将户型image组合 HXZH_1T2H_aa_1DY_A_ 模式，转换为combbox的item
        //函数计算的int数据用来控制窗口界面大小
        public int CombboxItem_HXZH_1T2H_aa_1DY_A_(List<string> TXANameList_samlleftpngs)
        {
            //目的套型组合模式 —— aa
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaLeft in TXAFullPath)
            {
                //创建一个组合list，所有套型a均可处理为镜像模式
                List<string> targetList = new List<string>();
                string TaRight = TaLeft.Replace("Left", "Right");

                //根据户型组合模式进行列表创建，当前为 aa
                targetList.Add(TaLeft);//添加每一组合TX文件的full_path
                targetList.Add(TaRight);

                //需要确保核心筒相同
                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                //new的必要性是为了，确保每一个控件都是新创建的
                Addstackpanel_Lst(targetList, stackpanel_Lst);
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aA_1DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;//返回数值用来求窗口的设置宽度
        }
        public void getCore_imgMove_1T2H_aA_1DY(List<StackPanel> stackpanel_Lst)//对核心筒png的位置进行移动
        //method中的大写字母，代表着与核心筒png计算移动距离的套型所在位置
        {
            //计算便宜量，并列图片之间的移动距离关系，需要单独进行测试，从而得出合理的公式
            string allCOREFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\CORE\"; //所有核心筒文件路径
            string allUNITFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT\"; //所有户型文件路径

            foreach (StackPanel _stackPanel in stackpanel_Lst)
            {
                //拿出StackPanel中png_list的name
                List<string> nameList = new List<string>();
                foreach (object obj in _stackPanel.Children)
                {
                    Image _image = obj as Image;
                    nameList.Add(_image.Name);
                }
                string aA_unit_imageName = nameList[1];//存在于aA中A位置的套型
                int aA_unit_nameLength;
                string aA_core_imageName = GetHxtPath(aA_unit_imageName, out aA_unit_nameLength);//先获取核心筒的name
                string aA_core_fullpath = allCOREFilePath + aA_core_imageName + @".png";//核心筒png所在位置
                string aA_unit_fullpath = allUNITFilePath + aA_unit_imageName + @".png";//套型png位置
                BitmapImage _aA_unit = ReturnBitmap(aA_unit_fullpath);
                BitmapImage _aA_core = ReturnBitmap(aA_core_fullpath);
                //求出 png 的宽度
                double _aA_core_width = _aA_core.Width;
                double _aA_unit_width = _aA_unit.Width;
                //计算向左移动的margin
                double move_aA_core_distiance = _aA_core_width / 2 + _aA_unit_width;

                //为stackpanel添加图片
                Image aA_core = new Image();
                _stackPanel.Children.Add(aA_core);//为每个_stackPanel追加核心筒png
                aA_core.Source = _aA_core;
                aA_core.HorizontalAlignment = HorizontalAlignment.Left;//为核心筒png设置对齐方式
                //调整图像位置
                Thickness aA_core_move = new Thickness(-1 * move_aA_core_distiance, 0, 0, 0);//设定核心筒向左的偏移量
                aA_core.Margin = aA_core_move;
            }
        }
        //将户型image组合 HXZU_1T2H_aaaa_2DY_A_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_aaaa_2DY_A_(List<string> TXANameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aaaa
            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaLeft in TXAFullPath)
            {
                //创建一个组合list，所有套型a均可处理为镜像模式
                List<string> targetList = new List<string>();
                string TaRight = TaLeft.Replace("Left", "Right");

                //根据户型组合模式进行列表创建，当前为 aaaa
                targetList.Add(TaLeft);//添加每一组合TX文件的full_path
                targetList.Add(TaRight);
                targetList.Add(TaLeft);
                targetList.Add(TaRight);

                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                //new的必要性是为了，确保每一个控件都是新创建的
                Addstackpanel_Lst(targetList, stackpanel_Lst);
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        public void getCore_imgMove_1T2H_aAaA_2DY(List<StackPanel> stackpanel_Lst)//对核心筒png的位置进行移动
        //method中的大写字母，代表着与核心筒png计算移动距离的套型所在位置
        {
            //计算便宜量，并列图片之间的移动距离关系，需要单独进行测试，从而得出合理的公式
            string allCOREFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\CORE\"; //所有核心筒文件路径
            string allUNITFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT\"; //所有户型文件路径

            foreach (StackPanel _stackPanel in stackpanel_Lst)
            {
                //拿出StackPanel中png_list的name
                List<string> nameList = new List<string>();
                foreach (object obj in _stackPanel.Children)
                {
                    Image _image = obj as Image;
                    nameList.Add(_image.Name);
                }
                //两单元均采用aAaA模式拾取核心筒png
                string aAaa_unit_imageName = nameList[1];//存在于aA中A位置的套型
                string aaAa_unit_imageName = nameList[2];//存在于aA中A位置的套型
                string aaaA_unit_imageName = nameList[3];//存在于aA中A位置的套型
                int aAaa_unit_nameLength;
                int aaaA_unit_nameLength;
                string aAaa_core_imageName = GetHxtPath(aAaa_unit_imageName, out aAaa_unit_nameLength);//先获取核心筒的name
                string aaaA_core_imageName = GetHxtPath(aaaA_unit_imageName, out aaaA_unit_nameLength);//先获取核心筒的name

                string aAaa_unit_fullpath = allUNITFilePath + aAaa_unit_imageName + @".png";//套型png位置
                string aaAa_unit_fullpath = allUNITFilePath + aAaa_unit_imageName + @".png";//套型png位置
                string aaaA_unit_fullpath = allUNITFilePath + aaaA_unit_imageName + @".png";//套型png位置
                string aAaa_core_fullpath = allCOREFilePath + aAaa_core_imageName + @".png";//核心筒png所在位置
                string aaaA_core_fullpath = allCOREFilePath + aaaA_core_imageName + @".png";//核心筒png所在位置

                BitmapImage _aAaa_unit = ReturnBitmap(aAaa_unit_fullpath);
                BitmapImage _aaAa_unit = ReturnBitmap(aaAa_unit_fullpath);
                BitmapImage _aaaA_unit = ReturnBitmap(aaaA_unit_fullpath);
                BitmapImage _aAaa_core = ReturnBitmap(aAaa_core_fullpath);
                BitmapImage _aaaA_core = ReturnBitmap(aaaA_core_fullpath);
                //求出 png 的宽度
                double _aAaa_unit_width = _aAaa_unit.Width;
                double _aaAa_unit_width = _aaAa_unit.Width;
                double _aaaA_unit_width = _aaaA_unit.Width;
                double _aAaa_core_width = _aAaa_core.Width;
                double _aaaA_core_width = _aaaA_core.Width;
                //计算向左移动的margin
                double move_aAaa_core_distiance = _aAaa_core_width / 2 + _aAaa_unit_width + _aaAa_unit_width + _aaaA_unit_width;
                double move_aaaA_core_distiance = _aaaA_core_width / 2 + _aaaA_unit_width;

                //为stackpanel添加图片
                Image aAaa_core = new Image();
                Image aaaA_core = new Image();
                _stackPanel.Children.Add(aAaa_core);//为每个_stackPanel追加核心筒png
                _stackPanel.Children.Add(aaaA_core);//为每个_stackPanel追加核心筒png
                aAaa_core.Source = _aAaa_core;
                aaaA_core.Source = _aaaA_core;
                aAaa_core.HorizontalAlignment = HorizontalAlignment.Left;//为核心筒png设置对齐方式
                aaaA_core.HorizontalAlignment = HorizontalAlignment.Left;
                //调整图像位置
                Thickness aAaa_core_move = new Thickness(-1 * move_aAaa_core_distiance, 0, 0, 0);//设定核心筒向左的偏移量
                Thickness aaaA_core_move = new Thickness(-1 * move_aaaA_core_distiance, 0, 0, 0);//设定核心筒向左的偏移量
                aAaa_core.Margin = aAaa_core_move;
                aaaA_core.Margin = aaaA_core_move;
            }
        }
        //将户型image组合 HXZU_1T2H_aaaaaa_3DY_A_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_aaaaaa_3DY_A_(List<string> TXANameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aaaa
            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaLeft in TXAFullPath)
            {
                //创建一个组合list，所有套型a均可处理为镜像模式
                List<string> targetList = new List<string>();
                string TaRight = TaLeft.Replace("Left", "Right");

                //根据户型组合模式进行列表创建，当前为 aaaaaa
                targetList.Add(TaLeft);//添加每一组合TX文件的full_path
                targetList.Add(TaRight);
                targetList.Add(TaLeft);
                targetList.Add(TaRight);
                targetList.Add(TaLeft);
                targetList.Add(TaRight);

                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                //new的必要性是为了，确保每一个控件都是新创建的
                Addstackpanel_Lst(targetList, stackpanel_Lst);

            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        public void getCore_imgMove_1T2H_aAaAaA_3DY(List<StackPanel> stackpanel_Lst)//对核心筒png的位置进行移动
        //method中的大写字母，代表着与核心筒png计算移动距离的套型所在位置
        {
            //计算便宜量，并列图片之间的移动距离关系，需要单独进行测试，从而得出合理的公式
            string allCOREFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\CORE\"; //所有核心筒文件路径
            string allUNITFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT\"; //所有户型文件路径

            foreach (StackPanel _stackPanel in stackpanel_Lst)
            {
                //拿出StackPanel中png_list的name
                List<string> nameList = new List<string>();
                foreach (object obj in _stackPanel.Children)
                {
                    Image _image = obj as Image;
                    string _imageName = _image.Name;
                    nameList.Add(_imageName);
                }
                //两单元均采用aAaA模式拾取核心筒png
                string aAaaaa_unit_imageName = nameList[1];//存在于aA中A位置的套型
                string aaAaaa_unit_imageName = nameList[2];//存在于aA中A位置的套型
                string aaaAaa_unit_imageName = nameList[3];//存在于aA中A位置的套型
                string aaaaAa_unit_imageName = nameList[4];//存在于aA中A位置的套型
                string aaaaaA_unit_imageName = nameList[5];//存在于aA中A位置的套型
                int aAaaaa_unit_nameLength;
                int aaaAaa_unit_nameLength;
                int aaaaaA_unit_nameLength;
                string aAaaaa_core_imageName = GetHxtPath(aAaaaa_unit_imageName, out aAaaaa_unit_nameLength);//先获取核心筒的name
                string aaaAaa_core_imageName = GetHxtPath(aaaAaa_unit_imageName, out aaaAaa_unit_nameLength);//先获取核心筒的name
                string aaaaaA_core_imageName = GetHxtPath(aaaaaA_unit_imageName, out aaaaaA_unit_nameLength);//先获取核心筒的name

                string aAaaaa_unit_fullpath = allUNITFilePath + aAaaaa_unit_imageName + @".png";//套型png位置
                string aaAaaa_unit_fullpath = allUNITFilePath + aaAaaa_unit_imageName + @".png";//套型png位置
                string aaaAaa_unit_fullpath = allUNITFilePath + aaaAaa_unit_imageName + @".png";//套型png位置
                string aaaaAa_unit_fullpath = allUNITFilePath + aaaaAa_unit_imageName + @".png";//套型png位置
                string aaaaaA_unit_fullpath = allUNITFilePath + aaaaaA_unit_imageName + @".png";//套型png位置
                string aAaaaa_core_fullpath = allCOREFilePath + aAaaaa_core_imageName + @".png";//核心筒png所在位置
                string aaaAaa_core_fullpath = allCOREFilePath + aaaAaa_core_imageName + @".png";//核心筒png所在位置
                string aaaaaA_core_fullpath = allCOREFilePath + aaaaaA_core_imageName + @".png";//核心筒png所在位置

                BitmapImage _aAaaaa_unit = ReturnBitmap(aAaaaa_unit_fullpath);
                BitmapImage _aaAaaa_unit = ReturnBitmap(aaAaaa_unit_fullpath);
                BitmapImage _aaaAaa_unit = ReturnBitmap(aaaAaa_unit_fullpath);
                BitmapImage _aaaaAa_unit = ReturnBitmap(aaaaAa_unit_fullpath);
                BitmapImage _aaaaaA_unit = ReturnBitmap(aaaaaA_unit_fullpath);
                BitmapImage _aAaaaa_core = ReturnBitmap(aAaaaa_core_fullpath);
                BitmapImage _aaaAaa_core = ReturnBitmap(aaaAaa_core_fullpath);
                BitmapImage _aaaaaA_core = ReturnBitmap(aaaaaA_core_fullpath);
                //求出 png 的宽度
                double _aAaaaa_unit_width = _aAaaaa_unit.Width;
                double _aaAaaa_unit_width = _aaAaaa_unit.Width;
                double _aaaAaa_unit_width = _aaaAaa_unit.Width;
                double _aaaaAa_unit_width = _aaaaAa_unit.Width;
                double _aaaaaA_unit_width = _aaaaaA_unit.Width;
                double _aAaaaa_core_width = _aAaaaa_core.Width;
                double _aaaAaa_core_width = _aaaAaa_core.Width;
                double _aaaaaA_core_width = _aaaaaA_core.Width;
                //计算向左移动的margin
                double move_aAaaaa_core_distiance = _aAaaaa_core_width / 2 + _aAaaaa_unit_width + _aaAaaa_unit_width + _aaaAaa_unit_width + _aaaaAa_unit_width + _aaaaaA_unit_width;
                double move_aaaAaa_core_distiance = _aaaAaa_core_width / 2 + _aaaAaa_unit_width + _aaaaAa_unit_width + _aaaaaA_unit_width;
                double move_aaaaaA_core_distiance = _aaaaaA_core_width / 2 + _aaaaaA_unit_width;

                //为stackpanel添加图片
                Image aAaaaa_core = new Image();
                Image aaaAaa_core = new Image();
                Image aaaaaA_core = new Image();
                _stackPanel.Children.Add(aAaaaa_core);//为每个_stackPanel追加核心筒png
                _stackPanel.Children.Add(aaaAaa_core);//为每个_stackPanel追加核心筒png
                _stackPanel.Children.Add(aaaaaA_core);//为每个_stackPanel追加核心筒png
                aAaaaa_core.Source = _aAaaaa_core;
                aaaAaa_core.Source = _aaaAaa_core;
                aaaaaA_core.Source = _aaaaaA_core;
                aAaaaa_core.HorizontalAlignment = HorizontalAlignment.Left;//为核心筒png设置对齐方式
                aaaAaa_core.HorizontalAlignment = HorizontalAlignment.Left;
                aaaaaA_core.HorizontalAlignment = HorizontalAlignment.Left;
                //调整图像位置
                Thickness aAaaaa_core_move = new Thickness(-1 * move_aAaaaa_core_distiance, 0, 0, 0);//设定核心筒向左的偏移量
                Thickness aaaAaa_core_move = new Thickness(-1 * move_aaaAaa_core_distiance, 0, 0, 0);//设定核心筒向左的偏移量
                Thickness aaaaaA_core_move = new Thickness(-1 * move_aaaaaA_core_distiance, 0, 0, 0);//设定核心筒向左的偏移量
                aAaaaa_core.Margin = aAaaaa_core_move;
                aaaAaa_core.Margin = aaaAaa_core_move;
                aaaaaA_core.Margin = aaaaaA_core_move;
            }
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_ab_1DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aaaa

            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();
                    if (TaALeft != TaBLeft)//首先判断户型不同
                    {
                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);
                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的
                        if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                        {
                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aA_1DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abba_2DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aaaa
            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();
                    if (TaALeft != TaBLeft)
                    {
                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);
                        targetList.Add(TaBLeft);
                        string TaARight = TaALeft.Replace("Left", "Right");
                        targetList.Add(TaARight);
                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的

                        if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                        {
                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_aabbaa_3DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aabbaa
            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();
                    if (TaALeft != TaBLeft)
                    {
                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaARight = TaALeft.Replace("Left", "Right");
                        targetList.Add(TaARight);
                        targetList.Add(TaBLeft);
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);
                        targetList.Add(TaALeft);
                        targetList.Add(TaARight);
                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的
                        //if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                        //{
                        //}
                        Addstackpanel_Lst(targetList, stackpanel_Lst);
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abbbba_3DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— abbbba

            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();
                    if (TaALeft != TaBLeft)
                    {
                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);
                        targetList.Add(TaBLeft);
                        targetList.Add(TaBRight);
                        targetList.Add(TaBLeft);
                        string TaARight = TaALeft.Replace("Left", "Right");
                        targetList.Add(TaARight);
                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的
                        if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                        {
                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abbc_2DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— abbbba

            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(TXCNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    if (TaALeft != TaBLeft)
                    {
                        foreach (string TaCLeft in TXCFullPath)
                        {
                            List<string> targetList = new List<string>();
                            if (TaALeft != TaCLeft && TaBLeft != TaCLeft)
                            {
                                targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                string TaBRight = TaBLeft.Replace("Left", "Right");
                                targetList.Add(TaBRight);
                                targetList.Add(TaBLeft);
                                string TaCRight = TaCLeft.Replace("Left", "Right");
                                targetList.Add(TaCRight);
                                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                                //new的必要性是为了，确保每一个控件都是新创建的
                                if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                                {
                                    Addstackpanel_Lst(targetList, stackpanel_Lst);
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_ab_1DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_aabbac_3DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aabbac

            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(TXCNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    if (TaALeft != TaBLeft)//套A与套B不同
                    {
                        foreach (string TaCLeft in TXCFullPath)
                        {
                            List<string> targetList = new List<string>();
                            List<string> _targetList = new List<string>();
                            if (TaALeft != TaCLeft && TaBLeft != TaCLeft)//套A和套B，分别与套C不同
                            {
                                targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                string TaARight = TaALeft.Replace("Left", "Right");
                                targetList.Add(TaARight);
                                targetList.Add(TaBLeft);
                                string TaBRight = TaBLeft.Replace("Left", "Right");
                                targetList.Add(TaBRight);
                                targetList.Add(TaALeft);
                                string TaCRight = TaCLeft.Replace("Left", "Right");
                                targetList.Add(TaCRight);
                                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                                //new的必要性是为了，确保每一个控件都是新创建的

                                _targetList.Add(TaALeft);
                                _targetList.Add(TaCLeft);
                                if (JudgeContainHXT(_targetList))//其次判断，核心筒编号是否相同
                                {
                                    Addstackpanel_Lst(targetList, stackpanel_Lst);
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abbbbc_3DY_ABC_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abbbbc_3DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aabbac

            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(TXCNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    if (TaALeft != TaBLeft)//套A与套B不同
                    {
                        foreach (string TaCLeft in TXCFullPath)
                        {
                            List<string> targetList = new List<string>();
                            List<string> _targetList = new List<string>();
                            List<string> __targetList = new List<string>();
                            if (TaALeft != TaCLeft && TaBLeft != TaCLeft)//套A和套B，分别与套C不同
                            {
                                targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                string TaBRight = TaBLeft.Replace("Left", "Right");
                                targetList.Add(TaBRight);
                                targetList.Add(TaBLeft);
                                targetList.Add(TaBRight);
                                targetList.Add(TaBLeft);
                                string TaCRight = TaCLeft.Replace("Left", "Right");
                                targetList.Add(TaCRight);
                                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                                //new的必要性是为了，确保每一个控件都是新创建的

                                //其次判断，核心筒编号是否相同,套型C不用判断
                                _targetList.Add(TaALeft);
                                _targetList.Add(TaBLeft);
                                __targetList.Add(TaBLeft);
                                __targetList.Add(TaCLeft);

                                if (JudgeContainHXT(_targetList) && JudgeContainHXT(__targetList))//其次判断，核心筒编号是否相同
                                {
                                    Addstackpanel_Lst(targetList, stackpanel_Lst);
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccba_3DY_ABC_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abccba_3DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aabbac

            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //对套型C进行处理，窗口开在山墙面的不要
            List<string> targetTXCNameList = new List<string>();
            foreach (string str in TXCNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXCNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(targetTXCNameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    if (TaALeft != TaBLeft)//套A与套B不同
                    {
                        foreach (string TaCLeft in TXCFullPath)
                        {
                            List<string> targetList = new List<string>();
                            List<string> _targetList = new List<string>();
                            if (TaALeft != TaCLeft && TaBLeft != TaCLeft)//套A和套B，分别与套C不同
                            {
                                targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                string TaBRight = TaBLeft.Replace("Left", "Right");
                                targetList.Add(TaBRight);
                                targetList.Add(TaCLeft);
                                string TaCRight = TaCLeft.Replace("Left", "Right");
                                targetList.Add(TaCRight);
                                targetList.Add(TaBLeft);
                                string TaARight = TaALeft.Replace("Left", "Right");
                                targetList.Add(TaARight);
                                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                                //new的必要性是为了，确保每一个控件都是新创建的

                                //其次判断，核心筒编号是否相同,套型C不用判断
                                _targetList.Add(TaALeft);
                                _targetList.Add(TaBLeft);
                                if (JudgeContainHXT(_targetList))
                                {
                                    Addstackpanel_Lst(targetList, stackpanel_Lst);
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abccbd_3DY_ABCD_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs, List<string> TXDNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aabbac

            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //对套型C进行处理，窗口开在山墙面的不要
            List<string> targetTXCNameList = new List<string>();
            foreach (string str in TXCNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXCNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(targetTXCNameList, huxingPathList);
            List<string> TXDFullPath = GetTaegetFileFullPath(TXDNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    if (TaALeft != TaBLeft)//套A与套B不同
                    {
                        foreach (string TaCLeft in TXCFullPath)
                        {
                            if (TaALeft != TaCLeft && TaBLeft != TaCLeft)//套A和套B，分别与套C不同
                            {
                                foreach (string TaDLeft in TXDFullPath)
                                {
                                    if (TaALeft != TaDLeft && TaBLeft != TaDLeft && TaCLeft != TaDLeft)
                                    {
                                        List<string> targetList = new List<string>();
                                        List<string> _targetList = new List<string>();
                                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                        string TaBRight = TaBLeft.Replace("Left", "Right");
                                        targetList.Add(TaBRight);
                                        targetList.Add(TaCLeft);
                                        string TaCRight = TaCLeft.Replace("Left", "Right");
                                        targetList.Add(TaCRight);
                                        targetList.Add(TaBLeft);
                                        string TaDRight = TaDLeft.Replace("Left", "Right");
                                        targetList.Add(TaDRight);

                                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                                        //new的必要性是为了，确保每一个控件都是新创建的

                                        //其次判断，核心筒编号是否相同,套型C不用判断
                                        _targetList.Add(TaALeft);
                                        _targetList.Add(TaBLeft);
                                        _targetList.Add(TaCLeft);
                                        _targetList.Add(TaDLeft);
                                        if (JudgeContainHXT(_targetList))//其次判断，核心筒编号是否相同
                                        {
                                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_2T4H_abba_1DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abba

            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表
            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();

                    targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                    targetList.Add(TaBLeft);
                    string TaBRight = TaBLeft.Replace("Left", "Right");
                    targetList.Add(TaBRight);
                    string TaARight = TaALeft.Replace("Left", "Right");
                    targetList.Add(TaARight);
                    //对aa组合，进行图片生成处理，需要new image，new stackpanel
                    //new的必要性是为了，确保每一个控件都是新创建的
                    if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                    {
                        Addstackpanel_Lst(targetList, stackpanel_Lst);
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_2T4H_abbA_1DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        public void getCore_imgMove_2T4H_abbA_1DY(List<StackPanel> stackpanel_Lst)//对核心筒png的位置进行移动
        //method中的大写字母，代表着与核心筒png计算移动距离的套型所在位置
        {
            //计算偏移量，并列图片之间的移动距离关系，需要单独进行测试，从而得出合理的公式
            string allCOREFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\CORE\"; //所有核心筒文件路径
            string allUNITFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT\"; //所有户型文件路径

            foreach (StackPanel _stackPanel in stackpanel_Lst)
            {
                //拿出StackPanel中png_list的name
                Image Abba_unit_img = _stackPanel.Children[0] as Image;
                Image aBba_unit_img = _stackPanel.Children[1] as Image;
                Image abBa_unit_img = _stackPanel.Children[2] as Image;
                Image abbA_unit_img = _stackPanel.Children[3] as Image;
                //1T2H采用abbA模式拾取核心筒png
                string Abba_unit_imageName = Abba_unit_img.Name;//存在于aA中A位置的套型
                string aBba_unit_imageName = aBba_unit_img.Name;//存在于aA中A位置的套型
                string abBa_unit_imageName = abBa_unit_img.Name;//存在于aA中A位置的套型
                string abbA_unit_imageName = abbA_unit_img.Name;//存在于aA中A位置的套型
                int abbA_unit_nameLength;
                string abbA_core_imageName = GetHxtPath(abbA_unit_imageName, out abbA_unit_nameLength);//先获取核心筒的name

                string aBba_unit_fullpath = allUNITFilePath + aBba_unit_imageName + @".png";//套型png位置
                string abBa_unit_fullpath = allUNITFilePath + abBa_unit_imageName + @".png";//套型png位置
                string abbA_unit_fullpath = allUNITFilePath + abbA_unit_imageName + @".png";//套型png位置
                string abbA_core_fullpath = allCOREFilePath + abbA_core_imageName + @".png";//核心筒png所在位置

                BitmapImage _aBba_unit = ReturnBitmap(aBba_unit_fullpath);
                BitmapImage _abBa_unit = ReturnBitmap(abBa_unit_fullpath);
                BitmapImage _abbA_unit = ReturnBitmap(abbA_unit_fullpath);
                BitmapImage _abbA_core = ReturnBitmap(abbA_core_fullpath);
                //求出 png 的宽度
                double _aBba_unit_width = _aBba_unit.Width;
                double _abBa_unit_width = _abBa_unit.Width;
                double _abbA_unit_width = _abbA_unit.Width;
                double _abbA_core_width = _abbA_core.Width;
                //计算向左移动的margin
                double move_abbA_core_distiance_L = _aBba_unit_width + _abBa_unit_width + _abbA_unit_width;

                //为stackpanel添加图片
                Image abbA_core_L = new Image();
                _stackPanel.Children.Add(abbA_core_L);//为每个_stackPanel追加核心筒png
                abbA_core_L.Source = _abbA_core;
                abbA_core_L.HorizontalAlignment = HorizontalAlignment.Left;//为核心筒png设置对齐方式
                //调整图像位置
                //首先进行条件判断，判断是否为连廊式核心筒 是则创建一个与其镜像的元素 同时添加核心筒连廊
                if (!abbA_core_imageName.Contains(@"CORE_5408") && !abbA_core_imageName.Contains(@"CORE_335406"))//core_335406为1T4H core_5408为剪刀楼梯
                {
                    //1、核心筒是否需要镜像的问题
                    //计算向左移动的margin
                    double move_abbA_core_distiance_R = _abbA_core_width + _abbA_unit_width;
                    //为stackpanel添加图片
                    Image abbA_core_R = new Image();
                    _stackPanel.Children.Add(abbA_core_R);//为每个_stackPanel追加核心筒png
                    abbA_core_R.Source = _abbA_core;
                    abbA_core_R.HorizontalAlignment = HorizontalAlignment.Left;
                    //镜像设置
                    image_mirrorHorizontal_(abbA_core_R);
                    //调整图像位置
                    //abba abbc 求出0与 3 位置的 偏移量
                    int Abba_unit_imageName_length = Abba_unit_imageName.Length;
                    int abbA_unit_imageName_length = abbA_unit_imageName.Length;
                    string Abba_overlap_distance_path = allUNITFilePath + Abba_unit_imageName.Substring(0, Abba_unit_imageName_length - 11) + @".txt";
                    string abbA_overlap_distance_path = allUNITFilePath + abbA_unit_imageName.Substring(0, abbA_unit_imageName_length - 12) + @".txt";

                    List<string> Abba_txt = ReadTxt(Abba_overlap_distance_path);//读取txt文件里面的数据；
                    List<string> abbA_txt = ReadTxt(abbA_overlap_distance_path);

                    string Abba_overlay_up = Abba_txt[0];//overlay 拿出变量所在行的数据
                    string abbA_overlay_up = abbA_txt[0];//overlay 取dowm值   
                    string Abba_overlay_down = Abba_txt[1];//overlay 拿出变量所在行的数据
                    string abbA_overlay_down = abbA_txt[1];//overlay 取dowm值
                    //MessageBox.Show(Abba_overlay_up);
                    //MessageBox.Show(abbA_overlay_up);
                    //MessageBox.Show(Abba_overlay_down);
                    //MessageBox.Show(abbA_overlay_down);

                    //读取overlay对应的数值
                    double Abba_overlap_distance_dou_up = GetDoubleFromString(Abba_overlay_up);//读出png上凹凸处的左右宽度
                    double abbA_overlap_distance_dou_up = GetDoubleFromString(abbA_overlay_up);//读出png上凹凸处的左右宽度      
                    double Abba_overlap_distance_dou_down = GetDoubleFromString(Abba_overlay_down);//读出png上凹凸处的左右宽度
                    double abbA_overlap_distance_dou_down = GetDoubleFromString(abbA_overlay_down);//读出png上凹凸处的左右宽度

                    Thickness aBba_move = new Thickness(-1 * Abba_overlap_distance_dou_down / 3.13, 0, 0, 0);//aBba,大写字母位置的户型向左进行移动
                    Thickness abbA_move = new Thickness(-1 * abbA_overlap_distance_dou_down / 3.13, 0, 0, 0);//abbA,大写字母位置的户型向左进行移动
                    //需要推到走廊核心筒两个端部的数据
                    //判断核心筒部件与相邻套型png的关系
                    Thickness abbA_core_move_L = new Thickness(-1 * move_abbA_core_distiance_L + Abba_overlap_distance_dou_down / 3.13 + abbA_overlap_distance_dou_down / 3.13 - Abba_overlap_distance_dou_up / 3.13, 0, 0, 0);//设定核心筒向左的偏移量
                    Thickness abbA_core_move_R = new Thickness(-1 * move_abbA_core_distiance_R + abbA_overlap_distance_dou_up / 3.13, 0, 0, 0);//设定核心筒向左的偏移量

                    aBba_unit_img.Margin = aBba_move;//图像移动后，其后面的图像会跟随其运动；
                    abbA_unit_img.Margin = abbA_move;
                    abbA_core_L.Margin = abbA_core_move_L;
                    abbA_core_R.Margin = abbA_core_move_R;

                    //2、加核心筒走廊的策略
                    string _core_vestibule_path = allCOREFilePath + abbA_core_imageName + @"_vestibule.png";//核心筒连廊所在位置
                    int count_vest = Convert.ToInt32(Math.Ceiling((abbA_core_move_R.Left - abbA_core_move_L.Left - _abbA_core_width) / 16));//除以单个走廊的png宽度
                    BitmapImage _core_vestibule = ReturnBitmap(_core_vestibule_path);
                    //图像png像素宽度为300，对应wpf宽度为50
                    for (int i = 1; i <= count_vest; i++)
                    {
                        Image _core_vest = new Image();
                        _stackPanel.Children.Add(_core_vest);//为每个_stackPanel追加核心筒png
                        _core_vest.Source = _core_vestibule;
                        _core_vest.HorizontalAlignment = HorizontalAlignment.Left;
                        _core_vest.VerticalAlignment = VerticalAlignment.Top;
                        double vest_movedistance = -abbA_core_move_R.Left + _core_vestibule.Width * i;
                        Thickness vest_move = new Thickness(-1 * vest_movedistance, 0, 0, 0);
                        _core_vest.Margin = vest_move;
                    }
                }
                //else if (abbA_core_imageName.Contains(@"CORE_5408"))
                else if (abbA_core_imageName.Contains(@"CORE_335406"))
                {
                    //abba abbc 求出0与 3 位置的 偏移量
                    int Abba_unit_imageName_length = Abba_unit_imageName.Length;
                    int abbA_unit_imageName_length = abbA_unit_imageName.Length;
                    string Abba_overlap_distance_path = allUNITFilePath + Abba_unit_imageName.Substring(0, Abba_unit_imageName_length - 11) + @".txt";
                    string abbA_overlap_distance_path = allUNITFilePath + abbA_unit_imageName.Substring(0, abbA_unit_imageName_length - 12) + @".txt";

                    List<string> Abba_txt = ReadTxt(Abba_overlap_distance_path);//读取txt文件里面的数据；
                    List<string> abbA_txt = ReadTxt(abbA_overlap_distance_path);

                    string Abba_overlay = Abba_txt[0];//overlay 拿出变量所在行的数据
                    string abbA_overlay = abbA_txt[0];//overlay

                    //读取overlay对应的数值
                    double Abba_overlap_distance_double = GetDoubleFromString(Abba_overlay);
                    double abbA_overlap_distance_double = GetDoubleFromString(abbA_overlay);

                    //对套型图片位置，设置距离进行移动
                    Thickness aBbA_move = new Thickness(-1 * Abba_overlap_distance_double / 3.13, 0, 0, 0);//aBbA,大写字母位置的户型向左进行移动
                    aBba_unit_img.Margin = aBbA_move;//图像移动后，其后面的图像会跟随其运动；
                    abbA_unit_img.Margin = aBbA_move;
                    Thickness abbA_core_move_L = new Thickness(-1 * move_abbA_core_distiance_L + Abba_overlap_distance_double / 3.13, 0, 0, 0);//调整核心筒的位置
                    abbA_core_L.Margin = abbA_core_move_L;
                }
                else if (abbA_core_imageName.Contains(@"CORE_5408"))
                {
                    //abba abbc 求出0与 3 位置的 偏移量
                    int Abba_unit_imageName_length = Abba_unit_imageName.Length;
                    int abbA_unit_imageName_length = abbA_unit_imageName.Length;
                    string Abba_overlap_distance_path = allUNITFilePath + Abba_unit_imageName.Substring(0, Abba_unit_imageName_length - 11) + @".txt";
                    string abbA_overlap_distance_path = allUNITFilePath + abbA_unit_imageName.Substring(0, abbA_unit_imageName_length - 12) + @".txt";

                    List<string> Abba_txt = ReadTxt(Abba_overlap_distance_path);//读取txt文件里面的数据；
                    List<string> abbA_txt = ReadTxt(abbA_overlap_distance_path);

                    string Abba_overlay = Abba_txt[0];//overlay 拿出变量所在行的数据
                    string abbA_overlay = abbA_txt[0];//overlay

                    //读取overlay对应的数值
                    double Abba_overlap_distance_double = GetDoubleFromString(Abba_overlay);
                    double abbA_overlap_distance_double = GetDoubleFromString(abbA_overlay);

                    //对套型图片位置，设置距离进行移动
                    Thickness aBbA_move = new Thickness(-1 * Abba_overlap_distance_double / 3.13, 0, 0, 0);//aBbA,大写字母位置的户型向左进行移动
                    aBba_unit_img.Margin = aBbA_move;//图像移动后，其后面的图像会跟随其运动；
                    abbA_unit_img.Margin = aBbA_move;
                    Thickness abbA_core_move_L = new Thickness(-1 * move_abbA_core_distiance_L + Abba_overlap_distance_double / 3.13 + abbA_overlap_distance_double / 3.13, 0, 0, 0);//调整核心筒的位置
                    abbA_core_L.Margin = abbA_core_move_L;
                }
            }
        }
        /// <summary>
        /// 将图像水平镜像
        /// </summary>
        public void image_mirrorHorizontal_(Image abbA_core_R)//图像水平镜像设置
        {
            //镜像设置
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform(-1, 1);//水平镜像设置
            transformGroup.Children.Add(scaleTransform);
            abbA_core_R.LayoutTransform = transformGroup;
        }
        /// <summary>
        /// 获取一个字符串里面的数字，要求字符串末尾 = 后面只显示一段数字
        /// </summary>
        /// <returns></returns>
        public double GetDoubleFromString(string Abba_overlay)
        {
            int Abba_overlay_length = Abba_overlay.Length;
            int index = Abba_overlay.IndexOf("=");
            string Abba_overlap_distance_ = "";

            for (int i = index + 1; i < Abba_overlay_length; i++)
            {

                string temp_str = Abba_overlay.Substring(i, 1);
                double temp_doub = 0;
                if (double.TryParse(temp_str, out temp_doub))
                {
                    Abba_overlap_distance_ += temp_str;
                }
            }
            double Abba_overlap_distance_double = 0;
            double.TryParse(Abba_overlap_distance_, out Abba_overlap_distance_double);
            return Abba_overlap_distance_double;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_2T4H_abbaabba_2DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abba
            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();

                    targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                    targetList.Add(TaBLeft);
                    string TaBRight = TaBLeft.Replace("Left", "Right");
                    targetList.Add(TaBRight);
                    string TaARight = TaALeft.Replace("Left", "Right");
                    targetList.Add(TaARight);
                    targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                    targetList.Add(TaBLeft);
                    targetList.Add(TaBRight);
                    targetList.Add(TaARight);
                    //对aa组合，进行图片生成处理，需要new image，new stackpanel
                    //new的必要性是为了，确保每一个控件都是新创建的
                    if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                    {
                        Addstackpanel_Lst(targetList, stackpanel_Lst);
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_2T4H_abbaabbA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        public void getCore_imgMove_2T4H_abbaabbA_2DY(List<StackPanel> stackpanel_Lst)//对核心筒png的位置进行移动
        //method中的大写字母，代表着与核心筒png计算移动距离的套型所在位置
        {
            //计算偏移量，并列图片之间的移动距离关系，需要单独进行测试，从而得出合理的公式
            string allCOREFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\CORE\"; //所有核心筒文件路径
            string allUNITFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT\"; //所有户型文件路径

            foreach (StackPanel _stackPanel in stackpanel_Lst)
            {
                //拿出StackPanel中png_list的name
                Image Abbaabba_unit_img = _stackPanel.Children[0] as Image;
                Image aBbaabba_unit_img = _stackPanel.Children[1] as Image;
                Image abBaabba_unit_img = _stackPanel.Children[2] as Image;
                Image abbAabba_unit_img = _stackPanel.Children[3] as Image;
                Image abbaAbba_unit_img = _stackPanel.Children[4] as Image;
                Image abbaaBba_unit_img = _stackPanel.Children[5] as Image;
                Image abbaabBa_unit_img = _stackPanel.Children[6] as Image;
                Image abbaabbA_unit_img = _stackPanel.Children[7] as Image;

                //两单元均采用abbA模式拾取核心筒png
                string Abbaabba_unit_imageName = Abbaabba_unit_img.Name;//存在于aA中A位置的套型
                string aBbaabba_unit_imageName = aBbaabba_unit_img.Name;//存在于aA中A位置的套型
                string abBaabba_unit_imageName = abBaabba_unit_img.Name;//存在于aA中A位置的套型
                string abbAabba_unit_imageName = abbAabba_unit_img.Name;//存在于aA中A位置的套型
                string abbaAbba_unit_imageName = abbaAbba_unit_img.Name;//存在于aA中A位置的套型
                string abbaaBba_unit_imageName = abbaaBba_unit_img.Name;//存在于aA中A位置的套型
                string abbaabBa_unit_imageName = abbaabBa_unit_img.Name;//存在于aA中A位置的套型
                string abbaabbA_unit_imageName = abbaabbA_unit_img.Name;//存在于aA中A位置的套型
                int abbA_unit_nameLength;
                string abbAabba_core_imageName = GetHxtPath(abbAabba_unit_imageName, out abbA_unit_nameLength);//先获取核心筒的name

                string Abbaabba_unit_fullpath = allUNITFilePath + Abbaabba_unit_imageName + @".png";//套型png位置
                string aBbaabba_unit_fullpath = allUNITFilePath + aBbaabba_unit_imageName + @".png";//套型png位置
                string abBaabba_unit_fullpath = allUNITFilePath + abBaabba_unit_imageName + @".png";//套型png位置
                string abbAabba_unit_fullpath = allUNITFilePath + abbAabba_unit_imageName + @".png";//套型png位置
                string abbaAbba_unit_fullpath = allUNITFilePath + abbaAbba_unit_imageName + @".png";//套型png位置
                string abbaaBba_unit_fullpath = allUNITFilePath + abbaaBba_unit_imageName + @".png";//套型png位置
                string abbaabBa_unit_fullpath = allUNITFilePath + abbaabBa_unit_imageName + @".png";//套型png位置
                string abbaabbA_unit_fullpath = allUNITFilePath + abbaabbA_unit_imageName + @".png";//套型png位置
                string abbAabba_core_fullpath = allCOREFilePath + abbAabba_core_imageName + @".png";//核心筒png所在位置

                BitmapImage _Abbaabba_unit = ReturnBitmap(Abbaabba_unit_fullpath);
                BitmapImage _aBbaabba_unit = ReturnBitmap(aBbaabba_unit_fullpath);
                BitmapImage _abBaabba_unit = ReturnBitmap(abBaabba_unit_fullpath);
                BitmapImage _abbAabba_unit = ReturnBitmap(abbAabba_unit_fullpath);
                BitmapImage _abbaAbba_unit = ReturnBitmap(abbaAbba_unit_fullpath);
                BitmapImage _abbaaBba_unit = ReturnBitmap(abbaaBba_unit_fullpath);
                BitmapImage _abbaabBa_unit = ReturnBitmap(abbaabBa_unit_fullpath);
                BitmapImage _abbaabbA_unit = ReturnBitmap(abbaabbA_unit_fullpath);
                BitmapImage _abbAabba_core = ReturnBitmap(abbAabba_core_fullpath);
                //求出 png 的宽度
                double _Abbaabba_unit_width = _Abbaabba_unit.Width;
                double _aBbaabba_unit_width = _aBbaabba_unit.Width;
                double _abBaabba_unit_width = _abBaabba_unit.Width;
                double _abbAabba_unit_width = _abbAabba_unit.Width;
                double _abbaAbba_unit_width = _abbaAbba_unit.Width;
                double _abbaaBba_unit_width = _abbaaBba_unit.Width;
                double _abbaabBa_unit_width = _abbaabBa_unit.Width;
                double _abbaabbA_unit_width = _abbaabbA_unit.Width;
                double _abbAabba_core_width = _abbAabba_core.Width;
                //计算向左移动的margin
                double move_abbAabba_core_distiance_L = _aBbaabba_unit_width + _abBaabba_unit_width + _abbAabba_unit_width + _abbaAbba_unit_width + _abbaaBba_unit_width + _abbaabBa_unit_width + _abbaabbA_unit_width;
                double move_abbaabbA_core_distiance_L = _abbaaBba_unit_width + _abbaabBa_unit_width + _abbaabbA_unit_width;

                //为stackpanel添加图片
                Image abbAabba_core_L = new Image();
                Image abbaabbA_core_L = new Image();
                _stackPanel.Children.Add(abbAabba_core_L);//为每个_stackPanel追加核心筒png
                _stackPanel.Children.Add(abbaabbA_core_L);//为每个_stackPanel追加核心筒png
                abbAabba_core_L.Source = _abbAabba_core;
                abbaabbA_core_L.Source = _abbAabba_core;
                abbAabba_core_L.HorizontalAlignment = HorizontalAlignment.Left;//为核心筒png设置对齐方式
                abbaabbA_core_L.HorizontalAlignment = HorizontalAlignment.Left;
                //调整图像位置
                if (!abbAabba_core_imageName.Contains(@"CORE_5408") && !abbAabba_core_imageName.Contains(@"CORE_335406"))
                {
                    //1、核心筒是否需要镜像的问题
                    //计算向左移动的margin
                    double move_abbAabba_core_distiance_R = _abbAabba_core_width + _abbAabba_unit_width + _abbaAbba_unit_width + _abbaaBba_unit_width + _abbaabBa_unit_width + _abbaabbA_unit_width;
                    double move_abbaabbA_core_distiance_R = _abbAabba_core_width + _abbaabbA_unit_width;
                    //为stackpanel添加图片
                    Image abbAabba_core_R = new Image();
                    Image abbaabbA_core_R = new Image();
                    _stackPanel.Children.Add(abbAabba_core_R);//为每个_stackPanel追加镜像的核心筒png
                    _stackPanel.Children.Add(abbaabbA_core_R);
                    abbAabba_core_R.Source = _abbAabba_core;
                    abbaabbA_core_R.Source = _abbAabba_core;
                    abbAabba_core_R.HorizontalAlignment = HorizontalAlignment.Left;
                    abbaabbA_core_R.HorizontalAlignment = HorizontalAlignment.Left;
                    //镜像设置
                    image_mirrorHorizontal_(abbAabba_core_R);
                    image_mirrorHorizontal_(abbaabbA_core_R);
                    //调整图像位置

                    //abba abbc 求出0与 3 位置的 偏移量
                    int Abbaabba_unit_imageName_length = Abbaabba_unit_imageName.Length;
                    int abbAabba_unit_imageName_length = abbAabba_unit_imageName.Length;
                    int abbaAbba_unit_imageName_length = abbaAbba_unit_imageName.Length;
                    int abbaabbA_unit_imageName_length = abbaabbA_unit_imageName.Length;


                    string Abbaabba_overlap_distance_path = allUNITFilePath + Abbaabba_unit_imageName.Substring(0, Abbaabba_unit_imageName_length - 11) + @".txt";
                    string abbAabba_overlap_distance_path = allUNITFilePath + abbAabba_unit_imageName.Substring(0, abbAabba_unit_imageName_length - 12) + @".txt";
                    string abbaAbba_overlap_distance_path = allUNITFilePath + abbaAbba_unit_imageName.Substring(0, abbaAbba_unit_imageName_length - 11) + @".txt";
                    string abbaabbA_overlap_distance_path = allUNITFilePath + abbaabbA_unit_imageName.Substring(0, abbaabbA_unit_imageName_length - 12) + @".txt";

                    List<string> Abbaabba_txt = ReadTxt(Abbaabba_overlap_distance_path);//读取txt文件里面的数据；
                    List<string> abbAabba_txt = ReadTxt(abbAabba_overlap_distance_path);
                    List<string> abbaAbba_txt = ReadTxt(abbaAbba_overlap_distance_path);
                    List<string> abbaabbA_txt = ReadTxt(abbaabbA_overlap_distance_path);

                    string Abbaabba_overlay_up = Abbaabba_txt[0];//overlay 拿出变量所在行的数据
                    string abbAabba_overlay_up = abbAabba_txt[0];//overlay 取dowm值   
                    string abbaAbba_overlay_up = abbaAbba_txt[0];//overlay 取dowm值   
                    string abbaabbA_overlay_up = abbaabbA_txt[0];//overlay 取dowm值   
                    string Abbaabba_overlay_down = Abbaabba_txt[1];//overlay 拿出变量所在行的数据
                    string abbAabba_overlay_down = abbAabba_txt[1];//overlay 取dowm值
                    string abbaAbba_overlay_down = abbaAbba_txt[1];//overlay 取dowm值
                    string abbaabbA_overlay_down = abbaabbA_txt[1];//overlay 取dowm值
                    //MessageBox.Show(Abba_overlay_up);
                    //MessageBox.Show(abbA_overlay_up);
                    //MessageBox.Show(Abba_overlay_down);
                    //MessageBox.Show(abbA_overlay_down);

                    //读取overlay对应的数值
                    double Abbaabba_overlap_distance_dou_up = GetDoubleFromString(Abbaabba_overlay_up);//读出png上凹凸处的左右宽度
                    double abbAabba_overlap_distance_dou_up = GetDoubleFromString(abbAabba_overlay_up);//读出png上凹凸处的左右宽度      
                    double abbaAbba_overlap_distance_dou_up = GetDoubleFromString(abbaAbba_overlay_up);//读出png上凹凸处的左右宽度      
                    double abbaabbA_overlap_distance_dou_up = GetDoubleFromString(abbaabbA_overlay_up);//读出png上凹凸处的左右宽度      
                    double Abbaabba_overlap_distance_dou_down = GetDoubleFromString(Abbaabba_overlay_down);//读出png上凹凸处的左右宽度
                    double abbAabba_overlap_distance_dou_down = GetDoubleFromString(abbAabba_overlay_down);//读出png上凹凸处的左右宽度
                    double abbaAbba_overlap_distance_dou_down = GetDoubleFromString(abbaAbba_overlay_down);//读出png上凹凸处的左右宽度
                    double abbaabbA_overlap_distance_dou_down = GetDoubleFromString(abbaabbA_overlay_down);//读出png上凹凸处的左右宽度

                    Thickness aBbaabba_move = new Thickness(-1 * Abbaabba_overlap_distance_dou_down / 3.13, 0, 0, 0);//aBba,大写字母位置的户型向左进行移动
                    Thickness abbAabba_move = new Thickness(-1 * abbAabba_overlap_distance_dou_down / 3.13, 0, 0, 0);//abbA,大写字母位置的户型向左进行移动
                    Thickness abbaBbba_move = new Thickness(-1 * abbaAbba_overlap_distance_dou_down / 3.13, 0, 0, 0);//abbA,大写字母位置的户型向左进行移动
                    Thickness abbaabbA_move = new Thickness(-1 * abbaabbA_overlap_distance_dou_down / 3.13, 0, 0, 0);//abbA,大写字母位置的户型向左进行移动
                    //需要推到走廊核心筒两个端部的数据
                    //判断核心筒部件与相邻套型png的关系
                    Thickness abbAabba_core_move_L = new Thickness(-1 * move_abbAabba_core_distiance_L + Abbaabba_overlap_distance_dou_down / 3.13 + abbAabba_overlap_distance_dou_down / 3.13 + abbaAbba_overlap_distance_dou_down / 3.13 + abbaabbA_overlap_distance_dou_down / 3.13 - Abbaabba_overlap_distance_dou_up / 3.13, 0, 0, 0);//设定核心筒向左的偏移量
                    Thickness abbaabbA_core_move_L = new Thickness(-1 * move_abbaabbA_core_distiance_L + abbaAbba_overlap_distance_dou_down / 3.13 + abbaabbA_overlap_distance_dou_down / 3.13 - abbAabba_overlap_distance_dou_up / 3.13, 0, 0, 0);//设定核心筒向左的偏移量
                    Thickness abbAabba_core_move_R = new Thickness(-1 * move_abbAabba_core_distiance_R + abbaAbba_overlap_distance_dou_down / 3.13 + abbaabbA_overlap_distance_dou_down / 3.13 + abbaAbba_overlap_distance_dou_up / 3.13, 0, 0, 0);//设定核心筒向左的偏移量
                    Thickness abbaabbA_core_move_R = new Thickness(-1 * move_abbaabbA_core_distiance_R + abbaabbA_overlap_distance_dou_up / 3.13, 0, 0, 0);//设定核心筒向左的偏移量

                    aBbaabba_unit_img.Margin = aBbaabba_move;//图像移动后，其后面的图像会跟随其运动；
                    abbAabba_unit_img.Margin = abbAabba_move;
                    abbaaBba_unit_img.Margin = abbaBbba_move;
                    abbaabbA_unit_img.Margin = abbaabbA_move;

                    abbAabba_core_L.Margin = abbAabba_core_move_L;
                    abbaabbA_core_L.Margin = abbaabbA_core_move_L;
                    abbAabba_core_R.Margin = abbAabba_core_move_R;
                    abbaabbA_core_R.Margin = abbaabbA_core_move_R;

                    //2、加核心筒走廊的策略
                    string _core_vestibule_path = allCOREFilePath + abbAabba_core_imageName + @"_vestibule.png";//核心筒连廊所在位置
                    BitmapImage _core_vestibule = ReturnBitmap(_core_vestibule_path);
                    //图像png像素宽度为300，对应wpf宽度为50
                    int count_vest_L = Convert.ToInt32(Math.Ceiling((abbAabba_core_move_R.Left - abbAabba_core_move_L.Left - _abbAabba_core_width) / 16));//除以单个走廊的png宽度
                    int count_vest_R = Convert.ToInt32(Math.Ceiling((abbaabbA_core_move_R.Left - abbaabbA_core_move_L.Left - _abbAabba_core_width) / 16));//除以单个走廊的png宽度
                    for (int i = 1; i <= count_vest_L; i++)
                    {
                        Image _core_vest_1 = new Image();
                        _stackPanel.Children.Add(_core_vest_1);//为每个_stackPanel追加核心筒png
                        _core_vest_1.Source = _core_vestibule;
                        _core_vest_1.HorizontalAlignment = HorizontalAlignment.Left;
                        _core_vest_1.VerticalAlignment = VerticalAlignment.Top;
                        double vest_movedistance_1 = -abbAabba_core_move_R.Left + _core_vestibule.Width * i;
                        Thickness vest_move_1 = new Thickness(-1 * vest_movedistance_1, 0, 0, 0);
                        _core_vest_1.Margin = vest_move_1;
                    }
                    for (int i = 1; i <= count_vest_R; i++)
                    {
                        Image _core_vest_1 = new Image();
                        _stackPanel.Children.Add(_core_vest_1);//为每个_stackPanel追加核心筒png
                        _core_vest_1.Source = _core_vestibule;
                        _core_vest_1.HorizontalAlignment = HorizontalAlignment.Left;
                        _core_vest_1.VerticalAlignment = VerticalAlignment.Top;
                        double vest_movedistance_1 = -abbaabbA_core_move_R.Left + _core_vestibule.Width * i;
                        Thickness vest_move_1 = new Thickness(-1 * vest_movedistance_1, 0, 0, 0);
                        _core_vest_1.Margin = vest_move_1;
                    }
                }
                else if (abbAabba_core_imageName.Contains(@"CORE_335406"))
                {
                    //如何读取图片png的咬合距离
                    //abba abbc 求出0与 3 位置的 偏移量
                    int Abbaabba_unit_imageName_length = Abbaabba_unit_imageName.Length;
                    int abbAabba_unit_imageName_length = abbAabba_unit_imageName.Length;
                    int abbaAbba_unit_imageName_length = abbaAbba_unit_imageName.Length;
                    int abbaabbA_unit_imageName_length = abbaabbA_unit_imageName.Length;
                    string Abbaabba_overlap_distance_path = allUNITFilePath + Abbaabba_unit_imageName.Substring(0, Abbaabba_unit_imageName_length - 11) + @".txt";
                    string abbAabba_overlap_distance_path = allUNITFilePath + abbAabba_unit_imageName.Substring(0, abbAabba_unit_imageName_length - 12) + @".txt";
                    string abbaAbba_overlap_distance_path = allUNITFilePath + abbaAbba_unit_imageName.Substring(0, abbaAbba_unit_imageName_length - 11) + @".txt";
                    string abbaabbA_overlap_distance_path = allUNITFilePath + abbaabbA_unit_imageName.Substring(0, abbaabbA_unit_imageName_length - 12) + @".txt";

                    List<string> Abbaabba_txt = ReadTxt(Abbaabba_overlap_distance_path);//读取txt文件里面的数据；
                    List<string> abbAabba_txt = ReadTxt(abbAabba_overlap_distance_path);
                    List<string> abbaAbba_txt = ReadTxt(abbaAbba_overlap_distance_path);
                    List<string> abbaabbA_txt = ReadTxt(abbaabbA_overlap_distance_path);

                    string Abbaabba_overlay = Abbaabba_txt[0];//overlay 拿出变量所在行的数据
                    string abbAabba_overlay = abbAabba_txt[0];//overlay
                    string abbaAbba_overlay = abbaAbba_txt[0];//overlay
                    string abbaabbA_overlay = abbaabbA_txt[0];//overlay

                    //读取overlay对应的数值
                    double Abbaabba_overlap_distance_double = GetDoubleFromString(Abbaabba_overlay);
                    double abbAabba_overlap_distance_double = GetDoubleFromString(abbAabba_overlay);
                    double abbaAbba_overlap_distance_double = GetDoubleFromString(abbaAbba_overlay);
                    double abbaabbA_overlap_distance_double = GetDoubleFromString(abbaabbA_overlay);

                    Thickness aBbAabba_move = new Thickness(-1 * Abbaabba_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    Thickness abbAabba_move = new Thickness(-1 * abbAabba_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    Thickness abbaAbba_move = new Thickness(-1 * abbaAbba_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    Thickness abbaabbA_move = new Thickness(-1 * abbaabbA_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    aBbaabba_unit_img.Margin = aBbAabba_move;
                    abbAabba_unit_img.Margin = abbAabba_move;
                    abbaaBba_unit_img.Margin = abbaAbba_move;
                    abbaabbA_unit_img.Margin = abbaabbA_move;
                    Thickness abbAabba_core_move_L = new Thickness(-1 * move_abbAabba_core_distiance_L + abbAabba_overlap_distance_double / 3.13 + abbaAbba_overlap_distance_double / 3.13 + abbaabbA_overlap_distance_double / 3.13, 0, 0, 0);//调整核心筒的位置
                    Thickness abbaabbA_core_move_L = new Thickness(-1 * move_abbaabbA_core_distiance_L + abbaabbA_overlap_distance_double / 3.13, 0, 0, 0);//调整核心筒的位置
                    abbAabba_core_L.Margin = abbAabba_core_move_L;
                    abbaabbA_core_L.Margin = abbaabbA_core_move_L;
                }
                else if (abbAabba_core_imageName.Contains(@"CORE_5408"))
                {
                    //如何读取图片png的咬合距离
                    //abba abbc 求出0与 3 位置的 偏移量
                    int Abbaabba_unit_imageName_length = Abbaabba_unit_imageName.Length;
                    int abbAabba_unit_imageName_length = abbAabba_unit_imageName.Length;
                    int abbaAbba_unit_imageName_length = abbaAbba_unit_imageName.Length;
                    int abbaabbA_unit_imageName_length = abbaabbA_unit_imageName.Length;
                    string Abbaabba_overlap_distance_path = allUNITFilePath + Abbaabba_unit_imageName.Substring(0, Abbaabba_unit_imageName_length - 11) + @".txt";
                    string abbAabba_overlap_distance_path = allUNITFilePath + abbAabba_unit_imageName.Substring(0, abbAabba_unit_imageName_length - 12) + @".txt";
                    string abbaAbba_overlap_distance_path = allUNITFilePath + abbaAbba_unit_imageName.Substring(0, abbaAbba_unit_imageName_length - 11) + @".txt";
                    string abbaabbA_overlap_distance_path = allUNITFilePath + abbaabbA_unit_imageName.Substring(0, abbaabbA_unit_imageName_length - 12) + @".txt";

                    List<string> Abbaabba_txt = ReadTxt(Abbaabba_overlap_distance_path);//读取txt文件里面的数据；
                    List<string> abbAabba_txt = ReadTxt(abbAabba_overlap_distance_path);
                    List<string> abbaAbba_txt = ReadTxt(abbaAbba_overlap_distance_path);
                    List<string> abbaabbA_txt = ReadTxt(abbaabbA_overlap_distance_path);

                    string Abbaabba_overlay = Abbaabba_txt[0];//overlay 拿出变量所在行的数据
                    string abbAabba_overlay = abbAabba_txt[0];//overlay
                    string abbaAbba_overlay = abbaAbba_txt[0];//overlay
                    string abbaabbA_overlay = abbaabbA_txt[0];//overlay

                    //读取overlay对应的数值
                    double Abbaabba_overlap_distance_double = GetDoubleFromString(Abbaabba_overlay);
                    double abbAabba_overlap_distance_double = GetDoubleFromString(abbAabba_overlay);
                    double abbaAbba_overlap_distance_double = GetDoubleFromString(abbaAbba_overlay);
                    double abbaabbA_overlap_distance_double = GetDoubleFromString(abbaabbA_overlay);

                    Thickness aBbAabba_move = new Thickness(-1 * Abbaabba_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    Thickness abbAabba_move = new Thickness(-1 * abbAabba_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    Thickness abbaAbba_move = new Thickness(-1 * abbaAbba_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    Thickness abbaabbA_move = new Thickness(-1 * abbaabbA_overlap_distance_double / 3.13, 0, 0, 0);//aBbAabba,大写字母位置的户型向左进行移动
                    aBbaabba_unit_img.Margin = aBbAabba_move;
                    abbAabba_unit_img.Margin = abbAabba_move;
                    abbaaBba_unit_img.Margin = abbaAbba_move;
                    abbaabbA_unit_img.Margin = abbaabbA_move;
                    Thickness abbAabba_core_move_L = new Thickness(-1 * move_abbAabba_core_distiance_L + Abbaabba_overlap_distance_double / 3.13 + abbAabba_overlap_distance_double / 3.13 + abbaAbba_overlap_distance_double / 3.13 + abbaabbA_overlap_distance_double / 3.13, 0, 0, 0);//调整核心筒的位置
                    Thickness abbaabbA_core_move_L = new Thickness(-1 * move_abbaabbA_core_distiance_L + abbaAbba_overlap_distance_double / 3.13 + abbaabbA_overlap_distance_double / 3.13, 0, 0, 0);//调整核心筒的位置
                    abbAabba_core_L.Margin = abbAabba_core_move_L;
                    abbaabbA_core_L.Margin = abbaabbA_core_move_L;
                }
            }
        }

        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_2T4H_abbc_1DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abba

            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(TXCNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    foreach (string TaCLeft in TXCFullPath)
                    {
                        if (TaALeft != TaCLeft)
                        {
                            //创建一个组合list，所有套型a均可处理为镜像模式
                            //根据户型组合模式进行列表创建，当前为 ab
                            List<string> targetList = new List<string>();

                            targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                            targetList.Add(TaBLeft);
                            string TaBRight = TaBLeft.Replace("Left", "Right");
                            targetList.Add(TaBRight);
                            string TaCRight = TaCLeft.Replace("Left", "Right");
                            targetList.Add(TaCRight);
                            //对aa组合，进行图片生成处理，需要new image，new stackpanel
                            //new的必要性是为了，确保每一个控件都是新创建的
                            if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                            {
                                Addstackpanel_Lst(targetList, stackpanel_Lst);
                            }
                        }
                    }

                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_2T4H_abbA_1DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_2T4H_abbccbba_2DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abbccbba

            //对套型C进行处理，窗口开在山墙面的不要
            List<string> targetTXCNameList = new List<string>();
            foreach (string str in TXCNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXCNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(targetTXCNameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    foreach (string TaCLeft in TXCFullPath)
                    {
                        if (TaALeft != TaCLeft)
                        {
                            //创建一个组合list，所有套型a均可处理为镜像模式
                            //根据户型组合模式进行列表创建，当前为 ab
                            List<string> targetList = new List<string>();

                            targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                            targetList.Add(TaBLeft);
                            string TaBRight = TaBLeft.Replace("Left", "Right");
                            targetList.Add(TaBRight);
                            string TaCRight = TaCLeft.Replace("Left", "Right");
                            targetList.Add(TaCRight);
                            targetList.Add(TaCLeft);
                            targetList.Add(TaBLeft);
                            targetList.Add(TaBRight);
                            string TaARight = TaALeft.Replace("Left", "Right");
                            targetList.Add(TaARight);
                            //对abbccbba组合，进行图片生成处理，需要new image，new stackpanel
                            //new的必要性是为了，确保每一个控件都是新创建的
                            if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                            {
                                Addstackpanel_Lst(targetList, stackpanel_Lst);
                            }
                        }
                    }

                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_2T4H_abbaabbA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_2T4H_abbaabbc_2DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abbccbba

            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(TXCNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    foreach (string TaCLeft in TXCFullPath)
                    {
                        if (TaALeft != TaCLeft)
                        {
                            //创建一个组合list，所有套型a均可处理为镜像模式
                            //根据户型组合模式进行列表创建，当前为 ab
                            List<string> targetList = new List<string>();

                            targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                            targetList.Add(TaBLeft);
                            string TaBRight = TaBLeft.Replace("Left", "Right");
                            targetList.Add(TaBRight);
                            string TaARight = TaALeft.Replace("Left", "Right");
                            targetList.Add(TaARight);//添加每一组合TX文件的full_path
                            targetList.Add(TaALeft);
                            targetList.Add(TaBLeft);
                            targetList.Add(TaBRight);
                            string TaCRight = TaCLeft.Replace("Left", "Right");
                            targetList.Add(TaCRight);
                            //对abbccbba组合，进行图片生成处理，需要new image，new stackpanel
                            //new的必要性是为了，确保每一个控件都是新创建的
                            if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                            {
                                Addstackpanel_Lst(targetList, stackpanel_Lst);
                            }
                        }
                    }

                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_2T4H_abbaabbA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_2T4H_abbccbbd_2DY_ABCD_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs, List<string> TXDNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abbccbbd

            //对套型C进行处理，窗口开在山墙面的不要
            List<string> targetTXCNameList = new List<string>();
            foreach (string str in TXCNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXCNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(targetTXCNameList, huxingPathList);
            List<string> TXDFullPath = GetTaegetFileFullPath(TXDNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    foreach (string TaCLeft in TXCFullPath)
                    {
                        if (TaALeft != TaCLeft)
                        {
                            foreach (string TaDLeft in TXDFullPath)
                            {
                                if (TaALeft != TaDLeft && TaCLeft != TaDLeft)
                                {
                                    //创建一个组合list，所有套型a均可处理为镜像模式
                                    //根据户型组合模式进行列表创建，当前为 ab
                                    List<string> targetList = new List<string>();

                                    targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                    targetList.Add(TaBLeft);
                                    string TaBRight = TaBLeft.Replace("Left", "Right");
                                    targetList.Add(TaBRight);
                                    string TaCRight = TaCLeft.Replace("Left", "Right");
                                    targetList.Add(TaCRight);
                                    targetList.Add(TaCLeft);
                                    targetList.Add(TaBLeft);
                                    targetList.Add(TaBRight);
                                    string TaDRight = TaDLeft.Replace("Left", "Right");
                                    targetList.Add(TaDRight);
                                    //对abbccbba组合，进行图片生成处理，需要new image，new stackpanel
                                    //new的必要性是为了，确保每一个控件都是新创建的
                                    if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                                    {
                                        Addstackpanel_Lst(targetList, stackpanel_Lst);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_2T4H_abbaabbA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccbd_3DY_ABCD_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abaaba_3DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— 2T4H_abbccbbd

            //对套型C进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    if (TaALeft != TaBLeft)
                    {
                        //创建一个组合list，所有套型a均可处理为镜像模式
                        //根据户型组合模式进行列表创建，当前为 ab
                        List<string> targetList = new List<string>();
                        List<string> _targetList = new List<string>();
                        List<string> __targetList = new List<string>();

                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);
                        targetList.Add(TaALeft);
                        string TaARight = TaALeft.Replace("Left", "Right");
                        targetList.Add(TaARight);
                        targetList.Add(TaBLeft);
                        targetList.Add(TaARight);
                        //对abbccbba组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的
                        _targetList.Add(TaALeft);
                        _targetList.Add(TaBLeft);
                        if (JudgeContainHXT(_targetList))//其次判断，核心筒编号是否相同
                        {
                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_abccba_3DY_ABC_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_abaabc_3DY_ABC_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs, List<string> TXCNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aabbac

            //对套型A进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            List<string> TXCFullPath = GetTaegetFileFullPath(TXCNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    if (TaALeft != TaBLeft)//套A与套B不同
                    {
                        foreach (string TaCLeft in TXCFullPath)
                        {
                            List<string> targetList = new List<string>();
                            List<string> _targetList = new List<string>();
                            if (TaALeft != TaCLeft && TaBLeft != TaCLeft)//套A和套B，分别与套C不同
                            {
                                targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                                string TaBRight = TaBLeft.Replace("Left", "Right");
                                targetList.Add(TaBRight);
                                targetList.Add(TaALeft);
                                string TaARight = TaALeft.Replace("Left", "Right");
                                targetList.Add(TaARight);
                                targetList.Add(TaBLeft);
                                string TaCRight = TaCLeft.Replace("Left", "Right");
                                targetList.Add(TaCRight);
                                //对aa组合，进行图片生成处理，需要new image，new stackpanel
                                //new的必要性是为了，确保每一个控件都是新创建的

                                //其次判断，核心筒编号是否相同,套型C不用判断
                                _targetList.Add(TaALeft);
                                _targetList.Add(TaBLeft);
                                _targetList.Add(TaCLeft);
                                if (JudgeContainHXT(_targetList))
                                {
                                    Addstackpanel_Lst(targetList, stackpanel_Lst);
                                }
                            }
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_aaab_2DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_aaab_2DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— aaaa
            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXBNameList = new List<string>();
            foreach (string str in TXBNameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXBNameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(TXANameList_samlleftpngs, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(targetTXBNameList, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();
                    if (TaALeft != TaBLeft)
                    {
                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaARight = TaALeft.Replace("Left", "Right");
                        targetList.Add(TaARight);
                        targetList.Add(TaALeft);
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);

                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的

                        if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                        {
                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaA_2DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }
        //将户型image组合 HXZU_1T2H_aaaaab_3DY_AB_ 模式，转换为combbox的item
        public int CombboxItem_HXZU_1T2H_aaaaab_3DY_AB_(List<string> TXANameList_samlleftpngs, List<string> TXBNameList_samlleftpngs)
        {
            //重设listbox1的索引值
            this.listBox1.SelectedIndex = -1;
            //目的套型组合模式 —— abbbba

            //对套型B进行处理，窗口开在山墙面的不要
            List<string> targetTXANameList = new List<string>();
            foreach (string str in TXANameList_samlleftpngs)
            {
                if (!str.Contains("_NT_") && !str.Contains("_BT_"))
                {
                    targetTXANameList.Add(str);
                }
            }
            //先获取文件的fullpath_List
            List<string> TXAFullPath = GetTaegetFileFullPath(targetTXANameList, huxingPathList);
            List<string> TXBFullPath = GetTaegetFileFullPath(TXBNameList_samlleftpngs, huxingPathList);
            //先对套型name进行组合，每个组合建立一个list<string>，目的是为了显示图片列表

            //创建list_stackpanel_source
            List<StackPanel> stackpanel_Lst = new List<StackPanel>();//判断
            foreach (string TaALeft in TXAFullPath)
            {
                foreach (string TaBLeft in TXBFullPath)
                {
                    //创建一个组合list，所有套型a均可处理为镜像模式
                    //根据户型组合模式进行列表创建，当前为 ab
                    List<string> targetList = new List<string>();
                    if (TaALeft != TaBLeft)
                    {
                        targetList.Add(TaALeft);//添加每一组合TX文件的full_path
                        string TaARight = TaALeft.Replace("Left", "Right");
                        targetList.Add(TaARight);
                        targetList.Add(TaALeft);
                        targetList.Add(TaARight);
                        targetList.Add(TaALeft);
                        string TaBRight = TaBLeft.Replace("Left", "Right");
                        targetList.Add(TaBRight);
                        //对aa组合，进行图片生成处理，需要new image，new stackpanel
                        //new的必要性是为了，确保每一个控件都是新创建的
                        if (JudgeContainHXT(targetList))//其次判断，核心筒编号是否相同
                        {
                            Addstackpanel_Lst(targetList, stackpanel_Lst);
                        }
                    }
                }
            }
            //需要对筛选出来的套型PNG组合stackpanel_Lst，在list末尾追加核心筒PNG，并合理排序
            getCore_imgMove_1T2H_aAaAaA_3DY(stackpanel_Lst);
            this.listBox1.ItemsSource = stackpanel_Lst;//使用source源的方式
            return stackpanel_Lst.Count;
        }

        //将户型组合放进stackpanel
        public void Addstackpanel_Lst(List<string> targetList, List<StackPanel> stackpanel_Lst)
        {
            StackPanel stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            foreach (string Tx in targetList)
            {
                //为stackpanel添加图片
                Image _image = new Image();
                _image.Source = ReturnBitmap(Tx);
                int length = Tx.Length;
                //从文件的全路径字段中，找到文件name，并以tooltip的形式显示
                for (int i = length - 1; i >= 0; i--)
                {
                    if (Tx.Substring(i, 1) == "U")
                    {
                        int sub_len = length - i;
                        string imaName = Tx.Substring(i, sub_len);
                        int _sub_len = imaName.Length;
                        _image.Name = imaName.Substring(0, _sub_len - 4);
                        CreatImageTootip(_image, _image.Name);
                        break;
                    }
                }
                stackpanel.Children.Add(_image);
            }
            stackpanel_Lst.Add(stackpanel);
        }
        //创建一个image及其tooltip
        public void CreatImageTootip(Image ima_, string ima_name)
        {
            //创建tooiTip
            ToolTip tooltip = new ToolTip();
            tooltip.Placement = PlacementMode.Bottom;
            StackPanel stackpanel = new StackPanel();
            tooltip.Content = ima_name;
            ToolTipService.SetInitialShowDelay(tooltip, 30000);//时间设置无效
            //设置image的tooltip
            ima_.ToolTip = tooltip;
        }
        //判断一个列表是否含有相同核心筒
        public bool JudgeContainHXT(List<string> targetList)
        {
            int count = targetList.Count;
            if (count < 0)
                return false;
            int calCount = 0;
            string strHXT = "";
            for (int i = 0; i < 100; i++)
            {
                if (radioButton.IsChecked == true)
                {
                    calCount = 0;
                    if (i < 10)//首次判断 _Core_3301_
                    {
                        strHXT = "_CORE_33" + "0" + i.ToString();
                        //其次判断 _Core_330101_

                    }
                    else
                    {
                        strHXT = "_CORE_33" + i.ToString();
                    }
                    foreach (string _str in targetList)
                    {
                        if (_str.Contains(strHXT))
                        {
                            calCount++;
                        }
                    }
                    if (calCount == count)
                        break;

                }
                else if (radioButton1.IsChecked == true)
                {
                    calCount = 0;
                    if (i < 10)
                    {
                        strHXT = "_CORE_3354" + "0" + i.ToString();
                    }
                    else
                    {
                        strHXT = "_CORE_3354" + i.ToString();
                    }
                    foreach (string _str in targetList)
                    {
                        if (_str.Contains(strHXT))
                        {
                            calCount++;
                        }
                    }
                    if (calCount == count)
                        break;

                }
                else if (radioButton2.IsChecked == true)
                {
                    calCount = 0;
                    if (i < 10)
                    {
                        strHXT = "_CORE_54" + "0" + i.ToString();//首次判断 _Core_335401_
                        //其次判断 _Core_33540101_

                    }
                    else
                    {
                        strHXT = "_CORE_54" + i.ToString();
                    }
                    foreach (string _str in targetList)
                    {
                        if (_str.Contains(strHXT))
                        {
                            calCount++;
                        }
                    }
                    if (calCount == count)
                        break;
                }
            }
            if (count == calCount)
                return true;
            else
                return false;

        }
        #endregion
        //获取目标_A_B_C_D套型套型库的namelist
        private List<string> GetTX_A_B_C_D_names(System.Windows.Controls.ComboBox comboBox2,
            System.Windows.Controls.ComboBox comboBox4,
            System.Windows.Controls.TextBox textbox,
            System.Windows.Controls.ComboBox comboBox5,
            List<string> huxing33NameList_samlleftpng,
            string t_type)
        {
            string selhouseType = comboBox2.SelectedItem.ToString();//房型
            string houseType = "";
            int index_fang = selhouseType.IndexOf("房");
            if (selhouseType.Substring(0, index_fang) == "2 ")
            {
                houseType = "_2F" + selhouseType.Substring(index_fang + 1, 1) + "W" + "_";
            }
            else if (selhouseType.Substring(0, index_fang) == "2+1 ")
            {
                houseType = "_2_1F" + selhouseType.Substring(index_fang + 1, 1) + "W" + "_";
            }
            else if (selhouseType.Substring(0, index_fang) == "3 ")
            {
                houseType = "_3F" + selhouseType.Substring(index_fang + 1, 1) + "W" + "_";
            }
            else if (selhouseType.Substring(0, index_fang) == "3+1 ")
            {
                houseType = "_3_1F" + selhouseType.Substring(index_fang + 1, 1) + "W" + "_";
            }
            else if (selhouseType.Substring(0, index_fang) == "4 ")
            {
                houseType = "_4F" + selhouseType.Substring(index_fang + 1, 1) + "W" + "_";
            }
            else if (selhouseType.Substring(0, index_fang) == "4+1 ")
            {
                houseType = "_4_1F" + selhouseType.Substring(index_fang + 1, 1) + "W" + "_";
            }
            //开间
            string selkaijian = comboBox4.SelectedItem.ToString();
            string kaijian = "_" + selkaijian + "K_";
            //客厅位置
            string selketingweizhi = comboBox5.SelectedItem.ToString();
            string ketingweizhi = "_" + selketingweizhi.Substring(0, 2) + "_";
            //面积-需要对户型name参数进行面积数据读取后，进行与输入参数进行比较
            string selarea = textbox.Text;
            double _area;
            double.TryParse(selarea, out _area);
            //对目标高度的户型库进行条件抓取
            List<string> targetPngNames = new List<string>();
            foreach (string temp_str in huxing33NameList_samlleftpng)
            {
                if (temp_str.Contains(t_type))
                {
                    if (temp_str.Contains(houseType) && temp_str.Contains(kaijian) && temp_str.Contains(ketingweizhi))
                    {
                        //下列代码是对面积进行抓取
                        int index = temp_str.IndexOf("K");
                        string index2 = temp_str.Substring(index + 2, 1);
                        string index3 = temp_str.Substring(index + 3, 1);
                        string index4 = temp_str.Substring(index + 4, 1);
                        string tarArea = "";
                        double _index2, _index3, _index4;
                        if (double.TryParse(index2, out _index2))
                        {
                            tarArea += index2;
                            if (double.TryParse(index3, out _index3))
                            {
                                tarArea += index3;
                                if (double.TryParse(index4, out _index4))
                                {
                                    tarArea += index4;
                                }
                            }
                        }
                        double _tarArea;
                        if (double.TryParse(tarArea, out _tarArea))
                        {
                            if (Math.Abs(_tarArea - _area) <= 10)//面积浮动区间上下 10 ㎡，区间长度为20 ㎡
                            {
                                targetPngNames.Add(temp_str);
                            }
                        }
                    }
                }
            }
            return targetPngNames;
        }
        //进入套型设计选择界面
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //s锁定户型组合主选项
            this.groupBox1.IsEnabled = false;
            this.groupBox_Copy2.IsEnabled = false;
            this.groupBox_Copy3.IsEnabled = false;
            //清空基于套型生成的户型组合示意图
            List<Image> nullImageList = new List<Image>();
            this.listBox1.ItemsSource = nullImageList;
            this.button3.IsEnabled = false;
            //展开窗口
            this.Width = 1000;//2个套型
            this.button_Copy.IsEnabled = false;
            this.listBox.IsEnabled = false;
            RestTX();
            if (this.listBox.SelectedIndex >= 0)
            {
                try
                {
                    //该if判断用来设定套型ABCDEF的开关-isEnabled
                    Image selImage = this.listBox.SelectedItem as Image;
                    if (selImage.Name.Contains("_A_"))
                    {
                        this.TXA.IsEnabled = true;
                        this.TXB.IsEnabled = false;
                        this.TXC.IsEnabled = false;
                        this.TXD.IsEnabled = false;
                    }
                    else if (selImage.Name.Contains("_AB_"))
                    {
                        this.TXA.IsEnabled = true;
                        this.TXB.IsEnabled = true;
                        this.TXC.IsEnabled = false;
                        this.TXD.IsEnabled = false;
                    }
                    else if (selImage.Name.Contains("_ABC_"))
                    {
                        this.TXA.IsEnabled = true;
                        this.TXB.IsEnabled = true;
                        this.TXC.IsEnabled = true;
                        this.TXD.IsEnabled = false;
                    }
                    else if (selImage.Name.Contains("_ABCD_"))
                    {
                        this.TXA.IsEnabled = true;
                        this.TXB.IsEnabled = true;
                        this.TXC.IsEnabled = true;
                        this.TXD.IsEnabled = true;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
            else
            {
                MessageBox.Show("请选择户组合示意图");
            }
        }

        #region 生成基础设计模型
        //生成revit模型，每一个户型组合对应一个模型方法，需要一个对应的makequest
        //在启动revit api 方法之前，需要传递模型的文件路径
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //设置判断选项，是调取轮廓，还是调取模型
            if (this.radioButton9.IsChecked == true)
            {
                CMD._Recalloptions = "outline_sola";
            }
            else if (this.radioButton10.IsChecked == true)
            {
                CMD._Recalloptions = "basic_model";
            }
            if (this.listBox1.IsEnabled == true)
            {
                this.listBox1.IsEnabled = false;
                this.button3.Content = "请重新选择";
                //该if，用来判断模型组合的布局规则
                string HX_rvtFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\UNIT\";
                string HX_HXT_rvtFilePath = @"W:\BIM_ARCH\03.插件\户型库\ParametricStudyForDwellings\BIM_Model\CORE\";
                Image selImage = this.listBox.SelectedItem as Image;
                if (listBox1.SelectedIndex >= 0)
                {
                    //开启1T2H模式
                    if (selImage.Name == "HXZH_1T2H_aa_1DY_A_" || selImage.Name == "HXZH_2T2H_aa_1DY_A_")
                    {
                        Get_1T2H_aa_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aa_);//与创建模型命令进行对接
                    }//01
                    else if (selImage.Name == "HXZU_1T2H_aaaa_2DY_A_" || selImage.Name == "HXZU_2T2H_aaaa_2DY_A_")
                    {
                        Get_1T2H_aa_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aaaa_);//与创建模型命令进行对接
                    }//02
                    else if (selImage.Name == "HXZU_1T2H_aaaaaa_3DY_A_" || selImage.Name == "HXZU_2T2H_aaaaaa_3DY_A_")
                    {
                        Get_1T2H_aa_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aaaaaa_);//与创建模型命令进行对接
                    }//03
                    else if (selImage.Name == "HXZU_1T2H_ab_1DY_AB_" || selImage.Name == "HXZU_2T2H_ab_1DY_AB_")
                    {
                        Get_1T2H_ab_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_ab_);//与创建模型命令进行对接
                    }//4
                    else if (selImage.Name == "HXZU_1T2H_abba_2DY_AB_" || selImage.Name == "HXZU_2T2H_abba_2DY_AB_")
                    {
                        Get_1T2H_ab_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abba_);//与创建模型命令进行对接
                    }//05
                    else if (selImage.Name == "HXZU_1T2H_aaab_2DY_AB_" || selImage.Name == "HXZU_2T2H_aaab_2DY_AB_")
                    {
                        Get_1T2H_aaab_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aaab_);//与创建模型命令进行对接
                    }//06
                    else if (selImage.Name == "HXZU_1T2H_aabbaa_3DY_AB_" || selImage.Name == "HXZU_2T2H_aabbaa_3DY_AB_")
                    {
                        Get_1T2H_aabbaa_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aabbaa_);
                    }//07
                    else if (selImage.Name == "HXZU_1T2H_aaaaab_3DY_AB_" || selImage.Name == "HXZU_2T2H_aaaaab_3DY_AB_")
                    {
                        Get_1T2H_aaaaab_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aaaaab_);
                    }//08
                    else if (selImage.Name == "HXZU_1T2H_abbbba_3DY_AB_" || selImage.Name == "HXZU_2T2H_abbbba_3DY_AB_")
                    {
                        Get_1T2H_aabbaa_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abbbba_);
                    }//09
                    else if (selImage.Name == "HXZU_1T2H_abbc_2DY_ABC_" || selImage.Name == "HXZU_2T2H_abbc_2DY_ABC_")
                    {
                        Get_1T2H_abbc_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abbc_);
                    }//10
                    else if (selImage.Name == "HXZU_1T2H_aabbac_3DY_ABC_" || selImage.Name == "HXZU_2T2H_aabbac_3DY_ABC_")
                    {
                        Get_1T2H_aabbac_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_aabbac_);
                    }//11
                    else if (selImage.Name == "HXZU_1T2H_abbbbc_3DY_ABC_" || selImage.Name == "HXZU_2T2H_abbbbc_3DY_ABC_")
                    {
                        Get_1T2H_abaabc_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abbbbc_);
                    }//12
                    else if (selImage.Name == "HXZU_1T2H_abccba_3DY_ABC_" || selImage.Name == "HXZU_2T2H_abccba_3DY_ABC_")
                    {
                        Get_1T2H_abccba_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abccba_);
                    }//13
                    else if (selImage.Name == "HXZU_1T2H_abccbd_3DY_ABCD_" || selImage.Name == "HXZU_2T2H_abccbd_3DY_ABCD_")
                    {
                        Get_1T2H_abccbd_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abccbd_);
                    }//14
                     //增加组合模式
                    else if (selImage.Name == "HXZU_1T2H_abaaba_3DY_AB_" || selImage.Name == "HXZU_2T2H_abaaba_3DY_AB_")
                    {
                        Get_1T2H_ab_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abaaba_);
                    }//15
                    else if (selImage.Name == "HXZU_1T2H_abaabc_3DY_ABC_" || selImage.Name == "HXZU_2T2H_abaabc_3DY_ABC_")
                    {
                        Get_1T2H_abaabc_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_1T2H_abaabc_);
                    }//16
                     //开启2T4H模式
                    else if (selImage.Name == "HXZU_2T4H_abba_1DY_AB_" || selImage.Name == "HXZU_1T4H_abba_1DY_AB_")
                    {
                        Get_2T4H_abba_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_2T4H_abba_);
                    }//17
                    else if (selImage.Name == "HXZU_2T4H_abbaabba_2DY_AB_" || selImage.Name == "HXZU_1T4H_abbaabba_2DY_AB_")
                    {
                        Get_2T4H_abba_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_2T4H_abbaabba_);
                    }//18
                    else if (selImage.Name == "HXZU_2T4H_abbc_1DY_ABC_" || selImage.Name == "HXZU_1T4H_abbc_1DY_ABC_")
                    {
                        Get_2T4H_abbc_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_2T4H_abbc_);
                    }//19
                    else if (selImage.Name == "HXZU_2T4H_abbccbba_2DY_ABC_" || selImage.Name == "HXZU_1T4H_abbccbba_2DY_ABC_")
                    {
                        Get_2T4H_abbc_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_2T4H_abbccbba_);
                    }//20
                    else if (selImage.Name == "HXZU_2T4H_abbaabbc_2DY_ABC_" || selImage.Name == "HXZU_1T4H_abbaabbc_2DY_ABC_")
                    {
                        Get_2T4H_abbaabbc_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_2T4H_abbaabbc_);
                    }//21
                    else if (selImage.Name == "HXZU_2T4H_abbccbbd_2DY_ABCD_" || selImage.Name == "HXZU_1T4H_abbccbbd_2DY_ABCD_")
                    {
                        Get_2T4H_abbccbbd_RVTpath(HX_rvtFilePath, HX_HXT_rvtFilePath);
                        makeRequest(RequestId.Creat_2T4H_abbccbbd_);
                    }//22
                }
            }
            else if (this.listBox1.IsEnabled == false)
            {
                this.listBox1.IsEnabled = true;
                this.button3.Content = "确认";
            }

        }

        //生成模型 1T2H 模式为aa 1DY 
        //生成模型 1T2H 模式为aaaa 2DY
        //生成模型 1T2H 模式为aaaaaa 3DY
        public void Get_1T2H_aa_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)//使用的为 TXA_left 套型
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            //逆序寻找 core 的index
            int TXAnameLength;
            string selHXT = GetHxtPath(TXAname, out TXAnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selHXT + @".rvt";

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selHXT;
        }
        //生成模型 1T2H 模式为ab 1DY 
        //生成模型 1T2H 模式为abba 2DY 
        //生成模型 1T2H 模式为abaaba 3DY 
        public void Get_1T2H_ab_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)//使用的为 TXA_left 套型
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图像右
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength = TXBname.Length;
            string selHXT = GetHxtPath(TXAname, out TXAnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selHXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 12) + @".rvt";// -12 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selHXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 12);
        }
        //生成模型 1T2H 模式为aaab 2DY 
        public void Get_1T2H_aaab_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)//使用的为 TXA_left 套型
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[3];//为套型B左边套，来自于图像右
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength = TXBname.Length;
            string selHXT = GetHxtPath(TXAname, out TXAnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selHXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 12) + @".rvt";// -12 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selHXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 12);
        }
        //生成模型 1T2H 模式为aabbaa 3DY
        //生成模型 1T2H 模式为abbbba 3DY
        public void Get_1T2H_aabbaa_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[2];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";
            CMD._TXB_HXT_rvt = HX_HXT_rvtFilePath + selTXB_HXT + @".rvt";

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
            CMD._TXB_HXT_name = selTXB_HXT;

        }
        //生成模型 1T2H 模式为aaaaab 3DY 
        public void Get_1T2H_aaaaab_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)//使用的为 TXA_left 套型
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[5];//为套型B左边套，来自于图像右
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength = TXBname.Length;
            string selHXT = GetHxtPath(TXAname, out TXAnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selHXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 12) + @".rvt";// -12 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selHXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 12);
        }
        //生成模型 1T2H 模式为abbc 2DY
        public void Get_1T2H_abbc_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[2];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[3];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";
            CMD._TXB_HXT_rvt = HX_HXT_rvtFilePath + selTXB_HXT + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 12) + @".rvt";// -11 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
            CMD._TXB_HXT_name = selTXB_HXT;
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 12);
        }
        //生成模型 1T2H 模式为abccba 3DY
        public void Get_1T2H_abccba_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[2];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 12) + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 11) + @".rvt";// -11 源于_right_
            CMD._TXC_HXT_rvt = HX_HXT_rvtFilePath + selTXC_HXT + @".rvt";

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 12);
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 11);
            CMD._TXC_HXT_name = selTXC_HXT;
        }

        //生成模型 1T2H 模式为aabbac 3DY
        public void Get_1T2H_aabbac_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[2];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[5];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";
            CMD._TXB_HXT_rvt = HX_HXT_rvtFilePath + selTXB_HXT + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 12) + @".rvt";// -11 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
            CMD._TXB_HXT_name = selTXB_HXT;
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 12);
        }

        //生成模型 1T2H 模式为abbbbc 3DY
        //生成模型 1T2H 模式为abaabc 3DY
        public void Get_1T2H_abaabc_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[5];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 12) + @".rvt";
            CMD._TXB_HXT_rvt = HX_HXT_rvtFilePath + selTXB_HXT + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 12) + @".rvt";// -11 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 12);
            CMD._TXB_HXT_name = selTXB_HXT;
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 12);
        }
        //生成模型 1T2H 模式为abccbd 3DY
        public void Get_1T2H_abccbd_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[2];//为套型B左边套，来自于图左
            string TXDname = HXnameLst[5];//为套型B左边套，来自于图左
            //MessageBox.Show(TXDname);
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            int TXDnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            string selTXD_HXT = GetHxtPath(TXDname, out TXDnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 12) + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 11) + @".rvt";// -11 源于_small_left
            CMD._TXC_HXT_rvt = HX_HXT_rvtFilePath + selTXC_HXT + @".rvt";
            CMD._TXD_rvt = HX_rvtFilePath + TXDname.Substring(0, TXDnameLength - 12) + @".rvt";// -11 源于_small_Right
                                                                                               //MessageBox.Show(CMD._TXD_rvt);

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 12);
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 11);
            CMD._TXC_HXT_name = selTXC_HXT;
            CMD._TXD_name = TXDname.Substring(0, TXDnameLength - 12);

        }

        //生成模型 2T4H 模式为 abba 1DY
        //生成模型 2T4H 模式为 abbaabba 2DY
        public void Get_2T4H_abba_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
        }
        //生成模型 2T4H 模式为 abbc 1DY
        //生成模型 2T4H 模式为 abbccbba 2DY
        public void Get_2T4H_abbc_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[3];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 12) + @".rvt";// -12 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 12);
        }
        //生成模型 2T4H 模式为 abbaabbc 2DY
        public void Get_2T4H_abbaabbc_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[7];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 12) + @".rvt";// -12 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 12);
        }
        //生成模型 2T4H 模式为 abbccbbd 2DY
        public void Get_2T4H_abbccbbd_RVTpath(string HX_rvtFilePath, string HX_HXT_rvtFilePath)
        {
            List<string> HXnameLst = ConvertImgToNamelst();
            string TXAname = HXnameLst[0];//为套型A左边套
            string TXBname = HXnameLst[1];//为套型B左边套，来自于图左
            string TXCname = HXnameLst[4];//为套型B左边套，来自于图左
            string TXDname = HXnameLst[7];//为套型B左边套，来自于图左
            //逆序寻找 H 的index
            int TXAnameLength;
            int TXBnameLength;
            int TXCnameLength;
            int TXDnameLength;
            string selTXA_HXT = GetHxtPath(TXAname, out TXAnameLength);
            string selTXB_HXT = GetHxtPath(TXBname, out TXBnameLength);
            string selTXC_HXT = GetHxtPath(TXCname, out TXCnameLength);
            string selTXD_HXT = GetHxtPath(TXDname, out TXDnameLength);
            CMD._TXA_rvt = HX_rvtFilePath + TXAname.Substring(0, TXAnameLength - 11) + @".rvt";// -11 源于_left_
            CMD._TXA_HXT_rvt = HX_HXT_rvtFilePath + selTXA_HXT + @".rvt";
            CMD._TXB_rvt = HX_rvtFilePath + TXBname.Substring(0, TXBnameLength - 11) + @".rvt";
            CMD._TXC_rvt = HX_rvtFilePath + TXCname.Substring(0, TXCnameLength - 11) + @".rvt";
            CMD._TXD_rvt = HX_rvtFilePath + TXDname.Substring(0, TXDnameLength - 12) + @".rvt";// -12 源于_right_

            CMD._TXA_name = TXAname.Substring(0, TXAnameLength - 11);
            CMD._TXA_HXT_name = selTXA_HXT;
            CMD._TXB_name = TXBname.Substring(0, TXBnameLength - 11);
            CMD._TXC_name = TXCname.Substring(0, TXCnameLength - 11);
            CMD._TXD_name = TXDname.Substring(0, TXDnameLength - 12);
        }
        /// <summary>
        /// 读取txt里面的内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> ReadTxt(string path)
        {
            List<string> txtLines = new List<string>();
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                txtLines.Add(line);
            }
            return txtLines;
        }
        //将图片列表转化为其name_list
        public List<string> ConvertImgToNamelst()
        {
            StackPanel selStackPanel = listBox1.SelectedItem as StackPanel;
            List<string> HXnameLst = new List<string>();
            foreach (Image temp_image in selStackPanel.Children.OfType<Image>())
            {
                HXnameLst.Add(temp_image.Name);
            }
            return HXnameLst;
        }
        //寻找目的核心筒文件name
        public string GetHxtPath(string TXAname, out int TXAnameLength)
        {
            TXAnameLength = TXAname.Length;
            //逆序寻找 H 的index
            string selHXT = "";
            for (int i = TXAnameLength - 1; i > 0; i--)
            {
                if (TXAname.Substring(i, 1) == "C")
                {
                    selHXT = TXAname.Substring(i, 5);
                    int _count = TXAnameLength - i - 1;
                    for (int j = 0; j < _count; j++)
                    {
                        string _temp = TXAname.Substring(i + 5 + j, 1);
                        double _int;
                        //一直追加core后面的数字
                        if (double.TryParse(_temp, out _int))
                        {
                            selHXT += _temp;
                        }
                        else
                            break;
                    }
                    if (radioButton.IsChecked == true)
                    {
                        if (selHXT.Substring(0, 7) == "CORE_33" && selHXT.Substring(0, 9) != "CORE_3354")//该处问题为，CORE_33系列核心筒的编号不可以出现CORE_3354
                        {
                            return selHXT;
                        }
                    }
                    else if (radioButton1.IsChecked == true)
                    {
                        if (selHXT.Substring(0, 9) == "CORE_3354")//该处问题为，CORE_33系列核心筒不可以大于54个
                        {
                            return selHXT;
                        }
                    }
                    else if (radioButton2.IsChecked == true)
                    {
                        if (selHXT.Substring(0, 7) == "CORE_54")
                        {
                            return selHXT;
                        }
                    }
                    selHXT = "";
                }
            }
            return selHXT;
        }
        #endregion
        //生成基于户型组合的套型设计平面窗口大小
        private void ModifyWindows_TXZH(int count)
        {
            this.Width = 1000 + 250 * count;//设置窗口大小
        }

        //获取目标路径文件的fullPath
        public List<string> GetTaegetFileFullPath(List<string> _fileNameList, List<string> _fileFullpathList)
        {
            List<string> _TaegetFileFullPath = new List<string>();
            foreach (string _str in _fileNameList)//对图像文件进行处理
            {
                foreach (string __str in _fileFullpathList)
                {
                    if (__str.Contains(_str))
                    {
                        _TaegetFileFullPath.Add(__str);
                    }
                }
            }
            return _TaegetFileFullPath;
        }
        //获取image并释放资源，不会占用系统文件
        //public BitmapImage ReturnBitmap(string imagePath)
        //{
        //    //Read byte[] from png file
        //    BinaryReader binReader = new BinaryReader(File.Open(imagePath, FileMode.Open));
        //    FileInfo fileInfo = new FileInfo(imagePath);
        //    byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);
        //    binReader.Close();

        //    //Init bitmap
        //    BitmapImage bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.StreamSource = new MemoryStream(bytes);
        //    bitmap.EndInit();
        //    return bitmap;
        //}
        //获取image并释放资源
        public BitmapImage ReturnBitmap(string imagePath)
        {
            //Read byte[] from png file
            //BinaryReader binReader = new BinaryReader(File.Open(imagePath, FileMode.Open));
            //FileInfo fileInfo = new FileInfo(imagePath);
            //byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);
            //binReader.Close();

            //Init bitmap
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            //bitmap.StreamSource = new MemoryStream(bytes);
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }


        //获取文件夹里的所有文件的完整路径string
        public List<string> GetDirectory(string srcPtah, out List<string> _fileNameList)
        {
            List<string> _filePathList = new List<string>();
            _fileNameList = new List<string>();
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPtah);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();

                foreach (FileSystemInfo f_info in fileinfo)
                {
                    if (f_info is DirectoryInfo)
                    {
                        continue;
                    }
                    else
                    {
                        _filePathList.Add(f_info.FullName);//子文件的full_path
                        _fileNameList.Add(f_info.Name);//子文件的name
                    }
                }
                return _filePathList;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.button3.IsEnabled = true;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Hxzuhe_auto();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Hxzuhe_auto();
            }
        }
    }//class
}//namespace
