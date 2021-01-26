using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Entity_
    {

        #region Entity schema externalStroge 架构 外部存储相关

        public static void SetEntity(Document doc)
        {

            DataStorage dataStorage = null;

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("start");
                dataStorage = DataStorage.Create(doc);
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("start");

                // 创建数据模式 相当于class
                SchemaBuilder schemaBulider = new SchemaBuilder(Guid.NewGuid());

                schemaBulider.SetReadAccessLevel(AccessLevel.Public);
                schemaBulider.SetWriteAccessLevel(AccessLevel.Public);
                schemaBulider.SetSchemaName("myFistSchema");
                schemaBulider.SetDocumentation("Data store for socket related info in a wall");

                // 相当于创建 class 里面的 字段
                FieldBuilder fieldBuilder01 = schemaBulider.AddSimpleField("SocketLocation", typeof(XYZ));
                fieldBuilder01.SetUnitType(UnitType.UT_Length);
                FieldBuilder fieldBuilder02 = schemaBulider.AddSimpleField("SocketNumber", typeof(string));

                Schema _schema = schemaBulider.Finish();

                // 创建数据实体
                Entity entity = new Entity(_schema);

                // 为实例字段赋值
                entity.Set("SocketNumber", dataStorage.Name);

                dataStorage.SetEntity(entity);

                trans.Commit();
            }

            List<Guid> listSchemaGuid = dataStorage.GetEntitySchemaGuids().ToList();
            Schema schema = Schema.Lookup(listSchemaGuid[0]);
            Entity selEntity = dataStorage.GetEntity(schema);

            string testStr = selEntity.Get<string>("SocketNumber");
            XYZ oriPoint = selEntity.Get<XYZ>("SocketLocation", DisplayUnitType.DUT_MILLIMETERS);

            string showSuccess = testStr + "\n" + oriPoint.ToString() + "\n" + "entity测试成功";
            showSuccess.TaskDialogErrorMessage();
        }



        #endregion



    }
}
