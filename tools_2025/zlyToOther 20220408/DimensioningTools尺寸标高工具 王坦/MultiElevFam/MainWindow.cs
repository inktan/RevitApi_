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

using goa.Common;

namespace AutoFillUpLevelHeightAnnotation
{
    public partial class MainWindow : System.Windows.Forms.Form
    {
        private FamilyInstance fi;
        private WindowPositionRecorder wpr;
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;
        public MainWindow()
        {
            InitializeComponent();
            this.wpr = new WindowPositionRecorder(this, this.Text);

            this.Text += " " + APP.Version;
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);

            this.FormClosing += onFormClosing;
        }

        private void onFormClosing(object sender, EventArgs e)
        {
            this.wpr.RecordPosition();
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
            //this.dozeOff();
        }
        #endregion


        internal void Pick()
        {
            var uidoc = goa.Common.APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var filter = new GenericAnnotationSelectionFilter();
            Form_cursorPrompt.Start("选择一个标高族。", APP.MainWindow);
            var pickRef = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, filter);
            var fi = doc.GetElement(pickRef) as FamilyInstance;
            if (!isValidFamilyInstance(fi))
            {
                TaskDialog.Show("错误", "选择的族无效。");
                return;
            }
            else
            {
                this.fi = fi;
            }
        }

        internal void AutoFillUp()
        {
            if (!validInputs())
            {
                TaskDialog.Show("错误", "请检查输入条件。");
                return;
            }

            double startElevation, levelHeight;
            int numLevel;
            levelHeight = double.Parse(this.textBox_levelHeight.Text);
            startElevation = double.Parse(this.textBox_startElevation.Text);
            numLevel = int.Parse(this.textBox_numLevel.Text);

            var pMap = getParameterMap(this.fi);

            var doc = goa.Common.APP.UIApp.ActiveUIDocument.Document;
            double currentHeight = startElevation;

            using (TransactionGroup tg = new TransactionGroup(doc, "自动填写标高"))
            {
                tg.Start();
                using (Transaction trans = new Transaction(doc, "clean up"))
                {
                    trans.Start();
                    //clean up existing parameter values
                    foreach (Parameter p in this.fi.Parameters)
                    {
                        if (p.Definition.Name.Contains("标高")
                            && p.StorageType == StorageType.String)
                        {
                            p.Set("");
                        }
                    }
                    trans.Commit();
                }

                using (Transaction trans = new Transaction(doc, "fill up"))
                {
                    trans.Start();
                    //fill up
                    for (int i = 1; i < numLevel + 1; i++)
                    {
                        if (pMap.ContainsKey(i) == false)
                            continue;
                        string h = String.Format("{0:0.00}", currentHeight);
                        if (currentHeight.IsAlmostEqualByDifference(0))
                        {
                            h = "±" + h;
                        }
                        if (i == 1)
                        {
                            h = "H = " + h;
                        }

                        var p = pMap[i];
                        p.Set(h);
                        currentHeight += levelHeight;
                    }
                    trans.Commit();
                }
                tg.Assimilate();
            }
        }

        private bool validInputs()
        {
            double d;
            int i;
            bool b1 = double.TryParse(this.textBox_levelHeight.Text, out d);
            bool b2 = int.TryParse(this.textBox_numLevel.Text, out i);
            bool b3 = double.TryParse(this.textBox_startElevation.Text, out d);

            return b1 && b2 && b3 && this.fi != null;
        }

        private bool isValidFamilyInstance(FamilyInstance _fi)
        {
            var p = _fi.GetParameterByName("标高 1");
            return p != null;
        }

        private Dictionary<int, Parameter> getParameterMap(Element _e)
        {
            var map = new Dictionary<int, Parameter>();
            foreach (Parameter p in _e.Parameters)
            {
                if (p.Definition.Name.Contains("标高")
                    && p.StorageType == StorageType.String)
                {
                    string s = p.Definition.Name.RemoveAll("标高 ");
                    int i;
                    bool b = int.TryParse(s, out i);
                    if (b)
                    {
                        map[i] = p;
                    }
                }
            }
            return map;
        }

        private void button_pickFamily_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.Pick);
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.AutoFillUp);
        }
    }
}
