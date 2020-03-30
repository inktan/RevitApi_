using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.UI;

namespace ModelessFormExternalEvent
{
    /// <summary>
    /// The class of our modeless dialog.
    /// </summary>
    /// <remarks>
    /// Besides other methods, it has one method per each command button.
    /// In each of those methods nothing else is done but raising an external
    /// event with a specific request set in the request handler.
    /// </remarks>
    /// 
    public partial class ModelessForm : Form
    {
        // In this sample, the dialog owns the handler and the event objects,
        // but it is not a requirement. They may as well be static properties
        // of the application.

        private RequestHandler m_Handler;//声明字段/变量-请求处理器-外部事件响应实例
        private ExternalEvent m_ExEvent;//声明字段/变量-外部事件


        /// <summary>
        ///   Dialog instantiation//对话实例化
        /// </summary>
        /// 
        public ModelessForm(ExternalEvent exEvent, RequestHandler handler)//构造函数
        {
            InitializeComponent();//初始化组件
            m_Handler = handler;//初始化字段/变量
            m_ExEvent = exEvent;//初始化字段/变量
        }

        /// <summary>
        /// Form closed event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)//重写窗体关闭事件
        {
            // we own both the event and the handler
            // we should dispose it before we are closed
            m_ExEvent.Dispose();//释放所占内存空间
            m_ExEvent = null;//重置外部事件为空
            m_Handler = null;//重置请求处理器，访问父类的非私有成员

            // do not forget to call the base class
            base.OnFormClosed(e);//base关键词
        }


        /// <summary>
        ///   Control enabler / disabler 
        /// </summary>
        ///
        private void EnableCommands(bool status)//控制全局控件的开关
        {
            foreach (Control ctrl in this.Controls)//this关键词表示当前对象
            {
                ctrl.Enabled = status;
            }
            if (!status)
            {
                this.btnExit.Enabled = true;//可控制关闭除btnExit之外的所有空间
            }
        }


        /// <summary>
        ///   A private helper method to make a request
        ///   and put the dialog to sleep at the same time.
        /// </summary>
        /// <remarks>
        ///   It is expected that the process which executes the request 
        ///   (the Idling helper in this particular case) will also
        ///   wake the dialog up after finishing the execution.
        /// </remarks>
        ///
        private void MakeRequest(RequestId request)//触发按钮调用的方法。不同的按钮对应不同的枚举值
        {
            m_Handler.Request.Make(request);//外部事件内部函数调用
            m_ExEvent.Raise();//启动外部事件
            DozeOff();
        }


        /// <summary>
        ///   DozeOff -> disable all controls (but the Exit button)
        /// </summary>
        /// 
        private void DozeOff()//可关闭所有控件
        {
            EnableCommands(false);
        }


        /// <summary>
        ///   WakeUp -> enable all controls
        /// </summary>
        /// 
        public void WakeUp()//可开启所有控件
        {
            EnableCommands(true);
        }

        private void BtnFlipLeftRight_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.FlipLeftRight);
        }

        private void BtnFlipUpDown_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.FlipInOut);
        }

        private void BtnFlipLeft_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.MakeLeft);//枚举值需要手工设置对应关系

        }

        private void BtnFlipUp_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.TurnOut);
        }

        private void BtnFlipRight_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.MakeRight);
        }

        private void BtnFlipDown_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.TurnIn);
        }

        private void BtnRotate_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Rotate);
        }

        private void BtnDeleted_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Delete);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
