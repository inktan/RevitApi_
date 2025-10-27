using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.Runtime;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;

[assembly: ExtensionApplication(typeof(ZrxDotNetCSProject.PlugInApplication))]
[assembly: CommandClass(typeof(ZrxDotNetCSProject.Commands))]

namespace ZrxDotNetCSProject
{
    public class PlugInApplication : IExtensionApplication
    {
        public void Initialize()
        {
            // Add your initialize code here.
        }

        public void Terminate()
        {
            // Add your uninitialize code here.
        }
    }

    class Commands
    {
        [CommandMethod("HelloCS")]
        static public void HelloCS()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Application.DocumentManager.GetDocument(db);
            Editor ed = doc.Editor;

            ed.WriteMessage("\nHello World!");

            PromptPointResult promptPointResult01 = ed.GetPoint("请输入第一个点");
            if (promptPointResult01.Status == PromptStatus.OK)
            {
                PromptPointResult promptPointResult00 = ed.GetPoint("请输入第二个点");
            }

            // 开启事务
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // 获取块表
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                // 获取模型空间块表记录
                BlockTableRecord modelSpace = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 在模型空间中添加一条直线
                Line line = new Line(new Point3d(0, 0, 0), new Point3d(100, 100, 0));
                modelSpace.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                // 提交事务
                tr.Commit();
            }

        }

    }
}
