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

namespace ThreeDViewNavi
{
    internal partial class MainWindow : System.Windows.Forms.Form
    {
        private WindowPositionRecorder positionRecorder;

        internal MainWindow(UIDocument _uidoc)
        {
            InitializeComponent();
            RequestMaker.Init();
            RegisterWinFormEvents(true);

            //this.Text += " " + APP.Version;            
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
            RegisterWinFormEvents(false);
            this.FormClosing -= onFormClosing;
            RequestMaker.makeRequest(RequestId.close);
        }
        #endregion

        internal void ShowKeyText(string keys)
        {
            this.label_keys.Text = keys;
        }

        private void button_q_Click(object sender, EventArgs e)
        {

        }

        private void button_E_Click(object sender, EventArgs e)
        {

        }

        private void button_w_Click(object sender, EventArgs e)
        {

        }

        private void button_S_Click(object sender, EventArgs e)
        {

        }

        private void button_A_Click(object sender, EventArgs e)
        {

        }

        private void button_D_Click(object sender, EventArgs e)
        {

        }

        private void button_up_Click(object sender, EventArgs e)
        {

        }

        private void button_down_Click(object sender, EventArgs e)
        {

        }

        private void button_left_Click(object sender, EventArgs e)
        {

        }

        private void button_right_Click(object sender, EventArgs e)
        {

        }

        private void button_ground_Click(object sender, EventArgs e)
        {

        }

        private void button_restore_Click(object sender, EventArgs e)
        {
            
        }
    }
}
