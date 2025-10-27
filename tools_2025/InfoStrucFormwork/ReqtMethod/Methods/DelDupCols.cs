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
    internal class DelDupCols : RequestMethod
    {
        public DelDupCols(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<FamilyInstance> cols = collector.WherePasses(filter).ToElements().Where(p => p is FamilyInstance).Select(p => p as FamilyInstance).Where(p => p.Symbol.FamilyName.Contains("柱")).ToList();

            //cols.Count.ToString().TaskDialogErrorMessage();

            // 2. 按位置分组
            var groups = GroupBeamsByLocation(cols);

            using (Transaction trans = new Transaction(doc, "删除重复梁"))
            {
                trans.Start();

                int deletedCount = 0;
                foreach (var group in groups)
                {
                    if (group.Value.Count > 1)
                    {
                        // 保留第一个实例，删除其他重复实例
                        for (int i = 1; i < group.Value.Count; i++)
                        {
                            doc.Delete(group.Value[i].Id);
                            deletedCount++;
                        }
                    }
                }

                trans.Commit();

                TaskDialog.Show("完成", $"共删除 {deletedCount} 个重复柱");
            }

        }

        // 按位置分组梁实例
        private Dictionary<string, List<FamilyInstance>> GroupBeamsByLocation(List<FamilyInstance> cols)
        {
            var groups = new Dictionary<string, List<FamilyInstance>>();
            const double positionTolerance = 0.03; // 10mm位置容差 1英尺= 0.3048米
            const double heightOverlap = 0.03;      // 10mm高度重叠 1英尺= 0.3048米

            foreach (FamilyInstance col in cols)
            {
                if (col.Location is LocationPoint locPoint)
                {
                    XYZ position = locPoint.Point;
                    string key = FindSimilarPositionGroup(groups, col, position, positionTolerance, heightOverlap);

                    if (key == null)
                    {
                        // 新位置组
                        key = $"{Math.Round(position.X, 3)}_{Math.Round(position.Y, 3)}";
                        groups[key] = new List<FamilyInstance> { col };
                    }
                    else
                    {
                        // 添加到现有组
                        groups[key].Add(col);
                    }
                }
            }

            return groups;
        }

        // 查找相似位置组
        private string FindSimilarPositionGroup(Dictionary<string, List<FamilyInstance>> groups, FamilyInstance col, XYZ position, double positionTolerance, double heightOverlap)
        {
            foreach (var group in groups)
            {
                // 获取组中第一个梁的位置
                XYZ firstPosition = ((LocationPoint)group.Value[0].Location).Point;

                // 检查XY平面位置是否接近
                double xyDistance = new XYZ(position.X, position.Y, 0).DistanceTo(new XYZ(firstPosition.X, firstPosition.Y, 0));

                if (xyDistance <= positionTolerance)
                {
                    // 检查Z轴高度重叠
                    double z1Start = GetBeamZStart(group.Value[0]);
                    double z1End = GetBeamZEnd(group.Value[0]);

                    double z2Start = GetBeamZStart(col);
                    double z2End = GetBeamZEnd(col);

                    // 计算高度重叠
                    double overlap = Math.Min(z1End, z2End) - Math.Max(z1Start, z2Start);

                    if (overlap >= heightOverlap)
                    {
                        return group.Key;
                    }
                }
            }

            return null;
        }

        // 获取梁的起始高度
        private double GetBeamZStart(FamilyInstance beam)
        {
            BoundingBoxXYZ bbox = beam.get_BoundingBox(null);
            return bbox.Min.Z;
        }

        // 获取梁的结束高度
        private double GetBeamZEnd(FamilyInstance beam)
        {
            BoundingBoxXYZ bbox = beam.get_BoundingBox(null);
            return bbox.Max.Z;
        }

    }
}

