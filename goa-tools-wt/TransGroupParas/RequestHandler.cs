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

namespace TransGroupParas
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
                    case RequestId.TransferGroupFimalyParas:
                        {
                            MainMethod_tmep(uiapp);//计算轴网数据
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
        //所有相关族实例的类型标记-事务

        public void MainMethod_tmep(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            Autodesk.Revit.DB.View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            //下列数据需要winform获得
            Document originDoc = getDocment(CMD.documentSet, CMD.originFileName);//获取源文档
            Document targetDoc = getDocment(CMD.documentSet, CMD.targetFileName);//获取目标文档

            //获取源与目标文档的组元素列表
            ICollection<Element> targetMoGroupTypes = (new FilteredElementCollector(targetDoc))
                .WherePasses(new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_IOSModelGroups), new ElementCategoryFilter(BuiltInCategory.OST_IOSDetailGroups)))
                .WhereElementIsElementType()
                .ToElements();
            //ICollection<Element> targetMoGroupTypes = (new FilteredElementCollector(targetDoc)).OfCategory(BuiltInCategory.OST_IOSModelGroups).WhereElementIsElementType().ToElements();
            //获取源与源文档的模型组元素类别列表
            //ICollection<Element> originMoGroupTypes = (new FilteredElementCollector(originDoc)).OfCategory(BuiltInCategory.OST_IOSModelGroups).WhereElementIsElementType().ToElements();
            ICollection<Element> originMoGroupTypes = (new FilteredElementCollector(originDoc))
                .WherePasses(new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_IOSModelGroups), new ElementCategoryFilter(BuiltInCategory.OST_IOSDetailGroups)))
                .WhereElementIsElementType()
                .ToElements();

            IList<string> originMoDeGroupTypeNames = getIlistNames(originMoGroupTypes);//源文档内的组类型name-list

            if (CMD.selectedGroupNames.Count > 0)//判断选择的组是否为空
            {
                using (Transaction modifyPara = new Transaction(targetDoc))
                {
                    modifyPara.Start("modifyParameters");

                    foreach (string str in CMD.selectedGroupNames)
                    {
                        if (originMoDeGroupTypeNames.Contains(str))//判断选择组类型是否存在源文档组类型中
                        {
                            //源文档操作
                            GroupType originGroupType = GetGroupTypeFromStr(originMoGroupTypes, str) as GroupType;//通过选择字符串获取源文档中对应的组类别元素
                            ICollection<ElementId> originfamilysymbolIds = GetfamilysymbolIdsInGroup(originDoc, originGroupType);//获取源文档中目标组内的所有元素唯一族类型Id列表
                                                                                                                                 //目标文档操作
                            GroupType targetGroupType = GetGroupTypeFromStr(targetMoGroupTypes, str) as GroupType;//通过选择字符串获取源文档中对应的组类别元素
                            ICollection<ElementId> targetfamilysymbolIds = GetfamilysymbolIdsInGroup(targetDoc, targetGroupType);//获取源文档中目标组内的的所有元素唯一族类型Id列表

                            //尝试族类型参数传递
                            foreach (ElementId orieleId in originfamilysymbolIds)
                            {
                                Element oriEle = originDoc.GetElement(orieleId);
                                string orieleName = oriEle.Name;

                                foreach (ElementId targeldId in targetfamilysymbolIds)
                                {
                                    Element targEle = targetDoc.GetElement(targeldId);
                                    string targeleName = targEle.Name;

                                    if (orieleName == targeleName)//如果元素类型的名字一致，则对其进行族类型参数传递
                                    {
                                        //ShowTaskDialog("异常提示", orieleName);

                                        ParameterSet ori_parameterSet = oriEle.Parameters;//源文档参数集
                                        ParameterSet targe_parameterSet = targEle.Parameters;//目标文档参数集

                                        foreach (Parameter ori_parameter in ori_parameterSet)
                                        {
                                            string ori_paraName = ori_parameter.Definition.Name;

                                            foreach (Parameter targe_parameter in targe_parameterSet)
                                            {
                                                string targe_paraName = targe_parameter.Definition.Name;

                                                if (ori_paraName == targe_paraName)
                                                {
                                                    //过滤的参数均为StorageType == Integer的参数
                                                    if (!targe_parameter.IsReadOnly && targe_parameter.StorageType != StorageType.ElementId)
                                                    {
                                                        //开展子事务SubTransaction
                                                        using (SubTransaction subModifyPara = new SubTransaction(targetDoc))
                                                        {
                                                            subModifyPara.Start();
                                                            Object ori_object = ParameterExtension_temp.GetMeaningfulValue_temp(ori_parameter);//原始参数读取
                                                            ParameterExtension_temp.SetValue_temp(targe_parameter, ori_object);//目标参数修改
                                                            subModifyPara.Commit();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ShowTaskDialog("成功提示", "组名为-" + str + "-的族类型参数传递完毕。");

                        }
                        else
                        {
                            ShowTaskDialog("异常提示", "源文档不存在名为-" + str + "-的组类型");
                            continue;
                        }
                    }
                    modifyPara.Commit();
                }//transaction
                   
                CMD.selectedGroupNames = new List<string>();//避免选项被累计的问题
            }
            else
            {
                ShowTaskDialog("异常提示", "未选择任何组。");
            }
        } //main method

        //以下为各种method-------------------------------------------------------------------
        //通过字符串获取组内元素的唯一族类型id列表
        public ICollection<ElementId> GetfamilysymbolIdsInGroup(Document originDoc, GroupType originGroupType)
        {
            GroupSet groupSet = originGroupType.Groups;//获取该组类型的所有组实例

            IList<ElementId> groupMembers = new List<ElementId>();
            foreach (Group group in groupSet)
            {
                groupMembers = group.GetMemberIds();//取出最后一个组实例的所有元素ID
            }

            ICollection<ElementId> familysymbolIds = new List<ElementId>();
            foreach (ElementId eleid in groupMembers)
            {
                ElementId _eleId = originDoc.GetElement(eleid).GetTypeId();//获取组内元素的类型id
                if (_eleId.IntegerValue != -1)//---判断一个元素是否存在族类型---解决的一个主要问题是，填充样式在组内是以子类别图元形式存在
                {
                    familysymbolIds.Add(_eleId);
                }
            }
            familysymbolIds = familysymbolIds.Distinct().ToList();//剔除元素Id的重复值，得到源文档目标组内的族类型id
            return familysymbolIds;
        }

        //通过字符串获取文档中组类型
        public Element GetGroupTypeFromStr(ICollection<Element> originMoDeGroupTypes, string str)
        {
            Element groupType = null;
            foreach (Element ele in originMoDeGroupTypes)
            {
                if (str == ele.Name)//判断选择组类型是否存在源文档组类型中
                {
                    groupType = ele;
                }
                else
                {
                    continue;
                }
            }
            return groupType;
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

        //获取元素的名称string列表
        public IList<string> getIlistNames(ICollection<Element> eles)
        {
            IList<string> strs = new List<string>();
            foreach (Element ele in eles)
            {
                string eleName = ele.Name;
                strs.Add(eleName);
            }
            return strs;
        }
        //获取指定文档
        public Document getDocment(DocumentSet documentSet, string str)
        {
            Document originDoc = null;

            foreach (Document document in documentSet)//获取所有的打开文档的标题列表
            {
                string docName = document.Title;
                if (str == docName)
                {
                    originDoc = document;
                }
            }
            return originDoc;
        }
    }// public class RequestHandler : IExternalEventHandler
}  // namespace

