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
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
//using goa.Common;
//using goa.Common.Exceptions;

namespace DimensioningTools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD_WindowSpotElev : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            var window = APP.MainWindow;
            try
            {
                Methods.WindowSpotElev();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //user cancelled
            }
            //catch (goa.Common.Exceptions.CommonUserExceptions ex)
            //{
            //    UserMessages.ShowMessage(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    UserMessages.ShowErrorMessage(ex, window);
            //}
            finally
            {
                //AssemblyLoader.Register(false);
                //Form_cursorPrompt.Stop();
                //if (window.IsAvailable())
                //{
                //    window.WakeUp();
                //    window.Activate();
                //    Registry.Register(true);
                //}
            }

            return Result.Succeeded;
        }
    }

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CMD_DimClosestPointInView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                      ref string message,
                      ElementSet elements)
        {
            var window = APP.MainWindow;
            try
            {
                Methods.DimClosestPointInView();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //user cancelled
            }
            //catch (goa.Common.Exceptions.CommonUserExceptions ex)
            //{
            //    UserMessages.ShowMessage(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    UserMessages.ShowErrorMessage(ex, window);
            //}
            finally
            {
                //AssemblyLoader.Register(false);
                //Form_cursorPrompt.Stop();
                //if (window.IsAvailable())
                //{
                //    window.WakeUp();
                //    window.Activate();
                //    Registry.Register(true);
                //}
            }

            return Result.Succeeded;
        }
    }
}
