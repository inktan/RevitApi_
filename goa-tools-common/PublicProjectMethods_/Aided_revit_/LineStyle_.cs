using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class LineStyle_
    {
        #region LineStyle LinePatternElement GraphicsStyle 线样式 线型图案


        /// <summary>
        /// 通过字符串寻找目标线样式
        /// </summary>
        public static GraphicsStyle GetGraphicsStyleByName(this Document doc, string lineStyleName)
        {
            List<GraphicsStyle> graphicsStyles = GetAllLineStyleIdsFromSetting(doc);
            GraphicsStyle graphicsStyle = null;
            foreach (var item in graphicsStyles)
            {
                if (item.Name == lineStyleName)
                {
                    graphicsStyle = item;
                    break;
                }
            }
            if (graphicsStyle == null)
            {
                foreach (var item in graphicsStyles)
                {
                    if (item.Name == "细线")
                    {
                        graphicsStyle = item;
                        break;
                    }
                }
            }
            return graphicsStyle;
        }

        /// <summary>
        /// 获取所有线性类别 方法一：通过创建实体线事务，通过事务回滚方式，获取-问题，容易出现事务嵌套事务的现象
        /// </summary>
        public static ICollection<ElementId> GetAllLineStyleIdsFromTransaction(this Document doc)
        {
            ICollection<ElementId> styles = new List<ElementId>();
            Transaction transaction = new Transaction(doc);
            transaction.Start("Create detail line");
            try
            {
                View view = doc.ActiveView;
                DetailCurve detailCurve = doc.Create.NewDetailCurve(view, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)));
                styles = detailCurve.GetLineStyleIds();

                transaction.RollBack();
            }
            catch (Exception ex)
            {
                transaction.RollBack();
            }
            return styles;
        }
        /// <summary>
        /// 获取所有线型类别 方法二：通过document setting
        /// </summary>
        public static List<GraphicsStyle> GetAllLineStyleIdsFromSetting(this Document doc)
        {
            List<GraphicsStyle> styles = new List<GraphicsStyle>();
            Settings documentSettings = doc.Settings;
            Categories ParentCategoyry = doc.Settings.Categories;
            Category ParentLineCategoyry = ParentCategoyry.get_Item(BuiltInCategory.OST_Lines);
            CategoryNameMap _CategoryNameMap = ParentLineCategoyry.SubCategories;
            foreach (Category lineStyle in _CategoryNameMap)
            {
                GraphicsStyle _GraphicsStyle = lineStyle.GetGraphicsStyle(GraphicsStyleType.Projection);
                styles.Add(_GraphicsStyle);
            }
            return styles;
        }
        /// <summary>
        /// 创建新的线样式,线宽为整数1-16
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Category CreatLineStyle(this Document doc, string LineStyleName, int LineWeight, Color newColor)
        {
            Category newCategory = null;
            Categories categories = doc.Settings.Categories;
            Category lineCategory = categories.get_Item(BuiltInCategory.OST_Lines);
            using (Transaction _CreatNewLineCategory = new Transaction(doc))//创建新的线样式
            {
                _CreatNewLineCategory.Start("_CreatNewLineCategory");
                newCategory = doc.Settings.Categories.NewSubcategory(lineCategory, LineStyleName);//
                newCategory.LineColor = newColor;
                newCategory.SetLineWeight(LineWeight, GraphicsStyleType.Projection);
                _CreatNewLineCategory.Commit();
            }
            return newCategory;
        }
        /// <summary>
        /// 创建线型图案 CreatLinePattern
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="patternName"></param>
        /// <returns></returns>
        public static LinePatternElement CreatLinePatternElement(this Document doc, string patternName)
        {
            List<LinePatternSegment> lstSegments = new List<LinePatternSegment>();
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.0));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.03));
            lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));

            LinePattern _linePattern = new LinePattern(patternName);
            _linePattern.SetSegments(lstSegments);

            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Create a linepattern element");
                LinePatternElement linePatternElement = LinePatternElement.Create(doc, _linePattern);
                trans.Commit();
                return linePatternElement;
            }
        }
        #endregion
    }
}
