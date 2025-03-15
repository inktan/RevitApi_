using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Document_
    {
        public static void SaveCsv(this Document doc, List<object[]> objects, string csvname)
        {
            // 将数据写出到csv
            string filePath = doc.PathName;
            int length = filePath.Length;

            if (length<1)
            {
                throw new NotImplementedException("请保存当前Revit文件后，再启动该插件，进行土方平衡计算。");
            }

            filePath = filePath.Substring(0, length - 4) + csvname + @".csv";

            if (filePath.IsNullOrEmpty())
            {
                //"由于该文件尚未保存，本次运算不会生成指标统计的Excel文件，请保存Revit文件后，再次点击-指标刷新-功能。".TaskDialogErrorMessage();
            }
            else
            {
                objects.ToCSV(csvname, filePath);
            }
        }
        public static void DelEleId(this Document document, ElementId elementId)
        {
            if (document.GetElement(elementId) == null)
            {
                return;
            }
            using (Transaction transaction = new Transaction(document, "删除单个图元"))
            {
                transaction.Start();
                document.Delete(elementId);
                transaction.Commit();
            }
        }
        public static void DelEleIds(this Document document, ICollection<ElementId> elementIds)
        {
            List<ElementId> eleIds = elementIds.Where(p => document.GetElement(p) != null).ToList();
            using (Transaction transaction = new Transaction(document, "同时删除多个图元"))
            {
                transaction.Start();
                document.Delete(eleIds);
                transaction.Commit();
            }
        }
    }
}
