using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


namespace ANNO_AVOD
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            View view = uidoc.ActiveView;
            //获取视图比例
            Parameter viewscale = GetViewScale(view);
            double scale = Convert.ToDouble(viewscale.AsInteger())/ 100.0;

            bool iscontinue = true;
            bool isfontArialNarrow = true;

            do
            { 
                Dimension dime = null;
                try
                {
                    dime = SelectDimension(uidoc);
                }
                catch
                {
                    iscontinue = false;
                    continue;
                }

                if (dime == null) { TaskDialog.Show("wrong", "貌似获取到的线性标注有点问题,请重新获取."); continue; }
                //判断是否属于线型标注
                if (dime.DimensionShape != DimensionShape.Linear) { TaskDialog.Show("wrong", "非线型标注,请重新获取."); continue; }

                //获取标注类型
                DimensionType type = doc.GetElement(dime.GetTypeId()) as DimensionType;
                //判断字符串类型是否是连续
                string dimensionstringtype = type.get_Parameter(BuiltInParameter.LINEAR_DIM_TYPE).AsValueString();
                if (dimensionstringtype != "连续" && dimensionstringtype != "Continuous") { TaskDialog.Show("wrong", "标注字符串类型不为连续."); break; };

                //开启事务,先将线型标注恢复初始状态.
                using (Transaction reset = new Transaction(doc))
                {
                    reset.Start("重置尺寸线标注");
                    DimensionSegmentArray segarrytemp = dime.Segments;
                    foreach(DimensionSegment seg in segarrytemp)
                    {
                        if (seg.IsTextPositionAdjustable())
                        {
                            seg.ResetTextPosition();
                        }
                    }
                    reset.Commit();
                }

                //获取文字字体
                if(isfontArialNarrow)
                {
                    Parameter textfont = GetTextFont(type);
                    if (!textfont.AsString().Equals("Arial Narrow"))
                    {
                        TaskDialog.Show("warning", "该标注字体不是Arial Narrow,可能会出现未预料的情况,但程序会继续执行.");
                        isfontArialNarrow = false;
                    }
                }

                //获取文字宽度系数
                Parameter textwidthscale = GetTextWidthScale(type);
                double widthscale = textwidthscale.AsDouble();

                //获取文字大小
                Parameter textsize = GetTextSize(type);
                double size = textsize.AsDouble();

                //获取文字偏移
                Parameter textdistancetoline = GetDistanceToLine(type);
                double offset = textdistancetoline.AsDouble();

                //确定标注方向,得道direction
                Line line = dime.Curve as Line;
                line.MakeUnbound();
                DimensionSegmentArray segarry = dime.Segments;
                int arrysize = segarry.Size;
                for(int i = 0; i<=arrysize-2;i++)
                {
                    DimensionSegment seg1 = segarry.get_Item(i);
                    DimensionSegment seg2 = segarry.get_Item(i+1);

                    XYZ textposition2 = seg1.TextPosition;
                    XYZ position2 = seg2.TextPosition;
                    XYZ leaderposition2 = seg2.LeaderEndPosition;
                    XYZ originposition2 = seg2.Origin;
                    XYZ leaderproject = line.Project(leaderposition2).XYZPoint;
                    double positiontoposition = textposition2.DistanceTo(position2);
                    double leadertoline = line.Distance(leaderposition2);
                    
                    //距离小于600
                    if (positiontoposition < (1.640419948*scale))
                    {
                        XYZ dir = Line.CreateBound(leaderposition2, leaderproject).Direction;                        

                        XYZ newposition = position2.Add(dir.Multiply(leadertoline * 2));
                        //transaction
                        using (Transaction transaction = new Transaction(doc))
                        {
                            transaction.Start("自动避让线性标注.");
                            //翻转标注
                            segarry.get_Item(i + 1).TextPosition = newposition;
                            transaction.Commit();
                        }
                        i++;
                    }
                }

            }
            while (iscontinue);



            return Result.Succeeded;
        }

        //选择标注
        public Dimension SelectDimension(UIDocument uidoc)
        {
            Reference refe = uidoc.Selection.PickObject(ObjectType.Element, new DimensionFilter(), "点选线型标注.");
            Dimension dime = uidoc.Document.GetElement(refe) as Dimension;
            return dime;
        }

        //获取视图比例
        public Parameter GetViewScale(View view)
        {
            Parameter para = view.get_Parameter(BuiltInParameter.VIEW_SCALE);
            return para;
        }
        //获取文字宽度系数
        public Parameter GetTextWidthScale(DimensionType type)
        {
            Parameter para = type.get_Parameter(BuiltInParameter.TEXT_WIDTH_SCALE);
            return para;
        }
        //获取文字大小
        public Parameter GetTextSize(DimensionType type)
        {
            Parameter para = type.get_Parameter(BuiltInParameter.TEXT_SIZE);
            return para;
        }
        //获取文字字体
        public Parameter GetTextFont(DimensionType type)
        {
            Parameter para = type.get_Parameter(BuiltInParameter.TEXT_FONT);
            return para;
        }
        //获取文字偏移
        public Parameter GetDistanceToLine(DimensionType type)
        {
            Parameter para = type.get_Parameter(BuiltInParameter.TEXT_DIST_TO_LINE);
            return para;
        }


    }
    public class DimensionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if ((BuiltInCategory)elem.Category.Id.IntegerValue == BuiltInCategory.OST_Dimensions)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
