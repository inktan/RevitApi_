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
    internal class CalRelativeH : RequestMethod
    {
        internal CalRelativeH(UIApplication _uiApp) : base(_uiApp)
        {

        }

        internal override void Execute()
        {

            List<Level> levels =
                (new FilteredElementCollector(CMD.Doc))
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);

            foreach (var item in collector.WherePasses(filter).ToElements())
            {
                if (!(item is FamilyInstance fi)) continue;

                string faName = fi.Symbol.FamilyName;
                if (faName.Contains("梁"))
                {
                    //参照标高
                    ElementId levelId = fi.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                    Level refLevel = this.doc.GetElement(levelId) as Level;
                    double levH = refLevel.Elevation;
                    //起点标高偏移
                    double end01OffsetValue = fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
                    //终点标高偏移
                    double end02OffsetValue = fi.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();

                    double endOffsetValue = end01OffsetValue;
                    if (end01OffsetValue> end02OffsetValue)
                    {
                        endOffsetValue = end02OffsetValue;
                    }

                    //Z轴偏移值
                    double zOffsetValue = fi.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).AsDouble();
                    //高度
                    double height = fi.LookupParameter("高度").AsDouble();
                    //底部高程
                    double beamBotHeight = levH + endOffsetValue + zOffsetValue - height;

                    Level level = levels.OrderBy(p => Math.Abs(beamBotHeight - p.Elevation)).FirstOrDefault();
                    if (level == null) continue;

                    string relativeH = Math.Round((beamBotHeight - level.Elevation).FeetToMilliMeter()).ToString();

                    try
                    {
                        //这里使用共享参数 结构-梁-梁底相对偏移
                        using (Transaction _trans = new Transaction(this.doc, "修改共享参数"))
                        {
                            _trans.Start();
                            _trans.DeleteErrOrWaringTaskDialog();
                            fi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(relativeH);
                            try
                            {
                                fi.LookupParameter("梁底相对偏移").Set(beamBotHeight - level.Elevation);
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
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



            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
