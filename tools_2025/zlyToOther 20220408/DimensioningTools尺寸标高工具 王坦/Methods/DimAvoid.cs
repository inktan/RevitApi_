using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using goa.Common;

namespace DimensioningTools
{
    internal static partial class Methods
    {
        internal static void DimAvoid()
        {
            UIDocument activeUIDocument = goa.Common.APP.UIApp.ActiveUIDocument;
            Document document = activeUIDocument.Document;
            Parameter viewScale = GetViewScale(activeUIDocument.ActiveView);
            double num = Convert.ToDouble(viewScale.AsInteger()) / 100;
            bool flag = true;
            bool flag1 = true;
            using (TransactionGroup tg = new TransactionGroup(document, "尺寸避让"))
            {
                tg.Start();
                do
                {
                    Dimension dimension = null;
                    try
                    {
                        dimension = SelectDimension(activeUIDocument);
                    }
                    catch(Autodesk.Revit.Exceptions.OperationCanceledException ex)
                    {
                        flag = false;
                        tg.Assimilate();
                        continue;
                    }
                    if (dimension == null)
                    {
                        TaskDialog.Show("wrong", "貌似获取到的线性标注有点问题,请重新获取.");
                    }
                    else if (dimension.DimensionShape == DimensionShape.Linear)
                    {
                        DimensionType element = document.GetElement(dimension.GetTypeId()) as DimensionType;
                        string str = element.get_Parameter(BuiltInParameter.LINEAR_DIM_TYPE).AsValueString();
                        if ((str == "连续" ? true : str == "Continuous"))
                        {
                            using (Transaction transaction = new Transaction(document))
                            {
                                transaction.Start("重置尺寸线标注");
                                foreach (DimensionSegment segment in dimension.Segments)
                                {
                                    if (segment.IsTextPositionAdjustable())
                                    {
                                        segment.ResetTextPosition();
                                    }
                                }
                                transaction.Commit();
                            }
                            if (flag1)
                            {
                                if (!GetTextFont(element).AsString().Equals("Arial Narrow"))
                                {
                                    TaskDialog.Show("warning", "该标注字体不是Arial Narrow,可能会出现未预料的情况,但程序会继续执行.");
                                    flag1 = false;
                                }
                            }
                            GetTextWidthScale(element).AsDouble();
                            GetTextSize(element).AsDouble();
                            GetDistanceToLine(element).AsDouble();
                            Line curve = dimension.Curve as Line;
                            curve.MakeUnbound();
                            DimensionSegmentArray segments = dimension.Segments;
                            int size = segments.Size;
                            for (int i = 0; i <= size - 2; i++)
                            {
                                DimensionSegment item = segments.get_Item(i);
                                DimensionSegment dimensionSegment = segments.get_Item(i + 1);
                                XYZ textPosition = item.TextPosition;
                                XYZ xYZ = dimensionSegment.TextPosition;
                                XYZ leaderEndPosition = dimensionSegment.LeaderEndPosition;
                                XYZ origin = dimensionSegment.Origin;
                                XYZ xYZPoint = curve.Project(leaderEndPosition).XYZPoint;
                                double num1 = textPosition.DistanceTo(xYZ);
                                double num2 = curve.Distance(leaderEndPosition);
                                if (num1 < 1.640419948 * num)
                                {
                                    XYZ direction = Line.CreateBound(leaderEndPosition, xYZPoint).Direction;
                                    XYZ xYZ1 = xYZ.Add(direction.Multiply(num2 * 2));
                                    using (Transaction transaction1 = new Transaction(document))
                                    {
                                        transaction1.Start("自动避让线性标注.");
                                        segments.get_Item(i + 1).TextPosition = xYZ1;
                                        transaction1.Commit();
                                    }
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            TaskDialog.Show("wrong", "标注字符串类型不为连续.");
                            break;
                        }
                    }
                    else
                    {
                        TaskDialog.Show("wrong", "非线型标注,请重新获取.");
                    }
                }
                while (flag);
            }
        }

        private static Parameter GetDistanceToLine(DimensionType type)
        {
            return type.get_Parameter(BuiltInParameter.TEXT_DIST_TO_LINE);
        }

        private static Parameter GetTextFont(DimensionType type)
        {
            return type.get_Parameter(BuiltInParameter.TEXT_FONT);
        }

        private static Parameter GetTextSize(DimensionType type)
        {
            return type.get_Parameter(BuiltInParameter.TEXT_SIZE);
        }

        private static Parameter GetTextWidthScale(DimensionType type)
        {
            return type.get_Parameter(BuiltInParameter.TEXT_WIDTH_SCALE);
        }

        private static Parameter GetViewScale(View view)
        {
            return view.get_Parameter(BuiltInParameter.VIEW_SCALE);
        }

        private static Dimension SelectDimension(UIDocument uidoc)
        {
            var filter = new ClassSelectionFilter<Dimension>();
            Form_cursorPrompt.Start("点选线型标注.", null);
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, filter);
            Form_cursorPrompt.Stop();
            return uidoc.Document.GetElement(reference) as Dimension;
        }
    }
}
