using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;

namespace InfoStrucFormwork
{
    internal class DelDupBeams : RequestMethod
    {
        public DelDupBeams(UIApplication _uiApp) : base(_uiApp)
        {
        }
        private const double PositionTolerance = 0.01; // 10mm位置容差
        private const double OverlapThreshold = 0.1;   // 100mm重叠阈值
        internal override void Execute()
        {
            // 1. 收集所有梁族实例
            var beams = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(f => f.Symbol.FamilyName.Contains("梁"))
                .Where(f => f.Location is LocationCurve)
                .ToList();

            // 2. 按位置分组并删除重复梁
            int deletedCount = DeleteDuplicateBeams(beams);

            TaskDialog.Show("完成", $"共删除 {deletedCount} 个重复梁");

        }
        private int DeleteDuplicateBeams(List<FamilyInstance> beams)
        {
            var groups = GroupBeamsByLine(beams);
            int deletedCount = 0;

            using (Transaction trans = new Transaction(doc, "删除重复梁"))
            {
                trans.Start();

                foreach (var group in groups.Where(g => g.Value.Count > 1))
                {
                    // 按长度排序，保留最长的梁
                    var sortedBeams = group.Value.OrderByDescending(b => GetCurveLength(b)).ToList();

                    for (int i = 1; i < sortedBeams.Count; i++)
                    {
                        doc.Delete(sortedBeams[i].Id);

                        //this.sel.SetElementIds(new List<ElementId>() { sortedBeams[i].Id });

                        deletedCount++;
                    }
                }

                trans.Commit();
            }

            return deletedCount;
        }
        private Dictionary<string, List<FamilyInstance>> GroupBeamsByLine(List<FamilyInstance> beams)
        {
            var groups = new Dictionary<string, List<FamilyInstance>>();

            foreach (var beam in beams)
            {
                if (beam.Location is LocationCurve locCurve && locCurve.Curve is Line line)
                {
                    string key = FindOverlappingGroup(groups, line);

                    if (key == null)
                    {
                        // 新组
                        key = GenerateLineKey(line);
                        groups[key] = new List<FamilyInstance> { beam };
                    }
                    else
                    {
                        // 添加到现有组
                        groups[key].Add(beam);
                    }
                }
            }

            return groups;
        }
        private string FindOverlappingGroup(Dictionary<string, List<FamilyInstance>> groups, Line newLine)
        {
            foreach (var group in groups)
            {
                var firstBeam = group.Value.First();
                if (firstBeam.Location is LocationCurve firstLocCurve && firstLocCurve.Curve is Line existingLine)
                {
                    if (AreLinesOverlapping(existingLine, newLine))
                    {
                        return group.Key;
                    }
                }
            }
            return null;
        }
        private bool AreLinesOverlapping(Line line1, Line line2)
        {
            // 检查两条线段是否平行
            if (!line1.Direction.IsAlmostEqualTo(line2.Direction))
                return false;

            // 检查两条线段是否共线
            XYZ pointOnLine1 = line1.GetEndPoint(0);
            XYZ pointOnLine2 = line2.GetEndPoint(0);
            XYZ direction = line1.Direction;

            if (!IsPointOnLine(pointOnLine1, line2) && !IsPointOnLine(pointOnLine2, line1))
                return false;

            // 计算重叠部分
            double start1 = GetProjectedLength(line1.GetEndPoint(0), direction);
            double end1 = GetProjectedLength(line1.GetEndPoint(1), direction);
            double start2 = GetProjectedLength(line2.GetEndPoint(0), direction);
            double end2 = GetProjectedLength(line2.GetEndPoint(1), direction);

            double overlap = Math.Min(end1, end2) - Math.Max(start1, start2);

            return overlap >= OverlapThreshold;
        }
        private bool IsPointOnLine(XYZ point, Line line)
        {
            XYZ closestPoint = line.Project(point).XYZPoint;
            return point.DistanceTo(closestPoint) < PositionTolerance;
        }

        private double GetProjectedLength(XYZ point, XYZ direction)
        {
            return point.X * direction.X + point.Y * direction.Y + point.Z * direction.Z;
        }

        private string GenerateLineKey(Line line)
        {
            XYZ start = line.GetEndPoint(0);
            XYZ end = line.GetEndPoint(1);
            XYZ mid = (start + end) / 2;
            return $"{Math.Round(mid.X, 3)}_{Math.Round(mid.Y, 3)}_{Math.Round(mid.Z, 3)}";
        }

        private double GetCurveLength(FamilyInstance beam)
        {
            if (beam.Location is LocationCurve locCurve)
            {
                return locCurve.Curve.Length;
            }
            return 0;
        }

    }
}

