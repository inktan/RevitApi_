using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace TagTools
{
    internal static class AppSetting
    {
        internal static double Spacing = UnitUtils.ConvertToInternalUnits(200, DisplayUnitType.DUT_MILLIMETERS);
    }
}
