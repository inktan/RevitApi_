using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class UnitUtils_
    {
        #region UnitConvertion 单位替换

        public static double FeetToMilliMeter(this double Feet)
        {
            return Feet * 304.8;
        }
        public static double MilliMeterToFeet(this double millimeters)
        {
            // 手动计算转换为英尺
            return millimeters / 304.8;
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
        /// <summary>
        /// 立方英尺转为立方米
        /// </summary>
        /// <returns></returns>
        public static double CUBIC_FEETtoCUBIC_METERS(this double _area)
        {
            return UnitUtils.Convert(_area, DisplayUnitType.DUT_CUBIC_FEET, DisplayUnitType.DUT_CUBIC_METERS);
        }
        /// <summary>
        /// 立方米转为立方英尺
        /// </summary>
        /// <returns></returns>
        public static double CUBIC_METERStoCUBIC_FEET(this double _area)
        {
            return UnitUtils.Convert(_area, DisplayUnitType.DUT_CUBIC_METERS, DisplayUnitType.DUT_CUBIC_FEET);
        }
        #endregion

    }
}
