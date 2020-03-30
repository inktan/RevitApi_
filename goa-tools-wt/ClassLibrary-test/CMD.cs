using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace ClassLibrary_test
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            APP.UIApp = commandData.Application;
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            ICollection<Element> targetMoGroupTypes = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_IOSModelGroups).WhereElementIsElementType().ToElements();
            MainWindow mainWindow = new MainWindow(uidoc);
            mainWindow.Show();
            return Result.Succeeded;

            Element ele = doc.GetElement(sel.PickObject(ObjectType.Element));


            Group group = ele as Group;
            IList<ElementId> elementIds = group.GetMemberIds();

            string str = "";
            foreach(ElementId _eleid in elementIds)
            {
                ElementId eleid = doc.GetElement(_eleid).GetTypeId();

                str += doc.GetElement(_eleid).Name +",,,"+ eleid + "\n\r";

                if (eleid.IntegerValue != -1)//判断一个元素是否存在族类型
                {
                    TaskDialog.Show("1", "1");
                }
            }
            TaskDialog.Show("1", str);

            return Result.Succeeded;

            IList<Element> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).ToElements();
            IList<string> tntNames = getIcollecionNames(doc, textNoteTypes);

            string tntfield = "Arial Narrow-2.0-1.00-HDDN";//目标文字注释类型

            if (!tntNames.Contains(tntfield))
            {
                //创建该字体名称
                ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);//创建标准字体
                TextNoteType textNoteType =doc.GetElement(defaultTextTypeId) as TextNoteType;
                TextNoteType newtnType;
                using (Transaction createNewtnType = new Transaction(doc))//事务结束
                {
                    createNewtnType.Start("createNewtnType");//开启事务
                    newtnType = textNoteType.Duplicate("Arial Narrow-2.0-1.00-HDDN123") as TextNoteType;//移动文字至中心位置
                    createNewtnType.Commit();//提交事务
                }
                //创建两个事务，否则前者不存在文档中
                using (Transaction modifyNewtnType = new Transaction(doc))//事务结束
                {
                    modifyNewtnType.Start("modifyNewtnType");//开启事务

                    //图形
                    newtnType.get_Parameter(BuiltInParameter.LINE_COLOR).Set(0);//颜色
                    newtnType.get_Parameter(BuiltInParameter.LINE_PEN).SetValueString("1");//线宽
                    newtnType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).SetValueString("透明");//背景
                    newtnType.get_Parameter(BuiltInParameter.TEXT_BOX_VISIBILITY).SetValueString("否");//显示边框
                    newtnType.get_Parameter(BuiltInParameter.LEADER_OFFSET_SHEET).SetValueString("0.0000 mm");//引线/边界偏移值
                    newtnType.get_Parameter(BuiltInParameter.LEADER_ARROWHEAD).SetValueString("无");//引线箭头---无效设置
                    //文字
                    newtnType.get_Parameter(BuiltInParameter.TEXT_FONT).Set("Arial Narrow");//文字字体
                    newtnType.get_Parameter(BuiltInParameter.TEXT_SIZE).SetValueString("2.0000 mm");//文字大小
                    newtnType.get_Parameter(BuiltInParameter.TEXT_TAB_SIZE).SetValueString("8.0000 mm");//标签尺寸
                    newtnType.get_Parameter(BuiltInParameter.TEXT_STYLE_BOLD).Set(0);//粗体
                    newtnType.get_Parameter(BuiltInParameter.TEXT_STYLE_ITALIC).Set(1);//斜体
                    newtnType.get_Parameter(BuiltInParameter.TEXT_STYLE_UNDERLINE).Set(0); ;//下划线
                    newtnType.get_Parameter(BuiltInParameter.TEXT_WIDTH_SCALE).SetValueString("1");//宽度系数

                    modifyNewtnType.Commit();//提交事务
                }
            }
            else
            {
            }

            return Result.Succeeded;
        }

        //输出列表元素的name列表
        public IList<string> getIcollecionNames(Document doc, IList<Element> eles)
        {
            IList<string> strs = new List<string>();
            foreach (Element ele in eles)
            {
                string eleName = ele.Name;
                strs.Add(eleName);
            }
            return strs;
        }
        //成功界面显示
        public void ShowTaskDialog(string mainInstruction, string mainContent)
        {
            TaskDialog mainDialog = new TaskDialog("Revit2020");
            mainDialog.MainInstruction = mainInstruction;
            mainDialog.MainContent = mainContent;
            mainDialog.Show();
        }

    }
}
