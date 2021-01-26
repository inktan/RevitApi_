using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public static class goaCustomFamilyFilter
    {
        #region Envi Family
        public static bool IsEnviFamilyInstanceElem(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            var fs = fi.Symbol;
            return IsEnviFamilySymbol(fs);
        }
        public static bool IsEnviFamilySymbol(FamilySymbol fs)
        {
            var type = fs.GetEnviFamilyType();
            return type == EnviFamilyType.wb_BalcDoor
                 || type == EnviFamilyType.wb_BayWindow_B
                 || type == EnviFamilyType.wb_BayWindow_C
                 || type == EnviFamilyType.wb_Window_B
                 || type == EnviFamilyType.wb_Window_C
                 || type == EnviFamilyType.wb_Equipment;
        }
        public static bool IsEnviOpening(Element _elem)
        {
            if (_elem is FamilyInstance == false)
                return false;
            var fi = _elem as FamilyInstance;
            var type = fi.GetEnviFamilyType();
            return type == EnviFamilyType.wb_BalcDoor
                 || type == EnviFamilyType.wb_BayWindow_B
                 || type == EnviFamilyType.wb_BayWindow_C
                 || type == EnviFamilyType.wb_Window_B
                 || type == EnviFamilyType.wb_Window_C;
        }
        public static bool IsBayWindow(FamilyInstance _fi)
        {
            var type = _fi.GetEnviFamilyType();
            return type == EnviFamilyType.wb_BayWindow_B
                || type == EnviFamilyType.wb_BayWindow_C;
        }
        #endregion Envi Family

        #region Facade Family
        /// <summary>
        /// 线脚、表皮、开槽、阵列、开洞
        /// </summary>
        public static bool IsSuperFacadeFamily(Element elem)
        {
            return IsSuperForm(elem)
                    || IsSuperRevealElem(elem)
                    || IsMullionArrayElem(elem)
                    || IsRevealArrayElem(elem)
                    || IsArrayOnFaceElem(elem)
                    || IsArrayOnLevelElem(elem)
                    || IsVoidFaceBasedFamilyInstanceElem(elem);
        }
        /// <summary>
        /// 线脚、表皮
        /// </summary>
        public static bool IsSuperForm(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            var fs = fi.Symbol;
            var p = fs.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key != null
                && FirmStandards.FacadeFamilyTypeID.ContainsKey(key))
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.SuperSkin
                    || type == FacadeFamilyType.SuperCornice;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 表皮
        /// </summary>
        public static bool IsSuperSkinInstance(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            var fs = fi.Symbol;
            return IsSuperSkinSymbol(fs);
        }
        public static bool IsSuperSkinSymbol(FamilySymbol fs)
        {
            var p = fs.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key != null
                && FirmStandards.FacadeFamilyTypeID.ContainsKey(key))
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.SuperSkin;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 线脚
        /// </summary>
        public static bool IsSuperCorniceElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsSuperCorniceInstance(fi);
        }
        public static bool IsSuperCorniceInstance(FamilyInstance _fi)
        {
            var fs = _fi.Symbol;
            return IsSuperCorniceSymbol(fs);
        }
        public static bool IsSuperCorniceSymbol(FamilySymbol fs)
        {
            var p = fs.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.FacadeFamilyTypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.SuperCornice;
            }
        }
        /// <summary>
        /// 开槽
        /// </summary>
        public static bool IsSuperRevealElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsSuperRevealInstance(fi);
        }
        public static bool IsSuperRevealInstance(FamilyInstance _fi)
        {
            var fs = _fi.Symbol;
            return IsSuperRevealSymbol(fs);
        }
        public static bool IsSuperRevealSymbol(FamilySymbol fs)
        {
            var p = fs.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.FacadeFamilyTypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.SuperReveal
                    || type == FacadeFamilyType.SuperRevealComp;
            }
        }
        /// <summary>
        /// 竖铤阵列 开槽阵列
        /// </summary>
        public static bool IsFacadeArrayElem(Element elem)
        {
            return IsMullionArrayElem(elem)
                || IsRevealArrayElem(elem);
        }
        /// <summary>
        /// 竖铤阵列 开槽阵列
        /// </summary>
        public static bool IsFacadeArraySymbol(FamilySymbol fs)
        {
            return IsMullionArraySymbol(fs)
                || IsRevealArraySymbol(fs);
        }
        /// <summary>
        /// 竖铤阵列
        /// </summary>
        public static bool IsMullionArrayElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsMullionArraySymbol(fi.Symbol);
        }
        /// <summary>
        /// 竖铤阵列
        /// </summary>
        public static bool IsMullionArraySymbol(FamilySymbol _familySymbol)
        {
            var p = _familySymbol.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.FacadeFamilyTypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.MullionArray_fb;
            }
        }
        /// <summary>
        /// 开槽阵列
        /// </summary>
        public static bool IsRevealArrayElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsRevealArraySymbol(fi.Symbol);
        }
        public static bool IsRevealArraySymbol(FamilySymbol _familySymbol)
        {
            var p = _familySymbol.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.FacadeFamilyTypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.RevealArray_fb;
            }
        }
        public static bool IsArrayLineBasedElem(Element elem)
        {
            return IsArrayOnFaceElem(elem)
                || IsArrayOnLevelElem(elem);
        }
        /// <summary>
        /// 基于线阵列-垂直面
        /// </summary>
        public static bool IsArrayOnFaceElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsArrayOnFaceSymbol(fi.Symbol);
        }
        public static bool IsArrayOnFaceSymbol(FamilySymbol _familySymbol)
        {
            var p = _familySymbol.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.FacadeFamilyTypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.ArrayOnFace;
            }
        }
        /// <summary>
        /// 基于线阵列-垂直面
        /// </summary>
        public static bool IsArrayOnLevelElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsArrayOnLevelSymbol(fi.Symbol);
        }
        public static bool IsArrayOnLevelSymbol(FamilySymbol _familySymbol)
        {
            var p = _familySymbol.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.FacadeFamilyTypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.FacadeFamilyTypeID[key];
                return type == FacadeFamilyType.ArrayOnLevel;
            }
        }
        #endregion Facade Family

        #region other
        /// <summary>
        /// 基于面开洞
        /// </summary>
        public static bool IsVoidFaceBasedFamilyInstanceElem(Element elem)
        {
            if (elem is FamilyInstance == false)
                return false;
            var fi = elem as FamilyInstance;
            return IsVoidFaceBasedFamilySymbolElem(fi.Symbol);
        }
        public static bool IsVoidFaceBasedFamilySymbolElem(Element familySymbol)
        {
            var p = familySymbol.get_Parameter(FirmStandards.FamilyTypeIDForAPI);
            if (p == null)
                return false;
            var key = p.AsString();
            if (key == null
                || FirmStandards.OtherFamilytypeID.ContainsKey(key) == false)
                return false;
            else
            {
                var type = FirmStandards.OtherFamilytypeID[key];
                return type == OtherFamilyType.VoidFaceBased;
            }
        }
        public static bool IsLineBased(Element _elem)
        {
            return IsSuperForm(_elem)
                || IsSuperRevealElem(_elem)
                || IsArrayOnFaceElem(_elem)
                || IsArrayOnLevelElem(_elem);
        }
        public static bool IsFaceBased(Element _elem)
        {
            return IsFacadeArrayElem(_elem)
                || IsVoidFaceBasedFamilyInstanceElem(_elem);
        }
        public static bool IsWallBased(Element _elem)
        {
            return IsEnviFamilyInstanceElem(_elem);
        }
        #endregion other
    }
}
