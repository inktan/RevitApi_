using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    //public static class TextNoteType_
    //{
    //    /// <summary>
    //    /// 寻找字体类型 （类型名字，字体大小已改，字体未改，默认为Aril）
    //    /// </summary>
    //    /// <param name="doc"></param>
    //    /// <param name="textNoteTypeName"></param>
    //    /// <param name="textSiz"></param>
    //    /// <returns></returns>
    //    public static TextNoteType FindTxType(this Document doc, string textNoteTypeName = @"明细表默认 3.0mm", double textSiz = 3.0)
    //    {
    //        List<TextNoteType> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).WhereElementIsElementType().Cast<TextNoteType>().ToList();
    //        TextNoteType textNoteType = textNoteTypes.Where(p => p.Name == textNoteTypeName).FirstOrDefault();

    //        using (Transaction duplicateTextNoteType = new Transaction(doc, "duplicateTextNoteType"))
    //        {
    //            duplicateTextNoteType.Start();
    //            if (textNoteType == null)
    //            {
    //                textNoteType = textNoteTypes.First().Duplicate(textNoteTypeName) as TextNoteType;
    //            }
    //            textNoteType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(textSiz.MilliMeterToFeet());
    //            duplicateTextNoteType.Commit();
    //        }

    //        return textNoteType;
    //    }
    //}
}
