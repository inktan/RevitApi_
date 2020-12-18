using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutColumnGrid
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        private Guid schemaGuid = new Guid("BB0FDB2E-09D0-4C2A-8A3C-56973CACC67E");
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region UI数据交互入口
            UIApplication uiapp = commandData.Application;
            UIDocument uIdoc = uiapp.ActiveUIDocument;
            Document doc = uIdoc.Document;
            #endregion

            #region 创建轴网基准线 3000mmm*3000mm 5*5
            List<Line> horizontalLines = new List<Line>();
            List<Line> verticalLines = new List<Line>();

            double distance = 3000;//轴网间距
            double _distance = UnitUtils.Convert(distance, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);

            XYZ origin = XYZ.Zero;
            XYZ horizontalXyzStart = new XYZ(-_distance, 0, 0);
            XYZ verticalXyzStart = new XYZ(0, -_distance, 0);

            int count = 6;
            double distanceBuffer = _distance * count;

            XYZ horizontalXyzEnd = new XYZ(-_distance + distanceBuffer, 0, 0);
            XYZ verticalXyzEnd = new XYZ(0, -_distance + distanceBuffer, 0);

            XYZ horizontalMoveDistance = new XYZ(0, _distance, 0);
            XYZ verticalMoveDistance = new XYZ(_distance, 0, 0);

            for (int i = 0; i < 5; i++)
            {
                horizontalLines.Add(Line.CreateBound(horizontalXyzStart + horizontalMoveDistance * i, horizontalXyzEnd + horizontalMoveDistance * i));
                verticalLines.Add(Line.CreateBound(verticalXyzStart + verticalMoveDistance * i, verticalXyzEnd + verticalMoveDistance * i));
            }
            #endregion

            List<string> ints = new List<string>() { "1", "2", "3", "4", "5" };
            List<string> letters = new List<string>() { "A", "B", "C", "D", "E" };

            #region 基于基准线，创建轴网
            using (var craetGrids = new Transaction(doc, "创建轴网"))
            {
                craetGrids.Start();
                for (int i = 0; i < 5; i++)
                {
                    Grid gridH = Grid.Create(doc, horizontalLines[i]);
                    gridH.Name = letters[i];
                    Grid gridV = Grid.Create(doc, verticalLines[i]);
                    gridV.Name = ints[i];
                }
                craetGrids.Commit();
            }
            #endregion

            FamilySymbol columnSymbol = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Columns).OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Cast<FamilySymbol>().First();
            Level level = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).WhereElementIsNotElementType().Cast<Level>().First();

            #region 在轴网相交的位置，放置柱子，并同时写入entity

            using (var craetGrids = new Transaction(doc, "放置柱子，并写入entity"))
            {
                craetGrids.Start();
                int i = 0;
                columnSymbol.Activate();

                verticalLines.ForEach(p0 =>
                {
                    int j = 0;
                    horizontalLines.ForEach(p1 =>
                    {
                        IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
                        SetComparisonResult overLap = p0.Intersect(p1, out intersectionResultArray);

                        XYZ interSectPoint = intersectionResultArray.get_Item(0).XYZPoint;

                        FamilyInstance parkingColumn = doc.Create.NewFamilyInstance(interSectPoint, columnSymbol, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                        SetEntityToBaseWallLoop(doc, schemaGuid, parkingColumn.Id, ints[i].ToString() + letters[j].ToString());

                        j++;
                    });
                    i++;
                });
                craetGrids.Commit();
            }

            #endregion

            return Result.Succeeded;
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 给column中写入entity
        /// </summary>
        public void SetEntityToBaseWallLoop(Document doc, Guid guid, ElementId elementId, string writeValue)
        {
            #region 设置entity 将上述生成的各个停车区域点集 存入地库外墙线线圈group的entity

            Schema schema = Schema.Lookup(guid);//guid为固定guid
            if (schema == null)
            {
                SchemaBuilder schemaBulider = new SchemaBuilder(schemaGuid);
                schemaBulider.SetReadAccessLevel(AccessLevel.Public);
                schemaBulider.SetWriteAccessLevel(AccessLevel.Public);
                schemaBulider.SetSchemaName("canParkingPlaces");
                schemaBulider.SetDocumentation("该类架构用于存储各个可停车区域的点集。");

                FieldBuilder fieldBuilder = schemaBulider.AddSimpleField("axisNumber", typeof(string));
                schema = schemaBulider.Finish();
            }
            #endregion

            #region entity赋值

            Entity entity = new Entity(schema);

            entity.Set<string>("axisNumber", writeValue);

            doc.GetElement(elementId).SetEntity(entity);

            #endregion
        }
    }
}
