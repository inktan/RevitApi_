using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common.Excel
{
    public static partial class ExMethods
    {
        #region Excel
        public static DataSet LoadExcelFileAsStrings(out string _filePath)
        {
            //config
            var configDataTable = new ExcelDataTableConfiguration();
            configDataTable.UseHeaderRow = true;
            configDataTable.ReadHeaderRow = (rowReader) => { };

            var configDataSet = new ExcelDataSetConfiguration();
            configDataSet.ConfigureDataTable = (tableReader) => configDataTable;

            //load excel file
            var ds0 = LoadExcelFile(configDataSet, out _filePath);
            if (ds0 == null)
                return null;

            //new dataset
            //re-format all double values as 0-digit strings
            var ds = new DataSet();
            foreach (DataTable dt0 in ds0.Tables)
            {
                var dt = new DataTable();
                ds.Tables.Add(dt);
                int count = dt0.Columns.Count;
                foreach (DataColumn col0 in dt0.Columns)
                {
                    DataColumn col = new DataColumn(col0.Caption);
                    col.DataType = typeof(string);
                    dt.Columns.Add(col);
                }

                foreach (DataRow row0 in dt0.Rows)
                {
                    var values = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        var value = row0[i];
                        string stringValue = "";
                        if (value.GetType() == typeof(double))
                        {
                            stringValue = Convert.ToInt32(value).ToString();
                        }
                        else
                            stringValue = value.ToString();

                        values[i] = stringValue;
                    }
                    dt.Rows.Add(values);
                }
            }

            return ds;
        }
        public static DataSet LoadExcelFile()
        {
            string s = "";
            return LoadExcelFile(null, out s);
        }
        public static DataSet LoadExcelFile(ExcelDataSetConfiguration _config, out string _filePath)
        {//https://github.com/ExcelDataReader/ExcelDataReader
            _filePath = openExcelFile();
            if (_filePath == null)
                return null;
            try
            {
                using (var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        if (_config == null)
                            return reader.AsDataSet();
                        else
                            return reader.AsDataSet(_config);
                    }
                }
            }
            catch (IOException ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
                return null;
            }
        }
        private static string openExcelFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx|Excel 97-2003 Workbook(*.xls)|*.xls";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileName = "report1.xlsx";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel
                || openFileDialog.FileName == "")
                return null;
            var filePath = openFileDialog.FileName;
            return filePath;
        }
        #endregion
    }
}
