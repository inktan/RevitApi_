using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicProjectMethods_;

namespace CommonLittleCommands
{

    class MoveGridEndpoint : RequestMethod
    {

        public MoveGridEndpoint(UIApplication uiapp) : base(uiapp)
        { }
        /// <summary>
        /// 
        /// </summary>
        internal override void Execute()
        {
            #region 与revit文档交互入口
            UIDocument uidoc = this.uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View actiView = doc.ActiveView;
            #endregion

            // 选择基准点
            XYZ pointGet = sel.PickPoint("请选择墙面上的一个点");
            // 选择轴网
            IList<Element> elements = sel.PickElementsByRectangle(new GridFilter(), "请拾取轴网");
            // 求基准线
            Grid grid1 = elements[0] as Grid;
            IList<Curve> gridCurves1 = grid1.GetCurvesInView(DatumExtentType.ViewSpecific, actiView);
            Line line1 = gridCurves1.First() as Line;

            // 基准点需要移动一个高度
            pointGet = pointGet + new XYZ(0, 0, line1.Origin.Z);

            XYZ direction1 = line1.Direction;

            Transform transform = Transform.CreateRotation(new XYZ(0, 0, 1), Math.PI / 2);
            Line line2 = line1.CreateTransformed(transform) as Line;
            XYZ direction2 = line2.Direction;

            // 判断基准线移动方向
            Line line_0 = Line.CreateBound(line1.GetEndPoint(0), pointGet);
            Line line_1 = Line.CreateBound(line1.GetEndPoint(1), pointGet);
            int intx = 0;
            Line baseline = null;
            if (line_0.Length < line_1.Length)
            {
                intx = 0;
                // 基准线需要左右延长一个极长距离
                baseline = Line.CreateBound(pointGet + (-1) * direction1 * 10 * 0.3048 - direction2 * 500, pointGet + (-1) * direction1 * 10 * 0.3048 + direction2 * 500);
            }
            else
            {
                intx = 1;
                baseline = Line.CreateBound(pointGet + direction1 * 10 * 0.3048 - direction2 * 500, pointGet + direction1 * 10 * 0.3048 + direction2 * 500);
            }

            // 处理每一根轴网
            foreach (Element ele in elements)
            {
                Grid singleGrid = ele as Grid;
                IList<Curve> singleCurves = singleGrid.GetCurvesInView(DatumExtentType.ViewSpecific, actiView);
                Line singelLine = singleCurves.First() as Line;

                XYZ singelDirection = singelLine.Direction;

                // 判断轴网方向与基准轴网的方向关系 选择需要保留的端点
                XYZ stayPoint = null;
                if (singelDirection.IsAlmostEqualTo(direction1 * (-1)))//反向
                {
                    stayPoint = singelLine.GetEndPoint(intx);
                }
                else// 同向
                {
                    if (intx == 0)
                    {
                        stayPoint = singelLine.GetEndPoint(1);
                    }
                    else
                    {
                        stayPoint = singelLine.GetEndPoint(0);
                    }
                }

                //求轴网和baseline的交点，基于交点和轴网远处端点创建新的轴网位置线
                IntersectionResultArray resultArray = new IntersectionResultArray();
                SetComparisonResult setComparisonResult = singelLine.Intersect(baseline, out resultArray);

                if (setComparisonResult == SetComparisonResult.Overlap)
                {
                    XYZ xYZ = resultArray.get_Item(0).XYZPoint;

                    Line newLine = Line.CreateBound(xYZ, stayPoint);

                    using (Transaction trans = new Transaction(doc, "modify"))
                    {
                        trans.Start();
                        singleGrid.SetDatumExtentType(DatumEnds.End0, actiView, DatumExtentType.ViewSpecific);
                        singleGrid.SetDatumExtentType(DatumEnds.End1, actiView, DatumExtentType.ViewSpecific);
                        singleGrid.SetCurveInView(DatumExtentType.ViewSpecific, actiView, newLine);
                        trans.Commit();
                    }

                }

            }

        }
    }

    public class GridFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Grid) //在这里判断鼠标下方的元素是什么
            {
                //过滤出所有直线轴网
                Line l = (elem as Grid).Curve as Line;
                if (l != null)
                    return true;

            }
            return false;

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
