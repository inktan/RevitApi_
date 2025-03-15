﻿using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using goa.Common;
using goa.Revit.DirectContext3D;
using PubFuncWt;
using Autodesk.Revit.DB.ExtensibleStorage;
using System.Linq;
using System.Windows.Interop;
using System.Reflection;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;

namespace InfoStrucFormwork
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        internal static UIDocument UIDoc;
        internal static Document Doc;
        internal static Selection Sel;
        internal static Level UpLevel;
        internal static Level Level;
        internal static Level DownLevel;
        internal static AutoGeneratedElementMgr AutoGeneratedElementMgr;

        public static XYZ PositonMoveDis { get; internal set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region 与revit文档交互入口
            UIApplication uiapp = commandData.Application;
            #endregion

            // tmep
            UIDoc = uiapp.ActiveUIDocument;
            Doc = uiapp.ActiveUIDocument.Document;
            Sel = uiapp.ActiveUIDocument.Selection;

            try
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                string dllPath = currentAssembly.Location;

                string directoryPath = Path.GetDirectoryName(dllPath);

                #region 调出操作窗口
                string assemblyPaht = directoryPath + @"\Teigha_Net64\TD_Mgd.dll";//引用位置
                //assemblyPaht = @"Z:\G2024-0010\00-Workshop\BIM\STRU\goa tools 精简版\Content\goa_tools_arch_min\Teigha_Net64\TD_Mgd.dll";

                //MessageBox.Show(assemblyPaht);
                Assembly a = Assembly.UnsafeLoadFrom(assemblyPaht);

                //assemblyPaht = @"W:\BIM_ARCH\03.插件\goa tools 精简版\Content\goa_tools_arch_min\goa.Lib2021.dll";
                //a = Assembly.UnsafeLoadFrom(assemblyPaht);

                //byte[] assemblyBuffer = File.ReadAllBytes(assemblyPaht);
                //Assembly a = Assembly.Load(assemblyBuffer);

                //IntPtr intPtr = Autodesk.Windows.ComponentManager.ApplicationWindow;
                //WindowInteropHelper helper = new WindowInteropHelper(MainWindow.Instance);
                //helper.Owner = intPtr;

                // 调出操作窗口
                if (MainWindow.Instance.IsVisible == true)
                {
                    MainWindow.Instance.Activate();
                }
                else
                    MainWindow.Instance.Show();

                #endregion

                #region 初始化设置

                TransactionGroup transactionGroup = new TransactionGroup(Doc);
                transactionGroup.Start("结构信息化");

                // entity相关
                // initialize schema and ds 初始化创建 DataStorage，并在其中SetEntity
                //AutoGeneratedElementMgr.Initialize(uiapp.ActiveUIDocument.Document);
                //AutoGeneratedElementMgr = new AutoGeneratedElementMgr(Doc, CMD.layerNamesId);

                // 自身插件相关
                //CMD.LayerNames = AutoGeneratedElementMgr.ReadIds();
                //SetViewModelLayerNames();

                ViewModel.Instance.FloorTypeNames.Clear();

                List<string> floorTypeNames = (new FilteredElementCollector(CMD.Doc))
                             .OfCategory(BuiltInCategory.OST_Floors)
                             .OfClass(typeof(FloorType))
                             .Cast<FloorType>()
                             .Select(p => p.Name)
                             .ToList();

                ViewModel.Instance.FloorTypeNames.AddRange(floorTypeNames);
                MainWindow.Instance.FloorTypeSel.SelectedIndex = 0;

                // DirectContext3D 相关
                GeometryDrawServersMgr.Init(uiapp);

                transactionGroup.Assimilate();
                #endregion
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                //if (Wpf_cursorPrompt.Instance.IsVisible == true)
                //{
                //    Wpf_cursorPrompt.Instance.timer.Stop();
                //    Wpf_cursorPrompt.Instance.Hide();
                //}
                message = ex.Message;
                return Result.Failed;
            }
        }// excute

    }//class
}//namespace



