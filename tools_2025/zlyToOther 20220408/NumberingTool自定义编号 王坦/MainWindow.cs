using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using goa.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NumberingTool
{
    internal partial class MainWindow : System.Windows.Forms.Form
    {
        internal Input Input 
        { get { return new Input(this.comboBox_paramName.SelectedItem.ToString(), this.textBox_prefix.Text, this.textBox_surfix.Text); } }

        private WindowPositionRecorder positionRecorder;
        internal MainWindow()
        {
            InitializeComponent();
            this.positionRecorder = new WindowPositionRecorder(this, this.Text);
            this.positionRecorder.LoadPosition();
            RequestMaker.Init();
            RegisterWinFormEvents(true);

            this.Text += " " + APP.Version;
        }

        #region modeless form related
        internal void WakeUp()
        {
            enableCommands(true);
        }
        internal void dozeOff()
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
        internal void CloseForm()
        {
            try
            {
                Registry.Register(false);
                this.positionRecorder.RecordPosition();
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, this);
            }
            finally
            {
                this.Close();
                this.Dispose();
            }
        }
        private bool windFormEventsRegistered = false;
        internal void RegisterWinFormEvents(bool _register)
        {
            if (windFormEventsRegistered == _register)
                return;
            if (_register)
            {
                this.Shown += onShown;
                this.FormClosing += onFormClosing;
            }
            else
            {
                this.Shown -= onShown;
                this.FormClosing -= onFormClosing;
            }
            windFormEventsRegistered = _register;
        }
        private void onShown(object sender, EventArgs e)
        {

        }
        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            RequestMaker.makeRequest(RequestId.close);
        }
        #endregion

        internal void RefreshUI()
        {
            this.label_numElems.Text = "已添加 " + UICmd.Elems.Count + " 个图元";
            this.comboBox_paramName.Items.Clear();
            this.comboBox_paramName.Items.AddRange(getParamNames(UICmd.Elems).ToArray());
        }

        private List<string> getParamNames(IEnumerable<Element> _elems)
        {
            var list = new List<string>();
            foreach (var e in _elems)
            {
                var names = e.Parameters.Cast<Parameter>()
                    .Where(x => x.IsReadOnly == false
                    && x.StorageType == StorageType.String
                    && x.Definition.Name != null
                    && x.Definition.Name != "")
                    .Select(x => x.Definition.Name);
                list.AddRange(names);
            }
            list = list.Distinct().OrderBy(x => x).ToList();
            return list;
        }

        private void button_numbering_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.Numbering);
        }

        private void button_addCurrentSelction_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.addCurrSelection);
        }
    }
}