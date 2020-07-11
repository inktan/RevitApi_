using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    /// <summary>
    /// See class summary of ProgressTracker.
    /// </summary>
    public partial class ProgressWindow : Form
    {
        public ProgressTracker Tracker { get; set; }
        protected override void OnClosed(EventArgs e)
        {
            Tracker.StopTask = true;
            Tracker.StopTracking_TS();
            base.OnClosed(e);
        }
        public ProgressWindow()
        {
            InitializeComponent();
            this.TopMost = true;
            this.Tracker = new ProgressTracker(this,
                this.progressBar1,
                this.label1,
                this.button_stop);
        }
    }
}
