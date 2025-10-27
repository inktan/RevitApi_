using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using goa.Common;
using goa.Excel;

namespace SHET_NEW
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private Document doc;
        private DataSet ds;

        public Form1(Document _doc)
        {
            InitializeComponent();
            this.doc = _doc;
            popUI();
        }

        private void popUI()
        {
            var titleblockTypes = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_TitleBlocks);
            foreach(var tb in titleblockTypes)
            {
                this.comboBox_titleblocks.Items.Add(tb);
            }

            //select A1 + 0.00 by default
            var item = this.comboBox_titleblocks.Items.Cast<Element>().FirstOrDefault(x => x.Name == "A1+0.00");
            if (item != null)
                this.comboBox_titleblocks.SelectedItem = item;
        }

        private void selectExcel()
        {
            string file = "";
            this.ds = ExMethods.LoadExcelFileAsStrings(out file);
            this.textBox_excel.Text = file;
        }

        private void create()
        {
            //check input
            if(this.ds == null)
            {
                TaskDialog.Show("消息", "请选择Excel文件。");
                return;
            }

            var table = this.ds.Tables[0];

            //check column names
            var col1 = table.Columns["图号"];
            var col2 = table.Columns["图名"];
            if (col1 == null || col2 == null)
            {
                TaskDialog.Show("信息", "选择的Excel文件格式错误。请确保第一个工作簿中有正确格式的信息。");
                return;
            }

            //get all sheets with sheet number
            var allSheets = new Dictionary<string, Element>();
            var sheets = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(ViewSheet)).Cast<ViewSheet>();
            foreach(var vs in sheets)
            {
                allSheets.Add(vs.SheetNumber, vs);
            }

            //create sheet per row
            bool error = false;
            List<string> errorOnSheets = new List<string>();
            var id = ((Element)this.comboBox_titleblocks.SelectedItem).Id;

            using (Transaction trans = new Transaction(doc, "批量创建图纸"))
            {
                trans.Start();
                foreach (DataRow row in table.Rows)
                {
                    try
                    {
                        Element sheet = null;
                        string sheetNumber = row["图号"] as string;
                        //check existing sheet number
                        if (allSheets.ContainsKey(sheetNumber))
                            sheet = allSheets[sheetNumber];
                        else
                            sheet = ViewSheet.Create(doc, id);
                        setParameters(row, sheet);
                    }
                    catch (Exception ex)
                    {
                        error = true;
                        errorOnSheets.Add(
                            row.ItemArray[1].ToString()
                            + " - "
                            + row.ItemArray[2].ToString());
                    }
                }
                trans.Commit();
            }

            if (error)
            {
                //show all the failed sheets
                string s = "";
                foreach (var name in errorOnSheets)
                {
                    s += name + "\r\n";
                }
                TaskDialog.Show("信息", "以下图纸发生错误：\r\n" + s);
            }
        }

        private void setParameters(DataRow _row, Element _elem)
        {
            var table = _row.Table;
            bool error = false;
            List<string> missingParameters = new List<string>();
            foreach (DataColumn col in table.Columns)
            {
                var colName = col.ColumnName;
                string paraName;
                BuiltInParameter bip = BuiltInParameter.INVALID;
                bool foundName = FirmStandards.SheetParameterNameMap.TryGetValue(colName, out paraName);
                bool foundBip = false;
                if (!foundName)
                    foundBip = FirmStandards.SheetBuiltInParameterMap.TryGetValue(colName, out bip);
                var paraValue = _row[col];
                Parameter parameter = null;
                if (foundName)
                    parameter = _elem.GetParameterByName(paraName);
                else if (foundBip)
                    parameter = _elem.get_Parameter(bip);

                if(parameter == null)
                {
                    error = true;
                    missingParameters.Add(colName);
                    continue;
                }
                parameter.SetValue(paraValue);
            }

            if (error)
            {
                string s = "";
                foreach(var name in missingParameters)
                {
                    s += name + "\r\n";
                }
                throw new Exception("cannot find parameter:\r\n" + s);
            }
        }

        private void button_excel_Click(object sender, EventArgs e)
        {
            selectExcel();
        }

        private void button_create_Click(object sender, EventArgs e)
        {
            create();
        }
    }
}
