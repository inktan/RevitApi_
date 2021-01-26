using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using goa.Common;
using System.Windows;


namespace goa.RevitUI
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD : IExternalCommand
    {
        internal static IList<FamilyInstance> allFi;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region 与revit文档交互入口
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            View acvtiView = doc.ActiveView;
            #endregion

            try
            {
                //pick module
                // allFi = BasicMethod.getFiFromSel(uidoc);
                if (allFi.Count == 0)
                {
                    TaskDialog.Show("提示", "请先选中模块内的图元");
                    return Result.Cancelled;
                }
                else
                {
                    //elemIds是选中模块内的所有图元

                    //执行对模块的复制、移动、镜像、旋转
                    //Methods.Move(doc,elemIds);
                    //Methods.Copy(doc, elemIds);
                    //Methods.MirrorPick(doc, elemIds);
                    //Methods.Rotate(doc, elemIds);

                }

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                goa.Common.UserMessages.ShowErrorMessage(ex, null);
                return Result.Failed;
            }
        }


    }
}
