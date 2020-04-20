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

using goa.Common;

namespace WIND_NUMB_new
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
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.CalAllFiMarks:
                        {
                            AddDoWinComments(uiapp);//计算门窗实例标记值
                            break;
                        }
                    case RequestId.CommandMarks:
                        {
                            AddCommandMarks(uiapp);//对当前选择视图中的实例进行文字标记注释
                            break;
                        }
                    case RequestId.CommandSingleMark:
                        {
                            AddCommandSingleMark(uiapp);//对单个实例标记进行修改
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
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                window.WakeUp();
                window.Activate();
            }
        }//execute

        //以下为各种method---------------------------------分割线---------------------------------


        //外部事件方法建立

        // 事务建立
        // 所有相关族实例的类型标记-事务

        public void AddDoWinComments(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            Autodesk.Revit.DB.View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            using (Transaction calAllFiMarks = new Transaction(doc))//事务结束
            {
                calAllFiMarks.Start("计算实例标记值");//开启事务

                //提取文档中特定的族-自定义墙身-阳台、转角窗、窗、设备平台
                FilteredElementCollector collector_4 = new FilteredElementCollector(doc);
                IList<Element> collector_4temp = collector_4.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElements();

                IList<Element> lst_eles_4 = getFieldFinstance(collector_4temp, "围护构件");//获取特定字段的族实例

                foreach (Element ele in lst_eles_4)
                {
                    ParameterSet eleParaSet = ele.Parameters;
                    IList<string> paraLst = new List<string>();//构建参数集的name-string-list
                    foreach (Parameter parameter in eleParaSet)
                    {
                        paraLst.Add(parameter.Definition.Name);
                    }
                    //if (!paraLst.Contains("洞口宽度") || !paraLst.Contains("洞口高度"))
                    if (!paraLst.Contains("洞口宽度"))
                    {
                        continue;
                    }
                    //通过族类型获取类型model参数
                    FamilyInstance fi = ele as FamilyInstance;
                    FamilySymbol fs = fi.Symbol;

                    IList<string> FSparaLst = new List<string>();//构建类型参数集的name-string-list
                    ParameterSet fsParaSet = fs.Parameters;
                    foreach (Parameter parameter in fsParaSet)
                    {
                        FSparaLst.Add(parameter.Definition.Name);
                    }
                    //if (!FSparaLst.Contains("洞口宽度") || !FSparaLst.Contains("洞口高度"))
                    if (!FSparaLst.Contains("洞口高度"))
                    {
                        continue;
                    }

                    string para01 = fs.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString();
                    double para02 = Convert.ToDouble(fi.LookupParameter("洞口宽度").AsValueString());
                    //double para03 = Convert.ToDouble(fi.LookupParameter("洞口高度").AsValueString());
                    double para03 = Convert.ToDouble(fs.LookupParameter("洞口高度").AsValueString());

                    string para04 = fi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();

                    if (para04 == "none")
                    {
                        string field = "";
                        //修改门窗族实例的标记值
                        ele.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(field);
                    }
                    else
                    {
                        //设置族实例的注释参数
                        string markField = getAndSetMark(ele, para01, para02, para03);
                    }
                }
                //提取文档中所有内建门窗族的实例
                collector_4 = new FilteredElementCollector(doc);
                IList<Element> collector_DoWn = collector_4.OfClass(typeof(FamilyInstance)).WherePasses(new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_Doors), new ElementCategoryFilter(BuiltInCategory.OST_Windows))).WhereElementIsNotElementType().ToElements();
                foreach (Element ele in collector_DoWn)
                {
                    //通过族类型获取类型model参数
                    FamilyInstance fi = ele as FamilyInstance;
                    FamilySymbol fs = fi.Symbol;

                    ParameterSet eleParaSet = fs.Parameters;
                    IList<string> paraLst = new List<string>();//构建参数集的name-string-list
                    foreach (Parameter parameter in eleParaSet)
                    {
                        paraLst.Add(parameter.Definition.Name);
                    }
                    if (!paraLst.Contains("宽度") || !paraLst.Contains("高度"))
                    {
                        continue;
                    }
                    string para01 = fs.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString();
                    double para02 = Convert.ToDouble(fs.LookupParameter("宽度").AsValueString());
                    double para03 = Convert.ToDouble(fs.LookupParameter("高度").AsValueString());

                    //设置族实例的注释参数
                    string markField = getAndSetMark(ele, para01, para02, para03);
                }

                calAllFiMarks.Commit();//提交事务
            }
            //ShowTaskDialog("Revit2020-操作成功提示", "内建门、窗\n\r" +"自定义墙身-阳台\n\r" +"自定义墙身-窗\n\r" +"自定义墙身-转角窗\n\r" +"自定义墙身-阳台\n\r" + "上述族的相关实例的类型标记已经注释完毕");
        } //main method

        //将目标视图所有可见实例标记值以文字形式注释到指定视图上
        public void AddCommandMarks(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            Autodesk.Revit.DB.View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            string str = "Arial Narrow-2.0-1.00-HDDN";
            //首先判断字体类型在不在,如果不存在该类型，则创建该类型，如果存在该类型，则删除其所有相关实例---重置标记功能
            bool bol = isCreatTextNoteType(doc, str);

            //遍历视图列表
            if (CMD.changStr == "activeView")
            {
                if (bol)//如果存在上述文字类型，则删除所有实例
                {
                    DeleteTNTins(doc, str, activeViewId);
                }
                VIewTextMark(doc, activeViewId);
            }
            else if (CMD.changStr == "slelectViews")
            {
                //让用户选择哪几个PLOT视图需要文字标记注释
                if (CMD.selectedViewNames.Count() <= 0)//判断是否选择不存在INTF视图的PLOT视图
                {
                    ShowTaskDialog("Revit2020-操作失败提示", "未选择任何PLOT视图");
                }
                else
                {
                    ICollection<ElementId> selectedfieldViewIds = selNameToEleIds(doc, CMD.selectedViewNames, CMD.fieldViewIds);//将选择目标字段视图转化为元素id
                    CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题

                    foreach (ElementId viewId in selectedfieldViewIds)
                    {
                        if (bol)//如果存在上述文字类型，则删除所有实例
                        {
                            DeleteTNTins(doc, str, viewId);
                        }
                        VIewTextMark(doc, viewId);
                    }
                }


            }
            else
            {
                ShowTaskDialog("Revit2020-操作异常提示", "未选择任何操作视图");
            }
        } //main method

        //选择单一元素进行标记值注释
        public void AddCommandSingleMark(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            Autodesk.Revit.DB.View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            //首先判断字体类型在不在,如果不存在该类型，则创建该类型，如果存在该类型，则删除其所有相关实例---重置标记功能

            isCreatTextNoteType(doc, "Arial Narrow-2.0-1.00-HDDN");

            //选择单个物体
            ElementId eleId = doc.GetElement(sel.PickObject(ObjectType.Element)).Id;//使用elementid索引具有绝对性

            //提取文档中特定的族-自定义墙身-阳台、转角窗、窗、设备平台
            IList<Element> collector_4temp = (new FilteredElementCollector(doc, activeViewId)).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElements();
            IList<Element> lst_eles_4 = getFieldFinstance(collector_4temp, "自定义墙身");//获取特定字段的族实例
            ICollection<ElementId> lst_eles_4_ids = ELEsToeleIds(doc, lst_eles_4);//将元素列表转化为Id列表

            //提取文档中所有内建门窗族的实例
            IList<Element> collector_DoWn = (new FilteredElementCollector(doc, activeViewId)).OfClass(typeof(FamilyInstance)).WherePasses(new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_Doors), new ElementCategoryFilter(BuiltInCategory.OST_Windows))).WhereElementIsNotElementType().ToElements();
            ICollection<ElementId> collector_DoWn_ids = ELEsToeleIds(doc, collector_DoWn);//将元素列表转化为Id列表

            //提取文档中的幕墙元素
            IList<Element> collector_glasswall = (new FilteredElementCollector(doc, activeViewId)).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements();
            IList<Element> lst_collector_glasswall = getFieldFinstance(collector_glasswall, "幕墙");//获取特定字段的族实例
            ICollection<ElementId> lst_collector_glasswall_ids = ELEsToeleIds(doc, lst_collector_glasswall);//将元素列表转化为Id列表

            //开展事务，文字注释标记
            transCreatMoveTNT(doc, eleId, activeView, lst_eles_4_ids, collector_DoWn_ids, lst_collector_glasswall_ids);

            //ShowTaskDialog("Revit2020-操作成功提示", "注释选择实例标记成功");
        } //main method

        //---------------------------------华丽的分割线---------------------------------

        //以下为各种method

        //对整个视图的元素MARK进行文字注释
        public void VIewTextMark(Document doc, ElementId viewid)
        {
            Autodesk.Revit.DB.View view = doc.GetElement(viewid) as Autodesk.Revit.DB.View;

            //提取文档中特定的族-自定义墙身-阳台、转角窗、窗、设备平台
            IList<Element> collector_4temp = (new FilteredElementCollector(doc, viewid)).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElements();
            IList<Element> lst_eles_4 = getFieldFinstance(collector_4temp, "围护构件");//获取特定字段的族实例
            ICollection<ElementId> lst_eles_4_ids = ELEsToeleIds(doc, lst_eles_4);//将元素列表转化为Id列表

            //提取文档中所有内建门窗族的实例
            IList<Element> collector_DoWn = (new FilteredElementCollector(doc, viewid)).OfClass(typeof(FamilyInstance)).WherePasses(new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_Doors), new ElementCategoryFilter(BuiltInCategory.OST_Windows))).WhereElementIsNotElementType().ToElements();
            //列表合并
            IList<Element> collector_DoWn_4 = collector_DoWn.Union(lst_eles_4).ToList();
            ICollection<ElementId> collector_DoWn_ids = ELEsToeleIds(doc, collector_DoWn);//将元素列表转化为Id列表

            //提取文档中的幕墙元素
            IList<Element> collector_glasswall = (new FilteredElementCollector(doc, viewid)).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements();
            IList<Element> lst_collector_glasswall = getFieldFinstance(collector_glasswall, "幕墙");//获取特定字段的族实例
            ICollection<ElementId> lst_collector_glasswall_ids = ELEsToeleIds(doc, lst_collector_glasswall);//将元素列表转化为Id列表

            //列表合并
            IList<Element> collector_DoWn_4_gW = collector_DoWn_4.Union(lst_collector_glasswall).ToList();
            ICollection<ElementId> collector_DoWn_4_gW_ids = ELEsToeleIds(doc, collector_DoWn_4_gW);//将元素列表转化为Id列表

            //开展事务组，文字注释标记
            using (TransactionGroup transGroup = new TransactionGroup(doc, "创建文字注释标记"))
            {
                if (transGroup.Start() == TransactionStatus.Started)
                {
                    foreach (ElementId eleId in collector_DoWn_4_gW_ids)//遍历所有实例元素
                    {
                        transCreatMoveTNT(doc, eleId, view, lst_eles_4_ids, collector_DoWn_ids, lst_collector_glasswall_ids);
                    }
                    transGroup.Assimilate();
                }
                else
                {
                    transGroup.RollBack();
                }
            }
            //ShowTaskDialog("Revit2020-操作成功提示", "内建门、窗\n\r" + "自定义墙身-阳台\n\r" + "自定义墙身-窗\n\r" + "自定义墙身-转角窗\n\r" + "自定义墙身-阳台\n\r" + "上述族实例在相关视图已经二维文字注释完毕");
        }

        //创建文字事务组
        public void transCreatMoveTNT(Document doc, ElementId eleId, Autodesk.Revit.DB.View view, ICollection<ElementId> lst_eles_4_ids, ICollection<ElementId> collector_DoWn_ids, ICollection<ElementId> lst_collector_glasswall_ids)
        {
            TextNote textNote = null;
            using (Transaction addCommandSingleMark = new Transaction(doc))//事务结束
            {
                addCommandSingleMark.Start("addCommandSingleMark");//开启事务
                if (lst_eles_4_ids.Contains(eleId))//元素id属于自定义门窗
                {
                    Element ele = doc.GetElement(eleId);
                    string field = ele.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();
                    if (field == null || field == "")
                    {
                        return;
                    }
                    textNote = AddrotateText_customWall(doc, ele, view, field);//将自定义墙身元素标记值转化为文字注释
                }
                else if (collector_DoWn_ids.Contains(eleId))//元素id属于内建门窗
                {
                    Element ele = doc.GetElement(eleId);
                    string field = ele.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();
                    if (field == null || field == "")
                    {
                        return;
                    }
                    textNote = AddrotateText(doc, ele, view, field);//将元素标记值转化为文字注释
                }
                else if (lst_collector_glasswall_ids.Contains(eleId))//元素id属于幕墙
                {
                    Element ele = doc.GetElement(eleId);
                    string field = ele.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                    //玻璃幕墙注释文字需要单独处理
                    if (field == null || field == "")
                    {
                        return;
                    }
                    textNote = AddrotateText_glassWall(doc, ele, view, field);//将玻璃幕墙元素的注释元素标记值转化为文字注释
                }
                else
                {
                    ShowTaskDialog("Revit2020-操作失败提示", "所选元素不属于门、窗、玻璃幕墙类型");
                    throw new NotImplementedException("所选元素不属于门窗类型");
                }
                addCommandSingleMark.Commit();//提交事务
            }
            //启动两个事务是因为，只要元素存在Revit文档中后，才可以继续对其操作
            if (textNote != null)
            {
                using (Transaction moveTextNote = new Transaction(doc))//事务结束
                {
                    moveTextNote.Start("moveTextNote");//开启事务
                    MoveTextNote(textNote, view);//移动文字至中心位置
                    moveTextNote.Commit();//提交事务
                }
            }
        }
        //移动文字长度一半的距离
        public void MoveTextNote(TextNote textNote, Autodesk.Revit.DB.View eleView)
        {
            XYZ upXYZ = textNote.UpDirection;//求定位点
            XYZ min = textNote.get_BoundingBox(eleView).Min;
            XYZ max = textNote.get_BoundingBox(eleView).Max;
            //求距离
            double distance_X = Math.Abs(min.X - max.X) / 2.0;//X方向
            double distance_Y = Math.Abs(min.Y - max.Y) / 2.0;//Y方向
            XYZ tntXYZ = textNote.Coord;
            double x = tntXYZ.X;
            double y = tntXYZ.Y;
            double z = tntXYZ.Z;
            double mistakeD = 0.9;//移动系数
            if (upXYZ.Y == 1)//左右移动
            {
                XYZ newLocXyz = new XYZ(x - distance_X * mistakeD, y, z);
                textNote.Coord = newLocXyz;
            }
            else if (upXYZ.X == -1)//上下移动
            {
                XYZ newLocXyz = new XYZ(x, y - distance_Y * mistakeD, z);
                textNote.Coord = newLocXyz;
            }
        }

        //判断文本的起始坐标位置---此处为门窗族实例的处理方式
        //判断字体是否旋转---通过对创建文本选项中的Rotation属性进行修改

        public TextNote AddrotateText(Document doc, Element ele, Autodesk.Revit.DB.View eleView, string field)
        {
            //获取实例元素的xyz定位坐标
            FamilyInstance fi = ele as FamilyInstance;
            XYZ facingOrientation = fi.FacingOrientation;
            XYZ min = ele.get_BoundingBox(eleView).Min;
            XYZ max = ele.get_BoundingBox(eleView).Max;
            XYZ middleEleXYZ = (min + max) / 2;
            //求距离
            double distance_X = Math.Abs(min.X - max.X) / 2.0;
            double distance_Y = Math.Abs(min.Y - max.Y) / 2.0;

            //声明变量
            TextNote tnt;
            double x;
            double y;
            double z;
            XYZ textLoc;
            double angle;
            //条件判断
            if (facingOrientation.Y == 1)//左往前，右往前
            {
                angle = 0;
                //文字左上角坐标点定位
                x = middleEleXYZ.X;
                y = middleEleXYZ.Y - distance_Y - 0.1;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else if (facingOrientation.Y == -1)//左往下，右往下
            {
                angle = 0;
                //文字左上角坐标点定位

                x = middleEleXYZ.X;
                y = middleEleXYZ.Y + distance_Y + 1.2;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else if (facingOrientation.X == 1)//左往右，右往右
            {
                angle = Math.PI / 2;
                //文字左上角坐标点定位

                x = middleEleXYZ.X - distance_X - 1.2;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else if (facingOrientation.X == -1)//左往左，右往左
            {
                angle = Math.PI / 2;
                //文字左上角坐标点定位

                x = middleEleXYZ.X + distance_X + 0.1;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else
            {
                angle = 0;
                //文字左上角坐标点定位

                x = middleEleXYZ.X;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
        }
        //判断文本的起始坐标位置---此处为---自定义墙身---的处理方式
        //判断字体是否旋转---通过对创建文本选项中的Rotation属性进行修改

        public TextNote AddrotateText_customWall(Document doc, Element ele, Autodesk.Revit.DB.View eleView, string field)
        {
            //获取实例元素的xyz定位坐标
            FamilyInstance fi = ele as FamilyInstance;
            XYZ facingOrientation = fi.FacingOrientation;
            XYZ min = ele.get_BoundingBox(eleView).Min;
            XYZ max = ele.get_BoundingBox(eleView).Max;
            XYZ middleEleXYZ = (min + max) / 2;
            //求距离
            double distance_X = Math.Abs(min.X - max.X) / 2.0;
            double distance_Y = Math.Abs(min.Y - max.Y) / 2.0;

            //声明变量
            TextNote tnt;
            double x;
            double y;
            double z;
            XYZ textLoc;
            double angle;
            //条件判断
            if (facingOrientation.Y == 1 || facingOrientation.Y == -1)//横向
            {
                angle = 0;
                //文字左上角坐标点定位
                x = middleEleXYZ.X;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else if (facingOrientation.X == 1 || facingOrientation.X == -1)//纵向
            {
                angle = Math.PI / 2;
                //文字左上角坐标点定位

                x = middleEleXYZ.X;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else
            {
                angle = 0;
                //文字左上角坐标点定位

                x = middleEleXYZ.X;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
        }
        //判断文本的起始坐标位置---此处为---玻璃幕墙---的处理方式
        //判断字体是否旋转---通过对创建文本选项中的Rotation属性进行修改

        public TextNote AddrotateText_glassWall(Document doc, Element ele, Autodesk.Revit.DB.View eleView, string field)
        {
            //获取实例元素的xyz定位坐标
            Wall wall = ele as Wall;
            XYZ orientation = wall.Orientation;
            XYZ min = ele.get_BoundingBox(eleView).Min;
            XYZ max = ele.get_BoundingBox(eleView).Max;
            XYZ middleEleXYZ = (min + max) / 2;
            //求距离
            double distance_X = Math.Abs(min.X - max.X) / 2.0;
            double distance_Y = Math.Abs(min.Y - max.Y) / 2.0;

            //声明变量
            TextNote tnt;
            double x;
            double y;
            double z;
            XYZ textLoc;
            double angle;
            //条件判断
            if (orientation.Y == 1 || orientation.Y == -1)//横向幕墙
            {
                angle = 0;
                //文字左上角坐标点定位
                x = middleEleXYZ.X;
                y = middleEleXYZ.Y;
                //y = middleEleXYZ.Y - distance_Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else if (orientation.X == 1 || orientation.X == -1)//竖向幕墙
            {
                angle = Math.PI / 2;
                //文字左上角坐标点定位

                x = middleEleXYZ.X;
                //x = middleEleXYZ.X + distance_X;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
            else
            {
                angle = 0;
                //文字左上角坐标点定位

                x = middleEleXYZ.X;
                y = middleEleXYZ.Y;
                z = middleEleXYZ.Z;
                textLoc = new XYZ(x, y, z);
                tnt = AddNewTextNote(doc, eleView.Id, textLoc, field, angle);
                return tnt;
            }
        }
        //创建门窗标记字体实例
        public TextNote AddNewTextNote(Document doc, ElementId viewId, XYZ textLoc, String str, double angle)
        {
            IList<Element> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).ToElements();
            string tntfield = "Arial Narrow-2.0-1.00-HDDN";//目标文字注释类型
            ElementId tntId = null;
            foreach (Element tnt in textNoteTypes)
            {
                if (tnt.Name == tntfield)//需要确认是否存在门窗标记类型的文字注释
                {
                    tntId = tnt.Id;
                }
            }
            TextNoteOptions opts = new TextNoteOptions(tntId);
            opts.Rotation = angle;
            TextNote textNote = TextNote.Create(doc, viewId, textLoc, str, opts);
            return textNote;
        }
        //获取特定field的族实例
        public IList<Element> getFieldFinstance(IList<Element> collector_4temp, string strField)
        {
            IList<Element> lst_eles_4 = new List<Element>();

            foreach (Element ele in collector_4temp)//获取特定族实例
            {
                if (ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString().Contains(strField))
                {
                    lst_eles_4.Add(ele);
                }
            }
            return lst_eles_4;
        }

        //求解并设置注释参数DOOR_NUMBER族实例---标记---值
        public string getAndSetMark(Element ele, string para01, double para02, double para03)
        {
            string field = "";
            if (para01 == null || para01 == "")
            {
                //修改门窗族实例的标记值
                ele.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(field);
            }
            else
            {
                double result02 = Math.Round(para02 / 100.0, 0, MidpointRounding.AwayFromZero);
                double result03 = Math.Round(para03 / 100.0, 0, MidpointRounding.AwayFromZero);
                if (result02 >= 1 && result03 >= 1)
                {
                    string result02str = result02.ToString();
                    string result03str = result03.ToString();
                    if (result02str.Length == 1)
                    {
                        result02str = "0" + result02str;
                    }
                    if (result03str.Length == 1)
                    {
                        result03str = "0" + result03str;
                    }
                    field = para01 + result02str + result03str;

                    //判断构件类型加上后缀a、b、c；
                    double result02_abc = Math.Round(para02 / 100.0, 2, MidpointRounding.AwayFromZero);
                    double result03_abc = Math.Round(para03 / 100.0, 2, MidpointRounding.AwayFromZero);
                    if ((int)result02_abc < result02_abc && (int)result03_abc == result03_abc)
                    {
                        field += "a";
                    }
                    else if ((int)result02_abc == result02_abc && (int)result03_abc < result03_abc)
                    {
                        field += "b";
                    }
                    else if ((int)result02_abc < result02_abc && (int)result03_abc < result03_abc)
                    {
                        field += "c";
                    }
                    //修改门窗族实例的标记值
                    ele.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(field);
                }
            }
            return field;
        }

        //首先判断目标字体类型在不在,如果不存在，则创建字体类型
        public bool isCreatTextNoteType(Document doc, string str)
        {
            IList<Element> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).ToElements();
            IList<string> tntNames = getIcollecionNames(doc, textNoteTypes);
            string tntfield = str;//目标文字注释类型
            if (!tntNames.Contains(tntfield))
            {
                //创建该字体名称
                ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);//创建标准字体
                TextNoteType textNoteType = doc.GetElement(defaultTextTypeId) as TextNoteType;
                TextNoteType newtnType;
                using (Transaction createNewtnType = new Transaction(doc))//事务结束
                {
                    createNewtnType.Start("createNewtnType");//开启事务
                    newtnType = textNoteType.Duplicate(str) as TextNoteType;//移动文字至中心位置
                    createNewtnType.Commit();//提交事务
                }
                //创建两个事务，否则前者不存在文档中
                using (Transaction modifyNewtnType = new Transaction(doc))//事务结束
                {
                    modifyNewtnType.Start("modifyNewtnType");//开启事务

                    //图形
                    newtnType.get_Parameter(BuiltInParameter.LINE_COLOR).Set(0);//颜色
                    newtnType.get_Parameter(BuiltInParameter.LINE_PEN).SetValueString("1");//线宽
                    newtnType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).Set(1); ;//背景
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
                return false;
            }
            else
            {
                return true;
            }
        }
        //删除相应字体实例
        public void DeleteTNTins(Document doc, string str, ElementId eleid)
        {
            IList<Element> textNoteInses = (new FilteredElementCollector(doc, eleid)).OfClass(typeof(TextNote)).WhereElementIsNotElementType().ToElements();

            if (textNoteInses != null && textNoteInses.Count() != 0)
            {
                using (Transaction deleteTexts = new Transaction(doc))//事务结束
                {
                    deleteTexts.Start("deleteTexts");//开启事务
                    foreach (Element ele in textNoteInses)
                    {
                        if (ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString().Contains(str))
                        {
                            doc.Delete(ele.Id);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    deleteTexts.Commit();//提交事务
                }
            }
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
        //element-ID列表转换为element 列表
        public IList<Element> eleIdsToELEs(Document doc, ICollection<ElementId> eleIds)
        {
            IList<Element> eles = new List<Element>();
            foreach (ElementId eleId in eleIds)
            {
                Element ele = doc.GetElement(eleId);
                eles.Add(ele);
            }
            return eles;
        }
        //element 列表转换为element-ID列表
        public ICollection<ElementId> ELEsToeleIds(Document doc, IList<Element> eles)
        {
            ICollection<ElementId> eleIds = new List<ElementId>();

            foreach (Element ele in eles)
            {
                ElementId eleId = ele.Id;
                eleIds.Add(eleId);
            }
            return eleIds;
        }
        //将选择视图名称string转化为元素Id
        public ICollection<ElementId> selNameToEleIds(Document doc, IList<string> selectedViewNames, ICollection<ElementId> eleIds)
        {
            IList<ElementId> selectedPlotViewIds = new List<ElementId>();

            foreach (string selectedstr in selectedViewNames)
            {
                foreach (ElementId eleId in eleIds)
                {
                    Autodesk.Revit.DB.View vie = doc.GetElement(eleId) as Autodesk.Revit.DB.View;
                    string vieName = vie.Name;
                    if (selectedstr == vieName)
                    {
                        selectedPlotViewIds.Add(eleId);
                    }
                }
            }
            return selectedPlotViewIds;
        }
    }  // public class RequestHandler : IExternalEventHandler
} // namespace

