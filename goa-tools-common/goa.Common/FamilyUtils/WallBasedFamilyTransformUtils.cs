using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using goa.Common.Exceptions;

namespace goa.Common
{
    public static class WallBasedFamilyTransformUtils
    {
        /// <summary>
        /// Copy wall-based family instances to multiple levels.
        /// </summary>
        /// <param name="_refs">wall-based family instances to be copied.</param>
        /// <param name="_tarLevels">target levels</param>
        /// <param name="_baseLevel">base level for calculating height change.</param>
        /// <param name="_snapShot">snap shot of potential host walls. Need to scan before calling this method.</param>
        /// <returns>wall-based family creators.</returns>
        public static List<FamilyCreator> CopyToLevels
            (FamilyInstance refFI,
            IEnumerable<Level> _tarLevels,
            Level _baseLevel,
            double _searchRange)
        {
            var doc = refFI.Document;
            var dop = doc.GetElement(DesignOption.GetActiveDesignOptionId(doc)) as DesignOption;
            var ops = new List<FamilyCreator>();
            var refPoints = HostUtils.GetPointsInsideWallHost(refFI);
            //get params
            var param = refFI.GetAllEditableParams();
            //get orientation
            var hand = refFI.HandOrientation;
            var facing = refFI.FacingOrientation;
            //search cat
            var bbRef = refFI.GetBoundingBoxForSolidGeometries();
            if (bbRef == null)
                bbRef = refFI.GetBoundingBoxInModelCS(null);
            var hostDir = HostUtils.GetHostDir(refFI);
            foreach (var tarLevel in _tarLevels)
            {
                //skip base level
                if (tarLevel.Id == _baseLevel.Id)
                    continue;
                var deltaZ = tarLevel.ProjectElevation - _baseLevel.ProjectElevation;
                var tf = Transform.CreateTranslation(new XYZ(0, 0, deltaZ));
                var tarPoints = refPoints.Select(x => tf.OfPoint(x)).ToList();

                //search host face
                var bbTar = bbRef.GetTransformed(tf);
                var bbFilter = Methods.GetBBIntersectFilter(bbTar, _searchRange);
                var search = new HostSearchSession(doc, HostCategory.Wall, bbFilter, hostDir, null, dop);
                var hostWall = search.FindHostWall(tarPoints);
                if (hostWall == null)
                    throw new HostNotFoundException(refFI.Id.ToString());

                var tarPos = tf.OfPoint(refFI.GetPos());
                var ctr = new WallBasedFamilyCreator
                    (refFI.Symbol,
                    hostWall,
                    tarPos,
                    hand, facing,
                    refFI.StructuralType,
                    param,
                    null);
                ops.Add(ctr);
            }
            return ops;
        }
        public static FamilyCreator Mirror
            (FamilyInstance refFI,
            Plane _mirrorPlane,
            double _hostSearchRange)
        {
            var doc = refFI.Document;
            var dop = doc.GetElement(DesignOption.GetActiveDesignOptionId(doc)) as DesignOption;
            var tf = Transform.CreateReflection(_mirrorPlane);
            var refPoints = HostUtils.GetPointsInsideWallHost(refFI);
            var tarPoints = refPoints.Select(x => tf.OfPoint(x)).ToList();
            //get params
            var param = refFI.GetAllEditableParams();
            //get orientation
            var hand = tf.OfVector(refFI.HandOrientation);
            var facing = tf.OfVector(refFI.FacingOrientation);
            //host search
            var refHostDir = HostUtils.GetHostDir(refFI);
            var hostDir = refHostDir == null
                ? null
                : tf.OfVector(refHostDir);
            var bb = refFI.GetBoundingBoxForSolidGeometries();
            if (bb == null)
                bb = refFI.GetBoundingBoxInModelCS(null);
            var bbMr = bb.GetTransformed(tf);
            var bbFilter = Methods.GetBBIntersectFilter(bbMr, 3.0);
            var search = new HostSearchSession(doc, HostCategory.Wall, bbFilter, hostDir, null, dop);
            var hostWall = search.FindHostWall(tarPoints);
            if (hostWall == null)
                throw new HostNotFoundException(refFI.Id.ToString());
            var tarPos = tf.OfPoint(refFI.GetPos());

            var ctr = new WallBasedFamilyCreator
                (refFI.Symbol,
                hostWall,
                tarPos,
                hand, facing,
                refFI.StructuralType,
                param,
                null);
            return ctr;
        }
    }
}
