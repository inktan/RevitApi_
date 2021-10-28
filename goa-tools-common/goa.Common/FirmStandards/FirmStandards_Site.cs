using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common
{
    public static partial class FirmStandards
    {
        #region 总图
        public static Dictionary<string, SitePlanFamilyType> SitePlanFamilyTypeID
            = new Dictionary<string, SitePlanFamilyType>()
            {
                {"SITE-HIGHRISE", SitePlanFamilyType.Block_Highrise }, //高层
                {"SITE-STACK", SitePlanFamilyType.Block_Stack }, //叠拼
                {"SITE-CALCPOINT", SitePlanFamilyType.CalcPoint }, //计算点
                {"SITE-SUNPATH", SitePlanFamilyType.SunPath }, //棒影图
                {"SITE-CODE-DISTANCE", SitePlanFamilyType.DistanceCheck }, //间距控制
            };

        public static List<Guid> SitePlanFamilyParam = new List<Guid>()
        {
                new Guid("111e1c2e-5bbe-4ad6-b493-cfd669fd9901"), //0, 总图体块-总建筑面积
                new Guid("8e371b1f-313e-4143-899f-30d59c28e25c"), //1, 总图体块-层数
                new Guid("e88dbee5-7ce4-46be-8f1f-e32699c18e1e"), //2, 住宅体块-体块族类型
                new Guid("33bfe04f-9c97-4c0e-a2fb-4abde4dd059e"), //3, 【类型】指标_标准层面积
        };

        public static bool IsSiteBlockFamily(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            var type = SitePlanFamilyType.Block_Highrise;
            return IsSiteBlockFamilySymbol(fi.Symbol, ref type);
        }
        public static bool IsSiteBlockFamily(Element _elem, ref SitePlanFamilyType _type)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            return IsSiteBlockFamilySymbol(fi.Symbol, ref _type);
        }
        public static bool IsSiteBlockFamilySymbol(FamilySymbol _fs)
        {
            SitePlanFamilyType type = SitePlanFamilyType.Block_Highrise;
            return IsSiteBlockFamilySymbol(_fs, ref type);
        }
        public static bool IsSiteBlockFamilySymbol(FamilySymbol _fs, ref SitePlanFamilyType _type)
        {
            var p = _fs.get_Parameter(FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var typeString = p.AsString();
            if (typeString == null
                || SitePlanFamilyTypeID.ContainsKey(typeString) == false)
                return false;
            _type = SitePlanFamilyTypeID[typeString];
            return
                _type == SitePlanFamilyType.Block_Highrise
                || _type == SitePlanFamilyType.Block_Stack;
        }
        public static SitePlanFamilyType GetSitePlanFamilyType(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return SitePlanFamilyType.INVALID;
            var fi = _elem as FamilyInstance;
            return GetSitePlanFamilyType(fi.Symbol);
        }
        public static SitePlanFamilyType GetSitePlanFamilyType(FamilySymbol _fs)
        {
            var p = _fs.get_Parameter(FamilyTypeIDForAPI);
            if (p == null)
                return SitePlanFamilyType.INVALID;
            var typeString = p.AsString();
            if (typeString == null
                || SitePlanFamilyTypeID.ContainsKey(typeString) == false)
                return SitePlanFamilyType.INVALID;
            else
                return SitePlanFamilyTypeID[typeString];
        }
        public static SitePlanFamilyType GetSitePlanFamilyType(Family _fam)
        {
            var doc = _fam.Document;
            var firstFSId = _fam.GetFamilySymbolIds().FirstOrDefault();
            if (firstFSId == null)
                return SitePlanFamilyType.INVALID;
            var fs = doc.GetElement(firstFSId) as FamilySymbol;
            return GetSitePlanFamilyType(fs);
        }
        #endregion
    }
    public enum SitePlanFamilyType
    {
        Block_Highrise,
        Block_Stack,
        CalcPoint,
        SunPath,
        DistanceCheck,
        INVALID,
    }
}
