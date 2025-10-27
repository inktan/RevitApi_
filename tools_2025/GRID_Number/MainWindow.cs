﻿using System;
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


namespace GRID_Number
{
    public partial class MainWindow : System.Windows.Forms.Form
    {
        private UIDocument uidoc;
        private Document doc;
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;
        public MainWindow(   )
        {
            InitializeComponent();
            this.Text += " 1.12" ;//窗口标题后缀
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);
            //this.uidoc = _uidoc;
            //this.doc = _uidoc.Document;
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
            this.dozeOff();
        }
        #endregion

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.Add("1、2、3、4…");
            this.comboBox1.Items.Add("A、B、C…1A、1B、1C、1D…");
            this.comboBox1.Items.Add("A、B、C…A1、B1、C1、D1…");
            this.comboBox1.Items.Add("A、B、C、…AA、AB、AC、AD…");
            this.comboBox1.Items.Add("A、B、C、…AA、BA、CA、DA…");
            this.comboBox1.Items.Add("A、B、C、…Z、a、b、c…z");

            this.comboBox4.Items.Add("名称");
            this.comboBox4.Items.Add("自定义编号");

            this.comboBox1.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;

            this.button_first.Text = "两点连线选择相交轴网";
            this.textBox3.Text = "已选择轴网数量为0";

            this.textBox1.Text = "空（选填）";
            this.textBox2.Text = "1";

        }

        private void Button_first_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem == null || this.comboBox1.SelectedItem.ToString() == "")
            {
                MessageBox.Show("未选择轴网类型");
            }
            else
            {
                if (this.comboBox1.SelectedIndex ==0)
                {
                    CMD.changeCommand = "gridCrosswise123";
                    string startNameFie;
                    if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "1")
                    {
                        startNameFie = "1";
                    }
                    else
                    {
                        startNameFie = textBox2.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox1.SelectedIndex == 1)
                {
                    CMD.changeCommand = "gridVertical1A";
                    string startNameFie;
                    if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox2.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox1.SelectedIndex == 2)
                {
                    CMD.changeCommand = "gridVerticalA1";
                    string startNameFie;
                    if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox2.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox1.SelectedIndex == 3)
                {
                    CMD.changeCommand = "gridVerticalAAAB";
                    string startNameFie;
                    if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox2.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox1.SelectedIndex == 4)
                {
                    CMD.changeCommand = "gridVerticalAABA";
                    string startNameFie;
                    if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox2.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                else if (this.comboBox1.SelectedIndex == 5)
                {
                    CMD.changeCommand = "gridVerticalAZaz";
                    string startNameFie;
                    if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "1")
                    {
                        startNameFie = "A";
                    }
                    else
                    {
                        startNameFie = textBox2.Text;
                    }
                    CMD.startGridName = startNameFie;
                }
                CMD.partField = textBox1.Text;//输入文本框的内容
                if (CMD.partField == null || CMD.partField == "" || CMD.partField == "空（选填）")
                {
                    CMD.partField = "";
                }
                else
                {
                    CMD.partField += "-";
                }

                // 是否含Z
                if (this.checkBox1.Checked)
                {
                  CMD.isContainZ = true;
                }
                else
                {
                    CMD.isContainZ = false;
                }

                //轴网类型确定
                CMD.selGridType = this.comboBox2.SelectedItem.ToString();
                makeRequest(RequestId.GridNmae);

            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex==0)
            {
                this.textBox2.Text = "1";
            }
            else
            {
                this.textBox2.Text = "A";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CMD.checkOverlapGrids = "0";
            makeRequest(RequestId.CheckOverlapGrids);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CMD.checkOverlapGrids = "1";
            makeRequest(RequestId.CheckOverlapGrids);
        }
    }//end class

}//namespace
