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

namespace FAKE_DIMS
{
    public partial class MainWindow : System.Windows.Forms.Form
    {
        private ExternalEvent m_ExEvent;
        private MainWindowRequestHandler m_Handler;
        private WindowPositionRecorder positionRecorder;

        public MainWindow()
        {
            InitializeComponent();
            this.positionRecorder = new WindowPositionRecorder(this, this.Text);

            this.Text += " " + APP.Version;
            this.m_Handler = new MainWindowRequestHandler();
            this.m_ExEvent = ExternalEvent.Create(m_Handler);

            this.Shown += onShown;
            this.FormClosing += onClosing;
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

        private void onShown(object sender, EventArgs e)
        {

        }
        private void onClosing(object sender, EventArgs e)
        {
            try { this.positionRecorder.RecordPosition(); }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.ConvertSingleView);
        }

        private void button_check_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.Check);
        }

        private void button_multiSelect_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.Select);
        }

        private void button_multiViews_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.ConvertMultiViews);
        }

        private void button_convertSelected_Click(object sender, EventArgs e)
        {
            makeRequest(RequestId.ConvertSelected);
        }
    }
}
