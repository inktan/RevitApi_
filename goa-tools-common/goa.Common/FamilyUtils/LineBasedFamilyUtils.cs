using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public static class LineBasedFamilyUtils
    {
        public static Line GetOffsetLocLine(FamilyInstance _fi)
        {
            var hand = _fi.HandFlipped ? _fi.HandOrientation * -1.0 : _fi.HandOrientation;
            var facing = _fi.FacingFlipped ? _fi.FacingOrientation * -1.0 : _fi.FacingOrientation;
            var norm = hand.CrossProduct(facing);
            var basePlane = Plane.CreateByNormalAndOrigin(norm, _fi.GetPos());
            var instanceOffset = _fi.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).AsDouble();
            var symbolOffset = GetSymbolOffset(_fi.Symbol);
            var profileOffset = GetProfileOffset(_fi.Symbol);
            var totalOffset = instanceOffset + symbolOffset + profileOffset;
            var tf = Transform.CreateTranslation(totalOffset * basePlane.Normal);
            var locLine = _fi.LocationLine();
            //extension at start and end?
            double startExt = GetStartExtension(_fi, true);
            double endExt = GetStartExtension(_fi, false);
            var exLine = locLine.ExtendOneEndByDist(-1.0 * startExt, 0);
            exLine = exLine.ExtendOneEndByDist(endExt, 1);
            return exLine.CreateTransformed(tf) as Line;
        }
        public static double GetSymbolOffset(FamilySymbol _fs)
        {
            var symbolOffsetP = _fs.get_Parameter(FirmStandards.SuperFacadeFamParam[11]);
            var symbolOffset = symbolOffsetP == null
                ? 0.0
                : symbolOffsetP.AsDouble();
            return symbolOffset;
        }
        public static double GetProfileOffset(FamilySymbol _fs)
        {
            var profileOffsetP = _fs.GetParameters("minY").FirstOrDefault();
            var profileOffset = profileOffsetP == null
                ? 0.0
                : profileOffsetP.AsDouble();
            return profileOffset;
        }
        public static double GetStartExtension(FamilyInstance _fi, bool _start)
        {
            var pKey = _start
                ? FirmStandards.SuperFacadeFamParam[12]
                : FirmStandards.SuperFacadeFamParam[13];
            var p = _fi.get_Parameter(pKey);
            return p == null
                ? 0.0 : p.AsDouble();
        }
        public static Dictionary<string, ParameterEditRecorder> GetAllEditableParams(this FamilyInstance _fi)
        {
            var dic = new Dictionary<string, ParameterEditRecorder>();
            foreach (Parameter p in _fi.Parameters)
            {
                string id = p.GetId();
                if (FirmStandards.ParametersNeedSync.Contains(id))
                {
                    var pr = new ParameterEditRecorder(p, p.GetValue());
                    dic[p.GetId()] = pr;
                }
            }
            return dic;
        }
    }
}
