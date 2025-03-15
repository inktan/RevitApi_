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
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;


namespace InfoStrucFormwork
{
    internal class BbCoverFb : RequestMethod
    {
        internal BbCoverFb(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            //获取所有的梁
            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);

            List<FamilyInstance> beams = collector.WherePasses(filter).Select(p => p as FamilyInstance).Where(p => p.Symbol.FamilyName.Contains("梁")).ToList();

            //beams.Count.ToString().TaskDialogErrorMessage();

            //获取所有的楼板
            //filter = new ElementClassFilter(typeof(Floor));
            //collector = new FilteredElementCollector(this.doc);
            //List<Floor> floors = collector.WherePasses(filter).Select(p => p as Floor).ToList();

            //floors.Count.ToString().TaskDialogErrorMessage();

            // 1 手动选择当前楼板
            Reference fiReference = this.sel.PickObject(ObjectType.Element, new FloorFliter());
            Floor floor = this.doc.GetElement(fiReference) as Floor;
            // 2 获取楼板的实际高度值
            Level baseLevel = this.doc.GetElement(floor.LevelId) as Level;
            double baseLevelHeight = baseLevel.Elevation;
            // 3 底部偏移
            double baseOffset = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();
            // 4 底部定位高度
            double floorHeight = baseLevelHeight + baseOffset;
            // 楼板厚度
            double floorThickness = floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
            //获取楼板中心线所在高度
            double centerFloorHeight = floorHeight - floorThickness * 0.5;

            //5 提取楼板线圈
            ElementClassFilter curveFilter = new ElementClassFilter(typeof(CurveElement));
            List<ElementId> dependentIds = floor.GetDependentElements(curveFilter).ToList();
            var curves = dependentIds.Select(x => (this.doc.GetElement(x).Location as LocationCurve).Curve).ToList();
            List<Coordinate> points = new List<Coordinate>();
            foreach (var curve in curves)
            {
                var p0 = curve.GetEndPoint(0);
                //var p1 = curve.GetEndPoint(1);
                points.Add(new Coordinate(p0.X, p0.Y));
                //points.Add(new Coordinate(p1.X, p1.Y));
            }
            points.Add(points[0]);
            var shell = new LinearRing(points.ToArray());
            var polygon = new Polygon(shell);

            List<ElementId> elementIds = new List<ElementId>();
            foreach (var beam in beams)
            {
                try
                {
                    Line line = beam.LocationCurve() as Line;

                    var p0 = line.GetEndPoint(0);
                    var p1 = line.GetEndPoint(1);

                    Coordinate[] beam_points = { new Coordinate(p0.X, p0.Y), new Coordinate(p1.X, p1.Y) };
                    LineString beam_lineString = new LineString(beam_points);

                    // 7 判断多边形是否与直线相交
                    bool intersects = polygon.Intersects(beam_lineString);
                    if (intersects)
                    {
                        //8 判断楼板上下高度2m范围存在的梁
                        ElementId beamLevelId = beam.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                        Level beamLevel = this.doc.GetElement(beamLevelId) as Level;
                        double beamLevelHeight = beamLevel.Elevation;
                        //起点标高偏移
                        double end01OffsetValue = beam.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
                        //终点标高偏移
                        double end02OffsetValue = beam.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();

                        double endOffsetValue = end01OffsetValue;
                        if (end01OffsetValue > end02OffsetValue)
                        {
                            endOffsetValue = end02OffsetValue;
                        }
                        double z_offset_vslue = beam.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).AsDouble();
                        double beamThickness = beam.LookupParameter("高度").AsDouble();

                        double centerBeamHeight = beamLevelHeight + endOffsetValue + z_offset_vslue - beamThickness * 0.5;

                        //Math.Abs(centerFloorHeight - centerBeamHeight).ToString().TaskDialogErrorMessage();

                        if (Math.Abs(centerFloorHeight - centerBeamHeight).FeetToMilliMeter() < 1500)
                        {
                            if (Math.Abs(centerFloorHeight - centerBeamHeight) > Math.Abs(floorThickness - beamThickness) * 0.5 + 0.0051)
                            {
                                elementIds.Add(beam.Id);
                            }
                        }
                        //break;



                    }

                    //intersects.ToString().TaskDialogErrorMessage();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            this.sel.SetElementIds(elementIds);




            //$"{sw.ElapsedMilliseconds} + 毫秒".TaskDialogErrorMessage();
        }
    }
}
