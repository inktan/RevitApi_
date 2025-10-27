using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using goa.Common;

namespace ThreeDViewNavi
{
    internal static class KeyBoardMouseMonitor
    {
        internal static HashSet<Keys> KeysHoldingDown = new HashSet<Keys>();
        internal static bool mouseDown = false;
        internal static int cursorX, cursorY;

        private static IKeyboardMouseEvents m_hook;
        private static Exception previousError;
        private static string instruction 
        { 
            get 
            {
                string s = Navi.Fly ? "【飞行】" : "【步行】";
                return s + "按ESC退出\r\n左键拖拽：环视\r\nWASDQE:移动视点\r\nR:倒回 G:落地\r\nSHIFT:快速 CTRL:超快\r\nSPACE:飞行/步行";
            }
        }

        private static HashSet<Keys> overrideKeys = new HashSet<Keys>()
        { 
            Keys.W, Keys.A, Keys.S, Keys.D, Keys.Q, Keys.E, 
            Keys.R, Keys.G,
            Keys.Up, Keys.Down, Keys.Left, Keys.Right, 
            Keys.ShiftKey, Keys.ControlKey, Keys.Space,
        };

        internal static bool RevitEventsRegistered = false;
        /// <summary>
        /// need to call inside API context.
        /// </summary>
        /// <param name="_register"></param>
        internal static void RegisterRevitEvents(bool _register)
        {
            if (RevitEventsRegistered == _register)
                return;

            //revit ui event
            if (_register)
            {
                var uiapp = goa.Common.APP.UIApp;
                uiapp.ApplicationClosing += onRevitUIEvent;
                uiapp.DialogBoxShowing += onRevitUIEvent;
                uiapp.DisplayingOptionsDialog += onRevitUIEvent;
            }
            else
            {
                var uiapp = goa.Common.APP.UIApp;
                uiapp.ApplicationClosing -= onRevitUIEvent;
                uiapp.DialogBoxShowing -= onRevitUIEvent;
                uiapp.DisplayingOptionsDialog -= onRevitUIEvent;
            }

            RevitEventsRegistered = _register;
        }

        internal static bool SystemEventsRegistered = false;
        internal static void RegisterSystemEvents(bool _register)
        {
            if (SystemEventsRegistered == _register)
                return;

            if (_register)
            {
                m_hook = Hook.AppEvents();

                m_hook.KeyDown += onKeyDown;
                m_hook.KeyUp += onKeyUp;
                m_hook.MouseDownExt += onMouseDown;
                m_hook.MouseUpExt += onMouseUp;
                m_hook.MouseMoveExt += onMouseMove;
                //m_hook.MouseDragStartedExt += onMouseDragStarted;
                //m_hook.MouseDragFinishedExt += onMouseDragFinished;

                Form_cursorPrompt.Start(instruction, goa.Common.APP.RevitWindow);
            }
            else
            {
                m_hook.KeyDown -= onKeyDown;
                m_hook.KeyUp -= onKeyUp;
                m_hook.MouseDownExt -= onMouseDown;
                m_hook.MouseUpExt -= onMouseUp;
                m_hook.MouseMoveExt -= onMouseMove;
                //m_hook.MouseDragStartedExt -= onMouseDragStarted;
                //m_hook.MouseDragFinishedExt -= onMouseDragFinished;

                m_hook.Dispose();

                Form_cursorPrompt.Stop();
            }

            SystemEventsRegistered = _register;
        }

        static void onRevitUIEvent(object sender, EventArgs e)
        {
            Navi.Stop();
        }

        static void onKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //exit
                if (e.KeyCode == Keys.Escape)
                {
                    Navi.Stop();
                    return;
                }
                if(e.KeyCode == Keys.Space)
                {
                    Navi.Fly = !Navi.Fly;
                    Form_cursorPrompt.Start(instruction, goa.Common.APP.RevitWindow);
                    return;
                }
                //other keys
                if (overrideKeys.Contains(e.KeyCode) == false)
                    return;

                KeysHoldingDown.Add(e.KeyCode);
                showStatusText();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        static void onKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (overrideKeys.Contains(e.KeyCode) == false)
                    return;

                KeysHoldingDown.Remove(e.KeyCode);
                showStatusText();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        static void onMouseDown(object sender, MouseEventExtArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left
                    || APP.MainWindow.CursorIsInside()
                    || APP.MainWindow.CursorIsInside())
                {
                    return;
                }
                else
                {
                    mouseDown = true;
                    e.Handled = true;
                    showStatusText();
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        static void onMouseUp(object sender, MouseEventExtArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left
                    || APP.MainWindow.CursorIsInside()
                    || APP.MainWindow.CursorIsInside())
                {
                    return;
                }
                else
                {
                    mouseDown = false;
                    e.Handled = true;
                    showStatusText();
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }
        static void onMouseMove(object sender, MouseEventExtArgs e)
        {
            try
            {
                if (mouseDown == false)
                    return;
                cursorX = e.X;
                cursorY = e.Y;
                showStatusText();
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }
        /*
        static void onMouseDragStarted(object sender, MouseEventExtArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left
                    || APP.MainWindow.CursorIsInside()
                    || APP.MainWindow.CursorIsInside())
                {
                    return;
                }

                dragging = true;
                dragX = e.X;
                dragY = e.Y;
                //e.Handled = true;
                showStatusText();
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        static void onMouseDragFinished(object sender, MouseEventExtArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left
                    || APP.MainWindow.CursorIsInside()
                    || APP.MainWindow.CursorIsInside())
                {
                    return;
                }

                dragging = false;
                //e.Handled = true;
                showStatusText();
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }
        */
        static string getStatus()
        {
            string keys = KeysHoldingDown.Select(x => x.ToString()).ToStringLines();
            string drag = "drag: " + mouseDown.ToString() + "\r\n";
            if (mouseDown)
                drag += cursorX.ToString() + ", " + cursorY.ToString();
            return drag + "\r\n" + keys + "\r\ntimer interval: " + Navi.timerInterval.ToString() + "ms";
        }

        static void showStatusText()
        {
            if (APP.MainWindow.IsAvailable())
            {
                var s = getStatus();
                APP.MainWindow.ShowKeyText(s);
            }
        }

        static void handleError(Exception ex)
        {
            RegisterSystemEvents(false);
            RequestMaker.makeRequest(RequestId.close);
            previousError = ex;
            UserMessages.ShowErrorMessage(ex, null);
        }

    }
}
