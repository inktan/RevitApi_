using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Linq;
using System.Windows.Interop;
using System.Reflection;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;
using System.Xaml;
using Teigha.Runtime;
using Teigha.DatabaseServices;

namespace CadTest
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //string assemblyPaht = @"C:\Autodesk\Teigha SDK 4.2�ٷ��ĵ� ��ʹ�û���\Teigha ʹ�û���\Teigha_Net_3_09_10(x64)\TD_Mgd.dll";
            //Assembly a = Assembly.UnsafeLoadFrom(assemblyPaht);

            string pathfile = @"Z:\G2023-0068\00-Workshop\ARCH\WORK\DD\PLOT-2\4#\02�ŵؿ�4��¥-TPLT-ALL.dwg";
            List<string> LayerNames = new List<string>();
            using (Services srv = new Services())
            {
                using (Database database = new Database(false, false))
                {
                    database.ReadDwgFile(pathfile, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
                    using (var trans = database.TransactionManager.StartTransaction())
                    {
                        using (LayerTable lt = (LayerTable)trans.GetObject(database.LayerTableId, OpenMode.ForRead))
                        {
                            foreach (ObjectId id in lt)
                            {
                                LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(id, OpenMode.ForRead);
                                LayerNames.Add(ltr.Name);
                            }
                        }
                        trans.Commit();
                    }
                }
            }

            // ���б��е��ַ����ϲ�Ϊһ���ַ���
            string combinedString = string.Join("\n ", LayerNames);
            TaskDialog.Show("revit", combinedString);

            return Result.Succeeded;
            //throw new NotImplementedException();
        }
    }
}



