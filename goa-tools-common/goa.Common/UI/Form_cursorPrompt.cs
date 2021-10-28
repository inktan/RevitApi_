using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    public partial class Form_cursorPrompt : Form
    {
        public static Form_cursorPrompt Current;
        private Timer timer = new Timer();
        public Form_cursorPrompt()
        {
            InitializeComponent();
            this.timer.Interval = 30;
            this.timer.Tick += onTimerTick;
            this.FormClosing += onClosing;
        }

        private Point offset = new Point(10, 0);
        private void onTimerTick(object sender, EventArgs e)
        {
            var p = Cursor.Position;
            this.Location = new Point(p.X + offset.X, p.Y + offset.Y);
        }

        private void onClosing(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.timer.Tick -= onTimerTick;
        }

        public static void Start(string _prompt, IWin32Window _parent)
        {
            if(Current.IsAvailable() == false)
            {
                Current = new Form_cursorPrompt();
                Current.Show(_parent);
            }
            Current.timer.Start();
            Current.showPrompt(_prompt);
        }

        public static void Stop()
        {
            if (Current.IsAvailable())
            {
                Current.timer.Stop();
                Current.Close();
                Current.Dispose();
            }
        }

        private void showPrompt(string _prompt)
        {
            this.label1.Text = _prompt;
        }
    }
}
