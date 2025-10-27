using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

using goa.Common;
using PubFuncWt;

namespace FakeElev_Refresh
{
    public class MainWindowRequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName()
        {
            return "fake elev update Request Handler";
        }
        public void Execute(UIApplication uiapp)
        {
            var window = APP.MainWindow;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.FakeElev_Refresh_activeview:
                        {
                            FakeElev_Refresh_activeview(uiapp);
                            break;
                        }
                    case RequestId.FakeElev_Refresh_allviews:
                        {
                            FakeElev_Refresh_allviews(uiapp);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {

            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                AssemblyLoader.Register(false);
                Form_cursorPrompt.Stop();
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //外部事件方法建立
        //当前视图假标高刷新
        public void FakeElev_Refresh_activeview(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            ElementId activeviewId = doc.ActiveView.Id;
            //获取视图ID列表
            ICollection<ElementId> ele_viewsectionIds = (new FilteredElementCollector(doc)).OfClass(typeof(ViewSection)).WhereElementIsNotElementType().ToElementIds();
            if (ele_viewsectionIds.Contains(activeviewId))
            {
                LevelPickFilter levelpickfilter = new LevelPickFilter();
                Form_cursorPrompt.Start("请选择零零标高", APP.MainWindow);
                Level level = doc.GetElement(sel.PickObject(ObjectType.Element, levelpickfilter)) as Level;
                double selevelelevation = level.Elevation;//请选择零零标高

                using (Transaction modifyParaElevation = new Transaction(doc))
                {
                    modifyParaElevation.Start("假标高计算");
                    //遍历视图列表
                    modifyElevationFake(doc, doc.ActiveView.Id, selevelelevation);//对单个视图id内的 假标高 族实例 进行数据修改
                    modifyParaElevation.Commit();
                }
            }
            else
            {
                TaskDialog.Show("Revit2020","请选择 立面 或者 剖面 视图");
            }
        }
        //所有立面、剖面假标高刷新
        public void FakeElev_Refresh_allviews(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            LevelPickFilter levelpickfilter = new LevelPickFilter();
            Form_cursorPrompt.Start("请选择零零标高", APP.MainWindow);
            Level level = doc.GetElement(sel.PickObject(ObjectType.Element, levelpickfilter)) as Level;
            double selevelelevation = level.Elevation;//请选择零零标高

            //获取视图ID列表
            ICollection<ElementId> ele_viewsectionIds = (new FilteredElementCollector(doc)).OfClass(typeof(ViewSection)).WhereElementIsNotElementType().ToElementIds();
            using (Transaction modifyParaElevation = new Transaction(doc))
            {
                modifyParaElevation.Start("modifyParaElevation");
                //遍历视图列表
                foreach (ElementId elementId in ele_viewsectionIds)
                {
                    modifyElevationFake(doc, elementId, selevelelevation);//对单个视图id内的 假标高 族实例 进行数据修改
                }
                modifyParaElevation.Commit();
            }

        }
        //以下为各种method----------------------------------------

        public class LevelPickFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Levels));
            }
            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
            }
        }
        //对单个视图id内的 假标高 族实例 进行数据修改
        public void modifyElevationFake(Document doc, ElementId elementId, double selevelelevation)
        {
            View sec_view = doc.GetElement(elementId) as View;
            if (!sec_view.IsTemplate)
            {
                IList<Element> eles = (new FilteredElementCollector(doc, elementId)).OfCategory(BuiltInCategory.OST_GenericAnnotation).WhereElementIsNotElementType().ToElements();//过滤出所有假标高相关族实例
                foreach (Element element in eles)
                {
                    string eleName = element.Name;
                    if (eleName.Contains("假标高"))
                    {
                        if (element.Location is LocationPoint)
                        {
                            ParameterSet parameterSet = element.Parameters;
                            foreach (Parameter parameter in parameterSet)
                            {
                                if (parameter.Definition.Name == "标高")
                                {
                                    if (!parameter.IsReadOnly)
                                    {
                                        LocationPoint locationPoint = element.Location as LocationPoint;
                                        XYZ xYZ = locationPoint.Point;
                                        double height = xYZ.Z - selevelelevation;//判断元素与零零标高的Z轴高差
                                                                                 //对标高文字进行格式标准化处理
                                        string str_height = standardDoubleToString(height);
                                        parameter.Set(str_height);//进行标高数值修改
                                    }
                                }
                                else if (parameter.Definition.Name == "标高数值")
                                {
                                    if (!parameter.IsReadOnly)
                                    {
                                        LocationPoint locationPoint = element.Location as LocationPoint;
                                        XYZ xYZ = locationPoint.Point;
                                        double height = xYZ.Z - selevelelevation;//判断元素与零零标高的Z轴高差
                                                                                 //对标高文字进行格式标准化处理
                                        string str_height = standardDoubleToString(height);
                                        parameter.Set(str_height);//进行标高数值修改
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //将revit内英寸数字改为字符串并标准化输出
        public string standardDoubleToString(double height)
        {
            double d = height.FeetToMilliMeter();

            double abs_d = Math.Abs(d);//求绝对值是为了进行下一步判断
            double _d = 0;
            if (abs_d / 100.0 >= 1 && abs_d / 1000.0 < 1)
            {
                _d = Math.Round(d / 1.0, 6, MidpointRounding.AwayFromZero);//四舍五入的算法
            }
            else if (abs_d / 1000.0 >= 1)
            {
                _d = Math.Round(d / 1.0, 7, MidpointRounding.AwayFromZero);
            }
            else if (abs_d / 10.0 >= 1 && abs_d / 100.0 < 1)
            {
                _d = Math.Round(d / 1.0, 5, MidpointRounding.AwayFromZero);
            }
            else if (abs_d / 1.0 >= 1 && abs_d / 10.0 < 1)
            {
                _d = Math.Round(d / 1.0, 4, MidpointRounding.AwayFromZero);
            }
            string str_height = _d.ToString();
            //对标高文字进行格式标准化处理
            if (str_height.Contains("."))
            {
                int _index = str_height.IndexOf(".");
                string backStr = str_height.Substring(_index);//以小数点为基准位置，判断是否满足 .000 格式，不满足末尾补齐
                if (backStr.Length == 2)
                {
                    str_height += "00";
                }
                else if (backStr.Length == 3)
                {
                    str_height += "0";
                }
            }
            else
            {
                str_height += ".000";
            }
            return str_height;
        }

    }// public class RequestHandler : IExternalEventHandler


    
} // namespace

