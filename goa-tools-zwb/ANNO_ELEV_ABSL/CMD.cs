using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

//绝对标高

namespace ANNO_ELEV_ABSL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            double sealevel = 0.0;

            //获取视图
            View activeview = uiDoc.ActiveView;

            #region 获取高程点类型
            FilteredElementCollector filteredElements = new FilteredElementCollector(doc);
            ElementClassFilter classFilter = new ElementClassFilter(typeof(SpotDimensionType));
            filteredElements.WherePasses(classFilter);
            List<SpotDimensionType> spotdimensiontypeList = filteredElements.Select(e => e as SpotDimensionType).ToList();
            #endregion

            #region 窗口
            //new一个对话框
            MyWindow mywindow = new MyWindow();
            //为高程点类型的ListBox赋值.
            List<System.Windows.Controls.CheckBox> checkboxs1 = new List<System.Windows.Controls.CheckBox>();
            foreach (SpotDimensionType type in spotdimensiontypeList)
            {
                System.Windows.Controls.CheckBox checkBox = new System.Windows.Controls.CheckBox();
                checkBox.Content = type.Name;
                checkboxs1.Add(checkBox);
            }
            mywindow.spotdimensionlistbox.ItemsSource = checkboxs1;
            #endregion

            if (mywindow.ShowDialog() == true)
            {
                #region 获取选中的标高类型
                List<string> uncheckeds2 = new List<string>();
                foreach (System.Windows.Controls.CheckBox whoisunchecked in mywindow.spotdimensionlistbox.ItemsSource)
                {
                    if (whoisunchecked.IsChecked != true) { uncheckeds2.Add(whoisunchecked.Content as string); }
                }
                for (int i = spotdimensiontypeList.Count - 1; i >= 0; i--)
                {
                    foreach (string str in uncheckeds2)
                    {
                        if (spotdimensiontypeList[i].Name == str)
                        {
                            spotdimensiontypeList.Remove(spotdimensiontypeList[i]);
                            break;
                        }
                    }
                }
                #endregion

                sealevel = Convert.ToDouble(mywindow.sealeveltextbox.Text) * 1000;

                #region 开启事务
                using (Transaction transaction = new Transaction(doc))  //修改高程点
                {
                    transaction.Start("HeightAboveSeaLevel");
                        foreach (SpotDimensionType type in spotdimensiontypeList)
                        {
                            FilteredElementCollector collector = new FilteredElementCollector(doc, activeview.Id);
                            ElementCategoryFilter categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_SpotElevations);
                            collector.WherePasses(categoryfilter);

                            List<SpotDimension> spotdimensionlist = new List<SpotDimension>();
                            spotdimensionlist = collector.Select(e => e as SpotDimension).ToList();
                            for (int i = spotdimensionlist.Count - 1; i >= 0; i--)
                            {
                                if (spotdimensionlist[i].GetTypeId() != type.Id) //视图样板也属于视图,可以用istemplate过滤.
                                {
                                    spotdimensionlist.Remove(spotdimensionlist[i]);
                                }
                                else
                                {
                                    double height = 0.0;
                                    ParameterSet parameters = spotdimensionlist[i].Parameters;
                                    Parameter para1 = spotdimensionlist[i].get_Parameter(BuiltInParameter.SPOT_ELEV_SINGLE_OR_UPPER_VALUE);
                                    height = (para1.AsDouble() * 304.8 + sealevel) / 1000;
                                    string heightstr = "(" + height.ToString("f3") + ")";
                                    Parameter para2 = spotdimensionlist[i].get_Parameter(BuiltInParameter.SPOT_ELEV_SINGLE_OR_UPPER_SUFFIX);
                                    para2.Set(heightstr);
                                }
                            }
                        }                 
                    uiDoc.RefreshActiveView();
                    transaction.Commit();
                }
                #endregion
            }

            return Result.Succeeded;
        }
    }
}
