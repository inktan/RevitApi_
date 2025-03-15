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
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Windows;

namespace InfoStrucFormwork
{
    internal class Test02 : RequestMethod
    {
        internal Test02(UIApplication _uiApp) : base(_uiApp)
        {


        }

        internal override void Execute()
        {
            List<Level> levels =
                (new FilteredElementCollector(CMD.Doc))
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);

            List<FamilyInstance> familyInstances = new List<FamilyInstance>();

            // 存储梁数据的列表
            List<string[]> beamData = new List<string[]>();
            beamData.Add(new string[]{ "id", "宽度", "高度" });

            foreach (var item in collector.WherePasses(filter).ToElements())
            {
                if (!(item is FamilyInstance fi)) continue;

                string faName = fi.Symbol.FamilyName;
                if (faName.Contains("梁"))
                {
                    familyInstances.Add(fi);
                    double weight = fi.LookupParameter("宽度").AsDouble();
                    double height = fi.LookupParameter("高度").AsDouble();
                    // 将数据添加到列表中
                    beamData.Add(new string[] { fi.Id.ToString(), weight.ToString(), height.ToString() });

                }
            }
            familyInstances.Count.ToString().TaskDialogErrorMessage();

            // 保存为 CSV 文件
            string csvFilePath = @"C:\BeamData.csv";
            SaveToCSV(beamData, csvFilePath);

            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
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
