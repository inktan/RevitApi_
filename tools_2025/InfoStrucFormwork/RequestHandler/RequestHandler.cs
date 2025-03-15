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


namespace InfoStrucFormwork
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
                    case RequestId.ConcWall:
                        {
                            requestMethod = new ConcWall(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConcCol:
                        {
                            requestMethod = new ConcCol(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConcBeamUi:
                        {
                            requestMethod = new ConcBeamUi(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConcBeamFollowFaceUi:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("多梁基于面创建");
                            requestMethod = new ConcBeamFollowFaceUi(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConcSingleBeamFollowFaceUi:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("梁吸附到面");
                            requestMethod = new ConcSingleBeamFollowFaceUi(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ConcColFollowFaceUi:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("梁吸附到面");
                            requestMethod = new ConcSingleColFollowFaceUi(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.FloorDivision:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("结构板网格化");

                            requestMethod = new FloorDivision(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.AlignToBoardBottom:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("梁顶对齐到板顶");

                            requestMethod = new AlignToBoardBottom(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.AlignToBoardTop:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("梁底对齐到板底");

                            requestMethod = new AlignToBoardTop(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.CalRelativeH:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("计算相对高度");

                            requestMethod = new CalRelativeH(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.BbCoverFb:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("梁底是否兜住板底");

                            requestMethod = new BbCoverFb(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ClearStruAna:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("清楚结构分析");

                            requestMethod = new ClearStruAna(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.EleSeparate:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("梁柱板墙分离");

                            requestMethod = new EleSeparate(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.StoreySteelBeamDoubleLine:
                        {
                            ViewModel.Instance.ConsiderRoof = false;
                            requestMethod = new StoreySteelBeamDoubleLine(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.RoofSteelBeamDoubleLine:
                        {
                            ViewModel.Instance.ConsiderRoof = true;
                            requestMethod = new StoreySteelBeamDoubleLine(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.StoreySteelBeamSingleLine:
                        {
                            ViewModel.Instance.ConsiderRoof = false;
                            requestMethod = new StoreySteelBeamSingleLine(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.RoofStelBeamSingleLine:
                        {
                            ViewModel.Instance.ConsiderRoof = true;
                            requestMethod = new StoreySteelBeamSingleLine(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.SlopeRoofBeam:
                        {
                            TransactionGroup transactionGroup = new TransactionGroup(uiapp.ActiveUIDocument.Document);
                            transactionGroup.Start("创建坡屋面折梁");

                            requestMethod = new SlopeRoofBeam(uiapp);
                            requestMethod.Execute();
                            transactionGroup.Assimilate();
                            GC.Collect();
                            break;
                        }
                    case RequestId.StoreyFloor:
                        {
                            requestMethod = new StoreyFloor(uiapp);
                            requestMethod.Execute();
                            
                            ViewModel.Instance.makeRequest(RequestId.StoreyFloorAux);
                            //GC.Collect();
                            break;
                        }
                    //case RequestId.StoreyFloorAux:
                    //    {
                    //        StrucFloorAux strucFloorAux = new StrucFloorAux();
                    //        strucFloorAux.Execute();

                    //        GC.Collect();
                    //        break;
                    //    }
                    case RequestId.BandingWhole:
                        {
                            requestMethod = new BandingWhole(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.BandingCombination:
                        {
                            requestMethod = new BandingCombination(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.BandingSel:
                        {
                            requestMethod = new BandingSel(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.Test01:
                        {
                            requestMethod = new Test01(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.Test02:
                        {
                            requestMethod = new Test02(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.Test03:
                        {
                            requestMethod = new Test03(uiapp);
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
