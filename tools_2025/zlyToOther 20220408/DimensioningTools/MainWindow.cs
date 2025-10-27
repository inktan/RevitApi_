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

namespace DimensioningTools
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

        private void button_dimClosestPoint_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.dimClosestPintInView);
        }

        private void button_windowSpotElevDim_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.windowSpotElevDim);
        }

        private void button_dimOnFloors_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.multiFloorElev);
        }

        private void button_multiLevelElev_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.multiLevelElev);
        }

        private void button_multiElevFam_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.multiElevFam);
        }

        private void button_fakeSpotElevUpdate_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.fakeSpotElevUpdate);
        }

        // ===!!!
        private void button_absElevs_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.absElev);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.dimAvoid);
        }

        private void button_fakeDim_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.fakeDim);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 假尺寸避让
            RequestMaker.makeRequest(RequestId.fakeDimAvoid);
        }
    }
}
