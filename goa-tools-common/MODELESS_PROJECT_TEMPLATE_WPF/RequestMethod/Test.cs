using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using PublicProjectMethods_;
using ClipperLib;
using g3;
using System.Diagnostics;


namespace MODELESS_PROJECT_TEMPLATE_WPF
{
    class Test : RequestMethod
    {
        public Test(UIApplication uiapp) : base(uiapp)
        { }
        /// <summary>
        /// 
        /// </summary>
        internal override void Execute()
        {
            #region 与revit文档交互入口
            UIDocument uidoc = this.uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View actiView = doc.ActiveView;
            #endregion

            FamilyInstance familyInstance = doc.GetElement(sel.PickObject(ObjectType.Element)) as FamilyInstance;

            Transform transform = familyInstance.GetTransform();

            transform.Origin.ToString().TaskDialogErrorMessage();
            transform.BasisX.ToString().TaskDialogErrorMessage();
            transform.BasisY.ToString().TaskDialogErrorMessage();
            transform.BasisZ.ToString().TaskDialogErrorMessage();

            InputParameter.Instance.strBackgroundMonitorDta += "\n"+ transform.Origin.ToString();
            InputParameter.Instance.strBackgroundMonitorDta += "\n"+ transform.BasisX.ToString();
            InputParameter.Instance.strBackgroundMonitorDta += "\n"+ transform.BasisY.ToString();
            InputParameter.Instance.strBackgroundMonitorDta += "\n"+ transform.BasisZ.ToString();

        }


    }

}
