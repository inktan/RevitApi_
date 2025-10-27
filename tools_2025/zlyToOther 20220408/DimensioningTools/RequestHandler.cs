using Autodesk.Revit.UI;
using goa.Common;
using goa.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensioningTools
{
    public class RequestHandler : IExternalEventHandler
    {
        public Request Request = new Request();
        public string GetName() { return "revit addin Request Handler"; }
        public void Execute(UIApplication app)
        {
            var window = APP.MainWindow;
            window.dozeOff();
            Registry.Register(false);

            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            break;
                        }
                    case RequestId.close:
                        {
                            window.CloseForm();
                            return;
                        }
                    case RequestId.dimClosestPintInView:
                        {
                            Methods.DimClosestPointInView();
                            break;
                        }
                    case RequestId.windowSpotElevDim:
                        {
                            Methods.WindowSpotElev();
                            break;
                        }
                    case RequestId.multiFloorElev:
                        {
                            Methods.DimOnFloors();
                            break;
                        }
                    case RequestId.multiLevelElev:
                        {
                            Methods.MultiLevelElev();
                            break;
                        }
                    case RequestId.multiElevFam:
                        {
                            AutoFillUpLevelHeightAnnotation.CMD.Run();
                            break;
                        }
                    case RequestId.fakeSpotElevUpdate:
                        {
                            FakeElev_Refresh.CMD.Run();
                            break;
                        }
                    case RequestId.absElev:
                        {
                            ANNO_ELEV_ABSL.CMD.Run(app.ActiveUIDocument);
                            break;
                        }
                    case RequestId.dimAvoid:
                        {
                            Methods.DimAvoid();
                            break;
                        }
                    case RequestId.fakeDimAvoid:
                        {
                            Methods.FakeDimAvoid();
                            break;
                        }
                    case RequestId.fakeDim:
                        {
                            FAKE_DIMS.CMD.Run();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //user cancelled
            }
            catch (goa.Common.Exceptions.IgnorableException ex)
            {
                //ignore
            }
            catch (InvalidCurveLoopException ex)
            {
                UserMessages.ShowMessage("线圈非完整连续，或未闭合。");
            }
            catch (goa.Common.Exceptions.CommonUserExceptions ex)
            {
                UserMessages.ShowMessage(ex.Message);
            }
            catch (Exception ex)
            {
                UserMessages.ShowErrorMessage(ex, window);
            }
            finally
            {
                AssemblyLoader.Register(false);
                Form_cursorPrompt.Stop();
                if (window.IsAvailable())
                {
                    window.WakeUp();
                    window.Activate();
                    Registry.Register(true);
                }
            }
        }
    }
}
