
/*******************************************************************************
*                                                                              *
* 全局刷新                                                                    *
*                                                                                *
*                                                                              *
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;
using System.Threading;

namespace BSMT_PpLayout
{
    internal enum OperationMode : int
    {
        /// <summary>
        /// 全局使用兜圈处理-基于原有道路兜圈处理
        /// </summary>
        GlobalCircle,
        Part_Genernal,
        /// <summary>
        /// 基于障碍物对子区域进行分割
        /// </summary>
        //ObstacleCuttingding,
        /// <summary>
        /// 以子区域为目标范围，基于周围道路进行道路空间生长
        /// </summary>
        Road2Growth,
        Part_Road2Growth,// 局部寻路
        AutoPathfinding,// 自动寻路

    }

    class DesignRefresh : RequestMethod
    {
        [System.Runtime.InteropServices.DllImport(@"C:\Windows\System32\user32.dll")]// 可指定文件全路径，也可以指定文件名，代码会自动按照一定寻找顺序进行搜索
        internal static extern IntPtr SetActiveWindow(IntPtr hwnd);// 激活与调用该函数的线程相关的顶层窗口

        public DesignRefresh(UIApplication uiApp) : base(uiApp) { }
        internal OperationMode OperationMode;
        internal override void Execute()
        {
            // 判断异常
            this.JudgeWheInputDataIsWrong();

            InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
            List<ElementId> selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
            List<ElemsViewLevel> elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级

            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                View nowView = elemsViewLevel.View;
                elemsViewLevel.DelUnUsefulEles();// 清除与地库边界暧昧不清的图元

                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;

                    if (this.OperationMode == OperationMode.GlobalCircle || this.OperationMode == OperationMode.Road2Growth || this.OperationMode == OperationMode.AutoPathfinding)
                    {
                        bsmt.DelUnUsefulParkSpaces();
                        bsmt.DelUnUsefulPillar();
                    }
                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();
                    bsmt.Computer_VeRa();

                    BsmtClipByRoad bsmtClipByRoad = new BsmtClipByRoad(bsmt);

                    List<PPLocPoint> psLocPoints = new List<PPLocPoint>();// 停车位放置位置 停车位族类型 放置角度

                    Refresh refresh = null;// 使用多态
                    GlobalData.Instance.PartAnchorPoint = new Vector2d();// 局部功能开启的一把钥匙

                    // 基于当前设计条件进行车位排布刷新，不涉及任何智能助理 —— 条件限定，对于最小停车单元均使用兜圈算法
                    if (this.OperationMode == OperationMode.GlobalCircle)
                    {
                        GlobalData.Instance.SelPointToAdjustStartPoint = new Vector2d();
                        refresh = new GlobalCircle(bsmtClipByRoad, this.doc);
                    }
                    else if (this.OperationMode == OperationMode.Part_Genernal)// 调整一排停车位的起点
                    {
                        GlobalData.Instance.SelPointToAdjustStartPoint = this.sel.PickPoint("请在目标区域内，任选一点").ToVector2d();
                        refresh = new GlobalCircle(bsmtClipByRoad, this.doc);
                    }
                    // 基于障碍物所在横向矩形空间的基准点生成水平向道路线，直接与已有道路相连，然后做全局兜圈计算
                    else if (this.OperationMode == OperationMode.Road2Growth)
                    {
                        GlobalData.Instance.SelPointToAdjustStartPoint = new Vector2d();
                        refresh = new Road2Growth(bsmtClipByRoad, this.doc);
                    }
                    else if (this.OperationMode == OperationMode.Part_Road2Growth)
                    {
                        //string m = "请在目标区域内，任选一点";
                        //Wpf_cursorPrompt.Start(m, MainWindow.Instance);
                        //SetActiveWindow(this.uiApp.MainWindowHandle);
                        //GlobalData.PartAnchorPoint = this.sel.PickPoint().ToVector2d();
                        //Wpf_cursorPrompt.Stop();

                        GlobalData.Instance.PartAnchorPoint = this.sel.PickPoint("请在目标区域内，任选一点").ToVector2d();
                        refresh = new Road2Growth(bsmtClipByRoad, this.doc);
                    }
                    else if (this.OperationMode == OperationMode.AutoPathfinding)
                    {
                        GlobalData.Instance.SelPointToAdjustStartPoint = new Vector2d();
                        refresh = new AutoPathfinding(bsmtClipByRoad, this.doc);
                    }

                    this.sw.Restart();
                    if (refresh != null)
                    {
                        //continue;

                         refresh.Execute();
                        //(this.sw.ElapsedMilliseconds / 1000.0).ToString().TaskDialogErrorMessage();
                        // 开启停车事务
                        (new PPLocPoint()).PointProjecToRevit(refresh.PpLocPoints, doc, nowView);
                        // 开启绘制地库中心线事务
                        (new PPLocPoint()).CreatingRoads(refresh.Roads, doc, nowView);
                        int i = 0;
                        //foreach (var item in refresh.PpLocPoints)
                        //{
                        //    i++;
                        //    if (i > 100)
                        //    {
                        //        break;
                        //    }
                        //    for (int j = 0; j < 1; j++)
                        //    {
                        //        List<XYZ> _xYZs = new List<XYZ>() { item.Vector2d.ToXYZ(), item.Vector2d.ToXYZ() + new XYZ(10, 10, 0) };
                        //        IEnumerable<Line> _lines = _xYZs.ToLines();
                        //        CMD.Doc.CreateDirectShapeWithNewTransaction(_lines, CMD.Doc.ActiveView);
                        //    }
                        //}

                        // 下一步优化方向
                        // 新的路网生成后，需要判断子区域是否为兜圈最大值
                        // 遇到问题，环状区域被一刀划两半，中间部位会出现两个缺口

                    }
                    this.sw.Stop();
                }
            }

            // 更新地库子区域
            // 需要重新抓取数据
            elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级
            foreach (ElemsViewLevel elemsViewLevel in elemsViewLevels)// 遍历单个视图
            {
                foreach (Bsmt bsmt in elemsViewLevel.Bsmts)// 遍历单个地库
                {
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))// 判断是否UI选择
                        continue;

                    // 更新地库子区域
                    bsmt.DelUnUsefulSubPsAreaExits();
                    bsmt.Computer_VeRa();
                    BsmtClipByRoad bsmtClipByRoad = new BsmtClipByRoad(bsmt);
                    bsmtClipByRoad.CreatFilledRegion();
                }
            }
        }
    }
}
