//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Autodesk.Revit.UI;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI.Selection;
//using System.Diagnostics;
//using PubFuncWt;
//using g3;

//namespace BSMT_PpLayout
//{

//    class ModifySurfaceTransparency : RequestMethod
//    {
//        public ModifySurfaceTransparency(UIApplication uiapp) : base(uiapp)
//        {
//        }
//        /// <summary>
//        /// 更改图元的透明度
//        /// </summary>
//        internal override void Execute()
//        {

//            Element element = doc.GetElement(sel.PickObject(ObjectType.Element));
//            using (Transaction trans = new Transaction(doc, "SetSurfaceTransparencyr"))
//            {
//                trans.Start();

//                if (!element.IsValidObject) return;//判断物体是否为有效物体

//                view.SetSurfaceTransparencyr(element.Id, GlobalData.transparency);
//                trans.Commit();
//            }

//        }

//    }
//}
