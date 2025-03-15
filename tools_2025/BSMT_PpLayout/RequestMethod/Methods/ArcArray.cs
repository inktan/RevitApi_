using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using g3;
using goa.Common.g3InterOp;
using ClipperLib;
using goa.Common;
using PubFuncWt;
using System.Diagnostics;

namespace BSMT_PpLayout
{
    class ArcArray : RequestMethod
    {

        public ArcArray(UIApplication uiapp) : base(uiapp)
        {

        }
        /// <summary>
        /// 线性阵列
        /// </summary>
        internal override void Execute()
        {

            "弧线阵列".TaskDialogErrorMessage();

            // 选择一根弧线后 再点击一个点 确定车位出现的位置

            // 弧线限定停车区域 删除与之相碰撞的停车位 车位的三个点
           
                // 弧线向外，偏移两次，定义围合区域

                // 弧线向内，弧线外向，定义围合区域 数据模块的加减

            // 

            // this 绑定 new bind 上下文 window or undefined

            // 
            // this 

            // this 

            //

        }
    }

}
