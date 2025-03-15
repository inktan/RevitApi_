using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class PickedBox_
    {
        /// <summary>
        /// BondingBox的底面线圈的四个角点
        /// </summary>
        public static IEnumerable<XYZ> BottomXyzs(this PickedBox pickedBox)
        {
            if (pickedBox == null) return new List<XYZ>();

            XYZ min = pickedBox.Min;
            XYZ max = pickedBox.Max;

            XYZ _rd = new XYZ(max.X, min.Y, 0);
            XYZ _lu = new XYZ(min.X, max.Y, 0);

            return new List<XYZ>() { min, _rd, max, _lu };
        }
    }
}
