using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    public static class FileSaveLoad
    {
        public static void SaveByDialog(object _objectToSave, string _filterString, string _defaultFileName)
        {
            //get file path to save to
            var saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = _filterString;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = _defaultFileName;
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel || saveFileDialog1.FileName == "")
            {
                return;
            }
            string filename = saveFileDialog1.FileName;
            Save(_objectToSave, filename, true);
        }

        public static T LoadByDialog<T>(string _filterString)
        {
            //get file path to load from
            var dialog = new OpenFileDialog();
            dialog.Filter = _filterString;
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.Cancel || dialog.FileName == "")
            {
                return default(T);
            }
            string filename = dialog.FileName;
            return Load<T>(filename, true);
        }


        public static T Load<T>(string _filePath, bool _showErrorMessage)
        {
            T loadedObject = default(T);
            FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(_filePath, FileMode.Open);
            }
            catch (FileNotFoundException ex)
            {
                return default(T);
            }
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new MyBinaryFormatterBinder();
            try
            {
                loadedObject = (T)formatter.Deserialize(fs);
            }
            catch (Exception ex)
            {
                if (_showErrorMessage)
                    UserMessages.ShowErrorMessage(ex, null);
                return default(T);
            }
            finally
            {
                fs.Close();
            }
            return loadedObject;
        }


        public static bool Save(object _object, string _filePath, bool _showError)
        {
            System.IO.FileStream fs = new FileStream(_filePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _object);
                return true;
            }
            catch (Exception ex)
            {
                if (_showError)
                    UserMessages.ShowErrorMessage(ex, null);
                return false;
            }
            finally
            {
                fs.Close();
            }
        }

        public static void CreateEmptyFile(string filename)
        {
            File.Create(filename).Dispose();
        }
    }
}
