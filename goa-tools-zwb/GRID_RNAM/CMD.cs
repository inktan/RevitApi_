using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

//轴网重命名

namespace GRID_RNAM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // get a selection 
            TaskDialog.Show("轴网重排重命名", "请选择要重排/重命名的轴网后点击左上角完成.");
            IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "选择要重命名的轴网");
            IEnumerable<Reference> enuselection = from refe in references
                                                  let grid = doc.GetElement(refe.ElementId) as Grid
                                                  where grid != null
                                                  select refe;
            IList<Reference> selection = enuselection.ToList();

            //set a window
            Window1 window = new Window1();
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.ShowDialog();

            //delete prefix
            if (window.radiobutton_delete.IsChecked == true)
            {
                if(window.textbox_deleteprefix.Text == null) { TaskDialog.Show("wrong", "请重新运行插件并输入要删除的前缀."); }
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("批量删除前缀");
                    foreach (Reference refe in selection)
                    {
                        Grid grid = doc.GetElement(refe.ElementId) as Grid;
                        if(grid == null) { continue; }
                        string dele = window.textbox_deleteprefix.Text;
                        int strlength = dele.Length;
                        string name = grid.Name;
                        if(name.IndexOf(dele) == 0)
                        {
                            grid.Name = name.Substring(strlength);
                        }
                    }
                    transaction.Commit();
                }
                return Result.Succeeded;
            }

            // exclude curve 
            foreach (Reference refe in selection)
            {
                Grid grid = doc.GetElement(refe.ElementId) as Grid;
                if (grid == null) { continue; }
                if (grid.IsCurved)
                { TaskDialog.Show("wrong", "目前暂不支持曲线轴网"); return Result.Failed; }
            }

            //Determine direction by startgrid
            TaskDialog.Show("确定方向", "请选择起始轴网,其余轴网排序的依据为距起始轴网距离的由小至大.");
            Reference selref = uidoc.Selection.PickObject(ObjectType.Element, "选择起点轴网");
            Grid gridstart = doc.GetElement(selref.ElementId) as Grid;
            if(gridstart == null) { TaskDialog.Show("wrong", "选择的不是轴网,请重新运行插件."); }
            foreach (Reference refe in selection)
            {
                Grid grid = doc.GetElement(refe.ElementId) as Grid;
                if (!IsParallel(gridstart, grid))
                { TaskDialog.Show("wrong", "暂时仅支持平行的轴网"); return Result.Failed; }
            }
            //Curve basecurve = gridstart.Curve;
            //XYZ p1 = basecurve.GetEndPoint(0);
            //XYZ p2 = basecurve.GetEndPoint(1);
            //double a = (p1.Y - p2.Y) / (p1.X - p2.X);
            //double b = p1.Y - a * p1.X;
            //XYZ newp1 = new XYZ(10000000 / 304.8, a * 10000000 / 304 + b, 0);
            //XYZ newp2 = new XYZ(-10000000 / 304.8, a * -10000000 / 304 + b, 0);
            //Line baseline = Line.CreateBound(newp1, newp2);
            Line baseline = gridstart.Curve as Line;
            baseline.MakeUnbound();

            //sort
            IEnumerable<Grid> tmpgrids = null;
            tmpgrids = from sel in selection
                       let grid = (doc.GetElement(sel.ElementId)) as Grid
                       let distance = baseline.Distance(GetPointForSort(grid))
                       orderby distance ascending
                       select grid;

            //IEnumerable to Ilist
            List<Grid> grids = tmpgrids.ToList();

            //get number & char
            int number = Convert.ToInt32(window.textbox_number.Text);
            char abc = Convert.ToChar(window.textbox_letter.Text);
            char efg = '@';

            // get a Prefix
            string prefixadd = null;
            if (window.textbox_addprefix.Text != null)
            { prefixadd = window.textbox_addprefix.Text; }
            else
            {
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("批量重命名轴网(无前缀)");
                    for (int i = 0; i < grids.Count; i++)
                    {
                        if (efg == '@')
                        { grids[i].Name = abc.ToString(); }
                        else { grids[i].Name = efg.ToString() + abc.ToString(); }
                        //exclude 'I' 'O' 'Z' 
                        abc++;
                        if (abc == 'I' || abc == 'O' || abc == 'i' || abc == 'o')
                        { abc++; }
                        if (abc == 'Z')
                        { abc++; abc++; abc++; abc++; abc++; abc++; abc++; }
                        if (abc == 'z')
                        {
                            abc = 'A';
                            efg++;
                        }
                    }
                    transaction.Commit();
                }
                return Result.Succeeded;
            }


            // from A or 1 or null
            if (window.radiobutton_letter.IsChecked == true)
            {
                // rename and start  transaction
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("批量重命名轴网(字母)");
                    for (int i = 0; i < grids.Count; i++)
                    {
                        if(efg == '@')
                        {grids[i].Name = prefixadd + abc.ToString();}
                        else { grids[i].Name = prefixadd +efg.ToString() +abc.ToString(); }
                        //exclude 'I' 'O' 'Z' 
                        abc++;
                        if (abc == 'I' || abc == 'O' || abc == 'i' || abc == 'o' )
                        { abc++; }
                        if (abc == 'Z')
                        { abc++; abc++; abc++; abc++; abc++; abc++; abc++; }
                        if(abc == 'z')
                        {
                            abc = 'A';
                            efg++;
                        }
                    }
                    transaction.Commit();
                }
            }
            if (window.radiobutton_number.IsChecked == true)
            {
                // rename and start  transaction
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("批量重命名轴网(数字)");
                    for (int i = 0; i < grids.Count; i++)
                    {
                        grids[i].Name = prefixadd + number.ToString();
                        number++;
                    }
                    transaction.Commit();
                }
            }
            return Result.Succeeded;
        }


        private XYZ GetPointForSort(Grid grid)
        {
            XYZ xyz1 = grid.Curve.GetEndPoint(0);
            XYZ xyz2 = grid.Curve.GetEndPoint(1);
            if(Math.Abs(xyz1.X - xyz2.X)<0.0001)
            { return xyz1.Y < xyz2.Y ? xyz1 : xyz2; }
            return xyz1.X < xyz2.X ? xyz1 : xyz2;
        }
        private bool IsParallel(Grid grid1, Grid grid2)
        {
            Line line1 = grid1.Curve as Line;
            Line line2 = grid2.Curve as Line;
            XYZ direction1 = line1.Direction;
            XYZ backdirection1 = direction1.Negate();
            XYZ direction2 = line2.Direction;
            if (direction2.IsAlmostEqualTo(direction1) || direction2.IsAlmostEqualTo(backdirection1)) { return true; }
            else { return false; }
        }

    }
}








            

    



        









