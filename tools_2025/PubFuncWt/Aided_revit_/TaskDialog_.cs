using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PubFuncWt
{
    public static class TaskDialog_
    {
        #region TaskDialog Revit内置消息弹窗
        /// <summary>
        /// Revit消息窗口弹出
        /// </summary>
        public static void TaskDialogErrorMessage(this string _str, string _title = "Revit Info")
        {
            //TaskDialog.Show(_title, _str);

            MessageBox.Show(_str, _title);
        }
        #endregion
    }
}
