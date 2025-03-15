using Autodesk.Revit.DB;

namespace BSMT_PpLayout
{
  internal class RevitEleCtrl
    {
        internal Element Ele { get; }
        internal ElementId Id
        {
            get
            {
                if (this.Ele.IsValidObject)
                    return this.Ele.Id;
                else
                    return new ElementId(-1);
            }
        }

        internal Document Doc
        {
            get
            {
                if (this.Ele.IsValidObject)
                    return this.Ele.Document;
                else
                    return null;
            }
        }
        internal View View
        {
            get
            {
                ElementId elementId = this.Ele.OwnerViewId;
                if (elementId.IntegerValue != -1)
                    return Doc.GetElement(elementId) as View;
                else
                    return Doc.ActiveView;
            }
        }
        internal EleProperty EleProperty { get; set; }

        internal RevitEleCtrl(Element _ele, EleProperty _eleProperty)
        {
            this.Ele = _ele;
            this.EleProperty = _eleProperty;
        }
        internal RevitEleCtrl(Element _ele)
        {
            this.Ele = _ele;
        }
        internal RevitEleCtrl()
        {
        }
    }
}
