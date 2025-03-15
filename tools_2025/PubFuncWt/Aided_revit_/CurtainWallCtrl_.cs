using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace PubFuncWt
{
    public class CurtainWallCtrl_
    {
        internal Wall CurtainWall;
        internal Dictionary<string, CurtainGridLineCtrl_> GridLineCtrl_s = new Dictionary<string, CurtainGridLineCtrl_>();
        internal Dictionary<string, Element> Panels = new Dictionary<string, Element>();//key: transform hash

        internal Frame3d Frame3d;

        internal CurtainWallCtrl_(Wall _cw)
        {
            this.CurtainWall = _cw;
            GetGrids();
            GetFrame3d();
        }
        /// <summary>
        /// 与普通墙体不同==对幕墙进行旋转180 or 镜像，操作后，orientation的变换情况一致
        /// 因此判断幕墙是否镜像，需判断里面的嵌板mirrored属性
        /// </summary>
        internal void GetFrame3d()
        {
            Curve curve = CurtainWall.LocationCurve();
            if (curve is Line)
            {
                Line line = curve as Line;
                Vector3d origin = line.Origin.ToVector3d();// 墙的原点
                Vector3d x = line.Direction.ToVector3d();// 墙的绘制方向
                Vector3d y = CurtainWall.Orientation.ToVector3d();// 墙外侧投影的法向量
                Vector3d z = x.Cross(y).Normalized;// 叉积符合右手定则

                this.Frame3d = new Frame3d(origin, x, y, z);
            }
            else if (curve is Arc)
            {
                Arc arc = curve as Arc;
                Vector3d origin = arc.GetEndPoint(0).ToVector3d();// 墙的原点
                Vector3d x = arc.ComputeDerivatives(0,true).BasisX.ToVector3d();// 墙的绘制方向
                Vector3d y = CurtainWall.Orientation.ToVector3d();// 墙外侧投影的法向量
                Vector3d z = x.Cross(y).Normalized;// 叉积符合右手定则

                this.Frame3d = new Frame3d(origin, x, y, z);
            }

        }
        public static string ToStringDigits(Transform _transform, int _digits)
        {
            string s = _transform.Origin.ToStringDigits(_digits);

            //s += _transform.BasisX.ToStringDigits(_digits) + "||";
            //s += _transform.BasisY.ToStringDigits(_digits) + "||";
            //s += _transform.BasisZ.ToStringDigits(_digits) + "||";
            //s += _transform.Scale.ToStringDigits(_digits);
            return s;
        }
        internal void GetPanels()
        {
            //get all panels into dic.
            //Use transform as key.       
            var doc = this.CurtainWall.Document;
            var ids = this.CurtainWall.CurtainGrid.GetPanelIds();
            foreach (var id in ids)
            {
                var panelElem = doc.GetElement(id);
                Transform tr = null;
                if (panelElem is Panel)
                {
                    var panel = panelElem as Panel;
                    tr = panel.Transform;
                }
                else if (panelElem is FamilyInstance)
                {
                    var fi = panelElem as FamilyInstance;
                    tr = fi.GetTotalTransform();
                }
                //if (this.CurtainWall.Flipped)
                //    tr.BasisY *= -1;
                this.Panels[ToStringDigits(tr, 2)] = panelElem;
            }
        }

        private string getStartEndKey(Curve _c)
        {
            return
            _c.GetEndPoint(0).ToStringDigits(2)
            + "||"
            + _c.GetEndPoint(1).ToStringDigits(2);
        }
        internal void GetMullions()
        {
            //get all mullions into grid line ctrl.
            //Find grid segment by testing base line stays within bound.
            //Use grid segment line as key.
            //Note that currently Revit API does not support
            //adding mullion on curtain wall borders. Will ignore such cases.
            var doc = this.CurtainWall.Document;
            var ids = this.CurtainWall.CurtainGrid.GetMullionIds();
            foreach (var id in ids)
            {
                var mullion = doc.GetElement(id) as Mullion;
                var mullionLine = mullion.LocationCurve as Line;
                if (mullionLine != null)
                {
                    foreach (var glc in this.GridLineCtrl_s.Values)
                    {
                        bool found = false;
                        foreach (var pair in glc.SegmentCurves)
                        {
                            var key = pair.Key;
                            var sc = pair.Value;
                            var segmentLine = Line.CreateBound(sc.GetEndPoint(0), sc.GetEndPoint(1));
                            //mullionLine.IsInside(segmentLine)
                            if (IsInside(mullionLine, segmentLine, 0.1640))
                            {
                                glc.Mullions[key] = mullion;
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                }
            }
        }
        internal static bool IsInside(Line _line, Line _testLine, double tolerance)
        {
            //end points both on test line
            var p0 = _line.GetEndPoint(0);
            var p1 = _line.GetEndPoint(1);
            var _p0 = _testLine.GetEndPoint(0);
            var _p1 = _testLine.GetEndPoint(1);

            double d1 = p0.DistanceTo(_p0);
            double d2 = p0.DistanceTo(_p1);
            //p0离_p0更近
            if (Math.Abs(d1) < Math.Abs(d2))
            {
                if (d1 < tolerance && p1.DistanceTo(_p1) < tolerance)
                {
                    return true;
                }
            }
            else
            {
                if (d2 < tolerance && p1.DistanceTo(_p0) < tolerance)
                {
                    return true;
                }
            }
            return false;
        }

        internal void GetGrids()
        {
            //get grids into dic.
            //Use start and end point as key.
            var doc = this.CurtainWall.Document;
            var ids = this.CurtainWall.CurtainGrid.GetUGridLineIds().ToList();
            ids.AddRange(this.CurtainWall.CurtainGrid.GetVGridLineIds());
            foreach (var id in ids)
            {
                var grid = doc.GetElement(id) as CurtainGridLine;
                var glCtrl = new CurtainGridLineCtrl_(grid);
                var key = glCtrl.GetKey();
                this.GridLineCtrl_s[key] = glCtrl;
            }
        }

        internal void GetGridSegments()
        {
            foreach (var glCtrl in this.GridLineCtrl_s.Values)
            {
                glCtrl.GetSegments();
            }
        }
    }

}
