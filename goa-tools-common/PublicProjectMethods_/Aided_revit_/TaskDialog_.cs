using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class TaskDialog_
    {
        #region TaskDialog Revit内置消息弹窗
        /// <summary>
        /// Revit消息窗口弹出
        /// </summary>
        public static void TaskDialogErrorMessage(this string _str)
        {
            string _title = "Error";
            TaskDialog.Show(_title, _str);
        }
        #endregion
    }
}
