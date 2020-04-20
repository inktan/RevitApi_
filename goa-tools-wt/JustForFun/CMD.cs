using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.ApplicationServices;
using System.IO;

namespace JustForFun
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            FamilySymbol columnType = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Columns).OfClass(typeof(FamilySymbol)).FirstOrDefault(x => x.Name == "457 x 475mm") as FamilySymbol;

            Level level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).FirstOrDefault(x => x.Name == "标高 1") as Level;

            List<XYZ> xyzlist = new List<XYZ>();
            for (int i = 0; i < 72; i++)
            {
                double x = 10 * (2 * Math.Cos(2 * Math.PI / 72 * i) - Math.Cos(2 * 2 * Math.PI / 72 * i));
                double y = 10 * (2 * Math.Sin(2 * Math.PI / 72 * i) - Math.Sin(2 * 2 * Math.PI / 72 * i));
                XYZ start = new XYZ(x, y, 0);
                xyzlist.Add(start);
            }
            double height = 15 / 0.3048;
            double offset = 0;

            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
            Transaction trans = new Transaction(doc, "创建多个柱子");
            foreach (XYZ item in xyzlist)
            {
                trans.Start();
                columnType.Activate();
                FamilyInstance column = doc.Create.NewFamilyInstance(item, columnType, level, StructuralType.NonStructural);
                trans.Commit();
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(100);
                familyInstances.Add(column);
            }

            Transaction transRotate = new Transaction(doc, "创建多个柱子");
            for (int j = 0; j < 100; j++)
            {
                transRotate.Start();
                for (int k = 0; k < xyzlist.Count; k++)
                {
                    Line line = Line.CreateBound(xyzlist[k], new XYZ(xyzlist[k].X, xyzlist[k].Y, 10));
                    ElementTransformUtils.RotateElement(doc, familyInstances[k].Id, line, Math.PI / 6.0);

                }
                transRotate.Commit();
                System.Windows.Forms.Application.DoEvents();

            }
            return Result.Succeeded;
        }
    }
}
