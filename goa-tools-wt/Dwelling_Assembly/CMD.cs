using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

using goa.Common;

namespace Dwelling_Assembly
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        //声明全局静态变量
        public static string[] houseType = {//房型
            "2 房1卫",
            "2+1 房1卫",
            "2+1 房2卫",
            "3 房1卫",
            "3 房2卫",
            "3+1 房2卫",
            "4 房2卫",
            "4 房3卫",
            "4 房4卫",
            "4+1 房2卫",
            "4+1 房3卫" };
        public static string[] kaiJianNum = {//开间
            "2",
            "2_5",
            "3",
            "3_5",
            "4",
            "4_5",
            "5" };
        public static string[] ketingweizhi = {//客厅位置
            "BT_边厅",
            "ST_南厅",
            "NT_北厅" };
        //声明静态RVT路径
        public static string _TXA_rvt = "null_path";
        public static string _TXB_rvt = "null_path";
        public static string _TXC_rvt = "null_path";
        public static string _TXD_rvt = "null_path";
        public static string _TXA_HXT_rvt = "null_path";
        public static string _TXB_HXT_rvt = "null_path";
        public static string _TXC_HXT_rvt = "null_path";
        public static string _TXD_HXT_rvt = "null_path";

        public static string _TXA_name = "null_path";
        public static string _TXB_name = "null_path";
        public static string _TXC_name = "null_path";
        public static string _TXD_name = "null_path";
        public static string _TXA_HXT_name = "null_path";
        public static string _TXB_HXT_name = "null_path";
        public static string _TXC_HXT_name = "null_path";
        public static string _TXD_HXT_name = "null_path";

        public static string _Recalloptions = "";

        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            try
            {
                MainWindow form = APP.MainWindow;
                if (null != form && form.IsVisible == true)
                {
                    form.Activate();
                    return Result.Succeeded;
                }

                //show new window
                if (null == form || !form.IsLoaded)
                    form = new MainWindow();
                APP.MainWindow = form;
                form.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }// excute
    }//class
}//namespace
