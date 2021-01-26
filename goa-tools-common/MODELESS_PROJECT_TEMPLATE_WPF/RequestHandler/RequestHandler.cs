using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace MODELESS_PROJECT_TEMPLATE_WPF
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
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            #region 将Revit窗口设置为前台窗口，鼠标焦点可即时捕捉

            IntPtr activeMainWindow = Autodesk.Windows.ComponentManager.ApplicationWindow;// 获得当前窗口句柄
            SetActiveWindow(activeMainWindow);
            #endregion

            TransactionGroup transGroupParkingPlace = new TransactionGroup(doc);//开启事务组

            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.SelDieectionShpaesByRectangle:
                        {
                            transGroupParkingPlace.Start("框选DirectionShapes");

                            SelDieectionShpaesByRectangle selDieectionShpaesByRectangle = new SelDieectionShpaesByRectangle(uiapp);
                            selDieectionShpaesByRectangle.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.SetSurfaceTransparency:
                        {
                            transGroupParkingPlace.Start("框选DirectionShapes");

                            SetSurfaceTransparency setSurfaceTransparency = new SetSurfaceTransparency(uiapp);
                            setSurfaceTransparency.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.CommentElementId:
                        {
                            transGroupParkingPlace.Start("解锁所有已锁定元素");

                            CommentElementId commentElementId = new CommentElementId(uiapp);
                            commentElementId.Execute();

                            GC.Collect();
                            break;
                        }
                    case RequestId.Test:
                        {
                            transGroupParkingPlace.Start("普通测试");

                            Test test = new Test(uiapp);
                            test.Execute();

                            GC.Collect();
                            break;
                        }
                    default:
                        {
                            GC.Collect();
                            break;
                        }
                }
                transGroupParkingPlace.Assimilate();
            }
            catch (Exception ex)
            {
                GC.Collect();
                TaskDialog.Show("error", ex.Message);
                transGroupParkingPlace.RollBack();
            }
            finally
            {
                GC.Collect();
                window.WakeUp();
                window.Activate();
            }
        }//execute

    }  // public class RequestHandler : IExternalEventHandler
} // namespace
