using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common;

namespace FAKE_DIMS
{
    public class DimCtrl
    {
        public Dimension Dim;

        private List<ElementId> familyInstances = new List<ElementId>();
        private Document doc;

        public DimCtrl(Dimension _dim)
        {
            this.Dim = _dim;
            this.doc = _dim.Document;
        }

        public void ChangeToFamilyInstance(Family _fa, View _ownerView)
        {
            var endPointsList = this.getEndPoints();
            var _fs = getSymbol(_fa);
            //check symbol is active
            if (!_fs.IsActive) _fs.Activate(); ;
            foreach(var pair in endPointsList)
            {
                if (pair[0].DistanceTo(pair[1]) < 0.01)
                    continue;
                Line line = null;
                try
                {
                    line = Line.CreateBound(pair[0], pair[1]);
                }
                catch { continue; }
                var fi = this.doc.Create.NewFamilyInstance(line, _fs, _ownerView);
                this.familyInstances.Add(fi.Id);
            }
        }

        public void SetTextToCenter()
        {
            foreach (var id in this.familyInstances)
            {
                var fi = this.doc.GetElement(id);
                var pL = fi.GetParameterByName("长度");
                var pD = fi.GetParameterByName("文字横向距离");
                pD.Set(pL.AsDouble() * 0.5);
            }
        }

        public void DeleteOriginalDim()
        {
            this.doc.Delete(this.Dim.Id);
        }

        private FamilySymbol getSymbol(Family _fa)
        {
            var doc = _fa.Document;
            var symbols = _fa.GetFamilySymbolIds()
                .Select(x => doc.GetElement(x) as FamilySymbol)
                .ToList();
            string gridDimKey = "G-DIMS-GRID";
            string compDimKey = "A-DIMS-ANNO";
            foreach(var fs in symbols)
            {
                if(fs.Name.Contains(gridDimKey) 
                    && this.Dim.Name.Contains(gridDimKey))
                {
                    return fs;
                }
                if(fs.Name.Contains(compDimKey)
                    && this.Dim.Name.Contains(compDimKey))
                {
                    return fs;
                }
            }
            return symbols.First();
        }

        private List<XYZ[]> getEndPoints()
        {
            List<XYZ[]> list = new List<XYZ[]>();
            if(this.Dim.NumberOfSegments == 0)
            {
                list.Add(getEndPoints(this.Dim));
            }
            else
            {
                var line = this.Dim.Curve as Line;
                var dir = line.Direction;
                foreach(DimensionSegment seg in this.Dim.Segments)
                {
                    list.Add(getEndPoints(seg, dir));
                }
            }
            //end points could be outside view plane (cause unknown)
            //get view plane, return projections
            var view = this.doc.GetElement(this.Dim.OwnerViewId) as View;
            var plane = view.GetViewBasePlane();
            if (plane == null)
                return list;
            for(int i = 0;i<list.Count;i++)
            {
                list[i][0] = plane.ProjectOnto(list[i][0]);
                list[i][1] = plane.ProjectOnto(list[i][1]);
            }
            return list;
        }

        private XYZ[] getEndPoints(Dimension _dim)
        {
            var line = _dim.Curve as Line;
            var dir = line.Direction;
            //reverse -1 directions
            if(dir.IsAlmostEqualToByDifference(new XYZ(-1,0,0), 0.0001)
                || dir.IsAlmostEqualToByDifference(new XYZ(0,-1,0), 0.0001)
                || dir.IsAlmostEqualToByDifference(new XYZ(0,0,-1), 0.0001))
            {
                dir = -1 * dir;
            }
            var length = _dim.Value;
            var origin = _dim.Origin;
            var p0 = _dim.Origin - dir * (double)length * 0.5;
            var p1 = _dim.Origin + dir * (double)length * 0.5;
            return new XYZ[2] { p0, p1 };
        }

        private XYZ[] getEndPoints(DimensionSegment _seg, XYZ _direction)
        {
            var length = _seg.Value;
            var dir = _direction;
            //reverse -1 directions
            if (dir.IsAlmostEqualToByDifference(new XYZ(-1, 0, 0), 0.0001)
                || dir.IsAlmostEqualToByDifference(new XYZ(0, -1, 0), 0.0001)
                || dir.IsAlmostEqualToByDifference(new XYZ(0, 0, -1), 0.0001))
            {
                dir = -1 * dir;
            }
            var origin = _seg.Origin;
            var p0 = _seg.Origin - dir * (double)length * 0.5;
            var p1 = _seg.Origin + dir * (double)length * 0.5;
            return new XYZ[2] { p0, p1 };
        }

        public List<GeometryObject>  TEST_GetPos()
        {
            if(this.Dim.Segments.Size == 0)
            {
                return getPos(this.Dim);
            }
            else
            {
                List<GeometryObject> list = new List<GeometryObject>();
                foreach (DimensionSegment seg in this.Dim.Segments)
                    list.AddRange(getPos(seg));
                return list;
            }
        }

        private List<GeometryObject> getPos(Dimension _dim)
        {
            List<GeometryObject> geom = new List<GeometryObject>();
            geom.Add(createArc(_dim.LeaderEndPosition));
            geom.Add(createArc(_dim.Origin));
            geom.Add(createArc(_dim.TextPosition));
            return geom;
        }

        private List<GeometryObject> getPos(DimensionSegment _dim)
        {
            List<GeometryObject> geom = new List<GeometryObject>();
            geom.Add(createArc(_dim.LeaderEndPosition));
            geom.Add(createArc(_dim.Origin));
            geom.Add(createArc(_dim.TextPosition));
            return geom;
        }

        private Arc createArc(XYZ _c)
        {
            return Arc.Create(_c, Math.PI, 0, Math.PI, XYZ.BasisX, XYZ.BasisY);
        }
    }
}
