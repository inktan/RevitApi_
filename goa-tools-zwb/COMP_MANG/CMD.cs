using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

//组件批量管理

namespace COMP_MANG
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementset)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            #region 获取项目内的标高
            FilteredElementCollector levelcollector = new FilteredElementCollector(doc);
            ElementCategoryFilter levelfilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
            levelcollector.WhereElementIsNotElementType();
            levelcollector.WherePasses(levelfilter);
            List<Level> levellist = levelcollector.Select(e => e as Level).ToList();
            #endregion


            #region 窗口
            MyWindow mywindow = new MyWindow();
            List<System.Windows.Controls.CheckBox> checkboxs = new List<System.Windows.Controls.CheckBox>();
            foreach (Level level in levellist)
            {
                System.Windows.Controls.CheckBox checkbox = new System.Windows.Controls.CheckBox();
                checkbox.Content = level.Name;
                checkbox.IsChecked = true;
                checkboxs.Add(checkbox);

            }
            mywindow.listbox.ItemsSource = checkboxs;

            if (mywindow.ShowDialog() == true)
            {
                List<string> uncheckeds = new List<string>();
                foreach (System.Windows.Controls.CheckBox whoisunchecked in mywindow.listbox.ItemsSource)
                {
                    if (whoisunchecked.IsChecked != true) { uncheckeds.Add(whoisunchecked.Content as string); }
                }

                for (int i = levellist.Count - 1; i >= 0; i--)
                {
                    foreach (string str in uncheckeds)
                    {
                        if (levellist[i].Name == str)
                        {
                            levellist.Remove(levellist[i]);
                            break;
                        }
                    }
                }
            }
            #endregion

            #region 根据类别和类型和标高处理框的图元(保留一个filters)
            List<ElementId> elementids = uiDoc.Selection.GetElementIds().ToList();
            List<ElementFilter> filters = new List<ElementFilter>();
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            ElementCategoryFilter filter3 = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filter4 = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
            ElementCategoryFilter filter5 = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementCategoryFilter filter6 = new ElementCategoryFilter(BuiltInCategory.OST_SpecialityEquipment);
            filters.Add(filter2);
            filters.Add(filter3);
            filters.Add(filter4);
            filters.Add(filter5);
            filters.Add(filter6);
            LogicalOrFilter orfilter = new LogicalOrFilter(filters);

            List<ElementFilter> filters2 = new List<ElementFilter>();
            foreach (Level level in levellist)
            {
                ElementLevelFilter filterlevel = new ElementLevelFilter(level.Id);
                filters2.Add(filterlevel);
            }
            LogicalOrFilter orfilter2 = new LogicalOrFilter(filters2);
            #endregion


            foreach (ElementId elementid in elementids)
            {
                //使用过滤器过滤不需要的图元
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.WherePasses(orfilter);
                collector.WherePasses(orfilter2);
                collector.WhereElementIsNotElementType(); //去掉类型,仅保留模型图元.


                Element element = doc.GetElement(elementid);
                //先判断选中的图元是不是在  常规模型\家具\窗\门\专用设备  五个类别内. 
                Category category = element.Category;
                BuiltInCategory builtincategory = (BuiltInCategory)category.Id.IntegerValue;
                if ((builtincategory != BuiltInCategory.OST_Furniture) && (builtincategory != BuiltInCategory.OST_GenericModel) && (builtincategory != BuiltInCategory.OST_Windows) &&
                    (builtincategory != BuiltInCategory.OST_Doors) &&(builtincategory != BuiltInCategory.OST_SpecialityEquipment))
                { continue; }


                //再根据类型进行筛选.
                ElementId typeid = element.GetTypeId();
                FamilyInstanceFilter filter7 = new FamilyInstanceFilter(doc, typeid);
                collector.WherePasses(filter7);


                //TaskDialog.Show("revit", Convert.ToString(collector.Count()));

                //根据XY相同 Z不同进行判断是否删除
                Location location = element.Location;
                LocationPoint locationpoint = location as LocationPoint;

                List<ElementId> ids = new List<ElementId>();

                XYZ point = locationpoint.Point;

                foreach (Element ele in collector)
                {
                    if (ele.Location as LocationPoint == null) { continue; }
                    XYZ point2 = (ele.Location as LocationPoint).Point;
                    if (point.X == point2.X && point.Y == point2.Y && point.Z != point2.Z) { ids.Add(ele.Id); }
                }

                using (Transaction transaction = new Transaction(doc))  //删除重复图元
                {
                    transaction.Start("GroupLike");
                    doc.Delete(ids);
                    transaction.Commit();
                }
            }
            TaskDialog.Show("revit", "已删除处于相同XY坐标不同Z坐标的同类型图元.");
            return Result.Succeeded;
        }
    }
}
