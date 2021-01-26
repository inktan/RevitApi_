using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common.Exceptions;

namespace goa.Common
{
    public enum HostingType
    {
        Unknown,
        LineBasedOnFace,
        LineBasedOnLevel,
        WallBased,
        FaceBased,
        LevelBased,
    }
    public enum HostCategory
    {
        none,
        Level,
        Wall,
        Floor,
        SuperSkin,
        EnvelopFamily,
        SuperCornice,
        ArrayLineBased,
    }

    public static class HostUtils
    {
        public static HostingType GetHostingType(FamilyInstance fi)
        {
            var lineBased = fi.Location is LocationCurve
                && fi.LocationCurve() is Line;
            if (lineBased)
            {
                if (fi.HostFace != null)
                    return HostingType.LineBasedOnFace;
                else if (fi.Host is Level)
                    return HostingType.LineBasedOnLevel;
                else
                    return HostingType.Unknown;
            }
            else
            {
                if (fi.Host == null)
                    return HostingType.Unknown;
                else if (fi.HostFace != null)
                {
                    return HostingType.FaceBased;
                }
                else if (fi.Host is Wall)
                {
                    return HostingType.WallBased;
                }
                else if (fi.Host is Level)
                {
                    return HostingType.LevelBased;
                }
                else
                    return HostingType.Unknown;
            }
        }
        public static HostCategory GetHostCategory(FamilyInstance fi)
        {
            if (fi.Host is Wall)
                return HostCategory.Wall;
            else if (fi.Host is Level)
                return HostCategory.Level;
            else if (fi.Host is Floor)
                return HostCategory.Floor;
            else if (fi.Host is FamilyInstance)
            {
                if (goaCustomFamilyFilter.IsSuperSkinInstance(fi.Host))
                    return HostCategory.SuperSkin;
                else if (goaCustomFamilyFilter.IsEnviFamilyInstanceElem(fi.Host))
                    return HostCategory.EnvelopFamily;
                else if (goaCustomFamilyFilter.IsSuperCorniceElem(fi.Host))
                    return HostCategory.SuperCornice;
            }
            return HostCategory.none;
        }
        public static XYZ GetHostDir(FamilyInstance fi)
        {
            var host = fi.Host;
            if (host == null)
                return null;
            else if (host.Location is LocationCurve)
            {
                var lc = host.LocationCurve();
                if (lc is Line)
                {
                    return host.LocationLine().Direction;
                }
                else
                {
                    return null;
                }
            }
            else if (host is FamilyInstance)
            {
                var hostFi = host as FamilyInstance;
                return hostFi.HandOrientation;
            }
            else
                return null;
        }
        public static List<XYZ> GetPointsInsideWallHost(FamilyInstance _fi)
        {
            if (goaCustomFamilyFilter.IsEnviFamilyInstanceElem(_fi))
            {
                var pos = _fi.GetPos();
                double width = _fi.get_Parameter(FirmStandards.EnviFamParam[7]).AsDouble();
                double height = _fi.Symbol.get_Parameter(FirmStandards.EnviFamParam[8]).AsDouble();
                var hand = _fi.HandOrientation;
                var facing = _fi.FacingOrientation;
                var cross = hand.CrossProduct(facing);
                if (_fi.HandFlipped)
                    cross *= -1.0;
                if (_fi.FacingFlipped)
                    cross *= -1.0;
                var p0 = pos - hand * 0.5 * width;
                var p1 = p0 + hand * width;
                var p2 = p1 + cross * height;
                var p3 = p0 + cross * height;
                var center = (p0 + p2) * 0.5;
                return new List<XYZ>() { center, p0, p1, p2, p3 };
            }
            else
            {
                return new List<XYZ>() { _fi.GetBoundingBoxInModelCS(null).GetCentroid() };
            }
        }
        public static XYZ GetHostNormal(FamilyInstance _lineBased)
        {
            var hand = _lineBased.HandOrientation;
            var facing = _lineBased.FacingOrientation;
            var cross = hand.CrossProduct(facing);
            if (_lineBased.HandFlipped)
                cross *= -1.0;
            if (_lineBased.FacingFlipped)
                cross *= -1.0;
            var norm = _lineBased.IsWorkPlaneFlipped
                ? cross * -1.0
                : cross;
            return norm;
        }
        public static void FixInstanceFaceProblem(Element _host)
        {
            var doc = _host.Document;
            var anyWall = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(Wall))
                .FirstOrDefault(x => ((Wall)x).WallType.Kind == WallKind.Basic
                    && x.SameDesignOption(_host));
            if (anyWall == null)
                return;
            else if (JoinGeometryUtils.AreElementsJoined(doc, _host, anyWall) == false)
            {
                try
                {
                    JoinGeometryUtils.JoinGeometry(doc, anyWall, _host);
                    JoinGeometryUtils.UnjoinGeometry(doc, anyWall, _host);
                }
                catch { }
            }
        }
        public static List<List<ElementId>> GetHostLevelList(IEnumerable<Element> _elems, Document _doc)
        {
            var dic = GetHostLevelMap(_elems, _doc);
            var count = dic.Values.OrderByDescending(x => x).First() + 1;
            var array = new List<ElementId>[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = new List<ElementId>();
            }
            foreach (var id in dic.Keys)
            {
                var depth = dic[id];
                array[depth].Add(id);
            }
            return array.ToList();
        }
        public static Dictionary<ElementId, int> GetHostLevelMap(IEnumerable<Element> _elems, Document _doc)
        {
            var dic = new Dictionary<ElementId, int>();
            var ids = new List<ElementId>();
            foreach (var elem in _elems)
            {
                if (elem != null && elem.IsValidObject)
                    ids.Add(elem.Id);
                else
                    ids.Add(ElementId.InvalidElementId);
            }
            //max of 5 levels of hosting depth
            for (int i = 0; i < 5; i++)
            {
                getHostLevelSingleLoop(_doc, ids, dic);
            }
            return dic;
        }
        public static bool CutsHost(FamilyInstance _fi)
        {
            if (_fi.Host == null)
                return false;
            var doc = _fi.Document;
            return InstanceVoidCutUtils.InstanceVoidCutExists(_fi.Host, _fi);
        }
        public static void CutUncutHost(FamilyInstance _fi, bool _cut)
        {
            if (_fi.Host == null)
                return;
            bool currCut = HostUtils.CutsHost(_fi);
            if (_cut == currCut)
                return;
            var doc = _fi.Document;
            if (_cut)
                InstanceVoidCutUtils.AddInstanceVoidCut(doc, _fi.Host, _fi);
            else
                InstanceVoidCutUtils.RemoveInstanceVoidCut(doc, _fi.Host, _fi);
        }
        private static void getHostLevelSingleLoop(Document _doc, IEnumerable<ElementId> _elemIds, Dictionary<ElementId, int> _dic)
        {
            var hashset = new HashSet<ElementId>(_elemIds);
            foreach (var id in _elemIds)
            {
                var elem = _doc.GetElement(id);
                if (elem == null)
                    continue;

                //starts with 0 depth
                if (_dic.ContainsKey(elem.Id) == false)
                    _dic[elem.Id] = 0;
                //if has host, and is one of fis
                //use the host's depth +1
                if (elem is FamilyInstance)
                {
                    var fi = elem as FamilyInstance;
                    if (fi.Host != null && hashset.Contains(fi.Host.Id))
                    {
                        int hostDepth = 0;
                        bool b = _dic.TryGetValue(fi.Host.Id, out hostDepth);
                        if (b)
                            _dic[fi.Id] = hostDepth + 1;
                        else
                            _dic[fi.Id] = 1;
                    }
                }
            }
        }
    }
}
