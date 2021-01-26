using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace goa.Common
{
    /// <summary>
    /// 围护构件
    /// </summary>
    public class EnviFamInstanceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return goaCustomFamilyFilter.IsEnviFamilyInstanceElem(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
    /// <summary>
    /// 线脚、表皮、开槽、阵列
    /// </summary>
    public class SuperFacadeFamilySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return goaCustomFamilyFilter.IsSuperFacadeFamily(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
    /// <summary>
    /// 线脚、表皮。
    /// </summary>
    public class SuperFormSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return goaCustomFamilyFilter.IsSuperForm(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
    /// <summary>
    /// 表皮
    /// </summary>
    public class SuperSkinSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return goaCustomFamilyFilter.IsSuperSkinInstance(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
    /// <summary>
    /// 开槽
    /// </summary>
    public class SuperRevealSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return goaCustomFamilyFilter.IsSuperRevealElem(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    /// 基于面空心
    /// </summary>
    public class VoidFaceBasedSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            var doc = elem.Document;
            var type = doc.GetElement(elem.GetTypeId());
            return goaCustomFamilyFilter.IsVoidFaceBasedFamilySymbolElem(type);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
