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

namespace BSMT_PpLayout
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

            TransactionGroup transGroupParkingPlace = new TransactionGroup(uiapp.ActiveUIDocument.Document);//开启事务组
            bool wheTransGroupRollBacek = true;

            RequestMethod requestMethod;

            try
            {
                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.SelBsmtExWall:
                        {
                            transGroupParkingPlace.Start("多选地库外墙轮廓线");
                            SelectElement selectElement = new SelectElement(uiapp);
                            selectElement.SelBasementExteriorWallOutLine();//该处暂定

                            GC.Collect();
                            break;
                        }
                    case RequestId.RestBsmtWallEles:
                        {
                            transGroupParkingPlace.Start("重置数据");
                            SelectElement selectElement = new SelectElement(uiapp);
                            selectElement.RestBasementWallElements();//该处暂定

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.SelColumsFs:
                        {
                            transGroupParkingPlace.Start("多点选柱");
                            SelectElement selectElement = new SelectElement(uiapp);
                            selectElement.SelColumsFs();//该处暂定

                            transGroupParkingPlace.RollBack();
                            wheTransGroupRollBacek = false;
                            GC.Collect();
                            break;
                        }
                    case RequestId.SelPSByLine:
                        {
                            transGroupParkingPlace.Start("多点选车");
                            SelectElement selectElement = new SelectElement(uiapp);
                            selectElement.SelPSByLine();

                            //transGroupParkingPlace.RollBack();
                            //wheTransGroupRollBacek = false;
                            GC.Collect();
                            break;
                        }

                    case RequestId.GlobalAroundBasedExistRoadSystem:
                        {
                            transGroupParkingPlace.Start("基于现有道路体系进行全局兜圈");

                            DesignRefresh designRefresh = new DesignRefresh(uiapp);
                            designRefresh.OperationMode = OperationMode.GlobalCircle;
                            designRefresh.Execute();//该处暂定

                            DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                            dataRefresh.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            //ShowDesignLocation showDesignLocation = new ShowDesignLocation(uiapp);
                            //showDesignLocation.Execute();//该处暂定

                            GC.Collect();
                            break;
                        }
                    case RequestId.AdjustStartPoint:// 调整柱子起点
                        {
                            transGroupParkingPlace.Start("指定子区域，计算当前子区域最优车道排布，并调整停车位起点");

                            DesignRefresh designRefresh = new DesignRefresh(uiapp);
                            designRefresh.OperationMode = OperationMode.Part_Genernal;
                            designRefresh.Execute();

                            DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                            dataRefresh.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    //case RequestId.FourRulesNorSou:
                    //    {
                    //        transGroupParkingPlace.Start("基于南北4种道路规则生成体系进行兜圈");

                    //        DesignRefresh designRefresh = new DesignRefresh(uiapp);
                    //        //designRefresh.OperationMode = OperationMode.PathFinding;
                    //        designRefresh.OperationMode = OperationMode.ObstacleCuttingding;
                    //        designRefresh.Execute();//该处暂定

                    //        DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                    //        dataRefresh.Execute();

                    //        GC.Collect();
                    //        break;
                    //    }
                    case RequestId.Road2Growth:
                        {
                            transGroupParkingPlace.Start("道路生长");

                            DesignRefresh designRefresh = new DesignRefresh(uiapp);
                            //designRefresh.OperationMode = OperationMode.PathFinding;
                            designRefresh.OperationMode = OperationMode.Road2Growth;
                            designRefresh.Execute();//该处暂定

                            DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                            dataRefresh.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.SubAreaLaneGeneration:
                        {
                            transGroupParkingPlace.Start("指定子区域，计算当前子区域最优车道排布");

                            DesignRefresh designRefresh = new DesignRefresh(uiapp);
                            designRefresh.OperationMode = OperationMode.Part_Road2Growth;
                            designRefresh.Execute();

                            DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                            dataRefresh.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.AutoPathfinding:
                        {
                            transGroupParkingPlace.Start("自动");

                            DesignRefresh designRefresh = new DesignRefresh(uiapp);
                            //designRefresh.OperationMode = OperationMode.PathFinding;
                            designRefresh.OperationMode = OperationMode.AutoPathfinding;
                            designRefresh.Execute();//该处暂定

                            DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                            dataRefresh.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    //case RequestId.ChangeDirectionByRectangle:
                    //    {
                    //        transGroupParkingPlace.Start("车位布局 指定矩形区域 水平/垂直切换");

                    //        ChangeDirection changeDirection = new ChangeDirection(uiapp);
                    //        ElementId selEleId = changeDirection.ChangeDirectionByRectangle();//该处暂定

                    //        DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                    //        dataRefresh.Execute(selEleId);

                    //        // 1、记录面板数据
                    //        RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                    //        _recordPanelData.Save();

                    //        GC.Collect();
                    //        break;
                    //    }

                    case RequestId.RefreshDataStatistics:
                        {
                            transGroupParkingPlace.Start("地库指标刷新，并对比目标指标，以及检查碰撞");

                            requestMethod = new DataRefresh(uiapp);// 数据输出
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.CheckCollisions:
                        {
                            transGroupParkingPlace.Start("");

                            requestMethod = new CheckCollisions(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.PointPillar:
                        {
                            transGroupParkingPlace.Start("点柱子");
                            requestMethod = new PointColumn(uiapp);
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.LookForEndPS:
                        {
                            transGroupParkingPlace.Start("寻找尽端停车位");
                            requestMethod = new LookForEndPS(uiapp);
                            requestMethod.Execute();

                            requestMethod = new DataRefresh(uiapp);// 数据输出
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    //case RequestId.TowerDistanceCheck:
                    //    {
                    //        transGroupParkingPlace.Start("塔楼距离检查");
                    //        requestMethod = new TowerDistanceCheck(uiapp);
                    //        requestMethod.Execute();

                    //        // 1、记录面板数据
                    //        RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                    //        _recordPanelData.Save();

                    //        GC.Collect();
                    //        break;
                    //    }
                    //case RequestId.BatchBreakLine:
                    //    {
                    //        transGroupParkingPlace.Start("批量打断车道中心线");
                    //        requestMethod = new BatchBreakLine(uiapp);
                    //        requestMethod.Execute();
                    //        GC.Collect();
                    //        break;
                    //    }
                    //case RequestId.Backtrack:
                    //    {
                    //        transGroupParkingPlace.Start("车位布局 兜圈");

                    //        ChangeDirection changeDirection = new ChangeDirection(uiapp);
                    //        ElementId selEleId = changeDirection.Backtrack();//该处暂定

                    //        DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                    //        dataRefresh.Execute(selEleId);

                    //        GC.Collect();
                    //        break;
                    //    }
                    case RequestId.LineArray:
                        {
                            transGroupParkingPlace.Start("车位布局 划线阵列");

                            LineArray lineArray = new LineArray(uiapp);
                            lineArray.Execute();//该处暂定

                            //DataRefresh dataRefresh = new DataRefresh(uiapp);// 数据输出
                            //dataRefresh.Execute(selEleId);

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.ArcArray:
                        {
                            transGroupParkingPlace.Start("车位布局 弧线阵列");

                            ArcArray arcArray = new ArcArray(uiapp);
                            arcArray.Execute();//该处暂定


                            GC.Collect();
                            break;
                        }
                    case RequestId.DimensionMarking:
                        {
                            transGroupParkingPlace.Start("尺寸标注");

                            requestMethod = new DimensionMarking(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.GridMarking:
                        {
                            transGroupParkingPlace.Start("轴网标注");

                            requestMethod = new GridMarking(uiapp);
                            requestMethod.Execute();
                            GC.Collect();
                            break;
                        }
                    case RequestId.ShowDesignLocation:
                        {
                            transGroupParkingPlace.Start("显示方案地库范围所在位置");

                            requestMethod = new ShowDesignLocation(uiapp);
                            requestMethod.Execute();//该处暂定

                            GC.Collect();
                            break;
                        }
                    case RequestId.AddSequenceNum:
                        {
                            transGroupParkingPlace.Start("数车位");
                            requestMethod = new AddSequenceNum(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.RoadIntersecIconChamfer:
                        {
                            transGroupParkingPlace.Start("道路相交处倒角图示");
                            requestMethod = new ChamferRoads(uiapp);
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.UpdatedPsTypes:// 多态
                        {
                            transGroupParkingPlace.Start("更新车位类型");
                            requestMethod = new UpdatedPsTypes(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.RampGenerated:
                        {
                            transGroupParkingPlace.Start("坡道生成");

                            requestMethod = new RampGenerated(uiapp);
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.DivideStorageRoom:
                        {
                            transGroupParkingPlace.Start("房间划分");

                            requestMethod = new SplitStorageRoom(uiapp);
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.FRoffset:
                        {
                            transGroupParkingPlace.Start("详图区域偏移");

                            requestMethod = new FRoffset(uiapp);
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.DrawBsmtWallLine:// 出图表达，增加一个环状黑色填充
                        {
                            transGroupParkingPlace.Start("绘制地库墙线");

                            requestMethod = new DrawBsmtWallLine(uiapp);
                            requestMethod.Execute();

                            // 1、记录面板数据
                            RecordPanelData _recordPanelData = new RecordPanelData(uiapp);
                            _recordPanelData.Save();

                            GC.Collect();
                            break;
                        }
                    case RequestId.ReadPanelData:
                        {
                            transGroupParkingPlace.Start("读取记录的面板数据");

                            RecordPanelData recordPanelData = new RecordPanelData(uiapp);
                            recordPanelData.Write();

                            GC.Collect();
                            break;
                        }
                    case RequestId.UpdateDesignList:
                        {
                            transGroupParkingPlace.Start("读取记录的面板数据");

                            InitialCMD initiaCMD = new InitialCMD(uiapp);
                            initiaCMD.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.ExtractCad:
                        {
                            transGroupParkingPlace.Start("提取Cad");

                            ExtractCad extractCad = new ExtractCad(uiapp);
                            extractCad.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.BrushLayer:
                        {
                            transGroupParkingPlace.Start("-");

                            BrushLayer brushLayer = new BrushLayer(uiapp);
                            brushLayer.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.LineCheck:
                        {
                            transGroupParkingPlace.Start("-");

                            LineCheck lineCheck = new LineCheck(uiapp);
                            lineCheck.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Temp01:
                        {
                            transGroupParkingPlace.Start("Temp");

                            requestMethod = new Temp01(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Temp02:
                        {
                            transGroupParkingPlace.Start("Temp");

                            requestMethod = new Temp02(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Temp03:
                        {
                            transGroupParkingPlace.Start("Temp");

                            requestMethod = new Temp03(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Temp04:
                        {
                            transGroupParkingPlace.Start("Temp");

                            requestMethod = new Temp04(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Temp05:
                        {
                            transGroupParkingPlace.Start("Temp");

                            requestMethod = new Temp05(uiapp);
                            requestMethod.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Temp06:
                        {
                            transGroupParkingPlace.Start("Temp");

                            requestMethod = new Temp06(uiapp);
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
                    transGroupParkingPlace.Assimilate();
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                MainWindow.Instance.WakeUp();
                MainWindow.Instance.Activate();

                if (!(ex is Autodesk.Revit.Exceptions.OperationCanceledException))//用户取消异常，不抛出异常信息
                {
                    TaskDialog.Show("error", ex.Message);
                }
                //Wpf_cursorPrompt.Instance.timer.Stop();
                //Wpf_cursorPrompt.Instance.Hide();

                GeometryDrawServersMgr.ClearAllServers();
                transGroupParkingPlace.RollBack();
            }
            finally
            {
                GC.Collect();
                MainWindow.Instance.WakeUp();
                MainWindow.Instance.Activate();

                #region 将Revit窗口设置为前台窗口，鼠标焦点可即时捕捉
                SetActiveWindow(uiapp.MainWindowHandle);
                #endregion
            }
        }//execute

    }  // public class RequestHandler : IExternalEventHandler
} // namespace
