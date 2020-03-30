using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


//批量开关轴号

namespace DATM_ON_OFF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //get a PickBox 
            PickedBox pickbox = uidoc.Selection.PickBox(PickBoxStyle.Enclosing, "框选要关闭/开启的轴号");

            //get a activeview
            View activeview = uidoc.ActiveView;

            //get Grid by visable
            FilteredElementCollector gridcollector = new FilteredElementCollector(doc);
            gridcollector.OfClass(typeof(Grid)).WhereElementIsNotElementType().WhereElementIsViewIndependent();
            IEnumerable<Grid> gridenumerable = from ele in gridcollector
                                    let grid = ele as Grid
                                    where grid.CanBeVisibleInView(activeview)
                                    select grid;
            List<Grid> grids = gridenumerable.ToList();
            //get level by visable
            FilteredElementCollector levelcollector = new FilteredElementCollector(doc);
            levelcollector.OfClass(typeof(Level)).WhereElementIsNotElementType().WhereElementIsViewIndependent();
            IEnumerable<Level> levelenumerable = from ele in levelcollector
                                     let level = ele as Level
                                     where level.CanBeVisibleInView(activeview)
                                     select level;
            List<Level> levels = levelenumerable.ToList();



            //get grid/level.endpoint & start  transaction
            //Determine which point is in the pickbox 
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("批量开关轴号");
                foreach (Grid grid in grids)
                {
                    IList<Curve> curves = grid.GetCurvesInView(DatumExtentType.ViewSpecific, activeview);
                    foreach (Curve curve in curves)
                    {
                        XYZ start = curve.GetEndPoint(0);
                        XYZ end = curve.GetEndPoint(1);
                        if (IsPointInPickbox(pickbox, start, activeview))
                        { ChangeBubbleVisible(grid, DatumEnds.End0, activeview); }

                        if (IsPointInPickbox(pickbox, end, activeview))
                        { ChangeBubbleVisible(grid, DatumEnds.End1, activeview); }

                    }
                }

                foreach (Level level in levels)
                {
                    IList<Curve> curves = level.GetCurvesInView(DatumExtentType.ViewSpecific, activeview);
                    foreach (Curve curve in curves)
                    {
                        XYZ start = curve.GetEndPoint(0);
                        XYZ end = curve.GetEndPoint(1);
                        if (IsPointInPickbox(pickbox, start, activeview))
                        { ChangeBubbleVisible(level, DatumEnds.End0, activeview); }

                        if (IsPointInPickbox(pickbox, end, activeview))
                        { ChangeBubbleVisible(level, DatumEnds.End1, activeview); }

                    }
                }



                uidoc.RefreshActiveView();
                transaction.Commit();
            }
            return Result.Succeeded;
        }



        //平面内判断一个点是否在一个矩形框(PickBox)内.
        private bool IsPointInPickbox(PickedBox pickbox, XYZ xyz, View view)
        {
            double x = Math.Round(xyz.X, 3);
            double y = Math.Round(xyz.Y, 3);
            double z = Math.Round(xyz.Z, 3);

            double xmin = Math.Round(pickbox.Min.X, 3);
            double xmax = Math.Round(pickbox.Max.X, 3);
            double ymin = Math.Round(pickbox.Min.Y, 3);
            double ymax = Math.Round(pickbox.Max.Y, 3);
            double zmin = Math.Round(pickbox.Min.Z, 3);
            double zmax = Math.Round(pickbox.Max.Z, 3);

            double tmp = 0;

            if (xmin > xmax)
            { tmp = xmin; xmin = xmax; xmax = tmp; }
            if (ymin > ymax)
            { tmp = ymin; ymin = ymax; ymax = tmp; }
            if (zmin > zmax)
            { tmp = zmin; zmin = zmax; zmax = tmp; }

            bool boolx = (x >= xmin && x <= xmax);
            bool booly = (y >= ymin && y <= ymax);
            bool boolz = (z >= zmin && z <= zmax);

            //平面图特殊情况特殊处理.
            if (view.ViewDirection.X == 0 && view.ViewDirection.Y == 0 && boolx && booly) { return true; }

            if (boolx && booly && boolz) { return true; }
            else { return false; }
        }

        private void ChangeBubbleVisible(Grid grid, DatumEnds datumend, View view)
        {
            if (grid.IsBubbleVisibleInView(datumend, view))
            { grid.HideBubbleInView(datumend, view); }
            else
            { grid.ShowBubbleInView(datumend, view); }
        }

        private void ChangeBubbleVisible(Level level, DatumEnds datumend, View view)
        {
            if (level.IsBubbleVisibleInView(datumend, view))
            { level.HideBubbleInView(datumend, view); }
            else
            { level.ShowBubbleInView(datumend, view); }
        }

    }
}



























/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


//批量开关轴号

namespace GRID_ON_OFF
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            TaskDialog.Show("GOA", "框选的轴号的显示状态会反转.");

            //get a PickBox 
            PickedBox pickbox = uidoc.Selection.PickBox(PickBoxStyle.Enclosing,"框选要关闭/开启的轴号");
            //TaskDialog.Show("test", pickbox.Min.ToString() + "\n" + pickbox.Max.ToString());


            //get a activeview
            View activeview = uidoc.ActiveView;

            //get Grid by visable
            FilteredElementCollector collectortmp = new FilteredElementCollector(doc);
            collectortmp.OfClass(typeof(Grid)).WhereElementIsNotElementType().WhereElementIsViewIndependent();
            IEnumerable<Grid> enu = from ele in collectortmp
                                    let grid = ele as Grid
                                    where grid.CanBeVisibleInView(activeview)
                                    select grid;
            List<Grid> grids = enu.ToList();

            //get grid.endpoint & start  transaction
            //Determine which point is in the pickbox 
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("批量开关轴号");
                foreach (Grid grid in grids)
                {
                    //List<Curve> curves = new List<Curve>();
                    Options option = new Options();
                    option.View = activeview;
                    option.IncludeNonVisibleObjects = false;
                    option.ComputeReferences = true;
                    GeometryElement geometry = grid.get_Geometry(option);
                    if (geometry == null) {  continue; }
                    //if(geometry.Count() != 1) { TaskDialog.Show("wrong", grid.Name + "/n请将<轴线中段>设为<连续>后再试."); return Result.Failed; }
                    XYZ start = null;
                    XYZ end = null;
                    GeometryObject geomobj = geometry.First();
                    Curve curve = geomobj as Curve;
                    if (curve == null) { continue; }
                    if (curve != null)
                    {
                        start = curve.GetEndPoint(1);
                        end = curve.GetEndPoint(0);
                    }

                    bool x = Math.Round(activeview.ViewDirection.X, 2) == 0.0;
                    bool y = Math.Round(activeview.ViewDirection.Y, 2) == 0.0;
                    bool z = Math.Round(activeview.ViewDirection.Z, 2) == 0.0;

                    if (x && y && !z)
                    {
                        if (IsPointInPickboxPlane(pickbox, end))
                        {
                            if (grid.IsBubbleVisibleInView(DatumEnds.End0, activeview))
                            { grid.HideBubbleInView(DatumEnds.End0, activeview); }
                            else
                            { grid.ShowBubbleInView(DatumEnds.End0, activeview); }
                        }
                        if (IsPointInPickboxPlane(pickbox, start))
                        {
                            if (grid.IsBubbleVisibleInView(DatumEnds.End1, activeview))
                            { grid.HideBubbleInView(DatumEnds.End1, activeview); }
                            else
                            { grid.ShowBubbleInView(DatumEnds.End1, activeview); }
                        }
                    }
                    if (!y && z)
                    {
                        if (IsPointInPickboxNorthOrSouth(pickbox, end))
                        {
                            if (grid.IsBubbleVisibleInView(DatumEnds.End0, activeview))
                            { grid.HideBubbleInView(DatumEnds.End0, activeview); }
                            else
                            { grid.ShowBubbleInView(DatumEnds.End0, activeview); }
                        }
                        if (IsPointInPickboxNorthOrSouth(pickbox, start))
                        {
                            if (grid.IsBubbleVisibleInView(DatumEnds.End1, activeview))
                            { grid.HideBubbleInView(DatumEnds.End1, activeview); }
                            else
                            { grid.ShowBubbleInView(DatumEnds.End1, activeview); }
                        }
                    }
                    if (!x && y && z)
                    {
                        if (IsPointInPickboxEastOrWest(pickbox, end))
                        {
                            if (grid.IsBubbleVisibleInView(DatumEnds.End0, activeview))
                            { grid.HideBubbleInView(DatumEnds.End0, activeview); }
                            else
                            { grid.ShowBubbleInView(DatumEnds.End0, activeview); }
                        }
                        if (IsPointInPickboxEastOrWest(pickbox, start))
                        {
                            if (grid.IsBubbleVisibleInView(DatumEnds.End1, activeview))
                            { grid.HideBubbleInView(DatumEnds.End1, activeview); }
                            else
                            { grid.ShowBubbleInView(DatumEnds.End1, activeview); }
                        }
                    }






                    //if (IsPointInPickbox(pickbox, end))
                    //{
                    //    if (grid.IsBubbleVisibleInView(DatumEnds.End0, activeview))
                    //    { grid.HideBubbleInView(DatumEnds.End0, activeview); }
                    //    else
                    //    { grid.ShowBubbleInView(DatumEnds.End0, activeview); }
                    //}

                    //if (IsPointInPickbox(pickbox, start))
                    //{
                    //    if (grid.IsBubbleVisibleInView(DatumEnds.End1, activeview))
                    //    { grid.HideBubbleInView(DatumEnds.End1, activeview); }
                    //    else
                    //    { grid.ShowBubbleInView(DatumEnds.End1, activeview); }
                    //}

                }
                uidoc.RefreshActiveView();
                transaction.Commit();
            }

            return Result.Succeeded;
        }



        //平面内判断一个点是否在一个矩形框(PickBox)内.

        //private bool IsPointInPickbox(PickedBox pickbox, XYZ xyz)
        //{
        //    double x = xyz.X;
        //    double y = xyz.Y;
        //    double z = xyz.Z;

        //    double xmin = pickbox.Min.X;
        //    double xmax = pickbox.Max.X;
        //    double ymin = pickbox.Min.Y;
        //    double ymax = pickbox.Max.Y;
        //    double zmin = pickbox.Min.Z;
        //    double zmax = pickbox.Max.Z;
        //    double tmp = 0;

        //    if (xmin < xmax)
        //    { tmp = xmin; xmin = xmax; xmax = tmp; }
        //    if (ymin < ymax)
        //    { tmp = ymin; ymin = ymax; ymax = tmp; }
        //    if (zmin < zmax)
        //    { tmp = zmin; zmin = zmax; zmax = tmp; }

        //    bool boolx = (x <= xmin && x >= xmax);
        //    bool booly = (y <= ymin && y >= ymax);
        //    bool boolz = (z <= zmin && z >= ymax);

        //    if (boolx && booly && boolz) { return true; }
        //    else { return false; }
        //}

        private bool IsPointInPickboxPlane(PickedBox pickbox, XYZ xyz)
        {
            double x = xyz.X;
            double y = xyz.Y;

            double x1 = pickbox.Min.X;
            double x2 = pickbox.Max.X;
            double y1 = pickbox.Min.Y;
            double y2 = pickbox.Max.Y;
            double tmp = 0;

            if (x1 < x2)
            { tmp = x1; x1 = x2; x2 = tmp; }
            if (y1 < y2)
            { tmp = y1; y1 = y2; y2 = tmp; }

            bool boolx = (x <= x1 && x >= x2);
            bool booly = (y <= y1 && y >= y2);

            if (boolx && booly) { return true; }
            else { return false; }
        }

        private bool IsPointInPickboxNorthOrSouth(PickedBox pickbox, XYZ xyz)
        {
            double x = xyz.X;
            double z = xyz.Z;

            double x1 = pickbox.Min.X;
            double x2 = pickbox.Max.X;
            double z1 = pickbox.Min.Z;
            double z2 = pickbox.Max.Z;
            double tmp = 0;

            if (x1 < x2)
            { tmp = x1; x1 = x2; x2 = tmp; }
            if (z1 < z2)
            { tmp = z1; z1 = z2; z2 = tmp; }

            bool boolx = (x <= x1 && x >= x2);
            bool boolz = (z <= z1 && z >= z2);

            if (boolx && boolz) { return true; }
            else { return false; }
        }

        private bool IsPointInPickboxEastOrWest(PickedBox pickbox, XYZ xyz)
        {
            double y = xyz.Y;
            double z = xyz.Z;

            double y1 = pickbox.Min.Y;
            double y2 = pickbox.Max.Y;
            double z1 = pickbox.Min.Z;
            double z2 = pickbox.Max.Z;
            double tmp = 0;

            if (y1 < y2)
            { tmp = y1; y1 = y2; y2 = tmp; }
            if (z1 < z2)
            { tmp = z1; z1 = z2; z2 = tmp; }

            bool booly = (y <= y1 && y >= y2);
            bool boolz = (z <= z1 && z >= z2);

            if (boolz && booly) { return true; }
            else { return false; }
        }



    }
}
*/
