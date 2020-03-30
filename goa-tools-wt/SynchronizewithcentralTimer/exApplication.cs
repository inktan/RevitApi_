using System;
using System.Windows.Forms;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;

namespace SynchronizewithcentralTimer
{
    public class exApplication : IExternalApplication
    {
        public static System.Timers.Timer aTimer = new System.Timers.Timer();//实例化Timer类
        public NeSync myCommand = null;//声明Revit外部事件包含命令
        public ExternalEvent hander = null;//声明Reivt外部事件

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);

            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                myCommand = new NeSync();//构建外部事件任务
                hander = ExternalEvent.Create(myCommand);//构建外部事件启动器
                //注册DocumentChanged事件
                application.ControlledApplication.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public void DocumentChangedForSomething(object sender, DocumentChangedEventArgs args)
        {
            Document doc = args.GetDocument();
            ModelPath modelPath = doc.GetWorksharingCentralModelPath();

            if (modelPath != null)//基于是否存在中心模型的文件位置，进行判断是否继续执行任务
            {
                aTimer.Interval = 10000;//设置间隔时间为10000毫秒
                aTimer.AutoReset = true;//如果只执行一次（false）如果一直执行(true),使用bool值进行首尾控制
                aTimer.Enabled = true; //timer可用 
                aTimer.Elapsed += new System.Timers.ElapsedEventHandler(NewSyncTimeSpan);//到达时间的时候执行指定方法
            }
        }
        public void NewSyncTimeSpan(object sender, System.Timers.ElapsedEventArgs e)
        {
            hander.Raise();//启动外部事件
            aTimer.Stop();//停止timer
        }
    }

    //创建外部事件
    #region
    public class NeSync : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            SyncWithRelinquishing(doc);
        }
        public string GetName()
        {
            return "Sync With Central Without Relinquishing";
        }

        //与中心模型同步操作代码展示
        #region
        public void SyncWithRelinquishing(Document doc)
        {
            TransactWithCentralOptions transOpts = new TransactWithCentralOptions();
            SynchLockCallback transCallBack = new SynchLockCallback();
            transOpts.SetLockCallback(transCallBack);

            SynchronizeWithCentralOptions syncOpts = new SynchronizeWithCentralOptions();
            RelinquishOptions relinquishOpts = new RelinquishOptions(true);
            syncOpts.SetRelinquishOptions(relinquishOpts);
            syncOpts.SaveLocalAfter = false;
            syncOpts.Comment = "User has already done something";

            try
            {
                doc.SynchronizeWithCentral(transOpts, syncOpts);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Synchronize Failed", e.Message);
            }
        }
        class SynchLockCallback : ICentralLockedCallback
        {
            public bool ShouldWaitForLockAvailability()
            {
                return false;
            }
        }
        #endregion
    }
    #endregion
}
