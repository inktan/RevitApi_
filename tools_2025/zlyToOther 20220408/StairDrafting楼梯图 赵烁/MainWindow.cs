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

namespace StairDrafting
{
    internal partial class MainWindow : System.Windows.Forms.Form
    {
        private WindowPositionRecorder positionRecorder;

        internal GraphicsStyle LineStyle { get { return this.comboBox_lineStyle.SelectedItem as GraphicsStyle; } }

        internal double Offset { get { return UnitUtils.ConvertToInternalUnits(double.Parse(this.textBox_finishThick.Text), DisplayUnitType.DUT_MILLIMETERS); } }

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

        private void popUI()
        {
            var all = getAllLineStyles();
            this.comboBox_lineStyle.Items.AddRange(all.ToArray());
            var defaultS = all.FirstOrDefault(x => x.Name.Contains("S-STRS"));
            if (defaultS != null)
            {
                int index = all.IndexOf(defaultS);
                this.comboBox_lineStyle.SelectedIndex = index;
            }
            else
            {
                this.comboBox_lineStyle.SelectedIndex = 0;
            }
        }

        private List<GraphicsStyle> getAllLineStyles()
        {
            var lineStyles = new FilteredElementCollector(goa.Common.APP.UIApp.ActiveUIDocument.Document)
            .WhereElementIsNotElementType()
            .OfClass(typeof(GraphicsStyle))
            .Cast<GraphicsStyle>()
            .Where(x => x.GraphicsStyleCategory.Parent != null
            && x.GraphicsStyleCategory.Parent.Id.IntegerValue ==
            (int)BuiltInCategory.OST_Lines)
            .OrderBy(x => x.Name)
            .ToList();
            return lineStyles;
        }

        private void button_drawStructThreadInPlan_Click(object sender, EventArgs e)
        {
            RequestMaker.makeRequest(RequestId.drawStructTreadInPlan);
        }
    }
}
