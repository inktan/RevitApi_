using Autodesk.Revit.DB;
using g3;
using goa.Common.g3InterOp;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeighaNet;
using PubFuncWt;

namespace InfoStrucFormwork
{
    /// <summary>
    /// 处理直线钢梁
    /// </summary>
    class SteelBeamInfo
    {
        /// <summary>
        /// 创建的梁
        /// </summary>
        public FamilyInstance FamilyInstance { get; internal set; }
        /// <summary>
        /// 钢梁 全段 基准线
        /// </summary>
        public LineInfo BaseLineInfo { get; set; }
        public ArcInfo ArcInfo { get; set; }
        public PolyLineInfo PolyLineInfo { get; set; }
        public TextInfo TextInfo { get; internal set; }
        public Polygon2d Polygon2d { get; internal set; }

        public Curve Curve { get; set; }

        
    }
}
