﻿/*******************************************************************************
*                                                                              *
* 1、数据读取                                                                  *
*                                                                              *
* 视图层级 => 删除位于地库边界外的元素 => 基于Boundingbox过滤元素，会造成部分  *
* 元素与地库边界产生暧昧关系 => 将图元分区到各个地库边界围合区域内；           *
*                                                                              *
* 地库边界围合区域层级 => 删除与地库边界暧昧不清的图元 => 对图元进一步分类；   *
*                                                                              *
* 2、区域划分                                                                  *
*                                                                              *
* 基于道路空间将地库停车空间，划分为各个子区域；                               *
* 将该地库范围的不可删除图元，分区到各个子区域中；                             *
* 针对各个子区域提取元素的几何信息，并赋予对应的属性；                         *
*                                                                              *
* 3、停车位排布计算                                                            *
*                                                                              *
*                                                                              *
*                                                                              *
*                                                                              *
*******************************************************************************/

using System;
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
using System.IO;

namespace BSMT_PpLayout
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        internal static Document Doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region 与revit文档交互入口
            UIApplication uiapp = commandData.Application;
            #endregion

            // tmep
            Doc = uiapp.ActiveUIDocument.Document;

            try
            {
                //Assembly currentAssembly = Assembly.GetExecutingAssembly();
                //string dllPath = currentAssembly.Location;

                //string directoryPath = Path.GetDirectoryName(dllPath);
                //string assemblyPaht = directoryPath + @"\Teigha_Net64\TD_Mgd.dll";//引用位置
                //assemblyPaht = @"W:\BIM_ARCH\03.插件\goa tools 精简版\Content\goa_tools_arch_min\Teigha_Net64\TD_Mgd.dll";

                //Assembly a = Assembly.UnsafeLoadFrom(assemblyPaht);
                //MessageBox.Show(assemblyPaht);

                #region 调出操作窗口

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

                // 写入通讯系统
                CommunicationSystem communicationSystem = new CommunicationSystem();


                TransactionGroup transactionGroup = new TransactionGroup(Doc);
                transactionGroup.Start("地库功能初始化");

                // entity相关
                // initialize schema and ds 初始化创建 DataStorage，并在其中SetEntity
                AutoGeneratedElementMgr.Initialize(uiapp.ActiveUIDocument.Document);

                // DirectContext3D 相关
                GeometryDrawServersMgr.Init(uiapp);

                // 地库相关
                InitialCMD initiaCMD = new InitialCMD(uiapp);
                initiaCMD.Execute();
                initiaCMD.ReLoadFStype();

                // 初始化类型名字列表
                RequestMethod requestMethod = new UpdatedPsTypes(uiapp);
                requestMethod.Execute();

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
