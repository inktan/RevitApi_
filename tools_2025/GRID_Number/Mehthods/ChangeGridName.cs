using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;

namespace GRID_Number
{
    class ChangeGridName : RequestMethod
    {
        public ChangeGridName(UIApplication _uiApp) : base(_uiApp)
        {

        }
        internal override void Execute()
        {
            //根据窗口所选轴号类型进行轴网编号
            List<string> GridName = new List<string>();
            if (CMD.changeCommand == "gridCrosswise123")
            {
                GridName = Grid_.GetGridName1_10();
            }
            else if (CMD.changeCommand == "gridVerticalA1")
            {
                GridName = Grid_.GetGridNameA1(CMD.isContainZ);
            }
            else if (CMD.changeCommand == "gridVertical1A")
            {
                GridName = Grid_.GetGridName1A(CMD.isContainZ);
            }
            else if (CMD.changeCommand == "gridVerticalAABA")
            {
                GridName = Grid_.GetGridNameAABA(CMD.isContainZ);
            }
            else if (CMD.changeCommand == "gridVerticalAAAB")
            {
                GridName = Grid_.GetGridNameAAAB(CMD.isContainZ);
            }
            else if (CMD.changeCommand == "gridVerticalAZaz")
            {
                GridName = Grid_.GetGridNameAZaz(CMD.isContainZ);
            }
            else
            {
                throw new NotImplementedException("未选择轴网类型");
            }
            if (!GridName.Contains(CMD.startGridName))//判断起始轴号是否与轴号类型匹配
            {
                throw new NotImplementedException("起始轴号与轴号类型不匹配，请更改。");
            }

            List<Grid> allGrids = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().Cast<Grid>().ToList();
            List<Grid> tarGrids = (new FilteredElementCollector(doc, this.view.Id)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().Cast<Grid>().ToList();

            ElementId elementId = CMD.DesignOptId();
            if (elementId != ElementId.InvalidElementId)
            {
                tarGrids = tarGrids.Where(x => x.IsInMainModelOrDesignOption(elementId)).ToList();
            }
            else
            {
                tarGrids = tarGrids.Where(x => x.DesignOption == null).ToList();
            }

            tarGrids = tarGrids.Where(p => p.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == CMD.selGridType).ToList();

            Line baseCurve = this.sel.GetLine();
            List<GridInfo> gridInfos = GetInterGrid(tarGrids, baseCurve).ToList().OrderBy(p => p.intersectPara).ToList();

            if (gridInfos.Count < 1) throw new NotImplementedException("未找到 同时符合 设计选项 和 轴网类型 的轴网");

            //获取与首根轴线平行的轴网集
            List<GridInfo> intersectGridneed = GetParallelGrid(gridInfos).ToList();

            // 去重
            GridInfoHandle gridInfoHandle = new GridInfoHandle(intersectGridneed);
            gridInfoHandle.RemoveDuplicates();
            if (gridInfoHandle.DupGridInfos.Count > 0)
            {
                sel.SetElementIds(gridInfoHandle.DupGridInfos.Select(p => p.Grid.Id).ToList());
                "请删除重复轴网。".TaskDialogErrorMessage();
                return;
            }

            intersectGridneed = gridInfoHandle.TarGridInfos;

            CMD.MainWindow.textBox3.Text = "已选择轴网数量为" + intersectGridneed.Count.ToString();
            sel.SetElementIds(intersectGridneed.Select(p => p.Grid.Id).ToList());

            SetGridNumber(GridName, allGrids, intersectGridneed);
        }
        /// <summary>
        /// 找到与基准线相交的轴网
        /// </summary>
        /// <param name="girds"></param>
        /// <param name="baseCurve"></param>
        /// <returns></returns>
        public IEnumerable<GridInfo> GetInterGrid(List<Grid> girds, Line baseCurve)
        {
            foreach (Grid grid in girds)//求出各个轴网与所画直线的关系
            {
                Line grid_curve = (grid as Grid).Curve as Line;// 这里只处理直线的轴网
                if (ReferenceEquals(grid_curve, null)) continue;

                XYZ middleXyz = (grid_curve.GetEndPoint(0) + grid_curve.GetEndPoint(1)) / 2;//求出线段的中点
                XYZ startPoint = middleXyz + grid_curve.Direction * 1000;
                XYZ endPoint = middleXyz - grid_curve.Direction * 1000;

                Line grid_curve_newLine = Line.CreateBound(startPoint, endPoint);

                IntersectionResultArray results;
                SetComparisonResult result = baseCurve.Intersect(grid_curve_newLine, out results);
                if (result == SetComparisonResult.Overlap)//判断基准线是否与轴网相交
                {
                    if (results != null && results.Size == 1)//判断交点的数量是否为1
                    {
                        yield return new GridInfo() { Grid = grid, Line = baseCurve, InterSectPoint = results.get_Item(0).XYZPoint };
                    }
                }
            }
        }
        /// <summary>
        /// 找到与首根轴网平行的所有轴网
        /// </summary>
        /// <param name="intersectGrids"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<GridInfo> GetParallelGrid(List<GridInfo> gridInfos)
        {
            yield return gridInfos.First();
            XYZ firstLineDirection = (gridInfos.First().Grid.Curve as Line).Direction;

            gridInfos.RemoveAt(0);
            foreach (GridInfo gridIfon in gridInfos)
            {
                double dotDoule = firstLineDirection.DotProduct((gridIfon.Grid.Curve as Line).Direction);//点积
                if (Math.Abs(dotDoule).EqualPrecision(1))//判断条件为E-12
                {
                    yield return gridIfon;//获得所有需要重新命名的轴网
                }
            }
        }

        //根据所选轴号类型进行轴网编号
        public void SetGridNumber(IList<string> GridName, List<Grid> allGrids, List<GridInfo> tarGrids)
        {
            using (Transaction changeGriName = new Transaction(doc))//开启单个轴网改名事务
            {
                if (changeGriName.Start("changeGriName") == TransactionStatus.Started)
                {
                    int j = 0;
                    for (int i = 0; i < tarGrids.Count; i++)
                    {
                        //获取起始轴号在轴号集中的序号
                        int index = GridName.IndexOf(CMD.startGridName);
                        //构造当前轴号
                        string gridName = CMD.partField + GridName[index + j];
                        j++;

                        //排除与当前构造轴号冲突的轴网轴号
                        foreach (Element ele in allGrids)
                        {
                            string temp = ele.Name;
                            if (temp == gridName || temp == "1/" + CMD.partField + gridName)
                            {
                                //这里判断轴号参数的类型-名称
                                if (CMD.MainWindow.comboBox4.SelectedIndex == 0)
                                {
                                    ele.Name = temp + Guid.NewGuid().ToString();
                                }
                                //这里判断轴号参数的类型-自定义编号
                                else if (CMD.MainWindow.comboBox4.SelectedIndex == 1)
                                {
                                    // 自定义编号，不需要更改
                                    //ele.get_Parameter(new Guid("02a45493-755a-4867-9340-8962355dfa3d")).Set(temp + Guid.NewGuid().ToString());
                                }
                            }
                        }
                        //分轴号的应用
                        if (i == 0)
                        {
                            //首位轴号中包含 /
                            if (tarGrids[i].Grid.Name.Contains("/"))
                            {
                                j--;
                                if (CMD.MainWindow.comboBox4.SelectedIndex == 0)
                                {
                                    tarGrids[i].Grid.Name = "手动改/" + gridName;
                                    "当前选择首个收好出现分轴号符号 / ，请确认是否正确。".TaskDialogErrorMessage();
                                }
                                else if (CMD.MainWindow.comboBox4.SelectedIndex == 1)
                                {
                                    //自定义编号的guid需要联系之前设置轴网族参数的同事
                                    tarGrids[i].Grid.get_Parameter(new Guid("02a45493-755a-4867-9340-8962355dfa3d")).Set("1/" + gridName);
                                }
                                //i--;
                            }
                            else
                            {
                                //名称
                                if (CMD.MainWindow.comboBox4.SelectedIndex == 0)
                                {
                                    tarGrids[i].Grid.Name = gridName;//更改轴网Name
                                }
                                //自定义编号
                                else if (CMD.MainWindow.comboBox4.SelectedIndex == 1)
                                {
                                    tarGrids[i].Grid.get_Parameter(new Guid("02a45493-755a-4867-9340-8962355dfa3d")).Set(gridName);
                                }
                            }
                        }
                        else
                        {
                            if (tarGrids[i].Grid.Name.Contains("/"))
                            {
                                j--;
                                if (tarGrids[i - 1].Grid.Name == "1/" + CMD.partField + GridName[index + j - 1])
                                {
                                    "连续出现两次 / 号，请设计师手动处理。".TaskDialogErrorMessage();
                                    break;
                                }
                                if (CMD.MainWindow.comboBox4.SelectedIndex == 0)
                                {
                                    tarGrids[i].Grid.Name = "1/" + CMD.partField + GridName[index + j - 1];
                                }
                                else if (CMD.MainWindow.comboBox4.SelectedIndex == 1)
                                {
                                    tarGrids[i].Grid.get_Parameter(new Guid("02a45493-755a-4867-9340-8962355dfa3d")).Set("1/" + CMD.partField + GridName[index + j - 1]);
                                }
                            }
                            else
                            {
                                if (CMD.MainWindow.comboBox4.SelectedIndex == 0)
                                {
                                    tarGrids[i].Grid.Name = gridName;//更改轴网Name
                                }
                                else if (CMD.MainWindow.comboBox4.SelectedIndex == 1)
                                {
                                    tarGrids[i].Grid.get_Parameter(new Guid("02a45493-755a-4867-9340-8962355dfa3d")).Set(gridName);
                                }
                            }
                        }
                    }
                }
                if (changeGriName.Commit() != TransactionStatus.Committed)
                {
                    "轴号修改出现异常，请重试。".TaskDialogErrorMessage();
                }
            }
        }
    }
}
