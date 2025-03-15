using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using PubFuncWt;
using g3;
using Octree;
using goa.Common;
using QuadTrees;
using System.Drawing;
using goa.Common.UserOperation;
using goa.Revit.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;

namespace MountSiteDesignAnaly
{
    class ClearCalResults : RequestMethod
    {

        public ClearCalResults(UIApplication uiapp) : base(uiapp)
        {
        }

        internal override void Execute()
        {
            ExternalServiceId externalDrawerServiceId = ExternalServices.BuiltInExternalServices.DirectContext3DService;
            var externalDrawerService = ExternalServiceRegistry.GetService(externalDrawerServiceId) as MultiServerService;
            if (externalDrawerService == null)
                return;

            foreach (var registeredServerId in externalDrawerService.GetRegisteredServerIds())
            {
                //var server = externalDrawerService.GetServer(registeredServerId) as GeometryDrawServer;
                //if (server == null)
                //    continue;

                externalDrawerService.RemoveServer(registeredServerId);
            }

            this.uiApp.ActiveUIDocument.UpdateAllOpenViews();

            // 窗口重置
            MainWindow.Instance.OverlayLegendGrid.Children.Clear();
            MainWindow.Instance.Height = 300;
            MainWindow.Instance.scrollViewer.Height = 0;

            //GeometryDrawServersMgr.ClearAllServers();
            // throw new NotImplementedException();

        }

    }
}
