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
    internal class Test03 : RequestMethod
    {
        internal Test03(UIApplication _uiApp) : base(_uiApp)
        {


        }

        internal override void Execute()
        {
            // 读取 CSV 文件
            string csvFilePath = @"C:\BeamData.csv";
            List<string[]> csvData = ReadCSV(csvFilePath);

            using (Transaction trans = new Transaction(doc, "Update Beam Dimensions"))
            {
                trans.Start();

                // 跳过标题行
                for (int i = 1; i < csvData.Count; i++)
                {
                    string[] row = csvData[i];
                    int beamId = int.Parse(row[0]);
                    double beamWidth = double.Parse(row[1]);
                    double beamHeight = double.Parse(row[2]);

                    // 根据 ID 获取梁元素
                    Element beam = doc.GetElement(new ElementId(beamId));
                    if (beam is FamilyInstance fi)
                    {
                        try
                        {
                            fi.LookupParameter("宽度").Set(beamWidth);
                            fi.LookupParameter("高度").Set(beamHeight);
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }

                // 提交事务
                trans.Commit();
            }

            TaskDialog.Show("提示", "梁数据已成功更新");

            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
        // 读取 CSV 文件
        private List<string[]> ReadCSV(string filePath)
        {
            List<string[]> data = new List<string[]>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }
            return data;
        }
    }
}
