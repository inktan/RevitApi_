using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Teigha.DatabaseServices;
using Teigha.Runtime;

class Program
{
    static void Main()
    {
        //string assemblyPaht = @"C:\Autodesk\Teigha SDK 4.2官方文档 及使用汇总\Teigha 使用汇总\Teigha_Net_3_09_10(x64)\TD_Mgd.dll";
        //Assembly a = Assembly.UnsafeLoadFrom(assemblyPaht);

        string pathfile = @"Z:\G2023-0068\00-Workshop\ARCH\WORK\DD\PLOT-2\3#\10_PLAN\test_dev.dwg";
        if (!File.Exists(pathfile))
        {
            Console.WriteLine("文件不存在！");
            return;
        }
        List<string> LayerNames = new List<string>();
        using (Services srv = new Services())
        {
            using (Database database = new Database(false, false))
            {
                database.ReadDwgFile(pathfile, FileOpenMode.OpenForReadAndAllShare, true, "");
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

                // 指向 BlockTable
                using (BlockTable blockTable = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForRead))
                {
                    // 循环 BlockTable
                    using (SymbolTableEnumerator enumerator = blockTable.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Console.WriteLine(enumerator.Current.ToString());
                            using (BlockTableRecord blockTableRecord = (BlockTableRecord)enumerator.Current.GetObject(OpenMode.ForRead))
                            {

                                Console.WriteLine(blockTableRecord.PathName.ToString());
                                //Console.WriteLine(blockTableRecord.Comments.ToString());
                                Console.WriteLine(blockTableRecord.Name.ToString());


                                // 循环 BlockTableRecord
                                foreach (ObjectId objectId in blockTableRecord)
                                {
                                    if (objectId != null && !objectId.IsNull && !objectId.IsErased && objectId.IsValid)
                                    {
                                        //this.indent++;
                                        //ParseEntity(objectId);
                                    }
                                }
                            }
                        }
                    }
                }


            }
        }

        // 将列表中的字符串合并为一个字符串
        //string combinedString = string.Join("\n ", LayerNames);
        //Console.WriteLine(combinedString);

        //Console.ReadKey();
    }
}
