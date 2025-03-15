using Autodesk.Revit.DB;
using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubFuncWt;

namespace BSMT_PpLayout
{
    abstract class PsCutClipPattern
    {
        internal string PsTypeName { get; set; }// 普通停车位，阵列时，允许为机械车位
        internal string MiniPsTypeName { get; set; }// 微型车位
        internal string HorPsTypeName { get; set; }// 平行式停车
        internal string MechanPsName { get; set; }// 机械车位

        internal PsCutClipPattern()
        {
            //提取停车位类型名字
            double w = GlobalData.Instance.pSWidth_num;
            double h = GlobalData.Instance.pSHeight_num;// 普通

            double miniW = GlobalData.Instance.miniPSWidth_num;
            double miniH = GlobalData.Instance.miniPSHeight_num;// 微型

            double psW_Hor = GlobalData.Instance.pSWidth_Hor_num;
            double psH_Hor = GlobalData.Instance.pSHeight_Hor_num;// 平行式停车

            this.PsTypeName = "停车位_" + w.FeetToMilliMeter().ToString() + "*" + h.FeetToMilliMeter().ToString();// 普通停车位
            this.MiniPsTypeName = "微型车位_" + miniW.FeetToMilliMeter().ToString() + "*" + miniH.FeetToMilliMeter().ToString();// 微型车位
            this.HorPsTypeName = "平行停车位_" + psW_Hor.FeetToMilliMeter().ToString() + "*" + psH_Hor.FeetToMilliMeter().ToString();// // 平行式停车
            this.MechanPsName = "机械车位_" + w.FeetToMilliMeter().ToString() + "*" + h.FeetToMilliMeter().ToString();// 机械车位
        }
        internal abstract List<PPLocPoint> Sweep(Polygon2d poly2d, Polygon2d bsmtPoly2d, List<Polygon2d> polygon2ds);
    }
}
