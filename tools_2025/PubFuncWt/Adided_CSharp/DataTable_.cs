using goa.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class DataTable_
    {
      
        public static void ToCSV(this List<object[]> objects, string csvname, string savePath, int columns = 10)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < columns; i++)
            {
                if (i < 10)
                {
                    dt.Columns.Add("0" + i.ToString(), typeof(String));//添加列
                }
                else
                {
                    dt.Columns.Add(i.ToString(), typeof(String));//添加列
                }
            }

            dt.Rows.Add(new object[] { csvname });

            foreach (var item in objects)
            {
                dt.Rows.Add(item);
            }

            if (savePath.IsNullOrEmpty())
            {
                //"由于该文件尚未保存，本次运算不会生成指标统计的Excel文件，请保存Revit文件后，再次点击-指标刷新-功能。".TaskDialogErrorMessage();
            }
            else
            {
                bool delete = FileUtils.DeleteFile(savePath);
                if (delete)
                {
                    // 表格数据添加生成时间
                    dt.Rows.Add(new object[] {DateTime.Now });

                    // 删除成功，才允许写出文件
                    dt.ToCSV(savePath);
                }
            }

        }
    }
}
