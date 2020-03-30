using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


//疏散距离(量多跟符号线的长度)

namespace ANNO_DIST
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            var selection = uidoc.Selection;
            if(selection == null) { TaskDialog.Show("wrong", "未选中符号线");return Result.Failed; }
            List<ElementId> elementIds = selection.GetElementIds().ToList();
            List<DetailCurve> detailCurves = new List<DetailCurve>();
            List<double> doubles = new List<double>();
            double length = 0;
            foreach (ElementId id in elementIds)
            {
                DetailCurve detailCurve = uidoc.Document.GetElement(id) as DetailCurve;
                if (detailCurve == null) { continue; }

                detailCurves.Add(detailCurve);
                doubles.Add(detailCurve.GeometryCurve.Length);
                length += detailCurve.GeometryCurve.Length;
            }
            string str1 = "疏散距离" + (length * 304.8 / 1000).ToString("f1") + "M";

            //prompt
            string str2 = "总共选中了" + Convert.ToString(detailCurves.Count) + "条线\n" + "分别长:";
            foreach (double dou in doubles)
            {
                str2 += "\n" + Convert.ToString(dou * 304.8);
            }
            str2 += "\n总长度:" + Convert.ToString(length * 304.8);
            str2 += "\n 请点击一点以确定放置标注的位置";
            TaskDialog.Show("GOA", str2);

            //get type
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(TextNoteType));
            TextNoteType textnotetype = null;
            foreach (TextNoteType type in collector)
            {
                if(type.Name == "G-Simhei-4.0-0.60-FIRE")
                {textnotetype = type;}
            }
            if(textnotetype == null) { TaskDialog.Show("wrong", "未找到名为G-Simhei-4.0-0.60-FIRE的字体");return Result.Failed; }

            #region 开启事务
            using (Transaction transaction = new Transaction(doc))  //放置文字
            {
                transaction.Start("疏散距离标注");
                ElementId elementid = doc.ActiveView.Id;
                ElementId elementid2 = textnotetype.Id;
                var point = uidoc.Selection.PickPoint(Autodesk.Revit.UI.Selection.ObjectSnapTypes.None, "选择文字插入点");
                TextNote textnote = TextNote.Create(doc, elementid, point, str1, elementid2);
                transaction.Commit();
            }
            #endregion

            return Result.Succeeded;
        }
    }
}

