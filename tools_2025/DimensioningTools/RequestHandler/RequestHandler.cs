using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.IFC;

using goa.Common;
using goa.Common.g3InterOp;
using ClipperLib;
using g3;
using Autodesk.Revit.DB.ExtensibleStorage;
using PubFuncWt;
using goa.Revit.DirectContext3D;


namespace DimensioningTools
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();

        //private ProgressWindow m_progressWindow = new ProgressWindow();
        //private ProgressTracker m_progressTracker { get { return this.m_progressWindow.Tracker; } }

        [System.Runtime.InteropServices.DllImport(@"C:\Windows\System32\user32.dll")]// 可指定文件全路径，也可以指定文件名，代码会自动按照一定寻找顺序进行搜索
        internal static extern IntPtr SetActiveWindow(IntPtr hwnd);// 激活与调用该函数的线程相关的顶层窗口

        [System.Runtime.InteropServices.DllImport(@"C:\Windows\System32\user32.dll")] 
        internal static extern bool SetForegroundWindow(IntPtr hwnd);

        public string GetName()
        {
            return "RequestHandler";
        }
        /// <summary>
        /// 可以理解为简单工厂模式
        /// </summary>
        /// <param name="uiapp"></param>
        public void Execute(UIApplication uiapp)
        {
            #region 将Revit窗口设置为前台窗口，鼠标焦点可即时捕捉
            SetActiveWindow(uiapp.MainWindowHandle);
            #endregion

            bool wheTransGroupRollBacek = true;

            RequestMethod requestMethod;

            try
            {
                MainWindow.Instance.DozeOff();

                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.ConvertCurrentView:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("基于cad绘制文字");

                            requestMethod = new ConvertCurrentView(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConvertMultiViews:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new ConvertMultiViews(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConvertSelected:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new ConvertSelected(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.FrameSelectFakeDims:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new FrameSelectFakeDims(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.DimClosestPointInView:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new DimClosestPointInView(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.DimAvoid:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new DimAvoid(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.FakeDimAvoid:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new FakeDimAvoid(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.WindowSpotElevDim:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new WindowSpotElevDim(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.PickElevationFamily:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new PickElevationFamily(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.AutoFillUp:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new AutoFillUp(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.FakeElev_Refresh_allviews:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new FakeElev_Refresh_allviews(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.FakeElev_Refresh_activeview:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new FakeElev_Refresh_activeview(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.DimOnFloors:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new DimOnFloors(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.AbsElev:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("替换房间名");

                            requestMethod = new AbsElev(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }

                    case RequestId.Test:
                        {
                            requestMethod = new Test(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    default:
                        {
                            GC.Collect();
                            break;
                        }
                }
                if (wheTransGroupRollBacek)
                {
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                MainWindow.Instance.WakeUp();
                //MainWindow.Instance.Activate();

                if (!(ex is Autodesk.Revit.Exceptions.OperationCanceledException))//用户取消异常，不抛出异常信息
                {
                    TaskDialog.Show("error", ex.Message);
                }
                //Wpf_cursorPrompt.Instance.timer.Stop();
                //Wpf_cursorPrompt.Instance.Hide();

                GeometryDrawServersMgr.ClearAllServers();
            }
            finally
            {
                GC.Collect();
                MainWindow.Instance.WakeUp();
                //MainWindow.Instance.Activate();
            }
        }//execute

    }  // public class RequestHandler : IExternalEventHandler
} // namespace
