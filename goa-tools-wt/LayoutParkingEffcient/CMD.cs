using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.IFC;

using System.Collections.ObjectModel;

using goa.Common;
using ClipperLib;
using wt_Common;

namespace LayoutParkingEffcient
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        #region 声明全局变量
        public static ElementId baseMentWallLoopId;

        public static ObservableCollection<string> TestList = new ObservableCollection<string>();

        public static double parkingPlaceHeight = Methods.MilliMeterToFeet(6000);
        public static double parkingPlaceWight = Methods.MilliMeterToFeet(2400);
        public static double Wd = Methods.MilliMeterToFeet(6000);
        public static double columnWidth = Methods.MilliMeterToFeet(500);
        public static double columnBurfferDistance = Methods.MilliMeterToFeet(50);
        public static double Wd_main = Methods.MilliMeterToFeet(6000);
        public static double redline_offset_distance = Methods.MilliMeterToFeet(6000);
        public static double basementWall_offset_distance = Methods.MilliMeterToFeet(400);

        public static string layoutMethod = "";//停车方式
        public static bool hidenDirectShape = true;//是否隐藏directShape
        public static int controlRefreshTime = 0;//用来控制，是否为首次计算车位排布
        public static bool isOptimalAlgorithm = false;//是否启动最优算法
        public static bool stopAlgorithm = true;//是否停止计算

        public static Guid parkingFixedGid = new Guid("c8f8bebf-a532-4b50-9b98-e02202990ebe");// 停车位族实例参数 停车位_固定 parameter GUID

        #endregion

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                #region 调出操作窗口
                MainWindow form = APP.MainWindow;
                if (null != form && form.IsVisible == true)
                {
                    form.Activate();
                    return Result.Succeeded;
                }

                //show new window
                else if (null == form || !form.IsLoaded)
                {
                    form = new MainWindow();
                    APP.MainWindow = form;
                    form.Show();
                }

                #endregion

                #region 注册DMu Dynamic Model Update
                //Document doc = commandData.Application.ActiveUIDocument.Document;
                //ParkingIupdater _parkingIupdater = new ParkingIupdater(commandData.Application.ActiveAddInId);
                //UpdaterRegistry.RegisterUpdater(_parkingIupdater, doc, true);
                //ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Lines);
                //UpdaterRegistry.AddTrigger(_parkingIupdater.GetUpdaterId(), doc, elementCategoryFilter, Element.GetChangeTypeAny());
                #endregion
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
