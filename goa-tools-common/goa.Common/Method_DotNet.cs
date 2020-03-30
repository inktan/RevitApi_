﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Data;

using ExcelDataReader;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace goa.Common
{
    public static partial class Methods
    {
        #region Dictionary
        public static void TryAddValue<T1, T2>(this Dictionary<T1, List<T2>> dic, T1 key, T2 value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key].Add(value);
            }
            else
            {
                dic[key] = new List<T2>() { value };
            }
        }
        #endregion

        #region Double         
        /// <summary>
        /// Compare two double values, using
        /// first value * scale as precision threshold.
        /// </summary>
        /// <param name="_thisDouble"></param>
        /// <param name="_double"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualByScale(this double _thisDouble, double _double, double _scale)
        {
            double precision = Math.Abs(_thisDouble * _scale);
            double difference = Math.Abs(_thisDouble - _double);
            if (difference > precision)
                return false;
            else
                return true;
        }
        public static bool IsAlmostEqualByDifference
            (this double _thisDouble, double _double, double _epsilon)
        {
            double difference = Math.Abs(_thisDouble - _double);
            if (difference > _epsilon)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Compare two double values, using
        /// 0.0001 as difference threshold.
        /// </summary>
        /// <param name="_thisDouble"></param>
        /// <param name="_double"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualByDifference(this double _thisDouble, double _double)
        {
            return IsAlmostEqualByDifference(_thisDouble, _double, 0.0001);
        }
        /// <summary>
        /// Compare two double values, using
        /// first value * 0.0001 as precision threshold.
        /// </summary>
        /// <param name="_thisDouble"></param>
        /// <param name="_double"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualByScale(this double _thisDouble, double _double)
        {
            return _thisDouble.IsAlmostEqualByScale(_double, 0.0001);
        }
        public static string ToStringDigits(this double _d, int _digits)
        {
            return Math.Round(_d, _digits).ToString();
        }
        #endregion

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

        #region Exception
        public static void ReThrow(this Exception ex)
        {
            var exInfo = ExceptionDispatchInfo.Capture(ex);
            exInfo.Throw();
        }
        #endregion

        #region FileIO
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
                //deserialize could throw "unable to find assembly"
                //exception when using addin manager. Goes away when 
                //loaded in Revit.
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
        #endregion

        #region String
        public static bool ToBoolean(this string _string)
        {
            var trueList = new string[] { "True", "Yes", "1", "是" };
            var falseList = new string[] { "False", "No", "0", "否" };
            if (trueList.Contains(_string, StringComparer.InvariantCultureIgnoreCase))
                return true;
            else if (falseList.Contains(_string, StringComparer.InvariantCultureIgnoreCase))
                return false;
            else
                throw new FormatException("Failed to convert \"" + _string + "\" to boolean value.");
        }
        public static string RemoveAll(this string _string, params string[] _removeList)
        {
            string output = _string;
            foreach (var s in _removeList)
            {
                output = output.Replace(s, "");
            }
            return output;
        }
        public static string[] SplitBy(this string _s, string _spliter)
        {
            var array = _s.Split(_spliter.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            return array;
        }
        #endregion

        #region DateTime
        public enum eRoundingDirection { up, down, nearest }
        public static DateTime RoundDateTime(this DateTime dt, int minutes)

        {
            TimeSpan t;
            eRoundingDirection direction = eRoundingDirection.nearest;
            switch (direction)
            {
                case eRoundingDirection.up:
                    t = (dt.Subtract(DateTime.MinValue)).Add(new TimeSpan(0, minutes, 0)); break;
                case eRoundingDirection.down:
                    t = (dt.Subtract(DateTime.MinValue)); break;
                default:
                    t = (dt.Subtract(DateTime.MinValue)).Add(new TimeSpan(0, minutes / 2, 0)); break;
            }

            return DateTime.MinValue.Add(new TimeSpan(0,
                   (((int)t.TotalMinutes) / minutes) * minutes, 0));
        }
        public static DateTime RoundDateTimeToMinute(this DateTime dt)
        {
            var newTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            if (dt.Second < 30)
                return newTime;
            else
                return newTime.AddMinutes(1);
        }
        #endregion

    }
}