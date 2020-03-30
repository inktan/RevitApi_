using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

//计算填充面积

namespace ANNO_AREA
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication application = commandData.Application;

            //get familySymbol
            FilteredElementCollector familycollector = new FilteredElementCollector(doc);
            familycollector.OfClass(typeof(Family));
            List<ElementId> symbolids = new List<ElementId>();
            foreach (Family fam in familycollector)
            {
                if (fam.Name == "A-SYMB-FIRE")
                { symbolids = fam.GetFamilySymbolIds().ToList(); }
            }
            if (symbolids.Count == 0) { TaskDialog.Show("wrong", "没有找到<A-SYMB-FIRE>族的类型"); }
            List<FamilySymbol> symbols = new List<FamilySymbol>();
            foreach(ElementId id in symbolids)
            { symbols.Add(doc.GetElement(id) as FamilySymbol); }

            //window
            Window1 window = new Window1();
            window.listbox.ItemsSource = symbols;

            //selected
            FamilySymbol annotype = null;
            if (window.ShowDialog() == true)
            {
                var obj = window.listbox.SelectedItem;
                annotype = obj as FamilySymbol;
            }




            //string annoname = "A-SYMB-FIRE";
            //FamilySymbol annotype = null;
            //bool symbolfound = false;
            //ElementCategoryFilter catefilter = new ElementCategoryFilter(BuiltInCategory.OST_GenericAnnotation);
            //ElementClassFilter classfilter = new ElementClassFilter(typeof(FamilySymbol));
            //FilteredElementCollector collector = new FilteredElementCollector(doc);
            //collector.WherePasses(catefilter).WherePasses(classfilter);
            //foreach (FamilySymbol fs in collector)
            //{
            //    if(fs.Name == annoname)
            //    {
            //        symbolfound = true;
            //        annotype = fs;
            //        break;
            //    }
            //}
            //if(!symbolfound) { TaskDialog.Show("未找到", "请载入<A-SYMB-FIRE>族"); return Result.Failed; }





            //select filled region
            IList<Reference> selection = uiDoc.Selection.PickObjects(ObjectType.Element, "选择要计算面积的填充区域");

            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("计算填充区域面积");

                foreach (Reference refe in selection)
                {
                    //get area para
                    FilledRegion filledregion = doc.GetElement(refe.ElementId) as FilledRegion;
                    if (filledregion == null) { continue; }
                    Parameter filledregionparameter = filledregion.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
                    string str = filledregionparameter.AsValueString();
                    str = "(" + str + "平方米)";

                    //get location
                    Location filledregionlocation = filledregion.Location;

                    //set a text 
                    BoundingBoxXYZ boundingboxxyz = filledregion.get_BoundingBox(uiDoc.ActiveView);
                    XYZ max = boundingboxxyz.Max;
                    XYZ min = boundingboxxyz.Min;
                    XYZ xyz = new XYZ((max.X + min.X) / 2, (max.Y + min.Y) / 2, (max.Z + min.Z) / 2);
                    FamilyInstance annoinstance = doc.Create.NewFamilyInstance(xyz, annotype, uiDoc.ActiveView);

                    //get parameter
                    ParameterSet annoparameterset = annoinstance.Parameters;
                    Parameter annoparameter = null;
                    foreach (Parameter para in annoparameterset)
                    {
                        if (para.Definition.Name == "面积" && para.Definition.ParameterType == ParameterType.Text)
                        {
                            annoparameter = para;
                        }
                    }
                    if (annoparameter == null) { TaskDialog.Show(" wrong", "未找到族内名为面积的参数"); return Result.Failed; }
                    annoparameter.Set(str);
                    //TaskDialog.Show(" ", annoparameter.AsString());

                }

                transaction.Commit();
            }

            return Result.Succeeded;
        }









    }
}
