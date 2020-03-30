using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;

namespace WorkSetSharedLevelsGrids
{
    public class CMD : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            GridLevelUpdater updater = new GridLevelUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);

            ElementClassFilter gridFilter = new ElementClassFilter(typeof(Grid));
            ElementClassFilter levelFilter = new ElementClassFilter(typeof(Level));
            IList<ElementFilter> elementFilters = new List<ElementFilter>();
            elementFilters.Add(gridFilter);
            elementFilters.Add(levelFilter);
            LogicalOrFilter orFilter = new LogicalOrFilter(elementFilters);

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), orFilter, Element.GetChangeTypeElementAddition());

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            GridLevelUpdater updater = new GridLevelUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }
    }
    public class GridLevelUpdater : IUpdater
    {
        static AddInId m_appid;
        static UpdaterId m_updaterId;

        public GridLevelUpdater(AddInId id)
        {
            m_appid = id;
            m_updaterId = new UpdaterId(m_appid, Guid.NewGuid());
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            foreach (ElementId addedElementId in data.GetAddedElementIds())
            {
                Element elem = doc.GetElement(addedElementId);
                Parameter wsparam = elem.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                //find all user's workset
                FilteredWorksetCollector worksets = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);
                if (worksets != null)
                {
                    foreach (Workset ws in worksets)
                    {
                        if (ws.Name == "共享标高和轴网" || ws.Name == "Shared Levels and Grids")
                        {
                            wsparam.Set(ws.Id.IntegerValue);
                        }
                    }
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return " To a WorkSet";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.GridsLevelsReferencePlanes;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Grid Or Level Type Updater";
        }
    }
}
