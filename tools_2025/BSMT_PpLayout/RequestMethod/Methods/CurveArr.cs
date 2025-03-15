using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    class CurveArr : RequestMethod
    {
        public CurveArr(UIApplication uiApp) : base(uiApp) { }
        internal override void Execute()
        {

            // 拾取一根弧线

            // 采取策略：路径向两侧偏移 -- 定位停车位的曲线路径 -- 在曲线路径上，阵列停车位

            // 量出弧线最小弧段对应的长度
            //throw new NotImplementedException();



        }
    }
}
