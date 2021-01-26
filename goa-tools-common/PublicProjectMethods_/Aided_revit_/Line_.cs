using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class Line_
    {
        /// <summary>
        /// 基于容差值判断两根线是否重合
        /// </summary>
        public static bool IsCoincide(this Line line1, Line line2)
        {
            bool isCoincide = false;
            XYZ endPointc100 = line1.GetEndPoint(0);
            XYZ endPointc101 = line1.GetEndPoint(1);

            XYZ endPointc200 = line2.GetEndPoint(0);
            XYZ endPointc201 = line2.GetEndPoint(1);

            if ((line1.Distance(endPointc200) < Precision_.TheShortestDistance && line1.Distance(endPointc201) < Precision_.TheShortestDistance)
                || (line2.Distance(endPointc100) < Precision_.TheShortestDistance && line2.Distance(endPointc101) < Precision_.TheShortestDistance)) //由于clipper之后的误差为0.2mm，因此，手动创建函数 线段点到直线的距离小于1mm则属于重合的情况
            {
                isCoincide = true;
            }

            return isCoincide;
        }

        public static XYZ MiddlePoint(this Line line)
        {
            XYZ xYZ01 = line.GetEndPoint(0);
            XYZ xYZ02 = line.GetEndPoint(1);

            return (xYZ01 + xYZ02) / 2;
        }
    }
}
