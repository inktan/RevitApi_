using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
//using NetTopologySuite.Geometries;

namespace ReadCadText
{
    internal class ExacText : RequestMethod
    {
        internal ExacText(UIApplication _uiApp) : base(_uiApp)
        {
        }

        public string PathName { get; set; }

        internal override void Execute()
        {
            PickCad pickCad = new PickCad(uiApp);
            pickCad.Execute();

            // 锁定图层的正则表达式
            List<string> patterns = new List<string>()
            {
                @".*a-room-iden",
            };

            // 文字
            List<TextInfo> textInfos = new List<TextInfo>();

            foreach (var pattern in patterns)
            {
                foreach (var item in pickCad.DwgParser.LayerNames)
                {
                    if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                    {
                        if (pickCad.DwgParser.TexLayertInfos.ContainsKey(item))
                        {
                            textInfos.AddRange(pickCad.DwgParser.TexLayertInfos[item]);
                        }
                    }
                }
            }
            //textInfos.Count.ToString().TaskDialogErrorMessage();

            List<TextNoteType> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).WhereElementIsElementType().Cast<TextNoteType>().ToList();
            string textNoteTypeName = @"明细表默认 3.0mm";
            TextNoteType textNoteType = textNoteTypes.Where(p => p.Name == textNoteTypeName).FirstOrDefault();

            using (Transaction duplicateTextNoteType = new Transaction(doc, "duplicateTextNoteType"))
            {
                duplicateTextNoteType.Start();
                if (textNoteType == null)
                {
                    textNoteType = textNoteTypes.First().Duplicate(textNoteTypeName) as TextNoteType;
                }
                textNoteType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(3.0.MilliMeterToFeet());
                duplicateTextNoteType.Commit();
            }

            // 存储梁数据的列表
            List<string[]> beamData = new List<string[]>();
            beamData.Add(new string[] { "id", "宽度", "高度" });

            Polygon2d polygon = this.view.CropBox.ToPolygon2d();
            int count=0;
            foreach (var item in textInfos)
            {
                XYZ position = item.Position + CMD.PositonMoveDis;
                if (!polygon.Contains(position.ToVector2d()))
                {
                    continue;
                }
                beamData.Add(new string[] { item.Text, item.Layer, item.Position.ToString() });
                try
                {
                    using (Transaction trans = new Transaction(this.doc))
                    {
                        trans.Start("基于cad创建文字");
                        TextNote textNote = TextNote.Create(this.doc, this.view.Id, position, item.Text, textNoteType.Id);
                        textNote.HorizontalAlignment = HorizontalTextAlignment.Right;
                        trans.Commit();
                    }
                    count++;
                }
                catch (Exception e)
                {
                    //e.ToString().TaskDialogErrorMessage();
                    //break;
                    //continue;
                    //throw;
                }
            }
            //count.ToString().TaskDialogErrorMessage();
            "基于cad绘制文字完成".TaskDialogErrorMessage();
            // 保存为 CSV 文件
            //string csvFilePath = @"C:\BeamData.csv";
            //SaveToCSV(beamData, csvFilePath);

        }
        // 保存数据到 CSV 文件
        private void SaveToCSV(List<string[]> data, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string[] row in data)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }


    }
}
