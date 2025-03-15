using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using goa.Common;

namespace PubFuncWt
{
    public static class Solid_
    {
        /// <summary>
        /// 假定均垂直拉伸体量
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static double GetHeight(this Solid solid)
        {
            PlanarFace planarFace = solid.Faces.Cast<Face>().Where(p => p is PlanarFace).Cast<PlanarFace>().Where(p => p.IsFaceFacingUp()).FirstOrDefault();
            if (planarFace == null)
            {
                return 0;
            }
            else
            {
                return solid.Volume / planarFace.Area;
            }
        }
    }
}
