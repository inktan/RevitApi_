using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using goa.Common.Exceptions;

namespace goa.Common.Exceptions
{
    public class DocSnapShotNullException : CommonUserExceptions
    {
        public override string Message
        {
            get
            {
                return "请扫描主体图元。";
            }
        }
    }
}

namespace goa.Common
{
    /// <summary>
    /// for host-search when create new instances.
    /// </summary>
    public class HostSearchSession
    {
        public static double HostSearchRange =
    UnitUtils.ConvertToInternalUnits(900, DisplayUnitType.DUT_MILLIMETERS);

        private Document Doc;
        private DesignOption designOption;
        private HostCategory HostCategory;
        private BoundingBoxIntersectsFilter Filter;
        public static double FaceSampleCell = UnitUtils.ConvertToInternalUnits(900, DisplayUnitType.DUT_MILLIMETERS);
        public static int NumNN = 1000;

        // hosting elements
        private XYZ hostDir;
        private List<Element> Hosts;
        private List<BoundingBoxXYZ> HostBBs;
        /// <summary>
        /// hosts' centroids.
        /// </summary>
        public List<XYZ> HostCoords;
        private KDTreeXYZ HostTree;

        /// <summary>
        /// planar face. 
        /// </summary>
        private List<PlanarFace> Faces = new List<PlanarFace>();
        private List<int> hostFaceMap = new List<int>();
        private XYZ faceNorm;
        /// <summary>
        /// sample points on hosts' faces. 
        /// Multiple points on one face
        /// to work with large face.
        /// Need to use index map to find owner face.
        /// </summary>
        public List<XYZ> FaceSampleCoords;
        /// <summary>
        /// tree of face coords.
        /// </summary>
        private KDTreeXYZ FaceSampleCoordsTree;
        /// <summary>
        /// sample point's owner face.
        /// </summary>
        private List<int> FaceSampleCoordsMap;

        /// <summary>
        /// setup a one-the-go host search.
        /// </summary>
        /// <param name="_doc">document to be searched inside</param>
        /// <param name="_hostCat">host category</param>
        /// <param name="_bbFilter">host bounding box intersects filter</param>
        /// <param name="_hostDir">host's direction. Either the same or the inverse. Could be Null.</param>
        /// <param name="_faceNorm">host face's normal. Cannot be inversed. Skip face search if Null.</param>
        /// <param name="_designOption">If null, will search main model and primary options. Otherwise search inside option and main model.</param>
        /// <param name="_numNN">optional. KNN tree search num of results.</param>
        public HostSearchSession
            (Document _doc,
            HostCategory _hostCat,
            BoundingBoxIntersectsFilter _bbFilter,
            XYZ _hostDir,
            XYZ _faceNorm,
            DesignOption _designOption)
        {
            this.Doc = _doc;
            this.HostCategory = _hostCat;
            this.Filter = _bbFilter;
            this.hostDir = _hostDir;
            this.faceNorm = _faceNorm;
            this.designOption = _designOption;
            this.scan();
        }
        private void scan()
        {
            this.Hosts = getHosts();

            this.HostBBs = this.Hosts
                .Select(x => x.GetBoundingBoxInModelCS(null))
                .ToList();
            this.HostCoords = this.HostBBs.
                Select(x => x.GetCentroid())
                .ToList();
            this.HostTree = new KDTreeXYZ(this.HostCoords);

            if (this.faceNorm != null)
            {
                for (int i = 0; i < this.Hosts.Count; i++)
                {
                    var host = this.Hosts[i];
                    var solids = host.GetAllSolids();
                    var faces = solids
                        .SelectMany(x => x.Faces.Cast<Face>())
                        .Where(f => f is PlanarFace)
                        .Cast<PlanarFace>()
                        .Where(x => x.FaceNormal.IsAlmostEqualToByDifference(this.faceNorm, 0.0001));
                    foreach (var f in faces)
                    {
                        if (f.Reference == null
                            || f.Reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_NONE)
                            continue;
                        this.Faces.Add(f);
                        this.hostFaceMap.Add(i);
                    }
                }

                getFaceCoordsMap(this.Faces, FaceSampleCell, out this.FaceSampleCoordsMap, out this.FaceSampleCoords);
                this.FaceSampleCoordsTree = new KDTreeXYZ(this.FaceSampleCoords);
            }
        }
        /// <summary>
        /// Face need to contain that point, 
        /// and parallel to both hand and facing dir. 
        /// (otherwise Revit will force rollback.)
        /// Return NULL if not found.
        /// </summary>
        public Face FindHostFace
            (XYZ _pos, out Element _host)
        {
            _host = null;
            if (this.Faces.Count == 0)
                return null;
            //get knn
            var knn = this.FaceSampleCoordsTree.SearchIndicesByCoord(_pos, NumNN);
            var planarFaces = new List<PlanarFace>();
            //get mapped back indices, remove duplicates
            var indices = knn.Select(x => this.FaceSampleCoordsMap[x]);
            indices = indices.Distinct();

            //search among knn
            List<int> hostIndices = new List<int>();
            foreach (var i in indices)
            {
                var f = this.Faces[i];

                //do not support curved face
                if (f is PlanarFace == false)
                    continue;
                //if face is invalid, will throw exception
                testFaceValidity(f);

                var pf = f as PlanarFace;
                planarFaces.Add(pf);
                hostIndices.Add(hostFaceMap[i]);

                var proj = f.Project(_pos);
                if (proj != null && proj.Distance.IsAlmostEqualByDifference(0))
                {
                    //perfect match
                    var hostIndex = this.hostFaceMap[i];
                    _host = this.Hosts[hostIndex];
                    return f;
                }
            }
            //if none of the faces matchs orientation
            //prompt user
            if (planarFaces.Count == 0)
                throw new CommonUserExceptions("目标区域碎面较多，未能找到合适的附着面。操作被取消。");

            // Perfect match not found?
            // User might have picked a line that's slightly off. 
            // Return the closest planar face instead.
            _host = this.Hosts[hostIndices.First()];
            return planarFaces.First();
        }
        /// <summary>
        /// First, try finding a face whose plane contains the line
        /// if not found, return the planar face with closest centroid.
        /// </summary>
        public PlanarFace FindHostFace
            (Line _line, out Element _host)
        {
            _host = null;
            if (this.Faces.Count == 0)
                return null;
            //get knn
            var center = _line.Evaluate(0.5, true);
            var knn = this.FaceSampleCoordsTree.SearchIndicesByCoord(center, NumNN);
            //get mapped back indices, remove duplicates
            var indices = knn.Select(x => this.FaceSampleCoordsMap[x]);
            indices = indices.Distinct();
            //search among knn for containment face
            double minDot = double.MaxValue;
            PlanarFace hostF = null;
            int hostIndex = 0;
            foreach (var i in indices)
            {
                var f = this.Faces[i];
                if (f is PlanarFace == false)
                    continue;
                var pf = f as PlanarFace;
                //if face is invalid, will throw exception
                testFaceValidity(pf);

                double distDot = (center - pf.Origin).DotProduct(pf.FaceNormal);
                double absDot = Math.Abs(distDot);
                if (absDot < minDot &&
                    absDot.IsAlmostEqualByDifference(minDot) == false)
                {
                    hostF = pf;
                    minDot = absDot;
                    hostIndex = hostFaceMap[i];

                    //break loop if perfect match
                    if (minDot.IsAlmostEqualByDifference(0))
                        break;
                }
            }
            _host = this.Hosts[hostIndex];
            return hostF;
        }
        /// <summary>
        /// Try find wall that contains at least on of the points.
        /// If not found, return the closest wall.
        /// </summary>
        public Wall FindHostWall(List<XYZ> _posInsideWall)
        {
            Wall first = null;
            var knn = this.HostTree.SearchIndicesByCoord(_posInsideWall.First(), NumNN);
            foreach (int i in knn)
            {
                var bb = this.HostBBs[i];
                if (_posInsideWall.Any(x => bb.IsInside(x)))
                {
                    return this.Hosts[i] as Wall;
                }
                if (first == null)
                    first = this.Hosts[i] as Wall;
            }
            return first;
        }
        private static bool parallel(PlanarFace pf, XYZ hand, XYZ facing)
        {
            var handDot = pf.FaceNormal.DotProduct(hand);
            var facingDot = pf.FaceNormal.DotProduct(facing);
            return handDot.IsAlmostEqualByDifference(0)
                && facingDot.IsAlmostEqualByDifference(0);
        }
        private static void testFaceValidity(Face _f)
        {
            try
            {
                double a = _f.Area;
            }
            catch
            {
                throw new goa.Common.Exceptions.DocSnapShotNullException();
            }
        }
        private List<Element> getHosts()
        {
            LogicalOrFilter dof = null;
            var mainModelFilter = new ElementDesignOptionFilter(ElementId.InvalidElementId);
            //if search main model, search main options as well
            if (this.designOption == null)
            {
                var primaryDopFilter = new PrimaryDesignOptionMemberFilter();
                dof = new LogicalOrFilter(mainModelFilter, primaryDopFilter);
            }
            else
            {
                var activeDoFilter = new ElementDesignOptionFilter(DesignOption.GetActiveDesignOptionId(this.Doc));
                dof = new LogicalOrFilter(activeDoFilter, mainModelFilter);
            }
            if (this.HostCategory == HostCategory.Wall)
            {
                var wallCollector = new FilteredElementCollector(this.Doc)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(Wall))
                    .WherePasses(this.Filter)
                    .WherePasses(dof)
                    .Cast<Wall>()
                    .Where(x => x.WallType.Kind == WallKind.Basic);
                if (this.hostDir != null)
                    wallCollector = wallCollector
                        .Where(x => x.LocationCurve() is Line
                        && x.LocationLine().Direction.IsParallel(this.hostDir));
                return wallCollector
                    .Cast<Element>()
                    .ToList();
            }
            else if (this.HostCategory == HostCategory.Floor)
            {
                var floorCollector = new FilteredElementCollector(this.Doc)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(Floor))
                    .WherePasses(this.Filter)
                    .WherePasses(dof);
                return floorCollector
                    .Cast<Element>().ToList();
            }
            else
            {
                var bic = this.HostCategory == HostCategory.EnvelopFamily
                    ? BuiltInCategory.OST_GenericModel
                    : BuiltInCategory.OST_StructuralFoundation;
                IEnumerable<Element> collector = new FilteredElementCollector(this.Doc)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(bic)
                    .WherePasses(this.Filter)
                    .WherePasses(dof);
                if (this.hostDir != null)
                    collector = collector
                        .Cast<FamilyInstance>()
                        .Where(x => x.HandOrientation.IsParallel(this.hostDir))
                        .Cast<Element>();
                if (this.HostCategory == HostCategory.EnvelopFamily)
                    return collector
                        .Where(x => goaCustomFamilyFilter.IsEnviFamilyInstanceElem(x))
                        .ToList();
                else if (this.HostCategory == HostCategory.SuperSkin)
                    return collector
                        .Where(x => goaCustomFamilyFilter.IsSuperSkinInstance(x))
                        .ToList();
                else if (this.HostCategory == HostCategory.SuperCornice)
                    return collector
                        .Where(x => goaCustomFamilyFilter.IsSuperCorniceElem(x))
                        .ToList();
                else if (this.HostCategory == HostCategory.ArrayLineBased)
                    return collector
                        .Where(x => goaCustomFamilyFilter.IsArrayLineBasedElem(x))
                        .ToList();
                else
                    return null;
            }
        }
        private static void getFaceCoordsMap(IList<PlanarFace> _faces, double _faceSampleCell, out List<int> _sampleCoordsMap, out List<XYZ> _sampleCoords)
        {
            _sampleCoordsMap = new List<int>();
            _sampleCoords = new List<XYZ>();
            for (int i = 0; i < _faces.Count; i++)
            {
                var f = _faces[i];
                var coords = getFaceCoords(f, _faceSampleCell);
                _sampleCoords.AddRange(coords);
                for (int j = 0; j < coords.Count; j++)
                    _sampleCoordsMap.Add(i);
            }
        }
        private static List<XYZ> getFaceCoords(Face _face, double _faceSampleCell)
        {
            var vertices = _face.GetVertices();
            var bb = vertices.GetBoundingBox();
            //projection matrix
            double step = _faceSampleCell;
            List<XYZ> list = new List<XYZ>() { _face.GetCentroid() };
            var edges = _face.GetEdgesAsCurveLoops().SelectMany(x => x.Cast<Curve>());
            list.AddRange(edges.SelectMany(x => x.DivideByDist(step, true).Select(tf => tf.Origin)));
            list.AddRange(_face.GetXYZMatrixOnFace(step).Keys);
            return list;
        }
    }
}
