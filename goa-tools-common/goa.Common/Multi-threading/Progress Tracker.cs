using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    /// <summary>
    /// Create one private instance inside your main window, together with one instance of progress window winform instance.
    /// Call progressTracker.StartTracking_TS method outside your loop.
    /// Call progressTracker.Current++ for each loop.
    /// Check progressTracker.StopTask for each loop, if true, break.
    /// Call progressTracker.StopTracking_TS outside the loop at the end.
    /// </summary>
    public class ProgressTracker
    {
        private Form form;
        private ProgressBar bar;
        private Label label;
        private Button button_stop;
        private readonly int formHeight1 = 165; //full size
        private readonly int formHeight2 = 120; //if bar and stop button not showing

        private System.Diagnostics.Stopwatch watch_showStopButton;
        private System.Diagnostics.Stopwatch watch_showForm;
        private System.Timers.Timer timer;
        private System.Timers.Timer timerHideForm;

        public int Current { get; set; }
        private int TimeToShowStopButton = 120000;
        private int TimeToShowForm = 2000;

        public bool RunningTask { get; set; }
        public bool StopTask = false;

        public ProgressTracker(Form _form, ProgressBar _bar, Label _label, Button _button)
        {
            this.form = _form;
            this.bar = _bar;
            this.label = _label;
            this.button_stop = _button;
            this.watch_showStopButton = new System.Diagnostics.Stopwatch();
            this.watch_showForm = new System.Diagnostics.Stopwatch();
            this.timer = new System.Timers.Timer(1000);
            this.timerHideForm = new System.Timers.Timer(2000);

            this.timer.Elapsed += TrackProgress_TS;
            this.timerHideForm.Elapsed += HideFormDoubleCheck_TS;
            this.timerHideForm.Start();
            this.button_stop.Click += ButtonStop_Click;
            this.form.FormClosing += onFormClosing;
        }

        public void SetText_TS(string _text)
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.label.Text = _text;
                });
            }
            else
                this.label.Text = _text;
        }

        public void StartTracking_TS(string taskName)
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    startTracking(taskName);
                });
            }
            else
                startTracking(taskName);
        }
        private void startTracking(string taskName)
        {
            try
            {
                this.TimeToShowForm = 2000; //use default value

                this.label.Text = taskName;
                this.label.Enabled = true;
                this.button_stop.Visible = false;
                this.bar.Visible = false;
                this.form.Height = this.formHeight2;

                this.watch_showStopButton.Reset();
                //this.watch_showStopButton.Start();
                this.watch_showForm.Reset();
                this.watch_showForm.Start();
                this.timer.Start();

                this.StopTask = false;
                this.RunningTask = true;
            }
            catch (Exception ex)
            {
                stopTracking();
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        public void StartTracking_TS(string taskName, int min, int max)
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    startTracking(taskName, min, max);
                });
            }
            else
                startTracking(taskName, min, max);
        }
        private void startTracking(string taskName, int min, int max)
        {
            try
            {
                this.TimeToShowForm = 2000;//use default
                this.TimeToShowStopButton = 5000;

                this.label.Text = taskName;
                this.label.Enabled = true;
                this.button_stop.Enabled = true;
                this.form.Height = this.formHeight1;

                this.Current = min;
                this.bar.Minimum = min;
                this.bar.Value = min;
                this.bar.Maximum = max;
                this.bar.Enabled = true;
                this.bar.Visible = true;

                this.watch_showStopButton.Reset();
                this.watch_showStopButton.Start();
                this.watch_showForm.Reset();
                this.watch_showForm.Start();
                this.timer.Start();

                this.RunningTask = true;
            }
            catch (Exception ex)
            {
                stopTracking();
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        public void StartTracking_TS(string taskName, int min, int max,
            int timeToShowWindow,
            int timeToShowStopButton)
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    startTracking(taskName, min, max, timeToShowWindow, timeToShowStopButton);
                });
            }
            else
                startTracking(taskName, min, max, timeToShowWindow, timeToShowStopButton);
        }
        private void startTracking(string taskName, int min, int max,
            int timeToShowWindow,
            int timeToShowStopButton)
        {
            try
            {
                this.TimeToShowForm = timeToShowWindow;
                this.TimeToShowStopButton = timeToShowStopButton;

                this.label.Text = taskName;
                this.label.Enabled = true;
                this.button_stop.Enabled = true;
                this.form.Height = this.formHeight2;

                this.Current = min;
                this.bar.Minimum = min;
                this.bar.Value = min;
                this.bar.Maximum = max;
                this.bar.Enabled = true;
                this.bar.Visible = true;

                this.watch_showStopButton.Reset();
                this.watch_showStopButton.Start();
                this.watch_showForm.Reset();
                this.watch_showForm.Start();
                this.timer.Start();

                this.RunningTask = true;
            }
            catch (Exception ex)
            {
                stopTracking();
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        public void TrackProgress_TS(object sender, EventArgs e)
        {
            if (this.m_formShowing == false)
            {
                showForm_newThread();
            }

            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    trackProgress();
                });
            }
            else
                trackProgress();
        }
        private void trackProgress()
        {
            try
            {
                if (this.RunningTask == false
                        || StopTask == true
                        || this.Current < this.bar.Minimum
                        || this.Current > this.bar.Maximum)
                    return;

                this.bar.Value = this.Current;
                this.form.BringToFront(); //often form appears behind
                if (this.button_stop.Visible == false)
                {
                    if (this.watch_showStopButton.ElapsedMilliseconds > this.TimeToShowStopButton)
                        this.button_stop.Visible = true;
                }
            }
            catch (Exception ex)
            {
                stopTracking();
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        public void StopTracking_TS()
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    stopTracking("", false);
                });
            }
            else
                stopTracking("", false);
        }
        public void StopTracking_TS(string message)
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    stopTracking(message, true);
                });
            }
            else
                stopTracking(message, true);
        }
        private void stopTracking()
        {
            try
            {
                this.watch_showStopButton.Reset();
                this.watch_showForm.Reset();
                this.timer.Stop();
                this.RunningTask = false;
                this.Current = 1;

                if (this.form.Visible)
                {
                    this.form.Hide();
                    this.m_formShowing = false;
                }
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        private void stopTracking(string message, bool showMessageBox)
        {
            try
            {
                stopTracking();

                this.label.Text = message;
                if (showMessageBox)
                    UserMessages.ShowMessage(message);
                if (this.StopTask)
                    this.StopTask = false;
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        private bool m_formShowing = false;
        private void showForm_newThread()
        {
            try
            {
                if (this.watch_showForm.ElapsedMilliseconds > this.TimeToShowForm)
                {
                    this.m_formShowing = true;
                    var startProgressWindow = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            this.form.ShowDialog();
                        }
                        catch
                        {
                            this.m_formShowing = false;
                            return;
                        }
                    });
                    MultiThreadMethods.WaitForFormToShowWithTimeOut(this.form, 100, 1000);
                }
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, this.form);
            }
        }
        public void HideFormDoubleCheck_TS(object sender, EventArgs e)
        {
            if (this.form.InvokeRequired)
            {
                this.form.BeginInvoke((MethodInvoker)delegate ()
                {
                    hideFormDoubleCheck();
                });
            }
            else
                hideFormDoubleCheck();
        }
        private void hideFormDoubleCheck()
        {
            if (this.RunningTask == false
                && this.form.Visible)
            {
                this.form.Hide();
                this.m_formShowing = false;
            }
        }
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            this.StopTask = true;
            this.stopTracking();
        }
        private void onFormClosing(object sender, EventArgs e)
        {
            this.StopTask = true;
            this.stopTracking();
        }
    }
}
