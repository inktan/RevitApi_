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


namespace TransGroupParas
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
            this.dozeOff();
        }
        #endregion

        private void MainWindow_Load(object sender, EventArgs e)
        {
            //可选窗口文档列表
            foreach (string str in CMD.openFileNames)
            {
                this.comboBox1.Items.Add(str);
                this.comboBox2.Items.Add(str);
            }
            this.comboBox1.SelectedIndex = 0;//默认通过设置索引值，显示文档名字
            this.comboBox2.SelectedIndex = 1;//默认通过设置索引值，显示文档名字
        }

        private void Button_first_Click(object sender, EventArgs e)
        {
            CMD.originFileName = this.comboBox1.SelectedItem.ToString();
            CMD.targetFileName = this.comboBox2.SelectedItem.ToString();

            CMD.selectedGroupNames = new List<string>();//重置选项值
            if (this.listBox1.SelectedItems.Count > 0)
            {
                foreach (string str in this.listBox1.SelectedItems)
                {
                    CMD.selectedGroupNames.Add(str);
                }
            }

            makeRequest(RequestId.TransferGroupFimalyParas);
            this.listBox1.SelectedItems.Clear();//数据复原为空
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            CMD.targetFileName = this.comboBox2.SelectedItem.ToString();

            //获取目标文档
            Document targetDoc = getDocment(CMD.documentSet, CMD.targetFileName);
            //listBox1获取目标文档模型组
            getlistboxGroups(targetDoc, BuiltInCategory.OST_IOSModelGroups);
        }

        private void RadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            CMD.targetFileName = this.comboBox2.SelectedItem.ToString();

            //获取目标文档
            Document targetDoc = getDocment(CMD.documentSet, CMD.targetFileName);
            //listBox1获取目标文档详图组
            getlistboxGroups(targetDoc, BuiltInCategory.OST_IOSDetailGroups);
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CMD.targetFileName = this.comboBox2.SelectedItem.ToString();

            //获取目标文档
            Document targetDoc = getDocment(CMD.documentSet, CMD.targetFileName);

            if (this.radioButton3.Checked)
            {
                //listBox1获取目标文档模型组
                getlistboxGroups(targetDoc, BuiltInCategory.OST_IOSModelGroups);

            }
            else if(this.radioButton4.Checked)
            {
                //listBox1获取目标文档详图组
                getlistboxGroups(targetDoc, BuiltInCategory.OST_IOSDetailGroups);
            }
        }
        //以下为各种method---------------------------------分割线---------------------------------
        //获取目标模型组
        public void getlistboxGroups(Document targetDoc, BuiltInCategory category)
        {
            IList<string> targetGroupNames = new List<string>();//目标文档中的模型/注释组列表

            //获取目标文档详图组
            IList<Element> MoDeGroups = (new FilteredElementCollector(targetDoc)).OfCategory(category).WhereElementIsNotElementType().ToElements();
            targetGroupNames = getIlistNames(MoDeGroups);//获取详图组的string-list

            //组列表去重
            targetGroupNames = targetGroupNames.Distinct().ToList();

            CMD.selectedGroupNames = new List<string>();//数据复原为空
            this.listBox1.SelectedItems.Clear();//数据复原为空
            this.listBox1.Items.Clear();//数据复原为空

            foreach (string str in targetGroupNames)
            {
                this.listBox1.Items.Add(str);
            }
        }
        //获取目标文档详图组

        //获取元素的名称string列表
        public IList<string> getIlistNames(ICollection<Element> eles)
        {
            IList<string> strs = new List<string>();
            foreach (Element ele in eles)
            {
                string eleName = ele.Name;
                strs.Add(eleName);
            }
            return strs;
        }

        //获取指定文档
        public Document getDocment(DocumentSet documentSet, string str)
        {
            Document originDoc = null;

            foreach (Document document in documentSet)//获取所有的打开文档的标题列表
            {
                string docName = document.Title;
                if (str == docName)
                {
                    originDoc = document;
                }
            }
            return originDoc;
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }//end class

}//namespace
