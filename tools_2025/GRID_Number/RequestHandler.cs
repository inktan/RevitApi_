using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using PubFuncWt;

using goa.Common;

namespace GRID_Number
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();

        [System.Runtime.InteropServices.DllImport(@"C:\Windows\System32\user32.dll")]// 可指定文件全路径，也可以指定文件名，代码会自动按照一定寻找顺序进行搜索
        internal static extern IntPtr SetActiveWindow(IntPtr hwnd);// 激活与调用该函数的线程相关的顶层窗口

        public string GetName()
        {
            return "Overlapping Elements Clean Up Request Handler";
        }
        public void Execute(UIApplication uiapp)
        {
            #region 将Revit窗口设置为前台窗口，鼠标焦点可即时捕捉
            SetActiveWindow(uiapp.MainWindowHandle);
            #endregion

            var window = CMD.MainWindow;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.GridNmae:
                        {
                            ChangeGridName changeGridName= new ChangeGridName(uiapp);//计算轴网数据
                            changeGridName.Execute();
                            break;
                        }
                    case RequestId.CheckOverlapGrids:
                        {
                            CheckOverlapGrids checkOverlapGrids = new CheckOverlapGrids(uiapp);
                            checkOverlapGrids.Execute();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("error", ex.Message);

                // UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                window.WakeUp();
                window.Activate();

                #region 将Revit窗口设置为前台窗口，鼠标焦点可即时捕捉
                SetActiveWindow(uiapp.MainWindowHandle);
                #endregion
            }
        }//execute

    }// public class RequestHandler : IExternalEventHandler

}  // namespace

