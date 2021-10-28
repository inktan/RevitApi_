using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

using goa.Common;

namespace goa.Revit.DirectContext3D
{
    /// <summary>
    /// This class manages geometry draw servers. 
    /// Servers are linked to document and addins that register them. 
    /// Call Init(), Update() or Clear() method.
    /// Register and unregister server must be called from valid Revit API context.
    /// Server's geometry and other data can be changed from modeless form directly, but not from other thread, or timer event, etc.
    /// </summary>
    public static class GeometryDrawServersMgr
    {
        public static List<GeometryDrawServer> Servers = new List<GeometryDrawServer>();

        #region Initialize and Exit
        static bool initialized = false;
        /// <summary>
        /// Call this method when launch your addin.
        /// </summary>
        public static void Init(UIApplication _uiapp)
        {
            if (initialized)
                return;
            APP.UIApp = _uiapp;
            registerEvents(true);
            Servers = new List<GeometryDrawServer>();

            initialized = true;
        }

        /// <summary>
        /// Unregister servers by document and addin. 
        /// Call this method when close an addin.
        /// </summary>
        /// <param name="_currentDoc">could be null.</param>
        /// <param name="_addinId">could be null.</param>
        public static void ClearServers(Document _currentDoc, string _addinId)
        {
            unregisterServers(_currentDoc, _addinId, true);
        }

        /// <summary>
        /// unregister all servers, regardless of document and addin.
        /// </summary>
        public static void ClearAllServers()
        {
            unregisterServers(null, null, true);
        }

        private static bool registered = false;
        private static void registerEvents(bool _register)
        {
            if (registered == _register)
                return;

            var app = APP.UIApp.Application;
            if (_register)
            {
                app.DocumentClosing += OnDocumentClosing;
            }
            else
            {
                app.DocumentClosing -= OnDocumentClosing;
            }
            registered = _register;
        }

        /// <summary>
        /// Implements the OnDocumentClosing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            unregisterServers(args.Document, null, false);
        }

        /// <summary>
        /// Cleans up by unregistering the servers corresponding to the specified document, 
        /// or all servers if the document is not provided.
        /// </summary>
        /// <param name="document">The document whose servers should be removed, or null.</param>
        /// <param name="updateViews">Update views of the affected document(s).</param>
        private static void unregisterServers(Document document, string _addinId, bool updateViews)
        {
            ExternalServiceId externalDrawerServiceId = ExternalServices.BuiltInExternalServices.DirectContext3DService;
            var externalDrawerService = ExternalServiceRegistry.GetService(externalDrawerServiceId) as MultiServerService;
            if (externalDrawerService == null)
                return;

            foreach (var registeredServerId in externalDrawerService.GetRegisteredServerIds())
            {
                var server = externalDrawerService.GetServer(registeredServerId) as GeometryDrawServer;
                if (server == null)
                    continue;
                if (!sameDoc(document, server) || !sameAddin(_addinId, server))
                    continue;
                externalDrawerService.RemoveServer(registeredServerId);
            }

            Servers.RemoveAll(server => sameAddin(_addinId, server) && sameDoc(document, server));
            if (updateViews)
            {
                UIDocument uidoc = APP.UIApp.ActiveUIDocument;
                if (uidoc != null)
                    uidoc.UpdateAllOpenViews();
            }
        }

        /// <summary>
        /// Unregister this server. Will check if is registered before proceed.
        /// </summary>
        /// <param name="_server"></param>
        public static void UnregisterServer(GeometryDrawServer _server)
        {
            ExternalServiceId externalDrawerServiceId = ExternalServices.BuiltInExternalServices.DirectContext3DService;
            var externalDrawerService = ExternalServiceRegistry.GetService(externalDrawerServiceId) as MultiServerService;
            if (externalDrawerService == null)
                return;
            else
            {
                var id = _server.GetServerId();
                if (externalDrawerService.IsRegisteredServerId(id))
                    externalDrawerService.RemoveServer(id);
            }
        }
        #endregion

        private static bool sameDoc(Document _doc, GeometryDrawServer _server)
        {
            return _doc == null || _doc.Equals(_server.Document);
        }

        private static bool sameAddin(string _addinId, GeometryDrawServer _server)
        {
            return _addinId == null || _server.AddinId == _addinId;
        }

        /// <summary>
        /// Call this method to display geometry on screen. 
        /// Call UIDocument.RefreshActiveView() to display geometry. 
        /// The servers will be automaticly created and registered.
        /// </summary>
        /// <param name="_serverInputs"></param>
        /// <param name="_addinId"></param>
        public static void ShowGraphics
            (GeometryDrawServerInputs _serverInputs, string _addinId, bool _clearServerOfSameAddin = true)
        {
            if (_serverInputs == null || _addinId == null)
                return; 

            //there is an un-documented limit on server buffer size.
            //need to split input into multiple ones, create one server for each,
            //to not to exceed that limit.
            var inputList = _serverInputs.SplitSelf();

            ShowGraphics(inputList, _addinId, _clearServerOfSameAddin);
        }

        /// <summary>
        /// Call this method to display geometry on screen. 
        /// Call UIDocument.RefreshActiveView() to display geometry.
        /// This method assumes each input is not exceeding limits of buffer size.
        /// If not sure, call
        /// </summary>
        /// <param name="_serverInputs"></param>
        /// <param name="_addinId"></param>
        public static void ShowGraphics
            (IList<GeometryDrawServerInputs> _serverInputs, string _addinId, bool _clearServerOfSameAddin = true)
        {
            if (_serverInputs == null || _addinId == null || _serverInputs.Count == 0)
                return;

            var uidoc = APP.UIApp.ActiveUIDocument;
            var doc = uidoc.Document;

            if (_clearServerOfSameAddin)
                ClearServers(doc, _addinId);

            foreach (var input in _serverInputs)
            {
                var server = CreateAndRegisterServer(uidoc, _addinId);
                //get geometry into server, set update
                server.Inputs = input;
                server.InputChanged = true;
            }
        }

        /// <summary>
        /// Create and register server to Revit external service registry.
        /// Will check if same server already registered. 
        /// Can only be called from valid API context.
        /// </summary>
        public static GeometryDrawServer CreateAndRegisterServer(UIDocument uidoc, string _addinId)
        {
            // Create the server and register it with the DirectContext3D service.
            var server = new GeometryDrawServer(uidoc, _addinId);
            RegisterServer(server);
            return server;
        }

        /// <summary>
        /// Register server to Revit external service registry.
        /// Will check if same server already registered. 
        /// Can only be called from valid API context.
        /// </summary>
        /// <param name="server"></param>
        public static void RegisterServer(GeometryDrawServer server)
        {
            ExternalService directContext3DService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            if (directContext3DService.IsRegisteredServerId(server.GetServerId()))
                return;

            directContext3DService.AddServer(server);

            Servers.Add(server);

            MultiServerService msDirectContext3DService = directContext3DService as MultiServerService;

            IList<Guid> serverIds = msDirectContext3DService.GetActiveServerIds();

            serverIds.Add(server.GetServerId());

            // Add the new server to the list of active servers.
            msDirectContext3DService.SetActiveServers(serverIds);
        }
    }
}
