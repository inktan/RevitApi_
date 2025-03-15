using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using goa.Common;
using goa.Common.g3InterOp;
using Octree;

namespace MountSiteDesignAnaly
{
    class BaseFaceCoil : SamplingCoil
    {
        internal BaseFaceCoil(BaseFace _face) : base(_face)
        {
            this.curves = this.GetCurves();
        }
        /// <summary>
        /// 楼板不可以是环状
        /// </summary>
        /// <returns></returns>
        internal override List<Curve> GetCurves()
        {
            return this.face.baseFace.GetEdgeCurves();
        }

    }
}
