
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;
using g3;
using System.Collections.Concurrent;
using goa.Common.g3InterOp;
using Autodesk.Revit.DB.ExternalService;
using goa.Revit.DirectContext3D;
using Autodesk.Revit.DB.ExtensibleStorage;
using goa.Common;
using goa.Common.UserOperation;



namespace BSMT_PpLayout
{
    /// <summary>
    /// 测试goa-common中的 DataStorage Entity Schema
    /// </summary>
    class Temp03 : RequestMethod
    {
        public Temp03(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {

            Vector2d vector2d = new Vector2d(0,1);
            vector2d.AngleRadToX().ToString().TaskDialogErrorMessage();


        }
    }
}
