using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace goa.Common
{
    [Serializable]
    public class ScreenLocation
    {
        public System.Drawing.Point Location;
        public ScreenLocation(System.Drawing.Point _p)
        {
            this.Location = _p;
        }
    }
    public class WindowPositionRecorder
    {
        private Form form;
        private string appName;
        private ScreenLocation recordedLocation;

        public WindowPositionRecorder(Form _form, string _appName)
        {
            this.form = _form;
            this.appName = _appName;
            this.form.StartPosition = FormStartPosition.Manual;
            this.LoadPosition();
        }
        public void RecordPosition()
        {
            this.recordedLocation = new ScreenLocation(this.form.Location);
            saveToFile();
        }
        public void LoadPosition()
        {
            this.recordedLocation = loadFromFile();
            if (this.recordedLocation != null)
                this.form.Location = this.recordedLocation.Location;
            else
                this.form.Location = Cursor.Position;
        }

        private void saveToFile()
        {
            var file = getFileName();
            FileSaveLoad.Save(this.recordedLocation, file, false);
        }

        private ScreenLocation loadFromFile()
        {
            var file = getFileName();
            var sl = FileSaveLoad.Load<ScreenLocation>(file, false);
            return sl;
        }
        private string getFileName()
        {
            return @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\" + Environment.UserName + "_" + this.appName + "_" + "windowLoc.wpr";
            //return Path.Combine(Path.GetTempPath(), this.appName + "_" + Environment.UserName + ".wpr");
        }
    }
}
