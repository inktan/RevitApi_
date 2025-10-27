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

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
using goa.Common.Exceptions;
using System.Windows.Controls;
//using NetTopologySuite.Geometries;

namespace DimensioningTools
{
    internal class AbsElev : RequestMethod
    {
        internal AbsElev(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            Document document = this.doc;
            double num = 0;
            View activeView = this.view;
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
            filteredElementCollector.WherePasses(new ElementClassFilter(typeof(SpotDimensionType)));
            List<SpotDimensionType> list = (
                from e in filteredElementCollector
                select e as SpotDimensionType).ToList<SpotDimensionType>();
            MyWindow myWindow = new MyWindow();
            List<CheckBox> checkBoxes = new List<CheckBox>();
            foreach (SpotDimensionType spotDimensionType in list)
            {
                CheckBox checkBox = new CheckBox()
                {
                    Content = spotDimensionType.Name
                };
                checkBoxes.Add(checkBox);
            }
            myWindow.spotdimensionlistbox.ItemsSource = checkBoxes;
            bool? isChecked = myWindow.ShowDialog();
            if (isChecked.GetValueOrDefault() & isChecked.HasValue)
            {
                List<string> strs = new List<string>();
                foreach (CheckBox itemsSource in myWindow.spotdimensionlistbox.ItemsSource)
                {
                    isChecked = itemsSource.IsChecked;
                    if (!isChecked.GetValueOrDefault() | !isChecked.HasValue)
                    {
                        strs.Add(itemsSource.Content as string);
                    }
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    foreach (string str in strs)
                    {
                        if (list[i].Name == str)
                        {
                            list.Remove(list[i]);
                            break;
                        }
                    }
                }
                num = Convert.ToDouble(myWindow.sealeveltextbox.Text) * 1000;
                using (Transaction transaction = new Transaction(document))
                {
                    transaction.Start("HeightAboveSeaLevel");
                    foreach (SpotDimensionType spotDimensionType1 in list)
                    {
                        FilteredElementCollector filteredElementCollector1 = new FilteredElementCollector(document, activeView.Id);
                        filteredElementCollector1.WherePasses(new ElementCategoryFilter((BuiltInCategory)(-2000263)));
                        List<SpotDimension> spotDimensions = new List<SpotDimension>();
                        spotDimensions = (
                            from e in filteredElementCollector1
                            select e as SpotDimension).ToList<SpotDimension>();
                        for (int j = spotDimensions.Count - 1; j >= 0; j--)
                        {
                            if (spotDimensions[j].GetTypeId() == spotDimensionType1.Id)
                            {
                                double num1 = 0;
                                Parameter parameter = spotDimensions[j].get_Parameter((BuiltInParameter)(-1006490));
                                num1 = (parameter.AsDouble() * 304.8 + num) / 1000;
                                string str1 = string.Concat("(", num1.ToString("f3"), ")");
                                Parameter parameter1 = spotDimensions[j].get_Parameter((BuiltInParameter)(-1006479));
                                parameter1.Set(str1);
                            }
                            else
                            {
                                spotDimensions.Remove(spotDimensions[j]);
                            }
                        }
                    }
              this.uiDoc.RefreshActiveView();
                    transaction.Commit();
                }
            }
        }

        public static void Run(UIDocument uidoc)
        {
            Document document = uidoc.Document;

            double num = 0;
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
            filteredElementCollector.WherePasses(new ElementClassFilter(typeof(SpotDimensionType)));
            List<SpotDimensionType> list = (
                from e in filteredElementCollector
                select e as SpotDimensionType).ToList<SpotDimensionType>();
            MyWindow myWindow = new MyWindow();
            List<CheckBox> checkBoxes = new List<CheckBox>();
            foreach (SpotDimensionType spotDimensionType in list)
            {
                CheckBox checkBox = new CheckBox()
                {
                    Content = spotDimensionType.Name
                };
                checkBoxes.Add(checkBox);
            }
            myWindow.spotdimensionlistbox.ItemsSource = checkBoxes;
            bool? isChecked = myWindow.ShowDialog();
            if (isChecked.GetValueOrDefault() & isChecked.HasValue)
            {
                List<string> strs = new List<string>();
                foreach (CheckBox itemsSource in myWindow.spotdimensionlistbox.ItemsSource)
                {
                    isChecked = itemsSource.IsChecked;
                    if (!isChecked.GetValueOrDefault() | !isChecked.HasValue)
                    {
                        strs.Add(itemsSource.Content as string);
                    }
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    foreach (string str in strs)
                    {
                        if (list[i].Name == str)
                        {
                            list.Remove(list[i]);
                            break;
                        }
                    }
                }
                num = Convert.ToDouble(myWindow.sealeveltextbox.Text) * 1000;
                using (Transaction transaction = new Transaction(document))
                {
                    transaction.Start("HeightAboveSeaLevel");
                    foreach (SpotDimensionType spotDimensionType1 in list)
                    {
                        FilteredElementCollector filteredElementCollector1 = new FilteredElementCollector(document, document.ActiveView.Id);
                        filteredElementCollector1.WherePasses(new ElementCategoryFilter((BuiltInCategory)(-2000263)));
                        List<SpotDimension> spotDimensions = new List<SpotDimension>();
                        spotDimensions = (
                            from e in filteredElementCollector1
                            select e as SpotDimension).ToList<SpotDimension>();
                        for (int j = spotDimensions.Count - 1; j >= 0; j--)
                        {
                            if (spotDimensions[j].GetTypeId() == spotDimensionType1.Id)
                            {
                                double num1 = 0;
                                Parameter parameter = spotDimensions[j].get_Parameter((BuiltInParameter)(-1006490));
                                num1 = (parameter.AsDouble() * 304.8 + num) / 1000;
                                string str1 = string.Concat("(", num1.ToString("f3"), ")");
                                Parameter parameter1 = spotDimensions[j].get_Parameter((BuiltInParameter)(-1006479));
                                parameter1.Set(str1);
                            }
                            else
                            {
                                spotDimensions.Remove(spotDimensions[j]);
                            }
                        }
                    }
                    uidoc.RefreshActiveView();
                    transaction.Commit();
                }
            }
        }


    }
}
