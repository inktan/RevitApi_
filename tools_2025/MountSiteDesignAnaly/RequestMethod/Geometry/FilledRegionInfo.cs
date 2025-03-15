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
    class FilledRegionInfo : RevitEleInfo
    {
        internal FilledRegion FilledRegion;

        /// <summary>
        /// 上部楼板，待输入覆土厚度
        /// </summary>
        /// <param name="_floor"></param>
        internal FilledRegionInfo(Element _FilledRegion) : base(_FilledRegion)
        {
            this.FilledRegion = _FilledRegion as FilledRegion;
        }
    }
}
