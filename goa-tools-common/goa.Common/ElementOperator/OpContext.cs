using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public class OpContext
    {
        public Document RefDoc;
        public string RefDocId;
        public Document TarDoc;
        public string TarDocId;
        public string DesignOptionId;
        public string DesignOptionName;
        public ElementId RefElemId;

        public OpContext
            (string _refDocId,
            string _tarDocId,
            Document _refDoc,
            Document _tarDoc,
            string _designOptionId,
            string _designOptionName,
            ElementId _refElemId)
        {
            this.RefDoc = _refDoc;
            this.TarDoc = _tarDoc;
            this.RefDocId = _refDocId;
            this.TarDocId = _tarDocId;
            this.DesignOptionId = _designOptionId;
            this.DesignOptionName = _designOptionName;
            this.RefElemId = _refElemId;
        }
        public static void GetActiveDesignOptionInfo(Document _doc, out DesignOption _dop, out string _designOptionUid, out string _designOptionName)
        {
            var dopId = DesignOption.GetActiveDesignOptionId(_doc);
            _dop = _doc.GetElement(dopId) as DesignOption;
            _designOptionName = dopId == ElementId.InvalidElementId ? "主模型" : _dop.Name;
            _designOptionUid = _dop == null ? null : _dop.UniqueId;
        }
        public static void GetDesignOptionInfo(Element _elem, out DesignOption _dop, out string _designOptionUid, out string _designOptionName)
        {
            _dop = _elem.DesignOption;
            _designOptionName = _dop == null ? "主模型" : _dop.Name;
            _designOptionUid = _dop == null ? null : _dop.UniqueId;
        }
    }
}
