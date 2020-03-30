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

namespace VIEW_INTF
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
                    case RequestId.NewBuiltViews:
                        {
                            NewBuiltViews(uiapp);
                            break;
                        }
                    case RequestId.UpdateVIews:
                        {
                            UpdateViews(uiapp);
                            break;
                        }
                    case RequestId.UpdateVIews_List:
                        {
                            UpdateVIews_List(uiapp);
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

        //外部事件方法建立
        //新建视图
        public void UpdateVIews_List(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题

            //所有视图列表需要设置尾部更新-窗口列表更新
            CMD.GetViews_All(doc, "PLOT-CD", "INTF-CD", "PLOT-DD", "PLOT-BP");//获取视图列表合辑
        }
        //新建视图
        public void NewBuiltViews(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //让用户选择哪几个PLOT视图需要复制生成
            if (CMD.selectedViewNames.Count() <= 0)//判断是否选择不存在INTF视图的PLOT视图
            {
                TaskDialog.Show("Revit2020", "未选择任何PLOT视图");
            }
            else
            {
                if (CMD.changStr == "Copy_PLOT_TO_INTF")
                {
                    copyPlotToFieldView(doc, CMD.selectedViewNames, CMD.originViewIds_INTF, "PLOT-CD", "INTF-CD", new CMD.myDelegate(getINTFCopyEleIds), "VIEW-Content", "03.INTF");//指定字段视图首次复制为目标视图
                }
                else if (CMD.changStr == "_Design_")
                {
                    copyPlotToFieldView(doc, CMD.selectedViewNames, CMD.originViewIds_DD, "PLOT-CD", "PLOT-DD", new CMD.myDelegate(getDesignCopyEleIds), "VIEW-Phase", "DD");//指定字段视图首次复制为目标视图
                }
                else if (CMD.changStr == "blue_Print")
                {
                    copyPlotToFieldView(doc, CMD.selectedViewNames, CMD.originViewIds_BP, "PLOT-CD", "PLOT-BP", new CMD.myDelegate(getBlueprintCopyEleIds), "VIEW-Phase", "BP");//指定字段视图首次复制为目标视图
                }
                CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题

                //所有视图列表需要设置尾部更新-窗口列表更新
                CMD.GetViews_All(doc, "PLOT-CD", "INTF-CD", "PLOT-DD", "PLOT-BP");//获取视图列表合辑
            }
        }
        //更新视图
        public void UpdateViews(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (CMD.changStr == "Copy_PLOT_TO_INTF")
            {
                //让用户选择哪几个PLOT视图需要生成BF蓝图
                if (CMD.selectedViewNames.Count() <= 0)//判断是否选择不存在INTF视图的PLOT视图
                {
                    TaskDialog.Show("Revit2020", "未选择任何INTF视图");
                }
                else
                {
                    upadteFieldView(doc, CMD.selectedViewNames, CMD.targetViewIds_INTF, CMD.originViewIds_INTF, "PLOT-CD", "INTF-CD", new CMD.myDelegate(getINTFCopyEleIds));//更新视图函数---借助委托，将函数以参数的形式传递到函数中
                }
            }
            else if(CMD.changStr == "_Design_")
            {
                //让用户选择哪几个PLOT视图需要生成BF蓝图
                if (CMD.selectedViewNames.Count() <= 0)//判断是否选择不存在INTF视图的PLOT视图
                {
                    TaskDialog.Show("Revit2020", "未选择任何DD方案图");
                }
                else
                {
                    upadteFieldView(doc, CMD.selectedViewNames, CMD.targetViewIds_DD, CMD.originViewIds_DD, "PLOT-CD", "PLOT-DD", new CMD.myDelegate(getDesignCopyEleIds));//更新视图函数---借助委托，将函数以参数的形式传递到函数中
                }
            }
            else if(CMD.changStr == "blue_Print")
            {
                //让用户选择哪几个PLOT视图需要生成BF蓝图
                if (CMD.selectedViewNames.Count() <= 0)
                {
                    TaskDialog.Show("Revit2020", "未选择任何BP蓝图");
                }
                else
                {
                    upadteFieldView(doc, CMD.selectedViewNames, CMD.targetViewIds_BP, CMD.originViewIds_BP, "PLOT-CD", "PLOT-BP", new CMD.myDelegate(getBlueprintCopyEleIds));//更新视图函数---借助委托，将函数以参数的形式传递到函数中
                }
            }
            CMD.selectedViewNames = new List<string>();//避免选项会被累计的问题

            //所有视图列表需要设置尾部更新-窗口列表更新
            CMD.GetViews_All(doc, "PLOT-CD", "INTF-CD", "PLOT-DD", "PLOT-BP");//获取视图列表合辑
        }

        //以下为各种method---------------------------------分割线---------------------------------

        //外部事件方法建立
        //更新视图的二维图元
        public void upadteFieldView_exter(Document doc, ICollection<ElementId> fieldViewIds, ICollection<ElementId> PlotViewIds, string origin_str, string target_str, CMD.myDelegate _myDelegate)
        {
            upadteFieldView(doc, CMD.selectedViewNames, fieldViewIds, PlotViewIds, origin_str, target_str, _myDelegate);//更新视图函数---借助委托，将函数以参数的形式传递到函数中

            showSuccess(target_str);
        }

        //指定字段视图首次复制为目标视图
        public void copyPlotToFieldView(Document doc, IList<string> selectedPlotView, ICollection<ElementId> PlotViewIds, string origin_str, string target_str, CMD.myDelegate _myDelegate, string str_view_content_para, string str_view_content)
        {
            using (Transaction copyViewPlot = new Transaction(doc, "copyPLOTto"))
            {
                copyViewPlot.Start("copyPLOTto");
                foreach (string selectedstr in selectedPlotView)
                {
                    foreach (ElementId sourceViewId in PlotViewIds)
                    {
                        Autodesk.Revit.DB.View sourceView = doc.GetElement(sourceViewId) as Autodesk.Revit.DB.View;
                        string vieName = sourceView.Name;
                        if (selectedstr == vieName)
                        {
                            Autodesk.Revit.DB.View destinationView = CreateDependentCopy(doc, sourceViewId);//复制PLOT视图为指定字段视图

                            destinationView.Name = vieName.Replace(origin_str, target_str);//设置目标视图变量//更改视图名字，更元素属性，不能使用中间过渡变量进行处理
                            destinationView.LookupParameter(str_view_content_para).Set(str_view_content); ;//设置目标视图变量//更改共享参数
                            ICollection<ElementId> selectedEleIds = _myDelegate(doc, sourceViewId);//设置目标函数变量//获取需要从PLOT视图复制的2D图元
                            if (selectedEleIds != null && selectedEleIds.Count() != 0)
                            {
                                ElementTransformUtils.CopyElements(sourceView, selectedEleIds, destinationView, null, null);//复制图元
                            }
                        }
                    }
                }
                copyViewPlot.Commit();
            }
            showSuccess(target_str);
        }

        //指定字段视图更新二维图元函数-updateFieldView
        public void upadteFieldView(Document doc, IList<string> selectedFieldView, ICollection<ElementId> FieldViewId, ICollection<ElementId> PlotViewIds, string origin_str, string target_str, CMD.myDelegate _myDelegate)
        {
            ICollection<ElementId> selectedfieldViewIds = selNameToEleIds(doc, selectedFieldView, FieldViewId);//将选择目标字段视图转化为元素id
            using (Transaction UpdateEles = new Transaction(doc, "UpdateEles"))
            {
                UpdateEles.Start("UpdateEles");
                foreach (ElementId eId in selectedfieldViewIds)//遍历每一个选择的INTF视图
                {
                    Autodesk.Revit.DB.View destinationView = doc.GetElement(eId) as Autodesk.Revit.DB.View;
                    string destinationViewName = destinationView.Name.Replace(target_str, origin_str);//设置目标视图变量//
                    foreach (ElementId sourceViewId in PlotViewIds)
                    {
                        string sourceName = doc.GetElement(sourceViewId).Name;
                        if (destinationViewName == sourceName)
                        {
                            ICollection<ElementId> all2DEleIds = getAllCopyEleIds(doc, eId);//删除INTF视图的二维图元
                            doc.Delete(all2DEleIds);
                            Autodesk.Revit.DB.View sourceView = doc.GetElement(sourceViewId) as Autodesk.Revit.DB.View;
                            ICollection<ElementId> selectedEleIds = _myDelegate(doc, sourceViewId);//设置目标函数变量//获取需要从PLOT视图复制的2D图元
                            if (selectedEleIds != null && selectedEleIds.Count() != 0)
                            {
                                ElementTransformUtils.CopyElements(sourceView, selectedEleIds, destinationView, null, null);//复制图元
                            }
                        }
                    }
                }
                UpdateEles.Commit();
            }
            showSuccess(target_str);

        }

        //成功界面显示
        public void showSuccess(string str)
        {
            TaskDialog mainDialog = new TaskDialog("Revit2020");
            mainDialog.MainInstruction = "Revit2020";
            mainDialog.MainContent = "You successfully copied all the elements from PLOT to " + str;
            mainDialog.Show();
        }

        //获取需要从目标视图所有的二维图元（更新INTF视图前，需要删除所有二维图元）
        public ICollection<ElementId> getAllCopyEleIds(Document doc, ElementId eleId)
        {
            FilteredElementCollector elementsCollector = new FilteredElementCollector(doc, eleId);
            ICollection<ElementId> eleIds = elementsCollector.OwnedByView(eleId).ToElementIds();//将选择视图字段转化为元素id
            eleIds.Remove(eleIds.First());
            return eleIds;                                                                         
            
            //ICollection<ElementId> tempvieEleIds = new List<ElementId>();//将选择视图字段转化为元素id

            //string tempstr01 = doc.GetElement(eleIds.ToList()[0]).Name;//***过滤掉未知空白元素****ExtenetElem-
            //string tempstr02 = doc.GetElement(eleIds.ToList()[1]).Name;//***过滤掉未知空白元素****A-01-PLOT02-class-SketchPlace
            //string tempstr03 = doc.GetElement(eleIds.ToList()[2]).Name;//***过滤掉未知空白元素****<In-session, Lighting>-class-SunAndShadowSetting
            //string tempstr04 = doc.GetElement(eleIds.ToList()[3]).Name;//***过滤掉未知空白元素****空白-

            //IEnumerable<ElementId> vieEleIds = from elementId in eleIds
            //                                   where doc.GetElement(elementId).Name != tempstr01
            //                                   where doc.GetElement(elementId).Name != tempstr02
            //                                   where doc.GetElement(elementId).Name != tempstr03
            //                                   where doc.GetElement(elementId).Name != tempstr04
            //                                   select elementId;
        }

        //获取需要从目标视图需要拷贝的二维图元（INTF使用）
        public ICollection<ElementId> getINTFCopyEleIds(Document doc, ElementId eleId )
        {
            FilteredElementCollector elementsCollector = new FilteredElementCollector(doc, eleId);//通过视图id过滤出所有二维元素
            //ICollection<ElementId> eleIds = elementsCollector.OwnedByView(eleId).WhereElementIsNotElementType().ToElementIds();//将选择视图字段转化为元素id
            ICollection<Element> eles = elementsCollector.OwnedByView(eleId).WhereElementIsNotElementType().ToElements();//将选择视图字段转化为元素s

            //对过滤出的元素进行二次过滤
            eles.Remove(eles.First());//

            ICollection<ElementId> icol_tempvieEleIds = new List<ElementId>();
            foreach (Element _ele in eles)
            {
                icol_tempvieEleIds.Add(_ele.Id);
            }

            foreach (Element _ele in eles)
            {
                string nameType = _ele.GetType().Name;
                if (nameType == "Group" || nameType == "RevisionCloud")//去除组和云线类型
                {
                    icol_tempvieEleIds.Remove(_ele.Id);
                }

                string nameFAMILY_AND_TYPE_PARAM = _ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString();
                int _lengthFATP = nameFAMILY_AND_TYPE_PARAM.Length;
                if (_lengthFATP >= 4)//对图元name长度进行判断
                {
                    if (nameFAMILY_AND_TYPE_PARAM.Substring(_lengthFATP - 4) == "HDDN")//剔除指定后缀字段元素
                    {
                        icol_tempvieEleIds.Remove(_ele.Id);
                    }
                }
                if (_lengthFATP >= 9)//对图元name长度进行判断
                {
                    if (nameFAMILY_AND_TYPE_PARAM.Substring(_lengthFATP - 9) == "HDDN-INTF")//剔除指定后缀字段元素
                    {
                        icol_tempvieEleIds.Remove(_ele.Id);
                    }
                }
            }
            elementsCollector = new FilteredElementCollector(doc, eleId);//通过视图id过滤出所有详图组元素
            ICollection<ElementId> groupIds = elementsCollector.OwnedByView(eleId).OfCategory(BuiltInCategory.OST_IOSDetailGroups).WhereElementIsNotElementType().ToElementIds();//将选择视图字段转化为元素id
            foreach (ElementId eleidtemp in groupIds)
            {
                Group group = doc.GetElement(eleidtemp) as Group;
                string groupName = group.Name;
                int _length = groupName.Length;
                if (_length >= 4)//对图元name长度进行判断
                {
                    if (groupName.Substring(_length - 4) == "HDDN")//如果组名后缀字段为HDDN,则剔除组内的元素
                    {
                        icol_tempvieEleIds.Remove(eleidtemp);

                        foreach (ElementId eleid_group in group.GetMemberIds())
                        {
                            icol_tempvieEleIds.Remove(eleid_group);
                        }
                    }
                }
                if (_length >= 9)//对图元name长度进行判断
                {
                    if (groupName.Substring(_length - 9) == "HDDN-INTF")//如果组名后缀字段为HDDN-INTF,则剔除组内的元素
                    {
                        icol_tempvieEleIds.Remove(eleidtemp);

                        foreach (ElementId eleid_group in group.GetMemberIds())
                        {
                            icol_tempvieEleIds.Remove(eleid_group);
                        }
                    }
                }
            }
            return icol_tempvieEleIds.ToList();
        }
        //获取需要从目标视图需要拷贝的二维图元（方案使用）
        public ICollection<ElementId> getDesignCopyEleIds(Document doc, ElementId eleId)
        {
            FilteredElementCollector elementsCollector = new FilteredElementCollector(doc, eleId);//通过视图id过滤出所有二维元素
            //ICollection<ElementId> eleIds = elementsCollector.OwnedByView(eleId).WhereElementIsNotElementType().ToElementIds();//将选择视图字段转化为元素id
            ICollection<Element> eles = elementsCollector.OwnedByView(eleId).WhereElementIsNotElementType().ToElements();//将选择视图字段转化为元素s

            //对过滤出的元素进行二次过滤
            eles.Remove(eles.First());//

            ICollection<ElementId> icol_tempvieEleIds = new List<ElementId>();
            foreach (Element _ele in eles)
            {
                icol_tempvieEleIds.Add(_ele.Id);
            }

            foreach (Element _ele in eles)
            {
                string nameType = _ele.GetType().Name;
                if (nameType == "Group" || nameType == "RevisionCloud")//去除组和云线类型
                {
                    icol_tempvieEleIds.Remove(_ele.Id);
                }
                string nameFAMILY_AND_TYPE_PARAM = _ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString();
                int _lengthFATP = nameFAMILY_AND_TYPE_PARAM.Length;
                if (_lengthFATP >= 4)//对图元name长度进行判断
                {
                    if (nameFAMILY_AND_TYPE_PARAM.Substring(_lengthFATP - 4) == "HDDN")//剔除指定后缀字段元素
                    {
                        icol_tempvieEleIds.Remove(_ele.Id);
                    }
                }
                if (_lengthFATP >= 7)//对图元name长度进行判断
                {
                    if (nameFAMILY_AND_TYPE_PARAM.Substring(_lengthFATP - 7) == "HDDN-DD")//剔除指定后缀字段元素
                    {
                        icol_tempvieEleIds.Remove(_ele.Id);
                    }
                }
            }
            elementsCollector = new FilteredElementCollector(doc, eleId);//通过视图id过滤出所有详图组元素
            ICollection<ElementId> groupIds = elementsCollector.OwnedByView(eleId).OfCategory(BuiltInCategory.OST_IOSDetailGroups).WhereElementIsNotElementType().ToElementIds();//将选择视图字段转化为元素id
            foreach (ElementId eleidtemp in groupIds)
            {
                Group group = doc.GetElement(eleidtemp) as Group;
                string groupName = group.Name;
                int _length = groupName.Length;
                if (_length >= 4)//对图元name长度进行判断
                {
                    if (groupName.Substring(_length - 4) == "HDDN")//如果组名后缀字段为HDDN,则剔除组内的元素
                    {
                        foreach (ElementId eleid_group in group.GetMemberIds())
                        {
                            icol_tempvieEleIds.Remove(eleid_group);
                        }
                    }
                }
                if (_length >= 7)//对图元name长度进行判断
                {
                    if (groupName.Substring(_length - 7) != "HDDN-DD")//如果组名后缀字段为HDDN-DD,则剔除组内的元素
                    {
                        foreach (ElementId eleid_group in group.GetMemberIds())
                        {
                            icol_tempvieEleIds.Remove(eleid_group);
                        }
                    }
                }

            }
            return icol_tempvieEleIds.ToList();
        }

        //获取需要从目标视图需要拷贝的二维图元（蓝图使用）
        public ICollection<ElementId> getBlueprintCopyEleIds(Document doc, ElementId eleId)
        {
            FilteredElementCollector elementsCollector = new FilteredElementCollector(doc, eleId);//通过视图id过滤出所有二维元素
            //ICollection<ElementId> eleIds = elementsCollector.OwnedByView(eleId).WhereElementIsNotElementType().ToElementIds();//将选择视图字段转化为元素id
            ICollection<Element> eles = elementsCollector.OwnedByView(eleId).WhereElementIsNotElementType().ToElements();//将选择视图字段转化为元素s

            //对过滤出的元素进行二次过滤
            eles.Remove(eles.First());//

            ICollection<ElementId> icol_tempvieEleIds = new List<ElementId>();
            foreach (Element _ele in eles)
            {
                icol_tempvieEleIds.Add(_ele.Id);
            }

            foreach (Element _ele in eles)
            {
                string nameType = _ele.GetType().Name;
                if (nameType == "Group" || nameType == "RevisionCloud")//去除组和云线类型
                {
                    icol_tempvieEleIds.Remove(_ele.Id);
                }
                string nameFAMILY_AND_TYPE_PARAM = _ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString();
                int _lengthFATP = nameFAMILY_AND_TYPE_PARAM.Length;
                if (_lengthFATP >= 4)//对图元name长度进行判断
                {
                    if (nameFAMILY_AND_TYPE_PARAM.Substring(_lengthFATP - 4) == "HDDN")//剔除指定后缀字段元素
                    {
                        icol_tempvieEleIds.Remove(_ele.Id);
                    }
                }
                if (_lengthFATP >= 7)//对图元name长度进行判断
                {
                    if (nameFAMILY_AND_TYPE_PARAM.Substring(_lengthFATP - 7) == "HDDN-BP")//剔除指定后缀字段元素
                    {
                        icol_tempvieEleIds.Remove(_ele.Id);
                    }
                }
            }
            elementsCollector = new FilteredElementCollector(doc, eleId);//通过视图id过滤出所有详图组元素
            ICollection<ElementId> groupIds = elementsCollector.OwnedByView(eleId).OfCategory(BuiltInCategory.OST_IOSDetailGroups).WhereElementIsNotElementType().ToElementIds();//将选择视图字段转化为元素id
            foreach (ElementId eleidtemp in groupIds)
            {
                Group group = doc.GetElement(eleidtemp) as Group;
                string groupName = group.Name;
                int _length = groupName.Length;
                if (_length >= 4)//对图元name长度进行判断
                {
                    if (groupName.Substring(_length - 4) == "HDDN")//如果组名后缀字段为HDDN,则剔除组内的元素
                    {
                        foreach (ElementId eleid_group in group.GetMemberIds())
                        {
                            icol_tempvieEleIds.Remove(eleid_group);
                        }
                    }
                }
                if (_length >= 7)//对图元name长度进行判断
                {
                    if (groupName.Substring(_length - 7) == "HDDN-BP")//如果组名后缀字段为HDDN-BP,则剔除组内的元素
                    {
                        foreach (ElementId eleid_group in group.GetMemberIds())
                        {
                            icol_tempvieEleIds.Remove(eleid_group);
                        }
                    }
                }

            }
            return icol_tempvieEleIds.ToList();
            //FilteredElementCollector elementsCollector = new FilteredElementCollector(doc);
            //ICollection<ElementId> eleIds = elementsCollector.OwnedByView(eleId).ToElementIds();//将选择视图字段转化为元素id
            //ICollection<ElementId> tempvieEleIds = new List<ElementId>();//将选择视图字段转化为元素id

            //foreach (ElementId elementId in eleIds)
            //{
            //    if (doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-PATT-ZONE-04"//需要复制的-填充-族类型-familysymbol

            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-DIMS-ARCL-2.5"//需要复制的-标注-族类型-familysymbol
            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-DIMS-GRID-2.5-SCAL"
            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-DIMS-GRID-2.5"

            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "A-LEVL-SLAB-2.5"//需要复制的-注释标高-族类型-familysymbol
            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "标高_假标高"

            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-Simfang-2.5-1.00"//需要复制的-注释文字-族类型-familysymbol
            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-Simfang-2.0-1.00"
            //        || doc.GetElement(elementId).get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "G-Simfang-3.5 边框")
            //    {
            //        tempvieEleIds.Add(elementId);
            //    }
            //}
            //return tempvieEleIds.ToList();
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

        //获取视图列表名称
        public IList<string> getIcollecionNames(Document doc, ICollection<ElementId> eleIds)
        {
            IList<string> strs = new List<string>();
            foreach (ElementId vieId in eleIds)
            {
                Autodesk.Revit.DB.View vie = doc.GetElement(vieId) as Autodesk.Revit.DB.View;
                string vieName = vie.Name;
                strs.Add(vieName);
            }
            return strs;
        }

        //输出列表中所有元素的名字
        public void elementNameListOut(Document doc, ICollection<ElementId> eleIds, string str)
        {
            string Names = null;
            foreach (ElementId eId in eleIds)
            {
                Names += doc.GetElement(eId).Name + "\n";
            }
            Names += str + eleIds.Count().ToString();
            TaskDialog.Show("Revit2020", Names);
        }

        //复制视图
        public Autodesk.Revit.DB.View CreateDependentCopy(Document doc, ElementId eleId)
        {
            Autodesk.Revit.DB.View vie = doc.GetElement(eleId) as Autodesk.Revit.DB.View;
            Autodesk.Revit.DB.View dependView = null;
            ElementId newViewId = null;
            if (vie.CanViewBeDuplicated(ViewDuplicateOption.Duplicate))
            {
                newViewId = vie.Duplicate(ViewDuplicateOption.Duplicate);
                dependView = doc.GetElement(newViewId) as Autodesk.Revit.DB.View;
                return dependView;
            }
            else
            {
                TaskDialog.Show("Wrong", "该视图不能被带细节复制");
                return dependView;
            }
        }

    }  // public class RequestHandler : IExternalEventHandler
} // namespace

