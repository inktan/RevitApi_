using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Selection_
    {
        /// <summary>
        /// 两点划线
        /// </summary>
        /// <param name="sel"></param>
        /// <returns></returns>
        public static Line GetLine(this Selection sel)
        {
            //创建与轴网相交的线
            XYZ startPoint = sel.PickPoint("请选择轴网标号的起点");
            XYZ endPoint = sel.PickPoint("请选择轴网标号的终点");
            XYZ startPoint_base = new XYZ(startPoint.X, startPoint.Y, 0.0);
            XYZ endPoint_base = new XYZ(endPoint.X, endPoint.Y, 0.0);
            Line baseCurve = Line.CreateBound(startPoint_base, endPoint_base) as Line;//创建两点连线
            return baseCurve;
        }
    }
}
