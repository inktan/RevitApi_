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


namespace COLR_SWIT
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

        private void Button2_Click(object sender, EventArgs e)//基于PLOT视图新建视图---
        {
            if (this.listBox1.SelectedItems.Count > 0)//确认是否有选择项
            {
                foreach (string str in this.listBox1.SelectedItems)
                {
                    CMD.selectedViewNames.Add(str);
                }
            }
            makeRequest(RequestId.ChangeRgbToBlack);
            this.listBox1.SelectedItems.Clear();//选择数据复原为空
        }
        private void Button3_Click(object sender, EventArgs e)//更新视图图元内容---
        {
            if (this.listBox1.SelectedItems.Count > 0)//确认是否有选择项
            {
                foreach (string str in this.listBox1.SelectedItems)
                {
                    CMD.selectedViewNames.Add(str);
                }
            }
            makeRequest(RequestId.ChangeBlackToRgb);
            this.listBox1.SelectedItems.Clear();//选择数据复原为空
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.UpdateVIews_List);

            if (radioButton1.Checked)
            {
                updateListBox_main(CMD.workViewNames);
            }
            else if(radioButton5.Checked)
            {
                this.listBox1.Items.Clear();
                this.listBox1.Items.Add(CMD.actiViewName);
            }
            else if(radioButton6.Checked)
            {
                updateListBox_main(CMD.plotViewNames);
            }
            else if (radioButton7.Checked)
            {
                updateListBox_main(CMD.INTFViewNames);
            }
        }
        private void RadioButton1_CheckedChanged(object sender, EventArgs e)//work视图
        {
            CMD.changStr = "_work";
            updateListBox_main(CMD.workViewNames);
        }

        private void RadioButton5_CheckedChanged(object sender, EventArgs e)//当前活动视图
        {
            CMD.changStr = "_actiView";
            this.listBox1.Items.Clear();
            this.listBox1.Items.Add(CMD.actiViewName);
        }

        private void RadioButton6_CheckedChanged(object sender, EventArgs e)//plot视图
        {
            CMD.changStr = "_plot";
            updateListBox_main(CMD.plotViewNames);
        }

        private void RadioButton7_CheckedChanged(object sender, EventArgs e)//intf视图
        {
            CMD.changStr = "_INTF";
            updateListBox_main(CMD.INTFViewNames);
        }

        //---------------------------------华丽的分割线---------------------------------

        //以下为各种method
        public void updateListBox_main(IList<string> plotViewNames)
        {
            this.listBox1.Items.Clear();

            foreach (string str in plotViewNames)
            {
                this.listBox1.Items.Add(str);
            }
        }


    }//class

}//namespace
