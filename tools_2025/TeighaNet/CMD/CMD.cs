using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using goa.Common;
using PubFuncWt;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.IO;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using System.Collections.Generic;

namespace TeighaNet
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        internal static Document Doc;
        internal static Selection Sel;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region 与revit文档交互入口
            UIApplication uiApp = commandData.Application;
            #endregion

            // tmep
            Doc = uiApp.ActiveUIDocument.Document;
            Sel = uiApp.ActiveUIDocument.Selection;

            Reference reference = Sel.PickObject(ObjectType.Element, new SelPickFilter_ImportInstance() { Doc = Doc });

            ImportInstance importInstance = Doc.GetElement(reference) as ImportInstance;

            CADLinkType type = CMD.Doc.GetElement(importInstance.GetTypeId()) as CADLinkType;
            string pathfile = ModelPathUtils.ConvertModelPathToUserVisiblePath(type.GetExternalFileReference().GetAbsolutePath());

            TeighaServices teighaServices = new TeighaServices(uiApp);
            teighaServices.TempDwgPath = pathfile;
            teighaServices.ParseDwg();

            foreach (var item in teighaServices.DwgParser.HatchLayerInfos)
            {
                item.Key.TaskDialogErrorMessage();
                foreach (var p in item.Value)
                {
                    foreach (var _p in p.Polygon2ds)
                    {
                        CMD.Doc.CreateDirectShapeWithNewTransaction(_p.ToCurveLoop().ToList());
                    }
                }
            }

            "end123".TaskDialogErrorMessage();
            return Result.Succeeded;
        }// excute

    }//class
}//namespace
