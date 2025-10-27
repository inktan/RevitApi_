using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using goa.Common;

namespace GRID_Number
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {

        public static List<string> GridTypeNames = new List<string>();
        public static string selGridType = "";
        public static bool isContainZ = false;

        public static List<DesignOptionWrapper> designOptionSets = new List<DesignOptionWrapper>();
        public static List<string> designOptionSetNames = new List<string>();

        public static MainWindow MainWindow;

        //声明全局静态变量
        public static string changeCommand = null;//判断条件字段
        public static string partField = null;//输入分区字段
        public static string startGridName;//输入起始字段，为字母
        public static string girdHeadPara;//输入起始字段，为字母

        public static string checkOverlapGrids;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    UserMessages.ShowErrorMessage(ex, null);
            //}
            ////check domain
            //if (ADValidationCheck.GetDirectoryEntryForCurrentUser() == null)
            //{
            //    TaskDialog.Show("信息", "需要连接goa网络。");
            //    return Result.Failed;
            //}

            APP.UIApp = commandData.Application;
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            Autodesk.Revit.DB.View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            //提取文档静态参量

            //check opened window
            CMD.MainWindow = new MainWindow();
            CMD.designOptionSets = new List<DesignOptionWrapper>();
            CMD.designOptionSetNames = new List<string>();

            CMD.designOptionSetNames.Add("主模型");
            CMD.MainWindow.comboBox3.Items.Add("主模型");
            var sets = DesignOptionSet.GetDesignOptionSets(commandData.Application.ActiveUIDocument.Document);
            foreach (var set in sets)
            {
                foreach (var opt in set.DesignOptions)
                {
                    var wrapper = new DesignOptionWrapper(set, opt);
                    CMD.designOptionSets.Add(wrapper);
                    CMD.designOptionSetNames.Add(wrapper.LongName);

                    CMD.MainWindow.comboBox3.Items.Add(wrapper.LongName);
                }
            }

            GridTypeNames = GetGridTypeNames(doc);
            foreach (string str in CMD.GridTypeNames)
            {
                CMD.MainWindow.comboBox2.Items.Add(str);
            }

            CMD.MainWindow.comboBox2.SelectedIndex = 0;
            CMD.MainWindow.comboBox3.SelectedIndex = 0;

            var p = Process.GetCurrentProcess();
            var revitWindow = new WindowHandle(p.MainWindowHandle);
            CMD.MainWindow.Show(revitWindow);

            return Result.Succeeded;
        }// excute

        //以下为各种method---------------------------------分割线---------------------------------

        public List<string> GetGridTypeNames(Document doc)
        {
            List<string> GridTypeNames = new List<string>();
            List<Element> eles = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsElementType().ToElements().ToList();
            foreach (Element ele in eles)
            {
                GridTypeNames.Add(ele.Name);
            }
            return GridTypeNames;
        }

        /// <summary>
        /// 确定设计选项
        /// </summary>
        /// <returns></returns>
        internal static ElementId DesignOptId()
        {
            if (CMD.MainWindow.comboBox3.SelectedIndex == -1)
                return ElementId.InvalidElementId;
            else
            {
                var dOpt = CMD.MainWindow.comboBox3.SelectedItem.ToString();

                foreach (var item in CMD.designOptionSets)
                {
                    if (item.LongName == dOpt)
                    {
                        return item.DesignOption.Id;
                    }
                }
                return ElementId.InvalidElementId;
            }
        }
    }
}
