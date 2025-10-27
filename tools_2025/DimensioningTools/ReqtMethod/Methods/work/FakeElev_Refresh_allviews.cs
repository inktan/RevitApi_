using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
using goa.Common.Exceptions;
//using NetTopologySuite.Geometries;

namespace DimensioningTools
{
    internal class FakeElev_Refresh_allviews : RequestMethod
    {
        internal FakeElev_Refresh_allviews(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            LevelPickFilter levelpickfilter = new LevelPickFilter();
            Level level = doc.GetElement(sel.PickObject(ObjectType.Element, levelpickfilter)) as Level;
            double selevelelevation = level.Elevation;//请选择零零标高

            //获取视图ID列表
            ICollection<ElementId> ele_viewsectionIds = (new FilteredElementCollector(doc)).OfClass(typeof(ViewSection)).WhereElementIsNotElementType().ToElementIds();
            using (Transaction modifyParaElevation = new Transaction(doc))
            {
                modifyParaElevation.Start("modifyParaElevation");
                //遍历视图列表
                foreach (ElementId elementId in ele_viewsectionIds)
                {
                    FakeElev_Refresh_activeview.modifyElevationFake(doc, elementId, selevelelevation);//对单个视图id内的 假标高 族实例 进行数据修改
                }
                modifyParaElevation.Commit();
            }
        }

    }
}
