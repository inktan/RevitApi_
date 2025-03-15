﻿using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;

namespace ClearFloorMarking
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        internal static UIDocument UIDoc;
        internal static Document Doc;
        internal static Selection Sel;
        internal static Level UpLevel;
        internal static Level Level;
        internal static Level DownLevel;

        public static XYZ PositonMoveDis { get; internal set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region 与revit文档交互入口
            UIApplication uiapp = commandData.Application;
            #endregion

            // tmep
            UIDoc = uiapp.ActiveUIDocument;
            Doc = uiapp.ActiveUIDocument.Document;
            Sel = uiapp.ActiveUIDocument.Selection;

            try
            {
                Reference reference = Sel.PickObject(ObjectType.Element, new FloorFliter());
                Element element = Doc.GetElement(reference);
                Floor floor = element as Floor;

                using (Transaction _trans = new Transaction(Doc, "清楚 注释"))
                {
                    _trans.Start();
                    floor.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("");

                    _trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }// excute

    }//class

    internal class FloorFliter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            if (elem is Floor)
                return true;
            else
                return false;
            //throw new NotImplementedException();
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}//namespace



