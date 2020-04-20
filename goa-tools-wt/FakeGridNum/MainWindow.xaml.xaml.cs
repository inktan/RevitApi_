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

namespace FakeGridNum
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
            List<string> GridNumType = new List<string>();
            GridNumType.Add("1、2、3、4…");
            GridNumType.Add("A、B、C…A1、B1、C1…");
            GridNumType.Add("A、B、C、…AA、BA、CA…");

            this.comboBox.ItemsSource = GridNumType;
            this.comboBox.SelectedIndex = 0;

            this.textBox.Text = "空（选填）";
            this.textBox1.Text = "1";
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.comboBox.SelectedItem == null || this.comboBox.SelectedItem.ToString() == "")
            {
                MessageBox.Show("未选择轴网类型");
            }
            else
            {
                if (this.comboBox.SelectedItem.ToString() == "1、2、3、4…")
                {
                    CMD.changeCommand = "gridCrosswise123";
                    string startNameFie;
                    if (textBox1.Text == null || textBox1.Text == "" || textBox1.Text == "1")
                    {
                        startNameFie = "1";
                    }
                    else
                    {
                        startNameFie = textBox1.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox.SelectedItem.ToString() == "A、B、C…A1、B1、C1…")
                {
                    CMD.changeCommand = "gridVerticalA1";
                    string startNameFie;
                    if (textBox1.Text == null || textBox1.Text == "" || textBox1.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox1.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox.SelectedItem.ToString() == "A、B、C、…AA、BA、CA…")
                {
                    CMD.changeCommand = "gridVerticalAA";
                    string startNameFie;
                    if (textBox1.Text == null || textBox1.Text == "" || textBox1.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox1.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                CMD.partField = textBox.Text;//输入文本框的内容
                if (CMD.partField == null || CMD.partField == "" || CMD.partField == "空（选填）")
                {
                    CMD.partField = "";
                }
                else
                {
                    CMD.partField += "-";
                }

                makeRequest(RequestId.GridReName);
            }
        }
    }//class
}//namespace
