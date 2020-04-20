using System;
using System.Collections.Generic;
using Form_ = System.Windows.Forms;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;

using goa.Common;

namespace dll_wpf
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();

        public string GetName()
        {
            return "Overlapping Elements Clean Up Request Handler";
        }
        public void Execute(UIApplication uiapp)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())//Request.Take()数据提取 只能有一次
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.TestMethod_temp:
                        {
                            TestMethod_temp(uiapp);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                //UserMessages.ShowErrorMessage(ex, window);
                TaskDialog.Show("error", ex.Message);

            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //外部事件方法建立
        /// <summary>
        /// 新建户型_1T2H_aa_1DY 函数架构 文档开启 在前 文档关闭 在后
        /// </summary>
        /// <param name="uiapp"></param>
        public void TestMethod_temp(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            //创建事务t1
            Transaction t1 = new Transaction(doc, "创建");//创建事务
            t1.Start();
            XYZ startPoint = sel.PickPoint("请选择轴网标号的起点");
            XYZ endPoint = sel.PickPoint("请选择轴网标号的终点");
            //创建一道结构墙
            Wall wall = Wall.Create(doc, Line.CreateBound(startPoint, endPoint), Level.Create(doc, 0).Id, false);//false表示创建结构墙
            t1.Commit();

            IList<Element> walls = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Walls) .WhereElementIsNotElementType().ToElements();
            foreach(Element ele in walls)
            {
                CMD.TestList.Add(ele.Name);
            }
            TaskDialog.Show("Revit",walls.Count.ToString());

        }
        //以下为各种method---------------------------------分割线---------------------------------

    }  // public class RequestHandler : IExternalEventHandler
} // namespace

