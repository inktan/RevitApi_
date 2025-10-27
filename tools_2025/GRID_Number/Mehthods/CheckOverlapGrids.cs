using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

using PubFuncWt;

namespace GRID_Number
{
    class CheckOverlapGrids : RequestMethod
    {
        public CheckOverlapGrids(UIApplication _uiApp) : base(_uiApp)
        {

        }
        internal override void Execute()
        {
            if (CMD.checkOverlapGrids == "0")
            {
                Execute_00();
            }
            else if (CMD.checkOverlapGrids == "1")
            {
                Execute_01();
            }
        }
        internal void Execute_00()
        {
            List<Grid> allGrids = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().Cast<Grid>().ToList();
            // 全部轴网
            int count = allGrids.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (j <= i)
                    {
                        continue;
                    }
                    if (allGrids[i].Curve is Line && allGrids[j].Curve is Line)
                    {
                        Line grid_curve01 = allGrids[i].Curve as Line;// 这里只处理直线的轴网
                        Line grid_curve02 = allGrids[j].Curve as Line;// 这里只处理直线的轴网

                        if (grid_curve01.ToSegment2d().Direction.EpsilonEqual(grid_curve02.ToSegment2d().Direction, 1E-10))
                        {
                            double dis = grid_curve01.ToSegment2d().P0.PedalDistanceToLine(grid_curve02.ToSegment2d().P0, grid_curve02.ToSegment2d().Direction);
                            if (dis < 1E-10)
                            {
                                List<ElementId> elementIds = new List<ElementId>() { allGrids[i].Id, allGrids[j].Id };
                                this.sel.SetElementIds(elementIds);
                                this.uiDoc.ShowElements(elementIds);
                                return;
                            }
                        }
                    }
                }
            }
        }

        internal void Execute_01()
        {
            List<Grid> allGrids = (new FilteredElementCollector(doc)).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().Cast<Grid>().ToList();

            // 设计选项的轴网
            ElementId elementId = CMD.DesignOptId();
            if (elementId != ElementId.InvalidElementId && elementId.IntegerValue != -1)
            {
                allGrids = allGrids.Where(x => x.IsInMainModelOrDesignOption(elementId)).ToList();
            }
            else
            {
                return;
            }
            int count = allGrids.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (j <= i)
                    {
                        continue;
                    }
                    if (allGrids[i].Curve is Line && allGrids[j].Curve is Line)
                    {
                        Line grid_curve01 = allGrids[i].Curve as Line;// 这里只处理直线的轴网
                        Line grid_curve02 = allGrids[j].Curve as Line;// 这里只处理直线的轴网

                        if (grid_curve01.ToSegment2d().Direction.EpsilonEqual(grid_curve02.ToSegment2d().Direction, 1E-10))
                        {
                            double dis = grid_curve01.ToSegment2d().P0.PedalDistanceToLine(grid_curve02.ToSegment2d().P0, grid_curve02.ToSegment2d().Direction);
                            if (dis < 1E-10)
                            {
                                List<ElementId> elementIds = new List<ElementId>() { allGrids[i].Id, allGrids[j].Id };
                                this.sel.SetElementIds(elementIds);
                                this.uiDoc.ShowElements(elementIds);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
