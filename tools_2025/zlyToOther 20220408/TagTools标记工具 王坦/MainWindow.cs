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

using goa.Common.Exceptions;

namespace TagTools
{
    internal partial class MainWindow : System.Windows.Forms.Form
    {
        private WindowPositionRecorder positionRecorder;

        internal MainWindow()
        {
            InitializeComponent();
            this.positionRecorder = new WindowPositionRecorder(this, this.Text);
            this.positionRecorder.LoadPosition();
            RequestMaker.Init();
            RegisterWinFormEvents(true);

            this.Text += " " + APP.Version;

            popUI();
        }

        private void popUI()
        {
            this.comboBox_atPos.Items.AddRange(Enum.GetNames(typeof(PosToHost)));
            this.comboBox_atPos.SelectedIndex = 0;
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

        internal void UpdateInput()
        {
            UIInput.PromptChangeAll = this.checkBox_promptChangeAll.Checked;
            UIInput.AlignToHostCentroid = this.checkBox_alignToHostCentroid.Checked;
            bool b1 = double.TryParse(this.textBox_atPosDist.Text, out UIInput.DistAtPosToHost);
            bool b2 = double.TryParse(this.textBox_moveTowardHostBy.Text, out UIInput.DistMoveTowardHost);
            if(!b1 || !b2)
            {
                if (!b1)
                    this.textBox_atPosDist.Text = "0";
                if (!b2)
                    this.textBox_moveTowardHostBy.Text = "0";
                throw new CommonUserExceptions("输入无效。");
            }
            UIInput.DistAtPosToHost = UnitUtils.ConvertToInternalUnits(UIInput.DistAtPosToHost, DisplayUnitType.DUT_MILLIMETERS);
            UIInput.DistMoveTowardHost = UnitUtils.ConvertToInternalUnits(UIInput.DistMoveTowardHost, DisplayUnitType.DUT_MILLIMETERS);
            UIInput.PosToHost = (PosToHost)Enum.Parse(typeof(PosToHost), this.comboBox_atPos.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.spaceOutSelectedTags);
        }

        private void button2_autoSpaceAllTags_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.spaceOutAllTags);
        }

        private void button_findOverlap_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.findOverlap);
        }

        private void button_showHost_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.showHost);
        }

        private void button_snapToHost_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.moveTowardHost);
        }

        private void button_tagAtPosRelativeToHost_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.tagAtPosRelativeToHost);
        }
    }
}
