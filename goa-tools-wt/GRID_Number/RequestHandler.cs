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

namespace GRID_Number
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
                    case RequestId.GridNmae:
                        {
                            changeGridNmae(uiapp);//计算轴网数据
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

        public void changeGridNmae(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;//Revit文档数据读取
            Autodesk.Revit.DB.View activeView = doc.ActiveView;
            ElementId activeViewId = activeView.Id;

            //创建过滤器
            IList<Element> girdsElements = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().ToElements();

            //创建与轴网相交的线
            Line baseCurve = GetBaseLine(sel);//创建两点连线
              
            int intersectCount;
            IList<Grid> intersectGrids = GetIntersectGrids(girdsElements, baseCurve, out intersectCount);//按照交点顺寻，获取所有相交轴网列表

            if (intersectCount <= 0)
            {
                MessageBox.Show("没有在轴网区域画基准线");
            }
            else
            {
                //请选择轴网轴号类型//设置窗口进行选择
                int count;
                IList<Grid> intersectGridneed = GetParallelGrid(intersectGrids, out count);//获取与首根轴线平行的轴网集
                APP.MainWindow.textBox3.Text = "已选择轴网数量为"+count.ToString();//窗口提示已选择轴网数量

                if (count <= 0)
                {
                    MessageBox.Show("未选择轴网");
                }
                else
                {
                    //高亮显示所有目标轴网
                    IList<Element> seleles = elesToELEs(doc, intersectGridneed);
                    ICollection<ElementId> selGridsIds = ELEsToeleIds(doc, seleles);
                    sel.SetElementIds(selGridsIds);

                    //根据窗口所选轴号类型进行轴网编号
                    if (APP.changeCommand == "gridCrosswise123")//横向轴号//采用1,2,3,4,5,6,,,的形式
                    {
                        IList<string> GridName = GetGridName1_10();//手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
                        SetGridNumber(doc, count, GridName, girdsElements, intersectGridneed);
                    }
                    else if (APP.changeCommand == "gridVerticalA1")//竖向轴号//采用A1,B2,C3,,,的形式
                    {
                        IList<string> GridName = GetGridNameA1();//手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
                        SetGridNumber(doc, count, GridName, girdsElements, intersectGridneed);

                    }
                    else if (APP.changeCommand == "gridVerticalAA")//竖向轴号//采用AA,BA,CA,,,的形式
                    {
                        IList<string> GridName = GetGridNameAA();//手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
                        SetGridNumber(doc, count, GridName, girdsElements, intersectGridneed);
                    }
                    else
                    {
                        MessageBox.Show("未选择轴网类型");
                    }
                }
            }//else
        } //main method

        //以下为各种method-------------------------------------------------------------------
        //按照相交点在基准线上的位置关系进行排序
        public IList<Grid> GetIntersectGrids(IList<Element> girdsElements, Line baseCurve, out int intersectCount)
        {
            IList<Grid> intersectGrids = new List<Grid>();//获取所有相交轴网列表

            //创建字典，double为相交点在基准线上的比例位置, Grid为相交轴线
            IDictionary<double, Grid> dictParaGrid = GetInterGridDict(girdsElements, baseCurve);

            //将字典转化为键值对列表，进行排序
            List<KeyValuePair<double, Grid>> lst = new List<KeyValuePair<double, Grid>>(dictParaGrid);
            //通过交点在基准线上的2维比例数据，进行轴网排序（s1为小在前则为正序，s2在前则为倒序）
            lst.Sort(delegate (KeyValuePair<double, Grid> s1, KeyValuePair<double, Grid> s2) { return s1.Key.CompareTo(s2.Key); });
            dictParaGrid.Clear();

            foreach (KeyValuePair<double, Grid> kvp in lst)
            {
                intersectGrids.Add(kvp.Value);
            }
            intersectCount = intersectGrids.Count;
            return intersectGrids;
        }
        //求的相交轴网中与第一根平行的所有轴网
        public IList<Grid> GetParallelGrid(IList<Grid> intersectGrids, out int count)
        {
            IList<Grid> intersectGridneed = new List<Grid>();//使用方向向量的点积求出与第一根轴网平行的所有轴网
            Grid grid_first = intersectGrids.First();
            intersectGrids.Remove(grid_first);
            intersectGridneed.Add(grid_first);
            Line firstLine = grid_first.Curve as Line;//求出第一条相交线的方向向量
            XYZ firstLineDirection = firstLine.Direction;

            foreach (Grid grid_i in intersectGrids)
            {
                Line line_i = grid_i.Curve as Line;
                XYZ line_i_Direction = line_i.Direction;

                //double dotProduct = firstLineDirection.DotProduct(line_i_Direction);//点积
                //if (dotProduct == 1 || dotProduct == -1)
                //{
                //    intersectGridneed.Add(grid_i);//获得所有需要重新命名的轴网
                //}

                XYZ crossXyz = firstLineDirection.CrossProduct(line_i_Direction);//叉积

                if (crossXyz.GetLength() > -0.000000000001 && crossXyz.GetLength() < 0.000000000001)//判断条件为E-12
                {
                    intersectGridneed.Add(grid_i);//获得所有需要重新命名的轴网
                }
            }
            count = intersectGridneed.Count;
            return intersectGridneed;
        }
        //创建基准线
        public  Line GetBaseLine(Selection sel)
        {
            //创建与轴网相交的线
            XYZ startPoint = sel.PickPoint("请选择轴网标号的起点");
            XYZ endPoint = sel.PickPoint("请选择轴网标号的终点");
            XYZ startPoint_base = new XYZ(startPoint.X, startPoint.Y, 0.0);
            XYZ endPoint_base = new XYZ(endPoint.X, endPoint.Y, 0.0);
            Line baseCurve = Line.CreateBound(startPoint_base, endPoint_base) as Line;//创建两点连线
            return baseCurve;
        }
        //求出轴网和基准线的相交点，以点的位置比例和轴网建立字典
        public IDictionary<double, Grid> GetInterGridDict(IList<Element> girdsElements, Line baseCurve)
        {
            IDictionary<double, Grid> dictParaGrid = new Dictionary<double, Grid>();//创建字典，double为相交点在基准线上的比例位置, Grid为相交轴线
            foreach (Element ele in girdsElements)//求出各个轴网与所画直线的关系
            {
                Grid grid_tmep = ele as Grid;
                Line grid_curve = grid_tmep.Curve as Line;
                XYZ direction = grid_curve.Direction;
                XYZ middleXyz = (grid_curve.GetEndPoint(0) + grid_curve.GetEndPoint(1)) / 2;//求出线段的中点
                double d = 10000;
                XYZ startPoint = middleXyz + direction * d;
                XYZ endPoint = middleXyz - direction * d ;
                Line grid_curve_newLine = Line.CreateBound(startPoint, endPoint);
                IntersectionResultArray results;
                SetComparisonResult result = baseCurve.Intersect(grid_curve_newLine, out results);
                if (result == SetComparisonResult.Overlap)//判断基准线是否与轴网相交
                {
                    if (results != null && results.Size == 1)//判断交点的数量是否为1
                    {
                        double intersectPara;
                        IntersectionResult iResult = results.get_Item(0);//发现无法直接获取相交点在基准线上的二维比例位置
                        XYZ intersectXYZ = iResult.XYZPoint;//得到基准线和轴网的交点
                        IntersectionResult projectXyz = baseCurve.Project(intersectXYZ);//获得交点在基准线上的投影点
                        intersectPara = projectXyz.Parameter;//将投影点转化为在基准线上的一维距离比例位置
                        dictParaGrid.Add(intersectPara, grid_tmep);
                    }
                }
            }
            return dictParaGrid;
        }
        //目标元素列表转化为eles列表
        public IList<Element> elesToELEs(Document doc, IList<Grid> intersectGridneed)
        {
            IList<Element> eles = new List<Element>();
            foreach (Grid grid in intersectGridneed)
            {
                Element ele = grid as Element;
                eles.Add(ele);
            }
            return eles;
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
        //根据所选轴号类型进行轴网编号
        public void SetGridNumber(Document doc, int count, IList<string> GridName, IList<Element> girdsElements, IList<Grid> intersectGridneed)
        {
            using (Transaction changeGriName = new Transaction(doc))//开启单个轴网改名事务
            {
                if (changeGriName.Start("changeGriName") == TransactionStatus.Started)
                {
                    if (GridName.Contains(APP.startGridName))//判断起始轴号是否与轴号类型匹配
                    {
                        int j = 0;
                        for (int i = 0; i < count; i++)
                        {
                            int index = GridName.IndexOf(APP.startGridName);
                            string gridName = APP.partField + GridName[index + j];
                            j++;

                            foreach (Element ele in girdsElements)//查找所有轴网是否有重复命名关系
                            {
                                string temp = ele.Name;
                                if (temp == gridName)
                                {
                                    ele.Name = temp + Guid.NewGuid().ToString();
                                }
                            }
                            //分轴号的应用
                            if (i == 0)
                            {
                                if (intersectGridneed[i].Name.Contains("/"))
                                {
                                    intersectGridneed[i].Name = "1/" + gridName;
                                    i--;
                                }
                                else
                                {
                                    intersectGridneed[i].Name = gridName;//更改轴网Name
                                }
                            }
                            else
                            {
                                if (intersectGridneed[i].Name.Contains("/"))
                                {
                                    j--;
                                    if (intersectGridneed[i - 1].Name == "1/" + APP.partField + GridName[index + j - 1])
                                    {
                                        showSuccess("连续出现两次 / 号，请设计师手动处理。");
                                        break;
                                    }
                                    intersectGridneed[i].Name = "1/" + APP.partField + GridName[index + j - 1];
                                }
                                else
                                {
                                    intersectGridneed[i].Name = gridName;//更改轴网Name
                                }
                            }
                        }
                    }
                    else
                    {
                        showSuccess("起始轴号与轴号类型不匹配，请更改。");
                    }
                    if (changeGriName.Commit() != TransactionStatus.Committed)
                    {
                        showSuccess("轴号修改出现异常，请重试。");
                    }
                }
                else
                {
                    showSuccess("轴号修改出现异常，请重试。");
                }
            }
        }

        //手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
        public IList<string> GetGridName1_10()
        {
            IList<string> lst_gridVertical1_10 = new List<string>();
            for (int i = 1; i <= 1000; i++)
            {
                lst_gridVertical1_10.Add(i.ToString());
            }
            return lst_gridVertical1_10;
        }
        //手动创建进深方向的轴号A、B、C、D、E、F、G、……A1、B1、C1、D1、E1、F1、G1、……
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

        //手动创建进深方向的轴号A、B、C、D、E、F、G、……AA、BA、CA、DA、EA、FA、GA、……
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
        //求两个直线的交点
        public double GetIntersectionParameter(Line line1, Line line2)
        {
            IntersectionResultArray results;
            SetComparisonResult result = line1.Intersect(line2, out results);
            MessageBox.Show(result.ToString());
            MessageBox.Show(results.Size.ToString());
            if (result != SetComparisonResult.Overlap)
            {
                throw new NotImplementedException("选择的两个线条的关系为平行或者重叠");
            }
            if (results == null || results.Size != 1)
            {
                throw new InvalidOperationException("测试的两个线条的关系相交异常");
            }
            IntersectionResult iResult = results.get_Item(0);
            double indexPara = iResult.Parameter;
            return indexPara;
        }

        //通过选择过滤器选择物体，此处为选择轴网
        public IList<Element> GetManyGridByRectangle(Selection sel)
        {
            ISelectionFilter selFilter = new GridSelectionFilter();
            IList<Element> eList = sel.PickElementsByRectangle(selFilter, "Select multiple grids") as IList<Element>;
            return eList;
        }

        //轴网选择过滤器，为class
        public class GridSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return (element.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Grids));
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return false;
            }
        }
        //成功界面显示
        public void showSuccess(string str)
        {
            TaskDialog mainDialog = new TaskDialog("Revit2020");
            mainDialog.MainInstruction = "Revit2020";
            mainDialog.MainContent = str;
            mainDialog.Show();
        }
    }// public class RequestHandler : IExternalEventHandler

}  // namespace

