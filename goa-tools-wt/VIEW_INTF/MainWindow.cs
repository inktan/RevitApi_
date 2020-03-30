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


namespace VIEW_INTF
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
            makeRequest(RequestId.NewBuiltViews);
            this.listBox1.SelectedItems.Clear();//选择数据复原为空

        }
        private void Button3_Click(object sender, EventArgs e)//更新视图图元内容---
        {
            if (this.listBox2.SelectedItems.Count > 0)//确认是否有选择项
            {
                foreach (string str in this.listBox2.SelectedItems)
                {
                    CMD.selectedViewNames.Add(str);
                }
            }
            makeRequest(RequestId.UpdateVIews);
            this.listBox2.SelectedItems.Clear();//选择数据复原为空
        }

        private void Button1_Click_1(object sender, EventArgs e)//更新视图列表
        {
            makeRequest(RequestId.UpdateVIews_List);
            updateListBox_sub();
        }

        private void Button4_Click(object sender, EventArgs e)//更新视图列表
        {
            makeRequest(RequestId.UpdateVIews_List);
            updateListBox_sub();
        }

        private void RadioButton5_CheckedChanged(object sender, EventArgs e)
        {
            updateListBox_main(CMD.getNotarget_ToNames_INTF, CMD.targetViewNames_INTF, "Copy_PLOT_TO_INTF", "新建INTF视图", "更新INTF视图", "选择INTF视图");
        }

        private void RadioButton6_CheckedChanged(object sender, EventArgs e)
        {
            updateListBox_main(CMD.getNotarget_ToNames_DD, CMD.targetViewNames_DD, "_Design_", "新建DD方案图", "更新DD方案图", "选择DD方案图");
        }

        private void RadioButton7_CheckedChanged(object sender, EventArgs e)
        {
            updateListBox_main(CMD.getNotarget_ToNames_BP, CMD.targetViewNames_BP, "blue_Print", "新建BP蓝图", "更新BP蓝图", "选择BP蓝图");
        }

        //---------------------------------华丽的分割线---------------------------------

        //以下为各种method
        //不同子选项rodioButton触发下的listbox视图列表
        public void updateListBox_sub()
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();
            if (radioButton5.Checked)
            {
                foreach (string str in CMD.getNotarget_ToNames_INTF)
                {
                    this.listBox1.Items.Add(str);
                }
                foreach (string str in CMD.targetViewNames_INTF)
                {
                    this.listBox2.Items.Add(str);
                }
            }
            else if (radioButton6.Checked)
            {
                foreach (string str in CMD.getNotarget_ToNames_DD)
                {
                    this.listBox1.Items.Add(str);
                }
                foreach (string str in CMD.targetViewNames_DD)
                {
                    this.listBox2.Items.Add(str);
                }
            }
            else if (radioButton7.Checked)
            {
                foreach (string str in CMD.getNotarget_ToNames_BP)
                {
                    this.listBox1.Items.Add(str);
                }
                foreach (string str in CMD.targetViewNames_BP)
                {
                    this.listBox2.Items.Add(str);
                }
            }
        }
        //不同主选项rodioButton触发下的listbox视图列表
        public void updateListBox_main(IList<string> getNotarget_ToNames_INTF, IList<string> targetViewNames_INTF, string str00, string str01, string str02, string str03)
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();

            CMD.changStr = str00;

            this.button2.Text = str01;
            this.button3.Text = str02;
            this.groupBox2.Text = str03;

            foreach (string str in getNotarget_ToNames_INTF)
            {
                this.listBox1.Items.Add(str);
            }
            foreach (string str in targetViewNames_INTF)
            {
                this.listBox2.Items.Add(str);
            }
        }


    }//class

}//namespace
