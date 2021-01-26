using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using goa.Common.Exceptions;

namespace goa.Common
{
    public static class FaceBasedFamilyTransformUtils
    {
        /// <summary>
        /// Copy face-based family instances to multiple levels.
        /// </summary>
        /// <param name="_refs">face-based family instances to be copied.</param>
        /// <param name="_tarLevels">target levels</param>
        /// <param name="_baseLevel">base level for calculating height change.</param>
        /// <param name="_snapShot">snap shot of potential host faces. Need to scan before calling this method.</param>
        /// <returns>face-based family creators of correct type.</returns>
        public static List<FamilyCreator> CopyToLevels
            (FamilyInstance refFI,
            IEnumerable<Level> _tarLevels,
            Level _baseLevel,
            double _searchRange)
        {
            var doc = refFI.Document;
            var ops = new List<FamilyCreator>();
            var refPos = refFI.GetPos();
            var dop = doc.GetElement(DesignOption.GetActiveDesignOptionId(doc)) as DesignOption;
            //get params
            var param = refFI.GetAllEditableParams();
            //get orientation
            var hand = refFI.HandOrientation;
            var facing = refFI.FacingOrientation;
            var faceNormal = refFI.GetPlane().Normal;
            //search cat
            var bbRef = refFI.GetBoundingBoxForSolidGeometries();
            if (bbRef == null)
                bbRef = refFI.GetBoundingBoxInModelCS(null);
            var searchCat = HostUtils.GetHostCategory(refFI);
            var hostDir = HostUtils.GetHostDir(refFI);
            foreach (var tarLevel in _tarLevels)
            {
                //skip base level
                if (tarLevel.Id == _baseLevel.Id)
                    continue;
                var deltaZ = tarLevel.ProjectElevation - _baseLevel.ProjectElevation;
                var tf = Transform.CreateTranslation(new XYZ(0, 0, deltaZ));
                var tarPos = tf.OfPoint(refPos);

                //search host face
                var bbTar = bbRef.GetTransformed(tf);
                var bbFilter = Methods.GetBBIntersectFilter(bbTar, _searchRange);
                var search = new HostSearchSession(doc, searchCat, bbFilter, hostDir, faceNormal, dop);
                Element host;
                var hostFace = search.FindHostFace(tarPos, out host);
                if (hostFace == null)
                    throw new HostNotFoundException(refFI.Id.ToString());

                var ctr = new FaceBasedFamilyCreator
                    (refFI.Symbol,
                    host,
                    hostFace,
                    tarPos,
                    hand,
                    facing,
                    param,
                    null);
                ops.Add(ctr);
            }
            return ops;
        }
        /// <summary>
        /// TRANSACTIONS INSIDE. 
        /// Create mirror ONLY FOR vertical host face. 
        /// </summary>
        public static FamilyCreator Mirror
            (FamilyInstance fi, Plane _plane, double _hostSearchRange)
        {
            var doc = fi.Document;
            var dop = doc.GetElement(DesignOption.GetActiveDesignOptionId(doc)) as DesignOption;
            var tf = Transform.CreateReflection(_plane);
            var refPos = fi.GetPos();
            var tarPos = tf.OfPoint(refPos);
            var hand = tf.OfVector(fi.HandOrientation);
            var facing = tf.OfVector(fi.FacingOrientation);
            var refPlane = fi.GetPlane();
            var faceNormal = tf.OfVector(refPlane.Normal);
            //find host face
            var bb = fi.GetBoundingBoxForSolidGeometries();
            if (bb == null)
                bb = fi.GetBoundingBoxInModelCS(null);
            bb = bb.GetTransformed(tf);
            var bbFilter = Methods.GetBBIntersectFilter(bb, _hostSearchRange);
            var hostCat = HostUtils.GetHostCategory(fi);
            var hostDir = HostUtils.GetHostDir(fi);
            var search = new HostSearchSession(doc, hostCat, bbFilter, hostDir, faceNormal, dop);
            Element host;
            var hostFace = search.FindHostFace(tarPos, out host);
            if (hostFace == null)
                throw new HostNotFoundException(fi.Id.ToString());
            var param = fi.GetAllEditableParams();
            var ctr = new FaceBasedFamilyCreator
                (fi.Symbol, host, hostFace, tarPos, hand, facing, param, null);
            return ctr;
        }
    }
}
