using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using PublicProjectMethods_;

namespace MODELESS_PROJECT_TEMPLATE_WPF
{
    class SetSurfaceTransparency : RequestMethod
    {
        public SetSurfaceTransparency(UIApplication uiapp) : base(uiapp)
        { }

        internal override void Execute()
        {
            #region 与revit文档交互入口
            UIDocument uidoc = this.uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View actiView = doc.ActiveView;
            #endregion

            IList<Element> selElements1 = sel.PickElementsByRectangle(new SelPickFilter_FilledRegion(), "请点选详图区域，地库外墙线线圈（线样式为绿色）");
            using (Transaction trans = new Transaction(doc, "SetSurfaceTransparencyr"))
            {
                trans.Start();
                foreach (Element element in selElements1)
                {
                    if (!element.IsValidObject) continue;//判断物体是否为有效物体
                    //if (element.Pinned) continue;
                    actiView.SetSurfaceTransparencyr(element.Id, InputParameter.Instance.intTransparency);
                }
                trans.Commit();
            }

            //throw new NotImplementedException();
        }
    }
}
