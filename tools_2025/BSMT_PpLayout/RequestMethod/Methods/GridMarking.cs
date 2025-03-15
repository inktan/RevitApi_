using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using PubFuncWt;
using g3;
using goa.Common;
using goa.Common.g3InterOp;
using goa.Revit.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using QuadTrees;
using System.Drawing;
using goa.Common.UserOperation;

namespace BSMT_PpLayout
{

    class GridMarking : RequestMethod
    {
        public GridMarking(UIApplication uiapp) : base(uiapp)
        {
        }
        internal override void Execute()
        {
            QuadTreeRectF<QTreeRevitEleCtrl> qtree = new QuadTreeRectF<QTreeRevitEleCtrl>();
            IList<Element> elements = new FilteredElementCollector(this.doc, this.view.Id).OfCategory(BuiltInCategory.OST_DetailComponents).WhereElementIsNotElementType().ToElements();

            foreach (var item in elements)
            {
                if (item.Name.Contains("柱子_"))
                {
                    RectangleF rectangleF = item.get_BoundingBox(this.view).ToRectangleF();
                    RevitEleCtrl revitEleCtrl = new RevitEleCtrl(item);
                    QTreeRevitEleCtrl qTreeRevitEleCtrl = new QTreeRevitEleCtrl(rectangleF, revitEleCtrl);
                    qtree.Add(qTreeRevitEleCtrl);
                }
            }
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();
            List<ElementId> elementIds = new List<ElementId>();

            // 借助 DirectContext3D 
            GeometryDrawServersMgr.ClearAllServers();

            // 这里使用划闭合多段线的方式，进行选择
            MulPointsSelection selection_ = new MulPointsSelection(this.uiDoc);
            selection_.PickPolygon();
            List<Vector2d> regionPoints = selection_.regionPoints.Points.ToVector2ds().ToList();
            Polygon2d polygon2d = new Polygon2d(regionPoints);

            List<QTreeRevitEleCtrl> qTreeRevitEleCtrls = qtree.GetObjects(polygon2d.ToRectangle());

            foreach (var item in qTreeRevitEleCtrls)
            {
                Polygon2d temp = item.Rect.ToPolygon2d();
                if (polygon2d.Contains(temp) || polygon2d.Intersects(temp))
                {
                    ElementId elementId = item.RevitEleCtrl.Id;
                    if (!elementIds.Contains(item.RevitEleCtrl.Id))
                    {
                        familyInstances.Add(item.RevitEleCtrl.Ele as FamilyInstance);
                        elementIds.Add(elementId);
                    }
                }
            }


            List<Vector2d> vector2ds = familyInstances.Select(p => p.Location as LocationPoint).Select(p => p.Point).ToVector2ds().ToList();

            // 当前选择柱子所在矩形空间
            Polygon2d rectangle = new Polygon2d(vector2ds).GetBounds().ToPolygon();
            Vector2d location = this.sel.PickPoint().ToVector2d();

            List<Segment2d> segment2ds = rectangle.SegmentItr().ToList();
            Segment2d bottom = segment2ds[0];
            Segment2d right = segment2ds[1];

            List<Line> lines = new List<Line>();

            Transform transform = Transform.CreateRotation(new XYZ(0, 0, 1), Math.PI / 2);
            XYZ tempOffsetDirection = GetDirection(familyInstances);// 尺寸参照的方向
            XYZ direction = transform.OfVector(tempOffsetDirection);// 尺寸定位线的方向

            if (tempOffsetDirection.X < 0)
            {
                tempOffsetDirection *= -1;
            }
            if (direction.Y < 0)
            {
                direction *= -1;
            }

            if (bottom.Length > right.Length)// 水平边长
            {
                double length = Math.Abs(location.y - rectangle.Center().y) + 50;

                int side = bottom.WhichSide(location); //右为 - 1 左为 1 上为 1 下为 - 1
                if (side == 1)//上
                {
                    lines = vector2ds.Select(p => p.Pedal(location, direction.ToVector2d())).DelDuplicate(0.5).Select(p => Line.CreateBound(p.ToXYZ(), p.ToXYZ() + (-1) * (length) * tempOffsetDirection)).ToList();
                }
                else if (side == -1)// 下
                {
                    lines = vector2ds.Select(p => p.Pedal(location, direction.ToVector2d())).DelDuplicate(0.5).Select(p => Line.CreateBound(p.ToXYZ(), p.ToXYZ() + (length) * tempOffsetDirection)).ToList();
                }
            }
            else if (bottom.Length < right.Length)// 竖直边长
            {
                double length = Math.Abs(location.x - rectangle.Center().x) + 50;

                int side = right.WhichSide(location);
                if (side == 1)// 左
                {
                    lines = vector2ds.Select(p => p.Pedal(location, direction.ToVector2d())).DelDuplicate(0.5).Select(p => Line.CreateBound(p.ToXYZ(), p.ToXYZ() + (length) * tempOffsetDirection)).ToList();
                }
                else if (side == -1)// 右
                {
                    lines = vector2ds.Select(p => p.Pedal(location, direction.ToVector2d())).DelDuplicate(0.5).Select(p => Line.CreateBound(p.ToXYZ(), p.ToXYZ() + (-1) * (length) * tempOffsetDirection)).ToList();
                }
            }

            if (lines.Count > 0)
            {
                using (Transaction trans = new Transaction(doc, "添加塔楼间距标注"))
                {
                    trans.Start();
                    lines.ForEach(p =>
                    {
                        Grid grid = Grid.Create(this.doc, p);
                    });
                    trans.Commit();
                }
            }

            // 清除3D服务器
            GeometryDrawServersMgr.ClearAllServers();
        }
        internal XYZ GetDirection(List<FamilyInstance> familyInstances)
        {
            List<XYZ> hands = familyInstances.Select(p => p.HandOrientation).ToList();
            XYZ xYZ01 = hands.GroupBy(x => x).OrderBy(x => x.Count()).Last().Key;
            // 基于一个点做相互垂直的两根辅助线
            // 判断所有点到辅助线的距离
            // 距离大的辅助线的方向，即为尺寸线的辅助线方向
            List<XYZ> origins = familyInstances.Select(p => p.LocationPoint()).ToList();
            XYZ origin = origins.First();
            Line line01 = Line.CreateBound(origin + 1000 * xYZ01, origin - 1000 * xYZ01);
            double dis01 = origins.Select(p => line01.Distance(p)).OrderBy(p => p).Last();

            Transform transform = Transform.CreateRotation(new XYZ(0, 0, 1), Math.PI / 2);
            XYZ xYZ02 = transform.OfVector(xYZ01);
            Line line02 = Line.CreateBound(origin + 1000 * xYZ02, origin - 1000 * xYZ02);
            double dis02 = origins.Select(p => line02.Distance(p)).OrderBy(p => p).Last();


            if (dis01 > dis02)
            {
                return xYZ01;
            }
            else
            {
                return xYZ02;
            }
        }
    }
}
