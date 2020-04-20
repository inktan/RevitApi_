using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace WIND_NUMB_new
{
    public partial class MainWindow : System.Windows.Forms.Form
    {
        private UIDocument uidoc;
        private Document doc;
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;
        public MainWindow(UIDocument _uidoc)
        {
            InitializeComponent();
            this.Text += " " + APP.Version;//窗口标题后缀
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);
            this.uidoc = _uidoc;
            this.doc = _uidoc.Document;
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
                foreach (System.Windows.Forms.Control ctrl in this.Controls)
                {
                    ctrl.Enabled = status;
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
            this.dozeOff();//操作进行中，可以是命令窗口灰屏
        }
        #endregion

        private void Button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.CalAllFiMarks);
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            CMD.changStr = "activeView";
            this.listBox1.Items.Add(CMD.actiViewName);

        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            CMD.changStr = "slelectViews";
            foreach (string str in CMD.fieldViewNames)//添加未有对应目标视图的PLOT视图
            {
                this.listBox1.Items.Add(str);
            }

        }
        private void Button3_Click_1(object sender, EventArgs e)
        {
            makeRequest(RequestId.CommandSingleMark);
            this.listBox1.SelectedItems.Clear();//数据复原为空
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count > 0)//确认是否有选择项
            {
                foreach (string str in this.listBox1.SelectedItems)
                {
                    CMD.selectedViewNames.Add(str);
                }
            }
            makeRequest(RequestId.CommandMarks);
            this.listBox1.SelectedItems.Clear();//数据复原为空
        }

        private void Button4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
