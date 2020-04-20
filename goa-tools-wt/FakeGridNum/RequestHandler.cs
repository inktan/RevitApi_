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
using wt_Common;

namespace FakeGridNum
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
                    case RequestId.GridReName:
                        {
                            GridReName(uiapp);
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
        public void GridReName(UIApplication uiapp)
        {
            #region 与revit文档交互入口
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion
            //获取所有的假轴号
            List<Element> AnnotationSymbolEles = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_GenericAnnotation).WhereElementIsNotElementType().ToElements().ToList();

            List<AnnotationSymbol> allFakeGridNumbers = new List<AnnotationSymbol>();//找到当前所有的目标假轴号
            foreach (Element _ele in AnnotationSymbolEles)
            {
                AnnotationSymbol _annotationSymbol = _ele as AnnotationSymbol;
                if (_annotationSymbol.Name == "A-SYMB-Grid Head")
                {
                    allFakeGridNumbers.Add(_annotationSymbol);
                }
            }

            List<Reference> pickFakeGridNumber = sel.PickObjects(ObjectType.Element, new SelPickFilter_GenericAnnotation(), "请多选择假轴号").ToList();

            List<AnnotationSymbol> FakeGridNumbers = new List<AnnotationSymbol>();
            foreach (Reference _reference in pickFakeGridNumber)
            {
                AnnotationSymbol annotationSymbol = doc.GetElement(_reference) as AnnotationSymbol;
                if (annotationSymbol.Name == "A-SYMB-Grid Head")
                {
                    FakeGridNumbers.Add(annotationSymbol);
                }
            }
            int FakeGridNumCount = FakeGridNumbers.Count;
            if (FakeGridNumCount <= 0)
            {
                _Methods.TaskDialogShowMessage("未选择假轴号");
            }
            else
            {
                //高亮显示所有目标假轴号
                //sel.SetElementIds(selGridsIds);

                //根据窗口所选轴号类型进行轴网编号
                if (CMD.changeCommand == "gridCrosswise123")//横向轴号//采用1,2,3,4,5,6,,,的形式
                {
                    IList<string> GridName = GetGridName1_10();//手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
                    SetGridNumber(doc, FakeGridNumCount, GridName, allFakeGridNumbers, FakeGridNumbers);
                }
                else if (CMD.changeCommand == "gridVerticalA1")//竖向轴号//采用A1,B2,C3,,,的形式
                {
                    IList<string> GridName = GetGridNameA1();//手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
                    SetGridNumber(doc, FakeGridNumCount, GridName, allFakeGridNumbers, FakeGridNumbers);

                }
                else if (CMD.changeCommand == "gridVerticalAA")//竖向轴号//采用AA,BA,CA,,,的形式
                {
                    IList<string> GridName = GetGridNameAA();//手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
                    SetGridNumber(doc, FakeGridNumCount, GridName, allFakeGridNumbers, FakeGridNumbers);
                }
                else
                {
                    _Methods.TaskDialogShowMessage("未选择轴网类型");
                }
            }//else
        }
        //以下为各种method---------------------------------分割线---------------------------------
        /// <summary>
        /// 根据所选轴号类型进行轴网编号
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="count"></param>
        /// <param name="GridName"></param>
        /// <param name="girdsElements"></param>
        /// <param name="intersectGridneed"></param>
        public void SetGridNumber(Document doc, int count, IList<string> GridName, List<AnnotationSymbol> allFakeGridNumbers, List<AnnotationSymbol> FakeGridNumbers)
        {
            using (Transaction changeGriName = new Transaction(doc))//开启单个轴网改名事务
            {
                if (changeGriName.Start("changeGriName") == TransactionStatus.Started)
                {
                    if (GridName.Contains(CMD.startGridName))//判断起始轴号是否与轴号类型匹配
                    {
                        int j = 0;
                        for (int i = 0; i < count; i++)
                        {
                            int index = GridName.IndexOf(CMD.startGridName);
                            string gridName = CMD.partField + GridName[index + j];
                            j++;

                            //foreach (AnnotationSymbol _AnnotationSymbol in allFakeGridNumbers)//查找所有轴网是否有重复命名关系
                            //{
                            //    Parameter _oneoffallParameters = _AnnotationSymbol.LookupParameter("GRID-No");
                            //    string temp = _oneoffallParameters.AsString();

                            //    if (temp == gridName)
                            //    {
                            //        _oneoffallParameters.Set(temp + Guid.NewGuid().ToString());
                            //    }
                            //}
                            //分轴号的应用
                            Parameter _Parameter = FakeGridNumbers[i].LookupParameter("GRID-No");
                            string fakeGridNum = _Parameter.AsString();
                            if (i == 0)
                            {
                                if (fakeGridNum.Contains("/"))
                                {
                                    _Parameter.Set("1/" + gridName);
                                    i--;
                                }
                                else
                                {
                                    _Parameter.Set(gridName);//更改轴网Name
                                }
                            }
                            else
                            {
                                if (fakeGridNum.Contains("/"))
                                {
                                    j--;
                                    Parameter __Parameter = FakeGridNumbers[i].LookupParameter("GRID-No");
                                    string _fakeGridNum = __Parameter.AsString();

                                    if (_fakeGridNum == "1/" + CMD.partField + GridName[index + j - 1])
                                    {
                                        _Methods.TaskDialogShowMessage("连续出现两次 / 号，请设计师手动处理。");
                                        break;
                                    }
                                    _Parameter.Set("1/" + CMD.partField + GridName[index + j - 1]);
                                }
                                else
                                {
                                    _Parameter.Set(gridName);//更改轴网Name
                                }
                            }
                        }
                    }
                    else
                    {
                        _Methods.TaskDialogShowMessage("起始轴号与轴号类型不匹配，请更改。");
                    }
                    if (changeGriName.Commit() != TransactionStatus.Committed)
                    {
                        _Methods.TaskDialogShowMessage("轴号修改出现异常，请重试。");
                    }
                }
                else
                {
                    _Methods.TaskDialogShowMessage("轴号修改出现异常，请重试。");
                }
            }
        }
        /// <summary>
        /// 手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
        /// </summary>
        /// <returns></returns>
        public IList<string> GetGridName1_10()
        {
            IList<string> lst_gridVertical1_10 = new List<string>();
            for (int i = 1; i <= 1000; i++)
            {
                lst_gridVertical1_10.Add(i.ToString());
            }
            return lst_gridVertical1_10;
        }
        /// <summary>
        /// 手动创建进深方向的轴号A、B、C、D、E、F、G、……A1、B1、C1、D1、E1、F1、G1、……
        /// </summary>
        /// <returns></returns>
        public IList<string> GetGridNameA1()
        {
            IList<string> lst_gridVerticalA1_Z1 = new List<string>();
            for (char startChar = 'A'; startChar <= 'Z'; startChar++)
            {
                if (startChar != 'I' && startChar != 'O' && startChar != 'Z')
                {
                    lst_gridVerticalA1_Z1.Add(startChar.ToString());
                }
            }
            int fistCountA1 = lst_gridVerticalA1_Z1.Count();
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "1");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "2");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "3");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "4");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "5");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "6");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "7");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "8");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "9");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "10");
            }
            return lst_gridVerticalA1_Z1;
        }
        /// <summary>
        /// 手动创建进深方向的轴号A、B、C、D、E、F、G、……AA、BA、CA、DA、EA、FA、GA、……
        /// </summary>
        /// <returns></returns>
        public IList<string> GetGridNameAA()
        {
            IList<string> lst_gridVerticalAA_ZZ = new List<string>();
            for (char startChar = 'A'; startChar <= 'Z'; startChar++)
            {
                if (startChar != 'I' && startChar != 'O' && startChar != 'Z')
                {
                    lst_gridVerticalAA_ZZ.Add(startChar.ToString());
                }
            }
            int fistCountAA = lst_gridVerticalAA_ZZ.Count();
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "A");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "B");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "C");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "D");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "E");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "F");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "G");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "H");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "J");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "K");
            }
            return lst_gridVerticalAA_ZZ;
        }



    }  // public class RequestHandler : IExternalEventHandler
} // namespace

