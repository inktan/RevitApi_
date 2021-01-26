using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

using goa.Common.Exceptions;

namespace goa.Common
{
    /// <summary>
    /// Line-based family instance has some unique behavior pattern. 
    /// Some API methods need to be used with care, others cannot be 
    /// used directly, otherwise crash Revit process.
    /// Methods here should be checked and updated, and all addin should use these methods
    /// when trying to apply transform operation on line-based family instance.
    /// All methods need open transaction.
    /// </summary>
    public static class LineBasedFamilyTransformUtils
    {
        /// <summary>
        /// Copy line-based family instances to multiple levels.
        /// </summary>
        /// <param name="_refs">line-based family instances to be copied.</param>
        /// <param name="_tarLevels">target levels</param>
        /// <param name="_baseLevel">base level for calculating height change.</param>
        /// <param name="_snapShot">snap shot of potential host faces. Need to scan before calling this method.</param>
        /// <returns>Line-based family creators of correct type.</returns>
        public static List<FamilyCreator> CopyToLevels
            (FamilyInstance _refFI,
            IEnumerable<Level> _tarLevels,
            Level _baseLevel,
            double _searchRange)
        {
            var doc = _refFI.Document;
            var dop = doc.GetElement(DesignOption.GetActiveDesignOptionId(doc)) as DesignOption;
            var ops = new List<FamilyCreator>();
            var refLocLine = _refFI.LocationLine();
            //get params
            var param = _refFI.GetAllEditableParams();
            //get orientation
            var facing = _refFI.FacingOrientation;
            var faceNormal = _refFI.GetPlane().Normal;
            //scan for host face
            var hostCat = HostUtils.GetHostCategory(_refFI);
            var hostDir = HostUtils.GetHostDir(_refFI);
            var bbRef = _refFI.GetBoundingBoxForSolidGeometries();
            if (bbRef == null)
                bbRef = _refFI.GetBoundingBoxInModelCS(null);
            bool cutsHost = HostUtils.CutsHost(_refFI);
            foreach (var tarLevel in _tarLevels)
            {
                //skip base level
                if (tarLevel.Id == _baseLevel.Id)
                    continue;
                var deltaZ = tarLevel.ProjectElevation - _baseLevel.ProjectElevation;
                var tf = Transform.CreateTranslation(new XYZ(0, 0, deltaZ));
                var tarLocLine = refLocLine.CreateTransformed(tf) as Line;
                //check host dir
                FamilyCreator ctr = null;
                var info = new LineBasedFamilyInfo(_refFI);
                //host on level
                if (hostCat == HostCategory.Level)
                {
                    ctr = new LineBasedFamilyCreator_onLevel
                        (_refFI.Symbol,
                        tarLocLine,
                        _refFI.FacingOrientation,
                        tarLevel,
                        _refFI.StructuralType,
                        param,
                        null);
                }
                //host on face
                else
                {
                    //search host face
                    var bbTar = bbRef.GetTransformed(tf);
                    var filter = Methods.GetBBIntersectFilter(bbTar, _searchRange);
                    var search = new HostSearchSession(doc, hostCat, filter, hostDir, faceNormal, dop);
                    Element host;
                    var hostFace = search.FindHostFace(tarLocLine, out host);
                    if (hostFace == null)
                        throw new HostNotFoundException(_refFI.Id.ToString());
                    var refOffsetLocLine = LineBasedFamilyUtils.GetOffsetLocLine(_refFI);
                    var tarOffsetLocLine = refOffsetLocLine.CreateTransformed(tf) as Line;
                    var lineBasedCtr = new LineBasedFamilyCreator_onPlanarFace
                        (_refFI.Symbol,
                          tarOffsetLocLine,
                          facing,
                          host,
                          hostFace,
                          param,
                          null);
                    lineBasedCtr.CutsHost = cutsHost;
                    ctr = lineBasedCtr;
                }
                ops.Add(ctr);
            }
            return ops;
        }
        public static FamilyCreator MirrorFaceBased
            (FamilyInstance _fi, Plane _plane, double _hostSearchRange)
        {
            var tf = Transform.CreateReflection(_plane);
            var doc = _fi.Document;
            var dop = doc.GetElement(DesignOption.GetActiveDesignOptionId(doc)) as DesignOption;
            //host search
            var bb = _fi.GetBoundingBoxForSolidGeometries();
            if (bb == null)
                bb = _fi.GetBoundingBoxInModelCS(null);
            bb = bb.GetTransformed(tf);
            var filter = Methods.GetBBIntersectFilter(bb, _hostSearchRange);
            var hostCat = HostUtils.GetHostCategory(_fi);
            var hostDir = HostUtils.GetHostDir(_fi);
            if (hostDir != null)
                hostDir = tf.OfVector(hostDir);

            var refOffsetLocLine = LineBasedFamilyUtils.GetOffsetLocLine(_fi);
            var tarOffsetLocLine = refOffsetLocLine.CreateTransformed(tf) as Line;
            var refLocLine = _fi.LocationLine();
            var tarLocLine = refLocLine.CreateTransformed(tf) as Line;
            var facing = tf.OfVector(_fi.FacingOrientation);
            var faceNormal = tf.OfVector(_fi.GetPlane().Normal);

            var search = new HostSearchSession(doc, hostCat, filter, hostDir, faceNormal, dop);
            Element host;
            var hostFace = search.FindHostFace(tarLocLine, out host);
            if (hostFace == null)
                throw new HostNotFoundException(_fi.Id.ToString());

            var param = _fi.GetAllEditableParams();
            var ctr = new LineBasedFamilyCreator_onPlanarFace
                (_fi.Symbol, tarOffsetLocLine, facing, host, hostFace, param, null);
            ctr.CutsHost = HostUtils.CutsHost(_fi);
            return ctr;
        }
    }
}
