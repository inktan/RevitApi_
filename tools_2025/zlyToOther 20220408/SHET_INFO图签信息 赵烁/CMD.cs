using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;

using goa.Common;
using goa.Excel;
using System.Data;

namespace SHET_INFO
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            try
            {
                //check domain
                if (ADValidationCheck.GetDirectoryEntryForCurrentUser() == null)
                {
                    TaskDialog.Show("信息", "需要连接goa网络。");
                    return Result.Failed;
                }

                var doc = commandData.Application.ActiveUIDocument.Document;

                string file = "";
                var ds = ExMethods.LoadExcelFileAsStrings(out file);
                if (ds == null)
                    return Result.Cancelled;

                var table = ds.Tables[0];
                //check column names
                var col1 = table.Columns["工程名称"];
                var col2 = table.Columns["子项名称"];
                if (col1 == null || col2 == null)
                {
                    TaskDialog.Show("信息", "选择的Excel文件格式错误。请确保第一个工作簿中有正确格式的信息。");
                    return Result.Cancelled;
                }

                //get values of column "子项"
                var names = new List<string>();
                for(int i = 0; i < table.Rows.Count; i++)
                {
                    names.Add(table.Rows[i][col2].ToString());
                }

                //prompt user select row
                Form_Dropdown form = new Form_Dropdown(names);
                var result = form.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                    return Result.Cancelled;
                var index = form.ComboBox.SelectedIndex;

                //for each column, find parameter, find parameter by name,
                //set parameter value
                var titleBlocks = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_TitleBlocks);
                bool error = false;
                List<string> errorOnSheets = new List<string>();
                using (Transaction trans = new Transaction(doc, "批量修改图框参数"))
                {
                    trans.Start();
                    foreach (var tb in titleBlocks)
                    {
                        var sheet = doc.GetElement(tb.OwnerViewId) as ViewSheet;
                        try
                        {
                            setParameters(table, sheet, FirmStandards.SheetParameterNameMap, index);
                        }
                        catch (Exception ex)
                        {
                            error = true;                            
                            errorOnSheets.Add(sheet.SheetNumber + " - " + sheet.Name);
                        }
                    }
                    trans.Commit();
                }

                if (error)
                {
                    //show all the failed sheets
                    string s = "";
                    foreach(var name in errorOnSheets)
                    {
                        s += name + "\r\n";
                    }
                    TaskDialog.Show("信息", "以下图纸发生错误：\r\n" + s);
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, null);
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        private void setParameters(DataTable _table, Element _elem, Dictionary<string, string> _nameMap, int _index)
        {
            foreach (DataColumn col in _table.Columns)
            {
                var colName = col.ColumnName;
                var paraName = _nameMap[colName];
                var paraValue = _table.Rows[_index][col];
                var parameter = _elem.GetParameterByName(paraName);
                parameter.SetValue(paraValue);
            }
        }    


    }
}
