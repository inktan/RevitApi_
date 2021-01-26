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
    class CommentElementId : RequestMethod
    {
        public CommentElementId(UIApplication uiapp) : base(uiapp)
        { }

        internal override void Execute()
        {
            #region 与revit文档交互入口
            UIDocument uidoc = this.uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View actiView = doc.ActiveView;
            #endregion

            IList<Element> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).ToElements();
            string tntfield = "仿宋_3.5mm";//目标文字注释类型
            ElementId tntId = null;
            foreach (Element tnt in textNoteTypes)
            {
                if (tnt.Name == tntfield)//需要确认是否存在门窗标记类型的文字注释
                {
                    tntId = tnt.Id;
                }
            }
            TextNoteOptions opts = new TextNoteOptions(tntId);
            opts.Rotation = 0;


            IList<Element> selElements1 = sel.PickElementsByRectangle("请框选元素");
            using (Transaction trans = new Transaction(doc, "SetSurfaceTransparencyr"))
            {
                trans.Start();
                foreach (Element element in selElements1)
                {
                    if (!element.IsValidObject) continue;//判断物体是否为有效物体
                                                         //if (element.Pinned) continue;
                    BoundingBoxXYZ boundingBoxXYZ = element.get_BoundingBox(actiView);
                    if (boundingBoxXYZ == null) continue;

                    XYZ textLoc = boundingBoxXYZ.Min - new XYZ(20, 0, 0);
                    TextNote textNote = TextNote.Create(doc, actiView.Id, textLoc, element.Id.ToString(), opts);
                }
                trans.Commit();
            }

            //throw new NotImplementedException();
        }
    }
}
