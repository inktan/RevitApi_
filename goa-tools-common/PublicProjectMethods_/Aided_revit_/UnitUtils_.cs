using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class UnitUtils_
    {
        #region UnitConvertion 单位替换
        /// <summary>
        /// 毫米转换为英尺
        /// </summary>
        /// <returns></returns>
        public static double MilliMeterToFeet(this double _mm)
        {
            return UnitUtils.ConvertToInternalUnits(_mm, DisplayUnitType.DUT_MILLIMETERS);
        }
        /// <summary>
        /// 英尺转换为毫米
        /// </summary>
        /// <returns></returns>
        public static double FeetToMilliMeter(this double _feet)
        {
            return UnitUtils.ConvertFromInternalUnits(_feet, DisplayUnitType.DUT_MILLIMETERS);
        }
        /// <summary>
        /// 平方英尺转为平方米
        /// </summary>
        /// <returns></returns>
        public static double SQUARE_FEETtoSQUARE_METERS(this double _area)
        {
            return UnitUtils.Convert(_area, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);
        }
        /// <summary>
        /// 平方米转为平方英尺
        /// </summary>
        /// <returns></returns>
        public static double SQUARE_METERStoSQUARE_FEET(this double _area)
        {
            return UnitUtils.Convert(_area, DisplayUnitType.DUT_SQUARE_METERS, DisplayUnitType.DUT_SQUARE_FEET);
        }
        #endregion

    }
}
