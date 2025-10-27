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

namespace CoordinationInfo
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
        }

        internal void RefreshUI()
        {
            this.button_pickLevels.Text =
                UICmd.Levels.Count == 0
                ? "选择楼层"
                : "已选择" + UICmd.Levels.Count + "个楼层";
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

        private void button_pickLevels_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.pickLevels);
        }

        private void button_setAll_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.setCoordinationLevelAll);
        }

        private void button_setPicked_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.setCoordinationLevelPicked);
        }

        private void button_elemInLinkId_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.elemInLinkId);
        }
    }
}
