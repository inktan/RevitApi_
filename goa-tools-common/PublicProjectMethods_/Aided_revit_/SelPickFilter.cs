using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;

namespace PublicProjectMethods_
{
    #region pick选择过滤器 class
    /// <summary>
    /// UI界面 矩形 框选 选择过滤器_详图线
    /// </summary>
    public class SelPickFilter_DetailLine : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is DetailLine)
            {
                return true;
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }
        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 选择过滤器_ 模型线 详图线
    /// </summary>
    public class SelPickFilter_Model_Detail_Line : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is ModelCurve || elem is DetailCurve)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }/// <summary>
     /// 选择过滤器_详图填充区域
     /// </summary>
    public class SelPickFilter_FilledRegion : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FilledRegion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class SelPickFilter_FilledRegion_BaseMent : ISelectionFilter
    {
        public Document Doc { get; set; }

        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FilledRegion)
            {
                FilledRegionType _filledRegionType = Doc.GetElement(elem.GetTypeId()) as FilledRegionType;
                if (_filledRegionType.Name.Contains("地库外墙范围"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    /// <summary>
    /// 选择过滤器_详图组
    /// </summary>
    public class SelPickFilter_Groups : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is Group)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    /// <summary>
    /// UI界面 矩形 框选 选择过滤器_模型组
    /// </summary>
    public class SelPickFilter_ModelGroups : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            return (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_IOSModelGroups));
            //throw new NotImplementedException();
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 过滤器，详图项目下的族实例
    /// </summary>
    public class SelPickFilter_DetailComponentFIs : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_DetailComponents))
            {
                if (elem is FamilyInstance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return false;
            //throw new NotImplementedException();
        }
    }

    /// <summary>
    /// UI界面 矩形 框选 选择过滤器_模型线
    /// </summary>
    public class SelPickFilter_ModelLine : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is ModelLine)
            {
                return true;
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }
        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// UI界面 矩形 框选 选择过滤器_DirectShape
    /// </summary>
    public class SelPickFilter_DirectShape : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is DirectShape)
            {
                return true;
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }
        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// UI界面 矩形 框选 选择过滤器_DirectShape
    /// </summary>
    public class SelPickFilter_FamilyInstance : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                return true;
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }
        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
    #endregion

}
