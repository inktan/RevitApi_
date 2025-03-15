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


namespace InfoStrucFormwork
{
    internal class ClearStruAna : RequestMethod
    {
        internal ClearStruAna(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            ElementClassFilter filter = new ElementClassFilter(typeof(Floor));
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);
            foreach (var item in collector.WherePasses(filter).ToElements())
            {
                if (!(item is Floor)) continue;
                try
                {
                    using (Transaction _trans = new Transaction(this.doc, "清楚楼板结构分析"))
                    {
                        _trans.Start();
                        _trans.DeleteErrOrWaringTaskDialog();
                        //顶部偏移
                        (item as Floor).get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).Set(0);
                        _trans.Commit();
                    }
                }
                catch (Exception)
                {
                    continue;
                    //throw;
                }
            }

            filter = new ElementClassFilter(typeof(Wall));
            collector = new FilteredElementCollector(this.doc);
            foreach (var item in collector.WherePasses(filter).ToElements())
            {
                if (!(item is Wall)) continue;
                try
                {
                    using (Transaction _trans = new Transaction(this.doc, "清楚墙结构分析"))
                    {
                        _trans.Start();
                        _trans.DeleteErrOrWaringTaskDialog();
                        //顶部偏移
                        (item as Wall).get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0);
                        _trans.Commit();
                    }
                }
                catch (Exception)
                {
                    continue;
                    //throw;
                }
            }

            filter = new ElementClassFilter(typeof(FamilyInstance));
            collector = new FilteredElementCollector(this.doc);

            foreach (var item in collector.WherePasses(filter).ToElements())
            {
                if (!(item is FamilyInstance fi)) continue;

                string faName = fi.Symbol.FamilyName;
                if (faName.Contains("梁") || faName.Contains("柱") || faName.Contains("墙"))
                {
                    try
                    {
                        using (Transaction _trans = new Transaction(this.doc, "清楚梁柱墙的结构分析"))
                        {
                            _trans.Start();
                            _trans.DeleteErrOrWaringTaskDialog();
                            //顶部偏移
                            fi.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(0);
                            _trans.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //throw;
                    }
                }
            }
        }
    }
}
